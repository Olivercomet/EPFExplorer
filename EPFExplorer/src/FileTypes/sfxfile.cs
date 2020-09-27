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

        public uint offset;
        public byte[] filebytes;
        public uint filemagic;
        public uint sizedividedby4;
        public uint samplerate;
        public uint unk1;

        public binfile parentbinfile;


        public void Export() {

            if (filebytes != null)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.FileName = Path.GetFileName(parentbinfile.filename) + offset + ".wav";

                saveFileDialog1.Title = "Save .wav file";
                saveFileDialog1.CheckPathExists = true;
                saveFileDialog1.Filter = "ADPCM WAV (*.wav)|*.wav|All files (*.*)|*.*";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    ConvertToWAV();
                    File.WriteAllBytes(saveFileDialog1.FileName, filebytes);
                }
            }
        }

        public void ConvertToWAV() {

            byte[] output = new byte[0x3C + filebytes.Length];

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

            output[0x10] = 0x14;    //fmt chunk length
            output[0x11] = 0x00;
            output[0x12] = 0x00;
            output[0x13] = 0x00;

            output[0x14] = 0x11;    //ADPCM
            output[0x15] = 0x00;

            output[0x16] = 0x01;    //num channels
            output[0x17] = 0x00;

            if (samplerate == 44100)
            {
                output[0x18] = 0x44;    //sample rate
                output[0x19] = 0xAC;
                output[0x1A] = 0x00;
                output[0x1B] = 0x00;
            }
            else
            {
                output[0x18] = 0x22;    //sample rate
                output[0x19] = 0x56;
                output[0x1A] = 0x00;
                output[0x1B] = 0x00;
            }


            output[0x1C] = 0xA8;    //data rate
            output[0x1D] = 0x2B;
            output[0x1E] = 0x00;
            output[0x1F] = 0x00;

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

            uint samplecount = (uint)(filebytes.Length * 2);

            output[0x30] = (byte)samplecount;  //sample count
            output[0x31] = (byte)(samplecount >> 8);
            output[0x32] = (byte)(samplecount >> 16);
            output[0x33] = (byte)(samplecount >> 24);

            output[0x34] = (byte)'d';
            output[0x35] = (byte)'a';
            output[0x36] = (byte)'t';
            output[0x37] = (byte)'a';

            output[0x38] = (byte)filebytes.Length; //data size (aka, the size of filebytes from before)
            output[0x39] = (byte)(filebytes.Length >> 8);
            output[0x3A] = (byte)(filebytes.Length >> 16);
            output[0x3B] = (byte)(filebytes.Length >> 24);

            Array.Copy(filebytes, 0, output, 0x3C, filebytes.Length);   //copy ADPCM data into wav

            filebytes = output;
        }










    }
}
