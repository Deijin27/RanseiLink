using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public enum MoveAnimationPreviewMode
{
    Startup,
    Projectile,
    Impact,
}

public delegate MoveViewModel MoveViewModelFactory(IMove model);

public class MoveViewModel : ViewModelBase
{
    private readonly IExternalService _externalService;
    private readonly IMove _model;

    public MoveViewModel(IServiceContainer container, IMove model)
    {
        _externalService = container.Resolve<IExternalService>();

        SetPreviewAnimationModeCommand = new RelayCommand<MoveAnimationPreviewMode>(mode =>
        {
            PreviewAnimationMode = mode;
            UpdatePreviewAnimation();
        });

        _model = model;
        UpdatePreviewAnimation();
    }

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

    public TypeId[] TypeItems { get; } = EnumUtil.GetValues<TypeId>().ToArray();
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

    public MoveRangeId[] RangeItems { get; } = EnumUtil.GetValues<MoveRangeId>().ToArray();
    public MoveRangeId Range
    {
        get => _model.Range;
        set => RaiseAndSetIfChanged(_model.Range, value, v => _model.Range = v);
    }

    public MoveEffectId[] EffectItems { get; } = EnumUtil.GetValues<MoveEffectId>().ToArray();

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

    public MoveAnimationId[] AnimationItems { get; } = EnumUtil.GetValues<MoveAnimationId>().ToArray();

    public MoveAnimationId StartupAnimation
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

    public MoveAnimationId ProjectileAnimation
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

    public MoveAnimationId ImpactAnimation
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

    public MoveAnimationTargetId[] AnimationTargetItems { get; } = EnumUtil.GetValues<MoveAnimationTargetId>().ToArray();

    public MoveAnimationTargetId AnimationTarget1
    {
        get => _model.AnimationTarget1;
        set => RaiseAndSetIfChanged(_model.AnimationTarget1, value, v => _model.AnimationTarget1 = v);
    }

    public MoveAnimationTargetId AnimationTarget2
    {
        get => _model.AnimationTarget2;
        set => RaiseAndSetIfChanged(_model.AnimationTarget2, value, v => _model.AnimationTarget2 = v);
    }

    public MoveMovementAnimationId[] MovementAnimationItems { get; } = EnumUtil.GetValues<MoveMovementAnimationId>().ToArray();

    public MoveMovementAnimationId MovementAnimation
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
