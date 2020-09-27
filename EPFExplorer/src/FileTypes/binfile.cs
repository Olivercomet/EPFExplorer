using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public int samplecount;

        sfxfile[] samples = new sfxfile[0];

        bool isHR = false;

        public List<string> MusicNamesEPF = new List<string>() { "Main theme", "Unused UI theme", "Command Room", "Coffee Shop", "Ski Village", "HQ", "Town", "Gift Shop", "Ski Lodge", "Pizza Parlor", "Coffee Shop", "Test Robots theme", "Cart Surfer", "Ice Fishing", "Gadget Room", "Night Club", "Dojo", "Boiler Room", "Menu", "Stage", "Beach", "Mine Shack", "Mine", "Jet Pack Adventure", "Snowboarding", "Snow Trekker", "Mission Complete", "Nothing" };
        public List<string> MusicNamesHR = new List<string>() { "Command Room", "Coffee Shop", "Ski Village", "HQ", "Town", "Gift Shop", "Ski Lodge", "Pizza Parlor", "Ice Fishing", "Gadget room", "Night Club", "Spy Snake", "Boiler room", "Menu", "Stage", "Beach", "Mine Shack", "Mine", "Grapple Gadget", "Main theme", "Herbert's Base", "Herbert's Base 2", "Herbert behind Ski Lodge", "Herbert's theme", "Geyser theme", "Unused", "Aqua Rescue", "Tallest Mountain", "Tallest Mountain 2", "Puffle Training Caves", "Spy Snake", "Credits" };

        int offset_of_end_of_index_table = 0;

        public int[] ima_index_table = new int[]{
              -1, -1, -1, -1, 2, 4, 6, 8,
              -1, -1, -1, -1, 2, 4, 6, 8
            };

        public int[] ima_step_table = new int[]{
          7, 8, 9, 10, 11, 12, 13, 14, 16, 17,
          19, 21, 23, 25, 28, 31, 34, 37, 41, 45,
          50, 55, 60, 66, 73, 80, 88, 97, 107, 118,
          130, 143, 157, 173, 190, 209, 230, 253, 279, 307,
          337, 371, 408, 449, 494, 544, 598, 658, 724, 796,
          876, 963, 1060, 1166, 1282, 1411, 1552, 1707, 1878, 2066,
          2272, 2499, 2749, 3024, 3327, 3660, 4026, 4428, 4871, 5358,
          5894, 6484, 7132, 7845, 8630, 9493, 10442, 11487, 12635, 13899,
          15289, 16818, 18500, 20350, 22385, 24623, 27086, 29794, 32767
        };

        public void ReadMusicBin()
        {
            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                filemagic = reader.ReadUInt16();
                   
                if (filemagic == 0x0103)
                    {
                    isHR = true;
                    }

                samplecount = reader.ReadByte();
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

                offset_of_end_of_index_table = (int)reader.BaseStream.Position;

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


                samples = new sfxfile[samplecount];

                int pos = (int)(offset_of_end_of_index_table + 76);

                if (isHR)
                    {
                    pos += 4;
                    }


                bool keepgoing = true;

                for (int i = 0; i < samplecount; i++)
                    {
                    List<byte> newSampleBytes = new List<byte>();

                    int i_within_loop = i;

                    uint offset = (uint)pos;

                    while (keepgoing)
                       {
                        if (pos + 3 >= filebytes.Length)
                            {
                            keepgoing = false;
                            i = samplecount - 1;
                            }
                        else if (filebytes[pos] == 0x02 && filebytes[pos + 1] == 0x00 && filebytes[pos + 2] == 0x40 && filebytes[pos + 3] == 0x00)
                            {
                            keepgoing = false;
                            pos += 64;
                            if (isHR)
                                {
                                pos += 4;
                                }
                        }
                        else
                            {
                            newSampleBytes.Add(filebytes[pos]);
                            pos++;
                            }
                        }

                    newSampleBytes.RemoveRange(newSampleBytes.Count-0x13,0x13);

                    sfxfile newSample = new sfxfile();
                    newSample.parentbinfile = this;

                    newSample.samplerate = 44100;
                    newSample.filebytes = newSampleBytes.ToArray();
                    newSample.offset = offset;

                    samples[i_within_loop] = newSample;
                    keepgoing = true;
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
                        isHR = true;
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


        public void ExportMusicSamples()
        {
            MessageBox.Show("This will export all the samples to your chosen directory. Proceed?", "Export all samples", MessageBoxButtons.YesNo);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.FileName = null;

            saveFileDialog1.Title = "Save WAV samples";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.Filter = "ADPCM WAV (*.wav)|*.wav|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                for (int i = 0; i < samples.Length; i++)
                    {
                    if (samples[i] != null)
                        {
                        samples[i].ConvertToWAV();
                        File.WriteAllBytes(saveFileDialog1.FileName.Replace(".wav","")+"_"+i+".wav", samples[i].filebytes);
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
