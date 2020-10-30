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

        string saveExtenderCodeEU = "5211AAC8 020922F7\n1211AAC8 000022FF\n1211AAD8 000022FF\n1211AF3A 000022FF\n1211AF48 000022FF\n1211B0D2 000022FF\n1211BC36 0000BDF8\nD0000000 00000000";


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
            }
        }


        private void currentMissionChooser_SelectedIndexChanged(object sender, EventArgs e)
        {

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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SaveFileEditor_Load(object sender, EventArgs e)
        {

        }

        private void penguinNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (penguinNameTextBox.TextLength > 0x0C)
            {
                penguinNameTextBox.Text = penguinNameTextBox.Text.Substring(0, 0x0C);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void containsDownloadable_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void coinsChooser_ValueChanged(object sender, EventArgs e)
        {

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

                foreach (archivedfile file in activeSaveFile.embeddedArc.archivedfiles)
                {
                    file.ReadFile();    //this may look redundant, as rebuildarc does this anyway, but we need to read it out before changing the compression information, otherwise rebuildarc's readarc call will try to decompress even if it's not actually compressed yet
                    
                    if (!file.was_LZ10_compressed && !file.was_LZ11_compressed)
                        {
                        file.has_LZ11_filesize = true;
                        file.was_LZ11_compressed = true;
                        }
                }

                activeSaveFile.embeddedArc.use_custom_filename_table = false;

                activeSaveFile.embeddedArc.RebuildArc();

                if (activeSaveFile.embeddedArc.filebytes.Length > 0xF540)   //used to be 0xFEF0 when extended saves were allowed
                {
                    MessageBox.Show("That arc file is too big.\nIt should be smaller than 61KB.", "Arc file is too big", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        ARform.SetInfo(saveExtenderCodeEU,"Allow extended saves","For: Elite Penguin Force (EU version only)","Normal saves will be incompatible while the code is active.\nSaving the game is disabled while the code is active.");
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

        private void colourChooser_SelectedIndexChanged(object sender, EventArgs e)
        {

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
    }
}
