#nullable enable
namespace RanseiLink.GuiCore.Services;

public interface IFallbackSpriteManager
{
    Task CheckDefaultsPopulated();
    Task PopulateGraphicsDefaults();
}
