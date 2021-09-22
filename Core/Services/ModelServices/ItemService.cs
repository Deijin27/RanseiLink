using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;

namespace Core.Services.ModelServices
{
    public class ItemService : BaseModelService, IModelDataService<ItemId, IItem>
    {
        public ItemService(ModInfo mod) : base(mod, Constants.ItemRomPath, Item.DataLength) { }

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
