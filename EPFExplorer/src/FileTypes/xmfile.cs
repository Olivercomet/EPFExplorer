using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPFExplorer   
{
    public class xmfile        
    {
        public uint offset;
        public Byte[] filebytes;
        public uint size;

        public binfile parentbinfile;

        string moduleName = "default";

        public int number_of_patterns_in_one_loop;
        public int restartPosition;

        public int numchannels;
        public int numpatterns;
        public int numinstruments;

        public int tempo;
        public int bpm;

        List<Pattern> patterns = new List<Pattern>();
        List<Pattern> patternsInPlayingOrder = new List<Pattern>();

        public class Pattern {
            public int index;
            public int number_of_rows;
            public int patternSize;

            public List<Byte[]> rows = new List<Byte[]>();
        }

        public Pattern GetPatternWithIndex(List<Pattern> patternList, int index) { 
        
        foreach (Pattern p in patternList)
            {
            if (p.index == index)
                {
                    return p;
                }
            }

            return null;
        }


        public Byte[] GetRow(binfile bin, int pos) {

            int initialPos = pos;

            List<Byte> output = new List<Byte>();

            if (pos == -1)  //return a blank row
                 {
                 for (int i = 0; i < numchannels; i++)
                    {
                    output.Add(0x80);    
                    }

                 return output.ToArray();
                 }


            //extract control bytes from a run-on series of bits, where each section of 5 bits is a control byte

            Byte[] controlBytes = new byte[numchannels];

            int b = 0;

            while (b < numchannels)
            {
                controlBytes[b] = (byte)((bin.filebytes[bin.offsetOfMusicInstructionData + pos] & 0xF8) >> 3);
                b++;
                if (b == numchannels) { break; }

                controlBytes[b] = (byte)((bin.filebytes[bin.offsetOfMusicInstructionData + pos] & 0x07) << 2);
                controlBytes[b] |= (byte)((bin.filebytes[bin.offsetOfMusicInstructionData + pos + 1] & 0xC0) >> 6);
                b++;
                pos++;
                if (b == numchannels) { break; }

                controlBytes[b] = (byte)((bin.filebytes[bin.offsetOfMusicInstructionData + pos] & 0x3E) >> 1);
                b++;
                if (b == numchannels) { break; }

                controlBytes[b] = (byte)((bin.filebytes[bin.offsetOfMusicInstructionData + pos] & 0x01) << 4);
                controlBytes[b] |= (byte)((bin.filebytes[bin.offsetOfMusicInstructionData + pos + 1] & 0xF0) >> 4);
                b++;
                pos++;
                if (b == numchannels) { break; }

                controlBytes[b] = (byte)((bin.filebytes[bin.offsetOfMusicInstructionData + pos] & 0x0F) << 1);
                controlBytes[b] |= (byte)((bin.filebytes[bin.offsetOfMusicInstructionData + pos + 1] & 0x80) >> 7);
                b++;
                pos++;
                if (b == numchannels) { break; }

                controlBytes[b] = (byte)((bin.filebytes[bin.offsetOfMusicInstructionData + pos] & 0x7C) >> 2);
                b++;
                if (b == numchannels) { break; }

                controlBytes[b] = (byte)((bin.filebytes[bin.offsetOfMusicInstructionData + pos] & 0x03) << 3);
                controlBytes[b] |= (byte)((bin.filebytes[bin.offsetOfMusicInstructionData + pos + 1] & 0xE0) >> 5);
                b++;
                pos++;
                if (b == numchannels) { break; }

                controlBytes[b] = (byte)(bin.filebytes[bin.offsetOfMusicInstructionData + pos] & 0x1F);
                b++;
                pos++;
                if (b == numchannels) { break; }
            }


            pos = initialPos + 8;


            for (int i = 0; i < controlBytes.Length; i++)
                {
                if (controlBytes[i] == 0x00)
                {
                    controlBytes[i] = 0x80;
                    output.Add(controlBytes[i]);
                }
                else
                {
                    byte correctedControlByte = 0x80;

                    if ((controlBytes[i] & 0x10) == 0x10)  //note
                        {
                        correctedControlByte |= 0x01;
                        }
                    if ((controlBytes[i] & 0x08) == 0x08)  //instr
                    {
                        correctedControlByte |= 0x02;
                    }
                    if ((controlBytes[i] & 0x04) == 0x04)  //vol
                    {
                        correctedControlByte |= 0x04;
                    }
                    if ((controlBytes[i]  & 0x02) == 0x02)  //effect
                    {
                        correctedControlByte |= 0x08;
                    }
                    if ((controlBytes[i] & 0x01) == 0x01)  //effect params
                    {
                        correctedControlByte |= 0x10;
                    }

                    //add control byte to output
                    output.Add(correctedControlByte);

                    //look at its parameters and add those bytes too

                    byte mask = 0x80;

                    while (mask >= 1)
                    {
                        if ((controlBytes[i] & mask) == mask)
                        {
                            //add byte
                            output.Add(bin.filebytes[bin.offsetOfMusicInstructionData + pos]);
                            pos++;
                        }

                        mask = (byte)(mask >> 1);
                    }
                }
            }

            return output.ToArray();
        }



        public void ReadSongInfo() {

            if (filebytes.Length == 0)
                {
                return;
                }

            numchannels = filebytes[0];
            numpatterns = filebytes[1];
            number_of_patterns_in_one_loop = filebytes[2];

            restartPosition = filebytes[4];

            tempo = filebytes[5];
            bpm = filebytes[6];

            numinstruments = 0x80; //filebytes[9];

            int pos = 0x28;

            patterns = new List<Pattern>();

            patternsInPlayingOrder = new List<Pattern>();

            for (int i = 0; i < number_of_patterns_in_one_loop; i++)
                {

                if (GetPatternWithIndex(patterns,filebytes[pos]) == null)
                    {
                    Pattern newPattern = new Pattern();

                    newPattern.index = filebytes[pos];

                    patterns.Add(newPattern);

                    patternsInPlayingOrder.Add(newPattern);
                    }
                else
                    {
                    patternsInPlayingOrder.Add(GetPatternWithIndex(patterns,filebytes[pos]));
                    }
               
                pos++;
                }

            while (pos % 4 != 0)
                {
                pos++;
                }


            //start of the actual pattern info


            foreach(Pattern p in patterns)
                {
                p.number_of_rows = BitConverter.ToInt32(filebytes,pos);
                pos += 4;
            
                for (int i = 0; i < p.number_of_rows; i++)
                    {
                    p.rows.Add(GetRow(parentbinfile,BitConverter.ToInt32(filebytes, pos)));
                    p.patternSize += p.rows[p.rows.Count - 1].Length;

                    pos += 4;
                    }
                }
        }









        public List<Byte> MakeXM() {

            List<Byte> output = new List<byte>();

        foreach (char c in "Extended Module: ")
            {
                output.Add((byte)c);
            }

        foreach (char c in moduleName)
            {
                output.Add((byte)c);
            }

        while (output.Count < 0x25)
            {
                output.Add(0x00);
            }

            output.Add(0x1A);


            foreach (char c in "EPFExplorer")
            {
                output.Add((byte)c);
            }

            while (output.Count < 0x3A)
            {
                output.Add(0x00);
            }

            output.Add(0x04);
            output.Add(0x01);

            output.Add(0x14);
            output.Add(0x01);
            output.Add(0x00);
            output.Add(0x00);

            output.Add((byte)number_of_patterns_in_one_loop);
            output.Add((byte)(number_of_patterns_in_one_loop >> 8));

            output.Add((byte)restartPosition);
            output.Add((byte)(restartPosition >> 8));

            output.Add((byte)numchannels);
            output.Add((byte)(numchannels >> 8));

            output.Add((byte)numpatterns);
            output.Add((byte)(numpatterns >> 8));

            output.Add((byte)numinstruments);
            output.Add((byte)(numinstruments >> 8));

            output.Add(0x00);        //skip a bool that describes whether it has an amiga frequency table or not
            output.Add(0x00);

            output.Add((byte)tempo);
            output.Add((byte)(tempo >> 8));

            output.Add((byte)bpm);
            output.Add((byte)(bpm >> 8));

            for (int i = 0; i < patternsInPlayingOrder.Count; i++)
                {
                output.Add((byte)patternsInPlayingOrder[i].index);
                }

            while (output.Count < 0x150)
                {
                output.Add(0x00);
                }

            foreach (Pattern p in patterns)
                {
                output.Add(0x09);
                output.Add(0x00);
                output.Add(0x00);
                output.Add(0x00);

                output.Add(0x00);

                output.Add((byte)p.number_of_rows);
                output.Add((byte)(p.number_of_rows >> 8));

                output.Add((byte)(p.patternSize));
                output.Add((byte)((p.patternSize) >> 8));
                
                foreach (Byte[] row in p.rows)
                    {
                    foreach (Byte b in row)
                        {
                        output.Add(b);
                        }
                    }
                }

            //write instrument section

            string instrument_name = "instrument_";

            for (int i = 0; i < numinstruments; i++)
                {
                output.Add(0x11);
                output.Add(0x00);
                output.Add(0x00);
                output.Add(0x00);

                int namelength = 0;

                foreach (char c in (instrument_name + i.ToString()))
                    {
                    output.Add((byte)c);
                    namelength++;
                    }

                while (namelength < 0x16)
                    {
                    output.Add(0x00);
                    namelength++;
                    }

                output.Add(0x00);   //instrument type
                output.Add(0x00);   //number of samples. I'm hoping I can get away with this being zero...
                output.Add(0x00);
                }

           





            return output;
        }


    }
}
