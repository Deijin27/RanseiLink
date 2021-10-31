using RanseiLink.Core.Models;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System;
using System.Collections.Generic;

namespace RanseiLink.ViewModels
{
    public class WarriorNameTableItem : ViewModelBase
    {
        private readonly IWarriorNameTable _table;
        public WarriorNameTableItem(uint index, IWarriorNameTable table)
        {
            Index = index;
            _table = table;
        }
        public uint Index { get; }
        
        public string Name
        {
            get => _table.GetEntry(Index);
            set => RaiseAndSetIfChanged(Name, value, v => _table.SetEntry(Index, v));
        }
    }
    public class WarriorNameTableViewModel : ViewModelBase, ISaveableRefreshable
    {
        private readonly IDialogService _dialogService; 
        private readonly IBaseWarriorService _service;
        private IWarriorNameTable _model;

        public WarriorNameTableViewModel(IDialogService dialogService, IBaseWarriorService dataService)
        {
            _dialogService = dialogService;
            _service = dataService;
            Refresh();
        }

        public IReadOnlyList<WarriorNameTableItem> Items { get; private set; }

        public void Save()
        {
            try
            {
                _service.SaveNameTable(_model);
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
                _model = _service.RetrieveNameTable();
                var lst = new List<WarriorNameTableItem>();
                for (uint i = 0; i < WarriorNameTable.EntryCount; i++)
                {
                    lst.Add(new WarriorNameTableItem(i, _model));
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
