﻿// This file is automatically generated

#nullable enable

using RanseiLink.Core.Enums;
using RanseiLink.Core.Graphics;

namespace RanseiLink.Core.Models;

public partial class MoveRange : BaseDataWindow
{
    public const int DataLength = 0x4;
    public MoveRange(byte[] data) : base(data, DataLength) { }
    public MoveRange() : this(new byte[DataLength]) { }

    public bool GetInRange(int id)
    {
        return GetInt(0, 0 + id, 1) == 1;
    }

    public void SetInRange(int id, bool value)
    {
        SetInt(0, 0 + id, 1, value ? 1 : 0);
    }
}