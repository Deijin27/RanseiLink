using Core.Enums;
using Core.Services;
using System;
using System.Linq;

namespace Core.Randomization
{
    public class SimpleRandomizer : RandomizerBase
    {
        private static class Option
        {
            public const string ScenarioPokemon = "ScenarioPokemon";
            public const string Abilities = "PokemonAbilities";
            public const string Types = "PokemonTypes";
            public const string Moves = "PokemonMoves";
            public const string AllMaxLink98 = "AllMaxLink98";
            public const string AvoidDummys = "AvoidDummys";
            public const string PreventSameType = "PreventSameType";
            public const string EnsureMaxLink100 = "EnsureMaxLink100";
        }
        public SimpleRandomizer() : base("Randomize")
        {
            AddOption(Option.ScenarioPokemon, "Warrior's Pokemon");
            AddOption(Option.Abilities, "Pokemon's Abilities");
            AddOption(Option.Types, "Pokemon's Types");
            AddOption(Option.PreventSameType, "Prevent two of the same type on a pokemon");
            AddOption(Option.Moves, "Pokemon's Moves");
            AddOption(Option.AllMaxLink98, "Set All Max Links to at least 98");
            AddOption(Option.AvoidDummys, "Avoid Dummys", "Avoid dummy moves, abilities, etc when randomizing.");
        }

        public override void Apply(IDataService service)
        {
            PokemonId[] pokemonIds = EnumUtil.GetValues<PokemonId>().ToArray();
            AbilityId[] abilityIds = EnumUtil.GetValues<AbilityId>().ToArray();
            TypeId[] typeIds = EnumUtil.GetValues<TypeId>().ToArray();
            MoveId[] moveIds = EnumUtil.GetValues<MoveId>().ToArray();
            WarriorId[] warriorIds = EnumUtil.GetValues<WarriorId>().ToArray();
            ScenarioId[] scenarioIds = EnumUtil.GetValues<ScenarioId>().ToArray();

            if (OptionDict[Option.AvoidDummys].Enabled)
            {
                abilityIds = abilityIds.Where(i => !i.ToString().StartsWith("dummy")).ToArray();
                moveIds = moveIds.Where(i => !i.ToString().StartsWith("dummy")).ToArray();
            }

            Random random = new Random();

            using (var pokemonService = service.Pokemon.Disposable())
            {
                if (OptionDict[Option.Abilities].Enabled || OptionDict[Option.Types].Enabled || OptionDict[Option.Moves].Enabled)
                {
                    foreach (var pid in pokemonIds)
                    {
                        var poke = pokemonService.Retrieve(pid);

                        if (OptionDict[Option.Abilities].Enabled)
                        {
                            poke.Ability1 = random.Choice(abilityIds);
                            poke.Ability2 = random.Choice(abilityIds);
                            poke.Ability3 = random.Choice(abilityIds);
                        }
                        if (OptionDict[Option.Types].Enabled)
                        {
                            poke.Type1 = random.Choice(typeIds);
                            poke.Type2 = OptionDict[Option.PreventSameType].Enabled
                                ? random.Choice(typeIds.Where(i => i != poke.Type1).ToArray())
                                : random.Choice(typeIds);

                        }
                        if (OptionDict[Option.Moves].Enabled)
                        {
                            poke.Move = random.Choice(moveIds);
                        }
                        pokemonService.Save(pid, poke);
                    }
                }

                if (OptionDict[Option.ScenarioPokemon].Enabled)
                {
                    using (var scenarioPokemonService = service.ScenarioPokemon.Disposable())
                    {
                        foreach (var i in scenarioIds)
                        {
                            for (int j = 0; j < Constants.ScenarioPokemonCount; j++)
                            {
                                var sp = scenarioPokemonService.Retrieve(i, j);
                                sp.Pokemon = random.Choice(pokemonIds);
                                var pk = pokemonService.Retrieve(sp.Pokemon);
                                sp.Ability = random.Choice(new[] { pk.Ability1, pk.Ability2, pk.Ability3 });
                                scenarioPokemonService.Save(i, j, sp);
                            }
                        }
                    }
                }
            }

            using (var maxLinkService = service.MaxLink.Disposable())
            {
                if (OptionDict[Option.AllMaxLink98].Enabled)
                {
                    foreach (WarriorId wid in warriorIds)
                    {
                        var maxLinkTable = maxLinkService.Retrieve(wid);
                        foreach (PokemonId pid in pokemonIds)
                        {
                            if (maxLinkTable.GetMaxLink(pid) < 98)
                            {
                                maxLinkTable.SetMaxLink(pid, 98);
                            }
                        }
                        maxLinkService.Save(wid, maxLinkTable);
                    }
                }
            }
        }
    }
}
