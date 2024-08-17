
namespace RanseiLink.Core.Enums;

public enum WarriorSkillEffectId
{
    RaiseAttack,
    RaiseDefence,
    RaiseSpeed,
    RaiseAccuracy,
    RaiseCritChance,
    RaiseRange,
    LowerDefence,
    RestoreHp,
    SleepAllies,
    CureStatus,
    CureParalysis,
    CureSleep,
    CurePoison,
    CureBurn,
    CureFreeze,
    CureConfusion,
    CancelOpponentWarriorSkill,
    RestoreHpOverTime,
    CantMove,
    ClimbHigher,
    GuaranteeHit,
    Dodge,
    LowerHpToOne,
    BlockCrits,
    IncreaseStatusChance,
    BlockStatus,
    ChanceToFlinchOpponent, // probably 100 for most
    WeakenOpponentAttackIfMale, // the if male spec is potentially stored in the quantity
    IncreaseTension,
    AdditionalMoves,
    TwoChests,
    ConfuseOpponent,


    NoEffect = 64
}