using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RanseiLink.Core.Map;

public enum Terrain : byte
{
    Normal,
    Magma,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Sand,
    Soil,
    Snow,
    Swamp,
    Bog,
    Scaffolding,
    Hidden,
    HiddenPoison,
    Unused_1,
    Void,
    Default,
}

public class MapTerrainEntry
{
    public int[] Unknown1 { get; }
    public Terrain Terrain { get; set; }
    public byte Unknown3 { get; set; }
    
    public byte Unknown4 { get; set; }
    public byte Unknown5 { get; set; }

    public MapTerrainEntry(BinaryReader br)
    {
        Unknown1 = new int[9];
        for (int i = 0; i < 9; i++)
        {
            Unknown1[i] = br.ReadInt32();
        }
        Terrain = (Terrain)br.ReadByte();
        Unknown3 = br.ReadByte();
        Unknown4 = br.ReadByte();
        Unknown5 = br.ReadByte();
    }

    public void WriteTo(BinaryWriter bw)
    {
        foreach (var entry in Unknown1)
        {
            bw.Write(entry);
        }
        bw.Write((byte)Terrain);
        bw.Write(Unknown3);
        bw.Write(Unknown4);
        bw.Write(Unknown5);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (int b in Unknown1)
        {
            sb.Append(b.ToString("X").PadLeft(9, ' '));
        }
        sb.Append($"{Terrain,13}");
        sb.Append($"{Unknown3,4}");
        sb.Append($"{Unknown4,4}");
        sb.Append($"{Unknown5,4}");
        return sb.ToString();
    }
}

public class MapTerrainSection
{
    public List<List<MapTerrainEntry>> MapMatrix { get; }
    public MapTerrainSection(BinaryReader br, ushort width, ushort height)
    {
        MapMatrix = new List<List<MapTerrainEntry>>();

        for (int y = 0; y < height; y++)
        {
            List<MapTerrainEntry> row = new();
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
