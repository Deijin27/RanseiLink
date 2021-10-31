using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces
{
    public interface IBuilding : IDataWrapper, ICloneable<IBuilding> 
    { 
        KingdomId Kingdom { get; set; }
        string Name { get; set; }
    }
}