using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core;
using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RanseiConsole.Dev
{
    [Command("dev test", Description = "Temporary, for testing purposes.")]
    public class TestCommand : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            //Testing.LogDataGroupings(@"C:\Users\Mia\Desktop\KingdomGroups", IterateKingdoms(), i => i.Name);

            //BuildEnum(console, IterateBuilding(), i => i.Name);

            //console.Output.WriteLine(Testing.GetBits(IterateItems().ElementAt((int)ItemId.dummy_4)));
            //Test4(console);
            //Test2(console);

            //BuildEnum(console, IterateEventSpeakers(), i => i.Name);

            //var potion = IteratePokemon().ElementAt((int)PokemonId.Eevee);
            //console.Output.WriteLine(Testing.GetBits(potion));
            //Test2(console, true);

            //for (int scenarioNumber = 0; scenarioNumber < 11; scenarioNumber++)
            //{
            //    console.Output.WriteLine($"Scenario {scenarioNumber} ------------------------------------------------");
            //    int count = 0;
            //    foreach (var sp in IterateScenarioPokemon(scenarioNumber))
            //    {
            //        console.Output.WriteLine($"{count.ToString().PadLeft(3, '0')} {sp.Pokemon,-12} {sp.Ability,-12} IVs: HP {sp.GetUInt32(1, 5, 0)} / Atk {sp.GetUInt32(1, 5, 5)} / Def {sp.GetUInt32(1, 5, 10)} / Spe {sp.GetUInt32(1, 5, 15)}");
            //    }
            //}

            //for (int scenarioNumber = 0; scenarioNumber < 11; scenarioNumber++)
            //{
            //    console.Output.WriteLine($"\nScenario {scenarioNumber} -----------------------------------------------\n");
            //    List<ScenarioPokemon> scenarioPokemons = new List<ScenarioPokemon>();

            //    foreach (var sp in IterateScenarioPokemon(scenarioNumber))
            //    {
            //        scenarioPokemons.Add(sp);
            //    }

            //    int count = 0;
            //    foreach (var sb in IterateScenarioBushou(scenarioNumber))
            //    {
            //        console.Output.Write($"{count.ToString().PadLeft(3, '0')}: {sb.Warrior,-12} ");
            //        if (sb.ScenarioPokemonIsDefault)
            //        {
            //            console.Output.WriteLine("<default>");
            //        }
            //        else
            //        {
            //            console.Output.WriteLine($"{sb.ScenarioPokemon} ({scenarioPokemons[(int)sb.ScenarioPokemon].Pokemon})");
            //        }
            //        count++;
            //    }
            //}

            //int count = 0;
            //var pokemonIds = EnumUtil.GetValues<PokemonId>().ToArray();
            //foreach (var sp in IterateMaxSync())
            //{
            //    string wid = ((WarriorId)count).ToString();
            //    console.Output.Write($"{wid}: ".PadLeft(14, ' '));
            //    List<string> items = new List<string>();
            //    foreach (var pid in pokemonIds)
            //    {
            //        if (sp.GetMaxSync(pid) == 100)
            //        {
            //            console.Output.Write($"{pid}, ");
            //        }
            //    }
            //    console.Output.WriteLine();
            //    count++;
            //}

            //var count = 0;
            //foreach (var ba in IterateBaseBushouPart2())
            //{
            //    var bdw = new BaseDataWindow(ba, 0xC);
            //    console.Output.WriteLine("0x" + $"{count++:x}".PadLeft(2, '0').ToUpperInvariant() + $": {bdw.GetPaddedUtf8String(0, 0xb)}");
            //}

            return default;
        }

        void Test4(IConsole console)
        {
            // log byte groups
            var int_idx = 8;
            var bitCount = 2;
            var shift = 11;

            var gpk = IterateItems()
                .OrderBy(i => i.Name)
                .GroupBy(p => p.GetUInt32(int_idx, bitCount, shift))
                .OrderBy(g => g.Key)
                .ToArray();

            foreach (var group in gpk)
            {
                console.Output.WriteLine($"{group.Key} = 0x{group.Key:x} = 0b{Convert.ToString(group.Key, 2).PadLeft(8, '0')} ---------------------------------------");
                console.Output.WriteLine();

                foreach (var pk in group)
                {
                    console.Output.WriteLine($"{pk.Name}");
                }

                console.Output.WriteLine();
            }
        }

        void Test0(IConsole console)
        {
            var gpk = IterateMoveEffects()
                .Select((me, c) => (me, (MoveEffectId)c))
                .OrderBy(tup => tup.Item2)
                .GroupBy(i => i.Item1.UnknownB)
                .OrderBy(g => g.Key).ToArray();

            foreach (var group in gpk)
            {
                console.Output.WriteLine($"{group.Key} = 0x{group.Key:x} = 0b{Convert.ToString(group.Key, 2).PadLeft(8, '0')} ---------------------------------------");
                console.Output.WriteLine();

                foreach (var pk in group)
                {
                    console.Output.WriteLine($"{pk.Item2}");
                }

                console.Output.WriteLine();
            }
        }

        void Test1(IConsole console)
        {
            // log byte groups
            var int_idx = 0;
            var bitCount = 8; // 5; //8
            var shift = 9; //17; //9;

            var sp = IterateScenarioPokemon(1).ToArray();

            var gpk = IterateScenarioBushou(1).OrderBy(i => i.Warrior)
                .GroupBy(p => p.GetUInt32(int_idx, bitCount, shift))
                .OrderBy(g => g.Key).ToArray();

            foreach (var group in gpk)
            {
                console.Output.WriteLine($"{group.Key} = 0x{group.Key:x} = 0b{Convert.ToString(group.Key, 2).PadLeft(8, '0')} ---------------------------------------");
                console.Output.WriteLine();

                foreach (var pk in group)
                {
                    string sptext = pk.ScenarioPokemonIsDefault ? "<default>" : sp[pk.ScenarioPokemon].Pokemon.ToString();
                    console.Output.WriteLine($"{pk.Warrior} - {sptext}");
                }

                console.Output.WriteLine();
            }
        }

        void Test2(IConsole console, bool both1and0 = false)
        {
            //log bit groups
            var int_idx = 7;
            var minShift = 27;
            var maxShift = 31;

            var pokes = IteratePokemon().ToArray();

            for (int shift = minShift; shift <= maxShift; shift++)
            {
                console.Output.Write($"\n{shift}: ");
                foreach (var p in pokes)
                {
                    if (p.GetUInt32(int_idx, 1, shift) == 1)
                    {
                        console.Output.Write(p.Name + ", ");
                    }
                }
                if (both1and0)
                {
                    console.Output.Write($"\n  (0): ");
                    foreach (var p in pokes)
                    {
                        if (p.GetUInt32(int_idx, 1, shift) != 1)
                        {
                            console.Output.Write(p.Name + ", ");
                        }
                    }
                }
            }



            console.Output.WriteLine();
        }

        void BuildEnum<T>(IConsole console, IEnumerable<T> dataItems, Func<T, string> nameSelector)
        {
            foreach (var i in dataItems)
            {
                console.Output.WriteLine(nameSelector(i).Replace(" ", "").Replace("'", "") + "," );
            }
        }

        static string DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ransei");

        static IEnumerable<Pokemon> IteratePokemon()
        {
            using var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, "Pokemon.dat")));

            int count = (int)(file.BaseStream.Length / Pokemon.DataLength);
            for (int i = 0; i < count; i++)
            {
                var pk = file.ReadBytes(Pokemon.DataLength);
                yield return new Pokemon(pk);
            }

        }

        static IEnumerable<Move> IterateMoves()
        {
            using var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, "Waza.dat")));

            int count = (int)(file.BaseStream.Length / Move.DataLength);
            for (int i = 0; i < count; i++)
            {
                var pk = file.ReadBytes(Move.DataLength);
                yield return new Move(pk);
            }

        }

        static IEnumerable<Ability> IterateAbilities()
        {
            using var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, "Tokusei.dat")));

            int count = (int)(file.BaseStream.Length / Ability.DataLength);
            for (int i = 0; i < count; i++)
            {
                var pk = file.ReadBytes(Ability.DataLength);
                yield return new Ability(pk);
            }

        }

        static IEnumerable<WarriorSkill> IterateWarriorSkill()
        {
            using var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, "Saihai.dat")));

            int count = (int)(file.BaseStream.Length / WarriorSkill.DataLength);
            for (int i = 0; i < count; i++)
            {
                var pk = file.ReadBytes(WarriorSkill.DataLength);
                yield return new WarriorSkill(pk);
            }

        }

        static IEnumerable<Gimmick> IterateGimmick()
        {
            using var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, "Gimmick.dat")));

            int count = (int)(file.BaseStream.Length / Gimmick.DataLength);
            for (int i = 0; i < count; i++)
            {
                var pk = file.ReadBytes(Gimmick.DataLength);
                yield return new Gimmick(pk);
            }

        }

        static IEnumerable<Building> IterateBuilding()
        {
            using var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, "Building.dat")));

            int count = (int)(file.BaseStream.Length / Building.DataLength);
            for (int i = 0; i < count; i++)
            {
                var pk = file.ReadBytes(Building.DataLength);
                yield return new Building(pk);
            }

        }
        
        static IEnumerable<ScenarioPokemon> IterateScenarioPokemon(int scenario)
        {
            using var file = new BinaryReader(File.OpenRead(@$"C:\Users\Mia\Desktop\ConquestData\Scenario\Scenario{scenario.ToString().PadLeft(2, '0')}\ScenarioPokemon.dat"));

            int count = (int)(file.BaseStream.Length / ScenarioPokemon.DataLength);
            for (int i = 0; i < count; i++)
            {
                var pk = file.ReadBytes(ScenarioPokemon.DataLength);
                yield return new ScenarioPokemon(pk);
            }

        }

        static IEnumerable<ScenarioWarrior> IterateScenarioBushou(int scenario)
        {
            using var file = new BinaryReader(File.OpenRead(@$"C:\Users\Mia\Desktop\ConquestData\Scenario\Scenario{scenario.ToString().PadLeft(2, '0')}\ScenarioBushou.dat"));

            int count = (int)(file.BaseStream.Length / ScenarioWarrior.DataLength);
            for (int i = 0; i < count; i++)
            {
                var pk = file.ReadBytes(ScenarioWarrior.DataLength);
                yield return new ScenarioWarrior(pk);
            }

        }

        static IEnumerable<Item> IterateItems()
        {
            using var file = new BinaryReader(File.OpenRead(@"C:\Users\Mia\Desktop\ConquestData\Item.dat"));

            int count = (int)(file.BaseStream.Length / Item.DataLength);
            for (int i = 0; i < count; i++)
            {
                var item = file.ReadBytes(Item.DataLength);
                yield return new Item(item);
            }
        }

        static IEnumerable<Kingdom> IterateKingdoms()
        {
            using var file = new BinaryReader(File.OpenRead(@"C:\Users\Mia\Desktop\ConquestData\Kuni.dat"));

            int count = (int)(file.BaseStream.Length / Kingdom.DataLength);
            for (int i = 0; i < count; i++)
            {
                var item = file.ReadBytes(Kingdom.DataLength);
                yield return new Kingdom(item);
            }
        }

        static IEnumerable<EventSpeaker> IterateEventSpeakers()
        {
            using var file = new BinaryReader(File.OpenRead(@"C:\Users\Mia\Desktop\ConquestData\EventSpeaker.dat"));

            int count = (int)(file.BaseStream.Length / EventSpeaker.DataLength);
            for (int i = 0; i < count; i++)
            {
                var item = file.ReadBytes(EventSpeaker.DataLength);
                yield return new EventSpeaker(item);
            }
        }

        static IEnumerable<WarriorMaxLink> IterateMaxSync()
        {
            using var file = new BinaryReader(File.OpenRead(@"C:\Users\Mia\Desktop\ConquestData\BaseBushouMaxSyncTable.dat"));

            int count = (int)(file.BaseStream.Length / WarriorMaxLink.DataLength);
            for (int i = 0; i < count; i++)
            {
                var item = file.ReadBytes(WarriorMaxLink.DataLength);
                yield return new WarriorMaxLink(item);
            }
        }

        static IEnumerable<BaseWarrior> IterateBaseBushouPart1()
        {
            using var file = new BinaryReader(File.OpenRead(@"C:\Users\Mia\Desktop\ConquestData\BaseBushou.dat"));

            for (int i = 0; i < 0xFC; i++)
            {
                yield return new BaseWarrior(file.ReadBytes(0x14));
            }
        }

        static IEnumerable<MoveEffect> IterateMoveEffects()
        {
            using var file = new BinaryReader(File.OpenRead(@"C:\Users\Mia\Desktop\ConquestData\WazaEffect.dat"));

            int count = (int)(file.BaseStream.Length / MoveEffect.DataLength);
            for (int i = 0; i < count; i++)
            {
                var item = file.ReadBytes(MoveEffect.DataLength);
                yield return new MoveEffect(item);
            }
        }
    }
}
