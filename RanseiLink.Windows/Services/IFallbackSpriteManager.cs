#nullable enable
namespace RanseiLink.Windows.Services;

public interface IFallbackSpriteManager
{
    Task CheckDefaultsPopulated();
    Task PopulateGraphicsDefaults();
}
