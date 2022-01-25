using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RanseiLink.Tests.Mocks;

class MockPokemon : IPokemon
{
    public MockPokemon()
    {
        RequiresLv2Dict = new Dictionary<KingdomId, bool>();
        DefaultDict = new Dictionary<KingdomId, bool>();
        foreach (var val in EnumUtil.GetValuesExceptDefaults<KingdomId>())
        {
            RequiresLv2Dict[val] = false;
            DefaultDict[val] = false;
        }

        Data = new byte[Pokemon.DataLength]
        {
                0x47, 0x61, 0x6C, 0x6C, 0x61, 0x64, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2D, 0xF6, 0x9C, 0x01, 0x49,
                0xFF, 0xE4, 0x52, 0x0A, 0xCA, 0x4C, 0xF9, 0x0F, 0x1B, 0xC4, 0xAC, 0x50, 0x69, 0xFE, 0x03, 0x18,
                0x78, 0xC5, 0xEB, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00
        };
    }

    public AbilityId Ability1 { get; set; }
    public AbilityId Ability2 { get; set; }
    public AbilityId Ability3 { get; set; }
    public uint Atk { get; set; }
    public uint Def { get; set; }
    public EvolutionConditionId EvolutionCondition1 { get; set; }
    public EvolutionConditionId EvolutionCondition2 { get; set; }
    public uint Hp { get; set; }
    public bool IsLegendary { get; set; }
    public MoveId Move { get; set; }
    public string Name { get; set; }
    public uint NameOrderIndex { get; set; }
    public uint QuantityForEvolutionCondition1 { get; set; }
    public uint QuantityForEvolutionCondition2 { get; set; }
    public uint Spe { get; set; }
    public TypeId Type1 { get; set; }
    public TypeId Type2 { get; set; }

    public byte[] Data { get; set; }
    public uint NationalPokedexNumber { get; set; }
    public uint MovementRange { get; set; }
    public PokemonEvolutionRange EvolutionRange { get; set; } = new PokemonEvolutionRange();
    public uint UnknownValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public IdleMotionId IdleMotion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    Dictionary<KingdomId, bool> RequiresLv2Dict;
    Dictionary<KingdomId, bool> DefaultDict;

    public bool GetEncounterable(KingdomId kingdom, bool requiresLevel2)
    {
        if (requiresLevel2)
        {
            return RequiresLv2Dict[kingdom];
        }
        return DefaultDict[kingdom];
    }

    public void SetEncounterable(KingdomId kingdom, bool requiresLevel2, bool value)
    {
        if (requiresLevel2)
        {
            RequiresLv2Dict[kingdom] = value;
        }
        else
        {
            DefaultDict[kingdom] = value;
        }
    }

    public IPokemon Clone()
    {
        var cloneRequiresDict = new Dictionary<KingdomId, bool>();
        var cloneDefaultDict = new Dictionary<KingdomId, bool>();
        foreach (var i in RequiresLv2Dict)
        {
            cloneRequiresDict[i.Key] = i.Value;
        }
        foreach (var i in DefaultDict)
        {
            cloneDefaultDict[i.Key] = i.Value;
        }

        return new MockPokemon()
        {
            Ability1 = Ability1,
            Ability2 = Ability2,
            Ability3 = Ability3,
            Atk = Atk,
            Def = Def,
            EvolutionCondition1 = EvolutionCondition1,
            EvolutionCondition2 = EvolutionCondition2,
            Hp = Hp,
            IsLegendary = IsLegendary,
            Move = Move,
            Name = Name,
            NameOrderIndex = NameOrderIndex,
            MovementRange = MovementRange,
            QuantityForEvolutionCondition1 = QuantityForEvolutionCondition1,
            QuantityForEvolutionCondition2 = QuantityForEvolutionCondition2,
            Spe = Spe,
            Type1 = Type1,
            Type2 = Type2,
            EvolutionRange = new PokemonEvolutionRange()
            {
                CanEvolve = EvolutionRange.CanEvolve,
                MaxEntry = EvolutionRange.MaxEntry,
                MinEntry = EvolutionRange.MinEntry,
            },
            Data = Data,
            RequiresLv2Dict = cloneRequiresDict,
            DefaultDict = cloneDefaultDict
        };
    }

    public string Serialize()
    {
        throw new NotImplementedException();
    }

    public bool TryDeserialize(string serialized)
    {
        throw new NotImplementedException();
    }
}
