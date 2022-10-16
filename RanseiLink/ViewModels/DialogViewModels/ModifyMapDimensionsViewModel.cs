using RanseiLink.Core.Services;

namespace RanseiLink.ViewModels;

public class ModifyMapDimensionsViewModel : IModalDialogViewModel<bool>
{
    public ModifyMapDimensionsViewModel(ushort width, ushort height)
    {
        Width = width;
        Height = height;
    }

    public ushort Width { get; }
    public ushort Height { get; }

    public bool Result { get; private set; }
    public void OnClosing(bool result)
    {
        Result = result;
    }
}
