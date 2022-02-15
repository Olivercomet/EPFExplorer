using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

        public Dictionary<string, int> filenamesAndOffsets = new Dictionary<string, int>();

        int posInNodeTree; //for writing
        List<TreeNode> NullNodesInArchivedFileOrder; //for writing
        Byte[] nodeTree = new byte[0];
        List<Byte> data = new List<Byte>(); //for writing

        public bool isHR = false;

        public bool ben10mode = false;

        public Dictionary<byte[], int> AlreadyProcessedFilesAndOffsetsInData = new Dictionary<Byte[], int>();

        public bool randomizeOnNextSave = false;

        string[] FilenamesBannedFromRando = new string[] { "spygadget", "hud", "microgames", "spypod", "suitcase", "utilityborder", "bttn", "button", "mainmenu" };

        public bool is_only_sprite_container = false;


        public void ReadRdt()
        {
            filecount = BitConverter.ToUInt16(filebytes, 0x0F);


            if (BitConverter.ToUInt32(filebytes, 0x13) > filebytes.Length)
            {
                isHR = true;
            }

            if (isHR)
            {
                ReadHRRdt();
            }
            else
            {
                int currentOffset = 0x11;

                int number_of_top_level_trees = filebytes[currentOffset];


                for (int i = 0; i < number_of_top_level_trees; i++)
                {
                    currentOffset = BitConverter.ToInt32(filebytes, 0x12 + (5 * i) + 1);
                    ParseRDTNode(currentOffset, (char)filebytes[0x12 + (5 * i)] + "");    //only parse the first node if the string actually begins with that letter, and it will recursively go down the tree
                }
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
        }


        public void ReadHRRdt()
        {

            int currentOffset = 0x11;

            int number_of_top_level_folders = filebytes[currentOffset];
            currentOffset++;

            for (int i = 0; i < number_of_top_level_folders; i++)
            {
                string currentPath = "";
                while ((filebytes[currentOffset] & 0x80) == 0x80)
                {
                    currentPath += (char)(filebytes[currentOffset] ^ 0x80);
                    currentOffset++;
                }

                if (filebytes[currentOffset] != 0x00)
                {
                    currentPath += (char)filebytes[currentOffset];
                }
                currentOffset++;

                if (filebytes[currentOffset - 1] == 0x00)
                {
                    EndHerbertsRevengeChain(currentOffset, currentPath);
                }
                else
                {
                    ParseHerbertsRevengeRDTNode(BitConverter.ToInt32(filebytes, currentOffset), currentPath);
                }

                currentOffset += 4;
            }
        }

        public void ParseHerbertsRevengeRDTNode(int offsetOfThisNode, string baseString)
        {
            int subnodeCount = filebytes[offsetOfThisNode];
            offsetOfThisNode++;

            string currentStateOfString = baseString;

            for (int i = 0; i < subnodeCount; i++)
            {
                char letter = (char)filebytes[offsetOfThisNode];

                if ((letter & 0x80) == 0x80)    //it's one of those XORed strings
                {
                    while ((filebytes[offsetOfThisNode] & 0x80) == 0x80)
                    {
                        currentStateOfString += (char)(filebytes[offsetOfThisNode] ^ 0x80);
                        offsetOfThisNode++;
                    }

                    if ((char)filebytes[offsetOfThisNode] != 0x00)
                    {
                        currentStateOfString += (char)filebytes[offsetOfThisNode];
                    }
                    offsetOfThisNode++;

                    if (filebytes[offsetOfThisNode - 1] == 0x00)
                    {
                        EndHerbertsRevengeChain(offsetOfThisNode, currentStateOfString);
                    }
                    else
                    {
                        ParseHerbertsRevengeRDTNode(BitConverter.ToInt32(filebytes, offsetOfThisNode), currentStateOfString);
                    }
                    offsetOfThisNode += 4;
                }
                else
                {
                    if ((byte)letter == 0x00)   //it's the end of the string
                    {
                        offsetOfThisNode++;
                        EndHerbertsRevengeChain(offsetOfThisNode, currentStateOfString);
                        offsetOfThisNode += 4;
                    }
                    else          //keep following the string
                    {
                        currentStateOfString += letter;
                        offsetOfThisNode++;
                        ParseHerbertsRevengeRDTNode(BitConverter.ToInt32(filebytes, offsetOfThisNode), currentStateOfString);
                        offsetOfThisNode += 4;
                    }
                }

                currentStateOfString = baseString;
            }
        }

        public void EndHerbertsRevengeChain(int offsetOfThisNode, string currentStateOfString)
        {

            if (!filenamesAndOffsets.ContainsKey(currentStateOfString))
            {
                filenamesAndOffsets.Add(currentStateOfString, BitConverter.ToInt32(filebytes, offsetOfThisNode));
                offsetOfThisNode += 4;
                archivedfile newFile = new archivedfile();

                newFile.filename = currentStateOfString;
                newFile.offset = filenamesAndOffsets[currentStateOfString];
                newFile.form1 = form1;
                newFile.parentrdtfile = this;

                archivedfiles.Add(newFile);
            }
            Console.WriteLine(currentStateOfString + " file offset is " + filenamesAndOffsets[currentStateOfString]);
        }

        public void ParseRDTNode(int offsetOfThisNode, string baseString)
        {

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
                    Console.WriteLine(currentStateOfString + " file offset is " + filenamesAndOffsets[currentStateOfString]);
                }
                else          //keep following the string
                {
                    currentStateOfString += letter;
                    ParseRDTNode(BitConverter.ToInt32(filebytes, offsetOfThisNode + (i * 5) + 2), currentStateOfString);
                }
                currentStateOfString = baseString;
            }
        }




        public bool FilenameAllowedInRando(string input)
        {

            input = input.ToLower();

            foreach (string s in FilenamesBannedFromRando)
            {
                if (input.Contains(s))
                {
                    return false;
                }
            }

            return true;
        }

        public void RebuildRDT()
        {

            int longestFilenameLength = 0;
            AlreadyProcessedFilesAndOffsetsInData = new Dictionary<Byte[], int>();

            if (isHR)
            {
                MessageBox.Show("Saving Herbert's Revenge RDT files is not supported.", "Not supported", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (archivedfiles.Count > 1000)
            {
                DialogResult result = MessageBox.Show("Warning: This is a huge RDT with " + archivedfiles.Count + " files!\nIt will take a while to save.", "Saving will take a while", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                if (result != DialogResult.OK)
                {
                    return;
                }
            }

            List<archivedfile> archivedfiles4BPP = new List<archivedfile>();    //for the randomizer

            for (int i = 0; i < archivedfiles.Count; i++)   //make sure filenames use forward slashes, and that they don't start with them
            {
                if (archivedfiles[i].filebytes == null || archivedfiles[i].filebytes.Length == 0)   //if it hasn't been modified by the user, read it out of the original file
                {
                    archivedfiles[i].ReadFile();
                }

                if (randomizeOnNextSave && archivedfiles[i].RDTSpriteBPP == 4 && FilenameAllowedInRando(archivedfiles[i].filename))
                {
                    archivedfiles4BPP.Add(archivedfiles[i]);
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

            if (randomizeOnNextSave)    //randomise the filenames if random mode is on
            {
                Random rnd = new Random();
                string tempName = "";
                int randomIndex = 0;

                for (int i = 0; i < archivedfiles4BPP.Count; i++)
                {
                    //swap filenames
                    randomIndex = rnd.Next(0, archivedfiles4BPP.Count);
                    tempName = archivedfiles4BPP[randomIndex].filename;
                    archivedfiles4BPP[randomIndex].filename = archivedfiles4BPP[i].filename;
                    archivedfiles4BPP[i].filename = tempName;

                    //then swap settings to make sure the new sprite is clickable if it needs to be
                    List<rdtSubfileData.setting> savedSettings = archivedfiles4BPP[randomIndex].rdtSubfileDataList[1].spriteSettings;
                    archivedfiles4BPP[randomIndex].rdtSubfileDataList[1].spriteSettings = archivedfiles4BPP[i].rdtSubfileDataList[1].spriteSettings;
                    archivedfiles4BPP[i].rdtSubfileDataList[1].spriteSettings = savedSettings;
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
                        targetNode = targetNode.Nodes.Find(letter, false)[0];
                    }
                }
                NullNodesInArchivedFileOrder.Add(targetNode.Nodes.Add("nullNode"));
                NullNodesInArchivedFileOrder[NullNodesInArchivedFileOrder.Count - 1].Text = "nullNode";
                NullNodesInArchivedFileOrder[NullNodesInArchivedFileOrder.Count - 1].Name = "nullNode";
                nodeTreeSize += 5;
            }



            //make byte array version of node tree

            nodeTree = new byte[nodeTreeSize];
            data = new List<byte>();

            posInNodeTree = 0;

            AddSubNodesToByteArray(root, nodeTree);


            //now combine all the elements to make a new rdt file

            List<Byte> output = new List<Byte>();

            //add header
            output.Add(0x52);   //R
            output.Add(0x44);   //D
            output.Add(0x54);   //T
            output.Add(0x01);   //01
            output.Add(0x01);   //01
            output.Add(0xEF);   //is corrected to total filesize later
            output.Add(0xEF);   //is corrected to total filesize later
            output.Add(0xEF);   //is corrected to total filesize later
            output.Add(0xEF);   //is corrected to total filesize later
            output.Add(0x01);   //01
            output.Add(0x00);   //00
            output.Add((byte)((0x11 + nodeTree.Length) & 0xFF));
            output.Add((byte)(((0x11 + nodeTree.Length) >> 8) & 0xFF));
            output.Add((byte)(((0x11 + nodeTree.Length) >> 16) & 0xFF));
            output.Add((byte)(((0x11 + nodeTree.Length) >> 24) & 0xFF));
            output.Add((byte)(archivedfiles.Count & 0xFF));
            output.Add((byte)((archivedfiles.Count >> 8) & 0xFF));

            for (int i = 0; i < nodeTree.Length; i++)
            {
                output.Add(nodeTree[i]);
            }

            for (int i = 0; i < data.Count; i++)
            {
                output.Add(data[i]);
            }

            output[5] = (byte)(output.Count & 0xFF);
            output[6] = (byte)((output.Count >> 8) & 0xFF);
            output[7] = (byte)((output.Count >> 16) & 0xFF);
            output[8] = (byte)((output.Count >> 24) & 0xFF);


            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            if (!is_only_sprite_container)
            {
                saveFileDialog1.FileName = Path.GetFileName(filename);
                saveFileDialog1.Title = "Save rdt file";
                saveFileDialog1.Filter = "1PP resource data (*.rdt)|*.rdt";
            }
            else
            {
                saveFileDialog1.FileName = Path.GetFileName(archivedfiles[0].filename);
                saveFileDialog1.Title = "Save sprite";
                saveFileDialog1.Filter = "1PP sprite data (*.sprite)|*.sprite";
            }

            saveFileDialog1.CheckPathExists = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = saveFileDialog1.FileName;
                File.WriteAllBytes(saveFileDialog1.FileName, output.ToArray());

                if (!is_only_sprite_container)
                {
                    Console.WriteLine("test");
                    form1.ParseRdt(filename);
                    form1.MakeFileTree();
                }
            }
        }


        public void AddSubNodesToByteArray(TreeNode node, Byte[] nodeTree)
        {

            nodeTree[posInNodeTree] = (byte)node.Nodes.Count;
            posInNodeTree++;
            int basePos = posInNodeTree;

            posInNodeTree = basePos + (node.Nodes.Count * 5);


            for (int i = 0; i < node.Nodes.Count; i++)
            {
                if (node.Nodes[i].Text == "nullNode")
                {
                    nodeTree[basePos + (i * 5)] = 0x00;
                    archivedfile file = archivedfiles[NullNodesInArchivedFileOrder.IndexOf(node.Nodes[i])];


                    List<rdtSubfileData> backuplist = new List<rdtSubfileData>(file.rdtSubfileDataList);

                    //prepare data for import

                    List<Image> images = new List<Image>();
                    List<Byte[]> palettes = new List<Byte[]>();

                    List<rdtSubfileData> savedImages = new List<rdtSubfileData>();
                    List<rdtSubfileData> savedPalettes = new List<rdtSubfileData>();



                    foreach (rdtSubfileData subfiledata in file.rdtSubfileDataList) //get palettes
                    {
                        if (subfiledata.graphicsType == "palette")
                        {
                            subfiledata.DecompressLZ10IfCompressed();
                            palettes.Add(subfiledata.filebytes);
                            savedPalettes.Add(subfiledata);
                        }
                    }

                    int imageIndex = 0;
                    List<int> skipIndices = new List<int>();

                    bool is_modified = false;

                    List<short> offsetsX = new List<short>();
                    List<short> offsetsY = new List<short>();

                    foreach (rdtSubfileData subfiledata in file.rdtSubfileDataList) //get images
                    {
                        if (subfiledata.graphicsType == "image")
                        {
                            if (subfiledata.image != null && !(file.RDTSpriteBPP == 3 || file.RDTSpriteBPP == 5)) //if it was modified or viewed by the user, use the modified one
                            {                               //oh, and we're ignoring modifications to "3BPP" and "5BPP" images (the ones with variable alpha)
                                images.Add(subfiledata.image);
                                savedImages.Add(subfiledata);
                                is_modified = true;
                            }
                            else          //otherwise, just read the existing image for the first time and apply the existing palette
                            {
                                subfiledata.LoadImage(form1.GetPalette(palettes[imageIndex], 1, file.RDTSpriteBPP));
                                images.Add(subfiledata.image);
                                skipIndices.Add(images.Count - 1); //so that we know not to bother rereading it and its palette
                                savedImages.Add(subfiledata);
                            }
                            offsetsX.Add(subfiledata.offsetX);
                            offsetsY.Add(subfiledata.offsetY);

                            imageIndex++;
                        }
                    }

                    //remove the existing images and palette subfiledata in the archivedfile
                    int indexOfFirstImageOrPalette = 0;

                    foreach (rdtSubfileData f in file.rdtSubfileDataList)
                    {
                        if (f.graphicsType != "image" && f.graphicsType != "palette")
                        {
                            indexOfFirstImageOrPalette++;
                            continue;
                        }
                        break;
                    }

                    file.rdtSubfileDataList.RemoveRange(indexOfFirstImageOrPalette, file.rdtSubfileDataList.Count - indexOfFirstImageOrPalette);


                    //now we add the updated images back to the list


                    //make global palette for sprite

                    rdtSubfileData newPalette = new rdtSubfileData();



                    // read individual frames

                    for (int j = 0; j < images.Count; j++)
                    {
                        //make palette 

                        newPalette = new rdtSubfileData();

                        if (skipIndices.Contains(j))   //no need to reread image data, as the user didn't edit or view it
                        {
                            newPalette = savedPalettes[0];
                            newPalette.graphicsType = "palette";
                            newPalette.DecompressLZ10IfCompressed();
                            file.rdtSubfileDataList.Add(newPalette);
                            file.rdtSubfileDataList.Add(savedImages[j]);
                        }
                        else                   //the user edited or viewed this file, so rebuild the image and palette
                        {
                            //make palette
                            newPalette = new rdtSubfileData();
                            Color[] palette = new Color[0];

                            if (file.RDTSpriteBPP == 4)
                            {
                                palette = new Color[16];
                            }
                            else if (file.RDTSpriteBPP == 8)
                            {
                                palette = new Color[256];
                            }

                            //put image colours in palette

                            if (file.RDTSpriteAlphaColour.A == 0 && file.RDTSpriteAlphaColour.R == 0 && file.RDTSpriteAlphaColour.G == 0 && file.RDTSpriteAlphaColour.B == 0)
                            {
                                file.RDTSpriteAlphaColour = form1.GetPalette(palettes[j], 1, file.RDTSpriteBPP)[0];
                            }

                            Color[] coloursToAdd = Get_Unique_Colours(images, palette.Length);
                            Array.Copy(coloursToAdd, 0, palette, 0, coloursToAdd.Length);
                            //Console.WriteLine("number of unique colours: " + coloursToAdd.Length);

                            file.RDTSpriteAlphaColour = Color.FromArgb(0x00, file.RDTSpriteAlphaColour.R, file.RDTSpriteAlphaColour.G, file.RDTSpriteAlphaColour.B); //make sure the dummy A value is 0x00

                            //now make sure the alpha colour is at index 0
                            if (palette[0] != file.RDTSpriteAlphaColour)
                            {
                                int checkIndex = 0;

                                while (checkIndex < palette.Length) //go through the palette looking for the alpha colour's current position
                                {
                                    if (palette[checkIndex] == file.RDTSpriteAlphaColour)
                                    {
                                        break;
                                    }
                                    checkIndex++;
                                }

                                //swap the alpha colour into index 0, and the index 0 colour to where the alpha colour used to be
                                palette[checkIndex] = palette[0];
                                palette[0] = file.RDTSpriteAlphaColour;
                            }

                            //CREATE BINARY NBFC IMAGE AND PALETTE, THEN MAKE SUBFILEDATAS FOR THEM AND ADD THEM TO LIST

                            //create binary palette


                            newPalette.subfileType = 0x04;
                            newPalette.graphicsType = "palette";

                            newPalette.filebytes = new byte[1 + (palette.Length * 2)];

                            int colorindex = 0;

                            foreach (Color c in palette)
                            {
                                ushort ABGR1555Color = form1.ColorToABGR1555(c);
                                newPalette.filebytes[1 + (colorindex * 2)] = (byte)(ABGR1555Color & 0x00FF);
                                newPalette.filebytes[2 + (colorindex * 2)] = (byte)((ABGR1555Color & 0xFF00) >> 8);

                                colorindex++;
                            }

                            file.rdtSubfileDataList.Add(newPalette);

                            //create binary image here

                            rdtSubfileData newImage = new rdtSubfileData();
                            newImage.subfileType = 0x04;

                            int fakeWidth = images[j].Width;   //fakewidth should only be used when setting the size of the byte array, and nowhere else!

                            if (file.RDTSpriteBPP == 4)
                            {
                                while (fakeWidth % 2 != 0)
                                {
                                    fakeWidth++;
                                }
                                newImage.filebytes = new byte[8 + ((fakeWidth / 2) * images[j].Height)];
                            }
                            else
                            {
                                newImage.filebytes = new byte[8 + (fakeWidth * images[j].Height)];
                            }

                            form1.WriteU16ToArray(newImage.filebytes, 0, (ushort)images[j].Width);
                            form1.WriteU16ToArray(newImage.filebytes, 2, (ushort)images[j].Height);
                            form1.WriteS16ToArray(newImage.filebytes, 4, offsetsX[j]);
                            form1.WriteS16ToArray(newImage.filebytes, 6, offsetsY[j]);

                            int curOffset = 8;

                            Bitmap imageTemp = (Bitmap)images[j];

                            Color newPixel;

                            if (file.RDTSpriteBPP == 4)
                            {
                                for (int y = 0; y < imageTemp.Height; y++)
                                {
                                    for (int x = 0; x < imageTemp.Width; x++)
                                    {
                                        newPixel = imageTemp.GetPixel(x, y);
                                        newImage.filebytes[curOffset] = (byte)(newImage.filebytes[curOffset] | (byte)form1.FindIndexOfColorInPalette(palette, Color.FromArgb(newPixel.A, newPixel.R & 0xF8, newPixel.G & 0xF8, newPixel.B & 0xF8)));
                                        if (x < imageTemp.Width - 1)
                                        {
                                            x++;
                                            newPixel = imageTemp.GetPixel(x, y);
                                            newImage.filebytes[curOffset] = (byte)(newImage.filebytes[curOffset] | (byte)(form1.FindIndexOfColorInPalette(palette, Color.FromArgb(newPixel.A, newPixel.R & 0xF8, newPixel.G & 0xF8, newPixel.B & 0xF8)) << 4));
                                        }

                                        curOffset++;
                                    }
                                }
                            }
                            else
                            {
                                for (int y = 0; y < imageTemp.Height; y++)
                                {
                                    for (int x = 0; x < imageTemp.Width; x++)
                                    {
                                        newPixel = imageTemp.GetPixel(x, y);
                                        newImage.filebytes[curOffset] = (byte)form1.FindIndexOfColorInPalette(palette, Color.FromArgb(newPixel.A, newPixel.R & 0xF8, newPixel.G & 0xF8, newPixel.B & 0xF8));
                                        curOffset++;
                                    }
                                }
                            }

                            file.rdtSubfileDataList.Add(newImage);
                        }
                    }

                    form1.WriteU16ToArray(file.rdtSubfileDataList[2].filebytes, 0, file.RDTSpriteNumFrames);
                    form1.WriteU16ToArray(file.rdtSubfileDataList[2].filebytes, 2, file.RDTSpriteWidth);
                    form1.WriteU16ToArray(file.rdtSubfileDataList[2].filebytes, 4, file.RDTSpriteHeight);
                    file.rdtSubfileDataList[2].filebytes[6] = (byte)file.RDTSpriteBPP;

                    file.rdtSubfileDataList[3].filebytes = new byte[file.RDTSpriteNumFrames * 2];

                    for (int j = 0; j < file.RDTSpriteFrameDurations.Count; j++)
                    {
                        form1.WriteU16ToArray(file.rdtSubfileDataList[3].filebytes, j * 2, file.RDTSpriteFrameDurations[j]);
                    }



                    file.rdtSubfileDataList[1].RebuildFilebytesFromSettings();  //rebuild centre bounds etc config


                    int offsetOfSubfileTable = 0x11 + nodeTree.Length + data.Count;

                    file.rdtSubfileDataList[0].filebytes = new Byte[8 + ((file.rdtSubfileDataList.Count - 1) * 4)];  //make space in the subfile table

                    file.rdtSubfileDataList[0].filebytes[0] = 2;
                    file.rdtSubfileDataList[0].filebytes[4] = 1;
                    form1.WriteU16ToArray(file.rdtSubfileDataList[0].filebytes, 0x0A, (ushort)(file.rdtSubfileDataList.Count - 2));

                    int pos_in_subfiletable = 0x0C;


                    //a lot of things need to be updated before the following happens! I don't know if they all read into their filebytes in the first place, so you need to rebuild them first!

                    int spritePaletteOffset = 0; //global palette for the sprite so that it can just be pasted around


                    foreach (rdtSubfileData subfiledata in file.rdtSubfileDataList)
                    {
                        subfiledata.writeAddress = 0;

                        int offsetOfThisSubfile = 0;
                        bool dontWrite = false;

                        int offset_in_subfiletable_where_address_should_be_written = (offsetOfSubfileTable - (0x11 + nodeTree.Length)) + pos_in_subfiletable;

                        if (file.rdtSubfileDataList.IndexOf(subfiledata) > 0)   //if it's not the subfile table, add an entry to the subfile table
                        {
                            if (offsetOfThisSubfile == 0)   //if it wasn't written to by the previous check
                            {
                                offsetOfThisSubfile = 0x11 + nodeTree.Length + data.Count;
                                subfiledata.writeAddress = offsetOfThisSubfile;
                            }

                            if (subfiledata.graphicsType == "palette" && spritePaletteOffset != 0)  //if it's not the first palette to be processed, just store a reference to the earlier palette
                            {
                                dontWrite = true;
                                offsetOfThisSubfile = spritePaletteOffset;
                            }
                            else if (subfiledata.graphicsType == "palette") //but if it is the first one, store its offset so we can reference it later
                            {
                                spritePaletteOffset = offsetOfThisSubfile;
                            }


                            subfiledata.writeAddress = offsetOfThisSubfile;


                            data[offset_in_subfiletable_where_address_should_be_written] = (byte)(offsetOfThisSubfile & 0x000000FF);
                            data[offset_in_subfiletable_where_address_should_be_written + 1] = (byte)((offsetOfThisSubfile & 0x0000FF00) >> 8);
                            data[offset_in_subfiletable_where_address_should_be_written + 2] = (byte)((offsetOfThisSubfile & 0x00FF0000) >> 16);
                            data[offset_in_subfiletable_where_address_should_be_written + 3] = (byte)((offsetOfThisSubfile & 0xFF000000) >> 24);

                            if (pos_in_subfiletable == 0x0C)
                            {
                                pos_in_subfiletable += 2;
                            }

                            pos_in_subfiletable += 4;
                        }

                        //now write the subfile file into data

                        if (dontWrite)  //if it's just an instance of an existing subfile, don't write a new one
                        {
                            continue;
                        }


                        //compress if needed

                        if (subfiledata.subfileType == 0x04)
                        {
                            subfiledata.filebytes = DSDecmp.NewestProgram.Compress(subfiledata.filebytes, new DSDecmp.Formats.Nitro.LZ10());
                        }

                        //check for identical files that were already processed

                        if (file.rdtSubfileDataList.IndexOf(subfiledata) > 0 && subfiledata.graphicsType != "palette")
                        {
                            foreach (Byte[] alreadyProcessedFile in AlreadyProcessedFilesAndOffsetsInData.Keys) //if this file is equal to one that was already processed
                            {
                                if (ByteArraysAreEqual(alreadyProcessedFile, subfiledata.filebytes))
                                {
                                    //this file is identical to one that was already processed, so just store a reference to the existing one
                                    dontWrite = true;

                                    data[offset_in_subfiletable_where_address_should_be_written] = (byte)AlreadyProcessedFilesAndOffsetsInData[alreadyProcessedFile];
                                    data[offset_in_subfiletable_where_address_should_be_written + 1] = (byte)(AlreadyProcessedFilesAndOffsetsInData[alreadyProcessedFile] >> 8);
                                    data[offset_in_subfiletable_where_address_should_be_written + 2] = (byte)(AlreadyProcessedFilesAndOffsetsInData[alreadyProcessedFile] >> 16);
                                    data[offset_in_subfiletable_where_address_should_be_written + 3] = (byte)(AlreadyProcessedFilesAndOffsetsInData[alreadyProcessedFile] >> 24);
                                    break;
                                }
                            }
                        }


                        if (dontWrite)  //check again for dontwrite
                        {
                            continue;
                        }

                        //otherwise, write subfiletype and size, then write filebytes

                        AlreadyProcessedFilesAndOffsetsInData.Add(subfiledata.filebytes, offsetOfThisSubfile);
                        //Console.WriteLine(AlreadyProcessedFilesAndOffsetsInData.Count);

                        data.Add((byte)subfiledata.subfileType);
                        data.Add((byte)0);

                        data.Add((byte)(subfiledata.filebytes.Length & 0x000000FF));
                        data.Add((byte)((subfiledata.filebytes.Length & 0x0000FF00) >> 8));
                        data.Add((byte)((subfiledata.filebytes.Length & 0x00FF0000) >> 16));
                        data.Add((byte)((subfiledata.filebytes.Length & 0xFF000000) >> 24));

                        foreach (Byte b in subfiledata.filebytes)
                        {
                            data.Add(b);
                        }
                    }

                    form1.WriteU32ToArray(nodeTree, basePos + (i * 5) + 1, (uint)offsetOfSubfileTable); //write offset of the subfile table to the last node in the string

                    file.rdtSubfileDataList = backuplist;
                }
                else
                {
                    nodeTree[basePos + (i * 5)] = (byte)node.Nodes[i].Text[0];
                    form1.WriteU32ToArray(nodeTree, basePos + (i * 5) + 1, 0x11 + (uint)posInNodeTree);
                    AddSubNodesToByteArray(node.Nodes[i], nodeTree);
                }
            }
        }


        public bool ByteArraysAreEqual(Byte[] array1, Byte[] array2)
        {

            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array2[i] != array1[i])
                {
                    return false;
                }
            }

            //  Console.WriteLine("Byte array match");
            return true;
        }



        public Color[] Get_Unique_Colours(List<Image> input, int maxColours)
        {

            List<Color> output = new List<Color>();

            Color potentialNewColour;

            foreach (Image img in input)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    for (int x = 0; x < img.Width; x++)
                    {
                        potentialNewColour = ((Bitmap)img).GetPixel(x, y);
                        potentialNewColour = Color.FromArgb(0x00, potentialNewColour.R & 0xF8, potentialNewColour.G & 0xF8, potentialNewColour.B & 0xF8);

                        if (!ListAlreadyContainsColour(output, potentialNewColour))
                        {
                            if (output.Count >= maxColours)
                            {
                                MessageBox.Show("The frames in this animation do not all share the same palette.\nImport will continue, but the extra colours will be discarded and will be transparent in-game.",
                                "Warning");

                                return output.ToArray();
                            }

                            output.Add(potentialNewColour);
                        }
                    }
                }
            }

            return output.ToArray();
        }


        public bool ListAlreadyContainsColour(List<Color> list, Color checkColour)
        {

            foreach (Color c in list)
            {
                if ((checkColour.R & 0xF8) == (c.R & 0xF8) && (checkColour.G & 0xF8) == (c.G & 0xF8) && (checkColour.B & 0xF8) == (c.B & 0xF8))
                {
                    return true;
                }
            }

            return false;
        }


        List<TreeNode> NodesForBatchExport = new List<TreeNode>();
        public void ExportFolder(TreeNode topnode)
        {
            NodesForBatchExport = new List<TreeNode>();

            NodesForBatchExport.Add(topnode);

            RecursivelyAddChildrenToExportList(topnode);

            //now we should have all child, grandchild, etc folders

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.FileName = "Save here";

            saveFileDialog1.Title = "Choose folder";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.Filter = "Directory |directory";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(saveFileDialog1.FileName);
                foreach (TreeNode folder in NodesForBatchExport)
                {
                    foreach (TreeNode child in folder.Nodes)
                    {
                        if (form1.treeNodesAndArchivedFiles.ContainsKey(child)) //if it's a file
                        {
                            archivedfile file = form1.treeNodesAndArchivedFiles[child];

                            string path;
                            if (file.filename[0] == '/')
                            {
                                path = Path.GetDirectoryName(saveFileDialog1.FileName) + file.filename.Replace('/', '\\');
                            }
                            else
                            {
                                path = Path.GetDirectoryName(saveFileDialog1.FileName) + "\\" + file.filename.Replace('/', '\\');
                            }

                            if (file.filename == "FILENAME_NOT_SET")
                            {
                                path = Path.GetDirectoryName(saveFileDialog1.FileName) + "\\Names_not_found\\" + file.hash + "." + file.filemagic;
                            }

                            Console.WriteLine(path);
                            file.ReadFile();
                            File.WriteAllBytes(form1.GetOrMakeDirectoryForFileName(path.Replace('/', '\\')), file.filebytes);
                        }
                    }
                }
            }
        }

        public void RecursivelyAddChildrenToExportList(TreeNode node)
        {
            foreach (TreeNode child in node.Nodes)  //for each child of this node
            {
                if (!form1.treeNodesAndArchivedFiles.ContainsKey(node)) //if it's a folder
                {
                    NodesForBatchExport.Add(child);
                    RecursivelyAddChildrenToExportList(child);
                }
            }
        }




    }

}
