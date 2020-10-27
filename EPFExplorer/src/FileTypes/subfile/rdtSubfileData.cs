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
            filebytes = new byte[basis.filebytes.Length];
            Array.Copy(basis.filebytes,0,filebytes,0,basis.filebytes.Length);
            IndexInList = basis.IndexInList;
            subfileType = basis.subfileType;
            spriteSettings = basis.spriteSettings;

            graphicsType = basis.graphicsType;

            width = basis.width;
            height = basis.height;

            offsetX = basis.offsetX;
            offsetY = basis.offsetY;

            image = basis.image;

            writeAddress = basis.writeAddress;
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

            public byte A;
            public byte B;

            public int FFcount = 0;
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
                curOffset += 2;

                newSetting.FFcount = 0;

                if (filebytes[curOffset] != 0xFF)
                    {
                    newSetting.FFcount = 2;     //Hopefully this holds true for all cases. I've only seen the FFs absent on FFcount 2.
                    curOffset += 2;
                    }
                else
                    {
                    while (filebytes[curOffset] == 0xFF && newSetting.FFcount < 4)
                        {
                        newSetting.FFcount++;
                        curOffset++;
                        }
                 }

                

                switch (newSetting.type)
                    {
                    case 0x03:  //bool
                        if (filebytes[curOffset] == 0x01)
                            {
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
                        if (newSetting.FFcount == 2)
                            {
                            newSetting.A = filebytes[curOffset];
                            curOffset++;
                            newSetting.B = filebytes[curOffset];
                            curOffset++;
                            }

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
                        File.WriteAllBytes("temp", filebytes);
                        break;
                    }
                spriteSettings.Add(newSetting);
                }
        }


        public void RebuildFilebytesFromSettings() {

            int size = 8;

            foreach (setting s in spriteSettings)
                {
                if(s.FFcount == 2)
                    {
                    size += 6;
                    }
                else
                    {
                    size += 8;
                    }

                size += s.name.Length;
                switch(s.type)
                    {
                    case 0x03:
                        size += 1;
                        break;
                    case 0x04:
                        size += 8;
                        break;
                    case 0x05:
                        size += 16;
                        if(s.FFcount == 2)
                            {
                            size += 2;
                            }
                        break;
                    }
                }

            filebytes = new byte[size];

            int curOffset = 6;

            parentfile.form1.WriteU16ToArray(filebytes, curOffset, (ushort)spriteSettings.Count);   //write setting count
            curOffset += 2;
          

            foreach (setting s in spriteSettings)
                {
                parentfile.form1.WriteU16ToArray(filebytes,curOffset,(ushort)s.name.Length);    //write name length
                curOffset += 2;

                for (int c = 0; c < s.name.Length; c++) //write name
                    {
                    filebytes[curOffset] = (byte)s.name[c];
                    curOffset++;
                    }

                parentfile.form1.WriteU16ToArray(filebytes, curOffset, (ushort)s.type);     //write setting type
                curOffset += 2;

                if (s.FFcount == 2)
                    {
                    parentfile.form1.WriteU16ToArray(filebytes, curOffset, 0xFFFF);     //write FFcount 2 padding
                    curOffset += 2;
                    }
                else
                    {
                    parentfile.form1.WriteU32ToArray(filebytes, curOffset, 0xFFFFFFFF);     //write FFcount 4 padding
                    curOffset += 4;
                    }
                

                switch (s.type)
                {
                    case 0x03:
                        if (s.trueOrFalse == true)
                            {
                            filebytes[curOffset] = 0x01;
                            }
                        curOffset++;
                        break;
                    case 0x04:
                        filebytes[curOffset] = (byte)(s.X);
                        filebytes[curOffset + 1] = (byte)(s.X >> 8);
                        filebytes[curOffset + 2] = (byte)(s.X >> 16);
                        filebytes[curOffset + 3] = (byte)(s.X >> 24);
                        curOffset += 4;
                        filebytes[curOffset] = (byte)(s.Y);
                        filebytes[curOffset + 1] = (byte)(s.Y >> 8);
                        filebytes[curOffset + 2] = (byte)(s.Y >> 16);
                        filebytes[curOffset + 3] = (byte)(s.Y >> 24);
                        curOffset += 4;
                        break;
                    case 0x05:
                        if (s.FFcount == 2)
                        {
                            filebytes[curOffset] = s.A;
                            curOffset++;
                            filebytes[curOffset] = s.B;
                            curOffset++;
                        }
                        filebytes[curOffset] = (byte)(s.X);
                        filebytes[curOffset + 1] = (byte)(s.X >> 8);
                        filebytes[curOffset + 2] = (byte)(s.X >> 16);
                        filebytes[curOffset + 3] = (byte)(s.X >> 24);
                        curOffset += 4;
                        filebytes[curOffset] = (byte)(s.Y);
                        filebytes[curOffset + 1] = (byte)(s.Y >> 8);
                        filebytes[curOffset + 2] = (byte)(s.Y >> 16);
                        filebytes[curOffset + 3] = (byte)(s.Y >> 24);
                        curOffset += 4;
                        filebytes[curOffset] = (byte)(s.X2);
                        filebytes[curOffset + 1] = (byte)(s.X2 >> 8);
                        filebytes[curOffset + 2] = (byte)(s.X2 >> 16);
                        filebytes[curOffset + 3] = (byte)(s.X2 >> 24);
                        curOffset += 4;
                        filebytes[curOffset] = (byte)(s.Y2);
                        filebytes[curOffset + 1] = (byte)(s.Y2 >> 8);
                        filebytes[curOffset + 2] = (byte)(s.Y2 >> 16);
                        filebytes[curOffset + 3] = (byte)(s.Y2 >> 24);
                        curOffset += 4;
                        break;
                }
            }
        }



        //stuff for graphics

        public string graphicsType = "";

        ushort width = 0;
        ushort height = 0;
        public ushort offsetX = 0;
        public ushort offsetY = 0;

        public Image image = null;

        public int writeAddress = 0; //for writing


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

            DecompressLZ10IfCompressed();

            if (subfileType == 2)
                {
                LoadSpriteSettings();
                }
            }


        public void DecompressLZ10IfCompressed() {
            if (filebytes[0] == 0x10 && BitConverter.ToInt32(filebytes, 1) > 0)
            {
                filebytes = DSDecmp.NewestProgram.Decompress(filebytes, new DSDecmp.Formats.Nitro.LZ10());
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
            if (filebytes == null || filebytes.Length == 0)
                {
                return;
                }

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
            offsetX = BitConverter.ToUInt16(filebytes, 4);
            offsetY = BitConverter.ToUInt16(filebytes, 6);

            image = parentfile.form1.NBFCtoImage(filebytes, 8, width, height, palette,parentfile.RDTSpriteBPP);
        
        }
    }
}
