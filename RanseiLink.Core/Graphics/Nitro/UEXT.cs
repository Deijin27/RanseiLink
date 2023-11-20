using System.IO;

namespace RanseiLink.Core.Graphics;

/// <summary>
/// Whomst are youmst?
/// </summary>
public class UEXT
{
    public uint Unknown { get; set; }

    public const string MagicNumber = "TXEU";

    public UEXT()
    {

    }

    public UEXT(BinaryReader br)
    {
        var initOffset = br.BaseStream.Position;
        var header = new NitroChunkHeader(br);
        if (header.MagicNumber != MagicNumber)
        {
            throw new InvalidDataException($"Unexpected magic number '{header.MagicNumber}' at offset 0x{initOffset:X}. (expected: {MagicNumber})");
        }

        Unknown = br.ReadUInt32();

        br.BaseStream.Position = initOffset + header.ChunkLength;
    }

    public void WriteTo(BinaryWriter bw)
    {
        var header = new NitroChunkHeader
        {
            MagicNumber = MagicNumber,
            ChunkLength = NitroChunkHeader.Length + 4
        };
        header.WriteTo(bw);
        bw.Write(Unknown);
    }
} 