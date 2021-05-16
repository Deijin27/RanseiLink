using Core.Enums;

namespace Core.Models.Interfaces
{
    public interface IBuilding : IDataWrapper
    {
        LocationId Location { get; set; }
        string Name { get; set; }
    }
}