using Core.Enums;

namespace Core.Models.Interfaces
{
    public interface IAbility : IDataWrapper
    {
        AbilityEffectId Effect1 { get; set; }
        uint Effect1Amount { get; set; }
        AbilityEffectId Effect2 { get; set; }
        uint Effect2Amount { get; set; }
        string Name { get; set; }
    }
}