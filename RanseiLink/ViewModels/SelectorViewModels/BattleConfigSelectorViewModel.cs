using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Linq;

namespace RanseiLink.ViewModels;

public class BattleConfigSelectorViewModel : SelectorViewModelBase<BattleConfigId, IBattleConfig, BattleConfigViewModel>
{
    private readonly BattleConfigViewModelFactory _factory;
    private readonly IEditorContext _context;

    public BattleConfigSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.BattleConfig, EnumUtil.GetValuesExceptDefaults<BattleConfigId>().ToArray()) 
    {
        _context = context;
        _factory = container.Resolve<BattleConfigViewModelFactory>();
        Selected = BattleConfigId.Aurora;
    }

    protected override BattleConfigViewModel NewViewModel(IBattleConfig model) => _factory(Selected, model, _context);
}
