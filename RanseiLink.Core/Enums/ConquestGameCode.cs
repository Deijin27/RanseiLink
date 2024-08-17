namespace RanseiLink.Core.Enums;

public enum ConquestGameCode
{
    VPYT,
    VPYJ,
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
}
