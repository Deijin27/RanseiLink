
namespace RanseiLink.Core.Maps;

public record MapId(int Map, int Variant)
{
    public override string ToString()
    {
        return $"map{Map.ToString().PadLeft(2, '0')}_{Variant.ToString().PadLeft(2, '0')}";
    }

    /// <summary>
    /// To file name used in game, i.e. ends in '.bin'
    /// </summary>
    /// <returns></returns>
    public string ToInternalFileName()
    {
        return $"{this}.bin";
    }

    /// <summary>
    /// To file name used for exporting, i.e. ends in '.pslm'
    /// </summary>
    /// <returns></returns>
    public string ToExternalFileName()
    {
        return $"{this}{PSLM.ExternalFileExtension}";
    }

    public static bool TryParseInternalFileName(string fileName, out MapId result)
    {
        result = null;

        if (fileName.Length != 12 || !fileName.StartsWith("map") || !fileName.EndsWith(".bin"))
        {
            return false;
        }

        string envString = fileName.Substring(3, 2);
        string variantString = fileName.Substring(6, 2);

        if (!int.TryParse(envString, out int env))
        {
            return false;
        }

        if (!int.TryParse(variantString, out int variant))
        {
            return false;
        }

        result =  new MapId(env, variant);
        return true;

    }

    public static explicit operator int(MapId value) => value.Map << 8 | value.Variant;
    public static explicit operator MapId(int value) => new MapId(value >> 8, value & 0xFF);
}
