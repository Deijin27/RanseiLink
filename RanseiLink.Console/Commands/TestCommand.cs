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
        foreach (var command in bmd.Model.Models[0].RenderCommands.Commands)
        {
            if (command.OpCode == RenderOpCode.BIND_MATERIAL)
            {
                var mat = bmd.Model.Models[0].Materials.Materials[command.Parameters[0]];
                console.Output.WriteLine($"{command.OpCode} : {mat.Texture}");
            }
            else if (command.OpCode == RenderOpCode.DRAW_MESH)
            {
                var msh = bmd.Model.Models[0].Meshes.MeshCommandList[command.Parameters[0]];
                console.Output.WriteLine($"{command.OpCode} : {msh.Name}");
            }
            else if (command.OpCode == RenderOpCode.MTX_MULT)
            {
                var objIdx = command.Parameters[0];
                var obj = bmd.Model.Models[0].Bones.Nodes[objIdx];
                if (obj.HasRotate)
                {
                    console.Output.WriteLine($"{command.OpCode} : {obj.Name} T{obj.Translation}, S{obj.Scale}, R({obj.Rotate})");
                }
                else
                {
                    console.Output.WriteLine($"{command.OpCode} : {obj.Name} T{obj.Translation}, S{obj.Scale}, R()");
                }
            }
            else
            {
                console.Output.WriteLine($"{command.OpCode} : {command.Flags}");
            }
            
        }
        console.Output.WriteLine();
        console.Output.WriteLine("Poly Commands ---------------------------------------------------------------------------------------------");
        foreach (var mesh in bmd.Model.Models[0].Meshes.MeshCommandList)
        {
            console.Output.WriteLine(":: Begin Mesh <{0}> ---------------------------------------------------------------------------------------------", mesh.Name);
            foreach (var c in mesh.Commands)
            {
                if (c.OpCode == MeshDisplayOpCode.BEGIN_VTXS)
                {
                    console.Output.WriteLine($"{c.OpCode} : {(PolygonType)c.Params[0]}");
                }
                else
                {
                    console.Output.WriteLine(c.OpCode);
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
            Export(console, bmd);
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
