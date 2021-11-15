using RanseiLink.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RanseiLink.Console.Dev;

static class Testing
{
    public static string GetBits(byte[] data)
    {
        string bits = "";
        foreach (var b in data)
        {
            bits = Convert.ToString(b, 2).PadLeft(8, '0') + bits;
        }
        return bits;
    }

    public static void LogDataGroupings<T>(string folderToPutLogsInto, IEnumerable<T> dataItems, Func<T, int, string> nameSelector) where T : BaseDataWindow
    {
        Directory.CreateDirectory(folderToPutLogsInto);
        var opk = dataItems.Select((a, b) => (a, b)).OrderBy(i => nameSelector(i.a, 0)).ToArray();
        var dataLength = opk[0].a.Data.Length;

        for (int i = 0; i < dataLength; i++)
        {
            using var sw = new StreamWriter(File.Create(Path.Combine(folderToPutLogsInto, i.ToString().PadLeft(2, '0') + $" - 0x{i:x}.txt")));
            var gpk = opk.GroupBy(p => p.a.Data[i]).OrderBy(g => g.Key);

            foreach (var group in gpk)
            {
                sw.WriteLine($"{group.Key} = 0x{group.Key:x} = 0b{Convert.ToString(group.Key, 2).PadLeft(8, '0')} ---------------------------------------");
                sw.WriteLine();

                foreach (var pk in group)
                {
                    sw.WriteLine(nameSelector(pk.a, pk.b));
                }

                sw.WriteLine();
            }
        }
    }
}
