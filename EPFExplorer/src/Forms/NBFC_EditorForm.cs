using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EPFExplorer
{
    public partial class NBFC_EditorForm : Form
    {
        public NBFC_EditorForm()
        {
            InitializeComponent();
        }

        public Form1 form1;

        public nbfcTilesetFile activeNbfc;
        public mpbfile activeNbfs;
        public nbfpfile activeNbfp;

        public Color TSBAlphaColour;

        public Image image;

        public bool userDisagreedWithWidth = false;

        public bool justAutoCorrected = false;

        public class Tile
        {
            public byte[] tileImage = new byte[64];
            public bool flipX = false;
            public bool flipY = false;

            public bool hasSimilarTile = false;
            public Tile SimilarTile;    //the master tile that this one stores a reference to. (although this one can still have its own unique flipping etc)
        }

        public Dictionary<string, int> MPBFilesAndWidthsInTiles = new Dictionary<string, int>()
        {
        };



        private void chooseTileset_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "Nitro Tileset (*.nbfc)|*.nbfc";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                activeNbfc = new nbfcTilesetFile();
                activeNbfc.form1 = form1;
                activeNbfc.filepath = openFileDialog1.FileName;
                activeNbfc.filebytes = File.ReadAllBytes(activeNbfc.filepath);
                activeNbfc.Load();
            }
        }

        private void chooseTilemap_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "Nitro Tilemap (*.nbfs)|*.nbfs";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                activeNbfs = new mpbfile();
                activeNbfs.form1 = form1;
                //activeNbfs.editorForm = this;
                activeNbfs.filepath = openFileDialog1.FileName;
                activeNbfs.filebytes = File.ReadAllBytes(activeNbfs.filepath);
                activeNbfs.Load();
            }
        }

        public void LoadBoth()
        {

            if (!userDisagreedWithWidth && (activeNbfs.known_tile_width != 0 && ImageWidthInTiles.Value != activeNbfs.known_tile_width))
            {
                ImageWidthInTiles.Value = activeNbfs.known_tile_width;
                userDisagreedWithWidth = true;  //we have given them one chance to keep this the same, and then just let them change it after that if they really want to
            }


            if (activeNbfs == null)
            {
                MessageBox.Show("No tilemap loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (activeNbfc == null)
            {
                MessageBox.Show("No tileset loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (activeNbfp == null)
            {
                MessageBox.Show("No palette loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            if ((activeNbfs.highest_tile_offset) / 64 > activeNbfc.number_of_tiles)
            {
                MessageBox.Show("That tilemap references tiles that are beyond the end of the tileset!\nTileset number of tiles: " + activeNbfc.number_of_tiles + ".\nThe tilemap's highest tile is: " + ((activeNbfs.highest_tile_offset) / 64), "Tilemap is too big for tileset", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int heightInTiles = (activeNbfs.filebytes.Length / 2) / (int)ImageWidthInTiles.Value;

            Byte[] imageForDisplay = new byte[((int)ImageWidthInTiles.Value * 8) * (heightInTiles * 8)];

            //make an epf format image from the tiles, and then convert it to an Image and display it

            int pos_in_output_image = 0;

            //Console.WriteLine("Image dimensions will be: "+ ((int)ImageWidthInTiles.Value * 8) + " by " + (heightInTiles * 8));

            for (int y = 0; y < heightInTiles; y++)
            {
                for (int x = 0; x < (int)ImageWidthInTiles.Value; x++)
                {
                    ushort IndexFromMPB = BitConverter.ToUInt16(activeNbfs.filebytes, (y * 2 * (int)ImageWidthInTiles.Value) + (x * 2)); ;

                    bool flipX = false;
                    bool flipY = false;

                    if ((IndexFromMPB & 0x0800) == 0x0800)
                    {
                        flipY = true;
                    }

                    if ((IndexFromMPB & 0x0400) == 0x0400)
                    {
                        flipX = true;
                    }

                    int offset_of_tile_in_tsb = 0;

                    offset_of_tile_in_tsb = 64 * (0x03FF & IndexFromMPB);

                    if (!flipX && !flipY)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            Array.Copy(activeNbfc.filebytes, offset_of_tile_in_tsb + (i * 8), imageForDisplay, pos_in_output_image + (i * (int)ImageWidthInTiles.Value * 8), 8);
                        }
                    }
                    else if (flipX && flipY)
                    {
                        for (int i = 7; i >= 0; i--)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                imageForDisplay[pos_in_output_image + ((7 - i) * (int)ImageWidthInTiles.Value * 8) + (7 - j)] = activeNbfc.filebytes[offset_of_tile_in_tsb + (i * 8) + j];
                            }
                        }
                    }
                    else if (flipX)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                imageForDisplay[pos_in_output_image + (i * (int)ImageWidthInTiles.Value * 8) + (7 - j)] = activeNbfc.filebytes[offset_of_tile_in_tsb + (i * 8) + j];
                            }
                        }
                    }
                    else if (flipY)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            Array.Copy(activeNbfc.filebytes, offset_of_tile_in_tsb + ((7 - i) * 8), imageForDisplay, pos_in_output_image + (i * (int)ImageWidthInTiles.Value * 8), 8);
                        }
                    }

                    pos_in_output_image += 8;
                }
                pos_in_output_image += ((int)ImageWidthInTiles.Value * 8) * 7;
            }

            image = form1.NBFCtoImage(imageForDisplay, 0, (int)ImageWidthInTiles.Value * 8, heightInTiles * 8, activeNbfp.palette, 8);
            pixelBox1.Image = image;
        }

        private void ImageWidthInTiles_ValueChanged(object sender, EventArgs e)
        {
            if (justAutoCorrected)
            {
                justAutoCorrected = false;
            }
            else
            {
                LoadBoth();
            }
        }

        private void LOAD_Click(object sender, EventArgs e)
        {
            LoadBoth();
        }

        private void SaveToTSBAndMPB_Click(object sender, EventArgs e)
        {
            if (activeNbfs == null)
            {
                MessageBox.Show("No MPB loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (activeNbfc == null)
            {
                MessageBox.Show("No TSB loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Title = "Save NBFS";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.Filter = "Nitro Tilemap (*.nbfs)|*.nbfs";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(saveFileDialog1.FileName, activeNbfs.filebytes);
            }
            else
            {
                return;
            }

            saveFileDialog1.Title = "Save NBFC";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.Filter = "Nitro Tileset (*.nbfc)|*.nbfc";
            saveFileDialog1.FileName = "";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(saveFileDialog1.FileName, activeNbfc.filebytes);
            }
            else
            {
                return;
            }

            saveFileDialog1.Title = "Save NBFP";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.Filter = "Nitro Palette (*.nbfp)|*.nbfp";
            saveFileDialog1.FileName = "";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(saveFileDialog1.FileName, activeNbfp.filebytes);
            }
            else
            {
                return;
            }
        }

        private void exportToPNG_Click_1(object sender, EventArgs e)
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

        private void importFromPNG_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select image file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "PNG image (*.png)|*.png";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                activeNbfc = new nbfcTilesetFile();
                activeNbfs = new mpbfile();
                activeNbfp = new nbfpfile();
                // activeNbfs.editorForm = this;

                Bitmap image = (Bitmap)Image.FromFile(openFileDialog1.FileName);

                //check dimensions
                if (image.Width % 8 != 0)
                {
                    MessageBox.Show("Image width must be a multiple of 8.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (image.Height % 8 != 0)
                {
                    MessageBox.Show("Image height must be a multiple of 8.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //check palette
                List<Color> palette = new List<Color>();

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        Color newColour = image.GetPixel(x, y);

                        if (!palette.Contains(newColour))
                        {
                            palette.Add(newColour);
                        }

                        if (palette.Count > 256)
                        {
                            MessageBox.Show("The image must not have more than 256 colours.", "Too many colours", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }

                if (palette.Count < 256 && !palette.Contains(Color.FromArgb(0xFF, 0xFF, 0x00, 0xFF)) && !palette.Contains(Color.FromArgb(0x00, 0xFF, 0x00, 0xFF)))  //if there's space for it, sneakily add the magenta alpha anyway
                {
                    palette.Add(Color.FromArgb(0xFF, 0xFF, 0x00, 0xFF));
                }

                MessageBox.Show("Alpha colour is enforced as RGB 0xFF00FF (a bright magenta). If you want transparency in your image, use this colour.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);

                TSBAlphaColour = Color.FromArgb(0xFF, 0xFF, 0x00, 0xFF);

                //swap the alpha colour to the start

                for (int c = 0; c < palette.Count; c++)
                {
                    if ((palette[c].R & 0xF8) == (TSBAlphaColour.R & 0xF8) && (palette[c].G & 0xF8) == (TSBAlphaColour.G & 0xF8) && (palette[c].B & 0xF8) == (TSBAlphaColour.B & 0xF8))
                    {
                        //swap the first slot colour here, and put the alpha colour in the first slot instead
                        palette[c] = palette[0];
                        palette[0] = TSBAlphaColour;
                        break;
                    }
                }

                //truncate colours

                TSBAlphaColour = Color.FromArgb(0xFF, TSBAlphaColour.R & 0xF8, TSBAlphaColour.G & 0xF8, TSBAlphaColour.B & 0xF8);

                for (int c = 0; c < palette.Count; c++)
                {
                    palette[c] = Color.FromArgb(0xFF, palette[c].R & 0xF8, palette[c].G & 0xF8, palette[c].B & 0xF8);
                }

                //turn image into tiles

                activeNbfp.palette = palette.ToArray();

                byte[] NBFCimage = (form1.ImageToNBFC(image, 8, activeNbfp.palette));

                NBFCimage.Reverse();

                List<Tile> tiles = new List<Tile>();

                int width_in_tiles = image.Width / 8;
                int height_in_tiles = image.Height / 8;

                int pos = 0;


                List<Tile> uniqueTiles = new List<Tile>();

                for (int y = 0; y < height_in_tiles; y++)
                {
                    for (int x = 0; x < width_in_tiles; x++)
                    {
                        //make a new tile from the current pos in the image
                        Tile newTile = new Tile();

                        for (int i = 7; i >= 0; i--)
                        {
                            Array.Copy(NBFCimage, pos + (image.Width * i), newTile.tileImage, 8 * i, 8);
                        }

                        //now that we have the tile, compare it to the other tiles

                        for (int t = 0; t < tiles.Count; t++)
                        {
                            if (tiles[t].hasSimilarTile)    //only use tiles that aren't already references to other tiles
                            {
                                continue;
                            }

                            int tileSimilarlity = AreTilesSimilar(newTile, tiles[t]);

                            if (tileSimilarlity > 0) //if they are similar (i.e. identical but one of them is a mirror of the other)
                            {
                                newTile.hasSimilarTile = true;
                                newTile.SimilarTile = tiles[t];

                                if (tileSimilarlity == 1)   //then they are just the same tile
                                {
                                }
                                else if (tileSimilarlity == 2)   //then they are the same but one of them is X flipped
                                {
                                    newTile.flipX = true;
                                }
                                else if (tileSimilarlity == 3)   //then they are the same but one of them is Y flipped
                                {
                                    newTile.flipY = true;
                                }
                                else if (tileSimilarlity == 4)   //then they are the same but one of them is flipped on both X and Y
                                {
                                    newTile.flipX = true;
                                    newTile.flipY = true;
                                }
                                break;
                            }
                        }

                        if (!newTile.hasSimilarTile)
                        {
                            uniqueTiles.Add(newTile);
                        }

                        tiles.Add(newTile);
                        pos += 8;
                    }

                    if (pos % image.Width == 0)
                    {
                        pos += image.Width * 7;
                    }
                }

                //now we should have a list of tiles, some of which are mirrors of each other if applicable


                activeNbfc.filebytes = new byte[uniqueTiles.Count * 64];

                //now write palette to nbfp

                activeNbfp.filebytes = new byte[0x200];

                for (int c = 0; c < palette.Count; c++)
                {
                    ushort ABGR1555Colour = form1.ColorToABGR1555(palette[c]);

                    activeNbfp.filebytes[(c * 2)] = (byte)ABGR1555Colour;
                    activeNbfp.filebytes[(c * 2) + 1] = (byte)(ABGR1555Colour >> 8);
                }

                //now write tiles to tsb, but only the ones that are their own tile and not just referencing another one



                int tileWritingPos = 0x00;

                for (int t = 0; t < uniqueTiles.Count; t++)
                {
                    if (!uniqueTiles[t].hasSimilarTile)   //only process it if it's a master tile
                    {
                        foreach (Byte b in uniqueTiles[t].tileImage)
                        {
                            activeNbfc.filebytes[tileWritingPos] = b;
                            tileWritingPos++;
                        }
                    }
                }

                //MPB

                activeNbfs.filebytes = new byte[tiles.Count * 2];


                for (int t = 0; t < tiles.Count; t++)
                {
                    ushort tileDescriptor = 0;

                    if (tiles[t].hasSimilarTile)
                    {
                        //then write a reference to the similar tile instead

                        tileDescriptor = (ushort)uniqueTiles.IndexOf(tiles[t].SimilarTile);


                        if (tiles[t].flipX)
                        {
                            tileDescriptor |= 0x0400;
                        }

                        if (tiles[t].flipY)
                        {
                            tileDescriptor |= 0x0800;
                        }

                    }
                    else    //otherwise, just write this tile normally
                    {
                        tileDescriptor = (ushort)uniqueTiles.IndexOf(tiles[t]);
                    }

                    activeNbfs.filebytes[(t * 2)] = (byte)tileDescriptor;
                    activeNbfs.filebytes[(t * 2) + 1] = (byte)(tileDescriptor >> 8);
                }

                LoadBoth();
            }
        }


        public byte AreTilesSimilar(Tile tile1, Tile tile2)
        {
            //test for completely identical tiles

            bool CompletelyIdentical = true;

            for (int i = 0; i < tile1.tileImage.Length; i++)
            {
                if (tile1.tileImage[i] != tile2.tileImage[i])
                {
                    CompletelyIdentical = false;
                    break;
                }
            }

            if (CompletelyIdentical)
            {
                return 1;
            }


            //test for X flipped identical tiles

            bool XFlippedIdentical = true;

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (tile1.tileImage[(y * 8) + x] != tile2.tileImage[(y * 8) + (7 - x)])
                    {
                        XFlippedIdentical = false;
                        break;
                    }
                }
            }

            if (XFlippedIdentical)
            {
                return 2;
            }

            //test for Y flipped identical tiles

            bool YFlippedIdentical = true;

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (tile1.tileImage[(y * 8) + x] != tile2.tileImage[((7 - y) * 8) + x])
                    {
                        YFlippedIdentical = false;
                        break;
                    }
                }
            }

            if (YFlippedIdentical)
            {
                return 3;
            }

            //test for X and Y flipped identical tiles

            bool BothFlippedIdentical = true;

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (tile1.tileImage[(y * 8) + x] != tile2.tileImage[((7 - y) * 8) + (7 - x)])
                    {
                        BothFlippedIdentical = false;
                        break;
                    }
                }
            }

            if (BothFlippedIdentical)
            {
                return 4;
            }

            return 0;   //the tiles are not similar
        }

        private void thirdBitAddAmountTemp_ValueChanged(object sender, EventArgs e)
        {
            LoadBoth();
        }

        private void choosePalette_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "Nitro Palette (*.nbfp)|*.nbfp";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                activeNbfp = new nbfpfile();
                activeNbfp.form1 = form1;
                activeNbfp.filepath = openFileDialog1.FileName;
                activeNbfp.filebytes = File.ReadAllBytes(activeNbfp.filepath);
                activeNbfp.Load();
            }
        }
    }
}
