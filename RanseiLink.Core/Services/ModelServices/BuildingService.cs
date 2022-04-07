using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using System.IO;
using System;

namespace RanseiLink.Core.Services.ModelServices;

public interface IBuildingService : IModelService<Building>
{
}

public class BuildingService : BaseModelService<Building>, IBuildingService
{
    public BuildingService(string BuildingDatFile) : base(BuildingDatFile, 0, 118) { }

    public BuildingService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.BuildingRomPath)) { }

    public override void Reload()
    {
        _cache.Clear();
        using var br = new BinaryReader(File.OpenRead(_dataFile));
        for (int id = _minId; id <= _maxId; id++)
        {
            _cache.Add(new Building(br.ReadBytes(Building.DataLength)));
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
        return ((BuildingId)id).ToString();
    }
}