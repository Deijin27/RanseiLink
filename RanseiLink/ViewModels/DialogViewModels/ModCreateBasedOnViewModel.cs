using RanseiLink.Core.Services;

namespace RanseiLink.ViewModels;

public class ModCreateBasedOnViewModel : ViewModelBase, IModalDialogViewModel<bool>
{
    public ModCreateBasedOnViewModel(ModInfo baseMod)
    {
        ModInfo = new ModInfo() { Name = baseMod.Name, Version = baseMod.Version, Author = baseMod.Author };
    }

    public bool Result { get; private set; }

    public void OnClosing(bool result)
    {
        Result = result;
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
