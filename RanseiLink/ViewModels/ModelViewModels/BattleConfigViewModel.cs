using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Graphics;
using System.Collections.Generic;
using System.Windows.Input;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public interface IBattleConfigViewModel
{
    void SetModel(BattleConfigId id, BattleConfig model);
}

public class BattleConfigViewModel : ViewModelBase, IBattleConfigViewModel
{
    private BattleConfig _model;

    public BattleConfigViewModel(IMapService mapService, IJumpService jumpService)
    {
        _model = new BattleConfig();
        
        MapItems = mapService.GetMapIds();

        JumpToMapCommand = new RelayCommand<MapId>(id => jumpService.JumpTo(MapSelectorEditorModule.Id, (int)id));
    }

    public void SetModel(BattleConfigId id, BattleConfig model)
    {
        Id = id;
        _model = model;
        RaiseAllPropertiesChanged();
    }

    public BattleConfigId Id { get; private set; }

    public MapId Map
    {
        get => _model.MapId;
        set => RaiseAndSetIfChanged(_model.MapId, value, v => _model.MapId = value);
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
}
