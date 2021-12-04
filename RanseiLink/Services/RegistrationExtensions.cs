using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using RanseiLink.Services.Concrete;
using RanseiLink.ViewModels;

namespace RanseiLink.Services;

public static class RegistrationExtensions
{
    public static void RegisterWpfServices(this IServiceContainer container)
    {
        var settingsService = container.Resolve<ISettingsService>();
        var pluginFormLoader = container.Resolve<IPluginFormLoader>();

        // register services

        container.RegisterLazySingleton<IDialogService>(() => new DialogService(settingsService));
        container.RegisterLazySingleton<IPluginService>(() => new PluginService(pluginFormLoader));
        container.RegisterLazySingleton<IThemeService>(() => new ThemeService(settingsService));
        container.RegisterLazySingleton<IExternalService>(() => new ExternalService());

        // register view models

        container.RegisterLazySingleton(() => new MainWindowViewModel(container));
        container.RegisterLazySingleton(() => new ModSelectionViewModel(container));
        container.RegisterSingleton<MainEditorViewModelFactory>(mod => new MainEditorViewModel(container, mod));

        container.RegisterSingleton<AbilityViewModelFactory>(i => new AbilityViewModel(i));
        container.RegisterSingleton<BaseWarriorViewModelFactory>(i => new BaseWarriorViewModel(i));
        container.RegisterSingleton<EventSpeakerViewModelFactory>(i => new EventSpeakerViewModel(i));
        container.RegisterSingleton<MaxLinkViewModelFactory>(i => new MaxLinkViewModel(i));
        container.RegisterSingleton<MoveRangeViewModelFactory>(i => new MoveRangeViewModel(i));
        container.RegisterSingleton<MoveViewModelFactory>(i => new MoveViewModel(container, i));
        container.RegisterSingleton<PokemonViewModelFactory>(i => new PokemonViewModel(i));
        container.RegisterSingleton<ScenarioAppearPokemonViewModelFactory>(i => new ScenarioAppearPokemonViewModel(i));
        container.RegisterSingleton<ScenarioWarriorViewModelFactory>((sid, dataService, model) => new ScenarioWarriorViewModel(container, dataService, sid, model));
        container.RegisterSingleton<WarriorSkillViewModelFactory>(i => new WarriorSkillViewModel(i));
        
        container.RegisterSingleton<ScenarioKingdomViewModelFactory>(i => new ScenarioKingdomViewModel(i));
        container.RegisterSingleton<ScenarioPokemonViewModelFactory>(model => new ScenarioPokemonViewModel(model));
        
        container.RegisterSingleton<WarriorNameTableViewModelFactory>(i => new WarriorNameTableViewModel(container, i));
        container.RegisterSingleton<EvolutionTableViewModelFactory>(i => new EvolutionTableViewModel(container, i));

        container.RegisterSingleton<AbilitySelectorViewModelFactory>(i => new AbilitySelectorViewModel(container, i));
        container.RegisterSingleton<BaseWarriorSelectorViewModelFactory>(i => new BaseWarriorSelectorViewModel(container, i));
        container.RegisterSingleton<EventSpeakerSelectorViewModelFactory>(i => new EventSpeakerSelectorViewModel(container, i));
        container.RegisterSingleton<MaxLinkSelectorViewModelFactory>(i => new MaxLinkSelectorViewModel(container, i));
        container.RegisterSingleton<MoveRangeSelectorViewModelFactory>(i => new MoveRangeSelectorViewModel(container, i));
        container.RegisterSingleton<MoveSelectorViewModelFactory>(i => new MoveSelectorViewModel(container, i));
        container.RegisterSingleton<PokemonSelectorViewModelFactory>(i => new PokemonSelectorViewModel(container, i));
        container.RegisterSingleton<ScenarioAppearPokemonSelectorViewModelFactory>(i => new ScenarioAppearPokemonSelectorViewModel(container, i));
        container.RegisterSingleton<ScenarioKingdomSelectorViewModelFactory>(i => new ScenarioKingdomSelectorViewModel(container, i));
        container.RegisterSingleton<WarriorSkillSelectorViewModelFactory>(i => new WarriorSkillSelectorViewModel(container, i));

        container.RegisterSingleton<ScenarioPokemonSelectorViewModelFactory>(i => new ScenarioPokemonSelectorViewModel(container, i));
        container.RegisterSingleton<ScenarioWarriorSelectorViewModelFactory>(i => new ScenarioWarriorSelectorViewModel(container, i));


    }
}
