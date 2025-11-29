using OpenTK.Graphics.OpenGL;
using RanseiLink.Core.Models;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Archive;
using RanseiLink.Core;
using System.IO;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Services;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using System.Collections.Generic;
using FluentResults;
using OpenTK.Mathematics;
using System;
using System.Linq;

namespace RanseiLink.View3D;

public record SceneRendererState(List<ModelInfo> Models, BattleConfig? Bc, int GridCellCount);

public record ModelInfo(NSBMD Model, Vector3 Translation = default);

public record ResetZoomEventArgs(double zoom);

public interface ISceneRenderer
{
    event EventHandler<ResetZoomEventArgs>? RequestResetZoom;
    void Configure(SceneRenderOptions options);
    Result LoadScene(BattleConfigId id);
    Result LoadScene(MapId mapId);
    Result LoadScene(GimmickObjectId id, int variant);
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

    public event EventHandler<ResetZoomEventArgs>? RequestResetZoom;

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

    public Result LoadScene(GimmickObjectId id, int variant)
    {
        _state = null;
        MaterialRegistry.UnloadMaterials();
        string? fallbackPac = null;
        if (variant != 0)
        {
            fallbackPac = _overrideDataProvider.GetDataFile(GetGimmickPath((int)id, 0)).File;
        }
        var gimRomPath = GetGimmickPath((int)id, variant);
        var gimPac = _overrideDataProvider.GetDataFile(gimRomPath).File;
        if (!File.Exists(gimPac))
        {
            return Result.Fail($"Specified 3D model '{gimRomPath}' does not exist in game data. This is not a bug, it just isn't there.");
        }

        var nsbmd = LoadMaterialsFromPac(gimPac, fallbackPac);
        List<ModelInfo> models = [];
        models.Add(new ModelInfo(nsbmd));

        _state = new SceneRendererState(models, null, 5);
        RequestResetZoom?.Invoke(this, new ResetZoomEventArgs(2));
        return Result.Ok();
    }

    private NSBMD LoadMaterialsFromPac(string pac, string? fallbackPac = null)
    {
        var tempFolder = FileUtil.GetTemporaryDirectory();
        if (fallbackPac != null)
        {
            // unpack the fallback, then files that are overwritten will get stomped over
            // when we unpack the main one.
            PAC.Unpack(fallbackPac, tempFolder, true, 4);
        }
        PAC.Unpack(pac, tempFolder, true, 4);

        var files = Directory.GetFiles(tempFolder).ToDictionary(
            x => PAC.ExtensionToFileTypeNumber(Path.GetExtension(x)), x => x);
        if (!files.TryGetValue(PAC.FileTypeNumber.NSBMD, out var nsbmdPath))
        {
            throw new Exception($"No nsbmd found in {pac}");
        }
        if (!files.TryGetValue(PAC.FileTypeNumber.NSBTX, out var nsbtxPath))
        {
            throw new Exception($"No nsbtx found in {pac}");
        }
        var nsbmd = new NSBMD(nsbmdPath);
        var nsbtx = new NSBTX(nsbtxPath);

        Directory.Delete(tempFolder, true);

        MaterialRegistry.LoadMaterials(nsbmd, nsbtx);
        return nsbmd;
    }

    private static string GetGimmickPath(int id, int variant)
    {
        return Path.Combine("graphics", "ikusa_obj", $"OBJ{id:000}_{variant:00}.pac");
    }

    private Result LoadSceneInternal(MapId mapId, BattleConfig? bc)
    {
        MaterialRegistry.UnloadMaterials();

        List<ModelInfo> models = [];

        string mapRomPath = Path.Combine("graphics", "ikusa_map", mapId.ToInternalModelPacName());
        var pac = _overrideDataProvider.GetDataFile(mapRomPath).File;
        if (!File.Exists(pac))
        {
            return Result.Fail($"Specified 3D model '{mapRomPath}' does not exist in game data. This is not a bug, it just isn't there.");
        }

        var nsbmd = LoadMaterialsFromPac(pac);
        models.Add(new ModelInfo(nsbmd));

        var pslm = _mapService.Retrieve((int)mapId);
        var mapGridHeight = pslm.TerrainSection.MapMatrix.Count;
        var mapGridWidth = pslm.TerrainSection.MapMatrix[0].Count;

        
        if (_options.HasFlag(SceneRenderOptions.ShowGimmicks))
        {
            var gimmickModels = new Dictionary<GimmickId, NSBMD>();
            foreach (var gimmick in pslm.GimmickSection.Items)
            {
                // Make sure we don't load the same materials twice
                if (!gimmickModels.TryGetValue(gimmick.Gimmick, out var gimNsbmd))
                {
                    // Load materials
                    var gimmickModel = _gimmickService.Retrieve((int)gimmick.Gimmick);
                    var gimmickObject = (int)gimmickModel.State1Sprite;
                    if (gimmickObject >= (int)GimmickObjectId.Default)
                    {
                        continue;
                    }
                    var gimRomPath = GetGimmickPath(gimmickObject, 0);
                    var gimPac = _overrideDataProvider.GetDataFile(gimRomPath).File;
                    if (!File.Exists(gimPac))
                    {
                        continue;
                    }

                    gimNsbmd = LoadMaterialsFromPac(gimPac);
                }
                
                // get the elevation in the middle of the cell
                var elevation = pslm.TerrainSection.MapMatrix[gimmick.Position.Y][gimmick.Position.X].SubCellZValues[4];
                var gimmickTranslation = new Vector3((-mapGridWidth / 2 + gimmick.Position.X) * 100, elevation, (-mapGridHeight / 2 + gimmick.Position.Y) * 100);
                
                // Register in models list
                models.Add(new ModelInfo(gimNsbmd, gimmickTranslation));
            }
        }

        _state = new SceneRendererState(models, bc, mapGridHeight);

        RequestResetZoom?.Invoke(this, new ResetZoomEventArgs(10));
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

        foreach (var model in _state.Models)
        {
            var isTranslated = model.Translation != default;
            if (isTranslated)
            {
                // Push translation
                GL.MatrixMode(MatrixMode.Projection);
                GL.PushMatrix();
                GL.MatrixMode(MatrixMode.Modelview);
                GL.PushMatrix();
                GL.Translate(model.Translation);
            }

            ModelRenderer.Draw(model.Model.Model.Models[0]);

            if (isTranslated)
            {
                // Pop translation
                GL.MatrixMode(MatrixMode.Projection);
                GL.PopMatrix();
                GL.MatrixMode(MatrixMode.Modelview);
                GL.PopMatrix();
            }
        }
        
        if (_options.HasFlag(SceneRenderOptions.DrawGrid))
        {
            GradientUtil.DrawGrid(new Color4(77, 77, 77, 255), Vector3.Zero, 100, _state.GridCellCount,
                depthTest: !_options.HasFlag(SceneRenderOptions.GridDisableDepthTesting));
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
