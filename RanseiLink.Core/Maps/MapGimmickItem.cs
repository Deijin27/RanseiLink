using RanseiLink.Core.Enums;
using System.Text;

namespace RanseiLink.Core.Maps;

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
