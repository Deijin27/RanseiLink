using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.Core.Models;

public class ScenarioAppearPokemon : BaseDataWindow, IScenarioAppearPokemon
{
    public const int DataLength = 0xC8;
    public ScenarioAppearPokemon(byte[] data) : base(data, DataLength) { }
    public ScenarioAppearPokemon() : this(new byte[DataLength]) { }
    public bool GetCanAppear(PokemonId id)
    {
        return GetByte((int)id) == AppearsValue;
    }

    private const byte AppearsValue = 0x20;
    private const byte DoesNotAppearValue = 0;

    public void SetCanAppear(PokemonId id, bool canAppear)
    {
        SetByte((int)id, (byte)(canAppear ? AppearsValue : DoesNotAppearValue));
    }

    public IScenarioAppearPokemon Clone()
    {
        return new ScenarioAppearPokemon((byte[])Data.Clone());
    }
}
