using System;
using System.IO;

namespace Projekat2
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintUsage();
                return;
            }

            string command = args[0];

            switch (command)
            {
                case "ldpc-build":
                    if (args.Length < 7) { PrintUsage(); return; }

                    int n1 = int.Parse(args[1]);
                    int m1 = int.Parse(args[2]);
                    int wr1 = int.Parse(args[3]);
                    int wc1 = int.Parse(args[4]);
                    int seed1 = int.Parse(args[5]);
                    string outputPath1 = args[6];

                    int[,] H1 = LdpcMatrixBuilder.Build(n1, m1, wr1, wc1, seed1);
                    MatrixIO.SaveToFile(H1, outputPath1);
                    MatrixIO.PrintToConsole(H1);
                    break;

                case "ldpc-syndrome":
                    if (args.Length < 7) { PrintUsage(); return; }

                    int n2 = int.Parse(args[1]);
                    int m2 = int.Parse(args[2]);
                    int wr2 = int.Parse(args[3]);
                    int wc2 = int.Parse(args[4]);
                    int seed2 = int.Parse(args[5]);
                    string outputPath2 = args[6];

                    int[,] H2 = LdpcMatrixBuilder.Build(n2, m2, wr2, wc2, seed2);
                    SyndromeTable.Build(H2, n2, m2, outputPath2, out int dMin2);
                    Console.WriteLine($"Kodno rastojanje d_min = {dMin2}");
                    Console.WriteLine($"Tabela sindroma/korektora sacuvana u: {outputPath2}");
                    break;

                case "ldpc-decode":
                    if (args.Length < 11) { PrintUsage(); return; }

                    int n3 = int.Parse(args[1]);
                    int m3 = int.Parse(args[2]);
                    int wr3 = int.Parse(args[3]);
                    int wc3 = int.Parse(args[4]);
                    int seed3 = int.Parse(args[5]);
                    double th03 = double.Parse(args[6]);
                    double th13 = double.Parse(args[7]);
                    int maxIter3 = int.Parse(args[8]);
                    string receivedStr3 = args[9];
                    string outputPath3 = args[10];

                    int[,] H3 = LdpcMatrixBuilder.Build(n3, m3, wr3, wc3, seed3);
                    int[] received3 = new int[n3];
                    for (int i = 0; i < n3; i++)
                    {
                        received3[i] = receivedStr3[i] == '1' ? 1 : 0;
                    }

                    int[] decoded3 = GallagerB.Decode(H3, m3, n3, received3, th03, th13, maxIter3, out bool success3);
                    string decodedStr3 = string.Join("", decoded3);
                    File.WriteAllText(outputPath3, $"Uspesno dekodovano (svi checkovi zadovoljeni): {success3}\nDekodovano: {decodedStr3}\n");
                    Console.WriteLine($"Uspesno: {success3}, dekodovano: {decodedStr3}");
                    break;

                case "ldpc-weakness":
                    if (args.Length < 9) { PrintUsage(); return; }

                    int n4 = int.Parse(args[1]);
                    int m4 = int.Parse(args[2]);
                    int wr4 = int.Parse(args[3]);
                    int wc4 = int.Parse(args[4]);
                    int seed4 = int.Parse(args[5]);
                    double th04 = double.Parse(args[6]);
                    double th14 = double.Parse(args[7]);
                    int maxIter4 = int.Parse(args[8]);
                    string outputPath4 = args.Length > 9 ? args[9] : "weakness.txt";

                    int[,] H4 = LdpcMatrixBuilder.Build(n4, m4, wr4, wc4, seed4);
                    WeaknessSearch.Find(H4, n4, m4, th04, th14, maxIter4, outputPath4);
                    Console.WriteLine($"Rezultat sacuvan u: {outputPath4}");
                    Console.WriteLine();
                    Console.WriteLine(File.ReadAllText(outputPath4));
                    break;

                default:
                    PrintUsage();
                    break;
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("Upotreba:");
            Console.WriteLine("  Projekat2 ldpc-build <n> <n-k> <wr> <wc> <seed> <izlazni_fajl>");
            Console.WriteLine("  Projekat2 ldpc-syndrome <n> <n-k> <wr> <wc> <seed> <izlazni_fajl>");
            Console.WriteLine("  Projekat2 ldpc-decode <n> <n-k> <wr> <wc> <seed> <th0> <th1> <maxIter> <primljeni_bitovi> <izlazni_fajl>");
            Console.WriteLine("  Projekat2 ldpc-weakness <n> <n-k> <wr> <wc> <seed> <th0> <th1> <maxIter> <izlazni_fajl>");
        }
    }
}