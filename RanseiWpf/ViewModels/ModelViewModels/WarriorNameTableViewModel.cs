using Core.Models;
using Core.Models.Interfaces;
using Core.Services.ModelServices;
using System.Collections.Generic;

namespace RanseiWpf.ViewModels
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
        private readonly IBaseWarriorService _service;
        private IWarriorNameTable _model;

        public WarriorNameTableViewModel(IBaseWarriorService dataService)
        {
            _service = dataService;
            Refresh();
        }

        public IReadOnlyList<WarriorNameTableItem> Items { get; private set; }

        public void Save()
        {
            _service.SaveNameTable(_model);
        }

        public void Refresh()
        {
            _model = _service.RetrieveNameTable();
            var lst = new List<WarriorNameTableItem>();
            for (uint i = 0; i < WarriorNameTable.EntryCount; i++)
            {
                lst.Add(new WarriorNameTableItem(i, _model));
            }
            Items = lst;
        }
    }
}
