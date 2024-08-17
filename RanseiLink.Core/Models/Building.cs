using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public class Building : BaseDataWindow
{
    public static int DataLength(ConquestGameCode culture) => culture == ConquestGameCode.VPYJ ? 0x20 : 0x24;

    private readonly int _cultureNameLength;
    private readonly int _cultureBinOffset;
    private readonly int _cultureBC1Offset;

    public Building(byte[] data, ConquestGameCode culture) : base(data, DataLength(culture)) 
    {
        _cultureNameLength = culture == ConquestGameCode.VPYJ ? 0x10 : 0x12;
        _cultureBinOffset = culture == ConquestGameCode.VPYJ ? 0 : 1;
        _cultureBC1Offset = culture == ConquestGameCode.VPYJ ? 16 : 0;
    }
    public Building(ConquestGameCode culture) : this(new byte[DataLength(culture)], culture) { }

    public string Name
    {
        get => GetPaddedUtf8String(0, _cultureNameLength);
        set => SetPaddedUtf8String(0, _cultureNameLength, value);
    }

    #region Referenced Buildings

    public BuildingId Building1 // R4_C24_L8
    {
        get => (BuildingId)GetByte(_cultureNameLength + 1);
        set => SetByte(_cultureNameLength + 1, (byte)value);
    }

    public BuildingId Building2 // R5_C0_L8
    {
        get => (BuildingId)GetByte(_cultureNameLength + 2);
        set => SetByte(_cultureNameLength + 2, (byte)value);
    }

    public BuildingId Building3 // R5_C8_L8
    {
        get => (BuildingId)GetByte(_cultureNameLength + 3);
        set => SetByte(_cultureNameLength + 3, (byte)value);
    }

    public BuildingId Building4 // R5_C16_L8
    {
        get => (BuildingId)GetByte(_cultureNameLength + 4);
        set => SetByte(_cultureNameLength + 4, (byte)value);
    }

    public BuildingId Building5 // R5_C24_L8
    {
        get => (BuildingId)GetByte(_cultureNameLength + 5);
        set => SetByte(_cultureNameLength + 5, (byte)value);
    }

    public BuildingId Building6 // R6_C0_L8
    {
        get => (BuildingId)GetByte(_cultureNameLength + 6);
        set => SetByte(_cultureNameLength + 6, (byte)value);
    }

    public BuildingId Building7 // R6_C8_L8
    {
        get => (BuildingId)GetByte(_cultureNameLength + 7);
        set => SetByte(_cultureNameLength + 7, (byte)value);
    }

    public BuildingId Building8 // R5_C16_L8
    {
        get => (BuildingId)GetByte(_cultureNameLength + 8);
        set => SetByte(_cultureNameLength + 8, (byte)value);
    }

    public BuildingId[] Buildings
    {
        get => new[] { Building1, Building2, Building3, Building4, Building5, Building6, Building7, Building8 };
        set
        {
            if (value.Length == 8)
            {
                Building1 = value[0];
                Building2 = value[1];
                Building3 = value[2];
                Building4 = value[3];
                Building5 = value[4];
                Building6 = value[5];
                Building7 = value[6];
                Building8 = value[7];
            }
        }
    }

    #endregion

    public KingdomId Kingdom // R5_C24_L8
    {
        get => (KingdomId)GetByte(_cultureNameLength + 9);
        set => SetByte(_cultureNameLength + 9, (byte)value);
    }

    public BattleConfigId BattleConfig1
    {
        get => (BattleConfigId)GetInt(6 + _cultureBinOffset, _cultureBC1Offset + 0, 7);
        set => SetInt(6 + _cultureBinOffset, _cultureBC1Offset + 0, 7, (int)value);
    }

    public BattleConfigId BattleConfig2
    {
        get => (BattleConfigId)GetInt(6 + _cultureBinOffset, _cultureBC1Offset + 7, 7);
        set => SetInt(6 + _cultureBinOffset, _cultureBC1Offset + 7, 7, (int)value);
    }

    public BattleConfigId BattleConfig3
    {
        get => (BattleConfigId)GetInt(7 + _cultureBinOffset, 0, 7);
        set => SetInt(7 + _cultureBinOffset, 0, 7, (int)value);
    }

    public int Sprite1
    {
        get => GetInt(7 + _cultureBinOffset, 7, 7);
        set => SetInt(7 + _cultureBinOffset, 7, 7, (int)value);
    }

    public int Sprite2
    {
        get => GetInt(7 + _cultureBinOffset, 14, 7);
        set => SetInt(7 + _cultureBinOffset, 14, 7, (int)value);
    }

    public int Sprite3
    {
        get => GetInt(7 + _cultureBinOffset, 21, 7);
        set => SetInt(7 + _cultureBinOffset, 21, 7, (int)value);
    }

    public BuildingFunctionId Function
    {
        get => (BuildingFunctionId)GetInt(7 + _cultureBinOffset, 28, 4);
        set => SetInt(7 + _cultureBinOffset, 28, 4, (int)value);
    }

}