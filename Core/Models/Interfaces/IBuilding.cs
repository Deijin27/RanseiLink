using Core.Enums;

namespace Core.Models.Interfaces
{
    public interface IBuilding : IDataWrapper, ICloneable<IBuilding> 
    { 
        LocationId Location { get; set; }
        string Name { get; set; }
    }
}