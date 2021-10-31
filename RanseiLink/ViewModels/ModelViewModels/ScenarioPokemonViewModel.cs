using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using System.Linq;

namespace RanseiLink.ViewModels
{
    public class ScenarioPokemonViewModel : ViewModelBase, IViewModelForModel<IScenarioPokemon>
    {
        public IScenarioPokemon Model { get; set; }

        public PokemonId[] PokemonItems { get; } = EnumUtil.GetValuesExceptDefaults<PokemonId>().ToArray();
        public AbilityId[] AbilityItems { get; } = EnumUtil.GetValues<AbilityId>().ToArray();
 
        public PokemonId Pokemon
        {
            get => Model.Pokemon;
            set => RaiseAndSetIfChanged(Model.Pokemon, value, v => Model.Pokemon = v);
        }

        public AbilityId Ability
        {
            get => Model.Ability;
            set => RaiseAndSetIfChanged(Model.Ability, value, v => Model.Ability = v);
        }

        public uint HpIv
        {
            get => Model.HpIv;
            set => RaiseAndSetIfChanged(Model.HpIv, value, v => Model.HpIv = v);
        }

        public uint AtkIv
        {
            get => Model.AtkIv;
            set => RaiseAndSetIfChanged(Model.AtkIv, value, v => Model.AtkIv = v);
        }

        public uint DefIv
        {
            get => Model.DefIv;
            set => RaiseAndSetIfChanged(Model.DefIv, value, v => Model.DefIv = v);
        }

        public uint SpeIv
        {
            get => Model.SpeIv;
            set => RaiseAndSetIfChanged(Model.SpeIv, value, v => Model.SpeIv = v);
        }
    }
}
