
namespace RanseiLink.Core.Services;

public record BattleEnvironment(uint Environment, uint Variant)
{
    public override string ToString()
    {
        return $"map{Environment.ToString().PadLeft(2, '0')}_{Variant.ToString().PadLeft(2, '0')}.bin";
    }

    public static bool TryParse(string fileName, out BattleEnvironment result)
    {
        result = null;

        if (fileName.Length != 12 || !fileName.StartsWith("map") || !fileName.EndsWith(".bin"))
        {
            return false;
        }

        string envString = fileName.Substring(3, 2);
        string variantString = fileName.Substring(6, 2);

        if (!uint.TryParse(envString, out uint env))
        {
            return false;
        }

        if (!uint.TryParse(variantString, out uint variant))
        {
            return false;
        }

        result =  new BattleEnvironment(env, variant);
        return true;

    }
}
