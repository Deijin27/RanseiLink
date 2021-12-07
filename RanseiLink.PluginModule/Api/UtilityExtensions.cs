using System;
using System.Collections.Generic;

namespace RanseiLink.PluginModule.Api;

public static class UtilityExtensions
{
    public static T Choice<T>(this Random random, T[] collection)
    {
        return collection[random.Next(0, collection.Length)];
    }
    public static T Choice<T>(this Random random, IList<T> collection)
    {
        return collection[random.Next(0, collection.Count)];
    }
}
