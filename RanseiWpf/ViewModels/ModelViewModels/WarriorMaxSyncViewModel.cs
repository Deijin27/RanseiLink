using Core;
using Core.Enums;
using Core.Models.Interfaces;
using System.Collections.Generic;

namespace RanseiWpf.ViewModels
{
    public class WarriorMaxSyncListItem
    {
        public uint MaxSyncValue { get; set; }
        public PokemonId Pokemon { get; set; }
    }
    public class WarriorMaxSyncViewModel : ViewModelBase, IViewModelForModel<IWarriorMaxLink>
    {
        private IWarriorMaxLink _model;
        public IWarriorMaxLink Model
        {
            get
            {
                foreach (var i in Items)
                {
                    _model.SetMaxLink(i.Pokemon, i.MaxSyncValue);
                }
                return _model;
            }
            set
            {
                _model = value;
                var items = new List<WarriorMaxSyncListItem>();
                foreach (PokemonId pid in EnumUtil.GetValues<PokemonId>())
                {
                    items.Add(new WarriorMaxSyncListItem()
                    {
                        MaxSyncValue = value.GetMaxLink(pid),
                        Pokemon = pid
                    });
                }
                Items = items;
            }
        }

        private IList<WarriorMaxSyncListItem> _items;
        public IList<WarriorMaxSyncListItem> Items
        {
            get => _items;
            set => RaiseAndSetIfChanged(ref _items, value);
        }
    }
}
