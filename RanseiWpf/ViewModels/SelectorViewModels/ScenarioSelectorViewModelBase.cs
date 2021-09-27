using Core;
using Core.Enums;
using System;
using System.Linq;

namespace RanseiWpf.ViewModels
{
    public abstract class ScenarioSelectorViewModelBase<TModel, TViewModel> : ViewModelBase, ISaveableRefreshable where TViewModel : IViewModelForModel<TModel>
    {
        public ScenarioSelectorViewModelBase(Func<ScenarioId, TViewModel> newViewModel, uint minIndex, uint maxIndex)
        {
            NewViewModel = newViewModel;
            MinIndex = minIndex;
            MaxIndex = maxIndex;
        }

        private readonly Func<ScenarioId, TViewModel> NewViewModel;

        protected void Init()
        {
            _selectedScenario = ScenarioId.TheLegendOfRansei;
            _selectedItem = 0;
            TModel model = RetrieveModel(SelectedScenario, SelectedItem);
            var vm = NewViewModel(SelectedScenario);
            vm.Model = model;
            NestedViewModel = vm;
        }


        private TViewModel _nestedViewModel;
        public TViewModel NestedViewModel
        {
            get => _nestedViewModel;
            set
            {
                if (!value.Equals(_nestedViewModel))
                {
                    if (_nestedViewModel is ISaveable saveable)
                    {
                        saveable.Save();
                    }
                    _nestedViewModel = value;
                    RaisePropertyChanged();
                }
            }
        }

        public uint MinIndex { get; }
        public uint MaxIndex { get; }

        private uint _selectedItem;
        public uint SelectedItem
        {
            get => _selectedItem;
            set
            {
                Save();
                TModel model = RetrieveModel(SelectedScenario, value);
                var vm = NewViewModel(SelectedScenario);
                vm.Model = model;
                NestedViewModel = vm;
                _selectedItem = value;
            }
        }

        public ScenarioId[] ScenarioItems { get; } = EnumUtil.GetValues<ScenarioId>().ToArray();

        private ScenarioId _selectedScenario;
        public ScenarioId SelectedScenario
        {
            get => _selectedScenario;
            set
            {
                Save();
                TModel model = RetrieveModel(value, SelectedItem);
                var vm = NewViewModel(value);
                vm.Model = model;
                NestedViewModel = vm;
                _selectedScenario = value;
            }
        }

        protected abstract TModel RetrieveModel(ScenarioId scenario, uint index);
        protected abstract void SaveModel(ScenarioId scenario, uint index, TModel model);

        public virtual void Save()
        {
            if (NestedViewModel != null)
            {
                if (_nestedViewModel is ISaveable saveable)
                {
                    saveable.Save();
                }
                SaveModel(SelectedScenario, SelectedItem, NestedViewModel.Model);
            }
        }

        public void Refresh()
        {
            TModel model = RetrieveModel(SelectedScenario, SelectedItem);
            var vm = NewViewModel(SelectedScenario);
            vm.Model = model;
            NestedViewModel = vm;
        }
    }
}
