using Hifss.Extensions;
using System.IO;

namespace Hifss
{
    internal class GifReader
    {
        public const int ExtensionIntroducer = 0x21;
        public const int ImageDescriptor = 0x2C;
        public const int Trailer = 0x3B;

        private Stream _stream;
        private ExtensionResolver _extensionResolver;

        public GifReader(Stream stream)
        {
            _stream = stream;
            _extensionResolver = new ExtensionResolver();
        }

        public int ReadType()
        {
            int readByte = _stream.ReadByte();

            if (readByte == ExtensionIntroducer ||
                readByte == ImageDescriptor ||
                readByte == Trailer)
                return readByte;

            return -1;
        }

        public Extension ReadExtension()
        {
            int extensionLabel = _stream.ReadByte();
            Extension extension = _extensionResolver.GetExtension(extensionLabel);
            extension.Read(_stream);

            //Skipping extension terminator
            _stream.Seek(1, SeekOrigin.Current);

            return extension;
        }
    }
}
