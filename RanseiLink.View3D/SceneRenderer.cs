using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using RanseiLink.Core.Models;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Archive;
using RanseiLink.Core;
using System.IO;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Services;
using System.Linq;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using System.Collections.Generic;
using FluentResults;

namespace RanseiLink.View3D;

public record SceneRendererState(NSBMD Nsbmd, BattleConfig? Bc, PSLM Pslm, Dictionary<GimmickId, NSBMD> GimmickModels);

public interface ISceneRenderer
{
    void Configure(SceneRenderOptions options);
    Result LoadScene(BattleConfigId id);
    Result LoadScene(MapId mapId);
    void Render();
}

public class SceneRenderer : ISceneRenderer
{
    private readonly IOverrideDataProvider _overrideDataProvider;
    private readonly IBattleConfigService _battleConfigService;
    private readonly IMapService _mapService;
    private readonly IGimmickService _gimmickService;
    private SceneRenderOptions _options;

    private SceneRendererState? _state;

    public SceneRenderer(
        IOverrideDataProvider overrideDataProvider,
        IBattleConfigService battleConfigService,
        IMapService mapService,
        IGimmickService gimmickService)
    {
        _overrideDataProvider = overrideDataProvider;
        _battleConfigService = battleConfigService;
        _mapService = mapService;
        _gimmickService = gimmickService;
    }

    public void Configure(SceneRenderOptions options)
    {
        _options = options;
    }

    public Result LoadScene(MapId mapId)
    {
        _state = null;
        return LoadSceneInternal(mapId, null);
    }

    public Result LoadScene(BattleConfigId id)
    {
        _state = null;
        var bc = _battleConfigService.Retrieve((int)id);
        var mapId = bc.MapId;
        return LoadSceneInternal(mapId, bc);
    }

    private Result LoadSceneInternal(MapId mapId, BattleConfig? bc)
    {
        MaterialRegistry.UnloadMaterials();

        string mapRomPath = Path.Combine("graphics", "ikusa_map", mapId.ToInternalModelPacName());
        var pac = _overrideDataProvider.GetDataFile(mapRomPath).File;
        if (!File.Exists(pac))
        {
            return Result.Fail($"Specified 3D model '{mapRomPath}' does not exist in game data. This is not a bug, it just isn't there.");
        }

        var tempFolder = FileUtil.GetTemporaryDirectory();
        PAC.Unpack(pac, tempFolder, true, 4);

        var nsbmd = new NSBMD(Path.Combine(tempFolder, "0000.nsbmd"));
        var nsbtx = new NSBTX(Path.Combine(tempFolder, "0001.nsbtx"));

        Directory.Delete(tempFolder, true);

        MaterialRegistry.LoadMaterials(nsbmd, nsbtx);

        var pslm = _mapService.Retrieve(mapId);
        var gimmickModels = new Dictionary<GimmickId, NSBMD>();
        if (_options.HasFlag(SceneRenderOptions.ShowGimmicks))
        {
            foreach (var gimmickId in pslm.GimmickSection.Items.Select(x => x.Gimmick).Distinct())
            {
                var gimmick = _gimmickService.Retrieve((int)gimmickId);
                var gimmickObject = (int)gimmick.State1Sprite;
                if (gimmickObject > 97)
                {
                    continue;
                }
                var gimRomPath = Path.Combine("graphics", "ikusa_obj", $"OBJ{gimmickObject:000}_{0:00}.pac");
                var gimPac = _overrideDataProvider.GetDataFile(gimRomPath).File;
                var gimTempFolder = FileUtil.GetTemporaryDirectory();
                PAC.Unpack(gimPac, gimTempFolder, true, 4);

                var gimNsbmd = new NSBMD(Path.Combine(gimTempFolder, "0000.nsbmd"));
                var gimNsbtx = new NSBTX(Path.Combine(gimTempFolder, "0001.nsbtx"));
                gimmickModels[gimmickId] = gimNsbmd;

                Directory.Delete(gimTempFolder, true);

                MaterialRegistry.LoadMaterials(gimNsbmd, gimNsbtx);
            }
        }

        _state = new SceneRendererState(nsbmd, bc, pslm, gimmickModels);

        return Result.Ok();
    }

    public void Render()
    {
        if (_state == null)
        {
            return;
        }
        GL.ClearColor(Color4.Gray);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        if (_state.Bc != null)
        {
            VerticalGradientBackground(_state.Bc);
        }

        ModelRenderer.Draw(_state.Nsbmd.Model.Models[0]);

        var mapGridHeight = _state.Pslm.TerrainSection.MapMatrix.Count;
        var mapGridWidth = _state.Pslm.TerrainSection.MapMatrix[0].Count;

        if (_options.HasFlag(SceneRenderOptions.ShowGimmicks))
        {
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
        }
        
        if (_options.HasFlag(SceneRenderOptions.DrawGrid))
        {
            GradientUtil.DrawGrid(new Color4(77, 77, 77, 255), Vector3.Zero, 100, mapGridHeight);
        }
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
