using RanseiLink.Core.Resources;

namespace RanseiLink.Core.Text;

public static class NameLoader
{
    public static string LoadName(byte[] nameData)
    {
        var result = EncodingProvider.ShiftJIS.GetString(nameData);
        foreach (var kvp in CharacterTableResource.LoadTable)
        {
            result = result.Replace(kvp.Key, kvp.Value);
        }
        return result;
    }

    public static byte[] SaveName(string name)
    {
        foreach (var kvp in CharacterTableResource.SaveTable)
        {
            name = name.Replace(kvp.Key, kvp.Value);
        }
        return EncodingProvider.ShiftJIS.GetBytes(name);
    }
}