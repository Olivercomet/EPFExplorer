using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPFExplorer
{
    public class binfile
    {
        public string binname;
        public string filename;
        public byte[] filebytes;

        uint filecount;

        public uint filemagic;

        public List<sfxfile> sfxfiles = new List<sfxfile>();
        public List<xmfile> xmfiles = new List<xmfile>();

        public Dictionary<uint, int> uniquefilemagicsandfreq = new Dictionary<uint, int>();

        public Dictionary<uint, string> knownfilemagic = new Dictionary<uint, string>();

        public uint offsetOfMusicInstructionData;

        public string binMode;


        public List<string> MusicNamesEPF = new List<string>() { "Main theme", "Unused UI theme", "Command Room", "Coffee Shop", "Ski Village", "HQ", "Town", "Gift Shop", "Ski Lodge", "Pizza Parlor", "Coffee Shop", "Test Robots theme", "Cart Surfer", "Ice Fishing", "Gadget Room", "Night Club", "Dojo", "Boiler Room", "Menu", "Stage", "Beach", "Mine Shack", "Mine", "Jet Pack Adventure", "Snowboarding", "Snow Trekker", "Mission Complete", "Nothing" };
        public List<string> MusicNamesHR = new List<string>() { "Command Room", "Coffee Shop", "Ski Village", "HQ", "Town", "Gift Shop", "Ski Lodge", "Pizza Parlor", "Ice Fishing", "Gadget room", "Night Club", "Spy Snake", "Boiler room", "Menu", "Stage", "Beach", "Mine Shack", "Mine", "Grapple Gadget", "Main theme", "Herbert's Base", "Herbert's Base 2", "Herbert behind Ski Lodge", "Herbert's theme", "Geyser theme", "Unused", "Aqua Rescue", "Tallest Mountain", "Tallest Mountain 2", "Puffle Training Caves", "Spy Snake", "Credits" };

        public void ReadMusicBin()
        {
            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                filemagic = reader.ReadUInt16();

                reader.BaseStream.Position++;   //skip over what is probably the sample count
                filecount = reader.ReadByte();  //music file count

                reader.BaseStream.Position = 0x08;
                offsetOfMusicInstructionData = reader.ReadUInt32();


                reader.BaseStream.Position = 0x10;

                Console.WriteLine(filecount);
                for (int i = 0; i < filecount; i++)
                {
                    xmfile newxmfile = new xmfile();
                    xmfiles.Add(newxmfile);
                    newxmfile.parentbinfile = this;
                    newxmfile.offset = reader.ReadUInt32();
                }

                Console.WriteLine(xmfiles.Count);

                for (int i = 0; i < xmfiles.Count; i++)
                {
                    if (i < xmfiles.Count - 1)
                        {
                        xmfiles[i].size = xmfiles[i + 1].offset - xmfiles[i].offset;
                        }
                    else
                        {
                        xmfiles[i].size = (uint)(filebytes.Length - xmfiles[i].offset); //it might be bigger than its intended size, but it won't matter once it's exported to XM
                        }

                    reader.BaseStream.Position = xmfiles[i].offset;

                    if (xmfiles[i].size < filebytes.Length)
                    {
                        xmfiles[i].filebytes = reader.ReadBytes((int)xmfiles[i].size);
                    }
                }


                if (xmfiles[2].offset == 2054556)
                    {
                    ApplyEPFMusicFileNames();
                    }
                else if (xmfiles[2].offset == 2339468)
                    {
                    ApplyHRMusicFileNames();
                    }
                else
                    {
                    foreach (xmfile xm in xmfiles)
                        {
                        xm.name = Path.GetFileName(filename) + xm.offset;
                        }
                    }
            }
        }
        public void ReadBin()
        {

            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                filemagic = reader.ReadUInt16();

               
               filecount = reader.ReadUInt16();
                
                    
                Console.WriteLine(filecount);
                for (int i = 0; i < filecount; i++)
                {
                    sfxfile newsfxfile = new sfxfile();
                    sfxfiles.Add(newsfxfile);
                    newsfxfile.parentbinfile = this;
                    newsfxfile.offset = reader.ReadUInt32();
                    newsfxfile.sizedividedby4 = reader.ReadUInt32();
                    if (filemagic == 0x0103)    //hr only
                        {
                        Console.WriteLine("hr");
                        reader.BaseStream.Position += 0x04;
                        }
                    newsfxfile.samplerate = reader.ReadUInt16();
                    
                    newsfxfile.unk1 = reader.ReadUInt16();
                  
                }

                Console.WriteLine(sfxfiles.Count);

                for (int i = 0; i < sfxfiles.Count; i++)
                {
                    reader.BaseStream.Position = sfxfiles[i].offset;

                    if (sfxfiles[i].sizedividedby4 * 4 < filebytes.Length)
                    {
                        sfxfiles[i].filebytes = reader.ReadBytes((int)sfxfiles[i].sizedividedby4 * 4);
                    }
                }
            }
        }


        public void ApplyEPFMusicFileNames() {

            for (int i = 0; i < xmfiles.Count; i++)
            {
                xmfiles[i].name = MusicNamesEPF[i] + ".xm";
            }

        }

        public void ApplyHRMusicFileNames() { 
        
            for (int i = 0; i < xmfiles.Count; i++)
                {
                xmfiles[i].name = MusicNamesHR[i] + ".xm";
                }

        }


    }
}
