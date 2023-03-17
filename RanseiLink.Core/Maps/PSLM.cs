#nullable enable
using System.IO;
using System.Text;

namespace RanseiLink.Core.Maps;

/// <summary>
/// PSL Map
/// </summary>
public class PSLM
{
    public const string ExternalFileExtension = ".pslm";

    public struct Header
    {
        public const int HeaderLength = 0x10;
        public const string MagicNumber = "MLSP"; // PSL Map (PSL was the temporary name of the game)

        public ushort FileLength;
        public ushort Width;
        public ushort Height;
        public ushort TerrainSectionOffset;
        public ushort GimmickSectionOffset;
        public ushort GimmickCount;

        public Header(BinaryReader br)
        {
            var magicNumber = br.ReadMagicNumber();
            if (magicNumber != MagicNumber)
            {
                throw new InvalidDataException($"Unexpected magic number '{magicNumber}'. (expected: {MagicNumber})");
            }

            FileLength = br.ReadUInt16();
            Width = br.ReadUInt16();
            Height = br.ReadUInt16();
            TerrainSectionOffset = br.ReadUInt16();
            GimmickSectionOffset = br.ReadUInt16();
            GimmickCount = br.ReadUInt16();
        }

        public void WriteTo(BinaryWriter bw)
        {
            bw.WriteMagicNumber(MagicNumber);
            bw.Write(FileLength);
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
            sb.AppendLine($"{nameof(FileLength)}: 0x{FileLength:X}");
            sb.AppendLine($"{nameof(Width)}: {Width}");
            sb.AppendLine($"{nameof(Height)}: {Height}");
            sb.AppendLine($"{nameof(TerrainSectionOffset)}: 0x{TerrainSectionOffset:X}");
            sb.AppendLine($"{nameof(GimmickSectionOffset)}: 0x{GimmickSectionOffset:X}");
            sb.AppendLine($"{nameof(GimmickCount)}: {GimmickCount}");

            return sb.ToString();
        }

    }

    public PSLM(BinaryReader br)
    {
        var header = new Header(br);
        PositionSection = new MapPokemonPositionSection(br);
        if (br.BaseStream.Position != header.TerrainSectionOffset)
        {
            throw new InvalidDataException($"Invalid position of terrain section 0x{br.BaseStream.Position}");
        }
        TerrainSection = new MapTerrainSection(br, header.Width, header.Height);
        if (br.BaseStream.Position != header.GimmickSectionOffset)
        {
            throw new InvalidDataException($"Invalid position of gimmick section 0x{br.BaseStream.Position}");
        }
        GimmickSection = new MapGimmickSection(br, header.GimmickCount);
    }

    public void WriteTo(BinaryWriter bw)
    {
        var header = new Header
        {
            GimmickCount = (ushort)GimmickSection.Items.Count,
            Width = (ushort)TerrainSection.MapMatrix[0].Count,
            Height = (ushort)TerrainSection.MapMatrix.Count,
        };

        bw.Seek(Header.HeaderLength, SeekOrigin.Begin);
        PositionSection.WriteTo(bw);
        header.TerrainSectionOffset = (ushort)bw.BaseStream.Position;
        TerrainSection.WriteTo(bw);
        header.GimmickSectionOffset = (ushort)bw.BaseStream.Position;
        GimmickSection.WriteTo(bw);

        header.FileLength = (ushort)bw.BaseStream.Length;
        bw.BaseStream.Seek(0, SeekOrigin.Begin);
        header.WriteTo(bw);
    }

    public MapPokemonPositionSection PositionSection { get; set; }
    public MapTerrainSection TerrainSection { get; set; }
    public MapGimmickSection GimmickSection { get; set; }
}