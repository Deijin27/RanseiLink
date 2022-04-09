using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public enum GimmickAnimationPreviewMode
{
    One,
    Two
}

public interface IGimmickViewModel
{
    void SetModel(GimmickId id, Gimmick model);
}

public class GimmickViewModel : ViewModelBase, IGimmickViewModel
{
    private Gimmick _model;
    private readonly IExternalService _externalService;
    private readonly IOverrideSpriteProvider _spriteProvider;

    public GimmickViewModel(IExternalService externalService, IOverrideSpriteProvider overrideSpriteProvider, IJumpService jumpService)
    {
        _model = new Gimmick();

        _externalService = externalService;
        _spriteProvider = overrideSpriteProvider;

        JumpToGimmickRangeCommand = new RelayCommand<GimmickRangeId>(id => jumpService.JumpTo(GimmickRangeSelectorEditorModule.Id, (int)id));

        SetPreviewAnimationModeCommand = new RelayCommand<GimmickAnimationPreviewMode>(mode =>
        {
            PreviewAnimationMode = mode;
            UpdatePreviewAnimation(false);
        });

        UpdatePreviewAnimation(false);
    }

    public void SetModel(GimmickId id, Gimmick model)
    {
        Id = id;
        _model = model;
        UpdatePreviewAnimation(true);
        RaiseAllPropertiesChanged();
    }

    public GimmickId Id { get; private set; }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public GimmickObjectId State1Sprite
    {
        get => _model.State1Object;
        set => RaiseAndSetIfChanged(_model.State1Object, value, v => _model.State1Object = v);
    }

    public GimmickObjectId State2Sprite
    {
        get => _model.State2Object;
        set => RaiseAndSetIfChanged(_model.State2Object, value, v => _model.State2Object = v);
    }

    public MoveEffectId Effect
    {
        get => _model.Effect;
        set => RaiseAndSetIfChanged(_model.Effect, value, v => _model.Effect = v);
    }

    public TypeId AttackType
    {
        get => _model.AttackType;
        set => RaiseAndSetIfChanged(_model.AttackType, value, v => _model.AttackType = v);
    }

    public TypeId DestroyType
    {
        get => _model.DestroyType;
        set => RaiseAndSetIfChanged(_model.DestroyType, value, v => _model.DestroyType = v);
    }

    public GimmickRangeId Range
    {
        get => _model.Range;
        set => RaiseAndSetIfChanged(_model.Range, value, v => _model.Range = v);
    }

    public MoveAnimationId Animation1
    {
        get => _model.Animation1;
        set
        {
            if (RaiseAndSetIfChanged(_model.Animation1, value, v => _model.Animation1 = v))
            {
                OnAnimationChanged();
            }
        }
    }

    public MoveAnimationId Animation2
    {
        get => _model.Animation2;
        set
        {
            if (RaiseAndSetIfChanged(_model.Animation2, value, v => _model.Animation2 = v))
            {
                OnAnimationChanged();
            }
        }
    }


    public ICommand JumpToGimmickRangeCommand { get; }



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

    public int Image
    {
        get => _model.Image;
        set
        {
            if (RaiseAndSetIfChanged(_model.Image, value, v => _model.Image = v))
            {
                RaisePropertyChanged(nameof(ImagePath));
            }
        }
    }

    public string ImagePath => _spriteProvider.GetSpriteFile(SpriteType.StlStageObje, Image).File;
}

