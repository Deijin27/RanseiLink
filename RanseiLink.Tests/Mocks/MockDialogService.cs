using RanseiLink.Core.Services;
using RanseiLink.Services;
using System;
using System.Threading.Tasks;

namespace RanseiLink.Tests.Mocks;

public class MockDialogService : IDialogService
{

    public bool CreateMod(out ModInfo modInfo, out string romPath)
    {
        throw new NotImplementedException();
    }

    public bool CreateModBasedOn(ModInfo baseMod, out ModInfo newModInfo)
    {
        throw new NotImplementedException();
    }

    public bool ExportMod(ModInfo info, out string folder)
    {
        throw new NotImplementedException();
    }

    public bool ImportMod(out string file)
    {
        throw new NotImplementedException();
    }

    public bool EditModInfo(ModInfo info)
    {
        throw new NotImplementedException();
    }

    public bool CommitToRom(ModInfo info, out string romPath)
    {
        throw new NotImplementedException();
    }

    public bool RequestRomFile(out string result)
    {
        throw new NotImplementedException();
    }

    public bool ConfirmDelete(ModInfo info)
    {
        throw new NotImplementedException();
    }

    public bool RequestFolder(out string result)
    {
        throw new NotImplementedException();
    }

    public bool RequestModFile(out string result)
    {
        throw new NotImplementedException();
    }

    public int ShowMessageBoxCallCount { get; set; } = 0;
    public MessageBoxResult ShowMessageBoxResult { get; set; } = MessageBoxResult.Ok;
    public MessageBoxResult ShowMessageBox(MessageBoxArgs options)
    {
        ShowMessageBoxCallCount++;
        return ShowMessageBoxResult;
    }

    public void ProgressDialog(Func<IProgress<string>, IProgress<int>, Task> workAsync, string title = null)
    {
        throw new NotImplementedException();
    }

    public bool UpgradeMods(out string romPath)
    {
        throw new NotImplementedException();
    }

    public bool ModifyMapDimensions(ref ushort width, ref ushort height)
    {
        throw new NotImplementedException();
    }

    public bool RequestFile(string title, string defaultExt, string filter, out string result)
    {
        throw new NotImplementedException();
    }

    public bool CommitToRom(ModInfo info, out string romPath, out PatchOptions patchOptions)
    {
        throw new NotImplementedException();
    }

    public void ProgressDialog(Action<IProgress<ProgressInfo>> work, bool delayOnCompletion = true)
    {
        throw new NotImplementedException();
    }

    public bool PopulateDefaultSprites(out string romPath)
    {
        throw new NotImplementedException();
    }

    public bool SimplfyPalette(int maxColors, string original, string simplified)
    {
        throw new NotImplementedException();
    }

    public bool RequestFolder(string title, out string result)
    {
        throw new NotImplementedException();
    }
}
