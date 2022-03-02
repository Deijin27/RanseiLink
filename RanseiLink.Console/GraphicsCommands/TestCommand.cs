using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

//[Command("test")]
//public class TestCommand : BaseCommand
//{
//    public TestCommand(IServiceContainer container) : base(container) { }
//    public TestCommand() : base() { }

//    public override ValueTask ExecuteAsync(IConsole console)
//    {
//        var eve = new EVE(@"C:\Users\Mia\Desktop\00000065.eve");

//        //var ncer = NCER.Load(@"C:\Users\Mia\Desktop\graphics\still\stl_busho_f\stl_busho_f-Unpacked\0002.ncer");

//        //for (int i = 0; i < ncer.CellBanks.Banks[0].Length; i++)
//        //{
//        //    var cell = ncer.CellBanks.Banks[0][i];

//        //    console.Output.WriteLine($"Cell {i}");
//        //    console.Output.WriteLine($"  Y Offset: {cell.YOffset}");
//        //    console.Output.WriteLine($"  X Offset: {cell.XOffset}");
//        //    console.Output.WriteLine($"  Width   : {cell.Width}");
//        //    console.Output.WriteLine($"  Height  : {cell.Height}");
//        //}

//        console.Output.WriteLine("Complete!");

//        return default;
//    }
//}
