﻿// This file is automatically generated

#nullable enable

using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;


public partial interface IEventSpeakerService : IModelService<EventSpeaker> {}

public partial class EventSpeakerService : BaseDataModelService<EventSpeaker>, IEventSpeakerService
{
    public static EventSpeakerService Load(string dataFile, ConquestGameCode culture) => new EventSpeakerService(dataFile, culture);
    private EventSpeakerService(string dataFile, ConquestGameCode culture) : base(dataFile, 0, 59, () => new EventSpeaker(culture)) {}

    public EventSpeakerService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.EventSpeakerRomPath), mod.GameCode) {}

    public EventSpeaker Retrieve(EventSpeakerId id) => Retrieve((int)id);

    public override string IdToName(int id)
    {
        return Retrieve(id).Name;
    }
}