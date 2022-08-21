
using System.IO;

namespace RanseiLink.Core.Graphics
{

    /// <summary>
    /// Texture file
    /// </summary>
    public class NSBTX
    {
        public const string MagicNumber = "BTX0";
        public static readonly string[] FileExtensions = new[] { ".btx0", ".btx" };

        public NSTEX Texture { get; set; }

        public NSBTX()
        {

        }

        public NSBTX(string file)
        {
            using (var br = new BinaryReader(File.OpenRead(file)))
            {

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
                br.BaseStream.Position = chunkOffsets[0];
                Texture = new NSTEX(br);
            }
        }

        public void WriteTo(string file)
        {
            using (var bw = new BinaryWriter(File.OpenWrite(file)))
            {
                WriteTo(bw);
            }
        }

        public void WriteTo(BinaryWriter bw)
        {
            var header = new GenericFileHeader
            {
                MagicNumber = MagicNumber,
                ByteOrderMarker = 0xFEFF,
                Version = 1,
                ChunkCount = 1,
                HeaderLength = 0x10
            };

            // skip header section, to be written later
            bw.BaseStream.Seek(header.HeaderLength + 4 * header.ChunkCount, SeekOrigin.Begin);

            uint[] chunkOffsets = new uint[header.ChunkCount];

            // write TEX0
            chunkOffsets[0] = (uint)(bw.BaseStream.Position);
            Texture.WriteTo(bw);

            // return to start to write header
            var endOffset = bw.BaseStream.Position;
            bw.BaseStream.Position = 0;

            header.FileLength = (uint)endOffset;
            header.WriteTo(bw);
            foreach (var chunkOffset in chunkOffsets)
            {
                bw.Write(chunkOffset);
            }
        }
    }
}