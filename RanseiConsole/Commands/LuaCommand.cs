using CliFx.Attributes;
using CliFx.Infrastructure;
using Core;
using Core.Enums;
using Core.Services;
using NLua;
using RanseiConsole.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RanseiConsole.Commands
{
    [Command("lua", Description = "Run given lua script.")]
    public class LuaCommand : BaseCommand
    {
        public LuaCommand(IServiceContainer container) : base(container) { }
        public LuaCommand() : base() { }

        [CommandParameter(0, Description = "Absolute path to entry point script", Name = "path")]
        public string FilePath { get; set; }

        public override ValueTask ExecuteAsync(IConsole console)
        {
            var currentModService = Container.Resolve<ICurrentModService>();

            if (!currentModService.TryGetDataService(console, out IDataService dataService))
            {
                return default;
            }

            Directory.SetCurrentDirectory(Path.GetDirectoryName(FilePath));

            string fileName = Path.GetFileName(FilePath);

            using (var lua = new Lua())
            {
                lua.LoadCLRPackage();
                lua.DoString(@"
                    import('Core', 'Core.Enums')
                    import('Core', 'Core.Models')
                    import = function () end
                ");

                lua["service"] = dataService;

                // add all enum id items to the state to allow users to enumerate them with luanet.each
                void AddEnumToState<T>()
                {
                    string typeName = typeof(T).Name;
                    lua.DoString(typeName + "s = { " +
                        string.Join(", ", EnumUtil.GetValues<T>().Select(i => $"{typeName}.{i}"))
                        + " }"
                        );
                }

                AddEnumToState<AbilityEffectId>();
                AddEnumToState<AbilityId>();
                AddEnumToState<BuildingId>();
                AddEnumToState<EventSpeakerId>();
                AddEnumToState<EvolutionConditionId>();
                AddEnumToState<GenderId>();
                AddEnumToState<GimmickId>();
                AddEnumToState<ItemId>();
                AddEnumToState<KingdomId>();
                AddEnumToState<MoveEffectId>();
                AddEnumToState<MoveId>();
                AddEnumToState<MoveRangeId>();
                AddEnumToState<PokemonId>();
                AddEnumToState<ScenarioId>();
                AddEnumToState<TypeId>();
                AddEnumToState<WarriorId>();
                AddEnumToState<WarriorSkillEffectId>();
                AddEnumToState<WarriorSkillId>();
                AddEnumToState<WarriorSkillTargetId>();

                var luaFunctions = new LuaFunctions();
                lua.RegisterFunction("using", luaFunctions, typeof(LuaFunctions).GetMethod(nameof(LuaFunctions.Using)));

                lua.DoFile(fileName);

                luaFunctions.DisposeAll();
            }

            console.Output.WriteLine("Script executed successfully.");
            return default;
        }
    }

    internal class LuaFunctions
    {
        public IDisposable Using(IDisposable disposable)
        {
            Disposables.Add(disposable);
            return disposable;
        }

        private readonly IList<IDisposable> Disposables = new List<IDisposable>();

        public void DisposeAll()
        {
            foreach (var disposable in Disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
