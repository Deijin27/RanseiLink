using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;
using Core.Services;
using System.Collections.Generic;
using System.Linq;
using Core;

namespace RanseiWpf.ViewModels
{
    public class EvolutionTableItem
    {
        public EvolutionTableItem(int index, PokemonId pokemon, PokemonId[] pokemonItems)
        {
            Index = index;
            Pokemon = pokemon;
            PokemonItems = pokemonItems;
        }
        public int Index { get; }
        public PokemonId Pokemon { get; set; }
        public PokemonId[] PokemonItems { get; }
    }
    public class EvolutionTableViewModel : ViewModelBase, IViewModelForModel<IEvolutionTable>, ISaveableRefreshable
    {
        private readonly IModelDataService<IEvolutionTable> DataService;
        private IEvolutionTable evolutionTable;

        public EvolutionTableViewModel(IModelDataService<IEvolutionTable> dataService)
        {
            DataService = dataService;
            evolutionTable = DataService.Retrieve();
            var lst = new List<EvolutionTableItem>();
            for (int i = 0; i < EvolutionTable.DataLength; i++)
            {
                lst.Add(new EvolutionTableItem(i, evolutionTable.GetEntry(i), PokemonItems));
            }
            Items = lst;
        }

        public IEvolutionTable Model { get; set; }

        public IReadOnlyList<EvolutionTableItem> Items { get; private set; }

        public PokemonId[] PokemonItems { get; } = EnumUtil.GetValues<PokemonId>().ToArray();

        public void Save()
        {
            foreach (var item in Items)
            {
                evolutionTable.SetEntry(item.Index, item.Pokemon);
            }
            DataService.Save(evolutionTable);
        }

        public void Refresh()
        {
            evolutionTable = DataService.Retrieve();
            var lst = new List<EvolutionTableItem>();
            for (int i = 0; i < EvolutionTable.DataLength; i++)
            {
                lst.Add(new EvolutionTableItem(i, evolutionTable.GetEntry(i), PokemonItems));
            }
            Items = lst;
        }
    }
}
