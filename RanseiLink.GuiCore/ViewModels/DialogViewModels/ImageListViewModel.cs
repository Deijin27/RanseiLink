using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.ViewModels;
public class ImageListViewModel
{
    public ImageListViewModel(IEnumerable<SpriteFile> sprites, SpriteItemViewModelFactory factory)
    {
        Sprites = sprites.Select(x => factory.Create(x)).ToArray();
    }

    public SpriteItemViewModel[] Sprites { get; }

}
