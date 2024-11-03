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
    public byte Unknown1 { get; set; }
    public byte Unknown2 { get; set; }
    public MapPokemonItem(Position position, byte unknown1 = 1, byte unknown2 = 2)
    {
        Position = position;
        Unknown1 = unknown1;
        Unknown2 = unknown2;
    }
    public MapPokemonItem(BinaryReader br) : this(new Position(br), br.ReadByte(), br.ReadByte())
    {
    }

    public void WriteTo(BinaryWriter bw)
    {
        Position.WriteTo(bw);
        bw.Write(Unknown1);
        bw.Write(Unknown2);
    }
}