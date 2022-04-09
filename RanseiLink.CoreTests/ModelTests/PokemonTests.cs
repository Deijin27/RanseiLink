using RanseiLink.Core.Models;
using RanseiLink.Core.Enums;
using Xunit;
using RanseiLink.Core;

namespace RanseiLink.CoreTests.ModelTests;

public class PokemonTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        Pokemon p = new Pokemon(new byte[]
        {
                0x47, 0x61, 0x6C, 0x6C, 0x61, 0x64, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2D, 0xF6, 0x9C, 0x01, 0x49,
                0xFF, 0xE4, 0x52, 0x0A, 0xCA, 0x4C, 0xF9, 0x0F, 0x1B, 0xC4, 0xAC, 0x50, 0x69, 0xFE, 0x03, 0x18,
                0x78, 0xC5, 0xEB, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00
        });

        Assert.Equal("Gallade", p.Name);
        Assert.Equal(TypeId.Psychic, p.Type1);
        Assert.Equal(TypeId.Fighting, p.Type2);
        Assert.Equal(AbilityId.Parry, p.Ability1);
        Assert.Equal(AbilityId.Conqueror, p.Ability2);
        Assert.Equal(AbilityId.Justified, p.Ability3);
        Assert.Equal(MoveId.PsychoCut, p.Move);
        Assert.Equal(EvolutionConditionId.Item, p.EvolutionCondition1);
        Assert.Equal(ItemId.DawnStone, (ItemId)p.QuantityForEvolutionCondition1);
        Assert.Equal(EvolutionConditionId.WarriorGender, p.EvolutionCondition2);
        Assert.Equal(GenderId.Male, (GenderId)p.QuantityForEvolutionCondition2);
        Assert.Equal(3, p.MovementRange);
        Assert.Equal(246, p.Hp);
        Assert.Equal(255, p.Atk);
        Assert.Equal(185, p.Def);
        Assert.Equal(165, p.Spe);
        Assert.False(p.IsLegendary);
        Assert.Equal(0x40, p.NameOrderIndex);
        Assert.Equal(475, p.NationalPokedexNumber);
        foreach (KingdomId location in EnumUtil.GetValuesExceptDefaults<KingdomId>())
        {
            Assert.False(p.GetEncounterable(location, false));
        }
    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        Pokemon p = new Pokemon
        {
            Name = "Gallade",
            Type1 = TypeId.Psychic,
            Type2 = TypeId.Fighting,
            Ability1 = AbilityId.Parry,
            Ability2 = AbilityId.Conqueror,
            Ability3 = AbilityId.Justified,
            Move = MoveId.PsychoCut,
            EvolutionCondition1 = EvolutionConditionId.Item,
            QuantityForEvolutionCondition1 = (int)ItemId.DawnStone,
            EvolutionCondition2 = EvolutionConditionId.WarriorGender,
            QuantityForEvolutionCondition2 = (int)GenderId.Male,
            MovementRange = 3,
            Hp = 246,
            Atk = 255,
            Def = 185,
            Spe = 165,
            IsLegendary = true,
            NameOrderIndex = 0x40,
            NationalPokedexNumber = 475,
        };
        p.SetEncounterable(KingdomId.Aurora, true, true);
        p.SetEncounterable(KingdomId.Cragspur, false, true);
        p.SetEncounterable(KingdomId.Chrysalia, true, false);
        p.SetEncounterable(KingdomId.Illusio, false, false);

        Assert.Equal("Gallade", p.Name);
        Assert.Equal(TypeId.Psychic, p.Type1);
        Assert.Equal(TypeId.Fighting, p.Type2);
        Assert.Equal(AbilityId.Parry, p.Ability1);
        Assert.Equal(AbilityId.Conqueror, p.Ability2);
        Assert.Equal(AbilityId.Justified, p.Ability3);
        Assert.Equal(MoveId.PsychoCut, p.Move);
        Assert.Equal(EvolutionConditionId.Item, p.EvolutionCondition1);
        Assert.Equal(ItemId.DawnStone, (ItemId)p.QuantityForEvolutionCondition1);
        Assert.Equal(EvolutionConditionId.WarriorGender, p.EvolutionCondition2);
        Assert.Equal(GenderId.Male, (GenderId)p.QuantityForEvolutionCondition2);
        Assert.Equal(3, p.MovementRange);
        Assert.Equal(246, p.Hp);
        Assert.Equal(255, p.Atk);
        Assert.Equal(185, p.Def);
        Assert.Equal(165, p.Spe);
        Assert.True(p.IsLegendary);
        Assert.Equal(0x40, p.NameOrderIndex);
        Assert.Equal(475, p.NationalPokedexNumber);

        Assert.True(p.GetEncounterable(KingdomId.Aurora, true));
        Assert.True(p.GetEncounterable(KingdomId.Cragspur, false));
        Assert.False(p.GetEncounterable(KingdomId.Chrysalia, true));
        Assert.False(p.GetEncounterable(KingdomId.Illusio, false));

        // Add Array equal test when possible
    }
}
