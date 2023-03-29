using System;
using System.Linq;

namespace RanseiLink.Windows.ValueConverters;

internal class BoolAggregateAndValueConverter : MultiValueConverter<bool, bool>
{
    protected override bool Convert(bool[] values)
    {
        return values.Aggregate((a, b) => a && b);
    }

    protected override bool[] ConvertBack(bool value)
    {
        throw new NotImplementedException();
    }
}
