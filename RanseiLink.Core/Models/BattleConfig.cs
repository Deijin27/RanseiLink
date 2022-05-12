using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Graphics;

namespace RanseiLink.Core.Models
{
    public class BattleConfig : BaseDataWindow
    {
        public const int DataLength = 0x18;
        public BattleConfig(byte[] data) : base(data, DataLength) { }
        public BattleConfig() : this(new byte[DataLength]) { }

        public int Environment
        {
            get => GetInt(0, 0, 6);
            set => SetInt(0, 0, 6, value);
        }

        public int EnvironmentVariant
        {
            get => GetInt(0, 6, 5);
            set => SetInt(0, 6, 5, value);
        }

        public MapId MapId
        {
            get => new MapId(GetInt(0, 0, 6), GetInt(0, 6, 5));
            set
            {
                SetInt(0, 0, 6, value.Map);
                SetInt(0, 6, 5, value.Variant);
            }
        }

        public Rgb15 UpperAtmosphereColor
        {
            get => Rgb15.From((ushort)GetInt(0, 11, 15));
            set => SetInt(0, 11, 15, value.ToUInt16());
        }

        public Rgb15 MiddleAtmosphereColor
        {
            get => Rgb15.From((ushort)GetInt(1, 0, 15));
            set => SetInt(1, 0, 15, value.ToUInt16());
        }

        public Rgb15 LowerAtmosphereColor
        {
            get => Rgb15.From((ushort)GetInt(1, 15, 15));
            set => SetInt(1, 15, 15, value.ToUInt16());
        }

        public BattleVictoryConditionFlags VictoryCondition
        {
            get => (BattleVictoryConditionFlags)GetInt(2, 0, 5);
            set => SetInt(2, 0, 5, (int)value);
        }

        public BattleVictoryConditionFlags DefeatCondition
        {
            get => (BattleVictoryConditionFlags)GetInt(2, 5, 5);
            set => SetInt(2, 5, 5, (int)value);
        }

        public int NumberOfTurns
        {
            get => GetInt(2, 24, 5);
            set => SetInt(2, 24, 5, value);
        }

        private const int _itemStartByte = 12;

        public ItemId Treasure1
        {
            get => (ItemId)GetByte(_itemStartByte + 0);
            set => SetByte(_itemStartByte + 0, (byte)value);
        }

        public ItemId Treasure2
        {
            get => (ItemId)GetByte(_itemStartByte + 1);
            set => SetByte(_itemStartByte + 1, (byte)value);
        }

        public ItemId Treasure3
        {
            get => (ItemId)GetByte(_itemStartByte + 2);
            set => SetByte(_itemStartByte + 2, (byte)value);
        }

        public ItemId Treasure4
        {
            get => (ItemId)GetByte(_itemStartByte + 3);
            set => SetByte(_itemStartByte + 3, (byte)value);
        }

        public ItemId Treasure5
        {
            get => (ItemId)GetByte(_itemStartByte + 4);
            set => SetByte(_itemStartByte + 4, (byte)value);
        }

        public ItemId Treasure6
        {
            get => (ItemId)GetByte(_itemStartByte + 5);
            set => SetByte(_itemStartByte + 5, (byte)value);
        }

        public ItemId Treasure7
        {
            get => (ItemId)GetByte(_itemStartByte + 6);
            set => SetByte(_itemStartByte + 6, (byte)value);
        }

        public ItemId Treasure8
        {
            get => (ItemId)GetByte(_itemStartByte + 7);
            set => SetByte(_itemStartByte + 7, (byte)value);
        }

        public ItemId Treasure9
        {
            get => (ItemId)GetByte(_itemStartByte + 8);
            set => SetByte(_itemStartByte + 8, (byte)value);
        }

        public ItemId Treasure10
        {
            get => (ItemId)GetByte(_itemStartByte + 9);
            set => SetByte(_itemStartByte + 9, (byte)value);
        }

        public ItemId Treasure11
        {
            get => (ItemId)GetByte(_itemStartByte + 10);
            set => SetByte(_itemStartByte + 10, (byte)value);
        }

        public ItemId Treasure12
        {
            get => (ItemId)GetByte(_itemStartByte + 11);
            set => SetByte(_itemStartByte + 11, (byte)value);
        }
    }
}