using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using System.IO;
using System;

namespace RanseiLink.Core.Services.ModelServices;

public interface IMoveRangeService : IModelService<MoveRange>
{
}

public class MoveRangeService : BaseModelService<MoveRange>, IMoveRangeService
{
    public MoveRangeService(string MoveRangeDatFile) : base(MoveRangeDatFile, 0, 29) { }

    public MoveRangeService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.MoveRangeRomPath)) { }

    public override void Reload()
    {
        _cache.Clear();
        using var br = new BinaryReader(File.OpenRead(_dataFile));
        for (int id = _minId; id <= _maxId; id++)
        {
            _cache.Add(new MoveRange(br.ReadBytes(MoveRange.DataLength)));
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
        return ((MoveRangeId)id).ToString();
    }
}