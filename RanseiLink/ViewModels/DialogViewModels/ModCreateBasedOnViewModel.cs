using RanseiLink.Core.Services;

namespace RanseiLink.ViewModels
{
    public class ModCreateBasedOnViewModel : ViewModelBase
    {
        public ModCreateBasedOnViewModel(ModInfo baseMod)
        {
            BaseMod = baseMod;
            ModInfo = new ModInfo() { Name = baseMod.Name, Version = baseMod.Version, Author = baseMod.Author };
        }

        public ModInfo BaseMod { get; }
        public ModInfo ModInfo { get; }

        public string Name
        {
            get => ModInfo.Name;
            set => RaiseAndSetIfChanged(ModInfo.Name, value, v => ModInfo.Name = v);
        }

        public string Author
        {
            get => ModInfo.Author;
            set => RaiseAndSetIfChanged(ModInfo.Author, value, v => ModInfo.Author = v);
        }

        public string Version
        {
            get => ModInfo.Version;
            set => RaiseAndSetIfChanged(ModInfo.Version, value, v => ModInfo.Version = v);
        }
    }
}
