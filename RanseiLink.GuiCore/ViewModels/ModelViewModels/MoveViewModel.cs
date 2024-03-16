#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public enum MoveAnimationPreviewMode
{
    Startup,
    Projectile,
    Impact,
    Additional,
    Movement,
}

public class MoveViewModel : ViewModelBase
{
    private MoveId _id;
    private Move _model;
    private readonly ICachedMsgBlockService _msgService;
    private readonly IExternalService _externalService;
    private readonly IPokemonService _pokemonService;
    private readonly ICachedSpriteProvider _cachedSpriteProvider;

    public MoveViewModel(
        ICachedMsgBlockService msgService, 
        IExternalService externalService, 
        IJumpService jumpService, 
        IPokemonService pokemonService,
        ICachedSpriteProvider cachedSpriteProvider)
    {
        _msgService = msgService;
        _externalService = externalService;
        _pokemonService = pokemonService;
        _cachedSpriteProvider = cachedSpriteProvider;
        _model = new Move();
        SetPreviewAnimationModeCommand = new RelayCommand<MoveAnimationPreviewMode>(mode =>
        {
            PreviewAnimationMode = mode;
            UpdatePreviewAnimation();
        });

        UpdatePreviewAnimation();

        JumpToMoveRangeCommand = new RelayCommand<MoveRangeId>(id => jumpService.JumpTo(MoveRangeSelectorEditorModule.Id, (int)id));
        _selectPokemonCommand = new RelayCommand<PokemonMiniViewModel>(pk => { if (pk != null) jumpService.JumpTo(PokemonWorkspaceModule.Id, pk.Id); });
    }

    public void SetModel(MoveId id, Move model)
    {
        _id = id;
        Id = (int)id;
        _model = model;
        UpdatePreviewAnimation(true);
        RaiseAllPropertiesChanged();
    }

    public int Id { get; private set; }

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

    public MoveMovementId Movement
    {
        get => _model.Movement;
        set => RaiseAndSetIfChanged(_model.Movement, value, v => _model.Movement = v);
    }

    public MoveUnknownOptionId UnknownOption
    {
        get => _model.UnknownOption;
        set => RaiseAndSetIfChanged(_model.UnknownOption, value, v => _model.UnknownOption = v);
    }

    public int UnknownValue_6_28_4
    {
        get => _model.UnknownValue_6_28_4;
        set => RaiseAndSetIfChanged(_model.UnknownValue_6_28_4, value, v => _model.UnknownValue_6_28_4 = v);
    }

    public TypeId Type
    {
        get => _model.Type;
        set => RaiseAndSetIfChanged(_model.Type, value, v => _model.Type = v);
    }

    public int Power
    {
        get => _model.Power;
        set => RaiseAndSetIfChanged(_model.Power, value, v => _model.Power = v);
    }

    public int Accuracy
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

    public int Effect1Chance
    {
        get => _model.Effect1Chance;
        set => RaiseAndSetIfChanged(_model.Effect1Chance, value, v => _model.Effect1Chance = v);
    }

    public MoveEffectId Effect2
    {
        get => _model.Effect2;
        set => RaiseAndSetIfChanged(_model.Effect2, value, v => _model.Effect2 = v);
    }

    public int Effect2Chance
    {
        get => _model.Effect2Chance;
        set => RaiseAndSetIfChanged(_model.Effect2Chance, value, v => _model.Effect2Chance = v);
    }

    public MoveEffectId Effect3
    {
        get => _model.Effect3;
        set => RaiseAndSetIfChanged(_model.Effect3, value, v => _model.Effect3 = v);
    }

    public int Effect3Chance
    {
        get => _model.Effect3Chance;
        set => RaiseAndSetIfChanged(_model.Effect3Chance, value, v => _model.Effect3Chance = v);
    }

    public MoveEffectId Effect4
    {
        get => _model.Effect4;
        set => RaiseAndSetIfChanged(_model.Effect4, value, v => _model.Effect4 = v);
    }

    public int Effect4Chance
    {
        get => _model.Effect4Chance;
        set => RaiseAndSetIfChanged(_model.Effect4Chance, value, v => _model.Effect4Chance = v);
    }

    public MoveAnimationId StartupAnimation
    {
        get => _model.StartupAnimation;
        set
        {
            if (RaiseAndSetIfChanged(_model.StartupAnimation, value, v => _model.StartupAnimation = v))
            {
                OnAnimationChanged();
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
                OnAnimationChanged();
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
                OnAnimationChanged();
            }
        }
    }

    public MoveAnimationId AdditionalAnimation
    {
        get => _model.AdditionalAnimation;
        set
        {
            if (RaiseAndSetIfChanged(_model.AdditionalAnimation, value, v => _model.AdditionalAnimation = v))
            {
                OnAnimationChanged();
            }
        }
    }

    public MoveMovementAnimationId MovementAnimation
    {
        get => _model.MovementAnimation;
        set
        {
            if (RaiseAndSetIfChanged(_model.MovementAnimation, value, v => _model.MovementAnimation = v))
            {
                OnAnimationChanged();
            }
        }
    }

    public string Description
    {
        get => _msgService.GetMsgOfType(MsgShortcut.MoveDescription, Id);
        set => _msgService.SetMsgOfType(MsgShortcut.MoveDescription, Id, value);
    }

    public ICommand JumpToMoveRangeCommand { get; }



    private string? _currentPreviewAnimationUri;
    public string? CurrentPreviewAnimationUri
    {
        get => _currentPreviewAnimationUri;
        set => RaiseAndSetIfChanged(ref _currentPreviewAnimationUri, value);
    }

    private string? _currentPreviewAnimationName;
    public string? CurrentPreviewAnimationName
    {
        get => _currentPreviewAnimationName;
        set => RaiseAndSetIfChanged(ref _currentPreviewAnimationName, value);
    }

    private MoveAnimationPreviewMode PreviewAnimationMode { get; set; } = MoveAnimationPreviewMode.Startup;

    public ICommand SetPreviewAnimationModeCommand { get; }

    protected void OnAnimationChanged()
    {
        UpdatePreviewAnimation();
    }

    private void UpdatePreviewAnimation(bool suppressPropertyChanged = false)
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
            case MoveAnimationPreviewMode.Additional:
                CurrentPreviewAnimationUri = GetAnimationUri(AdditionalAnimation);
                CurrentPreviewAnimationName = AdditionalAnimation.ToString();
                break;
            case MoveAnimationPreviewMode.Movement:
                CurrentPreviewAnimationUri = _externalService.GetMoveMovementAnimationUri(MovementAnimation);
                CurrentPreviewAnimationName = MovementAnimation.ToString();
                break;
        };
        if (!suppressPropertyChanged)
        {
            RaisePropertyChanged(nameof(CurrentPreviewAnimationUri));
            RaisePropertyChanged(nameof(CurrentPreviewAnimationName));
        }
    }

    private string GetAnimationUri(MoveAnimationId id)
    {
        return _externalService.GetMoveAnimationUri(id);
    }

    private readonly ICommand _selectPokemonCommand;

    public List<PokemonMiniViewModel> PokemonWithMove
    {
        get
        {
            var list = new List<PokemonMiniViewModel>();
            foreach (var id in _pokemonService.ValidIds())
            {
                var pokemon = _pokemonService.Retrieve(id);
                if (pokemon.Move == _id)
                {
                    list.Add(new PokemonMiniViewModel(_cachedSpriteProvider, pokemon, id, _selectPokemonCommand));
                }
            }
            return list;
        }
    }
}
