﻿using System.Reflection;

namespace RanseiLink.Core.Resources;

public static class CharacterTableResource
{
    static CharacterTableResource()
    {
        // Initialize Tables
        LoadTable = new Dictionary<string, string>();
        SaveTable = new Dictionary<string, string>();
        Stream? stream = null;
        try
        {
            stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(_tablePath);
            var reader = new StreamReader(stream!);

            while (!reader.EndOfStream)
            {
                string? line = reader.ReadLine();
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

    const string _tablePath = "RanseiLink.Core.Resources.CharacterTable.tbl";

    public static Dictionary<string, string> LoadTable { get; private set; }
    public static Dictionary<string, string> SaveTable { get; private set; }
}