using System;
using System.IO;

namespace EPFExplorer
{
    public class mpbfile
    {
        public MPB_TSB_EditorForm editorForm;

        public Form1 form1;

        public string filepath;

        public byte[] filebytes;

        public int highest_tile_offset;

        public int known_tile_width = 0;

        public void Load()
        {

            //check for compression


            if (filebytes[0] == 0x11)
            {
                Console.WriteLine("we will guess at LZ11 compression");
                filebytes = DSDecmp.NewestProgram.Decompress(filebytes, new DSDecmp.Formats.Nitro.LZ11());
            }
            else if (filebytes[0] == 0x10)
            {
                Console.WriteLine("we will guess at LZ10 compression");
                filebytes = DSDecmp.NewestProgram.Decompress(filebytes, new DSDecmp.Formats.Nitro.LZ10());
            }


            if (filepath != null)
            {
                string justName = Path.GetFileName(filepath);

                if (editorForm != null && editorForm.MPBFilesAndWidthsInTiles.ContainsKey(justName))
                {
                    known_tile_width = editorForm.MPBFilesAndWidthsInTiles[justName];
                    editorForm.justAutoCorrected = true;
                    editorForm.ImageWidthInTiles.Value = known_tile_width;
                }
            }

            highest_tile_offset = 0;

            for (int i = 0; i < filebytes.Length; i += 2)
            {
                int potentialHighestOffset = BitConverter.ToUInt16(filebytes, i) & 0x1FFF;

                if ((BitConverter.ToUInt16(filebytes, i) & 0x2000) == 0x2000)
                {
                    potentialHighestOffset += 8192;
                }

                if (potentialHighestOffset > highest_tile_offset)
                {
                    highest_tile_offset = potentialHighestOffset;
                }
            }
        }
    }
}
