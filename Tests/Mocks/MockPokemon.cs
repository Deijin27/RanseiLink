using Core;
using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.Mocks
{
    class MockPokemon : IPokemon
    {
        public MockPokemon()
        {
            RequiresLv2Dict = new Dictionary<LocationId, bool>();
            DefaultDict = new Dictionary<LocationId, bool>();
            foreach (var val in EnumUtil.GetValues<LocationId>())
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

        Dictionary<LocationId, bool> RequiresLv2Dict;
        Dictionary<LocationId, bool> DefaultDict;

        public bool GetEncounterable(LocationId location, bool requiresLevel2)
        {
            if (requiresLevel2)
            {
                return RequiresLv2Dict[location];
            }
            return DefaultDict[location];
        }

        public void SetEncounterable(LocationId location, bool requiresLevel2, bool value)
        {
            if (requiresLevel2)
            {
                RequiresLv2Dict[location] = value;
            }
            else
            {
                DefaultDict[location] = value;
            }
        }

        public IPokemon Clone()
        {
            var cloneRequiresDict = new Dictionary<LocationId, bool>();
            var cloneDefaultDict = new Dictionary<LocationId, bool>();
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
                QuantityForEvolutionCondition1 = QuantityForEvolutionCondition1,
                QuantityForEvolutionCondition2 = QuantityForEvolutionCondition2,
                Spe = Spe,
                Type1 = Type1,
                Type2 = Type2,
                Data = Data,
                RequiresLv2Dict = cloneRequiresDict,
                DefaultDict = cloneDefaultDict
            };
        }
    }
}
