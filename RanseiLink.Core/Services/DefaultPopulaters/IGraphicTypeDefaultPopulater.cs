using RanseiLink.Core.Resources;

namespace RanseiLink.Core.Services.DefaultPopulaters;

public interface IGraphicTypeDefaultPopulater
{
    void ProcessExportedFiles(string defaultDataFolder, IGraphicsInfo gInfo);
}
