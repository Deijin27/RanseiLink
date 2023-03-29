using RanseiLink.Core.Services;

namespace RanseiLink.Windows.ViewModels;

public class ModifyMapDimensionsViewModel : ViewModelBase, IModalDialogViewModel<bool>
{
    public ModifyMapDimensionsViewModel(ushort width, ushort height)
    {
        Width = width;
        Height = height;
    }

    private int _width;
    private int _height;
    public int Width
    {
        get => _width;
        set => RaiseAndSetIfChanged(ref _width, value);
    }
    public int Height
    {
        get => _height;
        set => RaiseAndSetIfChanged(ref _height, value);
    }

    public bool Result { get; private set; }
    public void OnClosing(bool result)
    {
        Result = result;
    }
}
