using RanseiLink.Core.Enums;

namespace RanseiLink.Windows.Services;

public interface IExternalService
{
    public string GetMoveAnimationUri(MoveAnimationId id);
    public string GetMoveMovementAnimationUri(MoveMovementAnimationId id);
}
