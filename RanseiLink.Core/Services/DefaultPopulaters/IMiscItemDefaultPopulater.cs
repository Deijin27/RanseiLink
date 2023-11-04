using RanseiLink.Core.Resources;

namespace RanseiLink.Core.Services.DefaultPopulaters;

public interface IMiscItemDefaultPopulater
{
    MetaMiscItemId Id { get; }
    void ProcessExportedFiles(string defaultDataFolder, MiscConstants gInfo, MiscItem miscItem);
}
