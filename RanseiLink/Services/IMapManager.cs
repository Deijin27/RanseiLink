using RanseiLink.Core.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RanseiLink.Services;

public interface IMapManager
{
    public bool IsOverriden(MapId id);
    public bool ExportPac(MapId id);
    public bool ImportPac(MapId id);
    public bool ExportObj(MapId id);
    public bool ImportObj(MapId id);
    public bool ExportPslm(MapId id);
    public bool ImportPslm(MapId id);
    public bool ExportBundle(MapId id);
    public bool ImportBundle(MapId id);
    bool RevertModelToDefault(MapId id);
}