using System;
using System.IO;
using System.Text;

namespace Projekat2
{
    static class MatrixIO
    {
        public static void SaveToFile(int[,] H, string path)
        {
            int rows = H.GetLength(0);
            int cols = H.GetLength(1);

            var sb = new StringBuilder();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    sb.Append(H[i, j]);
                    if (j < cols - 1)
                    {
                        sb.Append(' ');
                    }
                }
                sb.Append('\n');
            }

            File.WriteAllText(path, sb.ToString());
        }

        public static void PrintToConsole(int[,] H)
        {
            int rows = H.GetLength(0);
            int cols = H.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write(H[i, j]);
                    Console.Write(' ');
                }
                Console.WriteLine();
            }
        }
    }
}