
using RanseiLink.Core.Enums;
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

    #region Utility methods

    public string GetAbilityDescription(AbilityId id);
    public void SetAbilityDescription(AbilityId id, string description);

    public string GetAbilityHotSpringsDescription(AbilityId id);
    public void SetAbilityHotSpringsDescription(AbilityId id, string description);

    string GetMoveDescription(MoveId id);
    void SetMoveDescription(MoveId id, string description);

    string GetWarriorSkillDescription(WarriorSkillId id);
    void SetWarriorSkillDescription(WarriorSkillId id, string description);

    string GetItemDescription(ItemId id);
    void SetItemDescription(ItemId id, string description);

    string GetItemDescription2(ItemId id);
    void SetItemDescription2(ItemId id, string description);
    string GetAbilityHotSpringsDescription2(AbilityId id);
    void SetAbilityHotSpringsDescription2(AbilityId id, string description);

    #endregion

}
