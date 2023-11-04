using RanseiLink.Core.Resources;

namespace RanseiLink.Core.Services.DefaultPopulaters;

public interface IGraphicTypeDefaultPopulater
{
    MetaSpriteType Id { get; }
    void ProcessExportedFiles(string defaultDataFolder, IGraphicsInfo gInfo);
}