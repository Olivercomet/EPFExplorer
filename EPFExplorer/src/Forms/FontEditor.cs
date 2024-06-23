using System;
using System.IO;
using System.Windows.Forms;

namespace EPFExplorer
{
    public partial class FontEditor : Form
    {
        public FontEditor()
        {
            InitializeComponent();
        }

        FNTfile activeFont;
        FNTfile.letter selectedLetter;

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select font file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "1PP font data (*.fnt)|*.fnt";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                activeFont = new FNTfile();
                activeFont.filebytes = File.ReadAllBytes(openFileDialog1.FileName);

                activeFont.Load();

                listBox1.Items.Clear();

                foreach (FNTfile.letter l in activeFont.letters)
                {
                    listBox1.Items.Add(l.name);
                }
            }
        }

        public void SelectLetterForEdit(char letter)
        {

            foreach (FNTfile.letter l in activeFont.letters)
            {
                if (l.name == letter)
                {
                    selectedLetter = l;
                    pixelBox1.Image = selectedLetter.images[0];

                    break;
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (activeFont == null)
            {
                return;
            }

            SelectLetterForEdit(listBox1.SelectedItem.ToString()[0]);
        }
    }
}
