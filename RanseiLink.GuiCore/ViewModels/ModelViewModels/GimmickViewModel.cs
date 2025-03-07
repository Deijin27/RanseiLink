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
    private readonly IExternalService _externalService;
    private readonly IOverrideDataProvider _spriteProvider;

    public ICommand JumpToGimmickCommand { get; }

    public GimmickViewModel(INicknameService nicknameService, IExternalService externalService, IOverrideDataProvider overrideSpriteProvider, IJumpService jumpService, IIdToNameService idToNameService)
    {
        _externalService = externalService;
        _spriteProvider = overrideSpriteProvider;

        GimmickItems = idToNameService.GetComboBoxItemsPlusDefault<IGimmickService>();
        GimmickRangeItems = nicknameService.GetAllNicknames(nameof(GimmickRangeId));

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
            case nameof(Animation1):
            case nameof(Animation2):
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
        }
    }

    public void SetModel(GimmickId id, Gimmick model)
    {
        _id = id;
        _model = model;
        UpdatePreviewAnimation(true);
        RaiseAllPropertiesChanged();
    }

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
                _currentPreviewAnimationUri = GetAnimationUri(Animation1);
                _currentPreviewAnimationName = Animation1.ToString();
                break;
            case GimmickAnimationPreviewMode.Two:
                _currentPreviewAnimationUri = GetAnimationUri(Animation2);
                _currentPreviewAnimationName = Animation2.ToString();
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

    public void SetModel(int id, object model)
    {
        SetModel((GimmickId)id, (Gimmick)model);
    }

    public string Image1Path => _spriteProvider.GetSpriteFile(SpriteType.StlStageObje, Image1).File;
    public string Image2Path => _spriteProvider.GetSpriteFile(SpriteType.StlStageObje, Image2).File;
    public string Image3Path => _spriteProvider.GetSpriteFile(SpriteType.StlStageObje, Image3).File;
}