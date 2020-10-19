using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPFExplorer   
{
    public class xmfile        
    {
        public uint offset;
        public Byte[] filebytes;
        public uint size;

        public string name;

        public binfile parentbinfile;

        public int number_of_patterns_in_one_loop;
        public int restartPosition;

        public int numchannels;
        public int numpatterns;
        public int numinstruments;

        public int clipvaluethingy;

        public int unkValue1;
        public int unkValue2;
        public int unkValue3;
        public int unkOffset;

        public int tempo;
        public int bpm;

        public List<Pattern> patterns = new List<Pattern>();
        public List<Pattern> patternsInPlayingOrder = new List<Pattern>();
        public List<sfxfile> samples = new List<sfxfile>();

        public string customExportFolder = "";

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
            bool loopStart = true;

            while (b < numchannels)
            {
                controlBytes[b] = (byte)((bin.filebytes[bin.offsetOfMusicInstructionData + pos] & 0xF8) >> 3);
                b++;
                loopStart = false;
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
                loopStart = true;
                if (b == numchannels) { break; }
            }

            if (!loopStart) pos++;

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
                    if ((controlBytes[i] & 0x04) == 0x04)  //effect
                    {
                        correctedControlByte |= 0x08;
                    }
                    if ((controlBytes[i] & 0x02) == 0x02)  //vol
                    {
                        correctedControlByte |= 0x04;
                    }
                    if ((controlBytes[i] & 0x01) == 0x01)  //effect params
                    {
                        correctedControlByte |= 0x10;
                    }

                    //add control byte to output
                    output.Add(correctedControlByte);

                    //look at its parameters and add those bytes too

                    byte mask = 0x80;
                    byte effect = 0xFF;

                    while (mask >= 1)
                    {
                        if ((controlBytes[i] & mask) == mask)
                        {
                            //add byte
                            if ((controlBytes[i] & 0x06) == 0x06 && (mask == 0x04 || mask == 0x02)) {
                                if (mask == 0x04) {
                                    output.Add(bin.filebytes[bin.offsetOfMusicInstructionData + pos + 1]);
                                } else {
                                    output.Add(bin.filebytes[bin.offsetOfMusicInstructionData + pos - 1]);
                                    effect = bin.filebytes[bin.offsetOfMusicInstructionData + pos - 1];
                                }
                            } else if (mask == 0x01 && effect == 0x09) {
                                output.Add((byte)(bin.filebytes[bin.offsetOfMusicInstructionData + pos] >> 1));
                            } else if (mask == 0x01 && effect == 0x04) {
                                output.Add((byte)((bin.filebytes[bin.offsetOfMusicInstructionData + pos] & 0xF0) | ((bin.filebytes[bin.offsetOfMusicInstructionData + pos] & 0x0F) >> 1)));
                            } else {
                                output.Add(bin.filebytes[bin.offsetOfMusicInstructionData + pos]);
                                if (mask == 0x04) effect = bin.filebytes[bin.offsetOfMusicInstructionData + pos];
                            }
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

            clipvaluethingy = filebytes[8];
            numinstruments = filebytes[9];
            unkValue1 = filebytes[10];
            unkValue2 = filebytes[11];
            unkValue3 = filebytes[12];
            unkOffset = BitConverter.ToInt32(filebytes,0x13);

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

            for (int i = 0; i < Math.Min(name.Length-3, 20); i++)
            {
                output.Add((byte)name[i]);
            }

            while (output.Count < 0x25)
            {
                output.Add(0x20);
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

            output.Add((byte)parentbinfile.samplecount);
            output.Add((byte)(parentbinfile.samplecount >> 8));

            output.Add(0x01);     
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
            string sample_name = "sample_";

            for (int i = 0; i < samples.Count; i++) {
                sfxfile sample = (i >= 0 && i < samples.Count && samples[i] != null) ? samples[i] : null;
                output.Add(252);
                output.Add(0);
                output.Add(0);
                output.Add(0);

                int namelength = 0;

                foreach (char c in (instrument_name + i.ToString())) {
                    output.Add((byte)c);
                    namelength++;
                }

                while (namelength < 0x16) {
                    output.Add(0x00);
                    namelength++;
                }

                output.Add(0);
                output.Add(1);
                output.Add(0);

                output.Add(40);
                output.Add(0);
                output.Add(0);
                output.Add(0);
                for (int j = 0; j < 96; j++) output.Add(0);
                if (sample != null) {
                    for (int j = 0; j < 24; j++) {
                        output.Add(sample != null ? (byte)(sample.volenv.nodes[j] & 0xFF) : (byte)0);
                        output.Add(sample != null ? (byte)(sample.volenv.nodes[j] >> 8) : (byte)0);
                    }
                    for (int j = 0; j < 24; j++) {
                        output.Add(sample != null ? (byte)(sample.panenv.nodes[j] & 0xFF) : (byte)0);
                        output.Add(sample != null ? (byte)(sample.panenv.nodes[j] >> 8) : (byte)0);
                    }
                    output.Add((byte)sample.volenv.count);
                    output.Add((byte)sample.panenv.count);
                    output.Add(sample.volenv.sustainPoint != 0xFF ? sample.volenv.sustainPoint : (byte)0);
                    output.Add(sample.volenv.loopStart != 0xFF ? sample.volenv.loopStart : (byte)0);
                    output.Add(sample.volenv.loopEnd != 0xFF ? sample.volenv.loopEnd : (byte)0);
                    output.Add(sample.panenv.sustainPoint != 0xFF ? sample.panenv.sustainPoint : (byte)0);
                    output.Add(sample.panenv.loopStart != 0xFF ? sample.panenv.loopStart : (byte)0);
                    output.Add(sample.panenv.loopEnd != 0xFF ? sample.panenv.loopEnd : (byte)0);
                    output.Add((byte)(
                        (sample.volenv.count > 0 ? 1 : 0) |
                        (sample.volenv.sustainPoint < sample.volenv.count ? 2 : 0) |
                        (sample.volenv.loopStart < sample.volenv.count && sample.volenv.loopEnd < sample.volenv.count ? 4 : 0)
                        ));
                    output.Add((byte)(
                        (sample.panenv.count > 0 ? 1 : 0) |
                        (sample.panenv.sustainPoint < sample.panenv.count ? 2 : 0) |
                        (sample.panenv.loopStart < sample.panenv.count && sample.panenv.loopEnd < sample.panenv.count ? 4 : 0)
                        ));
                    output.Add(0);
                    output.Add(0);
                    output.Add(0);
                    output.Add(0);
                    output.Add(0);
                    output.Add(0);
                } else {
                    for (int j = 0; j < 96 + 16; j++) output.Add(0);
                }
                for (int j = 0; j < 11; j++) output.Add(0);
                short[] pcm = sample != null ? sample.ConvertToPCM() : new short[0] { };
                output.Add((byte)((pcm.Length * 2) & 0xFF));
                output.Add((byte)(((pcm.Length * 2) >> 8) & 0xFF));
                output.Add((byte)(((pcm.Length * 2) >> 16) & 0xFF));
                output.Add((byte)(((pcm.Length * 2) >> 24) & 0xFF));
                if (sample != null) {
                    output.Add((byte)((sample.loopstart * 4) & 0xFF));
                    output.Add((byte)(((sample.loopstart * 4) >> 8) & 0xFF));
                    output.Add((byte)(((sample.loopstart * 4) >> 16) & 0xFF));
                    output.Add((byte)(((sample.loopstart * 4) >> 24) & 0xFF));
                    output.Add((byte)((( sample.loopend) * 4) & 0xFF));
                    output.Add((byte)((((sample.loopend) * 4) >> 8) & 0xFF));
                    output.Add((byte)((((sample.loopend) * 4) >> 16) & 0xFF));
                    output.Add((byte)((((sample.loopend) * 4) >> 24) & 0xFF));
                } else {
                    for (int j = 0; j < 8; j++) output.Add(0);
                }
                output.Add(sample != null ? sample.defaultvol : (byte)0);
                output.Add(sample != null ? (byte)sample.finetune : (byte)0);
                output.Add((byte)(0x10 | (sample != null && sample.loopstart != 0xFFFFFFFF && sample.loopend != 0xFFFFFFFF ? 1 : 0)));
                output.Add(sample != null ? sample.defaultpan : (byte)0x80);
                output.Add(sample != null ? (byte)sample.transpose : (byte)0);
                output.Add(0);

                namelength = 0;

                foreach (char c in (sample_name + i.ToString())) {
                    output.Add((byte)c);
                    namelength++;
                }

                while (namelength < 0x16) {
                    output.Add(0x00);
                    namelength++;
                }

                short old = 0;
                for (int j = 0; j < pcm.Length; j++) {
                    short n = (short)(pcm[j] - old);
                    output.Add((byte)(n & 0xFF));
                    output.Add((byte)((n >> 8) & 0xFF));
                    old = pcm[j];
                }
            }



            return output;
        }



        public void Export()
        {

            if (filebytes != null)
            {
                if (customExportFolder == "" || customExportFolder == null)
                    {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.FileName = name;

                    saveFileDialog1.Title = "Save XM file";
                    saveFileDialog1.CheckPathExists = true;
                    saveFileDialog1.Filter = "Extended Module (*.xm)|*.xm|All files (*.*)|*.*";

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                        File.WriteAllBytes(saveFileDialog1.FileName, MakeXM().ToArray());
                        }
                    }
                else
                    {
                    File.WriteAllBytes(Path.Combine(customExportFolder,name), MakeXM().ToArray());
                    customExportFolder = "";
                    }
                
            }
        }


        public void Replace_With_New_XM (string filename) {

            byte[] newXMfilebytes = File.ReadAllBytes(filename);

            patterns = new List<Pattern>();
            patternsInPlayingOrder = new List<Pattern>();

            // read XM from file

            number_of_patterns_in_one_loop = BitConverter.ToUInt16(newXMfilebytes,0x40);
            restartPosition = BitConverter.ToUInt16(newXMfilebytes, 0x42);
            numchannels = BitConverter.ToUInt16(newXMfilebytes, 0x44);
            numpatterns = BitConverter.ToUInt16(newXMfilebytes, 0x46);
            numinstruments = BitConverter.ToUInt16(newXMfilebytes, 0x48);
     
            tempo = BitConverter.ToUInt16(newXMfilebytes, 0x4C);
            bpm = BitConverter.ToUInt16(newXMfilebytes, 0x4E);

            int pos = 0x50;

            for (int i = 0; i < number_of_patterns_in_one_loop; i++)
            {
                if (GetPatternWithIndex(patterns, newXMfilebytes[pos]) == null)
                {
                    Pattern newPattern = new Pattern();

                    newPattern.index = newXMfilebytes[pos];

                    patterns.Add(newPattern);

                    patternsInPlayingOrder.Add(newPattern);
                }
                else
                {
                    patternsInPlayingOrder.Add(GetPatternWithIndex(patterns, newXMfilebytes[pos]));
                }
                pos++;
            }

            while(newXMfilebytes[pos] == 0x00)
                {
                pos++;
                }

            //start of pattern data

            for (int i = 0; i < numpatterns; i++)
                {
                pos += 5;

                patterns[i].number_of_rows = BitConverter.ToUInt16(newXMfilebytes,pos);
                pos += 2;
                patterns[i].patternSize = BitConverter.ToUInt16(newXMfilebytes, pos);
                pos += 2;

                patterns[i].rows = new List<byte[]>();

                for (int r = 0; r < patterns[i].number_of_rows; r++)        //add rows
                    {
                    patterns[i].rows.Add(GetRegularXMRow(newXMfilebytes,pos));
                    pos += patterns[i].rows[patterns[i].rows.Count - 1].Length;
                    }
                }
        }


        public byte[] GetRegularXMRow(byte[] bytes, int pos) {

            List<byte> output = new List<byte>();

            int channelsProcessed = 0;
        
            while (channelsProcessed < numchannels)
                {
                if ((bytes[pos] & 0x80) == 0x80)    //if it's a compressed command
                    {
                    //then check the number of compressed bytes that follow and add those
                    int number_of_bytes_to_use = 0;

                    for (int mask = 0x01; mask <= 0x10; mask *= 2)
                        {
                        if ((bytes[pos] & mask) == mask)
                            {
                            number_of_bytes_to_use++;
                            }
                        }

                    output.Add(bytes[pos]);
                    pos++;

                    for (int i = 0; i < number_of_bytes_to_use; i++)
                        {
                        output.Add(bytes[pos]);
                        pos++;
                        }
                    }
                else
                    {
                    //but if it's not a compressed command, just add five bytes
                    for (int i = 0; i < 5; i++)
                        {
                        output.Add(bytes[pos]);
                        pos++;
                        }
                    }
                channelsProcessed++;
                }

            return output.ToArray();
        }


        public List<byte> MakeEPFXMHeader() {

            List<byte> output = new List<byte>();

            output.Add((byte)numchannels);              //offset 0
            output.Add((byte)numpatterns);                  //offset 1
            output.Add((byte)number_of_patterns_in_one_loop);   //offset 2
            output.Add(0x00);   //idk what this is!
            output.Add((byte)restartPosition);          //offset 4
            output.Add((byte)tempo);            //offset 5
            output.Add((byte)bpm);                  //offset 6
            output.Add(0x00);   //idk what this is!
            output.Add((byte)clipvaluethingy);  //offset 8
            output.Add((byte)numinstruments);   //offset 9
            output.Add((byte)unkValue1);
            output.Add((byte)unkValue2);
            output.Add((byte)unkValue3);
            output.Add((byte)unkOffset);
            output.Add((byte)(unkOffset >> 8));
            output.Add((byte)(unkOffset >> 16));
            output.Add((byte)(unkOffset >> 24));

            while (output.Count < 0x28)
                {
                output.Add(0x00);
                }

            foreach (Pattern p in patternsInPlayingOrder)
                {
                output.Add((byte)p.index);
                }

            while (output.Count % 4 != 0)
                {
                output.Add(0x00);
                }

            return output;
        }


        public byte[] ConvertRowToEPFFormat(byte[] input)
            {
            List<byte> data = new List<byte>();

            int number_of_bytes_needed_for_control_bytes = (numchannels * 5);   //not done yet

            while (number_of_bytes_needed_for_control_bytes % 8 != 0)
                {
                number_of_bytes_needed_for_control_bytes++;
                }

            number_of_bytes_needed_for_control_bytes /= 8;  //now it is done

            Byte[] ControlBytes = new byte[number_of_bytes_needed_for_control_bytes];

            int current_byte_in_controlbytes = 0;
            int current_bit_in_byte = 0;

            int pos = 0;

            if(name.Contains("Command") && input.Length == 31 && input[0] == 136)
                {
                Console.WriteLine("breakpoint");
                }    

            for (int channel = 0; channel < numchannels; channel++)
                {
                int number_of_data_bytes = 0;

                if (input[pos] == 0x80) //if it's just a blank note
                    {
                    for (int i = 0; i < 5; i++) //write zeroes to the bit list
                        {
                        current_bit_in_byte++;
                        if (current_bit_in_byte > 7)
                            { current_byte_in_controlbytes++; current_bit_in_byte = 0; }
                        }
                    pos++;
                    continue;   //and go to the next control byte in the input
                    }

                if ((input[pos] & 0x80) == 0x80)
                {
                    for (int mask = 0x01; mask <= 0x10; mask *= 2)       //convert control byte to EPF format and add to bit list
                    {
                        if ((input[pos] & mask) == mask)    //check note/instr/vol/effect/effect params
                        {
                            ControlBytes[current_byte_in_controlbytes] |= (byte)(1 << (7 - current_bit_in_byte));
                            number_of_data_bytes++;
                        }
                        current_bit_in_byte++;

                        if (current_bit_in_byte > 7)
                        { current_byte_in_controlbytes++; current_bit_in_byte = 0; }
                    }

                    pos++;
                }
                else
                {
                    for (int i = 0; i < 5; i++) //write ones to the bit list
                    {
                        ControlBytes[current_byte_in_controlbytes] |= (byte)(1 << (7 - current_bit_in_byte));
                        current_bit_in_byte++;
                        if (current_bit_in_byte > 7)
                        { current_byte_in_controlbytes++; current_bit_in_byte = 0; }
                    }
                    number_of_data_bytes = 5;
                }

                //now add its data bytes
               
                for (int i = 0; i < number_of_data_bytes; i++)
                    {
                    data.Add(input[pos]);
                    pos++;
                    }
                }

            List<byte> output = new List<byte>();
            output.AddRange(ControlBytes);
            output.AddRange(data);

            return output.ToArray();
            }



    }
}
