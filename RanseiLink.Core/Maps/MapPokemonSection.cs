namespace RanseiLink.Core.Maps;

public class MapPokemonSection
{
    public MapPokemonItem[] Positions { get; }

    private const int __positionsCount = 32;
    public MapPokemonSection(BinaryReader br)
    {
        Positions = new MapPokemonItem[__positionsCount];
        for (int i = 0; i < __positionsCount; i++)
        {
            Positions[i] = new MapPokemonItem(br);
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