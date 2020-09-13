using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPFExplorer
{
    public class rdtSubfileData
    {
        public rdtSubfileData()
            {
            }

       public rdtSubfileData(rdtSubfileData basis)
            {
            parentfile = basis.parentfile;
            filebytes = basis.filebytes;
            IndexInList = basis.IndexInList;
            subfileType = basis.subfileType;
            spriteSettings = basis.spriteSettings;

            graphicsType = basis.graphicsType;

            width = basis.width;
            height = basis.height;

            image = basis.image;
            }

        public archivedfile parentfile;

        //can be a centre bounds file, a sprite, a palette, etc
        public Byte[] filebytes = new byte[0];

        public int IndexInList = 0; //temp while I'm working some stuff out

        public int subfileType; //0x02 sprite settings, 0x03 subfile table, 0x04 compressed file


        //stuff for 0x02 sprite settings file

        public List<setting> spriteSettings = new List<setting>();

        public class setting {
            public string name = "";
            public ushort type;    //0x03 bool, 0x04 vector2, 0x05 2D rect

            public bool trueOrFalse = false;

            public int X;
            public int Y;

            public int X2;
            public int Y2; 
        }

       

        public void LoadSpriteSettings() {    //load sprite settings file (centre, bounds, etc)

            int curOffset = 6;

            ushort settingCount = BitConverter.ToUInt16(filebytes, curOffset);
            curOffset += 2;

            for (int i = 0; i < settingCount; i++)
                {
                setting newSetting = new setting();

                ushort stringLength = BitConverter.ToUInt16(filebytes, curOffset);
                curOffset += 2;

                for (int c = 0; c < stringLength; c++)
                    {
                    newSetting.name += (char)filebytes[curOffset];
                    curOffset++;
                    }

                newSetting.type = BitConverter.ToUInt16(filebytes, curOffset);
                curOffset += 6; //I don't know if the 0xFFFFFFFF is padding or not, so skip over it for now

                switch (newSetting.type)
                    {
                    case 0x03:  //bool
                        if (filebytes[curOffset] == 0x01)
                            {
                            Console.WriteLine(newSetting.name + " true");
                            newSetting.trueOrFalse = true;  
                            }
                        curOffset++;
                        break;

                    case 0x04:  //vector2
                        newSetting.X = BitConverter.ToInt32(filebytes, curOffset);
                        curOffset += 4;
                        newSetting.Y = BitConverter.ToInt32(filebytes, curOffset);
                        curOffset += 4;
                        break;

                    case 0x05:  //2D rect
                        newSetting.X = BitConverter.ToInt32(filebytes, curOffset);
                        curOffset += 4;
                        newSetting.Y = BitConverter.ToInt32(filebytes, curOffset);
                        curOffset += 4;
                        newSetting.X2 = BitConverter.ToInt32(filebytes, curOffset);
                        curOffset += 4;
                        newSetting.Y2 = BitConverter.ToInt32(filebytes, curOffset);
                        curOffset += 4;
                        break;
                    default:
                        MessageBox.Show("Unknown sprite setting with ID \""+newSetting.type+"\"", "Unrecognised item", MessageBoxButtons.OK);
                        break;
                    }
                Console.WriteLine(newSetting.name);
                spriteSettings.Add(newSetting);
                }
        }



        //stuff for graphics

        public string graphicsType = "";

        ushort width = 0;
        ushort height = 0;

        public Image image = null;


        //generic functions follow

        public void ReadRawData(Byte[] bytes, int offset)
            {
            if (offset > bytes.Length || offset < 0)
                {
                return;
                }

            subfileType = BitConverter.ToUInt16(bytes,offset);
            int size = BitConverter.ToInt32(bytes,offset+2);

            if (size < 0 || size > parentfile.parentrdtfile.filebytes.Length)
                {
                return;
                }

            filebytes = new byte[size];
            Array.Copy(bytes,offset+6,filebytes,0,size);
            Console.WriteLine("RDT data found: type " + subfileType);

            if (filebytes[0] == 0x10 && BitConverter.ToInt32(filebytes, 1) > 0)
                {
                filebytes = DSDecmp.NewestProgram.Decompress(filebytes, new DSDecmp.Formats.Nitro.LZ10());
                }

            //File.WriteAllBytes(IndexInList.ToString(), filebytes);

            if (subfileType == 2)
                {
                LoadSpriteSettings();
                }
            }

        public void Parse() { 

            if (graphicsType == "GraphicsMetadata")
                {
                LoadGraphicsMetadata();
                return;
                }
            else if (graphicsType == "GraphicsFrameDurations")
                {
                LoadFrameDurations();
                return;
                }
        
        switch (subfileType)
            {
                case 0x02:
                    graphicsType = null;
                    Console.WriteLine("TEST");
                    LoadSpriteSettings();
                    break;
                case 0x03:
                    graphicsType = null;
                    MessageBox.Show("Why is an RDT subfile table being parsed by Parse()? It should have been handled earlier.", "That shouldn't have happened", MessageBoxButtons.OK);
                    break;
                case 0x04: //graphics

                    break;
                default:
                    graphicsType = null;
                    MessageBox.Show("Unknown RDT subfiledata with ID \"" + subfileType + "\"", "Unrecognised item", MessageBoxButtons.OK);
                    break;
            }
        }

        public void LoadGraphicsMetadata() {
            parentfile.RDTSpriteNumFrames = BitConverter.ToUInt16(filebytes,0);
            parentfile.RDTSpriteWidth = BitConverter.ToUInt16(filebytes, 2);
            parentfile.RDTSpriteHeight = BitConverter.ToUInt16(filebytes, 4);
            parentfile.RDTSpriteBPP = filebytes[6];
        }


        public void LoadFrameDurations() {

            parentfile.RDTSpriteFrameDurations = new List<ushort>();

            for (int i = 0; i < filebytes.Length; i+=2)
                {
                parentfile.RDTSpriteFrameDurations.Add(BitConverter.ToUInt16(filebytes,i));
                }
        }


        public void LoadImage(Color[] palette) {

            width = BitConverter.ToUInt16(filebytes, 0);
            height = BitConverter.ToUInt16(filebytes, 2);
            Console.WriteLine("going to create image of width " + width + " and height " + height);

            image = parentfile.NBFCtoImage(filebytes, 8, width, height, palette,parentfile.RDTSpriteBPP);

        }

    }
}
