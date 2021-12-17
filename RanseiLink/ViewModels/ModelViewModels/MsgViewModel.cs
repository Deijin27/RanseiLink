
using RanseiLink.Services;

namespace RanseiLink.ViewModels.ModelViewModels;

public class MsgViewModel
{
    private readonly ChangeTrackedBlock _block;

    public MsgViewModel(int blockId, int id, ChangeTrackedBlock block)
    {
        BlockId = blockId;
        Id = id;
        _block = block;
    }

    public int BlockId { get; }
    public int Id { get; }
    public string Text
    {
        get => _block[Id];
        set => _block[Id] = value;
    }
}
