﻿// This file is automatically generated

#nullable enable

using RanseiLink.Core.Enums;
using RanseiLink.Core.Graphics;

namespace RanseiLink.Core.Models;

public partial class EventSpeaker : BaseDataWindow, INamedModel
{
    public event EventHandler? NameChanged;
    private ConquestGameCode _culture;
    public static int DataLength(ConquestGameCode culture)
    {
        return culture switch
        {
            ConquestGameCode.VPYJ => 0xC,
            _ => 0x12
        };
    }
    public EventSpeaker(byte[] data, ConquestGameCode culture) : base(data, DataLength(culture)) { _culture = culture; }
    public EventSpeaker(ConquestGameCode culture) : this(new byte[DataLength(culture)], culture) { }

    public int Name_MaxLength
    {
        get
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                return 0xA;
            }
            else
            {
                return 0x10;
            }
        }
    }
    public string Name
    {
        get => GetPaddedUtf8String(0, Name_MaxLength);
        set
        {
            SetPaddedUtf8String(0, Name_MaxLength, value);
            NameChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public int Sprite
    {
        get
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                return GetByte(0xB);
            }
            else
            {
                return GetByte(0x11);
            }
        }
        set
        {
            if (_culture == ConquestGameCode.VPYJ)
            {
                SetByte(0xB, (byte)value);
            }
            else
            {
                SetByte(0x11, (byte)value);
            }
        }
    }
}