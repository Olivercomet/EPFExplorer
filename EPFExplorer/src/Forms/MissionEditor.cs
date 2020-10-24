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
    public partial class MissionEditor : Form
    {
        public Form1 form1;

        public arcfile mainArc;
        public arcfile downloadArc;

        public rdtfile gameRdt;
        public rdtfile downloadRdt;

        public archivedfile tuxedoDL;

        public int ReserveDownloadNpcs;
        public int ReserveDownloadExits;
        public int ReserveDownloadItems;

        public List<DownloadItem> downloadItems = new List<DownloadItem>();

        public MissionEditor()
        {
            InitializeComponent();
        }

        public enum InteractionType {
            NPC = 0x01,
            Door = 0x02,
            InventoryItem = 0x03,
            Uninteractable = 0x04,
            Interactable = 0x05
        }
        public class DownloadItem {

            public int ID;
            public string spritePath;

            public int Xpos;
            public int Ypos;

            public InteractionType interactionType = InteractionType.Interactable;
            public bool SpawnedByDefault;
            public int unk1;
            public string luaScriptPath;
            public int unk2;

            public int room;

            public int unk3;

            public string destinationRoom;

            public bool locked;

            public int destPosX;
            public int destPosY;

            public bool flipX;
            public bool flipY;
        }

        public void LoadFormControls() {
            foreach (Form1.Room r in form1.rooms)
            {
                selectedRoomBox.Items.Add(r.DisplayName);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void selectedRoomBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chooseCustomRoomBackground_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Custom backgrounds take up lots of space! \nAre you sure you want to use one?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {


            }
        }

        private void chooseMainArc_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select fs.arc file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "1PP archives (*.arc)|*.arc";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                mainArc = new arcfile();
                mainArc.form1 = form1;
                mainArc.filebytes = File.ReadAllBytes(openFileDialog1.FileName);
                mainArc.ReadArc();
                fsArcLabel.Text = openFileDialog1.FileName;
            }
        }

        private void chooseGameRdt_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select game.rdt file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "1PP resource data (*.rdt)|*.rdt";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                gameRdt = new rdtfile();
                gameRdt.form1 = form1;
                gameRdt.filebytes = File.ReadAllBytes(openFileDialog1.FileName);
                gameRdt.ReadRdt();
                gameRdtLabel.Text = openFileDialog1.FileName;
            }
        }

        private void chooseDownloadArc_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select download.arc file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "1PP archives (*.arc)|*.arc";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                downloadArc = new arcfile();
                downloadArc.form1 = form1;
                downloadArc.filebytes = File.ReadAllBytes(openFileDialog1.FileName);
                downloadArc.ReadArc();
                downloadArcLabel.Text = openFileDialog1.FileName;
            }
        }

        private void loadMission_Click(object sender, EventArgs e)
        {
            if (mainArc == null || gameRdt == null || downloadArc == null)
            {
                MessageBox.Show("You need to add the above three files first!");
                return;
            }

            if (tuxedoDL != null)
            {
                DialogResult result = MessageBox.Show("Are you sure? This will reload the mission in the editor, and any changes you have made will be lost!", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes)
                {
                    return;
                }
            }

            //read tuxedoDL

            tuxedoDL = downloadArc.GetFileByName("/chunks/tuxedoDL.luc");
            tuxedoDL.ReadFile();
            tuxedoDL.DecompileLuc(tuxedoDL.filebytes, "tuxedoDL_TEMP");

            string[] tuxedoDLdecompiled = File.ReadAllLines("tuxedoDL_TEMP");
            File.Delete("tuxedoDL_TEMP");

            //parse downloadItems (aka, the mission objects) from tuxedoDL

            downloadItems = new List<DownloadItem>();

            for (int i = 0; i < tuxedoDLdecompiled.Length; i++)
            {
                if (tuxedoDLdecompiled[i].Contains("_util.ReserveDownloadNpcs"))
                {
                    ReserveDownloadNpcs = int.Parse(tuxedoDLdecompiled[i].Replace("_util.ReserveDownloadNpcs(", "").Replace(")", ""));
                }
                else if (tuxedoDLdecompiled[i].Contains("_util.ReserveDownloadExits"))
                {
                    ReserveDownloadExits = int.Parse(tuxedoDLdecompiled[i].Replace("_util.ReserveDownloadExits(", "").Replace(")", ""));
                }
                else if (tuxedoDLdecompiled[i].Contains("_util.ReserveDownloadItems"))
                {
                    ReserveDownloadItems = int.Parse(tuxedoDLdecompiled[i].Replace("_util.ReserveDownloadItems(", "").Replace(")", ""));
                }
                else if (tuxedoDLdecompiled[i].Contains("_util.AddDownloadItem"))
                {
                    LoadDownloadItemFromString(tuxedoDLdecompiled[i].Substring(0, tuxedoDLdecompiled[i].Length - 1).Replace("_util.AddDownloadItem(", ""));
                }
            }

            UpdateCurrentCapacity();





        }

        public void UpdateCurrentCapacity() {

            //work out current capacity and display it on the progress bar

            int size = 8 + (downloadArc.archivedfiles.Count * 0x0C); //filecount + checksum + (0x0C * filecount)

            foreach (archivedfile f in downloadArc.archivedfiles)
            {
                f.ReadFile();
                f.CompressFileLZ11();
                f.was_LZ10_compressed = false;
                size += f.filebytes.Length;
                f.DecompressFile();
            }

            while (size % 4 != 0)
            {
                size++;
            }

            if (size >= capacityProgressBar.Value)
                {
                MessageBox.Show("Your mission will be too big to fit in a save file!\nIt is " + ((((float)size/(float)capacityProgressBar.Maximum)*(float)100.00) - (float)100.00) + "% over the maximum size.","Size limit exceeded",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                capacityProgressBar.Value = capacityProgressBar.Maximum - 1;
                return;
                }
            capacityProgressBar.Value = size;
        }


        public void LoadDownloadItemFromString(string args)
            {
            string[] splitString = args.Replace(", ",",").Split(',');

            DownloadItem newDownloadItem = new DownloadItem();

            newDownloadItem.ID = int.Parse(splitString[0]);
            newDownloadItem.spritePath = splitString[1].Substring(1,splitString[1].Length-2);
            newDownloadItem.Xpos = int.Parse(splitString[2]);
            newDownloadItem.Ypos = int.Parse(splitString[3]);
            newDownloadItem.interactionType = (InteractionType)int.Parse(splitString[4]);
            if (splitString[5] == "true" ? newDownloadItem.SpawnedByDefault = true : newDownloadItem.SpawnedByDefault = false)
            newDownloadItem.unk1 = int.Parse(splitString[6]);
            newDownloadItem.luaScriptPath = splitString[7].Substring(1, splitString[7].Length - 2);
            newDownloadItem.unk2 = int.Parse(splitString[8]);
            newDownloadItem.room = int.Parse(splitString[9]);
            newDownloadItem.unk3 = int.Parse(splitString[10]);
            newDownloadItem.destinationRoom = splitString[11].Substring(1, splitString[11].Length - 2);
            if (splitString[12] == "true" ? newDownloadItem.locked = true : newDownloadItem.locked = false)
            newDownloadItem.destPosX = int.Parse(splitString[13]);
            newDownloadItem.destPosY = int.Parse(splitString[14]);
            if (splitString[15] == "true" ? newDownloadItem.flipX = true : newDownloadItem.flipX = false)
            if (splitString[16] == "true" ? newDownloadItem.flipY = true : newDownloadItem.flipY = false)

             downloadItems.Add(newDownloadItem);
        }

        private void recalculateCapacityButton_Click(object sender, EventArgs e)
        {
            UpdateCurrentCapacity();
        }
    }
}
