
namespace RanseiLink.Core.Models.Interfaces;

public interface IMoveAnimation : IDataWrapper, ICloneable<IMoveAnimation>
{
    uint UnknownA { get; set; }
    uint UnknownB { get; set; }
}
