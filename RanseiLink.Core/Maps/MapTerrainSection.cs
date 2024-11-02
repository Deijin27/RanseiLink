using System.Text;

namespace RanseiLink.Core.Maps;

public enum MapBounds
{
    InBounds,
    OutOfBounds,
    Pokemon_0,
    Pokemon_1,
    Pokemon_2,
    Pokemon_3,
    Pokemon_4,
    Pokemon_5,
    Pokemon_6,
    Pokemon_7,
    Pokemon_8,
    Pokemon_9,
    Pokemon_10,
    Pokemon_11,

    // unknowns probably aren't used. Are they anywhere
    Unknown_0,
    Unknown_1,
    Unknown_2,
    Unknown_3,
    Unknown_4,
    Unknown_5,
    Unknown_6,
    Unknown_7,
    Unknown_8,
    Unknown_9,
    Unknown_10,
    Unknown_11,
    Unknown_12,
    Unknown_13,
    Unknown_14,
    Unknown_15,
    Unknown_16,
    Unknown_17,
    Unknown_18,
    Unknown_19,
    Unknown_20,
    Unknown_Penultimate = 254,
    Unknown_Final = 255
}
public class MapTerrainEntry
{
    /// <summary>
    /// The elevation of each sub-cell in the grid. There is 9 sub cells
    /// </summary>
    public float[] SubCellZValues { get; }
    public TerrainId Terrain { get; set; }
    public MapBounds Unknown3 { get; set; }

    public byte Unknown4 { get; set; }
    public OrientationAlt Orientation { get; set; }

    public MapTerrainEntry()
    {
        SubCellZValues = new float[9];
        Terrain = TerrainId.Default;
        Unknown4 = 0xFF;
    }

    public MapTerrainEntry(BinaryReader br)
    {
        SubCellZValues = new float[9];
        for (int i = 0; i < 9; i++)
        {
            SubCellZValues[i] = Util.FixedPoint.Fix_1_19_12(br.ReadInt32()) * 25;
        }
        Terrain = (TerrainId)br.ReadByte();
        Unknown3 = (MapBounds)br.ReadByte();
        Unknown4 = br.ReadByte();
        Orientation = (OrientationAlt)br.ReadByte();
    }

    public void WriteTo(BinaryWriter bw)
    {
        foreach (var entry in SubCellZValues)
        {
            bw.Write(Util.FixedPoint.InverseFix_1_19_12(entry / 25));
        }
        bw.Write((byte)Terrain);
        bw.Write((byte)Unknown3);
        bw.Write(Unknown4);
        bw.Write((byte)Orientation);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var b in SubCellZValues)
        {
            sb.Append($"{b,9:X}");
        }
        sb.Append($"{Terrain,13}");
        sb.Append($"{Unknown3,4}");
        sb.Append($"{Unknown4,4}");
        sb.Append($"{Orientation,4}");
        return sb.ToString();
    }
}

public class MapTerrainSection
{
    public List<List<MapTerrainEntry>> MapMatrix { get; }
    public MapTerrainSection(BinaryReader br, ushort width, ushort height)
    {
        MapMatrix = [];

        for (int y = 0; y < height; y++)
        {
            var row = new List<MapTerrainEntry>();
            for (int x = 0; x < width; x++)
            {
                row.Add(new MapTerrainEntry(br));
            }
            MapMatrix.Add(row);
        }
    }

    public void WriteTo(BinaryWriter bw)
    {
        foreach (var row in MapMatrix)
        {
            foreach (var entry in row)
            {
                entry.WriteTo(bw);
            }
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        int y = 0;
        foreach (var row in MapMatrix)
        {
            sb.AppendLine($"\nRow: {y++}\n");
            foreach (var entry in row)
            {
                sb.AppendLine(entry.ToString());
            }
        }
        return sb.ToString();
    }
}