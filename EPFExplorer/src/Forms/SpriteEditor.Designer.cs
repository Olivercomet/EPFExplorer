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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpriteEditor));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.exportRawFilesButton = new System.Windows.Forms.Button();
            this.BPP_8_radioButton = new System.Windows.Forms.RadioButton();
            this.BPP_4_radioButton = new System.Windows.Forms.RadioButton();
            this.labelBPP = new System.Windows.Forms.Label();
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
            this.label10 = new System.Windows.Forms.Label();
            this.alphaColourNext = new System.Windows.Forms.Button();
            this.offsetYUpDown = new System.Windows.Forms.NumericUpDown();
            this.alphaColourPrev = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.alphacolourdisplay = new System.Windows.Forms.PictureBox();
            this.offsetXUpDown = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.exportFrame = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.durationBox = new System.Windows.Forms.NumericUpDown();
            this.replaceFrame = new System.Windows.Forms.Button();
            this.addFrame = new System.Windows.Forms.Button();
            this.deleteFrame = new System.Windows.Forms.Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.offsetYUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alphacolourdisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetXUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.durationBox)).BeginInit();
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
            this.groupBox1.Controls.Add(this.exportRawFilesButton);
            this.groupBox1.Controls.Add(this.BPP_8_radioButton);
            this.groupBox1.Controls.Add(this.BPP_4_radioButton);
            this.groupBox1.Controls.Add(this.labelBPP);
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
            this.groupBox1.Size = new System.Drawing.Size(266, 246);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General Settings";
            // 
            // exportRawFilesButton
            // 
            this.exportRawFilesButton.Location = new System.Drawing.Point(124, 69);
            this.exportRawFilesButton.Name = "exportRawFilesButton";
            this.exportRawFilesButton.Size = new System.Drawing.Size(123, 31);
            this.exportRawFilesButton.TabIndex = 21;
            this.exportRawFilesButton.Text = "Export raw files";
            this.exportRawFilesButton.UseVisualStyleBackColor = true;
            this.exportRawFilesButton.Click += new System.EventHandler(this.exportRawFilesButton_Click);
            // 
            // BPP_8_radioButton
            // 
            this.BPP_8_radioButton.AutoSize = true;
            this.BPP_8_radioButton.Location = new System.Drawing.Point(168, 214);
            this.BPP_8_radioButton.Name = "BPP_8_radioButton";
            this.BPP_8_radioButton.Size = new System.Drawing.Size(61, 21);
            this.BPP_8_radioButton.TabIndex = 20;
            this.BPP_8_radioButton.TabStop = true;
            this.BPP_8_radioButton.Text = "8bpp";
            this.BPP_8_radioButton.UseVisualStyleBackColor = true;
            this.BPP_8_radioButton.CheckedChanged += new System.EventHandler(this.BPP_8_radioButton_CheckedChanged);
            // 
            // BPP_4_radioButton
            // 
            this.BPP_4_radioButton.AutoSize = true;
            this.BPP_4_radioButton.Location = new System.Drawing.Point(101, 214);
            this.BPP_4_radioButton.Name = "BPP_4_radioButton";
            this.BPP_4_radioButton.Size = new System.Drawing.Size(61, 21);
            this.BPP_4_radioButton.TabIndex = 19;
            this.BPP_4_radioButton.TabStop = true;
            this.BPP_4_radioButton.Text = "4bpp";
            this.BPP_4_radioButton.UseVisualStyleBackColor = true;
            this.BPP_4_radioButton.CheckedChanged += new System.EventHandler(this.BPP_4_radioButton_CheckedChanged);
            // 
            // labelBPP
            // 
            this.labelBPP.AutoSize = true;
            this.labelBPP.Location = new System.Drawing.Point(6, 216);
            this.labelBPP.Name = "labelBPP";
            this.labelBPP.Size = new System.Drawing.Size(89, 17);
            this.labelBPP.TabIndex = 18;
            this.labelBPP.Text = "Colour depth";
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
            this.boundsY2.ValueChanged += new System.EventHandler(this.boundsY2_ValueChanged);
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
            this.boundsX2.ValueChanged += new System.EventHandler(this.boundsX2_ValueChanged);
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
            this.boundsY.ValueChanged += new System.EventHandler(this.boundsY_ValueChanged);
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
            this.boundsX.ValueChanged += new System.EventHandler(this.boundsX_ValueChanged);
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
            this.centreY.ValueChanged += new System.EventHandler(this.centreY_ValueChanged);
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
            this.centreX.ValueChanged += new System.EventHandler(this.centreX_ValueChanged);
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
            this.rotatableCheckbox.CheckedChanged += new System.EventHandler(this.rotatableCheckbox_CheckedChanged);
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
            this.loopingCheckbox.CheckedChanged += new System.EventHandler(this.loopingCheckbox_CheckedChanged);
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
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.alphaColourNext);
            this.groupBox2.Controls.Add(this.offsetYUpDown);
            this.groupBox2.Controls.Add(this.alphaColourPrev);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.alphacolourdisplay);
            this.groupBox2.Controls.Add(this.offsetXUpDown);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.exportFrame);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.durationBox);
            this.groupBox2.Controls.Add(this.replaceFrame);
            this.groupBox2.Controls.Add(this.addFrame);
            this.groupBox2.Controls.Add(this.deleteFrame);
            this.groupBox2.Controls.Add(this.curFrameDisplay);
            this.groupBox2.Controls.Add(this.nextFrame);
            this.groupBox2.Controls.Add(this.prevFrame);
            this.groupBox2.Location = new System.Drawing.Point(616, 294);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(266, 340);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Animation";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(165, 118);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 17);
            this.label10.TabIndex = 26;
            this.label10.Text = "Y";
            // 
            // alphaColourNext
            // 
            this.alphaColourNext.Location = new System.Drawing.Point(180, 301);
            this.alphaColourNext.Name = "alphaColourNext";
            this.alphaColourNext.Size = new System.Drawing.Size(27, 31);
            this.alphaColourNext.TabIndex = 12;
            this.alphaColourNext.Text = ">";
            this.alphaColourNext.UseVisualStyleBackColor = true;
            this.alphaColourNext.Click += new System.EventHandler(this.alphaColourNext_Click);
            // 
            // offsetYUpDown
            // 
            this.offsetYUpDown.Location = new System.Drawing.Point(188, 118);
            this.offsetYUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.offsetYUpDown.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.offsetYUpDown.Name = "offsetYUpDown";
            this.offsetYUpDown.Size = new System.Drawing.Size(59, 22);
            this.offsetYUpDown.TabIndex = 25;
            this.offsetYUpDown.ValueChanged += new System.EventHandler(this.offsetYUpDown_ValueChanged);
            // 
            // alphaColourPrev
            // 
            this.alphaColourPrev.Location = new System.Drawing.Point(105, 301);
            this.alphaColourPrev.Name = "alphaColourPrev";
            this.alphaColourPrev.Size = new System.Drawing.Size(27, 31);
            this.alphaColourPrev.TabIndex = 11;
            this.alphaColourPrev.Text = "<";
            this.alphaColourPrev.UseVisualStyleBackColor = true;
            this.alphaColourPrev.Click += new System.EventHandler(this.alphaColourPrev_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(71, 120);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 17);
            this.label11.TabIndex = 24;
            this.label11.Text = "X";
            // 
            // alphacolourdisplay
            // 
            this.alphacolourdisplay.Location = new System.Drawing.Point(138, 301);
            this.alphacolourdisplay.Name = "alphacolourdisplay";
            this.alphacolourdisplay.Size = new System.Drawing.Size(36, 31);
            this.alphacolourdisplay.TabIndex = 10;
            this.alphacolourdisplay.TabStop = false;
            this.alphacolourdisplay.Click += new System.EventHandler(this.alphacolourdisplay_Click);
            // 
            // offsetXUpDown
            // 
            this.offsetXUpDown.Location = new System.Drawing.Point(94, 120);
            this.offsetXUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.offsetXUpDown.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.offsetXUpDown.Name = "offsetXUpDown";
            this.offsetXUpDown.Size = new System.Drawing.Size(65, 22);
            this.offsetXUpDown.TabIndex = 23;
            this.offsetXUpDown.ValueChanged += new System.EventHandler(this.offsetXUpDown_ValueChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(16, 120);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(46, 17);
            this.label12.TabIndex = 22;
            this.label12.Text = "Offset";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(54, 298);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 34);
            this.label9.TabIndex = 9;
            this.label9.Text = "Alpha\r\ncolour";
            // 
            // exportFrame
            // 
            this.exportFrame.Location = new System.Drawing.Point(57, 157);
            this.exportFrame.Name = "exportFrame";
            this.exportFrame.Size = new System.Drawing.Size(156, 30);
            this.exportFrame.TabIndex = 8;
            this.exportFrame.Text = "Export frame";
            this.exportFrame.UseVisualStyleBackColor = true;
            this.exportFrame.Click += new System.EventHandler(this.exportFrame_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(62, 72);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 34);
            this.label8.TabIndex = 7;
            this.label8.Text = "Frame\r\nduration";
            // 
            // durationBox
            // 
            this.durationBox.Location = new System.Drawing.Point(131, 79);
            this.durationBox.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.durationBox.Name = "durationBox";
            this.durationBox.Size = new System.Drawing.Size(82, 22);
            this.durationBox.TabIndex = 6;
            this.durationBox.ValueChanged += new System.EventHandler(this.durationBox_ValueChanged);
            // 
            // replaceFrame
            // 
            this.replaceFrame.Location = new System.Drawing.Point(57, 193);
            this.replaceFrame.Name = "replaceFrame";
            this.replaceFrame.Size = new System.Drawing.Size(156, 30);
            this.replaceFrame.TabIndex = 5;
            this.replaceFrame.Text = "Replace frame";
            this.replaceFrame.UseVisualStyleBackColor = true;
            this.replaceFrame.Click += new System.EventHandler(this.replaceFrame_Click);
            // 
            // addFrame
            // 
            this.addFrame.Location = new System.Drawing.Point(57, 265);
            this.addFrame.Name = "addFrame";
            this.addFrame.Size = new System.Drawing.Size(156, 30);
            this.addFrame.TabIndex = 4;
            this.addFrame.Text = "Add new frame";
            this.addFrame.UseVisualStyleBackColor = true;
            this.addFrame.Click += new System.EventHandler(this.addFrame_Click);
            // 
            // deleteFrame
            // 
            this.deleteFrame.Location = new System.Drawing.Point(57, 229);
            this.deleteFrame.Name = "deleteFrame";
            this.deleteFrame.Size = new System.Drawing.Size(156, 30);
            this.deleteFrame.TabIndex = 3;
            this.deleteFrame.Text = "Delete frame";
            this.deleteFrame.UseVisualStyleBackColor = true;
            this.deleteFrame.Click += new System.EventHandler(this.deleteFrame_Click);
            // 
            // curFrameDisplay
            // 
            this.curFrameDisplay.AutoSize = true;
            this.curFrameDisplay.Location = new System.Drawing.Point(102, 18);
            this.curFrameDisplay.Name = "curFrameDisplay";
            this.curFrameDisplay.Size = new System.Drawing.Size(60, 17);
            this.curFrameDisplay.TabIndex = 2;
            this.curFrameDisplay.Text = "Frame 0";
            // 
            // nextFrame
            // 
            this.nextFrame.Location = new System.Drawing.Point(138, 38);
            this.nextFrame.Name = "nextFrame";
            this.nextFrame.Size = new System.Drawing.Size(75, 23);
            this.nextFrame.TabIndex = 1;
            this.nextFrame.Text = ">";
            this.nextFrame.UseVisualStyleBackColor = true;
            this.nextFrame.Click += new System.EventHandler(this.nextFrame_Click);
            // 
            // prevFrame
            // 
            this.prevFrame.Location = new System.Drawing.Point(57, 38);
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
            this.ImageBox.Size = new System.Drawing.Size(598, 592);
            this.ImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ImageBox.TabIndex = 4;
            this.ImageBox.TabStop = false;
            // 
            // SpriteEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 646);
            this.Controls.Add(this.ImageBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
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
            ((System.ComponentModel.ISupportInitialize)(this.offsetYUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alphacolourdisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetXUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.durationBox)).EndInit();
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
        private System.Windows.Forms.Button addFrame;
        private System.Windows.Forms.Button deleteFrame;
        private System.Windows.Forms.Button replaceFrame;
        private System.Windows.Forms.Label labelBPP;
        private System.Windows.Forms.RadioButton BPP_4_radioButton;
        private System.Windows.Forms.RadioButton BPP_8_radioButton;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown durationBox;
        private System.Windows.Forms.Button exportFrame;
        private System.Windows.Forms.PictureBox alphacolourdisplay;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button alphaColourNext;
        private System.Windows.Forms.Button alphaColourPrev;
        private System.Windows.Forms.Button exportRawFilesButton;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.NumericUpDown offsetYUpDown;
        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.NumericUpDown offsetXUpDown;
        private System.Windows.Forms.Label label12;
    }
}