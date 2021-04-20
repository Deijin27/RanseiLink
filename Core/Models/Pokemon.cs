using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Core.Enums;
using Core.Structs;

namespace Core.Models
{
    public class Pokemon : BaseDataWindow
    {
        public const int DataLength = 0x30;
        public Pokemon(byte[] data) : base(data, DataLength) { }

        public Pokemon() : base(new byte[DataLength], DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 11);
            set => SetPaddedUtf8String(0, 11, value);
        }

        public UInt9 Hp
        {
            get => (UInt9)GetUInt16(12);
        }

        public EvolutionConditionId EvolutionCondition1
        {
            get => (EvolutionConditionId)(byte)(UInt4)(GetByte(13) >> 2);
        }
        public EvolutionConditionId EvolutionCondition2
        {
            get => (EvolutionConditionId)(byte)(UInt4)(GetUInt16(13) >> 6);
        }

        public UInt9 Atk
        {
            get => (UInt9)GetUInt16(16);
        }

        public UInt9 Def
        {
            get => (UInt9)(GetUInt32(16) >> 10);
        }

        public UInt9 Spe
        {
            get => (UInt9)(GetUInt32(16) >> 20);
        }

        public TypeId Type1
        {
            get => (TypeId)(int)(UInt5)GetByte(0x14);
            set => SetByte(0x14, (byte)(GetByte(0x14) & ~(0b11111) | (int)value));
        }

        public TypeId Type2
        {
            get => (TypeId)(int)(UInt5)(GetUInt16(0x14) >> 5);
            set => SetUInt16(0x14, (byte)(GetByte(0x14) & ~(0b11111 << 5) | ((int)value << 5)));
        }

        public MoveId Move // uint8
        {
            get => (MoveId)(GetUInt16(0x15) >> 2); // masking is done automatically by the cast since the value is exactly 1 byte long
            set => SetUInt16(0x15, (ushort)((GetUInt16(0x15) & ~(0xFF << 2)) | ((int)value << 2)));
        }

        public AbilityId Ability1 // uint8 but their compression treats it as uint9 for no apparent reason
        {
            get => (AbilityId)GetByte(0x18);
            set => SetByte(0x18, (byte)value);
        }

        public AbilityId Ability2 // uint8, but their compression treats it as uint9 for no apparent reason
        {
            get => (AbilityId)(GetUInt16(0x19) >> 1);
            set => SetUInt16(0x19, (ushort)(GetUInt16(0x19) & ~(0b11111111 << 1) | ((int)value << 1)));
        }

        public AbilityId Ability3 // uint8, but their compression treats it as uint9 for no apparent reason
        {
            get => (AbilityId)(GetUInt16(0x1A) >> 2);
            set => SetUInt16(0x1A, (ushort)(GetUInt16(0x1A) & ~(0b11111111 << 2) | ((int)value << 2)));
        }

        public UInt9 QuantityForEvolutionCondition1
        {
            get => (UInt9)GetUInt32(28);
        }

        public UInt9 QuantityForEvolutionCondition2
        {
            get => (UInt9)(GetUInt32(28) >> 18);
        }

    }
}
