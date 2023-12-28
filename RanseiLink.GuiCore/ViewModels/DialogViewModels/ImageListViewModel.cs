using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.ViewModels;
public class ImageListViewModel
{
    public ImageListViewModel(IEnumerable<SpriteFile> sprites, SpriteItemViewModel.Factory factory)
    {
        Sprites = sprites.Select(x => factory().Init(x)).ToArray();
    }

    public SpriteItemViewModel[] Sprites { get; }

}
