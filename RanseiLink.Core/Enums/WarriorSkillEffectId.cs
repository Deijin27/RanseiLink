
namespace RanseiLink.Core.Enums;

public enum WarriorSkillEffectId : uint
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
    CureParalysis, // Maybe swap with poison
    CureSleep, // Maybe swap with confusion
    CurePoison, // Maybe swap with paralysis
    CureBurn, // Maybe swap with freeze
    CureFreeze, // Maybe swap with burn
    CureConfusion, // Maybe swap with sleep
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
