using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPFExplorer
{
    public partial class SaveFileEditor : Form
    {
        public SaveFileEditor()
        {
            InitializeComponent();
            inventoryBox.Items.Clear();
        }

        savefile activeSaveFile;


        public Form1 form1;
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.InitialDirectory = Properties.Settings.Default.DSSaveFileDir;

            openFileDialog1.Filter = "DS Save (*.sav,*.dsv)|*.sav;*.dsv";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                activeSaveFile = new savefile();
                activeSaveFile.saveFileEditor = this;
                activeSaveFile.LoadFromFile(openFileDialog1.FileName);
                Properties.Settings.Default.DSSaveFileDir = Path.GetDirectoryName(openFileDialog1.FileName);
                Properties.Settings.Default.Save();
                importDownloadArc.Enabled = true;
                importNewspaperImage.Enabled = true;
                exportNewspaperImage.Enabled = true;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activeSaveFile == null)
                {
                MessageBox.Show("You need to open a valid save file first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
                }
            activeSaveFile.SaveToFile();
        }

        private void penguinNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (penguinNameTextBox.TextLength > 0x0C)
            {
                penguinNameTextBox.Text = penguinNameTextBox.Text.Substring(0, 0x0C);
            }
        }

        private void exportDownloadArc_Click(object sender, EventArgs e)
        {

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.FileName = Path.GetFileName(activeSaveFile.embeddedArc.filename);

            saveFileDialog1.InitialDirectory = Properties.Settings.Default.DLCArcsDir;
            saveFileDialog1.Title = "Export file";
            saveFileDialog1.CheckPathExists = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(saveFileDialog1.FileName, activeSaveFile.embeddedArc.filebytes);
                Properties.Settings.Default.DLCArcsDir = Path.GetDirectoryName(saveFileDialog1.FileName);
                Properties.Settings.Default.Save();
            }
        }

        private void importDownloadArc_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select .arc file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.InitialDirectory = Properties.Settings.Default.DLCArcsDir;
            openFileDialog1.Filter = "1PP archives (*.arc)|*.arc";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.DLCArcsDir = Path.GetDirectoryName(openFileDialog1.FileName);
                Properties.Settings.Default.Save();

                byte[] potentialFile = File.ReadAllBytes(openFileDialog1.FileName);

                activeSaveFile.embeddedArc = new arcfile();
                activeSaveFile.embeddedArc.form1 = form1;
                activeSaveFile.embeddedArc.filebytes = potentialFile;

                //make some recommended changes to the arc before import (so that it fits)
                activeSaveFile.embeddedArc.ReadArc();

                author.Text = "Author: None";
                authorNote.Text = "Author's note: None";

                for (int i = activeSaveFile.embeddedArc.archivedfiles.Count - 1; i >= 0; i--)
                {
                    activeSaveFile.embeddedArc.archivedfiles[i].ReadFile();    //this may look redundant, as rebuildarc does this anyway, but we need to read it out before changing the compression information, otherwise rebuildarc's readarc call will try to decompress even if it's not actually compressed yet
                    
                    if (!activeSaveFile.embeddedArc.archivedfiles[i].was_LZ10_compressed && !activeSaveFile.embeddedArc.archivedfiles[i].was_LZ11_compressed)
                        {
                        activeSaveFile.embeddedArc.archivedfiles[i].has_LZ11_filesize = true;
                        activeSaveFile.embeddedArc.archivedfiles[i].was_LZ11_compressed = true;
                        }

                    if (activeSaveFile.embeddedArc.archivedfiles[i].filename.ToLower() == "/info.txt")
                        {
                        activeSaveFile.SetAuthorDetails();
                        activeSaveFile.embeddedArc.archivedfiles.RemoveAt(i);
                        }
                }

                activeSaveFile.embeddedArc.use_custom_filename_table = false;

                activeSaveFile.embeddedArc.RebuildArc();

                if (activeSaveFile.embeddedArc.filebytes.Length > 0xF540)   //used to be 0xFEF0 when extended saves were allowed
                {
                    MessageBox.Show("That arc file is too big.\nIts effective size should be smaller than 61KB.\nYour arc was: "+((float)activeSaveFile.embeddedArc.filebytes.Length / (float)1024.00) +"KB.", "Arc file is too big", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    activeSaveFile.embeddedArc = null;
                    downloadableMissionNameDisplay.Text = "Downloadable Mission: None";
                    exportDownloadArc.Enabled = false;
                }
                else if (activeSaveFile.embeddedArc.filebytes.Length > 0xF540)   //exceeds even the custom size
                {
                    //THIS DOES NOT WORK IN-GAME, so it has been dummied out here.
                    if (MessageBox.Show("That arc file would usually be too big.\nHowever, it can still be loaded if you use an action replay code on the EU version of the game.\nWould you like to do this?", "Arc file is too big, but there is a solution", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                        activeSaveFile.extendedSaveMode = true;
                        activeSaveFile.GetDownloadableMissionName();
                        exportDownloadArc.Enabled = true;
                        ShowActionReplayCodeForm ARform = new ShowActionReplayCodeForm();
                        //ARform.SetInfo(saveExtenderCodeEU,"Allow extended saves","For: Elite Penguin Force (EU version only)","Normal saves will be incompatible while the code is active.\nSaving the game is disabled while the code is active.");
                        ARform.Show();
                        }
                    else
                        {
                        activeSaveFile.embeddedArc = null;
                        downloadableMissionNameDisplay.Text = "Downloadable Mission: None";
                        exportDownloadArc.Enabled = false;
                        }
                }
                else
                {
                    activeSaveFile.extendedSaveMode = false;
                    activeSaveFile.GetDownloadableMissionName();
                    exportDownloadArc.Enabled = true;
                }
            }
        }

        private void quickLaunchButton_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.QuickLaunchRomPath == "" || Properties.Settings.Default.QuickLaunchRomPath == null)
                {
                MessageBox.Show("You need to set a quick launch rom first!", "Rom not set", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            else
                {
                QuickLaunch();
                }
        }

        public void QuickLaunch()
        {
            activeSaveFile.SaveToFile();
            System.Diagnostics.Process.Start(@Properties.Settings.Default.QuickLaunchRomPath);
        }

        private void ChooseQuickLaunchRom_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select Nintendo DS rom";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.InitialDirectory = Properties.Settings.Default.QuickLaunchRomPath;
            openFileDialog1.Filter = "Nintendo DS rom (*.nds)|*.nds|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.QuickLaunchRomPath = openFileDialog1.FileName;
                Properties.Settings.Default.Save();

                DialogResult result = MessageBox.Show("Quick launch path set. Launch rom now?", "Path set successfully", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    QuickLaunch();
                }
            }
        }

        private void exportNewspaperImage_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
                {
                return;
                }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Title = "Export image";
            saveFileDialog1.Filter = "PNG image (*.png)|*.png";
            saveFileDialog1.CheckPathExists = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                pictureBox1.Image.Save(saveFileDialog1.FileName);
                }
        }

        private void importNewspaperImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Import image";
            openFileDialog1.Filter = "PNG images (*.png)|*.png";
            openFileDialog1.CheckPathExists = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap potentialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);

                if (potentialImage.Width != 220 || potentialImage.Height != 96)
                    {
                    MessageBox.Show("Wrong image dimensions. The accepted dimensions are 220px wide by 96px high.");
                    return;
                    }

                List<Color> uniqueColors = new List<Color>();

                for(int y = 0; y < potentialImage.Height; y++)
                    {
                    for (int x = 0; x < potentialImage.Width; x++)
                        {
                        Color c = potentialImage.GetPixel(x, y);
                        c = Color.FromArgb(0xFF, c.R & 0xF8, c.G & 0xF8, c.B & 0xF8);
                        
                        if (!uniqueColors.Contains(c))
                            {
                            uniqueColors.Add(c);
                            }
                        }
                    }

                if (uniqueColors.Count > 15)
                    {
                    MessageBox.Show("Too many colours! Your image had: "+uniqueColors.Count+"colours. The limit is 15.");
                    return;
                    }

                uniqueColors.Insert(0, Color.FromArgb(0xFF, 0xFF, 0x00, 0xFF));    //insert alpha colour at the beginning

                pictureBox1.Image = potentialImage;

                activeSaveFile.newsletterPalette = uniqueColors.ToArray();
                activeSaveFile.newsletterImage = form1.ImageToNBFC(potentialImage, 4, activeSaveFile.newsletterPalette);
            }
        }
    }
}