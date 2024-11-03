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