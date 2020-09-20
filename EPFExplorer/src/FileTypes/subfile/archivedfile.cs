using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSDecmp;
using Ekona;
using System.Windows.Forms;

namespace EPFExplorer
{

    

    public class archivedfile
    {
        public Form1 form1;
        public SpriteEditor spriteEditor;

        public uint hash;
        public int offset;
        public int size;
        public byte[] filebytes;
        public uint filemagic;
        public string filename = "FILENAME_NOT_SET";

        public arcfile parentarcfile;
        public rdtfile parentrdtfile;

        public TreeNode treeNode;

        public bool should_this_file_be_decompressed__and_compressed_when_read = true;

        public bool has_LZ11_filesize = false;

        public bool was_LZ10_compressed = false;
        public bool was_LZ11_compressed = false;

        public List<string> STstrings = new List<string>(); //used if it's an ST file

        Byte textFileStringType = 0x00;


        //these rdt subfiledatas are kept in an archivedfile so that everything is compatible with the TreeView, with only minor changes needed
        public List<rdtSubfileData> rdtSubfileDataList = new List<rdtSubfileData>();    //the first one in this list should always be the subfile's table

        public ushort RDTSpriteNumFrames = 1;
        public ushort RDTSpriteWidth = 0;
        public ushort RDTSpriteHeight = 0;
        public byte RDTSpriteBPP = 4;
        public List<ushort> RDTSpriteFrameDurations = new List<ushort>();
        public Color RDTSpriteAlphaColour;


        public archivedfile()
        {

        }

        public archivedfile(archivedfile basis)
        {
        form1 = basis.form1;
        spriteEditor = basis.spriteEditor;

        hash = basis.hash;
        offset = basis.offset;
        size = basis.size;
        filebytes = basis.filebytes;
        filemagic = basis.filemagic;
        filename = basis.filename;

        parentarcfile = basis.parentarcfile;
        parentrdtfile = basis.parentrdtfile;

        treeNode = basis.treeNode;

        should_this_file_be_decompressed__and_compressed_when_read = basis.should_this_file_be_decompressed__and_compressed_when_read;

        has_LZ11_filesize = basis.has_LZ11_filesize;

        was_LZ10_compressed = basis.was_LZ10_compressed;
        was_LZ11_compressed = basis.was_LZ11_compressed;

        STstrings = basis.STstrings;

        textFileStringType = basis.textFileStringType;
        
        rdtSubfileDataList = basis.rdtSubfileDataList;  

        RDTSpriteNumFrames = basis.RDTSpriteNumFrames;
            RDTSpriteWidth = basis.RDTSpriteWidth;
            RDTSpriteHeight = basis.RDTSpriteHeight;
            RDTSpriteBPP = basis.RDTSpriteBPP;
            RDTSpriteFrameDurations = basis.RDTSpriteFrameDurations;
            RDTSpriteAlphaColour = basis.RDTSpriteAlphaColour;
        }

        public void ExportToFile() {        //when you export an individual file (so that it asks where you want to save it)

            if (filebytes == null || filebytes.Length == 0)
            {
                ReadFile(); //make sure the file is loaded into filebytes
                DecompressFile();
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();


            if (filename == "FILENAME_NOT_SET")
                {
                saveFileDialog1.FileName = hash + "." + filemagic;
                }
            else
                {
                switch (Path.GetExtension(filename).ToLower())
                    {
                    case ".st":
                        saveFileDialog1.Filter = "txt files (*.txt)|*.txt|st files (*.st)|*.st|All files (*.*)|*.*";
                        saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(filename) + ".txt";
                        break;
                    case ".st2":
                        saveFileDialog1.Filter = "txt files (*.txt)|*.txt|st2 files (*.st2)|*.st2|All files (*.*)|*.*";
                        saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(filename) + ".txt";
                        break;
                    default:
                        string extension = Path.GetExtension(filename);
                        saveFileDialog1.Filter = extension.Replace(".", "") + " files (*" + extension + ")|*" + extension + "|All files (*.*)|*.*";
                        saveFileDialog1.FileName = Path.GetFileName(filename);
                        break;
                    }
                }

            saveFileDialog1.Title = "Export file";
            saveFileDialog1.CheckPathExists = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Export(saveFileDialog1.FileName);
            }
        }

        public void Export(string path) //if doing a batch export, this will get called directly (so that it doesn't open a savefiledialog for every individual file)
        {
            if (Path.GetExtension(path).ToLower() == ".txt")
            {
                File.WriteAllLines(path, STstrings);
            }
            else
            {
                File.WriteAllBytes(path, filebytes);
            }
        }

        public void ReplaceFile()
        {
            ReadFile();
            DecompressFile();   //Must read file and decompress, so that we know whether or not to recompress the replacement file

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Replace with file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;


            //choose how to approach the savefiledialog based on the type of file we are replacing
            if (filename == "FILENAME_NOT_SET")
                {
                openFileDialog1.Filter = "All files (*.*)|*.*";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                    filebytes = File.ReadAllBytes(openFileDialog1.FileName);
                    }
                }
            else
                {
                switch (Path.GetExtension(filename))
                    {
                    case ".lua":
                    case ".luc":
                        openFileDialog1.Filter = "Compiled lua files (*.luc, *.luac, *.out)|*.luc;*.luac;*.out";
                        
                        if (openFileDialog1.ShowDialog() == DialogResult.OK)
                            {
                            filebytes = File.ReadAllBytes(openFileDialog1.FileName);
                            }
                        break;

                    case ".st":
                        openFileDialog1.Filter = "text files (*.txt, *.st)|*.txt;*.st";
                        if (openFileDialog1.ShowDialog() == DialogResult.OK)
                            {
                            if (Path.GetExtension(openFileDialog1.FileName) == ".txt")
                                {
                                Console.WriteLine("read new STstrings txt");

                                filebytes = new byte[1];    //set filebytes to at least 1, so that it realises this file has been modified

                                STstrings = File.ReadAllLines(openFileDialog1.FileName).ToList();
                            }
                            else
                                {
                                filebytes = File.ReadAllBytes(openFileDialog1.FileName);
                                }
                            }
                        break;
                    default:
                        openFileDialog1.Title = "Replace "+Path.GetFileName(filename);
                        if (openFileDialog1.ShowDialog() == DialogResult.OK)
                            {
                            filebytes = File.ReadAllBytes(openFileDialog1.FileName);
                            }
                        break;
                    }
                }  
        }

        public void ReadFile() { //actually reads the file into filebytes
            
        if (parentarcfile != null)  //if it's an arc subfile
            {
                if (offset != 0)
                {
                    filebytes = new byte[size];
                    Array.Copy(parentarcfile.filebytes, offset, filebytes, 0, size);
                }

                DecompressFile();   //check for compression

                switch (Path.GetExtension(filename).ToLower())
                {
                    case ".st":
                        ReadTextFileEPF();
                        break;
                    case ".st2":
                        ReadTextFileHR();
                        break;
                }
            }
        else if (parentrdtfile != null) //if it's an rdt subfile
            {
                rdtSubfileData subfileTable = new rdtSubfileData();   //read this subfile's table
                int overallIndex = 0;
                subfileTable.IndexInList = overallIndex;

                subfileTable.parentfile = this;
                subfileTable.ReadRawData(parentrdtfile.filebytes,offset);
                
                rdtSubfileDataList.Add(subfileTable);
                overallIndex++;

                if (subfileTable.filebytes == null || subfileTable.filebytes.Length == 0)
                    {
                    return;
                    }

                //now read the subfile table
                int numlists = BitConverter.ToInt32(subfileTable.filebytes,0);  //number of lists e.g. 2: the list of centre bounds, the list of compressed files
                Console.WriteLine(numlists);

                int curOffset = 4;  //offset in subfile table filebytes

                //From what I can tell, the file offsets in an RDT initially lead to a table (beginning with ID 0x03),
                //which contains lists of file types. The first one tends to be a centre bounds etc file, describing the
                //sprite. The next list tends to contain LZ10 compressed stuff, where the first file is the offset of 
                //animation metadata. Then, frame durations. Then after that, it's the offsets of alternating 
                //palettes and sprites. All these offsets are relative to the start of the RDT, which is a pain...

                for (int i = 0; i < numlists; i++)
                    {
                    int countInList = BitConverter.ToUInt16(subfileTable.filebytes,curOffset);
                    curOffset += 2;
                    for (int f = 0; f < countInList; f++)
                        {
                        rdtSubfileData newSubfile = new rdtSubfileData();
                        newSubfile.IndexInList = overallIndex;
                        newSubfile.parentfile = this;
                        newSubfile.ReadRawData(parentrdtfile.filebytes,BitConverter.ToInt32(subfileTable.filebytes,curOffset));
                        overallIndex++;

                        if (f == 0) { newSubfile.graphicsType = "GraphicsMetadata";}
                        else if (f == 1) { newSubfile.graphicsType = "GraphicsFrameDurations";}

                        if (f > 1)
                            {
                            if (f % 2 == 0)
                                {
                                newSubfile.graphicsType = "palette";
                                }
                            else
                                {
                                newSubfile.graphicsType = "image";
                                }
                            }

                        newSubfile.Parse();
                        rdtSubfileDataList.Add(newSubfile);
                        curOffset += 4;
                        }   
                    }

                filebytes = new Byte[1];    //set dummy filebytes so that this file registers as read by EPFExplorer
            }
        }


        public void OpenRDTSubfileInEditor() { 
        
        if (spriteEditor == null)   //if this hasn't been opened in the spriteeditor before, read the file
            {
                if (rdtSubfileDataList.Count == 0)
                    {
                    ReadFile();
                    }

                spriteEditor = new SpriteEditor();
                spriteEditor.sprite = this;
                spriteEditor.Show();

                spriteEditor.images = new List<rdtSubfileData>();
                spriteEditor.palettes = new List<rdtSubfileData>();

                //if (RDTSpriteBPP == 3)
                 //   {
                 //   Console.WriteLine("3BPP? oof. aborting.");
                 //   spriteEditor.Close();
                 //   return;
                 //   }

                foreach (rdtSubfileData file in rdtSubfileDataList) //get all the images and all the palettes
                    {
                    if (file.subfileType == 0x04)
                        {
                        if (file.graphicsType == "image")
                            {
                            spriteEditor.images.Add(file);
                            }
                        else if (file.graphicsType == "palette")
                            {
                            spriteEditor.palettes.Add(file);
                            }
                        }
                    }

                if (spriteEditor.images.Count == 0)
                    {
                    spriteEditor.Close();
                    MessageBox.Show("Image data not found...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                    }

                spriteEditor.RequestSpriteEditorImage(spriteEditor.curFrame);

                spriteEditor.ready = false;
            foreach (rdtSubfileData.setting s in rdtSubfileDataList[1].spriteSettings)
                {   
                switch (s.name)
                    {
                    case "isOAMSprite":
                        spriteEditor.OAMSpriteCheckbox.Checked = s.trueOrFalse;
                        break;
                    case "looping":
                        spriteEditor.loopingCheckbox.Checked = s.trueOrFalse;
                        break;
                    case "rotatable":
                        spriteEditor.rotatableCheckbox.Checked = s.trueOrFalse;
                        break;
                    case "center":
                        spriteEditor.centreX.Value = s.X;
                        spriteEditor.centreY.Value = s.Y;
                        break;
                    case "bounds":
                        spriteEditor.boundsX.Value = s.X;
                        spriteEditor.boundsY.Value = s.Y;
                        spriteEditor.boundsX2.Value = s.X2;
                        spriteEditor.boundsY2.Value = s.Y2;
                        break;

                    default:
                        Console.WriteLine("unhandled name "+s.name);
                        break;
                    }
                }
                spriteEditor.ready = true;
            }
        else
            {
                spriteEditor.BringToFront();
            }
        }



        public void DecompressFile() {

            if (size > 4 && filebytes != null && hash != 3088773188)
            {
                if ((filebytes[1] != 0x00 || filebytes[2] != 0x00) && filebytes[3] == 0x00)
                {
                    if (filebytes[0] == 0x10)
                    {
                        Console.WriteLine("looks like an LZ10 compressed file at offset " + offset);
                        filebytes = DSDecmp.NewestProgram.Decompress(filebytes, new DSDecmp.Formats.Nitro.LZ10());
                        was_LZ10_compressed = true;
                    }
                    else if (filebytes[0] == 0x11)
                    {
                        Console.WriteLine("looks like an LZ11 compressed file at offset " + offset);
                        filebytes = DSDecmp.NewestProgram.Decompress(filebytes, new DSDecmp.Formats.Nitro.LZ11());
                        was_LZ11_compressed = true;
                    }
                }
            }

            if (was_LZ10_compressed == false && was_LZ11_compressed == false)
                {
                Console.WriteLine("not every file was originally compressed");
                }


            if (filebytes == null)
            {
                Console.WriteLine("It seems DSDecmp may have messed up the decompression...");
                Console.WriteLine("file hash was: " + hash);
                return;
            }
        }


        public void CompressFileLZ10()
        {
            filebytes = DSDecmp.NewestProgram.Compress(filebytes, new DSDecmp.Formats.Nitro.LZ10());
            Console.WriteLine(filebytes[0]);
            was_LZ10_compressed = true;
            has_LZ11_filesize = false;

            if (filebytes == null)
            {
                Console.WriteLine("It seems DSDecmp may have messed up the compression...");
                Console.WriteLine("file hash was: " + hash);
                return;
            }
        }

        public void CompressFileLZ11()
        {
            filebytes = DSDecmp.NewestProgram.Compress(filebytes, new DSDecmp.Formats.Nitro.LZ11());
            was_LZ11_compressed = true;
            has_LZ11_filesize = true;

            if (filebytes == null)
            {
                Console.WriteLine("It seems DSDecmp may have messed up the compression...");
                Console.WriteLine("file hash was: " + hash);
                return;
            }
        }


        public string ReplaceSpecialChars(string input) {
            input = input.Replace(((char)0xAC) + "", "[endtextbox]");  //our placeholder for null
            input = input.Replace(((char)0x0A) + "", "[newline]");
            input = input.Replace(((char)0x0B) + "", "[playername]");
            return input;
        }

        public string RestoreSpecialChars(string input)
        {
            input = input.Replace("[endtextbox]", (char)0xAC+ "");   //our placeholder for null, because if we use 0x00, it counts it as string termination
            input = input.Replace("[newline]", (char)0x0A + "");
            input = input.Replace("[playername]", (char)0x0B + "");
            input = input.Replace('’', '\'');
            input = input.Replace('‘', '\'');
            input = input.Replace('“', '\"');
            input = input.Replace('”', '\"');
            input = input.Replace("…", "...");
            return input;
        }


        public void checkfilemagic()
        {
            filemagic = 99999999;   //unset

            if (filename != "FILENAME_NOT_SET")
                {
                return; //don't bother with auto-detection if we already know the filename, if we need to decompress, we can do it when the user actually select the file
                }

            //the following is all really shoddy, but it only activates if the real filename couldn't be located

            ReadFile();
            DecompressFile();

            if (filebytes.Length == 0x20 || filebytes.Length == 0x200)   //then it's probably a palette
                {
                filemagic = 0x7066626E;
                return;
                }

            if (filebytes.Length > 8)
            {
                filemagic = BitConverter.ToUInt32(new byte[] { filebytes[6], filebytes[7], filebytes[8], filebytes[9] }, 0);

                if (filebytes[0] == 0x00 && filebytes[1] != 0x00 && filebytes[4] == 0x00) //tentatively, text
                    {
                    filemagic = 0x74786574;
                    if (!parentarcfile.uniquefilemagicsandfreq.ContainsKey(filemagic))
                        {
                        //creating file magic entry in the dictionary
                        parentarcfile.uniquefilemagicsandfreq.Add(filemagic, 1);
                        ReadTextFileEPF();
                        }
                    else
                        {
                        //do not need to create entry
                        parentarcfile.uniquefilemagicsandfreq[filemagic] += 1;
                        ReadTextFileEPF();
                        }
                    }

                if (filebytes[1] != 0x00 && filebytes[2] != 0x00 && filebytes[2] != filebytes[1] && filebytes[3] == filebytes[1] && filebytes[4] != filebytes[1] && filebytes[5] == filebytes[1] && filebytes[6] != filebytes[1] && filebytes[7] == filebytes[1] && filebytes[8] != filebytes[1] && filebytes[9] == filebytes[1]) //tentatively, mpb
                {
                    filemagic = 0x4D504200;
                    if (!parentarcfile.uniquefilemagicsandfreq.ContainsKey(filemagic))
                    {
                        //creating file magic entry in the dictionary
                        parentarcfile.uniquefilemagicsandfreq.Add(filemagic, 1);
                    }
                    else
                    {
                        //do not need to create entry
                        parentarcfile.uniquefilemagicsandfreq[filemagic] += 1;
                    }
                }

                if (filebytes.Length > 0x0F && filebytes[0] == 0x00 && filebytes[1] == 0x00 && filebytes[2] != 0x00 && filebytes[3] == 0x00 && filebytes[4] != 0x00 && filebytes[5] == 0x00 && filebytes[6] != 0x00 && filebytes[7] == 0x00 && filebytes[8] != 0x00 && filebytes[9] == 0x00 && filebytes[10] != 0x00 && filebytes[11] == 0x00 && filebytes[12] != 0x00 && filebytes[13] == 0x00 && filebytes[14] != 0x00 && filebytes[15] == 0x00) //tentatively, font
                {
                    filemagic = 0x544E4F46;
                    if (!parentarcfile.uniquefilemagicsandfreq.ContainsKey(filemagic))
                    {
                        //creating file magic entry in the dictionary
                        parentarcfile.uniquefilemagicsandfreq.Add(filemagic, 1);
                    }
                    else
                    {
                        //do not need to create entry
                        parentarcfile.uniquefilemagicsandfreq[filemagic] += 1;
                    }
                }

                filemagic = BitConverter.ToUInt32(new byte[] { filebytes[1], filebytes[2], filebytes[3], filebytes[4] }, 0);

                switch (filemagic)
                {
                    case 0x5161754C:    //Luac file
                        if (!parentarcfile.uniquefilemagicsandfreq.ContainsKey(filemagic))
                        {
                            //creating file magic entry in the dictionary
                            parentarcfile.uniquefilemagicsandfreq.Add(filemagic, 1);
                        }
                        else
                        {
                             //do not need to create entry
                            parentarcfile.uniquefilemagicsandfreq[filemagic] += 1;
                        }

                        //LUAs in normal arcs are LZ10 compressed, whereas arcs embedded in save files, they are LZ11 compressed.

                        if (filebytes[0] == 0x10)
                        {   
                            filebytes = DSDecmp.NewestProgram.Decompress(filebytes,new DSDecmp.Formats.Nitro.LZ10());
                        }
                        else if (filebytes[0] == 0x11)
                        {
                            filebytes = DSDecmp.NewestProgram.Decompress(filebytes, new DSDecmp.Formats.Nitro.LZ11());
                        }

                        break;
                }
            }

            if (filebytes.Length > 7)
            {
                if (!parentarcfile.uniquefilemagicsandfreq.ContainsKey(filemagic))//if it still hasn't been assigned
                {
                    filemagic = BitConverter.ToUInt16(new byte[] { filebytes[4], filebytes[5] }, 0);

                    switch (filemagic)
                    {
                        case 0xAF12:    //FMV
                            if (!parentarcfile.uniquefilemagicsandfreq.ContainsKey(filemagic))
                            {
                                //creating file magic entry in the dictionary
                                parentarcfile.uniquefilemagicsandfreq.Add(filemagic, 1);
                            }
                            else
                            {
                                //do not need to create entry
                                parentarcfile.uniquefilemagicsandfreq[filemagic] += 1;
                            }
                            break;

                        default:
                            filemagic = 99999999;   //unset
                            break;
                    }
                }
            }
           
            if (filebytes.Length > 3)
            {
                if (!parentarcfile.uniquefilemagicsandfreq.ContainsKey(filemagic))//if it still hasn't been assigned
                {
                    filemagic = BitConverter.ToUInt16(new byte[] { filebytes[0], filebytes[1] }, 0);

                    switch (filemagic)
                    {
                        case 0x7C1F:    //maybesprite
                            if (!parentarcfile.uniquefilemagicsandfreq.ContainsKey(filemagic))
                            {
                                //creating file magic entry in the dictionary
                                parentarcfile.uniquefilemagicsandfreq.Add(filemagic, 1);

                            }
                            else
                            {
                                //do not need to create entry
                                parentarcfile.uniquefilemagicsandfreq[filemagic] += 1;
                            }
                            break;
                    }

                    if (!parentarcfile.uniquefilemagicsandfreq.ContainsKey(filemagic))//if it still hasn't been assigned
                        {
                        filemagic = BitConverter.ToUInt32(new byte[] { filebytes[0], filebytes[1], filebytes[2], filebytes[3] }, 0);

                        if (!parentarcfile.uniquefilemagicsandfreq.ContainsKey(filemagic))
                            {
                            //creating file magic entry in the dictionary
                            parentarcfile.uniquefilemagicsandfreq.Add(filemagic, 1);
                            }
                        else
                            {
                            //do not need to create entry
                            parentarcfile.uniquefilemagicsandfreq[filemagic] += 1;
                            }
                    }
                }  
            }
        }


        public Color[] GetPalette(Byte[] input, int offset, byte bpp) {

            Color[] palette = new Color[0];
            
            if (bpp == 4)
            {
                palette = new Color[16];

                for (int i = 0; i < 16; i++)
                {
                    palette[i] = form1.ABGR1555_to_RGBA32(BitConverter.ToUInt16(input, offset));
                    offset += 2;
                }
            }
            else if (bpp == 8)
            {
                palette = new Color[256];

                for (int i = 0; i < 256; i++)
                {
                    palette[i] = form1.ABGR1555_to_RGBA32(BitConverter.ToUInt16(input, offset));
                    offset += 2;
                }
            }
            else if (bpp == 3)
            {
                palette = new Color[8];

                Console.WriteLine("3BPP");

                for (int i = 0; i < 8; i++)
                {
                    palette[i] = form1.ABGR1555_to_RGBA32(BitConverter.ToUInt16(input, offset));
                    offset += 2;
                }
            }
            else if (bpp == 5)
            {
                palette = new Color[32];

                Console.WriteLine("5BPP");

                for (int i = 0; i < 32; i++)
                {
                    palette[i] = form1.ABGR1555_to_RGBA32(BitConverter.ToUInt16(input, offset));
                    offset += 2;
                }
            }


            return palette;
        }


        public Bitmap NBFCtoImage(Byte[] input, int offset, int width, int height, Color[] palette, byte bpp)  //palettes aren't always the same length, this function is designed for the image in the downloadable newsletter
        {
            Bitmap bm = new Bitmap(width,height);
            
            int curOffset = offset;

            if (bpp == 4)
            {
                bm = new Bitmap(width, height);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    { //each nibble is one pixel

                        //first nibble
                        Color c = palette[input[curOffset] & 0x0F];
                        bm.SetPixel(x, y, c);
                        x++;

                        if (x >= width) //check whether or not the line ended midway through the byte (i.e. if the width is odd). If so, don't read the second nibble, it's unused
                        {
                            curOffset++;
                            continue;
                        }

                        //second nibble
                        c = palette[(input[curOffset] & 0xF0) >> 4];
                        bm.SetPixel(x, y, c);
                        curOffset++;
                    }
                }
            }
            else if (bpp == 8)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color c = palette[input[offset + (y * width) + x]];
                        bm.SetPixel(x, y, c);

                    }
                }
            }
            else if (bpp == 3) //it seems this doesn't produce the correct image. However, it at least allows for 3BPP and prevents an exception
            {
                bm = new Bitmap(width, height);

                int currentBitInByte = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (x >= width) //check whether or not the line ended midway through the byte
                        {
                            curOffset++;
                            continue;
                        }

                        if (currentBitInByte > 7)
                        {
                            currentBitInByte = 0 + (currentBitInByte - 7);
                            curOffset++;
                        }

                        //3 bits
                        Color c = palette[(input[curOffset] >> currentBitInByte) & 0x07];
                        bm.SetPixel(x, y, c);
                        currentBitInByte += 3;
                    }
                }
            }
            else if (bpp == 5) //nothing yet
            {
                Console.WriteLine("5BPP image not yet handled");
            }

                return bm;
            }




        public void ReadTextFileEPF() {

            STstrings = new List<string>();

            int count = BitConverter.ToInt32(filebytes, 1);

            int offset = 0;

            string newString = "";

            if (filebytes[0] == 0x00)   //then the numbers in the index table describe the offsets of the strings
                {
                textFileStringType = 0x00;
                count = count - 1;  //need to do this or it tries to read past end of file

                for (int i = 0; i < count; i++) //for each string
                    {
                    offset = BitConverter.ToInt32(filebytes, 5 + (i * 4));
                    int endOffset = BitConverter.ToInt32(filebytes, 5 + ((i + 1) * 4));
                    newString = "";

                    while (offset != endOffset)
                    {
                        if (filebytes[offset] == 0x00)
                            {
                            newString += "[endtextbox]";
                            }
                        else
                            {
                            newString += (char)BitConverter.ToUInt16(filebytes, offset) + "";
                            }
                        offset += 2;
                    }

                    newString = ReplaceSpecialChars(newString);
                    STstrings.Add(newString);
                    }
                }
            else if (filebytes[0] == 0x01)  //then the numbers in the index table describe the end offsets of the strings
                {
                textFileStringType = 0x01;
                count = count - 1;  //need to do this or it tries to read past end of file

                int datastart = 5 + (count * 4) + 4;
                offset = datastart;

                for (int i = 0; i < count; i++) //for each string
                    {
                    offset = datastart + BitConverter.ToInt32(filebytes, 5 + (i * 4));
                    Console.WriteLine("offset "+offset);
                    int endOffset = (datastart + BitConverter.ToInt32(filebytes, 5 + ((i + 1) * 4))) - 2;   //it's minus 2 so that we ignore the null termination at the end
                    newString = "";

                    while (offset != endOffset)
                        {
                        if (filebytes[offset] == 0x00)
                            {
                            newString += "[endtextbox]";
                            }
                        else
                            {
                            newString += (char)BitConverter.ToUInt16(filebytes, offset) + "";
                            }
                        offset += 2;
                        }

                    newString = ReplaceSpecialChars(newString);
                    STstrings.Add(newString);
                    }
                }
            else
                {
                MessageBox.Show("Unknown ST file format with first byte "+filebytes[0], "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
        }


        public void ReadTextFileHR()
        {
            STstrings = new List<string>();

            ushort offsetOfLanguageTable = BitConverter.ToUInt16(filebytes, 0);


            int LanguageIndex = 0;

            int offset = offsetOfLanguageTable;

            while (filebytes[offset] != 0x00)   //if there is another language to process
                {
                string languagename = "";

                while (filebytes[offset] != 0x00) //get name of language
                    {
                    languagename += (char)filebytes[offset];
                    offset++;
                    }

                offset = offsetOfLanguageTable + (LanguageIndex * 0x20) + 0x18;

                int offsetOfIndexTable = BitConverter.ToInt32(filebytes, offset);
                offset += 0x04;

                int offsetOfData = BitConverter.ToInt32(filebytes, offset);
                offset += 0x04;

                int count = (offsetOfData - offsetOfIndexTable) / 0x10;

                STstrings.Add(languagename);

                for (int i = 0; i < count; i++)
                    {
                    uint hash = BitConverter.ToUInt32(filebytes, offsetOfIndexTable + (i * 0x10));

                    offset = BitConverter.ToInt32(filebytes, offsetOfIndexTable + (i * 0x10) + 0x04);

                    if (offset == 0x00000000)
                        {
                        //yeah no, we reached the end of the strings
                        break;
                        }

                    string newstring = "";

                    for (int pos = 0; pos < BitConverter.ToInt32(filebytes, offsetOfIndexTable + (i * 0x10) + 0x08); pos+=2)
                        {
                        if (BitConverter.ToUInt16(filebytes, offset) != 0x0000)
                            {
                            newstring += (char)BitConverter.ToUInt16(filebytes, offset) + "";
                            }

                        offset += 2; 
                        }
                    newstring = ReplaceSpecialChars(newstring);
                    STstrings.Add(newstring);
                    }

                LanguageIndex++;
                offset = offsetOfLanguageTable + (LanguageIndex * 0x20);
                } 
        }


        public void MakeSTfromStringsEPF() {

            Console.WriteLine("making ST from strings");

            if(STstrings.Count == 0)
                {
                return;
                }

            int sizeOfNewFile = 5 + (4 * STstrings.Count) + 4;

            List<string> formattedStrings = new List<string>(STstrings);

            for(int i = 0; i < formattedStrings.Count; i++)
                {
                formattedStrings[i] = RestoreSpecialChars(formattedStrings[i]);
                sizeOfNewFile += (formattedStrings[i].Length * 2);
                if (textFileStringType == 0x01) 
                    {
                    sizeOfNewFile += 2; //to account for the null bytes at the ends of strings 
                    }
                }

            filebytes = new byte[sizeOfNewFile];

            filebytes[0] = textFileStringType;

            parentarcfile.form1.WriteIntToArray(filebytes, 1, formattedStrings.Count + 1);

            int currentPos = 5 + (4 * (formattedStrings.Count + 1)); //set this to the start of data

            if (textFileStringType == 0x00) //measures offsets of strings from start of file
                {
                for (int i = 0; i < formattedStrings.Count; i++)
                    {
                    //write offset to index table
                    parentarcfile.form1.WriteIntToArray(filebytes, 5 + (i * 4), currentPos);

                    Console.WriteLine(formattedStrings[i]);

                    foreach (char c in formattedStrings[i])
                        {
                        if ((byte)c == 0xAC)    //replace our null placeholder with actual null
                            {
                            parentarcfile.form1.WriteU16ToArray(filebytes, currentPos, 0x0000);
                            }
                        else
                            {
                            parentarcfile.form1.WriteU16ToArray(filebytes, currentPos, (ushort)c);
                            }
                        currentPos += 2;
                        }
                    }

                //then write EOF to the end of the index table
                parentarcfile.form1.WriteIntToArray(filebytes, 5 + (4 * formattedStrings.Count), currentPos);
                }
            else if (textFileStringType == 0x01) //measures offsets of strings from start of data
            {
                currentPos = 0; //in this case, it is measured from the start of data

                for (int i = 0; i < formattedStrings.Count; i++)
                {
                    //write offset to index table
                    parentarcfile.form1.WriteIntToArray(filebytes, 5 + (i * 4), currentPos);

                    foreach (char c in formattedStrings[i])
                    {
                        if ((byte)c == 0xAC)    //replace our null placeholder with actual null
                        {
                            parentarcfile.form1.WriteU16ToArray(filebytes, 5 + (4 * STstrings.Count) + 4 + currentPos, 0x0000);
                        }
                        else
                        {
                            parentarcfile.form1.WriteU16ToArray(filebytes, 5 + (4 * STstrings.Count) + 4 + currentPos, (ushort)c);
                        }
                        currentPos += 2;
                    }
                    parentarcfile.form1.WriteU16ToArray(filebytes, 5 + (4 * STstrings.Count) + 4 + currentPos, (ushort)0x0000); //write null bytes to terminate string
                    currentPos += 2;
                }

                //then write EOF to the end of the index table
                parentarcfile.form1.WriteIntToArray(filebytes, 5 + (4 * formattedStrings.Count), currentPos);
            }
        }
    }
}
