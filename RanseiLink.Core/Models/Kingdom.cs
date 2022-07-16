using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models
{
    public class Kingdom : BaseDataWindow
    {
        public static int DataLength(ConquestGameCode culture) => culture == ConquestGameCode.VPYJ ? 0x14 : 0x18;

        private readonly int _cultureNameLength;
        private readonly int _cultureBinOffset;
        private readonly int _cultureFirstByteDataOffset;
        private readonly int _cultureSecondByteOffset;
        public Kingdom(byte[] data, ConquestGameCode culture = ConquestGameCode.VPYT) : base(data, DataLength(culture)) 
        {
            _cultureNameLength = culture == ConquestGameCode.VPYJ ? 0x8 : 0xA;
            _cultureBinOffset = culture == ConquestGameCode.VPYJ ? 0 : 1;
            _cultureFirstByteDataOffset = culture == ConquestGameCode.VPYJ ? 8 : 24;
            _cultureSecondByteOffset = culture == ConquestGameCode.VPYJ ? 16 : 0;
        }

        public Kingdom(ConquestGameCode culture = ConquestGameCode.VPYT) : this(new byte[DataLength(culture)]) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, _cultureNameLength);
            set => SetPaddedUtf8String(0, _cultureNameLength, value);
        }

        public int Unknown_R2_C24_L3
        {
            get => GetInt(2, _cultureFirstByteDataOffset, 3);
            set => SetInt(2, _cultureFirstByteDataOffset, 3, value);
        }

        #region Kingdoms you can battle using warriors in this kingdom

        public KingdomId MapConnection0
        {
            get => (KingdomId)GetInt(2, _cultureFirstByteDataOffset + 3, 5);
            set => SetInt(2, _cultureFirstByteDataOffset + 3, 5, (int)value);
        }

        public KingdomId MapConnection1
        {
            get => (KingdomId)GetInt(2 + _cultureBinOffset, _cultureSecondByteOffset + 0, 5);
            set => SetInt(2 + _cultureBinOffset, _cultureSecondByteOffset + 0, 5, (int)value);
        }

        public KingdomId MapConnection2
        {
            get => (KingdomId)GetInt(2 + _cultureBinOffset, _cultureSecondByteOffset + 5, 5);
            set => SetInt(2 + _cultureBinOffset, _cultureSecondByteOffset + 5, 5, (int)value);
        }

        public KingdomId MapConnection3
        {
            get => (KingdomId)GetInt(2 + _cultureBinOffset, _cultureSecondByteOffset + 10, 5);
            set => SetInt(2 + _cultureBinOffset, _cultureSecondByteOffset + 10, 5, (int)value);
        }

        public KingdomId MapConnection4
        {
            get => (KingdomId)GetInt(3 + _cultureBinOffset, 0, 5);
            set => SetInt(3 + _cultureBinOffset, 0, 5, (int)value);
        }

        public KingdomId MapConnection5
        {
            get => (KingdomId)GetInt(3 + _cultureBinOffset, 5, 5);
            set => SetInt(3 + _cultureBinOffset, 5, 5, (int)value);
        }

        public KingdomId MapConnection6
        {
            get => (KingdomId)GetInt(3 + _cultureBinOffset, 10, 5);
            set => SetInt(3 + _cultureBinOffset, 10, 5, (int)value);
        }

        public KingdomId MapConnection7
        {
            get => (KingdomId)GetInt(3 + _cultureBinOffset, 15, 5);
            set => SetInt(3 + _cultureBinOffset, 15, 5, (int)value);
        }

        public KingdomId MapConnection8
        {
            get => (KingdomId)GetInt(3 + _cultureBinOffset, 20, 5);
            set => SetInt(3 + _cultureBinOffset, 20, 5, (int)value);
        }

        public KingdomId MapConnection9
        {
            get => (KingdomId)GetInt(3 + _cultureBinOffset, 25, 5);
            set => SetInt(3 + _cultureBinOffset, 25, 5, (int)value);
        }

        public KingdomId MapConnection10
        {
            get => (KingdomId)GetInt(4 + _cultureBinOffset, 0, 5);
            set => SetInt(4 + _cultureBinOffset, 0, 5, (int)value);
        }

        public KingdomId MapConnection11
        {
            get => (KingdomId)GetInt(4 + _cultureBinOffset, 5, 5);
            set => SetInt(4 + _cultureBinOffset, 5, 5, (int)value);
        }

        public KingdomId MapConnection12
        {
            get => (KingdomId)GetInt(4 + _cultureBinOffset, 10, 5);
            set => SetInt(4 + _cultureBinOffset, 10, 5, (int)value);
        }

        public KingdomId[] MapConnections
        {
            get => new[] { MapConnection0, MapConnection1, MapConnection2, MapConnection3, MapConnection4, MapConnection5, MapConnection6, MapConnection7, MapConnection8, MapConnection9, MapConnection10, MapConnection11, MapConnection12 };
            set
            {
                MapConnection0 = value[0];
                MapConnection1 = value[1];
                MapConnection2 = value[2];
                MapConnection3 = value[3];
                MapConnection4 = value[4];
                MapConnection5 = value[5];
                MapConnection6 = value[6];
                MapConnection7 = value[7];
                MapConnection8 = value[8];
                MapConnection9 = value[9];
                MapConnection10 = value[10];
                MapConnection11 = value[11];
                MapConnection12 = value[12];
            }
        }

        #endregion

        public BattleConfigId BattleConfig
        {
            get => (BattleConfigId)GetInt(4 + _cultureBinOffset, 15, 7);
            set => SetInt(4 + _cultureBinOffset, 15, 7, (int)value);
        }

        public int Unknown_R5_C22_L4
        {
            get => GetInt(4 + _cultureBinOffset, 22, 4);
            set => SetInt(4 + _cultureBinOffset, 22, 4, value);
        }


        public int Unknown_R5_C26_L4
        {
            get => GetInt(4 + _cultureBinOffset, 26, 4);
            set => SetInt(4 + _cultureBinOffset, 26, 4, value);
        }

    }
}