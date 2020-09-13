using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPFExplorer
{
    public class rdtfile
    {
        public Form1 form1;

        public string arcname;
        public string filename;
        public byte[] filebytes;

        uint filecount;

        public List<archivedfile> archivedfiles = new List<archivedfile>();

        public Dictionary<string,int> filenamesAndOffsets = new Dictionary<string,int>();

        int posInNodeTree; //for writing
        List<TreeNode> NullNodesInArchivedFileOrder; //for writing

        bool isHR = false;
        public void ReadRdt()
        {
            filecount = BitConverter.ToUInt16(filebytes,0x0F);
            
            if (BitConverter.ToUInt32(filebytes,0x12) > filebytes.Length)
                {
                isHR = true;
                }

            if (isHR)
                {
                MessageBox.Show("HR RDT files are annoying. There won't be any filenames.",
                          "Filenames will not be shown");
                RDTBlagger3000();
                form1.MakeFileTree();
                return;
                }

            int currentOffset = 0x11;

            int number_of_top_level_trees = filebytes[currentOffset];


            for (int i = 0; i < number_of_top_level_trees; i++)
                {
                currentOffset = BitConverter.ToInt32(filebytes, 0x12 + (5 * i) + 1);
                ParseRDTNode(currentOffset, (char)filebytes[0x12 + (5 * i)]+"");    //only parse the first node if the string actually begins with that letter, and it will recursively go down the tree
                }

            //now all the filenames should be parsed, let's check though

            if (archivedfiles.Count == filecount)
                {
                Console.WriteLine("All RDT files accounted for!");
                }
            else
                {
                Console.WriteLine("ERROR! Either some files were missed, or we overcounted!");
                }

            form1.MakeFileTree();
        }

        public void RDTBlagger3000() { //attempts to scrape likely file offsets from the index table. Intended for HR RDTs, where the file tree is in an annoying format

            int endOfIndexTable = BitConverter.ToInt32(filebytes, 0x0B);

            for (int i = 0x12; i < endOfIndexTable; i++)
                {
                int possibleOffset = BitConverter.ToInt32(filebytes,i);

                if (possibleOffset >= endOfIndexTable && possibleOffset < filebytes.Length && filebytes[possibleOffset] == 0x03 && BitConverter.ToInt32(filebytes, possibleOffset+1) > 0)
                    {
                    archivedfile newFile = new archivedfile();
                    newFile.offset = possibleOffset;
                    newFile.filename = "PotentialFile"+newFile.offset.ToString();
                    newFile.form1 = form1;
                    newFile.parentrdtfile = this;
                    archivedfiles.Add(newFile);
                    }
            }
        }


        public void ParseRDTNode(int offsetOfThisNode, string baseString) {

            int subnodeCount = filebytes[offsetOfThisNode];

            string currentStateOfString = baseString;

            for (int i = 0; i < subnodeCount; i++)
                {
                char letter = (char)filebytes[offsetOfThisNode + (i * 5) + 1];

                if ((byte)letter == 0x00)   //it's the end of the string
                    {
                    if (!filenamesAndOffsets.ContainsKey(currentStateOfString))
                        {
                        filenamesAndOffsets.Add(currentStateOfString, BitConverter.ToInt32(filebytes, offsetOfThisNode + (i * 5) + 2));
                        archivedfile newFile = new archivedfile();

                        newFile.filename = currentStateOfString;
                        newFile.offset = filenamesAndOffsets[currentStateOfString];
                        newFile.form1 = form1;
                        newFile.parentrdtfile = this;

                        archivedfiles.Add(newFile);
                        }
                    Console.WriteLine(currentStateOfString + " file offset is "+ filenamesAndOffsets[currentStateOfString]);
                    }
                else          //keep following the string
                    {
                    currentStateOfString += letter;
                    ParseRDTNode(BitConverter.ToInt32(filebytes, offsetOfThisNode + (i * 5) + 2),currentStateOfString);
                    }
                currentStateOfString = baseString;
                }
        }






        public void RebuildRDT() {

        int longestFilenameLength = 0;

        for (int i = 0; i < archivedfiles.Count; i++)   //make sure filenames use forward slashes, and that they don't start with them
            {
            if (archivedfiles[i].filebytes == null || archivedfiles[i].filebytes.Length == 0)   //if it hasn't been modified by the user, read it out of the original file
                {
                archivedfiles[i].ReadFile();
                }

            archivedfiles[i].filename.Replace('\\', '/');

            if (archivedfiles[i].filename[0] == '/')
                {
                archivedfiles[i].filename = archivedfiles[i].filename.Substring(1, archivedfiles[i].filename.Length - 1);
                }

            if (archivedfiles[i].filename.Length > longestFilenameLength)
                {
                longestFilenameLength = archivedfiles[i].filename.Length;
                }
            }

            archivedfiles = archivedfiles.OrderBy(x => x.filename).ToList();    //sort alphabetically


            //make node tree for filenames

            TreeNode root = new TreeNode();

            NullNodesInArchivedFileOrder = new List<TreeNode>();    //the null-terminating nodes for the archivedfiles, in the same order so it can just be accessed by index

            int nodeTreeSize = 1;

                foreach (archivedfile file in archivedfiles)    //make a node tree of all the filenames
                    {
                    TreeNode targetNode = root;
                    for (int i = 0; i < file.filename.Length; i++)
                        {
                        string letter = file.filename[i].ToString();

                        if ((byte)letter[0] >= 0x61)     //winforms nodes are not case sensitive, so give lower case nodes a temporary '1' suffix, it will be removed later
                            {
                            letter += "1";
                            }

                        if (!targetNode.Nodes.ContainsKey(letter)) //if the node doesn't contain a subnode for our next letter, make one
                            {
                            targetNode = targetNode.Nodes.Add(letter);
                            targetNode.Text = letter;
                            targetNode.Name = letter;
                            nodeTreeSize += 6;
                            }
                        else
                            {
                            targetNode = targetNode.Nodes.Find(letter,false)[0];
                            }
                        }
                    NullNodesInArchivedFileOrder.Add(targetNode.Nodes.Add("nullNode"));
                    NullNodesInArchivedFileOrder[NullNodesInArchivedFileOrder.Count - 1].Text = "nullNode";
                    NullNodesInArchivedFileOrder[NullNodesInArchivedFileOrder.Count - 1].Name = "nullNode";
                    nodeTreeSize += 5;
                }

            //make byte array version of node tree

            Byte[] nodeTree = new byte[nodeTreeSize];

            posInNodeTree = 0;

            AddSubNodesToByteArray(root, nodeTree);

            File.WriteAllBytes("nodetree", nodeTree);   //temp
        }


        public void AddSubNodesToByteArray(TreeNode node, Byte[] nodeTree) {

            nodeTree[posInNodeTree] = (byte)node.Nodes.Count;
            posInNodeTree++;
            int basePos = posInNodeTree;

            posInNodeTree = basePos + (node.Nodes.Count * 5);
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                if (node.Nodes[i].Text == "nullNode")
                    {
                    nodeTree[basePos + (i * 5)] = 0x00;
                    archivedfile file = archivedfiles[NullNodesInArchivedFileOrder.IndexOf(node)];  //and then do something with it
                    //REBUILD FILE AND ADD TO DATA LIST HERE 

                    form1.WriteU32ToArray(nodeTree, basePos + (i * 5) + 1, 0x3F3F3F3F); //not done yet, will be the offset of the file
                    }
                else
                    {
                    nodeTree[basePos + (i * 5)] = (byte)node.Nodes[i].Text[0];
                    form1.WriteU32ToArray(nodeTree, basePos + (i * 5) + 1, 0x11 + (uint)posInNodeTree);
                    AddSubNodesToByteArray(node.Nodes[i], nodeTree);
                    }
            }
        }









    }
}
