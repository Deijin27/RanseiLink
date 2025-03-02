
namespace RanseiLink.GuiCore.Services;


public record AppVersion(int Major, int Minor, int Patch, AppReleaseType Type, int PrereleaseVersion)
{
    public override string ToString()
    {
        var str = $"{Major}.{Minor}.{Patch}";

        if (Type != AppReleaseType.Stable)
        {
            if (Type == AppReleaseType.ReleaseCandidate)
            {
                str += "-rc";
            }
            else if (Type == AppReleaseType.Alpha)
            {
                str += "-alpha";
            }
            else if (Type == AppReleaseType.Beta)
            {
                str += "-beta";
            }
            if (PrereleaseVersion > 0)
            {
                str += PrereleaseVersion;
            }
        }
        return str;
    }

    public bool IsPreRelease => Type != AppReleaseType.Stable;

    public bool IsNewerThan(AppVersion other)
    {
        if (Major > other.Major)
        {
            return true;
        }
        if (Major < other.Major)
        {
            return false;
        }
        // major is the same
        if (Minor > other.Minor)
        {
            return true;
        }
        if (Minor < other.Minor)
        {
            return false;
        }
        // minor is the same
        if (Patch > other.Patch)
        {
            return true;
        }
        if (Patch < other.Patch)
        {
            return false;
        }
        // patch is the same
        if (Type > other.Type)
        {
            return true;
        }
        if (Type < other.Type)
        {
            return false;
        }
        // type is the same
        return PrereleaseVersion > other.PrereleaseVersion;
    }

    public static AppVersion? Parse(string? str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }
        if (str.StartsWith("v"))
        {
            str = str.Substring(1);
        }
        var type = AppReleaseType.Stable;
        int typeVersion = 0;
        var dash = str.IndexOf('-');
        if (dash != -1)
        {
            var tstr = str.Substring(dash + 1);
            str = str.Substring(0, dash);

            if (tstr.StartsWith("alpha"))
            {
                type = AppReleaseType.Alpha;
                tstr = tstr.Substring("alpha".Length);
            }
            else if (tstr.StartsWith("beta"))
            {
                type = AppReleaseType.Beta;
                tstr = tstr.Substring("beta".Length);
            }
            else if (tstr.StartsWith("rc"))
            {
                type = AppReleaseType.ReleaseCandidate;
                tstr = tstr.Substring("rc".Length);
            }
            if (int.TryParse(tstr, out var t))
            {
                typeVersion = t;
            }
        }
        var parts = str.Split('.');
        if (parts.Length < 2)
        {
            return null;
        }
        if (!int.TryParse(parts[0], out var major))
        {
            return null;
        }
        if (!int.TryParse(parts[1], out var minor))
        {
            return null;
        }
        if (parts.Length == 2)
        {
            return new AppVersion(major, minor, 0, type, typeVersion);
        }
        if (!int.TryParse(parts[2], out var patch))
        {
            return null;
        }
        return new AppVersion(major, minor, patch, type, typeVersion);
    }
}