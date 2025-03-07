namespace RanseiLink.Core.Services;

public static class MiscUtil
{
    /// <summary>
    /// Estimated formula. It works very accurately, very high BSTs it starts to be inaccurate
    /// </summary>
    public static int CalculateStrength(int hp, int atk, int def, int spe, double link)
    {
        // the game considers the HP to have more weight on
        // the strength of a pokemon
        double bst = atk + def + spe + 1.666666666 * hp;
        int result = (int)((bst * 0.006 + 0.842) * link);
        // there seems to be a consistent +2 if you are at 100% link
        if (link >= 100)
        {
            result += 2;
        }
        if (result >= 1000)
        {
            // if the strength goes over 1000 in game,
            // it goes into the negatives for some reason
            result = -(result - 1000);
        }
        return result;
    }

    public static int PowerToStarCount(int power)
    {
        return power switch
        {
            > 50 => 5,
            > 40 => 4,
            > 30 => 3,
            > 20 => 2,
            > 0 => 1,
            _ => 0
        };
    }
}