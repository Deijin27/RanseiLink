using RanseiLink.Core.Resources;

namespace RanseiLink.Core.Text;

public static class NameLoader
{
    public static string LoadName(byte[] nameData)
    {
        var result = EncodingProvider.ShiftJIS.GetString(nameData);
        foreach (var (key, value) in CharacterTableResource.LoadTable)
        {
            result = result.Replace(key, value);
        }
        return result;
    }

    public static byte[] SaveName(string name)
    {
        foreach (var (key, value) in CharacterTableResource.SaveTable)
        {
            name = name.Replace(key, value);
        }
        return EncodingProvider.ShiftJIS.GetBytes(name);
    }
}
