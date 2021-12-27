using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RanseiLink.Core.Text;

public static class NameLoader
{
    public static string LoadName(byte[] nameData)
    {
        var result = EncodingProvider.ShiftJIS.GetString(nameData);
        foreach (var (key, value) in LoadTable)
        {
            result = result.Replace(key, value);
        }
        return result;
    }

    public static byte[] SaveName(string name)
    {
        foreach (var (key, value) in SaveTable)
        {
            name = name.Replace(key, value);
        }
        return EncodingProvider.ShiftJIS.GetBytes(name);
    }

    const string _tablePath = "RanseiLink.Core.Text.CharacterTable.tbl";

    static NameLoader()
    {
       InitializeTables();
    }

    private static void InitializeTables()
    {
        LoadTable = new();
        SaveTable = new();
        Stream stream = null;
        try
        {
            stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(_tablePath);
            var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                if (line[0] == '#')
                    continue;

                string[] fields = line.Split('=');
                LoadTable.Add(fields[0], fields[1]);
                SaveTable.Add(fields[1], fields[0]);
            }
        }
        finally
        {
            stream?.Dispose();
        }
    }

    public static Dictionary<string, string> LoadTable { get; private set; }
    public static Dictionary<string, string> SaveTable { get; private set; }

}
