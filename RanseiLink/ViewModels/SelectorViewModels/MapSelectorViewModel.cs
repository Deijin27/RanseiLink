using RanseiLink.Core.Map;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Linq;

namespace RanseiLink.ViewModels;

public class MapSelectorViewModel : SelectorViewModelBase<MapName, Map, MapViewModel>
{
    public MapSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.Map, context.DataService.MapName.GetMaps().ToArray()) 
    {
        Selected = Items.First();
    }

    protected override MapViewModel NewViewModel(Map model) => new(model);
}
