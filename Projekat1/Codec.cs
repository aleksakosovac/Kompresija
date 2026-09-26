using System;
using System.IO;

namespace Projekat1
{
    static class Codec
    {
        public static void Encode(string inputPath, string outputPath, Func<long[], int[]> buildLengths)
        {
            byte[] data = File.ReadAllBytes(inputPath);
            long N = data.LongLength;

            long[] Ni = new long[256];
            foreach (byte b in data)
            {
                Ni[b]++;
            }

            int[] lengths = buildLengths(Ni);
            var codes = CanonicalCode.BuildCodes(lengths);

            using (FileStream fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            {
                byte[] nBytes = BitConverter.GetBytes(N);
                fs.Write(nBytes, 0, nBytes.Length);

                byte[] lengthBytes = new byte[256];
                for (int i = 0; i < 256; i++)
                {
                    lengthBytes[i] = (byte)lengths[i];
                }
                fs.Write(lengthBytes, 0, lengthBytes.Length);

                BitWriter bitWriter = new BitWriter(fs);
                foreach (byte b in data)
                {
                    (uint code, int length) = codes[b];
                    bitWriter.WriteBits(code, length);
                }
                bitWriter.Flush();
            }
        }

        public static void Decode(string inputPath, string outputPath)
        {
            using (FileStream fs = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
            {
                byte[] nBytes = new byte[8];
                ReadExact(fs, nBytes, 8);
                long N = BitConverter.ToInt64(nBytes, 0);

                byte[] lengthBytes = new byte[256];
                ReadExact(fs, lengthBytes, 256);

                int[] lengths = new int[256];
                for (int i = 0; i < 256; i++)
                {
                    lengths[i] = lengthBytes[i];
                }

                var codes = CanonicalCode.BuildCodes(lengths);
                DecodeTreeNode root = DecodeTree.Build(codes);

                using (FileStream outFs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    BitReader bitReader = new BitReader(fs);

                    for (long i = 0; i < N; i++)
                    {
                        DecodeTreeNode node = root;
                        while (!node.IsLeaf)
                        {
                            int bit = bitReader.ReadBit();
                            node = bit == 0 ? node.Zero! : node.One!;
                        }
                        outFs.WriteByte(node.Symbol);
                    }
                }
            }
        }

        private static void ReadExact(Stream s, byte[] buffer, int count)
        {
            int offset = 0;
            while (offset < count)
            {
                int read = s.Read(buffer, offset, count - offset);
                if (read == 0)
                {
                    throw new EndOfStreamException("Neocekivan kraj fajla pri citanju zaglavlja.");
                }
                offset += read;
            }
        }
    }
}