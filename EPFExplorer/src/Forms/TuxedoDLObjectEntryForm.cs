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
    public partial class TuxedoDLObjectEntryForm : Form
    {
        List<Room> rooms = new List<Room>();

        public TuxedoDLObjectEntryForm()
        {
            InitializeComponent();

            rooms.Add(new Room("Beach", "BEACH0", 60));
            rooms.Add(new Room("Beacon","BEACON0",83));
            rooms.Add(new Room("Boiler Room", "BOILERROOM0", 93));
            rooms.Add(new Room("Book Room", "BOOKROOM0", 107));
            rooms.Add(new Room("Coffee Shop", "COFFEESHOP0", 61));
            rooms.Add(new Room("Command Room", "COMMANDROOM0", 62));
            rooms.Add(new Room("Dock", "DOCK0", 63));
            rooms.Add(new Room("Dojo", "DOJO0", 64));
            rooms.Add(new Room("Fishing Pond", "FISHING0", 108));
            rooms.Add(new Room("Forest", "FOREST0", 102));
            rooms.Add(new Room("Gadget Room", "GADGETROOM0", 65));
            rooms.Add(new Room("Gary's Room", "GARYSROOM0", 66));
            rooms.Add(new Room("Gift Shop", "GIFTSHOP0", 98));
            rooms.Add(new Room("Gift Shop Office", "GIFTOFFICE0", 99));
            rooms.Add(new Room("Gift Shop Roof", "GIFTROOF0", 100));
            rooms.Add(new Room("HQ", "HEADQUARTERS0", 67));
            rooms.Add(new Room("Lighthouse", "LIGHTHOUSE0", 82));
            rooms.Add(new Room("Lodge Attic", "ATTIC0", 109));
            rooms.Add(new Room("Lounge", "LOUNGE0", 106));
            rooms.Add(new Room("Mine Shed", "MINESHED0", 72));
            rooms.Add(new Room("Mine Shack", "MINESHACK0", 68));
            rooms.Add(new Room("Mine", "MINEINTERIOR0", 69));
            rooms.Add(new Room("Night Club", "NIGHTCLUB0", 94));
            rooms.Add(new Room("Pet Shop", "PETSHOP0", 74));
            rooms.Add(new Room("Pizza Parlor", "PIZZAPARLOR0", 75));
            rooms.Add(new Room("Plaza", "PLAZA0", 76));
            rooms.Add(new Room("Puffle Training Room", "PUFFLETRAINING0", 77));
            rooms.Add(new Room("Ski Hill", "SKIHILL0", 73));
            rooms.Add(new Room("Ski Lodge", "LODGE0", 90));
            rooms.Add(new Room("Ski Village", "SKIVILLAGE0", 78));
            rooms.Add(new Room("Sport Shop", "SPORTSHOP0", 80));
            rooms.Add(new Room("Stage", "STAGE0", 95));
            rooms.Add(new Room("Town", "TOWN0", 81));
            rooms.Add(new Room("Underground Pool", "POOL0", 92));



            DestinationRoomComboBox.Items.Add("none");
            foreach (Room r in rooms)
                {
                RoomComboBox.Items.Add(r.DisplayName);
                DestinationRoomComboBox.Items.Add(r.DisplayName);
                }
        }


        
        public class Room {

            public string DisplayName;
            public string InternalName;
            public int ID_for_objects;

            public Room(string _DisplayName, string _InternalName, int _ID_for_objects) {
                DisplayName = _DisplayName;
                InternalName = _InternalName;
                ID_for_objects = _ID_for_objects;
            }
        }

        private void FillWithExampleValues_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("This will overwrite the information you have entered. Continue?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result != DialogResult.Yes)
                {
                return;
                }

            ObjectIDUpDown.Value = 34503;
            RDTSpritePathBox.Text = "NPC/Rookie/Rookie";
            PosXUpDown.Value = 816;
            PosYUpDown.Value = 166;
            interactionTypeComboBox.SelectedItem = "NPC";
            unkBool1CheckBox.Checked = true;
            Unk1UpDown.Value = 0;
            LuaScriptPath.Text = "chunks/HQ0_NPC_Rookie.luc";
            Unk2UpDown.Value = 300;
            RoomComboBox.SelectedItem = "HQ";
            Unk3UpDown.Value = 0;
            DestinationRoomComboBox.SelectedIndex = 0;
            LockedCheckBox.Checked = false;
            destposX.Value = 0;
            destposY.Value = 0;
            UnkBool2CheckBox.Checked = false;
            UnkBool3CheckBox.Checked = false;
        }

        private void GenerateOutput_Button_Click(object sender, EventArgs e)
        {
            string output = "";

            output += "L0 = _util\n";
            output += "L0 = L0.AddDownloadItem\n";
            output += "L1 = " + ObjectIDUpDown.Value.ToString() + "\n";

            if (RDTSpritePathBox.Text[0] == '/')
                {
                RDTSpritePathBox.Text = RDTSpritePathBox.Text.Substring(1, RDTSpritePathBox.Text.Length - 1);
                }

            output += "L2 = " + "\"" + RDTSpritePathBox.Text + "\"\n";

            output += "L3 = " + PosXUpDown.Value.ToString() + "\n";
            output += "L4 = " + PosYUpDown.Value.ToString() + "\n";

            output += "L5 = ";

            switch ((string)interactionTypeComboBox.SelectedItem)
                {
                case "NPC":
                    output += "1";
                    break;
                case "Door":
                    output += "2";
                    break;
                case "Inventory Item":
                    output += "3";
                    break;
                case "Uninteractable":
                    output += "4";
                    break;
                case "Normal Object":
                    output += "5";
                    break;
                default:
                    output += "4";
                    break;
                }
            output += "\n";

            if (unkBool1CheckBox.Checked)
                {
                output += "L6 = true\n";
                }
            else
                {
                output += "L6 = false\n";
                }

            output += "L7 = " + Unk1UpDown.Value.ToString() + "\n";

            output += "L8 = " + "\"" + LuaScriptPath.Text.Replace("chunks/","scripts/").Replace(".luc",".lua").Replace("/","") + "\"\n";

            output += "L9 = " + Unk2UpDown.Value.ToString() + "\n";

            output += "L10 = ";

            foreach (Room r in rooms)
                {
                if (r.DisplayName == (string)RoomComboBox.SelectedItem)
                    {
                    output += r.ID_for_objects;
                    break;
                    }
                }

            if (output[output.Length - 1] == ' ') //if the room didn't find a match
                {
                output += "67"; //just put it in HQ
                }

            output += "\n";

            output += "L11 = " + Unk3UpDown.Value.ToString() + "\n";

            output += "L12 = ";

            foreach (Room r in rooms)
            {
                if (r.DisplayName == (string)DestinationRoomComboBox.SelectedItem)
                {
                    output += "\""+r.InternalName+"\"";
                    break;
                }
            }

            if (output[output.Length - 1] == ' ') //if the destination room didn't find a match
            {
                output += "\"\""; //just put it as none
            }

            output += "\n";

            if (LockedCheckBox.Checked)
            {
                output += "L13 = true\n";
            }
            else
            {
                output += "L13 = false\n";
            }

            output += "L14 = " + destposX.Value.ToString() + "\n";
            output += "L15 = " + destposY.Value.ToString() + "\n";

            if (UnkBool2CheckBox.Checked)
            {
                output += "L16 = true\n";
            }
            else
            {
                output += "L16 = false\n";
            }

            if (UnkBool3CheckBox.Checked)
            {
                output += "L17 = true\n";
            }
            else
            {
                output += "L17 = false\n";
            }

            output += "L0(L1, L2, L3, L4, L5, L6, L7, L8, L9, L10, L11, L12, L13, L14, L15, L16, L17)\n";

            outputBox.Text = output;

        }
    }
}
