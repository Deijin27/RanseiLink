using Core.Enums;
using System;

namespace RanseiWpf.ValueConverters
{
    public class EvolutionConditionIdToQuantityNameConverter : ValueConverter<EvolutionConditionId, string>
    {
        protected override string Convert(EvolutionConditionId value)
        {
            return value switch
            {
                EvolutionConditionId.Hp => "Required HP",
                EvolutionConditionId.Attack => "Required Atk",
                EvolutionConditionId.Defence => "Required Def",
                EvolutionConditionId.Speed => "Required Spe",
                EvolutionConditionId.Link => "Required Link",
                EvolutionConditionId.Location => "Required Location",
                EvolutionConditionId.WarriorGender => "Required Gender",
                EvolutionConditionId.Item => "Required Item",
                EvolutionConditionId.JoinOffer => "N/A",
                EvolutionConditionId.NoCondition => "N/A",
                _ => throw new Exception($"Invalid Enum Value of {value} in {nameof(Convert)} of {nameof(EvolutionConditionIdToQuantityNameConverter)}"),
            };
        }

        protected override EvolutionConditionId ConvertBack(string value)
        {
            throw new NotImplementedException();
        }
    }
}
