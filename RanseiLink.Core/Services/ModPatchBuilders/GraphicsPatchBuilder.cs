using RanseiLink.Core.Resources;
using RanseiLink.Core.Services.Concrete;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace RanseiLink.Core.Services.ModPatchBuilders
{
    public class GraphicsPatchBuilder : IPatchBuilder
    {
        private readonly IGraphicTypePatchBuilder[] _builders;
        private readonly IFallbackSpriteProvider _fallbackSpriteProvider;
        public GraphicsPatchBuilder(IGraphicTypePatchBuilder[] builders, IFallbackSpriteProvider fallbackSpriteProvider)
        {
            _builders = builders;
            _fallbackSpriteProvider = fallbackSpriteProvider;
        }

        public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, PatchOptions patchOptions)
        {
            if (!patchOptions.HasFlag(PatchOptions.IncludeSprites))
            {
                return;
            }

            if (!_fallbackSpriteProvider.IsDefaultsPopulated)
            {
                throw new Exception("Cannot patch sprites unless 'Populate Graphics Defaults' has been run");
            }

            Parallel.ForEach(GraphicsInfoResource.All, gInfo =>
            {
                foreach (var builder in _builders)
                {
                    builder.GetFilesToPatch(filesToPatch, gInfo);
                }
            });
        }
    }
}