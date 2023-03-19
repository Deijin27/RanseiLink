#nullable enable
using DryIoc;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Concrete;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Services.ModPatchBuilders;

namespace RanseiLink.Core
{
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

            builder.Register<IStrengthService, StrengthService>(Reuse.Singleton);
            builder.Register<IPokemonAnimationService, PokemonAnimationService>(Reuse.Singleton);

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