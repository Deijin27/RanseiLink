using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using System;
using System.Linq;

namespace RandomizerPlugin;


[Plugin("Randomizer", "Deijin", "1.0")]
public class RandomizerPlugin : IPlugin
{
    public void Run(IPluginContext context)
    {
        var optionService = context.ServiceContainer.Resolve<IPluginService>();
        var options = new RandomizationOptionForm();
        if (!optionService.RequestOptions(options))
        {
            return;
        }

        IDataService dataService = context.ServiceContainer.Resolve<DataServiceFactory>()(context.ActiveMod);

        PokemonId[] pokemonIds = EnumUtil.GetValuesExceptDefaults<PokemonId>().ToArray();
        AbilityId[] abilityIds = EnumUtil.GetValues<AbilityId>().ToArray();
        TypeId[] typeIds = EnumUtil.GetValues<TypeId>().ToArray();
        MoveId[] moveIds = EnumUtil.GetValues<MoveId>().ToArray();
        WarriorId[] warriorIds = EnumUtil.GetValuesExceptDefaults<WarriorId>().ToArray();
        ScenarioId[] scenarioIds = EnumUtil.GetValues<ScenarioId>().ToArray();

        if (options.AvoidDummys)
        {
            abilityIds = abilityIds.Where(i => !i.ToString().StartsWith("dummy")).ToArray();
            moveIds = moveIds.Where(i => !i.ToString().StartsWith("dummy")).ToArray();
        }

        var random = new Random(options.Seed.GetHashCode());

        using var pokemonService = dataService.Pokemon.Disposable();

        if (options.Abilities || options.Types || options.Moves)
        {
            foreach (var pid in pokemonIds)
            {
                var poke = pokemonService.Retrieve(pid);

                if (options.Abilities)
                {
                    poke.Ability1 = random.Choice(abilityIds);
                    poke.Ability2 = random.Choice(abilityIds);
                    poke.Ability3 = random.Choice(abilityIds);
                }
                if (options.Types)
                {
                    poke.Type1 = random.Choice(typeIds);
                    poke.Type2 = options.PreventSameType
                        ? random.Choice(typeIds.Where(i => i != poke.Type1).ToArray())
                        : random.Choice(typeIds);

                }
                if (options.Moves)
                {
                    poke.Move = random.Choice(moveIds);
                }
                pokemonService.Save(pid, poke);
            }
        }

        if (options.ScenarioPokemon)
        {
            using var scenarioPokemonService = dataService.ScenarioPokemon.Disposable();

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

        if (options.AllMaxLinkValue > 0)
        {
            using var maxLinkService = dataService.MaxLink.Disposable();

            foreach (WarriorId wid in warriorIds)
            {
                var maxLinkTable = maxLinkService.Retrieve(wid);
                foreach (PokemonId pid in pokemonIds)
                {
                    if (maxLinkTable.GetMaxLink(pid) < options.AllMaxLinkValue)
                    {
                        maxLinkTable.SetMaxLink(pid, options.AllMaxLinkValue);
                    }
                }
                maxLinkService.Save(wid, maxLinkTable);
            }
        }
    }
}

