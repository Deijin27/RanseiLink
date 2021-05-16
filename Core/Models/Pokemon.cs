using System;
using System.Text;
using Core.Enums;
using Core.Models.Interfaces;

namespace Core.Models
{
    public class Pokemon : BaseDataWindow, IPokemon
    {
        public const int DataLength = 0x30;
        public Pokemon(byte[] data) : base(data, DataLength) { }

        public Pokemon() : base(new byte[DataLength], DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 11);
            set => SetPaddedUtf8String(0, 11, value);
        }

        public uint Hp
        {
            get => GetUInt32(3, 9, 0);
            set => SetUInt32(3, 9, 0, value);
        }

        public EvolutionConditionId EvolutionCondition1
        {
            get => (EvolutionConditionId)GetUInt32(3, 4, 10);
            set => SetUInt32(3, 4, 10, (uint)value);
        }
        public EvolutionConditionId EvolutionCondition2
        {
            get => (EvolutionConditionId)GetUInt32(3, 4, 14);
            set => SetUInt32(3, 4, 14, (uint)value);
        }

        public uint Atk
        {
            get => GetUInt32(4, 9, 0);
            set => SetUInt32(4, 9, 0, value);
        }

        public uint Def
        {
            get => GetUInt32(4, 9, 10);
            set => SetUInt32(4, 9, 10, value);
        }

        public uint Spe
        {
            get => GetUInt32(4, 9, 20);
            set => SetUInt32(4, 9, 20, value);
        }

        public bool IsLegendary
        {
            get => GetUInt32(4, 1, 30) == 1u;
            set => SetUInt32(4, 1, 30, value ? 1u : 0u);
        }

        public TypeId Type1
        {
            get => (TypeId)GetUInt32(5, 5, 0);
            set => SetUInt32(5, 5, 0, (uint)value);
        }

        public TypeId Type2
        {
            get => (TypeId)GetUInt32(5, 5, 5);
            set => SetUInt32(5, 5, 5, (uint)value);
        }

        public MoveId Move
        {
            get => (MoveId)GetUInt32(5, 8, 10);
            set => SetUInt32(5, 8, 10, (uint)value);
        }

        public AbilityId Ability1 // uint8 but their compression treats it as uint9 for no apparent reason
        {
            get => (AbilityId)GetUInt32(6, 8, 0);
            set => SetUInt32(6, 8, 0, (uint)value);
        }

        public AbilityId Ability2 // uint8, but their compression treats it as uint9 for no apparent reason
        {
            get => (AbilityId)GetUInt32(6, 8, 9);
            set => SetUInt32(6, 8, 9, (uint)value);
        }

        public AbilityId Ability3 // uint8, but their compression treats it as uint9 for no apparent reason
        {
            get => (AbilityId)GetUInt32(6, 8, 18);
            set => SetUInt32(6, 8, 18, (uint)value);
        }

        public uint QuantityForEvolutionCondition1
        {
            get => GetUInt32(7, 9, 0);
            set => SetUInt32(7, 9, 0, value);
        }

        public uint QuantityForEvolutionCondition2
        {
            get => GetUInt32(7, 9, 18);
            set => SetUInt32(7, 9, 18, value);
        }

        public uint NameOrderIndex
        {
            get => GetUInt32(11, 8, 0);
            set => SetUInt32(11, 8, 0, value);
        }

        public bool GetEncounterable(LocationId location, bool requiresLevel2)
        {
            var shift = (byte)location * 3 + (requiresLevel2 ? 1 : 0);
            return (BitConverter.ToUInt64(Data, 9 * 4) >> shift & 1) == 1;
        }

        public void SetEncounterable(LocationId location, bool requiresLevel2, bool value)
        {
            var shift = (byte)location * 3 + (requiresLevel2 ? 1 : 0);
            var num = BitConverter.ToUInt64(Data, 9 * 4) & ~(1uL << shift);
            if (value)
            {
                num |= 1UL << shift;
            }
            BitConverter.GetBytes(num).CopyTo(Data, 9 * 4);
        }

        #region ToString

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.WriteProperty("Name", Name);
            sb.WriteProperty("Types", $"{Type1} / {Type2}");
            sb.WriteProperty("Abilities", $"{Ability1} / {Ability2} / {Ability3}");
            sb.WriteProperty("Move", Move.ToString());

            sb.WriteProperty("Evolution Conditions", string.Format("{0} {1}/ {2} {3}",
                EvolutionCondition1,
                RenderQuantityForEvolutionCondition(EvolutionCondition1, QuantityForEvolutionCondition1),
                EvolutionCondition2,
                RenderQuantityForEvolutionCondition(EvolutionCondition2, QuantityForEvolutionCondition2)
                ));

            sb.WriteProperty("Stats", $"{Hp} HP / {Atk} Atk / {Def} Def / {Spe} Spe");

            return sb.ToString();
        }

        private static string RenderQuantityForEvolutionCondition(EvolutionConditionId id, uint quantity)
        {
            switch (id)
            {
                case EvolutionConditionId.Hp:
                case EvolutionConditionId.Attack:
                case EvolutionConditionId.Defence:
                case EvolutionConditionId.Speed:
                    return $"({quantity}) ";

                case EvolutionConditionId.Link:
                    return $"({quantity}%) ";

                case EvolutionConditionId.Location:
                    return $"({(LocationId)quantity}) ";

                case EvolutionConditionId.WarriorGender:
                    return $"({(GenderId)quantity}) ";

                case EvolutionConditionId.Item:
                    return $"({(ItemId)quantity}) ";

                case EvolutionConditionId.JoinOffer:
                case EvolutionConditionId.NoCondition:
                    return "";

                default:
                    throw new ArgumentException("Unexpected enum value");
            }
        }

        public IPokemon Clone()
        {
            return new Pokemon((byte[])Data.Clone());
        }

        #endregion
    }
}
