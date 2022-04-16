using System.IO;

namespace RanseiLink.Core.Maps
{
    public class MapPokemonPositionSection
    {
        public Position[] Positions { get; }

        private const int PositionsCount = 64;
        public MapPokemonPositionSection(BinaryReader br)
        {
            Positions = new Position[PositionsCount];
            for (int i = 0; i < PositionsCount; i++)
            {
                Positions[i] = new Position(br);
            }
        }

        public void WriteTo(BinaryWriter bw)
        {
            foreach (var pos in Positions)
            {
                pos.WriteTo(bw);
            }
        }
    }
}