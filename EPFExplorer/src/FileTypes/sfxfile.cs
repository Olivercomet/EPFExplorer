using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPFExplorer
{
    class sfxfile
    {

        public uint offset;
        public byte[] filebytes;
        public uint filemagic;
        public uint sizedividedby4;
        public uint samplerate;
        public uint unk1;

        public binfile parentbinfile;


    }
}
