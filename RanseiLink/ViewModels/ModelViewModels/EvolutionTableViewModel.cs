using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;
using System.Collections.Generic;
using System.Linq;
using RanseiLink.Core;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System;

namespace RanseiLink.ViewModels
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
        private readonly IDialogService _dialogService;
        private readonly IPokemonService DataService;
        private IEvolutionTable evolutionTable;

        public EvolutionTableViewModel(IDialogService dialogService, IPokemonService dataService)
        {
            _dialogService = dialogService;
            DataService = dataService;
            Refresh();
        }

        public IEvolutionTable Model { get; set; }

        public IReadOnlyList<EvolutionTableItem> Items { get; private set; }

        public PokemonId[] PokemonItems { get; } = EnumUtil.GetValuesExceptDefaults<PokemonId>().ToArray();

        public void Save()
        {
            foreach (var item in Items)
            {
                evolutionTable.SetEntry(item.Index, item.Pokemon);
            }
            try
            {
                DataService.SaveEvolutionTable(evolutionTable);
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(new MessageBoxArgs()
                {
                    Icon = System.Windows.MessageBoxImage.Error,
                    Title = $"Error saving data in {GetType().Name}",
                    Message = e.Message
                });
            }
        }

        public void Refresh()
        {
            try
            {
                evolutionTable = DataService.RetrieveEvolutionTable();
                var lst = new List<EvolutionTableItem>();
                for (int i = 0; i < EvolutionTable.DataLength; i++)
                {
                    lst.Add(new EvolutionTableItem(i, evolutionTable.GetEntry(i), PokemonItems));
                }
                Items = lst;
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(new MessageBoxArgs()
                {
                    Icon = System.Windows.MessageBoxImage.Error,
                    Title = $"Error retrieving data in {GetType().Name}",
                    Message = e.Message
                });
            }
        }
    }
}
