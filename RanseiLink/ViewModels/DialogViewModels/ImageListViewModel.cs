using RanseiLink.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.ViewModels;
public class ImageListViewModel
{
    public ImageListViewModel(IEnumerable<SpriteFile> sprites, SpriteItemViewModelFactory factory)
    {
        Sprites = sprites.Select(x => factory(x)).ToArray();
    }

    public SpriteItemViewModel[] Sprites { get; }

}
