using System.Collections.Generic;

namespace Projekat2
{
    static class Combinatorics
    {
        public static IEnumerable<int> MasksByIncreasingWeight(int n)
        {
            int total = 1 << n;
            var buckets = new List<int>[n + 1];
            for (int w = 0; w <= n; w++)
            {
                buckets[w] = new List<int>();
            }

            for (int mask = 0; mask < total; mask++)
            {
                int w = GF2.PopCount(mask);
                buckets[w].Add(mask);
            }

            for (int w = 0; w <= n; w++)
            {
                foreach (int mask in buckets[w])
                {
                    yield return mask;
                }
            }
        }
    }
}