using RanseiLink.Core.Models;
using System.IO;
using System;

namespace RanseiLink.Core.Services.ModelServices;

public interface IEventSpeakerService : IModelService<EventSpeaker>
{
}

public class EventSpeakerService : BaseModelService<EventSpeaker>, IEventSpeakerService
{
    public EventSpeakerService(string EventSpeakerDatFile) : base(EventSpeakerDatFile, 0, 59) { }

    public EventSpeakerService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.EventSpeakerRomPath)) { }

    public override void Reload()
    {
        _cache.Clear();
        using var br = new BinaryReader(File.OpenRead(_dataFile));
        for (int id = _minId; id <= _maxId; id++)
        {
            _cache.Add(new EventSpeaker(br.ReadBytes(EventSpeaker.DataLength)));
        }
    }

    public override void Save()
    {
        using var bw = new BinaryWriter(File.OpenWrite(_dataFile));
        for (int id = _minId; id <= _maxId; id++)
        {
            bw.Write(_cache[id].Data);
        }
    }

    public override string IdToName(int id)
    {
        if (!ValidateId(id))
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }
        return _cache[id].Name;
    }
}