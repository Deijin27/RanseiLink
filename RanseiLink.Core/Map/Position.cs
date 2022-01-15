using System.IO;

namespace RanseiLink.Core.Map;

public record Position(byte X, byte Y)
{
    public Position(BinaryReader br) : this(br.ReadByte(), br.ReadByte())
    {
    }

    public void WriteTo(BinaryWriter bw)
    {
        bw.Write(X);
        bw.Write(Y);
    }
}
