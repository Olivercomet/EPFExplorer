using System.IO;

namespace Hifss.Extensions
{
    internal class GraphicsControlExtension : Extension
    {
        public FrameDisposalMethod DisposalMethod { get; private set; }
        public bool HasTransparency { get; private set; }
        public int Delay { get; private set; }
        public int TransparentColorIndex { get; private set; }

        private uint ExtensionSize;

        public override bool Read(Stream stream)
        {
            bool success = true;

            success &= readSize(stream);
            success &= readPackedFiled(stream);
            success &= readDelay(stream);
            success &= readTransparentColorIndex(stream);

            return true;
        }

        private bool readTransparentColorIndex(Stream stream)
        {
            int readByte = stream.ReadByte();

            if (readByte != -1)
            {
                TransparentColorIndex = readByte;
            }
            else
                return false;

            return true;
        }

        private bool readDelay(Stream stream)
        {
            byte[] sizeData = new byte[2];
            int readBytes = stream.Read(sizeData, 0, 2);

            if (readBytes == 2)
            {
                try
                {
                    Delay = int.Parse(sizeData[1].ToString("X") + sizeData[0].ToString("X"), System.Globalization.NumberStyles.HexNumber);
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

        private bool readPackedFiled(Stream stream)
        {
            int readByte = stream.ReadByte();

            if (readByte != -1)
            {
                int disposalMethod = ((byte)readByte >> 2 & 0b00000111);
                DisposalMethod = (FrameDisposalMethod)disposalMethod;

                HasTransparency = ((byte)readByte & 0b00000001) != 0;

                return true;
            }
            else
                return false;
        }

        private bool readSize(Stream stream)
        {
            int readByte = stream.ReadByte();

            if (readByte != -1)
            {
                ExtensionSize = (uint)readByte;
                return true;
            }

            return false;
        }
    }
}
