using RanseiLink.Core.Enums;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Text;
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

    private readonly List<List<Message>> _blocks = new();
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
        return _changeTrackedBlocks[blockId][id].Text;
    }

    private void SetWithOverflow(int blockId, int id, string value)
    {
        while (_changeTrackedBlocks[blockId].Count <= id)
        {
            id -= _changeTrackedBlocks[blockId].Count;
            blockId++;
        }
        _changeTrackedBlocks[blockId][id].Text = value;
        _changeTrackedBlocks[blockId].IsChanged = true;
    }

    private const string DescriptionNotSupportedMessage = "Description not supported";


    public string GetAbilityDescription(AbilityId id) 
    { 
        return id <= AbilityId.dummy7 ? GetWithOverflow(2, 171 + (int)id) : DescriptionNotSupportedMessage; 
    }
    public void SetAbilityDescription(AbilityId id, string description) 
    { 
        if (id <= AbilityId.dummy7) SetWithOverflow(2, 171 + (int)id, description); 
    }


    public string GetAbilityHotSpringsDescription(AbilityId id)
    {
        return id <= AbilityId.dummy7 ? GetWithOverflow(7, 109 + (int)id) : DescriptionNotSupportedMessage;
    }
    public void SetAbilityHotSpringsDescription(AbilityId id, string description)
    {
        if (id <= AbilityId.dummy7) SetWithOverflow(7, 109 + (int)id, description);
    }


    public string GetAbilityHotSpringsDescription2(AbilityId id)
    {
        return id <= AbilityId.dummy7 ? GetWithOverflow(8, 15 + (int)id) : DescriptionNotSupportedMessage;
    }
    public void SetAbilityHotSpringsDescription2(AbilityId id, string description)
    {
        if (id <= AbilityId.dummy7) SetWithOverflow(8, 15 + (int)id, description);
    }


    public string GetMoveDescription(MoveId id)
    {
        return id <= MoveId.LowKick ? GetWithOverflow(3, 32 + (int)id) : DescriptionNotSupportedMessage;
    }
    public void SetMoveDescription(MoveId id, string description)
    {
        if (id <= MoveId.LowKick) SetWithOverflow(3, 32 + (int)id, description);
    }


    public string GetWarriorSkillDescription(WarriorSkillId id)
    {
        return id <= WarriorSkillId.Sacrifice ? GetWithOverflow(3, 155 + (int)id) : DescriptionNotSupportedMessage;
    }
    public void SetWarriorSkillDescription(WarriorSkillId id, string description)
    { 
        if (id <= WarriorSkillId.Sacrifice) SetWithOverflow(3, 155 + (int)id, description); 
    }


    public string GetItemDescription(ItemId id)
    {
        return id <= ItemId.JigglypuffCharm ? GetWithOverflow(3, 218 + (int)id) : DescriptionNotSupportedMessage;
    }
    public void SetItemDescription(ItemId id, string description)
    { 
        if (id <= ItemId.JigglypuffCharm) SetWithOverflow(3, 218 + (int)id, description); 
    }


    public string GetItemDescription2(ItemId id)
    {
        return id <= ItemId.JigglypuffCharm ? GetWithOverflow(4, 94 + (int)id) : DescriptionNotSupportedMessage;
    }

    public void SetItemDescription2(ItemId id, string description)
    {
        if (id <= ItemId.JigglypuffCharm) SetWithOverflow(4, 94 + (int)id, description);
    }
}
