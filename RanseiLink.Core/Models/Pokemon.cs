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

    public IdleMotionId IdleMotion
    {
        get => (IdleMotionId)GetUInt32(3, 2, 29);
        set => SetUInt32(3, 2, 29, (uint)value);
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

    public bool AsymmetricBattleSprite
    {
        get => GetUInt32(4, 1, 31) == 1u;
        set => SetUInt32(4, 1, 31, value ? 1u : 0u);
    }

    public bool LongAttackAnimation
    {
        get => GetUInt32(7, 1, 31) == 1u;
        set => SetUInt32(7, 1, 31, value ? 1u : 0u);
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

    public uint MovementRange
    {
        get => GetUInt32(7, 3, 27);
        set => SetUInt32(7, 3, 27, value);
    }

    public uint MinEvolutionTableEntry
    {
        get => GetUInt32(8, 11, 0);
        set => SetUInt32(8, 11, 0, value);
    }

    public uint MaxEvolutionTableEntry
    {
        get => GetUInt32(8, 11, 11);
        set => SetUInt32(8, 11, 11, value);
    }

    public List<PokemonId> Evolutions { get; set; } = new();

    public uint NationalPokedexNumber
    {
        get => GetUInt32(8, 10, 22);
        set => SetUInt32(8, 10, 22, value);
    }

    public uint NameOrderIndex
    {
        get => GetUInt32(11, 8, 0);
        set => SetUInt32(11, 8, 0, value);
    }

    public uint UnknownValue
    {
        get => GetUInt32(2, 8, 24);
        set => SetUInt32(2, 8, 24, value);
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
