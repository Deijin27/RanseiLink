using System.Reflection;

namespace RanseiLink.Core.Resources;

public static class CharacterTableResource
{
    static CharacterTableResource()
    {
        // Initialize Tables
        LoadTable = [];
        SaveTable = [];
        using Stream stream = ResourceUtil.GetResourceStream(__tablePath);
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

    const string __tablePath = "CharacterTable.tbl";

    public static Dictionary<string, string> LoadTable { get; private set; }
    public static Dictionary<string, string> SaveTable { get; private set; }
}