namespace RanseiLink.GuiCore.ViewModels;

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
        set => SetProperty(ref _width, value);
    }
    public int Height
    {
        get => _height;
        set => SetProperty(ref _height, value);
    }

    public bool Result { get; private set; }
    public void OnClosing(bool result)
    {
        Result = result;
    }
}
