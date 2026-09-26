using System;
using System.Collections.Generic;
using System.IO;

namespace Projekat1
{
    static class LZW
    {
        public const int MaxDictionarySize = 4096;
        public const int CodeBits = 12;

        private class TrieNode
        {
            public Dictionary<byte, TrieNode>? Children;
            public int Code = -1;
        }

        public static void Encode(string inputPath, string outputPath)
        {
            byte[] data = File.ReadAllBytes(inputPath);
            long N = data.LongLength;

            using (FileStream fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            {
                byte[] nBytes = BitConverter.GetBytes(N);
                fs.Write(nBytes, 0, nBytes.Length);

                BitWriter writer = new BitWriter(fs);

                if (data.Length == 0)
                {
                    return;
                }

                var initial = new TrieNode[256];
                for (int b = 0; b < 256; b++)
                {
                    initial[b] = new TrieNode { Code = b };
                }

                int nextCode = 256;
                TrieNode current = initial[data[0]];

                for (int i = 1; i < data.Length; i++)
                {
                    byte c = data[i];

                    TrieNode? next = null;
                    current.Children?.TryGetValue(c, out next);

                    if (next != null)
                    {
                        current = next;
                    }
                    else
                    {
                        writer.WriteBits((uint)current.Code, CodeBits);

                        if (nextCode < MaxDictionarySize)
                        {
                            current.Children ??= new Dictionary<byte, TrieNode>();
                            current.Children[c] = new TrieNode { Code = nextCode };
                            nextCode++;
                        }

                        current = initial[c];
                    }
                }

                writer.WriteBits((uint)current.Code, CodeBits);
                writer.Flush();
            }
        }

        public static void Decode(string inputPath, string outputPath)
        {
            using (FileStream fs = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
            {
                byte[] nBytes = new byte[8];
                ReadExact(fs, nBytes, 8);
                long N = BitConverter.ToInt64(nBytes, 0);

                using (FileStream outFs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    if (N == 0)
                    {
                        return;
                    }

                    BitReader reader = new BitReader(fs);

                    var dictionary = new List<byte[]>(MaxDictionarySize);
                    for (int b = 0; b < 256; b++)
                    {
                        dictionary.Add(new byte[] { (byte)b });
                    }

                    long produced = 0;

                    int firstCode = ReadCode(reader);
                    byte[] w = dictionary[firstCode];
                    outFs.Write(w, 0, w.Length);
                    produced += w.Length;

                    while (produced < N)
                    {
                        int code = ReadCode(reader);

                        byte[] entry;
                        if (code < dictionary.Count)
                        {
                            entry = dictionary[code];
                        }
                        else if (code == dictionary.Count)
                        {
                            entry = new byte[w.Length + 1];
                            Array.Copy(w, entry, w.Length);
                            entry[w.Length] = w[0];
                        }
                        else
                        {
                            throw new InvalidDataException("Nevalidan LZW kod pri dekodiranju.");
                        }

                        outFs.Write(entry, 0, entry.Length);
                        produced += entry.Length;

                        if (dictionary.Count < MaxDictionarySize)
                        {
                            byte[] newEntry = new byte[w.Length + 1];
                            Array.Copy(w, newEntry, w.Length);
                            newEntry[w.Length] = entry[0];
                            dictionary.Add(newEntry);
                        }

                        w = entry;
                    }
                }
            }
        }

        private static int ReadCode(BitReader reader)
        {
            int code = 0;
            for (int k = 0; k < CodeBits; k++)
            {
                code = (code << 1) | reader.ReadBit();
            }
            return code;
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