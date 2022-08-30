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
            return GetCanAppear((int)id);
        }

        private const byte AppearsValue = 0x20;
        private const byte DoesNotAppearValue = 0;

        public void SetCanAppear(PokemonId id, bool canAppear)
        {
            SetCanAppear((int)id, canAppear);
        }

        public bool GetCanAppear(int id)
        {
            return GetByte(id) == AppearsValue;
        }

        public void SetCanAppear(int id, bool canAppear)
        {
            SetByte(id, canAppear ? AppearsValue : DoesNotAppearValue);
        }

    }
}