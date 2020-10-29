using static Hifss.Extensions.GraphicsControlExtension;

namespace Hifss
{
    public class GifImage
    {
        public byte[] Data { get; private set; }
        public uint Delay { get; private set; }
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public uint X { get; private set; }
        public uint Y { get; private set; }
        public FrameDisposalMethod DisposalMethod { get; private set; }

        public GifImage(byte[] data, uint x, uint y, uint width, uint height, int delay, FrameDisposalMethod disposalMethod)
        {
            Data = data;
            Width = width;
            Height = height;
            X = x;
            Y = y;
            Delay = (uint)delay;
        }
    }
}
