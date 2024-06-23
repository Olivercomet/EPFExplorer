using RedCell.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace EPFExplorer
{
    public partial class MissionEditor : Form
    {
        public Form1 form1;

        bool ready;

        public arcfile mainArc;
        public arcfile downloadArc;

        public rdtfile gameRdt;
        public rdtfile downloadRdt;

        public archivedfile tuxedoDL;

        public int ReserveDownloadNpcs;
        public int ReserveDownloadExits;
        public int ReserveDownloadItems;

        public Form1.Room selectedRoom;

        public List<archivedfile> luaScripts = new List<archivedfile>();

        public MPB_TSB_EditorForm mpb_tsb_editor = new MPB_TSB_EditorForm();
        public SpriteEditor spriteEditor = new SpriteEditor();

        public List<int> already_used_IDs = new List<int>();

        public List<PixelBox> pixelBoxes = new List<PixelBox>();

        public archivedfile downloadStrings;
        public archivedfile dialogStrings;
        public archivedfile gameStrings;

        bool includeDownloadStrings;
        bool includeDialogStrings;
        bool includeGameStrings;


        public MissionEditor()
        {
            InitializeComponent();
        }

        public enum InteractionType
        {
            NPC = 0x01,
            Door = 0x02,
            InventoryItem = 0x03,
            Uninteractable = 0x04,
            Interactable = 0x05,
            Puffle = 0x06,
            InteractionType7 = 0x07,
            InteractThenDespawn = 0x08
        }

        public enum InteractAnim
        {
            OneShotAnimation = 0x00,
            CycleThroughSprites = 0x01
        }
        public class DownloadItem
        {

            public int ID = 0;
            public string spritePath = "";

            public int Xpos = 0;
            public int Ypos = 0;

            public InteractionType interactionType = InteractionType.Interactable;
            public bool SpawnedByDefault = true;
            public int sound;
            public string luaScriptPath = "";
            public int unk2;

            public int room;

            public InteractAnim interactionSubtype;

            public string destinationRoom = "";

            public bool locked = false;

            public int destPosX;
            public int destPosY;

            public bool flipX = false;
            public bool flipY = false;

            public PixelBox image;
            public int displayOffsetX;
            public int displayOffsetY;
        }

        public void OnClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("Are you sure you want to exit the mission editor? Any unsaved changes will be lost!", "Are you sure you want to exit?", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) != System.Windows.Forms.DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }
        public void LoadFormControls()
        {

            DestinationRoomComboBox.Items.Add("None");

            foreach (Form1.Room r in form1.rooms)
            {
                selectedRoomBox.Items.Add(r.DisplayName);
                DestinationRoomComboBox.Items.Add(r.DisplayName);
            }

            DestinationRoomComboBox.SelectedIndex = 0;

            objectsTab.Enabled = false;
            luaScriptsTabPage.Enabled = false;
            textEditorTab.Enabled = false;

            mpb_tsb_editor.form1 = form1;

            ready = true;
        }


        private void selectedRoomBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedRoom = form1.rooms[selectedRoomBox.SelectedIndex];

            AddCurrentRoomObjectsToComboBox();

            if (roomObjectsComboBox.Items.Count > 0)
            {
                objectSettingsGroupBox.Enabled = true;
                roomObjectsComboBox.SelectedIndex = 0;
                deleteObject.Enabled = true;
                moveObjectUp.Enabled = true;
                moveObjectDown.Enabled = true;
            }
            else
            {
                objectSettingsGroupBox.Enabled = false;
                deleteObject.Enabled = false;
                moveObjectUp.Enabled = false;
                moveObjectDown.Enabled = false;
            }

            ChangeBackgroundImage();
            AddCurrentRoomPixelBoxes();
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
            if (mainArc == null || gameRdt == null)
            {
                MessageBox.Show("You need to add the above three files first! \nAlternatively, leave the last one blank to open a mission template instead.");
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

            if (!mainArc.archivedfiles[0].LuaUtiliesExist())
            {
                MessageBox.Show("Couldn't write lua utilities. Make sure you're NOT running EPFExplorer in the Program Files directory!");
            }

            downloadStrings = null;
            dialogStrings = null;
            gameStrings = null;

            includeDownloadStrings = false;
            includeDialogStrings = false;
            includeGameStrings = false;

            already_used_IDs = new List<int>();
            luaScripts = new List<archivedfile>();

            string[] tuxedoDLdecompiled = new string[0];


            if (downloadArc == null)
            {
                downloadArc = new arcfile();
                downloadArc.form1 = form1;
                downloadArc.filebytes = EPFExplorer.Properties.Resources.templateMission;
                downloadArc.filename = "TemplateMission.arc";
                downloadArcLabel.Text = "Template Mission loaded";
                downloadArc.ReadArc();
            }

            //read tuxedoDL

            tuxedoDL = downloadArc.GetFileByName("/chunks/tuxedoDL.luc");
            tuxedoDL.ReadFile();
            tuxedoDL.DecompressFile();
            tuxedoDL.DecompileLuc(tuxedoDL.filebytes, "tuxedoDL_TEMP");

            tuxedoDLdecompiled = File.ReadAllLines("tuxedoDL_TEMP");
            File.Delete("tuxedoDL_TEMP");

            saveMissionToolStripMenuItem.Enabled = true;

            //parse downloadItems (aka, the mission objects) from tuxedoDL

            foreach (Form1.Room r in form1.rooms)
            {
                r.Objects.Clear();
            }

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


            //read game.rdt filenames into the combobox

            RDTSpritePath.Items.Clear();

            foreach (archivedfile f in gameRdt.archivedfiles)
            {
                RDTSpritePath.Items.Add(f.filename);
            }

            //and do the same for downloads.rdt if it exists

            archivedfile temp = downloadArc.GetFileByName("/downloads.rdt");

            downloadRdt = new rdtfile();

            if (temp != null)
            {
                temp.ReadFile();
                downloadRdt.filebytes = temp.filebytes;
                downloadRdt.ReadRdt();

                foreach (archivedfile f in downloadRdt.archivedfiles)
                {
                    RDTSpritePath.Items.Add(f.filename);
                }
            }

            RDTSpritePath.Sorted = true;



            //put the lua filenames into the relevant combo boxes

            objectLuaScriptComboBox.Items.Clear();
            luaScriptComboBox.Items.Clear();


            foreach (archivedfile f in downloadArc.archivedfiles)
            {
                string ext = Path.GetExtension(f.filename).ToLower();

                if (ext != ".luc")
                {
                    continue;
                }

                if (f.filebytes == null || f.filebytes.Length == 0)
                {
                    f.ReadFile();
                }

                if (f.filename.ToLower().Contains("tuxedodl.luc"))
                {
                    continue;
                }

                luaScripts.Add(f);
                objectLuaScriptComboBox.Items.Add(Path.GetFileName(f.filename));
                luaScriptComboBox.Items.Add(Path.GetFileName(f.filename));
            }

            luaScriptComboBox.Sorted = true;
            objectLuaScriptComboBox.Sorted = true;
            objectLuaScriptComboBox.Sorted = false;
            objectLuaScriptComboBox.Items.Insert(0, "None");

            luaRichText.Text = "";

            objectsTab.Enabled = true;
            luaScriptsTabPage.Enabled = true;
            textEditorTab.Enabled = true;

            selectedRoomBox.SelectedIndex = 0;

            AddCurrentRoomObjectsToComboBox(); //force this in case the selected index was already zero (thus not triggering the selectedindexchanged function)

            if (selectedRoomBox.SelectedIndex == 0)
            {
                selectedRoomBox.SelectedIndex = 0;
            }
            else
            {
                selectedRoomBox_SelectedIndexChanged(null, null);
            }

            if (roomObjectsComboBox.Items.Count > 0)
            {
                roomObjectsComboBox.SelectedIndex = 0;
            }


            //read STs

            downloadStrings = downloadArc.GetFileByName("/strings/downloadstrings.st");
            dialogStrings = downloadArc.GetFileByName("/strings/dialogstrings.st");
            gameStrings = downloadArc.GetFileByName("/strings/gamestrings.st");

            //look for them in the download arc first, but if they're not found, use the one from the main arc

            if (dialogStrings == null)
            {
                //don't read it, the vanilla one is too big.
            }
            else
            {
                includeDialogStrings = true;  //we always include this if it was in download.arc before, but NOT if it was only in fs.arc
            }

            if (gameStrings == null)
            {
                //don't read it, the vanilla one is too big.
            }
            else
            {
                includeGameStrings = true;  //we always include this if it was in download.arc before, but NOT if it was only in fs.arc
            }

            if (downloadStrings != null)
            {
                includeDownloadStrings = true;  //we always include this if it was in download.arc before
                downloadStrings.ReadFile();
            }
            else
            {
                downloadStrings = new archivedfile();
                downloadStrings.form1 = form1;
                downloadStrings.parentarcfile = downloadArc;
                downloadStrings.filebytes = new byte[1] { 0x00 };
                downloadStrings.filename = "/strings/downloadstrings.st";
                downloadStrings.STstrings = new List<string>();
                downloadStrings.STstrings.Add(" ");
            }

            if (gameStrings != null)
            {
                gameStrings.ReadFile();
            }
            else
            {
                gameStrings = new archivedfile();
                gameStrings.form1 = form1;
                gameStrings.parentarcfile = downloadArc;
                gameStrings.filebytes = new byte[1] { 0x00 };
                gameStrings.filename = "/strings/gamestrings.st";
                gameStrings.STstrings = new List<string>();
                gameStrings.STstrings.Add(" ");
            }

            if (dialogStrings != null)
            {
                dialogStrings.ReadFile();
            }
            else
            {
                dialogStrings = new archivedfile();
                dialogStrings.form1 = form1;
                dialogStrings.parentarcfile = downloadArc;
                dialogStrings.filename = "/strings/dialogstrings.st";
                dialogStrings.filebytes = new byte[1] { 0x00 };
                dialogStrings.STstrings = new List<string>();
                dialogStrings.STstrings.Add(" ");
            }
        }

        public void UpdateCurrentCapacity()
        {

            //work out current capacity and display it on the progress bar

            int size = 8 + (downloadArc.archivedfiles.Count * 0x0C); //filecount + checksum + (0x0C * filecount)

            foreach (archivedfile f in downloadArc.archivedfiles)
            {
                f.ReadFile();

                int rememberCompression = 0;

                if (f.was_LZ10_compressed)
                {
                    rememberCompression = 10;
                }
                else if (f.was_LZ11_compressed)
                {
                    rememberCompression = 11;
                }
                f.CompressFileLZ11();

                size += f.filebytes.Length;
                f.DecompressFile();

                f.was_LZ11_compressed = false;
                f.has_LZ11_filesize = false;

                if (rememberCompression == 10)
                {
                    f.was_LZ10_compressed = true;
                }
                else if (rememberCompression == 11)
                {
                    f.was_LZ11_compressed = true;
                    f.has_LZ11_filesize = true;
                }
            }

            while (size % 4 != 0)
            {
                size++;
            }

            if (size >= capacityProgressBar.Maximum)
            {
                MessageBox.Show("Your mission will be too big to fit in a save file!\nIt is " + ((((float)size / (float)capacityProgressBar.Maximum) * (float)100.00) - (float)100.00) + "% over the maximum size.\nIf this is unexpected, try making gamestrings.st or dialogstrings.st smaller - the vanilla ones are too big!", "Size limit exceeded", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                capacityProgressBar.Value = capacityProgressBar.Maximum - 1;
                return;
            }
            capacityProgressBar.Value = size;
        }


        public void LoadDownloadItemFromString(string args)
        {
            string[] splitString = args.Replace(", ", ",").Split(',');

            DownloadItem newDownloadItem = new DownloadItem();

            newDownloadItem.ID = int.Parse(splitString[0]);
            newDownloadItem.spritePath = splitString[1].Substring(1, splitString[1].Length - 2);
            newDownloadItem.Xpos = int.Parse(splitString[2]);
            newDownloadItem.Ypos = int.Parse(splitString[3]);
            newDownloadItem.interactionType = (InteractionType)int.Parse(splitString[4]);
            if (splitString[5] == "true") { newDownloadItem.SpawnedByDefault = true; } else { newDownloadItem.SpawnedByDefault = false; }
            newDownloadItem.sound = int.Parse(splitString[6]);
            newDownloadItem.luaScriptPath = splitString[7].Substring(1, splitString[7].Length - 2);
            newDownloadItem.unk2 = int.Parse(splitString[8]);
            newDownloadItem.room = int.Parse(splitString[9]);
            newDownloadItem.interactionSubtype = (InteractAnim)int.Parse(splitString[10]);
            newDownloadItem.destinationRoom = splitString[11].Substring(1, splitString[11].Length - 2);
            if (splitString[12] == "true") { newDownloadItem.locked = true; } else { newDownloadItem.locked = false; }
            newDownloadItem.destPosX = int.Parse(splitString[13]);
            newDownloadItem.destPosY = int.Parse(splitString[14]);
            if (splitString[15] == "true") { newDownloadItem.flipX = true; } else { newDownloadItem.flipX = false; }
            if (splitString[16] == "true") { newDownloadItem.flipY = true; } else { newDownloadItem.flipY = false; }


            already_used_IDs.Add(newDownloadItem.ID);

            foreach (Form1.Room r in form1.rooms)
            {
                if (newDownloadItem.room == r.ID_for_objects)
                {
                    r.Objects.Add(newDownloadItem);
                    break;
                }
            }
        }

        private void recalculateCapacityButton_Click(object sender, EventArgs e)
        {
            if (downloadArc != null)
            {
                UpdateCurrentCapacity();
            }
        }

        private void objectsTab_Click(object sender, EventArgs e)
        {

        }

        private void MissionEditor_Load(object sender, EventArgs e)
        {

        }

        private void roomObjectsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (roomObjectsComboBox.SelectedIndex == -1 && selectedRoom.Objects.Count > 0)
            {
                roomObjectsComboBox.SelectedIndex = 0;
                return;
            }
            ObjectIDUpDown.Value = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].ID;
            PosXUpDown.Value = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].Xpos;
            PosYUpDown.Value = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].Ypos;
            spawnedByDefault.Checked = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].SpawnedByDefault;
            interactionTypeComboBox.SelectedIndex = ((int)selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].interactionType) - 1;
            FlipXCheckBox.Checked = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].flipX;
            FlipYCheckBox.Checked = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].flipY;
            soundUpDown.Value = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].sound;
            Unk2UpDown.Value = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].unk2;
            InteractionAnimTypeComboBox.SelectedIndex = (int)selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].interactionSubtype;
            LockedCheckBox.Checked = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].locked;
            destposX.Value = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].destPosX;
            Console.WriteLine(selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].destPosX);
            destposY.Value = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].destPosY;

            RDTSpritePath.Text = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].spritePath;

            bool validDestRoom = false;

            foreach (Form1.Room r in form1.rooms)
            {
                if (r.InternalName == selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].destinationRoom)
                {
                    DestinationRoomComboBox.SelectedIndex = form1.rooms.IndexOf(r) + 1;
                    validDestRoom = true;
                    break;
                }
            }

            if (!validDestRoom)
            {
                DestinationRoomComboBox.SelectedIndex = 0;
            }

            bool validLuaScriptPath = false;

            if (selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].luaScriptPath.Length > 2)
            {
                for (int i = 0; i < objectLuaScriptComboBox.Items.Count; i++)
                {
                    if (((string)objectLuaScriptComboBox.Items[i]).Replace(".luc", ".lua") == selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].luaScriptPath.Substring(7, selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].luaScriptPath.Length - 7))
                    {
                        objectLuaScriptComboBox.SelectedIndex = i;
                        validLuaScriptPath = true;
                        break;
                    }
                }
            }

            if (!validLuaScriptPath)
            {
                objectLuaScriptComboBox.SelectedIndex = 0;
                selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].luaScriptPath = "\"\"";
            }
        }

        public void AddLuaScriptsToObjectLuaComboBox()
        {

            int i = roomObjectsComboBox.SelectedIndex;

            objectLuaScriptComboBox.Items.Clear();

            foreach (archivedfile f in luaScripts)
            {
                objectLuaScriptComboBox.Items.Add(Path.GetFileName(f.filename));
            }

            objectLuaScriptComboBox.Sorted = true;
            objectLuaScriptComboBox.Sorted = false;
            objectLuaScriptComboBox.Items.Insert(0, "None");

            roomObjectsComboBox.SelectedIndex = i;
        }


        public void AddCurrentRoomObjectsToComboBox()
        {

            AddLuaScriptsToObjectLuaComboBox();

            roomObjectsComboBox.Items.Clear();

            if (selectedRoom.Objects.Count == 0)
            {
                return;
            }

            foreach (DownloadItem item in selectedRoom.Objects)
            {
                string nameToAdd = "";

                if (item.spritePath != null && item.spritePath != "")
                {
                    nameToAdd = item.spritePath;
                }
                else
                {
                    nameToAdd = item.luaScriptPath;
                }

                roomObjectsComboBox.Items.Add(nameToAdd);
            }
        }

        private void ObjectIDUpDown_ValueChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].ID = (int)ObjectIDUpDown.Value;
        }

        private void PosXUpDown_ValueChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].Xpos = (int)PosXUpDown.Value;
            if (selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].image != null)
            {
                selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].image.Location = new System.Drawing.Point(selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].Xpos - selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].displayOffsetX, selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].image.Location.Y);
            }
        }

        private void PosYUpDown_ValueChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].Ypos = (int)PosYUpDown.Value;
            if (selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].image != null)
            {
                selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].image.Location = new System.Drawing.Point(selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].image.Location.X, selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].Ypos - selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].displayOffsetY);
            }
        }

        private void FlipXCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].flipX != FlipXCheckBox.Checked)
            {
                selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].flipX = FlipXCheckBox.Checked;
                if (selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].image != null)
                {
                    selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].image.Image.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipX);
                    selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].image.Refresh();
                }
            }
        }

        private void FlipYCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].flipY != FlipYCheckBox.Checked)
            {
                selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].flipY = FlipYCheckBox.Checked;
                if (selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].image != null)
                {
                    selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].image.Image.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
                    selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].image.Refresh();
                }
            }
        }

        private void spawnedByDefault_CheckedChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].SpawnedByDefault = spawnedByDefault.Checked;
        }

        private void soundUpDown_ValueChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].sound = (int)soundUpDown.Value;
        }

        private void Unk2UpDown_ValueChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].unk2 = (int)Unk2UpDown.Value;
        }

        private void interactionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].interactionType = (InteractionType)(interactionTypeComboBox.SelectedIndex + 1);
        }

        private void destposX_ValueChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].destPosX = (int)destposX.Value;
        }

        private void destposY_ValueChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].destPosY = (int)destposY.Value;
        }

        private void LockedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].locked = LockedCheckBox.Checked;
        }

        private void moveObjectUp_Click(object sender, EventArgs e)
        {
            if (roomObjectsComboBox.SelectedIndex > 0)
            {
                //swap with the one before
                DownloadItem temp = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex - 1];
                selectedRoom.Objects[roomObjectsComboBox.SelectedIndex - 1] = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex];
                selectedRoom.Objects[roomObjectsComboBox.SelectedIndex] = temp;

                int i = roomObjectsComboBox.SelectedIndex;

                //refresh object list

                AddCurrentRoomObjectsToComboBox();
                roomObjectsComboBox.SelectedIndex = i - 1;

                AddCurrentRoomPixelBoxes();
            }
        }

        private void moveObjectDown_Click(object sender, EventArgs e)
        {
            if (roomObjectsComboBox.SelectedIndex < roomObjectsComboBox.Items.Count - 1)
            {
                //swap with the one afterwards
                DownloadItem temp = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex + 1];
                selectedRoom.Objects[roomObjectsComboBox.SelectedIndex + 1] = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex];
                selectedRoom.Objects[roomObjectsComboBox.SelectedIndex] = temp;

                int i = roomObjectsComboBox.SelectedIndex;

                //refresh object list

                AddCurrentRoomObjectsToComboBox();
                roomObjectsComboBox.SelectedIndex = i + 1;
                AddCurrentRoomPixelBoxes();
            }
        }

        public void UpdateLuaScriptComboBox()
        {

            int i = luaScriptComboBox.SelectedIndex;

            luaScriptComboBox.Items.Clear();
            foreach (archivedfile f in luaScripts)
            {
                luaScriptComboBox.Items.Add(Path.GetFileName(f.filename));
            }
            luaScriptComboBox.Sorted = true;

            if (i < luaScriptComboBox.Items.Count)
            {
                luaScriptComboBox.SelectedIndex = i;
            }
        }


        private void DestinationRoomComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ready)
            {
                return;
            }

            if (DestinationRoomComboBox.SelectedIndex == 0)
            {
                selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].destinationRoom = null;
                return;
            }

            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].destinationRoom = form1.rooms[DestinationRoomComboBox.SelectedIndex - 1].InternalName;
        }

        private void deleteObject_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete the object " + roomObjectsComboBox.Items[roomObjectsComboBox.SelectedIndex] + "?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                already_used_IDs.Remove(selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].ID);

                if (roomObjectsComboBox.SelectedIndex > 0)
                {
                    roomObjectsComboBox.SelectedIndex--;
                    roomObjectsComboBox.Items.RemoveAt(roomObjectsComboBox.SelectedIndex + 1);
                    selectedRoom.Objects.RemoveAt(roomObjectsComboBox.SelectedIndex + 1);
                }
                else if (roomObjectsComboBox.Items.Count > 1)
                {
                    roomObjectsComboBox.SelectedIndex = 1;
                    roomObjectsComboBox.Items.RemoveAt(0);
                    selectedRoom.Objects.RemoveAt(0);
                    roomObjectsComboBox.SelectedIndex = 0;
                }
                else
                {
                    roomObjectsComboBox.Items.Clear();
                    selectedRoom.Objects.Clear();
                    selectedRoomBox_SelectedIndexChanged(null, null);
                }
                AddCurrentRoomPixelBoxes();
            }
        }

        private void luaScriptComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            archivedfile scriptToLoad = null;

            foreach (archivedfile f in luaScripts)
            {
                if (f.filename.ToLower() == Path.Combine("/chunks/", (string)luaScriptComboBox.Items[luaScriptComboBox.SelectedIndex]).ToLower())
                {
                    scriptToLoad = f;
                    break;
                }
            }

            if (scriptToLoad == null)
            {
                return;
            }

            luaScriptNameBox.Text = Path.GetFileName(scriptToLoad.filename);

            scriptToLoad.DecompileLuc(scriptToLoad.filebytes, "lua_TEMP_DECOMPILED");
            luaRichText.Text = File.ReadAllText("lua_TEMP_DECOMPILED");
            File.Delete("lua_TEMP_DECOMPILED");
        }

        private void saveLua_Click(object sender, EventArgs e)
        {
            archivedfile scriptToSave = null;

            if (luaRichText.Text.Length == 0)
            {
                MessageBox.Show("Don't save a blank lua file - delete it instead.");
                return;
            }

            if (luaScriptComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Sorry, could you copy your code to the clipboard, reselect the lua script from the dropdown, and try again? This is to avoid an error.");
                return;
            }

            foreach (archivedfile f in luaScripts)
            {
                if (f.filename.ToLower() == Path.Combine("/chunks/", (string)luaScriptComboBox.Items[luaScriptComboBox.SelectedIndex]).ToLower())
                {
                    scriptToSave = f;
                    break;
                }
            }

            if (scriptToSave.filename != Path.Combine("/chunks/", luaScriptNameBox.Text.Replace(".lua", ".luc")))
            {
                if (!luaScriptNameBox.Text.Contains(".lua") && !luaScriptNameBox.Text.Contains(".luc"))
                {
                    luaScriptNameBox.Text += ".luc";
                }

                scriptToSave.filename = Path.Combine("/chunks/", luaScriptNameBox.Text.Replace(".lua", ".luc"));
                UpdateLuaScriptComboBox();
            }

            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "lua_TEMP_FOR_COMPILING"), luaRichText.Text);
            scriptToSave.filebytes = scriptToSave.LuaFromFileToLuc(scriptToSave.filebytes, Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "lua_TEMP_FOR_COMPILING"));
            File.Delete(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "lua_TEMP_FOR_COMPILING"));
            AddCurrentRoomObjectsToComboBox();
            if (roomObjectsComboBox.Items.Count > 0)
            {
                roomObjectsComboBox.SelectedIndex = 0;
            }
        }

        private void deleteLuaScriptButton_Click(object sender, EventArgs e)
        {
            if (luaScriptComboBox.SelectedIndex >= luaScriptComboBox.Items.Count)
            {
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete the lua script " + luaScriptComboBox.Items[luaScriptComboBox.SelectedIndex] + "?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                archivedfile scriptToDelete = null;

                foreach (archivedfile f in luaScripts)
                {
                    if (f.filename.ToLower() == Path.Combine("/chunks/", (string)luaScriptComboBox.Items[luaScriptComboBox.SelectedIndex]).ToLower())
                    {
                        scriptToDelete = f;
                        break;
                    }
                }

                luaScripts.Remove(scriptToDelete);

                if (luaScriptComboBox.SelectedIndex > 0)
                {
                    luaScriptComboBox.SelectedIndex--;
                    luaScriptComboBox.Items.RemoveAt(luaScriptComboBox.SelectedIndex + 1);
                }
                else if (luaScriptComboBox.Items.Count > 1)
                {
                    luaScriptComboBox.SelectedIndex = 1;
                    luaScriptComboBox.Items.RemoveAt(0);
                    luaScriptComboBox.SelectedIndex = 0;
                }
                else
                {
                    luaScriptComboBox.Items.Clear();
                }

                AddCurrentRoomObjectsToComboBox();
            }
        }

        private void addLuaScript_Click(object sender, EventArgs e)
        {
            int savedRoomObjectIndex = roomObjectsComboBox.SelectedIndex;

            archivedfile newScript = new archivedfile();
            newScript.form1 = form1;
            newScript.parentarcfile = downloadArc;
            newScript.filebytes = new byte[] { 0x1B, 0x4C, 0x75, 0x61, 0x51, 0x00, 0x01, 0x04, 0x04, 0x04, 0x04, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x02, 0x1D, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x06, 0x40, 0x40, 0x00, 0x1C, 0x80, 0x80, 0x00, 0x45, 0x80, 0x00, 0x00, 0x46, 0xC0, 0xC0, 0x00, 0x17, 0x40, 0x00, 0x00, 0x16, 0xC0, 0xFF, 0x7F, 0x05, 0x00, 0x00, 0x00, 0x06, 0x40, 0x40, 0x00, 0x1C, 0x80, 0x80, 0x00, 0x45, 0x80, 0x00, 0x00, 0x46, 0x00, 0xC1, 0x00, 0x17, 0x40, 0x00, 0x00, 0x16, 0xC0, 0xFF, 0x7F, 0x05, 0x00, 0x00, 0x00, 0x06, 0x40, 0x40, 0x00, 0x1C, 0x80, 0x80, 0x00, 0x45, 0x80, 0x00, 0x00, 0x46, 0x40, 0xC1, 0x00, 0x17, 0x40, 0x00, 0x00, 0x16, 0xC0, 0xFF, 0x7F, 0x05, 0x00, 0x00, 0x00, 0x06, 0x40, 0x40, 0x00, 0x1C, 0x80, 0x80, 0x00, 0x45, 0x80, 0x00, 0x00, 0x46, 0x80, 0xC1, 0x00, 0x17, 0x40, 0x00, 0x00, 0x16, 0xC0, 0xFF, 0x7F, 0x1E, 0x00, 0x80, 0x00, 0x07, 0x00, 0x00, 0x00, 0x04, 0x06, 0x00, 0x00, 0x00, 0x5F, 0x75, 0x74, 0x69, 0x6C, 0x00, 0x04, 0x0A, 0x00, 0x00, 0x00, 0x47, 0x65, 0x74, 0x52, 0x65, 0x61, 0x73, 0x6F, 0x6E, 0x00, 0x04, 0x07, 0x00, 0x00, 0x00, 0x5F, 0x63, 0x6F, 0x6E, 0x73, 0x74, 0x00, 0x04, 0x08, 0x00, 0x00, 0x00, 0x43, 0x52, 0x45, 0x41, 0x54, 0x45, 0x44, 0x00, 0x04, 0x08, 0x00, 0x00, 0x00, 0x54, 0x4F, 0x55, 0x43, 0x48, 0x45, 0x44, 0x00, 0x04, 0x0D, 0x00, 0x00, 0x00, 0x49, 0x54, 0x45, 0x4D, 0x5F, 0x44, 0x52, 0x4F, 0x50, 0x50, 0x45, 0x44, 0x00, 0x04, 0x08, 0x00, 0x00, 0x00, 0x43, 0x4F, 0x4D, 0x42, 0x49, 0x4E, 0x45, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            bool freeName = false;
            int i = 0;

            while (!freeName)
            {
                freeName = true;

                foreach (archivedfile f in luaScripts)
                {
                    if (f.filename == "/chunks/newScript" + i + ".luc")
                    {
                        freeName = false;
                        break;
                    }
                }

                if (freeName)
                {
                    newScript.filename = "/chunks/newScript" + i + ".luc";
                    break;
                }

                i++;
            }

            luaScripts.Add(newScript);

            UpdateLuaScriptComboBox();
            AddCurrentRoomObjectsToComboBox();

            luaScriptComboBox.SelectedIndex = 0;

            for (int j = 0; j < luaScriptComboBox.Items.Count; j++)
            {
                if ((string)luaScriptComboBox.Items[j] == Path.GetFileName(newScript.filename))
                {
                    luaScriptComboBox.SelectedIndex = j;
                    break;
                }
            }

            roomObjectsComboBox.SelectedIndex = savedRoomObjectIndex;
            saveLua_Click(null, null);
        }

        private void addObjectButton_Click(object sender, EventArgs e)
        {
            DownloadItem newDownloadItem = new DownloadItem();

            newDownloadItem.interactionType = InteractionType.Interactable;
            newDownloadItem.spritePath = "Objects/Crate";
            newDownloadItem.room = selectedRoom.ID_for_objects;
            newDownloadItem.SpawnedByDefault = true;
            newDownloadItem.Xpos = 100;
            newDownloadItem.Ypos = 100;

            Random rnd = new Random();
            do
            {
                newDownloadItem.ID = rnd.Next(30000, 60000);
            } while (already_used_IDs.Contains(newDownloadItem.ID));


            if (roomObjectsComboBox.Items.Count != 0)
            {
                selectedRoom.Objects.Insert(roomObjectsComboBox.SelectedIndex, newDownloadItem);
            }
            else
            {
                selectedRoom.Objects.Add(newDownloadItem);
                objectSettingsGroupBox.Enabled = true;
                deleteObject.Enabled = true;
                moveObjectUp.Enabled = true;
                moveObjectDown.Enabled = true;
            }


            int i = roomObjectsComboBox.SelectedIndex;

            if (i == -1)
            {
                i = 0;
            }

            AddCurrentRoomObjectsToComboBox();

            roomObjectsComboBox.SelectedIndex = i;
            AddCurrentRoomPixelBoxes();
        }

        private void objectLuaScriptComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (roomObjectsComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Sorry, could you please reselect your object from the list? Thanks.");
                return;
            }

            foreach (archivedfile f in luaScripts)
            {
                if ((string)objectLuaScriptComboBox.Items[objectLuaScriptComboBox.SelectedIndex] == Path.GetFileName(f.filename))
                {
                    selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].luaScriptPath = "scripts" + Path.GetFileName(f.filename).Replace(".luc", ".lua");
                    return;
                }
            }

            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].luaScriptPath = "";
        }

        private void RDTSpritePath_TextChanged(object sender, EventArgs e)
        {
            archivedfile newLinkedSprite = null;

            foreach (archivedfile f in gameRdt.archivedfiles)
            {
                if (f.filename == RDTSpritePath.Text)
                {
                    newLinkedSprite = f;
                    break;
                }
            }

            foreach (archivedfile f in downloadRdt.archivedfiles)
            {
                if (f.filename == RDTSpritePath.Text)
                {
                    newLinkedSprite = f;
                    break;
                }
            }

            if (newLinkedSprite == null && RDTSpritePath.Text != "" && RDTSpritePath.Text != "None")
            {
                return;
            }

            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].spritePath = RDTSpritePath.Text;
            AddCurrentRoomPixelBoxes();
        }

        private void ChangeBackgroundImage()
        {

            mpbfile tilemap = new mpbfile();
            tsbfile tileset = new tsbfile();

            mpb_tsb_editor.activeMpb = tilemap;
            mpb_tsb_editor.activeTsb = tileset;

            tilemap.form1 = form1;
            tileset.form1 = form1;

            string tilemapPathInArc = "";
            string tilesetPathInArc = "";

            switch (selectedRoom.InternalName)
            {
                case "ATTIC0":
                    tilemapPathInArc = "/levels/Attic0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/Attic.tsb";
                    break;
                case "BEACH0":
                    tilemapPathInArc = "/levels/Beach0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/Beach.tsb";
                    break;
                case "BEACON0":
                    tilemapPathInArc = "/levels/Beacon0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/Beacon.tsb";
                    break;
                case "BOILERROOM0":
                    tilemapPathInArc = "/levels/BoilerRoom0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/BoilerRoom.tsb";
                    break;
                case "BOOKROOM0":
                    tilemapPathInArc = "/levels/BookRoom0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/BookRoom.tsb";
                    break;
                case "COFFEESHOP0":
                    tilemapPathInArc = "/levels/CoffeeShop0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/CoffeeShop.tsb";
                    break;
                case "COMMANDROOM0":
                    tilemapPathInArc = "/levels/CommandRoom0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/CommandRoom.tsb";
                    break;
                case "DOCK0":
                    tilemapPathInArc = "/levels/Dock0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/Dock.tsb";
                    break;
                case "DOJO0":
                    tilemapPathInArc = "/levels/Dojo0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/Dojo.tsb";
                    break;
                case "FISHING0":
                    tilemapPathInArc = "/levels/Fishing0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/Fishing.tsb";
                    break;
                case "FOREST0":
                    tilemapPathInArc = "/levels/Forest0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/Forest.tsb";
                    break;
                case "GADGETROOM0":
                    tilemapPathInArc = "/levels/GadgetRoom0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/GadgetRoom.tsb";
                    break;
                case "GARYSROOM0":
                    tilemapPathInArc = "/levels/GarysRoom0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/GarysRooms.tsb";
                    break;
                case "GIFTOFFICE0":
                    tilemapPathInArc = "/levels/GiftOffice0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/GiftOffice.tsb";
                    break;
                case "GIFTROOF0":
                    tilemapPathInArc = "/levels/GiftRoof0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/GiftRoof.tsb";
                    break;
                case "GIFTSHOP0":
                    tilemapPathInArc = "/levels/GiftShop0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/GiftShop.tsb";
                    break;
                case "HEADQUARTERS0":
                    tilemapPathInArc = "/levels/HeadQuarters0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/HQ.tsb";
                    break;
                case "ICEBERG0":
                    tilemapPathInArc = "/levels/Iceberg0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/Iceberg.tsb";
                    break;
                case "ICERINK0":
                    tilemapPathInArc = "/levels/IceRink0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/IceRink.tsb";
                    break;
                case "LIGHTHOUSE0":
                    tilemapPathInArc = "/levels/Lighthouse0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/Lighthouse.tsb";
                    break;
                case "LODGE0":
                    tilemapPathInArc = "/levels/Lodge0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/Lodge.tsb";
                    break;
                case "LOUNGE0":
                    tilemapPathInArc = "/levels/Lounge0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/Lounge.tsb";
                    break;
                case "MINECRASH0":
                    tilemapPathInArc = "/levels/MineCrash0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/MineCrashSite.tsb";
                    break;
                case "MINELAIR0":
                    tilemapPathInArc = "/levels/MineLair0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/MineTunnelExit.tsb";
                    break;
                case "MINESHACK0":
                    tilemapPathInArc = "/levels/MineShack0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/MineExterior.tsb";
                    break;
                case "MINEINTERIOR0":
                    tilemapPathInArc = "/levels/MineInterior0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/MineInterior.tsb";
                    break;
                case "MINESHED0":
                    tilemapPathInArc = "/levels/MineShed0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/MineShedInterior.tsb";
                    break;
                case "NIGHTCLUB0":
                    tilemapPathInArc = "/levels/NightClub0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/NightClub.tsb";
                    break;
                case "PETSHOP0":
                    tilemapPathInArc = "/levels/PetShop0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/PetShop.tsb";
                    break;
                case "PIZZAPARLOR0":
                    tilemapPathInArc = "/levels/PizzaParlor0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/PizzaParlor.tsb";
                    break;
                case "PLAZA0":
                    tilemapPathInArc = "/levels/Plaza0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/Plaza.tsb";
                    break;
                case "POOL0":
                    tilemapPathInArc = "/levels/Pool0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/Pool.tsb";
                    break;
                case "PUFFLETRAINING0":
                    tilemapPathInArc = "/levels/PuffleTraining0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/PuffleTraining.tsb";
                    break;
                case "SKIHILL0":
                    tilemapPathInArc = "/levels/SkiHill0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/Mountain.tsb";
                    break;
                case "SKIVILLAGE0":
                    tilemapPathInArc = "/levels/SkiVillage0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/SkiVillage.tsb";
                    break;
                case "SNOWFORTS0":
                    tilemapPathInArc = "/levels/SnowForts0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/SnowForts.tsb";
                    break;
                case "SPORTSHOP0":
                    tilemapPathInArc = "/levels/SportShop0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/SportShop.tsb";
                    break;
                case "STAGE0":
                    tilemapPathInArc = "/levels/Stage0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/Stage.tsb";
                    break;
                case "TALLESTMOUNTAINTOP0":
                    tilemapPathInArc = "/levels/TallestMountainTop0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/TallestMountainTop.tsb";
                    break;
                case "TOWN0":
                    tilemapPathInArc = "/levels/Town0_map_0.mpb";
                    tilesetPathInArc = "/tilesets/Town.tsb";
                    break;
                default:
                    MessageBox.Show("EPFExplorer doesn't have tsb or mpb names listed for that room... even though it probably should!");
                    break;
            }

            //Try to get tilemap from download.arc if it's there. If not, fall back to vanilla.
            archivedfile tilemapArchivedFile = downloadArc.GetFileByName(tilemapPathInArc.ToLower());

            if (tilemapArchivedFile == null)
            {
                tilemapArchivedFile = mainArc.GetFileByName(tilemapPathInArc.ToLower());
            }

            tilemapArchivedFile.ReadFile();
            tilemap.filebytes = tilemapArchivedFile.filebytes;


            //Try to get tileset from download.arc if it's there. If not, fall back to vanilla.
            archivedfile tilesetArchivedFile = downloadArc.GetFileByName(tilesetPathInArc.ToLower());

            if (tilesetArchivedFile == null)
            {
                tilesetArchivedFile = mainArc.GetFileByName(tilesetPathInArc.ToLower());
            }

            if (tilesetArchivedFile == null)
            {
                MessageBox.Show(tilesetPathInArc + " not found! Are youi sure you're using fs.arc, from the first game?");
                return;
            }

            tilesetArchivedFile.ReadFile();
            tileset.filebytes = tilesetArchivedFile.filebytes;

            tilemap.Load();
            tileset.Load();

            if (selectedRoom.tilemapWidth != 0)
            {
                tilemap.known_tile_width = selectedRoom.tilemapWidth;
                mpb_tsb_editor.ImageWidthInTiles.Value = tilemap.known_tile_width;
            }


            mpb_tsb_editor.LoadBoth();

            backgroundImageBox.Image = mpb_tsb_editor.image;
        }


        public void AddCurrentRoomPixelBoxes()
        {

            for (int i = backgroundImageBox.Controls.Count - 1; i >= 0; i--)
            {
                backgroundImageBox.Controls[i].Dispose();
            }

            backgroundImageBox.Controls.Clear();
            pixelBoxes.Clear();

            foreach (DownloadItem item in selectedRoom.Objects)
            {
                PixelBox newPixelBox = new PixelBox();
                backgroundImageBox.Controls.Add(newPixelBox);

                archivedfile sprite = null;

                foreach (archivedfile f in gameRdt.archivedfiles)
                {
                    if (f.filename == item.spritePath)
                    {
                        sprite = f;
                        break;
                    }
                }
                foreach (archivedfile f in downloadRdt.archivedfiles)
                {
                    if (f.filename == item.spritePath)
                    {
                        sprite = f;
                        break;
                    }
                }

                if (sprite == null)
                {
                    foreach (archivedfile f in gameRdt.archivedfiles)
                    {
                        if (f.filename == "Tools/broken")
                        {
                            sprite = f;
                            break;
                        }
                    }
                }

                sprite.form1 = form1;
                if (sprite.filebytes == null || sprite.filebytes.Length == 0)
                {
                    sprite.ReadFile();
                }

                sprite.OpenRDTSubfileInEditor(false);
                newPixelBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                newPixelBox.Anchor = AnchorStyles.None;
                newPixelBox.SizeMode = PictureBoxSizeMode.AutoSize;
                newPixelBox.Parent = backgroundImageBox;
                newPixelBox.Image = sprite.spriteEditor.images[0].image;
                newPixelBox.BringToFront();

                item.displayOffsetX = (int)Math.Round((float)sprite.spriteEditor.centreX.Value - (float)sprite.spriteEditor.images[0].offsetX);
                item.displayOffsetY = (int)Math.Round((float)sprite.spriteEditor.centreY.Value - (float)sprite.spriteEditor.images[0].offsetY);

                if (item.flipX && item.flipY)
                {
                    newPixelBox.Image.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipXY);
                }
                else if (item.flipX)
                {
                    newPixelBox.Image.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipX);
                }
                else if (item.flipY)
                {
                    newPixelBox.Image.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
                }
                else
                {
                    newPixelBox.Image.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipNone);
                }

                newPixelBox.Refresh();

                //and finally, position the sprite in the viewport
                newPixelBox.Location = new System.Drawing.Point(item.Xpos - item.displayOffsetX, item.Ypos - item.displayOffsetY);
                newPixelBox.Show();

                item.image = newPixelBox;
                sprite.spriteEditor.Close();
            }
        }

        private void saveMissionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mainArc == null || downloadArc == null || gameRdt == null)
            {
                return;
            }

            string newTuxedoDL = "";

            ReserveDownloadNpcs = 0;
            ReserveDownloadExits = 0;
            ReserveDownloadItems = 0;

            foreach (Form1.Room r in form1.rooms)
            {
                foreach (DownloadItem item in r.Objects)
                {
                    if (item.interactionType == InteractionType.NPC)
                    {
                        ReserveDownloadNpcs++;
                    }
                    else if (item.interactionType == InteractionType.Door)
                    {
                        ReserveDownloadExits++;
                    }
                    else
                    {
                        ReserveDownloadItems++;
                    }
                }
            }

            newTuxedoDL += "_util.ReserveDownloadNpcs(" + ReserveDownloadNpcs + ")\n";
            newTuxedoDL += "_util.ReserveDownloadExits(" + ReserveDownloadExits + ")\n";
            newTuxedoDL += "_util.ReserveDownloadItems(" + ReserveDownloadItems + ")\n";


            foreach (Form1.Room r in form1.rooms)
            {
                foreach (DownloadItem item in r.Objects)
                {
                    if (item.spritePath == "\"\"" || item.spritePath == "None")
                    {
                        item.spritePath = "";
                    }

                    if (item.luaScriptPath == "\"\"" || item.luaScriptPath == "None")
                    {
                        item.luaScriptPath = "";
                    }

                    if (item.destinationRoom == "\"\"" || item.destinationRoom == "None")
                    {
                        item.destinationRoom = "";
                    }

                    if (item.interactionType == InteractionType.Door)
                    {
                        item.sound = 64;
                    }

                    newTuxedoDL += "_util.AddDownloadItem(";
                    newTuxedoDL += item.ID + ", ";
                    newTuxedoDL += "\"" + item.spritePath + "\", ";
                    newTuxedoDL += item.Xpos + ", ";
                    newTuxedoDL += item.Ypos + ", ";
                    newTuxedoDL += (int)item.interactionType + ", ";
                    if (item.SpawnedByDefault) { newTuxedoDL += "true, "; } else { newTuxedoDL += "false, "; }
                    newTuxedoDL += item.sound + ", ";
                    newTuxedoDL += "\"" + item.luaScriptPath + "\", ";
                    newTuxedoDL += item.unk2 + ", ";
                    newTuxedoDL += item.room + ", ";
                    newTuxedoDL += (int)item.interactionSubtype + ", ";
                    newTuxedoDL += "\"" + item.destinationRoom + "\", ";
                    if (item.locked) { newTuxedoDL += "true, "; } else { newTuxedoDL += "false, "; }
                    newTuxedoDL += item.destPosX + ", ";
                    newTuxedoDL += item.destPosY + ", ";
                    if (item.flipX) { newTuxedoDL += "true, "; } else { newTuxedoDL += "false, "; }
                    if (item.flipY) { newTuxedoDL += "true)\n"; } else { newTuxedoDL += "false)\n"; }
                }
            }


            List<archivedfile> extraFiles = new List<archivedfile>(); //any extra files like rdts, mpbs or tsbs that were hanging out in the old download.arc
            foreach (archivedfile f in downloadArc.archivedfiles)
            {
                if (!f.filename.Contains(".luc") && !f.filename.Contains(".lua") && !f.filename.Contains(".st"))
                {
                    extraFiles.Add(f);
                }
            }

            downloadArc.archivedfiles = new List<archivedfile>();

            foreach (archivedfile f in luaScripts)
            {
                downloadArc.archivedfiles.Add(f);
            }

            foreach (archivedfile f in extraFiles)
            {
                downloadArc.archivedfiles.Add(f);
            }

            if (includeDownloadStrings)
            {
                downloadArc.archivedfiles.Add(downloadStrings);
            }

            if (includeDialogStrings)
            {
                dialogStrings.filename = "/strings/dialogstrings.st";
                downloadArc.archivedfiles.Add(dialogStrings);
            }

            if (gameStrings.STstrings.Count < 2)
            {
                includeGameStrings = false;
            }

            if (includeGameStrings)
            {
                gameStrings.filename = "/strings/gamestrings.st";
                downloadArc.archivedfiles.Add(gameStrings);
            }


            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "lua_TEMP_FOR_COMPILING"), newTuxedoDL);
            tuxedoDL.filebytes = tuxedoDL.LuaFromFileToLuc(tuxedoDL.filebytes, Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "lua_TEMP_FOR_COMPILING"));
            File.Delete(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "lua_TEMP_FOR_COMPILING"));

            downloadArc.archivedfiles.Add(tuxedoDL);

            downloadArc.RebuildArc();

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(downloadArc.filename);
            saveFileDialog1.FileName = Path.GetFileName(downloadArc.filename);

            saveFileDialog1.Title = "Save arc file";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.Filter = "1PP archive (*.arc)|*.arc|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                downloadArc.filename = saveFileDialog1.FileName;
                File.WriteAllBytes(saveFileDialog1.FileName, downloadArc.filebytes);
            }

            tuxedoDL = null;

            byte[] filebytes = downloadArc.filebytes;
            downloadArc = new arcfile();
            downloadArc.form1 = form1;
            downloadArc.filebytes = filebytes;


            loadMission_Click(null, null);
        }

        private void textFileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (textIDUpDown.Value == 0)
            {
                textIDUpDown_ValueChanged(null, null);
            }
            else
            {
                textIDUpDown.Value = 0;
            }
        }

        private void textIDUpDown_ValueChanged(object sender, EventArgs e)
        {

            switch (textFileComboBox.SelectedItem)
            {
                case "downloadStrings.st":
                    if (textIDUpDown.Value < downloadStrings.STstrings.Count)
                    {
                        richTextST.Text = downloadStrings.STstrings[(int)textIDUpDown.Value].Replace("[newline]", "\n");
                    }
                    else
                    {
                        textIDUpDown.Value = downloadStrings.STstrings.Count - 1;
                    }
                    break;
                case "dialogStrings.st":
                    if (textIDUpDown.Value < dialogStrings.STstrings.Count)
                    {
                        richTextST.Text = dialogStrings.STstrings[(int)textIDUpDown.Value].Replace("[newline]", "\n");
                    }
                    else
                    {
                        textIDUpDown.Value = dialogStrings.STstrings.Count - 1;
                    }
                    break;
                case "gameStrings.st":
                    if (textIDUpDown.Value < gameStrings.STstrings.Count)
                    {
                        richTextST.Text = gameStrings.STstrings[(int)textIDUpDown.Value].Replace("[newline]", "\n");
                    }
                    else
                    {
                        textIDUpDown.Value = gameStrings.STstrings.Count - 1;
                    }
                    break;
            }
        }

        private void richTextST_TextChanged(object sender, EventArgs e)
        {
            switch (textFileComboBox.SelectedItem)
            {
                case "downloadStrings.st":
                    downloadStrings.STstrings[(int)textIDUpDown.Value] = richTextST.Text.Replace("\r", "").Replace("\n", "[newline]");
                    break;
                case "dialogStrings.st":
                    dialogStrings.STstrings[(int)textIDUpDown.Value] = richTextST.Text.Replace("\r", "").Replace("\n", "[newline]");
                    includeDialogStrings = true;
                    break;
                case "gameStrings.st":
                    gameStrings.STstrings[(int)textIDUpDown.Value] = richTextST.Text.Replace("\r", "").Replace("\n", "[newline]");
                    includeGameStrings = true;
                    break;
            }
        }

        private void addTextLine_Click(object sender, EventArgs e)
        {
            archivedfile strings = null;

            switch (textFileComboBox.SelectedItem)
            {
                case "downloadStrings.st":
                    strings = downloadStrings;
                    break;
                case "dialogStrings.st":
                    strings = dialogStrings;
                    break;
                case "gameStrings.st":
                    strings = gameStrings;
                    break;
            }

            if (textIDUpDown.Value == strings.STstrings.Count - 1)
            {
                downloadStrings.STstrings.Add("");
            }
            else
            {
                downloadStrings.STstrings.Insert((int)textIDUpDown.Value + 1, "");
            }

            textIDUpDown.Value++;
        }

        private void deleteTextLine_Click(object sender, EventArgs e)
        {
            switch (textFileComboBox.SelectedItem)
            {
                case "downloadStrings.st":
                    downloadStrings.STstrings.RemoveAt((int)textIDUpDown.Value);
                    break;
                case "dialogStrings.st":
                    dialogStrings.STstrings.RemoveAt((int)textIDUpDown.Value);
                    includeDialogStrings = true;
                    break;
                case "gameStrings.st":
                    gameStrings.STstrings.RemoveAt((int)textIDUpDown.Value);
                    includeGameStrings = true;
                    break;
            }

            if (textIDUpDown.Value > 0)
            {
                textIDUpDown.Value--;
            }
            else
            {
                textIDUpDown.Value++;
                textIDUpDown.Value--;
            }
        }

        private void InteractionSubtypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].interactionSubtype = (InteractAnim)InteractionAnimTypeComboBox.SelectedIndex;
        }
    }
}