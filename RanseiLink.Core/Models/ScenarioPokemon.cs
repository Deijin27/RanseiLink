using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public partial class ScenarioPokemon : BaseDataWindow
{
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
}