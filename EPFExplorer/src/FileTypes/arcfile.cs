using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EPFExplorer
{
    public class arcfile
    {
        public string arcname;
        public string filename;
        public byte[] filebytes;

        public uint filecount;

        public List<archivedfile> archivedfiles = new List<archivedfile>();

        public Dictionary<uint, int> uniquefilemagicsandfreq = new Dictionary<uint, int>();

        public Dictionary<uint, string> knownfilemagic = new Dictionary<uint, string>();


        public bool write_filenames = true;


        public Form1 form1;

        public bool allow_writing_custom_filename_table = true;

        public bool use_custom_filename_table = true;

        bool foundMatch = false;

        int number_of_filenames_successfully_applied = 0;


        List<TreeNode> NodesForBatchExport = new List<TreeNode>();

        public arcfile()
        {
            knownfilemagic.Add(0x74786574, "st");
            knownfilemagic.Add(0x74786500, "st2");
            knownfilemagic.Add(0x30444D42, "nsbmd");
            knownfilemagic.Add(0x30414342, "nsbca");
            knownfilemagic.Add(0xAF12, "flc");  //FLIC animation
            knownfilemagic.Add(0x7C1F, "tsb"); //tileset
            knownfilemagic.Add(0x5161754C, "luc");
            knownfilemagic.Add(0x544E4F46, "fnt");
            knownfilemagic.Add(0x4D504200, "mpb");
            knownfilemagic.Add(0x01544452, "rdt");
            knownfilemagic.Add(0x7066626E, "nbfp");
        }

        public void DisplayFileMagicFreq()
        {

            foreach (uint filemagic in uniquefilemagicsandfreq.Keys)
            {
                if (knownfilemagic.Keys.Contains(filemagic))
                {
                    Console.WriteLine(knownfilemagic[filemagic] + " Freq: " + uniquefilemagicsandfreq[filemagic]);
                }
                else
                {
                    Console.WriteLine(filemagic + " Freq: " + uniquefilemagicsandfreq[filemagic]);
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

                            file.Export(form1.GetOrMakeDirectoryForFileName(path.Replace('/', '\\').Replace(".luc", ".lua")));
                        }
                    }
                }
            }
        }

        public archivedfile GetFileByName(string filename)
        {

            if (filebytes == null || filebytes.Length == 0)
            {
                return null;
            }

            if (archivedfiles == null || archivedfiles.Count == 0)
            {
                ReadArc();
            }

            filename = filename.ToLower();

            foreach (archivedfile file in archivedfiles)
            {
                if (form1.CalculateHash(filename) == file.hash)
                {
                    return file;
                }
            }
            return null;
        }


        public void ExportAll()
        {

            string RootFolderName = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename));

            if (!Directory.Exists(RootFolderName))
            {
                Directory.CreateDirectory(RootFolderName);
            }

            string NamesNotFoundFolderName = Path.Combine(RootFolderName, "Names_not_found");

            if (!Directory.Exists(NamesNotFoundFolderName))
            {
                Directory.CreateDirectory(NamesNotFoundFolderName);
            }

            foreach (archivedfile file in archivedfiles)
            {
                if (knownfilemagic.ContainsKey(file.filemagic))
                {
                    if (file.filebytes != null)
                    {
                        if (file.filename != "FILENAME_NOT_SET")
                        {

                            File.WriteAllBytes(form1.GetOrMakeDirectoryForFileName(RootFolderName + "\\" + file.filename), file.filebytes);
                        }
                        else
                        {
                            File.WriteAllBytes(NamesNotFoundFolderName + "\\" + file.hash + "." + knownfilemagic[file.filemagic], file.filebytes);
                        }
                    }
                    if (file.filemagic == 0x4D504200)
                    {
                        Byte[] hashAsBytes = BitConverter.GetBytes(file.hash);
                        //Console.WriteLine(BitConverter.ToString(hashAsBytes));
                    }
                }
                else
                {
                    if (file.filebytes != null)
                    {
                        if (file.filename != "FILENAME_NOT_SET")
                        {
                            File.WriteAllBytes(form1.GetOrMakeDirectoryForFileName(RootFolderName + "\\" + file.filename), file.filebytes);
                        }
                        else
                        {
                            File.WriteAllBytes(NamesNotFoundFolderName + "\\" + file.hash + "." + file.filemagic.ToString(), file.filebytes);
                        }
                    }
                }
            }

            MessageBox.Show(filecount + " files were extracted to " + RootFolderName + "\\", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }




        public void SetArchivedFileName(int targetFileIndex, string s_for_filename)
        {

            if (targetFileIndex < filecount)
            {
                if (archivedfiles[targetFileIndex].filename != "FILENAME_NOT_SET")
                {
                    Console.WriteLine(archivedfiles[targetFileIndex].filename + " and " + s_for_filename + " are contesting the same hash!");
                }

                number_of_filenames_successfully_applied++;

                archivedfiles[targetFileIndex].filename = s_for_filename;

                foundMatch = true;
            }
        }



        public void TryToFindFilename(string s)
        {


            foundMatch = false;

            s = s.Replace("/scripts/", "/chunks/");
            s = s.Replace(".lua", ".luc");

            int targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(s.ToLower()), this); //try lower case string

            string s_for_filename = s;

            Console.WriteLine(s);

            //ok, this bit is actually pretty fragile. I changed it once and half the filenames disappeared. Maybe don't change it.

            if (targetFileIndex >= filecount || targetFileIndex < 0)   //if we couldn't find that exact hash in the arc file, try a variant of the filename to see if THAT has a hash in the arc file, and if so, set targetFileIndex accordingly
            {
                //Console.WriteLine("filename variant generator, activate!");

                targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(s), this); //try normal case
                if (targetFileIndex < filecount) { /*Console.WriteLine("method A worked");*/ SetArchivedFileName(targetFileIndex, s); } //if we succeeded, go to later on.

                targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(s.Replace(".luc", ".lua").ToLower()), this); //lower case luc
                if (targetFileIndex < filecount) {  /*Console.WriteLine("method B worked"); */ SetArchivedFileName(targetFileIndex, s.Replace(".luc", ".lua").ToLower()); } //if we succeeded, go to later on.

                targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(s.Replace(".luc", ".lua").Replace("chunks/", "scripts/").ToLower()), this); //lower case luc
                if (targetFileIndex < filecount) {  /*Console.WriteLine("method B2 worked"); */ SetArchivedFileName(targetFileIndex, s.Replace(".luc", ".lua").Replace("chunks/", "scripts/").ToLower()); } //if we succeeded, go to later on.


                targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(s.Replace(".luc", ".lua")), this); //normal case luc
                if (targetFileIndex < filecount) {  /*Console.WriteLine("method C worked"); */ SetArchivedFileName(targetFileIndex, s.Replace(".luc", ".lua")); } //if we succeeded, go to later on.

                targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(s.Replace(".luc", ".lua").Replace("chunks/", "scripts/")), this); //normal case luc
                if (targetFileIndex < filecount) {  /*Console.WriteLine("method C2 worked"); */ SetArchivedFileName(targetFileIndex, s.Replace(".luc", ".lua").Replace("chunks/", "scripts/")); } //if we succeeded, go to later on.


                targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(s.Substring(1, s.Length - 1)), this); //try without the initial slash
                if (targetFileIndex < filecount) {  /*Console.WriteLine("method D worked"); */ SetArchivedFileName(targetFileIndex, s.Substring(1, s.Length - 1)); } //if we succeeded, go to later on.

                targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(s.Substring(1, s.Length - 1).ToLower()), this); //try lower case without the initial slash
                if (targetFileIndex < filecount) {  /*Console.WriteLine("method E worked"); */ SetArchivedFileName(targetFileIndex, s.Substring(1, s.Length - 1).ToLower()); } //if we succeeded, go to later on.


                if (!s.Contains("."))   //if it doesn't currently have an extension, try some extensions
                {

                    targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(s.Replace(".lua", ".luc")), this);
                    if (targetFileIndex < filecount) {  /*Console.WriteLine("method F worked"); */ SetArchivedFileName(targetFileIndex, s.ToLower().Replace(".lua", ".luc")); } //if we succeeded, go to later on.

                    targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(s.ToLower().Replace(".lua", ".luc.l")), this); //try lower case
                    if (targetFileIndex < filecount) {  /*Console.WriteLine("method G worked"); */ SetArchivedFileName(targetFileIndex, s.ToLower().Replace(".lua", ".luc.l")); } //if we succeeded, go to later on.

                    targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(s.ToLower().Replace(".lua", ".luc.lz")), this); //try lower case
                    if (targetFileIndex < filecount) {  /*Console.WriteLine("method H worked");*/ SetArchivedFileName(targetFileIndex, s.ToLower().Replace(".lua", ".luc.lz")); } //if we succeeded, go to later on.

                    foreach (string possibleExtension in form1.extensions)
                    {
                        targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(s + possibleExtension), this);   //try possible extensions
                        if (targetFileIndex < filecount) {  /*Console.WriteLine("method I"); */ s_for_filename = s + possibleExtension; SetArchivedFileName(targetFileIndex, s_for_filename); } //if we succeeded, go to later on. Also set s to the string we just tested, because the extension version is preferable

                        targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(s + (possibleExtension.ToUpper())), this);   //also try extension in caps
                        if (targetFileIndex < filecount) {  /*Console.WriteLine("method J"); */ s_for_filename = s + (possibleExtension.ToUpper()); SetArchivedFileName(targetFileIndex, s_for_filename); } //if we succeeded, go to later on. Also set s to the string we just tested, because the extension version is preferable

                        targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash((s + possibleExtension).ToLower()), this); //also try lower case
                        if (targetFileIndex < filecount) {  /*Console.WriteLine("method K"); */ s_for_filename = (s + possibleExtension).ToLower(); SetArchivedFileName(targetFileIndex, s_for_filename); } //if we succeeded, go to later on. Also set s to the string we just tested, because the extension version is preferable

                        targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(Path.GetFileName(s).ToLower() + possibleExtension), this); //try only filename, with a new extension, lower case
                        if (targetFileIndex < filecount) {  /*Console.WriteLine("method L"); */ SetArchivedFileName(targetFileIndex, Path.GetFileName(s).ToLower() + possibleExtension); } //if we succeeded, go to later on.

                        targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(Path.GetFileName(s).ToLower() + possibleExtension + ".lz"), this); //try only filename, with a new extension, lower case
                        if (targetFileIndex < filecount) {  /*Console.WriteLine("method M"); */ SetArchivedFileName(targetFileIndex, Path.GetFileName(s).ToLower() + possibleExtension + ".lz"); } //if we succeeded, go to later on.

                        targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(Path.GetFileName(s).ToLower() + possibleExtension + ".lzc"), this); //try only filename, with a new extension, lower case
                        if (targetFileIndex < filecount) {  /*Console.WriteLine("method N"); */ SetArchivedFileName(targetFileIndex, Path.GetFileName(s).ToLower() + possibleExtension + ".lzc"); } //if we succeeded, go to later on.

                        targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(Path.GetFileName(s).ToLower() + possibleExtension + ".l"), this); //try only filename, with a new extension, lower case
                        if (targetFileIndex < filecount) {  /*Console.WriteLine("method O"); */ SetArchivedFileName(targetFileIndex, Path.GetFileName(s).ToLower() + possibleExtension + ".l"); } //if we succeeded, go to later on.

                        targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash((s.Replace(".lua", ".luc") + possibleExtension).ToLower()), this); //also try replacing lua with luc and adding an extension
                        if (targetFileIndex < filecount) {  /*Console.WriteLine("method P"); */ s_for_filename = (s.Replace(".lua", ".luc") + possibleExtension).ToLower(); SetArchivedFileName(targetFileIndex, s_for_filename); } //if we succeeded, go to later on. Also set s to the string we just tested, because the extension version is preferable

                        if (s[0] == '/')
                        {
                            //now try without the initial slash
                            targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(s.Substring(1, s.Length - 1) + possibleExtension), this);   //try possible extensions
                            if (targetFileIndex < filecount) {  /*Console.WriteLine("method Q");*/ s_for_filename = s.Substring(1, s.Length - 1) + possibleExtension; SetArchivedFileName(targetFileIndex, s_for_filename); } //if we succeeded, go to later on. Also set s to the string we just tested, because the extension version is preferable

                            targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(s.Substring(1, s.Length - 1) + (possibleExtension.ToUpper())), this);   //also try extension in caps
                            if (targetFileIndex < filecount) {  /*Console.WriteLine("method R");*/ s_for_filename = s.Substring(1, s.Length - 1) + (possibleExtension.ToUpper()); SetArchivedFileName(targetFileIndex, s_for_filename); } //if we succeeded, go to later on. Also set s to the string we just tested, because the extension version is preferable

                            targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash((s.Substring(1, s.Length - 1) + possibleExtension).ToLower()), this); //also try lower case
                            if (targetFileIndex < filecount) {  /*Console.WriteLine("method S");*/ s_for_filename = (s.Substring(1, s.Length - 1) + possibleExtension).ToLower(); SetArchivedFileName(targetFileIndex, s_for_filename); } //if we succeeded, go to later on. Also set s to the string we just tested, because the extension version is preferable
                        }
                    }
                }

                //otherwise, try more variations...

                string s_with_backslashes_instead = s.Replace("/", "\\");    //to make sure the Path functions operate correctly

                targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(s_with_backslashes_instead), this); //try filename, but with backslashes instead of forward slashes
                if (targetFileIndex < filecount)
                {  /*Console.WriteLine("method T"); SetArchivedFileName(targetFileIndex, s_with_backslashes_instead); } //if we succeeded, go to later on.

                targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(s_with_backslashes_instead.ToLower()), this); //try filename, but with backslashes instead of forward slashes, and lower case
                if (targetFileIndex < filecount) {  /*Console.WriteLine("method U");*/
                    SetArchivedFileName(targetFileIndex, s_with_backslashes_instead.ToLower());
                } //if we succeeded, go to later on.

                targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(Path.GetFileNameWithoutExtension(s)), this); //try filename without extension
                if (targetFileIndex < filecount) { /* Console.WriteLine("method V");*/ SetArchivedFileName(targetFileIndex, Path.GetFileNameWithoutExtension(s)); } //if we succeeded, go to later on.

                targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(Path.GetFileNameWithoutExtension(s).ToLower()), this); //try filename without extension, lower case
                if (targetFileIndex < filecount)
                {  /*Console.WriteLine("method W");*/
                    SetArchivedFileName(targetFileIndex, Path.GetFileNameWithoutExtension(s).ToLower());
                } //if we succeeded, go to later on.

                targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(Path.GetFileName(s)), this); //try only filename, with extension
                if (targetFileIndex < filecount) {  /*Console.WriteLine("method X");*/ SetArchivedFileName(targetFileIndex, Path.GetFileName(s)); } //if we succeeded, go to later on.

                targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(Path.GetFileName(s).ToLower()), this); //try only filename, with extension, lower case
                if (targetFileIndex < filecount) {  /*Console.WriteLine("method Y");*/ SetArchivedFileName(targetFileIndex, Path.GetFileName(s).ToLower()); } //if we succeeded, go to later on.

                targetFileIndex = form1.Find_Closest_File_To(form1.CalculateHash(Path.GetFileName(s).ToLower().Replace(".lua", ".luc")), this); //try only filename, with luc extension, lower case
                if (targetFileIndex < filecount) {  /*Console.WriteLine("method Z");*/ Console.WriteLine("does anything actually get this far"); SetArchivedFileName(targetFileIndex, Path.GetFileName(s).ToLower().Replace(".lua", ".luc")); } //if we succeeded, go to later on.

            }
            else
            {
                //If we got a valid filename from the above, then assign it to the file
                SetArchivedFileName(targetFileIndex, s_for_filename);
            }

            if (s.Contains(".lua") || s.Contains(".luc") && !foundMatch)
            {
                //Console.WriteLine("Couldn't find a matching hash for " + s);
            }

        }

        public void ReadArc()
        {

            number_of_filenames_successfully_applied = 0;

            filecount = form1.readU32FromArray(filebytes, 0);

            Console.WriteLine(filecount);
            for (int i = 0; i < filecount; i++)     //read file information from the arc's index table.
            {
                archivedfile newarchivedfile = new archivedfile();
                archivedfiles.Add(newarchivedfile);
                newarchivedfile.parentarcfile = this;
                newarchivedfile.hash = form1.readU32FromArray(filebytes, 0x04 + (0x0C * i));
                newarchivedfile.offset = form1.readIntFromArray(filebytes, 0x08 + (0x0C * i));
                newarchivedfile.size = form1.readIntFromArray(filebytes, 0x0C + (0x0C * i));
                newarchivedfile.form1 = form1;

                if (newarchivedfile.size < 0)
                {
                    newarchivedfile.has_LZ11_filesize = true;
                    newarchivedfile.size = Math.Abs(newarchivedfile.size);
                }
            }

            int endOfOriginalFileData = archivedfiles[archivedfiles.Count - 1].offset + archivedfiles[archivedfiles.Count - 1].size;

            while (endOfOriginalFileData % 0x04 != 0)
            {
                endOfOriginalFileData++;
            }

            endOfOriginalFileData += 4; //skip checksum

            byte[] customFilenameTable = null;

            if (filebytes.Length > endOfOriginalFileData)  //if there's more after the end of the checksum, it's a custom names section added by this program on a previous export, we can use this to obtain the file names
            {
                Console.WriteLine("Custom filename table activated");

                use_custom_filename_table = true;

                customFilenameTable = new byte[filebytes.Length - endOfOriginalFileData];

                Array.Copy(filebytes, endOfOriginalFileData, customFilenameTable, 0, filebytes.Length - endOfOriginalFileData);

                customFilenameTable = DSDecmp.NewestProgram.Decompress(customFilenameTable, new DSDecmp.Formats.Nitro.LZ11());

                List<string> customFilenames = new List<string>();

                string newstring = "";

                if (customFilenameTable != null)
                {
                    for (int i = 0; i < customFilenameTable.Length; i++)    //go through the byte array and parse all the strings
                    {
                        char newchar = (char)customFilenameTable[i];

                        if ((byte)newchar == 0x00)
                        {
                            customFilenames.Add(newstring);
                            newstring = "";
                        }
                        else if (newchar == '?' && newstring.Length == 0)
                        {
                            customFilenames.Add("FILENAME_NOT_SET");
                            newstring = "";
                        }
                        else
                        {
                            newstring += newchar;
                        }
                    }

                    for (int i = 0; i < archivedfiles.Count; i++)   //then apply the filenames from the table to the archivedfiles
                    {
                        archivedfiles[i].filename = customFilenames[i];
                    }
                }
            }

            if (customFilenameTable == null)//otherwise, try to match hashes with the filenames obtained from ARM9
            {
                foreach (string s in form1.stringsEPF)   //go through the potential filenames in the string array and label the files in the arc as best we can
                {
                    TryToFindFilename(s);
                }

                foreach (string s in form1.stringsHR)   //go through the potential filenames in the string array and label the files in the arc as best we can
                {
                    TryToFindFilename(s);
                }
            }

            //if tuxedoDL exists in this arc, read some lua filenames out of it
            archivedfile tuxedoDL = GetFileByName("/chunks/tuxedoDL.luc");

            if (tuxedoDL != null)
            {
                tuxedoDL.ReadFile();
                tuxedoDL.DecompressFile();
                tuxedoDL.DecompileLuc(tuxedoDL.filebytes, "tuxedoDL_TEMP");
                if (tuxedoDL.was_LZ10_compressed)
                {
                    tuxedoDL.CompressFileLZ10();
                }
                else if (tuxedoDL.was_LZ11_compressed)
                {
                    tuxedoDL.CompressFileLZ11();
                }
                tuxedoDL.filebytes = new byte[0];

                string[] tuxedoDLdecompiled = File.ReadAllLines("tuxedoDL_TEMP");
                File.Delete("tuxedoDL_TEMP");

                for (int line = 0; line < tuxedoDLdecompiled.Length; line++)
                {
                    if (tuxedoDLdecompiled[line].Contains("AddDownloadItem("))
                    {
                        tuxedoDLdecompiled[line] = tuxedoDLdecompiled[line].Substring(1, tuxedoDLdecompiled[line].Length - 2).Replace("AddDownloadItem(", "").Replace(", ", ",");

                        string[] splitString = tuxedoDLdecompiled[line].Split(',');

                        string potentialLuaName = splitString[7];

                        if (potentialLuaName == "\"\"")
                        {
                            continue;
                        }

                        potentialLuaName = splitString[7].Substring(1, splitString[7].Length - 2).Replace(".lua", ".luc");

                        potentialLuaName = "/chunks/" + potentialLuaName.Substring(7, potentialLuaName.Length - 7);

                        foreach (archivedfile f in archivedfiles)
                        {
                            if ((f.filename == "" || f.filename == "FILENAME_NOT_SET" || f.filename == null) && f.hash == form1.CalculateHash(potentialLuaName.ToLower()))
                            {
                                f.filename = potentialLuaName;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < archivedfiles.Count; i++)
            {
                archivedfiles[i].checkfilemagic();
            }

            Console.WriteLine(archivedfiles.Count);

            DisplayFileMagicFreq();
        }


        public void RebuildArc()
        {

            int newArcSize = 4; //because it will at least have the file count at the start
            List<byte> filenameDataForAppendingToArc = new List<byte>();

            //sort archived files by hash first (essential - otherwise the game's interval bisection file-finding method won't work)

            foreach (archivedfile file in archivedfiles)
            {
                if (file.filename != "FILENAME_NOT_SET")
                {
                    file.hash = form1.CalculateHash(file.filename.ToLower());
                }

                if (file.filename.Contains("ANTI-CORRUPTION FILE"))
                {
                    file.hash = 0x00000000; //anti corruption needs to be at the start
                }
            }


            archivedfiles = archivedfiles.OrderBy(f => (uint)f.hash).ToList();


            //now go through and check that the contents are ready to be reimported (i.e. changing string lists into st, etc)

            foreach (archivedfile file in archivedfiles)
            {
                if (file.filebytes != null && file.filebytes.Length != 0) //if the user has already modified the file in some way, make sure it has been put back into the game's file format
                {
                    switch (Path.GetExtension(file.filename))
                    {
                        case ".st":
                            file.MakeSTfromStringsEPF();
                            break;
                        default:
                            Console.WriteLine("There's no special method for reformatting a " + Path.GetExtension(file.filename) + " file for import, so we're just going to import it as-is. Filename is " + file.filename);
                            break;
                    }
                }
                else //otherwise, read the file from the arc so that everything can be put into position later
                {
                    file.ReadFile();
                    file.DecompressFile();
                }

                //compress files again if they were decompressed before


                if (file.was_LZ10_compressed)
                {
                    file.CompressFileLZ10();
                }

                if (file.was_LZ11_compressed)
                {
                    file.CompressFileLZ11();
                }


                newArcSize += 0x0C + file.filebytes.Length; //the length of index table entry and the length of file data

                while (newArcSize % 4 != 0) //account for the padding to a multiple of 4 bytes after a file
                {
                    newArcSize++;
                }

                //add filename of this file to the custom filenames list to be appended to the end (if the user chooses)
                if (file.filename == "FILENAME_NOT_SET")
                {
                    filenameDataForAppendingToArc.Add((byte)'?');    //will be skipped over when read, but it needs to fill a space so that the other ones line up.
                }
                else
                {
                    foreach (char c in file.filename)
                    {
                        filenameDataForAppendingToArc.Add((byte)c);
                    }
                    filenameDataForAppendingToArc.Add(0x00);
                }
            }

            //compress the custom filename table, but only add its length if we're actually going to include it
            Byte[] compressedFilenameTable = DSDecmp.NewestProgram.Compress(filenameDataForAppendingToArc.ToArray(), new DSDecmp.Formats.Nitro.LZ11());

            newArcSize += 4;    //plus 4 for the checksum

            if (use_custom_filename_table && allow_writing_custom_filename_table)
            {
                newArcSize += compressedFilenameTable.Length;
            }

            filebytes = new byte[newArcSize];

            form1.WriteIntToArray(filebytes, 0, archivedfiles.Count);

            int currentPos = 4 + (archivedfiles.Count * 0x0C);

            //write files into array
            for (int i = 0; i < archivedfiles.Count; i++)
            {
                //HASH
                if (archivedfiles[i].filename == "FILENAME_NOT_SET" || archivedfiles[i].filename.Contains("ANTI-CORRUPTION FILE"))
                {
                    form1.WriteU32ToArray(filebytes, 4 + (i * 0x0C), archivedfiles[i].hash);
                }
                else
                {

                    form1.WriteU32ToArray(filebytes, 4 + (i * 0x0C), form1.CalculateHash(archivedfiles[i].filename.ToLower()));
                }

                //OFFSET
                form1.WriteIntToArray(filebytes, 8 + (i * 0x0C), currentPos);
                archivedfiles[i].offset = currentPos;   //make sure EPFExplorer's version of the file is up to date too

                //SIZE
                if (archivedfiles[i].has_LZ11_filesize)
                {
                    form1.WriteIntToArray(filebytes, 0x0C + (i * 0x0C), -archivedfiles[i].filebytes.Length);
                }
                else
                {
                    form1.WriteIntToArray(filebytes, 0x0C + (i * 0x0C), archivedfiles[i].filebytes.Length);
                }

                archivedfiles[i].size = archivedfiles[i].filebytes.Length;  //make sure EPFExplorer's version of the file is up to date too

                Array.Copy(archivedfiles[i].filebytes, 0, filebytes, currentPos, archivedfiles[i].filebytes.Length);
                currentPos += archivedfiles[i].filebytes.Length;
                while (currentPos % 4 != 0)  //pad to multiple of 4
                {
                    filebytes[currentPos] = 0x00;
                    currentPos++;
                }
            }

            //write checksum
            Byte[] checksumArea = new Byte[currentPos];
            Array.Copy(filebytes, 0, checksumArea, 0, currentPos);
            uint checksum = Crc32.Compute(checksumArea);
            form1.WriteU32ToArray(filebytes, currentPos, checksum);
            currentPos += 4;

            //write the filename table, if enabled
            if (use_custom_filename_table && allow_writing_custom_filename_table)
            {
                for (int i = 0; i < compressedFilenameTable.Length; i++)
                {
                    filebytes[currentPos] = compressedFilenameTable[i];
                    currentPos++;
                }
            }

            Console.WriteLine("Arc rebuild complete!");
            filecount = (uint)archivedfiles.Count;

            foreach (archivedfile f in archivedfiles)  //so that they don't register as read if we start editing the file again right away. Otherwise, when it next rebuilds, it will think the files are still decompressed, even though they were compressed by the previous export!
            {
                f.filebytes = null;
            }
        }

        public void ViewArcInFileTree()
        {
            form1.MakeFileTree();
        }

        public archivedfile GetFileWithHash(uint hash)
        {
            foreach (archivedfile f in archivedfiles)
            {
                if (f.hash == hash)
                {
                    return f;
                }
            }
            return null;
        }
    }
}
