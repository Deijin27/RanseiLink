using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using NLua;
using System.IO;
using System.Linq;

namespace RanseiLink.Console.Services;

public interface ILuaService
{
    void RunScript(string scriptFilePath);
}

public class LuaService : ILuaService
{
    private readonly IModServiceContainer _modServiceContainer;
    public LuaService(IModServiceContainer modServiceContainer)
    {
        _modServiceContainer = modServiceContainer;
    }

    public void RunScript(string scriptFilePath)
    {
        Directory.SetCurrentDirectory(Path.GetDirectoryName(scriptFilePath));

        string fileName = Path.GetFileName(scriptFilePath);

        using (var lua = new Lua())
        {
            lua.LoadCLRPackage();

            string namespaceName = $"{nameof(RanseiLink)}.{nameof(Core)}";

            lua.DoString(@$"
                    import('{namespaceName}', '{namespaceName}.{nameof(Core.Enums)}')
                    import('{namespaceName}', '{namespaceName}.{nameof(Core.Models)}')
                    import = function () end
                ");

            lua["service"] = _modServiceContainer;
            lua.RegisterFunction("toInt", typeof(LuaService).GetMethod(nameof(ConvertToInt)));

            // add all enum id items to the state to allow users to enumerate them with luanet.each
            void AddEnumToState<T>()
            {
                string typeName = typeof(T).Name;
                lua.DoString(typeName + "s = { " +
                    string.Join(", ", EnumUtil.GetValuesExceptDefaultsWithFallback<T>().Select(i => $"{typeName}.{i}"))
                    + " }"
                    );
            }

            AddEnumToState<AbilityEffectId>();
            AddEnumToState<AbilityId>();
            AddEnumToState<BuildingId>();
            AddEnumToState<EpisodeId>();
            AddEnumToState<EventSpeakerId>();
            AddEnumToState<EvolutionConditionId>();
            AddEnumToState<GenderId>();
            AddEnumToState<GimmickId>();
            AddEnumToState<ItemId>();
            AddEnumToState<KingdomId>();
            AddEnumToState<MoveAnimationId>();
            AddEnumToState<MoveEffectId>();
            AddEnumToState<MoveId>();
            AddEnumToState<MoveMovementAnimationId>();
            AddEnumToState<MoveRangeId>();
            AddEnumToState<PokemonId>();
            AddEnumToState<RankUpConditionId>();
            AddEnumToState<ScenarioId>();
            AddEnumToState<TypeId>();
            AddEnumToState<WarriorClassId>();
            AddEnumToState<WarriorId>();
            AddEnumToState<WarriorLineId>();
            AddEnumToState<WarriorSkillEffectId>();
            AddEnumToState<WarriorSkillId>();
            AddEnumToState<WarriorSkillTargetId>();
            AddEnumToState<SpeakerId>();
            AddEnumToState<BattleConfigId>();
            AddEnumToState<GimmickRangeId>();
            AddEnumToState<GimmickObjectId>();
            AddEnumToState<EpisodeId>();

            lua.DoFile(fileName);
        }
    }

    public static int ConvertToInt(object value)
    {
        return (int)value;
    }
}