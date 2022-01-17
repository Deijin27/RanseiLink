
using RanseiLink.Core.Maps;
using System.Collections.Generic;

namespace RanseiLink.Core.Services;

public interface IMapIdService
{
    string MapFolderPath { get; }

    public ICollection<MapId> GetMaps();
}
