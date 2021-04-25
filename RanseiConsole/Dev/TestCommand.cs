using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
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
            //Testing.LogDataGroupings(@"C:\Users\Mia\Desktop\SaihaiGroups", IterateSaihai(), i => i.Name);

            //BuildEnum(console, IterateSaihai(), i => i.Name);

            //console.Output.WriteLine(Testing.GetBits(IterateSaihai().First()));

            Test1(console);

            return default;
        }

        void Test1(IConsole console)
        {
            var int_idx = 6;
            var shift = 10;
            var mask = 0b111111111;

            var gpk = IterateSaihai().OrderBy(i => i.Name)
                .GroupBy(p => (p.GetUInt32(int_idx * 4) >> shift) & mask)
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

        void BuildEnum<T>(IConsole console, IEnumerable<T> dataItems, Func<T, string> nameSelector)
        {
            foreach (var i in dataItems)
            {
                console.Output.WriteLine(nameSelector(i).Replace(" ", "") + "," );
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
    }
}
