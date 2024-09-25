using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;

namespace RanseiLink.Core.Models;

public partial class BattleConfig
{
    public MapId MapId
    {
        get => new(Map, MapVariant);
        set
        {
            Map = value.Map;
            MapVariant = value.Variant;
        }
    }

    private const int _itemStartByte = 12;

    public ItemId Treasure1
    {
        get => (ItemId)GetByte(_itemStartByte + 0);
        set => SetByte(_itemStartByte + 0, (byte)value);
    }

    public ItemId Treasure2
    {
        get => (ItemId)GetByte(_itemStartByte + 1);
        set => SetByte(_itemStartByte + 1, (byte)value);
    }

    public ItemId Treasure3
    {
        get => (ItemId)GetByte(_itemStartByte + 2);
        set => SetByte(_itemStartByte + 2, (byte)value);
    }

    public ItemId Treasure4
    {
        get => (ItemId)GetByte(_itemStartByte + 3);
        set => SetByte(_itemStartByte + 3, (byte)value);
    }

    public ItemId Treasure5
    {
        get => (ItemId)GetByte(_itemStartByte + 4);
        set => SetByte(_itemStartByte + 4, (byte)value);
    }

    public ItemId Treasure6
    {
        get => (ItemId)GetByte(_itemStartByte + 5);
        set => SetByte(_itemStartByte + 5, (byte)value);
    }

    public ItemId Treasure7
    {
        get => (ItemId)GetByte(_itemStartByte + 6);
        set => SetByte(_itemStartByte + 6, (byte)value);
    }

    public ItemId Treasure8
    {
        get => (ItemId)GetByte(_itemStartByte + 7);
        set => SetByte(_itemStartByte + 7, (byte)value);
    }

    public ItemId Treasure9
    {
        get => (ItemId)GetByte(_itemStartByte + 8);
        set => SetByte(_itemStartByte + 8, (byte)value);
    }

    public ItemId Treasure10
    {
        get => (ItemId)GetByte(_itemStartByte + 9);
        set => SetByte(_itemStartByte + 9, (byte)value);
    }

    public ItemId Treasure11
    {
        get => (ItemId)GetByte(_itemStartByte + 10);
        set => SetByte(_itemStartByte + 10, (byte)value);
    }

    public ItemId Treasure12
    {
        get => (ItemId)GetByte(_itemStartByte + 11);
        set => SetByte(_itemStartByte + 11, (byte)value);
    }
}