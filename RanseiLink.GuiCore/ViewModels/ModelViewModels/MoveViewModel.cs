#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using SixLabors.ImageSharp;

namespace RanseiLink.GuiCore.ViewModels;

public enum MoveAnimationPreviewMode
{
    Startup,
    Projectile,
    Impact,
    Additional,
    Movement,
}

public partial class MoveViewModel : ViewModelBase
{
    private readonly ICachedMsgBlockService _msgService;
    private readonly IExternalService _externalService;
    private readonly IPokemonService _pokemonService;
    private readonly ICachedSpriteProvider _cachedSpriteProvider;
    private readonly ISpriteService _spriteService;
    private readonly IPathToImageConverter _pathToImageConverter;
    private readonly IMoveRangeService _moveRangeService;

    public MoveViewModel(
        ICachedMsgBlockService msgService, 
        IExternalService externalService, 
        IJumpService jumpService, 
        IPokemonService pokemonService,
        ICachedSpriteProvider cachedSpriteProvider,
        ISpriteService spriteService,
        IPathToImageConverter pathToImageConverter,
        IMoveRangeService moveRangeService)
    {
        _msgService = msgService;
        _externalService = externalService;
        _pokemonService = pokemonService;
        _cachedSpriteProvider = cachedSpriteProvider;
        _spriteService = spriteService;
        _pathToImageConverter = pathToImageConverter;
        _moveRangeService = moveRangeService;
        SetPreviewAnimationModeCommand = new RelayCommand<MoveAnimationPreviewMode>(mode =>
        {
            PreviewAnimationMode = mode;
            UpdatePreviewAnimation();
        });

        UpdatePreviewAnimation();

        JumpToMoveRangeCommand = new RelayCommand<MoveRangeId>(id => jumpService.JumpTo(MoveRangeSelectorEditorModule.Id, (int)id));
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
                StarCount = _model.StarCount;
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
        _starCount = _model.StarCount;
        UpdatePreviewAnimation(true);
        RaiseAllPropertiesChanged();
    }

    public object? PreviewImage
    {
        get
        {
            using var img = _spriteService.GetMovePreview(_model, _moveRangeService.Retrieve((int)Range));
            return _pathToImageConverter.TryConvert(img);
        }
    }

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
