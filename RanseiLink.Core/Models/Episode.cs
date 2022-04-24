using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models
{
    /// <summary>
    /// Tokusei
    /// </summary>
    public class Episode : BaseDataWindow
    {
        public const int DataLength = 0x8;
        public Episode(byte[] data) : base(data, DataLength) { }
        public Episode() : this(new byte[DataLength]) { }

        public int Order
        {
            get => GetInt(0, 0, 9);
            set => SetInt(0, 0, 9, value);
        }

        public ScenarioId Scenario
        {
            get => (ScenarioId)GetInt(0, 9, 4);
            set => SetInt(0, 9, 4, (int)value);
        }

        public EpisodeId UnlockCondition
        {
            get => (EpisodeId)GetInt(1, 0, 6);
            set => SetInt(1, 0, 6, (int)value);
        }

        public int Difficulty
        {
            get => GetInt(1, 6, 3);
            set => SetInt(1, 6, 3, value);
        }

        public EpisodeClearConditionId ClearCondition 
        {
            get => (EpisodeClearConditionId)GetInt(1, 26, 4);
            set => SetInt(1, 26, 4, (int)value);
        }

        public bool IsStartKingdom(KingdomId kingdomId)
        {
            return GetInt(0, 13 + (int)kingdomId, 1) == 1;
        }

        public void SetIsStartKingdom(KingdomId kingdomId, bool isStartKingdom)
        {
            SetInt(0, 13 + (int)kingdomId, 1, isStartKingdom ? 1 : 0);
        }
    }
}