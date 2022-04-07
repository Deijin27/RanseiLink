using RanseiLink.Core.Text;
using System.Collections.Generic;

namespace RanseiLink.Services;

public class ChangeTrackedBlock
{
    public ChangeTrackedBlock(List<Message> block)
    {
        _blockInternal = block;
    }
    private readonly List<Message> _blockInternal;

    public Message this[int index] => _blockInternal[index];
    public int Count => _blockInternal.Count;
    public bool IsChanged { get; set; } = false;
}

public interface ICachedMsgBlockService
{
    int BlockCount { get; }

    public ChangeTrackedBlock Retrieve(int id);

    public void SaveChangedBlocks();

    public void RebuildCache();

    string GetMsgOfType(MsgShortcut type, int id);
    void SetMsgOfType(MsgShortcut type, int id, string value);

}

public enum MsgShortcut
{
    AbilityDescription,
    AbilityHotSpringsDescription,
    AbilityHotSpringsDescription2,
    MoveDescription,
    WarriorSkillDescription,
    ItemDescription,
    ItemDescription2,
    EpisodeName,
    EpisodeDescription,
}