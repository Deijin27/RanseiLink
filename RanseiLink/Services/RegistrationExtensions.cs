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
        container.RegisterSingleton<EditorContextFactory>((dataService, editor) => new EditorContext(dataService, editor));
        container.RegisterSingleton<ModListItemViewModelFactory>((parent, mod) => new ModListItemViewModel(parent, mod, container));

        container.RegisterSingleton<AbilityViewModelFactory>(i => new AbilityViewModel(i));
        container.RegisterSingleton<BaseWarriorViewModelFactory>((i, c) => new BaseWarriorViewModel(i, c));
        container.RegisterSingleton<BuildingViewModelFactory>(i => new BuildingViewModel(i));
        container.RegisterSingleton<EventSpeakerViewModelFactory>(i => new EventSpeakerViewModel(i));
        container.RegisterSingleton<ItemViewModelFactory>(i => new ItemViewModel(i));
        container.RegisterSingleton<MaxLinkViewModelFactory>(i => new MaxLinkViewModel(i));
        container.RegisterSingleton<MoveRangeViewModelFactory>(i => new MoveRangeViewModel(i));
        container.RegisterSingleton<MoveViewModelFactory>((i, c) => new MoveViewModel(container, i, c));
        container.RegisterSingleton<PokemonViewModelFactory>((i, c) => new PokemonViewModel(i, c));
        container.RegisterSingleton<ScenarioAppearPokemonViewModelFactory>(i => new ScenarioAppearPokemonViewModel(i));
        container.RegisterSingleton<ScenarioWarriorViewModelFactory>((sid, dataService, model) => new ScenarioWarriorViewModel(container, dataService, sid, model));
        container.RegisterSingleton<WarriorSkillViewModelFactory>(i => new WarriorSkillViewModel(i));
        
        container.RegisterSingleton<ScenarioKingdomViewModelFactory>(i => new ScenarioKingdomViewModel(i));
        container.RegisterSingleton<ScenarioPokemonViewModelFactory>((model, c) => new ScenarioPokemonViewModel(model, c));
        
        container.RegisterSingleton<WarriorNameTableViewModelFactory>(i => new WarriorNameTableViewModel(container, i));
        container.RegisterSingleton<EvolutionTableViewModelFactory>(i => new EvolutionTableViewModel(container, i));

        container.RegisterSingleton<AbilitySelectorViewModelFactory>(i => new AbilitySelectorViewModel(container, i));
        container.RegisterSingleton<BaseWarriorSelectorViewModelFactory>(i => new BaseWarriorSelectorViewModel(container, i));
        container.RegisterSingleton<BuildingSelectorViewModelFactory>(i => new BuildingSelectorViewModel(container, i));
        container.RegisterSingleton<EventSpeakerSelectorViewModelFactory>(i => new EventSpeakerSelectorViewModel(container, i));
        container.RegisterSingleton<ItemSelectorViewModelFactory>(i => new ItemSelectorViewModel(container, i));
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
