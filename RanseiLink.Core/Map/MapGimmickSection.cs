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
    public Position Position { get; set; }
    public Orientation Orientation { get; set; }
    public uint UnknownValue { get; set; }
    public List<(ushort, ushort)> UnknownList { get; set; } = new(); // usage seems to depend on gimmick type

    public MapGimmickItem()
    {
        Position = new Position(0, 0);
    }

    public MapGimmickItem(BinaryReader br)
    {
        Gimmick = (GimmickId)br.ReadByte();
        Position = new Position(br);
        Orientation = (Orientation)br.ReadByte();
        UnknownValue = br.ReadUInt32();
        var count = br.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            UnknownList.Add((br.ReadUInt16(), br.ReadUInt16()));
        }
    }

    public void WriteTo(BinaryWriter bw)
    {
        bw.Write(((byte)Gimmick));
        Position.WriteTo(bw);
        bw.Write(((byte)Orientation));
        bw.Write(UnknownValue);
        bw.Write(UnknownList.Count);
        foreach (var item in UnknownList)
        {
            bw.Write(item.Item1);
            bw.Write(item.Item2);
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Gimmick: {Gimmick}");
        sb.AppendLine($"Position: {Position}");
        sb.AppendLine($"Orientation: {Orientation}");
        sb.AppendLine($"Unknown Value: {UnknownValue:X}");
        if (UnknownList.Count > 0)
        {
            sb.AppendLine($"Unknown List:");
            foreach (var item in UnknownList)
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

    public MapGimmickSection(BinaryReader br, ushort gimmickCount)
    {
        Items = new List<MapGimmickItem>();

        if (gimmickCount == 0)
        {
            return;
        }
        // Read allocation table just to find start of items
        int firstItemStart = br.ReadInt32();
        br.BaseStream.Seek(firstItemStart - 4, SeekOrigin.Current);

        // Read items
        for (int i = 0; i < gimmickCount; i++)
        {
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
