using System;
using System.Collections.Generic;

namespace Projekat2
{
    static class LdpcMatrixBuilder
    {
        public static int[,] Build(int n, int m, int wr, int wc, int seed, int maxAttempts = 100000)
        {
            if (n % wr != 0)
            {
                throw new ArgumentException("n mora biti deljivo sa wr.");
            }

            int rowsPerGroup = n / wr;

            if (m % wc != 0)
            {
                throw new ArgumentException("n-k mora biti deljivo sa wc.");
            }

            if (m / wc != rowsPerGroup)
            {
                throw new ArgumentException("Parametri nisu konzistentni: (n-k)/wc mora biti jednako n/wr.");
            }

            // Prva grupa vrsta (deterministicka) - ne menja se izmedju pokusaja
            int[,] firstGroup = new int[rowsPerGroup, n];
            for (int i = 0; i < rowsPerGroup; i++)
            {
                for (int col = i * wr; col < (i + 1) * wr; col++)
                {
                    firstGroup[i, col] = 1;
                }
            }

            Random rnd = new Random(seed);

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                int[,] H = new int[m, n];
                for (int i = 0; i < rowsPerGroup; i++)
                {
                    for (int col = 0; col < n; col++)
                    {
                        H[i, col] = firstGroup[i, col];
                    }
                }

                for (int group = 1; group < wc; group++)
                {
                    int[] perm = RandomPermutation(n, rnd);

                    for (int i = 0; i < rowsPerGroup; i++)
                    {
                        int destRow = group * rowsPerGroup + i;
                        for (int col = 0; col < n; col++)
                        {
                            H[destRow, col] = firstGroup[i, perm[col]];
                        }
                    }
                }

                if (!HasDuplicateColumns(H, m, n))
                {
                    return H;
                }
            }

            throw new InvalidOperationException($"Nije pronadjena H matrica bez duplikata kolona za {maxAttempts} pokusaja.");
        }

        private static bool HasDuplicateColumns(int[,] H, int m, int n)
        {
            var seenColumns = new HashSet<string>();

            for (int col = 0; col < n; col++)
            {
                var chars = new char[m];
                for (int row = 0; row < m; row++)
                {
                    chars[row] = H[row, col] == 1 ? '1' : '0';
                }
                string key = new string(chars);

                if (!seenColumns.Add(key))
                {
                    return true;
                }
            }

            return false;
        }

        private static int[] RandomPermutation(int n, Random rnd)
        {
            int[] perm = new int[n];
            for (int i = 0; i < n; i++)
            {
                perm[i] = i;
            }

            for (int i = n - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                int temp = perm[i];
                perm[i] = perm[j];
                perm[j] = temp;
            }

            return perm;
        }
    }
}