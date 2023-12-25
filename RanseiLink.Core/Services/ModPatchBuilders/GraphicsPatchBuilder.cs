using RanseiLink.Core.Resources;
using System.Collections.Concurrent;

namespace RanseiLink.Core.Services.ModPatchBuilders;

[PatchBuilder]
public class GraphicsPatchBuilder : IPatchBuilder
{
    private readonly Dictionary<MetaSpriteType, IGraphicTypePatchBuilder> _builders;
    private readonly IFallbackDataProvider _fallbackSpriteProvider;
    private readonly ModInfo _mod;
    public GraphicsPatchBuilder(ModInfo mod, IGraphicTypePatchBuilder[] builders, IFallbackDataProvider fallbackSpriteProvider)
    {
        _fallbackSpriteProvider = fallbackSpriteProvider;
        _mod = mod;

        _builders = new();
        foreach (var builder in builders)
        {
            _builders.Add(builder.Id, builder);
        }
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

        Parallel.ForEach(GraphicsInfoResource.All, gInfo =>
        {
            _builders[gInfo.MetaType].GetFilesToPatch(filesToPatch, gInfo);
        });
    }
}