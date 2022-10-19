using System;

namespace RanseiLink.Core.Services
{
    [Flags]
    public enum FilePatchOptions
    {
        None = 0,
        DeleteSourceWhenDone = 1,
        VariableLength = 2,
    }

    public class FileToPatch
    {
        public string GamePath { get; }
        public string FileSystemPath { get; }
        public FilePatchOptions Options { get; }
        public FileToPatch(string gamePath, string fileSystemPath, FilePatchOptions options)
        {
            GamePath = gamePath;
            FileSystemPath = fileSystemPath;
            Options = options;
        }
    }

    public enum CannotPatchCategory
    {
        RomFileDoesntExist,
        GraphicsDefaultsNotPopulated,
        ModGameCodeNotCompatibleWithRom,
        RomGameCodeNotValid,

        Default = -1
    }

    public class CanPatchResult
    {
        public CanPatchResult(bool canPatch, CannotPatchCategory cannotPatchCategory, string reasonCannotPatch)
        {
            CanPatch = canPatch;
            CannotPatchCategory = cannotPatchCategory;
            ReasonCannotPatch = reasonCannotPatch;
        }

        public CanPatchResult(bool canPatch)
        {
            CanPatch = canPatch;
        }

        public bool CanPatch { get; }
        public CannotPatchCategory CannotPatchCategory { get; }
        public string ReasonCannotPatch { get; }

    }

    public interface IModPatchingService
    {
        CanPatchResult CanPatch(ModInfo modInfo, string romPath, PatchOptions patchOptions);
        void Patch(ModInfo modInfo, string romPath, PatchOptions patchOptions, IProgress<ProgressInfo> progress = null);
    }
}