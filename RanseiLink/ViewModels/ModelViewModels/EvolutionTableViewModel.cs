using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;
using System.Collections.Generic;
using System.Linq;
using RanseiLink.Core;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Services;
using System;
using System.Windows.Input;
using System.Windows;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate EvolutionTableViewModel EvolutionTableViewModelFactory(IEditorContext contextualPokemonService);

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
    private readonly IPokemonService _dataService;
    private IEvolutionTable _model;

    public EvolutionTableViewModel(IServiceContainer container, IEditorContext context)
    {
        _dialogService = container.Resolve<IDialogService>();
        _dataService = context.DataService.Pokemon;
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
            _dataService.SaveEvolutionTable(_model);
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                type: MessageBoxType.Error,
                title: $"Error saving data in {GetType().Name}",
                message: e.ToString()
            ));
        }
    }

    public void Refresh()
    {
        try
        {
            _model = _dataService.RetrieveEvolutionTable();
            var lst = new List<EvolutionTableItem>();
            for (int i = 0; i < EvolutionTable.DataLength; i++)
            {
                lst.Add(new EvolutionTableItem(i, _model.GetEntry(i), PokemonItems));
            }
            Items = lst;
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                type: MessageBoxType.Error,
                title: $"Error retrieving data in {GetType().Name}",
                message: e.ToString()
            ));
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
            _model = _dataService.RetrieveEvolutionTable();
            var lst = new List<EvolutionTableItem>();
            for (int i = 0; i < EvolutionTable.DataLength; i++)
            {
                lst.Add(new EvolutionTableItem(i, _model.GetEntry(i), PokemonItems));
            }
            Items = lst;
        }
        else
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                type: MessageBoxType.Warning,
                title: "Invalid Paste Data",
                message: "The data that you pasted is invalid. Make sure you have the right label and length." +
                          $"\n\nYou pasted:\n\n{text}\n\nWhat was expected was something like:\n\n{_model.Serialize()}"
            ));
        }
    }
}
