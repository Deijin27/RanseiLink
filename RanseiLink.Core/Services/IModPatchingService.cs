#nullable enable
using FluentResults;
using System;

namespace RanseiLink.Core.Services;

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

public interface IModPatchingService
{
    Result CanPatch(ModInfo modInfo, string romPath, PatchOptions patchOptions);
    void Patch(ModInfo modInfo, string romPath, PatchOptions patchOptions, IProgress<ProgressInfo>? progress = null);
}