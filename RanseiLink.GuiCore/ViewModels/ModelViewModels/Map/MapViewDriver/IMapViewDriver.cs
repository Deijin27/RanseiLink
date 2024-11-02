using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.GuiCore.ViewModels;

public interface IMapViewDriver
{
    Rgba32 GetCellColor(MapGridCellViewModel cell);
    Rgba32 GetSubCellColor(MapGridSubCellViewModel subCell);
    void OnMouseDownOnCell(MapGridCellViewModel cell);
    void OnMouseDownOnSubCell(MapGridSubCellViewModel cell);
}
