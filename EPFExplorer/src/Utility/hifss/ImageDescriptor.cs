using System;
using System.IO;

namespace Hifss
{
    internal class ImageDescriptor
    {
        public uint X { get; private set; }
        public uint Y { get; private set; }
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public uint LocalColorTableSize { get; private set; }
        public bool HasLocalColorTable { get; private set; }
        public bool Interlaced { get; private set; }

        public uint EntryCount
        {
            get { return (uint)Math.Pow(2, LocalColorTableSize + 1); }
        }

        public bool Read(Stream stream)
        {
            bool success = true;

            success &= readPosition(stream);
            success &= readSize(stream);
            success &= readPackedField(stream);

            return success;
        }

        private bool readPackedField(Stream stream)
        {
            int readByte = stream.ReadByte();

            if (readByte != -1)
            {
                HasLocalColorTable = ((byte)readByte & 0b10000000) != 0;
                Interlaced = ((byte)readByte & 0b01000000) != 0;
                LocalColorTableSize = (uint)((byte)readByte & 0b00000111);
            }
            else
                return false;

            return true;
        }

        private bool readSize(Stream stream)
        {
            byte[] sizeData = new byte[4];
            int readBytes = stream.Read(sizeData, 0, 4);

            if (readBytes == 4)
            {
                try
                {
                    Width = uint.Parse(sizeData[1].ToString("X").PadLeft(2, '0') + sizeData[0].ToString("X").PadLeft(2, '0'), System.Globalization.NumberStyles.HexNumber);
                    Height = uint.Parse(sizeData[3].ToString("X").PadLeft(2, '0') + sizeData[2].ToString("X").PadLeft(2, '0'), System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    return false;
                }
            }
            else
                return false;

            return true;
        }

        private bool readPosition(Stream stream)
        {
            byte[] sizeData = new byte[4];
            int readBytes = stream.Read(sizeData, 0, 4);

            if (readBytes == 4)
            {
                try
                {
                    X = uint.Parse(sizeData[1].ToString("X").PadLeft(2, '0') + sizeData[0].ToString("X").PadLeft(2, '0'), System.Globalization.NumberStyles.HexNumber);
                    Y = uint.Parse(sizeData[3].ToString("X").PadLeft(2, '0') + sizeData[2].ToString("X").PadLeft(2, '0'), System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    return false;
                }
            }
            else
                return false;

            return true;
        }
    }
}
