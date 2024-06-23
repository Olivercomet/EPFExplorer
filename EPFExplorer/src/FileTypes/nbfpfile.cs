using System.Drawing;

namespace EPFExplorer
{
    public class nbfpfile
    {
        public Form1 form1;
        public string filepath;
        public byte[] filebytes;
        public Color[] palette;

        public void Load()
        {
            palette = form1.GetPalette(filebytes, 0, 8);
        }
    }
}
