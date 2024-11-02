using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.GuiCore.ViewModels;

public class MapGridSubCellViewModel : ViewModelBase
{
    private readonly int _entryId;
    public MapGridSubCellViewModel(MapGridCellViewModel parent, int entryId)
    {
        _entryId = entryId;
        Parent = parent;
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
        get => Parent.TerrainEntry.SubCellZValues[_entryId];
        set => SetProperty(Z, value, v => Parent.TerrainEntry.SubCellZValues[_entryId] = value);
    }
}
