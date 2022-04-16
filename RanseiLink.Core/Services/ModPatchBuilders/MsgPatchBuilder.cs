using RanseiLink.Core.Services.Concrete;
using System.Collections.Concurrent;
using System.IO;

namespace RanseiLink.Core.Services.ModPatchBuilders
{
    public class MsgPatchBuilder : IPatchBuilder
    {
        private readonly IMsgService _msgService;
        private readonly ModInfo _mod;
        public MsgPatchBuilder(IMsgService msgService, ModInfo mod)
        {
            _msgService = msgService;
            _mod = mod;
        }

        public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, PatchOptions patchOptions)
        {
            string msgTmpFile = Path.GetTempFileName();
            _msgService.CreateMsgDat(Path.Combine(_mod.FolderPath, Constants.MsgFolderPath), msgTmpFile);
            filesToPatch.Add(new FileToPatch(Constants.MsgRomPath, msgTmpFile, FilePatchOptions.VariableLength | FilePatchOptions.DeleteSourceWhenDone));
        }
    }
}