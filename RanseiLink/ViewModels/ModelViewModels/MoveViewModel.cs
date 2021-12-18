using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public enum MoveAnimationPreviewMode
{
    Startup,
    Projectile,
    Impact,
}

public delegate MoveViewModel MoveViewModelFactory(MoveId id, IMove model, IEditorContext context);

public abstract class MoveViewModelBase : ViewModelBase
{
    protected readonly IMove _model;

    public MoveViewModelBase(MoveId id, IMove model)
    {
        Id = id;
        _model = model;
    }

    public MoveId Id { get; }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public bool MovementFlag_MovementOrKnockback
    {
        get => (_model.MovementFlags & MoveMovementFlags.MovementOrKnockback) != 0;
        set => RaiseAndSetIfChanged(MovementFlag_MovementOrKnockback, value, v => _model.MovementFlags ^= MoveMovementFlags.MovementOrKnockback);
    }

    public bool MovementFlag_InvertMovementDirection
    {
        get => (_model.MovementFlags & MoveMovementFlags.InvertMovementDirection) != 0;
        set => RaiseAndSetIfChanged(MovementFlag_InvertMovementDirection, value, v => _model.MovementFlags ^= MoveMovementFlags.InvertMovementDirection);
    }

    public bool MovementFlag_DoubleMovementDistance
    {
        get => (_model.MovementFlags & MoveMovementFlags.DoubleMovementDistance) != 0;
        set => RaiseAndSetIfChanged(MovementFlag_DoubleMovementDistance, value, v => _model.MovementFlags ^= MoveMovementFlags.DoubleMovementDistance);
    }

    public TypeId Type
    {
        get => _model.Type;
        set => RaiseAndSetIfChanged(_model.Type, value, v => _model.Type = v);
    }

    public uint Power
    {
        get => _model.Power;
        set => RaiseAndSetIfChanged(_model.Power, value, v => _model.Power = v);
    }

    public uint Accuracy
    {
        get => _model.Accuracy;
        set => RaiseAndSetIfChanged(_model.Accuracy, value, v => _model.Accuracy = v);
    }

    public MoveRangeId Range
    {
        get => _model.Range;
        set => RaiseAndSetIfChanged(_model.Range, value, v => _model.Range = v);
    }

    public MoveEffectId Effect1
    {
        get => _model.Effect1;
        set => RaiseAndSetIfChanged(_model.Effect1, value, v => _model.Effect1 = v);
    }

    public uint Effect1Chance
    {
        get => _model.Effect1Chance;
        set => RaiseAndSetIfChanged(_model.Effect1Chance, value, v => _model.Effect1Chance = v);
    }

    public MoveEffectId Effect2
    {
        get => _model.Effect2;
        set => RaiseAndSetIfChanged(_model.Effect2, value, v => _model.Effect2 = v);
    }

    public uint Effect2Chance
    {
        get => _model.Effect2Chance;
        set => RaiseAndSetIfChanged(_model.Effect2Chance, value, v => _model.Effect2Chance = v);
    }

    public virtual MoveAnimationId StartupAnimation
    {
        get => _model.StartupAnimation;
        set => RaiseAndSetIfChanged(_model.StartupAnimation, value, v => _model.StartupAnimation = v);
    }

    public virtual MoveAnimationId ProjectileAnimation
    {
        get => _model.ProjectileAnimation;
        set => RaiseAndSetIfChanged(_model.ProjectileAnimation, value, v => _model.ProjectileAnimation = v);
    }

    public virtual MoveAnimationId ImpactAnimation
    {
        get => _model.ImpactAnimation;
        set => RaiseAndSetIfChanged(_model.ImpactAnimation, value, v => _model.ImpactAnimation = v);
    }

    public virtual MoveMovementAnimationId MovementAnimation
    {
        get => _model.MovementAnimation;
        set => RaiseAndSetIfChanged(_model.MovementAnimation, value, v => _model.MovementAnimation = v);
    }
}

public class MoveViewModel : MoveViewModelBase
{
    private readonly ICachedMsgBlockService _msgService;
    private readonly IExternalService _externalService;

    public MoveViewModel(MoveId id, IServiceContainer container, IMove model, IEditorContext context) : base(id, model)
    {
        _msgService = context.CachedMsgBlockService;
        _externalService = container.Resolve<IExternalService>();

        SetPreviewAnimationModeCommand = new RelayCommand<MoveAnimationPreviewMode>(mode =>
        {
            PreviewAnimationMode = mode;
            UpdatePreviewAnimation();
        });

        UpdatePreviewAnimation();

        var jumpService = context.JumpService;
        JumpToMoveRangeCommand = new RelayCommand<MoveRangeId>(jumpService.JumpToMoveRange);
    }

    public string Description
    {
        get => _msgService.GetMoveDescription(Id);
        set => _msgService.SetMoveDescription(Id, value);
    }

    public ICommand JumpToMoveRangeCommand { get; }

    public override MoveAnimationId StartupAnimation
    {
        get => _model.StartupAnimation;
        set
        {
            if (RaiseAndSetIfChanged(_model.StartupAnimation, value, v => _model.StartupAnimation = v))
            {
                UpdatePreviewAnimation();
            }
        }
    }

    public override MoveAnimationId ProjectileAnimation
    {
        get => _model.ProjectileAnimation;
        set
        {
            if (RaiseAndSetIfChanged(_model.ProjectileAnimation, value, v => _model.ProjectileAnimation = v))
            {
                UpdatePreviewAnimation();
            }
        }
    }

    public override MoveAnimationId ImpactAnimation
    {
        get => _model.ImpactAnimation;
        set
        {
            if (RaiseAndSetIfChanged(_model.ImpactAnimation, value, v => _model.ImpactAnimation = v))
            {
                UpdatePreviewAnimation();
            }
        }
    }

    public override MoveMovementAnimationId MovementAnimation
    {
        get => _model.MovementAnimation;
        set => RaiseAndSetIfChanged(_model.MovementAnimation, value, v => _model.MovementAnimation = v);
    }

    private string _currentPreviewAnimationUri;
    public string CurrentPreviewAnimationUri
    {
        get => _currentPreviewAnimationUri;
        set => RaiseAndSetIfChanged(ref _currentPreviewAnimationUri, value);
    }

    private string _currentPreviewAnimationName;
    public string CurrentPreviewAnimationName
    {
        get => _currentPreviewAnimationName;
        set => RaiseAndSetIfChanged(ref _currentPreviewAnimationName, value);
    }

    private MoveAnimationPreviewMode PreviewAnimationMode { get; set; } = MoveAnimationPreviewMode.Startup;

    public ICommand SetPreviewAnimationModeCommand { get; }

    private void UpdatePreviewAnimation()
    {
        switch (PreviewAnimationMode)
        {
            case MoveAnimationPreviewMode.Startup:
                CurrentPreviewAnimationUri = GetAnimationUri(StartupAnimation);
                CurrentPreviewAnimationName = StartupAnimation.ToString();
                break;
            case MoveAnimationPreviewMode.Projectile:
                CurrentPreviewAnimationUri = GetAnimationUri(ProjectileAnimation);
                CurrentPreviewAnimationName = ProjectileAnimation.ToString();
                break;
            case MoveAnimationPreviewMode.Impact:
                CurrentPreviewAnimationUri = GetAnimationUri(ImpactAnimation);
                CurrentPreviewAnimationName = ImpactAnimation.ToString();
                break;
        };
    }

    private string GetAnimationUri(MoveAnimationId id)
    {
        return _externalService.GetMoveAnimationUri(id);
    }
}

public class MoveGridItemViewModel : MoveViewModelBase 
{
    public MoveGridItemViewModel(MoveId id, IMove model) : base(id, model)
    {
    }

}
