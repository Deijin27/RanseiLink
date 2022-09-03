using RanseiLink.ValueConverters;
using System.Windows.Media;

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
        set
        {
            if (RaiseAndSetIfChanged(Z, value, v => Parent.TerrainEntry.SubCellZValues[_entryId] = value))
            {
                RaisePropertyChanged(nameof(Brush));
            }
        }
    }

    public MapRenderMode RenderMode { get; }

    public Brush Brush => SubCellToBrushConverter.ConvertValue(this);
}
