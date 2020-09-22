namespace EPFExplorer
{
    partial class MPB_TSB_EditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MPB_TSB_EditorForm));
            this.pixelBox1 = new RedCell.UI.Controls.PixelBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ImageWidthInTiles = new System.Windows.Forms.NumericUpDown();
            this.chooseTilemap = new System.Windows.Forms.Button();
            this.chooseTileset = new System.Windows.Forms.Button();
            this.exportToPNG = new System.Windows.Forms.Button();
            this.LOAD = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pixelBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageWidthInTiles)).BeginInit();
            this.SuspendLayout();
            // 
            // pixelBox1
            // 
            this.pixelBox1.Location = new System.Drawing.Point(16, 30);
            this.pixelBox1.Name = "pixelBox1";
            this.pixelBox1.Size = new System.Drawing.Size(818, 414);
            this.pixelBox1.TabIndex = 0;
            this.pixelBox1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pixelBox1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(849, 458);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Image";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.LOAD);
            this.groupBox2.Controls.Add(this.exportToPNG);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.ImageWidthInTiles);
            this.groupBox2.Controls.Add(this.chooseTilemap);
            this.groupBox2.Controls.Add(this.chooseTileset);
            this.groupBox2.Location = new System.Drawing.Point(13, 478);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(849, 128);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(198, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Image width in tiles";
            // 
            // ImageWidthInTiles
            // 
            this.ImageWidthInTiles.Location = new System.Drawing.Point(213, 50);
            this.ImageWidthInTiles.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.ImageWidthInTiles.Name = "ImageWidthInTiles";
            this.ImageWidthInTiles.Size = new System.Drawing.Size(84, 22);
            this.ImageWidthInTiles.TabIndex = 2;
            this.ImageWidthInTiles.Value = new decimal(new int[] {
            72,
            0,
            0,
            0});
            this.ImageWidthInTiles.ValueChanged += new System.EventHandler(this.ImageWidthInTiles_ValueChanged);
            // 
            // chooseTilemap
            // 
            this.chooseTilemap.Location = new System.Drawing.Point(16, 78);
            this.chooseTilemap.Name = "chooseTilemap";
            this.chooseTilemap.Size = new System.Drawing.Size(170, 34);
            this.chooseTilemap.TabIndex = 1;
            this.chooseTilemap.Text = "Choose tilemap (.mpb)";
            this.chooseTilemap.UseVisualStyleBackColor = true;
            this.chooseTilemap.Click += new System.EventHandler(this.chooseTilemap_Click);
            // 
            // chooseTileset
            // 
            this.chooseTileset.Location = new System.Drawing.Point(16, 38);
            this.chooseTileset.Name = "chooseTileset";
            this.chooseTileset.Size = new System.Drawing.Size(170, 34);
            this.chooseTileset.TabIndex = 0;
            this.chooseTileset.Text = "Choose tileset (.tsb)";
            this.chooseTileset.UseVisualStyleBackColor = true;
            this.chooseTileset.Click += new System.EventHandler(this.chooseTileset_Click);
            // 
            // exportToPNG
            // 
            this.exportToPNG.Location = new System.Drawing.Point(574, 78);
            this.exportToPNG.Name = "exportToPNG";
            this.exportToPNG.Size = new System.Drawing.Size(260, 44);
            this.exportToPNG.TabIndex = 4;
            this.exportToPNG.Text = "Export to PNG";
            this.exportToPNG.UseVisualStyleBackColor = true;
            this.exportToPNG.Click += new System.EventHandler(this.exportToPNG_Click);
            // 
            // LOAD
            // 
            this.LOAD.Location = new System.Drawing.Point(201, 78);
            this.LOAD.Name = "LOAD";
            this.LOAD.Size = new System.Drawing.Size(96, 34);
            this.LOAD.TabIndex = 5;
            this.LOAD.Text = "LOAD";
            this.LOAD.UseVisualStyleBackColor = true;
            this.LOAD.Click += new System.EventHandler(this.LOAD_Click);
            // 
            // MPB_TSB_EditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 619);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MPB_TSB_EditorForm";
            this.Text = "EPFExplorer MPB/TSB Editor";
            ((System.ComponentModel.ISupportInitialize)(this.pixelBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageWidthInTiles)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private RedCell.UI.Controls.PixelBox pixelBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button chooseTileset;
        private System.Windows.Forms.Button chooseTilemap;
        private System.Windows.Forms.NumericUpDown ImageWidthInTiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button exportToPNG;
        private System.Windows.Forms.Button LOAD;
    }
}