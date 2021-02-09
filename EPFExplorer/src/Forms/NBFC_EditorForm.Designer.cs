namespace EPFExplorer
{
    partial class NBFC_EditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NBFC_EditorForm));
            this.pixelBox1 = new RedCell.UI.Controls.PixelBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.choosePalette = new System.Windows.Forms.Button();
            this.LOAD = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ImageWidthInTiles = new System.Windows.Forms.NumericUpDown();
            this.chooseTilemap = new System.Windows.Forms.Button();
            this.chooseTileset = new System.Windows.Forms.Button();
            this.SaveToTSBAndMPB = new System.Windows.Forms.Button();
            this.importFromPNG = new System.Windows.Forms.Button();
            this.exportToPNG = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.thirdBitAddAmountTemp = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.pixelBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageWidthInTiles)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.thirdBitAddAmountTemp)).BeginInit();
            this.SuspendLayout();
            // 
            // pixelBox1
            // 
            this.pixelBox1.Location = new System.Drawing.Point(12, 14);
            this.pixelBox1.Name = "pixelBox1";
            this.pixelBox1.Size = new System.Drawing.Size(792, 399);
            this.pixelBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pixelBox1.TabIndex = 0;
            this.pixelBox1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(849, 458);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Image";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pixelBox1);
            this.panel1.Location = new System.Drawing.Point(6, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(828, 431);
            this.panel1.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.choosePalette);
            this.groupBox2.Controls.Add(this.LOAD);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.ImageWidthInTiles);
            this.groupBox2.Controls.Add(this.chooseTilemap);
            this.groupBox2.Controls.Add(this.chooseTileset);
            this.groupBox2.Location = new System.Drawing.Point(13, 478);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(345, 158);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Load";
            // 
            // choosePalette
            // 
            this.choosePalette.Location = new System.Drawing.Point(18, 112);
            this.choosePalette.Name = "choosePalette";
            this.choosePalette.Size = new System.Drawing.Size(170, 34);
            this.choosePalette.TabIndex = 6;
            this.choosePalette.Text = "Choose palette (.nbfp)";
            this.choosePalette.UseVisualStyleBackColor = true;
            this.choosePalette.Click += new System.EventHandler(this.choosePalette_Click);
            // 
            // LOAD
            // 
            this.LOAD.Location = new System.Drawing.Point(198, 90);
            this.LOAD.Name = "LOAD";
            this.LOAD.Size = new System.Drawing.Size(134, 56);
            this.LOAD.TabIndex = 5;
            this.LOAD.Text = "LOAD DISPLAY";
            this.LOAD.UseVisualStyleBackColor = true;
            this.LOAD.Click += new System.EventHandler(this.LOAD_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(206, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Image width in tiles";
            // 
            // ImageWidthInTiles
            // 
            this.ImageWidthInTiles.Location = new System.Drawing.Point(221, 52);
            this.ImageWidthInTiles.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.ImageWidthInTiles.Name = "ImageWidthInTiles";
            this.ImageWidthInTiles.Size = new System.Drawing.Size(84, 22);
            this.ImageWidthInTiles.TabIndex = 2;
            this.ImageWidthInTiles.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.ImageWidthInTiles.ValueChanged += new System.EventHandler(this.ImageWidthInTiles_ValueChanged);
            // 
            // chooseTilemap
            // 
            this.chooseTilemap.Location = new System.Drawing.Point(18, 72);
            this.chooseTilemap.Name = "chooseTilemap";
            this.chooseTilemap.Size = new System.Drawing.Size(170, 34);
            this.chooseTilemap.TabIndex = 1;
            this.chooseTilemap.Text = "Choose tilemap (.nbfs)";
            this.chooseTilemap.UseVisualStyleBackColor = true;
            this.chooseTilemap.Click += new System.EventHandler(this.chooseTilemap_Click);
            // 
            // chooseTileset
            // 
            this.chooseTileset.Location = new System.Drawing.Point(18, 32);
            this.chooseTileset.Name = "chooseTileset";
            this.chooseTileset.Size = new System.Drawing.Size(170, 34);
            this.chooseTileset.TabIndex = 0;
            this.chooseTileset.Text = "Choose tileset (.nbfc)";
            this.chooseTileset.UseVisualStyleBackColor = true;
            this.chooseTileset.Click += new System.EventHandler(this.chooseTileset_Click);
            // 
            // SaveToTSBAndMPB
            // 
            this.SaveToTSBAndMPB.Location = new System.Drawing.Point(6, 51);
            this.SaveToTSBAndMPB.Name = "SaveToTSBAndMPB";
            this.SaveToTSBAndMPB.Size = new System.Drawing.Size(296, 35);
            this.SaveToTSBAndMPB.TabIndex = 10;
            this.SaveToTSBAndMPB.Text = "Save as new tileset, tilemap and palette";
            this.SaveToTSBAndMPB.UseVisualStyleBackColor = true;
            this.SaveToTSBAndMPB.Click += new System.EventHandler(this.SaveToTSBAndMPB_Click);
            // 
            // importFromPNG
            // 
            this.importFromPNG.Location = new System.Drawing.Point(6, 32);
            this.importFromPNG.Name = "importFromPNG";
            this.importFromPNG.Size = new System.Drawing.Size(172, 40);
            this.importFromPNG.TabIndex = 9;
            this.importFromPNG.Text = "Import image from PNG";
            this.importFromPNG.UseVisualStyleBackColor = true;
            this.importFromPNG.Click += new System.EventHandler(this.importFromPNG_Click);
            // 
            // exportToPNG
            // 
            this.exportToPNG.Location = new System.Drawing.Point(6, 78);
            this.exportToPNG.Name = "exportToPNG";
            this.exportToPNG.Size = new System.Drawing.Size(172, 34);
            this.exportToPNG.TabIndex = 8;
            this.exportToPNG.Text = "Export image to PNG";
            this.exportToPNG.UseVisualStyleBackColor = true;
            this.exportToPNG.Click += new System.EventHandler(this.exportToPNG_Click_1);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.exportToPNG);
            this.groupBox3.Controls.Add(this.importFromPNG);
            this.groupBox3.Location = new System.Drawing.Point(364, 479);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(184, 157);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Modify / Extract";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.SaveToTSBAndMPB);
            this.groupBox4.Location = new System.Drawing.Point(554, 479);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(308, 157);
            this.groupBox4.TabIndex = 12;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Save";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(897, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Third bit add amount";
            // 
            // thirdBitAddAmountTemp
            // 
            this.thirdBitAddAmountTemp.Location = new System.Drawing.Point(923, 129);
            this.thirdBitAddAmountTemp.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.thirdBitAddAmountTemp.Name = "thirdBitAddAmountTemp";
            this.thirdBitAddAmountTemp.Size = new System.Drawing.Size(84, 22);
            this.thirdBitAddAmountTemp.TabIndex = 6;
            this.thirdBitAddAmountTemp.ValueChanged += new System.EventHandler(this.thirdBitAddAmountTemp_ValueChanged);
            // 
            // NBFC_EditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(873, 640);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.thirdBitAddAmountTemp);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NBFC_EditorForm";
            this.Text = "EPFExplorer NBFC Editor";
            ((System.ComponentModel.ISupportInitialize)(this.pixelBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageWidthInTiles)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.thirdBitAddAmountTemp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button chooseTileset;
        private System.Windows.Forms.Button chooseTilemap;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button LOAD;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button SaveToTSBAndMPB;
        private System.Windows.Forms.Button importFromPNG;
        private System.Windows.Forms.Button exportToPNG;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ColorDialog colorDialog1;
        public RedCell.UI.Controls.PixelBox pixelBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown thirdBitAddAmountTemp;
        public System.Windows.Forms.NumericUpDown ImageWidthInTiles;
        private System.Windows.Forms.Button choosePalette;
    }
}