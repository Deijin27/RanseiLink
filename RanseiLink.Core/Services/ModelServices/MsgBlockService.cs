using RanseiLink.Core.Text;

namespace RanseiLink.Core.Services.ModelServices;

public interface IMsgBlockService
{
    int BlockCount { get; }
    List<Message> Retrieve(int id);
    void Save(int id, List<Message> block);
}

internal class MsgBlockService : IMsgBlockService
{
    private readonly IMsgService _msgService;
    private readonly ModInfo _mod;
    public MsgBlockService(ModInfo mod, IMsgService msgService)
    {
        _msgService = msgService;
        _mod = mod;
        BlockCount = Directory.GetFiles(Path.Combine(_mod.FolderPath, Constants.MsgFolderPath)).Length;
    }

    public int BlockCount { get; }

    public List<Message> Retrieve(int id)
    {
        string file = Path.Combine(_mod.FolderPath, Constants.MsgBlockPathFromId(id));
        return _msgService.LoadBlock(file);
    }

    public void Save(int id, List<Message> block)
    {
        string file = Path.Combine(_mod.FolderPath, Constants.MsgBlockPathFromId(id));
        _msgService.SaveBlock(file, block);
    }
}