using RanseiLink.Core.Enums;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RanseiLink.Core.Map;

public enum Orientation : byte
{
    East,
    South,
    West,
    North,
    Default = 0xFF
}

public class MapGimmickItem
{
    public GimmickId Gimmick { get; set; }
    public byte X { get; set; }
    public byte Y { get; set; }
    public Orientation Orientation { get; set; }
    public byte Unknown4 { get; set; }
    public byte Unknown5 { get; set; }
    public byte Unknown6 { get; set; }
    public byte Unknown7 { get; set; }
    public List<(ushort, ushort)> Unknown8 { get; set; }

    public MapGimmickItem(BinaryReader br)
    {
        Gimmick = (GimmickId)br.ReadByte();
        X = br.ReadByte();
        Y = br.ReadByte();
        Orientation = (Orientation)br.ReadByte();
        Unknown4 = br.ReadByte();
        Unknown5 = br.ReadByte();
        Unknown6 = br.ReadByte();
        Unknown7 = br.ReadByte();
        var count = br.ReadInt32();
        Unknown8 = new List<(ushort, ushort)>(count);
        for (int i = 0; i < count; i++)
        {
            Unknown8.Add((br.ReadUInt16(), br.ReadUInt16()));
        }
    }

    public void WriteTo(BinaryWriter bw)
    {
        bw.Write(((byte)Gimmick));
        bw.Write(X);
        bw.Write(Y);
        bw.Write(((byte)Orientation));
        bw.Write(((byte)Unknown4));
        bw.Write(((byte)Unknown5));
        bw.Write(((byte)Unknown6));
        bw.Write(((byte)Unknown7));
        bw.Write(Unknown8.Count);
        foreach (var item in Unknown8)
        {
            bw.Write(item.Item1);
            bw.Write(item.Item2);
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Gimmick: {Gimmick}");
        sb.AppendLine($"Position: X={X}, Y={Y}");
        sb.AppendLine($"Orientation: {Orientation}");
        sb.AppendLine($"Unknowns: {Unknown4:X} {Unknown5:X} {Unknown6:X} {Unknown7:X}");
        if (Unknown8.Count > 0)
        {
            sb.AppendLine($"Unknown8:");
            foreach (var item in Unknown8)
            {
                sb.AppendLine($"- {item.Item1}, {item.Item2}");
            }
        }

        return sb.ToString();
    }
}

public class MapGimmickSection
{
    public List<MapGimmickItem> Items { get; set; }

    public MapGimmickSection(BinaryReader br)
    {
        // save init offset for later
        long initOffset = br.BaseStream.Position;

        // Read allocation table
        int firstItemStart = br.ReadInt32();
        long firstItemPosition = initOffset + firstItemStart;
        List<int> itemStarts = new();
        itemStarts.Add(firstItemStart);
        while (br.BaseStream.Position < firstItemPosition)
        {
            itemStarts.Add(br.ReadInt32());
        }

        // Read items
        Items = new List<MapGimmickItem>();
        foreach (var start in itemStarts)
        {
            br.BaseStream.Position = initOffset + start;
            Items.Add(new MapGimmickItem(br));
        }

    }

    public void WriteTo(BinaryWriter bw)
    {
        var initOffset = bw.BaseStream.Position;
        bw.Seek(Items.Count * 4, SeekOrigin.Current);

        List<int> itemStarts = new();

        // write items
        foreach (var item in Items)
        {
            itemStarts.Add((int)(bw.BaseStream.Position - initOffset));
            item.WriteTo(bw);
        }

        // write item offsets
        bw.BaseStream.Position = initOffset;
        foreach (var start in itemStarts)
        {
            bw.Write(start);
        }
    }

}
