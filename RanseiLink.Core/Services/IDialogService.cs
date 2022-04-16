
using System;

namespace RanseiLink.Core.Services
{
    public interface IDialogService
    {
        bool UpgradeMods(out string romPath);
        bool CreateMod(out ModInfo modInfo, out string romPath);
        bool CreateModBasedOn(ModInfo baseMod, out ModInfo newModInfo);
        bool ExportMod(ModInfo info, out string folder);
        bool ImportMod(out string file);
        bool EditModInfo(ModInfo info);
        bool CommitToRom(ModInfo info, out string romPath, out PatchOptions patchOptions);
        bool RequestRomFile(out string result);
        bool ConfirmDelete(ModInfo info);
        MessageBoxResult ShowMessageBox(MessageBoxArgs options);
        bool RequestFolder(string title, out string result);
        bool RequestModFile(out string result);
        void ProgressDialog(Action<IProgress<ProgressInfo>> work, bool delayOnCompletion = true);
        bool ModifyMapDimensions(ref ushort width, ref ushort height);
        bool RequestFile(string title, string defaultExt, string filter, out string result);
        bool PopulateDefaultSprites(out string romPath);
        bool SimplfyPalette(int maxColors, string original, string simplified);
    }

    public class ProgressInfo
    {
        public string StatusText { get; }
        public int? Progress { get; }
        public int? MaxProgress { get; }
        public bool? IsIndeterminate { get; }
        public ProgressInfo(string statusText = null, int? progress = null, int? maxProgress = null, bool? isIndeterminate = null)
        {
            StatusText = statusText;
            Progress = progress;
            MaxProgress = maxProgress;
            IsIndeterminate = isIndeterminate;
        }
    }

}
