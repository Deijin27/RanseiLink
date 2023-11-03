
using System;

namespace RanseiLink.Core.Graphics; 

public struct Rgb15 : IEquatable<Rgb15>
{
    public static readonly Rgb15 White = new Rgb15(31, 31, 31);
    public static readonly Rgb15 Black = new Rgb15(0, 0, 0);

    public int R;
    public int G;
    public int B;
    public Rgb15(int r, int g, int b)
    {
        R = r;
        G = g;
        B = b;
    }
    private const int _mask = 0b11111;

    public static Rgb15 From(ushort value)
    {
        return new Rgb15
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

    public static bool operator ==(Rgb15 obj1, Rgb15 obj2) => obj1.Equals(obj2);
    public static bool operator !=(Rgb15 obj1, Rgb15 obj2) => !obj1.Equals(obj2);

    public override bool Equals(object? obj)
    {
        return obj is Rgb15 other && Equals(other);
    }

    public bool Equals(Rgb15 other)
    {
        return other.R == R && other.G == G && other.B == B;
    }

    public override int GetHashCode()
    {
        return ToUInt16();
    }

    public override string ToString()
    {
        return $"Rgb15({R}, {G}, {B})";
    }
}