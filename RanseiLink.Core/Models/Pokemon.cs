using System;
using System.Collections.Generic;
using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public class PokemonEvolutionRange
{
    public uint MinEntry { get; set; }
    public uint MaxEntry { get; set; }
    public bool CanEvolve { get; set; }
}

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

    public uint Hp
    {
        get => GetUInt32(3, 0, 9);
        set => SetUInt32(3, 0, 9, value);
    }

    public EvolutionConditionId EvolutionCondition1
    {
        get => (EvolutionConditionId)GetUInt32(3, 10, 4);
        set => SetUInt32(3, 10, 4, (uint)value);
    }
    public EvolutionConditionId EvolutionCondition2
    {
        get => (EvolutionConditionId)GetUInt32(3, 14, 4);
        set => SetUInt32(3, 14, 4, (uint)value);
    }

    public IdleMotionId IdleMotion
    {
        get => (IdleMotionId)GetUInt32(3, 29, 2);
        set => SetUInt32(3, 29, 2, (uint)value);
    }

    public uint Atk
    {
        get => GetUInt32(4, 0, 9);
        set => SetUInt32(4, 0, 9, value);
    }

    public uint Def
    {
        get => GetUInt32(4, 10, 9);
        set => SetUInt32(4, 10, 9, value);
    }

    public uint Spe
    {
        get => GetUInt32(4, 20, 9);
        set => SetUInt32(4, 20, 9, value);
    }

    public bool IsLegendary
    {
        get => GetUInt32(4, 30, 1) == 1u;
        set => SetUInt32(4, 30, 1, value ? 1u : 0u);
    }

    public bool AsymmetricBattleSprite
    {
        get => GetUInt32(4, 31, 1) == 1u;
        set => SetUInt32(4, 31, 1, value ? 1u : 0u);
    }

    public bool LongAttackAnimation
    {
        get => GetUInt32(7, 31, 1) == 1u;
        set => SetUInt32(7, 31, 1, value ? 1u : 0u);
    }

    public TypeId Type1
    {
        get => (TypeId)GetUInt32(5, 0, 5);
        set => SetUInt32(5, 0, 5, (uint)value);
    }

    public TypeId Type2
    {
        get => (TypeId)GetUInt32(5, 5, 5);
        set => SetUInt32(5, 5, 5, (uint)value);
    }

    public MoveId Move
    {
        get => (MoveId)GetUInt32(5, 10, 8);
        set => SetUInt32(5, 10, 8, (uint)value);
    }

    public AbilityId Ability1 // uint8 but their compression treats it as uint9 for no apparent reason
    {
        get => (AbilityId)GetUInt32(6, 0, 8);
        set => SetUInt32(6, 0, 8, (uint)value);
    }

    public AbilityId Ability2 // uint8, but their compression treats it as uint9 for no apparent reason
    {
        get => (AbilityId)GetUInt32(6, 9, 8);
        set => SetUInt32(6, 9, 8, (uint)value);
    }

    public AbilityId Ability3 // uint8, but their compression treats it as uint9 for no apparent reason
    {
        get => (AbilityId)GetUInt32(6, 18, 8);
        set => SetUInt32(6, 18, 8, (uint)value);
    }

    public uint QuantityForEvolutionCondition1
    {
        get => GetUInt32(7, 0, 9);
        set => SetUInt32(7, 0, 9, value);
    }

    public uint QuantityForEvolutionCondition2
    {
        get => GetUInt32(7, 18, 9);
        set => SetUInt32(7, 18, 9, value);
    }

    public uint MovementRange
    {
        get => GetUInt32(7, 27, 3);
        set => SetUInt32(7, 27, 3, value);
    }

    public uint MinEvolutionTableEntry
    {
        get => GetUInt32(8, 0, 11);
        set => SetUInt32(8, 0, 11, value);
    }

    public uint MaxEvolutionTableEntry
    {
        get => GetUInt32(8, 11, 11);
        set => SetUInt32(8, 11, 11, value);
    }

    public List<PokemonId> Evolutions { get; set; } = new();

    public uint NationalPokedexNumber
    {
        get => GetUInt32(8, 22, 10);
        set => SetUInt32(8, 22, 10, value);
    }

    public uint NameOrderIndex
    {
        get => GetUInt32(11, 0, 8);
        set => SetUInt32(11, 0, 8, value);
    }

    public uint UnknownValue
    {
        get => GetUInt32(2, 24, 8);
        set => SetUInt32(2, 24, 8, value);
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
