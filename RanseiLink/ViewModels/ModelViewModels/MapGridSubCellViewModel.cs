using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RanseiLink.ViewModels;

public class MapGridSubCellViewModel : ViewModelBase
{
    private readonly int _entryId;
    public MapGridSubCellViewModel(MapGridCellViewModel parent, int entryId, MapRenderMode renderMode)
    {
        _entryId = entryId;
        Parent = parent;
        RenderMode = renderMode;
    }

    public MapGridCellViewModel Parent { get; }

    public float Z
    {
        get => Parent.TerrainEntry.SubCellZValues[_entryId];
        set => RaiseAndSetIfChanged(Z, value, v => Parent.TerrainEntry.SubCellZValues[_entryId] = value);
    }

    public MapRenderMode RenderMode { get; }
}
