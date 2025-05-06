using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public class WorkspaceViewModel : ViewModelBase
{
    private IMiniViewModel? _selectedMiniVm;
    private readonly IModelService _modelService;
    private readonly List<IMiniViewModel> _allMiniVms = [];
    private int _selectedId;

    public WorkspaceViewModel(
        CopyPasteViewModel copyPasteVm,
        IBigViewModel bigViewModel,
        IModelService service,
        Func<ICommand, List<IMiniViewModel>> allMiniVms)
    {
        BigViewModel = bigViewModel;
        _modelService = service;
        var selectItemCommand = new RelayCommand<IMiniViewModel>(wa => { if (wa != null) NavigateToId(wa.Id); });

        _allMiniVms = allMiniVms(selectItemCommand);
        Items = new(_allMiniVms);

        CopyPasteVisible = service != null && service.RetrieveObject(0) is IDataWrapper;
        CopyPasteVm = copyPasteVm;

        SelectById(0);

        BigViewModel.PropertyChanged += BigViewModelPropertyChanged;

        
        CopyPasteVm.ModelPasted += CopyPasteVm_ModelPasted;
    }

    public event EventHandler<int>? RequestNavigateToId;

    private void NavigateToId(int id)
    {
        RequestNavigateToId?.Invoke(this, id);
    }

    public int SelectedId => _selectedId;

    private void CopyPasteVm_ModelPasted(object? sender, EventArgs e)
    {
        SelectById(_selectedId);
    }

    public bool CopyPasteVisible { get; }


    public CopyPasteViewModel CopyPasteVm { get; }

    private bool _ignorePropertyChanged;

    private void BigViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (_ignorePropertyChanged)
        {
            return;
        }
        // keep the mini views up to date with the data as the user is changing it
        SelectedMiniVm?.NotifyPropertyChanged(e.PropertyName);
    }

    public void SelectById(int id)
    {
        if (!(_modelService.ValidateId(id)))
        {
            return;
        }

        _ignorePropertyChanged = true;
        try
        {
            _selectedId = id;
            var model = _modelService.RetrieveObject(id);
            var newSelection = _allMiniVms[id];
            SelectedMiniVm = _allMiniVms.Find(x => x.Id == id);
            BigViewModel.SetModel(id, model);
            CopyPasteVm.Model = model as IDataWrapper;
        }
        finally
        {
            _ignorePropertyChanged = false;
        }
    }

    private string? _searchText;
    public string? SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                Search();
            }
        }
    }

    private void Search()
    {
        Items.Clear();

        if (string.IsNullOrWhiteSpace(_searchText))
        {
            foreach (var item in _allMiniVms)
            {
                Items.Add(item);
            }
            return;
        }

        var searchTerms = _searchText.Split();

        foreach (var item in _allMiniVms)
        {
            bool match = false;
            foreach (var term in searchTerms)
            {
                match = item.MatchSearchTerm(term);

                // must match all terms
                if (!match)
                {
                    break;
                }
            }

            if (match)
            {
                Items.Add(item);
            }
        }

        if (Items.Count == 1)
        {
            SelectById(Items[0].Id);
        }
    }

    public IMiniViewModel? SelectedMiniVm
    {
        get => _selectedMiniVm;
        set => SetProperty(ref _selectedMiniVm, value);
    }

    public IBigViewModel BigViewModel { get; }

    public ObservableCollection<IMiniViewModel> Items { get; }

    public int LeftColumnWidth { get; set; } = 196;
}
