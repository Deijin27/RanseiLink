using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public enum GimmickAnimationPreviewMode
{
    One,
    Two
}

public delegate GimmickViewModel GimmickViewModelFactory(GimmickId id, IGimmick model, IEditorContext context);

public abstract class GimmickViewModelBase : ViewModelBase
{
    protected readonly IGimmick _model;

    public GimmickViewModelBase(GimmickId id, IGimmick model)
    {
        Id = id;
        _model = model;
    }

    public GimmickId Id { get; }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
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


    protected virtual void OnAnimationChanged()
    {

    }
}

public class GimmickViewModel : GimmickViewModelBase
{
    private readonly IExternalService _externalService;

    public GimmickViewModel(GimmickId id, IServiceContainer container, IGimmick model, IEditorContext context) : base(id, model)
    {
        _externalService = container.Resolve<IExternalService>();

        SetPreviewAnimationModeCommand = new RelayCommand<GimmickAnimationPreviewMode>(mode =>
        {
            PreviewAnimationMode = mode;
            UpdatePreviewAnimation();
        });

        UpdatePreviewAnimation();
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

    protected override void OnAnimationChanged()
    {
        UpdatePreviewAnimation();
    }

    private void UpdatePreviewAnimation()
    {
        switch (PreviewAnimationMode)
        {
            case GimmickAnimationPreviewMode.One:
                CurrentPreviewAnimationUri = GetAnimationUri(Animation1);
                CurrentPreviewAnimationName = Animation1.ToString();
                break;
            case GimmickAnimationPreviewMode.Two:
                CurrentPreviewAnimationUri = GetAnimationUri(Animation2);
                CurrentPreviewAnimationName = Animation2.ToString();
                break;
        };
    }

    private string GetAnimationUri(MoveAnimationId id)
    {
        return _externalService.GetMoveAnimationUri(id);
    }
}

public class GimmickGridItemViewModel : GimmickViewModelBase 
{
    public GimmickGridItemViewModel(GimmickId id, IGimmick model) : base(id, model)
    {
    }

}
