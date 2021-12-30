using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Linq;

namespace RanseiLink.ViewModels;

public class AbilityGridViewModel : GridViewModelBase<AbilityId, IAbility, AbilityGridItemViewModel, IDisposableAbilityService>, ISaveableRefreshable
{
    public AbilityGridViewModel(IServiceContainer container, IEditorContext context)
        : base(
            container,
            context.DataService.Ability.Disposable, 
            EnumUtil.GetValuesExceptDefaults<AbilityId>().ToArray(), 
            (id, model) => new AbilityGridItemViewModel(id, model))
    {
    }
    public override int FrozenColumnCount => 2;
}

public class BaseWarriorGridViewModel : GridViewModelBase<WarriorId, IBaseWarrior, BaseWarriorGridItemViewModel, IDisposableBaseWarriorService>, ISaveableRefreshable
{
    public BaseWarriorGridViewModel(IServiceContainer container, IEditorContext context)
        : base(
            container,
            context.DataService.BaseWarrior.Disposable,
            EnumUtil.GetValuesExceptDefaults<WarriorId>().ToArray(),
            (id, model) => new BaseWarriorGridItemViewModel(id, model))
    {
    }
}

public class MoveGridViewModel : GridViewModelBase<MoveId, IMove, MoveGridItemViewModel, IDisposableMoveService>, ISaveableRefreshable
{
    public MoveGridViewModel(IServiceContainer container, IEditorContext context)
        : base(
            container,
            context.DataService.Move.Disposable,
            EnumUtil.GetValues<MoveId>().ToArray(),
            (id, model) => new MoveGridItemViewModel(id, model))
    {
    }

    public override int FrozenColumnCount => 2;
}

public class PokemonGridViewModel : GridViewModelBase<PokemonId, IPokemon, PokemonGridItemViewModel, IDisposablePokemonService>, ISaveableRefreshable
{
    public PokemonGridViewModel(IServiceContainer container, IEditorContext context)
        : base(
            container,
            context.DataService.Pokemon.Disposable,
            EnumUtil.GetValuesExceptDefaults<PokemonId>().ToArray(),
            (id, model) => new PokemonGridItemViewModel(id, model))
    {
    }

    public override int FrozenColumnCount => 2;
}

public class WarriorSkillGridViewModel : GridViewModelBase<WarriorSkillId, IWarriorSkill, WarriorSkillGridItemViewModel, IDisposableWarriorSkillService>, ISaveableRefreshable
{
    public WarriorSkillGridViewModel(IServiceContainer container, IEditorContext context)
        : base(
            container,
            context.DataService.WarriorSkill.Disposable,
            EnumUtil.GetValues<WarriorSkillId>().ToArray(),
            (id, model) => new WarriorSkillGridItemViewModel(id, model))
    {
    }

    public override int FrozenColumnCount => 2;
}

public class GimmickGridViewModel : GridViewModelBase<GimmickId, IGimmick, GimmickGridItemViewModel, IDisposableGimmickService>, ISaveableRefreshable
{
    public GimmickGridViewModel(IServiceContainer container, IEditorContext context)
        : base(
            container,
            context.DataService.Gimmick.Disposable,
            EnumUtil.GetValues<GimmickId>().ToArray(),
            (id, model) => new GimmickGridItemViewModel(id, model))
    {
    }

    public override int FrozenColumnCount => 2;
}

public class BuildingGridViewModel : GridViewModelBase<BuildingId, IBuilding, BuildingGridItemViewModel, IDisposableBuildingService>, ISaveableRefreshable
{
    public BuildingGridViewModel(IServiceContainer container, IEditorContext context)
        : base(
            container,
            context.DataService.Building.Disposable,
            EnumUtil.GetValues<BuildingId>().ToArray(),
            (id, model) => new BuildingGridItemViewModel(id, model))
    {
    }

    public override int FrozenColumnCount => 2;
}