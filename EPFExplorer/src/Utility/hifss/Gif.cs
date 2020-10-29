using System.Collections.Generic;
using System.IO;

namespace Hifss
{
    public class Gif
    {
        public List<GifImage> Frames = new List<GifImage>();

        public uint Width;
        public uint Height;
        public Color BackColor;

        public Gif()
        {
        }

        public Gif(string path)
        {
            LoadFromFile(path);
        }

        public bool LoadFromFile(string path)
        {
            return LoadFromMemory(File.ReadAllBytes(path));
        }

        public bool LoadFromMemory(byte[] data)
        {
            bool success = false;

            using (MemoryStream ms = new MemoryStream(data))
            {
                GifLoader loader = new GifLoader(ms);
                success = loader.LoadGif();

                if (success)
                {
                    Frames = loader.GetImages();
                    Width = loader.ScreenDescriptor.Width;
                    Height = loader.ScreenDescriptor.Height;
                    BackColor = loader.GetBackColor();
                }
            }

            return success;
        }
    }
}
