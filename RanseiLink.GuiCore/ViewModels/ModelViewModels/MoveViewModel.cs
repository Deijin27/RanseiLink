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

public partial class MoveViewModel : ViewModelBase, IBigViewModel
{
    private readonly ICachedMsgBlockService _msgService;
    private readonly IExternalService _externalService;
    private readonly IPokemonService _pokemonService;
    private readonly ICachedSpriteProvider _cachedSpriteProvider;
    private readonly IMoveAnimationService _moveAnimationService;

    public MoveViewModel(
        ICachedMsgBlockService msgService, 
        IExternalService externalService, 
        IJumpService jumpService, 
        IPokemonService pokemonService,
        ICachedSpriteProvider cachedSpriteProvider,
        INicknameService nicknameService,
        IIdToNameService idToNameService,
        IMoveAnimationService moveAnimationService)
    {
        _msgService = msgService;
        _externalService = externalService;
        _pokemonService = pokemonService;
        _cachedSpriteProvider = cachedSpriteProvider;
        _moveAnimationService = moveAnimationService;
        MoveRangeItems = nicknameService.GetAllNicknames(nameof(MoveRangeId));
        MoveAnimationItems = idToNameService.GetComboBoxItemsPlusDefault<IMoveAnimationService>();
        SetPreviewAnimationModeCommand = new RelayCommand<MoveAnimationPreviewMode>(mode =>
        {
            PreviewAnimationMode = mode;
            UpdatePreviewAnimation();
        });

        UpdatePreviewAnimation();

        JumpToMoveRangeCommand = new RelayCommand<int>(id => jumpService.JumpTo(MoveRangeWorkspaceModule.Id, id));
        _selectPokemonCommand = new RelayCommand<PokemonMiniViewModel>(pk => { if (pk != null) jumpService.JumpTo(PokemonWorkspaceModule.Id, pk.Id); });

        PropertyChanged += MoveViewModel_PropertyChanged;
    }

    private void MoveViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(StartupAnimation):
            case nameof(ProjectileAnimation):
            case nameof(ImpactAnimation):
            case nameof(AdditionalAnimation):
            case nameof(MovementAnimation):
                OnAnimationChanged();
                break;

            case nameof(MovementFlag_DoubleMovementDistance):
            case nameof(MovementFlag_InvertMovementDirection):
            case nameof(MovementFlag_MovementOrKnockback):
            case nameof(Movement):
            case nameof(Type):
            case nameof(StarCount):
            case nameof(Range):
                RaisePropertyChanged(nameof(PreviewImage));
                break;

            case nameof(Power):
                StarCount = MiscUtil.PowerToStarCount(_model.Power);
                break;
        }
    }

    // You can change power really fast, this makes less events fire so less preview image creation
    private int _starCount;
    public int StarCount
    {
        get => _starCount;
        set => SetProperty(ref _starCount, value);
    }

    public void SetModel(MoveId id, Move model)
    {
        _id = id;
        _model = model;
        _starCount = MiscUtil.PowerToStarCount(_model.Power);
        UpdatePreviewAnimation(true);
        RaiseAllPropertiesChanged();
    }

    public object? PreviewImage => _cachedSpriteProvider.GetMovePreview(_model);

    public bool MovementFlag_MovementOrKnockback
    {
        get => (_model.MovementFlags & MoveMovementFlags.MovementOrKnockback) != 0;
        set => SetProperty(MovementFlag_MovementOrKnockback, value, v => _model.MovementFlags ^= MoveMovementFlags.MovementOrKnockback);
    }

    public bool MovementFlag_InvertMovementDirection
    {
        get => (_model.MovementFlags & MoveMovementFlags.InvertMovementDirection) != 0;
        set => SetProperty(MovementFlag_InvertMovementDirection, value, v => _model.MovementFlags ^= MoveMovementFlags.InvertMovementDirection);
    }

    public bool MovementFlag_DoubleMovementDistance
    {
        get => (_model.MovementFlags & MoveMovementFlags.DoubleMovementDistance) != 0;
        set => SetProperty(MovementFlag_DoubleMovementDistance, value, v => _model.MovementFlags ^= MoveMovementFlags.DoubleMovementDistance);
    }

    public ICommand JumpToMoveRangeCommand { get; }

    private string? _currentPreviewAnimationUri;
    public string? CurrentPreviewAnimationUri
    {
        get => _currentPreviewAnimationUri;
        set => SetProperty(ref _currentPreviewAnimationUri, value);
    }

    private string? _currentPreviewAnimationName;
    public string? CurrentPreviewAnimationName
    {
        get => _currentPreviewAnimationName;
        set => SetProperty(ref _currentPreviewAnimationName, value);
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
                UpdateAnimation(StartupAnimation);
                break;
            case MoveAnimationPreviewMode.Projectile:
                UpdateAnimation(ProjectileAnimation);
                break;
            case MoveAnimationPreviewMode.Impact:
                UpdateAnimation(ImpactAnimation);
                break;
            case MoveAnimationPreviewMode.Additional:
                UpdateAnimation(AdditionalAnimation);
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

    private void UpdateAnimation(int id)
    {
        TrueMoveAnimationId trueAnim;
        if (!_moveAnimationService.ValidateId(id))
        {
            trueAnim = TrueMoveAnimationId.Default;
            
        }
        else
        {
            trueAnim = _moveAnimationService.Retrieve(id).Animation;
        }

        CurrentPreviewAnimationUri = _externalService.GetMoveAnimationUri(trueAnim);
        CurrentPreviewAnimationName = $"{(int)trueAnim:000} - {trueAnim}";
    }

    public void SetModel(int id, object model)
    {
        SetModel((MoveId)id, (Move)model);
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
