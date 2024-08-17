using System;
using System.Drawing;

namespace EPFExplorer
{
    public class tsbfile
    {
        public Form1 form1;

        public string filepath;

        public Byte[] filebytes;

        public Color[] palette;

        public int number_of_tiles;

        public void Load()
        {
            palette = new Color[256];   //create palette and fill it with the colours from the tsb

            for (int i = 0; i < 256; i++)
            {
                palette[i] = form1.ABGR1555_to_RGBA32(BitConverter.ToUInt16(filebytes, i * 2));
            }

            number_of_tiles = (filebytes.Length - 0x200) / 64;
        }
    }
}
