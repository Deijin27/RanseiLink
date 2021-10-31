using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IItemService : IModelDataService<ItemId, IItem>
    {
        IDisposableItemService Disposable();
    }

    public interface IDisposableItemService : IDisposableModelDataService<ItemId, IItem>
    {
    }

    public class ItemService : BaseModelService, IItemService
    {
        public ItemService(ModInfo mod) : base(mod, Constants.ItemRomPath, Item.DataLength, 133) { }

        public IDisposableItemService Disposable()
        {
            return new DisposableItemService(Mod);
        }

        public IItem Retrieve(ItemId id)
        {
            return new Item(RetrieveData((int)id));
        }

        public void Save(ItemId id, IItem model)
        {
            SaveData((int)id, model.Data);
        }
    }

    public class DisposableItemService : BaseDisposableModelService, IDisposableItemService
    {
        public DisposableItemService(ModInfo mod) : base(mod, Constants.ItemRomPath, Item.DataLength, 133) { }

        public IItem Retrieve(ItemId id)
        {
            return new Item(RetrieveData((int)id));
        }

        public void Save(ItemId id, IItem model)
        {
            SaveData((int)id, model.Data);
        }
    }
}
