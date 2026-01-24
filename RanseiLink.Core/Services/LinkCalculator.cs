namespace RanseiLink.Core.Services;

public static class LinkCalculator
{
    public static int CalculateLink(int exp)
    {
        int expTemp;
        var link = 0;
        do
        {
            expTemp = CalculateExp(link + 1);
            if (exp < expTemp)
            {
                return link;
            }
            link++;
        } while (link < 100);
        return link;
    }

    public static int CalculateExp(int link)
    {
        var tmp = link * (link * (link * 2 + 9) + 0x3aa5);
        return tmp / 300;
    }
}
