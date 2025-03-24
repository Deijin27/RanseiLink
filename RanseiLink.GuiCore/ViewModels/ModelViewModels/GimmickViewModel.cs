#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public enum GimmickAnimationPreviewMode
{
    One,
    Two
}

public partial class GimmickViewModel : ViewModelBase, IBigViewModel
{
    private readonly IMoveAnimationService _moveAnimationService;
    private readonly ICachedSpriteProvider _cachedSpriteProvider;
    private readonly IExternalService _externalService;
    private readonly IOverrideDataProvider _spriteProvider;

    public ICommand JumpToGimmickCommand { get; }

    public GimmickViewModel(IMoveAnimationService moveAnimationService, ICachedSpriteProvider cachedSpriteProvider, INicknameService nicknameService, IExternalService externalService, IOverrideDataProvider overrideSpriteProvider, IJumpService jumpService, IIdToNameService idToNameService)
    {
        _moveAnimationService = moveAnimationService;
        _cachedSpriteProvider = cachedSpriteProvider;
        _externalService = externalService;
        _spriteProvider = overrideSpriteProvider;

        GimmickItems = idToNameService.GetComboBoxItemsPlusDefault<IGimmickService>();
        //MoveAnimationItems = idToNameService.GetComboBoxItemsPlusDefault<IMoveAnimationService>();
        GimmickRangeItems = nicknameService.GetAllNicknames(nameof(GimmickRangeId));
        GimmickObjectItems = nicknameService.GetAllNicknames(nameof(GimmickObjectId));

        JumpToGimmickCommand = new RelayCommand<int>(id => jumpService.JumpTo(GimmickWorkspaceEditorModule.Id, id));
        JumpToGimmickRangeCommand = new RelayCommand<int>(id => jumpService.JumpTo(GimmickRangeWorkspaceModule.Id, id));

        SetPreviewAnimationModeCommand = new RelayCommand<GimmickAnimationPreviewMode>(mode =>
        {
            PreviewAnimationMode = mode;
            UpdatePreviewAnimation(false);
        });

        UpdatePreviewAnimation(false);

        PropertyChanged += GimmickViewModel_PropertyChanged;
    }

    private void GimmickViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Anim1):
            case nameof(Anim2):
                OnAnimationChanged();
                break;
            case nameof(Image1):
                RaisePropertyChanged(nameof(Image1Path));
                break;
            case nameof(Image2):
                RaisePropertyChanged(nameof(Image2Path));
                break;
            case nameof(Image3):
                RaisePropertyChanged(nameof(Image3Path));
                break;
            case nameof(AttackPower):
                StarCount = MiscUtil.PowerToStarCount(AttackPower);
                break;
            case nameof(StarCount):
            case nameof(AttackType):
            case nameof(Range):
                RaisePropertyChanged(nameof(PreviewImage));
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

    public void SetModel(GimmickId id, Gimmick model)
    {
        _id = id;
        _model = model;
        UpdatePreviewAnimation(true);
        RaiseAllPropertiesChanged();
    }

    public object? PreviewImage => _cachedSpriteProvider.GetGimmickPreview(_model);

    public ICommand JumpToGimmickRangeCommand { get; }

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

    private GimmickAnimationPreviewMode PreviewAnimationMode { get; set; } = GimmickAnimationPreviewMode.One;

    public ICommand SetPreviewAnimationModeCommand { get; }

    protected void OnAnimationChanged()
    {
        UpdatePreviewAnimation(false);
    }

    private void UpdatePreviewAnimation(bool suppressPropertyChanged)
    {
        switch (PreviewAnimationMode)
        {
            case GimmickAnimationPreviewMode.One:
                _currentPreviewAnimationUri = GetAnimationUri(Anim1);
                _currentPreviewAnimationName = Anim1.ToString();
                break;
            case GimmickAnimationPreviewMode.Two:
                _currentPreviewAnimationUri = GetAnimationUri(Anim2);
                _currentPreviewAnimationName = Anim2.ToString();
                break;
        };
        if (!suppressPropertyChanged)
        {
            RaisePropertyChanged(nameof(CurrentPreviewAnimationUri));
            RaisePropertyChanged(nameof(CurrentPreviewAnimationName));
        }
    }

    private string GetAnimationUri(int id)
    {
        if (!_moveAnimationService.ValidateId(id))
        {
            return _externalService.GetMoveAnimationUri(TrueMoveAnimationId.Default);
        }
        return _externalService.GetMoveAnimationUri(_moveAnimationService.Retrieve(id).Animation);
    }

    public void SetModel(int id, object model)
    {
        SetModel((GimmickId)id, (Gimmick)model);
    }

    public string Image1Path => _spriteProvider.GetSpriteFile(SpriteType.StlStageObje, Image1).File;
    public string Image2Path => _spriteProvider.GetSpriteFile(SpriteType.StlStageObje, Image2).File;
    public string Image3Path => _spriteProvider.GetSpriteFile(SpriteType.StlStageObje, Image3).File;
}