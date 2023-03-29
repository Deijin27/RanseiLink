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

namespace RanseiLink.View3D;

public class SceneRenderer
{
    private readonly IFallbackDataProvider _fallback;
    private readonly IBattleConfigService _battleConfigService;

    private NSBMD _nsbmd;
    private NSBTX _nsbtx;
    private bool _isDrawing = false;
    private BattleConfig _bc;
    
    public SceneRenderer()
    {
        RomFsFactory fact = f => new RomFs(f);
        var msg = new MsgService();
        var mm = new ModManager(@"C:\Users\Mia\AppData\Local\RanseiLink\Mods", fact, msg);
        _fallback = new FallbackDataProvider(fact, Array.Empty<IGraphicTypeDefaultPopulater>());
        var mod = mm.GetAllModInfo().First();
        _battleConfigService = new BattleConfigService(mod);
    }

    public void LoadScene(BattleConfigId id)
    {
        var bc = _battleConfigService.Retrieve((int)id);
        LoadScene(bc);
    }

    public void LoadScene(BattleConfig bc)
    {
        _isDrawing = false;
        MaterialRegistry.UnloadMaterials();

        _bc = bc;
        var map = bc.MapId;
        string mapRomPath = Path.Combine("graphics", "ikusa_map", $"MAP{map.Map:00}_{map.Variant:00}.pac");
        var pac = _fallback.GetDataFile(ConquestGameCode.VPYT, mapRomPath).File;

        var tempFolder = FileUtil.GetTemporaryDirectory();
        PAC.Unpack(pac, tempFolder, true, 4);

        _nsbmd = new NSBMD(Path.Combine(tempFolder, "0000.nsbmd"));
        _nsbtx = new NSBTX(Path.Combine(tempFolder, "0001.nsbtx"));

        Directory.Delete(tempFolder, true);

        MaterialRegistry.LoadMaterials(_nsbmd, _nsbtx);

        _isDrawing = true;
    }

    public void Render()
    {
        if (!_isDrawing)
        {
            return;
        }   
        GL.ClearColor(Color4.Gray);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        VerticalGradientBackground(_bc);

        ModelRenderer.Draw(_nsbmd.Model.Models[0]);
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
