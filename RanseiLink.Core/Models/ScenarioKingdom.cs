using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models
{
    public class ScenarioKingdom : BaseDataWindow
    {
        public const int DataLength = 0x11;
        public ScenarioKingdom(byte[] data) : base(data, DataLength) { }
        public ScenarioKingdom() : this(new byte[DataLength]) { }

        public int GetArmy(KingdomId kingdom)
        {
            return GetArmy((int)kingdom);
        }

        public void SetArmy(KingdomId kingdom, int armyId)
        {
            SetArmy((int)kingdom, armyId);
        }

        public int GetArmy(int kingdom)
        {
            return GetByte(kingdom);
        }

        public void SetArmy(int kingdom, int armyId)
        {
            SetByte(kingdom, (byte)armyId);
        }

    }
}