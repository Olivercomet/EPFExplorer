using System;
using System.IO;

namespace Hifss.Extensions
{
    internal class PlainTextExtension : Extension
    {
        public override bool Read(Stream stream)
        {
            //Skipping 2 blocks of data.
            for (int j = 0; j < 2; j++)
            {
                int bytesToSkip = stream.ReadByte();
                stream.Seek(bytesToSkip, SeekOrigin.Current);
            }

            return true;
        }
    }
}
