using RanseiLink.Console.Services;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Concrete;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Security.Cryptography;
using System.Text;

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


    void Dump(IConsole console, NSBMD bmd)
    {
        console.WriteLine("Bone Commands ---------------------------------------------------------------------------------------------");
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
                console.WriteLine($"{command.OpCode} : {mat.Texture}, {lineEnd}");
            }
            else if (command.OpCode == RenderOpCode.DRAW_MESH)
            {
                var ply = bmd.Model.Models[0].Polygons[command.Parameters[0]];
                console.WriteLine($"{command.OpCode} : {ply.Name}, {lineEnd}");
            }
            else if (command.OpCode == RenderOpCode.MTX_MULT)
            {
                var pmIdx = command.Parameters[0];
                var plymsh = bmd.Model.Models[0].Polymeshes[pmIdx];
                if (plymsh.Flag.HasFlag(NSMDL.Model.PolymeshData.TransFlag.Rotate))
                {
                    console.WriteLine($"{command.OpCode} : {plymsh.Name} T{plymsh.Translation}, S{plymsh.Scale}, R({plymsh.Rotate}), {lineEnd}");
                }
                else
                {
                    console.WriteLine($"{command.OpCode} : {plymsh.Name} T{plymsh.Translation}, S{plymsh.Scale}, R(), {lineEnd}");
                }
            }
            else
            {
                console.WriteLine($"{command.OpCode} : {lineEnd}");
            }
            
        }
        console.WriteLine();
        console.WriteLine("Poly Commands ---------------------------------------------------------------------------------------------");
        foreach (var ply in bmd.Model.Models[0].Polygons)
        {
            console.Output.WriteLine(":: Begin Mesh <{0}> ---------------------------------------------------------------------------------------------", ply.Name);
            int i = 0;
            foreach (var c in ply.Commands)
            {
                if (i++ % 4 == 0)
                {
                    console.WriteLine();
                }
                if (c.OpCode == PolygonDisplayOpCode.BEGIN_VTXS)
                {
                    console.WriteLine($"{c.OpCode} : {(PolygonType)c.Params[0]}");
                }
                else
                {
                    if (c.Params != null && c.Params.Length != 0)
                    {
                        console.Output.WriteLine("{0}: [{1}]", c.OpCode, string.Join(", ", c.Params.Select(x => x.ToString("X"))));
                    }
                    else
                    {
                        console.WriteLine(c.OpCode);
                    }
                }
            }
            console.WriteLine();
        }
    }

    void Export(IConsole console, NSBMD bmd)
    {
        var mdl = bmd.Model.Models[0];
        var obj = ConvertModels.ModelToObj(mdl);
        obj.Save(@$"C:\Users\Mia\Desktop\graphics\ikusa_map\{mdl.Name}-Unpacked\0000 - Extracted\{mdl.Name}.obj");
    }

    public static void GetStuff(IMoveService moveService, IMoveRangeService moveRanges, ISpriteService spriteService)
    {
        var move = moveService.Retrieve((int)MoveId.QuickAttack);
        var range = moveRanges.Retrieve((int)move.Range);

        using var baseImg = spriteService.GetMovePreview(move, range);

        baseImg.SaveAsPng(@"C:\Users\Mia\Desktop\out.png");
    }

    public static Point GetPoint(int row, int column)
    {
        return new Point(
            x: 18 + 6 * (row - column),
            y: -3 + 3 * (column + row)
            );
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!_currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.WriteLine("No mod selected");
            return default;
        }

        var sb = new StringBuilder();
        var dict = new Dictionary<int, string>();
        var battleConfigs = services.Get<IBattleConfigService>();

        foreach (var mid in services.Get<IMapService>().GetMapIds())
        {
            dict[(int)mid] = "Unused";
        }
        
        foreach (var id in battleConfigs.ValidIds())
        {
            var config = battleConfigs.Retrieve(id);
            var idEnum = (BattleConfigId)id;
            var idString = idEnum.ToString();
            var map = config.Map * 100 + config.MapVariant;
            dict[map] = idString;
        }

        foreach (var (key, value) in dict)
        {
            sb.AppendLine($"[{key}] = \"{value}\",");
        }

        File.WriteAllText(@"C:\Users\Mia\Desktop\maps.txt", sb.ToString());


        //var bmd = new NSBMD(FilePath);

        //if (DumpOption)
        //{
        //    Dump(console, bmd);
        //}
        //else
        //{
        //    console.WriteLine(FixedPoint.Fix(0b_10000_00000, 1, 3, 6));
        //    console.WriteLine(FixedPoint.Fix(0b_01111_11111, 1, 3, 6));
        //    console.WriteLine(FixedPoint.InverseFix(7.9999999f, 1, 3, 6));
        //    console.WriteLine(FixedPoint.InverseFix(-8f, 1, 3, 6));
        //    //Export(console, bmd);
        //}

        //if (!_currentModService.TryGetCurrentModServiceGetter(out var services))
        //{
        //    console.WriteLine("No mod selected");
        //    return default;
        //}

        //var service = services.Get<IBuildingService>();

        //foreach (var id in service.ValidIds())
        //{
        //    var building = service.Retrieve(id);
        //    console.WriteLine($"\n{(BuildingId)id}: {building.Kingdom}");
        //    foreach (var referenced in building.Buildings)
        //    {
        //        if (referenced == BuildingId.Default)
        //        {
        //            console.WriteLine("- default");
        //        }
        //        else
        //        {
        //            console.WriteLine($"- {referenced}: {service.Retrieve((int)referenced).Kingdom}");
        //        }
        //    }
        //}


        return default;
    }
}
