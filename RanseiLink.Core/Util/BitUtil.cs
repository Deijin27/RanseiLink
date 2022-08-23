namespace RanseiLink.Core.Util
{
    public static class BitUtil
    {
        /// <summary>
        /// value &lt;&lt; shift = result
        /// </summary>
        public static bool TryReverseLogicalShiftLeft(int value, int result, out int shift)
        {
            for (shift = 0; shift < 31; shift++)
            {
                if (value << shift == result)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
