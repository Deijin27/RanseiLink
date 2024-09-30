#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public class ScenarioAppearPokemonViewModel(IIdToNameService idToNameService) : ViewModelBase
{
    public void SetModel(ScenarioAppearPokemon model)
    {
        AppearItems.Clear();
        foreach (var id in EnumUtil.GetValuesExceptDefaults<PokemonId>())
        {
            var name = idToNameService.IdToName<IPokemonService>((int)id);
            var vm = new CheckBoxViewModel(name, 
                () => model.GetCanAppear(id),
                v => model.SetCanAppear(id, v)
                );
            AppearItems.Add(vm);  
        }
    }

    public ObservableCollection<CheckBoxViewModel> AppearItems { get; } = [];
}
