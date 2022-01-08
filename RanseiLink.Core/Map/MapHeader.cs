using System.IO;
using System.Text;

namespace RanseiLink.Core.Map;

public class MapHeader
{
    public const string MagicNumber = "MLSP";
    public ushort Length { get; set; }
    public ushort Unknown1 { get; set; }
    public ushort Unknown2 { get; set; }
    public ushort UnknownSection1Offset { get; set; }
    public ushort GimmickSectionOffset { get; set; }

    public MapHeader()
    {
    }

    public MapHeader(BinaryReader br)
    {
        var magicNumber = Encoding.UTF8.GetString(br.ReadBytes(4));
        if (magicNumber != MagicNumber)
        {
            throw new System.Exception($"Unexpected magic number '{magicNumber}' in {typeof(MapHeader).FullName}");
        }

        Length = br.ReadUInt16();
        Unknown1 = br.ReadUInt16();
        Unknown2 = br.ReadUInt16();
        UnknownSection1Offset = br.ReadUInt16();
        GimmickSectionOffset = br.ReadUInt16();
    }

    public void WriteTo(BinaryWriter bw)
    {
        bw.Write(Encoding.UTF8.GetBytes(MagicNumber));
        bw.Write(Length);
        bw.Write(Unknown1);
        bw.Write(Unknown2);
        bw.Write(UnknownSection1Offset);
        bw.Write(GimmickSectionOffset);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{(nameof(MagicNumber))}: {MagicNumber}");
        sb.AppendLine($"{nameof(Length)}: 0x{Length:X}");
        sb.AppendLine($"{nameof(Unknown1)}: 0x{Unknown1:X}");
        sb.AppendLine($"{nameof(Unknown2)}: 0x{Unknown2:X}");
        sb.AppendLine($"{nameof(UnknownSection1Offset)}: 0x{UnknownSection1Offset:X}");
        sb.AppendLine($"{nameof(GimmickSectionOffset)}: 0x{GimmickSectionOffset:X}");

        return sb.ToString();
    }

}
