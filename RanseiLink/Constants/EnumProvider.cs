using RanseiLink.Core;
using RanseiLink.Core.Enums;
using System.Linq;

namespace RanseiLink.Constants;

public static class EnumProvider
{
    public static AbilityEffectId[] AbilityEffectItems { get; } = EnumUtil.GetValues<AbilityEffectId>().ToArray();
    public static WarriorSpriteId[] WarriorSpriteItems { get; } = EnumUtil.GetValues<WarriorSpriteId>().ToArray();
    public static WarriorSprite2Id[] WarriorSpriteUnknownItems { get; } = EnumUtil.GetValues<WarriorSprite2Id>().ToArray();
    public static GenderId[] GenderItems { get; } = EnumUtil.GetValues<GenderId>().ToArray();
    public static TypeId[] TypeItems { get; } = EnumUtil.GetValues<TypeId>().ToArray();
    public static WarriorSkillId[] WarriorSkillItems { get; } = EnumUtil.GetValues<WarriorSkillId>().ToArray();
    public static WarriorId[] WarriorItems { get; } = EnumUtil.GetValues<WarriorId>().ToArray();
    public static PokemonId[] PokemonItems { get; } = EnumUtil.GetValues<PokemonId>().ToArray();
    public static RankUpConditionId[] RankUpConditionItems { get; } = EnumUtil.GetValues<RankUpConditionId>().ToArray();
    public static MoveRangeId[] MoveRangeItems { get; } = EnumUtil.GetValues<MoveRangeId>().ToArray();
    public static MoveEffectId[] MoveEffectItems { get; } = EnumUtil.GetValues<MoveEffectId>().ToArray();
    public static MoveAnimationId[] MoveAnimationItems { get; } = EnumUtil.GetValues<MoveAnimationId>().ToArray();
    public static MoveMovementAnimationId[] MoveMovementAnimationItems { get; } = EnumUtil.GetValues<MoveMovementAnimationId>().ToArray();
    public static KingdomId[] KingdomItems { get; } = EnumUtil.GetValues<KingdomId>().ToArray();
    public static EvolutionConditionId[] EvolutionConditionItems { get; } = EnumUtil.GetValues<EvolutionConditionId>().ToArray();
    public static MoveId[] MoveItems { get; } = EnumUtil.GetValues<MoveId>().ToArray();
    public static AbilityId[] AbilityItems { get; } = EnumUtil.GetValues<AbilityId>().ToArray();
    public static ScenarioId[] ScenarioItems { get; } = EnumUtil.GetValues<ScenarioId>().ToArray();
    public static WarriorClassId[] WarriorClassItems { get; } = EnumUtil.GetValues<WarriorClassId>().ToArray();
    public static WarriorSkillTargetId[] WarriorSkillTargetItems { get; } = EnumUtil.GetValues<WarriorSkillTargetId>().ToArray();
    public static WarriorSkillEffectId[] WarriorSkillEffectItems { get; } = EnumUtil.GetValues<WarriorSkillEffectId>().ToArray();
}
