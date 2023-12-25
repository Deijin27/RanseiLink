#nullable enable
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services;

namespace RanseiLink.Windows.Services.Concrete;
public class SpriteManager(IOverrideDataProvider overrideSpriteProvider, IDialogService dialogService) : ISpriteManager
{
    public bool SetOverride(SpriteType type, int id, string requestFileMsg)
    {
        var file = dialogService.ShowOpenSingleFileDialog(new OpenFileDialogSettings
        {
            Title = requestFileMsg,
            Filters =
            [
                new()
                {
                    Name = "PNG Image (.png)",
                    Extensions = [".png"]
                }
            ]
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
                    dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                        "Invalid dimensions",
                        $"The dimensions of this image should be {groupedGInfo.Width}x{groupedGInfo.Height}.\nFor this image type it is a strict requirement."
                        ));
                    return false;
                }

                var result = dialogService.ShowMessageBox(new MessageBoxSettings(
                    Title: "Invalid dimensions",
                    Message: $"The dimensions of this image should be {groupedGInfo.Width}x{groupedGInfo.Height}.\nIf will work if they are different, but may look weird in game.",
                    Buttons:
                    [
                        new MessageBoxButton("Proceed anyway", MessageBoxResult.No),
                        new MessageBoxButton("Auto Resize", MessageBoxResult.Yes),
                        new MessageBoxButton("Cancel", MessageBoxResult.Cancel),
                    ],
                    DefaultResult: MessageBoxResult.Cancel
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
            if (!dialogService.ShowDialogWithResult(vm))
            {
                return false;
            }
            file = temp;
        }

        overrideSpriteProvider.SetOverride(type, id, file);

        if (File.Exists(temp))
        {
            File.Delete(temp);
        }

        return true;
    }
}
