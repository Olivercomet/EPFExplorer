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
using System.Drawing.Drawing2D;

namespace EPFExplorer
{
    public partial class SpriteEditor : Form
    {
        public SpriteEditor()
        {
            InitializeComponent();
        }



        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            sprite.spriteEditor = null;

            base.OnClosing(e);
        }

        public int curFrame = 0;
        public archivedfile sprite;
        public List<rdtSubfileData> images = new List<rdtSubfileData>();
        public List<rdtSubfileData> palettes = new List<rdtSubfileData>();

        public void RequestSpriteEditorImage(int frame) {

            images[frame].LoadImage(sprite.GetPalette(palettes[frame].filebytes, 1, sprite.RDTSpriteBPP));
            ImageBox.Image = images[frame].image;
            curFrameDisplay.Text = "Frame " + (frame+1) + " / "+sprite.RDTSpriteNumFrames;
        }
        private void OAMSpriteCheckbox_CheckedChanged(object sender, EventArgs e)
        {

        }

  
        private void ImageBox_Click(object sender, EventArgs e)
        {

        }

        private void nextFrame_Click(object sender, EventArgs e)
        {
            if (loopingCheckbox.Checked && curFrame >= sprite.RDTSpriteNumFrames - 1) { curFrame = -1; }

            if (curFrame < sprite.RDTSpriteNumFrames - 1)
            {
                curFrame++;
                RequestSpriteEditorImage(curFrame);
            }
        }

        private void prevFrame_Click(object sender, EventArgs e)
        {
            if (loopingCheckbox.Checked && curFrame <= 0) { curFrame = sprite.RDTSpriteNumFrames; }

            if (curFrame > 0)
                {
                    curFrame--;
                    RequestSpriteEditorImage(curFrame); 
                }
        }
    }
}
