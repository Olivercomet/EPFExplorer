﻿using System;
using System.Collections.Generic;
using System.IO;
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

        public List<InstrumentForImport> usedInstrumentsOnImport = new List<InstrumentForImport>();//only used when importing from an xm file

        public class InstrumentForImport
        {  //only used when importing from an xm file
            public byte ID;
            public byte MapOntoID;
            public List<sfxfile> samples = new List<sfxfile>();
            public string name;
        }

        public class Pattern
        {
            public int index;
            public int number_of_rows;
            public int patternSize;

            public List<Byte[]> rows = new List<Byte[]>();
        }

        public Pattern GetPatternWithIndex(List<Pattern> patternList, int index)
        {

            foreach (Pattern p in patternList)
            {
                if (p.index == index)
                {
                    return p;
                }
            }

            return null;
        }


        public Byte[] GetRow(binfile bin, int pos)
        {

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

            byte[] controlBytes = new byte[numchannels];

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
                            if ((controlBytes[i] & 0x06) == 0x06 && (mask == 0x04 || mask == 0x02))
                            {
                                if (mask == 0x04)
                                {
                                    output.Add(bin.filebytes[bin.offsetOfMusicInstructionData + pos + 1]);
                                }
                                else
                                {
                                    output.Add(bin.filebytes[bin.offsetOfMusicInstructionData + pos - 1]);
                                    effect = bin.filebytes[bin.offsetOfMusicInstructionData + pos - 1];
                                }
                            }
                            else if (mask == 0x01 && effect == 0x09)
                            {
                                output.Add((byte)(bin.filebytes[bin.offsetOfMusicInstructionData + pos] >> 1));
                            }
                            else if (mask == 0x01 && effect == 0x04)
                            {
                                output.Add((byte)((bin.filebytes[bin.offsetOfMusicInstructionData + pos] & 0xF0) | ((bin.filebytes[bin.offsetOfMusicInstructionData + pos] & 0x0F) >> 1)));
                            }
                            else
                            {
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

        public void ReadSongInfo()
        {

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
            unkOffset = BitConverter.ToInt32(filebytes, 13);

            int pos = 0x28;

            patterns = new List<Pattern>();

            patternsInPlayingOrder = new List<Pattern>();

            for (int i = 0; i < number_of_patterns_in_one_loop; i++)
            {

                if (GetPatternWithIndex(patterns, filebytes[pos]) == null)
                {
                    Pattern newPattern = new Pattern();

                    newPattern.index = filebytes[pos];

                    patterns.Add(newPattern);

                    patternsInPlayingOrder.Add(newPattern);
                }
                else
                {
                    patternsInPlayingOrder.Add(GetPatternWithIndex(patterns, filebytes[pos]));
                }

                pos++;
            }

            while (pos % 4 != 0)
            {
                pos++;
            }


            //start of the actual pattern info


            foreach (Pattern p in patterns)
            {
                p.number_of_rows = BitConverter.ToInt32(filebytes, pos);
                pos += 4;

                for (int i = 0; i < p.number_of_rows; i++)
                {
                    p.rows.Add(GetRow(parentbinfile, BitConverter.ToInt32(filebytes, pos)));
                    p.patternSize += p.rows[p.rows.Count - 1].Length;

                    pos += 4;
                }
            }
        }

        public List<Byte> MakeXM()
        {

            List<Byte> output = new List<byte>();

            foreach (char c in "Extended Module: ")
            {
                output.Add((byte)c);
            }

            for (int i = 0; i < Math.Min(name.Length - 3, 20); i++)
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

                foreach (byte[] row in p.rows)
                {
                    foreach (byte b in row)
                    {
                        output.Add(b);
                    }
                }
            }

            //write instrument section

            string instrument_name = "instrument_";
            string sample_name = "sample_";

            for (int i = 0; i < samples.Count; i++)
            {
                sfxfile sample = (i >= 0 && i < samples.Count && samples[i] != null) ? samples[i] : null;
                output.Add(252);
                output.Add(0);
                output.Add(0);
                output.Add(0);

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

                output.Add(0);
                output.Add(1);
                output.Add(0);

                output.Add(40);
                output.Add(0);
                output.Add(0);
                output.Add(0);
                for (int j = 0; j < 96; j++) output.Add(0);
                if (sample != null)
                {
                    for (int j = 0; j < 24; j++)
                    {
                        output.Add(sample != null ? (byte)(sample.volenv.nodes[j] & 0xFF) : (byte)0);
                        output.Add(sample != null ? (byte)(sample.volenv.nodes[j] >> 8) : (byte)0);
                    }
                    for (int j = 0; j < 24; j++)
                    {
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
                }
                else
                {
                    for (int j = 0; j < 96 + 16; j++) output.Add(0);
                }
                for (int j = 0; j < 11; j++) output.Add(0);
                short[] pcm = sample != null ? sample.ConvertToPCM() : new short[0] { };
                output.Add((byte)((pcm.Length * 2) & 0xFF));
                output.Add((byte)(((pcm.Length * 2) >> 8) & 0xFF));
                output.Add((byte)(((pcm.Length * 2) >> 16) & 0xFF));
                output.Add((byte)(((pcm.Length * 2) >> 24) & 0xFF));
                if (sample != null)
                {
                    output.Add((byte)((sample.loopstart * 4) & 0xFF));
                    output.Add((byte)(((sample.loopstart * 4) >> 8) & 0xFF));
                    output.Add((byte)(((sample.loopstart * 4) >> 16) & 0xFF));
                    output.Add((byte)(((sample.loopstart * 4) >> 24) & 0xFF));

                    output.Add((byte)(((sample.loopend * 4) - 0x10) & 0xFF));
                    output.Add((byte)(((sample.loopend * 4) >> 8) & 0xFF));
                    output.Add((byte)(((sample.loopend * 4) >> 16) & 0xFF));
                    output.Add((byte)(((sample.loopend * 4) >> 24) & 0xFF));
                }
                else
                {
                    for (int j = 0; j < 8; j++) output.Add(0);
                }
                output.Add(sample != null ? sample.defaultvol : (byte)0);
                output.Add(sample != null ? (byte)sample.finetune : (byte)0);
                output.Add((byte)(0x10 | (sample != null && sample.loopstart != 0xFFFFFFFF && sample.loopend != 0xFFFFFFFF ? 1 : 0)));
                output.Add(sample != null ? sample.defaultpan : (byte)0x80);
                output.Add(sample != null ? (byte)sample.transpose : (byte)0);
                output.Add(0);

                namelength = 0;

                foreach (char c in (sample_name + i.ToString()))
                {
                    output.Add((byte)c);
                    namelength++;
                }

                while (namelength < 0x16)
                {
                    output.Add(0x00);
                    namelength++;
                }

                short old = 0;
                for (int j = 0; j < pcm.Length; j++)
                {
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
                    File.WriteAllBytes(Path.Combine(customExportFolder, name), MakeXM().ToArray());
                    customExportFolder = "";
                }
            }
        }


        public void Replace_With_New_XM(string filename)
        {

            byte[] newXMfilebytes = File.ReadAllBytes(filename);

            patterns = new List<Pattern>();
            patternsInPlayingOrder = new List<Pattern>();

            // read XM from file

            number_of_patterns_in_one_loop = BitConverter.ToUInt16(newXMfilebytes, 0x40);
            restartPosition = BitConverter.ToUInt16(newXMfilebytes, 0x42);
            numchannels = BitConverter.ToUInt16(newXMfilebytes, 0x44);
            numpatterns = BitConverter.ToUInt16(newXMfilebytes, 0x46);
            numinstruments = BitConverter.ToUInt16(newXMfilebytes, 0x48);

            tempo = BitConverter.ToUInt16(newXMfilebytes, 0x4C);
            bpm = BitConverter.ToUInt16(newXMfilebytes, 0x4E);

            int pos = 0x50;

            for (int i = 0; i < numpatterns; i++)   //initialise as many patterns as we require
            {
                if (GetPatternWithIndex(patterns, i) == null)
                {
                    Pattern newPattern = new Pattern();

                    newPattern.index = i;

                    patterns.Add(newPattern);
                }
            }

            for (int i = 0; i < number_of_patterns_in_one_loop; i++)    //get playing order of patterns
            {
                if (GetPatternWithIndex(patterns, newXMfilebytes[pos]) != null)
                {
                    patternsInPlayingOrder.Add(GetPatternWithIndex(patterns, newXMfilebytes[pos]));
                }
                else
                {
                    Console.WriteLine("??? Playing order uses a pattern that doesn't exist: pattern " + newXMfilebytes[pos]);
                }
                pos++;
            }

            while (newXMfilebytes[pos] == 0x00)
            {
                pos++;
            }

            //start of pattern data

            usedInstrumentsOnImport = new List<InstrumentForImport>();

            for (int i = 0; i < numpatterns; i++)
            {
                pos += 5;

                patterns[i].number_of_rows = BitConverter.ToUInt16(newXMfilebytes, pos);
                pos += 2;
                patterns[i].patternSize = BitConverter.ToUInt16(newXMfilebytes, pos);
                pos += 2;

                patterns[i].rows = new List<byte[]>();

                if (patterns[i].patternSize == 0)
                { //if pattern size is zero, do this instead
                    List<byte> blankRow = new List<byte>();
                    for (int b = 0; b < numchannels; b++)
                    {
                        blankRow.Add(0x80);
                    }
                    for (int r = 0; r < patterns[i].number_of_rows; r++)
                    {
                        patterns[i].rows.Add(blankRow.ToArray());
                    }
                    continue;
                }

                for (int r = 0; r < patterns[i].number_of_rows; r++)        //add rows
                {
                    patterns[i].rows.Add(GetRegularXMRow(newXMfilebytes, pos));
                    pos += patterns[i].rows[patterns[i].rows.Count - 1].Length;
                    ConvertRowToEPFFormat(patterns[i].rows[patterns[i].rows.Count - 1]);   //This isn't actually converting the row right at this moment. It's so that we find out which instruments need to be loaded.
                }
            }

            /*
            samples = new List<sfxfile>();

            for (int i = 0; i < numinstruments; i++)
            {
                Console.WriteLine("pos is " + pos);
                InstrumentForImport newInstrument = new InstrumentForImport();

                newInstrument.ID = (byte)i;
                newInstrument.MapOntoID = (byte)i;

                pos += 4;

                for (int c = 0; c < 0x16; c++)
                {
                    if ((char)newXMfilebytes[pos] != 0x00)
                    {
                        newInstrument.name += (char)newXMfilebytes[pos];
                    }
                    pos++;
                }

                Console.WriteLine("end of instr name " + pos);

                pos++;

                int num_samples_in_this_instrument = BitConverter.ToInt16(newXMfilebytes,pos);
                pos += 2;

                pos += 100;

                Console.WriteLine("start of vol env " + pos);


                sfxfile.envelope volenv = new sfxfile.envelope();
                    sfxfile.envelope panenv = new sfxfile.envelope();

                    for (int j = 0; j < 24; j++)
                        {
                        volenv.nodes[j] = (short)(newXMfilebytes[pos] | (newXMfilebytes[pos+1] << 8));
                        pos += 2;
                        }

                pos += 16;

                Console.WriteLine("start of pan env " + pos);

                    for (int j = 0; j < 24; j++)
                        {
                        panenv.nodes[j] = (short)(newXMfilebytes[pos] | (newXMfilebytes[pos + 1] << 8));
                        pos += 2;
                        }

                pos += 16;

                Console.WriteLine("end of panning envelope "+pos);

                    volenv.count = newXMfilebytes[pos];
                    pos++;
                    panenv.count = newXMfilebytes[pos];
                    pos++;

                    volenv.sustainPoint = newXMfilebytes[pos] != 0 ? newXMfilebytes[pos] : (byte)0xFF;
                    pos++;
                    volenv.loopStart = newXMfilebytes[pos] != 0 ? newXMfilebytes[pos] : (byte)0xFF;
                    pos++;
                    volenv.loopEnd = newXMfilebytes[pos] != 0 ? newXMfilebytes[pos] : (byte)0xFF;
                    pos++;

                    panenv.sustainPoint = newXMfilebytes[pos] != 0 ? newXMfilebytes[pos] : (byte)0xFF;
                    pos++;
                    panenv.loopStart = newXMfilebytes[pos] != 0 ? newXMfilebytes[pos] : (byte)0xFF;
                    pos++;
                    panenv.loopEnd = newXMfilebytes[pos] != 0 ? newXMfilebytes[pos] : (byte)0xFF;
                    pos++;

                    pos++; //skip over volume byte
                    pos++; //skip over pan byte

                    pos += 6;


                for (int s = 0; s < num_samples_in_this_instrument; s++)
                {
                    Console.WriteLine("start of sample header at " + pos);
                    sfxfile newSample = new sfxfile();

                    newSample.volenv = volenv;
                    newSample.panenv = panenv;

                    pos += 11;

                    int sampleLength = BitConverter.ToInt32(newXMfilebytes, pos);
                    pos += 4;

                    short[] pcm = new short[sampleLength];

                    newSample.loopstart = BitConverter.ToUInt32(newXMfilebytes, pos) / 4;
                    pos += 4;

                    newSample.loopend = BitConverter.ToUInt32(newXMfilebytes, pos) / 4;
                    pos += 4;

                    newSample.defaultvol = newXMfilebytes[pos];
                    pos++;

                    int finetune = newXMfilebytes[pos];
                    if (finetune > 0x7F) {
                        finetune -= 256;
                    }
                    newSample.finetune = (sbyte)finetune;
                    pos++;

                    pos++; //skip sample type

                    newSample.defaultpan = newXMfilebytes[pos];
                    pos++;

                    int transpose = newXMfilebytes[pos];
                    if (transpose > 0x7F)
                    {
                        transpose -= 256;
                    }
                    newSample.transpose = (sbyte)transpose;
                    pos++;

                    pos++;

                    newSample.name = "";

                    for (int c = 0; c < 0x16; c++)
                    {
                        if ((char)newXMfilebytes[pos] != 0x00)
                        {
                            newSample.name += (char)newXMfilebytes[pos];
                        }
                        pos++;
                    }

                    short old = 0;
                    for (int j = 0; j < sampleLength * 2; j++)
                    {
                        short diff = BitConverter.ToInt16(newXMfilebytes, pos);
                        pos += 2;

                        pcm[j] = (short)(old + diff);
                        old = pcm[j];
                    }

                    newSample.isPCM = true;
                    newInstrument.samples.Add(newSample);
                }

            foreach (InstrumentForImport instr in usedInstrumentsOnImport) {    //if this instrument we've just processed is actually used, then update it in the list, otherwise leave it out

                    if (instr.ID == i) {
                        usedInstrumentsOnImport[i] = newInstrument;
                        break;
                    }
                }

                if (usedInstrumentsOnImport[i] != newInstrument) {  //null the instrument if we decided it shouldn't be in the list
                    newInstrument = null;
                }
            }

             if (samples.Count < parentbinfile.samplecount)  //if the sample count is less than the sample count of the entire game, assume it's a custom XM and that it requires instrument remapping onto the game's instruments
             {
                 InstrumentMappingForm instrumentMappingForm = new InstrumentMappingForm();

                instrumentMappingForm.InitializeLists(this);

                 instrumentMappingForm.Show();
            }*/
        }

        public byte[] GetRegularXMRow(byte[] bytes, int pos)
        {

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


        public List<byte> MakeEPFXMHeader()
        {

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

            byte[] ControlBytes = new byte[number_of_bytes_needed_for_control_bytes];

            int current_byte_in_controlbytes = 0;
            int current_bit_in_byte = 0;

            int pos = 0;

            for (int channel = 0; channel < numchannels; channel++)
            {
                List<byte> hasTheseFlags = new List<Byte>();

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
                else if ((input[pos] & 0x80) == 0x80)  //if it's a compressed note, with the different bits telling us which sections are included
                {
                    byte[] controlBitsInOrderTheyShouldBeProcessed = new byte[] { 0x01, 0x02, 0x08, 0x04, 0x10 };

                    for (int i = 0; i < controlBitsInOrderTheyShouldBeProcessed.Length; i++)       //convert control byte to EPF format and add to bit list
                    {
                        byte mask = controlBitsInOrderTheyShouldBeProcessed[i];

                        if ((input[pos] & mask) == mask)    //If the bit was there, put it in the EPF format one too, in the available bit position. If not, then the available bit position is left as zero.
                        {
                            hasTheseFlags.Add(mask);

                            ControlBytes[current_byte_in_controlbytes] |= (byte)(0x01 << (7 - current_bit_in_byte));
                            number_of_data_bytes++;
                        }

                        current_bit_in_byte++;

                        if (current_bit_in_byte > 7)
                        { current_byte_in_controlbytes++; current_bit_in_byte = 0; }
                    }

                    pos++;
                }
                else   //if it's an uncompressed note with all sections included
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


                for (int i = 0; i < number_of_data_bytes; i++)  //and now just correct some things
                {
                    //make a note of the instruments used
                    if (i == hasTheseFlags.IndexOf(0x02))
                    {
                        bool alreadyProcessedInstrument = false;

                        foreach (InstrumentForImport instr in usedInstrumentsOnImport)
                        {
                            if (instr.ID == data[(data.Count - number_of_data_bytes) + i])
                            {
                                alreadyProcessedInstrument = true;
                                break;
                            }
                        }

                        if (!alreadyProcessedInstrument)
                        {
                            usedInstrumentsOnImport.Add(new InstrumentForImport() { ID = data[(data.Count - number_of_data_bytes) + i] });
                        }
                    }
                    //swap the two pesky bytes that show up in the wrong order (if they are both there)
                    else if (i == hasTheseFlags.IndexOf(0x08) && (i + 1 == hasTheseFlags.IndexOf(0x04)))
                    {
                        byte temp = data[(data.Count - number_of_data_bytes) + i];
                        data[(data.Count - number_of_data_bytes) + i] = data[(data.Count - number_of_data_bytes) + i + 1];
                        data[(data.Count - number_of_data_bytes) + i + 1] = temp;
                    }
                }
            }

            List<byte> output = new List<byte>();
            output.AddRange(ControlBytes);
            output.AddRange(data);

            return output.ToArray();
        }
    }
}