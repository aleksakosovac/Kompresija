namespace Projekat2
{
    static class GF2
    {
        public static int[] ComputeColumnMasks(int[,] H, int m, int n)
        {
            int[] colMasks = new int[n];
            for (int col = 0; col < n; col++)
            {
                int mask = 0;
                for (int row = 0; row < m; row++)
                {
                    if (H[row, col] == 1)
                    {
                        mask |= (1 << row);
                    }
                }
                colMasks[col] = mask;
            }
            return colMasks;
        }

        public static int Syndrome(int[] colMasks, int n, int errorMask)
        {
            int syndrome = 0;
            for (int col = 0; col < n; col++)
            {
                if (((errorMask >> col) & 1) == 1)
                {
                    syndrome ^= colMasks[col];
                }
            }
            return syndrome;
        }

        public static int PopCount(int mask)
        {
            int count = 0;
            while (mask != 0)
            {
                count += mask & 1;
                mask >>= 1;
            }
            return count;
        }
    }
}