using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Linq;

namespace RanseiLink.ViewModels;

public delegate KingdomSelectorViewModel KingdomSelectorViewModelFactory(IEditorContext context);

public class KingdomSelectorViewModel : SelectorViewModelBase<KingdomId, IKingdom, KingdomViewModel>
{
    private readonly KingdomViewModelFactory _factory;
    private readonly IEditorContext _context;

    public KingdomSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.Kingdom, EnumUtil.GetValuesExceptDefaults<KingdomId>().ToArray()) 
    {
        _context = context;
        _factory = container.Resolve<KingdomViewModelFactory>();
        Selected = KingdomId.Aurora;
    }

    protected override KingdomViewModel NewViewModel(IKingdom model) => _factory(Selected, model, _context);
}
