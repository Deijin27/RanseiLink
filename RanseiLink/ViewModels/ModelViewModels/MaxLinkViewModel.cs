﻿using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using System.Collections.Generic;

namespace RanseiLink.ViewModels
{
    public class WarriorMaxSyncListItem : ViewModelBase
    {
        private readonly IMaxLink _model;
        public WarriorMaxSyncListItem(PokemonId pokemon, IMaxLink model)
        {
            _model = model;
            Pokemon = pokemon;
        }
        public uint MaxSyncValue
        {
            get => _model.GetMaxLink(Pokemon);
            set => RaiseAndSetIfChanged(MaxSyncValue, value, v => _model.SetMaxLink(Pokemon, v));
        }
        public PokemonId Pokemon { get; set; }
    }
    public class MaxLinkViewModel : ViewModelBase, IViewModelForModel<IMaxLink>
    {
        private IMaxLink _model;
        public IMaxLink Model
        {
            get => _model;
            set
            {
                _model = value;
                var items = new List<WarriorMaxSyncListItem>();
                foreach (PokemonId pid in EnumUtil.GetValuesExceptDefaults<PokemonId>())
                {
                    items.Add(new WarriorMaxSyncListItem(pid, value));
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