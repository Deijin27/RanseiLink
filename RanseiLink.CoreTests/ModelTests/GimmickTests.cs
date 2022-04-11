using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using Xunit;

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
        });

        Assert.Equal("Pillar of Fire", a.Name);
        Assert.Equal(10, a.Image);
        Assert.Equal(TypeId.Fire, a.AttackType);
        Assert.Equal(TypeId.Water, a.DestroyType);
        Assert.Equal(GimmickObjectId.PillarOfFire_State1, a.State1Object);
        Assert.Equal(GimmickObjectId.PillarOfFire_State2, a.State2Object);
        Assert.Equal(MoveEffectId.NoEffect, a.Effect);
        Assert.Equal(0, a.UnknownQuantity1);
        Assert.Equal(MoveAnimationId.Default, a.Animation1);
        Assert.Equal(MoveAnimationId.GreySmokeYellowStars, a.Animation2);
        Assert.Equal(GimmickRangeId.NoRange, a.Range);

    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        Gimmick a = new Gimmick
        {
            Name = "Pillar of Fire",
            Image = 24,
            AttackType = TypeId.Dragon,
            DestroyType = TypeId.Ghost,
            State1Object = GimmickObjectId.CogWheel,
            State2Object = GimmickObjectId.DriftingIce,
            Effect = MoveEffectId.ChanceToParalyzeTarget,
            UnknownQuantity1 = 4,
            Animation1 = MoveAnimationId.BlackSplatter,
            Animation2 = MoveAnimationId.BlueYellowStars,
            Range = GimmickRangeId.RightBackCorner,
        };

        Assert.Equal("Pillar of Fire", a.Name);
        Assert.Equal(24, a.Image);
        Assert.Equal(TypeId.Dragon, a.AttackType);
        Assert.Equal(TypeId.Ghost, a.DestroyType);
        Assert.Equal(GimmickObjectId.CogWheel, a.State1Object);
        Assert.Equal(GimmickObjectId.DriftingIce, a.State2Object);
        Assert.Equal(MoveEffectId.ChanceToParalyzeTarget, a.Effect);
        Assert.Equal(4, a.UnknownQuantity1);
        Assert.Equal(MoveAnimationId.BlackSplatter, a.Animation1);
        Assert.Equal(MoveAnimationId.BlueYellowStars, a.Animation2);
        Assert.Equal(GimmickRangeId.RightBackCorner, a.Range);
    }
}
