#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Services;
using System.Text.RegularExpressions;

namespace RanseiLink.GuiCore.ViewModels;

public partial class BattleConfigViewModel : ViewModelBase
{
    private readonly IAsyncDialogService _dialogService;
    private readonly IOverrideDataProvider _overrideDataProvider;
    private readonly IMapViewerService _mapViewerService;

    public BattleConfigViewModel(
        IMapService mapService, 
        IJumpService jumpService, 
        IIdToNameService idToNameService,
        IAsyncDialogService dialogService,
        IOverrideDataProvider overrideDataProvider,
        IMapViewerService mapViewerService)
    {
        _dialogService = dialogService;
        _overrideDataProvider = overrideDataProvider;
        _mapViewerService = mapViewerService;
        MapItems = mapService.GetMapIds();

        ItemItems = idToNameService.GetComboBoxItemsPlusDefault<IItemService>();

        JumpToMapCommand = new RelayCommand<MapId>(id => jumpService.JumpTo(MapSelectorEditorModule.Id, (int)id));
        JumpToItemCommand = new RelayCommand<int>(id => jumpService.JumpTo(ItemSelectorEditorModule.Id, id));
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

    private void BattleConfigViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Minimap):
            case nameof(MinimapVariant):
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
        RaiseAllPropertiesChanged();
    }

    public ICommand View3DModelCommand { get; }

    public MapId MapId
    {
        get => _model.MapId;
        set => SetProperty(_model.MapId, value, v => _model.MapId = value);
    }

    public string? MinimapSpritePath 
    { 
        get 
        {
            var idx = Minimaps.FindIndex(x => x.Minimap == Minimap && x.MinimapVariant == MinimapVariant);
            if (idx < 0)
            {
                return null;
            }
            return _overrideDataProvider.GetSpriteFile(SpriteType.Minimap, idx).File; 
        } 
    }

    #region Victory Conditions

    public bool VictoryCondition_Unknown_Aurora
    {
        get => (_model.VictoryCondition & BattleVictoryConditionFlags.Unknown_AuroraDragnor) != 0;
        set => SetProperty(VictoryCondition_Unknown_Aurora, value, v => _model.VictoryCondition ^= BattleVictoryConditionFlags.Unknown_AuroraDragnor);
    }

    public bool VictoryCondition_Unknown_ViperiaDragnor
    {
        get => (_model.VictoryCondition & BattleVictoryConditionFlags.Unknown_ViperiaDragnor) != 0;
        set => SetProperty(VictoryCondition_Unknown_ViperiaDragnor, value, v => _model.VictoryCondition ^= BattleVictoryConditionFlags.Unknown_ViperiaDragnor);
    }

    public bool VictoryCondition_Unknown_Greenleaf
    {
        get => (_model.VictoryCondition & BattleVictoryConditionFlags.Unknown_Greenleaf) != 0;
        set => SetProperty(VictoryCondition_Unknown_Greenleaf, value, v => _model.VictoryCondition ^= BattleVictoryConditionFlags.Unknown_Greenleaf);
    }

    public bool VictoryCondition_HoldAllBannersFor5Turns
    {
        get => (_model.VictoryCondition & BattleVictoryConditionFlags.HoldAllBannersFor5Turns) != 0;
        set => SetProperty(VictoryCondition_HoldAllBannersFor5Turns, value, v => _model.VictoryCondition ^= BattleVictoryConditionFlags.HoldAllBannersFor5Turns);
    }

    public bool VictoryCondition_ClaimAllBanners
    {
        get => (_model.VictoryCondition & BattleVictoryConditionFlags.ClaimAllBanners) != 0;
        set => SetProperty(VictoryCondition_ClaimAllBanners, value, v => _model.VictoryCondition ^= BattleVictoryConditionFlags.ClaimAllBanners);
    }

    #endregion

    #region Defeat Conditions

    public bool DefeatCondition_Unknown_Aurora
    {
        get => (_model.DefeatCondition & BattleVictoryConditionFlags.Unknown_AuroraDragnor) != 0;
        set => SetProperty(VictoryCondition_Unknown_Aurora, value, v => _model.DefeatCondition ^= BattleVictoryConditionFlags.Unknown_AuroraDragnor);
    }

    public bool DefeatCondition_Unknown_ViperiaDragnor
    {
        get => (_model.DefeatCondition & BattleVictoryConditionFlags.Unknown_ViperiaDragnor) != 0;
        set => SetProperty(DefeatCondition_Unknown_ViperiaDragnor, value, v => _model.DefeatCondition ^= BattleVictoryConditionFlags.Unknown_ViperiaDragnor);
    }

    public bool DefeatCondition_Unknown_Greenleaf
    {
        get => (_model.DefeatCondition & BattleVictoryConditionFlags.Unknown_Greenleaf) != 0;
        set => SetProperty(DefeatCondition_Unknown_Greenleaf, value, v => _model.DefeatCondition ^= BattleVictoryConditionFlags.Unknown_Greenleaf);
    }

    public bool DefeatCondition_HoldAllBannersFor5Turns
    {
        get => (_model.DefeatCondition & BattleVictoryConditionFlags.HoldAllBannersFor5Turns) != 0;
        set => SetProperty(DefeatCondition_HoldAllBannersFor5Turns, value, v => _model.DefeatCondition ^= BattleVictoryConditionFlags.HoldAllBannersFor5Turns);
    }

    public bool DefeatCondition_ClaimAllBanners
    {
        get => (_model.DefeatCondition & BattleVictoryConditionFlags.ClaimAllBanners) != 0;
        set => SetProperty(DefeatCondition_ClaimAllBanners, value, v => _model.DefeatCondition ^= BattleVictoryConditionFlags.ClaimAllBanners);
    }

    #endregion


    public ICollection<MapId> MapItems { get; }

    public ICommand JumpToMapCommand { get; }
    public ICommand JumpToItemCommand { get; }

    public async void View3DModel()
    {
        await _mapViewerService.ShowDialog(_id);
    }
}
