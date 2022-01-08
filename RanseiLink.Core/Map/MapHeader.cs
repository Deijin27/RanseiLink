using System.IO;
using System.Text;

namespace RanseiLink.Core.Map;

public class MapHeader
{
    public const int DataLength = 0x10;
    public const string MagicNumber = "MLSP"; // Pokemon Shoubu Level Map?
    public ushort Length { get; set; }
    public ushort Width { get; set; }
    public ushort Height { get; set; }
    public ushort TerrainSectionOffset { get; set; }
    public ushort GimmickSectionOffset { get; set; }
    public ushort GimmickCount { get; set; }

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
        Width = br.ReadUInt16();
        Height = br.ReadUInt16();
        TerrainSectionOffset = br.ReadUInt16();
        GimmickSectionOffset = br.ReadUInt16();
        GimmickCount = br.ReadUInt16();
    }

    public void WriteTo(BinaryWriter bw)
    {
        bw.Write(Encoding.UTF8.GetBytes(MagicNumber));
        bw.Write(Length);
        bw.Write(Width);
        bw.Write(Height);
        bw.Write(TerrainSectionOffset);
        bw.Write(GimmickSectionOffset);
        bw.Write(GimmickCount);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{(nameof(MagicNumber))}: {MagicNumber}");
        sb.AppendLine($"{nameof(Length)}: 0x{Length:X}");
        sb.AppendLine($"{nameof(Width)}: {Width}");
        sb.AppendLine($"{nameof(Height)}: {Height}");
        sb.AppendLine($"{nameof(TerrainSectionOffset)}: 0x{TerrainSectionOffset:X}");
        sb.AppendLine($"{nameof(GimmickSectionOffset)}: 0x{GimmickSectionOffset:X}");
        sb.AppendLine($"{nameof(GimmickCount)}: {GimmickCount}");

        return sb.ToString();
    }

}
