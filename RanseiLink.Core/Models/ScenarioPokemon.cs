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
        get => (AbilityId)GetInt(1, 20, 8);
        set => SetInt(1, 20, 8, (int)value);
    }

    public int HpIv
    {
        get => GetInt(1, 0, 5);
        set => SetInt(1, 0, 5, value);
    }

    public int AtkIv
    {
        get => GetInt(1, 5, 5);
        set => SetInt(1, 5, 5, value);
    }

    public int DefIv
    {
        get => GetInt(1, 10, 5);
        set => SetInt(1, 10, 5, value);
    }

    public int SpeIv
    {
        get => GetInt(1, 15, 5);
        set => SetInt(1, 15, 5, value);
    }

    public ushort Exp
    {
        get => GetUInt16(2);
        set => SetUInt16(2, value);
    }

}
