using RanseiLink.Core.Resources;
using System.Collections.Concurrent;

namespace RanseiLink.Core.Services.ModPatchBuilders;

[PatchBuilder]
public class GraphicsPatchBuilder : IPatchBuilder
{
    private readonly IFallbackDataProvider _fallbackSpriteProvider;
    private readonly IOverrideDataProvider _overrideDataProvider;
    private readonly IServiceGetter _modServiceGetter;
    private readonly ModInfo _mod;
    public GraphicsPatchBuilder(ModInfo mod, IFallbackDataProvider fallbackSpriteProvider, IOverrideDataProvider overrideDataProvider, IServiceGetter modServiceGetter)
    {
        _fallbackSpriteProvider = fallbackSpriteProvider;
        _overrideDataProvider = overrideDataProvider;
        _modServiceGetter = modServiceGetter;
        _mod = mod;
    }

    public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, PatchOptions patchOptions)
    {
        if (!patchOptions.HasFlag(PatchOptions.IncludeSprites))
        {
            return;
        }

        if (!_fallbackSpriteProvider.IsDefaultsPopulated(_mod.GameCode))
        {
            throw new Exception("Cannot patch sprites unless 'Populate Graphics Defaults' has been run");
        }

        var graphicsProviderFolder = Constants.DefaultDataFolder(_mod.GameCode);

        var graphicsPatchContext = new GraphicsPatchContext(
            filesToPatch,
            _overrideDataProvider, 
            _fallbackSpriteProvider,
            graphicsProviderFolder,
            _modServiceGetter);

        Parallel.ForEach(GraphicsInfoResource.All, gInfo =>
        {
            gInfo.GetFilesToPatch(graphicsPatchContext);
        });
    }
}