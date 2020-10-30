namespace EPFExplorer
{
    partial class ShowActionReplayCodeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowActionReplayCodeForm));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.ForLabel = new System.Windows.Forms.Label();
            this.sideEffectsLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 174);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(305, 349);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.ZoomFactor = 1.2F;
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Location = new System.Drawing.Point(12, 17);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(35, 17);
            this.TitleLabel.TabIndex = 1;
            this.TitleLabel.Text = "Title";
            // 
            // ForLabel
            // 
            this.ForLabel.AutoSize = true;
            this.ForLabel.Location = new System.Drawing.Point(12, 45);
            this.ForLabel.Name = "ForLabel";
            this.ForLabel.Size = new System.Drawing.Size(33, 17);
            this.ForLabel.TabIndex = 2;
            this.ForLabel.Text = "For:";
            // 
            // sideEffectsLabel
            // 
            this.sideEffectsLabel.AutoSize = true;
            this.sideEffectsLabel.Location = new System.Drawing.Point(14, 23);
            this.sideEffectsLabel.Name = "sideEffectsLabel";
            this.sideEffectsLabel.Size = new System.Drawing.Size(83, 17);
            this.sideEffectsLabel.TabIndex = 3;
            this.sideEffectsLabel.Text = "SideEffects:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.sideEffectsLabel);
            this.groupBox1.Location = new System.Drawing.Point(15, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(302, 74);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Notes";
            // 
            // ShowActionReplayCodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 547);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ForLabel);
            this.Controls.Add(this.TitleLabel);
            this.Controls.Add(this.richTextBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ShowActionReplayCodeForm";
            this.Text = "Action Replay Code";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label ForLabel;
        private System.Windows.Forms.Label sideEffectsLabel;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}