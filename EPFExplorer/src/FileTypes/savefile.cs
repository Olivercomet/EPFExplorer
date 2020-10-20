using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DSDecmp;


namespace EPFExplorer
{
    public class savefile
    {
        public SaveFileEditor saveFileEditor;

        public Byte[] filebytes = new byte[0];

        public string filepath = "";

        public Game game = Game.EPF;


        public string oldPenguinName = ""; //so that it can be erased before the new one is put in
        public string oldOnlineName1 = ""; //so that it can be erased before the new one is put in
        public string oldOnlineName2 = ""; //so that it can be erased before the new one is put in
        public string oldOnlineName3 = ""; //so that it can be erased before the new one is put in

        public enum Game {
            EPF = 0x424F4E44,
            HR = 0x474F4C44
        }


        public int currentMission = 0;
        public int penguinColour = 0;

        public arcfile embeddedArc;


        public string[] MissionNamesEPF = new string[] {
        "M1: The Mystery Unfolds",
        "M2: Left to your own Devices",
        "M3: Rookie on the Rocks",
        "M4: Looking for Clues",
        "M5: All's Well That Ends Weld",
        "M6: Do It Yourself Carting",
        "M7: Double Trouble",
        "M8: Flying High Pitched",
        "M9: Super Secret Gadgets",
        "M10: Damage Control",
        "M11: Robotomy 101",
        "M12: Robots on the Run",
        "M13: An Agent's Work is Never Done",
        "Free roam"
        };

        public string[] MissionNamesHR = new string[] {
        "M1: The Elite Penguin Force",
        "M2: Secret of the Fur",
        "M3: Questions for a Crab",
        "M4: Mysterious Tremors",
        "M5: Spy & Seek",
        "M6: Waddle Squad",
        "M7: The Veggie Villain",
        "M8: Suspect At Large",
        "M9: Herbert's Plan",
        "M10: The Ultimate Mission"
        };

        public string[] PenguinColours = new string[] { //for HR, at least
        "Unchosen",
        "Light Blue",
        "Black",
        "Blue",
        "Brown",
        "Dark Green",
        "Pink",
        "Green", //0x07
        "Lime Green",
        "Orange",
        "Peach",
        "Purple",
        "Red",
        "Yellow" //0x0D
        };


        public string[] ItemsHR = new string[] {
        "Sunglasses",
        "Goggles",
        "Crab Costume 1",
        "Bow tie",
        "Suit",
        "Rucksack",
        "Life Ring",
        "Flashlight",
        "Miner Hat",
        "Crab Costume 2",
        "Spider Costume",
        "Binoculars",
        "Laptop"
        };

        public string[] ItemsEPF = new string[] {
        "Top hat red stripe", //0x01
        "Hair with tiara", //0x02
        "Nothing",  //0x03
        "Nothing", //0x04
        "Nothing", //0x05
        "Nothing", //0x06
        "NV goggles",
        "Gary’s glasses", //0x08
        "Posh suit",//0x09
        "Pink dress",//0x0A
        "Nothing",     //0x0B
        "Nothing",     //0x0C
        "snowboard", //0x0D
        "Nothing",     //0x0E
        "Black guitar", //0x0F
        "Nothing",//0x10
        "Nothing",//0x11
        "Red Bow Tie",//0x12
        "Necklace",//0x13
        "Nothing",//0x14
        "PH's hat",//0x15
        "Unknown Gold Trophy",//0x16
        "Unknown Silver Trophy",//0x17
        "Gold Mullet Medal",//0x18
        "Gold Chest Medal",//0x19
        "Cart Surfer Gold Medal",//0x1A
        "Cart Surfer Silver Medal",//0x1B
        "Night Club Gold Medal",//0x1C
        "Night Club Silver Medal",//0x1D
        "Jetpack Gold Medal",//0x1E
        "Jetpack Silver Medal",//0x1F
        "Snowboarding Gold Medal",//0x20
        "Snowboarding Silver Medal",//0x21
        "Trekker Gold Medal",//0x22
        "Trekker Silver Medal",//0x23
        "Black hoodie",//0x24
        "Parka", //0x25
        "Black suit", //0x26
        "Blue raincoat", //0x27
        "Apron", //0x28
        "Dazzle Dress", //0x29
        "Grass Skirt", //0x2A
        "Jean Jacket", //0x2B
        "Life Jacket", //0x2C
        "Long Johns", //0x2D
        "Pink Duffle Coat",//0x2E
        "Pink hoodie",//0x2F
        "Pink Ski Jacket",//0x30
        "Pink swimsuit",//0x31
        "Red Jumper",
        "Poncho",
        "Princess costume",
        "Raincoat",
        "Red shorts",
        "Construction vest",
        "Orange Ski Jacket",
        "Stripey red and yellow top",
        "Suede jacket",
        "Track jacket",
        "Sun dress",
        "Pink Flower Hoodie",
        "Purple dress",
        "3-D glasses",
        "Dress shoes",
        "Black eyeglasses",
        "Black sunglasses",
        "Diva sunglasses",
        "Funny face glasses",
        "Black sneakers",
        "Brown sandals",
        "Brown shoes",
        "Bunny slippers", //0x48
        "Flippers",
        "Hiking boots",
        "Pink sandals",
        "Fishing rod",
        "Gold watch",
        "Hockey stick",
        "Pink purse",
        "Beehive hair",
        "Black toque",
        "Blue cap",
        "Blue earmuffs",
        "Afro",
        "Diving helmet",
        "Dive mask",
        "Pink sunhat",
        "Brown female hair",
        "Blonde female hair",
        "Yellow hard hat",
        "Headphones",
        "Pink earmuffs",
        "Pink toque",
        "Russian hat",
        "Santa hat",
        "Blue mohawk",
        "Sunstriker hair",
        "Tiara",
        "Top hat",
        "Red viking helmet",
        "Black tie",
        "Blue cape",
        "Bow tie",
        "Lei",
        "Pearls",
        "Pendant",
        "Backpack",
        "Scarf",
        "Dark trenchcoat",
        "Nothing?",
        "Glacier suit",
        "Tweed coat",
        "Light trenchcoat",
        "Magnifying glass",
        "Tweed hat",
        "Fedora",
        "Magic wand",
        "Cowboy hat",
        "Red propeller cap",
        "Paper boat hat",
        "Eye patch",
        "Blue sneakers", //7A
        "Fireman helmet",
        "Blue graduate cap",
        "Pink sneakers",
        "Pink fairy wings",
        "Hockey helmet",
        "Pink cape",
        "Knight helmet",
        "Skating shoes",
        "Sombrero",
        "Fish costume",
        "Jester hat",
        "Black superhero mask",
        "Jester costume",
        "Knight costume",
        "Fireman jacket",
        "Shadow guy costume",
        "Gamma gal costume",
        "Astronaut costume",
        "Astronaut helmet",
        "Puffle Training Diploma"



        };







        public string GetStringAtOffset(int offset) {

            string output = "";

            while (filebytes[offset] != 0x00)
                {
                output += (char)filebytes[offset];
                offset += 2;
                }

            return output;
        }


        void GetUnlocks(Byte b) //for EPF at least
            {
            if ((b & 0x01) == 0x01) 
            {saveFileEditor.mapUnlockable.Checked = true;}
            else{saveFileEditor.mapUnlockable.Checked = false;}

            if ((b & 0x02) == 0x02)
            { saveFileEditor.HQteleportUnlockable.Checked = true; }
            else { saveFileEditor.HQteleportUnlockable.Checked = false; }

            if ((b & 0x04) == 0x04)
            { saveFileEditor.inventoryUnlockable.Checked = true; }
            else { saveFileEditor.inventoryUnlockable.Checked = false; }

            if ((b & 0x08) == 0x08)
            { saveFileEditor.whistleUnlockable.Checked = true; }
            else { saveFileEditor.whistleUnlockable.Checked = false; }

        }



        void GetPuffles(Byte b) //for EPF at least
        {
            if ((b & 0x01) == 0x01)
            { saveFileEditor.puffleBouncer.Checked = true; }
            else { saveFileEditor.puffleBouncer.Checked = false; }

            if ((b & 0x02) == 0x02)
            { saveFileEditor.puffleBlast.Checked = true; }
            else { saveFileEditor.puffleBlast.Checked = false; }

            if ((b & 0x04) == 0x04)
            { saveFileEditor.puffleFlare.Checked = true; }
            else { saveFileEditor.puffleFlare.Checked = false; }

            if ((b & 0x08) == 0x08)
            { saveFileEditor.pufflePop.Checked = true; }
            else { saveFileEditor.pufflePop.Checked = false; }

            if ((b & 0x10) == 0x10)
            { saveFileEditor.puffleLoop.Checked = true; }
            else { saveFileEditor.puffleLoop.Checked = false; }

            if ((b & 0x20) == 0x20)
            { saveFileEditor.puffleFlit.Checked = true; }
            else { saveFileEditor.puffleFlit.Checked = false; }

            if ((b & 0x40) == 0x40)
            { saveFileEditor.puffleChirp.Checked = true; }
            else { saveFileEditor.puffleChirp.Checked = false; }

            if ((b & 0x80) == 0x80)
            { saveFileEditor.puffleChill.Checked = true; }
            else { saveFileEditor.puffleChill.Checked = false; }
        }




        public void GetCurrentMission(ushort missionValue)     //For HR. Converts mission value into an index
        {
            currentMission = 0;
            currentMission += missionValue & 0x0001;
            currentMission += (missionValue & 0x0002) >>1;
            currentMission += (missionValue & 0x0004) >> 2;
            currentMission += (missionValue & 0x0008) >> 3;
            currentMission += (missionValue & 0x0010) >> 4;
            currentMission += (missionValue & 0x0020) >> 5;
            currentMission += (missionValue & 0x0040) >> 6;
            currentMission += (missionValue & 0x0080) >> 7;
            currentMission += (missionValue & 0x0100) >> 8;
            currentMission += (missionValue & 0x0200) >> 9;
            currentMission += (missionValue & 0x0400) >> 10;
            currentMission += (missionValue & 0x0800) >> 11;
            currentMission += (missionValue & 0x1000) >> 12;
            currentMission += (missionValue & 0x2000) >> 13;
            currentMission += (missionValue & 0x4000) >> 14;
            currentMission += (missionValue & 0x8000) >> 15;
        }

        public ushort GetNewHRMissionValue(int missionIndex)   //convert the index back into a mission value
            {
            ushort output = 0;

            int ORvalue = 1;

            for (int i = 0; i < missionIndex; i++)
                {
                output = (ushort)(output | ORvalue);

                ORvalue *= 2;
                }

            //results in a value where the number of bits set is equal to the index of the mission

            return output;
            }


        public void LoadFromFile(string FileName)
            {
            filebytes = File.ReadAllBytes(FileName);
            filepath = FileName;

            saveFileEditor.inventoryBox.Items.Clear();

            saveFileEditor.exportDownloadArc.Enabled = false;

            using (BinaryReader reader = new BinaryReader(File.Open(FileName, FileMode.Open)))
            {
                game = (Game)reader.ReadInt32();

                if (game == Game.EPF)
                {
                    reader.BaseStream.Position = 0x100;

                    if (reader.ReadInt32() == 0x00000000)
                    {
                        //then there's no embedded arc file, skip to later in the file
                    }
                    else
                    {   //load embedded arc file (e.g. puffle pranksters)
                        reader.BaseStream.Position -= 0x04;
                        embeddedArc = new arcfile();
                        embeddedArc.form1 = saveFileEditor.form1;
                        embeddedArc.filename = Path.Combine(Path.GetDirectoryName(FileName), "download.arc");

                        embeddedArc.filecount = reader.ReadUInt32();

                        reader.BaseStream.Position = (0x104 + (0x0C * embeddedArc.filecount)) - 0x08;   //go to where the offset for the last file is listed

                        int filesize = reader.ReadInt32();        //the arc is at least as big as the offset of the last file, we just need to add the size of the last file to bring us up to the end of the arc

                        int sizeToAddToTotalSize = reader.ReadInt32();

                        if (sizeToAddToTotalSize < 0)   //if it's one of those weird FF sizes (for LZ11-compressed files I guess)
                             {
                            sizeToAddToTotalSize += (-2 * sizeToAddToTotalSize);
                             }

                        filesize += sizeToAddToTotalSize;

                        while ((filesize % 4) != 0)
                            {
                            filesize++;
                            }

                        filesize += 4;  //account for checksum

                        embeddedArc.filebytes = new byte[filesize];
                        Array.Copy(filebytes, 0x100, embeddedArc.filebytes, 0, filesize);

                        embeddedArc.ReadArc();

                        //the arc is then padded with 0x00 until the end of the line, and then the CRC-32 of the arc file follows.

                        Byte[] newsletterImage = new byte[0x2960];

                        Array.Copy(filebytes,0xCCF0,newsletterImage,0,0x2960);

                        Console.WriteLine("test");
                        GetDownloadableMissionName();
                        Console.WriteLine("test2");
                        saveFileEditor.exportDownloadArc.Enabled = true;

                        //Bitmap bm = embeddedArc.archivedfiles[0].NBFCtoImage(newsletterImage);

                        //bm.Save(filepath + "image.png");


                        //File.WriteAllBytes(filepath+"image.raw",newsletterImage);

                        Console.WriteLine("Last offset was " + reader.BaseStream.Position);
                    }

                    //LOAD INVENTORY
                    reader.BaseStream.Position = 0xF650;

                    saveFileEditor.inventoryBox.Items.Clear();

                    for (int i = 0; i < ItemsEPF.Length; i++)
                    {
                        saveFileEditor.inventoryBox.Items.Add(ItemsEPF[i]);
                        if (reader.ReadByte() == 0x01) { saveFileEditor.inventoryBox.SetItemChecked(i,true);}
                    }


                    //LOAD LIFETIME COINS

                    reader.BaseStream.Position = 0xF704;
                    saveFileEditor.lifetimeCoins.Value = reader.ReadUInt32(); //lifetime coins

                    //LOAD HIGH SCORES

                    reader.BaseStream.Position = 0xF708;
                    saveFileEditor.highScore1.Value = reader.ReadUInt32(); //snowboarding

                    reader.BaseStream.Position = 0xF710;
                    saveFileEditor.highScore2.Value = reader.ReadUInt32(); //cart surfer

                    reader.BaseStream.Position = 0xF70C;
                    saveFileEditor.highScore3.Value = reader.ReadUInt32(); //ice fishing

                    reader.BaseStream.Position = 0xF714;
                    saveFileEditor.highScore5.Value = reader.ReadUInt32(); //dance challenge

                    reader.BaseStream.Position = 0xF718;
                    saveFileEditor.highScore4.Value = reader.ReadUInt32(); //jet pack adventure   //yes, I know that 4 and 5 are in the wrong order here, it's so that it matches the order shown in-game

                    reader.BaseStream.Position = 0xF71C;
                    saveFileEditor.highScore6.Value = reader.ReadUInt32(); //snow trekker

                    //READ PENGUIN NAME
                    saveFileEditor.penguinNameTextBox.Text = GetStringAtOffset(0xF720);
                    oldPenguinName = saveFileEditor.penguinNameTextBox.Text;

                    //READ ONLINE NAMES
                    saveFileEditor.onlineName1.Text = GetStringAtOffset(0xF73A);
                    oldOnlineName1 = saveFileEditor.penguinNameTextBox.Text;
                    saveFileEditor.onlineName2.Text = GetStringAtOffset(0xF754);
                    oldOnlineName2 = saveFileEditor.penguinNameTextBox.Text;
                    saveFileEditor.onlineName3.Text = GetStringAtOffset(0xF76E);
                    oldOnlineName3 = saveFileEditor.penguinNameTextBox.Text;

                    //READ COINS
                    reader.BaseStream.Position = 0xF7E4;
                    saveFileEditor.coinsChooser.Value = reader.ReadUInt32();

                    //LOAD CURRENT MISSION
                    reader.BaseStream.Position = 0xF7EC;
                    currentMission = reader.ReadByte();

                    saveFileEditor.currentMissionChooser.Items.Clear();

                    foreach (string s in MissionNamesEPF)
                    {
                        saveFileEditor.currentMissionChooser.Items.Add(s);
                    }

                    saveFileEditor.currentMissionChooser.SelectedIndex = currentMission;

                    //READ PENGUIN COLOUR
                    reader.BaseStream.Position = 0xF7F0;
                    penguinColour = reader.ReadByte();

                    saveFileEditor.colourChooser.Items.Clear();

                    foreach (string s in PenguinColours)
                    {
                        saveFileEditor.colourChooser.Items.Add(s);
                    }

                    saveFileEditor.colourChooser.SelectedIndex = penguinColour;

                    //LOAD PUFFLES 
                    reader.BaseStream.Position = 0xF7FB;
                    GetPuffles(reader.ReadByte());

                    //LOAD UNLOCKS (map, puffle whistle, etc.)
                    reader.BaseStream.Position = 0xF7FC;
                    GetUnlocks(reader.ReadByte());




                    //in Herbert's Revenge at least, the u16 at offset 0xA6 is a bitfield describing which missions are unlocked
                    //(number of bits set + 1) = current mission
                }
                else if (game == Game.HR)
                {
                    //READ PENGUIN COLOUR
                    reader.BaseStream.Position = 0x24;
                    penguinColour = reader.ReadByte();

                    saveFileEditor.colourChooser.Items.Clear();

                    foreach (string s in PenguinColours)
                    {
                        saveFileEditor.colourChooser.Items.Add(s);
                    }

                    saveFileEditor.colourChooser.SelectedIndex = penguinColour;

                    //READ COINS
                    reader.BaseStream.Position = 0x28;
                    saveFileEditor.coinsChooser.Value = reader.ReadUInt32();

                    //READ PENGUIN NAME
                    saveFileEditor.penguinNameTextBox.Text = GetStringAtOffset(0x58);
                    oldPenguinName = saveFileEditor.penguinNameTextBox.Text;

                    //READ CURRENT MISSION
                    reader.BaseStream.Position = 0xA6;
                    GetCurrentMission(reader.ReadUInt16());

                    saveFileEditor.currentMissionChooser.Items.Clear();

                    foreach (string s in MissionNamesHR)
                        {
                        saveFileEditor.currentMissionChooser.Items.Add(s);
                        }

                    saveFileEditor.currentMissionChooser.SelectedIndex = currentMission;

                    //LOAD INVENTORY
                    reader.BaseStream.Position = 0x0124;

                    saveFileEditor.inventoryBox.Items.Clear();

                    for (int i = 0; i < ItemsHR.Length; i++)
                    {
                        saveFileEditor.inventoryBox.Items.Add(ItemsHR[i]);
                        if (reader.ReadByte() == 0x01) { saveFileEditor.inventoryBox.SetItemChecked(i, true); }
                    }

                }
                else
                {
                    filepath = null;
                    filebytes = null;
                MessageBox.Show("That is not a Club Penguin DS save file!", "File magic not recognised", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public void SaveToFile(){  //========================== WRITING TO A NEW FILE FOLLOWS ==========================

            if (filepath == null)
                {
                MessageBox.Show("You need to open a valid save file first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
                }

            Byte[] checksumArea;
            uint checksum;

            if (game == Game.EPF)   //============ ELITE PENGUIN FORCE ============
            {
                int endOfDownloadArc = 0;

                if (embeddedArc != null)
                {
                    //WRITE DOWNLOADABLE MISSION    (maybe you should also clear a space for it beforehand? in case the old mission was bigger)

                    Array.Copy(embeddedArc.filebytes, 0, filebytes, 0x100, embeddedArc.filebytes.Length);
                }


                //WRITE INVENTORY

                for (int i = 0; i < saveFileEditor.inventoryBox.Items.Count; i++)
                {
                    if (saveFileEditor.inventoryBox.GetItemChecked(i))
                    {
                        filebytes[0xF650 + i] = 01;
                    }
                    else
                    {
                        filebytes[0xF650 + i] = 00;
                    }
                }

                //WRITE LIFETIME COINS
                WriteU32ToArray(filebytes, 0xF704, (uint)saveFileEditor.lifetimeCoins.Value); //lifetime coins

                //WRITE MINIGAME HIGH SCORES
                WriteU32ToArray(filebytes,0xF708,(uint)saveFileEditor.highScore1.Value); //snowboarding
                WriteU32ToArray(filebytes, 0xF710, (uint)saveFileEditor.highScore2.Value); //cart surfer
                WriteU32ToArray(filebytes, 0xF70C, (uint)saveFileEditor.highScore3.Value); //ice fishing
                WriteU32ToArray(filebytes, 0xF714, (uint)saveFileEditor.highScore5.Value); //dance challenge
                WriteU32ToArray(filebytes, 0xF718, (uint)saveFileEditor.highScore4.Value); //jet pack adventure
                WriteU32ToArray(filebytes, 0xF71C, (uint)saveFileEditor.highScore6.Value); //snow trekker

                //ERASE OLD PENGUIN NAME (otherwise, if the new name is shorter, it won't overwrite the entire length old name)
                for (int i = 0; i < oldPenguinName.Length * 2; i++)
                {
                    filebytes[0xF720 + i] = 0x00;
                }

                //WRITE NEW PENGUIN NAME
                WriteU16StringToArray(filebytes, 0xF720, saveFileEditor.penguinNameTextBox.Text);
                oldPenguinName = saveFileEditor.penguinNameTextBox.Text;

                //ERASE OLD ONLINE NAMES (otherwise, if the new names are shorter, it won't overwrite the entire lengths of the old names)
                for (int i = 0; i < oldOnlineName1.Length * 2; i++)
                    {
                    filebytes[0xF73A + i] = 0x00;
                    }
                for (int i = 0; i < oldOnlineName2.Length * 2; i++)
                    {
                    filebytes[0xF754 + i] = 0x00;
                    }
                for (int i = 0; i < oldOnlineName3.Length * 2; i++)
                    {
                    filebytes[0xF76E + i] = 0x00;
                    }

                //WRITE NEW ONLINE NAMES
                WriteU16StringToArray(filebytes, 0xF73A, saveFileEditor.onlineName1.Text);
                oldOnlineName1 = saveFileEditor.onlineName1.Text;
                WriteU16StringToArray(filebytes, 0xF754, saveFileEditor.onlineName2.Text);
                oldOnlineName2 = saveFileEditor.onlineName1.Text;
                WriteU16StringToArray(filebytes, 0xF76E, saveFileEditor.onlineName3.Text);
                oldOnlineName3 = saveFileEditor.onlineName1.Text;

                //WRITE NEW COINS
                WriteU32ToArray(filebytes, 0xF7E4, (uint)saveFileEditor.coinsChooser.Value);

                //WRITE CURRENT MISSION
                filebytes[0xF7EC] = (byte)saveFileEditor.currentMissionChooser.SelectedIndex;

                //WRITE NEW COLOUR
                filebytes[0xF7F0] = (byte)saveFileEditor.colourChooser.SelectedIndex;

                //WRITE PUFFLES

                int puffles = 0x00;

                if (saveFileEditor.puffleBouncer.Checked)
                { puffles = puffles | 0x01; }

                if (saveFileEditor.puffleBlast.Checked)
                { puffles = puffles | 0x02; }

                if (saveFileEditor.puffleFlare.Checked)
                { puffles = puffles | 0x04; }

                if (saveFileEditor.pufflePop.Checked)
                { puffles = puffles | 0x08; }

                if (saveFileEditor.puffleLoop.Checked)
                { puffles = puffles | 0x10; }

                if (saveFileEditor.puffleFlit.Checked)
                { puffles = puffles | 0x20; }

                if (saveFileEditor.puffleChirp.Checked)
                { puffles = puffles | 0x40; }

                if (saveFileEditor.puffleChill.Checked)
                { puffles = puffles | 0x80; }

                filebytes[0xF7FB] = (byte)(puffles & 0xFF);

                //WRITE UNLOCKABLES (map, puffle whistle, etc)

                int unlocks = 0x00;

                if (saveFileEditor.mapUnlockable.Checked)
                    {unlocks = unlocks | 0x01;}

                if (saveFileEditor.HQteleportUnlockable.Checked)
                { unlocks = unlocks | 0x02; }

                if (saveFileEditor.inventoryUnlockable.Checked)
                { unlocks = unlocks | 0x04; }

                if (saveFileEditor.whistleUnlockable.Checked)
                { unlocks = unlocks | 0x08; }

                filebytes[0xF7FC] = (byte)(unlocks & 0xFF);


                if (embeddedArc != null)
                    {
                    //DOWNLOAD.ARC CHECKSUM CALCULATION
                    //redoing the checksum here just in case

                    endOfDownloadArc = (0x100 + embeddedArc.filebytes.Length) - 4; //minus 4, because the length includes the checksum, but that's what we want to write to

                    while (endOfDownloadArc % 4 != 0) //pad to multiple of 4
                    {
                        filebytes[endOfDownloadArc] = 0;
                        endOfDownloadArc++;
                    }

                    checksumArea = new Byte[endOfDownloadArc - 0x100];

                    Array.Copy(filebytes, 0x100, checksumArea, 0x0, endOfDownloadArc - 0x100);

                    checksum = Crc32.Compute(checksumArea);
                    WriteU32ToArray(filebytes, endOfDownloadArc, checksum);
                    }

                //CHECKSUM CALCULATION EPF

                checksumArea = new Byte[0xF700];

                Array.Copy(filebytes, 0x100, checksumArea, 0x0, 0xF700);

                checksum = Crc32.Compute(checksumArea);
                WriteU32ToArray(filebytes, 0x0C, checksum);
            }
        else if (game == Game.HR)   //============ HERBERT'S REVENGE ============
            {
                //WRITE NEW COINS
                WriteU32ToArray(filebytes,0x28,(uint)saveFileEditor.coinsChooser.Value);

                //ERASE OLD PENGUIN NAME (otherwise, if the new name is shorter, it won't overwrite the entire length old name)
                for (int i = 0; i < oldPenguinName.Length * 2; i++)
                {
                    filebytes[0x58 + i] = 0x00;
                }

                //WRITE NEW PENGUIN NAME
                WriteU16StringToArray(filebytes, 0x58, saveFileEditor.penguinNameTextBox.Text);
                oldPenguinName = saveFileEditor.penguinNameTextBox.Text;


                //WRITE NEW COLOUR
                filebytes[0x24] = (byte)saveFileEditor.colourChooser.SelectedIndex;

                //WRITE NEW MISSION
                WriteU16ToArray(filebytes, 0xA6, GetNewHRMissionValue(saveFileEditor.currentMissionChooser.SelectedIndex));

                //WRITE INVENTORY

                for (int i = 0; i < saveFileEditor.inventoryBox.Items.Count; i++)
                {
                    if (saveFileEditor.inventoryBox.GetItemChecked(i))
                    {
                        filebytes[0x0124 + i] = 01;
                    }
                    else
                    {
                        filebytes[0x0124 + i] = 00;
                    }
                }



                //CHECKSUM CALCULATION HR

                checksumArea = new Byte[0x130];
                
                Array.Copy(filebytes,0x20,checksumArea,0x0,0x130);

                checksum = Crc32.Compute(checksumArea);
                WriteU32ToArray(filebytes,0x08,checksum);
            }

            File.WriteAllBytes(filepath, filebytes);
        }


        public void WriteU16ToArray(Byte[] array, int offset, ushort input) {

            array[offset] = (byte)(input & 0x00FF);
            array[offset + 1] = (byte)((input & 0xFF00) >> 8);
        }

        public void WriteU32ToArray(Byte[] array, int offset, uint input)
        {
            array[offset] = (byte)(input & 0x000000FF);
            array[offset + 1] = (byte)((input & 0x0000FF00) >> 8);
            array[offset + 2] = (byte)((input & 0x00FF0000) >> 16);
            array[offset + 3] = (byte)((input & 0xFF000000) >> 24);
        }

        public void WriteU16StringToArray(Byte[] array, int offset, string input)
        {
        for (int i = 0; i < input.Length; i++)
            {
            array[offset + (i * 2)] = (byte)input[i];
            array[offset + (i * 2) + 1] = 0x00;
            }
        array[offset + (input.Length * 2)] = 0x00;
        array[offset + (input.Length * 2) + 1] = 0x00;
        }

        public void GetDownloadableMissionName() {

            archivedfile downloadstrings = embeddedArc.GetFileByName("/strings/downloadstrings.st");

            if (downloadstrings == null)
                {
                Console.WriteLine("downloadstrings was not found in that arc!");
                }
            else
                {
                downloadstrings.ReadFile();
                saveFileEditor.downloadableMissionNameDisplay.Text = "Downloadable Mission: " + downloadstrings.STstrings[0];
                }
        }
    }
}
