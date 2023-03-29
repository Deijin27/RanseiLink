using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Windows.ViewModels;
using System.Collections.Generic;

namespace RanseiLink.Windows.Services;

public interface IIdToNameService
{
    List<SelectorComboBoxItem> GetComboBoxItemsExceptDefault<TService>() where TService : IModelService;
    List<SelectorComboBoxItem> GetComboBoxItemsPlusDefault<TService>() where TService : IModelService;
    string IdToName<TService>(int id) where TService : IModelService;
}
