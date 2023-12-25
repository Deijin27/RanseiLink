using System.Collections.Concurrent;

namespace RanseiLink.Core.Services.ModPatchBuilders;

[PatchBuilder]
public class MsgPatchBuilder(IMsgService msgService, ModInfo mod) : IPatchBuilder
{
    public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, PatchOptions patchOptions)
    {
        string msgTmpFile = Path.GetTempFileName();
        msgService.CreateMsgDat(Path.Combine(mod.FolderPath, Constants.MsgFolderPath), msgTmpFile);
        filesToPatch.Add(new FileToPatch(Constants.MsgRomPath, msgTmpFile, FilePatchOptions.VariableLength | FilePatchOptions.DeleteSourceWhenDone));
    }
}