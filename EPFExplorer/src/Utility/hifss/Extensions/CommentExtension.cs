using System;
using System.IO;

namespace Hifss.Extensions
{
    internal class CommentExtension : Extension
    {
        public override bool Read(Stream stream)
        {
            int bytesToSkip = stream.ReadByte();
            stream.Seek(bytesToSkip, SeekOrigin.Current);

            return true;
        }
    }
}
