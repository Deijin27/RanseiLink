using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.ViewModels;

public delegate MoveSelectorViewModel MoveSelectorViewModelFactory(IMoveService service);

public class MoveSelectorViewModel : SelectorViewModelBase<MoveId, IMove, MoveViewModel>
{
    private readonly MoveViewModelFactory _factory;
    public MoveSelectorViewModel(IServiceContainer container, IMoveService dataService)
        : base(container, dataService) 
    { 
        _factory = container.Resolve<MoveViewModelFactory>();
        Selected = MoveId.Splash;
    }

    protected override MoveViewModel NewViewModel(IMove model) => _factory(model);
}
