using RanseiLink.Core.Enums;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace RanseiLink.ValueConverters;
public class WarriorClassToBrushConverter : ValueConverter<WarriorClassId, Brush>
{
    private static readonly Dictionary<WarriorClassId, Brush> _dict = new()
    {
        [WarriorClassId.ArmyLeader] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B2A771")),
        [WarriorClassId.ArmyMember] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8970A3")),
        [WarriorClassId.FreeWarrior] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7DA370")),
        [WarriorClassId.Unknown_3] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A37070")),
        [WarriorClassId.Unknown_4] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7082A3")),
        [WarriorClassId.Default] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A1A1A1")),
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
