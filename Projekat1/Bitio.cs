using System;
using System.IO;

namespace Projekat1
{
    class BitWriter
    {
        private readonly Stream _stream;
        private byte _current;
        private int _bitsInCurrent;

        public BitWriter(Stream stream)
        {
            _stream = stream;
        }

        public void WriteBits(uint code, int length)
        {
            for (int i = length - 1; i >= 0; i--)
            {
                int bit = (int)((code >> i) & 1);
                _current = (byte)((_current << 1) | bit);
                _bitsInCurrent++;

                if (_bitsInCurrent == 8)
                {
                    _stream.WriteByte(_current);
                    _current = 0;
                    _bitsInCurrent = 0;
                }
            }
        }

        public void Flush()
        {
            if (_bitsInCurrent > 0)
            {
                _current = (byte)(_current << (8 - _bitsInCurrent));
                _stream.WriteByte(_current);
                _current = 0;
                _bitsInCurrent = 0;
            }
        }
    }

    class BitReader
    {
        private readonly Stream _stream;
        private int _currentByte;
        private int _bitsLeft;

        public BitReader(Stream stream)
        {
            _stream = stream;
            _bitsLeft = 0;
        }

        public int ReadBit()
        {
            if (_bitsLeft == 0)
            {
                _currentByte = _stream.ReadByte();
                if (_currentByte == -1)
                {
                    throw new EndOfStreamException("Neocekivan kraj kodiranog fajla.");
                }
                _bitsLeft = 8;
            }

            _bitsLeft--;
            return (_currentByte >> _bitsLeft) & 1;
        }
    }
}