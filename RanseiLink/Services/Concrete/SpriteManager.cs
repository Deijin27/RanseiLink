using RanseiLink.Core.Resources;
using RanseiLink.Core.Services;
using System.IO;

namespace RanseiLink.Services.Concrete;
public class SpriteManager : ISpriteManager
{
    private readonly IOverrideSpriteProvider _spriteProvider;
    private readonly IDialogService _dialogService;

    public SpriteManager(IOverrideSpriteProvider overrideSpriteProvider, IDialogService dialogService)
    {
        _spriteProvider = overrideSpriteProvider;
        _dialogService = dialogService;
    }

    public bool SetOverride(SpriteType type, int id, string requestFileMsg)
    {
        if (!_dialogService.RequestFile(requestFileMsg, ".png", "PNG Image (.png)|*.png", out string file))
        {
            return false;
        }

        string temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".png");

        IGraphicsInfo gInfo = GraphicsInfoResource.Get(type);

        if (gInfo.Width != null && gInfo.Height != null)
        {
            int width = (int)gInfo.Width;
            int height = (int)gInfo.Height;

            if (!Core.Graphics.ImageSimplifier.ImageMatchesSize(file, width, height))
            {
                if (gInfo.StrictHeight || gInfo.StrictWidth)
                {
                    _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                        "Invalid dimensions",
                        $"The dimensions of this image should be {gInfo.Width}x{gInfo.Height}.\nFor this image type it is a strict requirement."
                        ));
                    return false;
                }

                var result = _dialogService.ShowMessageBox(new MessageBoxArgs(
                    title: "Invalid dimensions",
                    message: $"The dimensions of this image should be {gInfo.Width}x{gInfo.Height}.\nIf will work if they are different, but may look weird in game.",
                    buttons: new[]
                    {
                        new MessageBoxButton("Proceed anyway", MessageBoxResult.No),
                        new MessageBoxButton("Auto Resize", MessageBoxResult.Yes),
                        new MessageBoxButton("Cancel", MessageBoxResult.Cancel),
                    },
                    defaultResult: MessageBoxResult.Cancel
                    ));

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        Core.Graphics.ImageSimplifier.ResizeImage(file, width, height, temp);
                        file = temp;
                        break;
                    case MessageBoxResult.No:
                        break;
                    default:
                        return false;
                }
            }
        }


        int paletteCapacity;
        if (gInfo is MiscConstants miscInfo)
        {
            paletteCapacity = miscInfo.Items[id].PaletteCapacity;
        }
        else
        {
            paletteCapacity = gInfo.PaletteCapacity;
        }

        if (Core.Graphics.ImageSimplifier.SimplifyPalette(file, paletteCapacity, temp))
        {
            if (!_dialogService.SimplfyPalette(paletteCapacity, file, temp))
            {
                return false;
            }
            file = temp;
        }

        _spriteProvider.SetOverride(type, id, file);

        if (File.Exists(temp))
        {
            File.Delete(temp);
        }

        return true;
    }
}
