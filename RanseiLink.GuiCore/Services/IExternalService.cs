#nullable enable
using RanseiLink.Core.Enums;

namespace RanseiLink.GuiCore.Services;

public interface IExternalService
{
    public string GetMoveAnimationUri(MoveAnimationId id);
    public string GetMoveMovementAnimationUri(MoveMovementAnimationId id);
}
