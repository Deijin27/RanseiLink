using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services.ModelServices;
using System.Linq;
using RanseiLink.Core.Enums;
using System.Collections.Generic;
using System.IO;

namespace RanseiLink.Console.Commands;

[Command("test", Description = "Test command")]
public  class TestCommand : ICommand
{
    private readonly ICurrentModService _currentModService;
    public TestCommand(ICurrentModService currentModService)
    {
        _currentModService = currentModService;
    }

    private string byteToString(byte b)
    {
        return b.ToString("X").PadLeft(2, '0');
    }

    private void Test(IConsole console, string[] strings)
    {
        console.Output.WriteLine();
        foreach (var s in strings)
        {
            console.Output.WriteLine(s);
        }

        var nodes = RanseiLink.Core.Graphics.RadixTreeGenerator.Generate(strings);

        var memoryStream = new MemoryStream();
        var bw = new BinaryWriter(memoryStream);
        var br = new BinaryReader(memoryStream);
        foreach (var node in nodes)
        {
            memoryStream.Position = 0;
            node.WriteTo(bw);
            memoryStream.Position = 0;
            console.Output.WriteLine($"{byteToString(br.ReadByte())} {byteToString(br.ReadByte())} {byteToString(br.ReadByte())} {byteToString(br.ReadByte())}");
        }
        memoryStream.Dispose();
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        var strings = new string[24];
        for (int i = 0; i < 24; i++)
        {
            var s = i.ToString().PadLeft(2, '0');
            strings[i] = $"base_fix_{s}";
        }
        Test(console, strings);

        Test(console, new string[] { "base_fix_f_pl", "base_fix_b_pl" });
        Test(console, new string[] { "map00_00_04", "map00_00_04a" });

        
        //if (!_currentModService.TryGetCurrentModServiceGetter(out var services))
        //{
        //    console.Output.WriteLine("No mod selected");
        //    return default;
        //}

        //var service = services.Get<IBuildingService>();

        //foreach (var id in service.ValidIds())
        //{
        //    var building = service.Retrieve(id);
        //    console.Output.WriteLine($"\n{(BuildingId)id}: {building.Kingdom}");
        //    foreach (var referenced in building.Buildings)
        //    {
        //        if (referenced == BuildingId.Default)
        //        {
        //            console.Output.WriteLine("- default");
        //        }
        //        else
        //        {
        //            console.Output.WriteLine($"- {referenced}: {service.Retrieve((int)referenced).Kingdom}");
        //        }
        //    }
        //}


        return default;
    }
}
