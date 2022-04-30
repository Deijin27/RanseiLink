using RanseiLink.Core.Enums;
using System;

namespace RanseiLink.Core.Models
{
    public class ScenarioWarrior : BaseDataWindow
    {
        public const int DataLength = 0x20;

        public ScenarioWarrior(byte[] data) : base(data, DataLength) { }

        public ScenarioWarrior() : this(new byte[DataLength]) { }

        public WarriorId Warrior
        {
            get => (WarriorId)GetByte(0);
            set => SetByte(0, (byte)value);
        }

        public WarriorClassId Class
        {
            get => (WarriorClassId)GetInt(0, 9, 3);
            set => SetInt(0, 9, 3, (int)value);
        }

        public KingdomId Kingdom
        {
            get => (KingdomId)GetInt(0, 12, 5);
            set => SetInt(0, 12, 5, (int)value);
        }

        public int Army
        {
            get => GetInt(0, 17, 5);
            set => SetInt(0, 17, 5, value);
        }

        public ItemId Item
        {
            get => (ItemId)GetInt(2, 2, 8);
            set => SetInt(2, 2, 8, (int)value);
        }

        public int GetScenarioPokemon(int id)
        {
            if (id > 7 || id < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(id)} is out of range. Scenario warriors only have 8 pokemon");
            }
            return GetUInt16(0xE + id * 2);
        }

        public void SetScenarioPokemon(int id, int value)
        {
            if (id > 7 || id < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(id)} is out of range. Scenario warriors only have 8 pokemon");
            }
            SetUInt16(0xE + id * 2, (ushort)value);
        }


        public void MakeScenarioPokemonDefault(int id)
        {
            SetScenarioPokemon(id, DefaultScenarioPokemon);
        }

        public bool ScenarioPokemonIsDefault(int id)
        {
            return GetScenarioPokemon(id) == DefaultScenarioPokemon;
        }

        public const ushort DefaultScenarioPokemon = 1100;
    }
}