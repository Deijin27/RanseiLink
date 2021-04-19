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
    public class TestCommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            var int_idx = 7;
            var shift = 18;
            var mask = 0b111111111;

            var gpk = IteratePokemon().OrderBy(i => i.Name)
                .GroupBy(p => (p.GetUInt32(int_idx * 4) >> shift) & mask)
                .OrderBy(g => g.Key).ToArray();

            foreach (var group in gpk)
            {
                console.Output.WriteLine($"{group.Key} = 0x{group.Key:x} = 0b{Convert.ToString(group.Key, 2).PadLeft(8, '0')} ---------------------------------------");
                console.Output.WriteLine();

                foreach (var pk in group)
                {
                    console.Render(pk, 0);
                }

                console.Output.WriteLine();
            }

            return default;
        }

        const string DataFolder = @"C:\Users\Mia\Desktop\PokemonRomEditing\Conquest";

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
    }
}
