using RanseiLink.Core.Models;
using System.IO;
using System;

namespace RanseiLink.Core.Services.ModelServices;

public interface IWarriorSkillService : IModelService<WarriorSkill>
{
}

public class WarriorSkillService : BaseModelService<WarriorSkill>, IWarriorSkillService
{
    public WarriorSkillService(string WarriorSkillDatFile) : base(WarriorSkillDatFile, 0, 72) { }

    public WarriorSkillService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.WarriorSkillRomPath)) { }

    public override void Reload()
    {
        _cache.Clear();
        using var br = new BinaryReader(File.OpenRead(_dataFile));
        for (int id = _minId; id <= _maxId; id++)
        {
            _cache.Add(new WarriorSkill(br.ReadBytes(WarriorSkill.DataLength)));
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