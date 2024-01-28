#nullable enable
using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.ViewModels;

public class AnimationViewModel : ViewModelBase
{
    private readonly IAnimGuiManager _manager;
    private readonly AnimationTypeId _type;
    private readonly Func<int> _id;

    public AnimationViewModel(IAnimGuiManager manager, AnimationTypeId type, int id) : this(manager, type, () => id)
    {
        
    }

    public AnimationViewModel(IAnimGuiManager manager, AnimationTypeId type, Func<int> id)
    {
        _manager = manager;
        _type = type;
        _id = id;

        ImportCommand = new RelayCommand(async () =>
        {
            await _manager.Import(_type, _id());
            RaisePropertyChanged(nameof(IsOverriden));
            RevertCommand?.RaiseCanExecuteChanged();
        });

        ExportCommand = new RelayCommand(async () =>
        {
            await _manager.Export(_type, _id());
        });

        RevertCommand = new RelayCommand(async () =>
        {
            await _manager.RevertToDefault(_type, _id());
            RaisePropertyChanged(nameof(IsOverriden));
            RevertCommand?.RaiseCanExecuteChanged();
        },
        () => IsOverriden);
    }

    public bool IsOverriden
    {
        get => _manager.IsOverriden(_type, _id());
    }
    

    public ICommand ImportCommand { get; }
    public ICommand ExportCommand { get; }
    public RelayCommand RevertCommand { get; }
}
