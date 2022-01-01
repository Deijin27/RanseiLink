
namespace RanseiLink.Core.Types;

public record Rgb555(uint R, uint G, uint B)
{
    private static readonly uint _mask = 0b11111;

    public static Rgb555 From(uint value)
    {
        return new
        (
            value & _mask, 
            (value >> 5) & _mask, 
            (value >> 10) & _mask
        );
    }

    public uint ToUInt32()
    {
        return R | G << 5 | B << 10;
    }
}
