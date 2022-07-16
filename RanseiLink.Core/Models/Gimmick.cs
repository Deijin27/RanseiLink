using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models
{
    public class Gimmick : BaseDataWindow
    {
        public static int DataLength(ConquestGameCode culture) => culture == ConquestGameCode.VPYJ ? 0x24 : 0x28;

        private readonly int _cultureNameLength;
        private readonly int _cultureBinOffset;

        public Gimmick(byte[] data, ConquestGameCode culture = ConquestGameCode.VPYT) : base(data, DataLength(culture)) 
        {
            _cultureNameLength = culture == ConquestGameCode.VPYJ ? 0xE : 0x10;
            _cultureBinOffset = culture == ConquestGameCode.VPYJ ? 0 : 1;
        }
        public Gimmick(ConquestGameCode culture = ConquestGameCode.VPYT) : this(new byte[DataLength(culture)], culture) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, _cultureNameLength);
            set => SetPaddedUtf8String(0, _cultureNameLength, value);
        }

        /// <summary>
        /// Top screen square image when you hover over the object
        /// </summary>
        public int Image
        {
            get => GetByte(_cultureNameLength + 1);
            set => SetByte(_cultureNameLength + 1, (byte)value);
        }

        /// <summary>
        /// If attack damage caused by this, this is the type of the attack
        /// </summary>
        public TypeId AttackType
        {
            get => (TypeId)GetInt(4 + _cultureBinOffset, 0, 5);
            set => SetInt(4 + _cultureBinOffset, 0, 5, (int)value);
        }

        /// <summary>
        /// The type of attack that can destroy this gimmick
        /// </summary>
        public TypeId DestroyType
        {
            get => (TypeId)GetInt(4 + _cultureBinOffset, 5, 5);
            set => SetInt(4 + _cultureBinOffset, 5, 5, (int)value);
        }

        /// <summary>
        /// Sprite shown on bottom screen in battle
        /// </summary>
        public GimmickObjectId State1Object
        {
            get => (GimmickObjectId)GetInt(4 + _cultureBinOffset, 11, 7);
            set => SetInt(4 + _cultureBinOffset, 11, 7, (int)value);
        }

        public GimmickObjectId State2Object
        {
            get => (GimmickObjectId)GetInt(4 + _cultureBinOffset, 18, 7);
            set => SetInt(4 + _cultureBinOffset, 18, 7, (int)value);
        }

        public MoveEffectId Effect
        {
            get => (MoveEffectId)GetInt(4 + _cultureBinOffset, 25, 7);
            set => SetInt(4 + _cultureBinOffset, 25, 7, (int)value);
        }

        /// <summary>
        /// Seems like a multipurpose quantity. For some, this is probably attack power, others something else
        /// </summary>
        public int UnknownQuantity1
        {
            get => GetInt(5 + _cultureBinOffset, 0, 8);
            set => SetInt(5 + _cultureBinOffset, 0, 8, value);
        }

        public MoveAnimationId Animation1
        {
            get => (MoveAnimationId)GetInt(5 + _cultureBinOffset, 8, 8);
            set => SetInt(5 + _cultureBinOffset, 8, 8, (int)value);
        }

        public MoveAnimationId Animation2
        {
            get => (MoveAnimationId)GetInt(5 + _cultureBinOffset, 16, 8);
            set => SetInt(5 + _cultureBinOffset, 16, 8, (int)value);
        }

        public GimmickRangeId Range
        {
            get => (GimmickRangeId)GetInt(7 + _cultureBinOffset, 19, 5);
            set => SetInt(7 + _cultureBinOffset, 19, 5, (int)value);
        }

    }
}