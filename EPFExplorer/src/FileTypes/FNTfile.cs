using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPFExplorer
{
    public class FNTfile
    {
        public byte[] filebytes;

        public int metaDataSize;
        public int imageDataStart;
        public int imageDataSize;
        public int numLetters;
        public int characterHeightInPixels;

        public List<letter> letters = new List<letter>();

        public class letter {
            public char name;

            public int type;
            public int imageOffset;
            public List<Bitmap> images = new List<Bitmap>();

            public int height;
            public int width;
        } 

        public void Load() {
            letters = new List<letter>();
            int pos = 0;
            metaDataSize = BitConverter.ToInt32(filebytes, pos);
            pos += 4;
            imageDataStart = BitConverter.ToInt32(filebytes, pos);
            pos += 4;
            imageDataSize = BitConverter.ToInt32(filebytes, pos);
            pos += 4;
            numLetters = BitConverter.ToInt16(filebytes, pos);
            pos = 0x10;
            characterHeightInPixels = BitConverter.ToInt16(filebytes, pos);
            pos = 0x14;

            for (int i = 0; i < numLetters; i++) {
                letter newLetter = new letter() { name = (char)BitConverter.ToInt16(filebytes, pos)};
                pos += 2;
                pos += 2;
                letters.Add(newLetter);
            }

            pos += 0x10;

            foreach (letter l in letters) {
                l.height = filebytes[pos];
                pos += 2;
                l.width = filebytes[pos];
                pos += 2;
                pos += 0x06;
                l.imageOffset = BitConverter.ToInt32(filebytes, pos);
                pos += 4;
                pos++;
                l.type = filebytes[pos];
                pos++;
                
            }

            foreach (letter l in letters) {

                int numVersions = 1;

                if (l.type != 0)
                {
                    for (int bitShiftedTemp = l.type; bitShiftedTemp > 0; bitShiftedTemp >>= 1) {
                        numVersions += bitShiftedTemp & 0x01;
                    }
                    numVersions = 2;
                }
                else
                {
                    l.height = characterHeightInPixels;

                    if (letters.IndexOf(l) == (letters.Count - 1))
                    {
                        l.width = (((imageDataStart + imageDataSize) - l.imageOffset) / numVersions) / characterHeightInPixels;
                    }
                    else
                    {
                        l.width = ((letters[letters.IndexOf(l) + 1].imageOffset - l.imageOffset) / numVersions) / characterHeightInPixels;
                    }
                }


                for (int i = 0; i < numVersions; i++)
                {
                    pos = l.imageOffset;

                    l.images.Add(new Bitmap(l.width, l.height));

                    for (int y = 0; y < l.height; y++)
                    {
                        for (int x = 0; x < l.width; x++)
                        {
                            l.images[i].SetPixel(x, y, Color.FromArgb(filebytes[pos], 0, 0, 0));
                            pos++;
                        }
                    }
                }
            }
        }
    }
}
