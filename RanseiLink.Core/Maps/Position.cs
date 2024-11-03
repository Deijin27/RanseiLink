namespace RanseiLink.Core.Maps;

public class Position
{
    public byte X { get; set; }
    public byte Y { get; set; }
    public Position(byte x, byte y)
    {
        X = x;
        Y = y;
    }

    public Position(BinaryReader br) : this(br.ReadByte(), br.ReadByte())
    {
    }

    public void WriteTo(BinaryWriter bw)
    {
        bw.Write(X); 
        bw.Write(Y);
    }
}
