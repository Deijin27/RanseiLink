
using RanseiLink.Core.Enums;

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

    #endregion

}
