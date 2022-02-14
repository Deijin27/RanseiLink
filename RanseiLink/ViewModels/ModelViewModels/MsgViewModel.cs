
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

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
        get => _block[Id].Text;
        set
        {
            _block[Id].Text = value;
            _block.IsChanged = true;
        }
    }

    public string Context
    {
        get => _block[Id].Context;
        set
        {
            _block[Id].Context = value;
            _block.IsChanged = true;
        }
    }

    public string BoxConfig
    {
        get => _block[Id].BoxConfig; 
        set
        {
            _block[Id].BoxConfig = value;
            _block.IsChanged = true;
        }
    }

    public int GroupId => _block[Id].GroupId;

    public int ElementId => _block[Id].ElementId;
}
