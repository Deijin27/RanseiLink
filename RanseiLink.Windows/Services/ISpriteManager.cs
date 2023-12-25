#nullable enable
using RanseiLink.Core.Services;

namespace RanseiLink.Windows.Services;

public interface ISpriteManager
{
    bool SetOverride(SpriteType type, int id, string requestFileMsg);
}