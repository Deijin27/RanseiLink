using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Services.ModelServices;

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
        private readonly IPokemonService DataService;
        private IEvolutionTable evolutionTable;

        public EvolutionTableViewModel(IPokemonService dataService)
        {
            DataService = dataService;
            Refresh();
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
            DataService.SaveEvolutionTable(evolutionTable);
        }

        public void Refresh()
        {
            evolutionTable = DataService.RetrieveEvolutionTable();
            var lst = new List<EvolutionTableItem>();
            for (int i = 0; i < EvolutionTable.DataLength; i++)
            {
                lst.Add(new EvolutionTableItem(i, evolutionTable.GetEntry(i), PokemonItems));
            }
            Items = lst;
        }
    }
}
