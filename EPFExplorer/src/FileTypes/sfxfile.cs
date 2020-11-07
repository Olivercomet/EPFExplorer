using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPFExplorer
{
    public class sfxfile
    {
        public class envelope {
            public short[] nodes = new short[24];
            public int count;
            public byte sustainPoint;
            public byte loopStart;
            public byte loopEnd;
        }

        public uint offset;
        public byte[] filebytes;
        public uint filemagic;
        public uint sizedividedby4;
        public uint samplerate;
        public sbyte finetune = 0;
        public sbyte transpose = 0x11;
        public byte defaultvol = 0x40;
        public byte defaultpan = 0x80;
        public uint loopstart;
        public uint loopend;
        public bool isPCM = false;
        public envelope volenv = new envelope();
        public envelope panenv = new envelope();

        public int indexInBin;

        public string customExportFolder = "";
        public static T Clamp<T>(T val, T min, T max) where T : IComparable<T> {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public binfile parentbinfile;

        private static int[] ima_index_table = new int[]{
              -1, -1, -1, -1, 2, 4, 6, 8,
              -1, -1, -1, -1, 2, 4, 6, 8
            };

        private static short[] ima_step_table = new short[]{
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

        public short[] ConvertToPCM() {
            if (isPCM) {
                short[] output = new short[filebytes.Length / 2];
                for (int i = 0; i < filebytes.Length / 2; i++) {
                    output[i] = (short)(filebytes[i * 2] | (filebytes[i * 2 + 1] << 8));
                }
                return output;
            } else {
                List<short> output = new List<short>();
                int predictor = (short)(filebytes[0] | (filebytes[1] << 8));
                int step_index = filebytes[2], step;
                for (int i = 4; i < filebytes.Length; i++) {
                    int diff;
                    byte nl = (byte)(filebytes[i] & 0x0f);
                    step = ima_step_table[step_index];
                    step_index = Clamp(step_index + ima_index_table[nl], 0, 88);
                    diff = step >> 3;
                    if ((nl & 4) != 0) diff += step;
                    if ((nl & 2) != 0) diff += (step >> 1);
                    if ((nl & 1) != 0) diff += (step >> 2);
                    if ((nl & 8) != 0) predictor = Clamp(predictor - diff, -32768, 32767);
                    else predictor = Clamp(predictor + diff, -32768, 32767);
                    output.Add((short)predictor);

                    nl = (byte)((filebytes[i] & 0xf0) >> 4);
                    step = ima_step_table[step_index];
                    step_index = Clamp(step_index + ima_index_table[nl], 0, 88);
                    diff = step >> 3;
                    if ((nl & 4) != 0) diff += step;
                    if ((nl & 2) != 0) diff += (step >> 1);
                    if ((nl & 1) != 0) diff += (step >> 2);
                    if ((nl & 8) != 0) predictor = Clamp(predictor - diff, -32768, 32767);
                    else predictor = Clamp(predictor + diff, -32768, 32767);
                    output.Add((short)predictor);
                }
                return output.ToArray();
            }
        }

        public void Export() {

            if (filebytes != null)
            {
                string name = Path.GetFileName(parentbinfile.filename) + offset;
                if (customExportFolder == "" || customExportFolder == null)
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.FileName = Path.GetFileName(parentbinfile.filename) + indexInBin + ".wav";

                    saveFileDialog1.Title = "Save .wav file";
                    saveFileDialog1.CheckPathExists = true;
                    saveFileDialog1.Filter = isPCM ? "PCM WAV (*.wav)|*.wav|All files (*.*)|*.*" : "ADPCM WAV (*.wav)|*.wav|All files (*.*)|*.*";

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                        ConvertToWAV();
                        File.WriteAllBytes(saveFileDialog1.FileName, filebytes);
                        }
                }
                else
                {
                    ConvertToWAV();
                    File.WriteAllBytes(Path.Combine(customExportFolder, name + ".wav"), filebytes);
                    customExportFolder = "";
                }
            }
        }

        public void ConvertToWAV() {

            byte[] output = new byte[(isPCM ? 0x2C : 0x3C) + filebytes.Length];

            output[0] = (byte)'R';
            output[1] = (byte)'I';
            output[2] = (byte)'F';
            output[3] = (byte)'F';

            uint riffChunkSize = (uint)(output.Length - 8);

            output[4] = (byte)riffChunkSize;  //riff chunk size
            output[5] = (byte)(riffChunkSize >> 8);
            output[6] = (byte)(riffChunkSize >> 16);
            output[7] = (byte)(riffChunkSize >> 24);

            output[8] = (byte)'W';
            output[9] = (byte)'A';
            output[0x0A] = (byte)'V';
            output[0x0B] = (byte)'E';

            output[0x0C] = (byte)'f';
            output[0x0D] = (byte)'m';
            output[0x0E] = (byte)'t';
            output[0x0F] = (byte)' ';

            int dataOffset;

            if (isPCM) {
                output[0x10] = 0x10;    //fmt chunk length
                output[0x11] = 0x00;
                output[0x12] = 0x00;
                output[0x13] = 0x00;

                output[0x14] = 0x01;    //ADPCM
                output[0x15] = 0x00;

                output[0x16] = 0x01;    //num channels
                output[0x17] = 0x00;

                output[0x18] = (byte)samplerate;    //sample rate
                output[0x19] = (byte)(samplerate >> 8);
                output[0x1A] = (byte)(samplerate >> 16);
                output[0x1B] = (byte)(samplerate >> 24);

                uint samplecount = (uint)(filebytes.Length * 2);

                float LengthInSeconds = (float)((float)samplecount / (float)samplerate);

                uint datarate = (uint)Math.Round((float)filebytes.Length / LengthInSeconds);

                output[0x1C] = (byte)datarate;    //data rate
                output[0x1D] = (byte)(datarate >> 8);
                output[0x1E] = (byte)(datarate >> 16);
                output[0x1F] = (byte)(datarate >> 24);

                output[0x20] = 0x02;    //data block size
                output[0x21] = 0x00;

                output[0x22] = 0x10;    //bits per sample
                output[0x23] = 0x00;

                dataOffset = 0x24;
            } else {
                output[0x10] = 0x14;    //fmt chunk length
                output[0x11] = 0x00;
                output[0x12] = 0x00;
                output[0x13] = 0x00;

                output[0x14] = 0x11;    //ADPCM
                output[0x15] = 0x00;

                output[0x16] = 0x01;    //num channels
                output[0x17] = 0x00;

                output[0x18] = (byte)samplerate;    //sample rate
                output[0x19] = (byte)(samplerate>>8);
                output[0x1A] = (byte)(samplerate>>16);
                output[0x1B] = (byte)(samplerate>>24);

                uint samplecount = (uint)(filebytes.Length * 2);

                float LengthInSeconds = (float)((float)samplecount / (float)samplerate);

                uint datarate = (uint)Math.Round((float)filebytes.Length / LengthInSeconds);

                output[0x1C] = (byte)datarate;    //data rate
                output[0x1D] = (byte)(datarate >> 8);
                output[0x1E] = (byte)(datarate >> 16);
                output[0x1F] = (byte)(datarate >> 24);

                output[0x20] = 0xFF;    //data block size apparently, but this works
                output[0x21] = 0xFF;

                output[0x22] = 0x04;    //bits per sample
                output[0x23] = 0x00;

                output[0x24] = 0x02;    //idk
                output[0x25] = 0x00;

                output[0x26] = 0xF9;    //idk
                output[0x27] = 0x01;

                output[0x28] = (byte)'f';
                output[0x29] = (byte)'a';
                output[0x2A] = (byte)'c';
                output[0x2B] = (byte)'t';

                output[0x2C] = 0x04;
                output[0x2D] = 0x00;
                output[0x2E] = 0x00;
                output[0x2F] = 0x00;

                output[0x30] = (byte)samplecount;  //sample count
                output[0x31] = (byte)(samplecount >> 8);
                output[0x32] = (byte)(samplecount >> 16);
                output[0x33] = (byte)(samplecount >> 24);

                dataOffset = 0x34;
            }

            output[dataOffset] = (byte)'d';
            output[dataOffset+1] = (byte)'a';
            output[dataOffset+2] = (byte)'t';
            output[dataOffset+3] = (byte)'a';

            output[dataOffset+4] = (byte)filebytes.Length; //data size (aka, the size of filebytes from before)
            output[dataOffset+5] = (byte)(filebytes.Length >> 8);
            output[dataOffset+6] = (byte)(filebytes.Length >> 16);
            output[dataOffset+7] = (byte)(filebytes.Length >> 24);

            Array.Copy(filebytes, 0, output, dataOffset+8, filebytes.Length);   //copy ADPCM data into wav

            filebytes = output;
        }
    }
}
