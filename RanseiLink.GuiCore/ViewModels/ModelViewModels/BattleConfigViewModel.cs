#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Services;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using RanseiLink.Core;

namespace RanseiLink.GuiCore.ViewModels;

public partial class BattleConfigViewModel : ViewModelBase, IBigViewModel
{
    private readonly IAsyncDialogService _dialogService;
    private readonly IOverrideDataProvider _overrideDataProvider;
    private readonly IMapViewerService _mapViewerService;
    private readonly INicknameService _nicknameService;

    public BattleConfigViewModel(
        IMapService mapService, 
        IJumpService jumpService, 
        IIdToNameService idToNameService,
        IAsyncDialogService dialogService,
        IOverrideDataProvider overrideDataProvider,
        IMapViewerService mapViewerService,
        INicknameService nicknameService)
    {
        _dialogService = dialogService;
        _overrideDataProvider = overrideDataProvider;
        _mapViewerService = mapViewerService;
        _nicknameService = nicknameService;

        MapItems = mapService.GetMapIds().Select(i => (SelectorComboBoxItem)new NicknamedSelectorComboBoxItem(
            id: (int)i,
            nicknameService,
            nameof(MapId),
            idString: i.ToString()[3..]
            )).ToList();

        ItemItems = idToNameService.GetComboBoxItemsPlusDefault<IItemService>();

        JumpToMapCommand = new RelayCommand<int>(id => jumpService.JumpTo(MapSelectorEditorModule.Id, id));
        JumpToItemCommand = new RelayCommand<int>(id => jumpService.JumpTo(ItemWorkspaceModule.Id, id));
        View3DModelCommand = new RelayCommand(View3DModel);

        Minimaps = [];
        
        foreach (var spriteFile in _overrideDataProvider.GetAllSpriteFiles(SpriteType.Minimap))
        {
            var match = Regex.Match(spriteFile.File, @"minimap(\d\d)_(\d\d)");
            if (match.Success)
            {
                Minimaps.Add(new MinimapInfo(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)));
            }
            else
            {
                throw new System.Exception("Minimap file unexpected path");
            }
        }

        this.PropertyChanged += BattleConfigViewModel_PropertyChanged;
    }

    public string Nickname
    {
        get => _nicknameService.GetNickname(nameof(BattleConfigId), (int)_id);
        set
        {
            if (Nickname != value)
            {
                _nicknameService.SetNickname(nameof(BattleConfigId), (int)_id, value);
                RaisePropertyChanged();
            }
        }
    }

    private void BattleConfigViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Minimap):
                RaisePropertyChanged(nameof(MinimapSpritePath));
                break;
        }
    }

    public record MinimapInfo(int Minimap, int MinimapVariant);

    public List<MinimapInfo> Minimaps { get; }

    public void SetModel(BattleConfigId id, BattleConfig model)
    {
        _id = id;
        _model = model;

        DefeatConditionItems.Clear();
        VictoryConditionItems.Clear();
        foreach (var (item, name) in EnumUtil.GetValuesWithNames<BattleVictoryConditionFlags>())
        {
            if (item != 0)
            {
                DefeatConditionItems.Add(new CheckBoxViewModel(name,
                     () => (DefeatCondition & item) != 0,
                     v => DefeatCondition ^= item
                ));
                VictoryConditionItems.Add(new CheckBoxViewModel(name,
                     () => (VictoryCondition & item) != 0,
                     v => VictoryCondition ^= item
                ));
            }
        }

        RaiseAllPropertiesChanged();
    }

    public ICommand View3DModelCommand { get; }

    public int MapId
    {
        get => (int)_model.MapId;
        set => SetProperty(_model.MapId, (MapId)value, v => _model.MapId = v);
    }

    public string? MinimapSpritePath 
    { 
        get 
        {
            var idx = Minimaps.FindIndex(x => x.Minimap == Minimap && x.MinimapVariant == 0);
            if (idx < 0)
            {
                return null;
            }
            return _overrideDataProvider.GetSpriteFile(SpriteType.Minimap, idx).File; 
        } 
    }

    public ObservableCollection<CheckBoxViewModel> DefeatConditionItems { get; } = [];
    public ObservableCollection<CheckBoxViewModel> VictoryConditionItems { get; } = [];

    public List<SelectorComboBoxItem> MapItems { get; }

    public ICommand JumpToMapCommand { get; }
    public ICommand JumpToItemCommand { get; }

    public async void View3DModel()
    {
        await _mapViewerService.ShowDialog(_id);
    }

    public void SetModel(int id, object model)
    {
        SetModel((BattleConfigId)id, (BattleConfig)model);
    }
}
