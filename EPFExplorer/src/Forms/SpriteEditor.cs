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

            if (images[frame].image == null)
                {
                images[frame].LoadImage(sprite.GetPalette(palettes[frame].filebytes, 1, sprite.RDTSpriteBPP));
                }
            
            if (sprite.RDTSpriteBPP == 8)
                {
                BPP_8_radioButton.Checked = true;    
                }
            else
                {
                BPP_4_radioButton.Checked = true;
                }
            ImageBox.Image = images[frame].image;
            curFrameDisplay.Text = "Frame " + (frame+1) + " / "+sprite.RDTSpriteNumFrames;
            durationBox.Value = sprite.RDTSpriteFrameDurations[curFrame];
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

        private void deleteFrame_Click(object sender, EventArgs e)
        {
            if (images.Count < 2)
                {
                MessageBox.Show("You can't have zero frames!", "Cannot delete frame", MessageBoxButtons.OK);
                return;
                }

            images.RemoveAt(curFrame);
            palettes.RemoveAt(curFrame);
            sprite.RDTSpriteFrameDurations.RemoveAt(curFrame);

            if (curFrame >= images.Count)
                {
                curFrame = images.Count - 1;
                }

            sprite.RDTSpriteNumFrames--;

            SendUpdateToRDT();
        }


        public void SendUpdateToRDT() {

            //remove the existing images and palettes in the archivedfile
            int indexOfFirstImageOrPalette = 0;

            foreach (rdtSubfileData f in sprite.rdtSubfileDataList)
            {
                if (f.graphicsType != "image" || f.graphicsType != "palette")
                {
                    indexOfFirstImageOrPalette++;
                    continue;
                }
                break;
            }

            sprite.rdtSubfileDataList.RemoveRange(indexOfFirstImageOrPalette, sprite.rdtSubfileDataList.Count - indexOfFirstImageOrPalette);


            //readd the edited ones

            for (int i = 0; i < images.Count; i++)
            {
                sprite.rdtSubfileDataList.Add(palettes[i]);
                sprite.rdtSubfileDataList.Add(images[i]);
            }

            RequestSpriteEditorImage(curFrame);
        }

        private void durationBox_ValueChanged(object sender, EventArgs e)
        {
            sprite.RDTSpriteFrameDurations[curFrame] = (ushort)durationBox.Value;
            SendUpdateToRDT();
        }

        private void replaceFrame_Click(object sender, EventArgs e)
        {
            GetNewFrameImageFromFile(false);
        }


        public void GetNewFrameImageFromFile(bool addingFrame) {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select image file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.Filter = "png (*.png)|*.png";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                images[curFrame].image = Image.FromFile(openFileDialog1.FileName);
                //need to set palette as well
                }
            else
                {
                if (addingFrame)    //if the file dialog was aborted, we undo the addition of the new frame here
                    {
                    images.RemoveAt(curFrame);
                    palettes.RemoveAt(curFrame);
                    sprite.RDTSpriteFrameDurations.RemoveAt(curFrame);
                    sprite.RDTSpriteNumFrames--;
                    curFrame--;
                    }
                }

            SendUpdateToRDT();
        }

        private void addFrame_Click(object sender, EventArgs e)
        {
            sprite.RDTSpriteNumFrames++;
            curFrame++;

            rdtSubfileData newSubFileData = new rdtSubfileData(images[curFrame - 1]);

            images.Insert(curFrame,newSubFileData);   //create new dummy image for this to fill
            palettes.Insert(curFrame, palettes[curFrame - 1]); //create new dummy image for this to fill
            sprite.RDTSpriteFrameDurations.Insert(curFrame, sprite.RDTSpriteFrameDurations[curFrame - 1]);

            GetNewFrameImageFromFile(true);
        }

        private void exportFrame_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Title = "Save frame";
            saveFileDialog1.FileName = Path.GetFileName(sprite.filename+"_"+(curFrame+1));
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = ".png";
            saveFileDialog1.Filter = "png (*.png)|*.png";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                images[curFrame].image.Save(saveFileDialog1.FileName);
            }
        }
    }
}
