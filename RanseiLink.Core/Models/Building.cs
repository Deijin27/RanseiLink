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

        #region Referenced Buildings

        public BuildingId Building1 // R4_C24_L8
        {
            get => (BuildingId)GetByte(19);
            set => SetByte(19, (byte)value);
        }

        public BuildingId Building2 // R5_C0_L8
        {
            get => (BuildingId)GetByte(20);
            set => SetByte(20, (byte)value);
        }

        public BuildingId Building3 // R5_C8_L8
        {
            get => (BuildingId)GetByte(21);
            set => SetByte(21, (byte)value);
        }

        public BuildingId Building4 // R5_C16_L8
        {
            get => (BuildingId)GetByte(22);
            set => SetByte(22, (byte)value);
        }

        public BuildingId Building5 // R5_C24_L8
        {
            get => (BuildingId)GetByte(23);
            set => SetByte(23, (byte)value);
        }

        public BuildingId Building6 // R6_C0_L8
        {
            get => (BuildingId)GetByte(24);
            set => SetByte(24, (byte)value);
        }

        public BuildingId Building7 // R6_C8_L8
        {
            get => (BuildingId)GetByte(25);
            set => SetByte(25, (byte)value);
        }

        public BuildingId Building8 // R5_C16_L8
        {
            get => (BuildingId)GetByte(26);
            set => SetByte(26, (byte)value);
        }

        public BuildingId[] Buildings
        {
            get => new[] { Building1, Building2, Building3, Building4, Building5, Building6, Building7, Building8 };
            set
            {
                if (value.Length == 8)
                {
                    Building1 = value[0];
                    Building2 = value[1];
                    Building3 = value[2];
                    Building4 = value[3];
                    Building5 = value[4];
                    Building6 = value[5];
                    Building7 = value[6];
                    Building8 = value[7];
                }
            }
        }

        #endregion

        public KingdomId Kingdom // R5_C24_L8
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