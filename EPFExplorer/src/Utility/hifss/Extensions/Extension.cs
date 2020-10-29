using System.IO;

namespace Hifss.Extensions
{
    internal abstract class Extension
    {
        public abstract bool Read(Stream stream);
    }
}
