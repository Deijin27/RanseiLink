
namespace RanseiLink.Services;

public class ChangeTrackedBlock
{
    public ChangeTrackedBlock(string[] block)
    {
        _blockInternal = block;
    }
    private string[] _blockInternal;

    public string this[int index]
    {
        get => _blockInternal[index];
        set
        {
            _blockInternal[index] = value;
            IsChanged = true;
        }
    }
    public int Count => _blockInternal.Length;
    public bool IsChanged { get; set; } = false;
}

public interface ICachedMsgBlockService
{
    int BlockCount { get; }

    public ChangeTrackedBlock Retrieve(int id);

    public void SaveChangedBlocks();

    public void RebuildCache();

}
