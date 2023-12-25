using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Windows.ViewModels;

public class SelectorComboBoxItem : ViewModelBase
{
    private string _name;
    private string _idAndName;
    public SelectorComboBoxItem(int id, string name)
    {
        Id = id;
        IdString = id.ToString().PadLeft(3, '0');
        _idAndName = $"{IdString} - {name}";
        _name = name;
    }
    public int Id { get; }
    public string IdString { get; }
    public string IdAndName => _idAndName;
    public string Name => _name;

    public void UpdateName(string newValue)
    {
        _name = newValue;
        _idAndName = $"{IdString} - {newValue}";
        RaisePropertyChanged(nameof(Name));
        RaisePropertyChanged(nameof(IdAndName));
    }
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