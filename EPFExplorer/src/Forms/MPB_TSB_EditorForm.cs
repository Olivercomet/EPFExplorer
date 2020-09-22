using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPFExplorer
{
    public partial class MPB_TSB_EditorForm : Form
    {
        public MPB_TSB_EditorForm()
        {
            InitializeComponent();
        }

        public Form1 form1;

        public tsbfile activeTsb;
        public mpbfile activeMpb;


        private void chooseTileset_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "1PP tileset binary (*.tsb)|*.tsb";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                activeTsb = new tsbfile();
                activeTsb.form1 = form1;
                activeTsb.filepath = openFileDialog1.FileName;
                activeTsb.Load();

                if (activeMpb != null)
                {
      
                }
            }
        }

        private void chooseTilemap_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "1PP tilemap binary (*.mpb)|*.mpb";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                activeMpb = new mpbfile();
                activeMpb.form1 = form1;
                activeMpb.filepath = openFileDialog1.FileName;
                activeMpb.Load();

            }
        }

        public void LoadBoth() { 
        
        if (activeMpb == null)
            {
                MessageBox.Show("No MPB loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

        if (activeTsb == null)
            {
                MessageBox.Show("No TSB loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if ((activeMpb.highest_tile_offset - 0x200) / 64 > activeTsb.number_of_tiles)
            {
            MessageBox.Show("That tilemap references tiles that are beyond the end of the tileset!", "Tilemap is too big for tileset", MessageBoxButtons.OK,MessageBoxIcon.Information);
            return;
            }

            int heightInTiles = (activeMpb.filebytes.Length / 2) / (int)ImageWidthInTiles.Value;

            Byte[] imageForDisplay = new byte[((int)ImageWidthInTiles.Value*8)*(heightInTiles*8)];

            //make an epf format image from the tiles, and then convert it to an Image and display it

            int pos_in_output_image = 0;

            Console.WriteLine("Image dimensions will be: "+ ((int)ImageWidthInTiles.Value * 8) + " by " + (heightInTiles * 8));
            

            for (int y = 0; y < heightInTiles; y++)
                {
                for (int x = 0; x < (int)ImageWidthInTiles.Value; x++)
                    {
                    int offset_of_tile_in_tsb = 0x200 + (64 * (0x3FFF & BitConverter.ToUInt16(activeMpb.filebytes, (y*2*(int)ImageWidthInTiles.Value) + (x*2)))); //cut the highest two bits off the tile. They seem to indicate something else (flipping hopefully?)

                    for (int i = 0; i < 8; i++)
                        {
                        Array.Copy(activeTsb.filebytes, offset_of_tile_in_tsb + (i*8), imageForDisplay, pos_in_output_image + (i * (int)ImageWidthInTiles.Value * 8), 8);
                        }

                    pos_in_output_image += 8;
                    }
                pos_in_output_image += ((int)ImageWidthInTiles.Value * 8) * 7;
             }

            pixelBox1.Image = form1.NBFCtoImage(imageForDisplay, 0, (int)ImageWidthInTiles.Value * 8, heightInTiles * 8, activeTsb.palette, 8);
        
        
        }

        private void ImageWidthInTiles_ValueChanged(object sender, EventArgs e)
        {
            LoadBoth();
        }

        private void exportToPNG_Click(object sender, EventArgs e)
        {
            if (pixelBox1.Image != null)
                {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Title = "Save image";
                saveFileDialog1.CheckPathExists = true;
                saveFileDialog1.Filter = "PNG image (*.png)|*.png|All files (*.*)|*.*";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    pixelBox1.Image.Save(saveFileDialog1.FileName);
                }








            }
        }

        private void LOAD_Click(object sender, EventArgs e)
        {
            LoadBoth();
        }
    }
}
