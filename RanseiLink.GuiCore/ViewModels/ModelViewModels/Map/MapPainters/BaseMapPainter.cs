using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.GuiCore.ViewModels;

public abstract class BaseMapPainter : ViewModelBase
{

    public abstract string Name { get; }

    private static Rgba32 __transparent = Color.Transparent;
    public virtual Rgba32 GetCellColor(MapGridCellViewModel cell)
    {
        return __transparent;
    }

    public virtual Rgba32 GetSubCellColor(MapGridSubCellViewModel subCell)
    {
        return __transparent;
    }

    public virtual void OnMouseDownOnCell(MapGridCellViewModel cell)
    {
    }

    public virtual void OnMouseDownOnSubCell(MapGridSubCellViewModel cell)
    {
    }
}
