using System;

namespace RanseiLink.Core.Services;

public interface IModPatchingService
{
    bool CanPatch(ModInfo modInfo, string romPath, PatchOptions patchOptions, out string reasonCannotPatch);
    void Patch(ModInfo modInfo, string romPath, PatchOptions patchOptions, IProgress<ProgressInfo> progress = null);
}