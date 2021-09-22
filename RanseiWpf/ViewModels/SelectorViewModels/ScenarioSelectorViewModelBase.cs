
namespace RanseiWpf.ViewModels
{
    public abstract class ScenarioSelectorViewModelBase<TModel, TViewModel> : ViewModelBase, ISaveable where TViewModel : IViewModelForModel<TModel>, new()
    {
        public ScenarioSelectorViewModelBase(uint minIndex, uint maxIndex)
        {
            MinIndex = minIndex;
            MaxIndex = maxIndex;
        }

        protected void Init()
        {
            _selectedScenario = 0;
            _selectedItem = 0;
            TModel model = RetrieveModel(SelectedScenario, SelectedItem);
            NestedViewModel = new TViewModel() { Model = model };
        }


        private TViewModel _nestedViewModel;
        public TViewModel NestedViewModel
        {
            get => _nestedViewModel;
            set => RaiseAndSetIfChanged(ref _nestedViewModel, value);
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
                NestedViewModel = new TViewModel() { Model = model };
                _selectedItem = value;
            }
        }

        private uint _selectedScenario;
        public uint SelectedScenario
        {
            get => _selectedScenario;
            set
            {
                Save();
                TModel model = RetrieveModel(value, SelectedItem);
                NestedViewModel = new TViewModel() { Model = model };
                _selectedScenario = value;
            }
        }

        protected abstract TModel RetrieveModel(uint scenario, uint index);
        protected abstract void SaveModel(uint scenario, uint index, TModel model);

        public virtual void Save()
        {
            if (NestedViewModel != null)
            {
                SaveModel(SelectedScenario, SelectedItem, NestedViewModel.Model);
            }
        }
    }
}
