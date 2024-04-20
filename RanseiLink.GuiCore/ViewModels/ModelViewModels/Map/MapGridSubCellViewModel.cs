namespace RanseiLink.GuiCore.ViewModels;

public record MapSubCellInfo(MapRenderMode RenderMode, float Z);

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
            if (Set(Z, value, v => Parent.TerrainEntry.SubCellZValues[_entryId] = value))
            {
                Notify(nameof(Info));
            }
        }
    }

    public MapRenderMode RenderMode { get; }

    public MapSubCellInfo Info => new(RenderMode, Z);
}
