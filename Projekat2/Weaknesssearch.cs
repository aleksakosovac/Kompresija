using System.IO;
using System.Text;

namespace Projekat2
{
    static class WeaknessSearch
    {
        public static void Find(int[,] H, int n, int m, double th0, double th1, int maxIterations, string outputPath)
        {
            foreach (int mask in Combinatorics.MasksByIncreasingWeight(n))
            {
                if (mask == 0)
                {
                    continue;
                }

                int[] received = new int[n];
                for (int i = 0; i < n; i++)
                {
                    received[i] = (mask >> i) & 1;
                }

                int[] decoded = GallagerB.Decode(H, m, n, received, th0, th1, maxIterations, out bool success);

                bool correctedToZero = success;
                if (correctedToZero)
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (decoded[i] != 0)
                        {
                            correctedToZero = false;
                            break;
                        }
                    }
                }

                if (!correctedToZero)
                {
                    int weight = GF2.PopCount(mask);
                    int decodedMask = ToMask(decoded);

                    var sb = new StringBuilder();
                    sb.AppendLine($"Najmanja tezina greske koju Gallager B ne ispravlja: {weight}");
                    sb.AppendLine($"Greska e            = {ToBitString(mask, n)}");
                    sb.AppendLine($"Dekodovano (posle najvise {maxIterations} iteracija, success={success}) = {ToBitString(decodedMask, n)}");
                    File.WriteAllText(outputPath, sb.ToString());
                    return;
                }
            }

            File.WriteAllText(outputPath, "Nije pronadjena greska koja dovodi do neuspeha (proverene sve tezine do n).");
        }

        private static int ToMask(int[] bits)
        {
            int mask = 0;
            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i] == 1)
                {
                    mask |= (1 << i);
                }
            }
            return mask;
        }

        private static string ToBitString(int value, int bits)
        {
            char[] chars = new char[bits];
            for (int i = 0; i < bits; i++)
            {
                chars[bits - 1 - i] = ((value >> i) & 1) == 1 ? '1' : '0';
            }
            return new string(chars);
        }
    }
}