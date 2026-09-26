using System;

namespace Projekat1
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
                case "entropy":
                    if (args.Length < 2) { PrintUsage(); return; }
                    Entropy.Run(args[1]);
                    break;

                case "sf-encode":
                    if (args.Length < 3) { PrintUsage(); return; }
                    ShannonFanoCodec.Encode(args[1], args[2]);
                    Console.WriteLine("Shannon-Fano kodiranje zavrseno.");
                    break;

                case "sf-decode":
                    if (args.Length < 3) { PrintUsage(); return; }
                    ShannonFanoCodec.Decode(args[1], args[2]);
                    Console.WriteLine("Shannon-Fano dekodiranje zavrseno.");
                    break;

                case "huff-encode":
                    if (args.Length < 3) { PrintUsage(); return; }
                    HuffmanCodec.Encode(args[1], args[2]);
                    Console.WriteLine("Huffman kodiranje zavrseno.");
                    break;

                case "huff-decode":
                    if (args.Length < 3) { PrintUsage(); return; }
                    HuffmanCodec.Decode(args[1], args[2]);
                    Console.WriteLine("Huffman dekodiranje zavrseno.");
                    break;

                case "lz77-encode":
                    if (args.Length < 3) { PrintUsage(); return; }
                    LZ77.Encode(args[1], args[2]);
                    Console.WriteLine("LZ77 kodiranje zavrseno.");
                    break;

                case "lz77-decode":
                    if (args.Length < 3) { PrintUsage(); return; }
                    LZ77.Decode(args[1], args[2]);
                    Console.WriteLine("LZ77 dekodiranje zavrseno.");
                    break;

                case "lzw-encode":
                    if (args.Length < 3) { PrintUsage(); return; }
                    LZW.Encode(args[1], args[2]);
                    Console.WriteLine("LZW kodiranje zavrseno.");
                    break;

                case "lzw-decode":
                    if (args.Length < 3) { PrintUsage(); return; }
                    LZW.Decode(args[1], args[2]);
                    Console.WriteLine("LZW dekodiranje zavrseno.");
                    break;

                default:
                    PrintUsage();
                    break;
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("Upotreba:");
            Console.WriteLine("  Projekat1 entropy <fajl>");
            Console.WriteLine("  Projekat1 sf-encode <ulazni_fajl> <izlazni_fajl>");
            Console.WriteLine("  Projekat1 sf-decode <kodirani_fajl> <dekodirani_fajl>");
            Console.WriteLine("  Projekat1 huff-encode <ulazni_fajl> <izlazni_fajl>");
            Console.WriteLine("  Projekat1 huff-decode <kodirani_fajl> <dekodirani_fajl>");
            Console.WriteLine("  Projekat1 lz77-encode <ulazni_fajl> <izlazni_fajl>");
            Console.WriteLine("  Projekat1 lz77-decode <kodirani_fajl> <dekodirani_fajl>");
            Console.WriteLine("  Projekat1 lzw-encode <ulazni_fajl> <izlazni_fajl>");
            Console.WriteLine("  Projekat1 lzw-decode <kodirani_fajl> <dekodirani_fajl>");
        }
    }
}