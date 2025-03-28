namespace RanseiLink.Core.Enums;

public enum ConquestGameCode
{
    /// <summary>
    /// North America
    /// </summary>
    VPYT,
    /// <summary>
    /// Japan
    /// </summary>
    VPYJ,
    /// <summary>
    /// Europe
    /// </summary>
    VPYP,
}

public static class ConquestGameCodeExtensions
{
    public static bool IsCompatibleWith(this ConquestGameCode gameCode1, ConquestGameCode gameCode2)
    {
        return gameCode1.CompatibilitySet() == gameCode2.CompatibilitySet();
    }

    public static ConquestGameCode CompatibilitySet(this ConquestGameCode gameCode)
    {
        switch (gameCode)
        {
            case ConquestGameCode.VPYT: return ConquestGameCode.VPYT;
            case ConquestGameCode.VPYJ: return ConquestGameCode.VPYJ;
            case ConquestGameCode.VPYP: return ConquestGameCode.VPYT;
            default: throw new ArgumentException(nameof(gameCode));
        }
    }

    public static string UserFriendlyName(this  ConquestGameCode gameCode)
    {
        return gameCode switch
        {
            ConquestGameCode.VPYT => "North America",
            ConquestGameCode.VPYJ => "Japan",
            ConquestGameCode.VPYP => "Europe",
            _ => throw new ArgumentException(nameof(gameCode)),
        };
    }
}
