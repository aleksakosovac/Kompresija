using System;

namespace Projekat1
{
    static class CanonicalCode
    {
        public static (uint code, int length)[] BuildCodes(int[] lengths)
        {
            var result = new (uint code, int length)[256];

            int maxLen = 0;
            for (int i = 0; i < 256; i++)
            {
                if (lengths[i] > maxLen)
                {
                    maxLen = lengths[i];
                }
            }

            if (maxLen == 0)
            {
                return result;
            }

            int[] blCount = new int[maxLen + 1];
            for (int i = 0; i < 256; i++)
            {
                if (lengths[i] > 0)
                {
                    blCount[lengths[i]]++;
                }
            }

            int[] firstCode = new int[maxLen + 1];
            int code = 0;
            blCount[0] = 0;
            for (int len = 1; len <= maxLen; len++)
            {
                code = (code + blCount[len - 1]) << 1;
                firstCode[len] = code;
            }

            int[] nextCode = (int[])firstCode.Clone();

            for (int sym = 0; sym < 256; sym++)
            {
                int len = lengths[sym];
                if (len > 0)
                {
                    result[sym] = ((uint)nextCode[len], len);
                    nextCode[len]++;
                }
            }

            return result;
        }
    }
}