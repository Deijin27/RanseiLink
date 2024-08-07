﻿#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.ViewModels;

public enum GimmickAnimationPreviewMode
{
    One,
    Two
}

public class GimmickViewModel : ViewModelBase
{
    private Gimmick _model;
    private readonly IExternalService _externalService;
    private readonly IOverrideDataProvider _spriteProvider;

    public GimmickViewModel(IExternalService externalService, IOverrideDataProvider overrideSpriteProvider, IJumpService jumpService)
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
        set => SetProperty(_model.Name, value, v => _model.Name = v);
    }

    public GimmickObjectId State1Sprite
    {
        get => _model.State1Object;
        set => SetProperty(_model.State1Object, value, v => _model.State1Object = v);
    }

    public GimmickObjectId State2Sprite
    {
        get => _model.State2Object;
        set => SetProperty(_model.State2Object, value, v => _model.State2Object = v);
    }

    public MoveEffectId Effect
    {
        get => _model.Effect;
        set => SetProperty(_model.Effect, value, v => _model.Effect = v);
    }

    public TypeId AttackType
    {
        get => _model.AttackType;
        set => SetProperty(_model.AttackType, value, v => _model.AttackType = v);
    }

    public TypeId DestroyType
    {
        get => _model.DestroyType;
        set => SetProperty(_model.DestroyType, value, v => _model.DestroyType = v);
    }

    public GimmickRangeId Range
    {
        get => _model.Range;
        set => SetProperty(_model.Range, value, v => _model.Range = v);
    }

    public MoveAnimationId Animation1
    {
        get => _model.Animation1;
        set
        {
            if (SetProperty(_model.Animation1, value, v => _model.Animation1 = v))
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
            if (SetProperty(_model.Animation2, value, v => _model.Animation2 = v))
            {
                OnAnimationChanged();
            }
        }
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

    public int Image
    {
        get => _model.Image;
        set
        {
            if (SetProperty(_model.Image, value, v => _model.Image = v))
            {
                RaisePropertyChanged(nameof(ImagePath));
            }
        }
    }

    public string ImagePath => _spriteProvider.GetSpriteFile(SpriteType.StlStageObje, Image).File;
}

