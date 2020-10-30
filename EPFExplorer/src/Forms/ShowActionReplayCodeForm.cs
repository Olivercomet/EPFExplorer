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
    public partial class ShowActionReplayCodeForm : Form
    {
        public ShowActionReplayCodeForm()
        {
            InitializeComponent();
        }

        public void SetInfo(string ARcode, string titleText, string forText, string sideEffectsText) {
            richTextBox1.Text = ARcode;
            TitleLabel.Text = titleText;
            ForLabel.Text = forText;
            sideEffectsLabel.Text = sideEffectsText;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
