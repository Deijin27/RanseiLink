using System.IO;

namespace RanseiLink.Core.Maps;

public class Map
{
    public Map(BinaryReader br)
    {
        Header = new(br);
        PositionSection = new(br);
        if (br.BaseStream.Position != Header.TerrainSectionOffset)
        {
            throw new System.Exception($"Invalid position of terrain section 0x{br.BaseStream.Position}");
        }
        TerrainSection = new(br, Header.Width, Header.Height);
        if (br.BaseStream.Position != Header.GimmickSectionOffset)
        {
            throw new System.Exception($"Invalid position of gimmick section 0x{br.BaseStream.Position}");
        }
        GimmickSection = new(br, Header.GimmickCount);
    }

    public void WriteTo(BinaryWriter bw)
    {
        Header.GimmickCount = (ushort)GimmickSection.Items.Count;
        Header.Width = (ushort)TerrainSection.MapMatrix[0].Count;
        Header.Height = (ushort)TerrainSection.MapMatrix.Count;

        bw.Seek(MapHeader.DataLength, SeekOrigin.Begin);
        PositionSection.WriteTo(bw);
        Header.TerrainSectionOffset = (ushort)bw.BaseStream.Position;
        TerrainSection.WriteTo(bw);
        Header.GimmickSectionOffset = (ushort)bw.BaseStream.Position;
        GimmickSection.WriteTo(bw);

        Header.Length = (ushort)bw.BaseStream.Length;
        bw.BaseStream.Seek(0, SeekOrigin.Begin);
        Header.WriteTo(bw);
    }

    public MapHeader Header { get; set; }
    public MapPokemonPositionSection PositionSection { get; set; }
    public MapTerrainSection TerrainSection { get; set; }
    public MapGimmickSection GimmickSection { get; set; }
}
