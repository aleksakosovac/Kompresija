using System;
using System.IO;

namespace Projekat1
{
    static class Entropy
    {
        public static void Run(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            long N = data.LongLength;

            long[] Ni = new long[256];
            foreach (byte b in data)
            {
                Ni[b]++;
            }

            double H = 0.0;
            for (int i = 0; i < 256; i++)
            {
                if (Ni[i] == 0)
                {
                    continue;
                }

                double pi = (double)Ni[i] / N;
                H -= pi * Math.Log2(pi);
            }

            Console.WriteLine($"Fajl: {path}");
            Console.WriteLine($"Velicina fajla (N): {N} bajtova");
            Console.WriteLine($"Bajt-entropija H(p): {H:F6} bita/bajt");
        }
    }
}
