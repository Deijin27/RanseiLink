
using System.IO;

namespace RanseiLink.Core.Graphics;

/// <summary>
/// Texture file
/// </summary>
public class BTX0
{
    public const string MagicNumber = "BTX0";

    public TEX0 Texture { get; set; }

    public BTX0(BinaryReader br)
    {
        long initOffset = br.BaseStream.Position;

        // first a typical file header
        var header = new GenericFileHeader(br);

        if (header.MagicNumber != MagicNumber)
        {
            throw new InvalidDataException($"Unexpected magic number in file header '{header.MagicNumber}' (expected: {MagicNumber})");
        }

        // thing that this format does that other formats don't seem to do.
        uint[] chunkOffsets = new uint[header.ChunkCount];
        for (int i = 0; i < header.ChunkCount; i++)
        {
            chunkOffsets[i] = br.ReadUInt32();
        }

        // read TEX0
        br.BaseStream.Position = initOffset + chunkOffsets[0];
        Texture = new TEX0(br);
    }

    public void WriteTo(BinaryWriter bw)
    {
        long initOffset = bw.BaseStream.Position;

        var header = new GenericFileHeader
        {
            MagicNumber = MagicNumber,
            Version = 1,
            ChunkCount = 1,
        };

        // skip header section, to be written later
        bw.BaseStream.Seek(header.HeaderLength + 4 * header.ChunkCount, SeekOrigin.Begin);

        uint[] chunkOffsets = new uint[header.ChunkCount];

        // write TEX0
        chunkOffsets[0] = (uint)(bw.BaseStream.Position - initOffset);
        Texture.WriteTo(bw);

        // return to start to write header
        var endOffset = bw.BaseStream.Position;
        bw.BaseStream.Position = initOffset;

        header.FileLength = (uint)(endOffset - initOffset);
        header.WriteTo(bw);
        foreach (var chunkOffset in chunkOffsets)
        {
            bw.Write(chunkOffset);
        }

        // return to end of file
        bw.BaseStream.Position = endOffset;
    }
}
