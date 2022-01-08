using System.IO;

namespace RanseiLink.Core.Map;

public class Map
{
    public Map(BinaryReader br)
    {
        MapHeader header = new(br);

        br.BaseStream.Position = header.GimmickSectionOffset;
        GimmickSection = new(br);
    }

    public MapGimmickSection GimmickSection { get; set; }
}
