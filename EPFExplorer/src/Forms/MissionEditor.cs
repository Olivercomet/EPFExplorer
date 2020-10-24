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
    public partial class MissionEditor : Form
    {
        public Form1 form1;

        public arcfile mainArc;
        public arcfile downloadArc;

        public rdtfile gameRdt;
        public rdtfile downloadRdt;

        public MissionEditor()
        {
            InitializeComponent();

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
    }
}
