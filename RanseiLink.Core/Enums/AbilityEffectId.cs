
namespace RanseiLink.Core.Enums;

public enum AbilityEffectId : uint
{
    Unused_1,
    IncreaseUserAttack,
    IncreaseUserDefence,
    IncreaseUserMovementRange,
    Unknown_1, // Raises user speed (maybe after flinching)
    IncreaseUserAccuracy,
    Unused_2,
    DecreaseOpponentAttack,
    DecreaseOpponentDefence,
    DecreaseOpponentMovementRange,
    Unknown_3,
    DecreaseOpponentAccuracy,


    NoEffect = 20, // why is aquaboost in this group???

}
