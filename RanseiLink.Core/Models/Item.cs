﻿using System;
using System.Collections.Generic;
using System.Text;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.Core.Models
{
    public class Item : BaseDataWindow, IItem
    {
        public const int DataLength = 0x24;
        public Item(byte[] data) : base(data, DataLength)
        {
        }

        public Item() : base(new byte[DataLength], DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 20);
            set => SetPaddedUtf8String(0, 20, value);
        }

        public IItem Clone()
        {
            return new Item((byte[])Data.Clone());
        }
    }
}