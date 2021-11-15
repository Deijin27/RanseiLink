using RanseiLink.Core.Randomization;
using RanseiLink.Core.Services;
using System.Windows;

namespace RanseiLink.Services;

public interface IDialogService
{
    bool CreateMod(out ModInfo modInfo, out string romPath);
    bool CreateModBasedOn(ModInfo baseMod, out ModInfo newModInfo);
    bool ExportMod(ModInfo info, out string folder);
    bool ImportMod(out string file);
    bool EditModInfo(ModInfo info);
    bool CommitToRom(ModInfo info, out string romPath);
    bool RequestRomFile(out string result);
    bool ConfirmDelete(ModInfo info);
    MessageBoxResult ShowMessageBox(MessageBoxArgs options);
    bool Randomize(IRandomizer randomizer);
    bool RequestFolder(out string result);
    bool RequestModFile(out string result);
}
