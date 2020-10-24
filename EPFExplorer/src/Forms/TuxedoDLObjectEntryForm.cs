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
        public TuxedoDLObjectEntryForm()
        {
            InitializeComponent();

            

            DestinationRoomComboBox.Items.Add("none");
            foreach (Form1.Room r in form1.rooms)
                {
                RoomComboBox.Items.Add(r.DisplayName);
                DestinationRoomComboBox.Items.Add(r.DisplayName);
                }
        }

        public Form1 form1;

        
        

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
            FlipXCheckBox.Checked = false;
            FlipYCheckBox.Checked = false;
        }

        private void GenerateOutput_Button_Click(object sender, EventArgs e)
        {
            string output = "";

            output += "_util.AddDownloadItem(";
            output += ObjectIDUpDown.Value.ToString() + ", ";

            if (RDTSpritePathBox.Text[0] == '/')
                {
                RDTSpritePathBox.Text = RDTSpritePathBox.Text.Substring(1, RDTSpritePathBox.Text.Length - 1);
                }

            output += "\"" + RDTSpritePathBox.Text + "\", ";

            output += PosXUpDown.Value.ToString() + ", ";
            output += PosYUpDown.Value.ToString() + ", ";

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
            output += ",";

            if (unkBool1CheckBox.Checked)
                {
                output += "true, ";
                }
            else
                {
                output += "false, ";
                }

            output += Unk1UpDown.Value.ToString() + ", ";

            output += "\"" + LuaScriptPath.Text.Replace("chunks/","scripts/").Replace(".luc",".lua").Replace("/","") + "\", ";

            output += Unk2UpDown.Value.ToString() + ", ";

            foreach (Form1.Room r in form1.rooms)
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

            output += ", ";

            output += Unk3UpDown.Value.ToString() + ", ";

            foreach (Form1.Room r in form1.rooms)
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

            output += ", ";

            if (LockedCheckBox.Checked)
            {
                output += "true, ";
            }
            else
            {
                output += "false, ";
            }

            output += destposX.Value.ToString() + ", ";
            output += destposY.Value.ToString() + ", ";

            if (FlipXCheckBox.Checked)
            {
                output += "true, ";
            }
            else
            {
                output += "false, ";
            }

            if (FlipYCheckBox.Checked)
            {
                output += "true)";
            }
            else
            {
                output += "false)";
            }

            outputBox.Text = output;

        }
    }
}
