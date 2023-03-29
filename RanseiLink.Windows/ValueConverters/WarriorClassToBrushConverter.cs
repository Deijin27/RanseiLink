using RanseiLink.Core.Enums;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace RanseiLink.Windows.ValueConverters;
public class WarriorClassToBrushConverter : ValueConverter<WarriorClassId, Brush>
{
    private static readonly Dictionary<WarriorClassId, Brush> _dict = new()
    {
        [WarriorClassId.ArmyLeader] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d6c678")),
        [WarriorClassId.ArmyMember] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a37fc9")),
        [WarriorClassId.FreeWarrior_1] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#80c468")),
        [WarriorClassId.FreeWarrior_2] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cf4c4c")),
        [WarriorClassId.FreeWarrior_3] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#558cf2")),
        [WarriorClassId.Default] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c7c7c7")),
    };

    protected override Brush Convert(WarriorClassId value)
    {
        return _dict[value];
    }

    protected override WarriorClassId ConvertBack(Brush value)
    {
        throw new NotImplementedException();
    }

    public static WarriorClassToBrushConverter Instance { get; } = new WarriorClassToBrushConverter();
}
