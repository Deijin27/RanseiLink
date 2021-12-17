using System.IO;

namespace RanseiLink.Core.Services.ModelServices;

public interface IMsgBlockService : IModelDataService<int, string[]>
{
    public int BlockCount { get; }
}

internal class MsgBlockService : IMsgBlockService
{
    private readonly IMsgService _msgService;
    private readonly ModInfo _mod;
    public MsgBlockService(ModInfo mod, IMsgService msgService)
    {
        _msgService = msgService;
        _mod = mod;
        BlockCount = msgService.BlockCount;
    }

    public int BlockCount { get; }

    public string[] Retrieve(int id)
    {
        string file = Path.Combine(_mod.FolderPath, Constants.MsgBlockPathFromId(id));
        return _msgService.LoadBlock(file);
    }

    public void Save(int id, string[] block)
    {
        string file = Path.Combine(_mod.FolderPath, Constants.MsgBlockPathFromId(id));
        _msgService.SaveBlock(file, block);
    }
}
