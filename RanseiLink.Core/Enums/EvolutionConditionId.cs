
namespace RanseiLink.Core.Enums;


public enum EvolutionConditionId
{
    /// <summary>
    /// Perform any action, with at least ___ HP afterwards
    /// </summary>
    Hp,

    /// <summary>
    /// Perform any action, with at least ___ Attack afterwards
    /// </summary>
    Attack,

    /// <summary>
    /// Perform any action, with at least ___ Defence afterwards
    /// </summary>
    Defence,

    /// <summary>
    /// Perform any action, with at least ___ Speed afterwards
    /// </summary>
    Speed,

    /// <summary>
    /// Perform any action, with at least ___% link afterwards
    /// </summary>
    Link,

    /// <summary>
    /// Perform any action at specific kingdom
    /// </summary>
    Kingdom,

    /// <summary>
    /// Win a battle with a warrior of a specific gender
    /// </summary>
    WarriorGender,

    /// <summary>
    /// With ___ item equipt
    /// </summary>
    Item,

    /// <summary>
    /// Score a KO that makes a warrior offer to join your army
    /// </summary>
    JoinOffer,

    /// <summary>
    /// Default placeholder
    /// </summary>
    NoCondition,
}