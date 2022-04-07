//#define PATCHER_BUG_FIXING
using RanseiLink.Core.RomFs;
using RanseiLink.Core.Services.ModPatchBuilders;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Core.Services.Concrete;


[Flags]
public enum FilePatchOptions
{
    None = 0,
    DeleteSourceWhenDone = 1,
    VariableLength = 2,
}

public record FileToPatch(string GamePath, string FileSystemPath, FilePatchOptions Options);


public class ModPatchingService : IModPatchingService
{

    private readonly RomFsFactory _ndsFactory;
    private readonly IFallbackSpriteProvider _fallbackSpriteProvider;
    private readonly IPatchBuilder[] _builders;

    public ModPatchingService(RomFsFactory ndsFactory, IFallbackSpriteProvider fallbackSpriteProvider, IPatchBuilder[] builders)
    {
        _fallbackSpriteProvider = fallbackSpriteProvider;
        _ndsFactory = ndsFactory;
        _builders = builders;
    }

    public bool CanPatch(ModInfo modInfo, string romPath, PatchOptions patchOptions, out string reasonCannotPatch)
    {
        if (patchOptions.HasFlag(PatchOptions.IncludeSprites))
        {
            if (!_fallbackSpriteProvider.IsDefaultsPopulated)
            {
                reasonCannotPatch = "Cannot patch sprites unless 'Populate Graphics Defaults' has been run";
                return false;
            }
        }

        reasonCannotPatch = "";
        return true;
    }

    public void Patch(ModInfo modInfo, string romPath, PatchOptions patchOptions, IProgress<ProgressInfo> progress = null)
    {
        progress?.Report(new ProgressInfo(StatusText: "Preparing to patch...", IsIndeterminate: true));

        ConcurrentBag<FileToPatch> filesToPatch = new();
        Exception exception = null;
        try
        {
            GetFilesToPatch(filesToPatch, patchOptions);

#if PATCHER_BUG_FIXING
            string debugOut = FileUtil.MakeUniquePath(Path.Combine(FileUtil.DesktopDirectory, "patch_debug_dump"));
            Directory.CreateDirectory(debugOut);
            foreach (var file in filesToPatch)
            {
                string dest = Path.Combine(debugOut, file.GamePath.Replace(Path.DirectorySeparatorChar, '~'));
                if (file.Options.HasFlag(FilePatchOptions.DeleteSourceWhenDone))
                {
                    File.Move(file.FileSystemPath, dest);
                }
                else
                {
                    File.Copy(file.FileSystemPath, dest);
                }
                
            }
            return;
#endif

            progress?.Report(new ProgressInfo(IsIndeterminate: false, MaxProgress: filesToPatch.Count, StatusText: "Patching..."));
            int count = 0;
            using var nds = _ndsFactory(romPath);
            foreach (var file in filesToPatch)
            {
                if (file.Options.HasFlag(FilePatchOptions.VariableLength))
                {
                    nds.InsertVariableLengthFile(file.GamePath, file.FileSystemPath);
                }
                else
                {
                    nds.InsertFixedLengthFile(file.GamePath, file.FileSystemPath);
                }
                progress?.Report(new ProgressInfo(Progress: ++count));
            }
        }
        catch (Exception e)
        {
            exception = e;
        }
        finally
        {
            progress?.Report(new ProgressInfo(StatusText: "Cleaning up temporary files...", IsIndeterminate: true));

            foreach (var file in filesToPatch)
            {
                if (file.Options.HasFlag(FilePatchOptions.DeleteSourceWhenDone))
                {
                    File.Delete(file.FileSystemPath);
                }
            }
        }

        if (exception != null)
        {
            throw exception;
        }

        progress?.Report(new ProgressInfo(StatusText: "Done!", IsIndeterminate: false));
    }

    private void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, PatchOptions patchOptions)
    {
        Parallel.ForEach(_builders, builder =>
        {
            builder.GetFilesToPatch(filesToPatch, patchOptions);
        });
    }
}
