//#define PATCHER_BUG_FIXING
using FluentResults;
using RanseiLink.Core.Enums;
using RanseiLink.Core.RomFs;
using RanseiLink.Core.Services.ModPatchBuilders;
using System.Collections.Concurrent;

namespace RanseiLink.Core.Services.Concrete;

public class ModPatchingService(RomFsFactory ndsFactory, IFallbackDataProvider fallbackSpriteProvider, IModServiceGetterFactory modServiceGetterFactory) : IModPatchingService
{
    public Result CanPatch(ModInfo modInfo, string romPath, PatchOptions patchOptions)
    {
        if (!File.Exists(romPath))
        {
            return Result.Fail($"Rom file '{romPath}' does not exist.");
        }

        using (var br = new BinaryReader(File.OpenRead(romPath)))
        {
            var header = new NdsHeader(br);
            if (!Enum.TryParse(header.GameCode, out ConquestGameCode romGameCode))
            {
                return Result.Fail($"Unexpected game code '{header.GameCode}', this may not be a conquest rom, or it may be a culture we don't know of yet");
            }
            if (!modInfo.GameCode.IsCompatibleWith(romGameCode))
            {
                return Result.Fail($"Game code of mod '{modInfo.GameCode}' ({modInfo.GameCode.UserFriendlyName()}) is not compatible with game code of rom '{romGameCode}' ({modInfo.GameCode.UserFriendlyName()})");
            }
        }

        if (patchOptions.HasFlag(PatchOptions.IncludeSprites))
        {
            if (!fallbackSpriteProvider.IsDefaultsPopulated(modInfo.GameCode))
            {
                return Result.Fail("Cannot patch sprites unless 'Populate Graphics Defaults' has been run");
            }
        }

        return Result.Ok();
    }

    public void Patch(ModInfo modInfo, string romPath, PatchOptions patchOptions, IProgress<ProgressInfo>? progress = null)
    {
        progress?.Report(new ProgressInfo(StatusText: "Preparing to patch...", IsIndeterminate: true));

        ConcurrentBag<FileToPatch> filesToPatch = new ConcurrentBag<FileToPatch>();
        Exception? exception = null;
        try
        {
            using (var services = modServiceGetterFactory.Create(modInfo))
            {
                var patchers = services.Get<IEnumerable<IPatchBuilder>>();
                GetFilesToPatch(patchers, filesToPatch, patchOptions);

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

                progress?.Report(new ProgressInfo(IsIndeterminate: false, MaxProgress: filesToPatch.Count + 1, StatusText: "Patching..."));
                int count = 0;

                using (var nds = ndsFactory(romPath))
                {
                    PatchBanner(nds, modInfo);
                    progress?.Report(new ProgressInfo(Progress: ++count));

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

    private void GetFilesToPatch(IEnumerable<IPatchBuilder> patchBuilders, ConcurrentBag<FileToPatch> filesToPatch, PatchOptions patchOptions)
    {
        Parallel.ForEach(patchBuilders, builder =>
        {
            builder.GetFilesToPatch(filesToPatch, patchOptions);
        });
    }

    /// <summary>
    /// Banner is special case because it's not in the file system
    /// </summary>
    private void PatchBanner(IRomFs romFs, ModInfo modInfo)
    {
        string bannerInfoFile = Path.Combine(modInfo.FolderPath, Constants.BannerInfoFile);
        string bannerImageFile = Path.Combine(modInfo.FolderPath, Constants.BannerImageFile);
        if (!File.Exists(bannerInfoFile) && !File.Exists(bannerImageFile))
        {
            return;
        }

        var banner = romFs.GetBanner();
        
        if (File.Exists(bannerInfoFile))
        {
            if (banner.TryLoadInfoFromXml(bannerInfoFile).IsFailed)
            {
                return;
            }
        }
        if (File.Exists(bannerImageFile))
        {
            if (banner.TryLoadImageFromPng(bannerImageFile).IsFailed)
            {
                return;
            }
        }

        romFs.SetBanner(banner);
    }
}