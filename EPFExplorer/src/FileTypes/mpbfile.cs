using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPFExplorer
{
    public class mpbfile
    {
        public Form1 form1;

        public string filepath;

        public Byte[] filebytes;

        public int highest_tile_offset;

        public int known_tile_width = 0;

        public void Load() {

            highest_tile_offset = 0;

            for (int i = 0; i < filebytes.Length; i+=2)
                {
                int potentialHighestOffset = BitConverter.ToUInt16(filebytes, i);

                if (potentialHighestOffset > highest_tile_offset)
                    {
                    highest_tile_offset = potentialHighestOffset;
                    }
                }
        }
    }
}
