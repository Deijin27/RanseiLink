//#define PATCHER_BUG_FIXING
using RanseiLink.Core.Enums;
using RanseiLink.Core.RomFs;
using RanseiLink.Core.Services.ModPatchBuilders;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Core.Services.Concrete
{
    public class ModPatchingService : IModPatchingService
    {

        private readonly RomFsFactory _ndsFactory;
        private readonly IFallbackDataProvider _fallbackSpriteProvider;
        private readonly IModServiceGetterFactory _modServiceGetterFactory;

        public ModPatchingService(RomFsFactory ndsFactory, IFallbackDataProvider fallbackSpriteProvider, IModServiceGetterFactory modServiceGetterFactory)
        {
            _fallbackSpriteProvider = fallbackSpriteProvider;
            _ndsFactory = ndsFactory;
            _modServiceGetterFactory = modServiceGetterFactory;
        }

        public CanPatchResult CanPatch(ModInfo modInfo, string romPath, PatchOptions patchOptions)
        {
            if (!File.Exists(romPath))
            {
                return new CanPatchResult(false, CannotPatchCategory.RomFileDoesntExist, $"Rom file '{romPath}' does not exist.");
            }

            using (var br = new BinaryReader(File.OpenRead(romPath)))
            {
                var header = new NdsHeader(br);
                if (!Enum.TryParse(header.GameCode, out ConquestGameCode romGameCode))
                {
                    return new CanPatchResult(false, CannotPatchCategory.RomGameCodeNotValid, $"Unexpected game code '{header.GameCode}', this may not be a conquest rom, or it may be a culture we don't know of yet");
                }
                if (modInfo.GameCode.IsCompatibleWith(romGameCode))
                {
                    return new CanPatchResult(false, CannotPatchCategory.ModGameCodeNotCompatibleWithRom, $"Game code of mod '{modInfo.GameCode}' is not compatible with game code of rom '{romGameCode}'");
                }
            }

            if (patchOptions.HasFlag(PatchOptions.IncludeSprites))
            {
                if (!_fallbackSpriteProvider.IsDefaultsPopulated(modInfo.GameCode))
                {
                    return new CanPatchResult(false, CannotPatchCategory.GraphicsDefaultsNotPopulated, "Cannot patch sprites unless 'Populate Graphics Defaults' has been run");
                }
            }

            return new CanPatchResult(true);
        }

        public void Patch(ModInfo modInfo, string romPath, PatchOptions patchOptions, IProgress<ProgressInfo> progress = null)
        {
            progress?.Report(new ProgressInfo(statusText: "Preparing to patch...", isIndeterminate: true));

            ConcurrentBag<FileToPatch> filesToPatch = new ConcurrentBag<FileToPatch>();
            Exception exception = null;
            try
            {
                using (var services = _modServiceGetterFactory.Create(modInfo))
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

                    progress?.Report(new ProgressInfo(isIndeterminate: false, maxProgress: filesToPatch.Count + 1, statusText: "Patching..."));
                    int count = 0;

                    using (var nds = _ndsFactory(romPath))
                    {
                        PatchBanner(nds, modInfo);
                        progress?.Report(new ProgressInfo(progress: ++count));

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
                            progress?.Report(new ProgressInfo(progress: ++count));
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
                progress?.Report(new ProgressInfo(statusText: "Cleaning up temporary files...", isIndeterminate: true));

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

            progress?.Report(new ProgressInfo(statusText: "Done!", isIndeterminate: false));
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
                if (!banner.TryLoadInfoFromXml(bannerInfoFile, out _))
                {
                    return;
                }
            }
            if (File.Exists(bannerImageFile))
            {
                if (!banner.TryLoadImageFromPng(bannerImageFile, out _))
                {
                    return;
                }
            }

            romFs.SetBanner(banner);
        }
    }
}