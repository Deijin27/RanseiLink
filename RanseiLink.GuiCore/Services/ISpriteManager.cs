#nullable enable
using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.Services;

public interface ISpriteManager
{
    Task<bool> SetOverride(SpriteType type, int id, string requestFileMsg);
}