using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Hifss.LZW
{
    internal class LZWDecompressor
    {
        private byte _minCodeSize;
        private byte _subBlockSize;
        private List<byte> _data = new List<byte>();
        private List<uint> _codeStream = new List<uint>();
        private List<uint> _decompressedData = new List<uint>();
        private Decoder _decoder = new Decoder();

        public bool Decompress(Stream stream, out uint[] values)
        {
            bool success = true;

            success &= readCodeSize(stream);
            if (!success)
            {
                values = null;
                return false;
            }

            success &= readAllSubBlocks(stream);
            success &= getCodes();
            success &= decode();

            values = _decompressedData.ToArray();
            return success;
        }

        private bool decode()
        {
            uint clearCode = _codeStream[0];
            CodeTable codeTable = new CodeTable(_minCodeSize);
            output(_codeStream[1]);
            uint code;

            for (int i = 2; i < _codeStream.Count; i++)
            {
                code = _codeStream[i];

                if (codeTable.HasCode(code))
                {
                    if (codeTable[code][0] == (1 << _minCodeSize) + 1)
                    {
                        break;
                    }
                    else if (codeTable[code][0] == (1 << _minCodeSize))
                    {
                        codeTable.Initialize();
                        i++;
                        output(_codeStream[i]);
                    }
                    else
                    {
                        output(codeTable[code]);

                        List<uint> prevCode = new List<uint>(codeTable[_codeStream[i - 1]]);
                        prevCode.Add(codeTable[code][0]);

                        codeTable.Add(prevCode.ToArray());
                    }
                }
                else
                {
                    uint prevCodeValue = codeTable[_codeStream[i - 1]][0];

                    List<uint> prevAndCurrentCode = new List<uint>(codeTable[_codeStream[i - 1]]);
                    prevAndCurrentCode.Add(prevCodeValue);

                    output(prevAndCurrentCode.ToArray());
                    codeTable.Add(prevAndCurrentCode.ToArray());
                }
            }

            return true;
        }

        private void output(params uint[] values)
        {
            _decompressedData.AddRange(values);
        }

        private bool getCodes()
        {
            _codeStream.AddRange(_decoder.ReadAllCodes(_data, _minCodeSize));

            //TODO Error checking
            return true;
        }

        private bool readAllSubBlocks(Stream stream)
        {
            bool success = true;

            while (readSubBlockSize(stream))
                success &= readSubBlock(stream);

            return success;
        }

        private bool readSubBlock(Stream stream)
        {
            byte[] subBlockData = new byte[_subBlockSize];

            int readBytes = 0;
            while (readBytes < _subBlockSize)
            {
                readBytes += stream.Read(subBlockData, 0, _subBlockSize - readBytes);
            }

            _data.AddRange(subBlockData);

            //TODO Error checking
            return true;
        }

        private bool readSubBlockSize(Stream stream)
        {
            int readByte = stream.ReadByte();

            if (readByte != -1 && readByte != 0)
            {
                _subBlockSize = (byte)readByte;
            }
            else
                return false;
            return true;
        }

        private bool readCodeSize(Stream stream)
        {
            int readByte = stream.ReadByte();

            if (readByte != -1)
            {
                _minCodeSize = (byte)readByte;
            }
            else
                return false;
            return true;
        }
    }
}
