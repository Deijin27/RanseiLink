
using System;
using System.Threading.Tasks;

namespace RanseiLink.Core.Services;

public interface IDialogService
{
    bool UpgradeMods(out string romPath);
    bool CreateMod(out ModInfo modInfo, out string romPath);
    bool CreateModBasedOn(ModInfo baseMod, out ModInfo newModInfo);
    bool ExportMod(ModInfo info, out string folder);
    bool ImportMod(out string file);
    bool EditModInfo(ModInfo info);
    bool CommitToRom(ModInfo info, out string romPath);
    bool RequestRomFile(out string result);
    bool ConfirmDelete(ModInfo info);
    MessageBoxResult ShowMessageBox(MessageBoxArgs options);
    bool RequestFolder(out string result);
    bool RequestModFile(out string result);
    void ProgressDialog(Func<IProgress<string>, IProgress<int>, Task> workAsync, string title = null);
    bool ModifyMapDimensions(ref ushort width, ref ushort height);
}
