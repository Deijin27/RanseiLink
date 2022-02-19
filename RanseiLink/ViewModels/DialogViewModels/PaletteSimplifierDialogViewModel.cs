
namespace RanseiLink.ViewModels;

public class PaletteSimplifierDialogViewModel
{
    public PaletteSimplifierDialogViewModel(int maxColors, string original, string simplified)
    {
        MaximumColors = maxColors;
        Original = original;
        Simplified = simplified;
    }
    public int MaximumColors { get; }
    public string Original { get; }
    public string Simplified { get; }
}
