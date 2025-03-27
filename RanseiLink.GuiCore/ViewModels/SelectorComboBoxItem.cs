using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public class NamedModelSelectorComboBoxItem : SelectorComboBoxItem
{
    private readonly INamedModel _model;

    public NamedModelSelectorComboBoxItem(int id, INamedModel model) : base(id, model.Name)
    {
        _model = model;
        _model.NameChanged += Model_NameChanged;
    }

    private void Model_NameChanged(object? sender, EventArgs e)
    {
        UpdateName(_model.Name);
    }
}

public class NicknamedSelectorComboBoxItem : SelectorComboBoxItem
{
    private readonly string _nicknameCategory;

    public NicknamedSelectorComboBoxItem(int id, INicknameService nicknameService, string nicknameCategory) 
        : base(id, nicknameService.GetNickname(nicknameCategory, id))
    {
        _nicknameCategory = nicknameCategory;
        nicknameService.NicknameChanged += NicknameService_NicknameChanged;
    }

    public NicknamedSelectorComboBoxItem(int id, INicknameService nicknameService, string nicknameCategory, string idString)
        : base(id, idString, nicknameService.GetNickname(nicknameCategory, id))
    {
        _nicknameCategory = nicknameCategory;
        nicknameService.NicknameChanged += NicknameService_NicknameChanged;
    }

    private void NicknameService_NicknameChanged(object? sender, NicknameChangedArgs e)
    {
        if (e.Category == _nicknameCategory && e.Id == Id)
        {
            UpdateName(e.NewName);
        }
    }
}


public class SelectorComboBoxItem : ViewModelBase
{
    private string _name;
    private string _idAndName;
    public SelectorComboBoxItem(int id, string name) : this(id, id.ToString().PadLeft(3, '0'), name)
    {
    }

    public SelectorComboBoxItem(int id, string idString, string name)
    {
        Id = id;
        IdString = idString;
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
        List<SelectorComboBoxItem> items = [];
        foreach (var id in service.ValidIds())
        {
            var item = service.RetrieveObject(id);
            SelectorComboBoxItem comboItem;
            if (item is INamedModel namedModel)
            {
                comboItem = new NamedModelSelectorComboBoxItem(id, namedModel);
            }
            else
            {
               comboItem = new SelectorComboBoxItem(id, service.IdToName(id));
            }
            items.Add(comboItem);
        }
        return items;
    }

    public static List<SelectorComboBoxItem> GetComboBoxItemsPlusDefault<TModelService>(this TModelService service) where TModelService : IModelService
    {
        var items = GetComboBoxItemsExceptDefault(service);
        if (service.TryGetDefaultId(out var defaultId))
        {
            items.Add(new SelectorComboBoxItem(defaultId, "Default"));
        }
        return items;
    }
}