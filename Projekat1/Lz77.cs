using System;
using System.IO;

namespace Projekat1
{
    static class LZ77
    {
        public const int WindowSize = 4096;
        public const int MaxMatchLength = 255;
        public const int MinMatchLength = 3;

        private const int OffsetBits = 12;
        private const int LengthBits = 8;

        private const int HashBits = 15;
        private const int HashSize = 1 << HashBits;
        private const int MaxChain = 128;

        private class Search
        {
            private readonly byte[] _data;
            private readonly int[] _head;
            private readonly int[] _next;

            public Search(byte[] data)
            {
                _data = data;
                _head = new int[HashSize];
                for (int i = 0; i < HashSize; i++)
                {
                    _head[i] = -1;
                }
                _next = new int[data.Length];
            }

            private int Hash(int pos)
            {
                uint h = ((uint)_data[pos] << 16) | ((uint)_data[pos + 1] << 8) | _data[pos + 2];
                h = (h * 2654435761u) >> (32 - HashBits);
                return (int)(h & (HashSize - 1));
            }

            public void Insert(int pos)
            {
                if (pos + 2 >= _data.Length)
                {
                    return;
                }
                int h = Hash(pos);
                _next[pos] = _head[h];
                _head[h] = pos;
            }

            public void FindLongest(int pos, int total, out int bestOffset, out int bestLength)
            {
                bestOffset = 0;
                bestLength = 0;

                if (pos + 2 >= total)
                {
                    return;
                }

                int windowStart = pos - WindowSize;
                if (windowStart < 0)
                {
                    windowStart = 0;
                }

                int maxLen = total - pos;
                if (maxLen > MaxMatchLength)
                {
                    maxLen = MaxMatchLength;
                }

                int h = Hash(pos);
                int j = _head[h];
                int tries = 0;

                while (j != -1 && j >= windowStart && tries < MaxChain)
                {
                    int len = 0;
                    while (len < maxLen && _data[j + len] == _data[pos + len])
                    {
                        len++;
                    }

                    if (len > bestLength)
                    {
                        bestLength = len;
                        bestOffset = pos - j;
                        if (len >= maxLen)
                        {
                            break;
                        }
                    }

                    j = _next[j];
                    tries++;
                }
            }
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

                if (data.Length > 0)
                {
                    Search search = new Search(data);
                    int i = 0;

                    while (i < data.Length)
                    {
                        search.FindLongest(i, data.Length, out int offset, out int length);

                        if (length >= MinMatchLength)
                        {
                            writer.WriteBits(1, 1);
                            writer.WriteBits((uint)(offset - 1), OffsetBits);
                            writer.WriteBits((uint)length, LengthBits);

                            for (int k = 0; k < length; k++)
                            {
                                search.Insert(i + k);
                            }

                            i += length;
                        }
                        else
                        {
                            writer.WriteBits(0, 1);
                            writer.WriteBits(data[i], 8);

                            search.Insert(i);
                            i++;
                        }
                    }
                }

                writer.Flush();
            }
        }

        public static void Decode(string inputPath, string outputPath)
        {
            byte[] outBuffer;

            using (FileStream fs = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
            {
                byte[] nBytes = new byte[8];
                ReadExact(fs, nBytes, 8);
                long N = BitConverter.ToInt64(nBytes, 0);

                outBuffer = new byte[N];
                long produced = 0;

                BitReader reader = new BitReader(fs);

                while (produced < N)
                {
                    int flag = reader.ReadBit();

                    if (flag == 0)
                    {
                        int b = 0;
                        for (int k = 0; k < 8; k++)
                        {
                            b = (b << 1) | reader.ReadBit();
                        }
                        outBuffer[produced] = (byte)b;
                        produced++;
                    }
                    else
                    {
                        int offsetMinus1 = 0;
                        for (int k = 0; k < OffsetBits; k++)
                        {
                            offsetMinus1 = (offsetMinus1 << 1) | reader.ReadBit();
                        }
                        int offset = offsetMinus1 + 1;

                        int length = 0;
                        for (int k = 0; k < LengthBits; k++)
                        {
                            length = (length << 1) | reader.ReadBit();
                        }

                        int start = (int)produced - offset;
                        for (int k = 0; k < length; k++)
                        {
                            outBuffer[produced] = outBuffer[start + k];
                            produced++;
                        }
                    }
                }
            }

            File.WriteAllBytes(outputPath, outBuffer);
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