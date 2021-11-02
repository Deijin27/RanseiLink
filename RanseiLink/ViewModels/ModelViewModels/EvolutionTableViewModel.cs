using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;
using System.Collections.Generic;
using System.Linq;
using RanseiLink.Core;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System;
using System.Windows.Input;
using System.Windows;

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
    public class EvolutionTableViewModel : ViewModelBase, ISaveableRefreshable
    {
        private readonly IDialogService _dialogService;
        private readonly IPokemonService DataService;
        private IEvolutionTable _model;

        public EvolutionTableViewModel(IDialogService dialogService, IPokemonService dataService)
        {
            _dialogService = dialogService;
            DataService = dataService;
            Refresh();

            PasteCommand = new RelayCommand(Paste);
            CopyCommand = new RelayCommand(Copy);
        }

        public IReadOnlyList<EvolutionTableItem> Items { get; private set; }

        public PokemonId[] PokemonItems { get; } = EnumUtil.GetValuesExceptDefaults<PokemonId>().ToArray();

        public void Save()
        {
            foreach (var item in Items)
            {
                _model.SetEntry(item.Index, item.Pokemon);
            }
            try
            {
                DataService.SaveEvolutionTable(_model);
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
                _model = DataService.RetrieveEvolutionTable();
                var lst = new List<EvolutionTableItem>();
                for (int i = 0; i < EvolutionTable.DataLength; i++)
                {
                    lst.Add(new EvolutionTableItem(i, _model.GetEntry(i), PokemonItems));
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

        public ICommand CopyCommand { get; }
        public ICommand PasteCommand { get; }

        private void Copy()
        {
            Clipboard.SetText(_model.Serialize());
        }

        private void Paste()
        {
            string text = Clipboard.GetText();

            if (_model.TryDeserialize(text))
            {
                _model = DataService.RetrieveEvolutionTable();
                var lst = new List<EvolutionTableItem>();
                for (int i = 0; i < EvolutionTable.DataLength; i++)
                {
                    lst.Add(new EvolutionTableItem(i, _model.GetEntry(i), PokemonItems));
                }
                Items = lst;
            }
            else
            {
                _dialogService.ShowMessageBox(new MessageBoxArgs()
                {
                    Icon = MessageBoxImage.Warning,
                    Title = "Invalid Paste Data",
                    Message = "The data that you pasted is invalid. Make sure you have the right label and length." +
                              $"\n\nYou pasted:\n\n{text}\n\nWhat was expected was something like:\n\n{_model.Serialize()}"
                });
            }
        }
    }
}
