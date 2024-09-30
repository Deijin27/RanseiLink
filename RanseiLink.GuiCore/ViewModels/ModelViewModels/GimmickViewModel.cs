#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.ViewModels;

public enum GimmickAnimationPreviewMode
{
    One,
    Two
}

public partial class GimmickViewModel : ViewModelBase
{
    private readonly IExternalService _externalService;
    private readonly IOverrideDataProvider _spriteProvider;

    public GimmickViewModel(IExternalService externalService, IOverrideDataProvider overrideSpriteProvider, IJumpService jumpService)
    {
        _externalService = externalService;
        _spriteProvider = overrideSpriteProvider;

        JumpToGimmickRangeCommand = new RelayCommand<GimmickRangeId>(id => jumpService.JumpTo(GimmickRangeSelectorEditorModule.Id, (int)id));

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
            case nameof(Image):
                RaisePropertyChanged(nameof(ImagePath));
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

    public string ImagePath => _spriteProvider.GetSpriteFile(SpriteType.StlStageObje, Image).File;
}