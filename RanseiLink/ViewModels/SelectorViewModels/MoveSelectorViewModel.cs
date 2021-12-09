using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate MoveSelectorViewModel MoveSelectorViewModelFactory(IEditorContext context);

public class MoveSelectorViewModel : SelectorViewModelBase<MoveId, IMove, MoveViewModel>
{
    private readonly MoveViewModelFactory _factory;
    public MoveSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.Move) 
    { 
        _factory = container.Resolve<MoveViewModelFactory>();
        Selected = MoveId.Splash;
    }

    protected override MoveViewModel NewViewModel(IMove model) => _factory(model);
}
