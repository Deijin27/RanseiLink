using Core.Enums;
using Core.Models.Interfaces;

namespace Core.Models
{
    public class ScenarioWarrior : BaseDataWindow, IScenarioWarrior
    {
        public const int DataLength = 0x20;

        public ScenarioWarrior(byte[] data) : base(data, DataLength) { }
        public ScenarioWarrior() : base(new byte[DataLength], DataLength) { }

        public WarriorId Warrior
        {
            get => (WarriorId)GetByte(0);
            set => SetByte(0, (byte)value);
        }

        public uint ScenarioPokemon
        {
            get => GetByte(0xE);
            set => SetByte(0xE, (byte)value);
        }
        public bool ScenarioPokemonIsDefault
        {
            get => GetByte(0xF) == 4 && GetByte(0xE) == 76;
            set
            {
                if (value)
                {
                    SetByte(0xE, 76);
                    SetByte(0xF, 4);
                }
                else
                {
                    SetByte(0xE, 0);
                    SetByte(0xF, 0);
                }
            }
        }

        public IScenarioWarrior Clone()
        {
            return new ScenarioWarrior((byte[])Data.Clone());
        }
    }
}
