using RanseiLink.Core.Services;

namespace RanseiLink.ViewModels;

public class ModEditInfoViewModel : ViewModelBase
{
    public ModEditInfoViewModel(ModInfo info)
    {
        ModInfo = info;
    }

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
