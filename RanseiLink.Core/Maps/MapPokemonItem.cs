namespace RanseiLink.Core.Maps;

public class MapPokemonItem
{
    public byte X
    {
        get => Position.X;
        set => Position.X = value;
    }

    public byte Y
    {
        get => Position.Y;
        set => Position.Y = value;
    }

    public Position Position { get; set; }
    public OrientationAlt Orientation { get; set; }
    public byte Unknown2 { get; set; }
    public MapPokemonItem(Position position, OrientationAlt orientation = OrientationAlt.West, byte unknown2 = 2)
    {
        Position = position;
        Orientation = orientation;
        Unknown2 = unknown2;
    }
    public MapPokemonItem(BinaryReader br) : this(new Position(br), (OrientationAlt)br.ReadByte(), br.ReadByte())
    {
    }

    public void WriteTo(BinaryWriter bw)
    {
        Position.WriteTo(bw);
        bw.Write((byte)Orientation);
        bw.Write(Unknown2);
    }
}