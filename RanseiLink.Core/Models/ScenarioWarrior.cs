using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using System;

namespace RanseiLink.Core.Models;

public class ScenarioWarrior : BaseDataWindow, IScenarioWarrior
{
    public const int DataLength = 0x20;

    public ScenarioWarrior(byte[] data) : base(data, DataLength) { }

    public ScenarioWarrior() : this(new byte[DataLength]) { }

    public WarriorId Warrior
    {
        get => (WarriorId)GetByte(0);
        set => SetByte(0, (byte)value);
    }

    public WarriorClassId Class
    {
        get => (WarriorClassId)GetUInt32(0, 3, 9);
        set => SetUInt32(0, 3, 9, (uint)value);
    }

    public KingdomId Kingdom
    {
        get => (KingdomId)GetUInt32(0, 5, 12);
        set => SetUInt32(0, 5, 12, (uint)value);
    }

    public uint Army
    {
        get => GetUInt32(0, 5, 17);
        set => SetUInt32(0, 5, 17, value);
    }

    public ushort GetScenarioPokemon(int id)
    {
        if (id > 7 || id < 0)
        {
            throw new ArgumentOutOfRangeException($"{nameof(id)} is out of range. Scenario warriors only have 8 pokemon");
        }
        return GetUInt16(0xE + id * 2);
    }

    public void SetScenarioPokemon(int id, ushort value)
    {
        if (id > 7 || id < 0)
        {
            throw new ArgumentOutOfRangeException($"{nameof(id)} is out of range. Scenario warriors only have 8 pokemon");
        }
        SetUInt16(0xE + id * 2, value);
    }

    public IScenarioWarrior Clone()
    {
        return new ScenarioWarrior((byte[])Data.Clone());
    }

    public void MakeScenarioPokemonDefault(int id)
    {
        SetScenarioPokemon(id, DefaultScenarioPokemon);
    }

    public bool ScenarioPokemonIsDefault(int id)
    {
        return GetScenarioPokemon(id) == DefaultScenarioPokemon; 
    }

    public const ushort DefaultScenarioPokemon = 1100;
}
