#nullable enable
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.Services;

public interface IIdToNameService
{
    List<SelectorComboBoxItem> GetComboBoxItemsExceptDefault<TService>() where TService : IModelService;
    List<SelectorComboBoxItem> GetComboBoxItemsPlusDefault<TService>() where TService : IModelService;
    string IdToName<TService>(int id) where TService : IModelService;
}
