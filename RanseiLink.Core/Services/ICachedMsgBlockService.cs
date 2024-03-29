﻿using RanseiLink.Core.Text;
using System.Text.RegularExpressions;

namespace RanseiLink.Core.Services;

public record MessageAddedToBlockArgs(Message Message, int IndexInBlock);
public record MessageRemovedFromBlockArgs(Message Message);
public record MessageAddedArgs(Message Message, int IndexInBlock, int BlockId);
public record MessageRemovedArgs(Message Message, int BlockId);

public class ChangeTrackedBlock(List<Message> block)
{
    public Message this[int index] => block[index];
    public int Count => block.Count;
    public bool IsChanged { get; set; } = false;

    public bool CanAdd(int groupId)
    {
        int found = 0;
        foreach (var message in block)
        {
            if (message.GroupId == groupId)
            {
                found++;
                if (found > 2)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool CanRemove(int groupId)
    {
        int found = 0;
        foreach (var message in block)
        {
            if (message.GroupId == groupId)
            {
                found++;
                if (found > 3)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public event EventHandler<MessageAddedToBlockArgs>? MessageAdded;
    public event EventHandler<MessageRemovedFromBlockArgs>? MessageRemoved;

    /// <summary>
    /// Add a new message to the group of the given id
    /// </summary>
    /// <param name="groupId"></param>
    public void Add(int groupId)
    {
        if (!CanAdd(groupId))
        {
            return;
        }

        // find the last message in this group
        int lastIndex = -1;
        for (int i = block.Count - 1; i >= 0; i--)
        {
            var message = block[i];
            if (message.GroupId == groupId)
            {
                lastIndex = i;
                break;
            }
        }

        // create new message and insert it after last one in the group
        var secondToLastMessage = block[lastIndex - 1];
        var lastMessage = block[lastIndex];
        var newMessage = secondToLastMessage.Clone();
        // increment the speaker ids if present
        newMessage.Context = Regex.Replace(newMessage.Context, "{" + PnaConstNames.SpeakerId + /* language=regex */ @":(?<speakerId>\d+)}", new MatchEvaluator(match =>
        {
            var lastSpeakerCurrently = int.Parse(match.Groups["speakerId"].Value);
            var newLastSpeakerNumber = lastSpeakerCurrently + 1;
            var replaceWith = $"{{{PnaConstNames.SpeakerId}:{newLastSpeakerNumber}}}";
            return replaceWith;
        }));
        // increment the element ids where necessary
        newMessage.ElementId++;
        lastMessage.ElementId++;
        var newIndex = lastIndex; // not lastIndex+1 because there is a dummy item at the end of the group which should be ignored
        block.Insert(newIndex, newMessage);
        MessageAdded?.Invoke(this, new MessageAddedToBlockArgs(newMessage, newIndex));
    }

    public void Remove(Message message)
    {
        if (!block.Contains(message))
        {
            return;
        }
        if (!CanRemove(message.GroupId))
        {
            return;
        }

        block.Remove(message);
        // Fixup element ids
        int elementId = 0;
        foreach (var item in block)
        {
            if (item.GroupId == message.GroupId)
            {
                item.ElementId = elementId++;
            }
        }
        MessageRemoved?.Invoke(this, new MessageRemovedFromBlockArgs(message));
    }
}

public interface ICachedMsgBlockService
{
    int BlockCount { get; }

    ChangeTrackedBlock Retrieve(int id);

    void SaveChangedBlocks();

    void RebuildCache();

    string GetMsgOfType(MsgShortcut type, int id);
    void SetMsgOfType(MsgShortcut type, int id, string value);

    event EventHandler<MessageAddedArgs>? MessageAdded;

    event EventHandler<MessageRemovedArgs>? MessageRemoved;
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