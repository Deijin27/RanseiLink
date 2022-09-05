using RanseiLink.Core.Services.ModelServices;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.ViewModels;

public class SelectorComboBoxItem
{
    public SelectorComboBoxItem(int id, string name)
    {
        Id = id;
        IdString = id.ToString().PadLeft(3, '0');
        IdAndName = $"{IdString} - {name}";
        Name = name;
    }
    public int Id { get; }
    public string IdString { get; }
    public string IdAndName { get; }
    public string Name { get; }
}

public static class SelectorComboBoxItemExtensions
{
    public static List<SelectorComboBoxItem> GetComboBoxItemsExceptDefault<TModelService>(this TModelService service) where TModelService : IModelService
    {
        return service.ValidIds().Select(id => new SelectorComboBoxItem(id, service.IdToName(id))).ToList();
    }

    public static List<SelectorComboBoxItem> GetComboBoxItemsPlusDefault<TModelService>(this TModelService service) where TModelService : IModelService
    {
        var items = service.ValidIds().Select(id => new SelectorComboBoxItem(id, service.IdToName(id)));
        if (service.TryGetDefaultId(out var defaultId))
        {
            items = items.Append(new SelectorComboBoxItem(defaultId, "Default"));
        }
        return items.ToList();
    }
}