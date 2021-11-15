
namespace RanseiLink.Core.Models.Interfaces;

public interface IGimmick : IDataWrapper, ICloneable<IGimmick>
{
    string Name { get; set; }
}
