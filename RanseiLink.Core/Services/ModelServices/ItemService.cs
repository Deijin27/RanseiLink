using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using System.IO;
using System;

namespace RanseiLink.Core.Services.ModelServices;

public interface IItemService : IModelService<Item>
{
}

public class ItemService : BaseModelService<Item>, IItemService
{
    public ItemService(string ItemDatFile) : base(ItemDatFile, 0, 133, 134) { }

    public ItemService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.ItemRomPath)) { }

    public override void Reload()
    {
        _cache.Clear();
        using var br = new BinaryReader(File.OpenRead(_dataFile));
        for (int id = _minId; id <= _maxId; id++)
        {
            _cache.Add(new Item(br.ReadBytes(Item.DataLength)));
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