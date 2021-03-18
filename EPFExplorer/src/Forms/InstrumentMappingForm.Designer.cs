namespace EPFExplorer
{
    partial class InstrumentMappingForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.sourceInstrumentsBox = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.IngameInstrumentsBox = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.sourceInstrumentsBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(186, 285);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source instrument";
            // 
            // sourceInstrumentsBox
            // 
            this.sourceInstrumentsBox.FormattingEnabled = true;
            this.sourceInstrumentsBox.ItemHeight = 16;
            this.sourceInstrumentsBox.Location = new System.Drawing.Point(6, 30);
            this.sourceInstrumentsBox.Name = "sourceInstrumentsBox";
            this.sourceInstrumentsBox.Size = new System.Drawing.Size(164, 244);
            this.sourceInstrumentsBox.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(533, 185);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 51);
            this.button1.TabIndex = 1;
            this.button1.Text = "Accept";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(533, 242);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 52);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.IngameInstrumentsBox);
            this.groupBox2.Location = new System.Drawing.Point(223, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(272, 285);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Will replace in-game instrument...";
            // 
            // IngameInstrumentsBox
            // 
            this.IngameInstrumentsBox.FormattingEnabled = true;
            this.IngameInstrumentsBox.ItemHeight = 16;
            this.IngameInstrumentsBox.Location = new System.Drawing.Point(6, 30);
            this.IngameInstrumentsBox.Name = "IngameInstrumentsBox";
            this.IngameInstrumentsBox.Size = new System.Drawing.Size(260, 244);
            this.IngameInstrumentsBox.TabIndex = 0;
            // 
            // InstrumentRemapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 308);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Name = "InstrumentRemapForm";
            this.Text = "EPFExplorer - Instrument mapping";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox sourceInstrumentsBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox IngameInstrumentsBox;
    }
}