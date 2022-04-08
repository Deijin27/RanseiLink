using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public class ScenarioPokemon : BaseDataWindow
{
    public const int DataLength = 8;

    public ScenarioPokemon(byte[] data) : base(data, DataLength) { }
    public ScenarioPokemon() : base(new byte[DataLength], DataLength) { }

    public PokemonId Pokemon
    {
        get
        {
            byte value = GetByte(0);
            return value == 200 ? PokemonId.Default : (PokemonId)value;
        }
        set
        {
            byte byteVal = value == PokemonId.Default ? (byte)200 : (byte)value;
            SetByte(0, byteVal);
        }
    }

    public AbilityId Ability
    {
        get => (AbilityId)GetUInt32(1, 20, 8);
        set => SetUInt32(1, 20, 8, (uint)value);
    }

    public uint HpIv
    {
        get => GetUInt32(1, 0, 5);
        set => SetUInt32(1, 0, 5, value);
    }

    public uint AtkIv
    {
        get => GetUInt32(1, 5, 5);
        set => SetUInt32(1, 5, 5, value);
    }

    public uint DefIv
    {
        get => GetUInt32(1, 10, 5);
        set => SetUInt32(1, 10, 5, value);
    }

    public uint SpeIv
    {
        get => GetUInt32(1, 15, 5);
        set => SetUInt32(1, 15, 5, value);
    }

    public ushort Exp
    {
        get => GetUInt16(2);
        set => SetUInt16(2, value);
    }

}
