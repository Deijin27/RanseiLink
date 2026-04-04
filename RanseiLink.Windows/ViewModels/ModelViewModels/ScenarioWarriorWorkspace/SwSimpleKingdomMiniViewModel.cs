#nullable enable
using FactoryGenie;
using RanseiLink.Core.Enums;

namespace RanseiLink.Windows.ViewModels;

[GenerateFactory]
public class SwSimpleKingdomMiniViewModel(
    ICachedSpriteProvider spriteProvider, 
    IIdToNameService idToNameService, 
    [FactoryParam] KingdomId kingdom) 
    : BaseSimpleKingdomMiniViewModel(spriteProvider, idToNameService, kingdom)
{
    public virtual int Army
    {
        get => 17;
        set { }
    }
}
