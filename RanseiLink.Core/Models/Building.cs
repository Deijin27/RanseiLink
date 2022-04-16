using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models
{
    public class Building : BaseDataWindow
    {
        public const int DataLength = 0x24;
        public Building(byte[] data) : base(data, DataLength) { }
        public Building() : base(new byte[DataLength], DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 0x12);
            set => SetPaddedUtf8String(0, 0x12, value);
        }

        public KingdomId Kingdom
        {
            get => (KingdomId)GetByte(27);
            set => SetByte(27, (byte)value);
        }

        public BattleConfigId BattleConfig1
        {
            get => (BattleConfigId)GetInt(7, 0, 7);
            set => SetInt(7, 0, 7, (int)value);
        }

        public BattleConfigId BattleConfig2
        {
            get => (BattleConfigId)GetInt(7, 7, 7);
            set => SetInt(7, 7, 7, (int)value);
        }

        public BattleConfigId BattleConfig3
        {
            get => (BattleConfigId)GetInt(8, 0, 7);
            set => SetInt(8, 0, 7, (int)value);
        }

        public BuildingSpriteId Sprite1
        {
            get => (BuildingSpriteId)GetInt(8, 7, 7);
            set => SetInt(8, 7, 7, (int)value);
        }

        public BuildingSpriteId Sprite2
        {
            get => (BuildingSpriteId)GetInt(8, 14, 7);
            set => SetInt(8, 14, 7, (int)value);
        }

        public BuildingSpriteId Sprite3
        {
            get => (BuildingSpriteId)GetInt(8, 21, 7);
            set => SetInt(8, 21, 7, (int)value);
        }

        public BuildingFunctionId Function
        {
            get => (BuildingFunctionId)GetInt(8, 28, 4);
            set => SetInt(8, 28, 4, (int)value);
        }

    }
}