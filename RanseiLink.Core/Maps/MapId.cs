using RanseiLink.Core.Archive;

namespace RanseiLink.Core.Maps;

public readonly struct MapId : IEquatable<MapId>
{
    public int Map { get; }
    public int Variant { get; }
    public MapId(int map, int variant)
    {
        Map = map;
        Variant = variant;
    }
    public override string ToString()
    {
        return $"map{Map:00}_{Variant:00}";
    }

    /// <summary>
    /// To file name used in game for PSLM, i.e. 'map00_00.bin'
    /// </summary>
    public string ToInternalPslmName()
    {
        return $"{this}.bin";
    }

    /// <summary>
    /// To file name used for exporting PSLM, i.e. 'map00_00.pslm'
    /// </summary>
    public string ToExternalPslmName()
    {
        return $"{this}{PSLM.ExternalFileExtension}";
    }

    /// <summary>
    /// The file name used in game for 3D models, i.e. 'MAP00_00.pac'
    /// </summary>
    public string ToInternalModelPacName()
    {
        return $"MAP{Map:00}_{Variant:00}{PAC.FileExtension}";
    }

    public static bool TryParseInternalFileName(string fileName, out MapId result)
    {
        result = default;

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

        result = new MapId(env, variant);
        return true;

    }

    public static explicit operator int(MapId value) => value.Map << 8 | value.Variant;
    public static explicit operator MapId(int value) => new MapId(value >> 8, value & 0xFF);

    public static bool operator ==(MapId obj1, MapId obj2)
    {
        return obj1.Equals(obj2);
    }
    public static bool operator !=(MapId obj1, MapId obj2)
    {
        return !obj1.Equals(obj2);
    }

    public override bool Equals(object? obj)
    {
        return obj is MapId other && Equals(other);
    }

    public bool Equals(MapId other)
    {
        return other.Map == Map && other.Variant == Variant;
    }

    public override int GetHashCode()
    {
        return (int)this;
    }
}