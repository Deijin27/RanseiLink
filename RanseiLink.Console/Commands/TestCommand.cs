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
using RanseiLink.Core.Archive;
using Example;
using System;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Graphics.ExternalFormats;
using System.Numerics;

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

    void Model(IConsole console, IServiceGetter services)
    {
        var overrideDataProvider = services.Get<IOverrideDataProvider>();

        var outputFolder = @"C:\Users\Mia\Desktop\outputs";
        foreach (var id in Enum.GetValues<GimmickObjectId>())
        {
            string file0 = null;
            for (int i = 0; i < 10; i++)
            {
                var file = Constants.ResolveGimmickModelFilePath(id, i);
                var dataFile = overrideDataProvider.GetDataFile(file);
                if (!File.Exists(dataFile.File))
                {
                    break;
                }

                string baseFile = null;
                if (i == 0)
                {
                    file0 = dataFile.File;
                }
                else
                {
                    baseFile = file0;
                }

                var dest = Path.Combine(outputFolder, Constants.ResolveGimmickModelFileNameWithoutExt(id, i));
                var result = ModelExtractorGenerator.ExtractModelFromPac(
                    pac: dataFile.File,
                    destinationFolder: dest
                    //basePac: baseFile
                    );
                if (result.IsFailed)
                {
                    Directory.CreateDirectory(dest);
                    File.WriteAllText(Path.Combine(dest, "FailureLog.txt"), result.ToString());
                    console.Output.WriteLine(file + ": " + result.ToString());
                }
            }
        }
    }

    void GenerateElevationFromMap(IConsole console, IServiceGetter services)
    {
        PSLM map = services.Get<IMapService>().Retrieve(new MapId(0, 0));
        OBJ model = services.Get<OBJ>();
        var intermediate = ConvertModels.ObjToIntermediate(model);

        // Ask user for how big the map should be, but we could calculate the minimum size by getting the bounding box

        int columnCount = 10;
        int rowCount = 11;
        float cellSize = 100f;
        float cellThird = cellSize / 3;
        float cellSixth = cellThird / 2;

        var topLeftX = columnCount / 2f * cellSize;
        var topLeftY = rowCount / 2f * cellSize;

        for (int row = 0; row < rowCount; row++)
        {
            for (int column = 0; column < columnCount; column++)
            {
                // offset by half sub cell
                var cellTopLeftX = topLeftX + column * cellSize + cellSixth;
                var cellTopLeftY = topLeftY - row * cellSize - cellSixth;
                var cell = map.TerrainSection.MapMatrix[row][column];
                for (int subCellRow = 0; subCellRow < 3; subCellRow++)
                {
                    for (int subCellCol = 0; subCellCol < 3; subCellCol++)
                    {
                        var pointX = cellTopLeftX + subCellCol * cellThird;
                        var pointY = cellTopLeftY + subCellRow * cellThird;
                        if (IntersectRay(intermediate, pointX, pointY, out var intersect))
                        {
                            cell.SubCellZValues[subCellRow * 3 + subCellCol] = intersect.Z;
                        }
                    }
                }
            }
        }

    }

    static bool IntersectRay(List<Group> model, float x, float y, out Vector3 maxZIntersect)
    {
        var rayOrigin = new Vector3(x, y, 0);
        var rayDir = new Vector3(0, 0, 1);
        var intersects = new List<Vector3>();
        Vector3 intersect;
        foreach (var group in model)
        {
            foreach (var polygon in group.Polygons)
            {
                if (polygon.PolyType == PolygonType.TRI)
                {
                    if (IntersectTriangle(
                        rayOrigin: rayOrigin,
                        rayDir: rayDir,
                        triPointA: polygon.Vertices[0],
                        triPointB: polygon.Vertices[1],
                        triPointC: polygon.Vertices[2],
                        out intersect))
                    {
                        intersects.Add(intersect);
                    }
                }
                else if (polygon.PolyType == PolygonType.QUAD)
                {
                    
                    if (IntersectTriangle(
                        rayOrigin: rayOrigin,
                        rayDir: rayDir,
                        triPointA: polygon.Vertices[0],
                        triPointB: polygon.Vertices[1],
                        triPointC: polygon.Vertices[2],
                        out intersect))
                    {
                        intersects.Add(intersect);
                    }
                    else if (IntersectTriangle(
                        rayOrigin: rayOrigin,
                        rayDir: rayDir,
                        triPointA: polygon.Vertices[2],
                        triPointB: polygon.Vertices[3],
                        triPointC: polygon.Vertices[0],
                        out intersect))
                    {
                        intersects.Add(intersect);
                    }
                }
            }
        }

        if (intersects.Count > 0)
        {
            maxZIntersect = intersects.MaxBy(x => x.Z);
            return true;
        }
        else
        {
            maxZIntersect = default;
            return false;
        }
    }

    static bool IntersectTriangle(
        Vector3 rayOrigin,
        Vector3 rayDir,
        Vector3 triPointA, Vector3 triPointB, Vector3 triPointC,
        out Vector3 intersect
    )
    {
        Vector3 E1 = triPointB - triPointA;
        Vector3 E2 = triPointC - triPointA;
        var normal = Vector3.Cross(E1, E2);
        float det = -Vector3.Dot(rayDir, normal);
        float invdet = 1.0f / det;
        Vector3 AO = rayOrigin - triPointA;
        Vector3 DAO = Vector3.Cross(AO, rayDir);
        var u = Vector3.Dot(E2, DAO) * invdet;
        var v = -Vector3.Dot(E1, DAO) * invdet;
        var t = Vector3.Dot(AO, normal) * invdet;

        var intersects = det >= 1e-6 && t >= 0.0 && u >= 0.0 && v >= 0.0 && (u + v) <= 1.0;

        if (intersects)
        {
            intersect = rayOrigin + t * rayDir;
        }
        else
        {
            intersect = default;
        }

        return intersects;
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!_currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }




        //Model(console, services);
        


        //var bmd = new NSBMD(FilePath);

        //if (DumpOption)
        //{
        //    Dump(console, bmd);
        //}
        //else
        //{
        //    console.Output.WriteLine(FixedPoint.Fix(0b_10000_00000, 1, 3, 6));
        //    console.Output.WriteLine(FixedPoint.Fix(0b_01111_11111, 1, 3, 6));
        //    console.Output.WriteLine(FixedPoint.InverseFix(7.9999999f, 1, 3, 6));
        //    console.Output.WriteLine(FixedPoint.InverseFix(-8f, 1, 3, 6));
        //    //Export(console, bmd);
        //}

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
