using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services.Concrete;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace RanseiLink.Core.Services.ModPatchBuilders
{
    public class ScbgPatchBuilder : IGraphicTypePatchBuilder
    {
        private readonly IOverrideDataProvider _overrideSpriteProvider;
        public ScbgPatchBuilder(IOverrideDataProvider overrideSpriteProvider)
        {
            _overrideSpriteProvider = overrideSpriteProvider;
        }

        public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, IGraphicsInfo gInfo)
        {
            if (!(gInfo is ScbgConstants scbgInfo))
            {
                return;
            }

            var spriteFiles = _overrideSpriteProvider.GetAllSpriteFiles(scbgInfo.Type);
            if (!spriteFiles.Any(i => i.IsOverride))
            {
                return;
            }

            string[] filesToPack = spriteFiles.Select(i => i.File).ToArray();
            string data = Path.GetTempFileName();
            string info = Path.GetTempFileName();

            SCBGCollection
                .LoadPngs(filesToPack, tiled: true)
                .Save(data, info);

            filesToPatch.Add(new FileToPatch(scbgInfo.Data, data, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
            filesToPatch.Add(new FileToPatch(scbgInfo.Info, info, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));

        }
    }
}