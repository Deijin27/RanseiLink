
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RanseiLink.ViewModels;

public class PaletteSimplifierDialogViewModel
{
    public PaletteSimplifierDialogViewModel(int maxColors, string original, string simplified)
    {
        using (var originalFs = new FileStream(original, FileMode.Open, FileAccess.Read))
        {
            Original = BitmapFrame.Create(originalFs, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        }
        using (var simplifiedFs = new FileStream(original, FileMode.Open, FileAccess.Read))
        {
            Simplified = BitmapFrame.Create(simplifiedFs, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        }

    }
    public int MaximumColors { get; }
    public ImageSource Original { get; }
    public ImageSource Simplified { get; }
}
