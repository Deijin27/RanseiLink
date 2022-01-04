using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RanseiLink.Console.Dev;

[Command("dev test", Description = "Temporary, for testing purposes.")]
public class TestCommand : ICommand
{
    public ValueTask ExecuteAsync(IConsole console)
    {
        //Test6(console);
        //Testing.LogDataGroupings(@"C:\Users\Mia\Desktop\BuildingGroups", IterateBuilding(), (building, id) => $"{(BuildingId)id} (Name = {building.Name}, Kingdom = {building.Kingdom})");

        //BuildEnum(console, IterateBuilding(), i => i.Name);

        //console.Output.WriteLine(Testing.GetBits(IterateScenarioBushou(0).ElementAt(1).Data));
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

        //var used = new List<WarriorSpriteId>();
        //WarriorId count = 0;
        //var items = new List<(WarriorId, WarriorSpriteId)>();
        //foreach (var ba in IterateBaseBushouPart1())
        //{
        //    //console.Output.WriteLine($"{(WarriorId)count++}".PadLeft(15, ' ') + $": {ba.Id} ({(int)ba.Id})");
        //    if (!used.Contains(ba.Sprite))
        //    {
        //        used.Add(ba.Sprite);
        //        items.Add((count, ba.Sprite));
        //    }
        //    count++;
        //}

        //foreach (var (wid, wsid) in items.OrderBy(i => i.Item2))
        //{
        //    console.Output.WriteLine($"{wid}, // {wsid}");
        //}

        //console.Output.WriteLine(Testing.GetBits(IterateBaseBushouPart1().ElementAt((int)WarriorId.PlayerMale_2).Data));
        //console.Output.WriteLine();
        //console.Output.WriteLine(Testing.GetBits(IterateBaseBushouPart1().ElementAt((int)WarriorId.PlayerMale_3).Data));

        int count = 0;
        var sb = new StringBuilder();
        foreach (var trainer in IterateTrainers())
        {
            sb.AppendLine($"{count.ToString().PadLeft(3, '0')}: {trainer.GetPaddedUtf8String(0, 0x13)}");
            count++;
        }

        File.WriteAllText(@"C:\Users\Mia\Desktop\New Text Document.txt", sb.ToString());

        return default;
    }

    void Test6(IConsole console)
    {
        // log byte groups
        var int_idx = 7;
        var shift = 26;
        var bitCount = 5;

        console.Output.WriteLine($"[Row = {int_idx}, Col = {shift}, Len = {bitCount}]\n");

        var gpk = IterateMoves()
            .Select((model, id) => (model, (MoveId)id))
            .OrderBy(i => i.Item2)
            .GroupBy(p => p.model.GetUInt32(int_idx, bitCount, shift))
            .OrderBy(g => g.Key)
            .ToArray();

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

    void Test5(IConsole console)
    {
        // log byte groups
        var int_idx = 0;
        var shift = 8;
        var bitCount = 8;

        console.Output.WriteLine($"[Row = {int_idx}, Col = {shift}, Len = {bitCount}]\n");

        var gpk = IterateBaseBushouPart1()
            .Select((model, id) => (model, (WarriorId)id))
            .OrderBy(i => i.Item2)
            .GroupBy(p => p.model.GetUInt32(int_idx, bitCount, shift))
            .OrderBy(g => g.Key)
            .ToArray();

        foreach (var group in gpk)
        {
            console.Output.WriteLine($"{group.Key} = 0x{group.Key:x} = 0b{Convert.ToString(group.Key, 2).PadLeft(8, '0')} = {(WarriorSprite2Id)group.Key} ---------------------------------------");
            console.Output.WriteLine();

            foreach (var pk in group)
            {
                console.Output.WriteLine($"{pk.Item2}");
            }

            console.Output.WriteLine();
        }
    }

    void Test4(IConsole console)
    {
        // log byte groups
        var int_idx = 8;
        var shift = 0;
        var bitCount = 7;

        console.Output.WriteLine($"[Row = {int_idx}, Col = {shift}, Len = {bitCount}]\n");

        var gpk = IterateBuilding()
            .Select((building, id) => (building, (BuildingId)id))
            .OrderBy(i => i.building.Name)
            .GroupBy(p => p.building.GetUInt32(int_idx, bitCount, shift))
            .OrderBy(g => g.Key)
            .ToArray();

        foreach (var group in gpk)
        {
            console.Output.WriteLine($"{group.Key} = 0x{group.Key:x} = 0b{Convert.ToString(group.Key, 2).PadLeft(8, '0')} ---------------------------------------");
            console.Output.WriteLine();

            foreach (var pk in group)
            {
                console.Output.WriteLine($"{pk.Item2} ({pk.building.Kingdom})");
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
            console.Output.WriteLine(nameSelector(i).Replace(" ", "").Replace("'", "") + ",");
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

    static IEnumerable<MaxLink> IterateMaxSync()
    {
        using var file = new BinaryReader(File.OpenRead(@"C:\Users\Mia\Desktop\ConquestData\BaseBushouMaxSyncTable.dat"));

        int count = (int)(file.BaseStream.Length / MaxLink.DataLength);
        for (int i = 0; i < count; i++)
        {
            var item = file.ReadBytes(MaxLink.DataLength);
            yield return new MaxLink(item);
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

    static IEnumerable<MoveAnimation> IterateMoveEffects()
    {
        using var file = new BinaryReader(File.OpenRead(@"C:\Users\Mia\Desktop\ConquestData\WazaEffect.dat"));

        int count = (int)(file.BaseStream.Length / MoveAnimation.DataLength);
        for (int i = 0; i < count; i++)
        {
            var item = file.ReadBytes(MoveAnimation.DataLength);
            yield return new MoveAnimation(item);
        }
    }

    static IEnumerable<BaseDataWindow> IterateTrainers()
    {
        using var file = new BinaryReader(File.OpenRead(@"C:\Users\Mia\Desktop\ConquestData\Trainer.dat"));

        int count = (int)(file.BaseStream.Length / 0x2c);
        for (int i = 0; i < count; i++)
        {
            var item = file.ReadBytes(0x2c);
            yield return new BaseDataWindow(item, 0x2c);
        }
    }
}
