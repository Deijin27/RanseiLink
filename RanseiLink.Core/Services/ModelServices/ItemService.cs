using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

public interface IItemService : IModelService<Item>
{
}

public class ItemService : BaseNewableDataModelService<Item>, IItemService
{
    public static ItemService Load(string ItemDatFile) => new ItemService(ItemDatFile);
    private ItemService(string ItemDatFile) : base(ItemDatFile, 0, 133, 134) { }

    public ItemService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.ItemRomPath)) { }

    public Item Retrieve(ItemId id) => Retrieve((int)id);

    public override string IdToName(int id)
    {
        return Retrieve(id).Name;
    }
} 