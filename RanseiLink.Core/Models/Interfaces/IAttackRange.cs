namespace RanseiLink.Core.Models.Interfaces;

/// <summary>
/// The same for MoveRange and GimmickRange
/// </summary>
public interface IAttackRange : IDataWrapper, ICloneable<IAttackRange>
{
    bool GetInRange(int position);

    void SetInRange(int position, bool isInRange);
}
