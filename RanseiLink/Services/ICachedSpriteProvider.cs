using RanseiLink.Core.Services;
using System.Windows.Media;

namespace RanseiLink.Services;

/// <summary>
/// A service to avoid constantly reloading common images e.g. the little pokemon and warrior sprites.
/// Be careful not to use this too much and waste a lot of memory
/// </summary>
public interface ICachedSpriteProvider
{
    ImageSource GetSprite(SpriteType type, int id);
}
