#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.Services;

/// <summary>
/// A service to avoid constantly reloading common images e.g. the little pokemon and warrior sprites.
/// Be careful not to use this too much and waste a lot of memory
/// </summary>
public interface ICachedSpriteProvider
{
    IReadOnlyList<object?> GetClusterImages(string linkPath, int[] clusters);
    object? GetGimmickPreview(Gimmick model, MovePreviewOptions options = MovePreviewOptions.All);
    object? GetGimmickRangePreview(GimmickRangeId range);
    object? GetMovePreview(Move model, MovePreviewOptions options = MovePreviewOptions.All);
    object? GetMoveRangePreview(MoveRange model);
    object? GetMoveRangePreview(MoveRangeId range);
    object? GetSprite(SpriteType type, int id);
}
