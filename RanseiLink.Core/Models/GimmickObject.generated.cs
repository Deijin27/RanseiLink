﻿// This file is automatically generated

#nullable enable

using RanseiLink.Core.Enums;
using RanseiLink.Core.Graphics;

namespace RanseiLink.Core.Models;

public partial class GimmickObject : BaseDataWindow
{
    public const int DataLength = 0x4;
    public GimmickObject(byte[] data) : base(data, DataLength) { }
    public GimmickObject() : this(new byte[DataLength]) { }
}