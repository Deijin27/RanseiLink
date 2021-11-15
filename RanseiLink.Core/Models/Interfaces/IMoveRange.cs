namespace RanseiLink.Core.Models.Interfaces;

public interface IMoveRange : IDataWrapper, ICloneable<IMoveRange>
{
    bool GetInRange(int position);

    void SetInRange(int position, bool isInRange);
}
