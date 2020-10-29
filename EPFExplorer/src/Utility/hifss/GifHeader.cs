using System.IO;
using System.Text;

namespace Hifss
{
    internal class GifHeader
    {
        public enum GifVersion
        {
            GIF89a, GIF87a
        }

        public GifVersion Version { get; set; }

        public bool Read(Stream stream)
        {
            byte[] headerData = new byte[6];
            int readBytes = stream.Read(headerData, 0, 6);

            if (readBytes == 6)
            {
                string header = Encoding.ASCII.GetString(headerData);

                if (!header.StartsWith("GIF"))
                    return false;

                if (header.Substring(3, 3) == "89a")
                    Version = GifVersion.GIF89a;
                else if (header.Substring(3, 3) == "87a")
                    Version = GifVersion.GIF87a;
                else
                    return false;
            }
            else
                return false;

            return true;
        }
    }
}
