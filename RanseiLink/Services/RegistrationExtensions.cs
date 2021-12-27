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

        container.RegisterSingleton<AbilityViewModelFactory>((i, m, c) => new AbilityViewModel(i, m, c));
        container.RegisterSingleton<BaseWarriorViewModelFactory>((i, c) => new BaseWarriorViewModel(i, c));
        container.RegisterSingleton<BuildingViewModelFactory>(i => new BuildingViewModel(i));
        container.RegisterSingleton<EventSpeakerViewModelFactory>(i => new EventSpeakerViewModel(i));
        container.RegisterSingleton<ItemViewModelFactory>((i, m, c) => new ItemViewModel(i, m, c));
        container.RegisterSingleton<KingdomViewModelFactory>((i, m, c) => new KingdomViewModel(i, m, c));
        container.RegisterSingleton<MaxLinkViewModelFactory>(i => new MaxLinkViewModel(i));
        container.RegisterSingleton<MoveRangeViewModelFactory>(i => new MoveRangeViewModel(i));
        container.RegisterSingleton<MoveViewModelFactory>((i, m, c) => new MoveViewModel(i, container, m, c));
        container.RegisterSingleton<PokemonViewModelFactory>((i, c) => new PokemonViewModel(i, c));
        container.RegisterSingleton<ScenarioAppearPokemonViewModelFactory>(i => new ScenarioAppearPokemonViewModel(i));
        container.RegisterSingleton<ScenarioWarriorViewModelFactory>((sid, dataService, model) => new ScenarioWarriorViewModel(container, dataService, sid, model));
        container.RegisterSingleton<WarriorSkillViewModelFactory>((i, m, c) => new WarriorSkillViewModel(i, m, c));
        container.RegisterSingleton<ScenarioKingdomViewModelFactory>(i => new ScenarioKingdomViewModel(i));
        container.RegisterSingleton<ScenarioPokemonViewModelFactory>((model, c, scenario, id) => new ScenarioPokemonViewModel(model, c, scenario, id));
        container.RegisterSingleton<GimmickViewModelFactory>((i, m, c) => new GimmickViewModel(i, container, m, c));

        container.RegisterSingleton<EditorModuleRegistrationFunction>(editor => 
        {
            editor.AddModule<AbilitySelectorEditorModule>();
            editor.AddModule<MsgGridEditorModule>();
            editor.AddModule<AbilityGridEditorModule>();
            editor.AddModule<BaseWarriorSelectorEditorModule>();
            editor.AddModule<BaseWarriorGridEditorModule>();
            editor.AddModule<BuildingSelectorEditorModule>();
            editor.AddModule<EventSpeakerSelectorEditorModule>();
            editor.AddModule<EvolutionTableEditorModule>();
            editor.AddModule<GimmickSelectorEditorModule>();
            editor.AddModule<GimmickGridEditorModule>();
            editor.AddModule<ItemSelectorEditorModule>();
            editor.AddModule<KingdomSelectorEditorModule>();
            editor.AddModule<MaxLinkSelectorEditorModule>();
            editor.AddModule<MoveRangeSelectorEditorModule>();
            editor.AddModule<MoveSelectorEditorModule>();
            editor.AddModule<MoveGridEditorModule>();
            editor.AddModule<PokemonSelectorEditorModule>();
            editor.AddModule<PokemonGridEditorModule>();
            editor.AddModule<ScenarioAppearPokemonSelectorEditorModule>();
            editor.AddModule<ScenarioKingdomSelectorEditorModule>();
            editor.AddModule<ScenarioPokemonSelectorEditorModule>();
            editor.AddModule<ScenarioWarriorSelectorEditorModule>();
            editor.AddModule<ScenarioWarriorGridEditorModule>();
            editor.AddModule<WarriorNameTableEditorModule>();
            editor.AddModule<WarriorSkillSelectorEditorModule>();
            editor.AddModule<WarriorSkillGridEditorModule>();

            editor.LoadModuleOrderFromSetting();
        });
    }
}
