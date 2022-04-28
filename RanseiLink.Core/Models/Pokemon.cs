using System;
using System.Collections.Generic;
using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models
{
    public class Pokemon : BaseDataWindow
    {
        public const int DataLength = 0x30;
        public Pokemon(byte[] data) : base(data, DataLength) { }

        public Pokemon() : base(new byte[DataLength], DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 10);
            set => SetPaddedUtf8String(0, 10, value);
        }

        public int Hp
        {
            get => GetInt(3, 0, 9);
            set => SetInt(3, 0, 9, value);
        }

        public EvolutionConditionId EvolutionCondition1
        {
            get => (EvolutionConditionId)GetInt(3, 10, 4);
            set => SetInt(3, 10, 4, (int)value);
        }
        public EvolutionConditionId EvolutionCondition2
        {
            get => (EvolutionConditionId)GetInt(3, 14, 4);
            set => SetInt(3, 14, 4, (int)value);
        }

        public IdleMotionId IdleMotion
        {
            get => (IdleMotionId)GetInt(3, 29, 2);
            set => SetInt(3, 29, 2, (int)value);
        }

        public int Atk
        {
            get => GetInt(4, 0, 9);
            set => SetInt(4, 0, 9, value);
        }

        public int Def
        {
            get => GetInt(4, 10, 9);
            set => SetInt(4, 10, 9, value);
        }

        public int Spe
        {
            get => GetInt(4, 20, 9);
            set => SetInt(4, 20, 9, value);
        }

        public bool IsLegendary
        {
            get => GetInt(4, 30, 1) == 1;
            set => SetInt(4, 30, 1, value ? 1 : 0);
        }

        public bool AsymmetricBattleSprite
        {
            get => GetInt(4, 31, 1) == 1;
            set => SetInt(4, 31, 1, value ? 1 : 0);
        }

        public bool LongAttackAnimation
        {
            get => GetInt(7, 31, 1) == 1;
            set => SetInt(7, 31, 1, value ? 1 : 0);
        }

        public TypeId Type1
        {
            get => (TypeId)GetInt(5, 0, 5);
            set => SetInt(5, 0, 5, (int)value);
        }

        public TypeId Type2
        {
            get => (TypeId)GetInt(5, 5, 5);
            set => SetInt(5, 5, 5, (int)value);
        }

        public MoveId Move
        {
            get => (MoveId)GetInt(5, 10, 8);
            set => SetInt(5, 10, 8, (int)value);
        }

        public AbilityId Ability1
        {
            get => (AbilityId)GetInt(6, 0, 8);
            set => SetInt(6, 0, 8, (int)value);
        }

        public AbilityId Ability2
        {
            get => (AbilityId)GetInt(6, 9, 8);
            set => SetInt(6, 9, 8, (int)value);
        }

        public AbilityId Ability3
        {
            get => (AbilityId)GetInt(6, 18, 8);
            set => SetInt(6, 18, 8, (int)value);
        }

        public int QuantityForEvolutionCondition1
        {
            get => GetInt(7, 0, 9);
            set => SetInt(7, 0, 9, value);
        }

        public int QuantityForEvolutionCondition2
        {
            get => GetInt(7, 18, 9);
            set => SetInt(7, 18, 9, value);
        }

        public int MovementRange
        {
            get => GetInt(7, 27, 3);
            set => SetInt(7, 27, 3, value);
        }

        public int MinEvolutionTableEntry
        {
            get => GetInt(8, 0, 11);
            set => SetInt(8, 0, 11, value);
        }

        public int MaxEvolutionTableEntry
        {
            get => GetInt(8, 11, 11);
            set => SetInt(8, 11, 11, value);
        }

        public List<PokemonId> Evolutions { get; set; } = new List<PokemonId>();

        public int NationalPokedexNumber
        {
            get => GetInt(8, 22, 10);
            set => SetInt(8, 22, 10, value);
        }

        public int NameOrderIndex
        {
            get => GetInt(11, 0, 8);
            set => SetInt(11, 0, 8, value);
        }

        public int CatchRate
        {
            get => GetInt(2, 24, 8);
            set => SetInt(2, 24, 8, value);
        }

        public int UnknownAnimationValue
        {
            get => GetInt(3, 18, 4);
            set => SetInt(3, 18, 4, value);
        }

        public bool GetEncounterable(KingdomId kingdom, bool requiresLevel2)
        {
            var shift = (byte)kingdom * 3 + (requiresLevel2 ? 1 : 0);
            return (BitConverter.ToUInt64(Data, 9 * 4) >> shift & 1) == 1;
        }

        public void SetEncounterable(KingdomId kingdom, bool requiresLevel2, bool value)
        {
            var shift = (byte)kingdom * 3 + (requiresLevel2 ? 1 : 0);
            var num = BitConverter.ToUInt64(Data, 9 * 4) & ~(1uL << shift);
            if (value)
            {
                num |= 1UL << shift;
            }
            BitConverter.GetBytes(num).CopyTo(Data, 9 * 4);
        }

    }
}