namespace EPFExplorer
{
    partial class SpriteEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.boundsY2 = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.boundsX2 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.boundsY = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.boundsX = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.centreY = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.centreX = new System.Windows.Forms.NumericUpDown();
            this.Centre = new System.Windows.Forms.Label();
            this.rotatableCheckbox = new System.Windows.Forms.CheckBox();
            this.loopingCheckbox = new System.Windows.Forms.CheckBox();
            this.OAMSpriteCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.curFrameDisplay = new System.Windows.Forms.Label();
            this.nextFrame = new System.Windows.Forms.Button();
            this.prevFrame = new System.Windows.Forms.Button();
            this.ImageBox = new RedCell.UI.Controls.PixelBox();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.boundsY2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.boundsX2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.boundsY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.boundsX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.centreY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.centreX)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(894, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.boundsY2);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.boundsX2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.boundsY);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.boundsX);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.centreY);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.centreX);
            this.groupBox1.Controls.Add(this.Centre);
            this.groupBox1.Controls.Add(this.rotatableCheckbox);
            this.groupBox1.Controls.Add(this.loopingCheckbox);
            this.groupBox1.Controls.Add(this.OAMSpriteCheckbox);
            this.groupBox1.Location = new System.Drawing.Point(616, 42);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(266, 204);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General Settings";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(161, 174);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(25, 17);
            this.label6.TabIndex = 17;
            this.label6.Text = "Y2";
            // 
            // boundsY2
            // 
            this.boundsY2.Location = new System.Drawing.Point(188, 174);
            this.boundsY2.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.boundsY2.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.boundsY2.Name = "boundsY2";
            this.boundsY2.Size = new System.Drawing.Size(59, 22);
            this.boundsY2.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(63, 176);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 17);
            this.label7.TabIndex = 15;
            this.label7.Text = "X2";
            // 
            // boundsX2
            // 
            this.boundsX2.Location = new System.Drawing.Point(94, 176);
            this.boundsX2.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.boundsX2.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.boundsX2.Name = "boundsX2";
            this.boundsX2.Size = new System.Drawing.Size(65, 22);
            this.boundsX2.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(165, 146);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 17);
            this.label4.TabIndex = 13;
            this.label4.Text = "Y";
            // 
            // boundsY
            // 
            this.boundsY.Location = new System.Drawing.Point(188, 146);
            this.boundsY.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.boundsY.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.boundsY.Name = "boundsY";
            this.boundsY.Size = new System.Drawing.Size(59, 22);
            this.boundsY.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(71, 148);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 17);
            this.label5.TabIndex = 11;
            this.label5.Text = "X";
            // 
            // boundsX
            // 
            this.boundsX.Location = new System.Drawing.Point(94, 148);
            this.boundsX.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.boundsX.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.boundsX.Name = "boundsX";
            this.boundsX.Size = new System.Drawing.Size(65, 22);
            this.boundsX.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 148);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "Bounds";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(165, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Y";
            // 
            // centreY
            // 
            this.centreY.Location = new System.Drawing.Point(188, 109);
            this.centreY.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.centreY.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.centreY.Name = "centreY";
            this.centreY.Size = new System.Drawing.Size(59, 22);
            this.centreY.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(71, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "X";
            // 
            // centreX
            // 
            this.centreX.Location = new System.Drawing.Point(94, 111);
            this.centreX.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.centreX.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.centreX.Name = "centreX";
            this.centreX.Size = new System.Drawing.Size(65, 22);
            this.centreX.TabIndex = 4;
            // 
            // Centre
            // 
            this.Centre.AutoSize = true;
            this.Centre.Location = new System.Drawing.Point(6, 111);
            this.Centre.Name = "Centre";
            this.Centre.Size = new System.Drawing.Size(50, 17);
            this.Centre.TabIndex = 3;
            this.Centre.Text = "Centre";
            // 
            // rotatableCheckbox
            // 
            this.rotatableCheckbox.AutoSize = true;
            this.rotatableCheckbox.Location = new System.Drawing.Point(6, 75);
            this.rotatableCheckbox.Name = "rotatableCheckbox";
            this.rotatableCheckbox.Size = new System.Drawing.Size(91, 21);
            this.rotatableCheckbox.TabIndex = 2;
            this.rotatableCheckbox.Text = "Rotatable";
            this.rotatableCheckbox.UseVisualStyleBackColor = true;
            // 
            // loopingCheckbox
            // 
            this.loopingCheckbox.AutoSize = true;
            this.loopingCheckbox.Location = new System.Drawing.Point(6, 48);
            this.loopingCheckbox.Name = "loopingCheckbox";
            this.loopingCheckbox.Size = new System.Drawing.Size(81, 21);
            this.loopingCheckbox.TabIndex = 1;
            this.loopingCheckbox.Text = "Looping";
            this.loopingCheckbox.UseVisualStyleBackColor = true;
            // 
            // OAMSpriteCheckbox
            // 
            this.OAMSpriteCheckbox.AutoSize = true;
            this.OAMSpriteCheckbox.Location = new System.Drawing.Point(6, 21);
            this.OAMSpriteCheckbox.Name = "OAMSpriteCheckbox";
            this.OAMSpriteCheckbox.Size = new System.Drawing.Size(116, 21);
            this.OAMSpriteCheckbox.TabIndex = 0;
            this.OAMSpriteCheckbox.Text = "Is OAM Sprite";
            this.OAMSpriteCheckbox.UseVisualStyleBackColor = true;
            this.OAMSpriteCheckbox.CheckedChanged += new System.EventHandler(this.OAMSpriteCheckbox_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.curFrameDisplay);
            this.groupBox2.Controls.Add(this.nextFrame);
            this.groupBox2.Controls.Add(this.prevFrame);
            this.groupBox2.Location = new System.Drawing.Point(616, 252);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(266, 262);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Animation";
            // 
            // curFrameDisplay
            // 
            this.curFrameDisplay.AutoSize = true;
            this.curFrameDisplay.Location = new System.Drawing.Point(99, 41);
            this.curFrameDisplay.Name = "curFrameDisplay";
            this.curFrameDisplay.Size = new System.Drawing.Size(60, 17);
            this.curFrameDisplay.TabIndex = 2;
            this.curFrameDisplay.Text = "Frame 0";
            // 
            // nextFrame
            // 
            this.nextFrame.Location = new System.Drawing.Point(135, 61);
            this.nextFrame.Name = "nextFrame";
            this.nextFrame.Size = new System.Drawing.Size(75, 23);
            this.nextFrame.TabIndex = 1;
            this.nextFrame.Text = ">";
            this.nextFrame.UseVisualStyleBackColor = true;
            this.nextFrame.Click += new System.EventHandler(this.nextFrame_Click);
            // 
            // prevFrame
            // 
            this.prevFrame.Location = new System.Drawing.Point(54, 61);
            this.prevFrame.Name = "prevFrame";
            this.prevFrame.Size = new System.Drawing.Size(75, 23);
            this.prevFrame.TabIndex = 0;
            this.prevFrame.Text = "<";
            this.prevFrame.UseVisualStyleBackColor = true;
            this.prevFrame.Click += new System.EventHandler(this.prevFrame_Click);
            // 
            // ImageBox
            // 
            this.ImageBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.ImageBox.Location = new System.Drawing.Point(12, 42);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(598, 472);
            this.ImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ImageBox.TabIndex = 4;
            this.ImageBox.TabStop = false;
            // 
            // SpriteEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 536);
            this.Controls.Add(this.ImageBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SpriteEditor";
            this.Text = "EPFExplorer Sprite Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.boundsY2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.boundsX2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.boundsY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.boundsX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.centreY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.centreX)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label Centre;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        public RedCell.UI.Controls.PixelBox ImageBox;
        private System.Windows.Forms.Button nextFrame;
        private System.Windows.Forms.Button prevFrame;
        private System.Windows.Forms.Label curFrameDisplay;
        public System.Windows.Forms.CheckBox OAMSpriteCheckbox;
        public System.Windows.Forms.CheckBox loopingCheckbox;
        public System.Windows.Forms.CheckBox rotatableCheckbox;
        public System.Windows.Forms.NumericUpDown centreX;
        public System.Windows.Forms.NumericUpDown centreY;
        public System.Windows.Forms.NumericUpDown boundsX;
        public System.Windows.Forms.NumericUpDown boundsY;
        public System.Windows.Forms.NumericUpDown boundsX2;
        public System.Windows.Forms.NumericUpDown boundsY2;
    }
}