using System;

namespace EPFExplorer
{
    public class nbfcTilesetFile
    {
        public Form1 form1;

        public string filepath;

        public Byte[] filebytes;

        public int number_of_tiles;

        public void Load()
        {

            number_of_tiles = (filebytes.Length) / 64;
        }
    }
}
