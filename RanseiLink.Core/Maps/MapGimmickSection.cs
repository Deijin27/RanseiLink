using RanseiLink.Core.Enums;
using System.Text;

namespace RanseiLink.Core.Maps;

public enum Orientation : byte
{
    East,
    North,
    West,
    South,
    Default = 0xFF
}

public enum OrientationAlt : byte
{
    North,
    East,
    South,
    West,
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
}

public class MapGimmickItem
{
    public GimmickId Gimmick { get; set; }
    public Position Position { get; set; }
    public Orientation Orientation { get; set; }
    public int UnknownValue { get; set; }
    public List<(ushort, ushort)> UnknownList { get; set; } = []; // usage seems to depend on gimmick type

    public MapGimmickItem()
    {
        Position = new Position(0, 0);
    }

    public MapGimmickItem(BinaryReader br)
    {
        Gimmick = (GimmickId)br.ReadByte();
        Position = new Position(br);
        Orientation = (Orientation)br.ReadByte();
        UnknownValue = br.ReadInt32();
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
        Items = [];

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

        var itemStarts = new List<int>();

        // write items
        foreach (var item in Items)
        {
            itemStarts.Add((int)(bw.BaseStream.Position - initOffset));
            item.WriteTo(bw);
        }

        var endPosition = bw.BaseStream.Position;

        // write item offsets
        bw.BaseStream.Position = initOffset;
        foreach (var start in itemStarts)
        {
            bw.Write(start);
        }

        bw.BaseStream.Position = endPosition;
    }

}