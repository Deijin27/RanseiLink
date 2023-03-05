using DryIoc;
using RanseiLink.Core.RomFs;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Concrete;
using RanseiLink.Core.Services.DefaultPopulaters;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Services.ModPatchBuilders;
using RanseiLink.Core.Settings;
using System.IO;

namespace RanseiLink.Core
{

    public interface IModule
    {
        // Here we are using registration role of DryIoc Container for the builder
        void Load(IRegistrator builder);
    }

    public static class RegistratorExtensions
    {
        public static void RegisterModule(this IRegistrator registrator, IModule module) => module.Load(registrator);
    }

    public static class ContainerProvider
    {
        public static IContainer Container { get; set; }
        public static IModServiceGetterFactory ModServiceGetterFactory { get; set; }
    }

    public class CoreServiceModule : IModule
    {
        public void Load(IRegistrator builder)
        {
            RomFsFactory romFsFactory = file => new RomFs.RomFs(file);
            builder.RegisterInstance(romFsFactory);

            builder.Register<IMsgService, MsgService>(Reuse.Singleton);
            builder.Register<IFallbackDataProvider, FallbackDataProvider>(Reuse.Singleton);
            builder.Register<IModPatchingService, ModPatchingService>(Reuse.Singleton);

            string modFolder = Path.Combine(Constants.RootFolder, "Mods");
            builder.Register<IModManager, ModManager>(Reuse.Singleton,
                Made.Of(() => new ModManager(
                    modFolder,
                    Arg.Of<RomFsFactory>(),
                    Arg.Of<IMsgService>()
                )));

            string settingFolder = Path.Combine(Constants.RootFolder, "RanseiLinkSettings.xml");
            builder.Register<ISettingService, SettingService>(Reuse.Singleton,
                Made.Of(() => new SettingService(
                    settingFolder
                    )));

            builder.Register<IGraphicTypeDefaultPopulater, PkmdlDefaultPopulater>(Reuse.Singleton);
            builder.Register<IGraphicTypeDefaultPopulater, ScbgDefaultPopulater>(Reuse.Singleton);
            builder.Register<IGraphicTypeDefaultPopulater, StlDefaultPopulater>(Reuse.Singleton);
            builder.Register<IGraphicTypeDefaultPopulater, MiscDefaultPopulater>(Reuse.Singleton);

            var modServiceFactory = new ModServiceGetterFactory();
            modServiceFactory.AddModule(new CoreModServiceModule());
            ContainerProvider.ModServiceGetterFactory = modServiceFactory;
            builder.RegisterInstance<IModServiceGetterFactory>(modServiceFactory);
        }
    }

    public class CoreModServiceModule : IModule
    {
        public void Load(IRegistrator builder)
        {
            builder.Register<ICachedMsgBlockService, CachedMsgBlockService>(Reuse.Singleton);
            builder.Register<IAbilityService, AbilityService>(Reuse.Singleton);
            builder.Register<IBaseWarriorService, BaseWarriorService>(Reuse.Singleton);
            builder.Register<IBattleConfigService, BattleConfigService>(Reuse.Singleton);
            builder.Register<IBuildingService, BuildingService>(Reuse.Singleton);
            builder.Register<IEpisodeService, EpisodeService>(Reuse.Singleton);
            builder.Register<IEventSpeakerService, EventSpeakerService>(Reuse.Singleton);
            builder.Register<IGimmickObjectService, GimmickObjectService>(Reuse.Singleton);
            builder.Register<IGimmickRangeService, GimmickRangeService>(Reuse.Singleton);
            builder.Register<IGimmickService, GimmickService>(Reuse.Singleton);
            builder.Register<IItemService, ItemService>(Reuse.Singleton);
            builder.Register<IKingdomService, KingdomService>(Reuse.Singleton);
            builder.Register<IMapService, MapService>(Reuse.Singleton);
            builder.Register<IMaxLinkService, MaxLinkService>(Reuse.Singleton);
            builder.Register<IMoveAnimationService, MoveAnimationService>(Reuse.Singleton);
            builder.Register<IMoveRangeService, MoveRangeService>(Reuse.Singleton);
            builder.Register<IMoveService, MoveService>(Reuse.Singleton);
            builder.Register<IMsgBlockService, MsgBlockService>(Reuse.Singleton);
            builder.Register<IPokemonService, PokemonService>(Reuse.Singleton);
            builder.Register<IScenarioAppearPokemonService, ScenarioAppearPokemonService>(Reuse.Singleton);
            builder.Register<IScenarioKingdomService, ScenarioKingdomService>(Reuse.Singleton);
            builder.Register<IScenarioBuildingService, ScenarioBuildingService>(Reuse.Singleton);
            builder.Register<IScenarioPokemonService, ScenarioPokemonService>(Reuse.Singleton);
            builder.Register<IScenarioWarriorService, ScenarioWarriorService>(Reuse.Singleton);
            builder.Register<IWarriorSkillService, WarriorSkillService>(Reuse.Singleton);

            builder.Register<IOverrideDataProvider, OverrideDataProvider>(Reuse.Singleton);

            builder.Register<IPatchBuilder, GraphicsPatchBuilder>(Reuse.Singleton);
            builder.Register<IPatchBuilder, DataPatchBuilder>(Reuse.Singleton);
            builder.Register<IPatchBuilder, MsgPatchBuilder>(Reuse.Singleton);
            builder.Register<IPatchBuilder, MapPatchBuilder>(Reuse.Singleton);
            builder.Register<IPatchBuilder, MapModelPatchBuilder>(Reuse.Singleton);

            builder.Register<IGraphicTypePatchBuilder, StlPatchBuilder>(Reuse.Singleton);
            builder.Register<IGraphicTypePatchBuilder, ScbgPatchBuilder>(Reuse.Singleton);
            builder.Register<IGraphicTypePatchBuilder, PkmdlPatchBuilder>(Reuse.Singleton);
            builder.Register<IGraphicTypePatchBuilder, MiscPatchBuilder>(Reuse.Singleton);

            builder.Register<IBannerService, BannerService>(Reuse.Singleton);
        }
    }
}