#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Graphics;
using System.Collections.Generic;
using System.Windows.Input;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Windows.Services;
using RanseiLink.Core.Services;
using RanseiLink.Windows.Views.ModelViews.Map;
using System.Security.Cryptography;
using RanseiLink.View3D;

namespace RanseiLink.Windows.ViewModels;

public class BattleConfigViewModel : ViewModelBase
{
    private BattleConfig _model;
    private readonly ISceneRenderer _sceneRenderer;
    private readonly IDialogService _dialogService;

    public BattleConfigViewModel(
        IMapService mapService, 
        IJumpService jumpService, 
        IIdToNameService idToNameService, 
        ISceneRenderer sceneRenderer,
        IDialogService dialogService)
    {
        _model = new BattleConfig();
        _sceneRenderer = sceneRenderer;
        _dialogService = dialogService;
        
        MapItems = mapService.GetMapIds();

        ItemItems = idToNameService.GetComboBoxItemsPlusDefault<IItemService>();

        JumpToMapCommand = new RelayCommand<MapId>(id => jumpService.JumpTo(MapSelectorEditorModule.Id, (int)id));
        JumpToItemCommand = new RelayCommand<int>(id => jumpService.JumpTo(ItemSelectorEditorModule.Id, id));
        View3DModelCommand = new RelayCommand(View3DModel);
    }

    public void SetModel(BattleConfigId id, BattleConfig model)
    {
        Id = id;
        _model = model;
        RaiseAllPropertiesChanged();
    }

    public ICommand View3DModelCommand { get; }

    public BattleConfigId Id { get; private set; }

    public MapId Map
    {
        get => _model.MapId;
        set => RaiseAndSetIfChanged(_model.MapId, value, v => _model.MapId = value);
    }

    public int Minimap
    {
        get => _model.Minimap;
        set => RaiseAndSetIfChanged(_model.Minimap, value, v => _model.Minimap = value);
    }

    public int MinimapVariant
    {
        get => _model.MinimapVariant;
        set => RaiseAndSetIfChanged(_model.MinimapVariant, value, v => _model.MinimapVariant = value);
    }

    public int Unknown
    {
        get => _model.Unknown;
        set => RaiseAndSetIfChanged(_model.Unknown, value, v => _model.Unknown = v);
    }

    public int NumberOfTurns
    {
        get => _model.NumberOfTurns;
        set => RaiseAndSetIfChanged(_model.NumberOfTurns, value, v => _model.NumberOfTurns = v);
    }

    public Rgb15 UpperAtmosphereColor
    {
        get => _model.UpperAtmosphereColor;
        set => RaiseAndSetIfChanged(_model.UpperAtmosphereColor, value, v => _model.UpperAtmosphereColor = v);
    }

    public Rgb15 MiddleAtmosphereColor
    {
        get => _model.MiddleAtmosphereColor;
        set => RaiseAndSetIfChanged(_model.MiddleAtmosphereColor, value, v => _model.MiddleAtmosphereColor = v);
    }

    public Rgb15 LowerAtmosphereColor
    {
        get => _model.LowerAtmosphereColor;
        set => RaiseAndSetIfChanged(_model.LowerAtmosphereColor, value, v => _model.LowerAtmosphereColor = v);
    }

    #region Victory Conditions

    public bool VictoryCondition_Unknown_Aurora
    {
        get => (_model.VictoryCondition & BattleVictoryConditionFlags.Unknown_AuroraDragnor) != 0;
        set => RaiseAndSetIfChanged(VictoryCondition_Unknown_Aurora, value, v => _model.VictoryCondition ^= BattleVictoryConditionFlags.Unknown_AuroraDragnor);
    }

    public bool VictoryCondition_Unknown_ViperiaDragnor
    {
        get => (_model.VictoryCondition & BattleVictoryConditionFlags.Unknown_ViperiaDragnor) != 0;
        set => RaiseAndSetIfChanged(VictoryCondition_Unknown_ViperiaDragnor, value, v => _model.VictoryCondition ^= BattleVictoryConditionFlags.Unknown_ViperiaDragnor);
    }

    public bool VictoryCondition_Unknown_Greenleaf
    {
        get => (_model.VictoryCondition & BattleVictoryConditionFlags.Unknown_Greenleaf) != 0;
        set => RaiseAndSetIfChanged(VictoryCondition_Unknown_Greenleaf, value, v => _model.VictoryCondition ^= BattleVictoryConditionFlags.Unknown_Greenleaf);
    }

    public bool VictoryCondition_HoldAllBannersFor5Turns
    {
        get => (_model.VictoryCondition & BattleVictoryConditionFlags.HoldAllBannersFor5Turns) != 0;
        set => RaiseAndSetIfChanged(VictoryCondition_HoldAllBannersFor5Turns, value, v => _model.VictoryCondition ^= BattleVictoryConditionFlags.HoldAllBannersFor5Turns);
    }

    public bool VictoryCondition_ClaimAllBanners
    {
        get => (_model.VictoryCondition & BattleVictoryConditionFlags.ClaimAllBanners) != 0;
        set => RaiseAndSetIfChanged(VictoryCondition_ClaimAllBanners, value, v => _model.VictoryCondition ^= BattleVictoryConditionFlags.ClaimAllBanners);
    }

    #endregion

    #region Defeat Conditions

    public bool DefeatCondition_Unknown_Aurora
    {
        get => (_model.DefeatCondition & BattleVictoryConditionFlags.Unknown_AuroraDragnor) != 0;
        set => RaiseAndSetIfChanged(VictoryCondition_Unknown_Aurora, value, v => _model.DefeatCondition ^= BattleVictoryConditionFlags.Unknown_AuroraDragnor);
    }

    public bool DefeatCondition_Unknown_ViperiaDragnor
    {
        get => (_model.DefeatCondition & BattleVictoryConditionFlags.Unknown_ViperiaDragnor) != 0;
        set => RaiseAndSetIfChanged(DefeatCondition_Unknown_ViperiaDragnor, value, v => _model.DefeatCondition ^= BattleVictoryConditionFlags.Unknown_ViperiaDragnor);
    }

    public bool DefeatCondition_Unknown_Greenleaf
    {
        get => (_model.DefeatCondition & BattleVictoryConditionFlags.Unknown_Greenleaf) != 0;
        set => RaiseAndSetIfChanged(DefeatCondition_Unknown_Greenleaf, value, v => _model.DefeatCondition ^= BattleVictoryConditionFlags.Unknown_Greenleaf);
    }

    public bool DefeatCondition_HoldAllBannersFor5Turns
    {
        get => (_model.DefeatCondition & BattleVictoryConditionFlags.HoldAllBannersFor5Turns) != 0;
        set => RaiseAndSetIfChanged(DefeatCondition_HoldAllBannersFor5Turns, value, v => _model.DefeatCondition ^= BattleVictoryConditionFlags.HoldAllBannersFor5Turns);
    }

    public bool DefeatCondition_ClaimAllBanners
    {
        get => (_model.DefeatCondition & BattleVictoryConditionFlags.ClaimAllBanners) != 0;
        set => RaiseAndSetIfChanged(DefeatCondition_ClaimAllBanners, value, v => _model.DefeatCondition ^= BattleVictoryConditionFlags.ClaimAllBanners);
    }

    #endregion


    public ICollection<MapId> MapItems { get; }

    public ICommand JumpToMapCommand { get; }
    public ICommand JumpToItemCommand { get; }

    public List<SelectorComboBoxItem> ItemItems { get; }

    public int Treasure1
    {
        get => (int)_model.Treasure1;
        set => RaiseAndSetIfChanged(_model.Treasure1, (ItemId)value, v => _model.Treasure1 = v);
    }

    public int Treasure2
    {
        get => (int)_model.Treasure2;
        set => RaiseAndSetIfChanged(_model.Treasure2, (ItemId)value, v => _model.Treasure2 = v);
    }

    public int Treasure3
    {
        get => (int)_model.Treasure3;
        set => RaiseAndSetIfChanged(_model.Treasure3, (ItemId)value, v => _model.Treasure3 = v);
    }

    public int Treasure4
    {
        get => (int)_model.Treasure4;
        set => RaiseAndSetIfChanged(_model.Treasure4, (ItemId)value, v => _model.Treasure4 = v);
    }

    public int Treasure5
    {
        get => (int)_model.Treasure5;
        set => RaiseAndSetIfChanged(_model.Treasure5, (ItemId)value, v => _model.Treasure5 = v);
    }

    public int Treasure6
    {
        get => (int)_model.Treasure6;
        set => RaiseAndSetIfChanged(_model.Treasure6, (ItemId)value, v => _model.Treasure6 = v);
    }

    public int Treasure7
    {
        get => (int)_model.Treasure7;
        set => RaiseAndSetIfChanged(_model.Treasure7, (ItemId)value, v => _model.Treasure7 = v);
    }

    public int Treasure8
    {
        get => (int)_model.Treasure8;
        set => RaiseAndSetIfChanged(_model.Treasure8, (ItemId)value, v => _model.Treasure8 = v);
    }

    public int Treasure9
    {
        get => (int)_model.Treasure9;
        set => RaiseAndSetIfChanged(_model.Treasure9, (ItemId)value, v => _model.Treasure9 = v);
    }

    public int Treasure10
    {
        get => (int)_model.Treasure10;
        set => RaiseAndSetIfChanged(_model.Treasure10, (ItemId)value, v => _model.Treasure10 = v);
    }

    public int Treasure11
    {
        get => (int)_model.Treasure11;
        set => RaiseAndSetIfChanged(_model.Treasure11, (ItemId)value, v => _model.Treasure11 = v);
    }

    public int Treasure12
    {
        get => (int)_model.Treasure12;
        set => RaiseAndSetIfChanged(_model.Treasure12, (ItemId)value, v => _model.Treasure12 = v);
    }

    public void View3DModel()
    {
        _sceneRenderer.Configure(0);

        var window = new Map3DWindow(_sceneRenderer);
        var result = _sceneRenderer.LoadScene(Id);
        if (result.IsFailed)
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Failed to load model", result.ToString(), MessageBoxType.Warning));
            return;
        }
        window.ShowDialog();
    }
}
