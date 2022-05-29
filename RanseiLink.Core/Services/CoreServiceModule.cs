using Autofac;
using RanseiLink.Core.RomFs;
using RanseiLink.Core.Services.Concrete;
using RanseiLink.Core.Services.DefaultPopulaters;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Services.ModPatchBuilders;
using RanseiLink.Core.Settings;
using System.IO;

namespace RanseiLink.Core.Services
{
    public static class ContainerProvider
    {
        public static IContainer Container { get; set; }
        public static IModServiceGetterFactory ModServiceGetterFactory { get; set; }
    }

    public class CoreServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RomFsFactory romFsFactory = file => new RomFs.RomFs(file);
            builder.RegisterInstance(romFsFactory).As<RomFsFactory>();

            builder.RegisterType<MsgService>().As<IMsgService>().SingleInstance();
            builder.RegisterType<FallbackSpriteProvider>().As<IFallbackSpriteProvider>().SingleInstance().WithParameter("defaultDataFolder", Constants.DefaultDataProviderFolder);
            builder.RegisterType<ModPatchingService>().As<IModPatchingService>().SingleInstance();
            builder.RegisterType<ModManager>().As<IModManager>().SingleInstance().WithParameter("modFolder", Path.Combine(Constants.RootFolder, "Mods"));
            builder.RegisterType<SettingService>().As<ISettingService>().SingleInstance().WithParameter("settingsFilePath", Path.Combine(Constants.RootFolder, "RanseiLinkSettings.xml"));

            builder.RegisterType<PkmdlDefaultPopulater>().As<IGraphicTypeDefaultPopulater>().SingleInstance();
            builder.RegisterType<ScbgDefaultPopulater>().As<IGraphicTypeDefaultPopulater>().SingleInstance();
            builder.RegisterType<StlDefaultPopulater>().As<IGraphicTypeDefaultPopulater>().SingleInstance();
            builder.RegisterType<MiscDefaultPopulater>().As<IGraphicTypeDefaultPopulater>().SingleInstance();

            var modServiceFactory = new ModServiceGetterFactory();
            modServiceFactory.AddModule(new CoreModServiceModule());
            ContainerProvider.ModServiceGetterFactory = modServiceFactory;
            builder.RegisterInstance(modServiceFactory).As<IModServiceGetterFactory>();
        }
    }

    public class CoreModServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AbilityService>().As<IAbilityService>().SingleInstance();
            builder.RegisterType<BaseWarriorService>().As<IBaseWarriorService>().SingleInstance();
            builder.RegisterType<BattleConfigService>().As<IBattleConfigService>().SingleInstance();
            builder.RegisterType<BuildingService>().As<IBuildingService>().SingleInstance();
            builder.RegisterType<EpisodeService>().As<IEpisodeService>().SingleInstance();
            builder.RegisterType<EventSpeakerService>().As<IEventSpeakerService>().SingleInstance();
            builder.RegisterType<GimmickObjectService>().As<IGimmickObjectService>().SingleInstance();
            builder.RegisterType<GimmickRangeService>().As<IGimmickRangeService>().SingleInstance();
            builder.RegisterType<GimmickService>().As<IGimmickService>().SingleInstance();
            builder.RegisterType<ItemService>().As<IItemService>().SingleInstance();
            builder.RegisterType<KingdomService>().As<IKingdomService>().SingleInstance();
            builder.RegisterType<MapService>().As<IMapService>().SingleInstance();
            builder.RegisterType<MaxLinkService>().As<IMaxLinkService>().SingleInstance();
            builder.RegisterType<MoveAnimationService>().As<IMoveAnimationService>().SingleInstance();
            builder.RegisterType<MoveRangeService>().As<IMoveRangeService>().SingleInstance();
            builder.RegisterType<MoveService>().As<IMoveService>().SingleInstance();
            builder.RegisterType<MsgBlockService>().As<IMsgBlockService>().SingleInstance();
            builder.RegisterType<PokemonService>().As<IPokemonService>().SingleInstance();
            builder.RegisterType<ScenarioAppearPokemonService>().As<IScenarioAppearPokemonService>().SingleInstance();
            builder.RegisterType<ScenarioKingdomService>().As<IScenarioKingdomService>().SingleInstance();
            builder.RegisterType<ScenarioPokemonService>().As<IScenarioPokemonService>().SingleInstance();
            builder.RegisterType<ScenarioWarriorService>().As<IScenarioWarriorService>().SingleInstance();
            builder.RegisterType<WarriorSkillService>().As<IWarriorSkillService>().SingleInstance();

            builder.RegisterType<OverrideSpriteProvider>().As<IOverrideSpriteProvider>().SingleInstance();

            builder.RegisterType<GraphicsPatchBuilder>().As<IPatchBuilder>().SingleInstance();
            builder.RegisterType<DataPatchBuilder>().As<IPatchBuilder>().SingleInstance();
            builder.RegisterType<MsgPatchBuilder>().As<IPatchBuilder>().SingleInstance();
            builder.RegisterType<MapPatchBuilder>().As<IPatchBuilder>().SingleInstance();

            builder.RegisterType<StlPatchBuilder>().As<IGraphicTypePatchBuilder>().SingleInstance();
            builder.RegisterType<ScbgPatchBuilder>().As<IGraphicTypePatchBuilder>().SingleInstance();
            builder.RegisterType<PkmdlPatchBuilder>().As<IGraphicTypePatchBuilder>().SingleInstance();
            builder.RegisterType<MiscPatchBuilder>().As<IGraphicTypePatchBuilder>().SingleInstance();

            builder.RegisterType<BannerService>().As<IBannerService>().SingleInstance();
        }
    }
}