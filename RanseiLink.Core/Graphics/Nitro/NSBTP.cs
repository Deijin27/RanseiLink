using System.IO;
using System.Text;

namespace RanseiLink.Core.Graphics
{
    /// <summary>
    /// Model and material file
    /// </summary>
    public class NSBTP
    {
        public const string MagicNumber = "BTP0";
        public static readonly string[] FileExtensions = new[] { ".nsbtp", ".btp0", ".btp" };

        public NSPAT PatternAnimations { get; set; }

        public NSBTP()
        {

        }

        public NSBTP(string file)
        {
            using (var br = new BinaryReader(File.OpenRead(file)))
            {

                // first a typical file header
                var header = new NitroFileHeader(br);

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

                // read PAT
                br.BaseStream.Position = chunkOffsets[0];
                PatternAnimations = new NSPAT(br);
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
            var header = new NitroFileHeader
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

            // write PAT
            chunkOffsets[0] = (uint)(bw.BaseStream.Position);
            PatternAnimations.WriteTo(bw);

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
