﻿// This file is automatically generated

#nullable enable

using RanseiLink.Core.Enums;
using RanseiLink.Core.Graphics;

namespace RanseiLink.Core.Models;

public partial class Gimmick : BaseDataWindow
{
    private ConquestGameCode _culture;
    public static int DataLength(ConquestGameCode culture)
    {
        return culture switch
        {
            ConquestGameCode.VPYJ => 0x24,
            _ => 0x28
        };
    }
    public Gimmick(byte[] data, ConquestGameCode culture) : base(data, DataLength(culture)) { _culture = culture; }
    public Gimmick(ConquestGameCode culture) : this(new byte[DataLength(culture)], culture) { }

    public int Name_MaxLength
    {
        get
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                return 14;
            }
            else
            {
                return 16;
            }
        }
    }
    public string Name
    {
        get => GetPaddedUtf8String(0, Name_MaxLength);
        set => SetPaddedUtf8String(0, Name_MaxLength, value);
    }

    public int Image
    {
        get
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                return GetByte(15);
            }
            else
            {
                return GetByte(17);
            }
        }
        set
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                SetByte(15, (byte)value);
            }
            else
            {
                SetByte(17, (byte)value);
            }
        }
    }

    public TypeId AttackType
    {
        get
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                return (TypeId)GetInt(4, 0, 5);
            }
            else
            {
                return (TypeId)GetInt(5, 0, 5);
            }
        }
        set
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                SetInt(4, 0, 5, (int)value);
            }
            else
            {
                SetInt(5, 0, 5, (int)value);
            }
        }
    }

    public TypeId DestroyType
    {
        get
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                return (TypeId)GetInt(4, 5, 5);
            }
            else
            {
                return (TypeId)GetInt(5, 5, 5);
            }
        }
        set
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                SetInt(4, 5, 5, (int)value);
            }
            else
            {
                SetInt(5, 5, 5, (int)value);
            }
        }
    }

    public GimmickObjectId State1Sprite
    {
        get
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                return (GimmickObjectId)GetInt(4, 11, 7);
            }
            else
            {
                return (GimmickObjectId)GetInt(5, 11, 7);
            }
        }
        set
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                SetInt(4, 11, 7, (int)value);
            }
            else
            {
                SetInt(5, 11, 7, (int)value);
            }
        }
    }

    public GimmickObjectId State2Sprite
    {
        get
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                return (GimmickObjectId)GetInt(4, 18, 7);
            }
            else
            {
                return (GimmickObjectId)GetInt(5, 18, 7);
            }
        }
        set
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                SetInt(4, 18, 7, (int)value);
            }
            else
            {
                SetInt(5, 18, 7, (int)value);
            }
        }
    }

    public MoveEffectId Effect
    {
        get
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                return (MoveEffectId)GetInt(4, 25, 7);
            }
            else
            {
                return (MoveEffectId)GetInt(5, 25, 7);
            }
        }
        set
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                SetInt(4, 25, 7, (int)value);
            }
            else
            {
                SetInt(5, 25, 7, (int)value);
            }
        }
    }

    public int UnknownQuantity1
    {
        get
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                return GetInt(5, 0, 8);
            }
            else
            {
                return GetInt(6, 0, 8);
            }
        }
        set
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                SetInt(5, 0, 8, value);
            }
            else
            {
                SetInt(6, 0, 8, value);
            }
        }
    }

    public MoveAnimationId Animation1
    {
        get
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                return (MoveAnimationId)GetInt(5, 8, 8);
            }
            else
            {
                return (MoveAnimationId)GetInt(6, 8, 8);
            }
        }
        set
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                SetInt(5, 8, 8, (int)value);
            }
            else
            {
                SetInt(6, 8, 8, (int)value);
            }
        }
    }

    public MoveAnimationId Animation2
    {
        get
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                return (MoveAnimationId)GetInt(5, 16, 8);
            }
            else
            {
                return (MoveAnimationId)GetInt(6, 16, 8);
            }
        }
        set
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                SetInt(5, 16, 8, (int)value);
            }
            else
            {
                SetInt(6, 16, 8, (int)value);
            }
        }
    }

    public GimmickRangeId Range
    {
        get
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                return (GimmickRangeId)GetInt(7, 19, 5);
            }
            else
            {
                return (GimmickRangeId)GetInt(8, 19, 5);
            }
        }
        set
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                SetInt(7, 19, 5, (int)value);
            }
            else
            {
                SetInt(8, 19, 5, (int)value);
            }
        }
    }
}