using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPFExplorer
{
    public class nintendoarcfile
    {
        public byte[] filebytes = new byte[0];
        public string arcname;
        public string filename;

        public class ArchivedFileInfo
        {
            public Form1 form1;
            public nintendoarcfile parentArchive;

            public enum fileType
            {
                FILE = 0x00,
                DIRECTORY = 0x01
            }

            public fileType fileOrDir;

            public int nameOffset;
            public string name;
            public int offset;
            public int size;

            public byte[] filebytes = new byte[0];

            public void ReadFile()
            {
                filebytes = new byte[size];
                Array.Copy(parentArchive.filebytes, offset, filebytes, 0, size);
            }
        }


        public Form1 form1;

        public List<ArchivedFileInfo> archivedFiles = new List<ArchivedFileInfo>();

        public int dataOffset = 0;

        public ArchivedFileInfo GetFileFromArchive(string filename)
        {

            filename = filename.ToLower();

            foreach (ArchivedFileInfo f in archivedFiles)
            {
                if (f.name.ToLower() == filename)
                {
                    f.ReadFile();
                    return f;
                }
            }
            return null;
        }

        public void ReadArc() {

                        int nodeOffset = form1.readBigEndianIntFromArray(filebytes, 0x04);

                        dataOffset = form1.readBigEndianIntFromArray(filebytes, 0x0C);

                        int currentNode = 0;

                        int numNodes = 0;   //will then be set by the first node we encounter

                        do
                        {  //now read the nodes
                            ArchivedFileInfo newArchivedFile = new ArchivedFileInfo();
                            newArchivedFile.parentArchive = this;

                            newArchivedFile.fileOrDir = (ArchivedFileInfo.fileType)filebytes[nodeOffset];
                            newArchivedFile.nameOffset = 0x00FFFFFF & form1.readBigEndianIntFromArray(filebytes, nodeOffset);
                            nodeOffset += 4;

                            newArchivedFile.offset = form1.readBigEndianIntFromArray(filebytes, nodeOffset);
                            nodeOffset += 4;

                            newArchivedFile.size = form1.readBigEndianIntFromArray(filebytes, nodeOffset);
                            nodeOffset += 4;

                            if (numNodes == 0)
                            {//if this was the root folder, it tells us how many folders there are in the whole thing
                                numNodes = newArchivedFile.size;
                            }

                            archivedFiles.Add(newArchivedFile);
                            currentNode++;
                        } while (currentNode < numNodes);

                        //now set the filenames
                        foreach (ArchivedFileInfo f in archivedFiles)
                        {
                            int i = nodeOffset + f.nameOffset;

                            f.name = "";

                            while (filebytes[i] != 0x00)
                            {
                                f.name += (char)filebytes[i];
                                i++;
                            }

                        Console.WriteLine(f.name);
                        }
        }

        public void ViewArcInFileTree() {

            MessageBox.Show("Not yet implemented");
        
        }
    }
}
