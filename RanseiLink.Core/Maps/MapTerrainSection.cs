using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RanseiLink.Core.Maps
{

    public class MapTerrainEntry
    {
        /// <summary>
        /// The elevation of each sub-cell in the grid. There is 9 sub cells
        /// </summary>
        public int[] SubCellZValues { get; }
        public TerrainId Terrain { get; set; }
        public byte Unknown3 { get; set; }

        public byte Unknown4 { get; set; }
        public byte Unknown5 { get; set; }

        public MapTerrainEntry()
        {
            SubCellZValues = new int[9];
            Terrain = TerrainId.Default;
            Unknown4 = 0xFF;
        }

        public MapTerrainEntry(BinaryReader br)
        {
            SubCellZValues = new int[9];
            for (int i = 0; i < 9; i++)
            {
                SubCellZValues[i] = br.ReadInt32() / 0x800; // they're all divisible by this, may as well scale down
            }
            Terrain = (TerrainId)br.ReadByte();
            Unknown3 = br.ReadByte();
            Unknown4 = br.ReadByte();
            Unknown5 = br.ReadByte();
        }

        public void WriteTo(BinaryWriter bw)
        {
            foreach (var entry in SubCellZValues)
            {
                bw.Write(entry * 0x800);
            }
            bw.Write((byte)Terrain);
            bw.Write(Unknown3);
            bw.Write(Unknown4);
            bw.Write(Unknown5);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (int b in SubCellZValues)
            {
                sb.Append($"{b,9:X}");
            }
            sb.Append($"{Terrain,13}");
            sb.Append($"{Unknown3,4}");
            sb.Append($"{Unknown4,4}");
            sb.Append($"{Unknown5,4}");
            return sb.ToString();
        }
    }

    public class MapTerrainSection
    {
        public List<List<MapTerrainEntry>> MapMatrix { get; }
        public MapTerrainSection(BinaryReader br, ushort width, ushort height)
        {
            MapMatrix = new List<List<MapTerrainEntry>>();

            for (int y = 0; y < height; y++)
            {
                var row = new List<MapTerrainEntry>();
                for (int x = 0; x < width; x++)
                {
                    row.Add(new MapTerrainEntry(br));
                }
                MapMatrix.Add(row);
            }
        }

        public void WriteTo(BinaryWriter bw)
        {
            foreach (var row in MapMatrix)
            {
                foreach (var entry in row)
                {
                    entry.WriteTo(bw);
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            int y = 0;
            foreach (var row in MapMatrix)
            {
                sb.AppendLine($"\nRow: {y++}\n");
                foreach (var entry in row)
                {
                    sb.AppendLine(entry.ToString());
                }
            }
            return sb.ToString();
        }
    }
}