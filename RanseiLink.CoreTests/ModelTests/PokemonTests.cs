using RanseiLink.Core.Models;
using RanseiLink.Core.Enums;
using Xunit;
using RanseiLink.Core;
using FluentAssertions;

namespace RanseiLink.CoreTests.ModelTests;

public class PokemonTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        Pokemon a = new Pokemon(new byte[]
        {
            0x47, 0x61, 0x6C, 0x6C, 0x61, 0x64, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2D, 0xF6, 0x9C, 0x01, 0x49,
            0xFF, 0xE4, 0x52, 0x0A, 0xCA, 0x4C, 0xF9, 0x0F, 0x1B, 0xC4, 0xAC, 0x50, 0x69, 0xFE, 0x03, 0x18,
            0x78, 0xC5, 0xEB, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00
        });

        a.Name.Should().Be("Gallade");
        a.Type1.Should().Be(TypeId.Psychic);
        a.Type2.Should().Be(TypeId.Fighting);
        a.Ability1.Should().Be(AbilityId.Parry);
        a.Ability2.Should().Be(AbilityId.Conqueror);
        a.Ability3.Should().Be(AbilityId.Justified);
        a.Move.Should().Be(MoveId.PsychoCut);
        a.EvolutionCondition1.Should().Be(EvolutionConditionId.Item);
        ((ItemId)a.QuantityForEvolutionCondition1).Should().Be(ItemId.DawnStone);
        a.EvolutionCondition2.Should().Be(EvolutionConditionId.WarriorGender);
        ((GenderId)a.QuantityForEvolutionCondition2).Should().Be(GenderId.Male);
        a.MovementRange.Should().Be(3);
        a.Hp.Should().Be(246);
        a.Atk.Should().Be(255);
        a.Def.Should().Be(185);
        a.Spe.Should().Be(165);
        a.IsLegendary.Should().BeFalse();
        a.NameOrderIndex.Should().Be(0x40);
        a.NationalPokedexNumber.Should().Be(475);
        a.CatchRate.Should().Be(45);

        foreach (KingdomId location in EnumUtil.GetValuesExceptDefaults<KingdomId>())
        {
            a.GetEncounterable(location, false).Should().BeFalse();
        }
    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        Pokemon a = new Pokemon
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
            CatchRate = 37,
        };
        a.SetEncounterable(KingdomId.Aurora, true, true);
        a.SetEncounterable(KingdomId.Cragspur, false, true);
        a.SetEncounterable(KingdomId.Chrysalia, true, false);
        a.SetEncounterable(KingdomId.Illusio, false, false);

        a.Name.Should().Be("Gallade");
        a.Type1.Should().Be(TypeId.Psychic);
        a.Type2.Should().Be(TypeId.Fighting);
        a.Ability1.Should().Be(AbilityId.Parry);
        a.Ability2.Should().Be(AbilityId.Conqueror);
        a.Ability3.Should().Be(AbilityId.Justified);
        a.Move.Should().Be(MoveId.PsychoCut);
        a.EvolutionCondition1.Should().Be(EvolutionConditionId.Item);
        ((ItemId)a.QuantityForEvolutionCondition1).Should().Be(ItemId.DawnStone);
        a.EvolutionCondition2.Should().Be(EvolutionConditionId.WarriorGender);
        ((GenderId)a.QuantityForEvolutionCondition2).Should().Be(GenderId.Male);
        a.MovementRange.Should().Be(3);
        a.Hp.Should().Be(246);
        a.Atk.Should().Be(255);
        a.Def.Should().Be(185);
        a.Spe.Should().Be(165);
        a.IsLegendary.Should().BeTrue();
        a.NameOrderIndex.Should().Be(0x40);
        a.NationalPokedexNumber.Should().Be(475);
        a.CatchRate.Should().Be(37);

        a.GetEncounterable(KingdomId.Aurora, true).Should().BeTrue();
        a.GetEncounterable(KingdomId.Cragspur, false).Should().BeTrue();
        a.GetEncounterable(KingdomId.Chrysalia, true).Should().BeFalse();
        a.GetEncounterable(KingdomId.Illusio, false).Should().BeFalse();

        // Add Array equal test when possible
    }
}
