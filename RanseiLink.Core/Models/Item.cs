using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models
{
    public class Item : BaseDataWindow
    {
        public const int DataLength = 0x24;
        public Item(byte[] data) : base(data, DataLength)
        {
        }

        public Item() : base(new byte[DataLength], DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 20);
            set => SetPaddedUtf8String(0, 20, value);
        }

        public PurchaseMethodId PurchaseMethod
        {
            get => (PurchaseMethodId)GetInt(5, 16, 3);
            set => SetInt(5, 16, 3, (int)value);
        }

        public ItemCategoryId Category
        {
            get => (ItemCategoryId)GetInt(5, 19, 2);
            set => SetInt(5, 19, 2, (int)value);
        }

        public ItemEffectId Effect
        {
            get => (ItemEffectId)GetInt(5, 21, 5);
            set => SetInt(5, 21, 5, (int)value);
        }

        public int EffectDuration
        {
            get => GetInt(5, 26, 3);
            set => SetInt(5, 26, 3, value);
        }

        public ItemId CraftingIngredient1
        {
            get => (ItemId)GetInt(6, 0, 9);
            set => SetInt(6, 0, 9, (int)value);
        }

        public int CraftingIngredient1Amount
        {
            get => GetInt(6, 9, 7);
            set => SetInt(6, 9, 7, value);
        }

        public ItemId CraftingIngredient2
        {
            get => (ItemId)GetInt(6, 16, 9);
            set => SetInt(6, 16, 9, (int)value);
        }

        public int CraftingIngredient2Amount
        {
            get => GetInt(6, 25, 7);
            set => SetInt(6, 25, 7, value);
        }

        public ItemId UnknownItem
        {
            get => (ItemId)GetInt(7, 0, 9);
            set => SetInt(7, 0, 9, (int)value);
        }

        /// <summary>
        /// Max shop price / 100
        /// </summary>
        public int ShopPriceMultiplier
        {
            get => GetInt(7, 9, 9);
            set => SetInt(7, 9, 9, value);
        }

        public int QuantityForEffect
        {
            get => GetInt(7, 18, 9);
            set => SetInt(7, 18, 9, value);
        }

        public bool GetPurchasable(KingdomId kingdom)
        {
            return GetPurchasable((int)kingdom);
        }

        public void SetPurchasable(KingdomId kingdom, bool value)
        {
            SetPurchasable((int)kingdom, value);
        }

        public bool GetPurchasable(int kingdom)
        {
            return GetInt(8, kingdom, 1) == 1;
        }

        public void SetPurchasable(int kingdom, bool value)
        {
            SetInt(8, kingdom, 1, value ? 1 : 0);
        }

    }
}