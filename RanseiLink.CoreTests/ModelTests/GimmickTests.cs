using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.CoreTests.ModelTests;

public class GimmickTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        Gimmick a = new Gimmick(new byte[]
        {
                0x50, 0x69, 0x6C, 0x6C,
                0x61, 0x72, 0x20, 0x6F,
                0x66, 0x20, 0x46, 0x69,
                0x72, 0x65, 0x00, 0x00,
                0x00, 0x0A, 0x00, 0x00,
                0x41, 0x48, 0xAC, 0x80,
                0x00, 0xFF, 0xCA, 0x94,
                0x41, 0x20, 0x2A, 0x00,
                0xFF, 0x07, 0x01, 0x04,
                0x4A, 0x69, 0x73, 0x03,
        }, ConquestGameCode.VPYT);

        a.Name.Should().Be("Pillar of Fire");
        a.Image1.Should().Be(10);
        a.AttackType.Should().Be(TypeId.Fire);
        a.DestroyType.Should().Be(TypeId.Water);
        a.State1Sprite.Should().Be(GimmickObjectId.PillarOfFire_Ignited);
        a.State2Sprite.Should().Be(GimmickObjectId.PillarOfFire_Extinguished);
        a.Effect.Should().Be(MoveEffectId.NoEffect);
        a.Anim1.Should().Be((int)MoveAnimationId.Default);
        a.Anim2.Should().Be((int)MoveAnimationId.GreySmokeYellowStars);
        a.Range.Should().Be(GimmickRangeId.NoRange);

    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        Gimmick a = new Gimmick(ConquestGameCode.VPYT)
        {
            Name = "Pillar of Fire",
            Image1 = 24,
            AttackType = TypeId.Dragon,
            DestroyType = TypeId.Ghost,
            State1Sprite = GimmickObjectId.CogWheel,
            State2Sprite = GimmickObjectId.DriftingIce,
            Effect = MoveEffectId.ChanceToParalyzeTarget,
            Anim1 = (int)MoveAnimationId.BlackSplatter,
            Anim2 = (int)MoveAnimationId.BlueYellowStars,
            Range = GimmickRangeId.RightBackCorner,
        };

        a.Name.Should().Be("Pillar of Fire");
        a.Image1.Should().Be(24);
        a.AttackType.Should().Be(TypeId.Dragon);
        a.DestroyType.Should().Be(TypeId.Ghost);
        a.State1Sprite.Should().Be(GimmickObjectId.CogWheel);
        a.State2Sprite.Should().Be(GimmickObjectId.DriftingIce);
        a.Effect.Should().Be(MoveEffectId.ChanceToParalyzeTarget);
        a.Anim1.Should().Be((int)MoveAnimationId.BlackSplatter);
        a.Anim2.Should().Be((int)MoveAnimationId.BlueYellowStars);
        a.Range.Should().Be(GimmickRangeId.RightBackCorner);
    }
}
