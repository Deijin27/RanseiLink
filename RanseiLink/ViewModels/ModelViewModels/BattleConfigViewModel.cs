using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Collections.Generic;

namespace RanseiLink.ViewModels;

public delegate BattleConfigViewModel BattleConfigViewModelFactory(BattleConfigId id, IBattleConfig model, IEditorContext context);

public abstract class BattleConfigViewModelBase : ViewModelBase
{
    private readonly IBattleConfig _model;

    public BattleConfigViewModelBase(BattleConfigId id, IBattleConfig model)
    {
        Id = id;
        _model = model;
    }

    public BattleConfigId Id { get; }

    public BattleEnvironment Environment
    {
        get => new(_model.Environment, _model.EnvironmentVariant);
        set
        {
            if (Environment != value)
            {
                _model.Environment = value.Environment;
                _model.EnvironmentVariant = value.Variant;
                RaisePropertyChanged();
            }
        }
    }

    public uint NumberOfTurns
    {
        get => _model.NumberOfTurns;
        set => RaiseAndSetIfChanged(_model.NumberOfTurns, value, v => _model.NumberOfTurns = v);
    }

    #region Victory Conditions

    public bool VictoryCondition_Unknown_Aurora
    {
        get => (_model.VictoryCondition & BattleVictoryConditionFlags.Unknown_Aurora) != 0;
        set => RaiseAndSetIfChanged(VictoryCondition_Unknown_Aurora, value, v => _model.VictoryCondition ^= BattleVictoryConditionFlags.Unknown_Aurora);
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

    public bool VictoryCondition_Unknown_Pugilis
    {
        get => (_model.VictoryCondition & BattleVictoryConditionFlags.Unknown_Pugilis) != 0;
        set => RaiseAndSetIfChanged(VictoryCondition_Unknown_Pugilis, value, v => _model.VictoryCondition ^= BattleVictoryConditionFlags.Unknown_Pugilis);
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
        get => (_model.DefeatCondition & BattleVictoryConditionFlags.Unknown_Aurora) != 0;
        set => RaiseAndSetIfChanged(VictoryCondition_Unknown_Aurora, value, v => _model.DefeatCondition ^= BattleVictoryConditionFlags.Unknown_Aurora);
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

    public bool DefeatCondition_Unknown_Pugilis
    {
        get => (_model.DefeatCondition & BattleVictoryConditionFlags.Unknown_Pugilis) != 0;
        set => RaiseAndSetIfChanged(DefeatCondition_Unknown_Pugilis, value, v => _model.DefeatCondition ^= BattleVictoryConditionFlags.Unknown_Pugilis);
    }

    public bool DefeatCondition_ClaimAllBanners
    {
        get => (_model.DefeatCondition & BattleVictoryConditionFlags.ClaimAllBanners) != 0;
        set => RaiseAndSetIfChanged(DefeatCondition_ClaimAllBanners, value, v => _model.DefeatCondition ^= BattleVictoryConditionFlags.ClaimAllBanners);
    }

    #endregion

}

public class BattleConfigViewModel : BattleConfigViewModelBase
{
    public BattleConfigViewModel(BattleConfigId id, IBattleConfig model, IEditorContext context) : base(id, model) 
    {
        EnvironmentItems = context.DataService.BattleEnvironment.GetBattleEnvironments();
    }

    public ICollection<BattleEnvironment> EnvironmentItems { get; }

}
