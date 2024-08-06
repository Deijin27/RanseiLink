namespace RanseiLink.Core.Maps;

public class MapPokemonPositionSection
{
    public Position[] Positions { get; }

    private const int __positionsCount = 64;
    public MapPokemonPositionSection(BinaryReader br)
    {
        Positions = new Position[__positionsCount];
        for (int i = 0; i < __positionsCount; i++)
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