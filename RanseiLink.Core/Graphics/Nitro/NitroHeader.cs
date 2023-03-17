#nullable enable
using System.IO;

namespace RanseiLink.Core.Graphics;

public struct NitroFileHeader
{
    public string MagicNumber;
    public ushort ByteOrderMarker;
    public ushort Version;
    public uint FileLength;
    public ushort HeaderLength;
    public ushort ChunkCount;

    public NitroFileHeader(BinaryReader br)
    {
        MagicNumber = br.ReadMagicNumber();
        ByteOrderMarker = br.ReadUInt16();
        Version = br.ReadUInt16();
        FileLength = br.ReadUInt32();
        HeaderLength = br.ReadUInt16();
        ChunkCount = br.ReadUInt16();
    }

    public void WriteTo(BinaryWriter bw)
    {
        bw.WriteMagicNumber(MagicNumber);
        bw.Write(ByteOrderMarker);
        bw.Write(Version);
        bw.Write(FileLength);
        bw.Write(HeaderLength);
        bw.Write(ChunkCount);
    }
}

public struct NitroChunkHeader
{
    public const int Length = 8;

    public string MagicNumber;
    public uint ChunkLength;

    public NitroChunkHeader(BinaryReader br)
    {
        MagicNumber = br.ReadMagicNumber();
        ChunkLength = br.ReadUInt32();
    }

    public void WriteTo(BinaryWriter bw)
    {
        bw.WriteMagicNumber(MagicNumber);
        bw.Write(ChunkLength);
    }
}