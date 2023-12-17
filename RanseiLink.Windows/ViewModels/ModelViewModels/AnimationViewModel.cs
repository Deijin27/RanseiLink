#nullable enable
using RanseiLink.Core.Services;
using RanseiLink.Windows.Services;
using System.Windows.Input;

namespace RanseiLink.Windows.ViewModels;

public class AnimationViewModel : ViewModelBase
{
    private readonly IAnimGuiManager _manager;
    private readonly AnimationTypeId _type;
    private readonly int _id;

    public AnimationViewModel(IAnimGuiManager manager, AnimationTypeId type, int id)
    {
        _manager = manager;
        _type = type;
        _id = id;

        ImportCommand = new RelayCommand(() =>
        {
            _manager.Import(_type, _id);
            RaisePropertyChanged(nameof(IsOverriden));
        });
        ExportCommand = new RelayCommand(() => _manager.Export(_type, _id));
        RevertCommand = new RelayCommand(() =>
        {
            _manager.RevertToDefault(_type, _id);
            RaisePropertyChanged(nameof(IsOverriden));
        },
        () => IsOverriden);
    }

    public bool IsOverriden
    {
        get => _manager.IsOverriden(_type, _id);
    }
    

    public ICommand ImportCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand RevertCommand { get; }
}
