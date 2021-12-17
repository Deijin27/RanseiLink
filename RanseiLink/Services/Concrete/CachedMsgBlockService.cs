using RanseiLink.Core.Services.ModelServices;
using System.Collections.Generic;

namespace RanseiLink.Services.Concrete;

internal class CachedMsgBlockService : ICachedMsgBlockService
{
    private readonly IMsgBlockService _msgBlockService;
    public CachedMsgBlockService(IMsgBlockService msgBlockService)
    {
        _msgBlockService = msgBlockService;
        RebuildCache();
    }

    private readonly List<string[]> _blocks = new();
    private readonly List<ChangeTrackedBlock> _changeTrackedBlocks = new();

    public int BlockCount => _blocks.Count;

    public ChangeTrackedBlock Retrieve(int id)
    {
        return _changeTrackedBlocks[id];
    }

    public void SaveChangedBlocks()
    {
        for (int i = 0; i < _blocks.Count; i++)
        {
            if (_changeTrackedBlocks[i].IsChanged)
            {
                _msgBlockService.Save(i, _blocks[i]);
                _changeTrackedBlocks[i].IsChanged = false;
            }
        }
    }

    public void RebuildCache()
    {
        _blocks.Clear();
        _changeTrackedBlocks.Clear();
        for (int i = 0; i < _msgBlockService.BlockCount; i++)
        {
            var block = _msgBlockService.Retrieve(i);
            _blocks.Add(block);
            _changeTrackedBlocks.Add(new ChangeTrackedBlock(block));
        }
    }
}
