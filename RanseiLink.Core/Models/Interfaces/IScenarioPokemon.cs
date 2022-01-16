using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces;

public interface IScenarioPokemon : IDataWrapper, ICloneable<IScenarioPokemon>
{
    PokemonId Pokemon { get; set; }
    AbilityId Ability { get; set; }
    uint HpIv { get; set; }
    uint AtkIv { get; set; }
    uint DefIv { get; set; }
    uint SpeIv { get; set; }
    ushort Exp { get; set; }
}
