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

        public List<DownloadItem> downloadItems = new List<DownloadItem>();

        public List<archivedfile> luaScripts = new List<archivedfile>();
        public List<archivedfile> rdtSprites = new List<archivedfile>();

        public MPB_TSB_EditorForm mpb_tsb_editor = new MPB_TSB_EditorForm();
       

        public MissionEditor()
        {
            InitializeComponent();
        }

        public enum InteractionType {
            NPC = 0x01,
            Door = 0x02,
            InventoryItem = 0x03,
            Uninteractable = 0x04,
            Interactable = 0x05,
            Puffle = 0x06,
            InteractionType7 = 0x07,
            SpecialObject = 0x08
        }
        public class DownloadItem {

            public int ID = 0;
            public string spritePath = "";

            public int Xpos = 0;
            public int Ypos = 0;

            public InteractionType interactionType = InteractionType.Interactable;
            public bool SpawnedByDefault = true;
            public int unk1;
            public string luaScriptPath = "";
            public int unk2;

            public int room;

            public int unk3;

            public string destinationRoom = "";

            public bool locked = false;

            public int destPosX;
            public int destPosY;

            public bool flipX = false;
            public bool flipY = false;
        }

        public void LoadFormControls() {

            DestinationRoomComboBox.Items.Add("None");

            foreach (Form1.Room r in form1.rooms)
            {
                selectedRoomBox.Items.Add(r.DisplayName);
                DestinationRoomComboBox.Items.Add(r.DisplayName);
            }

            DestinationRoomComboBox.SelectedIndex = 0;

            missionSettingsTab.Enabled = false;
            objectsTab.Enabled = false;
            luaScriptsTabPage.Enabled = false;
            textEditorTab.Enabled = false;

            mpb_tsb_editor.form1 = form1;

            ready = true;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

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

            if(selectedRoom.tilemapWidth != 0)
                {
                tilemap.known_tile_width = selectedRoom.tilemapWidth;
                }

            //Try to get tileset from download.arc if it's there. If not, fall back to vanilla.
            archivedfile tilesetArchivedFile = downloadArc.GetFileByName(tilesetPathInArc.ToLower());

            if (tilesetArchivedFile == null)
            {
                tilesetArchivedFile = mainArc.GetFileByName(tilesetPathInArc.ToLower());
            }

            tilesetArchivedFile.ReadFile();
            tileset.filebytes = tilesetArchivedFile.filebytes;

            tilemap.Load();
            tileset.Load();
            
            mpb_tsb_editor.LoadBoth();

            backgroundImageBox.Image = mpb_tsb_editor.image;
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
            tuxedoDL.DecompressFile();
            tuxedoDL.DecompileLuc(tuxedoDL.filebytes, "tuxedoDL_TEMP");

            string[] tuxedoDLdecompiled = File.ReadAllLines("tuxedoDL_TEMP");
            File.Delete("tuxedoDL_TEMP");

            //parse downloadItems (aka, the mission objects) from tuxedoDL

            downloadItems = new List<DownloadItem>();

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
            objectLuaScriptComboBox.Items.Insert(0,"None");

            luaRichText.Text = "";

            missionSettingsTab.Enabled = true;
            objectsTab.Enabled = true;
            luaScriptsTabPage.Enabled = true;
            textEditorTab.Enabled = true;

            selectedRoomBox.SelectedIndex = 0;

            AddCurrentRoomObjectsToComboBox(); //force this in case the selected index was already zero (thus not triggering the selectedindexchanged function)

            selectedRoomBox.SelectedIndex = 0;
            roomObjectsComboBox.SelectedIndex = 0;
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

            if (size >= capacityProgressBar.Maximum)
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
            ObjectIDUpDown.Value = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].ID;
            PosXUpDown.Value = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].Xpos;
            PosYUpDown.Value = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].Ypos;
            spawnedByDefault.Checked = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].SpawnedByDefault;
            interactionTypeComboBox.SelectedIndex = ((int)selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].interactionType)-1;
            FlipXCheckBox.Checked = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].flipX;
            FlipYCheckBox.Checked = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].flipY;
            Unk1UpDown.Value = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].unk1;
            Unk2UpDown.Value = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].unk2;
            Unk3UpDown.Value = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].unk3;
            LockedCheckBox.Checked = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].locked;
            destposX.Value = selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].destPosX;
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

        public void AddLuaScriptsToObjectLuaComboBox() {

            objectLuaScriptComboBox.Items.Clear();

            foreach (archivedfile f in luaScripts)
                {
                objectLuaScriptComboBox.Items.Add(Path.GetFileName(f.filename));
                }
            
            objectLuaScriptComboBox.Sorted = true;
            objectLuaScriptComboBox.Sorted = false;
            objectLuaScriptComboBox.Items.Insert(0,"None");
        }


        public void AddCurrentRoomObjectsToComboBox() {

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
        }

        private void PosYUpDown_ValueChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].Ypos = (int)PosYUpDown.Value;
        }

        private void FlipXCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].flipX = FlipXCheckBox.Checked;
        }

        private void FlipYCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].flipY = FlipYCheckBox.Checked;
        }

        private void spawnedByDefault_CheckedChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].SpawnedByDefault = spawnedByDefault.Checked;
        }

        private void Unk1UpDown_ValueChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].unk1 = (int)Unk1UpDown.Value;
        }

        private void Unk2UpDown_ValueChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].unk2 = (int)Unk2UpDown.Value;
        }

        private void Unk3UpDown_ValueChanged(object sender, EventArgs e)
        {
            selectedRoom.Objects[roomObjectsComboBox.SelectedIndex].unk3 = (int)Unk3UpDown.Value;
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
                
            }
        }

        public void UpdateLuaScriptComboBox() {
            luaScriptComboBox.Items.Clear();
            foreach (archivedfile f in luaScripts)
            {
                luaScriptComboBox.Items.Add(Path.GetFileName(f.filename));
            }
            luaScriptComboBox.Sorted = true;
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
            DialogResult result = MessageBox.Show("Are you sure you want to delete the object "+ roomObjectsComboBox.Items[roomObjectsComboBox.SelectedIndex] + "?","Are you sure?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                {
                downloadItems.Remove(selectedRoom.Objects[roomObjectsComboBox.SelectedIndex]);

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
                    selectedRoomBox_SelectedIndexChanged(null,null);
                    }
                }
        }

        private void luaScriptComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            archivedfile scriptToLoad = null;

            foreach (archivedfile f in luaScripts)
                {
                if (f.filename.ToLower() == Path.Combine("/chunks/",(string)luaScriptComboBox.Items[luaScriptComboBox.SelectedIndex]).ToLower())
                    {
                    scriptToLoad = f;
                    break;
                    }
                }

            luaScriptNameBox.Text = Path.GetFileName(scriptToLoad.filename);

            scriptToLoad.DecompileLuc(scriptToLoad.filebytes, "lua_TEMP_DECOMPILED");
            luaRichText.Text = File.ReadAllText("lua_TEMP_DECOMPILED");
            File.Delete("lua_TEMP_DECOMPILED");
        }

        private void saveLua_Click(object sender, EventArgs e)
        {
            archivedfile scriptToSave = null;

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

            File.WriteAllText("lua_TEMP_FOR_COMPILING", luaRichText.Text);
            scriptToSave.filebytes = scriptToSave.LuaFromFileToLuc(scriptToSave.filebytes, "lua_TEMP_FOR_COMPILING");
            File.Delete("lua_TEMP_FOR_COMPILING");
            AddCurrentRoomObjectsToComboBox();
        }

        private void deleteLuaScriptButton_Click(object sender, EventArgs e)
        {
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
                if (f.filename == "/chunks/newScript"+i+".luc")
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
        }

        private void addObjectButton_Click(object sender, EventArgs e)
        {
            DownloadItem newDownloadItem = new DownloadItem();

            newDownloadItem.interactionType = InteractionType.Interactable;
            newDownloadItem.spritePath = "Objects/Crate";
            newDownloadItem.room = selectedRoom.ID_for_objects;

            downloadItems.Add(newDownloadItem);
            selectedRoom.Objects.Insert(roomObjectsComboBox.SelectedIndex, newDownloadItem);

            int i = roomObjectsComboBox.SelectedIndex;

            AddCurrentRoomObjectsToComboBox();

            roomObjectsComboBox.SelectedIndex = i;
        }

        private void objectLuaScriptComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
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

        private void RDTSpritePath_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
