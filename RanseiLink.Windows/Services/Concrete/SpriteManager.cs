using RanseiLink.Core.Resources;
using RanseiLink.Core.Services;
using RanseiLink.Windows.ViewModels;
using System.IO;

namespace RanseiLink.Windows.Services.Concrete;
public class SpriteManager : ISpriteManager
{
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly IDialogService _dialogService;
    private readonly ICachedSpriteProvider _cachedSpriteProvider;
    public SpriteManager(IOverrideDataProvider overrideSpriteProvider, IDialogService dialogService, ICachedSpriteProvider cachedSpriteProvider)
    {
        _spriteProvider = overrideSpriteProvider;
        _dialogService = dialogService;
        _cachedSpriteProvider = cachedSpriteProvider;
    }

    public bool SetOverride(SpriteType type, int id, string requestFileMsg)
    {
        var file = _dialogService.ShowOpenSingleFileDialog(new OpenFileDialogSettings
        {
            Title = requestFileMsg,
            Filters = new()
            {
                new()
                {
                    Name = "PNG Image (.png)",
                    Extensions = new() { ".png" }
                }
            }
        });
        if (string.IsNullOrEmpty(file))
        {
            return false;
        }

        string temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".png");

        IGraphicsInfo gInfo = GraphicsInfoResource.Get(type);

        if (gInfo is IGroupedGraphicsInfo groupedGInfo && groupedGInfo.Width != null && groupedGInfo.Height != null)
        {
            int width = (int)groupedGInfo.Width;
            int height = (int)groupedGInfo.Height;

            if (!Core.Graphics.ImageSimplifier.ImageMatchesSize(file, width, height))
            {
                if (groupedGInfo.StrictHeight || groupedGInfo.StrictWidth)
                {
                    _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                        "Invalid dimensions",
                        $"The dimensions of this image should be {groupedGInfo.Width}x{groupedGInfo.Height}.\nFor this image type it is a strict requirement."
                        ));
                    return false;
                }

                var result = _dialogService.ShowMessageBox(new MessageBoxSettings(
                    title: "Invalid dimensions",
                    message: $"The dimensions of this image should be {groupedGInfo.Width}x{groupedGInfo.Height}.\nIf will work if they are different, but may look weird in game.",
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


        int paletteCapacity = gInfo.GetPaletteCapacity(id);

        if (Core.Graphics.ImageSimplifier.SimplifyPalette(file, paletteCapacity, temp))
        {
            var vm = new SimplifyPaletteViewModel(paletteCapacity, file, temp);
            if (!_dialogService.ShowDialogWithResult(vm))
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
