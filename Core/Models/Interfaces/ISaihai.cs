using Core.Enums;

namespace Core.Models.Interfaces
{
    public interface ISaihai : IDataWrapper, ICloneable<ISaihai>
    {
        uint Duration { get; set; }
        SaihaiEffectId Effect1 { get; set; }
        uint Effect1Amount { get; set; }
        SaihaiEffectId Effect2 { get; set; }
        uint Effect2Amount { get; set; }
        SaihaiEffectId Effect3 { get; set; }
        uint Effect3Amount { get; set; }
        string Name { get; set; }
        SaihaiTargetId Target { get; set; }
    }
}