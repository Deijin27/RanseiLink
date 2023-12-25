namespace RanseiLink.Core.Graphics;

/// <summary>
/// Cell Labels
/// </summary>
public class LABL
{
    public List<string> Names { get; set; } = new List<string>();

    public const string MagicNumber = "LBAL";

    public LABL()
    {

    }

    public LABL(BinaryReader br)
    {
        var initOffset = br.BaseStream.Position;
        var header = new NitroChunkHeader(br);
        if (header.MagicNumber != MagicNumber)
        {
            throw new InvalidDataException($"Unexpected magic number '{header.MagicNumber}' at offset 0x{initOffset:X}. (expected: {MagicNumber})");
        }
        var endOffset = initOffset + header.ChunkLength;

        // it's weird that they don't store the number of names
        // it doesn't always correspond to the number of banks
        // this is the bests I could come up with to consistently work
        var labelOffsets = new List<uint>();
        uint offset = br.ReadUInt32();
        while (offset <= 0xFFFF)
        {
            labelOffsets.Add(offset);
            offset = br.ReadUInt32();
        }

        var nameStart = br.BaseStream.Position - 4;

        var buffer = new byte[50];
        // The labels are null-terminated strings
        foreach (var lblOff in labelOffsets)
        {
            br.BaseStream.Position = nameStart + lblOff;
            Names.Add(br.ReadNullTerminatedString(buffer));
        }

        br.BaseStream.Position = endOffset;
    }

    public void WriteTo(BinaryWriter bw)
    {
        var initOffset = bw.BaseStream.Position;
        var header = new NitroChunkHeader
        {
            MagicNumber = MagicNumber,
        };

        bw.Pad(NitroChunkHeader.Length + Names.Count * 4);
        uint[] nameOffsets = new uint[Names.Count];
        var nameStart = bw.BaseStream.Position;
        for (int i = 0; i < nameOffsets.Length; i++)
        {
            nameOffsets[i] = (uint)(bw.BaseStream.Position - nameStart);
            bw.WriteNullTerminatedString(Names[i]);
        }

        var endOffset = bw.BaseStream.Position;
        header.ChunkLength = (uint)(endOffset - initOffset);

        // write header
        bw.BaseStream.Position = initOffset;
        header.WriteTo(bw);
        foreach (uint name in nameOffsets)
        {
            bw.Write(name);
        }

        bw.BaseStream.Position = endOffset;
    }
}
