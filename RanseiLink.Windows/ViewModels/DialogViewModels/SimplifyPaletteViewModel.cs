namespace RanseiLink.Windows.ViewModels;

public class SimplifyPaletteViewModel : IModalDialogViewModel<bool>
{
    public SimplifyPaletteViewModel(int maxColors, string original, string simplified)
    {
        MaximumColors = maxColors;
        Original = original;
        Simplified = simplified;
    }
    public int MaximumColors { get; }
    public string Original { get; }
    public string Simplified { get; }

    public bool Result { get; private set; }
    public void OnClosing(bool result)
    {
        Result = result;
    }
}
