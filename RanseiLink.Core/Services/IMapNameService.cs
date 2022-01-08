
using System.Collections.Generic;

namespace RanseiLink.Core.Services;

public interface IMapNameService
{
    string MapFolderPath { get; }

    public ICollection<MapName> GetMaps();
}
