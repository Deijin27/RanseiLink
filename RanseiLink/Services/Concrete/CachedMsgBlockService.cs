using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RanseiLink.Services.Concrete;

internal class CachedMsgBlockService : ICachedMsgBlockService
{
    private readonly IMsgBlockService _msgBlockService;
    public CachedMsgBlockService(IMsgBlockService msgBlockService)
    {
        _msgBlockService = msgBlockService;
        RebuildCache();
    }

    private List<Message>[] _blocks;
    private ChangeTrackedBlock[] _changeTrackedBlocks;

    public int BlockCount => _blocks.Length;

    public ChangeTrackedBlock Retrieve(int id)
    {
        return _changeTrackedBlocks[id];
    }

    public void SaveChangedBlocks()
    {
        Parallel.For(0, _msgBlockService.BlockCount, i =>
        {
            if (_changeTrackedBlocks[i].IsChanged)
            {
                _msgBlockService.Save(i, _blocks[i]);
                _changeTrackedBlocks[i].IsChanged = false;
            }
        });
    }

    public void RebuildCache()
    {
        _blocks = new List<Message>[_msgBlockService.BlockCount];
        _changeTrackedBlocks = new ChangeTrackedBlock[_msgBlockService.BlockCount];
        Parallel.For(0, _msgBlockService.BlockCount, i =>
        {
            var block = _msgBlockService.Retrieve(i);
            _blocks[i] = block;
            _changeTrackedBlocks[i] = new ChangeTrackedBlock(block);
        });
    }

    

    private const string NotEditableMessage = "Not Editable";

    private static (int block, int offsetWithinBlock, int maxValidId) GetInfoForType(MsgShortcut type)
    {
        int block;
        int offsetWithinBlock;
        int maxValidId;
        switch (type)
        {
            case MsgShortcut.AbilityDescription:
                block = 2;
                offsetWithinBlock = 171;
                maxValidId = 105;
                break;
            case MsgShortcut.AbilityHotSpringsDescription:
                block = 7;
                offsetWithinBlock = 109;
                maxValidId = 105;
                break;
            case MsgShortcut.AbilityHotSpringsDescription2:
                block = 8;
                offsetWithinBlock = 15;
                maxValidId = 105;
                break;
            case MsgShortcut.MoveDescription:
                block = 3;
                offsetWithinBlock = 32;
                maxValidId = 122;
                break;
            case MsgShortcut.WarriorSkillDescription:
                block = 3;
                offsetWithinBlock = 155;
                maxValidId = 62;
                break;
            case MsgShortcut.ItemDescription:
                block = 3;
                offsetWithinBlock = 218;
                maxValidId = 125;
                break;
            case MsgShortcut.ItemDescription2:
                block = 4;
                offsetWithinBlock = 94;
                maxValidId = 125;
                break;
            default:
                throw new System.Exception($"Invalid msg type '{type}'");
        }
        return (block, offsetWithinBlock, maxValidId);
    }

    public string GetMsgOfType(MsgShortcut type, int id)
    {
        var (block, offsetWithinBlock, maxValidId) = GetInfoForType(type);

        if (id > maxValidId)
        {
            return NotEditableMessage;
        }

        return GetWithOverflow(block, offsetWithinBlock + id);
    }

    public void SetMsgOfType(MsgShortcut type, int id, string value)
    {
        var (block, offsetWithinBlock, maxValidId) = GetInfoForType(type);

        if (id > maxValidId)
        {
            return;
        }

        SetWithOverflow(block, offsetWithinBlock + id, value);
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

}
