using RanseiLink.Core.Resources;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RanseiLink.Core.Services.ModPatchBuilders;

[PatchBuilder]
public class MiscPatchBuilder : IGraphicTypePatchBuilder
{
    public MetaSpriteType Id => MetaSpriteType.Misc;

    private readonly IOverrideDataProvider _overrideSpriteProvider;
    private readonly Dictionary<MetaMiscItemId, IMiscItemPatchBuilder> _builders;
    public MiscPatchBuilder(IOverrideDataProvider overrideSpriteProvider, IMiscItemPatchBuilder[] builders)
    {
        _overrideSpriteProvider = overrideSpriteProvider;
        _builders = new();
        foreach (var builder in builders)
        {
            _builders.Add(builder.Id, builder);
        }
    }

    public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, IGraphicsInfo gInfo)
    {
        var miscInfo = (MiscConstants)gInfo;

        foreach (var item in miscInfo.Items)
        {
            var spriteFile = _overrideSpriteProvider.GetSpriteFile(miscInfo.Type, item.Id);
            if (!spriteFile.IsOverride)
            {
                continue;
            }

            _builders[item.MetaId].GetFilesToPatch(filesToPatch, miscInfo, item, spriteFile.File);
        }
    }
}
