using RanseiLink.Core.Enums;

namespace RanseiLink.Services;

public interface IExternalService
{
    public string GetMoveAnimationUri(MoveAnimationId id);
    public string GetMoveMovementAnimationUri(MoveMovementAnimationId id);
}
