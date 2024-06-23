using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EPFExplorer
{
    public partial class ChooseAlphaColourWindow : Form
    {

        public Form1 form1;
        public List<Color> colors;
        public ChooseAlphaColourWindow()
        {
            InitializeComponent();
        }

        public void Init()
        {
            if (colors.Count > 0)
            {
                colorBox.BackColor = colors[form1.alphaColorIndexForGifImport];
            }
        }

        private void chooseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void increaseButton_Click(object sender, EventArgs e)
        {
            if (form1.alphaColorIndexForGifImport < colors.Count - 1)
            {
                form1.alphaColorIndexForGifImport++;
            }

            colorBox.BackColor = colors[form1.alphaColorIndexForGifImport];
        }

        private void decreaseButton_Click(object sender, EventArgs e)
        {
            if (form1.alphaColorIndexForGifImport > 0)
            {
                form1.alphaColorIndexForGifImport--;
            }

            colorBox.BackColor = colors[form1.alphaColorIndexForGifImport];
        }
    }
}
