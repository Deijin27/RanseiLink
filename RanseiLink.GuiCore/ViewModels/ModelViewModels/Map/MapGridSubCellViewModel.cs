using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.GuiCore.ViewModels;

public class MapGridSubCellViewModel : ViewModelBase
{
    private readonly int _visualId;
    private readonly int _dataId;
    public MapGridSubCellViewModel(MapGridCellViewModel parent, int visualId, int entryId)
    {
        _dataId = entryId;
        Parent = parent;
        _visualId = visualId;
    }

    public MapGridCellViewModel Parent { get; }

    private Rgba32 _color;
    public Rgba32 Color
    {
        get => _color;
        set => SetProperty(ref _color, value);
    }

    public float Z
    {
        get => Parent.TerrainEntry.SubCellZValues[_dataId];
        set => SetProperty(Z, value, v => Parent.TerrainEntry.SubCellZValues[_dataId] = value);
    }

    public int VisualId => _visualId;
}
