using System;
using System.IO;

namespace Hifss
{
    internal class ScreenDescriptor
    {
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public uint BitsPerPixel { get; private set; }
        public uint GlobalColorTableSize { get; private set; }
        public uint BackgroundColorIndex { get; private set; }
        public bool HasGlobalColorTable { get; private set; }

        public uint EntryCount
        {
            get { return (uint)Math.Pow(2, GlobalColorTableSize + 1); }
        }

        public uint GlobalColorTableLength
        {
            get { return EntryCount * 3; }
        }


        public bool Read(Stream stream)
        {
            bool success = true;

            success &= readSize(stream);
            success &= readPackedField(stream);
            success &= readBackgroundColorIndex(stream);
            success &= readPixelAspectRatio(stream);

            return success;
        }

        private bool readPixelAspectRatio(Stream stream)
        {
            //Current implementation ignores pixel aspect ratio
            int readByte = stream.ReadByte();
            return readByte != -1;
        }

        private bool readBackgroundColorIndex(Stream stream)
        {
            int readByte = stream.ReadByte();

            if (readByte != -1)
            {
                BackgroundColorIndex = (uint)readByte;
            }
            else
                return false;

            return true;
        }

        private bool readPackedField(Stream stream)
        {
            int packedField = stream.ReadByte();

            if (packedField != -1)
            {
                HasGlobalColorTable = ((byte)packedField & 0b10000000) != 0;
                BitsPerPixel = (uint)(((byte)packedField >> 4) & 0b00000111);
                GlobalColorTableSize = (uint)((byte)packedField & 0b00000111);

                return true;
            }
            else
                return false;
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
    }
}
