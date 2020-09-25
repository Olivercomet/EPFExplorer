using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPFExplorer   
{
    class xmfile        
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

            Byte[] output = new byte[0x10];

            if (pos == -1)  //idk what this means at the moment, so I'm just going to return a blank array. it might mean repeat the previous one?
                 {
                 return output;
                 }

                for (int i = 0; i < 0x10; i++)
                {
                output[i] = bin.filebytes[bin.offsetOfMusicInstructionData + pos + i];
                }

            return output;
        }



        public void ReadSongInfo() {

            if (filebytes.Length == 0)
                {
                return;
                }

            numchannels = 1; // filebytes[0];
            numpatterns = filebytes[1];
            number_of_patterns_in_one_loop = filebytes[2];

            restartPosition = filebytes[4];

            tempo = filebytes[5];
            bpm = filebytes[6];

            numinstruments = filebytes[9];

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
