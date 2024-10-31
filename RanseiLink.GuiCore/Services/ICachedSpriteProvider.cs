#nullable enable
using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.Services;

/// <summary>
/// A service to avoid constantly reloading common images e.g. the little pokemon and warrior sprites.
/// Be careful not to use this too much and waste a lot of memory
/// </summary>
public interface ICachedSpriteProvider
{
    IReadOnlyList<object?> GetClusterImages(string linkPath, int[] clusters);
    object? GetSprite(SpriteType type, int id);
}
