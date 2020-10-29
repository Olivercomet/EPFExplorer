using System.Collections.Generic;
using System.IO;

namespace Hifss
{
    internal class ColorTable
    {
        private List<Color> _colors { get; set; } = new List<Color>();

        public Color this[int index]
        {
            get { return _colors[index]; }
        }

        public bool ReadColors(Stream stream, uint colorCount)
        {
            for (int i = 0; i < colorCount; i++)
            {
                try
                {
                    byte[] color = new byte[3];
                    int readBytes = stream.Read(color, 0, 3);

                    _colors.Add(new Color(color[0], color[1], color[2]));
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
    }
}
