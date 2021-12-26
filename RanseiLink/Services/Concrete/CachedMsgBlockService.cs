using RanseiLink.Core.Enums;
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

    private string GetWithOverflow(int blockId, int id)
    {
        while (_changeTrackedBlocks[blockId].Count <= id)
        {
            id -= _changeTrackedBlocks[blockId].Count;
            blockId++;
        }
        return _changeTrackedBlocks[blockId][id];
    }

    private void SetWithOverflow(int blockId, int id, string value)
    {
        while (_changeTrackedBlocks[blockId].Count <= id)
        {
            id -= _changeTrackedBlocks[blockId].Count;
            blockId++;
        }
        _changeTrackedBlocks[blockId][id] = value;
    }

    public string GetAbilityDescription(AbilityId id) => GetWithOverflow(2, 171 + (int)id);
    public void SetAbilityDescription(AbilityId id, string description) => SetWithOverflow(2, 171 + (int)id, description);
    public string GetAbilityHotSpringsDescription(AbilityId id) => GetWithOverflow(7, 109 + (int)id);
    public void SetAbilityHotSpringsDescription(AbilityId id, string description) => SetWithOverflow(7, 109 + (int)id, description);
    public string GetMoveDescription(MoveId id) => GetWithOverflow(3, 32 + (int)id);
    public void SetMoveDescription(MoveId id, string description) => SetWithOverflow(3, 32 + (int)id, description);
    public string GetWarriorSkillDescription(WarriorSkillId id) => GetWithOverflow(3, 155 + (int)id);
    public void SetWarriorSkillDescription(WarriorSkillId id, string description) => SetWithOverflow(3, 155 + (int)id, description);
    public string GetItemDescription(ItemId id) => GetWithOverflow(3, 218 + (int)id);
    public void SetItemDescription(ItemId id, string description) => SetWithOverflow(3, 218 + (int)id, description);
    public string GetItemDescription2(ItemId id) => GetWithOverflow(4, 94 + (int)id);
    public void SetItemDescription2(ItemId id, string description) => SetWithOverflow(4, 94 + (int)id, description);
}
