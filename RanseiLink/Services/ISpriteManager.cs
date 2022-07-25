using RanseiLink.Core.Services;

namespace RanseiLink.Services;

public interface ISpriteManager
{
    bool SetOverride(SpriteType type, int id, string requestFileMsg);
}