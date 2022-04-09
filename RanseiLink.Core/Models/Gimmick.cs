using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public class Gimmick : BaseDataWindow
{
    public const int DataLength = 0x28;
    public Gimmick(byte[] data) : base(data, DataLength) { }
    public Gimmick() : base(new byte[DataLength], DataLength) { }

    public string Name
    {
        get => GetPaddedUtf8String(0, 16);
        set => SetPaddedUtf8String(0, 16, value);
    }

    /// <summary>
    /// Top screen square image when you hover over the object
    /// </summary>
    public int Image
    {
        get => GetInt(4, 8, 8);
        set => SetInt(4, 8, 8, value);
    }

    /// <summary>
    /// If attack damage caused by this, this is the type of the attack
    /// </summary>
    public TypeId AttackType
    {
        get => (TypeId)GetInt(5, 0, 5);
        set => SetInt(5, 0, 5, (int)value);
    }

    /// <summary>
    /// The type of attack that can destroy this gimmick
    /// </summary>
    public TypeId DestroyType
    {
        get => (TypeId)GetInt(5, 5, 5);
        set => SetInt(5, 5, 5, (int)value);
    }

    /// <summary>
    /// Sprite shown on bottom screen in battle
    /// </summary>
    public GimmickObjectId State1Object
    {
        get => (GimmickObjectId)GetInt(5, 11, 7);
        set => SetInt(5, 11, 7, (int)value);
    }

    public GimmickObjectId State2Object
    {
        get => (GimmickObjectId)GetInt(5, 18, 7);
        set => SetInt(5, 18, 7, (int)value);
    }

    public MoveEffectId Effect
    {
        get => (MoveEffectId)GetInt(5, 25, 7);
        set => SetInt(5, 25, 7, (int)value);
    }

    /// <summary>
    /// Seems like a multipurpose quantity. For some, this is probably attack power, others something else
    /// </summary>
    public int UnknownQuantity1
    {
        get => GetInt(6, 0, 8);
        set => SetInt(6, 0, 8, value);
    }

    public MoveAnimationId Animation1
    {
        get => (MoveAnimationId)GetInt(6, 8, 8);
        set => SetInt(6, 8, 8, (int)value);
    }

    public MoveAnimationId Animation2
    {
        get => (MoveAnimationId)GetInt(6, 16, 8);
        set => SetInt(6, 16, 8, (int)value);
    }

    public GimmickRangeId Range
    {
        get => (GimmickRangeId)GetInt(8, 19, 5);
        set => SetInt(8, 19, 5, (int)value);
    }

}
