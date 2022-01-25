
namespace RanseiLink.Core.Graphics;

public record struct Rgb15(int R, int G, int B)
{
    private const int _mask = 0b11111;

    public static Rgb15 From(ushort value)
    {
        return new
        (
            value & _mask, 
            (value >> 5) & _mask, 
            (value >> 10) & _mask
        );
    }

    public ushort ToUInt16()
    {
        return (ushort)(R | G << 5 | B << 10);
    }
}
