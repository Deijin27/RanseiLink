using System;
using System.IO;

namespace RanseiLink.Core.Map;

public struct Position : IEquatable<Position>
{
    public byte X { get; set; }
    public byte Y { get; set; }

    public Position(byte x, byte y)
    {
        X = x;
        Y = y;
    }

    public Position(BinaryReader br)
    {
        X = br.ReadByte();
        Y = br.ReadByte();
    }

    public void WriteTo(BinaryWriter bw)
    {
        bw.Write(X);
        bw.Write(Y);
    }

    public override string ToString()
    {
        return $"X={X}, Y={Y}";
    }

    public bool Equals(Position other)
    {
        return other.X == X && other.Y == Y;
    }

    public override bool Equals(object obj)
    {
        return obj is Position position && Equals(position);
    }

    public static bool operator ==(Position left, Position right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Position left, Position right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return X << 8 | Y;
    }
}
