using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.GuiCore.ViewModels;

public class ElevationMapViewDriver : IMapViewDriver
{
    private const double __value = 0.6;
    private const double __saturation = 0.6;

    private static Rgba32 ColorFromHSV(double hue, double saturation, double value)
    {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);

        value *= 255;
        byte v = (byte)value;
        byte p = (byte)(value * (1 - saturation));
        byte q = (byte)(value * (1 - f * saturation));
        byte t = (byte)(value * (1 - (1 - f) * saturation));

        return hi switch
        {
            0 => new Rgba32(v, t, p),
            1 => new Rgba32(q, v, p),
            2 => new Rgba32(p, v, t),
            3 => new Rgba32(p, q, v),
            4 => new Rgba32(t, p, v),
            _ => new Rgba32(v, p, q)
        };
    }

    private bool _paintEntireCell;

    private static Rgba32 ZToColor(float value)
    {
        return ColorFromHSV((double)value / 40 / 25 * 255, __saturation, __value);
    }

    public Rgba32 GetCellColor(MapGridCellViewModel cell)
    {
        return Color.Transparent;
    }

    public Rgba32 GetSubCellColor(MapGridSubCellViewModel subCell)
    {
        return ZToColor(subCell.Z);
    }

    private float _elevationToPaint;

    public void OnMouseDownOnCell(MapGridCellViewModel cell)
    {
        if (_paintEntireCell)
        {
            cell.SubCell0.Z = _elevationToPaint;
            cell.SubCell1.Z = _elevationToPaint;
            cell.SubCell2.Z = _elevationToPaint;
            cell.SubCell3.Z = _elevationToPaint;
            cell.SubCell4.Z = _elevationToPaint;
            cell.SubCell5.Z = _elevationToPaint;
            cell.SubCell6.Z = _elevationToPaint;
            cell.SubCell7.Z = _elevationToPaint;
            cell.SubCell8.Z = _elevationToPaint;
        }
    }

    public void OnMouseDownOnSubCell(MapGridSubCellViewModel cell)
    {
        if (!_paintEntireCell)
        {
            cell.Z = _elevationToPaint;
        }
    }
}
