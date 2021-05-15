using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RanseiConsole.Dev
{
    [Command("dev test", Description = "Temporary, for testing purposes.")]
    public class TestCommand : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            Testing.LogDataGroupings(@"C:\Users\Mia\Desktop\ItemGroups", IterateItems(), i => i.Name);

            //BuildEnum(console, IterateBuilding(), i => i.Name);

            //console.Output.WriteLine(Testing.GetBits(IterateBuilding().First()));

            //Test2(console);

            //BuildEnum(console, IterateItems(), i => i.Name);

            return default;
        }

        void Test1(IConsole console)
        {
            // log byte groups
            var int_idx = 10;
            var shift = 2;
            var bitCount = 1;

            var gpk = IteratePokemon().OrderBy(i => i.ToString())
                .GroupBy(p => p.GetUInt32(int_idx, bitCount, shift))
                .OrderBy(g => g.Key).ToArray();

            foreach (var group in gpk)
            {
                console.Output.WriteLine($"{group.Key} = 0x{group.Key:x} = 0b{Convert.ToString(group.Key, 2).PadLeft(8, '0')} ---------------------------------------");
                console.Output.WriteLine();

                foreach (var pk in group)
                {
                    console.Output.WriteLine(pk.Name);
                }

                console.Output.WriteLine();
            }
        }

        void Test2(IConsole console)
        {
            //log bit groups
            var int_idx = 9;
            var minShift = 0;
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

        static IEnumerable<Saihai> IterateSaihai()
        {
            using var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, "Saihai.dat")));

            int count = (int)(file.BaseStream.Length / Saihai.DataLength);
            for (int i = 0; i < count; i++)
            {
                var pk = file.ReadBytes(Saihai.DataLength);
                yield return new Saihai(pk);
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
        
        static IEnumerable<ScenarioPokemon> IterateScenarioPokemon()
        {
            using var file = new BinaryReader(File.OpenRead(@"C:\Users\Mia\Desktop\ConquestData\Scenario\Scenario00\ScenarioPokemon.dat"));

            int count = (int)(file.BaseStream.Length / ScenarioPokemon.DataLength);
            for (int i = 0; i < count; i++)
            {
                var pk = file.ReadBytes(ScenarioPokemon.DataLength);
                yield return new ScenarioPokemon(pk);
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
    }
}
