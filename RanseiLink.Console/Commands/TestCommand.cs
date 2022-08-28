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
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Util;

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

    [CommandParameter(0, Description = "Path of btx0 data file.", Name = "bmd0File")]
    public string FilePath { get; set; }

    [CommandOption("dump", 'd')]
    public bool DumpOption { get; set; }

    void Dump(IConsole console, NSBMD bmd)
    {
        console.Output.WriteLine("Bone Commands ---------------------------------------------------------------------------------------------");
        foreach (var command in bmd.Model.Models[0].RenderCommands)
        {
            string lineEnd = $"Flag: {command.Flags}, Params: [";
            if (command.Parameters != null)
            {
                lineEnd += string.Join(", ", command.Parameters);
            }
            lineEnd += "]";
            if (command.OpCode == RenderOpCode.BIND_MATERIAL)
            {
                var mat = bmd.Model.Models[0].Materials[command.Parameters[0]];
                console.Output.WriteLine($"{command.OpCode} : {mat.Texture}, {lineEnd}");
            }
            else if (command.OpCode == RenderOpCode.DRAW_MESH)
            {
                var ply = bmd.Model.Models[0].Polygons[command.Parameters[0]];
                console.Output.WriteLine($"{command.OpCode} : {ply.Name}, {lineEnd}");
            }
            else if (command.OpCode == RenderOpCode.MTX_MULT)
            {
                var pmIdx = command.Parameters[0];
                var plymsh = bmd.Model.Models[0].Polymeshes[pmIdx];
                if (plymsh.Flag.HasFlag(NSMDL.Model.PolymeshData.TransFlag.Rotate))
                {
                    console.Output.WriteLine($"{command.OpCode} : {plymsh.Name} T{plymsh.Translation}, S{plymsh.Scale}, R({plymsh.Rotate}), {lineEnd}");
                }
                else
                {
                    console.Output.WriteLine($"{command.OpCode} : {plymsh.Name} T{plymsh.Translation}, S{plymsh.Scale}, R(), {lineEnd}");
                }
            }
            else
            {
                console.Output.WriteLine($"{command.OpCode} : {lineEnd}");
            }
            
        }
        console.Output.WriteLine();
        console.Output.WriteLine("Poly Commands ---------------------------------------------------------------------------------------------");
        foreach (var ply in bmd.Model.Models[0].Polygons)
        {
            console.Output.WriteLine(":: Begin Mesh <{0}> ---------------------------------------------------------------------------------------------", ply.Name);
            int i = 0;
            foreach (var c in ply.Commands)
            {
                if (i++ % 4 == 0)
                {
                    console.Output.WriteLine();
                }
                if (c.OpCode == PolygonDisplayOpCode.BEGIN_VTXS)
                {
                    console.Output.WriteLine($"{c.OpCode} : {(PolygonType)c.Params[0]}");
                }
                else
                {
                    if (c.Params != null && c.Params.Length != 0)
                    {
                        console.Output.WriteLine("{0}: [{1}]", c.OpCode, string.Join(", ", c.Params.Select(x => x.ToString("X"))));
                    }
                    else
                    {
                        console.Output.WriteLine(c.OpCode);
                    }
                }
            }
            console.Output.WriteLine();
        }
    }

    void Export(IConsole console, NSBMD bmd)
    {
        var mdl = bmd.Model.Models[0];
        var obj = ConvertModels.ModelToObj(mdl);
        obj.Save(@$"C:\Users\Mia\Desktop\graphics\ikusa_map\{mdl.Name}-Unpacked\0000 - Extracted\{mdl.Name}.obj");
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        var bmd = new NSBMD(FilePath);

        if (DumpOption)
        {
            Dump(console, bmd);
        }
        else
        {
            console.Output.WriteLine(FixedPoint.Fix(0b_10000_00000, 1, 3, 6));
            console.Output.WriteLine(FixedPoint.Fix(0b_01111_11111, 1, 3, 6));
            console.Output.WriteLine(FixedPoint.InverseFix(7.9999999f, 1, 3, 6));
            console.Output.WriteLine(FixedPoint.InverseFix(-8f, 1, 3, 6));
            //Export(console, bmd);
        }

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
