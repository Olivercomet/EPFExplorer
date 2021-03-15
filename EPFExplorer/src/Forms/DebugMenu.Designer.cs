namespace EPFExplorer
{
    partial class DebugMenu
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
            this.compareArcs = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // compareArcs
            // 
            this.compareArcs.Location = new System.Drawing.Point(34, 25);
            this.compareArcs.Name = "compareArcs";
            this.compareArcs.Size = new System.Drawing.Size(158, 44);
            this.compareArcs.TabIndex = 0;
            this.compareArcs.Text = "Compare two arcs";
            this.compareArcs.UseVisualStyleBackColor = true;
            this.compareArcs.Click += new System.EventHandler(this.compareArcs_Click);
            // 
            // DebugMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.compareArcs);
            this.Name = "DebugMenu";
            this.Text = "DebugMenu";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button compareArcs;
    }
}