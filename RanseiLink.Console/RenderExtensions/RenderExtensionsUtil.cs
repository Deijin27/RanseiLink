﻿#nullable enable
namespace RanseiLink.Console;

public static partial class RenderExtensions
{
    private static void WriteTitle(this IConsole console, object title)
    {
        using (console.WithForegroundColor(ConsoleColor.Cyan))
        {
            console.WriteLine(title);
        }
    }

    private static void WriteProperty(this IConsole console, object propertyName, object? propertyValue)
    {
        using (console.WithForegroundColor(ConsoleColor.White))
        {
            console.Write($"    {propertyName}: ");
        }
        using (console.WithForegroundColor(ConsoleColor.Gray))
        {
            console.WriteLine(propertyValue);
        }
    }
}
