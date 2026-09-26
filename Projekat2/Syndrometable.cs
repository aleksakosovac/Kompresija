using System.IO;
using System.Text;

namespace Projekat2
{
    static class SyndromeTable
    {
        public static void Build(int[,] H, int n, int m, string outputPath, out int dMin)
        {
            int[] colMasks = GF2.ComputeColumnMasks(H, m, n);

            int syndromeCount = 1 << m;
            int[] corrector = new int[syndromeCount];
            for (int i = 0; i < syndromeCount; i++)
            {
                corrector[i] = -1;
            }

            dMin = -1;

            foreach (int mask in Combinatorics.MasksByIncreasingWeight(n))
            {
                int syndrome = GF2.Syndrome(colMasks, n, mask);

                if (corrector[syndrome] == -1)
                {
                    corrector[syndrome] = mask;
                }

                if (mask != 0 && syndrome == 0 && dMin == -1)
                {
                    dMin = GF2.PopCount(mask);
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Kodno rastojanje d_min = {dMin}");
            sb.AppendLine();
            sb.AppendLine("Sindrom (m bita) -> Korektor (n bita)  [tezina korektora]");

            for (int s = 0; s < syndromeCount; s++)
            {
                string syndromeStr = ToBitString(s, m);

                if (corrector[s] == -1)
                {
                    sb.AppendLine($"{syndromeStr} -> NEDOSTIZAN (H matrica nije punog ranga)");
                    continue;
                }

                string correctorStr = ToBitString(corrector[s], n);
                int weight = GF2.PopCount(corrector[s]);
                sb.AppendLine($"{syndromeStr} -> {correctorStr}  [{weight}]");
            }

            File.WriteAllText(outputPath, sb.ToString());
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