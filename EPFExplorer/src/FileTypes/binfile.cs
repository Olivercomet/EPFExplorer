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
                        xmfiles[i].ReadSongInfo();
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

                int pos = offset_of_end_of_index_table;

                for (int i = 0; i < samplecount; i++) {

                    sfxfile newSample = new sfxfile();

                    /*
                     * So it turns out that the chunk of data at the top of each sample,
                     * which was previously ignored, is actually chock full of information
                     * about the sample, including some instrument parameters including
                     * volume and panning envelopes. The previous behavior of splitting by
                     * 0x02004000 was flawed as well - PCM samples would have 0x00004000 in
                     * that place instead, causing all sorts of wacky issues when trying to
                     * decode an ADPCM sample that is followed by one or more PCM samples
                     * (this happened quite a bit during debugging).
                     * 
                     * To keep things sane for the future, I'm including a speculative
                     * specification for the data contained in this chunk. Take this with a
                     * grain of salt, but I believe it should be correct (as long as my ears
                     * are working properly). Some of the descriptions may require
                     * understanding of the format of XM instruments. This also doesn't
                     * account for the reported 4 byte increase in size in HR - update this
                     * for HR!!!
                     * 
                     * Offset  | Size     | Description
                     * --------+----------+-------------------------------------------------
                     * 0x00    | 4        | The size of the sample data divided by 4 minus 4. (Data size = this * 4 + 4)
                     * 0x04    | 4        | The position of the start of the loop, or 0xFFFFFFFF for no loop. (?)
                     * 0x08    | 4        | The position of the end of the loop, or size+1 for no loop. (?)
                     * 0x0C    | 1        | The type of sample. 0 = s16 PCM, 2 = MS IMA ADPCM
                     * 0x0D    | 1        | Unknown (always 0). Could be upper byte of above.
                     * 0x0E    | 1        | Default volume for the sample.
                     * 0x0F    | 1        | Unknown (always 0). Could be upper byte of above.
                     * 0x10    | 1        | Finetune value. Stored as a signed byte.
                     * 0x11    | 1        | Transpose value. Stored as a signed byte.
                     * 0x12    | 1        | Default pan position. Stored as an unsigned byte, with 0x80 = center.
                     * 0x13    | 1        | Unknown (always 0).
                     * 0x14    | 1        | Number of nodes in the volume envelope.
                     * 0x15    | 1        | Position of volume envelope sustain node, or 0xFF for no sustain.
                     * 0x16    | 1        | Position of volume envelope loop start, or 0xFF for no loop.
                     * 0x17    | 1        | Position of volume envelope loop end, or 0xFF for no loop.
                     * 0x18    | 2*24     | Nodes in the volume envelope, alternating (x, y).
                     * 0x48    | 1        | Number of nodes in the panning envelope.
                     * 0x49    | 1        | Position of panning envelope sustain node, or 0xFF for no sustain.
                     * 0x4A    | 1        | Position of panning envelope loop start, or 0xFF for no loop.
                     * 0x4B    | 1        | Position of panning envelope loop end, or 0xFF for no loop.
                     * 0x4C    | 2*24     | Nodes in the panning envelope, alternating (x, y).
                     * 0x7C    | <size>   | The actual sample data.
                     */

                    uint offset = (uint)pos;
                    int length = (filebytes[pos] | (filebytes[pos + 1] << 8) | (filebytes[pos + 2] << 16) | (filebytes[pos + 3] << 24)) * 4 + 4;
                    newSample.isPCM = filebytes[pos + 12] != 2;
                    newSample.finetune = (sbyte)filebytes[pos + 16];
                    newSample.transpose = (sbyte)filebytes[pos + 17];
                    newSample.defaultvol = filebytes[pos + 14];
                    newSample.defaultpan = filebytes[pos + 18];

                    newSample.volenv.count = filebytes[pos + 20];
                    newSample.volenv.sustainPoint = filebytes[pos + 21];
                    newSample.volenv.loopStart = filebytes[pos + 22];
                    newSample.volenv.loopEnd = filebytes[pos + 23];
                    for (int j = 0; j < 24; j++) {
                        newSample.volenv.nodes[j] = (short)(filebytes[pos + j*2 + 24] | (filebytes[pos + j * 2 + 25] << 8));
                    }

                    newSample.panenv.count = filebytes[pos + 72];
                    newSample.panenv.sustainPoint = filebytes[pos + 73];
                    newSample.panenv.loopStart = filebytes[pos + 74];
                    newSample.panenv.loopEnd = filebytes[pos + 75];
                    for (int j = 0; j < 24; j++) {
                        newSample.panenv.nodes[j] = (short)(filebytes[pos + j * 2 + 76] | (filebytes[pos + j * 2 + 77] << 8));
                    }

                    pos += 120;
                    if (isHR) {
                        pos += 4;
                    }

                    newSample.parentbinfile = this;

                    newSample.samplerate = 44100;
                    newSample.filebytes = new byte[length];
                    Array.Copy(filebytes, pos, newSample.filebytes, 0, length);
                    newSample.offset = offset;
                    pos += length;
                    

                    samples[i] = newSample;
                    }

                foreach (xmfile xm in xmfiles) {
                    xm.samples = new List<sfxfile>(samples);
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



        public void SaveBin() {

            List<byte> musicdataSections = RebuildMusicDataSections();

            List<byte> newFileBytes = new List<byte>();

            for (int i = 0; i < offsetOfMusicInstructionData; i++)
                {
                newFileBytes.Add(filebytes[i]);
                }

            //now add new data

            for (int i = 0; i < musicdataSections.Count; i++)
                {
                newFileBytes.Add(musicdataSections[i]);
                }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Title = "Save bin file";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.Filter = "1PP music binary (*.bin)|*.bin|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                File.WriteAllBytes(saveFileDialog1.FileName, newFileBytes.ToArray());
                }
            }



        public List<byte> RebuildMusicDataSections() {

            List<byte> rowData = new List<byte>();
            List<byte> songInfos = new List<byte>();

            List<int> partialSongInfoOffsets = new List<int>();

            foreach (xmfile xm in xmfiles)
                {
                partialSongInfoOffsets.Add(songInfos.Count);

                songInfos.AddRange(xm.MakeEPFXMHeader());

                foreach (xmfile.Pattern p in xm.patterns)
                    {
                    songInfos.Add((byte)p.number_of_rows);
                    songInfos.Add((byte)(p.number_of_rows >> 8));
                    songInfos.Add((byte)(p.number_of_rows >> 16));
                    songInfos.Add((byte)(p.number_of_rows >> 24));

                    for (int i = 0; i < p.number_of_rows; i++)
                        {
                        bool emptyRow = true;

                        foreach (byte b in p.rows[i])
                            {
                            if (b != 0x80)
                                {
                                emptyRow = false;
                                break;
                                }
                            }

                        if (emptyRow)   //if the row is empty, just add FFs instead of an offset
                            {
                            songInfos.Add(0xFF);
                            songInfos.Add(0xFF);
                            songInfos.Add(0xFF);
                            songInfos.Add(0xFF);
                            continue;
                            }

                        //but if the row isn't empty, convert the row data to EPF format, add it to the row data list and store its offset here

                        uint offset_in_row_data = (uint)rowData.Count;

                        byte[] row_in_EPF_format = xm.ConvertRowToEPFFormat(p.rows[i]);

                        rowData.AddRange(row_in_EPF_format);

                        songInfos.Add((byte)offset_in_row_data);
                        songInfos.Add((byte)(offset_in_row_data >> 8));
                        songInfos.Add((byte)(offset_in_row_data >> 16));
                        songInfos.Add((byte)(offset_in_row_data >> 24));
                    }
                }
            }


            //correct the row data section size at the start of music.bin

            filebytes[0x0C] = (byte)rowData.Count;
            filebytes[0x0D] = (byte)(rowData.Count >> 8);
            filebytes[0x0E] = (byte)(rowData.Count >> 16);
            filebytes[0x0F] = (byte)(rowData.Count >> 24);

            //correct the song info offsets at the start of music.bin

            for (int i = 0; i < xmfiles.Count; i++)
                {
                int offset = 0x10 + (4 * i);

                uint offset_of_songInfo = offsetOfMusicInstructionData + (uint)rowData.Count + (uint)partialSongInfoOffsets[i];

                filebytes[offset] = (byte)offset_of_songInfo;
                filebytes[offset + 1] = (byte)(offset_of_songInfo >> 8);
                filebytes[offset + 2] = (byte)(offset_of_songInfo >> 16);
                filebytes[offset + 3] = (byte)(offset_of_songInfo >> 24);
                }


            List<byte> output = new List<byte>();

            output.AddRange(rowData);
            output.AddRange(songInfos);

            return output;
        }













    }
}
