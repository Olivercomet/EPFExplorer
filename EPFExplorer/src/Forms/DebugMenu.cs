using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace EPFExplorer
{
    public partial class DebugMenu : Form
    {
        public Form1 form1;

        public DebugMenu()
        {
            InitializeComponent();
        }

        private void compareArcs_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select first arc file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "1PP archives (*.arc)|*.arc";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                OpenFileDialog openFileDialog2 = new OpenFileDialog();

                openFileDialog2.Title = "Select second arc file";
                openFileDialog2.CheckFileExists = true;
                openFileDialog2.CheckPathExists = true;

                openFileDialog1.Filter = "1PP archives (*.arc)|*.arc";
                if (openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    arcfile arc1 = new arcfile();
                    arc1.arcname = Path.GetFileName(openFileDialog1.FileName);
                    arc1.filename = openFileDialog1.FileName;
                    arc1.filebytes = File.ReadAllBytes(openFileDialog1.FileName);
                    arc1.form1 = form1;
                    arc1.ReadArc();

                    arcfile arc2 = new arcfile();
                    arc2.arcname = Path.GetFileName(openFileDialog2.FileName);
                    arc2.filename = openFileDialog2.FileName;
                    arc2.filebytes = File.ReadAllBytes(openFileDialog2.FileName);
                    arc2.form1 = form1;
                    arc2.ReadArc();

                    List<string> report = new List<string>();

                    foreach (archivedfile f in arc1.archivedfiles)
                    {

                        if (arc2.GetFileWithHash(f.hash) == null)
                        { //if arc2 straight up doesn't have it
                            string evaluatedFilename = f.filename;
                            if (f.filename == "FILENAME_NOT_SET")
                            {
                                evaluatedFilename = f.hash.ToString();
                            }

                            report.Add("File " + evaluatedFilename + " was present in " + Path.GetFileNameWithoutExtension(arc1.filename) + ", but not " + Path.GetFileNameWithoutExtension(arc2.filename) + "!");
                        }
                        else
                        {
                            archivedfile equivalent = arc2.GetFileWithHash(f.hash);
                            if (equivalent != null)
                            {
                                f.ReadFile();
                                equivalent.ReadFile();
                                if (f.filebytes.Length != equivalent.filebytes.Length)
                                { //if it's present in both, but with different filesizes
                                    string evaluatedFilename = f.filename;
                                    if (f.filename == "FILENAME_NOT_SET")
                                    {
                                        evaluatedFilename = f.hash.ToString();
                                    }

                                    report.Add("File " + evaluatedFilename + " was present in both archives, but is a different size in " + Path.GetFileNameWithoutExtension(arc2.filename) + "!");
                                }
                            }
                        }
                    }

                    foreach (archivedfile f in arc2.archivedfiles)
                    {
                        if (arc1.GetFileWithHash(f.hash) == null)
                        { //if arc1 straight up doesn't have it
                            string evaluatedFilename = f.filename;
                            if (f.filename == "FILENAME_NOT_SET")
                            {
                                evaluatedFilename = f.hash.ToString();
                            }
                            report.Add("File " + evaluatedFilename + " was present in " + Path.GetFileNameWithoutExtension(arc2.filename) + ", but not " + Path.GetFileNameWithoutExtension(arc1.filename) + "!");
                        }
                        //don't need to do the second part again because it was already two-way
                    }

                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = ".txt files (*.txt)|*.txt";
                    saveFileDialog1.Title = "Save report";

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllLines(saveFileDialog1.FileName, report.ToArray());
                    }
                }
            }
        }
    }
}
