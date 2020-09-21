namespace EPFExplorer
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Toolstrip_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileExtractorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.massRDTExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateTuxedoDLObjectEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.FileTree = new System.Windows.Forms.TreeView();
            this.archivedFileContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteArchivedFile = new System.Windows.Forms.ToolStripMenuItem();
            this.archivedFolderContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addFileToFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.addFolderToFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.renameFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.renameRDTarchivedfile = new System.Windows.Forms.ToolStripMenuItem();
            this.exportRDTSpriteToPNGs = new System.Windows.Forms.ToolStripMenuItem();
            this.RDTExportRawData = new System.Windows.Forms.ToolStripMenuItem();
            this.exportRdtFileItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rdtSubfileContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openRDTarchivedfile = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.renameRDTarchfile = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.rdtSpriteToPNGs = new System.Windows.Forms.ToolStripMenuItem();
            this.rawDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteRDTarchivedfile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip2.SuspendLayout();
            this.archivedFileContextMenu.SuspendLayout();
            this.archivedFolderContextMenu.SuspendLayout();
            this.rdtSubfileContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip2
            // 
            this.menuStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip2.Location = new System.Drawing.Point(0, 0);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(981, 28);
            this.menuStrip2.TabIndex = 1;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Toolstrip_Open,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // Toolstrip_Open
            // 
            this.Toolstrip_Open.Name = "Toolstrip_Open";
            this.Toolstrip_Open.Size = new System.Drawing.Size(128, 26);
            this.Toolstrip_Open.Text = "Open";
            this.Toolstrip_Open.Click += new System.EventHandler(this.Toolstrip_Open_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveFileExtractorToolStripMenuItem,
            this.massRDTExportToolStripMenuItem,
            this.generateTuxedoDLObjectEntryToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(58, 24);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // saveFileExtractorToolStripMenuItem
            // 
            this.saveFileExtractorToolStripMenuItem.Name = "saveFileExtractorToolStripMenuItem";
            this.saveFileExtractorToolStripMenuItem.Size = new System.Drawing.Size(306, 26);
            this.saveFileExtractorToolStripMenuItem.Text = "Save File Editor";
            this.saveFileExtractorToolStripMenuItem.Click += new System.EventHandler(this.saveFileExtractorToolStripMenuItem_Click);
            // 
            // massRDTExportToolStripMenuItem
            // 
            this.massRDTExportToolStripMenuItem.Name = "massRDTExportToolStripMenuItem";
            this.massRDTExportToolStripMenuItem.Size = new System.Drawing.Size(306, 26);
            this.massRDTExportToolStripMenuItem.Text = "Mass RDT Export";
            this.massRDTExportToolStripMenuItem.Click += new System.EventHandler(this.massRDTExportToolStripMenuItem_Click);
            // 
            // generateTuxedoDLObjectEntryToolStripMenuItem
            // 
            this.generateTuxedoDLObjectEntryToolStripMenuItem.Name = "generateTuxedoDLObjectEntryToolStripMenuItem";
            this.generateTuxedoDLObjectEntryToolStripMenuItem.Size = new System.Drawing.Size(306, 26);
            this.generateTuxedoDLObjectEntryToolStripMenuItem.Text = "Generate TuxedoDL object entry";
            this.generateTuxedoDLObjectEntryToolStripMenuItem.Click += new System.EventHandler(this.generateTuxedoDLObjectEntryToolStripMenuItem_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1097, 140);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(226, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "palette tester";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FileTree
            // 
            this.FileTree.Location = new System.Drawing.Point(12, 31);
            this.FileTree.Name = "FileTree";
            this.FileTree.Size = new System.Drawing.Size(471, 467);
            this.FileTree.TabIndex = 4;
            // 
            // archivedFileContextMenu
            // 
            this.archivedFileContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.archivedFileContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replaceToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.deleteArchivedFile});
            this.archivedFileContextMenu.Name = "contextMenuStrip1";
            this.archivedFileContextMenu.Size = new System.Drawing.Size(133, 100);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(132, 24);
            this.replaceToolStripMenuItem.Text = "Replace";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(132, 24);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(132, 24);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // deleteArchivedFile
            // 
            this.deleteArchivedFile.Name = "deleteArchivedFile";
            this.deleteArchivedFile.Size = new System.Drawing.Size(132, 24);
            this.deleteArchivedFile.Text = "Delete";
            this.deleteArchivedFile.Click += new System.EventHandler(this.deleteArchivedFile_Click);
            // 
            // archivedFolderContextMenu
            // 
            this.archivedFolderContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.archivedFolderContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem1,
            this.renameFolderToolStripMenuItem,
            this.exportToolStripMenuItem1,
            this.deleteFolder});
            this.archivedFolderContextMenu.Name = "archivedFolderContextMenu";
            this.archivedFolderContextMenu.Size = new System.Drawing.Size(133, 100);
            // 
            // addToolStripMenuItem1
            // 
            this.addToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFileToFolder,
            this.addFolderToFolder});
            this.addToolStripMenuItem1.Name = "addToolStripMenuItem1";
            this.addToolStripMenuItem1.Size = new System.Drawing.Size(132, 24);
            this.addToolStripMenuItem1.Text = "Add";
            // 
            // addFileToFolder
            // 
            this.addFileToFolder.Name = "addFileToFolder";
            this.addFileToFolder.Size = new System.Drawing.Size(134, 26);
            this.addFileToFolder.Text = "File";
            this.addFileToFolder.Click += new System.EventHandler(this.addFileToFolder_Click);
            // 
            // addFolderToFolder
            // 
            this.addFolderToFolder.Name = "addFolderToFolder";
            this.addFolderToFolder.Size = new System.Drawing.Size(134, 26);
            this.addFolderToFolder.Text = "Folder";
            this.addFolderToFolder.Click += new System.EventHandler(this.addFolderToFolder_Click);
            // 
            // renameFolderToolStripMenuItem
            // 
            this.renameFolderToolStripMenuItem.Name = "renameFolderToolStripMenuItem";
            this.renameFolderToolStripMenuItem.Size = new System.Drawing.Size(132, 24);
            this.renameFolderToolStripMenuItem.Text = "Rename";
            this.renameFolderToolStripMenuItem.Click += new System.EventHandler(this.renameFolderToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem1
            // 
            this.exportToolStripMenuItem1.Name = "exportToolStripMenuItem1";
            this.exportToolStripMenuItem1.Size = new System.Drawing.Size(132, 24);
            this.exportToolStripMenuItem1.Text = "Export";
            this.exportToolStripMenuItem1.Click += new System.EventHandler(this.exportToolStripMenuItem1_Click);
            // 
            // deleteFolder
            // 
            this.deleteFolder.Name = "deleteFolder";
            this.deleteFolder.Size = new System.Drawing.Size(132, 24);
            this.deleteFolder.Text = "Delete";
            this.deleteFolder.Click += new System.EventHandler(this.deleteFolder_Click);
            // 
            // renameRDTarchivedfile
            // 
            this.renameRDTarchivedfile.Name = "renameRDTarchivedfile";
            this.renameRDTarchivedfile.Size = new System.Drawing.Size(210, 24);
            this.renameRDTarchivedfile.Text = "Rename";
            this.renameRDTarchivedfile.Click += new System.EventHandler(this.renameRDTarchivedfile_Click);
            // 
            // exportRDTSpriteToPNGs
            // 
            this.exportRDTSpriteToPNGs.Name = "exportRDTSpriteToPNGs";
            this.exportRDTSpriteToPNGs.Size = new System.Drawing.Size(173, 26);
            this.exportRDTSpriteToPNGs.Text = "PNG images";
            this.exportRDTSpriteToPNGs.Click += new System.EventHandler(this.exportRDTSpriteToPNGs_Click);
            // 
            // RDTExportRawData
            // 
            this.RDTExportRawData.Name = "RDTExportRawData";
            this.RDTExportRawData.Size = new System.Drawing.Size(173, 26);
            this.RDTExportRawData.Text = "Raw data";
            // 
            // exportRdtFileItem
            // 
            this.exportRdtFileItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportRDTSpriteToPNGs,
            this.RDTExportRawData});
            this.exportRdtFileItem.Name = "exportRdtFileItem";
            this.exportRdtFileItem.Size = new System.Drawing.Size(210, 24);
            this.exportRdtFileItem.Text = "Export";
            this.exportRdtFileItem.Click += new System.EventHandler(this.exportRdtFileItem_Click);
            // 
            // rdtSubfileContextMenu
            // 
            this.rdtSubfileContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.rdtSubfileContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openRDTarchivedfile,
            this.replaceToolStripMenuItem1,
            this.renameRDTarchfile,
            this.exportToolStripMenuItem2,
            this.deleteRDTarchivedfile});
            this.rdtSubfileContextMenu.Name = "contextMenuStrip1";
            this.rdtSubfileContextMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.rdtSubfileContextMenu.Size = new System.Drawing.Size(211, 152);
            // 
            // openRDTarchivedfile
            // 
            this.openRDTarchivedfile.Name = "openRDTarchivedfile";
            this.openRDTarchivedfile.Size = new System.Drawing.Size(210, 24);
            this.openRDTarchivedfile.Text = "Open";
            this.openRDTarchivedfile.Click += new System.EventHandler(this.openRDTarchivedfile_Click);
            // 
            // replaceToolStripMenuItem1
            // 
            this.replaceToolStripMenuItem1.Name = "replaceToolStripMenuItem1";
            this.replaceToolStripMenuItem1.Size = new System.Drawing.Size(210, 24);
            this.replaceToolStripMenuItem1.Text = "Replace";
            this.replaceToolStripMenuItem1.Click += new System.EventHandler(this.replaceToolStripMenuItem1_Click);
            // 
            // renameRDTarchfile
            // 
            this.renameRDTarchfile.Name = "renameRDTarchfile";
            this.renameRDTarchfile.Size = new System.Drawing.Size(210, 24);
            this.renameRDTarchfile.Text = "Rename";
            this.renameRDTarchfile.Click += new System.EventHandler(this.renameRDTarchfile_Click);
            // 
            // exportToolStripMenuItem2
            // 
            this.exportToolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rdtSpriteToPNGs,
            this.rawDataToolStripMenuItem});
            this.exportToolStripMenuItem2.Name = "exportToolStripMenuItem2";
            this.exportToolStripMenuItem2.Size = new System.Drawing.Size(210, 24);
            this.exportToolStripMenuItem2.Text = "Export";
            // 
            // rdtSpriteToPNGs
            // 
            this.rdtSpriteToPNGs.Name = "rdtSpriteToPNGs";
            this.rdtSpriteToPNGs.Size = new System.Drawing.Size(224, 26);
            this.rdtSpriteToPNGs.Text = "As PNG images";
            this.rdtSpriteToPNGs.Click += new System.EventHandler(this.rdtSpriteToPNGs_Click);
            // 
            // rawDataToolStripMenuItem
            // 
            this.rawDataToolStripMenuItem.Name = "rawDataToolStripMenuItem";
            this.rawDataToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.rawDataToolStripMenuItem.Text = "As .sprite file";
            this.rawDataToolStripMenuItem.Click += new System.EventHandler(this.rawDataToolStripMenuItem_Click);
            // 
            // deleteRDTarchivedfile
            // 
            this.deleteRDTarchivedfile.Name = "deleteRDTarchivedfile";
            this.deleteRDTarchivedfile.Size = new System.Drawing.Size(210, 24);
            this.deleteRDTarchivedfile.Text = "Delete";
            this.deleteRDTarchivedfile.Click += new System.EventHandler(this.deleteRDTarchivedfile_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(981, 510);
            this.Controls.Add(this.FileTree);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "EPFExplorer";
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.archivedFileContextMenu.ResumeLayout(false);
            this.archivedFolderContextMenu.ResumeLayout(false);
            this.rdtSubfileContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Toolstrip_Open;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveFileExtractorToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.TreeView FileTree;
        private System.Windows.Forms.ContextMenuStrip archivedFileContextMenu;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip archivedFolderContextMenu;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addFileToFolder;
        private System.Windows.Forms.ToolStripMenuItem addFolderToFolder;
        private System.Windows.Forms.ToolStripMenuItem renameFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteFolder;
        private System.Windows.Forms.ToolStripMenuItem massRDTExportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameRDTarchivedfile;
        private System.Windows.Forms.ToolStripMenuItem exportRDTSpriteToPNGs;
        private System.Windows.Forms.ToolStripMenuItem RDTExportRawData;
        public System.Windows.Forms.ToolStripMenuItem exportRdtFileItem;
        private System.Windows.Forms.ContextMenuStrip rdtSubfileContextMenu;
        private System.Windows.Forms.ToolStripMenuItem renameRDTarchfile;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem rdtSpriteToPNGs;
        private System.Windows.Forms.ToolStripMenuItem rawDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteRDTarchivedfile;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openRDTarchivedfile;
        private System.Windows.Forms.ToolStripMenuItem deleteArchivedFile;
        private System.Windows.Forms.ToolStripMenuItem generateTuxedoDLObjectEntryToolStripMenuItem;
    }
}

