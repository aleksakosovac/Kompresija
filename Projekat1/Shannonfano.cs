using System;
using System.Collections.Generic;

namespace Projekat1
{
    static class ShannonFano
    {
        public static int[] BuildLengths(long[] Ni)
        {
            var symbols = new List<(byte sym, long count)>();
            for (int i = 0; i < 256; i++)
            {
                if (Ni[i] > 0)
                {
                    symbols.Add(((byte)i, Ni[i]));
                }
            }

            int[] lengths = new int[256];

            if (symbols.Count == 0)
            {
                return lengths;
            }

            if (symbols.Count == 1)
            {
                lengths[symbols[0].sym] = 1;
                return lengths;
            }

            symbols.Sort((a, b) => b.count.CompareTo(a.count));

            Split(symbols, 0, symbols.Count - 1, lengths);

            return lengths;
        }

        private static void Split(List<(byte sym, long count)> symbols, int lo, int hi, int[] lengths)
        {
            if (lo == hi)
            {
                return;
            }

            long total = 0;
            for (int i = lo; i <= hi; i++)
            {
                total += symbols[i].count;
            }

            long running = 0;
            long bestDiff = long.MaxValue;
            int splitIndex = lo;

            for (int i = lo; i < hi; i++)
            {
                running += symbols[i].count;
                long diff = Math.Abs((total - running) - running);
                if (diff < bestDiff)
                {
                    bestDiff = diff;
                    splitIndex = i;
                }
            }

            for (int i = lo; i <= splitIndex; i++)
            {
                lengths[symbols[i].sym]++;
            }
            for (int i = splitIndex + 1; i <= hi; i++)
            {
                lengths[symbols[i].sym]++;
            }

            Split(symbols, lo, splitIndex, lengths);
            Split(symbols, splitIndex + 1, hi, lengths);
        }
    }
}