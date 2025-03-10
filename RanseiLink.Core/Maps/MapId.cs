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

    public static explicit operator int(MapId value) => value.Map * 100 + value.Variant;
    public static explicit operator MapId(int value) => new MapId(value / 100, value % 100);

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

    public static Dictionary<int, string> DefaultNicknames { get; } = new Dictionary<int, string>
    {
        [0000] = "Aurora",
        [0001] = "Unused",
        [0100] = "Ignis",
        [0101] = "Unused",
        [0200] = "Fontaine",
        [0201] = "Unused",
        [0300] = "Violight",
        [0301] = "Unused",
        [0400] = "Greenleaf",
        [0401] = "Unused",
        [0500] = "Nixtorm",
        [0501] = "Unused",
        [0600] = "Pugilis",
        [0601] = "Unused",
        [0700] = "Viperia",
        [0701] = "Unused",
        [0800] = "Terrera",
        [0801] = "Unused",
        [0900] = "Avia",
        [0901] = "Unused",
        [1000] = "Illusio",
        [1001] = "Unused",
        [1100] = "Chrysalia",
        [1101] = "Unused",
        [1200] = "Cragspur",
        [1201] = "Unused",
        [1300] = "Spectra",
        [1301] = "Unused",
        [1400] = "Dragnor",
        [1401] = "Unused",
        [1500] = "Yakasha",
        [1501] = "Unused",
        [1600] = "Valora",
        [1601] = "Unused",
        [3000] = "InfiniteTower",
        [3001] = "TestMap_1",
        [3002] = "TestMap_2",
        [3100] = "SacredRuins",
        [3101] = "TestMap_3",
        [3102] = "TestMap_4",
        [3103] = "Unused",
        [3104] = "Unused",
        [3200] = "Ravine_3",
        [3201] = "Ravine_2",
        [3202] = "Ravine_1",
        [3203] = "Unused",
        [3204] = "Unused",
        [3300] = "Cave_3",
        [3301] = "Cave_2",
        [3302] = "Cave_1",
        [3303] = "Unused",
        [3304] = "Unused",
        [3400] = "FloatingRock_3",
        [3401] = "FloatingRock_2",
        [3402] = "FloatingRock_1",
        [3403] = "Unused",
        [3404] = "Unused",
        [3500] = "Farm_3",
        [3501] = "Farm_2",
        [3502] = "Farm_1",
        [3503] = "Unused",
        [3504] = "Unused",
        [3600] = "SkyGarden_3",
        [3601] = "SkyGarden_2",
        [3602] = "SkyGarden_1",
        [3603] = "Unused",
        [3604] = "Unused",
        [3700] = "SnowyMountain_3",
        [3701] = "SnowyMountain_2",
        [3702] = "SnowyMountain_1",
        [3703] = "Unused",
        [3704] = "Unused",
        [3800] = "Park_3",
        [3801] = "Park_2",
        [3802] = "Park_1",
        [3803] = "Unused",
        [3804] = "Unused",
        [3900] = "TestMap_5",
        [3901] = "TestMap_6",
        [3902] = "TestMap_7",
        [3903] = "Unused",
        [3904] = "Unused",
    };
}