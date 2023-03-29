using OpenTK.Mathematics;
using System;
using OpenTK.Graphics.OpenGL;
using RanseiLink.Core.Models;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Archive;
using RanseiLink.Core;
using System.IO;
using RanseiLink.Core.Services.Concrete;
using RanseiLink.Core.RomFs;
using RanseiLink.Core.Services.DefaultPopulaters;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Services;
using System.Linq;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using System.Collections.Generic;

namespace RanseiLink.View3D;

public record SceneRendererState(NSBMD Nsbmd, BattleConfig Bc, PSLM Pslm, Dictionary<GimmickId, NSBMD> GimmickModels);

public class SceneRenderer
{
    private readonly IFallbackDataProvider _fallback;
    private readonly IBattleConfigService _battleConfigService;
    private readonly IMapService _mapService;
    private readonly IGimmickService _gimmickService;

    private SceneRendererState? _state;
    
    public SceneRenderer()
    {
        RomFsFactory fact = f => new RomFs(f);
        var msg = new MsgService();
        var mm = new ModManager(@"C:\Users\Mia\AppData\Local\RanseiLink\Mods", fact, msg);
        _fallback = new FallbackDataProvider(fact, Array.Empty<IGraphicTypeDefaultPopulater>());
        var mod = mm.GetAllModInfo().First();
        _battleConfigService = new BattleConfigService(mod);
        _mapService = new MapService(mod);
        _gimmickService = new GimmickService(mod);
    }

    public void LoadScene(BattleConfigId id)
    {
        _state = null;

        MaterialRegistry.UnloadMaterials();

        var bc = _battleConfigService.Retrieve((int)id);

        var mapId = bc.MapId;
        string mapRomPath = Path.Combine("graphics", "ikusa_map", mapId.ToInternalModelPacName());
        var pac = _fallback.GetDataFile(ConquestGameCode.VPYT, mapRomPath).File;

        var tempFolder = FileUtil.GetTemporaryDirectory();
        PAC.Unpack(pac, tempFolder, true, 4);

        var nsbmd = new NSBMD(Path.Combine(tempFolder, "0000.nsbmd"));
        var nsbtx = new NSBTX(Path.Combine(tempFolder, "0001.nsbtx"));

        Directory.Delete(tempFolder, true);

        MaterialRegistry.LoadMaterials(nsbmd, nsbtx);

        var pslm = _mapService.Retrieve(mapId);
        var gimmickModels = new Dictionary<GimmickId, NSBMD>();
        foreach (var gimmickId in pslm.GimmickSection.Items.Select(x => x.Gimmick).Distinct())
        {
            var gimmick = _gimmickService.Retrieve((int)gimmickId);
            var gimmickObject = (int)gimmick.State1Object;
            if (gimmickObject > 97)
            {
                continue;
            }
            var gimRomPath = Path.Combine("graphics", "ikusa_obj", $"OBJ{gimmickObject:000}_{0:00}.pac");
            var gimPac = _fallback.GetDataFile(ConquestGameCode.VPYT, gimRomPath).File;
            var gimTempFolder = FileUtil.GetTemporaryDirectory();
            PAC.Unpack(gimPac, gimTempFolder, true, 4);

            var gimNsbmd = new NSBMD(Path.Combine(gimTempFolder, "0000.nsbmd"));
            var gimNsbtx = new NSBTX(Path.Combine(gimTempFolder, "0001.nsbtx"));
            gimmickModels[gimmickId] = gimNsbmd;

            Directory.Delete(gimTempFolder, true);

            MaterialRegistry.LoadMaterials(gimNsbmd, gimNsbtx);
        }

        _state = new SceneRendererState(nsbmd, bc, pslm, gimmickModels);
    }

    public void Render()
    {
        if (_state == null)
        {
            return;
        }   
        GL.ClearColor(Color4.Gray);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        VerticalGradientBackground(_state.Bc);
        
        ModelRenderer.Draw(_state.Nsbmd.Model.Models[0]);

        var mapGridHeight = _state.Pslm.TerrainSection.MapMatrix.Count;
        var mapGridWidth = _state.Pslm.TerrainSection.MapMatrix[0].Count;

        foreach (var gimmick in _state.Pslm.GimmickSection.Items)
        {
            if (!_state.GimmickModels.TryGetValue(gimmick.Gimmick, out var gimmickModel))
            {
                continue;
            }
            var model = gimmickModel.Model.Models[0];
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();

            // get the elevation in the middle of the cell
            var elevation = _state.Pslm.TerrainSection.MapMatrix[gimmick.Position.Y][gimmick.Position.X].SubCellZValues[4];
            GL.Translate((-mapGridWidth / 2 + gimmick.Position.X) * 100, elevation, (-mapGridHeight / 2 + gimmick.Position.Y) * 100);
            ModelRenderer.Draw(model);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();

            //break;
        }

        
        GradientUtil.DrawGrid(new Color4(77, 77, 77, 255), Vector3.Zero, 100, mapGridHeight);
    }

    private static Color4 ConvertColor(Rgb15 color)
    {
        return new Color4((byte)(color.R * 8), (byte)(color.G * 8), (byte)(color.B * 8), 255);
    }

    private static void VerticalGradientBackground(BattleConfig battleConfig)
    {
        GradientUtil.VerticalGradientBackground(
            ConvertColor(battleConfig.UpperAtmosphereColor),
            ConvertColor(battleConfig.MiddleAtmosphereColor),
            ConvertColor(battleConfig.LowerAtmosphereColor)
            );
    }
}
