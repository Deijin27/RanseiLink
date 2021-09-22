using CliFx.Infrastructure;
using System;

namespace RanseiConsole
{
    internal static partial class RenderExtensions
    {
        private static void WriteTitle(this IConsole console, string title)
        {
            using (console.WithForegroundColor(ConsoleColor.Cyan))
            {
                console.Output.WriteLine(title);
            }
        }

        private static void WriteProperty(this IConsole console, string propertyName, string propertyValue)
        {
            using (console.WithForegroundColor(ConsoleColor.White))
            {
                console.Output.Write($"    {propertyName}: ");
            }
            using (console.WithForegroundColor(ConsoleColor.Gray))
            {
                console.Output.WriteLine(propertyValue);
            }
        }
    }
}
