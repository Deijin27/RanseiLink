using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models
{
    public class ScenarioAppearPokemon : BaseDataWindow
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
            SetByte((int)id, canAppear ? AppearsValue : DoesNotAppearValue);
        }

    }
}