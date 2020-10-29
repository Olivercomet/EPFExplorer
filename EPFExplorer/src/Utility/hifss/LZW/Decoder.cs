using System;
using System.Collections.Generic;

namespace Hifss.LZW
{
    internal class Decoder
    {
        internal const byte BYTE_SIZE = 8;
        private byte _readingMask = 0b00000001;
        private int _settingMask = 0b00000000000000000000000000000001;
        private int _result = 0b00000000000000000000000000000000;
        private byte _currentPosition = 0;
        private int _currentByteIndex = 0;
        private List<byte> _bytes;

        public Decoder()
        {
        }

        public List<uint> ReadAllCodes(List<byte> data, byte codeSize)
        {
            _bytes = data;
            List<uint> codes = new List<uint>();

            int increaseBitLengthValue = (int)Math.Pow(2, codeSize++);
            byte initCodeSize = codeSize;
            int code;
            int i = 0;

            while ((code = ReadNextCode(codeSize)) != -1)
            {
                if (code == (1 << initCodeSize - 1))
                {
                    codeSize = initCodeSize;
                    i = 0;
                    increaseBitLengthValue = i + (int)(1 << initCodeSize - 1);
                }

                i++;
                if (i == increaseBitLengthValue && codeSize < 12)
                {
                    increaseBitLengthValue = i + (int)(1 << codeSize++);
                }

                codes.Add((uint)code);
            }

            return codes;
        }

        public int ReadNextCode(byte length)
        {
            prepare();

            for (int i = 0; i < length; i++)
            {
                if (_currentByteIndex == _bytes.Count)
                    return -1;

                if ((_bytes[_currentByteIndex] & _readingMask) != 0)
                {
                    _result = _result | _settingMask;
                }

                _settingMask = _settingMask << 1;
                move(1);
            }

            return _result;
        }

        private void prepare()
        {
            _settingMask = 0b00000000000000000000000000000001;
            _result = 0b00000000000000000000000000000000;
        }

        private void resetReadingMask()
        {
            _currentPosition = 0;
            _readingMask = 0b00000001;
        }

        private void move(byte offset)
        {
            _currentPosition += offset;

            if (_currentPosition >= BYTE_SIZE)
            {
                resetReadingMask();
                _currentByteIndex++;
                return;
            }

            _readingMask = (byte)(_readingMask << offset);
        }
    }
}
