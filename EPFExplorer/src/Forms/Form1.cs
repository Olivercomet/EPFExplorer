﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EPFExplorer
{
    public partial class Form1 : Form
    {
        public arcfile activeArc;
        public binfile activeBin;
        public rdtfile activeRdt;

        public Mode mode = Mode.None;

        Dictionary<string, TreeNode> foldersProcessed = new Dictionary<string, TreeNode>();
        public Dictionary<TreeNode, archivedfile> treeNodesAndArchivedFiles = new Dictionary<TreeNode, archivedfile>();

        public List<string> extensions = new List<string>();

        public string startFilename;

        public List<Room> rooms = new List<Room>();

        public int alphaColorIndexForGifImport = 0;

        public enum Mode {
            None = 0x00,
            Arc = 0x01,
            Rdt = 0x02,
            Bin = 0x03
        }

        public Form1()
        {
            InitializeComponent();

            FileTree.NodeMouseClick += (sender, args) => FileTree.SelectedNode = args.Node;
            FileTree.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.FileTree_NodeMouseClick);
            FileTree.AfterLabelEdit += (sender, args) => FileTree.SelectedNode = args.Node;
            FileTree.AfterLabelEdit += new NodeLabelEditEventHandler(this.FileTree_AfterLabelEdit);

            ImageList myImageList = new ImageList();
            myImageList.ColorDepth = ColorDepth.Depth32Bit;
            myImageList.Images.Add(Properties.Resources.foldericon);
            myImageList.Images.Add(Properties.Resources.fileicon);
            FileTree.ImageList = myImageList;

            stringsEPF.Add("/WifiLogos.rdt");
            stringsEPF.Add("/BlackList.txt");

            extensions.Add(".txt");
            extensions.Add(".st");
            extensions.Add(".char");
            extensions.Add(".raw");
            extensions.Add(".map");
            extensions.Add(".st");
            extensions.Add(".st2");
            extensions.Add(".luc");
            extensions.Add(".lua.luc.l");
            extensions.Add(".lua.luc.lz");
            extensions.Add(".lua.luc.lzc");
            extensions.Add(".lzc");
            extensions.Add(".lz");
            extensions.Add(".l");
            extensions.Add(".luc.l");
            extensions.Add(".luc.l");
            extensions.Add(".luc.lz");
            extensions.Add(".bbx.lzc");
            extensions.Add(".nsbmd");
            extensions.Add(".gsc");
            extensions.Add(".gsc.lzc");
            extensions.Add(".plb");
            extensions.Add(".tsb");
            extensions.Add(".tsb.l");
            extensions.Add(".tsb.lzc");
            extensions.Add(".nsbca");
            extensions.Add(".nsbtp");
            extensions.Add(".nsbta");
            extensions.Add(".nsbma");
            extensions.Add(".nsbva");
            extensions.Add(".ntfp");
            extensions.Add(".ntft");
            extensions.Add(".nbfp");
            extensions.Add(".nbfc");
            extensions.Add(".nbfs");
            extensions.Add(".bbx");
            extensions.Add(".srl");
            extensions.Add(".vsm");
            extensions.Add(".dat");
            extensions.Add(".mpb");
            extensions.Add(".tsb");
            extensions.Add(".fnt");
            extensions.Add(".nclr");
            extensions.Add(".ncgr");
            extensions.Add(".nbgr");
            extensions.Add(".nscr");
            extensions.Add(".ncer");
            extensions.Add(".nanr");
            extensions.Add(".nftr");
            extensions.Add(".ncl");
            extensions.Add(".nsc");
            extensions.Add(".ncl.l");
            extensions.Add(".nsc.l");

            rooms.Add(new Room("Beach", "BEACH0", 60, 149));
            rooms.Add(new Room("Beacon", "BEACON0", 83, 106));
            rooms.Add(new Room("Boiler Room", "BOILERROOM0", 93, 179));
            rooms.Add(new Room("Book Room", "BOOKROOM0", 107, 150));
            rooms.Add(new Room("Coffee Shop", "COFFEESHOP0", 61, 150));
            rooms.Add(new Room("Command Room", "COMMANDROOM0", 62, 100));
            rooms.Add(new Room("Dock", "DOCK0", 63, 176));
            rooms.Add(new Room("Dojo", "DOJO0", 64, 76));
            rooms.Add(new Room("Fishing Pond", "FISHING0", 108, 82));
            rooms.Add(new Room("Forest", "FOREST0", 102, 150));
            rooms.Add(new Room("Gadget Room", "GADGETROOM0", 65, 174));
            rooms.Add(new Room("Gary's Room", "GARYSROOM0", 66, 176));
            rooms.Add(new Room("Gift Shop", "GIFTSHOP0", 98, 185));
            rooms.Add(new Room("Gift Shop Office", "GIFTOFFICE0", 99, 179));
            rooms.Add(new Room("Gift Shop Roof", "GIFTROOF0", 100, 41));
            rooms.Add(new Room("HQ", "HEADQUARTERS0", 67, 199));
            rooms.Add(new Room("Ice Rink", "ICERINK0", 91, 150));
            rooms.Add(new Room("Iceberg", "ICEBERG0", 103, 166));
            rooms.Add(new Room("Lighthouse", "LIGHTHOUSE0", 82, 151));
            rooms.Add(new Room("Lodge Attic", "ATTIC0", 109, 144));
            rooms.Add(new Room("Lounge", "LOUNGE0", 106, 150));
            rooms.Add(new Room("Mine", "MINEINTERIOR0", 69, 101));
            rooms.Add(new Room("Mine Shack", "MINESHACK0", 68, 62));
            rooms.Add(new Room("Mine Shed", "MINESHED0", 72, 74));
            rooms.Add(new Room("Mine Crash Site", "MINECRASH0", 70, 135));
            rooms.Add(new Room("Mine Lair", "MINELAIR0", 71, 88));
            rooms.Add(new Room("Night Club", "NIGHTCLUB0", 94, 190));
            rooms.Add(new Room("Pet Shop", "PETSHOP0", 74, 142));
            rooms.Add(new Room("Pizza Parlor", "PIZZAPARLOR0", 75, 155));
            rooms.Add(new Room("Plaza", "PLAZA0", 76, 100));
            rooms.Add(new Room("Puffle Training Room", "PUFFLETRAINING0", 77, 72));
            rooms.Add(new Room("Ski Hill", "SKIHILL0", 73, 73));
            rooms.Add(new Room("Ski Lodge", "LODGE0", 90, 176));
            rooms.Add(new Room("Ski Village", "SKIVILLAGE0", 78, 189));
            rooms.Add(new Room("Snow Forts", "SNOWFORTS0", 79, 91));
            rooms.Add(new Room("Sport Shop", "SPORTSHOP0", 80, 197));
            rooms.Add(new Room("Stage", "STAGE0", 95, 162));
            rooms.Add(new Room("Tallest Mountain", "TALLESTMOUNTAINTOP0", 96, 95));
            rooms.Add(new Room("Town", "TOWN0", 81, 99));
            rooms.Add(new Room("Underground Pool", "POOL0", 92, 84));
        }

        public class Room
        {
            public string DisplayName;
            public string InternalName;
            public int ID_for_objects;
            public List<MissionEditor.DownloadItem> Objects = new List<MissionEditor.DownloadItem>();

            public int tilemapWidth = 0;
            public Room(string _DisplayName, string _InternalName, int _ID_for_objects, int _tilemapWidth)
            {
                DisplayName = _DisplayName;
                InternalName = _InternalName;
                ID_for_objects = _ID_for_objects;
                tilemapWidth = _tilemapWidth;
            }
        }

        private void Toolstrip_Open_Click(object sender, EventArgs e)
        {
            Open();
        }

        //this table, used for hashing, starts at 0x021C4680 in the ram of EPF.
        uint[] table_for_hashing = new uint[] { 0, 1996959894, 3993919788, 2567524794, 124634137, 1886057615, 3915621685, 2657392035, 249268274, 2044508324, 3772115230, 2547177864, 162941995, 2125561021, 3887607047, 2428444049, 498536548, 1789927666, 4089016648, 2227061214, 450548861, 1843258603, 4107580753, 2211677639, 325883990, 1684777152, 4251122042, 2321926636, 335633487, 1661365465, 4195302755, 2366115317, 997073096, 1281953886, 3579855332, 2724688242, 1006888145, 1258607687, 3524101629, 2768942443, 901097722, 1119000684, 3686517206, 2898065728, 853044451, 1172266101, 3705015759, 2882616665, 651767980, 1373503546, 3369554304, 3218104598, 565507253, 1454621731, 3485111705, 3099436303, 671266974, 1594198024, 3322730930, 2970347812, 795835527, 1483230225, 3244367275, 3060149565, 1994146192, 31158534, 2563907772, 4023717930, 1907459465, 112637215, 2680153253, 3904427059, 2013776290, 251722036, 2517215374, 3775830040, 2137656763, 141376813, 2439277719, 3865271297, 1802195444, 476864866, 2238001368, 4066508878, 1812370925, 453092731, 2181625025, 4111451223, 1706088902, 314042704, 2344532202, 4240017532, 1658658271, 366619977, 2362670323, 4224994405, 1303535960, 984961486, 2747007092, 3569037538, 1256170817, 1037604311, 2765210733, 3554079995, 1131014506, 879679996, 2909243462, 3663771856, 1141124467, 855842277, 2852801631, 3708648649, 1342533948, 654459306, 3188396048, 3373015174, 1466479909, 544179635, 3110523913, 3462522015, 1591671054, 702138776, 2966460450, 3352799412, 1504918807, 783551873, 3082640443, 3233442989, 3988292384, 2596254646, 62317068, 1957810842, 3939845945, 2647816111, 81470997, 1943803523, 3814918930, 2489596804, 225274430, 2053790376, 3826175755, 2466906013, 167816743, 2097651377, 4027552580, 2265490386, 503444072, 1762050814, 4150417245, 2154129355, 426522225, 1852507879, 4275313526, 2312317920, 282753626, 1742555852, 4189708143, 2394877945, 397917763, 1622183637, 3604390888, 2714866558, 953729732, 1340076626, 3518719985, 2797360999, 1068828381, 1219638859, 3624741850, 2936675148, 906185462, 1090812512, 3747672003, 2825379669, 829329135, 1181335161, 3412177804, 3160834842, 628085408, 1382605366, 3423369109, 3138078467, 570562233, 1426400815, 3317316542, 2998733608, 733239954, 1555261956, 3268935591, 3050360625, 752459403, 1541320221, 2607071920, 3965973030, 1969922972, 40735498, 2617837225, 3943577151, 1913087877, 83908371, 2512341634, 3803740692, 2075208622, 213261112, 2463272603, 3855990285, 2094854071, 198958881, 2262029012, 4057260610, 1759359992, 534414190, 2176718541, 4139329115, 1873836001, 414664567, 2282248934, 4279200368, 1711684554, 285281116, 2405801727, 4167216745, 1634467795, 376229701, 2685067896, 3608007406, 1308918612, 956543938, 2808555105, 3495958263, 1231636301, 1047427035, 2932959818, 3654703836, 1088359270, 936918000, 2847714899, 3736837829, 1202900863, 817233897, 3183342108, 3401237130, 1404277552, 615818150, 3134207493, 3453421203, 1423857449, 601450431, 3009837614, 3294710456, 1567103746, 711928724, 3020668471, 3272380065, 1510334235, 755167117 };


        public List<string> stringsEPF = new List<string>() { "/3d/SuperRobot/SuperRobotBeach_jetpackFlame", "/3d/SuperRobot/SuperRobotDock_jetpackFlame", "/3d/SuperRobot/SuperRobotTown_jetpackFlame", "/3d/SuperRobot/SuperRobotTall_jetpackFlame", "/3d/SuperRobot/SuperRobotBeacon_jetpackFlame", "/3d/SuperRobot/SuperRobotRoof_jetpackFlame", "/3d/SuperRobot/SuperRobotSki_jetpackFlame", "/3d/SuperRobot/SuperRobotTall2_jetpackFlame", "/3d/SuperRobot/SuperRobotTall3_jetpackFlame", "/3d/SuperRobot/SuperRobotTall4_jetpackFlame", "/3d/SuperRobot/SuperRobotPufflesTall_jetpackFlame", "/3d/SuperRobot/SuperRobotPufflesTall2_jetpackFlame", "/3d/SuperRobot/SuperRobotPufflesTall3_jetpackFlame", "/3d/SuperRobot/SuperRobotPufflesTall4_jetpackFlame", "/NPC/Puffles/Black/BlackPuffle", "/NPC/Puffles/Black/BlackPuffle_INTRO", "/NPC/Puffles/Black/BlackPuffle_appear", "/NPC/Puffles/Black/BlackPuffle_idle", "/NPC/Puffles/Black/BlackPuffle_IDLE", "/NPC/Puffles/Black/BlackPuffle_IDLE_TO", "/NPC/Puffles/Black/BlackPuffle_TO_WALKING", "/NPC/Puffles/Black/BlackPuffle_walkLeft", "/NPC/Puffles/Black/BlackPuffle_walkRight", "/NPC/Puffles/Black/BlackPuffle_WALKING_TO", "/NPC/Puffles/Black/BlackPuffle_walkTO", "/NPC/Puffles/Black/BlackPuffle_walk", "/NPC/Puffles/Black/BlackPuffle_TURN", "/NPC/Puffles/Black/BlackPuffle_IDLE2READY", "/NPC/Puffles/Black/BlackPuffle_idle2ready", "/NPC/Puffles/Black/BlackPuffle_READY", "/NPC/Puffles/Black/BlackPuffle_ready", "/NPC/Puffles/Black/BlackPuffle_READY_TO", "/NPC/Puffles/Black/BlackPuffle_readyTO", "/NPC/Puffles/Black/BlackPuffle_STANDBY", "/NPC/Puffles/Black/BlackPuffle_standyby", "/NPC/Puffles/Black/BlackPuffle_SHOOT", "/NPC/Puffles/Black/BlackPuffle_READY2IDLE", "/NPC/Puffles/Black/BlackPuffle_ready2idle", "/NPC/Puffles/Black/BlackPuffle_JUMP", "/NPC/Puffles/Black/BlackPuffle_jumpExcited", "/NPC/Puffles/Black/BlackPuffle_sad", "/NPC/Puffles/Black/BlackPuffle_ready", "/NPC/Puffles/Black/BlackPuffle_shoot", "/NPC/Puffles/Black/BlackPuffle_smile", "/NPC/Puffles/Black/BlackPuffle_hit", "/NPC/Puffles/Black/BlackPuffle_fail", "/NPC/Puffles/Black/BlackPuffle_FAIL", "/NPC/Puffles/Black/BlackPuffle_noticeSomething", "/NPC/Puffles/Red/RedPuffle", "/NPC/Puffles/Red/RedPuffle_INTRO", "/NPC/Puffles/Red/RedPuffle_appear", "/NPC/Puffles/Red/RedPuffle_idle", "/NPC/Puffles/Red/RedPuffle_IDLE", "/NPC/Puffles/Red/RedPuffle_IDLE_TO", "/NPC/Puffles/Red/RedPuffle_TO_WALKING", "/NPC/Puffles/Red/RedPuffle_walkLeft", "/NPC/Puffles/Red/RedPuffle_walkRight", "/NPC/Puffles/Red/RedPuffle_WALKING_TO", "/NPC/Puffles/Red/RedPuffle_walkTO", "/NPC/Puffles/Red/RedPuffle_walk", "/NPC/Puffles/Red/RedPuffle_TURN", "/NPC/Puffles/Red/RedPuffle_IDLE2READY", "/NPC/Puffles/Red/RedPuffle_idle2ready", "/NPC/Puffles/Red/RedPuffle_READY", "/NPC/Puffles/Red/RedPuffle_ready", "/NPC/Puffles/Red/RedPuffle_READY_TO", "/NPC/Puffles/Red/RedPuffle_readyTO", "/NPC/Puffles/Red/RedPuffle_STANDBY", "/NPC/Puffles/Red/RedPuffle_standyby", "/NPC/Puffles/Red/RedPuffle_SHOOT", "/NPC/Puffles/Red/RedPuffle_READY2IDLE", "/NPC/Puffles/Red/RedPuffle_ready2idle", "/NPC/Puffles/Red/RedPuffle_JUMP", "/NPC/Puffles/Red/RedPuffle_jumpExcited", "/NPC/Puffles/Red/RedPuffle_sad", "/NPC/Puffles/Red/RedPuffle_ready", "/NPC/Puffles/Red/RedPuffle_shoot", "/NPC/Puffles/Red/RedPuffle_smile", "/NPC/Puffles/Red/RedPuffle_hit", "/NPC/Puffles/Red/RedPuffle_fail", "/NPC/Puffles/Red/RedPuffle_FAIL", "/NPC/Puffles/Red/RedPuffle_noticeSomething", "/NPC/Puffles/Blue/BluePuffle", "/NPC/Puffles/Blue/BluePuffle_INTRO", "/NPC/Puffles/Blue/BluePuffle_appear", "/NPC/Puffles/Blue/BluePuffle_idle", "/NPC/Puffles/Blue/BluePuffle_IDLE", "/NPC/Puffles/Blue/BluePuffle_IDLE_TO", "/NPC/Puffles/Blue/BluePuffle_TO_WALKING", "/NPC/Puffles/Blue/BluePuffle_walkLeft", "/NPC/Puffles/Blue/BluePuffle_walkRight", "/NPC/Puffles/Blue/BluePuffle_WALKING_TO", "/NPC/Puffles/Blue/BluePuffle_walkTO", "/NPC/Puffles/Blue/BluePuffle_walk", "/NPC/Puffles/Blue/BluePuffle_TURN", "/NPC/Puffles/Blue/BluePuffle_IDLE2READY", "/NPC/Puffles/Blue/BluePuffle_idle2ready", "/NPC/Puffles/Blue/BluePuffle_READY", "/NPC/Puffles/Blue/BluePuffle_ready", "/NPC/Puffles/Blue/BluePuffle_READY_TO", "/NPC/Puffles/Blue/BluePuffle_readyTO", "/NPC/Puffles/Blue/BluePuffle_STANDBY", "/NPC/Puffles/Blue/BluePuffle_standyby", "/NPC/Puffles/Blue/BluePuffle_SHOOT", "/NPC/Puffles/Blue/BluePuffle_READY2IDLE", "/NPC/Puffles/Blue/BluePuffle_ready2idle", "/NPC/Puffles/Blue/BluePuffle_JUMP", "/NPC/Puffles/Blue/BluePuffle_jumpExcited", "/NPC/Puffles/Blue/BluePuffle_sad", "/NPC/Puffles/Blue/BluePuffle_ready", "/NPC/Puffles/Blue/BluePuffle_shoot", "/NPC/Puffles/Blue/BluePuffle_smile", "/NPC/Puffles/Blue/BluePuffle_hit", "/NPC/Puffles/Blue/BluePuffle_fail", "/NPC/Puffles/Blue/BluePuffle_FAIL", "/NPC/Puffles/Blue/BluePuffle_noticeSomething", "/NPC/Puffles/Pink/PinkPuffle", "/NPC/Puffles/Pink/PinkPuffle_INTRO", "/NPC/Puffles/Pink/PinkPuffle_appear", "/NPC/Puffles/Pink/PinkPuffle_idle", "/NPC/Puffles/Pink/PinkPuffle_IDLE", "/NPC/Puffles/Pink/PinkPuffle_IDLE_TO", "/NPC/Puffles/Pink/PinkPuffle_TO_WALKING", "/NPC/Puffles/Pink/PinkPuffle_walkLeft", "/NPC/Puffles/Pink/PinkPuffle_walkRight", "/NPC/Puffles/Pink/PinkPuffle_WALKING_TO", "/NPC/Puffles/Pink/PinkPuffle_walkTO", "/NPC/Puffles/Pink/PinkPuffle_walk", "/NPC/Puffles/Pink/PinkPuffle_TURN", "/NPC/Puffles/Pink/PinkPuffle_IDLE2READY", "/NPC/Puffles/Pink/PinkPuffle_idle2ready", "/NPC/Puffles/Pink/PinkPuffle_READY", "/NPC/Puffles/Pink/PinkPuffle_ready", "/NPC/Puffles/Pink/PinkPuffle_READY_TO", "/NPC/Puffles/Pink/PinkPuffle_readyTO", "/NPC/Puffles/Pink/PinkPuffle_STANDBY", "/NPC/Puffles/Pink/PinkPuffle_standyby", "/NPC/Puffles/Pink/PinkPuffle_SHOOT", "/NPC/Puffles/Pink/PinkPuffle_READY2IDLE", "/NPC/Puffles/Pink/PinkPuffle_ready2idle", "/NPC/Puffles/Pink/PinkPuffle_JUMP", "/NPC/Puffles/Pink/PinkPuffle_jumpExcited", "/NPC/Puffles/Pink/PinkPuffle_sad", "/NPC/Puffles/Pink/PinkPuffle_ready", "/NPC/Puffles/Pink/PinkPuffle_shoot", "/NPC/Puffles/Pink/PinkPuffle_smile", "/NPC/Puffles/Pink/PinkPuffle_hit", "/NPC/Puffles/Pink/PinkPuffle_fail", "/NPC/Puffles/Pink/PinkPuffle_FAIL", "/NPC/Puffles/Pink/PinkPuffle_noticeSomething", "/NPC/Puffles/Green/GreenPuffle", "/NPC/Puffles/Green/GreenPuffle_INTRO", "/NPC/Puffles/Green/GreenPuffle_appear", "/NPC/Puffles/Green/GreenPuffle_idle", "/NPC/Puffles/Green/GreenPuffle_IDLE", "/NPC/Puffles/Green/GreenPuffle_IDLE_TO", "/NPC/Puffles/Green/GreenPuffle_TO_WALKING", "/NPC/Puffles/Green/GreenPuffle_walkLeft", "/NPC/Puffles/Green/GreenPuffle_walkRight", "/NPC/Puffles/Green/GreenPuffle_WALKING_TO", "/NPC/Puffles/Green/GreenPuffle_walkTO", "/NPC/Puffles/Green/GreenPuffle_walk", "/NPC/Puffles/Green/GreenPuffle_TURN", "/NPC/Puffles/Green/GreenPuffle_IDLE2READY", "/NPC/Puffles/Green/GreenPuffle_idle2ready", "/NPC/Puffles/Green/GreenPuffle_READY", "/NPC/Puffles/Green/GreenPuffle_ready", "/NPC/Puffles/Green/GreenPuffle_READY_TO", "/NPC/Puffles/Green/GreenPuffle_readyTO", "/NPC/Puffles/Green/GreenPuffle_STANDBY", "/NPC/Puffles/Green/GreenPuffle_standyby", "/NPC/Puffles/Green/GreenPuffle_SHOOT", "/NPC/Puffles/Green/GreenPuffle_READY2IDLE", "/NPC/Puffles/Green/GreenPuffle_ready2idle", "/NPC/Puffles/Green/GreenPuffle_JUMP", "/NPC/Puffles/Green/GreenPuffle_jumpExcited", "/NPC/Puffles/Green/GreenPuffle_sad", "/NPC/Puffles/Green/GreenPuffle_ready", "/NPC/Puffles/Green/GreenPuffle_shoot", "/NPC/Puffles/Green/GreenPuffle_smile", "/NPC/Puffles/Green/GreenPuffle_hit", "/NPC/Puffles/Green/GreenPuffle_fail", "/NPC/Puffles/Green/GreenPuffle_FAIL", "/NPC/Puffles/Green/GreenPuffle_noticeSomething", "/NPC/Puffles/Yellow/YellowPuffle", "/NPC/Puffles/Yellow/YellowPuffle_INTRO", "/NPC/Puffles/Yellow/YellowPuffle_appear", "/NPC/Puffles/Yellow/YellowPuffle_idle", "/NPC/Puffles/Yellow/YellowPuffle_IDLE", "/NPC/Puffles/Yellow/YellowPuffle_IDLE_TO", "/NPC/Puffles/Yellow/YellowPuffle_TO_WALKING", "/NPC/Puffles/Yellow/YellowPuffle_walkLeft", "/NPC/Puffles/Yellow/YellowPuffle_walkRight", "/NPC/Puffles/Yellow/YellowPuffle_WALKING_TO", "/NPC/Puffles/Yellow/YellowPuffle_walkTO", "/NPC/Puffles/Yellow/YellowPuffle_walk", "/NPC/Puffles/Yellow/YellowPuffle_TURN", "/NPC/Puffles/Yellow/YellowPuffle_IDLE2READY", "/NPC/Puffles/Yellow/YellowPuffle_idle2ready", "/NPC/Puffles/Yellow/YellowPuffle_READY", "/NPC/Puffles/Yellow/YellowPuffle_ready", "/NPC/Puffles/Yellow/YellowPuffle_READY_TO", "/NPC/Puffles/Yellow/YellowPuffle_readyTO", "/NPC/Puffles/Yellow/YellowPuffle_STANDBY", "/NPC/Puffles/Yellow/YellowPuffle_standyby", "/NPC/Puffles/Yellow/YellowPuffle_SHOOT", "/NPC/Puffles/Yellow/YellowPuffle_READY2IDLE", "/NPC/Puffles/Yellow/YellowPuffle_ready2idle", "/NPC/Puffles/Yellow/YellowPuffle_JUMP", "/NPC/Puffles/Yellow/YellowPuffle_jumpExcited", "/NPC/Puffles/Yellow/YellowPuffle_sad", "/NPC/Puffles/Yellow/YellowPuffle_ready", "/NPC/Puffles/Yellow/YellowPuffle_shoot", "/NPC/Puffles/Yellow/YellowPuffle_smile", "/NPC/Puffles/Yellow/YellowPuffle_hit", "/NPC/Puffles/Yellow/YellowPuffle_fail", "/NPC/Puffles/Yellow/YellowPuffle_FAIL", "/NPC/Puffles/Yellow/YellowPuffle_noticeSomething", "/NPC/Puffles/Purple/PurplePuffle", "/NPC/Puffles/Purple/PurplePuffle_INTRO", "/NPC/Puffles/Purple/PurplePuffle_appear", "/NPC/Puffles/Purple/PurplePuffle_idle", "/NPC/Puffles/Purple/PurplePuffle_IDLE", "/NPC/Puffles/Purple/PurplePuffle_IDLE_TO", "/NPC/Puffles/Purple/PurplePuffle_TO_WALKING", "/NPC/Puffles/Purple/PurplePuffle_walkLeft", "/NPC/Puffles/Purple/PurplePuffle_walkRight", "/NPC/Puffles/Purple/PurplePuffle_WALKING_TO", "/NPC/Puffles/Purple/PurplePuffle_walkTO", "/NPC/Puffles/Purple/PurplePuffle_walk", "/NPC/Puffles/Purple/PurplePuffle_TURN", "/NPC/Puffles/Purple/PurplePuffle_IDLE2READY", "/NPC/Puffles/Purple/PurplePuffle_idle2ready", "/NPC/Puffles/Purple/PurplePuffle_READY", "/NPC/Puffles/Purple/PurplePuffle_ready", "/NPC/Puffles/Purple/PurplePuffle_READY_TO", "/NPC/Puffles/Purple/PurplePuffle_readyTO", "/NPC/Puffles/Purple/PurplePuffle_STANDBY", "/NPC/Puffles/Purple/PurplePuffle_standyby", "/NPC/Puffles/Purple/PurplePuffle_SHOOT", "/NPC/Puffles/Purple/PurplePuffle_READY2IDLE", "/NPC/Puffles/Purple/PurplePuffle_ready2idle", "/NPC/Puffles/Purple/PurplePuffle_JUMP", "/NPC/Puffles/Purple/PurplePuffle_jumpExcited", "/NPC/Puffles/Purple/PurplePuffle_sad", "/NPC/Puffles/Purple/PurplePuffle_ready", "/NPC/Puffles/Purple/PurplePuffle_shoot", "/NPC/Puffles/Purple/PurplePuffle_smile", "/NPC/Puffles/Purple/PurplePuffle_hit", "/NPC/Puffles/Purple/PurplePuffle_fail", "/NPC/Puffles/Purple/PurplePuffle_FAIL", "/NPC/Puffles/Purple/PurplePuffle_noticeSomething", "/NPC/Puffles/White/WhitePuffle", "/NPC/Puffles/White/WhitePuffle_INTRO", "/NPC/Puffles/White/WhitePuffle_appear", "/NPC/Puffles/White/WhitePuffle_idle", "/NPC/Puffles/White/WhitePuffle_IDLE", "/NPC/Puffles/White/WhitePuffle_IDLE_TO", "/NPC/Puffles/White/WhitePuffle_TO_WALKING", "/NPC/Puffles/White/WhitePuffle_walkLeft", "/NPC/Puffles/White/WhitePuffle_walkRight", "/NPC/Puffles/White/WhitePuffle_WALKING_TO", "/NPC/Puffles/White/WhitePuffle_walkTO", "/NPC/Puffles/White/WhitePuffle_walk", "/NPC/Puffles/White/WhitePuffle_TURN", "/NPC/Puffles/White/WhitePuffle_IDLE2READY", "/NPC/Puffles/White/WhitePuffle_idle2ready", "/NPC/Puffles/White/WhitePuffle_READY", "/NPC/Puffles/White/WhitePuffle_ready", "/NPC/Puffles/White/WhitePuffle_READY_TO", "/NPC/Puffles/White/WhitePuffle_readyTO", "/NPC/Puffles/White/WhitePuffle_STANDBY", "/NPC/Puffles/White/WhitePuffle_standyby", "/NPC/Puffles/White/WhitePuffle_SHOOT", "/NPC/Puffles/White/WhitePuffle_READY2IDLE", "/NPC/Puffles/White/WhitePuffle_ready2idle", "/NPC/Puffles/White/WhitePuffle_JUMP", "/NPC/Puffles/White/WhitePuffle_jumpExcited", "/NPC/Puffles/White/WhitePuffle_sad", "/NPC/Puffles/White/WhitePuffle_ready", "/NPC/Puffles/White/WhitePuffle_shoot", "/NPC/Puffles/White/WhitePuffle_smile", "/NPC/Puffles/White/WhitePuffle_hit", "/NPC/Puffles/White/WhitePuffle_fail", "/NPC/Puffles/White/WhitePuffle_FAIL", "/NPC/Puffles/White/WhitePuffle_noticeSomething", "/Minigames/JetPack/NPC/JetPackGuy_Flying/flier_idle", "/palettes/aqua.nbfp", "/palettes/red.nbfp", "/palettes/black.nbfp", "/palettes/blue.nbfp", "/palettes/brown.nbfp", "/palettes/darkGreen.nbfp", "/palettes/fuschia.nbfp", "/palettes/green.nbfp", "/palettes/lime.nbfp", "/palettes/orange.nbfp", "/palettes/peach.nbfp", "/palettes/purple.nbfp", "/palettes/yellow.nbfp", "/palettes/fishing_aqua.nbfp", "/palettes/fishing_red.nbfp", "/palettes/fishing_black.nbfp", "/palettes/fishing_blue.nbfp", "/palettes/fishing_brown.nbfp", "/palettes/fishing_darkGreen.nbfp", "/palettes/fishing_fuschia.nbfp", "/palettes/fishing_green.nbfp", "/palettes/fishing_lime.nbfp", "/palettes/fishing_orange.nbfp", "/palettes/fishing_peach.nbfp", "/palettes/fishing_purple.nbfp", "/palettes/fishing_yellow.nbfp", "/palettes/cartsurferhard_aqua.nbfp", "/palettes/cartsurferhard_red.nbfp", "/palettes/cartsurferhard_black.nbfp", "/palettes/cartsurferhard_blue.nbfp", "/palettes/cartsurferhard_brown.nbfp", "/palettes/cartsurferhard_darkGreen.nbfp", "/palettes/cartsurferhard_fuschia.nbfp", "/palettes/cartsurferhard_green.nbfp", "/palettes/cartsurferhard_lime.nbfp", "/palettes/cartsurferhard_orange.nbfp", "/palettes/cartsurferhard_peach.nbfp", "/palettes/cartsurferhard_purple.nbfp", "/palettes/cartsurferhard_yellow.nbfp", "/palettes/cartsurfer_aqua.nbfp", "/palettes/cartsurfer_red.nbfp", "/palettes/cartsurfer_black.nbfp", "/palettes/cartsurfer_blue.nbfp", "/palettes/cartsurfer_brown.nbfp", "/palettes/cartsurfer_darkGreen.nbfp", "/palettes/cartsurfer_fuschia.nbfp", "/palettes/cartsurfer_green.nbfp", "/palettes/cartsurfer_lime.nbfp", "/palettes/cartsurfer_orange.nbfp", "/palettes/cartsurfer_peach.nbfp", "/palettes/cartsurfer_purple.nbfp", "/palettes/cartsurfer_yellow.nbfp", "/palettes/dance_aqua.nbfp", "/palettes/dance_red.nbfp", "/palettes/dance_black.nbfp", "/palettes/dance_blue.nbfp", "/palettes/dance_brown.nbfp", "/palettes/dance_darkGreen.nbfp", "/palettes/dance_fuschia.nbfp", "/palettes/dance_green.nbfp", "/palettes/dance_lime.nbfp", "/palettes/dance_orange.nbfp", "/palettes/dance_peach.nbfp", "/palettes/dance_pink.nbfp", "/palettes/dance_purple.nbfp", "/palettes/dance_yellow.nbfp", "/palettes/jetpack_aqua.nbfp", "/palettes/jetpack_red.nbfp", "/palettes/jetpack_black.nbfp", "/palettes/jetpack_blue.nbfp", "/palettes/jetpack_brown.nbfp", "/palettes/jetpack_darkGreen.nbfp", "/palettes/jetpack_fuschia.nbfp", "/palettes/jetpack_green.nbfp", "/palettes/jetpack_lime.nbfp", "/palettes/jetpack_orange.nbfp", "/palettes/jetpack_peach.nbfp", "/palettes/dance_pink.nbfp", "/palettes/jetpack_purple.nbfp", "/palettes/jetpack_yellow.nbfp", "/palettes/ProdownhillSnowboard_aqua.nbfp", "/palettes/ProdownhillSnowboard_red.nbfp", "/palettes/ProdownhillSnowboard_black.nbfp", "/palettes/ProdownhillSnowboard_blue.nbfp", "/palettes/ProdownhillSnowboard_brown.nbfp", "/palettes/ProdownhillSnowboard_darkGreen.nbfp", "/palettes/ProdownhillSnowboard_fuschia.nbfp", "/palettes/ProdownhillSnowboard_green.nbfp", "/palettes/ProdownhillSnowboard_lime.nbfp", "/palettes/ProdownhillSnowboard_orange.nbfp", "/palettes/ProdownhillSnowboard_peach.nbfp", "/palettes/ProdownhillSnowboard_pink.nbfp", "/palettes/ProdownhillSnowboard_purple.nbfp", "/palettes/ProdownhillSnowboard_yellow.nbfp", "/palettes/downhillSnowboard_aqua.nbfp", "/palettes/downhillSnowboard_red.nbfp", "/palettes/downhillSnowboard_black.nbfp", "/palettes/downhillSnowboard_blue.nbfp", "/palettes/downhillSnowboard_brown.nbfp", "/palettes/downhillSnowboard_darkGreen.nbfp", "/palettes/downhillSnowboard_fuschia.nbfp", "/palettes/downhillSnowboard_green.nbfp", "/palettes/downhillSnowboard_lime.nbfp", "/palettes/downhillSnowboard_orange.nbfp", "/palettes/downhillSnowboard_peach.nbfp", "/palettes/downhillSnowboard_pink.nbfp", "/palettes/downhillSnowboard_purple.nbfp", "/palettes/downhillSnowboard_yellow.nbfp", "/palettes/mapPuffle_blue", "/palettes/mapPuffle_red", "/palettes/mapPuffle_black", "/palettes/mapPuffle_green", "/palettes/mapPuffle_pink", "/palettes/mapPuffle_purple", "/palettes/mapPuffle_yellow", "/palettes/mapPuffle_white", "/palettes/mapPuffle_klutzy", "/3d/cat.nsbmd", "/strings/English_dialogstrings.st", "/strings/English_adhocstrings.st", "/strings/English_gamestrings.st", "/strings/English_savegamestrings.st", "/strings/English_downloadstrings.st", "/strings/English_wifistrings.st", "/strings/English_localizationStrings.st", "/strings/English_cutsceneStrings.st", "/strings/English_conversationStrings.st", "/strings/English_unlockablesStrings.st", "/strings/English_DisneyStrings.st", "/strings/English_disneyStrings.st", "/strings/English_ItemStrings.st", "/strings/Spanish_dialogstrings.st", "/strings/Spanish_adhocstrings.st", "/strings/Spanish_gamestrings.st", "/strings/Spanish_savegamestrings.st", "/strings/Spanish_downloadstrings.st", "/strings/Spanish_wifistrings.st", "/strings/Spanish_localizationStrings.st", "/strings/Spanish_cutsceneStrings.st", "/strings/Spanish_conversationStrings.st", "/strings/Spanish_unlockablesStrings.st", "/strings/Spanish_DisneyStrings.st", "/strings/Spanish_ItemStrings.st", "/strings/Italian_dialogstrings.st", "/strings/Italian_adhocstrings.st", "/strings/Italian_gamestrings.st", "/strings/Italian_savegamestrings.st", "/strings/Italian_downloadstrings.st", "/strings/Italian_wifistrings.st", "/strings/Italian_localizationStrings.st", "/strings/Italian_cutsceneStrings.st", "/strings/Italian_conversationStrings.st", "/strings/Italian_unlockablesStrings.st", "/strings/Italian_DisneyStrings.st", "/strings/Italian_ItemStrings.st", "/strings/French_dialogstrings.st", "/strings/French_adhocstrings.st", "/strings/French_gamestrings.st", "/strings/French_downloadstrings.st", "/strings/French_savegamestrings.st", "/strings/French_wifistrings.st", "/strings/French_localizationStrings.st", "/strings/French_cutsceneStrings.st", "/strings/French_conversationStrings.st", "/strings/French_unlockablesStrings.st", "/strings/French_DisneyStrings.st", "/strings/French_ItemStrings.st", "/strings/German_dialogstrings.st", "/strings/German_adhocstrings.st", "/strings/German_gamestrings.st", "/strings/German_downloadstrings.st", "/strings/German_downloadstrings.st", "/strings/German_savegamestrings.st", "/strings/German_wifistrings.st", "/strings/German_localizationStrings.st", "/strings/German_cutsceneStrings.st", "/strings/German_conversationStrings.st", "/strings/German_unlockablesStrings.st", "/strings/German_DisneyStrings.st", "/strings/German_ItemStrings.st", "/strings/Dutch_dialogstrings.st", "/strings/Dutch_adhocstrings.st", "/strings/Dutch_gamestrings.st", "/strings/Dutch_downloadstrings.st", "/strings/Dutch_savegamestrings.st", "/strings/Dutch_wifistrings.st", "/strings/Dutch_localizationStrings.st", "/strings/Dutch_cutsceneStrings.st", "/strings/Dutch_conversationStrings.st", "/strings/Dutch_unlockablesStrings.st", "/strings/Dutch_DisneyStrings.st", "/strings/Dutch_ItemStrings.st", "/strings/Danish_dialogstrings.st", "/strings/Danish_adhocstrings.st", "/strings/Danish_gamestrings.st", "/strings/Danish_downloadstrings.st", "/strings/Danish_savegamestrings.st", "/strings/Danish_wifistrings.st", "/strings/Danish_localizationStrings.st", "/strings/Danish_cutsceneStrings.st", "/strings/Danish_conversationStrings.st", "/strings/Danish_unlockablesStrings.st", "/strings/Danish_DisneyStrings.st", "/strings/Danish_ItemStrings.st", "/strings/Japanese_dialogstrings.st", "/strings/Japanese_adhocstrings.st", "/strings/Japanese_gamestrings.st", "/strings/Japanese_downloadstrings.st", "/strings/Japanese_savegamestrings.st", "/strings/Japanese_wifistrings.st", "/strings/Japanese_localizationStrings.st", "/strings/Japanese_cutsceneStrings.st", "/strings/Japanese_conversationStrings.st", "/strings/Japanese_unlockablesStrings.st", "/strings/Japanese_DisneyStrings.st", "/strings/Japanese_ItemStrings.st", "/strings/Chinese_dialogstrings.st", "/strings/Chinese_adhocstrings.st", "/strings/Chinese_gamestrings.st", "/strings/Chinese_downloadstrings.st", "/strings/Chinese_savegamestrings.st", "/strings/Chinese_wifistrings.st", "/strings/Chinese_localizationStrings.st", "/strings/Chinese_cutsceneStrings.st", "/strings/Chinese_conversationStrings.st", "/strings/Chinese_unlockablesStrings.st", "/strings/Chinese_DisneyStrings.st", "/strings/Chinese_ItemStrings.st", "/strings/Korean_dialogstrings.st", "/strings/Korean_adhocstrings.st", "/strings/Korean_gamestrings.st", "/strings/Korean_downloadstrings.st", "/strings/Korean_savegamestrings.st", "/strings/Korean_wifistrings.st", "/strings/Korean_localizationStrings.st", "/strings/Korean_cutsceneStrings.st", "/strings/Korean_conversationStrings.st", "/strings/Korean_unlockablesStrings.st", "/strings/Korean_DisneyStrings.st", "/strings/Korean_ItemStrings.st", "/strings/Hangul_dialogstrings.st", "/strings/Hangul_adhocstrings.st", "/strings/Hangul_gamestrings.st", "/strings/Hangul_downloadstrings.st", "/strings/Hangul_savegamestrings.st", "/strings/Hangul_wifistrings.st", "/strings/Hangul_localizationStrings.st", "/strings/Hangul_cutsceneStrings.st", "/strings/Hangul_conversationStrings.st", "/strings/Hangul_unlockablesStrings.st", "/strings/Hangul_DisneyStrings.st", "/strings/Hangul_ItemStrings.st", "/strings/Norwegian_dialogstrings.st", "/strings/Norwegian_adhocstrings.st", "/strings/Norwegian_gamestrings.st", "/strings/Norwegian_downloadstrings.st", "/strings/Norwegian_savegamestrings.st", "/strings/Norwegian_wifistrings.st", "/strings/Norwegian_localizationStrings.st", "/strings/Norwegian_cutsceneStrings.st", "/strings/Norwegian_conversationStrings.st", "/strings/Norwegian_unlockablesStrings.st", "/strings/Norwegian_DisneyStrings.st", "/strings/Norwegian_ItemStrings.st", "/strings/Swedish_dialogstrings.st", "/strings/Swedish_adhocstrings.st", "/strings/Swedish_gamestrings.st", "/strings/Swedish_downloadstrings.st", "/strings/Swedish_savegamestrings.st", "/strings/Swedish_wifistrings.st", "/strings/Swedish_localizationStrings.st", "/strings/Swedish_cutsceneStrings.st", "/strings/Swedish_conversationStrings.st", "/strings/Swedish_unlockablesStrings.st", "/strings/Swedish_DisneyStrings.st", "/strings/Swedish_ItemStrings.st", "/strings/UKEnglish_dialogstrings.st", "/strings/UKEnglish_adhocstrings.st", "/strings/UKEnglish_gamestrings.st", "/strings/UKEnglish_downloadstrings.st", "/strings/UKEnglish_savegamestrings.st", "/strings/UKEnglish_wifistrings.st", "/strings/UKEnglish_localizationStrings.st", "/strings/UKEnglish_cutsceneStrings.st", "/strings/UKEnglish_conversationStrings.st", "/strings/UKEnglish_unlockablesStrings.st", "/strings/UKEnglish_DisneyStrings.st", "/strings/UKEnglish_ItemStrings.st", "/strings/dialogstrings.st", "/strings/adhocstrings.st", "/strings/gamestrings.st", "/strings/downloadstrings.st", "/strings/downloadstrings.st", "/strings/savegamestrings.st", "/strings/wifistrings.st", "/strings/localizationStrings.st", "/strings/cutsceneStrings.st", "/strings/conversationStrings.st", "/strings/unlockablesStrings.st", "/strings/DisneyStrings.st", "/strings/disneyStrings.st", "/strings/ItemStrings.st", "/strings/itemStrings.st", "/strings/Credits.st", "/strings/credits.st", "/strings/English_dialogstrings.crc", "/strings/English_adhocstrings.crc", "/strings/English_gamestrings.crc", "/strings/English_savegamestrings.crc", "/strings/English_downloadstrings.crc", "/strings/English_wifistrings.crc", "/strings/English_localizationStrings.crc", "/strings/English_cutsceneStrings.crc", "/strings/English_conversationStrings.crc", "/strings/English_unlockablesStrings.crc", "/strings/English_DisneyStrings.crc", "/strings/English_disneyStrings.crc", "/strings/Spanish_dialogstrings.crc", "/strings/Spanish_adhocstrings.crc", "/strings/Spanish_gamestrings.crc", "/strings/Spanish_savegamestrings.crc", "/strings/Spanish_downloadstrings.crc", "/strings/Spanish_wifistrings.crc", "/strings/Spanish_localizationStrings.crc", "/strings/Spanish_cutsceneStrings.crc", "/strings/Spanish_conversationStrings.crc", "/strings/Spanish_unlockablesStrings.crc", "/strings/Spanish_DisneyStrings.crc", "/strings/Italian_dialogstrings.crc", "/strings/Italian_adhocstrings.crc", "/strings/Italian_gamestrings.crc", "/strings/Italian_savegamestrings.crc", "/strings/Italian_downloadstrings.crc", "/strings/Italian_wifistrings.crc", "/strings/Italian_localizationStrings.crc", "/strings/Italian_cutsceneStrings.crc", "/strings/Italian_conversationStrings.crc", "/strings/Italian_unlockablesStrings.crc", "/strings/Italian_DisneyStrings.crc", "/strings/French_dialogstrings.crc", "/strings/French_adhocstrings.crc", "/strings/French_gamestrings.crc", "/strings/French_downloadstrings.crc", "/strings/French_savegamestrings.crc", "/strings/French_wifistrings.crc", "/strings/French_localizationStrings.crc", "/strings/French_cutsceneStrings.crc", "/strings/French_conversationStrings.crc", "/strings/French_unlockablesStrings.crc", "/strings/French_DisneyStrings.crc", "/strings/German_dialogstrings.crc", "/strings/German_adhocstrings.crc", "/strings/German_gamestrings.crc", "/strings/German_downloadstrings.crc", "/strings/German_downloadstrings.crc", "/strings/German_savegamestrings.crc", "/strings/German_wifistrings.crc", "/strings/German_localizationStrings.crc", "/strings/German_cutsceneStrings.crc", "/strings/German_conversationStrings.crc", "/strings/German_unlockablesStrings.crc", "/strings/German_DisneyStrings.crc", "/strings/Dutch_dialogstrings.crc", "/strings/Dutch_adhocstrings.crc", "/strings/Dutch_gamestrings.crc", "/strings/Dutch_downloadstrings.crc", "/strings/Dutch_savegamestrings.crc", "/strings/Dutch_wifistrings.crc", "/strings/Dutch_localizationStrings.crc", "/strings/Dutch_cutsceneStrings.crc", "/strings/Dutch_conversationStrings.crc", "/strings/Dutch_unlockablesStrings.crc", "/strings/Dutch_DisneyStrings.crc", "/strings/Danish_dialogstrings.crc", "/strings/Danish_adhocstrings.crc", "/strings/Danish_gamestrings.crc", "/strings/Danish_downloadstrings.crc", "/strings/Danish_savegamestrings.crc", "/strings/Danish_wifistrings.crc", "/strings/Danish_localizationStrings.crc", "/strings/Danish_cutsceneStrings.crc", "/strings/Danish_conversationStrings.crc", "/strings/Danish_unlockablesStrings.crc", "/strings/Danish_DisneyStrings.crc", "/strings/Japanese_dialogstrings.crc", "/strings/Japanese_adhocstrings.crc", "/strings/Japanese_gamestrings.crc", "/strings/Japanese_downloadstrings.crc", "/strings/Japanese_savegamestrings.crc", "/strings/Japanese_wifistrings.crc", "/strings/Japanese_localizationStrings.crc", "/strings/Japanese_cutsceneStrings.crc", "/strings/Japanese_conversationStrings.crc", "/strings/Japanese_unlockablesStrings.crc", "/strings/Japanese_DisneyStrings.crc", "/strings/Chinese_dialogstrings.crc", "/strings/Chinese_adhocstrings.crc", "/strings/Chinese_gamestrings.crc", "/strings/Chinese_downloadstrings.crc", "/strings/Chinese_savegamestrings.crc", "/strings/Chinese_wifistrings.crc", "/strings/Chinese_localizationStrings.crc", "/strings/Chinese_cutsceneStrings.crc", "/strings/Chinese_conversationStrings.crc", "/strings/Chinese_unlockablesStrings.crc", "/strings/Chinese_DisneyStrings.crc", "/strings/Korean_dialogstrings.crc", "/strings/Korean_adhocstrings.crc", "/strings/Korean_gamestrings.crc", "/strings/Korean_downloadstrings.crc", "/strings/Korean_savegamestrings.crc", "/strings/Korean_wifistrings.crc", "/strings/Korean_localizationStrings.crc", "/strings/Korean_cutsceneStrings.crc", "/strings/Korean_conversationStrings.crc", "/strings/Korean_unlockablesStrings.crc", "/strings/Korean_DisneyStrings.crc", "/strings/Hangul_dialogstrings.crc", "/strings/Hangul_adhocstrings.crc", "/strings/Hangul_gamestrings.crc", "/strings/Hangul_downloadstrings.crc", "/strings/Hangul_savegamestrings.crc", "/strings/Hangul_wifistrings.crc", "/strings/Hangul_localizationStrings.crc", "/strings/Hangul_cutsceneStrings.crc", "/strings/Hangul_conversationStrings.crc", "/strings/Hangul_unlockablesStrings.crc", "/strings/Hangul_DisneyStrings.crc", "/strings/Norwegian_dialogstrings.crc", "/strings/Norwegian_adhocstrings.crc", "/strings/Norwegian_gamestrings.crc", "/strings/Norwegian_downloadstrings.crc", "/strings/Norwegian_savegamestrings.crc", "/strings/Norwegian_wifistrings.crc", "/strings/Norwegian_localizationStrings.crc", "/strings/Norwegian_cutsceneStrings.crc", "/strings/Norwegian_conversationStrings.crc", "/strings/Norwegian_unlockablesStrings.crc", "/strings/Norwegian_DisneyStrings.crc", "/strings/Swedish_dialogstrings.crc", "/strings/Swedish_adhocstrings.crc", "/strings/Swedish_gamestrings.crc", "/strings/Swedish_downloadstrings.crc", "/strings/Swedish_savegamestrings.crc", "/strings/Swedish_wifistrings.crc", "/strings/Swedish_localizationStrings.crc", "/strings/Swedish_cutsceneStrings.crc", "/strings/Swedish_conversationStrings.crc", "/strings/Swedish_unlockablesStrings.crc", "/strings/Swedish_DisneyStrings.crc", "/strings/UKEnglish_dialogstrings.crc", "/strings/UKEnglish_adhocstrings.crc", "/strings/UKEnglish_gamestrings.crc", "/strings/UKEnglish_downloadstrings.crc", "/strings/UKEnglish_savegamestrings.crc", "/strings/UKEnglish_wifistrings.crc", "/strings/UKEnglish_localizationStrings.crc", "/strings/UKEnglish_cutsceneStrings.crc", "/strings/UKEnglish_conversationStrings.crc", "/strings/UKEnglish_unlockablesStrings.crc", "/strings/UKEnglish_DisneyStrings.crc", "/strings/dialogstrings.crc", "/strings/adhocstrings.crc", "/strings/gamestrings.crc", "/strings/downloadstrings.crc", "/strings/downloadstrings.crc", "/strings/savegamestrings.crc", "/strings/wifistrings.crc", "/strings/localizationStrings.crc", "/strings/cutsceneStrings.crc", "/strings/conversationStrings.crc", "/strings/unlockablesStrings.crc", "/strings/DisneyStrings.crc", "/strings/disneyStrings.crc", "/strings/Credits.crc", "/strings/credits.crc", "/chunks/tuxedo.luc", "/chunks/tuxedoDL.luc", "/chunks/spypod.luc", "/chunks/SeasFPM01_NPC_GuitarGuy.luc", "/chunks/SeasFPM04_Beach_Item_PalmTree.luc", "/chunks/SeasFPM04_Item_EmptyTree.luc", "/chunks/SeasFPM04_Item_Coconut.luc", "/chunks/SeasFPM02_ItemFlowerHigh3.luc", "/chunks/SeasFPM02_ItemFlowerLow5.luc", "/chunks/System.luc", "/chunks/DL_IntroCS.luc", "/chunks/DL_OutroCS.luc", "/scripts/Screen.lua", "/chunks/Attic0.luc", "/chunks/Beach0.luc", "/chunks/Beacon0.luc", "/chunks/BoilerRoom0.luc", "/chunks/BookRoom0.luc", "/chunks/Catalog.luc", "/chunks/Catalog0.luc", "/chunks/Catalog1.luc", "/chunks/Catalog2.luc", "/chunks/Catalog3.luc", "/chunks/Catalog4.luc", "/chunks/Catalog5.luc", "/chunks/Catalog6.luc", "/chunks/Catalog7.luc", "/chunks/Catalog8.luc", "/chunks/Catalog9.luc", "/chunks/Catalog10.luc", "/chunks/Catalog11.luc", "/chunks/Catalog12.luc", "/chunks/Catalog13.luc", "/chunks/Catalog14.luc", "/chunks/Catalog15.luc", "/chunks/Catalog16.luc", "/chunks/Catalog17.luc", "/chunks/CaveInterior0.luc", "/chunks/CoffeeShop0.luc", "/chunks/CommandRoom0.luc", "/chunks/Dock0.luc", "/chunks/Dojo0.luc", "/chunks/Fishing.luc", "/chunks/Fishing0.luc", "/chunks/Fishing1.luc", "/chunks/Fishing2.luc", "/chunks/Fishing3.luc", "/chunks/Fishing4.luc", "/chunks/Forest0.luc", "/chunks/GadgetRoom0.luc", "/chunks/GarysNotebook.luc", "/chunks/GarysNotebook0.luc", "/chunks/GarysNotebook1.luc", "/chunks/GarysNotebook2.luc", "/chunks/GarysNotebook3.luc", "/chunks/GarysRoom0.luc", "/chunks/GiftOffice0.luc", "/chunks/GiftRoof0.luc", "/chunks/GiftShop0.luc", "/chunks/HeadQuarters0.luc", "/chunks/Iceberg0.luc", "/chunks/IceFishing.luc", "/chunks/IceRink0.luc", "/chunks/Jetpack", "/chunks/Jetpack0", "/chunks/Jetpack1", "/chunks/Jetpack2", "/chunks/Jetpack3", "/chunks/Jetpack4", "/chunks/Jetpack5", "/chunks/Jetpack6", "/chunks/JetpackBeacon2Mountain.luc", "/chunks/JetpackBotChase.luc", "/chunks/JetpackJPGChase.luc", "/chunks/JetpackMountain2Wild.luc", "/chunks/JetpackMultiplay.luc", "/chunks/JetpackSuperBotChase.luc", "/chunks/JetpackTestLevel.luc", "/chunks/LevelSelect.luc", "/chunks/LevelSelect0.luc", "/chunks/Lighthouse0.luc", "/chunks/Lodge0.luc", "/chunks/Lounge0.luc", "/chunks/MineCrash0.luc", "/chunks/MineFlashlight.luc", "/chunks/MineFlashlight0.luc", "/chunks/MineInterior0.luc", "/chunks/MineLair0.luc", "/chunks/MineShack0.luc", "/chunks/MineShed0.luc", "/chunks/NightClub0.luc", "/chunks/PetShop0.luc", "/chunks/PizzaParlor0.luc", "/chunks/Plaza0.luc", "/chunks/Pool0.luc", "/chunks/PuffleTraining0.luc", "/chunks/Robotomy.luc", "/chunks/Robotomy0.luc", "/chunks/Robotomy1.luc", "/chunks/Robotomy2.luc", "/chunks/Robotomy3.luc", "/chunks/Robotomy4.luc", "/chunks/Robotomy5.luc", "/chunks/Robotomy6.luc", "/chunks/Robotomy7.luc", "/chunks/Robotomy8.luc", "/chunks/Robotomy9.luc", "/chunks/Robotomy10.luc", "/chunks/Robotomy11.luc", "/chunks/Robotomy12.luc", "/chunks/SkiHill0.luc", "/chunks/SkiVillage0.luc", "/chunks/SlopeBattle0.luc", "/chunks/SnowCat.luc", "/chunks/SnowCat0.luc", "/chunks/SnowCat1.luc", "/chunks/SnowCat2.luc", "/chunks/SnowCat3.luc", "/chunks/SnowCat4.luc", "/chunks/SnowCat5.luc", "/chunks/SnowForts0.luc", "/chunks/SportShop0.luc", "/chunks/Stage0.luc", "/chunks/Stage1.luc", "/chunks/StageFlashlight.luc", "/chunks/TallestMountainTop0.luc", "/chunks/TallestMountainTop1.luc", "/chunks/Town0.luc", "/chunks/WildernessBerry0.luc", "/chunks/WildernessCave0.luc", "/chunks/WildernessClearing0.luc", "/chunks/WildernessCliff0.luc", "/chunks/WildernessPuffle0.luc", "/chunks/WildernessRiver0.luc", "/chunks/Template.lua", "/chunks/HQ0_NPC_Rookie.lua", "/chunks/decoderBoard.lua", "/chunks/World_NPC_WhitePuffle.lua", "/chunks/World0_NPC_WhitePuffle.lua", "/chunks/PetShop0_NPC_PetShopClerk.lua", "/Downloads/WhitePuffle.lua", "/chunks/WhitePuffle.lua", "/Downloads/WhitePuffle", "/chunks/TrainingRoom0_NPC_PH.lua", "/chunks/NPC/Puffles/Blue/BluePuffle.lua", "/NPC/Puffles/Blue/BluePuffle.lua", "/chunks/TrainingRoom0_NPC_Rookie.lua", "/chunks/SnowForts0_NPC_SnowFortNPC2.lua", "/chunks/TrainingRoom0_NPC_WhiteBouncer.lua", "/chunks/SnowForts0_NPC_SnowFortNPC.lua", "/chunks/Iceberg0_Object_Boulder.lua", "/chunks/Iceberg0_Items_PuffleCrate.lua", "/chunks/CoffeeShop0_NPC_Barista.lua", "/chunks/Iceberg0_Items_PuffleCrate.lua", "/chunks/BuyButtonScript.lua", "/chunks/BuyButtonScript.lua", "/chunks/PizzaShop0_NPC_PizzaChef.lua", "/chunks/download.lua", "/chunks/mission.lua", "/chunks/DownloadMission.lua", "/chunks/WifiMission.lua", "/chunks/game.lua", "/chunks/start.lua", "/chunks/content.lua", "/chunks/boot.lua", "/chunks/global.lua", "/chunks/debug.lua", "/chunks/init.lua", "/chunks/options.lua", "/chunks/util.lua", "/chunks/utils.lua", "/chunks/level.lua", "/chunks/main.lua", "/chunks/project.lua", "/chunks/menu.lua", "/chunks/inventory.lua", "/chunks/Beach0_Object_LHousDoor.lua", "/chunks/World0_Object_Message.lua", "/chunks/MinigameItem.lua", "/chunks/MissionObjects/M4C1/SnowTrail4.lua", "/MissionObjects/M4C1/SnowTrail4.lua", "/chunks/World0_Object_Paint.lua", "/chunks/PetShop0_NPC_PetShopClerk", "/chunks/PizzaShop0_NPC_PizzaChef", "/Unknown/", "/Failure/", "/Success/", "/Minigames/DanceChallenge/Gesture/Large/", "/Minigames/DanceChallenge/Gesture/Small/", "/Minigames/DanceChallenge/BackgroundSprites/", "/chunks/", "/UI/WarningDialog/warningDialog", "/fonts/faceFront.fnt", "/UI/buttonBig", "/tilesets/BookRoom.tsb", "/UI/ConversationSystem/borderpanels/Player/DialogButton/DialogButton_tail", "/levels/Catalog10_map_0.mpb", "/levels/Catalog11_map_0.mpb", "/levels/Catalog12_map_0.mpb", "/levels/Catalog13_map_0.mpb", "/levels/Catalog14_map_0.mpb", "/levels/Catalog15_map_0.mpb", "/levels/Catalog16_map_0.mpb", "/levels/Catalog17_map_0.mpb", "/levels/Catalog3_map_0.mpb", "/levels/Catalog4_map_0.mpb", "/levels/Catalog5_map_0.mpb", "/levels/Catalog6_map_0.mpb", "/levels/Catalog7_map_0.mpb", "/levels/Catalog8_map_0.mpb", "/levels/Catalog9_map_0.mpb", "/UI/buttonBig_down", "/fonts/faceFront.fnt", "/Minigames/Common/HUD/hudPanelSmall", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/fonts/DSdigital.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/UI/stylusTap", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/tilesets/GadgetRoom1.tsb", "/Minigames/JetPack/Hazards/jetpack", "/Minigames/JetPack/Hazards/jetpack_popping", "/Minigames/JetPack/Hazards/jetpack_flying", "/tilesets/Lounge.tsb", "/UI/buttonBig", "/fonts/faceFront.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/UI/arrowBack", "/UI/MicMenu/mic_volume", "/UI/MicMenu/mic_bar", "/fonts/smallUI.fnt", "/UI/MicMenu/FX_slider", "/UI/MicMenu/FX_bar", "/WifiLogos/loading", "/fonts/SnowFort.fnt", "/UI/buttonBig", "/fonts/faceFront.fnt", "/UI/MainMenu/IDCardButton", "/UI/MainMenu/IDCardButtonBig", "/UI/Smilie_HUD", "/UI/noBadge", "/UI/coin", "/fonts/smallUI.fnt", "/fonts/Comicrazy.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/ProfileCreation/Profile_card", "/UI/badge", "/fonts/smallUI.fnt", "/Tools/broken", "/PC/Spryte/cursor_touch2", "/PC/Spryte/cursor_door", "/PC/Spryte/cursor_talk", "/PC/Spryte/cursor_locked", "/UI/puffleCursor", "/UI/SpyPod/Cursors/mechanoduster", "/UI/SpyPod/Cursors/decoder", "/UI/SpyPod/Cursors/robotomy", "/UI/SpyPod/Cursors/flashlight", "/Tools/brokenImage", "/Particles/sparkle1", "/Particles/sparkle2", "/Particles/sparkle3", "/UI/buttonBig", "/fonts/faceFront.fnt", "/UI/MainMenu/Title_JetPackguy", "/UI/MainMenu/Title_GreenPuffle", "/UI/MainMenu/Title_Bottom", "/bg/Menus/Title_topscreen2", "/bg/Menus/Title_bottomscreen", "/UI/MainMenu/Title_BttnDGamer", "/UI/MainMenu/Title_BttnMicrophone", "/UI/MainMenu/Title_BttnPlay", "/WifiLogos/wireless_strength_level_w", "/WifiLogos/Wi-Fi_strength_level_w", "/UI/loading", "/fonts/MackFont.fnt", "/fonts/faceFront.fnt", "/fs.arc", "/chunks.arc", "/fonts/smallUI.fnt", "/Particles/Puffles/RedHit", "/Particles/Puffles/BlackHit", "/Particles/Puffles/PurpleHit", "/Particles/Puffles/PinkHit", "/Particles/Puffles/YellowHit", "/bg/colors", "/UI/DrawingPanel/brown", "/UI/DrawingPanel/lightGreen", "/UI/DrawingPanel/green", "/UI/DrawingPanel/blue", "/UI/DrawingPanel/lightBlue", "/UI/DrawingPanel/red", "/UI/DrawingPanel/orange", "/UI/DrawingPanel/purple", "/UI/DrawingPanel/lightPurple", "/UI/DrawingPanel/pink", "/UI/DrawingPanel/paleOrange", "/UI/DrawingPanel/yellow", "/UI/DrawingPanel/white", "/UI/DrawingPanel/black", "/UI/buttonBig", "/fonts/faceFront.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/fonts/faceFront.fnt", "/UI/arrowForward", "/UI/arrowBack", "/bg/CartSurfer/CartSurferTitleScreen", "/bg/Menus/Menus_bottom", "/bg/CartSurfer/Instructions/CartSurfer_1player_1", "/bg/CartSurfer/Instructions/CartSurfer_1player_2", "/bg/CartSurfer/Instructions/CartSurfer_1player_3", "/bg/CartSurfer/Instructions/CartSurfer_1player_4", "/bg/CartSurfer/Instructions/CartSurfer_1player_5", "/UI/coin", "/fonts/Comicrazy.fnt", "/fonts/faceFront.fnt", "/UI/ClothingCatalog/PageLButton", "/UI/ClothingCatalog/PageRButton", "/UI/ClothingCatalog/CloseButton", "/BG/ClothingCatalog/TopScreen", "/fonts/tinyUI.fnt", "/UI/ClothingCatalog/BuyButton", "/fonts/smallUI.fnt", "/UI/ClothingCatalog/blue", "/UI/ClothingCatalog/green", "/UI/ClothingCatalog/peach", "/UI/ClothingCatalog/black", "/UI/ClothingCatalog/red", "/UI/ClothingCatalog/yellow", "/UI/ClothingCatalog/purple", "/UI/ClothingCatalog/darkgreen", "/UI/ClothingCatalog/brown", "/UI/ClothingCatalog/pink", "/UI/ClothingCatalog/orange", "/UI/ClothingCatalog/lightblue", "/UI/ClothingCatalog/limegreen", "/UI/buttonBig", "/fonts/faceFront.fnt", "/UI/CommandCoach/Instructions_avitar", "/UI/HelpMenus/StylusDrag", "/UI/arrowForward", "/UI/arrowBack", "/bg/CommandCoach/CommandCoachTitleScreen", "/bg/Menus/Menus_bottom", "/bg/CommandCoach/Instructions/CommandCoach_topScreen", "/bg/CommandCoach/Instructions/CommandCoach_1", "/bg/CommandCoach/Instructions/CommandCoach_2", "/CutsceneAnims/CommandCoach_2", "/bg/CommandCoach/Instructions/CommandCoach_3", "/CutsceneAnims/CommandCoach_3", "/bg/CommandCoach/Instructions/CommandCoach_4", "/CutsceneAnims/CommandCoach_4", "/bg/CommandCoach/Instructions/CommandCoach_5", "/CutsceneAnims/CommandCoach_5", "/bg/CommandCoach/Instructions/CommandCoach_6", "/fonts/Comicrazy.fnt", "/UI/buttonBig", "/fonts/Comicrazy.fnt", "/bg/CreditsTop", "/bg/CreditsBottom", "/fonts/faceFront.fnt", "/UI/arrowForward", "/UI/arrowBack", "/bg/DanceChallenge/DanceChallengeTitleScreen", "/bg/Menus/Menus_bottom", "/bg/DanceChallenge/Instructions/Dance_1player_1", "/bg/DanceChallenge/Instructions/Dance_bottom", "/bg/DanceChallenge/Instructions/Dance_2player_1", "/bg/DanceChallenge/Instructions/Dance_2player_2", "/bg/DanceChallenge/Instructions/Dance_1player_2", "/bg/DanceChallenge/Instructions/Dance_2player_3", "/bg/DanceChallenge/Instructions/Dance_1player_3", "/UI/buttonBig", "/fonts/faceFront.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/UI/buttonBig", "/fonts/faceFront.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/UI/buttonBig", "/fonts/faceFront.fnt", "/fonts/smallUI.fnt", "/bg/Menus/Menus_bottom", "/UI/buttonBig", "/fonts/faceFront.fnt", "/UI/MainMenu/creditsBttn", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/UI/buttonWifiNarrow", "/fonts/Comicrazy.fnt", "/fonts/faceFront.fnt", "/bg/Menus/MultiplayerMenuTop", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/UI/Multiplayer/hostBttn", "/UI/Multiplayer/join2pBttn", "/UI/Multiplayer/join4pBttn", "/UI/arrowBack", "/fonts/faceFront.fnt", "/UI/arrowForward", "/UI/arrowBack", "/bg/icefishing/IceFishingTitleScreen", "/bg/Menus/Menus_bottom", "/bg/icefishing/Instructions/Fishing_1player_1", "/bg/icefishing/Instructions/Fishing_bottom", "/bg/icefishing/Instructions/Fishing_1player_2", "/bg/icefishing/Instructions/Fishing_1player_3", "/UI/buttonBig", "/fonts/faceFront.fnt", "/UI/arrowBack", "/fonts/faceFront.fnt", "/UI/arrowForward", "/UI/arrowBack", "/bg/JetPack/TitleScreen", "/bg/Menus/Menus_bottom", "/bg/JetPack/Instructions/JetPack_bottom", "/bg/JetPack/Instructions/JetPack_2player_1", "/bg/JetPack/Instructions/JetPack_1player_1", "/bg/JetPack/Instructions/JetPack_2player_2", "/bg/JetPack/Instructions/JetPack_1player_2", "/bg/JetPack/Instructions/JetPack_2player_3", "/bg/JetPack/Instructions/JetPack_1player_3", "/UI/LevelSelect/", "/UI/LevelSelect/HUD/MapBack", "/fonts/DSdigital.fnt", "/UI/LevelSelect/penguin", "/UI/LevelSelect/puffle", "/UI/LevelSelect/HUD/playerflag", "/UI/LevelSelect/level_hotspot_up", "/UI/LevelSelect/icons/", "/UI/LevelSelect/HUD/", "/UI/LevelSelect/teleportEffect", "/palettes/", "/UI/buttonBig", "/fonts/faceFront.fnt", "/UI/MainMenu/MainMenu_1player", "/UI/MainMenu/MainMenu_2player", "/UI/MainMenu/MainMenu_Options", "/UI/MainMenu/creditsBttn", "/UI/MainMenu/MainMenu_DGamer", "/UI/arrowBack", "/fonts/Comicrazy.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/UI/buttonBig", "/fonts/faceFront.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/UI/buttonBig", "/fonts/faceFront.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/UI/MinigameResultsMenu/Trophies/", "/UI/buttonMedium", "/fonts/faceFront.fnt", "/fonts/Comicrazy.fnt", "/UI/buttonBig", "/fonts/faceFront.fnt", "/fonts/Comicrazy.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/UI/MinigameMenu/IceFishingButton", "/UI/MinigameMenu/SnowBoardingButton", "/UI/MinigameMenu/CartSurferButton", "/UI/MinigameMenu/DanceChallengeButton", "/UI/MinigameMenu/JetPackButton", "/UI/MinigameMenu/SnowcatButton", "/UI/arrowBack", "/UI/buttonSmall", "/fonts/Comicrazy.fnt", "/fonts/faceFront.fnt", "/UI/WarningDialog/warningDialog", "/UI/buttonMedium", "/UI/ButtonSmaller", "/fonts/Comicrazy.fnt", "/bg/Menus/Menus_bottom", "/bg/IceFishing/Results/Results_top", "/bg/SnowBoarding/Results/Results_top", "/bg/CartSurfer/Results/Results_top", "/bg/DanceChallenge/Results/Results_top", "/bg/SnowCat/Results/Results_top", "/bg/JetPack/Results/Results_top", "/fonts/ComicrazyTitle.fnt", "/UI/MinigameResultsMenu/", "/Trophies/", "/UI/buttonSmall", "/fonts/bigUI.fnt", "/UI/buttonBig", "/fonts/faceFront.fnt", "/fonts/Comicrazy.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/UI/ProfileCreation/lightblue", "/UI/ProfileCreation/black", "/UI/ProfileCreation/blue", "/UI/ProfileCreation/brown", "/UI/ProfileCreation/darkgreen", "/UI/ProfileCreation/pink", "/UI/ProfileCreation/green", "/UI/ProfileCreation/limegreen", "/UI/ProfileCreation/orange", "/UI/ProfileCreation/peach", "/UI/ProfileCreation/purple", "/UI/ProfileCreation/red", "/UI/ProfileCreation/yellow", "/UI/ProfileCreation/bttn_select", "/UI/buttonBig", "/fonts/faceFront.fnt", "/fonts/Comicrazy.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/UI/ProfileCreation/NameField", "/fonts/faceFront.fnt", "/UI/arrowForward", "/UI/arrowBack", "/bg/SnowBoarding/SnowboardingTitleScreen", "/bg/Menus/Menus_bottom", "/bg/SnowBoarding/Instructions/SnowBoarding_bottom", "/bg/SnowBoarding/Instructions/SnowBoarding_1player_1", "/bg/SnowBoarding/Instructions/SnowBoarding_1player_2", "/bg/SnowBoarding/Instructions/SnowBoarding_1player_3", "/UI/buttonBig", "/fonts/faceFront.fnt", "/UI/arrowBack", "/fonts/faceFront.fnt", "/UI/arrowForward", "/UI/arrowBack", "/bg/SnowCat/SnowcatTitleScreen", "/bg/Menus/Menus_bottom", "/bg/SnowCat/Instructions/SnowCat_1player_1", "/bg/SnowCat/Instructions/SnowCat_bottom", "/bg/SnowCat/Instructions/SnowCat_1player_2", "/bg/SnowCat/Instructions/SnowCat_1player_3", "/bg/SnowCat/Instructions/SnowCat_1player_4", "/UI/SpyPod/Decoder/bttnExit", "/fonts/DSdigital.fnt", "/fonts/Comicrazy.fnt", "/BG/SpyLog/Spylog_top", "/BG/SpyLog/Spylog_bottom", "/BG/SpyLog/Spylog_frame", "/UI/SpyPod/SpyLog/bullet", "/UI/SpyPod/SpyLog/checkbox_empty", "/UI/SpyPod/SpyLog/checkbox_marked", "/UI/SpyPod/Decoder/bttnExit", "/fonts/Comicrazy.fnt", "/BG/SpyPod/SpyPod_top", "/BG/SpyPod/SpyPod_bottom", "/UI/SpyPod/btn_snowcat", "/UI/SpyPod/btn_duster", "/UI/SpyPod/btn_flash", "/UI/SpyPod/btn_hq", "/UI/SpyPod/btn_code", "/UI/SpyPod/btn_phone", "/UI/SpyPod/btn_raider", "/UI/SpyPod/btn_quest", "/UI/buttonBig", "/fonts/Comicrazy.fnt", "/UI/WifiContent/Login_penguin", "/UI/WifiContent/CancelKB", "/UI/WifiContent/OkayKB", "/bg/WifiContent/Wifi_Login_top", "/bg/Menus/Menus_bottom", "/UI/WifiContent/Bttn_LoginUsername", "/UI/WifiContent/Bttn_LoginPassword", "/UI/WifiContent/Bttn_LoginUsername_disabled", "/UI/buttonBig", "/fonts/Comicrazy.fnt", "/fonts/faceFront.fnt", "/UI/WifiContent/CoinUpload/CoinUpload_penguin", "/UI/WifiContent/TransferProgress", "/UI/WifiContent/CoinUpload/Plus", "/UI/WifiContent/CoinUpload/Minus", "/UI/chkmarkBttn", "/UI/backBttn", "/UI/WifiContent/CoinUpload/CoinPile", "/UI/WifiContent/CoinUpload/MoneyBag", "/bg/WifiContent/Wifi_Login_top", "/bg/wificontent/coinupload/coinupload_bottom", "/UI/buttonBig", "/fonts/Comicrazy.fnt", "/UI/WifiContent/Login_penguin", "/BG/WifiContent/Wifi_Login_top", "/BG/Menus/Menus_bottom", "/UI/WifiContent/Account", "/UI/WifiContent/Coins", "/UI/WifiContent/News", "/UI/WifiContent/Mission", "/UI/WifiContent/Poll", "/UI/arrowBack", "/UI/buttonBig", "/fonts/Comicrazy.fnt", "/fonts/bigUI.fnt", "/fonts/faceFront.fnt", "/bg/WifiContent/Wifi_Login_top", "/bg/Menus/Menus_bottom", "/UI/Multiplayer/shareNews", "/UI/Multiplayer/shareMission", "/UI/arrowBack", "/UI/buttonBigTall", "/fonts/Comicrazy.fnt", "/UI/WifiContent/Login_penguin", "/bg/WifiContent/Wifi_Login_top", "/bg/Menus/Menus_bottom", "/UI/arrowBack", "/UI/buttonBig", "/fonts/Comicrazy.fnt", "/UI/WifiContent/TransferProgress", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/UI/buttonBig", "/fonts/Comicrazy.fnt", "/UI/WifiContent/TransferProgress", "/UI/arrowBack", "/fonts/bigUI.fnt", "/fonts/smallUI.fnt", "/UI/WifiContent/NewsImage", "/bg/WifiContent/NewsBG_top", "/bg/WifiContent/NewsBG_bottom", "/UI/WifiContent/NewsImages/", "/UI/WifiContent/NewsIcons/", "/UI/buttonBig", "/fonts/faceFront.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/UI/MinigameMenu/SnowBoardingButton", "/UI/MinigameMenu/JetPackButton", "/UI/MinigameMenu/DanceChallengeButton", "/UI/arrowBack", "/UI/buttonBig", "/fonts/Comicrazy.fnt", "/fonts/faceFront.fnt", "/UI/Multiplayer/playerInactive", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/UI/Multiplayer/share", "/UI/Multiplayer/coach", "/UI/MainMenu/MainMenu_DGamer", "/UI/arrowBack", "/UI/Multiplayer/playerActive_player2", "/UI/Multiplayer/playerActive_player3", "/UI/Multiplayer/playerActive_player4", "/UI/buttonBig", "/fonts/Comicrazy.fnt", "/fonts/faceFront.fnt", "/bg/Menus/MultiplayerMenuTop", "/bg/Menus/Menus_bottom", "/UI/Multiplayer/singleCardBttn", "/UI/Multiplayer/2bttn", "/UI/Multiplayer/3and4bttn", "/UI/arrowBack", "/UI/WifiContent/buttonHostJoin", "/fonts/smallUI.fnt", "/fonts/faceFront.fnt", "/bg/Menus/MultiplayerMenuTop", "/bg/Menus/Menus_bottom", "/UI/arrowBack", "/UI/buttonBig", "/fonts/Comicrazy.fnt", "/fonts/bigUI.fnt", "/fonts/faceFront.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Menus/Menus_bottom", "/UI/Multiplayer/shareNews", "/UI/Multiplayer/shareMission", "/UI/arrowBack", "/download.arc", "/UI/ConversationSystem/borderpanels/npcChatBubble_tail_down", "/UI/ConversationSystem/borderpanels/npcChatBubble_tail", "/UI/PaperDoll/penguin/penguinMotion", "/UI/PaperDoll/penguin/body", "/palettes/", "/UI/ConversationSystem/", "/UI/ConversationSystem/borderpanels/Player/DialogButton/DialogBttn", "/UI/ConversationSystem/borderpanels/Player/DialogButton/DialogBttn", "/UI/ConversationSystem/borderpanels/Player/DialogButton/DialogBttn", "/fonts/bigUI.fnt", "/UI/ConversationSystem/borderpanels/npcChatBubblebrdr", "/dat/ConversationData.dat", "/UI/Zoom/", "/UI/UtilityBorder/Corner_TL", "/UI/UtilityBorder/Corner_TR", "/UI/UtilityBorder/Corner_BL", "/UI/UtilityBorder/Corner_BR", "/UI/UtilityBorder/BorderPanel", "/UI/UtilityBorder/BorderPanel_side", "/UI/UtilityBorder/Bttn_Spypod", "/UI/UtilityBorder/Bttn_Map", "/UI/UtilityBorder/Bttn_Inventory", "/UI/UtilityBorder/Bttn_Puffle", "/UI/UtilityBorder/Inventory_Selected", "/UI/InventoryPanel/InventoryBorderPanels/inventory", "/UI/UtilityBorder/Bttn_NoPuffle", "/UI/UtilityBorder/Map_Selected", "/UI/UtilityBorder/Spypod_Selected", "/UI/UtilityBorder/Puffle_Selected", "/UI/ConversationSystem/borderpanels/Player/DialogButton/DialogBttn", "/UI/noMap", "/UI/InventoryPanel/InventoryBorderPanels/current", "/UI/InventoryPanel/Bttn_DressUp", "/UI/inventoryBttnExit", "/UI/InventoryPanel/InventoryBorderPanels/selected", "/UI/PaperDoll/penguin/penguinMotion", "/UI/PaperDoll/penguin/body", "/UI/ConversationSystem/talkbubble_Indicator", "/NPC/CommandCoach/CommandCoach_Clothing", "/palettes/", "/UI/PaperDoll/spypod/spypod", "/UI/ConversationSystem/borderpanels/Player/DialogButton/DialogBttn", "/UI/noBadge", "/fonts/ComicrazyHUD.fnt", "/fonts/DSdigitalHUD.fnt", "/UI/coin", "/UI/Smilie_HUD", "/UI/HUDlocation/beach0", "/UI/CommandCoach/Penguin", "/UI/CommandCoach/HudIconBack", "/UI/MissionSelector/LightsBlink2", "/UI/badge1", "/UI/badge2", "/UI/badge3", "/UI/badge4", "/UI/badge5", "/UI/HUDlocation/", "/UI/Zoom/", "/palettes/coach_", "/UI/SpyGadgetPanel/BorderPanels/Spygadget", "/UI/SpyGadgetPanel/Code", "/UI/SpyGadgetPanel/Communicator", "/UI/SpyGadgetPanel/Duster", "/UI/SpyGadgetPanel/Flashlight", "/UI/SpyGadgetPanel/HQ", "/UI/SpyGadgetPanel/Quest", "/UI/SpyGadgetPanel/Robotmy", "/UI/SpyGadgetPanel/Snowcat", "/fonts/bigUI.fnt", "/UI/ConversationSystem/elipsisIcon", "/UI/ConversationSystem/borderpanels/Player/DialogButton/thoughtbubble_tail", "/UI/noBadge", "/fonts/ComicrazyHUD.fnt", "/UI/coin", "/UI/Smilie_HUD", "/UI/PlayerCard/NewPlayerCard/card", "/UI/clothingBttnExit", "/UI/badge1", "/UI/badge2", "/UI/badge3", "/UI/badge4", "/UI/badge5", "/UI/DressUpCard/NewDressUpCard/dress", "/dat/ClothingData.dat", "/UI/PaperDoll/", "/UI/DressUpCard/NewDressUpCard/tabL", "/UI/scrollbar_bg", "/UI/scrollbar_marker", "/UI/scrollbar_arrow_top", "/UI/scrollbar_arrow_bottom", "/fonts/faceFront.fnt", "/UI/WarningDialog/warningDialog", "/UI/MinigameResultsMenu/Trophies/Fishing_fish_gold", "/UI/MinigameResultsMenu/Trophies/Fishing_chest_gold", "/UI/MinigameResultsMenu/Trophies/CartSurfer_gold", "/UI/MinigameResultsMenu/Trophies/CartSurfer_silver", "/UI/MinigameResultsMenu/Trophies/Dance_gold", "/UI/MinigameResultsMenu/Trophies/Dance_silver", "/UI/MinigameResultsMenu/Trophies/JetPack_gold", "/UI/MinigameResultsMenu/Trophies/JetPack_silver", "/UI/MinigameResultsMenu/Trophies/SnowBoard_gold", "/UI/MinigameResultsMenu/Trophies/SnowBoard_silver", "/UI/MinigameResultsMenu/Trophies/SnowTrek_gold", "/UI/MinigameResultsMenu/Trophies/SnowTrek_silver", "/UI/Zoom/Diploma", "/fonts/smallUI.fnt", "/download.arc", "/UI/SpyPod/communicatorButton", "/fonts/Comicrazy.fnt", "/fonts/DSdigital.fnt", "/UI/stylusTap", "/UI/SpyPod/Communicator/selector", "/UI/SpyPod/Communicator/selector_center", "/BG/SpyPod/comunicator_top", "/BG/SpyPod/comunicator_bottom", "/UI/SpyPod/Decoder/bttnExit", "/BG/SpyPod/SpyPod_bottom", "/UI/SpyPod/Communicator/Dot", "/UI/SpyPod/Communicator/Gary", "/UI/SpyPod/Communicator/JetPackGuy", "/UI/SpyPod/Communicator/Rookie", "/UI/SpyPod/Communicator/PuffleHandler", "/UI/SpyPod/Communicator/Director", "/UI/UtilityBorder/Puffle_Backdrop", "/UI/PuffleMessageBorder/pufflemessage", "/fonts/Comicrazy.fnt", "/UI/loading", "/UI/SpyPod/Puffle/Puffles/Puffle", "/Minigames/IceFishing/Baited", "/Minigames/IceFishing/shock_line", "/Minigames/IceFishing/Baited_up", "/Minigames/IceFishing/Baited_down", "/Minigames/IceFishing/Baited_outWater", "/Minigames/IceFishing/unbaited", "/Minigames/IceFishing/shock_hook", "/Minigames/IceFishing/canOfworms", "/Minigames/IceFishing/treasureChest", "/Minigames/IceFishing/LakeCrabWalk", "/Minigames/IceFishing/crabWalk", "/Minigames/IceFishing/crabIdle", "/Minigames/IceFishing/crabCut", "/Fonts/FFMGscore.fnt", "/Minigames/IceFishing/counterCoin", "/Minigames/IceFishing/counterSquid", "/Minigames/IceFishing/counterFish", "/Minigames/IceFishing/counterWorm", "/Minigames/IceFishing/penguin_up", "/Minigames/IceFishing/puffle_idle", "/Minigames/IceFishing/holeOverlay", "/fonts/faceFront.fnt", "/Minigames/IceFishing/fishEscape", "/Minigames/IceFishing/fishHooked", "/Minigames/IceFishing/fish", "/Minigames/IceFishing/fish_smallEscape", "/Minigames/IceFishing/fish_smallHooked", "/Minigames/IceFishing/fish_small", "/Minigames/IceFishing/fish_largeEscape", "/Minigames/IceFishing/fish_largeHooked", "/Minigames/IceFishing/fish_large", "/Minigames/IceFishing/boot", "/Minigames/IceFishing/barrel", "/Minigames/IceFishing/penguin_electric", "/Minigames/IceFishing/penguin_down", "/Minigames/IceFishing/penguin_sad", "/Minigames/IceFishing/puffle_sad", "/Minigames/IceFishing/puffle_happy", "/palettes/fishing_", "/Minigames/IceFishing/jellyFish", "/Minigames/IceFishing/LakeCrabPush", "/Minigames/IceFishing/MulletSmallest", "/Minigames/IceFishing/MulletSmaller", "/Minigames/IceFishing/Mullet", "/Minigames/IceFishing/puffle_banner", "/fonts/Comicrazy.fnt", "/Minigames/IceFishing/sharkSmall", "/Minigames/IceFishing/shark", "/Minigames/IceFishing/sharkOpen", "/Minigames/IceFishing/squid", "/Minigames/IceFishing/squidHooked", "/Minigames/IceFishing/squidEscape", "/Minigames/IceFishing/squidHold", "/Particles/dust", "/fonts/faceFront.fnt", "/bg/Microgames/MechanoDuster/MechanoDuster_top", "/bg/Microgames/MechanoDuster/MechanodusterBG_Mountain", "/bg/Microgames/MechanoDuster/MechandoDusterBG_CoffeeShop", "/bg/Microgames/MechanoDuster/MechanoDusterBG_SuperRobotNose", "/bg/Microgames/MechanoDuster/MechandoDusterBG_MineShack", "/Microgames/MechanoDuster/MechanoDuster_Cursor", "/Microgames/MechanoDuster/MechanoDuster_Key", "/Microgames/MechanoDuster/MechanoDuster_Snow", "/Microgames/MechanoDuster/MechanoDuster_MissingPen", "/Microgames/MechanoDuster/MechanoDuster_CoffeeBean", "/Microgames/MechanoDuster/MechanoDuster_RobotNose", "/Microgames/MechanoDuster/MechanoDuster_Robolocator", "/Microgames/MechanoDuster/MechanoDuster_Jacket", "/Idle/", "/Jump/", "/LeanBack/", "/Surf/", "/SurfJump/", "/Minigames/CartSurfer/HUD/cartLives", "/Minigames/CartSurfer/HUD/scoreBackground", "/fonts/faceFront.fnt", "/palettes/cartsurferhard_", "/Minigames/CartSurfer/CartAnimations/CartRed", "/palettes/cartsurfer_", "/Minigames/CartSurfer/CartAnimations/Cart", "/Minigames/CartSurfer/ControlPad/", "/fonts/faceFront.fnt", "/Minigames/CartSurfer/Shadow/shadow", "/bg/CartSurfer/CartSurferBottomBG", "/bg/CartSurfer/CartSurferBottom_dPad", "/flc/straight1.flc", "/flc/upInto.flc", "/flc/up.flc", "/flc/upOut.flc", "/flc/downInto.flc", "/flc/down.flc", "/flc/downOut.flc", "/flc/leftInto.flc", "/flc/left.flc", "/flc/leftOut.flc", "/flc/rightInto.flc", "/flc/right.flc", "/flc/rightOut.flc", "/flc/end.flc", "/Minigames/CartSurfer/Shadow/shadow_fadein", "/Minigames/CartSurfer/Shadow/shadow_fadeout", "/Minigames/CartSurfer/Track/", "/UI/SpyPod/Decoder/bttnExit", "/UI/SpyPod/Decoder/bttnEraser", "/UI/SpyPod/Decoder/bttnPencil", "/bg/SpyPod/Decoder/Decoder_top", "/bg/SpyPod/Decoder/Decoder_bottom", "/fonts/DSdigitalCode.fnt", "/UI/SpyPod/Decoder/window", "/UI/SpyPod/Decoder/coverAll", "/fonts/DSdigitalCode.fnt", "/Microgames/Jigsaw/MapPiece", "/Microgames/Jigsaw/PufflePin", "/Microgames/Jigsaw/SnowCatPiece", "/bg/Microgames/Jigsaw/Jigsaw_top", "/bg/Microgames/Jigsaw/Jigsaw_bottom", "/bg/Microgames/Jigsaw/snowcat_gameBG", "/bg/Microgames/PuffleTrainingDoor/PuffleTrainingDoor_bottom", "/fonts/faceFront.fnt", "/PC/Spryte/cursor", "/Microgames/Jigsaw/rotateCursor", "/PC/Spryte/cursor", "/Microgames/Jigsaw/rotateHandle", "/Microgames/CombinationBox/", "/Microgames/CombinationLock/", "/bg/Microgames/CombinationBox/CombinationBox_top", "/bg/Microgames/CombinationLock/CombinationLock_top", "/bg/Microgames/CombinationBox/CombinationBox_bottom", "/bg/Microgames/CombinationLock/CombinationLock_bottom", "/fonts/bigUI.fnt", "/Microgames/CombinationLock/KeypadButton_exit", "/Minigames/DanceChallenge/DancerOneAnimations/Dancer", "/Minigames/DanceChallenge/DancerTwoAnimations/Dancer", "/Minigames/DanceChallenge/Message/goMessage", "/bg/DanceChallenge/DanceChallengeLHandTop", "/bg/DanceChallenge/DanceChallengeLHandBottom", "/bg/DanceChallenge/DanceChallengeRHandTop", "/bg/DanceChallenge/DanceChallengeRHandBottom", "/fonts/faceFront.fnt", "/Minigames/Common/HUD/hudPanelSmall", "/Minigames/DanceChallenge/Message/LookMessage", "/fonts/Comicrazy.fnt", "/UI/YouWin", "/fonts/bigUI.fnt", "/fonts/RockSnail.fnt", "/palettes/dance_", "/Microgames/Robotomy/Spark", "/Microgames/Robotomy/Battery", "/fonts/smallUI.fnt", "/Microgames/Robotomy/Bot", "/bg/Microgames/Robotomy/MenuTop", "/PC/Spryte/cursor", "/Microgames/Robotomy/Exit", "/fonts/smallUI.fnt", "/Microgames/Robotomy/Robot", "/Microgames/Robotomy/Bot", "/bg/Microgames/Robotomy/MenuTop", "/bg/Microgames/Robotomy/", "/Minigames/Common/HUD/hudPanelSmall", "/Minigames/JetPack/Pickups/coin", "/Minigames/JetPack/HUD/hudMeter", "/fonts/ComicrazyHUD.fnt", "/Minigames/JetPack/Player/penguin", "/Minigames/JetPack/Player2", "/Minigames/JetPack/Player3", "/Minigames/JetPack/Player4", "/penguin", "/bg/JetPack/waiting_top", "/bg/Menus/Menus_bottom", "/fonts/Comicrazy.fnt", "/Minigames/JetPack/NPC/JetPackGuy_Flying", "/UI/YouWin", "/fonts/bigUI.fnt", "/Minigames/JetPack/HUD/Multiplay/Highlighter", "/Minigames/JetPack/HUD/HudCoinLarge", "/Minigames/JetPack/HUD/HudCoin", "/Minigames/JetPack/HUD/Multiplay/Player1", "/Minigames/JetPack/HUD/Multiplay/Player2", "/Minigames/JetPack/HUD/Multiplay/Player3", "/Minigames/JetPack/HUD/Multiplay/Player4", "/Minigames/JetPack/HUD/Multiplay/", "/Minigames/JetPack/Hazards/anvil", "/Minigames/JetPack/Hazards/anvil_popping", "/Minigames/JetPack/Hazards/anvil_falling", "/Minigames/JetPack/NPC/JetPackGuy_Flying", "/Minigames/JetPack/NPC/JetBotActive", "/Minigames/JetPack/NPC/SuperBotActive", "/Minigames/JetPack/Clouds/cloud1", "/Minigames/JetPack/Clouds/cloud2", "/Minigames/JetPack/Clouds/cloud3", "/Minigames/JetPack/Hazards/pad_building", "/Minigames/JetPack/Hazards/anvil", "/Minigames/JetPack/Hazards/tree_left", "/Minigames/JetPack/Hazards/tree_right", "/Minigames/JetPack/Hazards/tree_pine1", "/Minigames/JetPack/Hazards/tree_pine2", "/Minigames/JetPack/Hazards/tree_pine3", "/Minigames/JetPack/Pickups/fuel", "/Minigames/JetPack/Pickups/coin", "/Minigames/JetPack/Pickups/coin_large", "/Minigames/JetPack/Landing/landing_pad", "/fonts/tinyUI.fnt", "/Minigames/JetPack/Pickups/coin", "/Particles/jetsmoke", "/palettes/jetpack_", "/UI/ConversationSystem/borderpanels/npcChatBubblebrdr", "/Minigames/JetPack/Landing/landing_pad", "/fonts/Comicrazy.fnt", "/bg/Microgames/Flashlight/TopScreen", "/Microgames/Flashlight/flashlight", "/fonts/faceFront.fnt", "/Microgames/Flashlight/Light", "/Microgames/Flashlight/Crank", "/Microgames/Jigsaw/rotateCursor", "/fonts/faceFront.fnt", "/Microgames/Flashlight/flashlight", "/Microgames/Flashlight/Light", "/Microgames/Flashlight/Crank", "/Microgames/GarysNotebook/Dot", "/bg/Microgames/GarysNotebook/GarysNotebook_arm_done", "/bg/Microgames/GarysNotebook/GarysNotebook_chest_done", "/bg/Microgames/GarysNotebook/GarysNotebook_head_done", "/bg/Microgames/GarysNotebook/GarysNotebook_top", "/fonts/faceFront.fnt", "/bg/Microgames/GarysNotebook/GarysNotebook_arm_done", "/bg/Microgames/GarysNotebook/GarysNotebook_chest_done", "/bg/Microgames/GarysNotebook/GarysNotebook_head_done", "/Minigames/Common/HUD/hudPanelSmall", "/Minigames/SnowCat/HUD/hudCoin", "/fonts/Comicrazy.fnt", "/Minigames/SnowCat/HUD/hudMeter", "/Minigames/SnowCat/HUD/hudTimer", "/Minigames/SnowCat/HUD/hudSpeedMeter", "/Minigames/SnowCat/SteeringWheel", "/bg/SnowCat/SnowCat_bottom", "/3d/SnowTrekker/Body", "/3d/SnowTrekker/Plow", "/3d/SnowTrekker/Pontoons", "/fonts/smallUI.fnt", "/3d/SnowTrekker/Body_idle", "/3d/SnowTrekker/Pontoons_deploy", "/3d/SnowTrekker/Pontoons_retract", "/3d/SnowTrekker/Plow_deploy", "/3d/SnowTrekker/Plow_retract", "/3d/SnowTrekker/Body_activate", "/fonts/Comicrazy.fnt", "/Particles/dust", "/Minigames/DownhillSnowboarding/HUD/snowboardLives", "/Minigames/DownhillSnowboarding/Gesture/backflip_gest", "/Minigames/DownhillSnowboarding/TopScreen/Crowd", "/Minigames/DownhillSnowboarding/TopScreen/Flags", "/fonts/ComicrazyTitle.fnt", "/Minigames/DownhillSnowboarding/Gesture/frontflip_gest", "/Minigames/DownhillSnowboarding/Gesture/headspin_gest", "/Minigames/DownhillSnowboarding/Gesture/360_gest", "/Minigames/DownhillSnowboarding/Gesture/indy_gest", "/Minigames/DownhillSnowboarding/Gesture/nosegrab_gest", "/Minigames/DownhillSnowboarding/Gesture/method_gest", "/Minigames/DownhillSnowboarding/Gesture/rocketair_gest", "/UI/YouWin", "/fonts/bigUI.fnt", "/fonts/Comicrazy.fnt", "/fonts/smallUI.fnt", "/Minigames/DownhillSnowboarding/BoarderAnimations/Boarder", "/bg/SnowBoarding/SnowboardingTopScreen", "/flc/snowboarding/slopescroll.flc", "/flc/snowboarding/ramp.flc", "/flc/snowboarding/slope2sky.flc", "/flc/snowboarding/skyscroll_up.flc", "/flc/snowboarding/skytop_up.flc", "/flc/snowboarding/skytop_down.flc", "/flc/snowboarding/skyscroll_Down.flc", "/flc/snowboarding/sky2slope.flc", "/flc/snowboarding/slopecrash.flc", "/fonts/faceFront.fnt", "/fonts/ComicrazyTitle.fnt", "/Minigames/DownhillSnowboarding/Track/", "/UI/loading", "/UI/ellipsis", "/UI/stylusTap", "/bg/LevelSelect/Map_top", "/bg/Microgames/GarysNotebook/GarysNotebook_arm_done", "/bg/Microgames/GarysNotebook/GarysNotebook_chest_done", "/bg/Microgames/GarysNotebook/GarysNotebook_head_done", "/fonts/DSdigital.fnt", "/UI/MissionSelector/LightsBlink", "/BG/MissionSelector/MissionSelect_top", "/BG/MissionSelector/MissionSelect_bottom", "/BG/MissionSelector/MissionSelect_frame", "/UI/MissionSelector/Bttn_Exit", "/UI/MissionSelector/Bttn_arrow_up", "/UI/MissionSelector/Bttn_arrow_down", "/UI/MissionSelector/Bttn_launch", "/UI/MissionSelector/Mission_Image", "/UI/MissionSelector/Mission_Image_m", "/UI/MissionSelector/Bttn_mission", "/UI/Zoom/flip", "/UI/Zoom/bttnExit", "/bg/Menus/Menus_bottom", "/fonts/bigUI.fnt", "/Sparkles/SparklyLeash/Univ_Obj_SparkleLeash1", "/download.arc", "/Downloads.rdt", "/Particles/Fire", "/Particles/droplet", "/Particles/debris1", "/Particles/debris2", "/Minigames/JetPack/Pickups/coin", "/3d/SuperRobot/animTextures/puf_ski_2", "/3d/SuperRobot/animTextures/puf_ski_3", "/3d/SuperRobot/animTextures/puf_ski_1", "/3d/SuperRobot/animTextures/puf_dock_1", "/3d/SuperRobot/animTextures/puf_dock_2", "/3d/SuperRobot/animTextures/puf_dock_3.", "/3d/SuperRobot/animTextures/puf_tall2_3", "/3d/SuperRobot/animTextures/puf_beach_1", "/3d/SuperRobot/animTextures/puf_beach_2", "/3d/SuperRobot/animTextures/puf_beach_3", "/3d/SuperRobot/animTextures/puf_tall2_1", "/3d/SuperRobot/animTextures/puf_tall2_2", "/3d/SuperRobot/animTextures/SRjetpackflame1", "/3d/SuperRobot/animTextures/SRjetpackflame2", "/3d/SuperRobot/animTextures/SRjetpackflame3", "/3d/SuperRobot/animTextures/SRjetpackflame4", "/3d/SuperRobot/SuperRobotBeach", "/3d/SuperRobot/SuperRobotPufflesBeach", "/3d/SuperRobot/SuperRobotDock", "/3d/SuperRobot/SuperRobotPufflesDock", "/3d/SuperRobot/SuperRobotTown", "/3d/SuperRobot/SuperRobotPufflesTown", "/3d/SuperRobot/SuperRobotTall", "/3d/SuperRobot/SuperRobotBeacon", "/3d/SuperRobot/SuperRobotPufflesBeacon", "/3d/SuperRobot/SuperRobotRoof", "/3d/SuperRobot/SuperRobotPufflesRoof", "/3d/SuperRobot/SuperRobotSki", "/3d/SuperRobot/SuperRobotPufflesSki", "/3d/SuperRobot/SuperRobotTall2", "/3d/SuperRobot/SuperRobotTall3", "/3d/SuperRobot/SuperRobotTall4", "/3d/SuperRobot/SuperRobotPufflesTall", "/3d/SuperRobot/SuperRobotPufflesTall2", "/3d/SuperRobot/SuperRobotPufflesTall3", "/3d/SuperRobot/SuperRobotPufflesTall4", "/3d/SuperRobot/SuperRobotBeach_idle", "/3d/SuperRobot/SuperRobotPufflesBeach_idle", "/3d/SuperRobot/SuperRobotDock_idle", "/3d/SuperRobot/SuperRobotPufflesDock_idle", "/3d/SuperRobot/SuperRobotTown_idle", "/3d/SuperRobot/SuperRobotPufflesTown_idle", "/3d/SuperRobot/SuperRobotTall_idle", "/3d/SuperRobot/SuperRobotBeacon_idle", "/3d/SuperRobot/SuperRobotPufflesBeacon_idle", "/3d/SuperRobot/SuperRobotRoof_idle", "/3d/SuperRobot/SuperRobotPufflesRoof_idle", "/3d/SuperRobot/SuperRobotSki_idle", "/3d/SuperRobot/SuperRobotSki_idle2", "/3d/SuperRobot/SuperRobotPufflesSki_idle", "/3d/SuperRobot/SuperRobotTall2_idle", "/3d/SuperRobot/SuperRobotTall3_idle", "/3d/SuperRobot/SuperRobotTall4_idle", "/3d/SuperRobot/SuperRobotPufflesTall_idle", "/3d/SuperRobot/SuperRobotPufflesTall2_idle", "/3d/SuperRobot/SuperRobotPufflesTall3_idle", "/3d/SuperRobot/SuperRobotPufflesTall4_idle", "/3d/SuperRobot/SuperRobotBeach_talk", "/3d/SuperRobot/SuperRobotDock_talk", "/3d/SuperRobot/SuperRobotTown_talk", "/3d/SuperRobot/SuperRobotTall_talk", "/3d/SuperRobot/SuperRobotBeacon_talk", "/3d/SuperRobot/SuperRobotRoof_talk", "/3d/SuperRobot/SuperRobotSki_talk", "/3d/SuperRobot/SuperRobotTall2_talk", "/3d/SuperRobot/SuperRobotTall3_talk", "/3d/SuperRobot/SuperRobotTall4_talk", "/3d/SuperRobot/SuperRobotPufflesTall_talk", "/3d/SuperRobot/SuperRobotPufflesTall2_talk", "/3d/SuperRobot/SuperRobotPufflesTall3_talk", "/3d/SuperRobot/SuperRobotPufflesTall4_talk", "/3d/SuperRobot/SuperRobotBeach_Jump", "/3d/SuperRobot/SuperRobotDock_Jump", "/3d/SuperRobot/SuperRobotTown_Jump", "/3d/SuperRobot/SuperRobotTall_Jump", "/3d/SuperRobot/SuperRobotBeacon_Jump", "/3d/SuperRobot/SuperRobotRoof_Jump", "/3d/SuperRobot/SuperRobotSki_Jump", "/3d/SuperRobot/SuperRobotTall2_Jump", "/3d/SuperRobot/SuperRobotTall3_Jump", "/3d/SuperRobot/SuperRobotTall4_Jump", "/3d/SuperRobot/SuperRobotPufflesTall_Jump", "/3d/SuperRobot/SuperRobotPufflesTall2_Jump", "/3d/SuperRobot/SuperRobotPufflesTall3_Jump", "/3d/SuperRobot/SuperRobotPufflesTall4_Jump", "/3d/SuperRobot/SuperRobotBeach_fly", "/3d/SuperRobot/SuperRobotPufflesBeach_fly", "/3d/SuperRobot/SuperRobotDock_fly", "/3d/SuperRobot/SuperRobotPufflesDock_fly", "/3d/SuperRobot/SuperRobotTown_fly", "/3d/SuperRobot/SuperRobotTall_fly", "/3d/SuperRobot/SuperRobotBeacon_fly", "/3d/SuperRobot/SuperRobotRoof_fly", "/3d/SuperRobot/SuperRobotSki_fly", "/3d/SuperRobot/SuperRobotPufflesSki_fly", "/3d/SuperRobot/SuperRobotTall2_fly", "/3d/SuperRobot/SuperRobotTall3_fly", "/3d/SuperRobot/SuperRobotTall4_fly", "/3d/SuperRobot/SuperRobotPufflesTall_fly", "/3d/SuperRobot/SuperRobotPufflesTall2_fly", "/3d/SuperRobot/SuperRobotPufflesTall3_fly", "/3d/SuperRobot/SuperRobotPufflesTall4_fly", "/3d/SuperRobot/SuperRobotRoof_sneeze", "/3d/Snowman/Snowman", "/3d/Snowman", "/3d/SuperRobot/SuperRobot_jetpackFlame", "/3d/SuperRobot/SuperRobotPufflesDock", "/3d/SuperRobot/SuperRobotPufflesSki", "/3d/SuperRobot/SuperRobotPufflesBeach", "/bg/MissionHUD_BG", "/chunks/", "/scripts/", "/UI/SpyPod/spypod", "/fonts/smallUI.fnt", "/WifiLogos/loading", "/UI/Zoom/", "/UI/Smilie", "/Particles/Gesture/sparkle4", "/Particles/Gesture/sparkle5", "/Particles/Gesture/sparkle6", "/Particles/Gesture/sparkle1", "/Particles/Gesture/sparkle2", "/Particles/Gesture/sparkle3", "/Particles/commandCoach", "/NPC/Puffles/Black/BlackPuffle", "/NPC/Puffles/Blue/BluePuffle", "/NPC/Puffles/Green/GreenPuffle", "/NPC/Puffles/Pink/PinkPuffle", "/NPC/Puffles/Purple/PurplePuffle", "/NPC/Puffles/Red/RedPuffle", "/NPC/Puffles/Yellow/YellowPuffle", "/tilesets/Attic.tsb", "/tilesets/Beach_BG.tsb", "/tilesets/Beach.tsb", "/tilesets/Beacon.tsb", "/tilesets/BoilerRoom.tsb", "/levels/Catalog1_map_0.mpb", "/tilesets/Catalog1.tsb", "/levels/Catalog2_map_0.mpb", "/tilesets/CaveInterior.tsb", "/tilesets/CoffeeShop.tsb", "/tilesets/CommandRoom.tsb", "/tilesets/Dock_BG.tsb", "/tilesets/Dock.tsb", "/tilesets/Dojo.tsb", "/tilesets/Fishing.tsb", "/tilesets/Forest.tsb", "/tilesets/GadgetRoom.tsb", "/levels/GarysNotebook1_map_0.mpb", "/levels/GarysNotebook2_map_0.mpb", "/levels/GarysNotebook3_map_0.mpb", "/tilesets/GarysNotebook.tsb", "/tilesets/GarysRooms.tsb", "/tilesets/GiftOffice.tsb", "/tilesets/GiftRoof.tsb", "/tilesets/GiftShop.tsb", "/tilesets/HQ.tsb", "/tilesets/Iceberg.tsb", "/levels/IceFishing_map_0.mpb", "/tilesets/IceFishing.tsb", "/tilesets/IceRink.tsb", "/tilesets/Jetpack_BG.tsb", "/tilesets/Jetpack.tsb", "/UI/LevelSelect/", "/levels/LevelSelect_map_0.mpb", "/tilesets/LevelSelectMap.tsb", "/tilesets/Lighthouse.tsb", "/tilesets/Lodge.tsb", "/tilesets/MineCrashSite.tsb", "/tilesets/MineExterior.tsb", "/MissionObjects/M2C3/scripted/", "/levels/MineFlashlight_map_0.mpb", "/tilesets/MineInterior.tsb", "/tilesets/MineShedInterior.tsb", "/tilesets/MineTunnelExit.tsb", "/tilesets/Mountain.tsb", "/tilesets/NightClub.tsb", "/tilesets/PetShop.tsb", "/tilesets/PizzaParlor.tsb", "/tilesets/Plaza.tsb", "/tilesets/Pool.tsb", "/tilesets/PuffleTraining.tsb", "/Microgames/Robotomy/", "/levels/Robotomy10_map_0.mpb", "/Microgames/Robotomy/", "/levels/Robotomy11_map_0.mpb", "/Microgames/Robotomy/", "/levels/Robotomy12_map_0.mpb", "/Microgames/Robotomy/", "/levels/Robotomy1_map_0.mpb", "/Microgames/Robotomy/", "/levels/Robotomy2_map_0.mpb", "/Microgames/Robotomy/", "/levels/Robotomy3_map_0.mpb", "/Microgames/Robotomy/", "/levels/Robotomy4_map_0.mpb", "/Microgames/Robotomy/", "/levels/Robotomy5_map_0.mpb", "/Microgames/Robotomy/", "/levels/Robotomy6_map_0.mpb", "/Microgames/Robotomy/", "/levels/Robotomy7_map_0.mpb", "/Microgames/Robotomy/", "/levels/Robotomy8_map_0.mpb", "/Microgames/Robotomy/", "/levels/Robotomy9_map_0.mpb", "/tilesets/Robotomy.tsb", "/tilesets/SkiVillage.tsb", "/tilesets/SlopeBattle_BG.tsb", "/tilesets/SlopeBattle.tsb", "/tilesets/SnowCat_BG.tsb", "/tilesets/SnowCat.tsb", "/tilesets/SnowForts.tsb", "/tilesets/SportShop.tsb", "/NPC/Dot/", "/Location/Stage/state/", "/levels/StageFlashlight_map_0.mpb", "/tilesets/Stage.tsb", "/tilesets/TallestMountainTop_BG.tsb", "/tilesets/TallestMountainTop.tsb", "/tilesets/Town.tsb", "/tilesets/Wilderness.tsb", "/UI/buttonBig", "/fonts/smallUI.fnt", "/UI/WifiContent/TransferProgress", "/fonts/faceFront.fnt", "/UI/arrowBack", "/bg/WifiContent/Wifi_Login_top", "/bg/Menus/Menus_bottom", "/fonts/smallUI.fnt", "/font/smallUI.fnt", "/UI/cancelBttn", "/UI/chkmarkBttn", "/fonts/bigUI.fnt", "/UI/WarningDialog/warningDialog", "/UI/WarningDialog/warningDialog", "/Keyboard/Keys/", "/Keyboard/keyboard", "/dat/AsciiMap.dat", "/dat/AccentMap.dat", "/Keyboard/cancel", "/Tools/broken", "/Tools/broken", "/strings/", "/attempt to yield across metamethod/C-call boundary", "/string/function/table expected", "/Scenes/DG1_FontColors", "/DGamer/Scenes", "/Scenes/DG1_Back2GameIcon.nclr", "/Scenes/ChatMenuSpritesCharFile", "/Scenes/AvatarChatBack", "/Scenes/DG1_TopScreenMasterPalette", "/Scenes/DG1_TopScreenMasterPalette.nclr", "/Scenes/DG1_HelpTextIcons", "/Scenes/DG1_Back2GameIcon.nclr", "/Scenes/DG1_MainMenuMasterPalette", "/Scenes/DG1_TopScreenMasterPalette", "/Scenes/DG1_Small_Floating_Bubbles_TopS", "/Scenes/DG1_Small_Floating_Bubbles", "/Scenes/WiFiLevelChar", "/Scenes/WiFiLevel", "/Scenes/DG1_WirelessTextbox", "/GuiContent.txt", "/DGamer/Scenes/MainMenu_Main_01.NCGR", "/scripts/Screen.lua", "/scripts/Screen.lua", "/scripts/M2C1_CommandRm0_NPC_Dot.lua", "/scripts/M2C1_CommandRm1_NPC_Dot.lua", "/scripts/M2C3_CommandRm0_NPC_Dot.lua", "/scripts/M3C3_CommandRm0_NPC_Gary.lua", "/scripts/M3C3_CommandRm0_NPC_Gary.lua", "/scripts/M1C2_CommandRoom0_NPC_Dot.lua", "/scripts/M1C3_CommandRoom0_NPC_Dot.lua", "/scripts/M3C1_CommandRoom0_NPC_Dot.lua", "/scripts/FP08_CommandRoom0_NPC_Gary.lua", "/scripts/M1C2_CommandRoom0_NPC_Dot2.lua", "/scripts/M3C3_CommandRm0_NPC_Director.lua", "/scripts/M2C1_CommandRm1_NPC_Director.lua", "/scripts/M2C3_CommandRm0_NPC_Director.lua", "/scripts/M2C1_CommandRm1_Items_Dossier.lua", "/scripts/M5C1_CommandRoom_NPC_Director.lua", "/scripts/M1C3_CommandRoom0_NPC_Director.lua", "/scripts/M3C3_CommandRm0_Objects_OilCan.lua", "/scripts/M2C1_CommandRm1_NPC_JetPackGuy.lua", "/scripts/M3C1_CommandRoom0_NPC_Director.lua", "/scripts/M2C3_CommandRm0_NPC_JetPackGuy.lua", "/scripts/M2C1_CommandRm0_NPC_JetPackGuy.lua", "/scripts/M1C2_CommandRoom0_NPC_Director.lua", "/scripts/FP_CommandRoom0_Items_HardDrive.lua", "/scripts/M1C3_CommandRoom0_NPC_JetPackGuy.lua", "/scripts/Doors_CommandRoom0_CommandDoor2HQ.lua", "/scripts/M3C1_CommandRoom0_ItemsM3C1_OilCan.lua", "/scripts/M1C2_CommandRoom0_ItemsM1C2_Antenna.lua", "/scripts/M3C3_CommandRm0_Objects_RoboLocator.lua", "/scripts/M1C2_CommandRoom0_ItemsM1C2_WhistleBox.lua", "/scripts/M1C2_CommandRoom0_ItemsM1C2_SpyPodHalf1.lua", "/scripts/M1C2_CommandRoom0_ItemsM1C2_SpyPodHalf2.lua", "/scripts/M1C2_CommandRoom0_ItemsM1C2_PuffleCrate.lua", "/scripts/M1C3_CommandRoom0_ItemsM1C3_BadgeGlasses.lua", "/scripts/M1C2_CommandRoom0_ItemsM1C2_TableComboBox.lua", "/scripts/M1C2_CommandRoom0_ItemsM1C2_TableComboBox.lua", "/scripts/M1C2_CommandRoom0_ItemsM1C2_FakeSpyPodHalf.lua", "/scripts/M1C2_CommandRoom0_ItemsM1C2_FakeSpyPodHalf.lua", "/scripts/M1C2_CommandRoom0_ItemsM1C2_M1C2SecretCode.lua", "/scripts/M3C1_CommandRoom0_ItemsM3C1_DeflatedInnerTube.lua", "/scripts/CommandCoach_CommandRoom0_CommandCoach_NPCCoach.lua", "/scripts/CommandCoach_CommandRoom0_LocationCommandRoom_CommandCoachOpen.lua", "/scripts/CommandCoach_CommandRoom0_LocationCommandRoom_CommandCoachCover.lua", "/NPC/Dot/", "/UI/Zoom/", "/NPC/Gary/", "/NPC/Director/", "/NPC/JetPackGuy/", "/NPC/Puffles/Blue/", "/NPC/CommandCoach/", "/UI/InventoryPanel/", "/MissionObjects/M3C1/", "/Objects/CommandCoach/", "/Location/CommandRoom/touch/", "/Location/CommandRoom/state/", "/Location/CommandRoom/static/", "/MissionObjects/M1C2/scripted/", "/MissionObjects/M1C3/scripted/", "/MissionObjects/M2C1/scripted/", "/levels/CommandRoom0_map_0.mpb", "/Location/CommandRoom/scripted/", "/scripts/M5C1_Dock_NPC_Gary.lua", "/scripts/M5C1_Dock_NPC_Loop.lua", "/scripts/M2C1_Items_NutsBolts.lua", "/scripts/SeasFPM01_NPC_SickNPC.lua", "/scripts/M2C1_Dock0_NPC_DockNPC.lua", "/scripts/M1C1_Dock0_NPC_DockNPC.lua", "/scripts/FPM06_Dock_Items_Boat1.lua", "/scripts/Doors_Dock0_DockDoor2Ski.lua", "/scripts/FPM06_Dock_NPC_LostBoats.lua", "/scripts/FPM06_Dock_NPC_LostBoats.lua", "/scripts/M5C1_Dock_NPC_SuperRobot.lua", "/scripts/SeasFPM02_Item_FlowerHigh.lua", "/scripts/SeasFPM02_Item_FlowerLow6.lua", "/scripts/Doors_Dock0_DockDoor2Beach.lua", "/scripts/M1C1_Dock0_NPC_DotInnerTubes.lua", "/scripts/M1C1_Dock0_NPC_DotEmptyTubes.lua", "/scripts/M1C2_Dock0_CommandCoach_SpecialItem.lua", "/NPC/Dot/", "/NPC/LostBoats/", "/3d/SuperRobot/SuperRobotDockG", "/NPC/SuperRobot/", "/NPC/SickPenguin/", "/NPC/Puffles/Pink/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Objects/WorldItems/", "/Location/Dock/state/", "/Location/Dock/touch/", "/Location/Dock/static/", "/Objects/CommandCoach/", "/NPC/GenericNPC/Yellow/", "/Objects/Seasonal/Xmas/", "/levels/Dock0_map_0.mpb", "/levels/Dock0_map_1.mpb", "/Objects/Seasonal/Summer/", "/MissionObjects/SeasonalM2/", "/Objects/Seasonal/Halloween/", "/Objects/Seasonal/AprilFools/", "/scripts/M2C2_Dojo0_NPC_Dot.lua", "/scripts/FP01_Dojo0_NPC_Dot.lua", "/scripts/M1C2_Dojo0_NPC_Dot.lua", "/scripts/M1C2_Dojo0_NPC_BouncerNPC.lua", "/scripts/FP01_Dojo0_NPC_BouncerNPC.lua", "/scripts/Doors_Dojo0_DojoDoor2Level.lua", "/scripts/Doors_Dojo0_DojoDoor2Puffle.lua", "/scripts/M3C2_Dojo0_NPC_PuffleHandler.lua", "/scripts/FPM05_Dojo_NPC_CoinCountingNPC.lua", "/scripts/FPM05_Dojo_Items_PizzaSeaWeedFake.lua", "/NPC/Dot/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/NPC/PuffleHandler/", "/NPC/CoinCountingNPC/", "/Location/Dojo/state/", "/MissionObjects/M5C1/", "/MissionObjects/M4C1/", "/Objects/CommandCoach/", "/levels/Dojo0_map_0.mpb", "/Objects/Seasonal/Xmas/", "/Objects/Seasonal/Summer/", "/Objects/Seasonal/Halloween/", "/Objects/Seasonal/AprilFools/", "/MissionObjects/FreeplayM4/scripted/", "/Objects/", "/scripts/M4C1_Fishing_Items_Hole.lua", "/scripts/M4C1_Fishing_Items_JPGClue.lua", "/scripts/M4C1_SkiLodge_NPC_JetPackGuy.lua", "/scripts/FP00_Fishing0_Items_FishingHole.lua", "/scripts/Fishing0_CommandCoach_SpecialItem.lua", "/NPC/JetPackGuy/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M4C1/", "/Objects/CommandCoach/", "/Location/Fishing/state/", "/Objects/Seasonal/Summer/", "/levels/Fishing0_map_0.mpb", "/Objects/Seasonal/AprilFools/", "/scripts/FPM10_NPC_Puffle6.lua", "/scripts/FP03_FOREST0_NPC_NPC1.lua", "/scripts/FP03_FOREST0_NPC_NPC2.lua", "/scripts/SeasFPM04_Item_Coconut.lua", "/scripts/SeasFPM04_Item_EmptyTree.lua", "/scripts/FP03_FOREST0_Items_Tracks.lua", "/scripts/SeasFPM02_Item_FlowerMid3.lua", "/scripts/SeasFPM02_Item_FlowerLow2.lua", "/scripts/FP03_FOREST0_NPC_TourGuide.lua", "/scripts/FPM07_Forest_NPC_ConfusedNPC.lua", "/scripts/FPM07_Forest_Items_CodeForest.lua", "/scripts/Doors_Forest0_Forest2Minigame.lua", "/scripts/SeasFPM04_Forest_Item_PalmTree.lua", "/scripts/M1C3_Forest0_CommandCoach_SpecialItem.lua", "/NPC/Tourist/", "/NPC/TourGuide/", "/NPC/ConfusedNPC/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/NPC/Puffles/Green/", "/Location/Stage/state/", "/Objects/CommandCoach/", "/Location/Forest/state/", "/levels/Forest0_map_0.mpb", "/Objects/Seasonal/Summer/", "/MissionObjects/SeasonalM2/", "/MissionObjects/SeasonalM4/", "/MissionObjects/M1C3/scripted/", "/MissionObjects/FreeplayM7/scripted/", "/scripts/M4C2_GadgetRoom_NPC_Dot.lua", "/scripts/M4C1_GadgetRoom_NPC_Gary.lua", "/scripts/M4C2_GadgetRoom_NPC_Gary.lua", "/scripts/FP09_GadgetRoom0_NPC_Gary.lua", "/scripts/M3C3_GadgetRoom0_NPC_Gary.lua", "/scripts/LocationGadgetRoom_BoomBox.lua", "/scripts/LocationGadgetRoom_Balloon.lua", "/scripts/M2C1_GadgetRm0_Object_Drawer.lua", "/scripts/M3C3_GadgetRoom_Objects_SnowCat.lua", "/scripts/M3C3_GadgetRoom0_NPC_GaryCalibrate.lua", "/scripts/Doors_GadgetRoom0_GadgetRoomDoor2HQ.lua", "/NPC/Dot/", "/NPC/Gary/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M4C1/", "/MissionObjects/M5C1/", "/MissionObjects/M3C3/", "/MissionObjects/M3C2/", "/Objects/CommandCoach/", "/Location/GadgetRoom/touch/", "/Location/GadgetRoom/state/", "/Location/GadgetRoom/static/", "/levels/GadgetRoom0_map_0.mpb", "/MissionObjects/M2C1/scripted/", "/scripts/M5C1_GadgetRoom_NPC_Dot.lua", "/scripts/LocationGadgetRoom_BoomBox.lua", "/scripts/LocationGadgetRoom1_Balloon.lua", "/scripts/M5C1_GadgetRoom_Items_Robots.lua", "/scripts/Doors_GadgetRoom0_GadgetRoomDoor2HQ.lua", "/UI/Zoom/", "/NPC/Dot/", "/NPC/Puffles/Blue/", "/MissionObjects/M5C1/", "/Objects/CommandCoach/", "/Location/GadgetRoom/touch/", "/Location/GadgetRoom/state/", "/Location/GadgetRoom/static/", "/levels/GadgetRoom1_map_0.mpb", "/scripts/M3C3_GarysRoom_NPC_Gary.lua", "/scripts/M3C3_GarysRoom_Objects_Model.lua", "/scripts/M2C2_GarysRoom0_ItemsM2C2_MineMap.lua", "/scripts/M2C2_GarysRoom0_ItemsM2C2_Notepad.lua", "/scripts/Doors_GarysRoom0_GarysRoomDoor2Sport.lua", "/scripts/Doors_GarysRoom0_GarysRoomDoor2Sport.lua", "/scripts/M2C2_GarysRoom0_ItemsM2C2_EncodedPlans.lua", "/NPC/Gary/", "/NPC/Puffles/Blue/", "/MissionObjects/M3C3/", "/Objects/CommandCoach/", "/Location/GarysRoom/touch/", "/Location/GarysRoom/state/", "/Location/GarysRoom/static/", "/levels/GarysRoom0_map_0.mpb", "/MissionObjects/M2C2/scripted/", "/scripts/M4C2_GiftOffice_ItemsM4C2_ComputerParts.lua", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M4C2/", "/Objects/CommandCoach/", "/Location/GiftOffice/state/", "/Location/GiftOffice/static/", "/levels/GiftOffice0_map_0.mpb", "/scripts/M5C1_GiftRoof_NPC_Gary.lua", "/scripts/M4C2_GiftRoof_NPC_WheelBot.lua", "/scripts/M4C2_GiftRoof_NPC_WheelBot.lua", "/scripts/M4C2_GiftRoof_NPC_WheelBot.lua", "/scripts/M4C2_GiftRoof_NPC_WheelBot.lua", "/scripts/M5C1_GiftRoof_NPC_SuperRobot.lua", "/scripts/Doors_GiftRoof0_RoofDoor2Office.lua", "/scripts/M4C2_GiftRoof0_CommandCoach_SpecialItem.lua", "/scripts/M5C1_GiftRoof0_CommandCoach_SpecialItem.lua", "/NPC/WheelBot/", "/3d/SuperRobot/", "/NPC/SuperRobot/SuperRobotRoofG", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M4C2/", "/Objects/CommandCoach/", "/Location/GiftRoof/state/", "/Location/GiftRoof/static/", "/levels/GiftRoof0_map_0.mpb", "/fonts/Comicrazy.fnt", "/bg/WifiContent/DGamerTop", "/bg/WifiContent/DGamerPoll", "/scripts/FPM10_NPC_Puffle5.lua", "/scripts/M4C2_GiftShop_NPC_WheelBot.lua", "/scripts/M4C2_GiftShop_NPC_WheelBot.lua", "/scripts/Doors_GiftShop0_GiftDoor2Town.lua", "/scripts/Doors_GiftShop0_GiftDoor2Office.lua", "/scripts/M5C1_GiftShop_NPC_GiftShopClerk.lua", "/scripts/FP00_GiftShop0_NPC_GiftShopOwner.lua", "/scripts/Doors_GiftShop0_GiftShopDoor2Office.lua", "/Objects/", "/NPC/WheelBot/", "/NPC/Puffles/Blue/", "/NPC/Puffles/Pink/", "/NPC/GiftShopOwner/", "/MissionObjects/M4C2/", "/Objects/CommandCoach/", "/Location/GiftShop/state/", "/Objects/Seasonal/Summer/", "/Location/SportShop/touch/", "/levels/GiftShop0_map_0.mpb", "/Objects/Seasonal/AprilFools/", "/scripts/M5C1_HQ_NPC_PH.lua", "/scripts/M5C1_HQ_NPC_JPG.lua", "/scripts/M5C1_HQ_NPC_Rookie.lua", "/scripts/HeadQuarters0_CodeBoard.lua", "/scripts/M4C3_Headquarters_NPC_Gary.lua", "/scripts/M1C2_HeadQuarters0_NPC_Dot.lua", "/scripts/FP10_HeadQuarters_NPC_Gary.lua", "/scripts/M2C1_HeadQuarters0_NPC_Rookie.lua", "/scripts/Doors_HeadQuarters0_HQDoor2Sport.lua", "/scripts/M2C1_HeadQuarter0_NPC_EyeScan3000.lua", "/scripts/Doors_HeadQuarters0_HQDoor2Gadget.lua", "/scripts/Doors_HeadQuarters0_HQDoor2Gadget.lua", "/scripts/Doors_HeadQuarters0_HQDoor2Command.lua", "/scripts/FP00_HeadQuarters0_NPC_FreeplayNPC3.lua", "/scripts/M1C2_HeadQuarters0_ItemsM1C2_HQMicroGameDoor.lua", "/NPC/Dot/", "/NPC/Gary/", "/NPC/Agent/", "/NPC/Rookie/", "/NPC/JetPackGuy/", "/NPC/Puffles/Blue/", "/NPC/PuffleHandler/", "/Objects/CommandCoach/", "/Location/HeadQuarters/state/", "/Location/HeadQuarters/touch/", "/Location/HeadQuarters/script/", "/Location/HeadQuarters/static/", "/levels/HeadQuarters0_map_0.mpb", "/scripts/M3C1_IceRink0_NPC_FanNPC.lua", "/scripts/M3C1_IceRink0_NPC_Goalie.lua", "/scripts/FPM08_IceRink_Items_Goal.lua", "/scripts/M3C1_IceRink0_NPC_Goalie.lua", "/scripts/FPM08_IceRink_NPC_Goalie.lua", "/scripts/FPM08_IceRink_NPC_Goalie.lua", "/scripts/M3C1_IceRink0_NPC_LoopNPC.lua", "/scripts/SeasFPM02_Item_FlowerHigh4.lua", "/scripts/FPM08_IceRink_NPC_LostMitten.lua", "/scripts/FPM08_IceRink_NPC_LostMitten.lua", "/scripts/Doors_IceRink_IceDoor2SnowForts.lua", "/scripts/FPM09_IceRink_Items_IcerinkDoor.lua", "/scripts/M2C1_IceRink0_CommandCoach_SpecialItem.lua", "/NPC/Fan/", "/NPC/Goalie/", "/NPC/LostMittens/", "/NPC/Puffles/Blue/", "/NPC/Puffles/Pink/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/Location/IceRink/state/", "/levels/IceRink0_map_0.mpb", "/MissionObjects/FreeplayM8/", "/MissionObjects/SeasonalM2/", "/MissionObjects/FreeplayM9/", "/Objects/Seasonal/Halloween/", "/Objects/Seasonal/AprilFools/", "/Objects/", "/scripts/FPM06_Dock_Items_Boat3.lua", "/scripts/FPM09_Iceberg_Items_Ice.lua", "/scripts/FPM04_Dock_NPC_DockNPCLook.lua", "/scripts/FPM08_Iceberg_Items_SnowPile.lua", "/scripts/Iceberg0_CommandCoach_SpecialItem.lua", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Objects/WorldItems/", "/Objects/CommandCoach/", "/NPC/GenericNPC/Yellow/", "/Objects/Seasonal/Summer/", "/levels/Iceberg0_map_0.mpb", "/levels/Iceberg0_map_1.mpb", "/MissionObjects/FreeplayM9/", "/MissionObjects/FreeplayM8/", "/Objects/Seasonal/AprilFools/", "/scripts/M2C1_Items_NutsBolts.lua", "/scripts/M1C1_Lighthouse0_NPC_DotBarrel.lua", "/scripts/M1C1_Lighthouse0_NPC_DotBarrel.lua", "/scripts/FPM04_Lighthouse_NPC_GuitarGuy.lua", "/scripts/Doors_Lighthouse0_LighthouseDoor2Beach.lua", "/scripts/Doors_Lighthouse0_LighthouseDoor2Beacon.lua", "/NPC/Dot/", "/NPC/GuitarGuy/", "/NPC/Puffles/Blue/", "/Objects/WorldItems/", "/MissionObjects/M5C1/", "/Objects/CommandCoach/", "/Location/Lighthouse/touch/", "/Location/Lighthouse/state/", "/Objects/Seasonal/AprilFools/", "/levels/Lighthouse0_map_0.mpb", "/scripts/SeasFPM01_NPC_BlazerX.lua", "/scripts/SeasFPM01_NPC_BlazerX.lua", "/scripts/FPM09_Lodge_Items_Fire.lua", "/scripts/M1C3_Lodge0_NPC_SickNPC.lua", "/scripts/M1C3_Lodge0_NPC_SickNPC.lua", "/scripts/Doors_Lodge0_LodgeDoor2Ski.lua", "/scripts/Doors_Lodge0_LodgeDoor2Fish.lua", "/scripts/Doors_Lodge0_LodgeDoor2Attic.lua", "/scripts/Doors_Lodge0_LodgeDoor2Attic.lua", "/scripts/M1C3_Lodge0_ItemsM1C3_Proboard.lua", "/NPC/BlazerX/", "/NPC/SickPenguin/", "/NPC/Puffles/Blue/", "/Objects/CommandCoach/", "/Location/Lodge/state/", "/Location/Lodge/touch/", "/Location/Lodge/static/", "/levels/Lodge0_map_0.mpb", "/MissionObjects/FreeplayM9/", "/Objects/Seasonal/AprilFools/", "/MissionObjects/M1C3/scripted/", "/scripts/FPM10_NPC_Puffle7.lua", "/scripts/FP00_Lounge0_NPC_FreeplayNPC5.lua", "/NPC/Puffles/Blue/", "/NPC/Puffles/Purple/", "/Objects/CommandCoach/", "/Location/Lounge/state/", "/levels/Lounge0_map_0.mpb", "/NPC/GenericNPC/Freeplay/", "/Objects/Seasonal/AprilFools/", "/scripts/M2C3_Crash_Coach_Prints.lua", "/scripts/M2C3_Crash_Coach_Prints.lua", "/scripts/M2C3_MineCrashSite0_NPC_GaryBump.lua", "/scripts/M2C3_MineCrashSite0_ItemsM2C3_Geyser.lua", "/scripts/M2C3_MineCrash0_ItemsM2C3_BalloonFake.lua", "/scripts/M2C3_MineCrashSite0_ItemsM2C3_Boulder.lua", "/scripts/M2C3_MineCrashSite0_NPC_GaryUnderCart.lua", "/scripts/M2C3_MineCrash0_ItemsM2C3_CrashedCart.lua", "/scripts/M2C3_MineCrash0_ItemsM2C3_BalloonFake.lua", "/scripts/M2C3_MineCrash0_ItemsM2C3_BalloonFake.lua", "/scripts/M2C3_MineCrashSite0_ItemsM2C3_CartRope.lua", "/scripts/Doors_MineCrash0_MineCrashDoor2MineLair.lua", "/scripts/M2C3_MineCrashSite0_ItemsM2C3_Flashlight.lua", "/scripts/Doors_MineCrash0_MineCrashDoor2MineInterior.lua", "/NPC/Gary/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/levels/MineCrash0_map_0.mpb", "/MissionObjects/M2C3/scripted/", "/Location/MineCrashSite/state/", "/Objects/CommandCoach/footprints/BotFPs/", "/scripts/FPM10_NPC_Puffle3.lua", "/scripts/M2C3_Mine_Coach_Prints.lua", "/scripts/M2C3_Mine_Coach_Prints.lua", "/scripts/M2C3_Mine_Coach_Prints.lua", "/scripts/M2C3_Mine_Coach_Prints.lua", "/scripts/M2C3_Mine_Coach_Prints.lua", "/scripts/M2C3_Mine_Coach_Prints.lua", "/scripts/SeasFPM02_Item_FlowerHigh2.lua", "/scripts/FPM09_MineInterior_Items_Lamp.lua", "/scripts/FP02_MineInterior0_NPC_NoNameNPC.lua", "/scripts/FPM07_MineInterior_Items_CodeMine.lua", "/scripts/FPM07_MineInterior_NPC_ConfusedNPC.lua", "/scripts/M3C1_MineInterior0_NPC_DotWithClue.lua", "/scripts/M3C1_MineInterior0_DeflatedInnerTube.lua", "/scripts/Doors_MineInterior0_MineInteriorDoor2Pool.lua", "/scripts/Doors_MineInterior0_MineInteriorDoor2Mission.lua", "/scripts/Doors_MineInterior0_MineInteriorDoor2Minigame.lua", "/scripts/FP00_MineInterior0_LocationMineInterior_MineCarts.lua", "/scripts/FP00_MineInterior0_LocationMineInterior_MineCarts.lua", "/scripts/FP00_MineInterior0_LocationMineInterior_MineCarts.lua", "/scripts/Doors_MineInterior0_MineInteriorDoor2MineExterior.lua", "/NPC/Dot/", "/NPC/MinesNPC/", "/NPC/ConfusedNPC/", "/NPC/Puffles/Blue/", "/NPC/Puffles/Black/", "/UI/InventoryPanel/", "/MissionObjects/M3C1/", "/Objects/CommandCoach/", "/MissionObjects/FreeplayM9/", "/MissionObjects/SeasonalM2/", "/Location/MineInterior/state/", "/Location/MineInterior/static/", "/levels/MineInterior0_map_0.mpb", "/MissionObjects/FreeplayM7/scripted/", "/Objects/CommandCoach/footprints/GaryMineFPs/", "/scripts/M2C3_MineTunnelExit_NPC_GaryBump.lua", "/scripts/Doors_MineLair0_MineLairDoor2Exit.lua", "/scripts/M2C3_MineTunnelExit_ItemsM2C3_Barrier.lua", "/scripts/M2C3_MineLair0_CommandCoach_SpecialItem.lua", "/NPC/Gary/", "/NPC/Puffles/Blue/", "/Objects/CommandCoach/", "/levels/MineLair0_map_0.mpb", "/MissionObjects/M2C3/scripted/", "/Location/MineTunnelExit/state/", "/fonts/faceFront.fnt", "/flc/creditsCrack.flc", "/bg/Cutscenes/BlankBlack", "/bg/Cutscenes/M5_STAGE_CREDIT6", "/bg/Cutscenes/M5_STAGE_CREDIT8", "/bg/Cutscenes/M5_STAGE_CREDIT2", "/bg/Cutscenes/M5_STAGE_CREDIT4", "/bg/Cutscenes/M5_STAGE_CREDIT12", "/bg/Cutscenes/M5_STAGE_CREDIT10", "/scripts/M3C3_MineShack_NPC_Miner.lua", "/scripts/SeasFPM02_Item_FlowerMid4.lua", "/scripts/M3C3_MineShack_Items_SnowCat.lua", "/scripts/M2C3_MineExterior0_NPC_Miner.lua", "/scripts/FPM04_MineShack_NPC_MinerPuzzled.lua", "/scripts/FP02_MineExterior0_NPC_NoNameNPC.lua", "/scripts/M3C3_MineShack_Objects_SnowStash.lua", "/scripts/M3C3_MineShack_Objects_CocoaTrail.lua", "/scripts/Doors_MineShack0_MineShackDoor2MineShed.lua", "/scripts/M2C2_MineShack0_CommandCoach_SpecialItem.lua", "/scripts/Doors_MineShack0_MineShackDoor2MineInterior.lua", "/scripts/Doors_MineShack0_MineShackDoor2MineInterior.lua", "/scripts/M2C3_MineExterior0_ItemsM2C3_TroughCart_23441.lua", "/scripts/M2C3_MineExterior0_ItemsM2C3_TroughWater_23318.lua", "/NPC/Miner/", "/NPC/MinesNPC/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M3C3/", "/MissionObjects/M3C1/", "/Objects/CommandCoach/", "/Objects/Seasonal/Xmas/", "/MissionObjects/SeasonalM2/", "/Objects/Seasonal/Halloween/", "/levels/MineShack0_map_0.mpb", "/MissionObjects/M2C3/static/", "/Location/MineExterior/state/", "/Location/MineExterior/static/", "/MissionObjects/M2C3/scripted/", "/scripts/M2C3_MineShed0_ItemsM2C3_SpoolWheels.lua", "/scripts/M2C3_MineShed0_ItemsM2C3_PulleyWheel1.lua", "/scripts/M2C3_MineShed0_ItemsM2C3_PulleyWheel2.lua", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/Location/MineShed/state/", "/levels/MineShed0_map_0.mpb", "/scripts/M3C1_NightClub0_NPC_DJ.lua", "/scripts/M4C1_NightClub_NPC_Dot.lua", "/scripts/FP00_NightClub0_NPC_DJ.lua", "/scripts/M4C1_NightClub_NPC_Dot.lua", "/scripts/M3C1_NightClub0_NPC_DJ.lua", "/scripts/M4C1_NightClub_NPC_Rory.lua", "/scripts/M4C1_NightClub_Items_Hole.lua", "/scripts/M3C1_NightClub0_NPC_PopNPC.lua", "/scripts/FP00_NightClub0_NPC_Dancer.lua", "/scripts/M3C1_NightClub0_NPC_DancerNPC.lua", "/scripts/SM03_NightClub_NPC_GDisguised.lua", "/scripts/SM03_NightClub_NPC_GDisguised.lua", "/scripts/M3C1_NightClub0_NPC_DancerNPC.lua", "/scripts/SM03_NightClub_NPC_AADisguised.lua", "/scripts/SM03_NightClub_NPC_AADisguised.lua", "/scripts/SM03_NightClub_NPC_JPGDisguised.lua", "/scripts/SM03_NightClub_NPC_JPGDisguised.lua", "/scripts/SM03_NightClub_NPC_DotDisguised.lua", "/scripts/M4C1_NightClub_Items_SnowTracks.lua", "/scripts/M4C1_NightClub_Items_SnowTracks.lua", "/scripts/M4C1_NightClub_Items_SnowTracks.lua", "/scripts/M4C1_NightClub_Items_SnowTracks.lua", "/scripts/M4C1_NightClub_Items_SnowTracks.lua", "/scripts/M4C1_NightClub_Items_SnowTracks.lua", "/scripts/M4C1_NightClub_Items_SnowTracks.lua", "/scripts/M4C1_NightClub_Items_SnowTracks.lua", "/scripts/M4C1_NightClub_Items_SnowTracks.lua", "/scripts/M4C1_NightClub_Items_SnowTracks.lua", "/scripts/M4C1_NightClub_Items_SnowTracks.lua", "/scripts/M4C1_NightClub_Items_SnowTracks.lua", "/scripts/M4C1_NightClub_Items_SnowTracks.lua", "/scripts/SM03_NightClub_NPC_DotDisguised.lua", "/scripts/M4C1_NightClub_Items_SnowTracks.lua", "/scripts/M4C1_NightClub_Items_SnowTracks.lua", "/scripts/Doors_NightClub0_NightDoor2Town.lua", "/scripts/Doors_NightClub0_NightDoor2Lounge.lua", "/scripts/Doors_NightClub0_NightDoor2Lounge.lua", "/scripts/Doors_NightClub0_NightDoor2Boiler.lua", "/scripts/SM03_NightClub_NPC_RookieDisguised.lua", "/scripts/M4C1_NightClub_Items_BucketofClues.lua", "/scripts/SM03_NightClub_NPC_RookieDisguised.lua", "/NPC/DJ/", "/NPC/Dot/", "/NPC/Rory/", "/NPC/Dancer/", "/NPC/GDisguised/", "/NPC/AADisguised/", "/NPC/DotDisguised/", "/NPC/Puffles/Blue/", "/NPC/JPGDisguised/", "/UI/InventoryPanel/", "/NPC/Puffles/Purple/", "/NPC/RookieDisguised/", "/MissionObjects/M4C1/", "/Objects/CommandCoach/", "/Objects/Seasonal/Summer/", "/Location/NightClub/state/", "/Location/NightClub/static/", "/Objects/Seasonal/Halloween/", "/levels/NightClub0_map_0.mpb", "/Objects/Seasonal/AprilFools/", "/scripts/FPM10_PetShop_NPC_PuffleOwner.lua", "/scripts/FP00_PetShop0_NPC_PetShopClerk.lua", "/scripts/M2C2_PetShop0_NPC_PetShopClerk.lua", "/scripts/FPM10_PetShop_NPC_PetShopClerk.lua", "/scripts/Doors_PetShop0_PetShopDoor2Employees.lua", "/NPC/PuffleOwner/", "/NPC/Puffles/Blue/", "/NPC/PetShopClerk/", "/Objects/CommandCoach/", "/Location/PetShop/touch/", "/Location/PetShop/state/", "/Location/PetShop/static/", "/Objects/Seasonal/Summer/", "/levels/PetShop0_map_0.mpb", "/Objects/Seasonal/AprilFools/", "/scripts/M2C1_PizzaShop0_NPC_PizzaChef.lua", "/scripts/FPM04_PizzaParlor_NPC_PizzaChef.lua", "/scripts/FPM05_PizzaParlor_NPC_PizzaChef.lua", "/scripts/M1C3_PizzaParlor0_NPC_PizzaChef.lua", "/scripts/FPM04_PizzaParlor_Items_PizzaSquidFake.lua", "/scripts/FPM04_PizzaParlor_Items_PizzaDessertFake.lua", "/scripts/M1C3_PizzaParlor0_ItemsM1C3_TableThermos.lua", "/scripts/FPM04_PizzaParlor_Items_PizzaSeaWeedFake.lua", "/scripts/FPM04_PizzaParlor_Items_PizzaHotSauceFake.lua", "/scripts/Doors_PizzaParlor0_PizzaParlorDoor2Kitchen.lua", "/scripts/FP00_PizzaParlor0_NPC_PizzaChefBehindCounter.lua", "/NPC/PizzaChef/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Location/PizzaShop/", "/Objects/CommandCoach/", "/Objects/Seasonal/Xmas/", "/Objects/Seasonal/Summer/", "/Location/PizzaShop/touch/", "/Location/PizzaShop/state/", "/Location/PizzaShop/static/", "/Objects/Seasonal/Halloween/", "/Objects/Seasonal/AprilFools/", "/levels/PizzaParlor0_map_0.mpb", "/MissionObjects/FreeplayM4/scripted/", "/scripts/M5C1_Plaza_NPC_Agent.lua", "/scripts/M4C1_Plaza_NPC_Rookie.lua", "/scripts/M4C1_Plaza_NPC_Rookie.lua", "/scripts/M2C1_Plaza_Coach_Prints.lua", "/scripts/M2C1_Plaza_Coach_Prints.lua", "/scripts/M2C1_Plaza_Coach_Prints.lua", "/scripts/M2C1_Plaza_Coach_Prints.lua", "/scripts/M2C1_Plaza_Coach_Prints.lua", "/scripts/M4C1_Plaza_Coach_Prints.lua", "/scripts/M2C1_Plaza_Coach_Prints.lua", "/scripts/M4C1_Plaza_Coach_Prints.lua", "/scripts/M5C1_Plaza_NPC_PizzaChef.lua", "/scripts/SeasFPM02_Item_FlowerLow.lua", "/scripts/FP00_Plaza0_NPC_TourGuide.lua", "/scripts/FPM08_Plaza_NPC_LostMitten.lua", "/scripts/FPM08_Plaza_NPC_LostJacket.lua", "/scripts/FPM08_Plaza_NPC_LostMitten.lua", "/scripts/Doors_Plaza0_PlazaDoor2Pet.lua", "/scripts/M1C1_Plaza0_NPC_DotMailbox.lua", "/scripts/FPM08_Plaza_NPC_LostJacket.lua", "/scripts/M1C1_Plaza0_NPC_DotMailbox.lua", "/scripts/M5C1_Plaza_NPC_PetShopClerk.lua", "/scripts/Doors_Plaza0_PlazaDoor2Pool.lua", "/scripts/M4C1_Plaza_Items_Jackhammer.lua", "/scripts/M4C1_Plaza_Items_RookieClue.lua", "/scripts/Doors_Plaza0_PlazaDoor2Pizza.lua", "/scripts/SeasFPM02_Plaza_NPC_AuntArctic.lua", "/scripts/Doors_Plaza0_PlazaDoor2Wilderness.lua", "/scripts/Doors_Plaza0_PlazaDoor2TheaterLeft.lua", "/scripts/Doors_Plaza0_PlazaDoor2TheaterLeft.lua", "/scripts/Doors_Plaza0_PlazaDoor2TheaterRight.lua", "/scripts/Doors_Plaza0_PlazaDoor2TheaterRight.lua", "/scripts/M3C1_Plaza0_CommandCoach_SpecialItem.lua", "/NPC/Dot/", "/NPC/Agent/", "/NPC/Rookie/", "/NPC/PizzaChef/", "/NPC/TourGuide/", "/NPC/AuntArctic/", "/NPC/LostMittens/", "/NPC/Puffles/Blue/", "/NPC/PetShopClerk/", "/UI/InventoryPanel/", "/MissionObjects/M4C1/", "/Location/Plaza/state/", "/NPC/NPCMissingJacket/", "/Objects/CommandCoach/", "/Objects/Seasonal/Xmas/", "/Location/Plaza/static/", "/levels/Plaza0_map_0.mpb", "/Objects/Seasonal/Summer/", "/MissionObjects/SeasonalM2/", "/Objects/Seasonal/Halloween/", "/Objects/Seasonal/AprilFools/", "/Objects/CommandCoach/footprints/", "/Objects/CommandCoach/footprints/BotFPs/", "/scripts/Doors_Lock_Generic.lua", "/scripts/FPM04_Pool_NPC_PartyNPC.lua", "/scripts/Doors_Pool0_PoolDoor2Boiler.lua", "/scripts/M3C1_Pool0_ItemsM3C1_WaterTrail.lua", "/scripts/M3C1_Pool0_ItemsM3C1_WaterTrail.lua", "/scripts/M3C1_Pool0_ItemsM3C1_WaterTrail.lua", "/scripts/M3C1_Pool0_ItemsM3C1_WaterTrail.lua", "/scripts/M3C1_Pool0_ItemsM3C1_WaterTrail.lua", "/scripts/M3C1_Pool0_ItemsM3C1_WaterTrail.lua", "/scripts/M3C1_Pool0_ItemsM3C1_WaterTrail.lua", "/scripts/Doors_Pool0_PoolDoor2MineInterior.lua", "/NPC/PoolNPC/", "/NPC/Puffles/Blue/", "/MissionObjects/M3C1/", "/Location/Pool/state/", "/Objects/CommandCoach/", "/levels/Pool0_map_0.mpb", "/Objects/Seasonal/AprilFools/", "/scripts/M3C2_PuffleTraining0_NPC_PH.lua", "/scripts/M2C2_PuffleTraining0_NPC_Flare_Sad.lua", "/scripts/M3C2_PuffleTraining0_ObjectsM3C2_Dome.lua", "/scripts/M3C2_PuffleTraining0_ObjectsM3C2_Jack.lua", "/scripts/M3C1_PuffleTraining0_ItemsM3C1_Weight.lua", "/scripts/M3C1_PuffleTraining0_ItemsM3C1_Bamboo.lua", "/scripts/M3C2_PuffleTraining0_ObjectsM3C2_Jack.lua", "/scripts/M3C2_PuffleTraining0_ObjectsM3C2_Jack.lua", "/scripts/M3C1_PuffleTraining0_ItemsM3C1_Jetpack.lua", "/scripts/M3C1_PuffleTraining0_ItemsM3C1_Jetpack.lua", "/scripts/M1C2_PuffleTraining0_NPC_PuffleHandler.lua", "/scripts/M3C2_PuffleTraining0_ObjectsM3C2_Crate.lua", "/scripts/M2C2_PuffleTraining0_NPC_PuffleHandler.lua", "/scripts/M3C2_PuffleTraining0_ObjectsM3C2_Weight.lua", "/scripts/M3C2_PuffleTraining0_ObjectsM3C2_Weight.lua", "/scripts/M2C2_PuffleTraining0_ItemsM2C2_KeyWelded.lua", "/scripts/M3C2_PuffleTraining0_ObjectsM3C2_Balloon.lua", "/scripts/M1C2_PuffleTraining0_NPC_DirectorMonitor.lua", "/scripts/M3C2_PuffleTraining0_ObjectsM3C2_IceChest.lua", "/scripts/M3C2_PuffleTraining0_ObjectsM3C2_FireChest.lua", "/scripts/M2C2_PuffleTraining0_ItemsM2C2_PostCardPen.lua", "/scripts/M1C2_PuffleTraining0_ItemsM1C2_BrokenCrate.lua", "/scripts/M1C2_PuffleTraining0_ItemsM1C2_PuffleCrate.lua", "/scripts/M3C2_PuffleTraining0_ObjectsM3C2_OpenChest.lua", "/scripts/M3C1_PuffleTraining0_NPC_PuffleHandlerBubble.lua", "/scripts/M3C1_PuffleTraining0_NPC_PuffleHandlerBubble.lua", "/scripts/M3C2_PuffleTraining0_ObjectsM3C2_NormalChest.lua", "/scripts/FP06_PuffleTraining0_NPC_PuffleHandlerBubble.lua", "/scripts/M3C1_PuffleTraining0_NPC_PuffleHandlerBubble.lua", "/scripts/M3C1_PuffleTraining0_NPC_PuffleHandlerBubble.lua", "/scripts/M2C2_PuffleTraining0_ItemsM2C2_AnvilKeyBroken.lua", "/scripts/Doors_PuffleTraining0_PuffleTrainingDoor2Dojo.lua", "/NPC/Director/", "/NPC/Puffles/Red/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/NPC/PuffleHandler/", "/NPC/Puffles/Black/", "/MissionObjects/M5C1/", "/MissionObjects/M3C1/", "/MissionObjects/M3C2/", "/Objects/CommandCoach/", "/MissionObjects/M2C2/static/", "/MissionObjects/M2C2/scripted/", "/Location/CommandRoom/scripted/", "/Location/PuffleTraining/state/", "/Location/PuffleTraining/script/", "/Location/PuffleTraining/static/", "/levels/PuffleTraining0_map_0.mpb", "/scripts/M4C3_SkiHill_NPC_Dot.lua", "/scripts/SeasFPM04_Item_Coconut.lua", "/scripts/SeasFPM04_Item_EmptyTree.lua", "/scripts/M4C3_SkiHill_NPC_SnowBot.lua", "/scripts/M1C3_SkiHill0_NPC_BlazerX.lua", "/scripts/SeasFPM02_Item_FlowerMid2.lua", "/scripts/FPM08_SkiHill_Items_Mitten.lua", "/scripts/FPM07_SkiHill_Items_CodeSki.lua", "/scripts/M2C1_Mountain_Items_SnowPile.lua", "/scripts/FPM07_SkiHill_NPC_ConfusedNPC.lua", "/scripts/SeasFPM04_SkiHill_Item_PalmTree.lua", "/scripts/Doors_SkiHill0_SkiHillDoor2Test.lua", "/scripts/M4C3_SkiHill_ItemsM4C3_ChairLift.lua", "/scripts/World_SkiHill_NPC_SickPenguinGrumpy.lua", "/scripts/Doors_SkiHill0_SkiHillDoor2SkiVillage.lua", "/scripts/M4C1_SkiHill0_CommandCoach_SpecialItem.lua", "/NPC/Dot/", "/NPC/BlazerX/", "/NPC/SnowBot/", "/NPC/ConfusedNPC/", "/NPC/SickPenguin/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M4C3/", "/Objects/CommandCoach/", "/Objects/Seasonal/Xmas/", "/Location/Mountain/state/", "/Location/Mountain/static/", "/levels/SkiHill0_map_0.mpb", "/MissionObjects/SeasonalM2/", "/MissionObjects/SeasonalM4/", "/MissionObjects/M1C3/static/", "/Objects/Seasonal/Halloween/", "/MissionObjects/M2C1/scripted/", "/MissionObjects/FreeplayM7/scripted/", "/scripts/M4C3_SkiVillage_NPC_Dot.lua", "/scripts/M5C1_SkiVillage_NPC_Gary.lua", "/scripts/SeasFPM02_Item_FlowerMid.lua", "/scripts/M2C1_Village_Coach_Prints.lua", "/scripts/M2C1_Village_Coach_Prints.lua", "/scripts/M2C1_Village_Coach_Prints.lua", "/scripts/M2C1_Village_Coach_Prints.lua", "/scripts/FP01_SkiVillage0_NPC_Rory.lua", "/scripts/M5C1_SkiVillage_NPC_Flare.lua", "/scripts/M2C1_Village_Coach_Prints.lua", "/scripts/M2C1_Village_Coach_Prints.lua", "/scripts/M2C1_Village_Coach_Prints.lua", "/scripts/M5C1_SkiVillage_NPC_Rookie.lua", "/scripts/M5C1_SkiVillage_NPC_Rookie.lua", "/scripts/M4C3_SkiVillage_NPC_Rookie.lua", "/scripts/M4C3_SkiVillage_NPC_Chase11.lua", "/scripts/M4C3_SkiVillage_NPC_BlazerX.lua", "/scripts/M4C3_SkiVillage_NPC_AuntArctic.lua", "/scripts/M5C1_SkiVillage_NPC_SuperRobot.lua", "/scripts/M2C1_SkiVillage0_NPC_DotSnowbank.lua", "/scripts/M1C2_SkiVillage0_NPC_DotTourGuide.lua", "/scripts/FP01_SkiVillage0_ItemsFP01_Thermos.lua", "/scripts/M1C3_SkiVillage0_NPC_DotSnowPenguin.lua", "/scripts/FP02_SkiVillage0_NPC_DotSnowPenguin.lua", "/scripts/M4C3_SkiVillage_ItemsM4C3_ChairLift.lua", "/scripts/Doors_SkiVillage0_SkiVillageDoor2Dock.lua", "/scripts/Doors_SkiVillage0_SkiVillageDoor2Beach.lua", "/scripts/Doors_SkiVillage0_SkiVillageDoor2Sport.lua", "/scripts/Doors_SkiVillage0_SkiVillageDoor2SkiHill.lua", "/scripts/Doors_SkiVillage0_SkiVillageDoor2SkiLodge.lua", "/NPC/Dot/", "/NPC/Rory/", "/NPC/Rookie/", "/NPC/BlazerX/", "/3d/SuperRobot/", "/NPC/SuperRobot/", "/NPC/AuntArctic/", "/NPC/SickPenguin/", "/NPC/Puffles/Blue/", "/NPC/Puffles/Black/", "/UI/InventoryPanel/", "/MissionObjects/M5C1/", "/MissionObjects/M4C3/", "/Objects/CommandCoach/", "/Objects/Seasonal/Xmas/", "/Objects/Seasonal/Summer/", "/MissionObjects/SeasonalM2/", "/Location/SkiVillage/touch/", "/Location/SkiVillage/state/", "/Location/SkiVillage/static/", "/Objects/Seasonal/Halloween/", "/Objects/Seasonal/AprilFools/", "/levels/SkiVillage0_map_0.mpb", "/MissionObjects/M1C3/scripted/", "/Objects/CommandCoach/footprints/", "/fonts/faceFront.fnt", "/bg/Menus/Menus_bottom", "/UI/HelpMenus/StylusDrag", "/bg/HelpMenus/StylusDrag_bottom", "/scripts/M4C3_Slopes_NPC_SnowBot.lua", "/scripts/M4C3_Slopes_NPC_SnowBotRoped.lua", "/NPC/SnowBot/", "/NPC/Puffles/Blue/", "/MissionObjects/M4C3/", "/levels/SlopeBattle0_map_0.mpb", "/levels/SlopeBattle0_map_1.mpb", "/levels/SlopeBattle0_map_2.mpb", "/scripts/SeasFPM01_NPC_Agent.lua", "/scripts/M5C1_IceFort_NPC_Agent.lua", "/scripts/FPM09_SnowForts_NPC_Rory.lua", "/scripts/SeasFPM02_Item_FlowerLow3.lua", "/scripts/SeasFPM02_Item_FlowerHigh5.lua", "/scripts/M3C1_Snowforts_Coach_Prints.lua", "/scripts/M3C1_Snowforts_Coach_Prints.lua", "/scripts/M3C1_Snowforts_Coach_Prints.lua", "/scripts/M3C1_Snowforts_Coach_Prints.lua", "/scripts/M3C1_Snowforts_Coach_Prints.lua", "/scripts/M3C1_Snowforts_Coach_Prints.lua", "/scripts/M3C1_Snowforts_Coach_Prints.lua", "/scripts/M3C1_Snowforts_Coach_Prints.lua", "/scripts/M1C1_SnowForts0_NPC_DotSnowman.lua", "/scripts/M1C1_SnowForts0_NPC_SnowFortNPC.lua", "/scripts/M1C1_SnowForts0_NPC_SnowFortNPC.lua", "/scripts/FPM05_SnowForts_NPC_SnowFortNPC2.lua", "/scripts/M2C2_SnowForts0_NPC_SnowFortNPC1.lua", "/scripts/M2C2_SnowForts0_NPC_SnowFortNPC2.lua", "/scripts/FPM05_SnowForts_NPC_SnowFortNPC1.lua", "/scripts/M1C1_SnowForts0_NPC_SnowFortNPC1a.lua", "/scripts/Doors_SnowForts0_SnowFortsDoor2Ice.lua", "/scripts/Doors_SnowForts0_SnowFortsDoor2Ice.lua", "/scripts/Doors_SnowForts0_SnowFortsDoor2Town.lua", "/scripts/Doors_SnowForts0_SnowFortsDoor2Town.lua", "/scripts/Doors_SnowForts0_SnowFortsDoor2Plaza.lua", "/scripts/Doors_SnowForts0_SnowFortsDoor2Plaza.lua", "/scripts/FPM05_SnowForts_Items_PizzaDessertFake.lua", "/scripts/M1C1_SnowForts0_ItemsM1C1_SnowFortsHat.lua", "/scripts/M1C1_SnowForts0_ItemsM1C1_HatGroundFake.lua", "/scripts/SnowForts0_LocationSnowForts_GearRightSide.lua", "/NPC/Dot/", "/NPC/Rory/", "/NPC/Agent/", "/NPC/Puffles/Blue/", "/NPC/SnowFortsNPC/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/Objects/Seasonal/Xmas/", "/Objects/Seasonal/Summer/", "/Location/SnowForts/touch/", "/Location/SnowForts/state/", "/MissionObjects/SeasonalM2/", "/Location/SnowForts/static/", "/levels/SnowForts0_map_0.mpb", "/Objects/Seasonal/Halloween/", "/Objects/Seasonal/AprilFools/", "/MissionObjects/M1C1/scripted/", "/MissionObjects/FreeplayM4/scripted/", "/Objects/CommandCoach/footprints/PuffleFPs/", "/scripts/M1C2_SportShop0_NPC_Dot.lua", "/scripts/M2C1_SportShop0_NPC_Dot.lua", "/scripts/FPM08_SkiShop_Items_Wardrobe.lua", "/scripts/FPM08_SkiShop_Items_Wardrobe.lua", "/scripts/FPM08_SkiShop_Items_Wardrobe.lua", "/scripts/FPM08_SkiShop_NPC_SkiShopClerk.lua", "/scripts/FPM08_SkiShop_NPC_SkiShopClerk.lua", "/scripts/FPM08_SportsShop_NPC_LostMitten.lua", "/scripts/FPM08_SportsShop_NPC_LostMitten.lua", "/scripts/M2C1_SportShop0_NPC_SportShopNPC.lua", "/scripts/M1C3_SportShop0_NPC_SportShopNPC.lua", "/scripts/FP00_SportShop0_NPC_SportShopNPC.lua", "/scripts/Doors_SportShop0_SportShopDoor2HQ.lua", "/scripts/Doors_SportShop0_SportShopDoor2Ski.lua", "/scripts/Doors_SportShop0_SportShopDoor2Garys.lua", "/scripts/Doors_SportShop0_SportShopDoor2Garys.lua", "/NPC/Dot/", "/Objects/", "/NPC/LostMittens/", "/NPC/SportShopNPC/", "/NPC/Puffles/Blue/", "/Objects/CommandCoach/", "/Objects/Seasonal/Summer/", "/Location/SportShop/state/", "/Location/SportShop/touch/", "/MissionObjects/FreeplayM8/", "/Location/SportShop/static/", "/levels/SportShop0_map_0.mpb", "/Objects/Seasonal/AprilFools/", "/MissionObjects/M2C3/scripted/", "/scripts/FPM10_NPC_Puffle2.lua", "/scripts/M3C1_Stage0_NPC_Rory.lua", "/scripts/M3C1_Stage0_NPC_Rory.lua", "/scripts/M3C1_Stage0_NPC_Manager.lua", "/scripts/M3C1_Stage0_NPC_Manager.lua", "/scripts/Doors_Stage0_Stage2Plaza.lua", "/scripts/M3C1_Stage0_ItemsM3C1_Trunk.lua", "/scripts/M3C1_Stage0_ItemsM3C1_JackHammer.lua", "/scripts/M3C1_Stage0_ItemsM3C1_JackHammer.lua", "/NPC/Rory/", "/NPC/Manager/", "/NPC/Puffles/Red/", "/NPC/Puffles/Blue/", "/MissionObjects/M3C1/", "/Location/Stage/state/", "/Objects/CommandCoach/", "/levels/Stage0_map_0.mpb", "/Objects/Seasonal/AprilFools/", "/scripts/M5C1_Stage_NPC_JPG.lua", "/scripts/M5C1_Stage_NPC_Dot.lua", "/scripts/M5C1_Stage_NPC_JPG.lua", "/scripts/M5C1_Stage_NPC_Dot.lua", "/scripts/M5C1_Stage_NPC_Rookie.lua", "/scripts/M5C1_Stage_NPC_Rookie.lua", "/scripts/Doors_Stage0_Stage2Plaza.lua", "/scripts/M5C1_Stage_NPC_PuffleHandler.lua", "/scripts/M5C1_Stage_NPC_PuffleHandler.lua", "/NPC/Dot/", "/NPC/Rookie/", "/NPC/JetPackGuy/", "/NPC/Puffles/Blue/", "/NPC/PuffleHandler/", "/Location/Stage/state/", "/Objects/CommandCoach/", "/Location/Stage/static/", "/levels/Stage1_map_0.mpb", "/scripts/M3C2_TALLESTM_NPC_PH.lua", "/scripts/M3C2_TALLESTM_NPC_PH.lua", "/scripts/M3C2_TALLESTM_NPC_PH.lua", "/scripts/M3C2_TALLESTM_NPC_JPG.lua", "/scripts/M3C2_TALLESTM_NPC_CHIRP.lua", "/scripts/M3C2_TALLESTM_Items_Rope.lua", "/scripts/M5C1_TallestMountain_Robot.lua", "/scripts/M5C1_TallestMountain_Robot.lua", "/scripts/M5C1_TallestMountain_Robot.lua", "/scripts/M5C1_TallestMountain_Robot.lua", "/scripts/M5C1_TallestM_Items_Jetpack.lua", "/scripts/M5C1_TallestM_Items_Jetpack.lua", "/scripts/M5C1_TallestMountain_NPC_Pop.lua", "/scripts/M5C1_TallestMountain_NPC_Gary.lua", "/scripts/M5C1_TallestMountain_NPC_Gary.lua", "/scripts/M3C2_TALLESTM_ItemsM3C2_PHHAT.lua", "/scripts/M3C2_TALLESTM_Object_ICEBLOCK.lua", "/scripts/M5C1_TallestMountain_NPC_Gary.lua", "/scripts/M3C2_TALLESTM_Objects_BALLOON.lua", "/scripts/M3C2_TALLESTM_Object_ICEBLOCK.lua", "/scripts/M5C1_TallestMountain_NPC_Chirp.lua", "/scripts/M3C2_TALLESTM_Objects_BALLOON3.lua", "/scripts/M3C2_TALLESTM_Objects_BALLOON2.lua", "/scripts/M4C3_TallestMountain_NPC_JetBot.lua", "/scripts/M4C3_TallestMountain_NPC_JetBot.lua", "/scripts/M4C3_TallestMountain_NPC_JetBot.lua", "/scripts/M4C3_TallestMountain_NPC_JetBot.lua", "/scripts/M4C3_TallestMountain_NPC_JetBot.lua", "/scripts/M4C3_TallestMountain_NPC_SnowBot.lua", "/scripts/M3C2_TALLESTM_ItemsM3C2_JETBACKPACK.lua", "/scripts/M3C2_TALLESTM_ItemsM3C2_JETBACKPACK.lua", "/scripts/M4C3_TallestMountain_ItemsM4C3_Bag1.lua", "/scripts/M4C3_TallestMountain_ItemsM4C3_Balloon1.lua", "/scripts/M4C3_TallestMountain_ItemsM4C3_Balloon2.lua", "/scripts/M4C3_TallestMountain_ItemsM4C3_Balloon3.lua", "/scripts/M3C2_TallestMountainTop0_CommandCoach_SpecialItem.lua", "/NPC/Gary/", "/NPC/JetBot/", "/NPC/SnowBot/", "/3d/SuperRobot/", "/NPC/SuperRobot/", "/NPC/JetPackGuy/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/NPC/PuffleHandler/", "/NPC/Puffles/Green/", "/NPC/Puffles/Yellow/", "/NPC/Puffles/Purple/", "/MissionObjects/M4C3/", "/MissionObjects/M3C1/", "/MissionObjects/M3C2/", "/MissionObjects/M5C1/", "/Objects/CommandCoach/", "/levels/TallestMountainTop0_map_1.mpb", "/levels/TallestMountainTop0_map_0.mpb", "/NPC/Puffles/Blue/", "/Objects/CommandCoach/", "/levels/TallestMountainTop1_map_0.mpb", "/levels/TallestMountainTop1_map_2.mpb", "/levels/TallestMountainTop1_map_1.mpb", "/scripts/M4C2_Town_NPC_Dot.lua", "/scripts/M4C2_Town_NPC_JPG.lua", "/scripts/FP10_Town_NPC_Dot.lua", "/scripts/M5C1_Town_NPC_Dot.lua", "/scripts/M5C1_Town_NPC_Gary.lua", "/scripts/M4C2_Town_NPC_Agent.lua", "/scripts/FP10_Town_NPC_Agent.lua", "/scripts/M3C1_Town_Coach_Prints.lua", "/scripts/M4C1_Town_Coach_Prints.lua", "/scripts/M3C1_Town_Coach_Prints.lua", "/scripts/M3C1_Town_Coach_Prints.lua", "/scripts/M3C1_Town0_NPC_Doorman.lua", "/scripts/M3C1_Town_Coach_Prints.lua", "/scripts/M4C1_Town_Coach_Prints.lua", "/scripts/M3C1_Town_Coach_Prints.lua", "/scripts/M3C1_Town_Coach_Prints.lua", "/scripts/M3C1_Town_Coach_Prints.lua", "/scripts/M3C1_Town_Coach_Prints.lua", "/scripts/SeasFPM04_Item_Coconut.lua", "/scripts/M3C1_Town_Coach_Prints.lua", "/scripts/M4C1_Town_Coach_Prints.lua", "/scripts/M4C1_Town_Coach_Prints.lua", "/scripts/M3C1_Town_Coach_Prints.lua", "/scripts/M3C1_Town_Coach_Prints.lua", "/scripts/M3C1_Town_Coach_Prints.lua", "/scripts/SeasFPM01_Town_NPC_Santa.lua", "/scripts/SeasFPM01_Town_Item_Gift.lua", "/scripts/FP10_Town_NPC_AuntArctic.lua", "/scripts/SeasFPM04_Item_EmptyTree.lua", "/scripts/FP10_Town_NPC_JetPackGuy.lua", "/scripts/M5C1_Town_NPC_SuperRobot.lua", "/scripts/M4C2_Town_NPC_AuntArctic.lua", "/scripts/FP00_Town0_NPC_AuntArctic.lua", "/scripts/M2C2_Town0_NPC_AuntArctic.lua", "/scripts/SeasFPM01_Town_Item_Gift2.lua", "/scripts/Doors_Town0_TownDoor2Dock.lua", "/scripts/SeasFPM01_Town_Item_Gift3.lua", "/scripts/SeasFPM01_Town_Item_Gift4.lua", "/scripts/M3C1_Town0_NPC_WaitingNPC.lua", "/scripts/Doors_Town0_TownDoor2Snow.lua", "/scripts/M4C2_Town_NPC_GiftShopNPC.lua", "/scripts/FP10_Town_NPC_GiftShopNPC.lua", "/scripts/FPM07_Town_Items_CodeTown.lua", "/scripts/SeasFPM02_Item_FlowerMid5.lua", "/scripts/SeasFPM02_Item_FlowerLow4.lua", "/scripts/Doors_Town0_TownDoor2Gift.lua", "/scripts/Doors_Town0_TownDoor2Gift.lua", "/scripts/FPM07_Town_NPC_ConfusedNPC.lua", "/scripts/Doors_Town0_TownDoor2Night.lua", "/scripts/Doors_Town0_TownDoor2Coffee.lua", "/scripts/SeasFPM04_Town_Item_PalmTree.lua", "/scripts/M2C2_Town0_ItemsM2C2_PostCard.lua", "/scripts/M4C2_Town_ItemsM4C2_Barricade.lua", "/scripts/M4C2_Town_ItemsM4C2_Barricade.lua", "/scripts/M1C1_Town0_NPC_CoinCountingNPC.lua", "/scripts/M4C2_Town_ItemsM4C2_BarricadeBubbled.lua", "/NPC/Dot/", "/NPC/Santa/", "/NPC/Rookie/", "/NPC/Bouncer/", "/3d/SuperRobot/", "/NPC/AuntArctic/", "/NPC/WaitingNPC/", "/NPC/SuperRobot/", "/NPC/JetPackGuy/", "/NPC/ConfusedNPC/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/NPC/GiftShopOwner/", "/NPC/CoinCountingNPC/", "/MissionObjects/M4C2/", "/Location/Town/state/", "/Location/Town/static/", "/Objects/CommandCoach/", "/Objects/Seasonal/Xmas/", "/levels/Town0_map_0.mpb", "/Objects/Seasonal/Summer/", "/MissionObjects/SeasonalM4/", "/MissionObjects/SeasonalM2/", "/Objects/Seasonal/Halloween/", "/Objects/Seasonal/AprilFools/", "/MissionObjects/FreeplayM7/scripted/", "/Objects/CommandCoach/footprints/BotFPs/", "/Objects/CommandCoach/footprints/PuffleFPs/", "/scripts/M1C3_WildernessBerry_Items_SnowTracks2.lua", "/scripts/M1C3_WildernessBerry_Items_SnowBarricade.lua", "/scripts/M1C3_WildernessBerry_Items_NibbledOberries.lua", "/scripts/M1C3_WildernessBerry0_CommandCoach_M1C3Shovel.lua", "/scripts/M1C3_WildernessBerry0_ItemsM1C3_SnowBarricadeShoveled.lua", "/scripts/M1C3_WildernessBerry_LocationWildernessBerry_OberryBush.lua", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/MissionObjects/M1C3/scripted/", "/Location/WildernessBerry/state/", "/Location/WildernessBerry/touch/", "/levels/WildernessBerry0_map_0.mpb", "/scripts/M3C3_WildernessCave_Objects_Cocoa.lua", "/scripts/M3C3_WildernessCave_Objects_Cocoa.lua", "/scripts/M3C3_WildernessCave_Objects_Cocoa.lua", "/scripts/M3C3_WildernessCave_Objects_Cocoa.lua", "/scripts/M3C3_WildernessCave_Objects_Cocoa.lua", "/scripts/M3C3_WildernessCave_Objects_Cocoa.lua", "/scripts/M3C3_WildernessCave_Objects_Tracks.lua", "/scripts/M3C3_WildernessCave_Objects_Tracks.lua", "/scripts/M3C3_WildernessCave_Objects_Tracks.lua", "/scripts/M3C3_WildernessCave_Objects_SnowCat.lua", "/scripts/M3C3_WildernessCave_Objects_SnowCat.lua", "/scripts/M1C3_WildernessCave_Items_SnowTracks4.lua", "/scripts/Doors_WildernessCave0_WildCaveDoor2River.lua", "/scripts/Doors_WildernessCave0_WildCaveDoor2Stump.lua", "/scripts/Doors_WildernessCave_WildCaveDoor2CaveInt.lua", "/scripts/Doors_WildernessCave_WildCaveDoor2CaveInt.lua", "/scripts/M3C3_WildernessCave0_CommandCoach_SpecialItem.lua", "/NPC/Puffles/Blue/", "/MissionObjects/M3C3/", "/MissionObjects/M4C1/", "/Objects/CommandCoach/", "/MissionObjects/M1C3/scripted/", "/Location/WildernessCave/state/", "/levels/WildernessCave0_map_0.mpb", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/FPMIntro_top", "/bg/Cutscenes/MissionIntroBottom", "/scripts/M1C3_WildernessClearing_Items_BrokenSpyPhone.lua", "/NPC/Puffles/Blue/", "/Objects/CommandCoach/", "/MissionObjects/M1C3/scripted/", "/Location/WildernessClearing/state/", "/levels/WildernessClearing0_map_0.mpb", "/scripts/M4C3_WildernessCliff_NPC_JPG.lua", "/scripts/M4C3_WildernessCliff_NPC_JetBot.lua", "/scripts/M4C3_WildernessCliff_NPC_JetBot.lua", "/scripts/M4C3_WildernessCliff_NPC_JetBot.lua", "/scripts/M4C3_WildernessCliff_NPC_JetBot.lua", "/scripts/M4C3_WildernessCliff_NPC_JetBot.lua", "/scripts/M4C3_WildernessCliff_NPC_SnowBot.lua", "/scripts/M4C3_WildernessCliff_NPC_SnowBot.lua", "/scripts/M4C3_WildernessCliff_ItemsM4C3_Loot.lua", "/scripts/M1C3_WildernessCliff0_Items_SnowCode.lua", "/scripts/M1C3_WildernessCliff0_ItemsM1C3_Cliff.lua", "/scripts/M4C3_WildernessCliff_ItemsM4C3_JetPack.lua", "/scripts/M1C3_WildernessCliff0_Items_SnowTracks1.lua", "/scripts/M4C3_WildernessCliff0_CommandCoach_SpecialItem.lua", "/NPC/JetBot/", "/NPC/SnowBot/", "/NPC/JetPackGuy/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M4C3/", "/Objects/CommandCoach/", "/MissionObjects/M1C3/scripted/", "/Location/WildernessCliff/state/", "/levels/WildernessCliff0_map_0.mpb", "/scripts/M1C3_WildernessPuffle_Items_Shelter.lua", "/scripts/M1C3_WildernessPuffle_ItemsM1C3_Fire.lua", "/scripts/M1C3_WildernessPuffle_Items_SnowTracks3.lua", "/scripts/Doors_WildernessPuffle_WildStumpDoor2Cave.lua", "/scripts/Doors_WildernessPuffle_WildStumpDoor2Berry.lua", "/scripts/M1C3_WildernessPuffle_Items_SurvivalGuideTorn.lua", "/scripts/Doors_WildernessPuffle_WildStumpDoor2Clearing.lua", "/NPC/Puffles/Blue/", "/Objects/CommandCoach/", "/MissionObjects/M1C3/scripted/", "/Location/WildernessStump/state/", "/levels/WildernessPuffle0_map_0.mpb", "/Objects/", "/scripts/M1C3_WildernessRiver_NPC_JetPackGuy.lua", "/scripts/M1C3_WildernessRiver_NPC_RookieRiver.lua", "/scripts/M1C3_WildernessRiver_NPC_RookieRiver2.lua", "/scripts/Doors_WildernessRiver_WildRiverDoor2Berry.lua", "/scripts/WildernessRiver0_CommandCoach_SpecialItem.lua", "/scripts/Doors_WildernessRiver_WildRiverDoor2Berry.lua", "/scripts/M1C3_WildernessRiver0_ItemsM1C3_SnowBarricade.lua", "/NPC/Rookie/", "/NPC/JetPackGuy/", "/NPC/Puffles/Blue/", "/Objects/CommandCoach/", "/MissionObjects/M1C3/scripted/", "/Location/WildernessRiver/state/", "/levels/WildernessRiver0_map_0.mpb", "/fonts/DSdigital.fnt", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/FPMIntro_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/FPMIntro_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/FPMIntro_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/FPMIntro_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/FPMIntro_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/FPMIntro_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/FPMIntro_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/MissionIntroBottom", "/synchn", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/FPMIntro_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/FPMIntro_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/MissionIntroBottom", "/bg/IceFishing/IceFishingCutsceneBG", "/bg/IceFishing/IceFishingTitleScreen", "/Minigames/IceFishing/Cutscene/PayoffPenguin", "/Minigames/IceFishing/Cutscene/PayoffWormcan", "/Minigames/IceFishing/Cutscene/PayoffSquidTrunk", "/bg/IceFishing/IceFishingCutsceneBG", "/bg/IceFishing/IceFishingTitleScreen", "/Minigames/IceFishing/Cutscene/PayoffMullet", "/Minigames/IceFishing/Cutscene/PayoffPenguin", "/Minigames/IceFishing/Cutscene/PayoffWormcan", "/flc/Intro.flc", "/bg/Cutscenes/CPLogo", "/fonts/faceFront.fnt", "/bg/Menus/Menus_bottom", "/flc/M1C1Intro03NewsPaperFull.flc", "/fonts/faceFront.fnt", "/bg/Cutscenes/M1C1Outro2", "/bg/Cutscenes/M1C1Outro3", "/bg/Cutscenes/M1C1Outro1", "/bg/Cutscenes/CutsceneText_top", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/DGamer/Scenes/DG1_Gifting_Sprite", "/Scenes/DG1_Gifting_Sprite", "/fonts/DSdigital.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/bg/Menus/Menus_bottom", "/CutsceneAnims/Badges/ice", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/bg/Menus/Menus_bottom", "/CutsceneAnims/Badges/wood", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/p/N/M", "/H3x", "/fonts/DSdigital.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/flc/M3C3_Talk_1.flc", "/flc/M3C3_Draw_2.flc", "/flc/M3C3_Draw_4.flc", "/flc/M3C3_Draw_5.flc", "/flc/M3C3_Talk_5.flc", "/flc/M3C3_Talk_2.flc", "/flc/M3C3_Draw_3.flc", "/flc/M3C3_Talk_4.flc", "/fonts/faceFront.fnt", "/flc/M3C3_Draw_1.flc", "/flc/M3C3_Talk_3.flc", "/bg/Cutscenes/CutsceneText_top_Gary", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/bg/Menus/Menus_bottom", "/CutsceneAnims/Badges/bronze", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/fonts/faceFront.fnt", "/fonts/DSdigital.fnt", "/bg/Cutscenes/M4C2_Gadget_3", "/bg/Cutscenes/M4C2_Gadget_1", "/bg/Cutscenes/M4C2_Gadget_2", "/bg/Cutscenes/M4C2_Gadget_4", "/bg/Cutscenes/MissionIntroTop", "/CutsceneAnims/PenguinHeads/red", "/CutsceneAnims/PenguinHeads/aqua", "/CutsceneAnims/PenguinHeads/blue", "/CutsceneAnims/PenguinHeads/lime", "/bg/Cutscenes/MissionIntroBottom", "/CutsceneAnims/PenguinHeads/black", "/CutsceneAnims/PenguinHeads/brown", "/CutsceneAnims/PenguinHeads/green", "/CutsceneAnims/PenguinHeads/peach", "/CutsceneAnims/PenguinHeads/orange", "/CutsceneAnims/PenguinHeads/purple", "/CutsceneAnims/PenguinHeads/yellow", "/CutsceneAnims/PenguinHeads/fuschia", "/bg/Cutscenes/CutsceneText_top_Gary", "/CutsceneAnims/PenguinHeads/darkGreen", "/bg/Cutscenes/CutsceneText_top_Penguin", "/Scenes/DG1_ChatGrey", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/fonts/Comicrazy.fnt", "/fonts/DSdigital.fnt", "/bg/Menus/Menus_bottom", "/bg/Cutscenes/M4C3_Outro1", "/bg/Cutscenes/M4C3_Outro3", "/bg/Cutscenes/M4C3_Outro2", "/CutsceneAnims/Badges/silver", "/bg/Cutscenes/CutsceneText_top", "/bg/Cutscenes/MissionIntroBottom", "/fonts/faceFront.fnt", "/flc/M5C1_Beach_full_3.flc", "/flc/M5C1_Beach_full_4.flc", "/flc/M5C1_Beach_full_1.flc", "/flc/M5C1_Beach_full_2.flc", "/CutsceneAnims/PenguinHeads/red", "/CutsceneAnims/PenguinHeads/aqua", "/CutsceneAnims/PenguinHeads/blue", "/CutsceneAnims/PenguinHeads/lime", "/CutsceneAnims/PenguinHeads/black", "/CutsceneAnims/PenguinHeads/brown", "/CutsceneAnims/PenguinHeads/green", "/CutsceneAnims/PenguinHeads/peach", "/CutsceneAnims/PenguinHeads/yellow", "/CutsceneAnims/PenguinHeads/orange", "/CutsceneAnims/PenguinHeads/purple", "/CutsceneAnims/PenguinHeads/fuschia", "/CutsceneAnims/PenguinHeads/darkGreen", "/bg/Cutscenes/CutsceneText_top_Robot", "/bg/Cutscenes/CutsceneText_top_Penguin", "/fonts/faceFront.fnt", "/flc/M5C1_Beacon_full_4.flc", "/flc/M5C1_Beacon_full_5.flc", "/flc/M5C1_Beacon_full_1.flc", "/flc/M5C1_Beacon_full_2.flc", "/flc/M5C1_Beacon_full_3.flc", "/CutsceneAnims/PenguinHeads/red", "/CutsceneAnims/PenguinHeads/aqua", "/CutsceneAnims/PenguinHeads/blue", "/CutsceneAnims/PenguinHeads/lime", "/CutsceneAnims/PenguinHeads/black", "/CutsceneAnims/PenguinHeads/brown", "/CutsceneAnims/PenguinHeads/green", "/CutsceneAnims/PenguinHeads/peach", "/CutsceneAnims/PenguinHeads/orange", "/CutsceneAnims/PenguinHeads/purple", "/CutsceneAnims/PenguinHeads/yellow", "/CutsceneAnims/PenguinHeads/fuschia", "/CutsceneAnims/PenguinHeads/darkGreen", "/bg/Cutscenes/CutsceneText_top_Robot", "/bg/Cutscenes/CutsceneText_top_Penguin", "/fonts/faceFront.fnt", "/flc/M5C1_Dock_full_4.flc", "/flc/M5C1_Dock_full_5.flc", "/flc/M5C1_Dock_full_1.flc", "/flc/M5C1_Dock_full_2.flc", "/flc/M5C1_Dock_full_3.flc", "/CutsceneAnims/PenguinHeads/red", "/CutsceneAnims/PenguinHeads/blue", "/CutsceneAnims/PenguinHeads/lime", "/CutsceneAnims/PenguinHeads/aqua", "/CutsceneAnims/PenguinHeads/brown", "/CutsceneAnims/PenguinHeads/green", "/CutsceneAnims/PenguinHeads/peach", "/CutsceneAnims/PenguinHeads/black", "/CutsceneAnims/PenguinHeads/orange", "/CutsceneAnims/PenguinHeads/purple", "/CutsceneAnims/PenguinHeads/yellow", "/CutsceneAnims/PenguinHeads/fuschia", "/CutsceneAnims/PenguinHeads/darkGreen", "/bg/Cutscenes/CutsceneText_top_Robot", "/bg/Cutscenes/CutsceneText_top_Penguin", "/bg/Cutscenes/CutsceneText_top_Gary_distressed", "/fonts/DSdigital.fnt", "/CutsceneAnims/StartButton", "/bg/Cutscenes/MissionIntroTop", "/bg/Cutscenes/MissionIntroBottom", "/fonts/faceFront.fnt", "/flc/M5C1_Town_full_2.flc", "/flc/M5C1_Town_full_1.flc", "/flc/M5C1_Mines_full_7.flc", "/flc/M5C1_Mines_full_3.flc", "/flc/M5C1_Mines_full_4.flc", "/flc/M5C1_Mines_full_8.flc", "/flc/M5C1_Mines_full_5.flc", "/flc/M5C1_Mines_full_6.flc", "/bg/Cutscenes/M5C1_Mines_bg_1", "/bg/Cutscenes/M5C1_Mines_bg_2", "/CutsceneAnims/PenguinHeads/red", "/CutsceneAnims/PenguinHeads/blue", "/CutsceneAnims/PenguinHeads/lime", "/CutsceneAnims/PenguinHeads/aqua", "/CutsceneAnims/PenguinHeads/black", "/CutsceneAnims/PenguinHeads/brown", "/CutsceneAnims/PenguinHeads/green", "/CutsceneAnims/PenguinHeads/peach", "/CutsceneAnims/PenguinHeads/orange", "/CutsceneAnims/PenguinHeads/purple", "/CutsceneAnims/PenguinHeads/yellow", "/CutsceneAnims/PenguinHeads/fuschia", "/CutsceneAnims/PenguinHeads/darkGreen", "/bg/Cutscenes/CutsceneText_top_Robot", "/bg/Cutscenes/CutsceneText_top_Penguin", "/bg/Cutscenes/CutsceneText_top_Gary_distressed", "/flc/End05full.flc", "/flc/End01full.flc", "/flc/End02full.flc", "/flc/End06full.flc", "/flc/End03full.flc", "/flc/End04full.flc", "/fonts/faceFront.fnt", "/fonts/DSdigital.fnt", "/bg/Menus/Menus_bottom", "/CutsceneAnims/Badges/gold", "/bg/Cutscenes/MissionIntroBottom", "/bg/Cutscenes/CutsceneText_top_Gary", "/bg/Cutscenes/CutsceneText_top_Director", "/fonts/faceFront.fnt", "/flc/M5C1_Roof_full_3.flc", "/flc/M5C1_Roof_full_4.flc", "/flc/M5C1_Roof_full_1.flc", "/flc/M5C1_Roof_full_2.flc", "/CutsceneAnims/PenguinHeads/red", "/CutsceneAnims/PenguinHeads/lime", "/CutsceneAnims/PenguinHeads/aqua", "/CutsceneAnims/PenguinHeads/blue", "/CutsceneAnims/PenguinHeads/green", "/CutsceneAnims/PenguinHeads/peach", "/CutsceneAnims/PenguinHeads/black", "/CutsceneAnims/PenguinHeads/brown", "/CutsceneAnims/PenguinHeads/yellow", "/CutsceneAnims/PenguinHeads/orange", "/CutsceneAnims/PenguinHeads/purple", "/CutsceneAnims/PenguinHeads/fuschia", "/bg/Cutscenes/CutsceneText_top_Robot", "/CutsceneAnims/PenguinHeads/darkGreen", "/bg/Cutscenes/CutsceneText_top_Penguin", "/fonts/faceFront.fnt", "/flc/M5C1_Ski_full_4.flc", "/flc/M5C1_Ski_full_5.flc", "/flc/M5C1_Ski_full_1.flc", "/flc/M5C1_Ski_full_2.flc", "/flc/M5C1_Ski_full_3.flc", "/CutsceneAnims/PenguinHeads/red", "/CutsceneAnims/PenguinHeads/blue", "/CutsceneAnims/PenguinHeads/lime", "/CutsceneAnims/PenguinHeads/aqua", "/CutsceneAnims/PenguinHeads/brown", "/CutsceneAnims/PenguinHeads/green", "/CutsceneAnims/PenguinHeads/peach", "/CutsceneAnims/PenguinHeads/black", "/CutsceneAnims/PenguinHeads/orange", "/CutsceneAnims/PenguinHeads/purple", "/CutsceneAnims/PenguinHeads/yellow", "/CutsceneAnims/PenguinHeads/fuschia", "/CutsceneAnims/PenguinHeads/darkGreen", "/bg/Cutscenes/CutsceneText_top_Robot", "/bg/Cutscenes/CutsceneText_top_Penguin", "/fonts/faceFront.fnt", "/flc/M5C1_TallMtn01_full_1.flc", "/flc/M5C1_TallMtn01_full_4.flc", "/flc/M5C1_TallMtn01_full_3.flc", "/CutsceneAnims/PenguinHeads/red", "/flc/M5C1_TallMtn01_full_2.flc", "/CutsceneAnims/PenguinHeads/aqua", "/CutsceneAnims/PenguinHeads/blue", "/CutsceneAnims/PenguinHeads/lime", "/CutsceneAnims/PenguinHeads/black", "/CutsceneAnims/PenguinHeads/brown", "/CutsceneAnims/PenguinHeads/green", "/CutsceneAnims/PenguinHeads/peach", "/CutsceneAnims/PenguinHeads/orange", "/CutsceneAnims/PenguinHeads/purple", "/CutsceneAnims/PenguinHeads/yellow", "/CutsceneAnims/PenguinHeads/fuschia", "/bg/Cutscenes/CutsceneText_top_Robot", "/CutsceneAnims/PenguinHeads/darkGreen", "/bg/Cutscenes/CutsceneText_top_Penguin", "/fonts/faceFront.fnt", "/CutsceneAnims/PenguinHeads/red", "/flc/M5C1_TallMtn02_full_3.flc", "/flc/M5C1_TallMtn02_full_2.flc", "/flc/M5C1_TallMtn02_full_1.flc", "/CutsceneAnims/PenguinHeads/aqua", "/CutsceneAnims/PenguinHeads/blue", "/CutsceneAnims/PenguinHeads/lime", "/CutsceneAnims/PenguinHeads/black", "/CutsceneAnims/PenguinHeads/brown", "/CutsceneAnims/PenguinHeads/green", "/CutsceneAnims/PenguinHeads/peach", "/CutsceneAnims/PenguinHeads/purple", "/CutsceneAnims/PenguinHeads/yellow", "/CutsceneAnims/PenguinHeads/orange", "/CutsceneAnims/PenguinHeads/fuschia", "/CutsceneAnims/PenguinHeads/darkGreen", "/bg/Cutscenes/CutsceneText_top_Robot", "/bg/Cutscenes/CutsceneText_top_Penguin", "/fonts/faceFront.fnt", "/flc/M5C1_TallMtn03_full_1.flc", "/bg/Cutscenes/CutsceneText_top_Gary", "/fonts/faceFront.fnt", "/flc/M5C1_TallMtn04_full_1.flc", "/bg/Cutscenes/CutsceneText_top_RobotHole", "/fonts/faceFront.fnt", "/bg/Cutscenes/M5C1_town_1", "/bg/Cutscenes/M5C1_town_2", "/bg/Cutscenes/CutsceneText_top_Robot", "/bg/BlankBlack", "/flc/1PLogotop.flc", "/bg/Cutscenes/DisneyLogo", "/bg/Cutscenes/NintendoTop", "/bg/Cutscenes/1PLogobottom", "/bg/Cutscenes/DisneyLogoCopyright", "/fonts/faceFront.fnt", "/flc/HowToRobot_Draw_2.flc", "/flc/HowToRobot_Draw_3.flc", "/flc/HowToRobot_Talk_3.flc", "/flc/HowToRobot_Draw_1.flc", "/flc/HowToRobot_Talk_2.flc", "/flc/HowToRobot_Talk_1.flc", "/bg/Cutscenes/CutsceneText_top_Gary", "/fonts/faceFront.fnt", "/flc/M1C2_SpyGadget_2.flc", "/flc/M1C2_SpyGadget_4.flc", "/flc/M1C2_SpyGadget_5.flc", "/flc/M1C2_SpyGadget_3.flc", "/flc/M1C2_SpyGadget_1.flc", "/bg/Cutscenes/CutsceneText_top_Director", "/Minigames/SnowCat/", "/levels/SnowCat1_map_0.mpb", "/levels/SnowCat1_map_1.mpb", "/Minigames/SnowCat/Pickups/", "/Minigames/SnowCat/Breakables/", "/Minigames/SnowCat/Unbreakables/", "/Scenes/DG1_MainMenuMasterPalette", "/Scenes/DG1_MainMenuMasterPalette", "/Scenes/DG1_MainMenuMasterPalette", "/Scenes/DG1_MainMenuMasterPalette", "/Minigames/SnowCat/", "/levels/SnowCat2_map_0.mpb", "/levels/SnowCat2_map_1.mpb", "/Minigames/SnowCat/Pickups/", "/Minigames/SnowCat/Breakables/", "/Minigames/SnowCat/Unbreakables/", "/Minigames/SnowCat/", "/levels/SnowCat3_map_0.mpb", "/levels/SnowCat3_map_1.mpb", "/Minigames/SnowCat/Pickups/", "/Minigames/SnowCat/Breakables/", "/Minigames/SnowCat/Unbreakables/", "/Minigames/SnowCat/", "/levels/SnowCat4_map_0.mpb", "/levels/SnowCat4_map_1.mpb", "/Minigames/SnowCat/Pickups/", "/Minigames/SnowCat/Breakables/", "/Minigames/SnowCat/Unbreakables/", "/Minigames/SnowCat/", "/levels/SnowCat5_map_0.mpb", "/levels/SnowCat5_map_1.mpb", "/Minigames/SnowCat/Pickups/", "/Minigames/SnowCat/Breakables/", "/Minigames/SnowCat/Unbreakables/", "/Minigames/JetPack/Deco/", "/Minigames/JetPack/Hazards/", "/Minigames/JetPack/Pickups/", "/Minigames/JetPack/Landing/", "/levels/JetpackBeacon2Mountain_map_0.mpb", "/Minigames/JetPack/NPC/", "/Minigames/JetPack/Deco/", "/Minigames/JetPack/Pickups/", "/Minigames/JetPack/Landing/", "/Minigames/JetPack/Hazards/", "/levels/JetpackBotChase_map_0.mpb", "/Minigames/JetPack/NPC/", "/Minigames/JetPack/Deco/", "/Minigames/JetPack/Pickups/", "/Minigames/JetPack/Landing/", "/Minigames/JetPack/Hazards/", "/levels/JetpackJPGChase_map_0.mpb", "/Minigames/JetPack/Deco/", "/Minigames/JetPack/Hazards/", "/Minigames/JetPack/Landing/", "/Minigames/JetPack/Pickups/", "/levels/JetpackMountain2Wild_map_0.mpb", "/Minigames/JetPack/Deco/", "/Minigames/JetPack/Hazards/", "/Minigames/JetPack/Pickups/", "/levels/JetpackMultiplay_map_0.mpb", "/Minigames/JetPack/NPC/", "/Minigames/JetPack/Deco/", "/Minigames/JetPack/Pickups/", "/Minigames/JetPack/Landing/", "/Minigames/JetPack/Hazards/", "/levels/JetpackSuperBotChase_map_0.mpb", "/Minigames/JetPack/NPC/", "/Minigames/JetPack/Clouds/", "/Minigames/JetPack/Hazards/", "/Minigames/JetPack/Pickups/", "/Minigames/JetPack/Landing/", "/levels/JetpackTestLevel_map_0.mpb", "/Minigames/JetPack/Deco/", "/Minigames/JetPack/Pickups/", "/Minigames/JetPack/Hazards/", "/levels/JetpackMultiplay2_map_0.mpb", "/Objects/", "/scripts/FPM10_NPC_Puffle1.lua", "/scripts/Attic0_CommandCoach_SpecialItem.lua", "/NPC/Puffles/Blue/", "/Location/Attic/state/", "/levels/Attic0_map_0.mpb", "/scripts/M5C1_Beach_NPC_Flit.lua", "/scripts/M5C1_Beach_NPC_Gary.lua", "/scripts/M2C1_Items_NutsBolts.lua", "/scripts/SeasFPM04_Item_Coconut.lua", "/scripts/FPM06_Dock_Items_Boat2.lua", "/scripts/SeasFPM01_NPC_GuitarGuy.lua", "/scripts/M2C1_Beach_Coach_Prints.lua", "/scripts/M2C1_Beach_Coach_Prints.lua", "/scripts/M2C1_Beach_Coach_Prints.lua", "/scripts/M2C1_Beach_Coach_Prints.lua", "/scripts/M2C1_Beach_Coach_Prints.lua", "/scripts/M2C1_Beach_Coach_Prints.lua", "/scripts/M2C1_Beach_Coach_Prints.lua", "/scripts/M2C1_Beach_Coach_Prints.lua", "/scripts/M2C1_Beach_Coach_Prints.lua", "/scripts/SeasFPM04_Item_EmptyTree.lua", "/scripts/M5C1_Beach_NPC_SuperRobot.lua", "/scripts/SeasFPM02_Item_FlowerLow5.lua", "/scripts/Doors_Beach0_BeachDoor2Ski.lua", "/scripts/SeasFPM02_Item_FlowerHigh3.lua", "/scripts/Doors_Beach0_BeachDoor2Dock.lua", "/scripts/Doors_Beach0_BeachDoor2Light.lua", "/scripts/FP00_Beach0_NPC_FreeplayNPC2.lua", "/scripts/SeasFPM04_Beach_Item_PalmTree.lua", "/scripts/M1C1_Beach0_CommandCoach_SpecialItem.lua", "/NPC/GuitarGuy/", "/3d/SuperRobot/", "/NPC/SuperRobot/", "/NPC/Puffles/Blue/", "/NPC/Puffles/Green/", "/UI/InventoryPanel/", "/Objects/WorldItems/", "/Location/Beach/touch/", "/Location/Beach/state/", "/Objects/CommandCoach/", "/Objects/Seasonal/Xmas/", "/Location/Beach/static/", "/levels/Beach0_map_0.mpb", "/levels/Beach0_map_1.mpb", "/NPC/GenericNPC/Freeplay/", "/MissionObjects/SeasonalM4/", "/MissionObjects/SeasonalM2/", "/Objects/Seasonal/Halloween/", "/Objects/Seasonal/AprilFools/", "/Objects/CommandCoach/footprints/", "/scripts/M3C2_BEACON_NPC_JPG.lua", "/scripts/M3C2_BEACON_NPC_FLIT.lua", "/scripts/M2C1_Items_NutsBolts.lua", "/scripts/M5C1_Beacon_NPC_Gary.lua", "/scripts/M3C2_FPBEACON_NPC_JPG.lua", "/scripts/M3C2_FPBEACON_NPC_FLIT.lua", "/scripts/M5C1_Beacon_Items_Anvil.lua", "/scripts/M5C1_Beacon_NPC_Bouncer.lua", "/scripts/M5C1_Beacon_Items_Balloon.lua", "/scripts/M5C1_Beacon_Items_Jetpack.lua", "/scripts/M5C1_Beacon_NPC_SuperRobot.lua", "/scripts/FPM05_Beacon_NPC_AuntArctic.lua", "/scripts/FP00_Beacon0_ItemsM3C2_Jetpack.lua", "/scripts/Doors_Beacon0_BeaconDoor2Light.lua", "/scripts/M3C2_BEACON_ItemsM3C2_JETBACKPACK.lua", "/scripts/FPM05_Beacon_Items_PizzaHotSauceFake.lua", "/scripts/M2C1_LighthouseUp0_ItemsM2C1_PizzaBox.lua", "/scripts/M2C1_LighthouseUp0_ItemsM2C1_Telescope.lua", "/scripts/Doors_Beacon0_LighthouseUpDoor2JetPack.lua", "/NPC/JetBot/", "/3d/SuperRobot/", "/NPC/SuperRobot/", "/NPC/AuntArctic/", "/NPC/JetPackGuy/", "/NPC/Puffles/Blue/", "/NPC/Puffles/Green/", "/UI/InventoryPanel/", "/Objects/WorldItems/", "/MissionObjects/M5C1/", "/MissionObjects/M3C2/", "/Objects/CommandCoach/", "/levels/Beacon0_map_0.mpb", "/MissionObjects/M2C1/touch/", "/Location/Lighthouse/state/", "/Objects/Seasonal/AprilFools/", "/Location/LighthouseUp/script/", "/MissionObjects/FreeplayM4/scripted/", "/scripts/M3C1_BoilerRoom0_NPC_DJ.lua", "/scripts/M3C1_Pool0_ItemsM3C1_WaterTrail.lua", "/scripts/M3C1_Pool0_ItemsM3C1_WaterTrail.lua", "/scripts/M3C1_Pool0_ItemsM3C1_WaterTrail.lua", "/scripts/FP_BoilerRoom_ReplacementBoiler.lua", "/scripts/Doors_BoilerRoom0_BoilerDoor2Pool.lua", "/scripts/FP00_BoilerRoom0_NPC_FreeplayNPC1.lua", "/scripts/M3C1_BoilerRoom0_ItemsM3C1_Boiler.lua", "/scripts/M3C1_BoilerRoom0_ItemsM3C1_OilCan.lua", "/scripts/Doors_BoilerRoom0_BoilerDoor2Night.lua", "/scripts/M3C1_BoilerRoom0_ItemsM3C1_FileCabinet.lua", "/scripts/M3C1_BoilerRoom0_ItemsM3C1_FileCabinet.lua", "/NPC/DJ/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M3C1/", "/Objects/CommandCoach/", "/NPC/GenericNPC/Freeplay/", "/Location/BoilerRoom/state/", "/levels/BoilerRoom0_map_0.mpb", "/Location/BoilerRoom/scripted/", "/scripts/FPM10_NPC_Puffle4.lua", "/scripts/FP00_BookRoom0_NPC_FreeplayNPC4.lua", "/NPC/Puffles/Blue/", "/NPC/Puffles/Yellow/", "/Objects/CommandCoach/", "/Location/BookRoom/state/", "/NPC/GenericNPC/Freeplay/", "/levels/BookRoom0_map_0.mpb", "/Objects/Seasonal/AprilFools/", "/scripts/M3C3_CaveInterior_Items_OilCan.lua", "/scripts/M3C3_CaveInterior_Objects_Tracks.lua", "/scripts/M3C3_CaveInterior_Objects_Tracks.lua", "/scripts/M3C3_CaveInterior_Objects_Tracks.lua", "/scripts/M3C3_CaveInterior_NPC_RobotShadow.lua", "/scripts/M3C3_CaveInterior_Objects_Barrier.lua", "/scripts/M3C3_CaveInterior_Objects_Barrier2.lua", "/scripts/Doors_CaveInt0_CaveIntDoor2WildCave.lua", "/scripts/M3C3_CaveInterior_Objects_SnowboardWarnings.lua", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M3C3/", "/MissionObjects/M4C1/", "/Objects/CommandCoach/", "/Location/CaveInt/state/", "/Location/CaveInt/static/", "/levels/CaveInterior0_map_0.mpb", "/scripts/M3C3_CoffeeShop_NPC_Rookie.lua", "/scripts/M3C3_CoffeeShop_NPC_Barrista.lua", "/scripts/FPM05_CoffeeShop_NPC_Barista.lua", "/scripts/M2C2_CoffeeShop0_NPC_Barista.lua", "/scripts/FP01_CoffeeShop0_NPC_Barista.lua", "/scripts/M1C1_CoffeeShop0_NPC_Barista.lua", "/scripts/FP00_CoffeeShop0_NPC_BaristaCup.lua", "/scripts/Doors_CoffeeShop0_CoffeeDoor2Town.lua", "/scripts/M1C1_CoffeeShop0_ItemsM1C1_MapPart.lua", "/scripts/M1C1_CoffeeShop0_ItemsM1C1_MapPart.lua", "/scripts/M1C1_CoffeeShop0_ItemsM1C1_MapPart.lua", "/scripts/M3C3_CoffeeShop_Items_CocoaMachine.lua", "/scripts/M3C3_CoffeeShop_Objects_PoliceTape.lua", "/scripts/M1C1_CoffeeShop0_ItemsM1C1_MapPart.lua", "/scripts/SeasFPM04_PizzaParlor_NPC_PizzaChef.lua", "/scripts/FP00_CoffeeShop0_ItemsFP00_Newspaper.lua", "/scripts/FPM05_CoffeeShop_Items_PizzaSquidFake.lua", "/scripts/M2C2_CoffeeShop0_ItemsM2C2_MissingPen.lua", "/scripts/FP01_CoffeeShop0_ItemsFP01_CouchWrench.lua", "/scripts/M2C2_CoffeeShop0_ItemsM2C2_SpilledBeans.lua", "/scripts/Doors_CoffeeShop0_CoffeeShopDoor2Upstairs.lua", "/scripts/Doors_CoffeeShop0_CoffeeShopDoor2Upstairs.lua", "/scripts/Doors_CoffeeShop0_CoffeeShopDoor2Employees.lua", "/scripts/M1C1_CoffeeShop0_ItemsM1C1_CoffeeShopPuffle.lua", "/NPC/Rookie/", "/NPC/Barista/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M3C3/", "/Objects/CommandCoach/", "/Objects/Seasonal/Xmas/", "/Objects/Seasonal/Summer/", "/Location/CoffeeShop/state/", "/Location/CoffeeShop/touch/", "/Location/CoffeeShop/static/", "/Objects/Seasonal/AprilFools/", "/levels/CoffeeShop0_map_0.mpb", "/MissionObjects/M2C2/scripted/", "/MissionObjects/FreeplayM4/scripted/", "/MissionObjects/FreeplayWorld/scripted/", "/char/ybBgStep11.ncl.l", "/char/ybBgOption.ncl.l", "/char/ybBgOption1.ncl.l", "/char/ybBgStep31.ncl.l", "/char/xb4ApListBack.nsc.l", "/char/ybBgStep2.ncl.l", "/char/ybBgStep21.ncl.l", "/char/jb3ListBack.nsc.l", "/char/ybBgStep11.ncl.l", "/char/ybBgStep21.ncl.l", "/char/jtNull.nsc.l", "/char/jb2HlAp.nsc.l", "/char/jb4HlIp.nsc.l", "/char/jb4HlWep.nsc.l", "/char/jb4HlUsb.nsc.l", "/char/jb4HlDns1.nsc.l", "/char/jb4HlSsid.nsc.l", "/char/jb5HlMove.nsc.l", "/char/jb2HlWiFi.nsc.l", "/char/jb5HlInfo.nsc.l", "/char/jb4HlMask.nsc.l", "/char/jb4HlSet2.nsc.l", "/char/jb4HlDns0.nsc.l", "/char/jb4HlSet3.nsc.l", "/char/jb4HlSet1.nsc.l", "/char/jb3HlList1.nsc.l", "/char/jb3HlList2.nsc.l", "/char/jb3HlList3.nsc.l", "/char/jb5HlErase.nsc.l", "/char/jb5HlOption.nsc.l", "/char/jb4HlGateway.nsc.l", "/char/jbBgHl.ncg.l", "/msg/kor.bmg.l", "/msg/eng.bmg.l", "/msg/ita.bmg.l", "/msg/ger.bmg.l", "/msg/fre.bmg.l", "/msg/spa.bmg.l", "/msg/jap.bmg.l", "/msg/usa.bmg.l", "/char/jtMain.nce.l", "/char/jbMain.nce.l", "/char/jtBgMain.ncg.l", "/char/jtBgMain.ncl.l", "/char/jtObjMain.ncg.l", "/char/xtObjMain.ncl.l", "/char/jbBgStep1.ncg.l", "/char/jbBgStep1.ncl.l", "/char/jbObjMain.ncg.l", "/char/ybObjMain.ncl.l", "/char/jtTop.nsc.l", "/char/jtStep1.nsc.l", "/char/jbBgStep1.ncg.l", "/char/jbBgStep1.ncl.l", "/char/jb2Menu.nsc.l", "/char/yb5Multi.nsc.l", "/char/yb5Multi.nsc.l", "/char/jb5Info.nsc.l", "/char/jbBgOption.ncg.l", "/char/jb5OptMenu.nsc.l", "/char/yb5Multi.nsc.l", "/char/yb5Multi.nsc.l", "/char/yb5Multi.nsc.l", "/char/yb5Multi.nsc.l", "/char/jb5Move.nsc.l", "/char/yb5Multi.nsc.l", "/char/jbBgStep3.ncg.l", "/char/ybBgStep3.ncl.l", "/char/xb4Multi.nsc.l", "/char/xb4Multi.nsc.l", "/char/jb4ApList.nsc.l", "/char/ybObjKb.ncl.l", "/char/jbBgStep3.ncg.l", "/char/ybBgStep3.ncl.l", "/char/xb4Edit.nsc.l", "/char/ybObjMain.ncl.l", "/char/ybObjKb.ncl.l", "/char/jbBgStep3.ncg.l", "/char/ybBgStep3.ncl.l", "/char/xb4EditAddr.nsc.l", "/char/ybObjMain.ncl.l", "/char/jb4Error.nsc.l", "/char/ybObjKb.ncl.l", "/char/jbBgStep2.ncg.l", "/char/jbBgStep21.ncg.l", "/char/jb3List.nsc.l", "/char/ybObjMain.ncl.l", "/char/jbBgStep3.ncg.l", "/char/ybBgStep3.ncl.l", "/char/xb4None.nsc.l", "/char/xb4Multi.nsc.l", "/char/xb4Multi.nsc.l", "/char/xb4Multi.nsc.l", "/char/jbBgStep3.ncg.l", "/char/ybBgStep3.ncl.l", "/char/xb4Multi.nsc.l", "/char/jbBgStep3.ncg.l", "/char/ybBgStep3.ncl.l", "/char/xb4Multi.nsc.l", "/char/ybObjWay.ncl.l", "/char/jbBgStep1.ncg.l", "/char/jbBgStep1.ncl.l", "/char/jb2Ap.nsc.l", "/char/ybObjMain.ncl.l", "/char/jbBgStep2.ncg.l", "/char/ybBgStep2.ncl.l", "/char/jb3Way.nsc.l", "/char/jbBgStep3.ncg.l", "/char/ybBgStep3.ncl.l", "/char/xb4Multi.nsc.l", "/char/xb4Multi.nsc.l", "/char/xb4None.nsc.l", "/char/xb4Multi.nsc.l", "/char/jbBgStep2.ncg.l", "/char/ybBgStep2.ncl.l", "/char/xb3Multi.nsc.l", "/char/jbBgStep3.ncg.l", "/char/ybBgStep3.ncl.l", "/char/jb4Usb.nsc.l", "/sound/sound_data.sdat.l", "/char/jtTop.nsc.l", "/char/jtStep1.nsc.l", "/char/jtStep2.nsc.l", "/char/jtStep3.nsc.l", "/char/jtOption.nsc.l", "/msg/lc_s.NFTR.l", "/msg/kc_m.NFTR.l", "/msg/lc_m.NFTR.l", "/HTTP/", "/chunks/luatest.luc", "/chunks/luatest_embeddingtest.luc", "/chunks/luatest_extensiontest.luc", "/strings/english_notestrings.st", "/strings/english_notes.st", "/strings/english_notesstrings.st" };

        public string[] stringsHR = new string[] { "/chunks/", "/scripts/", "/levels/AquaGrabber.tsb", "/levels/AquaRescue3.tsb", "/levels/AquagrabberDash.tsb", "/levels/Attic.tsb", "/levels/Beach.tsb", "/levels/Beach_BG.tsb", "/levels/Beacon.tsb", "/levels/BoilerRoom.tsb", "/levels/BookRoom.tsb", "/levels/CaveInteriorHerb.tsb", "/levels/CoffeeMachine.tsb", "/levels/CoffeeShop.tsb", "/levels/CommandRoom.tsb", "/levels/CornMaze.tsb", "/levels/DVDMachine.tsb", "/levels/DesignTiles.tsb", "/levels/Fishing.tsb", "/levels/Forest.tsb", "/levels/FurAnalyzer.tsb", "/levels/GadgetRoomCrabMach_map_0.mpb", "/levels/GadgetRoom.tsb", "/levels/GarysRooms.tsb", "/levels/GiftOffice.tsb", "/levels/GiftRoof.tsb", "/levels/GiftShop.tsb", "/levels/GrapplingBG.tsb", "/levels/GrapplingBonus.tsb", "/levels/GrapplingHook_HerbBase.tsb", "/levels/GrapplingMountain.tsb", "/levels/Grappling.tsb", "/levels/HQ.tsb", "/levels/HerbDesk.tsb", "/levels/HerbertsCave.tsb", "/levels/HerbsCamp.tsb", "/levels/IceRinkFlood_map_0.mpb", "/levels/IceRink.tsb", "/levels/JH_6_map_0.mpb", "/levels/JH_Snow_map_0.mpb", "/levels/JH_Snow.tsb", "/levels/JH_geyser_map_0.mpb", "/levels/JH_mountain_map_0.mpb", "/levels/JackhammerBonus1_map_0.mpb", "/levels/JackhammerBonus2_map_0.mpb", "/levels/JackhammerBonus3_map_0.mpb", "/levels/Jackhammer_map_0.mpb", "/levels/Jackhammer.tsb", "/levels/Jackhammer_geyser.tsb", "/levels/Jackhammer_mountain.tsb", "/levels/Labyrinth1.tsb", "/levels/LabyrinthCave.tsb", "/levels/LabyrinthCorn.tsb", "/levels/LabyrinthWilderness.tsb", "/levels/LevelSelect_map_0.mpb", "/levels/LevelSelectMap.tsb", "/levels/Lighthouse.tsb", "/levels/Lobby.tsb", "/levels/Lodge.tsb", "/levels/Lounge.tsb", "/levels/MineCrashSite.tsb", "/levels/MineExterior.tsb", "/levels/MineInterior.tsb", "/scripts/Doors_Mine2Int.lua", "/scripts/C4_MineShack_NPC_Dot.lua", "/scripts/C4_MineShack_NPC_Rookie.lua", "/scripts/C4_MineShack_Item_Geyser.lua", "/levels/MineShackPuddles_map_0.mpb", "/levels/MineShackPuddles_map_1.mpb", "/levels/MineShedInterior.tsb", "/levels/MineTunnelExit.tsb", "/levels/Mountain.tsb", "/levels/NightClub.tsb", "/levels/PetShop.tsb", "/levels/PizzaParlor.tsb", "/levels/Plaza.tsb", "/levels/Pool.tsb", "/levels/PuffleTraining.tsb", "/levels/SkiVillage2_map_1.mpb", "/levels/SkiVillage2_map_0.mpb", "/levels/SkiVillage.tsb", "/levels/SnowFortsFlood_map_1.mpb", "/levels/SnowFortsFlood_map_0.mpb", "/levels/SnowForts.tsb", "/levels/SportShop.tsb", "/levels/Stage.tsb", "/levels/TallestMountainTop.tsb", "/levels/TallestMountainTop_BG.tsb", "/levels/TallestMtn.tsb", "/levels/TownFlood_map_1.mpb", "/levels/TownFlood_map_0.mpb", "/levels/Town.tsb", "/levels/TrainingCavesRiver.tsb", "/levels/TrainingCaves.tsb", "/levels/Trough.tsb", "/levels/UG_Gift0.tsb", "/levels/Wilderness.tsb", "/Minigames/Aquagrabber/tube1", "/strings/English_dialogstrings.st2", "/strings/English_adhocstrings.st2", "/strings/English_gamestrings.st2", "/strings/English_savegamestrings.st2", "/strings/English_downloadstrings.st2", "/strings/English_wifistrings.st2", "/strings/English_localizationStrings.st2", "/strings/English_cutsceneStrings.st2", "/strings/English_conversationStrings.st2", "/strings/English_unlockablesStrings.st2", "/strings/English_DisneyStrings.st2", "/strings/English_disneyStrings.st2", "/strings/Spanish_dialogstrings.st2", "/strings/Spanish_adhocstrings.st2", "/strings/Spanish_gamestrings.st2", "/strings/Spanish_savegamestrings.st2", "/strings/Spanish_downloadstrings.st2", "/strings/Spanish_wifistrings.st2", "/strings/Spanish_localizationStrings.st2", "/strings/Spanish_cutsceneStrings.st2", "/strings/Spanish_conversationStrings.st2", "/strings/Spanish_unlockablesStrings.st2", "/strings/Spanish_DisneyStrings.st2", "/strings/Italian_dialogstrings.st2", "/strings/Italian_adhocstrings.st2", "/strings/Italian_gamestrings.st2", "/strings/Italian_savegamestrings.st2", "/strings/Italian_downloadstrings.st2", "/strings/Italian_wifistrings.st2", "/strings/Italian_localizationStrings.st2", "/strings/Italian_cutsceneStrings.st2", "/strings/Italian_conversationStrings.st2", "/strings/Italian_unlockablesStrings.st2", "/strings/Italian_DisneyStrings.st2", "/strings/French_dialogstrings.st2", "/strings/French_adhocstrings.st2", "/strings/French_gamestrings.st2", "/strings/French_downloadstrings.st2", "/strings/French_savegamestrings.st2", "/strings/French_wifistrings.st2", "/strings/French_localizationStrings.st2", "/strings/French_cutsceneStrings.st2", "/strings/French_conversationStrings.st2", "/strings/French_unlockablesStrings.st2", "/strings/French_DisneyStrings.st2", "/strings/German_dialogstrings.st2", "/strings/German_adhocstrings.st2", "/strings/German_gamestrings.st2", "/strings/German_downloadstrings.st2", "/strings/German_downloadstrings.st2", "/strings/German_savegamestrings.st2", "/strings/German_wifistrings.st2", "/strings/German_localizationStrings.st2", "/strings/German_cutsceneStrings.st2", "/strings/German_conversationStrings.st2", "/strings/German_unlockablesStrings.st2", "/strings/German_DisneyStrings.st2", "/strings/Dutch_dialogstrings.st2", "/strings/Dutch_adhocstrings.st2", "/strings/Dutch_gamestrings.st2", "/strings/Dutch_downloadstrings.st2", "/strings/Dutch_savegamestrings.st2", "/strings/Dutch_wifistrings.st2", "/strings/Dutch_localizationStrings.st2", "/strings/Dutch_cutsceneStrings.st2", "/strings/Dutch_conversationStrings.st2", "/strings/Dutch_unlockablesStrings.st2", "/strings/Dutch_DisneyStrings.st2", "/strings/Danish_dialogstrings.st2", "/strings/Danish_adhocstrings.st2", "/strings/Danish_gamestrings.st2", "/strings/Danish_downloadstrings.st2", "/strings/Danish_savegamestrings.st2", "/strings/Danish_wifistrings.st2", "/strings/Danish_localizationStrings.st2", "/strings/Danish_cutsceneStrings.st2", "/strings/Danish_conversationStrings.st2", "/strings/Danish_unlockablesStrings.st2", "/strings/Danish_DisneyStrings.st2", "/strings/Japanese_dialogstrings.st2", "/strings/Japanese_adhocstrings.st2", "/strings/Japanese_gamestrings.st2", "/strings/Japanese_downloadstrings.st2", "/strings/Japanese_savegamestrings.st2", "/strings/Japanese_wifistrings.st2", "/strings/Japanese_localizationStrings.st2", "/strings/Japanese_cutsceneStrings.st2", "/strings/Japanese_conversationStrings.st2", "/strings/Japanese_unlockablesStrings.st2", "/strings/Japanese_DisneyStrings.st2", "/strings/Chinese_dialogstrings.st2", "/strings/Chinese_adhocstrings.st2", "/strings/Chinese_gamestrings.st2", "/strings/Chinese_downloadstrings.st2", "/strings/Chinese_savegamestrings.st2", "/strings/Chinese_wifistrings.st2", "/strings/Chinese_localizationStrings.st2", "/strings/Chinese_cutsceneStrings.st2", "/strings/Chinese_conversationStrings.st2", "/strings/Chinese_unlockablesStrings.st2", "/strings/Chinese_DisneyStrings.st2", "/strings/Korean_dialogstrings.st2", "/strings/Korean_adhocstrings.st2", "/strings/Korean_gamestrings.st2", "/strings/Korean_downloadstrings.st2", "/strings/Korean_savegamestrings.st2", "/strings/Korean_wifistrings.st2", "/strings/Korean_localizationStrings.st2", "/strings/Korean_cutsceneStrings.st2", "/strings/Korean_conversationStrings.st2", "/strings/Korean_unlockablesStrings.st2", "/strings/Korean_DisneyStrings.st2", "/strings/Hangul_dialogstrings.st2", "/strings/Hangul_adhocstrings.st2", "/strings/Hangul_gamestrings.st2", "/strings/Hangul_downloadstrings.st2", "/strings/Hangul_savegamestrings.st2", "/strings/Hangul_wifistrings.st2", "/strings/Hangul_localizationStrings.st2", "/strings/Hangul_cutsceneStrings.st2", "/strings/Hangul_conversationStrings.st2", "/strings/Hangul_unlockablesStrings.st2", "/strings/Hangul_DisneyStrings.st2", "/strings/Norwegian_dialogstrings.st2", "/strings/Norwegian_adhocstrings.st2", "/strings/Norwegian_gamestrings.st2", "/strings/Norwegian_downloadstrings.st2", "/strings/Norwegian_savegamestrings.st2", "/strings/Norwegian_wifistrings.st2", "/strings/Norwegian_localizationStrings.st2", "/strings/Norwegian_cutsceneStrings.st2", "/strings/Norwegian_conversationStrings.st2", "/strings/Norwegian_unlockablesStrings.st2", "/strings/Norwegian_DisneyStrings.st2", "/strings/Swedish_dialogstrings.st2", "/strings/Swedish_adhocstrings.st2", "/strings/Swedish_gamestrings.st2", "/strings/Swedish_downloadstrings.st2", "/strings/Swedish_savegamestrings.st2", "/strings/Swedish_wifistrings.st2", "/strings/Swedish_localizationStrings.st2", "/strings/Swedish_cutsceneStrings.st2", "/strings/Swedish_conversationStrings.st2", "/strings/Swedish_unlockablesStrings.st2", "/strings/Swedish_DisneyStrings.st2", "/strings/UKEnglish_dialogstrings.st2", "/strings/UKEnglish_adhocstrings.st2", "/strings/UKEnglish_gamestrings.st2", "/strings/UKEnglish_downloadstrings.st2", "/strings/UKEnglish_savegamestrings.st2", "/strings/UKEnglish_wifistrings.st2", "/strings/UKEnglish_localizationStrings.st2", "/strings/UKEnglish_cutsceneStrings.st2", "/strings/UKEnglish_conversationStrings.st2", "/strings/UKEnglish_unlockablesStrings.st2", "/strings/UKEnglish_DisneyStrings.st2", "/strings/dialogstrings.st2", "/strings/adhocstrings.st2", "/strings/adhocStrings.st2", "/strings/gamestrings.st2", "/strings/downloadstrings.st2", "/strings/downloadstrings.st2", "/strings/savegamestrings.st2", "/strings/wifistrings.st2", "/strings/localizationStrings.st2", "/strings/cutsceneStrings.st2", "/strings/conversationStrings.st2", "/strings/unlockablesStrings.st2", "/strings/DisneyStrings.st2", "/strings/disneyStrings.st2", "/strings/Credits.st2", "/strings/credits.st2", "/strings/English_dialogstrings.crc", "/strings/English_adhocstrings.crc", "/strings/English_gamestrings.crc", "/strings/English_savegamestrings.crc", "/strings/English_downloadstrings.crc", "/strings/English_wifistrings.crc", "/strings/English_localizationStrings.crc", "/strings/English_cutsceneStrings.crc", "/strings/English_conversationStrings.crc", "/strings/English_unlockablesStrings.crc", "/strings/English_DisneyStrings.crc", "/strings/English_disneyStrings.crc", "/strings/Spanish_dialogstrings.crc", "/strings/Spanish_adhocstrings.crc", "/strings/Spanish_gamestrings.crc", "/strings/Spanish_savegamestrings.crc", "/strings/Spanish_downloadstrings.crc", "/strings/Spanish_wifistrings.crc", "/strings/Spanish_localizationStrings.crc", "/strings/Spanish_cutsceneStrings.crc", "/strings/Spanish_conversationStrings.crc", "/strings/Spanish_unlockablesStrings.crc", "/strings/Spanish_DisneyStrings.crc", "/strings/Italian_dialogstrings.crc", "/strings/Italian_adhocstrings.crc", "/strings/Italian_gamestrings.crc", "/strings/Italian_savegamestrings.crc", "/strings/Italian_downloadstrings.crc", "/strings/Italian_wifistrings.crc", "/strings/Italian_localizationStrings.crc", "/strings/Italian_cutsceneStrings.crc", "/strings/Italian_conversationStrings.crc", "/strings/Italian_unlockablesStrings.crc", "/strings/Italian_DisneyStrings.crc", "/strings/French_dialogstrings.crc", "/strings/French_adhocstrings.crc", "/strings/French_gamestrings.crc", "/strings/French_downloadstrings.crc", "/strings/French_savegamestrings.crc", "/strings/French_wifistrings.crc", "/strings/French_localizationStrings.crc", "/strings/French_cutsceneStrings.crc", "/strings/French_conversationStrings.crc", "/strings/French_unlockablesStrings.crc", "/strings/French_DisneyStrings.crc", "/strings/German_dialogstrings.crc", "/strings/German_adhocstrings.crc", "/strings/German_gamestrings.crc", "/strings/German_downloadstrings.crc", "/strings/German_downloadstrings.crc", "/strings/German_savegamestrings.crc", "/strings/German_wifistrings.crc", "/strings/German_localizationStrings.crc", "/strings/German_cutsceneStrings.crc", "/strings/German_conversationStrings.crc", "/strings/German_unlockablesStrings.crc", "/strings/German_DisneyStrings.crc", "/strings/Dutch_dialogstrings.crc", "/strings/Dutch_adhocstrings.crc", "/strings/Dutch_gamestrings.crc", "/strings/Dutch_downloadstrings.crc", "/strings/Dutch_savegamestrings.crc", "/strings/Dutch_wifistrings.crc", "/strings/Dutch_localizationStrings.crc", "/strings/Dutch_cutsceneStrings.crc", "/strings/Dutch_conversationStrings.crc", "/strings/Dutch_unlockablesStrings.crc", "/strings/Dutch_DisneyStrings.crc", "/strings/Danish_dialogstrings.crc", "/strings/Danish_adhocstrings.crc", "/strings/Danish_gamestrings.crc", "/strings/Danish_downloadstrings.crc", "/strings/Danish_savegamestrings.crc", "/strings/Danish_wifistrings.crc", "/strings/Danish_localizationStrings.crc", "/strings/Danish_cutsceneStrings.crc", "/strings/Danish_conversationStrings.crc", "/strings/Danish_unlockablesStrings.crc", "/strings/Danish_DisneyStrings.crc", "/strings/Japanese_dialogstrings.crc", "/strings/Japanese_adhocstrings.crc", "/strings/Japanese_gamestrings.crc", "/strings/Japanese_downloadstrings.crc", "/strings/Japanese_savegamestrings.crc", "/strings/Japanese_wifistrings.crc", "/strings/Japanese_localizationStrings.crc", "/strings/Japanese_cutsceneStrings.crc", "/strings/Japanese_conversationStrings.crc", "/strings/Japanese_unlockablesStrings.crc", "/strings/Japanese_DisneyStrings.crc", "/strings/Chinese_dialogstrings.crc", "/strings/Chinese_adhocstrings.crc", "/strings/Chinese_gamestrings.crc", "/strings/Chinese_downloadstrings.crc", "/strings/Chinese_savegamestrings.crc", "/strings/Chinese_wifistrings.crc", "/strings/Chinese_localizationStrings.crc", "/strings/Chinese_cutsceneStrings.crc", "/strings/Chinese_conversationStrings.crc", "/strings/Chinese_unlockablesStrings.crc", "/strings/Chinese_DisneyStrings.crc", "/strings/Korean_dialogstrings.crc", "/strings/Korean_adhocstrings.crc", "/strings/Korean_gamestrings.crc", "/strings/Korean_downloadstrings.crc", "/strings/Korean_savegamestrings.crc", "/strings/Korean_wifistrings.crc", "/strings/Korean_localizationStrings.crc", "/strings/Korean_cutsceneStrings.crc", "/strings/Korean_conversationStrings.crc", "/strings/Korean_unlockablesStrings.crc", "/strings/Korean_DisneyStrings.crc", "/strings/Hangul_dialogstrings.crc", "/strings/Hangul_adhocstrings.crc", "/strings/Hangul_gamestrings.crc", "/strings/Hangul_downloadstrings.crc", "/strings/Hangul_savegamestrings.crc", "/strings/Hangul_wifistrings.crc", "/strings/Hangul_localizationStrings.crc", "/strings/Hangul_cutsceneStrings.crc", "/strings/Hangul_conversationStrings.crc", "/strings/Hangul_unlockablesStrings.crc", "/strings/Hangul_DisneyStrings.crc", "/strings/Norwegian_dialogstrings.crc", "/strings/Norwegian_adhocstrings.crc", "/strings/Norwegian_gamestrings.crc", "/strings/Norwegian_downloadstrings.crc", "/strings/Norwegian_savegamestrings.crc", "/strings/Norwegian_wifistrings.crc", "/strings/Norwegian_localizationStrings.crc", "/strings/Norwegian_cutsceneStrings.crc", "/strings/Norwegian_conversationStrings.crc", "/strings/Norwegian_unlockablesStrings.crc", "/strings/Norwegian_DisneyStrings.crc", "/strings/Swedish_dialogstrings.crc", "/strings/Swedish_adhocstrings.crc", "/strings/Swedish_gamestrings.crc", "/strings/Swedish_downloadstrings.crc", "/strings/Swedish_savegamestrings.crc", "/strings/Swedish_wifistrings.crc", "/strings/Swedish_localizationStrings.crc", "/strings/Swedish_cutsceneStrings.crc", "/strings/Swedish_conversationStrings.crc", "/strings/Swedish_unlockablesStrings.crc", "/strings/Swedish_DisneyStrings.crc", "/strings/UKEnglish_dialogstrings.crc", "/strings/UKEnglish_adhocstrings.crc", "/strings/UKEnglish_gamestrings.crc", "/strings/UKEnglish_downloadstrings.crc", "/strings/UKEnglish_savegamestrings.crc", "/strings/UKEnglish_wifistrings.crc", "/strings/UKEnglish_localizationStrings.crc", "/strings/UKEnglish_cutsceneStrings.crc", "/strings/UKEnglish_conversationStrings.crc", "/strings/UKEnglish_unlockablesStrings.crc", "/strings/UKEnglish_DisneyStrings.crc", "/strings/dialogstrings.crc", "/strings/adhocstrings.crc", "/strings/adhocStrings.crc", "/strings/gamestrings.crc", "/strings/downloadstrings.crc", "/strings/downloadstrings.crc", "/strings/savegamestrings.crc", "/strings/wifistrings.crc", "/strings/localizationStrings.crc", "/strings/cutsceneStrings.crc", "/strings/conversationStrings.crc", "/strings/unlockablesStrings.crc", "/strings/DisneyStrings.crc", "/strings/disneyStrings.crc", "/strings/Credits.crc", "/strings/credits.crc", "/PC/Spryte/cursor", "/UI/puffleCursor", "/UI/SpyPod/Cursors/mechanoduster", "/UI/SpyPod/Cursors/decoder", "/UI/SpyPod/Cursors/scissors", "/UI/SpyPod/Cursors/comb", "/UI/SpyPod/Cursors/wrench", "/UI/SpyPod/Cursors/flashlight", "/UI/InventoryPanel/Oberry", "/Tools/brokenImage", "/Particles/sparkle1", "/Particles/sparkle2", "/Particles/sparkle3", "/UI/buttonBig", "/fonts/faceFront.fnt", "/UI/MainMenu/Title_JetPackguy", "/UI/MainMenu/Title_GreenPuffle", "/UI/MainMenu/Title_Bottom", "/bg/Cutscenes/CPLogo", "/bg/Microgames/MicrogameTS", "/UI/MainMenu/Title_BttnDGamer", "/UI/MainMenu/Title_BttnMicrophone", "/UI/MainMenu/Title_BttnPlay", "/WifiLogos/wireless_strength_level_w", "/WifiLogos/Wi-Fi_strength_level_w", "/sound/sfx.bin", "/sound/music.bin", "/UI/chkmarkBttnBlue", "/fonts/MackFont.fnt", "/fonts/faceFront.fnt", "/UI/loading", "/Downloads.rdt", "/download.arc", "/UI/SpyPod/spypod", "/Particles/Fire", "/Particles/droplet", "/Particles/debris1", "/Particles/debris2", "/Minigames/Labyrinth/Coin", "/fonts/SnowFort.fnt", "/3d/SuperRobot/animTextures/puf_ski_2", "/3d/SuperRobot/animTextures/puf_ski_3", "/3d/SuperRobot/animTextures/puf_ski_1", "/3d/SuperRobot/animTextures/puf_dock_1", "/3d/SuperRobot/animTextures/puf_dock_2", "/3d/SuperRobot/animTextures/puf_dock_3", "/3d/SuperRobot/animTextures/puf_tall2_3", "/3d/SuperRobot/animTextures/puf_beach_1", "/3d/SuperRobot/animTextures/puf_beach_2", "/3d/SuperRobot/animTextures/puf_beach_3", "/3d/SuperRobot/animTextures/puf_tall2_1", "/3d/SuperRobot/animTextures/puf_tall2_2", "/3d/SuperRobot/animTextures/SRjetpackflame1", "/3d/SuperRobot/animTextures/SRjetpackflame2", "/3d/SuperRobot/animTextures/SRjetpackflame3", "/3d/SuperRobot/animTextures/SRjetpackflame4", "/3d/SuperRobot/SuperRobotPufflesTall2", "/3d/SuperRobot/SuperRobot_jetpackFlame", "/3d/SuperRobot/SuperRobotPufflesDock", "/3d/SuperRobot/SuperRobotPufflesSki", "/3d/SuperRobot/SuperRobotPufflesBeach", "/bg/MissionHUD_BG", "/chunks/", "/Particles/Puffles/RedHit", "/Particles/Puffles/BlackHit", "/Particles/Puffles/PurpleHit", "/Particles/Puffles/PinkHit", "/Particles/Puffles/YellowHit", "/Particles/Puffles/WhiteHit", "/Particles/Puffles/KlutzyHit", "/UI/LevelSelect/", "/NPC/M4/Dot/", "/NPC/M3/Dot/", "/NPC/M4/Rookie/", "/NPC/Puffles/Blue/", "/MissionObjects/M4/", "/Objects/CommandCoach/", "/Location/MineExterior/state/", "/Particles/Gesture/sparkle1", "/Particles/Gesture/sparkle2", "/Particles/Gesture/sparkle3", "/Particles/commandCoach", "/UI/noBadge", "/fonts/Burbank.fnt", "/UI/coin", "/UI/PlayerCard/NewPlayerCard/card", "/UI/clothingBttnExit", "/UI/badge1", "/UI/badge2", "/UI/badge3", "/UI/badge4", "/UI/badge5", "/UI/DressUpCard/NewDressUpCard/dress", "/UI/PaperDoll/", "/csv/ClothingData.dat", "/UI/DressUpCard/NewDressUpCard/tabL", "/UI/scrollbar_bg", "/UI/scrollbar_marker", "/UI/scrollbar_arrow_top", "/UI/scrollbar_arrow_bottom", "/fonts/faceFront.fnt", "/UI/WarningDialog/warningDialog", "/UI/Zoom/Diploma", "/bg/colors", "/UI/DrawingPanel/brown", "/UI/DrawingPanel/lightGreen", "/UI/DrawingPanel/green", "/UI/DrawingPanel/blue", "/UI/DrawingPanel/lightBlue", "/UI/DrawingPanel/red", "/UI/DrawingPanel/orange", "/UI/DrawingPanel/purple", "/UI/DrawingPanel/lightPurple", "/UI/DrawingPanel/pink", "/UI/DrawingPanel/paleOrange", "/UI/DrawingPanel/yellow", "/UI/DrawingPanel/white", "/UI/DrawingPanel/black", "/UI/ConversationSystem/borderpanels/npcChatBubble_tail_down", "/UI/ConversationSystem/borderpanels/npcChatBubble_tail", "/UI/PaperDoll/penguin/penguinMotion", "/UI/PaperDoll/penguin/body", "/palettes/", "/UI/ConversationSystem/", "/UI/ConversationSystem/borderpanels/Player/DialogButton/DialogBttn", "/UI/ConversationSystem/borderpanels/Player/DialogButton/DialogBttn", "/UI/ConversationSystem/borderpanels/Player/DialogButton/DialogBttn", "/fonts/bigUI.fnt", "/UI/ConversationSystem/borderpanels/npcChatBubblebrdr", "/UI/Zoom/", "/UI/UtilityBorder/Corner_TL", "/UI/UtilityBorder/Corner_TR", "/UI/UtilityBorder/Corner_BL", "/UI/UtilityBorder/Corner_BR", "/UI/UtilityBorder/BorderPanel", "/UI/UtilityBorder/BorderPanel_side", "/UI/UtilityBorder/Bttn_Spypod", "/UI/UtilityBorder/Bttn_Map", "/UI/UtilityBorder/Bttn_Inventory", "/UI/UtilityBorder/Bttn_Puffle", "/UI/InventoryPanel/InventoryBorderPanels/inventory", "/UI/UtilityBorder/Bttn_NoPuffle", "/UI/UtilityBorder/Map_Selected", "/UI/UtilityBorder/Spypod_Selected", "/UI/UtilityBorder/Puffle_Selected", "/UI/ConversationSystem/borderpanels/Player/DialogButton/DialogBttn", "/UI/noMap", "/UI/UtilityBorder/Inventory_Selected", "/UI/InventoryPanel/InventoryBorderPanels/current", "/UI/InventoryPanel/Bttn_DressUp", "/UI/inventoryBttnExit", "/UI/InventoryPanel/InventoryBorderPanels/selected", "/UI/PaperDoll/penguin/penguinMotion", "/UI/PaperDoll/penguin/body", "/UI/ConversationSystem/talkbubble_Indicator", "/NPC/CommandCoach/CommandCoach_Clothing", "/palettes/", "/UI/PaperDoll/spypod/spypod", "/UI/ConversationSystem/borderpanels/Player/DialogButton/DialogBttn", "/fonts/Burbank.fnt", "/fonts/smallUI.fnt", "/UI/coin", "/UI/HUDlocation/beach0", "/UI/CommandCoach/Penguin", "/UI/CommandCoach/HudIconBack", "/UI/HUDlocation/", "/NPC/Puffles/", "/Blue/BluePuffle", "/Red/RedPuffle", "/Black/BlackPuffle", "/Purple/PurplePuffle", "/Pink/PinkPuffle", "/Green/GreenPuffle", "/Yellow/YellowPuffle", "/White/WhitePuffle", "/Klutzy/Klutzy", "/UI/Zoom/", "/palettes/coach_", "/UI/SignalTracking/Signal_x", "/UI/SignalTracking/Bars_1", "/UI/SignalTracking/Bars_2", "/UI/SignalTracking/Bars_3", "/UI/SignalTracking/Bars_4", "/UI/SignalTracking/Bars_5", "/UI/SignalTracking/Signal_on", "/UI/SignalTracking/Signal_right", "/UI/SignalTracking/Signal_left", "/UI/SpyGadgetPanel/BorderPanels/Spygadget", "/UI/SpyGadgetPanel/Code", "/UI/SpyGadgetPanel/Communicator", "/UI/SpyGadgetPanel/Duster", "/UI/SpyGadgetPanel/HQ", "/UI/SpyGadgetPanel/Quest", "/UI/SpyGadgetPanel/Scissors", "/UI/SpyGadgetPanel/Wrench", "/UI/SpyGadgetPanel/Comb", "/UI/SpyGadgetPanel/Instructions", "/UI/SpyGadgetPanel/Snake", "/fonts/bigUI.fnt", "/UI/ConversationSystem/elipsisIcon", "/UI/ConversationSystem/borderpanels/Player/DialogButton/thoughtbubble_tail", "/UI/ConversationSystem/borderpanels/Player/DialogButton/DialogButton_tail", "/UI/buttonBig", "/fonts/Burbank.fnt", "/UI/CommandCoach/Instructions_avitar", "/UI/HelpMenus/StylusDrag", "/UI/arrowForward", "/UI/arrowBack", "/bg/CommandCoach/CommandCoachTitleScreen", "/bg/Microgames/MicrogameTS", "/bg/CommandCoach/Instructions/CommandCoach_1", "/bg/CommandCoach/Instructions/CommandCoach_2", "/CutsceneAnims/CommandCoach_2", "/bg/CommandCoach/Instructions/CommandCoach_3", "/CutsceneAnims/CommandCoach_3", "/bg/CommandCoach/Instructions/CommandCoach_4", "/CutsceneAnims/CommandCoach_4", "/bg/CommandCoach/Instructions/CommandCoach_5", "/CutsceneAnims/CommandCoach_5", "/bg/CommandCoach/Instructions/CommandCoach_6", "/fonts/Burbank.fnt", "/UI/buttonBig", "/fonts/Burbank.fnt", "/bg/CreditsTop", "/bg/CreditsBottom", "/UI/buttonBig", "/fonts/faceFront.fnt", "/Minigames/Common/HUD/hudPanelSmall", "/bg/Cutscenes/CPLogo", "/bg/Microgames/MicrogameTS", "/UI/buttonBig", "/fonts/faceFront.fnt", "/fonts/smallUI.fnt", "/bg/Microgames/MicrogameTS", "/UI/buttonBig", "/fonts/faceFront.fnt", "/fonts/smallUI.fnt", "/UI/MainMenu/creditsBttn", "/bg/Cutscenes/CPLogo", "/bg/Microgames/MicrogameTS", "/UI/buttonBig", "/fonts/faceFront.fnt", "/UI/arrowBack", "/UI/buttonWifiNarrow", "/fonts/Burbank.fnt", "/bg/Menus/MultiplayerMenuTop", "/bg/Microgames/MicrogameTS", "/UI/Multiplayer/hostBttn", "/UI/Multiplayer/join2pBttn", "/UI/arrowBack", "/UI/LevelSelect/", "/UI/LevelSelect/HUD/MapBack", "/fonts/DSdigital.fnt", "/UI/LevelSelect/penguin", "/UI/LevelSelect/puffle", "/UI/LevelSelect/HUD/playerflag", "/UI/LevelSelect/klutzy", "/HUD/", "/UI/LevelSelect/level_hotspot_up", "/UI/LevelSelect/icons/", "/UI/LevelSelect/HUD/", "/UI/LevelSelect/teleportEffect", "/palettes/skiVillage.nbfp", "/palettes/mapPenguin_aqua.nbfp", "/palettes/mapPenguin_black.nbfp", "/palettes/mapPenguin_blue.nbfp", "/palettes/mapPenguin_brown.nbfp", "/palettes/mapPenguin_darkGreen.nbfp", "/palettes/mapPenguin_fuschia.nbfp", "/palettes/mapPenguin_green.nbfp", "/palettes/mapPenguin_lime.nbfp", "/palettes/mapPenguin_orange.nbfp", "/palettes/mapPenguin_peach.nbfp", "/palettes/mapPenguin_purple.nbfp", "/palettes/mapPenguin_red.nbfp", "/palettes/mapPenguin_yellow.nbfp", "/palettes/mapPenguin_pink.nbfp", "/UI/buttonBig", "/fonts/faceFront.fnt", "/UI/MainMenu/MainMenu_1player", "/UI/MainMenu/MainMenu_DGamer", "/UI/MainMenu/MainMenu_Options", "/UI/MainMenu/MainMenu_2player", "/UI/arrowBack", "/UI/MainMenu/creditsBttn", "/fonts/Burbank.fnt", "/bg/Microgames/MicrogameTS", "/UI/MainMenu/creditsBttn", "/fonts/smallUI.fnt", "/fonts/Burbank.fnt", "/fonts/SmallUI.fnt", "/BG/MissionHUD_BG", "/UI/MainMenu/epfBadge", "/UI/Menus/playerCreate_coin", "/UI/Menus/playerCreate_progressBar", "/UI/Menus/playerCreate_progressMeter", "/UI/buttonBig", "/fonts/Burbank.fnt", "/bg/Cutscenes/CPLogo", "/bg/Microgames/MicrogameTS", "/UI/arrowBack", "/UI/MicMenu/mic_volume", "/UI/MicMenu/mic_bar", "/UI/MicMenu/FX_slider", "/UI/MicMenu/FX_bar", "/WifiLogos/loading", "/UI/buttonBig", "/fonts/faceFront.fnt", "/fonts/smallUI.fnt", "/UI/MainMenu/creditsBttn", "/bg/Cutscenes/CPLogo", "/bg/Microgames/MicrogameTS", "/UI/buttonBig", "/fonts/faceFront.fnt", "/bg/Cutscenes/CPLogo", "/bg/Microgames/MicrogameTS", "/UI/buttonMedium", "/fonts/faceFront.fnt", "/fonts/Burbank.fnt", "/fonts/Burbank.fnt", "/UI/MinigameMenu/prevArrow", "/UI/MinigameMenu/nextArrow", "/UI/MinigameMenu/skipButton", "/UI/MinigameMenu/playButton", "/UI/MinigameMenu/instructionsButton", "/UI/arrowBack", "/UI/MinigameMenu/", "/french/", "/spanish/", "/UI/MinigameMenu/medalSilver", "/UI/MinigameMenu/medalGold", "/UI/MinigameMenu/medalSlot", "/bg/minigameMissions_top", "/bg/AquaGrabber/AquaGrabberMissions_BS", "/bg/AquaGrabber/AquaGrabber_TS", "/bg/AquaGrabber/AquaGrabber_BS", "/bg/Jackhammer/titleScreenMissions_bottom", "/bg/Jackhammer/titleScreen_top", "/bg/Jackhammer/titleScreen_top_02", "/bg/Jackhammer/titleScreen_bottom", "/bg/Jackhammer/titleScreen_bottom_02", "/bg/Grapple/TitleScreenBottomMissions", "/bg/Grapple/TitleScreen", "/bg/Grapple/TitleScreenBottom", "/bg/Labyrinth/titleScreenMissions_bottom", "/bg/Labyrinth/titleScreen_top", "/bg/Labyrinth/titleScreen_bottom", "/UI/buttonBig", "/fonts/faceFront.fnt", "/fonts/Burbank.fnt", "/bg/Cutscenes/CPLogo", "/bg/Microgames/MicrogameTS", "/UI/MinigameMenu/LabyrinthButton", "/UI/MinigameMenu/JackhammerButton", "/UI/MinigameMenu/GrapplingButton", "/UI/MinigameMenu/AquaGrabberButton", "/UI/arrowBack", "/UI/buttonSmall", "/fonts/Burbank.fnt", "/UI/buttonMedium", "/fonts/Burbank.fnt", "/fonts/Burbank.fnt", "/fonts/ComicrazyTitle.fnt", "/UI/MinigameMenu/", "/french/", "/spanish/", "/UI/MinigameMenu/medalSilver", "/UI/MinigameMenu/medalGold", "/UI/MinigameMenu/medalSlot", "/bg/minigameMissions_top", "/bg/AquaGrabber/AquaGrabberMissions_BS", "/bg/AquaGrabber/AquaGrabber_TS", "/bg/AquaGrabber/AquaGrabber_BS", "/bg/Jackhammer/titleScreenMissions_bottom", "/bg/Jackhammer/titleScreen_top", "/bg/Jackhammer/titleScreen_bottom", "/bg/Grapple/TitleScreenBottomMissions", "/bg/Grapple/TitleScreen", "/bg/Grapple/TitleScreenBottom", "/bg/Labyrinth/titleScreenMissions_bottom", "/bg/Labyrinth/titleScreen_top", "/bg/Labyrinth/titleScreen_bottom", "/UI/MinigameResultsMenu/", "/bg/MissionSelector/missionSelectTop_M6", "/bg/MissionSelector/missionSelectTop_M8", "/bg/MissionSelector/missionSelectTop_M9", "/bg/MissionSelector/missionSelectBottom", "/bg/MissionSelector/missionSelectTop_M5", "/bg/MissionSelector/missionSelectTop_CH1", "/bg/MissionSelector/missionSelectTop_M10", "/bg/MissionSelector/missionSelectTop_M11", "/bg/MissionSelector/missionSelectTop_CH2", "/bg/MissionSelector/missionSelectTop_CH3", "/bg/MissionSelector/missionSelectTop_CH4", "/bg/MissionSelector/missionSelectBottomVR", "/UI/buttonDebug", "/fonts/faceFront.fnt", "/UI/MissionSelector/Bttn_newLaunch", "/UI/MissionSelector/Bttn_next", "/UI/MissionSelector/Bttn_previous", "/UI/backBttn", "/fonts/Burbank.fnt", "/UI/blackPixel", "/UI/MissionSelector/snakeIcon", "/UI/MissionSelector/clothingIcon_01", "/UI/MissionSelector/M1polaroid_01", "/UI/MissionSelector/M1polaroid_02", "/BG/MissionSelector/missionSelectTop_Classified", "/UI/MissionSelector/M", "/UI/MissionSelector/clothingIcon_", "/UI/missionAccomplished", "/UI/MissionSelector/clothingItem_", "/UI/SpyPod/Decoder/bttnExit", "/UI/SpyPod/SpyLog/scrollbar_marker", "/UI/SpyPod/SpyLog/scrollbar_arrow_top", "/UI/SpyPod/SpyLog/scrollbar_arrow_bottom", "/UI/SpyPod/SpyLog/frame", "/UI/SpyPod/SpyLog/dataFlow", "/UI/SpyPod/SpyLog/dataMapBlinks", "/fonts/Burbank.fnt", "/BG/SpyLog/Spylog_top", "/BG/SpyLog/Spylog_bottom", "/UI/SpyPod/SpyLog/checkbox_empty", "/UI/SpyPod/SpyLog/checkbox_marked", "/UI/SpyPod/SpyLog/subbullet", "/UI/SpyPod/SpyLog/subbullet_marked", "/UI/buttonBig", "/fonts/faceFront.fnt", "/fonts/Burbank.fnt", "/UI/Menus/buttonLong", "/UI/arrowBack", "/UI/MicMenu/FX_bar", "/UI/MicMenu/FX_slider", "/fonts/burbank.fnt", "/bg/Cutscenes/CPLogo", "/bg/Microgames/MicrogameTS", "/UI/buttonSmall", "/fonts/Burbank.fnt", "/UI/buttonBig", "/fonts/faceFront.fnt", "/fonts/Burbank.fnt", "/bg/Cutscenes/CPLogo", "/bg/Microgames/MicrogameTS", "/UI/ProfileCreation/lightblue", "/UI/ProfileCreation/black", "/UI/ProfileCreation/blue", "/UI/ProfileCreation/brown", "/UI/ProfileCreation/darkgreen", "/UI/ProfileCreation/pink", "/UI/ProfileCreation/green", "/UI/ProfileCreation/limegreen", "/UI/ProfileCreation/orange", "/UI/ProfileCreation/peach", "/UI/ProfileCreation/purple", "/UI/ProfileCreation/red", "/UI/ProfileCreation/yellow", "/UI/ProfileCreation/bttn_select", "/UI/buttonBig", "/fonts/Burbank.fnt", "/UI/MainMenu/IDCardButton", "/UI/MainMenu/IDCardButtonBig", "/UI/noBadge", "/UI/coin", "/fonts/smallUI.fnt", "/bg/Cutscenes/CPLogo", "/bg/Microgames/MicrogameTS", "/UI/badge", "/UI/buttonBig", "/fonts/Burbank.fnt", "/bg/Cutscenes/CPLogo", "/bg/Microgames/MicrogameTS", "/csv/asciikeymap.dat", "/UI/ProfileCreation/NameField", "/UI/backBttn", "/UI/SpyPod/Decoder/bttnExit", "/fonts/Comicrazy.fnt", "/BG/SpyPod/SpyPod_bottom", "/UI/SpyPod/btn_comb", "/UI/SpyPod/btn_duster", "/UI/SpyPod/btn_flash", "/UI/SpyPod/btn_hq", "/UI/SpyPod/btn_code", "/UI/SpyPod/btn_phone", "/UI/SpyPod/btn_scissors", "/UI/SpyPod/btn_wrench", "/UI/SpyPod/btn_quest", "/UI/buttonBig", "/fonts/Burbank.fnt", "/UI/WifiContent/Login_penguin", "/UI/exitBttn", "/UI/ProfileCreation/bttn_select", "/bg/WifiContent/Wifi_Login_top", "/bg/Microgames/MicrogameTS", "/UI/WifiContent/Bttn_LoginUsername", "/UI/WifiContent/Bttn_LoginPassword", "/fonts/smallUI.fnt", "/csv/asciikeymap.dat", "/UI/buttonBig", "/fonts/Burbank.fnt", "/UI/WifiContent/CoinUpload/CoinUpload_penguin", "/UI/WifiContent/TransferProgress", "/UI/WifiContent/CoinUpload/Plus", "/UI/WifiContent/CoinUpload/Minus", "/UI/checkBttn", "/UI/exitBttn", "/UI/WifiContent/CoinUpload/CoinPile", "/UI/WifiContent/CoinUpload/MoneyBag", "/bg/WifiContent/Wifi_Login_top", "/bg/wificontent/coinupload/coinupload_bottom", "/fonts/smallUI.fnt", "/UI/buttonBig", "/fonts/Burbank.fnt", "/UI/WifiContent/Login_penguin", "/BG/WifiContent/Wifi_Login_top", "/bg/Microgames/MicrogameTS", "/UI/WifiContent/Account", "/UI/WifiContent/Coins", "/UI/exitBttn", "/UI/Menus/buttonHuge", "/fonts/Burbank.fnt", "/bg/Microgames/MicrogameTS", "/UI/exitBttn", "/UI/buttonBig", "/fonts/Comicrazy.fnt", "/fonts/faceFront.fnt", "/bg/CommandCoach/CommandCoachTitleScreen", "/bg/Microgames/MicrogameTS", "/UI/buttonBig", "/fonts/Comicrazy.fnt", "/fonts/Burbank.fnt", "/bg/Menus/MultiplayerMenuTop", "/bg/Microgames/MicrogameTS", "/UI/Menus/download", "/UI/Menus/commandCoach", "/UI/arrowBack", "/multiboot/tuxedo.nbfc", "/multiboot/tuxedo.nbfp", "/multiboot/jackhammer-signed.srl", "/WifiLogos/loading", "/UI/WifiContent/buttonHostJoin", "/fonts/smallUI.fnt", "/fonts/Burbank.fnt", "/bg/Menus/MultiplayerMenuTop", "/bg/Microgames/MicrogameTS", "/UI/arrowBack", "/UI/Zoom/", "/game.arc", "/common.arc", "/chunks.arc", "/fonts/smallUI.fnt", "/common.rdt", "/game.rdt", "/Minigames/Aquagrabber/HudCoin_idle", "/Minigames/Aquagrabber/HudTube_idle", "/Minigames/Aquagrabber/dangerArrows", "/Minigames/Aquagrabber/grid", "/Minigames/Aquagrabber/grid2", "/fonts/Burbank.fnt", "/UI/winMinigame", "/UI/loseMinigame", "/Minigames/Aquagrabber/ammo", "/Minigames/Aquagrabber/coin", "/Minigames/Aquagrabber/whaleButton_up", "/Minigames/Aquagrabber/octopusButton_up", "/Minigames/Aquagrabber/stalactite1", "/Minigames/Aquagrabber/stalactite2", "/Minigames/Aquagrabber/stalactite3", "/Minigames/Aquagrabber/spinyFish", "/Minigames/Aquagrabber/eel", "/Minigames/Aquagrabber/bubble1", "/Minigames/Aquagrabber/bubble2", "/Minigames/Aquagrabber/bubble3", "/Minigames/Aquagrabber/whaleButton_charge", "/Minigames/Aquagrabber/octopusButton_charge", "/Minigames/Aquagrabber/tube2", "/Minigames/Aquagrabber/slowdown", "/Minigames/Aquagrabber/slowdown_collected", "/Minigames/Aquagrabber/coin_large", "/Minigames/Aquagrabber/tube3", "/BG/Microgames/Balance/Forest", "/BG/Microgames/Balance/topScreen_BG", "/Microgames/Balance/snowfortCircle", "/Microgames/Balance/forestCircle", "/Microgames/Balance/plazaCircle", "/UI/bal_normal", "/UI/bal_pirate", "/UI/bal_sensei", "/UI/bal_dj", "/body/costumeCadence_idle", "/body/costumeCadenceScarf_idle", "/body/costumeCadenceShoes_idle", "/body/costumeSensei_idle", "/body/costumeRockhopper_idle", "/body/costumeRockhopperHat_idle", "/fonts/Comicrazy.fnt", "/fonts/smallUI.fnt", "/fonts/SmallUI.fnt", "/fonts/ComicrazyTitle.fnt", "/BG/Microgames/Balance/Forest", "/BG/Microgames/Balance/SnowForts", "/BG/Microgames/Balance/Plaza", "/Microgames/Balance/base_selection", "/Microgames/Balance/fan_S", "/Microgames/Balance/fan_R", "/Microgames/Balance/fan_C", "/Microgames/Balance/fan_RandS", "/Microgames/Balance/fan_CandR", "/Microgames/Balance/fan_SandC", "/BG/Microgames/MicrogameTS", "/BG/Microgames/BarBending/BarBending", "/fonts/burbank.fnt", "/Microgames/BarBending/jetpackON", "/Microgames/BarBending/bars", "/Microgames/BarBending/belt", "/Microgames/BarBending/beltBack", "/Microgames/BarBending/Arrow", "/fonts/ComicrazyTitle.fnt", "/Microgames/BarBending/jetpack", "/BG/Microgames/MicrogameTS", "/bg/Microgames/ShakeBarrel/SB_BS", "/fonts/burbank.fnt", "/Microgames/ShakeBarrel/BarrelStill", "/Microgames/ShakeBarrel/BarrelShake", "/BG/Microgames/MicrogameTS", "/BG/Microgames/Beaker/BeakersBG", "/fonts/burbank.fnt", "/Microgames/Beaker/Beaker8", "/Microgames/Beaker/Beaker5", "/Microgames/Beaker/Beaker3", "/Microgames/Beaker/helpScroll", "/Microgames/Beaker/helpBottle_scroll", "/Microgames/Beaker/BeakerPour", "/Microgames/Beaker/Beaker8_glow", "/Microgames/Beaker/Beaker5_glow", "/Microgames/Beaker/Beaker3_glow", "/fonts/ComicrazyTitle.fnt", "/Microgames/Beaker/helpBottle_empty", "/BG/Microgames/MicrogameTS", "/bg/Microgames/PipeDreams/PD_BG", "/fonts/burbank.fnt", "/Microgames/PipeDreams/MeterRed", "/fonts/ComicrazyTitle.fnt", "/Microgames/PipeDreams/MeterGoDown", "/Microgames/PipeDreams/StraightAcross", "/Microgames/PipeDreams/StraightUp", "/Microgames/PipeDreams/Elbow1", "/Microgames/PipeDreams/Elbow4", "/Microgames/PipeDreams/Elbow3", "/Microgames/PipeDreams/Elbow2", "/bg/Microgames/ColorLock", "/BG/Microgames/MicrogameTS", "/fonts/burbank.fnt", "/Microgames/ColorWheel/CL_Open", "/fonts/ComicrazyTitle.fnt", "/Microgames/ColorWheel/CL_TopLeft_Out", "/Microgames/ColorWheel/CL_TopRight_Out", "/Microgames/ColorWheel/CL_bttmRight_Out", "/Microgames/ColorWheel/CL_bttmleft_Out", "/Microgames/ColorWheel/CL_TopLeft_Med", "/Microgames/ColorWheel/CL_TopLeft_In", "/Microgames/ColorWheel/CL_TopRight_Med", "/Microgames/ColorWheel/CL_TopRight_In", "/Microgames/ColorWheel/CL_bttmRight_Med", "/Microgames/ColorWheel/CL_bttmRight_In", "/Microgames/ColorWheel/CL_BttmLeft_Med", "/Microgames/ColorWheel/CL_BttmLeft_In", "/Microgames/ColorWheel/CL_Locked", "/UI/SpyPod/communicatorButton", "/fonts/Burbank.fnt", "/UI/stylusTap", "/UI/SpyPod/Communicator/selector", "/UI/SpyPod/Communicator/selector_center", "/fonts/BurbankSmall.fnt", "/BG/SpyPod/comunicator_top", "/BG/SpyPod/comunicator_bottom", "/UI/SpyPod/Decoder/bttnExit", "/BG/SpyPod/SpyPod_bottom", "/UI/SpyPod/Communicator/Dot", "/UI/SpyPod/Communicator/Gary", "/UI/SpyPod/Communicator/JetPackGuy", "/UI/SpyPod/Communicator/Rookie", "/UI/SpyPod/Communicator/PuffleHandler", "/UI/SpyPod/Communicator/Director", "/UI/SpyPod/Communicator/Static", "/UI/SpyPod/Communicator/Herbert", "/UI/SpyPod/Communicator/GaryWorried", "/UI/SpyPod/Decoder/bttnExit", "/UI/SpyPod/Decoder/bttnEraser", "/UI/SpyPod/Decoder/bttnPencil", "/bg/SpyPod/Decoder/Decoder_top", "/bg/SpyPod/Decoder/Decoder_bottom", "/fonts/DSdigitalCode.fnt", "/UI/SpyPod/Decoder/window", "/UI/SpyPod/Decoder/coverAll", "/fonts/DSdigitalCode.fnt", "/BG/Microgames/MicrogameTS", "/Microgames/DVDCleaning/Disc", "/Microgames/DVDCleaning/Spray", "/Microgames/DVDCleaning/scratches", "/Microgames/DVDCleaning/cleaningRag", "/Microgames/DVDCleaning/cleaningBottle", "/bg/Microgames/DVDCleaning/DVDCleaningBG", "/fonts/burbank.fnt", "/Microgames/DVDCleaning/poof", "/fonts/ComicrazyTitle.fnt", "/Microgames/Gears/Peg", "/Microgames/Gears/Gear", "/BG/Microgames/MicrogameTS", "/bg/Microgames/Gears/Gears_BG", "/Microgames/Gears/Sproket1", "/Microgames/Gears/Sproket2", "/fonts/burbank.fnt", "/fonts/ComicrazyTitle.fnt", "/Minigames/GrapplingHook/grapplingHook", "/Minigames/GrapplingHook/grapplingHook_idle", "/Minigames/GrapplingHook/grapplingHook2_idle", "/Minigames/GrapplingHook/grapplingHook3_idle", "/Minigames/GrapplingHook/grapplingHook4_idle", "/Minigames/GrapplingHook/grapplingHook_attached", "/Minigames/GrapplingHook/grapplingHook2_attached", "/Minigames/GrapplingHook/grapplingHook3_attached", "/Minigames/GrapplingHook/grapplingHook4_attached", "/Minigames/GrapplingHook/checkpointActive", "/Minigames/GrapplingHook/checkpointActivate", "/Minigames/GrapplingHook/checkpointInactive", "/Minigames/Labyrinth/Coin", "/Minigames/Labyrinth/Coin_10", "/Minigames/GrapplingHook/extraLife", "/Minigames/GrapplingHook/extraLifeClaw", "/Minigames/GrapplingHook/pizzaPickup", "/Minigames/GrapplingHook/stalagmite1", "/Minigames/GrapplingHook/stalagmite2", "/Minigames/GrapplingHook/horizHazard", "/Minigames/GrapplingHook/vertHazard", "/Minigames/GrapplingHook/snowballShooter", "/Minigames/GrapplingHook/collapsePlatform", "/Minigames/GrapplingHook/checkpointInactive", "/Minigames/GrapplingHook/finish", "/Minigames/Labyrinth/Coin", "/Minigames/GrapplingHook/pizzaPickup", "/Particles/aquabubble", "/Minigames/Common/HUD/hudPanelSmall", "/Minigames/Labyrinth/Coin", "/Minigames/GrapplingHook/extraLifeUI", "/Minigames/GrapplingHook/pizzaHUD", "/fonts/Burbank.fnt", "/Minigames/GrapplingHook/penguinIdle", "/Minigames/GrapplingHook/extraLifeUIClaw", "/UI/winMinigame", "/UI/loseMinigame", "/Minigames/GrapplingHook/grapplingHook", "/fonts/smallUI.fnt", "/Minigames/GrapplingHook/penguinIdle", "/UI/whitePixel", "/palettes/jetpack_", "/Minigames/GrapplingHook/penguinTeeter", "/Minigames/GrapplingHook/penguinWalk", "/Minigames/GrapplingHook/penguinJump_up", "/Minigames/GrapplingHook/penguinJump_peak", "/Minigames/GrapplingHook/penguinJump_down", "/Minigames/GrapplingHook/penguinShoot", "/Minigames/GrapplingHook/penguinSwing", "/Minigames/GrapplingHook/penguinSpin", "/Minigames/GrapplingHook/penguinIdle_TO", "/Minigames/GrapplingHook/penguinIdle_TO2", "/Minigames/GrapplingHook/penguinFallingToDeath", "/Minigames/GrapplingHook/penguinDie", "/Minigames/GrapplingHook/snowball", "/Minigames/GrapplingHook/start", "/BG/Microgames/MicrogameTS", "/Microgames/GridPuzzle/GridBox", "/BG/Microgames/Jigsaw/KeyPad_bottom", "/fonts/burbank.fnt", "/MissionObjects/UniversalItems/MapBackDown", "/MissionObjects/UniversalItems/MapBackUp", "/fonts/ComicrazyTitle.fnt", "/fonts/burbank.fnt", "/BG/Microgames/HB_TRAC/HTDot_BS", "/BG/Microgames/HB_TRAC/HTDot_TS", "/Microgames/HerbertTracking/Dot/DotRockTop", "/Microgames/HerbertTracking/Dot/DotSnowTop", "/Microgames/HerbertTracking/Dot/DotTreeTop", "/Microgames/HerbertTracking/Dot/DotRockMiddle", "/Microgames/HerbertTracking/Dot/DotSnowMiddle", "/Microgames/HerbertTracking/Dot/DotTreeMiddle", "/Microgames/HerbertTracking/Dot/DotRockBottom", "/Microgames/HerbertTracking/Dot/DotSnowBottom", "/Microgames/HerbertTracking/Dot/DotTreeBottom", "/Microgames/HerbertTracking/Dot/dotInstructions", "/fonts/Burbank.fnt", "/Microgames/HerbertTracking/Dot/Timer10", "/fonts/ComicrazyTitle.fnt", "/BG/Microgames/HB_TRAC/HTJPG_TS", "/BG/Microgames/HB_TRAC/HTJPG_BS", "/Microgames/HerbertTracking/JPG/target", "/fonts/burbank.fnt", "/Microgames/HerbertTracking/JPG/JPG", "/UI/blackPixel", "/UI/buttonMedium", "/fonts/Burbank.fnt", "/fonts/ComicrazyTitle.fnt", "/BG/Microgames/HB_TRAC/HTRookie_TS", "/BG/Microgames/HB_TRAC/HTRookie_BS", "/Microgames/HerbertTracking/Rookie/Arrow", "/Microgames/HerbertTracking/Rookie/Rookie", "/Microgames/HerbertTracking/Rookie/Rookie_lost", "/Microgames/HerbertTracking/Rookie/Rookie_pizza", "/Microgames/HerbertTracking/Rookie/Rookie_focus", "/Microgames/HerbertTracking/Rookie/Rookie_sleep", "/Microgames/HerbertTracking/Rookie/spinnerWheel", "/fonts/burbank.fnt", "/UI/buttonMedium", "/fonts/Burbank.fnt", "/fonts/ComicrazyTitle.fnt", "/Microgames/HerbertTracking/Rookie/", "/Particles/sparkle4", "/BG/Microgames/HB_TRAC/HT_TS", "/BG/Microgames/HB_TRAC/HT_BS", "/Microgames/HerbertTracking/HT/MapSpot_clue", "/Microgames/HerbertTracking/HT/MapSpot_noClue", "/Microgames/HerbertTracking/HT/MapSpot", "/Microgames/HerbertTracking/HT/StatusMeter", "/Microgames/HerbertTracking/status", "/fonts/Burbank.fnt", "/UI/ConversationSystem/borderpanels/npcChatBubblebrdr", "/Microgames/HerbertTracking/HT/clueMeterPulse", "/Microgames/HerbertTracking/HT/JPGIcon", "/Microgames/HerbertTracking/HT/RookieIcon", "/Microgames/HerbertTracking/HT/DotIcon", "/Microgames/HerbertTracking/jpgPortrait", "/Microgames/HerbertTracking/dotPortrait", "/Microgames/HerbertTracking/rookiePortrait", "/ / ", "/Microgames/JPG_Reel/Reel", "/BG/Microgames/MicrogameTS", "/Microgames/JPG_Reel/Arrow", "/BG/Microgames/JPG_Reel/reelBG_Bottom", "/fonts/burbank.fnt", "/Microgames/JPG_Reel/ArrowDummy", "/Microgames/JPG_Reel/jpg", "/Microgames/JPG_Reel/OverlayArrow", "/fonts/ComicrazyTitle.fnt", "/0/0", "/Minigames/Jackhammer/HudPanel", "/Minigames/Jackhammer/HudCoin_idle", "/0/", "/Minigames/Jackhammer/fuel", "/fonts/Burbank.fnt", "/Minigames/Jackhammer/meterGlow/meterGlow", "/Minigames/Jackhammer/powerMeter", "/Minigames/Jackhammer/breakerHUD", "/Minigames/Jackhammer/lowFuel", "/Minigames/Jackhammer/breaker", "/Minigames/Jackhammer/hotSauceMeter", "/UI/winMinigame", "/UI/loseMinigame", "/Particles/rockShard", "/Minigames/Jackhammer/Exit", "/Minigames/Jackhammer/dirt1", "/Minigames/Jackhammer/dirt2", "/Minigames/Jackhammer/dirt3", "/Minigames/Jackhammer/Snow/dirt1", "/Minigames/Jackhammer/Snow/dirt2", "/Minigames/Jackhammer/Snow/dirt3", "/Minigames/Jackhammer/ladder", "/Minigames/Jackhammer/ice", "/Minigames/Jackhammer/diamond", "/Minigames/Jackhammer/Snow/snow_ice", "/Minigames/Jackhammer/Snow/diamond", "/Minigames/Jackhammer/rock", "/Minigames/Jackhammer/Snow/snow_rock", "/Minigames/Jackhammer/rock_permanent", "/Minigames/Jackhammer/penguin", "/palettes/jackhammer_", "/palettes/jackhammer_aqua.nbfp", "/palettes/jackhammer_black.nbfp", "/palettes/jackhammer_blue.nbfp", "/palettes/jackhammer_brown.nbfp", "/palettes/jackhammer_darkGreen.nbfp", "/palettes/jackhammer_fuschia.nbfp", "/palettes/jackhammer_green.nbfp", "/palettes/jackhammer_hotsauce.nbfp", "/palettes/jackhammer_lime.nbfp", "/palettes/jackhammer_orange.nbfp", "/palettes/jackhammer_peach.nbfp", "/palettes/jackhammer_pink.nbfp", "/palettes/jackhammer_purple.nbfp", "/palettes/jackhammer_red.nbfp", "/palettes/jackhammer_yellow.nbfp", "/Particles/dust", "/Minigames/Jackhammer/coin_large", "/Minigames/Jackhammer/coin", "/Minigames/Jackhammer/bit", "/Minigames/Jackhammer/hotSauce", "/Minigames/Jackhammer/fuel", "/Minigames/Jackhammer/treasureChest", "/Minigames/Jackhammer/rock_shake", "/Minigames/Jackhammer/rock_fall", "/Minigames/Jackhammer/edge", "/Minigames/Jackhammer/Snow/edge", "/Minigames/Jackhammer/dirt_break", "/Minigames/Jackhammer/Snow/dirt_break", "/Minigames/Jackhammer/ice_break", "/Microgames/Jigsaw/Kite", "/Microgames/Jigsaw/cookie", "/Microgames/Jigsaw/KeyPad", "/Microgames/Jigsaw/Geyser", "/Microgames/Jigsaw/Windmill", "/Microgames/Jigsaw/MapPiece", "/bg/Microgames/MicrogameTS", "/Microgames/Jigsaw/PufflePin", "/Microgames/Jigsaw/JackHammer", "/Microgames/Jigsaw/SnowCatPiece", "/Microgames/Jigsaw/BlueprintPiece", "/bg/Microgames/Jigsaw/Jigsaw_bottom", "/bg/Microgames/Jigsaw/Geyser_bottom", "/bg/Microgames/Jigsaw/cookie_gameBG", "/Microgames/Jigsaw/Cookie/CookieStack", "/bg/Microgames/Jigsaw/Windmill_bottom", "/bg/Microgames/Jigsaw/GridPiece_bottom", "/bg/Microgames/Jigsaw/JackHammer_bottom", "/PC/Spryte/cursor", "/fonts/burbank.fnt", "/fonts/ComicrazyTitle.fnt", "/Microgames/Jigsaw/rotateCursor", "/PC/Spryte/cursor", "/Microgames/Jigsaw/rotateHandle", "/bg/Microgames/MicrogameTS", "/BG/Microgames/MicrogameTS", "/Microgames/CombinationBox/", "/Microgames/CombinationLock/", "/fonts/bigUI.fnt", "/Microgames/CombinationLock/KeypadButton_exit", "/Minigames/Labyrinth/map_location", "/Minigames/Labyrinth/map_hiddenGoal", "/Minigames/Labyrinth/puffle", "/Minigames/Labyrinth/map_hidden", "/Minigames/Labyrinth/puffleRedIcon", "/Minigames/Labyrinth/puffleWhiteIcon", "/Minigames/Labyrinth/puffleYellowIcon", "/Minigames/Labyrinth/youAreHere", "/Minigames/Labyrinth/UpFootprint", "/Minigames/Labyrinth/SideFootprint", "/Minigames/Labyrinth/panel_timer", "/Minigames/Labyrinth/Coin", "/Minigames/Labyrinth/clock", "/Minigames/Labyrinth/pizzaHUD", "/bg/labyrinth/", "/fonts/Burbank.fnt", "/UI/winMinigame", "/UI/loseMinigame", "/Minigames/Labyrinth/doorOpeningIcon", "/Minigames/Labyrinth/doorOpeningIconBlue", "/Minigames/Labyrinth/doorOpeningIconRed", "/Minigames/Labyrinth/doorOpeningIconGreen", "/Minigames/Labyrinth/doorOpeningIconOrange", "/Minigames/Labyrinth/doorOpeningIconPurple", "/Minigames/Labyrinth/doorOpeningIconTeal", "/Minigames/Labyrinth/woodenDoor", "/Minigames/Labyrinth/penguin", "/Minigames/Labyrinth/penquinshadow", "/Minigames/Labyrinth/aButtonPrompt", "/Minigames/Labyrinth/bButtonPrompt", "/Minigames/Jackhammer/coin", "/palettes/LabyrinthPenguin_", "/palettes/LabyrinthPenguin_aqua.nbfp", "/palettes/LabyrinthPenguin_black.nbfp", "/palettes/LabyrinthPenguin_blue.nbfp", "/palettes/LabyrinthPenguin_brown.nbfp", "/palettes/LabyrinthPenguin_darkGreen.nbfp", "/palettes/LabyrinthPenguin_fuschia.nbfp", "/palettes/LabyrinthPenguin_green.nbfp", "/palettes/LabyrinthPenguin_lime.nbfp", "/palettes/LabyrinthPenguin_orange.nbfp", "/palettes/LabyrinthPenguin_peach.nbfp", "/palettes/LabyrinthPenguin_pink.nbfp", "/palettes/LabyrinthPenguin_purple.nbfp", "/palettes/LabyrinthPenguin_red.nbfp", "/palettes/LabyrinthPenguin_yellow.nbfp", "/Minigames/Labyrinth/puffleshadow", "/Minigames/Labyrinth/puffleCannon", "/Minigames/Labyrinth/puffleWhiteBlow", "/Minigames/Labyrinth/puffleIceBreath", "/Minigames/Labyrinth/puffleWhistle", "/Minigames/Labyrinth/puffleHelm", "/Minigames/Labyrinth/puffle", "/Minigames/Labyrinth/switch2", "/Minigames/Labyrinth/switch3", "/Minigames/Labyrinth/switch4", "/Minigames/Labyrinth/switch5", "/Minigames/Labyrinth/switch6", "/Minigames/Labyrinth/switch", "/Particles/dust", "/fonts/Burbank.fnt", "/bg/Microgames/MicrogameTS", "/bg/Microgames/MechanoDuster/MechanoDusterBG_Snow", "/bg/Microgames/MechanoDuster/MechanoDusterBG_CoffeeShop", "/bg/Microgames/MechanoDuster/MechanoDusterBG_SuperRobotNose", "/bg/Microgames/MechanoDuster/MechanoDusterBG_Cave", "/bg/Microgames/MechanoDuster/MechandoDusterBG_MineShack", "/bg/Microgames/MechanoDuster/MechanoDusterBG_PizzaShop", "/bg/Microgames/MechanoDuster/MechanoDusterBG_Beard", "/Microgames/MechanoDuster/MechanoDuster_Cursor", "/Microgames/MechanoDuster/MechanoDuster_Key", "/Microgames/MechanoDuster/MechanoDuster_Snow", "/Microgames/MechanoDuster/MechanoDuster_MissingPen", "/Microgames/MechanoDuster/MechanoDuster_CoffeeBean", "/Microgames/MechanoDuster/MechanoDuster_RobotNose", "/Microgames/MechanoDuster/MechanoDuster_Grid", "/Microgames/MechanoDuster/MechanoDuster_Ash", "/Microgames/MechanoDuster/MechanoDuster_Jacket", "/Microgames/MechanoDuster/MechanoDuster_RagCursor", "/Microgames/MechanoDuster/MechanoDuster_none", "/Microgames/MechanoDuster/MechanoDuster_Cocoa1", "/Microgames/MechanoDuster/MechanoDuster_Sauce1", "/Microgames/MechanoDuster/MechanoDuster_Brush", "/Microgames/MechanoDuster/MechanoDuster_Beard", "/Microgames/MechanoDuster/BeardFG", "/Microgames/MechanoDuster/MechanoDuster_Cocoa", "/Microgames/MechanoDuster/MechanoDuster_Sauce", "/UI/blackPixel", "/BG/Microgames/MicrogameTS", "/Microgames/KlutzyMirror/Timer", "/Microgames/KlutzyMirror/Klutzy6", "/Microgames/KlutzyMirror/TimerVR", "/Microgames/KlutzyMirror/Klutzy5", "/Microgames/KlutzyMirror/Klutzy1", "/Microgames/KlutzyMirror/Klutzy2", "/Microgames/KlutzyMirror/Klutzy3", "/Microgames/KlutzyMirror/Klutzy4", "/Microgames/KlutzyMirror/Klutzy_1", "/Microgames/KlutzyMirror/Klutzy_2", "/Microgames/KlutzyMirror/Klutzy_3", "/Microgames/KlutzyMirror/Klutzy_4", "/Microgames/KlutzyMirror/Klutzy_5", "/Microgames/KlutzyMirror/Klutzy_6", "/BG/Microgames/KlutzyMirror/KM_TS", "/BG/Microgames/KlutzyMirror/KMTraining_TS", "/fonts/burbank.fnt", "/Microgames/KlutzyMirror/ShadowCover", "/Microgames/KlutzyMirror/X", "/Microgames/KlutzyMirror/VRNum", "/UI/buttonMedium", "/fonts/Burbank.fnt", "/fonts/ComicrazyTitle.fnt", "/UI/UtilityBorder/Puffle_Backdrop", "/UI/PuffleMessageBorder/pufflemessage", "/fonts/Burbank.fnt", "/UI/loading", "/UI/SpyPod/Puffle/Puffles/Puffle", "/BG/Microgames/MicrogameTS", "/BG/Microgames/InflateDuck/InflateDuck_BS", "/fonts/burbank.fnt", "/Microgames/InflateDuck/duck", "/Microgames/InflateDuck/pump_down", "/fonts/ComicrazyTitle.fnt", "/Microgames/RopePull/Pull", "/bg/Microgames/RopePullBG", "/BG/Microgames/MicrogameTS", "/fonts/burbank.fnt", "/fonts/ComicrazyTitle.fnt", "/BG/Microgames/MicrogameTS", "/BG/Microgames/SolarPanel/SP_BS", "/fonts/burbank.fnt", "/Microgames/SolarPanel/Tool1", "/Microgames/SolarPanel/Tool2", "/Microgames/SolarPanel/ModeSwitch", "/Microgames/SolarPanel/StartOver_up", "/Microgames/SolarPanel/RedLight", "/Microgames/SolarPanel/YellowLight", "/Microgames/SolarPanel/BlueLight", "/fonts/ComicrazyTitle.fnt", "/Microgames/SolarPanel/Node", "/Microgames/SolarPanel/SolderSegment", "/Microgames/SolarPanel/Node_glow", "/Microgames/SolarPanel/SolderSegmentDown", "/Microgames/SolarPanel/SolderSegmentLeft", "/Microgames/SolarPanel/SolderSegmentRight", "/BG/Microgames/SnakeGame/SnakeTS", "/bg/Microgames/SnakeGame/SnakeBG", "/fonts/burbank.fnt", "/Microgames/SnakeGame/SnakeHead", "/Microgames/SnakeGame/SnakeExplode", "/fonts/smallUI.fnt", "/fonts/ComicrazyTitle.fnt", "/UI/winMinigameRetro", "/UI/loseMinigameRetro", "/fonts/DSDigital.fnt", "/Microgames/SnakeGame/OCoffee", "/Microgames/SnakeGame/OCookie", "/Microgames/SnakeGame/OPizza", "/Microgames/SnakeGame/SnakeLink", "/Microgames/SnakeGame/SnakeExplode", "/Microgames/SnakeGame/OPizza", "/Microgames/SnakeGame/OCookie", "/Microgames/SnakeGame/OCoffee", "/palettes/jetpack_", "/Microgames/WallPuzzle/JPG", "/Microgames/WallPuzzle/Dot", "/BG/Microgames/MicrogameTS", "/Microgames/WallPuzzle/Player", "/Microgames/WallPuzzle/Rookie", "/BG/Microgames/WallPuzzle/wallpuzzleBG", "/fonts/burbank.fnt", "/fonts/ComicrazyTitle.fnt", "/Microgames/Welding/blow", "/Microgames/Welding/torch", "/Microgames/Welding/sparks", "/Microgames/Welding/puffle", "/BG/Microgames/MicrogameTS", "/Microgames/Welding/weld_hot", "/Microgames/Welding/ice_freeze", "/BG/Microgames/Welding/freezeBG", "/bg/Microgames/Welding/weldingBG", "/fonts/burbank.fnt", "/Microgames/Welding/weldingFG", "/fonts/ComicrazyTitle.fnt", "/Microgames/Wrench/Bolt", "/Microgames/Wrench/M5Bolt", "/BG/Microgames/MicrogameTS", "/Microgames/Wrench/Bolt_falling", "/bg/Microgames/Wrench/M5_Coffee", "/bg/Microgames/Wrench/M2_window", "/bg/Microgames/Wrench/M2_fuzebox", "/Microgames/Wrench/M5Bolt_falling", "/bg/Microgames/Wrench/M10_Gearbox", "/bg/Microgames/Wrench/M11WrenchBG", "/bg/Microgames/Wrench/M2_VRStation", "/bg/Microgames/Wrench/C3_puffleCage", "/bg/Microgames/Jigsaw/Jigsaw_bottom", "/bg/Microgames/Wrench/C1_jackHammer", "/bg/Microgames/Wrench/M8_boilerWindow", "/fonts/burbank.fnt", "/Microgames/Wrench/WrenchBig", "/Microgames/Wrench/Wrench", "/Microgames/Wrench/M5Hose", "/Microgames/Wrench/wrenchPuffleCage", "/fonts/ComicrazyTitle.fnt", "/BG/Microgames/Reflect/R_BS", "/BG/Microgames/Reflect/R_TS", "/BG/Microgames/MicrogameTS", "/fonts/burbank.fnt", "/Microgames/Reflect/dotIcon", "/Microgames/Reflect/dotMirror_on", "/Microgames/Reflect/jpgIcon", "/Microgames/Reflect/jpgMirror", "/Microgames/Reflect/playerIcon", "/Microgames/Reflect/playerMirror", "/Microgames/Reflect/rookieIcon", "/Microgames/Reflect/rookieMirror", "/Microgames/Reflect/herbIcon", "/Microgames/Reflect/magGlass", "/UI/blackPixel", "/UI/MainMenu/creditsBttn", "/fonts/smallUI.fnt", "/Microgames/Reflect/magGlass_fall", "/Microgames/Reflect/rookieMirror_on", "/Microgames/Reflect/jpgMirror_on", "/fonts/ComicrazyTitle.fnt", "/Microgames/Reflect/magGlass_burn", "/Microgames/Reflect/dotIcon_win", "/Microgames/Reflect/jpgIcon_win", "/Microgames/Reflect/playerIcon_win", "/Microgames/Reflect/rookieIcon_win", "/Microgames/Reflect/herbIcon_win", "/palettes/playerIcon_aqua.nbfp", "/palettes/playerIcon_black.nbfp", "/palettes/playerIcon_blue.nbfp", "/palettes/playerIcon_brown.nbfp", "/palettes/playerIcon_darkGreen.nbfp", "/palettes/playerIcon_fuschia.nbfp", "/palettes/playerIcon_green.nbfp", "/palettes/playerIcon_lime.nbfp", "/palettes/playerIcon_orange.nbfp", "/palettes/playerIcon_peach.nbfp", "/palettes/playerIcon_pink.nbfp", "/palettes/playerIcon_purple.nbfp", "/palettes/playerIcon_yellow.nbfp", "/palettes/playerIcon_red.nbfp", "/console.clubpenguin.com/submit_uk.php", "/fonts/smallUI.fnt", "/NPC/Puffles/Black/BlackPuffle", "/NPC/Puffles/Blue/BluePuffle", "/NPC/Puffles/Green/GreenPuffle", "/NPC/Puffles/Klutzy/Klutzy", "/NPC/Puffles/Pink/PinkPuffle", "/NPC/Puffles/Purple/PurplePuffle", "/NPC/Puffles/Red/RedPuffle", "/NPC/Puffles/White/WhitePuffle", "/NPC/Puffles/Yellow/YellowPuffle", "/WifiLogos/loading", "/UI/loading", "/UI/ellipsis", "/UI/stylusTap", "/fonts/DSdigital.fnt", "/bg/Cutscenes/CPLogo", "/bg/Cutscenes/BlankBlack", "/UI/stylusTap", "/bg/Cutscenes/CPLogo", "/bg/Microgames/MicrogameTS", "/bg/", "/bg/LevelSelect/Map_top", "/fonts/DSdigital.fnt", "/UI/MissionSelector/LightsBlink", "/BG/MissionSelector/MissionSelect_top", "/BG/MissionSelector/MissionSelect_bottom", "/BG/MissionSelector/MissionSelect_frame", "/UI/MissionSelector/Bttn_Exit", "/UI/MissionSelector/Bttn_arrow_up", "/UI/MissionSelector/Bttn_arrow_down", "/UI/MissionSelector/Bttn_launch", "/UI/MissionSelector/Mission_Image", "/UI/MissionSelector/Mission_Image_m", "/UI/MissionSelector/Bttn_mission", "/fonts/BurbankSmall.fnt", "/UI/PuffleMessageBorder/pufflemessage", "/UI/SpyPod/Communicator/", "/UI/SpyLog", "/UI/Zoom/bttnExit", "/bg/Microgames/MicrogameTS", "/fonts/bigUI.fnt", "/Sparkles/SparklyLeash/Univ_Obj_SparkleLeash1", "/french/", "/english/", "/spanish/", "/UI/cancelBttn", "/UI/chkmarkBttn", "/fonts/bigUI.fnt", "/fonts/hugeUI.fnt", "/archivelist.txt", "/UI/WarningDialog/warningDialog", "/UI/WarningDialog/warningDialog", "/Keyboard/Keys/", "/Keyboard/keyboard", "/csv/accentkeymap.dat", "/Keyboard/cancel", "/fonts/smallUI.fnt", "/font/smallUI.fnt", "/particles/sparkle", "/Tools/broken", "/tools/broken", "/Tools/broken", "/attempt to yield across metamethod/C-call boundary", "/string/function/table expected", "/1", "/1", "/Scenes/DG1_FontColors", "/Scenes/DG1_TopScreenMasterPalette", "/DGamer/Scenes", "/Scenes/DG1_Back2GameIcon.nclr", "/Scenes/ChatMenuSpritesCharFile", "/Scenes/AvatarChatBack", "/Scenes/DG1_TopScreenMasterPalette", "/Scenes/DG1_TopScreenMasterPalette.nclr", "/Scenes/DG1_HelpTextIcons", "/Scenes/DG1_Back2GameIcon.nclr", "/Scenes/DG1_MainMenuMasterPalette", "/Scenes/DG1_TopScreenMasterPalette", "/Scenes/DG1_Small_Floating_Bubbles_TopS", "/Scenes/DG1_Small_Floating_Bubbles", "/Scenes/WiFiLevelChar", "/Scenes/WiFiLevel", "/Scenes/DG1_WirelessTextbox", "/c/", "/GuiContent.txt", "/t/", "/DGamer/Scenes/MainMenu_Main_01.NCGR", "/flc/Outro.flc", "/bg/Cutscenes/CPLogo", "/flc/Lockdown.flc", "/bg/Cutscenes/FPMOutro_top", "/fonts/Burbank.fnt", "/bg/Microgames/MicrogameTS", "/Microgames/Trough/Trough_flip", "/Microgames/Trough/TutorialLogMove", "/Microgames/Trough/TutorialStylusTap", "/CutsceneAnims/C4/geyser", "/bg/Cutscenes/FPMOutro_top", "/CutsceneAnims/C4/penguins1", "/CutsceneAnims/C4/penguins2", "/CutsceneAnims/C4/penguins3", "/bg/Cutscenes/M10_FillGeyser", "/CutsceneAnims/C3/melt", "/bg/Cutscenes/C3_Melt", "/CutsceneAnims/C3/sunray", "/bg/Cutscenes/FPMOutro_top", "/flc/GogglesOff.flc", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/M11VideoMachineOpen", "/CutsceneAnims/M11DVDPanel/MineShackType", "/levels/AquaGrabber0_map_0.mpb", "/levels/AquaGrabber0_map_1.mpb", "/levels/AquaGrabber0_map_2.mpb", "/levels/AquaGrabber1_map_0.mpb", "/levels/AquaGrabber1_map_1.mpb", "/levels/AquaGrabber1_map_2.mpb", "/levels/AquaGrabber2_map_0.mpb", "/levels/AquaGrabber2_map_1.mpb", "/levels/AquaGrabber2_map_2.mpb", "/r", "/levels/AquaGrabber3_map_0.mpb", "/levels/AquaGrabber3_map_1.mpb", "/levels/AquaGrabber3_map_2.mpb", "/levels/M8_caves_map_0.mpb", "/Minigames/Labyrinth/", "/levels/M11_cornMaze1_map_0.mpb", "/Minigames/Labyrinth/", "/levels/M11_cornMaze2_map_0.mpb", "/Minigames/Labyrinth/", "/levels/C3_cave_map_0.mpb", "/levels/C3_cave_map_1.mpb", "/Minigames/Labyrinth/", "/levels/M9_frozenLake_map_0.mpb", "/Minigames/Labyrinth/", "/levels/B1_bonus_map_0.mpb", "/Minigames/Labyrinth/", "/levels/B2_bonus_map_0.mpb", "/Minigames/Labyrinth/", "/levels/B3_bonus_map_0.mpb", "/Minigames/Labyrinth/", "/fonts/Comicrazy.fnt", "/bg/WifiContent/DGamerTop", "/bg/WifiContent/DGamer_bottomscreen", "/levels/GrapplingHookC2HerbBase_map_0.mpb", "/levels/GrapplingHookC2HerbBase_map_1.mpb", "/Minigames/Labyrinth/", "/Minigames/GrapplingHook/", "/levels/GrapplingHookC3TallestMtn_map_0.mpb", "/levels/GrapplingHookC3TallestMtn_map_1.mpb", "/Minigames/Labyrinth/", "/Minigames/GrapplingHook/", "/levels/GrapplingHookC3TallestMtn2Top_map_0.mpb", "/levels/GrapplingHookC3TallestMtn2Top_map_1.mpb", "/Minigames/Labyrinth/", "/Minigames/GrapplingHook/", "/levels/GrapplingSkiHill_map_0.mpb", "/levels/GrapplingSkiHill_map_1.mpb", "/Minigames/Labyrinth/", "/Minigames/GrapplingHook/", "/levels/GrapplingBonus1_map_0.mpb", "/levels/GrapplingBonus1_map_1.mpb", "/Minigames/Labyrinth/", "/Minigames/GrapplingHook/", "/levels/GrapplingBonus2_map_0.mpb", "/levels/GrapplingBonus2_map_1.mpb", "/Minigames/Labyrinth/", "/Minigames/GrapplingHook/", "/levels/GrapplingBonus3_map_0.mpb", "/levels/GrapplingBonus3_map_1.mpb", "/Minigames/Labyrinth/", "/Minigames/GrapplingHook/", "/levels/GrapplingTutorial01_map_0.mpb", "/levels/GrapplingTutorial01_map_1.mpb", "/Minigames/Labyrinth/", "/Minigames/GrapplingHook/", "/scripts/M9_CombineConnectFour1.lua", "/scripts/M9_CombineConnectFour1.lua", "/scripts/M9_CombineConnectFour1.lua", "/scripts/M9_CombineConnectFour1.lua", "/scripts/M9_CombineConnectFour1.lua", "/scripts/M9_CombineStringKitestick.lua", "/scripts/ALL_Attic_Item_Inflatable.lua", "/scripts/C4_Attic_Item_FestivalFloat.lua", "/levels/Attic0_map_0.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M4/", "/Objects/CommandCoach/", "/Location/Attic/state/", "/flc/IntroG.flc", "/flc/Intro4.flc", "/flc/Intro3.flc", "/flc/IntroG2.flc", "/flc/IntroDot.flc", "/flc/IntroJPG.flc", "/bg/Cutscenes/CPLogo", "/flc/IntroRookie.flc", "/fonts/BurbankSmall.fnt", "/fonts/ComicrazyTitle.fnt", "/scripts/M10_Beach_NPC_JPG.lua", "/scripts/M8_Beach_Item_Net.lua", "/scripts/M5_SleepingPenguin.lua", "/scripts/M5_Beacon_Item_FuelInv.lua", "/scripts/Doors_BeachDoor2Lighthouse.lua", "/levels/Beach0_map_0.mpb", "/levels/Beach0_map_1.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Objects/WorldItems/", "/Objects/CommandCoach/", "/Location/Beach/touch/", "/Location/Beach/state/", "/MissionObjects/SecretFur/", "/NPC/WaddleSquad/JetPackGuy/", "/MissionObjects/WaddleSquad/static/", "/scripts/M5_Beacon_Item_Fuel.lua", "/scripts/M5_Beacon_Item_Fuel.lua", "/scripts/M8_Beacon_Item_Jetpack.lua", "/scripts/M5_Beacon_Item_FuelInv.lua", "/scripts/Doors_Beacon0_LighthouseUpDoor2JetPack.lua", "/x/", "/levels/Beacon0_map_0.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/MissionObjects/SecretFur/", "/Location/Lighthouse/state/", "/Location/LighthouseUp/script/", "/scripts/ALL_Item_Inflatable.lua", "/scripts/C4_BoilerRoom_Item_WhaleFloat.lua", "/levels/BoilerRoom0_map_0.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M4/", "/Objects/CommandCoach/", "/Location/BoilerRoom/state/", "/Location/BoilerRoom/scripted/", "/scripts/ALL_Item_Inflatable.lua", "/scripts/Doors_BoilerDoor2Pool.lua", "/scripts/M8_Tunnels_NPC_Klutzy.lua", "/scripts/M8_Tunnels_NPC_Herbert.lua", "/scripts/M8_Tunnels_Item_Boiler.lua", "/scripts/M8_Tunnels_NPC_Herbert.lua", "/scripts/M8_Tunnels_NPC_Herbert.lua", "/scripts/M8_Tunnels_NPC_Herbert.lua", "/scripts/Doors_BoilerDoor2Night.lua", "/scripts/M8_Tunnels_NPC_Herbert.lua", "/scripts/M8_Tunnels_NPC_Herbert.lua", "/scripts/M8_Tunnels_Item_Boiler.lua", "/scripts/Doors_InvisDoorBoilerRoom.lua", "/levels/BoilerRoomTremors0_map_0.mpb", "/Location/", "/NPC/Puffles/Blue/", "/MissionObjects/M4/", "/Objects/CommandCoach/", "/Location/BoilerRoom/state/", "/NPC/MysteriousTremors/Klutzy/", "/NPC/MysteriousTremors/Herbert/", "/MissionObjects/MysteriousTremors/touch/", "/MissionObjects/MysteriousTremors/static/", "/scripts/M11_Mancala_NPC.lua", "/scripts/M11_MancalaBoard.lua", "/levels/BookRoom0_map_0.mpb", "/Location/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/NPC/M11/BookRoomNPC/", "/Objects/CommandCoach/", "/scripts/M6_Cave_Item_Lever.lua", "/scripts/M6_Cave_NPC_Herbert.lua", "/scripts/M6_Cave_Item_Weight.lua", "/scripts/M6_Cave_Item_inCageBG.lua", "/scripts/M6_Cave_Item_TroughUp.lua", "/scripts/M6_Cave_Item_inCageBG.lua", "/scripts/M6_Cave_Item_TroughUp.lua", "/scripts/M6_Cave_Item_inCageBG.lua", "/scripts/M6_Cave_Item_doorClosed.lua", "/levels/CaveInteriorHerb0_map_0.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/NPC/QuestionsCrab/Klutzy/", "/NPC/QuestionsCrab/Herbert/", "/Location/CaveIntHerb/touch/", "/Location/CaveIntHerb/state/", "/MissionObjects/QuestionsCrab/state/", "/MissionObjects/QuestionsCrab/static/", "/MissionObjects/QuestionsCrab/scripted/", "/scripts/C2_FakeRock1.lua", "/scripts/C2_HerbsOldBase_DirtPile1.lua", "/scripts/C2_HerbsOldBase_DirtPile1.lua", "/levels/CaveInteriorHerbEmpty_map_0.mpb", "/NPC/Puffles/Blue/", "/MissionObjects/M2/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/Location/Beach/state/", "/MissionObjects/QuestionsCrab/state/", "/scripts/C2_FakeRock1.lua", "/scripts/M6_HotOberryBag2Sauce.lua", "/scripts/M6_Cave_Item_doorOpen.lua", "/scripts/M6_Cave_Item_Blueprint.lua", "/scripts/M6_GrapplingAnchor2Rope.lua", "/scripts/M6_Doors_BeachDoor2Dock.lua", "/scripts/M6_Cave_Item_closedDoor.lua", "/scripts/M6_GrapplingRope2Anchor.lua", "/scripts/C2_HerbsOldBase_DirtPile1.lua", "/scripts/C2_HerbsOldBase_DirtPile1.lua", "/levels/CaveInteriorHerbOpen0_map_0.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M2/", "/Location/Beach/state/", "/Location/CaveIntHerb/state/", "/Location/CaveIntHerb/touch/", "/MissionObjects/QuestionsCrab/state/", "/scripts/M2_CoffeeMachine_ExitButton.lua", "/scripts/M5_CoffeeMachine_Item_CupTop.lua", "/scripts/M5_CoffeeMachine_Item_MilkHose.lua", "/scripts/M5_CoffeeMachine_Item_HotCocoa.lua", "/scripts/M5_CoffeeMachine_Item_CupPlace.lua", "/scripts/M5_CoffeeMachine_Item_ChocoHole.lua", "/scripts/M5_CoffeeMachine_Item_ColdButton.lua", "/scripts/M5_CoffeeMachine_Item_ChocoButton.lua", "/levels/CoffeeMachine0_map_0.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Location/CoffeeMachine/", "/MissionObjects/UniversalItems/", "/scripts/Doors_CoffeeShopDoor2Upstairs.lua", "/scripts/Doors_CoffeeShopDoor2Employees.lua", "/scripts/Doors_CoffeeShop0_CoffeeDoor2Town.lua", "/scripts/Doors_CoffeeShop0_CoffeeShopDoor2Upstairs.lua", "/scripts/Doors_CoffeeShop0_CoffeeShopDoor2Upstairs.lua", "/levels/CoffeeShop0_map_0.mpb", "/NPC/Puffles/Blue/", "/Location/Pool/state/", "/Objects/CommandCoach/", "/Location/CoffeeShop/state/", "/Location/CoffeeShop/touch/", "/Location/CoffeeShop/static/", "/flc/1PLogotop.flc", "/bg/Cutscenes/DisneyLogo", "/bg/Cutscenes/BlankBlack", "/bg/Cutscenes/NintendoTop", "/bg/Cutscenes/1PLogobottom", "/bg/Cutscenes/1PLogotop", "/bg/Cutscenes/DisneyLogoCopyright", "/scripts/M5_CoffeeShop_NPC_Barista.lua", "/scripts/M5_CoffeeShop_coffeeMess1.lua", "/scripts/Doors_CoffeeShopDoor2Upstairs.lua", "/scripts/Doors_CoffeeShopDoor2Employees.lua", "/scripts/M5_CoffeeShop_Item_CoffeeMachine.lua", "/levels/CoffeeShopCreature0_map_0.mpb", "/NPC/Puffles/Blue/", "/Objects/CommandCoach/", "/NPC/SecretFur/Barista/", "/MissionObjects/SecretFur/", "/Location/CoffeeShop/state/", "/Location/CoffeeShop/touch/", "/Location/CoffeeShop/static/", "/scripts/M8_CoffeeShop_NPC_Barista.lua", "/scripts/M8_CoffeeShop_NPC_Barista.lua", "/scripts/M8_CoffeeShop_Item_Cookies.lua", "/scripts/Doors_CoffeeShopDoor2Upstairs.lua", "/scripts/Doors_CoffeeShopDoor2Employees.lua", "/*", "/levels/CoffeeShopTremors0_map_0.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/Location/CoffeeShop/state/", "/Location/CoffeeShop/touch/", "/Location/CoffeeShop/static/", "/NPC/MysteriousTremors/Barista/", "/MissionObjects/MysteriousTremors/static/", "/scripts/Screen.lua", "/scripts/Screen.lua", "/scripts/M5_VR_Gary.lua", "/scripts/M5_VR_Director.lua", "/scripts/C2_CrabDisguise.lua", "/scripts/Door_CommandRoom.lua", "/scripts/C1_CommandRoom_Dot.lua", "/scripts/C1_CommandRoom_JPG.lua", "/scripts/C2_CommandRoom_JPG.lua", "/scripts/C3_CommandRoom_Dot.lua", "/scripts/C3_CommandRoom_JPG.lua", "/scripts/C2_CommandRoom_Dot.lua", "/scripts/C3_CommandRoom_Gary.lua", "/scripts/C2_CommandRoom_Gary.lua", "/scripts/C1_CommandRoom_Rookie.lua", "/scripts/C3_CommandRoom_Rookie.lua", "/scripts/C2_CommandRoom_Rookie.lua", "/scripts/C4_CommandRoom_NPC_JPG.lua", "/scripts/C4_CommandRoom_NPC_Dot.lua", "/scripts/C1_CommandRoom_Director.lua", "/scripts/C3_CommandRoom_Director.lua", "/scripts/C4_CommandRoom_NPC_Gary.lua", "/scripts/C2_CommandRoom_Director.lua", "/scripts/C3_InventoryItem_Tracker.lua", "/scripts/C4_CommandRoom_NPC_Rookie.lua", "/scripts/C4_CommandRoom_NPC_Director.lua", "/scripts/FP_CommandRoom0_Items_HardDrive.lua", "/scripts/Doors_CommandRoom0_CommandDoor2HQ.lua", "/scripts/CommandCoach_CommandRoom0_LocationCommandRoom_CommandCoachOpen.lua", "/levels/CommandRoom0_map_0.mpb", "/UI/", "/NPC/M1/Dot/", "/NPC/M4/JPG/", "/NPC/M4/Dot/", "/NPC/M3/Dot/", "/NPC/M3/JPG/", "/NPC/M2/Dot/", "/NPC/M4/Gary/", "/NPC/M3/Gary/", "/NPC/M2/Gary/", "/NPC/M11/Gary/", "/NPC/M1/Rookie/", "/NPC/M4/Rookie/", "/NPC/M3/Rookie/", "/NPC/M2/Rookie/", "/NPC/M2/Puffles/", "/NPC/M1/Director/", "/NPC/M4/Director/", "/NPC/M3/Director/", "/NPC/M2/Director/", "/NPC/CommandCoach/", "/NPC/Puffles/Blue/", "/NPC/M1/JetPackGuy/", "/NPC/M2/JetPackGuy/", "/UI/InventoryPanel/", "/MissionObjects/M3/", "/NPC/M3/Puffle/commandRoom/", "/Location/CommandRoom/touch/", "/Location/CommandRoom/state/", "/Location/CommandRoom/static/", "/Location/CommandRoom/scripted/", "/scripts/M11_CornMazeHerbert.lua", "/scripts/M11_CornMaze_Klutzy.lua", "/scripts/M11_Maze_NPC_Rookie.lua", "/scripts/M11_CornMaze_MonitorWire.lua", "/levels/CornMaze0_map_0.mpb", "/levels/CornMaze0_map_1.mpb", "/NPC/M11/Rookie/", "/NPC/M11/Klutzy/", "/NPC/M11/Herbert/", "/NPC/Puffles/Blue/", "/MissionObjects/M11/", "/scripts/M11_CornMaze1_River.lua", "/scripts/M11_Maze_NPC_Rookie.lua", "/scripts/M11_CornMaze1_RiverPlank.lua", "/scripts/M11_CornMaze1_RiverPlank.lua", "/levels/CornMaze1_map_0.mpb", "/levels/CornMaze1_map_1.mpb", "/Location/", "/NPC/M11/Rookie/", "/NPC/Puffles/Blue/", "/MissionObjects/M3/", "/MissionObjects/M11/", "/MissionObjects/MysteriousTremors/static/", "/levels/CornMaze2_map_0.mpb", "/levels/CornMaze2_map_1.mpb", "/Location/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/scripts/M11_Maze_NPC_Rookie.lua", "/scripts/M11_CornMaze3_Ladder.lua", "/scripts/M11_CornMaze3_LeftTorch.lua", "/scripts/M11_CornMaze3_RightTorch.lua", "/levels/CornMaze3_map_0.mpb", "/levels/CornMaze3_map_1.mpb", "/Location/", "/NPC/M11/Rookie/", "/NPC/Puffles/Blue/", "/Location/CornMaze/", "/MissionObjects/M11/", "/scripts/M11_CornField_Door2Back.lua", "/levels/CornMazeBegin0_map_0.mpb", "/levels/CornMazeBegin0_map_1.mpb", "/Location/", "/NPC/Puffles/Blue/", "/scripts/M11_CornMazeSecret_River.lua", "/scripts/M11_CornMazeSecret_River.lua", "/scripts/M11_CornMazeSecret_Item_SnakePiece.lua", "/levels/CornMazeSecret_map_0.mpb", "/levels/CornMazeSecret_map_1.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Location/CornMaze/", "/MissionObjects/M3/", "/MissionObjects/MysteriousTremors/static/", "/scripts/M9_Dock_NPC.lua", "/scripts/M10_Dock_NPC.lua", "/scripts/M10_Docks_Rory.lua", "/scripts/M9_Dock_Item_Pump.lua", "/scripts/M5_Docks_DocksNPC.lua", "/scripts/M9_CombinePumpDuck.lua", "/scripts/M10_Docks_NPC_Decoy.lua", "/scripts/M10_Docks_Item_Hole.lua", "/scripts/M8_Docks_NPC_Klutzy.lua", "/scripts/M8_Docks_NPC_Herbert.lua", "/scripts/M5_Dock_Item_lantern.lua", "/scripts/M10_Docks_NPC_Klutzy.lua", "/scripts/M8_Docks_NPC_Herbert.lua", "/scripts/M8_Docks_Item_Lantern.lua", "/scripts/Doors_InvisDoor2Beach.lua", "/scripts/M9_Dock_Item_BoatStart.lua", "/scripts/Doors_Dock0_DockDoor2Ski.lua", "/scripts/Doors_Dock0_DockDoor2Beach.lua", "/levels/Dock0_map_0.mpb", "/levels/Dock0_map_1.mpb", "/Location/", "/NPC/Puffles/Blue/", "/MissionObjects/M3/", "/UI/InventoryPanel/", "/MissionObjects/M2/", "/Objects/WorldItems/", "/NPC/SpySeek/DockNPC/", "/Location/Dock/state/", "/Location/Dock/touch/", "/NPC/WaddleSquad/Rory/", "/Objects/CommandCoach/", "/Location/Dock/static/", "/Location/Plaza/state/", "/NPC/WaddleSquad/Klutzy/", "/NPC/WaddleSquad/DockNPC/", "/MissionObjects/SecretFur/", "/NPC/MysteriousTremors/Klutzy/", "/NPC/MysteriousTremors/Herbert/", "/MissionObjects/SpySeek/scripted/", "/MissionObjects/MysteriousTremors/static/", "/fonts/faceFront.fnt", "/scripts/M11_DVDMachine_Panel.lua", "/scripts/M7_DVDMachine_ExitButton.lua", "/scripts/M11_DVDMachine_OpenPanel.lua", "/levels/DVDMachine0_map_0.mpb", "/MissionObjects/M11/", "/MissionObjects/SecretFur/", "/MissionObjects/UniversalItems/", "/scripts/M6_Fishing_Item_Lever.lua", "/scripts/M5_FishingHole_Item_Fur.lua", "/scripts/M6_FishingHole_NPC_Gary.lua", "/scripts/M5_FishingHole_Item_Trap.lua", "/scripts/M5_FishingHole_NPC_Shadow.lua", "/scripts/M6_FishingHole_NPC_Herbert.lua", "/scripts/M6_FishingHole_NPC_Herbert.lua", "/scripts/M5_FishingHole_NPC_Fisherman.lua", "/scripts/M9_Fishing_Item_Door2HerbsCamp.lua", "/levels/Fishing0_map_0.mpb", "/levels/Fishing0_map_1.mpb", "/Location/", "/NPC/Gary/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/Location/Fishing/state/", "/NPC/SecretFur/Creature/", "/NPC/SecretFur/FishingNPC/", "/MissionObjects/SecretFur/", "/NPC/QuestionsCrab/Klutzy/", "/NPC/QuestionsCrab/Herbert/", "/MissionObjects/QuestionsCrab/static/", "/MissionObjects/MysteriousTremors/static/", "/scripts/M9_CombineStickBlueprint.lua", "/levels/Forest0_map_0.mpb", "/levels/Forest0_map_1.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M3/", "/Objects/CommandCoach/", "/Location/Forest/state/", "/MissionObjects/SpySeek/static/", "/scripts/M5_FurAnalyzer_ScreenEnd.lua", "/scripts/M5_FurAnalyzer_Item_Slot.lua", "/scripts/M5_FurAnalyzer_ExitButton.lua", "/scripts/M5_GadgetRoom0_Item_PinkFur.lua", "/levels/FurAnalyzer0_map_0.mpb", "/NPC/Puffles/Blue/", "/MissionObjects/SecretFur/", "/MissionObjects/UniversalItems/", "/scripts/M5_Items_Goggles.lua", "/scripts/M9_CombineDuckGum.lua", "/scripts/Door_GadgetRoom2HQ.lua", "/scripts/M9_CombineDuckPump.lua", "/scripts/M8_HeliumAndBalloon.lua", "/scripts/M9_CombineCameraPhone.lua", "/scripts/M8_GadgetRoom_NPC_Gary.lua", "/scripts/M5_GadgetRoom_NPC_Gary.lua", "/scripts/M6_GadgetRoom_CrabCage.lua", "/scripts/M6_GadgetRoom_NPC_Gary.lua", "/scripts/M6_GadgetRoom_NPC_Gary.lua", "/scripts/M9_GadgetRoom_NPC_Gary.lua", "/scripts/M9_CombineDuckReceiver.lua", "/scripts/M9_CombineReceiver2Other.lua", "/scripts/M6_GadgetRoom_Translator.lua", "/scripts/M9_CombineBlueprintStick.lua", "/scripts/M9_CombineReceiver2Other.lua", "/scripts/M6_GadgetRoom_Translator.lua", "/scripts/M9_CombineReceiver2Other.lua", "/scripts/M8_GadgetRoom_Item_Helium.lua", "/scripts/M8_GadgetRoom_Item_Hammer.lua", "/scripts/M6_GadgetRoom_NPC_GaryFix.lua", "/scripts/M5_GadgetRoom_NPC_GaryEnd.lua", "/scripts/LocationGadgetRoom_Balloon.lua", "/scripts/M5_GadgetRoom_Item_Goggles.lua", "/scripts/LocationGadgetRoom_BoomBox.lua", "/scripts/M5_GadgetRoom_Item_Analyzer.lua", "/scripts/M5_GadgetRoom_Item_Analyzer.lua", "/scripts/M5_GadgetRoom_Item_Analyzer.lua", "/scripts/M5_GadgetRoom_Item_Analyzer.lua", "/scripts/GadgetRoom_Item_GaryJetPack.lua", "/levels/GadgetRoom0_map_0.mpb", "/NPC/Puffles/Blue/", "/NPC/SpySeek/Gary/", "/UI/InventoryPanel/", "/NPC/SecretFur/Gary/", "/Objects/CommandCoach/", "/NPC/QuestionsCrab/Gary/", "/MissionObjects/SecretFur/", "/NPC/QuestionsCrab/Klutzy/", "/Location/GadgetRoom/touch/", "/Location/GadgetRoom/state/", "/NPC/MysteriousTremors/Gary/", "/Location/GadgetRoom/static/", "/MissionObjects/QuestionsCrab/static/", "/MissionObjects/MysteriousTremors/static/", "/scripts/Doors_GarysRoom0_GarysRoomDoor2Sport.lua", "/levels/GarysRoom0_map_0.mpb", "/NPC/Puffles/Blue/", "/Location/GarysRoom/touch/", "/Location/GarysRoom/state/", "/Location/GarysRoom/static/", "/scripts/M11_GSFootPrints.lua", "/scripts/M11_CombineKernels.lua", "/scripts/M11_GiftShop_JPGTemp.lua", "/scripts/C4_CombineBalloonTape.lua", "/scripts/Doors_OfficeDoor2Roof.lua", "/levels/GiftOffice0_map_0.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/NPC/M11/JetPackGuy/", "/MissionObjects/M11/", "/Objects/CommandCoach/", "/Location/GiftOffice/state/", "/Location/GiftOffice/static/", "/scripts/Doors_OfficeDoor2Roof.lua", "/levels/GiftOfficeTremors0_map_0.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/Location/GiftOffice/state/", "/levels/GiftRoof0_map_0.mpb", "/NPC/Puffles/Blue/", "/Location/GiftRoof/state/", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/HerbertDock1.flc", "/scripts/M11_GSFootPrints.lua", "/scripts/C4_Item_Floaties.lua", "/scripts/C4_Item_LifeJacket.lua", "/scripts/M11_CombineKernels.lua", "/scripts/Doors_GiftShop2Town.lua", "/scripts/C4_GiftShop_GiftOwner.lua", "/scripts/M8_GiftShop_VaultDoor.lua", "/scripts/M11_GiftShop_NPC_RookieTemp.lua", "/levels/GiftShop0_map_0.mpb", "/Objects/", "/NPC/M11/Rookie/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M11/", "/Location/Pool/state/", "/Objects/CommandCoach/", "/Location/GiftShop/state/", "/Location/SportShop/touch/", "/NPC/WaddleSquad/GiftShopOwner/", "/scripts/M10_AddSolarPanel.lua", "/scripts/M8_GiftShop_VaultDoor.lua", "/scripts/M10_GiftShop_NPC_Owner.lua", "/scripts/M10_GiftShop_NPC_Owner.lua", "/scripts/M10_GiftShop1_GiftShopBox.lua", "/scripts/M10_GiftShoptemp_NPC_Rookie.lua", "/scripts/M10_GiftShop1_GiftShopTable.lua", "/scripts/M10_GiftShop_Item_SnakePiece.lua", "/scripts/M10_GiftShop1_GiftShopClothes.lua", "/scripts/Doors_GiftShop0_GiftShopDoor2Office.lua", "/levels/GiftShop1_map_0.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/NPC/WaddleSquad/Rookie/", "/Location/GiftShop/state/", "/Location/SportShop/touch/", "/NPC/WaddleSquad/GiftShopOwner/", "/MissionObjects/WaddleSquad/scripted/", "/scripts/M8_GiftShop_VaultDoor.lua", "/scripts/M8_GiftShop2Town_Tremors.lua", "/scripts/Doors_GiftShop0_GiftShopDoor2Office.lua", "/levels/GiftShopTremors0_map_0.mpb", "/Objects/", "/NPC/Puffles/Blue/", "/Objects/CommandCoach/", "/Location/GiftShop/state/", "/Location/SportShop/touch/", "/scripts/C2_HKeyD_HBDen.lua", "/scripts/C2_Door2StorageSwitch.lua", "/scripts/C2_HerbsBase_CameraObject.lua", "/scripts/C2_HerbsBase_CameraObject.lua", "/levels/HBDen_map_0.mpb", "/UI/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M2/", "/Objects/CommandCoach/", "/MissionObjects/SecretFur/", "/Location/HerbertsBaseDen/state/", "/Location/HerbertsBaseDen/touch/", "/Location/HerbertsBaseDen/static/", "/scripts/C2_SkyLight.lua", "/scripts/C2_DoorPanel2Den.lua", "/scripts/C2_HKeyB_HBLobby.lua", "/scripts/C2_HBLobby_Statue.lua", "/scripts/C2_HBLobby_SecurityDoor.lua", "/scripts/C2_HerbsBase_CameraObject.lua", "/levels/HBLobby_map_0.mpb", "/UI/", "/NPC/Puffles/Blue/", "/MissionObjects/M2/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/MissionObjects/SecretFur/", "/Location/HerbertsBaseDen/static/", "/Location/HerbertsBaseLobby/touch/", "/Location/HerbertsBaseLobby/state/", "/Location/HerbertsBaseLobby/static/", "/MissionObjects/MysteriousTremors/static/", "/scripts/C2_TripleLockDoor.lua", "/scripts/C2_HBStorage_Doors.lua", "/scripts/C2_HKeyC_HBStorage.lua", "/scripts/C2_HBStorage_Klutzy.lua", "/scripts/C2_HBWorkShop_Herbert.lua", "/scripts/C2_HerbsBase_CameraObject.lua", "/scripts/C2_HerbsBase_CameraObject.lua", "/levels/HBStorage_map_0.mpb", "/UI/", "/NPC/M2/Klutzy/", "/NPC/M2/Herbert/", "/NPC/Puffles/Blue/", "/MissionObjects/M2/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/MissionObjects/SecretFur/", "/Location/HerbertsBaseDen/static/", "/Location/HerbertsBaseStorage/touch/", "/Location/HerbertsBaseStorage/state/", "/scripts/C2_HBWorkshop_Comp.lua", "/scripts/C2_HBWorkshop_Poster.lua", "/scripts/C2_HBWorkshop_Poster.lua", "/scripts/C2_HBWorkShop_Herbert.lua", "/scripts/C2_HBWorkshop_Flipchart.lua", "/scripts/C2_HBWorkShop_Blueprint.lua", "/levels/HBWorkshop_map_0.mpb", "/NPC/M2/Herbert/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/MissionObjects/SecretFur/", "/Location/HerbertsBaseDen/static/", "/Location/HerbertsBaseWorkshop/touch/", "/Location/HerbertsBaseWorkshop/state/", "/Location/HerbertsBaseWorkshop/static/", "/scripts/C2_CombineBPpieces.lua", "/scripts/C2_CombineBPpieces.lua", "/scripts/C2_CombineBPpieces.lua", "/scripts/C2_CombineBPpieces.lua", "/scripts/C2_BluePrintPieceTree.lua", "/scripts/M9_CombinePhoneCamera.lua", "/scripts/M9_HerbsCamp_Item_Tree.lua", "/scripts/M9_HerbsCamp_Item_Tree.lua", "/scripts/M9_HerbsCamp_Item_Tree.lua", "/scripts/C2_HerbsCamp2SkiVillage.lua", "/scripts/C2_HerbertCamp_Item_Bucket.lua", "/scripts/C2_HerbertCamp_Item_HeavyRock.lua", "/levels/HerbsCamp0_map_0.mpb", "/levels/HerbsCamp0_map_1.mpb", "/Location/", "/NPC/Puffles/Blue/", "/MissionObjects/M2/", "/UI/InventoryPanel/", "/Location/HerbCamp/", "/Objects/CommandCoach/", "/MissionObjects/SecretFur/", "/MissionObjects/SpySeek/scripted/", "/scripts/M8_HQ_NPC_Gary.lua", "/scripts/M9_HQ_NPC_Gary.lua", "/scripts/M8_HQ_NPC_Gary.lua", "/scripts/M11_HQ_NPC_JPG.lua", "/scripts/M10_HQ_NPC_JPG.lua", "/scripts/M11_HQ_NPC_Dot.lua", "/scripts/M10_HQ_NPC_Gary.lua", "/scripts/M10_HQ_NPC_Gary.lua", "/scripts/M10_HQ_NPC_Gary.lua", "/scripts/M6_HQ_NPC_Klutzy.lua", "/scripts/M10_HQ_Item_Panel.lua", "/scripts/M9_HQ_NPC_GaryMap.lua", "/scripts/M10_HQ_NPC_Rookie.lua", "/scripts/M10_HQ_NPC_Herbert.lua", "/scripts/M9_HQ_Item_Monitor.lua", "/scripts/M11_HQ_NPC_Herbert.lua", "/scripts/Doors_HQDoor2Sport.lua", "/scripts/M11_HQ_Item_CRDoor.lua", "/scripts/M9_HQ_Item_Monitor.lua", "/scripts/M10_HQ_NPC_Director.lua", "/scripts/M11_HQ_NPC_GaryTemp.lua", "/scripts/Doors_HQDoor2Gadget.lua", "/scripts/M11_HQ_Item_Notepad.lua", "/scripts/M11_HQ_NPC_GaryTemp.lua", "/scripts/M9_CombineCameraPhone.lua", "/scripts/M11_HQ_NPC_RookieTemp.lua", "/scripts/M11_HQ_NPC_RookieTemp.lua", "/scripts/M11_HQ_Item_OrangeBook.lua", "/scripts/HeadQuarters0_CodeBoard.lua", "/scripts/M11_HQ_Item_CRDoorPanel.lua", "/levels/HeadQuarters0_map_0.mpb", "/NPC/M11/Dot/", "/NPC/M11/Gary/", "/NPC/M11/Rookie/", "/NPC/M11/Herbert/", "/NPC/Puffles/Blue/", "/NPC/SpySeek/Gary/", "/UI/InventoryPanel/", "/MissionObjects/M11/", "/NPC/M11/JetPackGuy/", "/Objects/CommandCoach/", "/NPC/WaddleSquad/Gary/", "/NPC/WaddleSquad/Rookie/", "/NPC/WaddleSquad/Herbert/", "/NPC/WaddleSquad/Director/", "/NPC/QuestionsCrab/Klutzy/", "/NPC/MysteriousTremors/Gary/", "/NPC/WaddleSquad/JetPackGuy/", "/Location/HeadQuarters/state/", "/Location/HeadQuarters/touch/", "/Location/HeadQuarters/static/", "/MissionObjects/SpySeek/static/", "/MissionObjects/SpySeek/scripted/", "/MissionObjects/WaddleSquad/scripted/", "/scripts/Doors_IceRink_IceDoor2SnowForts.lua", "/levels/IceRink0_map_0.mpb", "/NPC/Puffles/Blue/", "/Objects/CommandCoach/", "/Location/IceRink/state/", "/fonts/bigUI.fnt", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/HerbertDock2.flc", "/scripts/M9_DuckNWrongPlace.lua", "/scripts/M9_DuckNWrongPlace.lua", "/scripts/M9_DuckNWrongPlace.lua", "/scripts/M9_DuckNWrongPlace.lua", "/scripts/C4_IceBerg_NPC_Gary.lua", "/scripts/ALL_Item_Inflatable.lua", "/scripts/M9_Iceberg_Item_Water.lua", "/scripts/C4_CombineBalloonHelium.lua", "/scripts/C4_IceBerg_Item_Aquagrabber.lua", "/levels/Iceberg0_map_0.mpb", "/levels/Iceberg0_map_1.mpb", "/NPC/M4/Gary/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M4/", "/Objects/WorldItems/", "/Objects/CommandCoach/", "/MissionObjects/SpySeek/scripted/", "/scripts/M11_LH_DVDs.lua", "/scripts/M11_LH_DVDBox.lua", "/scripts/M11_LH_Music_NPC.lua", "/scripts/M8_Beach_Item_Net.lua", "/scripts/M8_BalloonAndHelium.lua", "/scripts/M5_Lighthouse_NPC_NPC2.lua", "/scripts/M5_Lighthouse_NPC_NPC1.lua", "/scripts/M5_TrapCombineRope_Net.lua", "/scripts/M8_Lighthouse_Item_Box.lua", "/scripts/M10_Lighthouse_Item_Soda.lua", "/scripts/M8_Lighthouse_NPC_SodaNPC.lua", "/scripts/M8_Lighthouse_Item_Barrel.lua", "/scripts/M8_Lighthouse_Item_Barrel.lua", "/scripts/M8_Lighthouse_Item_Barrel.lua", "/scripts/M8_Lighthouse_Item_Barrel.lua", "/scripts/M8_Lighthouse_Item_Barrel.lua", "/scripts/M8_Lighthouse_NPC_BalloonNPC.lua", "/scripts/Doors_LighthouseDownDoor2LighthouseUp.lua", "/scripts/Doors_LighthouseDownDoor2LighthouseUp.lua", "/scripts/Doors_Lighthouse0_LighthouseDoor2Beach.lua", "/scripts/Doors_Lighthouse0_LighthouseDoor2Beacon.lua", "/levels/Lighthouse0_map_0.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/MissionObjects/SecretFur/", "/Location/Lighthouse/state/", "/Location/Lighthouse/touch/", "/NPC/M11/PartyNPCs/PartyNPC3/", "/NPC/SecretFur/LighthouseNPCs/", "/NPC/MysteriousTremors/SodaNPC/", "/NPC/MysteriousTremors/BalloonNPC/", "/MissionObjects/MysteriousTremors/static/", "/scripts/Lobby_Phone.lua", "/levels/Lobby_map_0.mpb", "/Location/Lobby/", "/NPC/Puffles/Blue/", "/Location/Pool/state/", "/scripts/Door2Attic.lua", "/scripts/Door2Attic.lua", "/scripts/M9_Lodge_bothNPCs.lua", "/scripts/Doors_LodgeDoor2Fish.lua", "/scripts/M5_SkiLodge_NPC_Sofa.lua", "/scripts/M5_World_Item_Candle.lua", "/scripts/M9_CombineConnectFour1.lua", "/scripts/M9_CombineConnectFour1.lua", "/scripts/M9_CombineConnectFour1.lua", "/scripts/Doors_Lodge0_LodgeDoor2Ski.lua", "/scripts/Doors_Lodge0_LodgeDoor2Ski.lua", "/levels/Lodge0_map_0.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Location/Pool/state/", "/Objects/CommandCoach/", "/Location/Lodge/state/", "/Location/Lodge/touch/", "/Location/Lodge/static/", "/NPC/SpySeek/LodgeNPCs/", "/NPC/SecretFur/LodgeNPC/", "/MissionObjects/SecretFur/", "/levels/Lounge0_map_0.mpb", "/NPC/Puffles/Blue/", "/Location/Lounge/state/", "/scripts/CornField_Door2MazeLeft.lua", "/scripts/Maze0_Item_Door2MazeLeft.lua", "/scripts/CornField_Door2MazeRight.lua", "/scripts/Maze0_Item_Door2MazeRight.lua", "/levels/Maze0_map_0.mpb", "/NPC/Puffles/Blue/", "/Location/Plaza/state/", "/MissionObjects/MysteriousTremors/static/", "/scripts/CornField_Door2MazeLeft.lua", "/scripts/CornField_Door2MazeRight.lua", "/scripts/Maze0_Item_Door2MazeLeft.lua", "/scripts/Maze0_Item_Door2MazeRight.lua", "/levels/Maze1_map_0.mpb", "/NPC/Puffles/Blue/", "/Location/Plaza/state/", "/MissionObjects/MysteriousTremors/static/", "/scripts/Maze0_Item_Door2MazeLeft.lua", "/scripts/Maze0_Item_Door2MazeRight.lua", "/levels/MazeBegin0_map_0.mpb", "/NPC/Puffles/Blue/", "/Location/Plaza/state/", "/MissionObjects/MysteriousTremors/static/", "/scripts/Doors_MineCrash0_MineCrashDoor2MineLair.lua", "/scripts/Doors_MineCrash0_MineCrashDoor2MineInterior.lua", "/levels/MineCrash0_map_0.mpb", "/NPC/Puffles/Blue/", "/Location/MineCrashSite/state/", "/scripts/Doors_MineInterior0_MineInteriorDoor2Pool.lua", "/scripts/Doors_MineInterior0_MineInteriorDoor2Minigame.lua", "/scripts/Doors_MineInterior0_MineInteriorDoor2MineExterior.lua", "/scripts/FP00_MineInterior0_LocationMineInterior_MineCarts.lua", "/scripts/FP00_MineInterior0_LocationMineInterior_MineCarts.lua", "/scripts/FP00_MineInterior0_LocationMineInterior_MineCarts.lua", "/levels/MineInterior0_map_0.mpb", "/NPC/Puffles/Blue/", "/Location/Dock/state/", "/Objects/CommandCoach/", "/Location/MineInterior/state/", "/Location/MineInterior/static/", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/HerbertDock3.flc", "/scripts/Doors_MineLair0_MineLairDoor2Exit.lua", "/levels/MineLair0_map_0.mpb", "/NPC/Puffles/Blue/", "/Location/MineTunnelExit/state/", "/scripts/M11_MineShackCorn_Rookie.lua", "/scripts/M11_MineShack_CornMazeDoor.lua", "/scripts/Doors_MineShack0_MineShackDoor2MineShed.lua", "/scripts/Doors_MineShack0_MineShackDoor2MineInterior.lua", "/scripts/Doors_MineShack0_MineShackDoor2MineInterior.lua", "/levels/MineShack0_map_0.mpb", "/levels/MineShack0_map_1.mpb", "/NPC/M11/Rookie/", "/NPC/Puffles/Blue/", "/Location/CornMaze/", "/Objects/CommandCoach/", "/Location/MineExterior/state/", "/Location/MineExterior/static/", "/scripts/M9_MineShack_NPC_Rory.lua", "/scripts/M10_MineShack_Item_Anvil.lua", "/scripts/M9_CombineStickBlueprint.lua", "/scripts/M9_MineShack_Item_Troughs.lua", "/scripts/M9_MineShack_Item_MineCart.lua", "/scripts/M9_MineShack_Item_SnakePiece.lua", "/scripts/M9_MineShack_Item_MineCartFixed.lua", "/scripts/M9_MineShack_Item_MineCartReceiver.lua", "/levels/MineShack1_map_0.mpb", "/levels/MineShack1_map_1.mpb", "/NPC/Puffles/Blue/", "/NPC/SpySeek/Rory/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/MissionObjects/SpySeek/static/", "/MissionObjects/SpySeek/scripted/", "/scripts/Doors_Mine2Int.lua", "/scripts/C4_MineShackFlood_NPC_JPG.lua", "/scripts/C4_MineShackFlood_NPC_Dot.lua", "/scripts/C4_MineShackFlood_NPC_Gary.lua", "/scripts/C4_MineShackFlood_Item_Anvil.lua", "/scripts/C4_MineShackFlood_NPC_Rookie.lua", "/scripts/C4_MineShackFlood_NPC_Herbert.lua", "/scripts/C4_MineShackFlood_Item_Statue.lua", "/scripts/C4_MineShackFlood_Item_Geyser.lua", "/scripts/Doors_MineShack0_MineShackDoor2MineShed.lua", "/levels/MineShackFlood_map_0.mpb", "/levels/MineShackFlood_map_1.mpb", "/NPC/M4/Dot/", "/NPC/M4/JPG/", "/NPC/M4/Gary/", "/NPC/M4/Rookie/", "/NPC/M4/Herbert/", "/NPC/Puffles/Blue/", "/MissionObjects/M4/", "/UI/InventoryPanel/", "/Objects/WorldItems/", "/Location/MineExterior/state/", "/levels/MineShed0_map_0.mpb", "/NPC/Puffles/Blue/", "/Location/MineShed/state/", "/scripts/Doors_NightDoor2Lounge.lua", "/scripts/Doors_NightDoor2Lounge.lua", "/scripts/M10_NightClub_Item_Trap.lua", "/scripts/M10_NightClub0_ItemCage1.lua", "/scripts/M10_NightClub_Item_Lever.lua", "/scripts/M10_NightClub0_ItemCage1.lua", "/scripts/M10_NightClub_NPC_Herbert.lua", "/scripts/M10_NightClub_Item_GearBox.lua", "/scripts/M10_NightClub0_Item_Puffle.lua", "/scripts/M10_NightClub_Item_PulleyRope.lua", "/scripts/Doors_NightClub0_NightDoor2Boiler.lua", "/scripts/Doors_NightClub0_NightDoor2Lounge.lua", "/levels/NightClub0_map_0.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Location/Pool/state/", "/Objects/CommandCoach/", "/NPC/WaddleSquad/Herbert/", "/Location/NightClub/state/", "/Location/NightClub/static/", "/Location/HeadQuarters/static/", "/MissionObjects/WaddleSquad/scripted/", "/scripts/M10_NightClub_NPC_JPG.lua", "/scripts/M10_NightClub_NPC_JPG.lua", "/scripts/M10_NightClub_NPC_Gary.lua", "/scripts/M10_NightClub_NPC_Rookie.lua", "/scripts/M10_NightClub_FallenCage.lua", "/scripts/M10_NightClub_FallenCage.lua", "/scripts/M10_NightClub_FallenCage.lua", "/scripts/M10_NightClub_Spotlight1.lua", "/scripts/M10_NightClub_Spotlight2.lua", "/scripts/M10_NightClub_Spotlight3.lua", "/scripts/M10_NightClub_Spotlight1.lua", "/scripts/M10_NightClub_Spotlight2.lua", "/scripts/M10_NightClub_Spotlight3.lua", "/scripts/M10_NightClub_NPC_Rookie.lua", "/scripts/M10_NightClub_NPC_Herbert.lua", "/scripts/M10_NightClub_NPC_Herbert.lua", "/scripts/M10_NightClub_Item_JetPack.lua", "/levels/NightClubMagnet0_map_0.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/NPC/WaddleSquad/Gary/", "/NPC/WaddleSquad/Rookie/", "/NPC/WaddleSquad/Herbert/", "/Location/NightClub/static/", "/NPC/WaddleSquad/JetPackGuy/", "/MissionObjects/WaddleSquad/scripted/", "/p/", "/scripts/Doors_NightDoor2Lounge.lua", "/levels/NightClubTremors0_map_0.mpb", "/NPC/Puffles/Blue/", "/Objects/CommandCoach/", "/Location/NightClub/state/", "/Location/NightClub/static/", "/scripts/Doors_PetShop0_PetShopDoor2Employees.lua", "/levels/PetShop0_map_0.mpb", "/NPC/Puffles/Blue/", "/Location/PetShop/state/", "/Location/PetShop/touch/", "/Location/PetShop/static/", "/scripts/M5_PizzaShop_Mess.lua", "/scripts/M5_PizzaShop_Mess.lua", "/scripts/M5_World_Item_Candle.lua", "/scripts/M5_World_Item_Candle.lua", "/scripts/M5_World_Item_Candle.lua", "/scripts/M5_PizzaShop_Item_Pizza.lua", "/scripts/M5_PizzaShop_NPC_PizzaChef.lua", "/scripts/M5_PizzaShop_Item_HotSauce.lua", "/scripts/M6_PizzaShop_NPC_PizzaChef.lua", "/scripts/M8_PizzaShop_NPC_PizzaChef.lua", "/scripts/C4_PizzaShop_NPC_PizzaChef.lua", "/scripts/M5_PizzaShop_Item_HotSauce.lua", "/scripts/C4_PizzaParlor_Item_MopNWig.lua", "/scripts/M5_PizzaShop_Item_ChocoSauce.lua", "/scripts/C4_PizzaParlor_Item_PizzaPan.lua", "/scripts/M5_PizzaShop_Item_ChocoSauce.lua", "/scripts/Doors_PizzaParlorDoor2Kitchen.lua", "/levels/PizzaParlor0_map_0.mpb", "/NPC/Puffles/Blue/", "/MissionObjects/M4/", "/UI/InventoryPanel/", "/Location/PizzaShop/", "/Objects/CommandCoach/", "/NPC/SecretFur/PizzaChef/", "/Location/PizzaShop/state/", "/Location/PizzaShop/touch/", "/MissionObjects/SecretFur/", "/Location/PizzaShop/static/", "/NPC/MysteriousTremors/PizzaChef/", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/TownGift1.flc", "/scripts/M9_Plaza_NPC1.lua", "/scripts/M9_Plaza_NPC1.lua", "/scripts/M9_Plaza_NPC1.lua", "/scripts/C4_Plaza_NPC_Dot.lua", "/scripts/M9_GumOnPenguins.lua", "/scripts/Doors_PlazaDoor2Pet.lua", "/scripts/Doors_PlazaDoor2Pizza.lua", "/scripts/M8_MapCombineLeftRight.lua", "/scripts/M8_Plaza_Item_MapPiece.lua", "/scripts/M8_Plaza_NPC_GenericNPC.lua", "/scripts/M8_Plaza_NPC_GenericNPC.lua", "/scripts/M8_Plaza_NPC_GenericNPC.lua", "/scripts/M8_Plaza_NPC_GenericNPC.lua", "/scripts/M8_Plaza_NPC_NewspaperNPC.lua", "/scripts/Doors_Plaza0_PlazaDoor2Pet.lua", "/scripts/M8_Plaza_NPC_paperNPCPizza.lua", "/scripts/Doors_PlazaDoor2Wilderness.lua", "/scripts/Doors_PlazaDoor2TheaterLeft.lua", "/scripts/Doors_Plaza0_PlazaDoor2Pool.lua", "/scripts/Doors_PlazaDoor2TheaterLeft.lua", "/scripts/Doors_PlazaDoor2TheaterRight.lua", "/scripts/Doors_PlazaDoor2TheaterRight.lua", "/scripts/Doors_Plaza0_PlazaDoor2Wilderness.lua", "/levels/Plaza0_map_0.mpb", "/levels/Plaza0_map_1.mpb", "/NPC/M4/Dot/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M3/", "/Objects/CommandCoach/", "/Location/Plaza/state/", "/NPC/SpySeek/PlazaNPCs/", "/Location/Plaza/static/", "/MissionObjects/SecretFur/", "/NPC/MysteriousTremors/PaperNPC/", "/NPC/MysteriousTremors/StageNPCs/", "/MissionObjects/MysteriousTremors/scripted/", "/scripts/Doors_Pool2Mine.lua", "/scripts/C4_LifePreserver.lua", "/scripts/Doors_Lock_Generic.lua", "/scripts/ALL_Item_Inflatable.lua", "/scripts/Doors_Pool0_PoolDoor2Boiler.lua", "/levels/Pool0_map_0.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Location/Pool/state/", "/Objects/CommandCoach/", "/scripts/Doors_PuffleTraining0_PuffleTrainingDoor2Dojo.lua", "/levels/PuffleTraining0_map_0.mpb", "/NPC/Puffles/Blue/", "/Objects/CommandCoach/", "/Location/PuffleTraining/state/", "/scripts/C3_SkiHill_NPC.lua", "/scripts/C3_ClimbDownSkiHill.lua", "/scripts/C3_ClimbDownSkiHill.lua", "/scripts/M9_SkiHill_Item_Post.lua", "/scripts/M6_SkiHill_NPC_Klutzy.lua", "/scripts/M9_CombineKitestickString.lua", "/scripts/M9_CombineKitestringReceiver.lua", "/scripts/M6_SkiHill_Item_door2wilderness.lua", "/scripts/Doors_SkiHill0_SkiHillDoor2SkiVillage.lua", "/levels/SkiHill0_map_0.mpb", "/levels/SkiHill0_map_1.mpb", "/Location/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M3/", "/Objects/CommandCoach/", "/Location/Mountain/state/", "/NPC/QuestionsCrab/Klutzy/", "/Location/Mountain/static/", "/MissionObjects/SecretFur/", "/NPC/MysteriousTremors/TownNPC1/", "/MissionObjects/SpySeek/scripted/", "/scripts/M11_Marbles.lua", "/scripts/M11_Village_JPG.lua", "/@/", "/@/", "/scripts/M11_DVD_Machine.lua", "/scripts/C2_Ski_Village_NPC.lua", "/scripts/M11_Village_Rookie.lua", "/scripts/M11_Village_Rookie.lua", "/scripts/M11_Village_CornCob.lua", "/scripts/M11_Village_Herbert.lua", "/scripts/M11_Village_NPCsTemp.lua", "/scripts/M11_Village_NPCsTemp.lua", "/scripts/M11_Village_NPCsTemp.lua", "/scripts/M11_Village_NPCsTemp.lua", "/scripts/M11_Village_NPCsTemp.lua", "/scripts/M11_Village_NPCsTemp.lua", "/scripts/M6_SkiVillage_NPC_NPC2.lua", "/scripts/M6_SkiVillage_NPC_NPC1.lua", "/scripts/M6_SkiVillageDoor2Dock.lua", "/scripts/M6_SkiVillageDoor2Beach.lua", "/scripts/M6_SkiVillageDoor2Beach.lua", "/scripts/Doors_SkiVillageDoor2Lodge.lua", "/scripts/Doors_SkiVillageDoor2SkiHill.lua", "/scripts/Doors_SkiVillage0_SkiVillageDoor2SkiLodge.lua", "/levels/SkiVillage0_map_0.mpb", "/levels/SkiVillage0_map_1.mpb", "/NPC/M11/Rookie/", "/NPC/M11/Herbert/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M11/", "/NPC/M11/JetPackGuy/", "/Objects/CommandCoach/", "/MissionObjects/SecretFur/", "/Location/SkiVillage/state/", "/Location/SkiVillage/touch/", "/Location/SkiVillage/static/", "/NPC/M11/PartyNPCs/PartyNPC2/", "/NPC/M11/PartyNPCs/PartyNPC1/", "/NPC/M11/PartyNPCs/PartyNPC3/", "/Location/SkiVillageNew/State/", "/NPC/MysteriousTremors/TownNPC2/", "/NPC/QuestionsCrab/SkiVillageNPCs/", "/scripts/M11_VillageDot.lua", "/scripts/M11_Village_JPG.lua", "/scripts/M11_Village_Rookie.lua", "/scripts/M11_Village_NPCsTemp.lua", "/scripts/M11_Village_NPCsTemp.lua", "/scripts/M11_Village_NPCsTemp.lua", "/scripts/M11_Village_NPC_GaryTemp.lua", "/levels/SkiVillage1_map_0.mpb", "/levels/SkiVillage1_map_1.mpb", "/NPC/M11/Dot/", "/NPC/M11/Gary/", "/NPC/M11/Rookie/", "/NPC/Puffles/Blue/", "/NPC/M11/JetPackGuy/", "/Location/SkiVillage/state/", "/NPC/M11/PartyNPCs/PartyNPC3/", "/NPC/M11/PartyNPCs/PartyNPC1/", "/NPC/M11/PartyNPCs/PartyNPC2/", "/scripts/C4_Item_DuckTube.lua", "/scripts/C4_SkiVillage_NPC_JPG.lua", "/scripts/C4_CombineBalloonTape.lua", "/scripts/C4_CombineBalloonHelium.lua", "/scripts/C4_SkiVillage_Item_Belt.lua", "/scripts/C4_SkiVillage_NPC_Rookie.lua", "/scripts/C4_SkiVillage_NPC_Herbert.lua", "/scripts/C4_SkiVillage_Item_Klutzy.lua", "/scripts/C4_SkiVillage_NPC_Herbert.lua", "/scripts/C4_SkiVillage_Item_Balloon.lua", "/scripts/Doors_SkiVillageDoor2Beach.lua", "/scripts/Doors_SkiVillageDoor2Beach.lua", "/scripts/C4_SkiVillage_Herbert_Basket.lua", "/levels/SkiVillageFlood_map_0.mpb", "/levels/SkiVillageFlood_map_1.mpb", "/NPC/M4/JPG/", "/NPC/M4/Rookie/", "/NPC/M4/Klutzy/", "/NPC/M4/Herbert/", "/NPC/Puffles/Blue/", "/MissionObjects/M4/", "/UI/InventoryPanel/", "/Objects/WorldItems/", "/Objects/CommandCoach/", "/Location/SkiVillage/state/", "/Location/SkiVillage/static/", "/Location/SkiVillageNew/State/", "/scripts/C2_SkyLightPanel.lua", "/scripts/C2_SkyLightPanel.lua", "/levels/SkyLight0_map_0.mpb", "/NPC/Puffles/Blue/", "/MissionObjects/SecretFur/", "/scripts/Doors_SnowFortsDoor2Ice.lua", "/scripts/M8_snowFort_Item_MapPiece.lua", "/scripts/M5_SnowForts_Item_SnowPile.lua", "/scripts/M5_SkiVillage_Item_SnakePiece.lua", "/scripts/Doors_SnowForts0_SnowFortsDoor2Ice.lua", "/scripts/Doors_SnowForts0_SnowFortsDoor2Ice.lua", "/scripts/Doors_SnowForts0_SnowFortsDoor2Plaza.lua", "/scripts/SnowForts0_LocationSnowForts_GearRightSide.lua", "/levels/SnowForts0_map_0.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Objects/WorldItems/", "/Objects/CommandCoach/", "/Location/SnowForts/touch/", "/MissionObjects/SecretFur/", "/Location/SnowForts/state/", "/Location/SnowForts/static/", "/MissionObjects/SpySeek/static/", "/MissionObjects/MysteriousTremors/scripted/", "/scripts/M8_SportShop_Pegs.lua", "/scripts/C4_CombineBalloonTape.lua", "/scripts/M6_SportShop_NPC_Klutzy.lua", "/scripts/M6_SportShop_SportShopNPC.lua", "/scripts/Doors_SportShopDoor2Garys.lua", "/scripts/Doors_SportShop0_SportShopDoor2HQ.lua", "/scripts/Doors_SportShop0_SportShopDoor2Ski.lua", "/scripts/Doors_SportShop0_SportShopDoor2Garys.lua", "/levels/SportShop0_map_0.mpb", "/Objects/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/Location/SportShop/touch/", "/Location/SportShop/state/", "/NPC/QuestionsCrab/Klutzy/", "/Location/SportShop/static/", "/NPC/QuestionsCrab/SportShopNPC/", "/MissionObjects/MysteriousTremors/static/", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/TownGift2.flc", "/scripts/M11_VideoStage_NPC.lua", "/scripts/C4_Stage_Item_Paint.lua", "/scripts/C4_Stage_Item_Paint.lua", "/scripts/C4_Stage_StageManager.lua", "/scripts/C4_Stage_Item_WhiteBeard.lua", "/scripts/C4_Stage_Item_WhiteBeard.lua", "/scripts/Doors_Stage0_Stage2Plaza.lua", "/levels/Stage0_map_0.mpb", "/NPC/M4/Manager/", "/NPC/Puffles/Blue/", "/MissionObjects/M4/", "/UI/InventoryPanel/", "/NPC/M11/MancallaNPC/", "/Objects/CommandCoach/", "/Location/Stage/state/", "/scripts/C3_MountainTop_JPG.lua", "/scripts/C3_MountainTop_Dot.lua", "/scripts/C3_MountainTop_Rookie.lua", "/scripts/C4_MountainTop_NPC_Dot.lua", "/scripts/C4_MountainTop_NPC_JPG.lua", "/scripts/C4_MountainTop_NPC_Rookie.lua", "/scripts/M8_TallestMountain_Item_Jetpack.lua", "/scripts/M8_TallestMountain_Item_SnakePiece.lua", "/levels/TallestMountainTop0_map_0.mpb", "/levels/TallestMountainTop0_map_1.mpb", "/NPC/M4/Dot/", "/NPC/M4/JPG/", "/NPC/M3/JPG/", "/NPC/M3/Dot/", "/NPC/M4/Rookie/", "/NPC/M3/Rookie/", "/NPC/Puffles/Blue/", "/MissionObjects/M4/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/Location/LighthouseUp/script/", "/scripts/C3_IceBricks.lua", "/scripts/C3_IceBricks.lua", "/scripts/C3_IceBricks.lua", "/scripts/C3_IceBricks.lua", "/scripts/C3_IceBricks.lua", "/scripts/C3_MountainTop_JPG.lua", "/scripts/C3_MountainTop_Dot.lua", "/scripts/C3_MountainTop_Rookie.lua", "/scripts/C3_MountainTop_Herbert.lua", "/levels/TallestMountainTop1_map_0.mpb", "/levels/TallestMountainTop1_map_1.mpb", "/NPC/M3/Dot/", "/NPC/M3/JPG/", "/NPC/M3/Rookie/", "/NPC/M3/Herbert/", "/NPC/Puffles/Blue/", "/MissionObjects/M3/", "/UI/InventoryPanel/", "/Location/HerbertsBaseLobby/static/", "/scripts/C3_ClimbToSaveDot.lua", "/scripts/C3_SaveJetPackGuy.lua", "/scripts/C3_SaveJetPackGuy.lua", "/scripts/C3_TallestMt_Rookie.lua", "/scripts/C3_TallestMt_RookieHat.lua", "/scripts/C3_SavePufflesInIceCage.lua", "/scripts/C3_SavePufflesInIceCage.lua", "/levels/TallestMt1_map_0.mpb", "/levels/TallestMt1_map_1.mpb", "/Objects/", "/Location/", "/NPC/M3/Rookie/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M3/", "/Objects/CommandCoach/", "/Location/Beach/state/", "/Location/Mountain/state/", "/scripts/WindmillStand.lua", "/scripts/C3_ClimbToMtTop.lua", "/scripts/CreatePropeller.lua", "/scripts/CreatePropeller.lua", "/scripts/C3_BreakFakeTree.lua", "/scripts/C3_TallestMt_Dot.lua", "/scripts/C3_CombineMetalScraps.lua", "/scripts/C3_CombineMetalScraps.lua", "/scripts/C3_CombineMetalScraps.lua", "/scripts/C3_CombineMetalScraps.lua", "/levels/TallestMt2_map_0.mpb", "/levels/TallestMt2_map_1.mpb", "/NPC/M3/Dot/", "/NPC/M3/Puffle/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/MissionObjects/M3/", "/Location/Dock/state/", "/Objects/CommandCoach/", "/MissionObjects/SpySeek/scripted/", "/scripts/C3_Boulder.lua", "/scripts/C3_Boulder.lua", "/scripts/C3_TallestMt_JPG.lua", "/scripts/C3_TallestMt_JPG.lua", "/scripts/C3_Free_PurplePuffle.lua", "/levels/TallestMt3_map_0.mpb", "/levels/TallestMt3_map_1.mpb", "/NPC/M3/JPG/", "/NPC/Puffles/Blue/", "/MissionObjects/M3/", "/Objects/CommandCoach/", "/Location/Lighthouse/state/", "/scripts/C3_YellowPuffle.lua", "/scripts/C3_ClimbBaseofTallMt.lua", "/levels/TallestMtnBase_map_0.mpb", "/levels/TallestMtnBase_map_1.mpb", "/Location/", "/NPC/Puffles/Blue/", "/NPC/Puffles/Yellow/", "/Objects/CommandCoach/", "/MissionObjects/SpySeek/scripted/", "/scripts/M5_Town_TownNPC.lua", "/scripts/M5_Town_TownNPC.lua", "/scripts/M11_Town_GameNPCs.lua", "/scripts/M8_Town0_NPC_Flit.lua", "/scripts/M8_Town0_NPC_Flit.lua", "/scripts/M8_Town0_NPC_Flit.lua", "/scripts/M8_CookieSign_Item.lua", "/scripts/Doors_TownDoor2Gift.lua", "/scripts/Doors_TownDoor2Snow.lua", "/scripts/M10_Town0_TownItems.lua", "/scripts/M8_Town_NPC_Generic.lua", "/scripts/M8_Town_NPC_Generic.lua", "/scripts/M9_Town_BrownPenguin.lua", "/scripts/Doors_TownDoor2Beach.lua", "/scripts/Doors_TownDoor2Coffee.lua", "/scripts/Doors_TownDoor2Coffee.lua", "/scripts/Doors_TownDoor2Coffee.lua", "/scripts/M8_Town_Item_MapPiece2.lua", "/scripts/Doors_TownDoor2NightClub.lua", "/scripts/M8_Town_NPC_GiftShopOwner.lua", "/scripts/M8_Town_NPC_GiftShopOwner.lua", "/scripts/TownTremors0_Item_GiftShop.lua", "/@/", "/levels/Town0_map_0.mpb", "/levels/Town0_map_1.mpb", "/NPC/M3/TownNPC/", "/NPC/M11/TownNPCs/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Objects/WorldItems/", "/Location/Town/state/", "/Location/Town/static/", "/Objects/CommandCoach/", "/MissionObjects/SecretFur/", "/NPC/SpySeek/BrownPenguin/", "/NPC/MysteriousTremors/Puffle/", "/NPC/MysteriousTremors/TownNPC2/", "/NPC/MysteriousTremors/TownNPC1/", "/MissionObjects/WaddleSquad/scripted/", "/MissionObjects/MysteriousTremors/static/", "/MissionObjects/MysteriousTremors/scripted/", "/scripts/M8_Town0_NPC_Flit.lua", "/scripts/M8_Town0_NPC_Flit.lua", "/scripts/M8_Town0_NPC_Flit.lua", "/scripts/M8_CookieSign_Item.lua", "/scripts/M8_Town_NPC_Generic.lua", "/scripts/M8_Town_Item_MapPiece.lua", "/scripts/M8_Town_Item_MapPiece2.lua", "/scripts/M8_MapCombineRightLeft.lua", "/scripts/M8_Town_NPC_GiftShopOwner.lua", "/scripts/Doors_Town0_TownDoor2Snow.lua", "/scripts/TownTremors0_Item_GiftShop.lua", "/@/", "/levels/TownTremors0_map_0.mpb", "/levels/TownTremors0_map_1.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Location/Town/state/", "/Objects/CommandCoach/", "/NPC/MysteriousTremors/Puffle/", "/NPC/MysteriousTremors/TownNPC2/", "/NPC/MysteriousTremors/TownNPC1/", "/MissionObjects/MysteriousTremors/state/", "/MissionObjects/MysteriousTremors/static/", "/MissionObjects/MysteriousTremors/scripted/", "/scripts/C1_Mines7_JPG.lua", "/scripts/C1_Mines7_Dot.lua", "/scripts/C1_Caves_Crate.lua", "/scripts/C1_Mines_Target.lua", "/scripts/C1_Mines_Target.lua", "/scripts/C1_Caves_Crate2.lua", "/scripts/C1_BendMetalBars.lua", "/scripts/C1_Mines7_Rookie.lua", "/scripts/C1_Mines_HangingTarget.lua", "/scripts/C1_CreateGrapplingHook.lua", "/scripts/C1_CreateGrapplingHook.lua", "/scripts/M1_PuffleTraining_Exit.lua", "/scripts/C1_JetpackAndStickCombine.lua", "/scripts/C1_JetpackAndStickCombine.lua", "/levels/TrainingCave1_map_0.mpb", "/NPC/M1/Dot/", "/NPC/M1/Rookie/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/NPC/M1/JetPackGuy/", "/MissionObjects/M1/", "/Objects/CommandCoach/", "/Location/SkiVillage/state/", "/Location/TrainingCave/state/", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/TownGift3.flc", "/scripts/C1_CaveBox.lua", "/scripts/C1_Mines2_JPG.lua", "/scripts/C1_Mines2_Dot.lua", "/scripts/C1_Mines2_Rookie.lua", "/scripts/C1_Mines_SpotToCrossRiver.lua", "/scripts/C1_Mines_SignToCrossRiver.lua", "/scripts/C1_Mines_CleanDirtyJackHammerParts.lua", "/levels/TrainingCave2_map_0.mpb", "/NPC/M1/Dot/", "/NPC/M1/Rookie/", "/NPC/Puffles/Blue/", "/NPC/M1/JetPackGuy/", "/UI/InventoryPanel/", "/MissionObjects/M1/", "/Objects/CommandCoach/", "/MissionObjects/MysteriousTremors/static/", "/scripts/C1_Training3_JPG.lua", "/scripts/C1_Training3_Dot.lua", "/scripts/C1_Training3_Rookie.lua", "/levels/TrainingCave3_map_0.mpb", "/NPC/M1/Dot/", "/NPC/M1/Rookie/", "/NPC/Puffles/Blue/", "/NPC/M1/JetPackGuy/", "/MissionObjects/M1/", "/Objects/CommandCoach/", "/Location/SkiVillage/state/", "/scripts/C1_ClimbWall.lua", "/scripts/C1_Mines4_JPG.lua", "/scripts/C1_Mines4_Dot.lua", "/scripts/C1_Mines4_Rookie.lua", "/scripts/C1_CreateGrapplingHook.lua", "/levels/TrainingCave4_map_0.mpb", "/NPC/M1/Dot/", "/NPC/M1/Rookie/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/NPC/M1/JetPackGuy/", "/Objects/CommandCoach/", "/Location/SkiVillage/state/", "/MissionObjects/MysteriousTremors/static/", "/scripts/C1_Mines5_Dot.lua", "/scripts/C1_Mines5_JPG.lua", "/scripts/C1_Mines5_Rookie.lua", "/scripts/C1_Mines5_Director.lua", "/levels/TrainingCave5_map_0.mpb", "/NPC/M1/Dot/", "/NPC/M1/Rookie/", "/NPC/M1/Director/", "/NPC/Puffles/Blue/", "/MissionObjects/M1/", "/Location/SkiVillage/state/", "/NPC/WaddleSquad/JetPackGuy/", "/scripts/C1_TC6_Sign.lua", "/scripts/C1_Mines6_JPG.lua", "/scripts/C1_Mines6_Dot.lua", "/scripts/C1_Mines6_Rookie.lua", "/scripts/C1_MetalWallDoor.lua", "/scripts/C1_MetalWallDoor.lua", "/levels/TrainingCave6_map_0.mpb", "/NPC/M1/Dot/", "/NPC/M2/Dot/", "/NPC/M1/Rookie/", "/NPC/Puffles/Blue/", "/NPC/M1/JetPackGuy/", "/MissionObjects/M1/", "/Location/Beach/state/", "/Objects/CommandCoach/", "/MissionObjects/M1/state/", "/NPC/WaddleSquad/JetPackGuy/", "/scripts/M1_CaveExit.lua", "/scripts/C1_Mines_PH.lua", "/scripts/M1_CaveExit.lua", "/scripts/C1_Mines_Dot.lua", "/scripts/C1_Mines_Dot.lua", "/scripts/C1_Mines_JPG.lua", "/scripts/C1_Mines_JPG.lua", "/scripts/C1_Mines_Rookie.lua", "/scripts/C1_Mines_Rookie.lua", "/scripts/C1_Mines_Director.lua", "/scripts/C1_Mines_PuffleTrainingDoors.lua", "/scripts/C1_Mines_PuffleTrainingDoors.lua", "/scripts/C1_Mines_PuffleTrainingDoors.lua", "/levels/TrainingCave7_map_0.mpb", "/Location/", "/NPC/M1/Dot/", "/NPC/M1/Rookie/", "/NPC/M1/Director/", "/NPC/Puffles/Blue/", "/MissionObjects/M1/", "/NPC/M1/JetPackGuy/", "/Objects/CommandCoach/", "/NPC/M1/PuffleHandler/", "/Location/PuffleTraining/script/", "/Location/WildernessStump/state/", "/scripts/M9_trough_tutorial.lua", "/scripts/M9_TroughGame_Trough.lua", "/scripts/M9_Troughs_ExitButton.lua", "/scripts/M9_TroughGame_LogHor1Left.lua", "/scripts/M9_TroughGame_LogVert3Bot.lua", "/scripts/M9_TroughGame_LogHor2Left.lua", "/scripts/M9_TroughGame_LogVert2Top.lua", "/scripts/M9_TroughGame_LogVert3Top.lua", "/scripts/M9_TroughGame_LogVert1Bot.lua", "/scripts/M9_TroughGame_LogVert1Top.lua", "/scripts/M9_TroughGame_LogVert2Bot.lua", "/scripts/M9_TroughGame_LogHor2Right.lua", "/scripts/M9_TroughGame_LogHor1Right.lua", "/levels/Trough_map_0.mpb", "/NPC/Puffles/Blue/", "/Microgames/Trough/", "/MissionObjects/UniversalItems/", "/scripts/M8_Tunnel_Item_SnakePiece.lua", "/scripts/M8_Tunnel_Item_GlassToSnakePiece.lua", "/levels/UG_Boiler0_map_0.mpb", "/Location/", "/NPC/Puffles/Blue/", "/Objects/CommandCoach/", "/MissionObjects/MysteriousTremors/state/", "/scripts/UGGift0_Door_GiftDoor.lua", "/scripts/UGGift0_Item_GiftShop.lua", "/levels/UG_Gift0_map_0.mpb", "/NPC/Puffles/Blue/", "/Objects/CommandCoach/", "/Location/Beach/state/", "/MissionObjects/MysteriousTremors/static/", "/MissionObjects/MysteriousTremors/scripted/", "/scripts/UGGift0_Item_GiftShop.lua", "/scripts/UGGift0_Item_GiftShop.lua", "/scripts/UGGift0_Item_GiftShop.lua", "/scripts/M8_UG1_Item_DoorToBoiler.lua", "/levels/UG_Gift1_map_0.mpb", "/Location/", "/NPC/Puffles/Blue/", "/Location/Beach/state/", "/MissionObjects/MysteriousTremors/static/", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/TownGift4.flc", "/scripts/C2_WildernessBerry_JPG.lua", "/levels/WildernessBerry0_map_0.mpb", "/levels/WildernessBerry0_map_1.mpb", "/NPC/Puffles/Blue/", "/NPC/M2/JetPackGuy/", "/MissionObjects/M2/", "/Objects/CommandCoach/", "/Location/WildernessBerry/state/", "/Location/WildernessBerry/touch/", "/MissionObjects/QuestionsCrab/static/", "/scripts/C2_CrabCostume.lua", "/scripts/M6_HQ_NPC_Klutzy.lua", "/scripts/C2_Wilderness_JPG.lua", "/scripts/C2_Wilderness_Dot.lua", "/scripts/Doors_WildCaveDoor2CaveInt.lua", "/scripts/M6_Wilderness_Item_PetDoor2Cave.lua", "/scripts/Doors_WildernessCave0_WildCaveDoor2River.lua", "/scripts/Doors_WildernessCave0_WildCaveDoor2Stump.lua", "/levels/WildernessCave0_map_0.mpb", "/levels/WildernessCave0_map_1.mpb", "/NPC/M2/Dot/", "/NPC/Puffles/Blue/", "/NPC/M2/JetPackGuy/", "/MissionObjects/M2/", "/Objects/CommandCoach/", "/NPC/QuestionsCrab/Klutzy/", "/Location/WildernessCave/state/", "/MissionObjects/QuestionsCrab/state/", "/MissionObjects/QuestionsCrab/static/", "/scripts/M6_HotOberrySauce2Bag.lua", "/scripts/M6_Wilderness_Item_Bag.lua", "/levels/WildernessClearing0_map_0.mpb", "/levels/WildernessClearing0_map_1.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Objects/CommandCoach/", "/Location/WildernessClearing/state/", "/MissionObjects/QuestionsCrab/state/", "/MissionObjects/QuestionsCrab/static/", "/scripts/M6_Wilderness_NPC_Klutzy.lua", "/levels/WildernessCliff0_map_0.mpb", "/levels/WildernessCliff0_map_1.mpb", "/NPC/Puffles/Blue/", "/MissionObjects/M3/", "/Objects/CommandCoach/", "/Location/Beach/state/", "/NPC/QuestionsCrab/Klutzy/", "/Location/WildernessCliff/state/", "/scripts/M6_Cliff_Item_Mountain.lua", "/scripts/M6_GrapplingAnchor2Rope.lua", "/levels/WildernessCliffClose0_map_0.mpb", "/levels/WildernessCliffClose0_map_1.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/Location/Beach/state/", "/MissionObjects/MysteriousTremors/static/", "/scripts/C2_HerbsDoor.lua", "/scripts/C2_FakeRock2.lua", "/scripts/C2_HerbsDoorMat.lua", "/scripts/C2_HerbsDoorMat.lua", "/scripts/M6_Wilderness_NPC_Flare.lua", "/scripts/Doors_WildernessPuffle_WildStumpDoor2Berry.lua", "/scripts/Doors_WildernessPuffle_WildStumpDoor2Berry.lua", "/scripts/Doors_WildernessPuffle_WildStumpDoor2Clearing.lua", "/scripts/Doors_WildernessPuffle_WildStumpDoor2Clearing.lua", "/levels/WildernessPuffle0_map_0.mpb", "/levels/WildernessPuffle0_map_1.mpb", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/NPC/Puffles/Black/", "/MissionObjects/M2/", "/Objects/CommandCoach/", "/Location/WildernessStump/state/", "/MissionObjects/QuestionsCrab/static/", "/scripts/C3_WIceBridge.lua", "/scripts/C3_WhitePuffle.lua", "/scripts/C3_WRiverBridge.lua", "/scripts/C3_WRiverBridge.lua", "/scripts/M6_Wilderness_River_Fish.lua", "/scripts/M6_Wilderness_Item_SnakePiece.lua", "/scripts/Doors_WildernessRiver_WildRiverDoor2Berry.lua", "/scripts/Doors_WildernessRiver_WildRiverDoor2Berry.lua", "/levels/WildernessRiver0_map_0.mpb", "/levels/WildernessRiver0_map_1.mpb", "/Location/", "/NPC/Puffles/Blue/", "/UI/InventoryPanel/", "/NPC/Puffles/White/", "/MissionObjects/M3/", "/Objects/CommandCoach/", "/MissionObjects/SecretFur/", "/Location/WildernessRiver/state/", "/Location/WildernessRiver/static/", "/MissionObjects/QuestionsCrab/static/", "/MissionObjects/MysteriousTremors/static/", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/TownGift5.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/TownGiftFail1.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/TownGiftFail2.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/TownGiftFail3.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/UGGift1.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/UGGift2.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/UGGift3.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/UGGift4.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/UGGiftFail1.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/UGGiftFail2.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/MysteriousTremors/UGGiftFail3.flc", "/fonts/bigUI.fnt", "/bg/Cutscenes/FPMOutro_top", "/flc/SecretFur/CaptureKlutzy.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/QuestionsCrab/FallCliff.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/QuestionsCrab/KlutzySkiLift.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/QuestionsCrab/SuperFlare.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/QuestionsCrab/KlutzyEscapes.flc", "/bg/HerbsCamp", "/bg/Cutscenes/FPMOutro_top", "/CutsceneAnims/BinocHerbsCamp/code", "/CutsceneAnims/BinocHerbsCamp/smoke", "/CutsceneAnims/BinocHerbsCamp/bucket", "/CutsceneAnims/BinocHerbsCamp/leftMeter", "/CutsceneAnims/BinocHerbsCamp/sliderDot", "/CutsceneAnims/BinocHerbsCamp/threeLights", "/CutsceneAnims/BinocHerbsCamp/klutzyLeave", "/CutsceneAnims/BinocHerbsCamp/klutzyBlink", "/CutsceneAnims/BinocHerbsCamp/bucketStart", "/CutsceneAnims/BinocHerbsCamp/klutzyDance", "/CutsceneAnims/BinocHerbsCamp/herbertStart", "/CutsceneAnims/BinocHerbsCamp/herbertLeave", "/CutsceneAnims/BinocHerbsCamp/herbertDance", "/DGamer/Scenes/DG1_Gifting_Sprite", "/Scenes/DG1_Gifting_Sprite", "/fonts/bigUI.fnt", "/bg/Cutscenes/FPMOutro_top", "/flc/QuestionsCrab/CageDrop.flc", "/CutsceneAnims/QuestionsCrab/HerbPeek", "/UI/ConversationSystem/npcChatBubbleFlip2", "/CutsceneAnims/QuestionsCrab/HerbPeek_talk", "/CutsceneAnims/QuestionsCrab/HerbPeek_appear", "/bg/Cutscenes/FPMOutro_top", "/flc/QuestionsCrab/CageLift.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/QuestionsCrab/HerbertBackfire.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/WaddleSquad/NCwallBreak.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/WaddleSquad/NC_cageDrop.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/WaddleSquad/NC_trapped.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/WaddleSquad/beach_blastOff.flc", "/bg/Cutscenes/BlankBlack", "/flc/WaddleSquad/NC_trappedJP.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/QuestionsCrab/GHookToss.flc", "/fonts/bigUI.fnt", "/bg/Cutscenes/M9_Video", "/bg/Cutscenes/FPMOutro_top", "/CutsceneAnims/M9Video/Herb10", "/CutsceneAnims/M9Video/BallFly", "/CutsceneAnims/M9Video/Herb4All", "/CutsceneAnims/M9Video/BallLeft", "/CutsceneAnims/M9Video/Herb1ExpD", "/CutsceneAnims/M9Video/Herb1ExpE", "/CutsceneAnims/M9Video/Herb2Read", "/CutsceneAnims/M9Video/BallRight", "/CutsceneAnims/M9Video/Herb3Tran", "/CutsceneAnims/M9Video/Herb1ExpA", "/CutsceneAnims/M9Video/Herb1ExpB", "/CutsceneAnims/M9Video/Herb1ExpC", "/CutsceneAnims/M9Video/Herb11Left", "/CutsceneAnims/M9Video/StaticLeft", "/CutsceneAnims/M9Video/Herb11Right", "/CutsceneAnims/M9Video/Klutzy5Hide", "/CutsceneAnims/M9Video/StaticRight", "/CutsceneAnims/M9Video/BallPullLeft", "/CutsceneAnims/M9Video/Klutzy1Blink", "/CutsceneAnims/M9Video/Klutzy6Click", "/CutsceneAnims/M9Video/BallPullRight", "/UI/ConversationSystem/npcChatBubble", "/CutsceneAnims/M9Video/Klutzy4BallPop", "/CutsceneAnims/M9Video/Klutzy2LookBall", "/CutsceneAnims/M9Video/Klutzy3LookBlink", "/UI/ConversationSystem/npcChatBubbleFlip", "/bg/Cutscenes/FPMOutro_top", "/flc/QuestionsCrab/Woodchopper.flc", "/fonts/bigUI.fnt", "/CutsceneAnims/GaryHead", "/CutsceneAnims/M11Video/goJPG", "/bg/Cutscenes/M11Vid_GiftShop", "/bg/Cutscenes/M11Vid_GiftOffice", "/CutsceneAnims/M11Video/gsRookie", "/bg/Cutscenes/CutsceneText_top_Penguin", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/M11VideoMachineOpen", "/CutsceneAnims/M11DVDPanel/panelFall1", "/CutsceneAnims/M11DVDPanel/panelFall2", "/CutsceneAnims/M11DVDPanel/MineShackType", "/CutsceneAnims/C2/CamFlash", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/C2Camera/Palm", "/CutsceneAnims/C2/CameraLight", "/CutsceneAnims/C2/cameraBattery3", "/CutsceneAnims/C2/cameraBattery2", "/CutsceneAnims/C2/CamFlash", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/C2Camera/Palm", "/CutsceneAnims/C2/CameraLight", "/CutsceneAnims/C2/cameraBattery2", "/CutsceneAnims/C2/cameraBattery1", "/CutsceneAnims/C2/CamFlash", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/C2Camera/Palm", "/CutsceneAnims/C2/CameraLight", "/CutsceneAnims/C2/cameraBattery1", "/CutsceneAnims/C2/cameraBattery0", "/CutsceneAnims/C2/CamFlash", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/C2Camera/Bench", "/CutsceneAnims/C2/CameraLight", "/CutsceneAnims/C2/cameraBattery3", "/CutsceneAnims/C2/cameraBattery2", "/CutsceneAnims/C2/CamFlash", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/C2Camera/Bench", "/CutsceneAnims/C2/CameraLight", "/CutsceneAnims/C2/cameraBattery2", "/CutsceneAnims/C2/cameraBattery1", "/CutsceneAnims/C2/CamFlash", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/C2Camera/Bench", "/CutsceneAnims/C2/CameraLight", "/CutsceneAnims/C2/cameraBattery1", "/CutsceneAnims/C2/cameraBattery0", "/CutsceneAnims/C2/CamFlash", "/bg/Cutscenes/FPMOutro_top", "/CutsceneAnims/C2/CameraLight", "/bg/Cutscenes/C2Camera/Supply", "/CutsceneAnims/C2/cameraBattery3", "/CutsceneAnims/C2/cameraBattery2", "/Scenes/DG1_ChatGrey", "/CutsceneAnims/C2/CamFlash", "/bg/Cutscenes/FPMOutro_top", "/CutsceneAnims/C2/CameraLight", "/bg/Cutscenes/C2Camera/Supply", "/CutsceneAnims/C2/cameraBattery2", "/CutsceneAnims/C2/cameraBattery1", "/CutsceneAnims/C2/CamFlash", "/bg/Cutscenes/FPMOutro_top", "/CutsceneAnims/C2/CameraLight", "/bg/Cutscenes/C2Camera/Supply", "/CutsceneAnims/C2/cameraBattery1", "/CutsceneAnims/C2/cameraBattery0", "/CutsceneAnims/C2/CamFlash", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/C2Camera/Gary", "/CutsceneAnims/C2/CameraLight", "/CutsceneAnims/C2/cameraBattery4", "/CutsceneAnims/C2/cameraBattery3", "/flc/C4/CS1_HerbCrash.flc", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/FPMOutro_top", "/flc/C4/CS2_IslandSink1.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/C4/CS3_IslandRaise1.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/C4/CS4_IslandRaise2.flc", "/bg/Cutscenes/C2_DoorOpen", "/CutsceneAnims/C2/DoorOpen", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/FPMOutro_top", "/flc/C3/DestroyedMagnifyingGlass.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/M11/explodePopcorn.flc", "/CutsceneAnims/SnakePiece", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/SnakePieceBG", "/flc/C3/AgentCapture.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/C1/NPaper.flc", "/fonts/Burbank.fnt", "/NPC/M2/Herbert/phone", "/UI/HelpMenus/StylusDrag", "/BG/SpyPod/SpyPod_bottom", "/BG/SpyPod/comunicator_top", "/bg/Cutscenes/FPMOutro_top", "/bg/Microgames/MicrogameTS", "/bg/HelpMenus/StylusDrag_bottom", "/UI/SpyPod/Communicator/Director", "/flc/GogglesOn.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/GogglesOn.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/GogglesOn.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/GogglesOn.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/GogglesOn.flc", "/bg/Cutscenes/FPMOutro_top", "/flc/GogglesOn.flc", "/bg/Cutscenes/FPMOutro_top", "/fonts/Burbank.fnt", "/bg/Microgames/MicrogameTS", "/UI/HelpMenus/inventoryCombine", "/bg/HelpMenus/InventoryCombine_bottom", "/Scenes/DG1_MainMenuMasterPalette", "/Scenes/DG1_MainMenuMasterPalette", "/Scenes/DG1_MainMenuMasterPalette", "/Scenes/DG1_MainMenuMasterPalette", "/Scenes/DG1_MainMenuMasterPalette", "/fonts/Burbank.fnt", "/UI/HelpMenus/puffleShoot", "/bg/Microgames/MicrogameTS", "/fonts/Burbank.fnt", "/bg/Microgames/MicrogameTS", "/UI/HelpMenus/herbertTrackingStylus", "/bg/HelpMenus/herbertTrackerTap_bottom", "/fonts/Burbank.fnt", "/bg/Microgames/MicrogameTS", "/bg/HelpMenus/balance01_bottom", "/bg/HelpMenus/balance02_bottom", "/UI/HelpMenus/balanceHighlight_01", "/UI/HelpMenus/balanceHighlight_02", "/fonts/Burbank.fnt", "/bg/Microgames/MicrogameTS", "/bg/Jackhammer/JackInstructions_02", "/bg/Jackhammer/JackInstructions_03", "/bg/Jackhammer/JackInstructions_01", "/fonts/Burbank.fnt", "/bg/Microgames/MicrogameTS", "/bg/AquaGrabber/AquaInstructions_01", "/bg/AquaGrabber/AquaInstructions_03", "/bg/AquaGrabber/AquaInstructions_04", "/bg/AquaGrabber/AquaInstructions_02", "/fonts/Burbank.fnt", "/bg/Microgames/MicrogameTS", "/bg/Labyrinth/LabyrinthInstructions_03", "/bg/Labyrinth/LabyrinthInstructions_02", "/bg/Labyrinth/LabyrinthInstructions_01", "/fonts/Burbank.fnt", "/bg/Microgames/MicrogameTS", "/bg/Grapple/GrappleInstructions_01", "/bg/Grapple/GrappleInstructions_03", "/bg/Grapple/GrappleInstructions_04", "/bg/Grapple/GrappleInstructions_02", "/fonts/bigUI.fnt", "/bg/Cutscenes/FPMOutro_top", "/BG/Cutscenes/BlankBlack.bmp", "/fonts/Burbank.fnt", "/UI/HelpMenus/spyGadget02", "/UI/HelpMenus/spyGadget03", "/UI/HelpMenus/spyGadget01", "/bg/Microgames/MicrogameTS", "/UI/HelpMenus/spyGadgetCover", "/bg/HelpMenus/spyGadget02_bottom", "/bg/HelpMenus/spyGadget03_bottom", "/bg/HelpMenus/spyGadget01_bottom", "/fonts/Burbank.fnt", "/NPC/M2/Herbert/phone", "/bg/Cutscenes/M8_Rink", "/BG/SpyPod/SpyPod_bottom", "/CutsceneAnims/M8/HerbIce", "/BG/SpyPod/comunicator_top", "/CutsceneAnims/M8/RookieTalk", "/CutsceneAnims/M8/RookiePeek", "/UI/SpyPod/Communicator/Rookie", "/fonts/Burbank.fnt", "/NPC/M2/Herbert/phone", "/bg/Cutscenes/M8_Rink", "/BG/SpyPod/SpyPod_bottom", "/BG/SpyPod/comunicator_top", "/CutsceneAnims/M8/Herbleave", "/CutsceneAnims/M8/RookieTalk", "/UI/SpyPod/Communicator/Rookie", "/flc/M11/warp.flc", "/bg/Cutscenes/FPMOutro_top", "/CutsceneAnims/SnakePiece", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/SnakePieceBG", "/CutsceneAnims/SnakePiece", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/SnakePieceBG", "/CutsceneAnims/SnakePiece2", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/SnakePieceBG", "/CutsceneAnims/SnakePiece3", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/SnakePieceBG", "/CutsceneAnims/SnakePiece4", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/SnakePieceBG", "/CutsceneAnims/SnakePiece5", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/SnakePieceBG", "/CutsceneAnims/SnakePiece6", "/bg/Cutscenes/FPMOutro_top", "/bg/Cutscenes/SnakePieceBG", "/bg/Cutscenes/C4_Sky", "/flc/C4/CS5_HerbBlown.flc", "/bg/Cutscenes/FPMOutro_top", "/CutsceneAnims/C4/klutzyClimb", "/char/ybBgStep11.ncl.l", "/char/ybBgOption.ncl.l", "/char/ybBgOption1.ncl.l", "/char/ybBgStep31.ncl.l", "/char/xb4ApListBack.nsc.l", "/char/ybBgStep2.ncl.l", "/char/ybBgStep21.ncl.l", "/char/jb3ListBack.nsc.l", "/char/ybBgStep11.ncl.l", "/char/ybBgStep21.ncl.l", "/nas.nintendowifi.net/ac", "/nas.test.nintendowifi.net/ac", "/nas.dev.nintendowifi.net/ac", "/nas.nintendowifi.net/ac", "/nas.nintendowifi.net/ac", "/nas.nintendowifi.net/ac", "/bg/CreditsBottom.nbfc", "/bg/CreditsBottom.nbfs", "/bg/CreditsBottom.nbfp", "/bg/CreditsTop.nbfc", "/bg/CreditsTop.nbfs", "/bg/CreditsTop.nbfp", "/bg/HerbsCamp.nbfc", "/bg/HerbsCamp.nbfs", "/bg/HerbsCamp.nbfp", "/bg/MissionHUD_BG.nbfc", "/bg/MissionHUD_BG.nbfs", "/bg/MissionHUD_BG.nbfp", "/bg/colors.nbfc", "/bg/colors.nbfs", "/bg/colors.nbfp", "/bg/minigameMissions_top.nbfc", "/bg/minigameMissions_top.nbfs", "/bg/minigameMissions_top.nbfp", "/bg/AquaGrabber/AquaGrabberMissions_BS.nbfc", "/bg/AquaGrabber/AquaGrabberMissions_BS.nbfs", "/bg/AquaGrabber/AquaGrabberMissions_BS.nbfp", "/bg/AquaGrabber/AquaGrabber_BS.nbfc", "/bg/AquaGrabber/AquaGrabber_BS.nbfs", "/bg/AquaGrabber/AquaGrabber_BS.nbfp", "/bg/AquaGrabber/AquaGrabber_TS.nbfc", "/bg/AquaGrabber/AquaGrabber_TS.nbfs", "/bg/AquaGrabber/AquaGrabber_TS.nbfp", "/bg/AquaGrabber/AquaInstructions_01.nbfc", "/bg/AquaGrabber/AquaInstructions_01.nbfs", "/bg/AquaGrabber/AquaInstructions_01.nbfp", "/bg/AquaGrabber/AquaInstructions_02.nbfc", "/bg/AquaGrabber/AquaInstructions_02.nbfs", "/bg/AquaGrabber/AquaInstructions_02.nbfp", "/bg/AquaGrabber/AquaInstructions_03.nbfc", "/bg/AquaGrabber/AquaInstructions_03.nbfs", "/bg/AquaGrabber/AquaInstructions_03.nbfp", "/bg/AquaGrabber/AquaInstructions_04.nbfc", "/bg/AquaGrabber/AquaInstructions_04.nbfs", "/bg/AquaGrabber/AquaInstructions_04.nbfp", "/bg/CommandCoach/CommandCoachTitleScreen.nbfc", "/bg/CommandCoach/CommandCoachTitleScreen.nbfs", "/bg/CommandCoach/CommandCoachTitleScreen.nbfp", "/bg/CommandCoach/CommandCoachTitleScreen_FR.nbfc", "/bg/CommandCoach/CommandCoachTitleScreen_FR.nbfs", "/bg/CommandCoach/CommandCoachTitleScreen_FR.nbfp", "/bg/CommandCoach/Instructions/CommandCoach_1.nbfc", "/bg/CommandCoach/Instructions/CommandCoach_1.nbfs", "/bg/CommandCoach/Instructions/CommandCoach_1.nbfp", "/bg/CommandCoach/Instructions/CommandCoach_2.nbfc", "/bg/CommandCoach/Instructions/CommandCoach_2.nbfs", "/bg/CommandCoach/Instructions/CommandCoach_2.nbfp", "/bg/CommandCoach/Instructions/CommandCoach_3.nbfc", "/bg/CommandCoach/Instructions/CommandCoach_3.nbfs", "/bg/CommandCoach/Instructions/CommandCoach_3.nbfp", "/bg/CommandCoach/Instructions/CommandCoach_4.nbfc", "/bg/CommandCoach/Instructions/CommandCoach_4.nbfs", "/bg/CommandCoach/Instructions/CommandCoach_4.nbfp", "/bg/CommandCoach/Instructions/CommandCoach_5.nbfc", "/bg/CommandCoach/Instructions/CommandCoach_5.nbfs", "/bg/CommandCoach/Instructions/CommandCoach_5.nbfp", "/bg/CommandCoach/Instructions/CommandCoach_6.nbfc", "/bg/CommandCoach/Instructions/CommandCoach_6.nbfs", "/bg/CommandCoach/Instructions/CommandCoach_6.nbfp", "/bg/Cutscenes/C2_DoorOpen.nbfc", "/bg/Cutscenes/C2_DoorOpen.nbfs", "/bg/Cutscenes/C2_DoorOpen.nbfp", "/bg/Cutscenes/C3_Melt.nbfc", "/bg/Cutscenes/C3_Melt.nbfs", "/bg/Cutscenes/C3_Melt.nbfp", "/bg/Cutscenes/C4_Sky.nbfc", "/bg/Cutscenes/C4_Sky.nbfs", "/bg/Cutscenes/C4_Sky.nbfp", "/bg/Cutscenes/CPLogo.nbfc", "/bg/Cutscenes/CPLogo.nbfs", "/bg/Cutscenes/CPLogo.nbfp", "/bg/Cutscenes/CPLogo_FR.nbfc", "/bg/Cutscenes/CPLogo_FR.nbfs", "/bg/Cutscenes/CPLogo_FR.nbfp", "/bg/Cutscenes/CPLogo_SP.nbfc", "/bg/Cutscenes/CPLogo_SP.nbfs", "/bg/Cutscenes/CPLogo_SP.nbfp", "/bg/Cutscenes/CutsceneText_top_Penguin.nbfc", "/bg/Cutscenes/CutsceneText_top_Penguin.nbfs", "/bg/Cutscenes/CutsceneText_top_Penguin.nbfp", "/bg/Cutscenes/FPMOutro_top.nbfc", "/bg/Cutscenes/FPMOutro_top.nbfs", "/bg/Cutscenes/FPMOutro_top.nbfp", "/bg/Cutscenes/HerbertsBaseLobby.nbfc", "/bg/Cutscenes/HerbertsBaseLobby.nbfs", "/bg/Cutscenes/HerbertsBaseLobby.nbfp", "/bg/Cutscenes/HerbertsBaseWorkshop.nbfc", "/bg/Cutscenes/HerbertsBaseWorkshop.nbfs", "/bg/Cutscenes/HerbertsBaseWorkshop.nbfp", "/bg/Cutscenes/M10_FillGeyser.nbfc", "/bg/Cutscenes/M10_FillGeyser.nbfs", "/bg/Cutscenes/M10_FillGeyser.nbfp", "/bg/Cutscenes/M11Vid_GiftOffice.nbfc", "/bg/Cutscenes/M11Vid_GiftOffice.nbfs", "/bg/Cutscenes/M11Vid_GiftOffice.nbfp", "/bg/Cutscenes/M11Vid_GiftShop.nbfc", "/bg/Cutscenes/M11Vid_GiftShop.nbfs", "/bg/Cutscenes/M11Vid_GiftShop.nbfp", "/bg/Cutscenes/M11VideoMachineOpen.nbfc", "/bg/Cutscenes/M11VideoMachineOpen.nbfs", "/bg/Cutscenes/M11VideoMachineOpen.nbfp", "/bg/Cutscenes/M8_Rink.nbfc", "/bg/Cutscenes/M8_Rink.nbfs", "/bg/Cutscenes/M8_Rink.nbfp", "/bg/Cutscenes/M9_Video.nbfc", "/bg/Cutscenes/M9_Video.nbfs", "/bg/Cutscenes/M9_Video.nbfp", "/bg/Cutscenes/SnakePieceBG.nbfc", "/bg/Cutscenes/SnakePieceBG.nbfs", "/bg/Cutscenes/SnakePieceBG.nbfp", "/bg/Cutscenes/C2Camera/Bench.nbfc", "/bg/Cutscenes/C2Camera/Bench.nbfs", "/bg/Cutscenes/C2Camera/Bench.nbfp", "/bg/Cutscenes/C2Camera/Gary.nbfc", "/bg/Cutscenes/C2Camera/Gary.nbfs", "/bg/Cutscenes/C2Camera/Gary.nbfp", "/bg/Cutscenes/C2Camera/Palm.nbfc", "/bg/Cutscenes/C2Camera/Palm.nbfs", "/bg/Cutscenes/C2Camera/Palm.nbfp", "/bg/Cutscenes/C2Camera/Supply.nbfc", "/bg/Cutscenes/C2Camera/Supply.nbfs", "/bg/Cutscenes/C2Camera/Supply.nbfp", "/bg/Grapple/GrappleInstructions_01.nbfc", "/bg/Grapple/GrappleInstructions_01.nbfs", "/bg/Grapple/GrappleInstructions_01.nbfp", "/bg/Grapple/GrappleInstructions_02.nbfc", "/bg/Grapple/GrappleInstructions_02.nbfs", "/bg/Grapple/GrappleInstructions_02.nbfp", "/bg/Grapple/GrappleInstructions_03.nbfc", "/bg/Grapple/GrappleInstructions_03.nbfs", "/bg/Grapple/GrappleInstructions_03.nbfp", "/bg/Grapple/GrappleInstructions_04.nbfc", "/bg/Grapple/GrappleInstructions_04.nbfs", "/bg/Grapple/GrappleInstructions_04.nbfp", "/bg/Grapple/TitleScreen.nbfc", "/bg/Grapple/TitleScreen.nbfs", "/bg/Grapple/TitleScreen.nbfp", "/bg/Grapple/TitleScreenBottom.nbfc", "/bg/Grapple/TitleScreenBottom.nbfs", "/bg/Grapple/TitleScreenBottom.nbfp", "/bg/Grapple/TitleScreenBottomMissions.nbfc", "/bg/Grapple/TitleScreenBottomMissions.nbfs", "/bg/Grapple/TitleScreenBottomMissions.nbfp", "/bg/HelpMenus/InventoryCombine_bottom.nbfc", "/bg/HelpMenus/InventoryCombine_bottom.nbfs", "/bg/HelpMenus/InventoryCombine_bottom.nbfp", "/bg/HelpMenus/StylusDrag_bottom.nbfc", "/bg/HelpMenus/StylusDrag_bottom.nbfs", "/bg/HelpMenus/StylusDrag_bottom.nbfp", "/bg/HelpMenus/balance01_bottom.nbfc", "/bg/HelpMenus/balance01_bottom.nbfs", "/bg/HelpMenus/balance01_bottom.nbfp", "/bg/HelpMenus/balance02_bottom.nbfc", "/bg/HelpMenus/balance02_bottom.nbfs", "/bg/HelpMenus/balance02_bottom.nbfp", "/bg/HelpMenus/herbertTrackerTap_bottom.nbfc", "/bg/HelpMenus/herbertTrackerTap_bottom.nbfs", "/bg/HelpMenus/herbertTrackerTap_bottom.nbfp", "/bg/HelpMenus/spyGadget01_bottom.nbfc", "/bg/HelpMenus/spyGadget01_bottom.nbfs", "/bg/HelpMenus/spyGadget01_bottom.nbfp", "/bg/HelpMenus/spyGadget02_bottom.nbfc", "/bg/HelpMenus/spyGadget02_bottom.nbfs", "/bg/HelpMenus/spyGadget02_bottom.nbfp", "/bg/HelpMenus/spyGadget03_bottom.nbfc", "/bg/HelpMenus/spyGadget03_bottom.nbfs", "/bg/HelpMenus/spyGadget03_bottom.nbfp", "/bg/Labyrinth/B1_bonus_small.nbfc", "/bg/Labyrinth/B1_bonus_small.nbfs", "/bg/Labyrinth/B1_bonus_small.nbfp", "/bg/Labyrinth/B2_bonus_small.nbfc", "/bg/Labyrinth/B2_bonus_small.nbfs", "/bg/Labyrinth/B2_bonus_small.nbfp", "/bg/Labyrinth/B3_bonus_small.nbfc", "/bg/Labyrinth/B3_bonus_small.nbfs", "/bg/Labyrinth/B3_bonus_small.nbfp", "/bg/Labyrinth/C3_cave_small.nbfc", "/bg/Labyrinth/C3_cave_small.nbfs", "/bg/Labyrinth/C3_cave_small.nbfp", "/bg/Labyrinth/LabyrinthInstructions_01.nbfc", "/bg/Labyrinth/LabyrinthInstructions_01.nbfs", "/bg/Labyrinth/LabyrinthInstructions_01.nbfp", "/bg/Labyrinth/LabyrinthInstructions_02.nbfc", "/bg/Labyrinth/LabyrinthInstructions_02.nbfs", "/bg/Labyrinth/LabyrinthInstructions_02.nbfp", "/bg/Labyrinth/LabyrinthInstructions_03.nbfc", "/bg/Labyrinth/LabyrinthInstructions_03.nbfs", "/bg/Labyrinth/LabyrinthInstructions_03.nbfp", "/bg/Labyrinth/M11_cornMaze1_small.nbfc", "/bg/Labyrinth/M11_cornMaze1_small.nbfs", "/bg/Labyrinth/M11_cornMaze1_small.nbfp", "/bg/Labyrinth/M11_cornMaze2_small.nbfc", "/bg/Labyrinth/M11_cornMaze2_small.nbfs", "/bg/Labyrinth/M11_cornMaze2_small.nbfp", "/bg/Labyrinth/M8_caves_small.nbfc", "/bg/Labyrinth/M8_caves_small.nbfs", "/bg/Labyrinth/M8_caves_small.nbfp", "/bg/Labyrinth/M9_frozenLake_small.nbfc", "/bg/Labyrinth/M9_frozenLake_small.nbfs", "/bg/Labyrinth/M9_frozenLake_small.nbfp", "/bg/Labyrinth/titleScreenMissions_bottom.nbfc", "/bg/Labyrinth/titleScreenMissions_bottom.nbfs", "/bg/Labyrinth/titleScreenMissions_bottom.nbfp", "/bg/Labyrinth/titleScreen_bottom.nbfc", "/bg/Labyrinth/titleScreen_bottom.nbfs", "/bg/Labyrinth/titleScreen_bottom.nbfp", "/bg/Labyrinth/titleScreen_top.nbfc", "/bg/Labyrinth/titleScreen_top.nbfs", "/bg/Labyrinth/titleScreen_top.nbfp", "/bg/LevelSelect/Map_top.nbfc", "/bg/LevelSelect/Map_top.nbfs", "/bg/LevelSelect/Map_top.nbfp", "/bg/Menus/MultiplayerMenuTop.nbfc", "/bg/Menus/MultiplayerMenuTop.nbfs", "/bg/Menus/MultiplayerMenuTop.nbfp", "/bg/Microgames/ColorLock.nbfc", "/bg/Microgames/ColorLock.nbfs", "/bg/Microgames/ColorLock.nbfp", "/bg/Microgames/MicrogameTS.nbfc", "/bg/Microgames/MicrogameTS.nbfs", "/bg/Microgames/MicrogameTS.nbfp", "/bg/Microgames/RopePullBG.nbfc", "/bg/Microgames/RopePullBG.nbfs", "/bg/Microgames/RopePullBG.nbfp", "/bg/Microgames/Balance/Forest.nbfc", "/bg/Microgames/Balance/Forest.nbfs", "/bg/Microgames/Balance/Forest.nbfp", "/bg/Microgames/Balance/Plaza.nbfc", "/bg/Microgames/Balance/Plaza.nbfs", "/bg/Microgames/Balance/Plaza.nbfp", "/bg/Microgames/Balance/SnowForts.nbfc", "/bg/Microgames/Balance/SnowForts.nbfs", "/bg/Microgames/Balance/SnowForts.nbfp", "/bg/Microgames/Balance/topScreen_BG.nbfc", "/bg/Microgames/Balance/topScreen_BG.nbfs", "/bg/Microgames/Balance/topScreen_BG.nbfp", "/bg/Microgames/BarBending/BarBending.nbfc", "/bg/Microgames/BarBending/BarBending.nbfs", "/bg/Microgames/BarBending/BarBending.nbfp", "/bg/Microgames/Beaker/BeakersBG.nbfc", "/bg/Microgames/Beaker/BeakersBG.nbfs", "/bg/Microgames/Beaker/BeakersBG.nbfp", "/bg/Microgames/DVDCleaning/DVDCleaningBG.nbfc", "/bg/Microgames/DVDCleaning/DVDCleaningBG.nbfs", "/bg/Microgames/DVDCleaning/DVDCleaningBG.nbfp", "/bg/Microgames/GearShift/gearShiftBG.nbfc", "/bg/Microgames/GearShift/gearShiftBG.nbfs", "/bg/Microgames/GearShift/gearShiftBG.nbfp", "/bg/Microgames/Gears/Gears_BG.nbfc", "/bg/Microgames/Gears/Gears_BG.nbfs", "/bg/Microgames/Gears/Gears_BG.nbfp", "/bg/Microgames/HB_TRAC/HTDot_BS.nbfc", "/bg/Microgames/HB_TRAC/HTDot_BS.nbfs", "/bg/Microgames/HB_TRAC/HTDot_BS.nbfp", "/bg/Microgames/HB_TRAC/HTDot_TS.nbfc", "/bg/Microgames/HB_TRAC/HTDot_TS.nbfs", "/bg/Microgames/HB_TRAC/HTDot_TS.nbfp", "/bg/Microgames/HB_TRAC/HTJPG_BS.nbfc", "/bg/Microgames/HB_TRAC/HTJPG_BS.nbfs", "/bg/Microgames/HB_TRAC/HTJPG_BS.nbfp", "/bg/Microgames/HB_TRAC/HTJPG_TS.nbfc", "/bg/Microgames/HB_TRAC/HTJPG_TS.nbfs", "/bg/Microgames/HB_TRAC/HTJPG_TS.nbfp", "/bg/Microgames/HB_TRAC/HTRookie_BS.nbfc", "/bg/Microgames/HB_TRAC/HTRookie_BS.nbfs", "/bg/Microgames/HB_TRAC/HTRookie_BS.nbfp", "/bg/Microgames/HB_TRAC/HTRookie_TS.nbfc", "/bg/Microgames/HB_TRAC/HTRookie_TS.nbfs", "/bg/Microgames/HB_TRAC/HTRookie_TS.nbfp", "/bg/Microgames/HB_TRAC/HT_BS.nbfc", "/bg/Microgames/HB_TRAC/HT_BS.nbfs", "/bg/Microgames/HB_TRAC/HT_BS.nbfp", "/bg/Microgames/HB_TRAC/HT_TS.nbfc", "/bg/Microgames/HB_TRAC/HT_TS.nbfs", "/bg/Microgames/HB_TRAC/HT_TS.nbfp", "/bg/Microgames/InflateDuck/InflateDuck_BS.nbfc", "/bg/Microgames/InflateDuck/InflateDuck_BS.nbfs", "/bg/Microgames/InflateDuck/InflateDuck_BS.nbfp", "/bg/Microgames/JPG_Reel/reelBG_Bottom.nbfc", "/bg/Microgames/JPG_Reel/reelBG_Bottom.nbfs", "/bg/Microgames/JPG_Reel/reelBG_Bottom.nbfp", "/bg/Microgames/Jigsaw/Geyser_bottom.nbfc", "/bg/Microgames/Jigsaw/Geyser_bottom.nbfs", "/bg/Microgames/Jigsaw/Geyser_bottom.nbfp", "/bg/Microgames/Jigsaw/GridPiece_bottom.nbfc", "/bg/Microgames/Jigsaw/GridPiece_bottom.nbfs", "/bg/Microgames/Jigsaw/GridPiece_bottom.nbfp", "/bg/Microgames/Jigsaw/JackHammer_bottom.nbfc", "/bg/Microgames/Jigsaw/JackHammer_bottom.nbfs", "/bg/Microgames/Jigsaw/JackHammer_bottom.nbfp", "/bg/Microgames/Jigsaw/Jigsaw_bottom.nbfc", "/bg/Microgames/Jigsaw/Jigsaw_bottom.nbfs", "/bg/Microgames/Jigsaw/Jigsaw_bottom.nbfp", "/bg/Microgames/Jigsaw/KeyPad_bottom.nbfc", "/bg/Microgames/Jigsaw/KeyPad_bottom.nbfs", "/bg/Microgames/Jigsaw/KeyPad_bottom.nbfp", "/bg/Microgames/Jigsaw/Windmill_bottom.nbfc", "/bg/Microgames/Jigsaw/Windmill_bottom.nbfs", "/bg/Microgames/Jigsaw/Windmill_bottom.nbfp", "/bg/Microgames/Jigsaw/cookie_gameBG.nbfc", "/bg/Microgames/Jigsaw/cookie_gameBG.nbfs", "/bg/Microgames/Jigsaw/cookie_gameBG.nbfp", "/bg/Microgames/KlutzyMirror/KMTraining_TS.nbfc", "/bg/Microgames/KlutzyMirror/KMTraining_TS.nbfs", "/bg/Microgames/KlutzyMirror/KMTraining_TS.nbfp", "/bg/Microgames/KlutzyMirror/KM_TS.nbfc", "/bg/Microgames/KlutzyMirror/KM_TS.nbfs", "/bg/Microgames/KlutzyMirror/KM_TS.nbfp", "/bg/Microgames/MechanoDuster/MechandoDusterBG_MineShack.nbfc", "/bg/Microgames/MechanoDuster/MechandoDusterBG_MineShack.nbfs", "/bg/Microgames/MechanoDuster/MechandoDusterBG_MineShack.nbfp", "/bg/Microgames/MechanoDuster/MechanoDusterBG_Beard.nbfc", "/bg/Microgames/MechanoDuster/MechanoDusterBG_Beard.nbfs", "/bg/Microgames/MechanoDuster/MechanoDusterBG_Beard.nbfp", "/bg/Microgames/MechanoDuster/MechanoDusterBG_Cave.nbfc", "/bg/Microgames/MechanoDuster/MechanoDusterBG_Cave.nbfs", "/bg/Microgames/MechanoDuster/MechanoDusterBG_Cave.nbfp", "/bg/Microgames/MechanoDuster/MechanoDusterBG_CoffeeShop.nbfc", "/bg/Microgames/MechanoDuster/MechanoDusterBG_CoffeeShop.nbfs", "/bg/Microgames/MechanoDuster/MechanoDusterBG_CoffeeShop.nbfp", "/bg/Microgames/MechanoDuster/MechanoDusterBG_PizzaShop.nbfc", "/bg/Microgames/MechanoDuster/MechanoDusterBG_PizzaShop.nbfs", "/bg/Microgames/MechanoDuster/MechanoDusterBG_PizzaShop.nbfp", "/bg/Microgames/MechanoDuster/MechanoDusterBG_Plaza.nbfc", "/bg/Microgames/MechanoDuster/MechanoDusterBG_Plaza.nbfs", "/bg/Microgames/MechanoDuster/MechanoDusterBG_Plaza.nbfp", "/bg/Microgames/MechanoDuster/MechanoDusterBG_Snow.nbfc", "/bg/Microgames/MechanoDuster/MechanoDusterBG_Snow.nbfs", "/bg/Microgames/MechanoDuster/MechanoDusterBG_Snow.nbfp", "/bg/Microgames/PipeDreams/PD_BG.nbfc", "/bg/Microgames/PipeDreams/PD_BG.nbfs", "/bg/Microgames/PipeDreams/PD_BG.nbfp", "/bg/Microgames/Reflect/R_BS.nbfc", "/bg/Microgames/Reflect/R_BS.nbfs", "/bg/Microgames/Reflect/R_BS.nbfp", "/bg/Microgames/Reflect/R_TS.nbfc", "/bg/Microgames/Reflect/R_TS.nbfs", "/bg/Microgames/Reflect/R_TS.nbfp", "/bg/Microgames/ShakeBarrel/SB_BS.nbfc", "/bg/Microgames/ShakeBarrel/SB_BS.nbfs", "/bg/Microgames/ShakeBarrel/SB_BS.nbfp", "/bg/Microgames/SnakeGame/SnakeBG.nbfc", "/bg/Microgames/SnakeGame/SnakeBG.nbfs", "/bg/Microgames/SnakeGame/SnakeBG.nbfp", "/bg/Microgames/SnakeGame/SnakeTS.nbfc", "/bg/Microgames/SnakeGame/SnakeTS.nbfs", "/bg/Microgames/SnakeGame/SnakeTS.nbfp", "/bg/Microgames/SolarPanel/SP_BS.nbfc", "/bg/Microgames/SolarPanel/SP_BS.nbfs", "/bg/Microgames/SolarPanel/SP_BS.nbfp", "/bg/Microgames/Trough/TroughBG.nbfc", "/bg/Microgames/Trough/TroughBG.nbfs", "/bg/Microgames/Trough/TroughBG.nbfp", "/bg/Microgames/WallPuzzle/wallpuzzleBG.nbfc", "/bg/Microgames/WallPuzzle/wallpuzzleBG.nbfs", "/bg/Microgames/WallPuzzle/wallpuzzleBG.nbfp", "/bg/Microgames/Welding/freezeBG.nbfc", "/bg/Microgames/Welding/freezeBG.nbfs", "/bg/Microgames/Welding/freezeBG.nbfp", "/bg/Microgames/Welding/weldingBG.nbfc", "/bg/Microgames/Welding/weldingBG.nbfs", "/bg/Microgames/Welding/weldingBG.nbfp", "/bg/Microgames/Wrench/C1_jackHammer.nbfc", "/bg/Microgames/Wrench/C1_jackHammer.nbfs", "/bg/Microgames/Wrench/C1_jackHammer.nbfp", "/bg/Microgames/Wrench/C3_puffleCage.nbfc", "/bg/Microgames/Wrench/C3_puffleCage.nbfs", "/bg/Microgames/Wrench/C3_puffleCage.nbfp", "/bg/Microgames/Wrench/M10_Gearbox.nbfc", "/bg/Microgames/Wrench/M10_Gearbox.nbfs", "/bg/Microgames/Wrench/M10_Gearbox.nbfp", "/bg/Microgames/Wrench/M11WrenchBG.nbfc", "/bg/Microgames/Wrench/M11WrenchBG.nbfs", "/bg/Microgames/Wrench/M11WrenchBG.nbfp", "/bg/Microgames/Wrench/M2_VRStation.nbfc", "/bg/Microgames/Wrench/M2_VRStation.nbfs", "/bg/Microgames/Wrench/M2_VRStation.nbfp", "/bg/Microgames/Wrench/M2_fuzebox.nbfc", "/bg/Microgames/Wrench/M2_fuzebox.nbfs", "/bg/Microgames/Wrench/M2_fuzebox.nbfp", "/bg/Microgames/Wrench/M2_window.nbfc", "/bg/Microgames/Wrench/M2_window.nbfs", "/bg/Microgames/Wrench/M2_window.nbfp", "/bg/Microgames/Wrench/M5_Coffee.nbfc", "/bg/Microgames/Wrench/M5_Coffee.nbfs", "/bg/Microgames/Wrench/M5_Coffee.nbfp", "/bg/Microgames/Wrench/M8_boilerWindow.nbfc", "/bg/Microgames/Wrench/M8_boilerWindow.nbfs", "/bg/Microgames/Wrench/M8_boilerWindow.nbfp", "/bg/MissionSelector/missionSelectBottom.nbfc", "/bg/MissionSelector/missionSelectBottom.nbfs", "/bg/MissionSelector/missionSelectBottom.nbfp", "/bg/MissionSelector/missionSelectBottomVR.nbfc", "/bg/MissionSelector/missionSelectBottomVR.nbfs", "/bg/MissionSelector/missionSelectBottomVR.nbfp", "/bg/MissionSelector/missionSelectTop_CH1.nbfc", "/bg/MissionSelector/missionSelectTop_CH1.nbfs", "/bg/MissionSelector/missionSelectTop_CH1.nbfp", "/bg/MissionSelector/missionSelectTop_CH2.nbfc", "/bg/MissionSelector/missionSelectTop_CH2.nbfs", "/bg/MissionSelector/missionSelectTop_CH2.nbfp", "/bg/MissionSelector/missionSelectTop_CH3.nbfc", "/bg/MissionSelector/missionSelectTop_CH3.nbfs", "/bg/MissionSelector/missionSelectTop_CH3.nbfp", "/bg/MissionSelector/missionSelectTop_CH4.nbfc", "/bg/MissionSelector/missionSelectTop_CH4.nbfs", "/bg/MissionSelector/missionSelectTop_CH4.nbfp", "/bg/MissionSelector/missionSelectTop_Classified.nbfc", "/bg/MissionSelector/missionSelectTop_Classified.nbfs", "/bg/MissionSelector/missionSelectTop_Classified.nbfp", "/bg/MissionSelector/missionSelectTop_Classified_FR.nbfc", "/bg/MissionSelector/missionSelectTop_Classified_FR.nbfs", "/bg/MissionSelector/missionSelectTop_Classified_FR.nbfp", "/bg/MissionSelector/missionSelectTop_Classified_SP.nbfc", "/bg/MissionSelector/missionSelectTop_Classified_SP.nbfs", "/bg/MissionSelector/missionSelectTop_Classified_SP.nbfp", "/bg/MissionSelector/missionSelectTop_M10.nbfc", "/bg/MissionSelector/missionSelectTop_M10.nbfs", "/bg/MissionSelector/missionSelectTop_M10.nbfp", "/bg/MissionSelector/missionSelectTop_M11.nbfc", "/bg/MissionSelector/missionSelectTop_M11.nbfs", "/bg/MissionSelector/missionSelectTop_M11.nbfp", "/bg/MissionSelector/missionSelectTop_M5.nbfc", "/bg/MissionSelector/missionSelectTop_M5.nbfs", "/bg/MissionSelector/missionSelectTop_M5.nbfp", "/bg/MissionSelector/missionSelectTop_M6.nbfc", "/bg/MissionSelector/missionSelectTop_M6.nbfs", "/bg/MissionSelector/missionSelectTop_M6.nbfp", "/bg/MissionSelector/missionSelectTop_M8.nbfc", "/bg/MissionSelector/missionSelectTop_M8.nbfs", "/bg/MissionSelector/missionSelectTop_M8.nbfp", "/bg/MissionSelector/missionSelectTop_M9.nbfc", "/bg/MissionSelector/missionSelectTop_M9.nbfs", "/bg/MissionSelector/missionSelectTop_M9.nbfp", "/bg/SpyLog/Spylog_bottom.nbfc", "/bg/SpyLog/Spylog_bottom.nbfs", "/bg/SpyLog/Spylog_bottom.nbfp", "/bg/SpyLog/Spylog_frame.nbfc", "/bg/SpyLog/Spylog_frame.nbfs", "/bg/SpyLog/Spylog_frame.nbfp", "/bg/SpyLog/Spylog_top.nbfc", "/bg/SpyLog/Spylog_top.nbfs", "/bg/SpyLog/Spylog_top.nbfp", "/bg/SpyPod/SpyPod_bottom.nbfc", "/bg/SpyPod/SpyPod_bottom.nbfs", "/bg/SpyPod/SpyPod_bottom.nbfp", "/bg/SpyPod/comunicator_bottom.nbfc", "/bg/SpyPod/comunicator_bottom.nbfs", "/bg/SpyPod/comunicator_bottom.nbfp", "/bg/SpyPod/comunicator_top.nbfc", "/bg/SpyPod/comunicator_top.nbfs", "/bg/SpyPod/comunicator_top.nbfp", "/bg/SpyPod/Decoder/Decoder_bottom.nbfc", "/bg/SpyPod/Decoder/Decoder_bottom.nbfs", "/bg/SpyPod/Decoder/Decoder_bottom.nbfp", "/bg/SpyPod/Decoder/Decoder_top.nbfc", "/bg/SpyPod/Decoder/Decoder_top.nbfs", "/bg/SpyPod/Decoder/Decoder_top.nbfp", "/bg/WifiContent/DGamerTop.nbfc", "/bg/WifiContent/DGamerTop.nbfs", "/bg/WifiContent/DGamerTop.nbfp", "/bg/WifiContent/DGamer_bottomscreen.nbfc", "/bg/WifiContent/DGamer_bottomscreen.nbfs", "/bg/WifiContent/DGamer_bottomscreen.nbfp", "/bg/WifiContent/DGamer_topscreen.nbfc", "/bg/WifiContent/DGamer_topscreen.nbfs", "/bg/WifiContent/DGamer_topscreen.nbfp", "/bg/WifiContent/Wifi_Login_top.nbfc", "/bg/WifiContent/Wifi_Login_top.nbfs", "/bg/WifiContent/Wifi_Login_top.nbfp", "/bg/WifiContent/CoinUpload/CoinUpload_bottom.nbfc", "/bg/WifiContent/CoinUpload/CoinUpload_bottom.nbfs", "/bg/WifiContent/CoinUpload/CoinUpload_bottom.nbfp", "/flc/1PLogotop.flc", "/flc/GogglesOff.flc", "/flc/GogglesOn.flc", "/flc/Intro.flc", "/flc/Intro3.flc", "/flc/Intro4.flc", "/flc/IntroDot.flc", "/flc/IntroG.flc", "/flc/IntroG2.flc", "/flc/IntroJPG.flc", "/flc/IntroRookie.flc", "/flc/Lockdown.flc", "/flc/Outro.flc", "/flc/OutroII.flc", "/flc/C1/CombineItems.flc", "/flc/C1/NPaper.flc", "/flc/C1/NPaper_FR.flc", "/flc/C1/NPaper_SP.flc", "/flc/C3/AgentCapture.flc", "/flc/C4/CS1_HerbCrash.flc", "/flc/C4/CS2_IslandSink1.flc", "/flc/C4/CS3_IslandRaise1.flc", "/flc/C4/CS4_IslandRaise2.flc", "/flc/C4/CS5_HerbBlown.flc", "/flc/M11/explodePopcorn.flc", "/flc/M11/warp.flc", "/flc/MysteriousTremors/HerbertDock1.flc", "/flc/MysteriousTremors/HerbertDock2.flc", "/flc/MysteriousTremors/HerbertDock3.flc", "/flc/MysteriousTremors/TownGift1.flc", "/flc/MysteriousTremors/TownGift2.flc", "/flc/MysteriousTremors/TownGift3.flc", "/flc/MysteriousTremors/TownGift4.flc", "/flc/MysteriousTremors/TownGift5.flc", "/flc/MysteriousTremors/TownGiftFail1.flc", "/flc/MysteriousTremors/TownGiftFail2.flc", "/flc/MysteriousTremors/TownGiftFail3.flc", "/flc/MysteriousTremors/UGGift1.flc", "/flc/MysteriousTremors/UGGift2.flc", "/flc/MysteriousTremors/UGGift3.flc", "/flc/MysteriousTremors/UGGift4.flc", "/flc/MysteriousTremors/UGGiftFail1.flc", "/flc/MysteriousTremors/UGGiftFail2.flc", "/flc/MysteriousTremors/UGGiftFail3.flc", "/flc/QuestionsCrab/CageDrop.flc", "/flc/QuestionsCrab/CageLift.flc", "/flc/QuestionsCrab/FallCliff.flc", "/flc/QuestionsCrab/GHookToss.flc", "/flc/QuestionsCrab/HerbertBackfire.flc", "/flc/QuestionsCrab/KlutzyEscapes.flc", "/flc/QuestionsCrab/KlutzySkiLift.flc", "/flc/QuestionsCrab/SuperFlare.flc", "/flc/QuestionsCrab/Woodchopper.flc", "/flc/SecretFur/CaptureKlutzy.flc", "/flc/WaddleSquad/NC_cageDrop.flc", "/flc/WaddleSquad/NC_trapped.flc", "/flc/WaddleSquad/NC_trappedJP.flc", "/flc/WaddleSquad/NCwallBreak.flc", "/flc/WaddleSquad/beach_blastOff.flc", "/levels/AquaGrabber0_map_0.mpb", "/levels/AquaGrabber0_map_1.mpb", "/levels/AquaGrabber0_map_2.mpb", "/levels/AquaGrabber1_map_0.mpb", "/levels/AquaGrabber1_map_1.mpb", "/levels/AquaGrabber1_map_2.mpb", "/levels/AquaGrabber2_map_0.mpb", "/levels/AquaGrabber2_map_1.mpb", "/levels/AquaGrabber2_map_2.mpb", "/levels/AquaGrabber3_map_0.mpb", "/levels/AquaGrabber3_map_1.mpb", "/levels/AquaGrabber3_map_2.mpb", "/levels/Attic0_map_0.mpb", "/levels/B1_bonus_map_0.mpb", "/levels/B2_bonus_map_0.mpb", "/levels/B3_bonus_map_0.mpb", "/levels/Beach0_map_0.mpb", "/levels/Beach0_map_1.mpb", "/levels/Beacon0_map_0.mpb", "/levels/BoilerRoom0_map_0.mpb", "/levels/BoilerRoomTremors0_map_0.mpb", "/levels/BookRoom0_map_0.mpb", "/levels/C3_cave_map_0.mpb", "/levels/C3_cave_map_1.mpb", "/levels/French/CaveInteriorHerb0_map_0.mpb", "/levels/Spanish/CaveInteriorHerb0_map_0.mpb", "/levels/CaveInteriorHerb0_map_0.mpb", "/levels/French/CaveInteriorHerbEmpty_map_0.mpb", "/levels/Spanish/CaveInteriorHerbEmpty_map_0.mpb", "/levels/CaveInteriorHerbEmpty_map_0.mpb", "/levels/French/CaveInteriorHerbOpen0_map_0.mpb", "/levels/Spanish/CaveInteriorHerbOpen0_map_0.mpb", "/levels/CaveInteriorHerbOpen0_map_0.mpb", "/levels/French/CoffeeMachine0_map_0.mpb", "/levels/Spanish/CoffeeMachine0_map_0.mpb", "/levels/CoffeeMachine0_map_0.mpb", "/levels/French/CoffeeShop0_map_0.mpb", "/levels/Spanish/CoffeeShop0_map_0.mpb", "/levels/CoffeeShop0_map_0.mpb", "/levels/French/CoffeeShopCreature0_map_0.mpb", "/levels/Spanish/CoffeeShopCreature0_map_0.mpb", "/levels/CoffeeShopCreature0_map_0.mpb", "/levels/French/CoffeeShopTremors0_map_0.mpb", "/levels/Spanish/CoffeeShopTremors0_map_0.mpb", "/levels/CoffeeShopTremors0_map_0.mpb", "/levels/French/CommandRoom0_map_0.mpb", "/levels/Spanish/CommandRoom0_map_0.mpb", "/levels/CommandRoom0_map_0.mpb", "/levels/CornMaze0_map_0.mpb", "/levels/CornMaze0_map_1.mpb", "/levels/CornMaze1_map_0.mpb", "/levels/CornMaze1_map_1.mpb", "/levels/CornMaze2_map_0.mpb", "/levels/CornMaze2_map_1.mpb", "/levels/CornMaze3_map_0.mpb", "/levels/CornMaze3_map_1.mpb", "/levels/CornMazeBegin0_map_0.mpb", "/levels/CornMazeBegin0_map_1.mpb", "/levels/CornMazeSecret_map_0.mpb", "/levels/CornMazeSecret_map_1.mpb", "/levels/Dock0_map_0.mpb", "/levels/Dock0_map_1.mpb", "/levels/Dojo0_map_0.mpb", "/levels/DVDMachine0_map_0.mpb", "/levels/French/Fishing0_map_0.mpb", "/levels/Spanish/Fishing0_map_0.mpb", "/levels/Fishing0_map_0.mpb", "/levels/Fishing0_map_1.mpb", "/levels/Forest0_map_0.mpb", "/levels/Forest0_map_1.mpb", "/levels/French/FurAnalyzer0_map_0.mpb", "/levels/Spanish/FurAnalyzer0_map_0.mpb", "/levels/FurAnalyzer0_map_0.mpb", "/levels/French/GadgetRoom0_map_0.mpb", "/levels/Spanish/GadgetRoom0_map_0.mpb", "/levels/GadgetRoom0_map_0.mpb", "/levels/French/GadgetRoomCrabMach_map_0.mpb", "/levels/Spanish/GadgetRoomCrabMach_map_0.mpb", "/levels/GadgetRoomCrabMach_map_0.mpb", "/levels/French/GarysRoom0_map_0.mpb", "/levels/Spanish/GarysRoom0_map_0.mpb", "/levels/GarysRoom0_map_0.mpb", "/levels/French/GiftOffice0_map_0.mpb", "/levels/Spanish/GiftOffice0_map_0.mpb", "/levels/GiftOffice0_map_0.mpb", "/levels/French/GiftOfficeTremors0_map_0.mpb", "/levels/Spanish/GiftOfficeTremors0_map_0.mpb", "/levels/GiftOfficeTremors0_map_0.mpb", "/levels/GiftRoof0_map_0.mpb", "/levels/GiftShop_Surveillance_map_0.mpb", "/levels/French/GiftShop0_map_0.mpb", "/levels/Spanish/GiftShop0_map_0.mpb", "/levels/GiftShop0_map_0.mpb", "/levels/French/GiftShop1_map_0.mpb", "/levels/Spanish/GiftShop1_map_0.mpb", "/levels/GiftShop1_map_0.mpb", "/levels/French/GiftShopTremors0_map_0.mpb", "/levels/Spanish/GiftShopTremors0_map_0.mpb", "/levels/GiftShopTremors0_map_0.mpb", "/levels/GrapplingBonus1_map_0.mpb", "/levels/GrapplingBonus1_map_1.mpb", "/levels/GrapplingBonus2_map_0.mpb", "/levels/GrapplingBonus2_map_1.mpb", "/levels/GrapplingBonus3_map_0.mpb", "/levels/GrapplingBonus3_map_1.mpb", "/levels/GrapplingHookC2HerbBase_map_0.mpb", "/levels/GrapplingHookC2HerbBase_map_1.mpb", "/levels/GrapplingHookC3TallestMtn_map_0.mpb", "/levels/GrapplingHookC3TallestMtn_map_1.mpb", "/levels/GrapplingHookC3TallestMtn2Top_map_0.mpb", "/levels/GrapplingHookC3TallestMtn2Top_map_1.mpb", "/levels/GrapplingSkiHill_map_0.mpb", "/levels/GrapplingSkiHill_map_1.mpb", "/levels/GrapplingTutorial01_map_0.mpb", "/levels/GrapplingTutorial01_map_1.mpb", "/levels/HBDen_map_0.mpb", "/levels/HBLobby_map_0.mpb", "/levels/HBStorage_map_0.mpb", "/levels/HBWorkshop_map_0.mpb", "/levels/French/HeadQuarters0_map_0.mpb", "/levels/Spanish/HeadQuarters0_map_0.mpb", "/levels/HeadQuarters0_map_0.mpb", "/levels/HerbsCamp0_map_0.mpb", "/levels/HerbsCamp0_map_1.mpb", "/levels/Iceberg0_map_0.mpb", "/levels/Iceberg0_map_1.mpb", "/levels/French/IceRink0_map_0.mpb", "/levels/Spanish/IceRink0_map_0.mpb", "/levels/IceRink0_map_0.mpb", "/levels/French/IceRinkFlood_map_0.mpb", "/levels/Spanish/IceRinkFlood_map_0.mpb", "/levels/IceRinkFlood_map_0.mpb", "/levels/Jackhammer_map_0.mpb", "/levels/JackhammerBonus1_map_0.mpb", "/levels/JackhammerBonus2_map_0.mpb", "/levels/JackhammerBonus3_map_0.mpb", "/levels/JH_6_map_0.mpb", "/levels/JH_geyser_map_0.mpb", "/levels/JH_mountain_map_0.mpb", "/levels/JH_Snow_map_0.mpb", "/levels/LevelSelect_map_0.mpb", "/levels/French/Lighthouse0_map_0.mpb", "/levels/Spanish/Lighthouse0_map_0.mpb", "/levels/Lighthouse0_map_0.mpb", "/levels/Lobby_map_0.mpb", "/levels/Lodge0_map_0.mpb", "/levels/Lounge0_map_0.mpb", "/levels/M11_cornMaze1_map_0.mpb", "/levels/M11_cornMaze2_map_0.mpb", "/levels/M8_caves_map_0.mpb", "/levels/M9_frozenLake_map_0.mpb", "/levels/Maze0_map_0.mpb", "/levels/Maze1_map_0.mpb", "/levels/MazeBegin0_map_0.mpb", "/levels/MineCrash0_map_0.mpb", "/levels/MineFlashlight_map_0.mpb", "/levels/MineInterior0_map_0.mpb", "/levels/MineLair0_map_0.mpb", "/levels/MineShack0_map_0.mpb", "/levels/MineShack0_map_1.mpb", "/levels/MineShack1_map_0.mpb", "/levels/MineShack1_map_1.mpb", "/levels/MineShackFlood_map_0.mpb", "/levels/MineShackFlood_map_1.mpb", "/levels/MineShackPuddles_map_0.mpb", "/levels/MineShackPuddles_map_1.mpb", "/levels/MineShed0_map_0.mpb", "/levels/French/NightClub0_map_0.mpb", "/levels/Spanish/NightClub0_map_0.mpb", "/levels/NightClub0_map_0.mpb", "/levels/French/NightClubMagnet0_map_0.mpb", "/levels/Spanish/NightClubMagnet0_map_0.mpb", "/levels/NightClubMagnet0_map_0.mpb", "/levels/French/NightClubTremors0_map_0.mpb", "/levels/Spanish/NightClubTremors0_map_0.mpb", "/levels/NightClubTremors0_map_0.mpb", "/levels/PetShop0_map_0.mpb", "/levels/French/PizzaParlor0_map_0.mpb", "/levels/Spanish/PizzaParlor0_map_0.mpb", "/levels/PizzaParlor0_map_0.mpb", "/levels/French/Plaza0_map_0.mpb", "/levels/Spanish/Plaza0_map_0.mpb", "/levels/Plaza0_map_0.mpb", "/levels/Plaza0_map_1.mpb", "/levels/Pool0_map_0.mpb", "/levels/PuffleTraining0_map_0.mpb", "/levels/French/SkiHill0_map_0.mpb", "/levels/Spanish/SkiHill0_map_0.mpb", "/levels/SkiHill0_map_0.mpb", "/levels/SkiHill0_map_1.mpb", "/levels/French/SkiVillage0_map_0.mpb", "/levels/Spanish/SkiVillage0_map_0.mpb", "/levels/SkiVillage0_map_0.mpb", "/levels/SkiVillage0_map_1.mpb", "/levels/SkiVillage1_map_0.mpb", "/levels/SkiVillage1_map_1.mpb", "/levels/French/SkiVillage2_map_0.mpb", "/levels/Spanish/SkiVillage2_map_0.mpb", "/levels/SkiVillage2_map_0.mpb", "/levels/SkiVillage2_map_1.mpb", "/levels/French/SkiVillageFlood_map_0.mpb", "/levels/Spanish/SkiVillageFlood_map_0.mpb", "/levels/SkiVillageFlood_map_0.mpb", "/levels/SkiVillageFlood_map_1.mpb", "/levels/SkyLight0_map_0.mpb", "/levels/French/SnowForts0_map_0.mpb", "/levels/Spanish/SnowForts0_map_0.mpb", "/levels/SnowForts0_map_0.mpb", "/levels/French/SnowFortsFlood_map_0.mpb", "/levels/Spanish/SnowFortsFlood_map_0.mpb", "/levels/SnowFortsFlood_map_0.mpb", "/levels/SnowFortsFlood_map_1.mpb", "/levels/French/SportShop0_map_0.mpb", "/levels/Spanish/SportShop0_map_0.mpb", "/levels/SportShop0_map_0.mpb", "/levels/French/Stage0_map_0.mpb", "/levels/Spanish/Stage0_map_0.mpb", "/levels/Stage0_map_0.mpb", "/levels/TallestMountainTop0_map_0.mpb", "/levels/TallestMountainTop0_map_1.mpb", "/levels/TallestMountainTop1_map_0.mpb", "/levels/TallestMountainTop1_map_1.mpb", "/levels/TallestMt1_map_0.mpb", "/levels/TallestMt1_map_1.mpb", "/levels/TallestMt2_map_0.mpb", "/levels/TallestMt2_map_1.mpb", "/levels/TallestMt3_map_0.mpb", "/levels/TallestMt3_map_1.mpb", "/levels/TallestMtnBase_map_0.mpb", "/levels/TallestMtnBase_map_1.mpb", "/levels/French/Town0_map_0.mpb", "/levels/Spanish/Town0_map_0.mpb", "/levels/Town0_map_0.mpb", "/levels/Town0_map_1.mpb", "/levels/French/TownFlood_map_0.mpb", "/levels/Spanish/TownFlood_map_0.mpb", "/levels/TownFlood_map_0.mpb", "/levels/TownFlood_map_1.mpb", "/levels/French/TownTremors0_map_0.mpb", "/levels/Spanish/TownTremors0_map_0.mpb", "/levels/TownTremors0_map_0.mpb", "/levels/TownTremors0_map_1.mpb", "/levels/TrainingCave1_map_0.mpb", "/levels/TrainingCave2_map_0.mpb", "/levels/TrainingCave3_map_0.mpb", "/levels/TrainingCave4_map_0.mpb", "/levels/TrainingCave5_map_0.mpb", "/levels/TrainingCave6_map_0.mpb", "/levels/TrainingCave7_map_0.mpb", "/levels/Trough_map_0.mpb", "/levels/UG_Boiler0_map_0.mpb", "/levels/UG_Gift0_map_0.mpb", "/levels/UG_Gift1_map_0.mpb", "/levels/WildernessBerry0_map_0.mpb", "/levels/WildernessBerry0_map_1.mpb", "/levels/WildernessCave0_map_0.mpb", "/levels/WildernessCave0_map_1.mpb", "/levels/WildernessClearing0_map_0.mpb", "/levels/WildernessClearing0_map_1.mpb", "/levels/WildernessCliff0_map_0.mpb", "/levels/WildernessCliff0_map_1.mpb", "/levels/WildernessCliffClose0_map_0.mpb", "/levels/WildernessCliffClose0_map_1.mpb", "/levels/WildernessPuffle0_map_0.mpb", "/levels/WildernessPuffle0_map_1.mpb", "/levels/WildernessRiver0_map_0.mpb", "/levels/WildernessRiver0_map_1.mpb", "/levels/AquaGrabber.tsb", "/levels/AquaGrabber.tsb", "/levels/AquagrabberDash.tsb", "/levels/AquaGrabber.tsb", "/levels/AquaGrabber.tsb", "/levels/AquagrabberDash.tsb", "/levels/AquaRescue3.tsb", "/levels/AquaRescue3.tsb", "/levels/AquagrabberDash.tsb", "/levels/AquaRescue3.tsb", "/levels/AquaRescue3.tsb", "/levels/AquagrabberDash.tsb", "/levels/Attic.tsb", "/levels/LabyrinthCave.tsb", "/levels/LabyrinthWilderness.tsb", "/levels/LabyrinthWilderness.tsb", "/levels/Beach.tsb", "/levels/Beach_BG.tsb", "/levels/Beacon.tsb", "/levels/BoilerRoom.tsb", "/levels/BoilerRoom.tsb", "/levels/BookRoom.tsb", "/levels/LabyrinthCave.tsb", "/levels/LabyrinthCave.tsb", "/levels/CaveInteriorHerb.tsb", "/levels/CaveInteriorHerb.tsb", "/levels/CaveInteriorHerb.tsb", "/levels/CoffeeMachine.tsb", "/levels/CoffeeShop.tsb", "/levels/CoffeeShop.tsb", "/levels/CoffeeShop.tsb", "/levels/CommandRoom.tsb", "/levels/HerbDesk.tsb", "/levels/Beach_BG.tsb", "/levels/CornMaze.tsb", "/levels/Beach_BG.tsb", "/levels/CornMaze.tsb", "/levels/Beach_BG.tsb", "/levels/CornMaze.tsb", "/levels/Beach_BG.tsb", "/levels/CornMaze.tsb", "/levels/Beach_BG.tsb", "/levels/CornMaze.tsb", "/levels/Beach_BG.tsb", "/levels/Beach.tsb", "/levels/Beach_BG.tsb", "/levels/Dojo.tsb", "/levels/DVDMachine.tsb", "/levels/Fishing.tsb", "/levels/Beach_BG.tsb", "/levels/Forest.tsb", "/levels/Beach_BG.tsb", "/levels/FurAnalyzer.tsb", "/levels/GadgetRoom.tsb", "/levels/GadgetRoom.tsb", "/levels/GarysRooms.tsb", "/levels/GiftOffice.tsb", "/levels/GiftOffice.tsb", "/levels/GiftRoof.tsb", "/levels/GiftShopVid.tsb", "/levels/GiftShop.tsb", "/levels/GiftShop.tsb", "/levels/GiftShop.tsb", "/levels/DesignTiles.tsb", "/levels/Beach_BG.tsb", "/levels/GrapplingHook_HerbBase.tsb", "/levels/Grappling.tsb", "/levels/GrapplingMountain.tsb", "/levels/Beach_BG.tsb", "/levels/GrapplingHook_HerbBase.tsb", "/levels/Grappling.tsb", "/levels/GrapplingMountain.tsb", "/levels/Beach_BG.tsb", "/levels/GrapplingMountain.tsb", "/levels/Beach_BG.tsb", "/levels/GrapplingMountain.tsb", "/levels/Beach_BG.tsb", "/levels/GrapplingBonus.tsb", "/levels/GrapplingBG.tsb", "/levels/HerbertsCave.tsb", "/levels/HerbertsCave.tsb", "/levels/HerbertsCave.tsb", "/levels/HerbertsCave.tsb", "/levels/HQ.tsb", "/levels/HerbsCamp.tsb", "/levels/Beach_BG.tsb", "/levels/Beach.tsb", "/levels/Beach_BG.tsb", "/levels/IceRink.tsb", "/levels/IceRink.tsb", "/levels/Jackhammer.tsb", "/levels/JH_Snow.tsb", "/levels/JH_Snow.tsb", "/levels/Jackhammer_mountain.tsb", "/levels/Jackhammer_geyser.tsb", "/levels/Jackhammer_geyser.tsb", "/levels/Jackhammer_mountain.tsb", "/levels/JH_Snow.tsb", "/levels/LevelSelectMap.tsb", "/levels/Lighthouse.tsb", "/levels/Lobby.tsb", "/levels/Lodge.tsb", "/levels/Lounge.tsb", "/levels/LabyrinthCorn.tsb", "/levels/LabyrinthCorn.tsb", "/levels/LabyrinthCave.tsb", "/levels/LabyrinthWilderness.tsb", "/levels/UG_Gift0.tsb", "/levels/UG_Gift0.tsb", "/levels/UG_Gift0.tsb", "/levels/MineCrashSite.tsb", "/levels/MineTunnelExit.tsb", "/levels/MineInterior.tsb", "/levels/MineTunnelExit.tsb", "/levels/MineExterior.tsb", "/levels/Beach_BG.tsb", "/levels/MineExterior.tsb", "/levels/Beach_BG.tsb", "/levels/MineExterior.tsb", "/levels/Beach_BG.tsb", "/levels/MineExterior.tsb", "/levels/Beach_BG.tsb", "/levels/MineShedInterior.tsb", "/levels/NightClub.tsb", "/levels/NightClub.tsb", "/levels/NightClub.tsb", "/levels/PetShop.tsb", "/levels/PizzaParlor.tsb", "/levels/Plaza.tsb", "/levels/Beach_BG.tsb", "/levels/Pool.tsb", "/levels/PuffleTraining.tsb", "/levels/Mountain.tsb", "/levels/Beach_BG.tsb", "/levels/SkiVillage.tsb", "/levels/Beach_BG.tsb", "/levels/SkiVillage.tsb", "/levels/Beach_BG.tsb", "/levels/SkiVillage.tsb", "/levels/Beach_BG.tsb", "/levels/SkiVillage.tsb", "/levels/Beach_BG.tsb", "/levels/Labyrinth1.tsb", "/levels/SnowForts.tsb", "/levels/SnowForts.tsb", "/levels/Beach_BG.tsb", "/levels/SportShop.tsb", "/levels/Stage.tsb", "/levels/TallestMountainTop.tsb", "/levels/TallestMountainTop_BG.tsb", "/levels/TallestMountainTop.tsb", "/levels/TallestMountainTop_BG.tsb", "/levels/TallestMtn.tsb", "/levels/Beach_BG.tsb", "/levels/TallestMtn.tsb", "/levels/Beach_BG.tsb", "/levels/TallestMtn.tsb", "/levels/Beach_BG.tsb", "/levels/TallestMtn.tsb", "/levels/Beach_BG.tsb", "/levels/Town.tsb", "/levels/Beach_BG.tsb", "/levels/Town.tsb", "/levels/Beach_BG.tsb", "/levels/Town.tsb", "/levels/Beach_BG.tsb", "/levels/TrainingCaves.tsb", "/levels/TrainingCavesRiver.tsb", "/levels/TrainingCaves.tsb", "/levels/TrainingCaves.tsb", "/levels/TrainingCaves.tsb", "/levels/TrainingCaves.tsb", "/levels/TrainingCaves.tsb", "/levels/Trough.tsb", "/levels/UG_Gift0.tsb", "/levels/UG_Gift0.tsb", "/levels/UG_Gift0.tsb", "/levels/Wilderness.tsb", "/levels/Beach_BG.tsb", "/levels/Wilderness.tsb", "/levels/Beach_BG.tsb", "/levels/Wilderness.tsb", "/levels/Beach_BG.tsb", "/levels/Wilderness.tsb", "/levels/Beach_BG.tsb", "/levels/Wilderness.tsb", "/levels/Beach_BG.tsb", "/levels/Wilderness.tsb", "/levels/Beach_BG.tsb", "/levels/Wilderness.tsb", "/levels/Beach_BG.tsb", "/fonts/BurbankSmall.fnt", "/fonts/DSdigital.fnt", "/fonts/DSdigitalCode.fnt", "/fonts/DSdigitalHUD.fnt", "/fonts/FFMGScore.fnt", "/fonts/MackFont.fnt", "/fonts/PengSpy.fnt", "/fonts/SnowFort.fnt", "/fonts/XPED.fnt", "/fonts/rockSnail.fnt", "/fonts/tinyUI.fnt", "/palettes/LabyrinthPenguin_aqua.nbfp", "/palettes/LabyrinthPenguin_black.nbfp", "/palettes/LabyrinthPenguin_blue.nbfp", "/palettes/LabyrinthPenguin_brown.nbfp", "/palettes/LabyrinthPenguin_darkGreen.nbfp", "/palettes/LabyrinthPenguin_fuschia.nbfp", "/palettes/LabyrinthPenguin_green.nbfp", "/palettes/LabyrinthPenguin_lime.nbfp", "/palettes/LabyrinthPenguin_orange.nbfp", "/palettes/LabyrinthPenguin_peach.nbfp", "/palettes/LabyrinthPenguin_purple.nbfp", "/palettes/LabyrinthPenguin_red.nbfp", "/palettes/LabyrinthPenguin_yellow.nbfp", "/palettes/aqua.nbfp", "/palettes/black.nbfp", "/palettes/blue.nbfp", "/palettes/brown.nbfp", "/palettes/coach_aqua.nbfp", "/palettes/coach_black.nbfp", "/palettes/coach_blue.nbfp", "/palettes/coach_brown.nbfp", "/palettes/coach_darkGreen.nbfp", "/palettes/coach_fuschia.nbfp", "/palettes/coach_green.nbfp", "/palettes/coach_lime.nbfp", "/palettes/coach_orange.nbfp", "/palettes/coach_peach.nbfp", "/palettes/coach_purple.nbfp", "/palettes/coach_red.nbfp", "/palettes/coach_yellow.nbfp", "/palettes/dance_aqua.nbfp", "/palettes/dance_black.nbfp", "/palettes/dance_blue.nbfp", "/palettes/dance_brown.nbfp", "/palettes/dance_darkGreen.nbfp", "/palettes/dance_fuschia.nbfp", "/palettes/dance_green.nbfp", "/palettes/dance_lime.nbfp", "/palettes/dance_orange.nbfp", "/palettes/dance_peach.nbfp", "/palettes/dance_purple.nbfp", "/palettes/dance_red.nbfp", "/palettes/dance_yellow.nbfp", "/palettes/darkGreen.nbfp", "/palettes/fuschia.nbfp", "/palettes/green.nbfp", "/palettes/jetpack_aqua.nbfp", "/palettes/jetpack_black.nbfp", "/palettes/jetpack_blue.nbfp", "/palettes/jetpack_brown.nbfp", "/palettes/jetpack_darkGreen.nbfp", "/palettes/jetpack_fuschia.nbfp", "/palettes/jetpack_green.nbfp", "/palettes/jetpack_lime.nbfp", "/palettes/jetpack_orange.nbfp", "/palettes/jetpack_peach.nbfp", "/palettes/jetpack_purple.nbfp", "/palettes/jetpack_red.nbfp", "/palettes/jetpack_yellow.nbfp", "/palettes/lime.nbfp", "/palettes/mapPenguin_aqua.nbfp", "/palettes/mapPenguin_black.nbfp", "/palettes/mapPenguin_blue.nbfp", "/palettes/mapPenguin_brown.nbfp", "/palettes/mapPenguin_darkGreen.nbfp", "/palettes/mapPenguin_fuschia.nbfp", "/palettes/mapPenguin_green.nbfp", "/palettes/mapPenguin_lime.nbfp", "/palettes/mapPenguin_orange.nbfp", "/palettes/mapPenguin_peach.nbfp", "/palettes/mapPenguin_purple.nbfp", "/palettes/mapPenguin_red.nbfp", "/palettes/mapPenguin_yellow.nbfp", "/palettes/mapPuffle_black.nbfp", "/palettes/mapPuffle_blue.nbfp", "/palettes/mapPuffle_green.nbfp", "/palettes/mapPuffle_pink.nbfp", "/palettes/mapPuffle_purple.nbfp", "/palettes/mapPuffle_red.nbfp", "/palettes/mapPuffle_white.nbfp", "/palettes/mapPuffle_yellow.nbfp", "/palettes/orange.nbfp", "/palettes/peach.nbfp", "/palettes/playerIcon_aqua.nbfp", "/palettes/playerIcon_black.nbfp", "/palettes/playerIcon_blue.nbfp", "/palettes/playerIcon_brown.nbfp", "/palettes/playerIcon_darkGreen.nbfp", "/palettes/playerIcon_fuschia.nbfp", "/palettes/playerIcon_green.nbfp", "/palettes/playerIcon_lime.nbfp", "/palettes/playerIcon_orange.nbfp", "/palettes/playerIcon_peach.nbfp", "/palettes/playerIcon_purple.nbfp", "/palettes/playerIcon_red.nbfp", "/palettes/playerIcon_yellow.nbfp", "/palettes/purple.nbfp", "/palettes/red.nbfp", "/palettes/yellow.nbfp", "/strings/DialogStrings.st2", "/strings/DialogStrings.crc", "/strings/GameStrings.st2", "/strings/GameStrings.crc", "/strings/ItemStrings.st2", "/strings/ItemStrings.crc", "/strings/DisneyStrings.st2", "/strings/DisneyStrings.crc", "/common.rdt", "/csv/ClothingData.dat", "/csv/ClothingData.h", "/csv/accentKeyMap.dat", "/csv/accentKeyMap.h", "/csv/asciiKeyMap.dat", "/csv/asciiKeyMap.h", "/archive_list.txt", "/levels/Jackhammer_map_0.mpb", "/levels/Jackhammer.tsb", "/sound/music.bin", "/sound/sfx.bin", "/strings/GameStrings.st2", "/strings/GameStrings.crc", "/archive_list.txt", "/bg/Cutscenes/1PLogobottom.nbfc", "/bg/Cutscenes/1PLogobottom.nbfs", "/bg/Cutscenes/1PLogobottom.nbfp", "/bg/Cutscenes/1PLogotop.nbfc", "/bg/Cutscenes/1PLogotop.nbfs", "/bg/Cutscenes/1PLogotop.nbfp", "/bg/Cutscenes/BlankBlack.nbfc", "/bg/Cutscenes/BlankBlack.nbfs", "/bg/Cutscenes/BlankBlack.nbfp", "/bg/Cutscenes/DisneyLogo.nbfc", "/bg/Cutscenes/DisneyLogo.nbfs", "/bg/Cutscenes/DisneyLogo.nbfp", "/bg/Cutscenes/DisneyLogoCopyright.nbfc", "/bg/Cutscenes/DisneyLogoCopyright.nbfs", "/bg/Cutscenes/DisneyLogoCopyright.nbfp", "/bg/Cutscenes/NintendoTop.nbfc", "/bg/Cutscenes/NintendoTop.nbfs", "/bg/Cutscenes/NintendoTop.nbfp", "/bg/Jackhammer/JackInstructions_01.nbfc", "/bg/Jackhammer/JackInstructions_01.nbfs", "/bg/Jackhammer/JackInstructions_01.nbfp", "/bg/Jackhammer/JackInstructions_02.nbfc", "/bg/Jackhammer/JackInstructions_02.nbfs", "/bg/Jackhammer/JackInstructions_02.nbfp", "/bg/Jackhammer/JackInstructions_03.nbfc", "/bg/Jackhammer/JackInstructions_03.nbfs", "/bg/Jackhammer/JackInstructions_03.nbfp", "/bg/Jackhammer/titleScreenMissions_bottom.nbfc", "/bg/Jackhammer/titleScreenMissions_bottom.nbfs", "/bg/Jackhammer/titleScreenMissions_bottom.nbfp", "/bg/Jackhammer/titleScreen_bottom.nbfc", "/bg/Jackhammer/titleScreen_bottom.nbfs", "/bg/Jackhammer/titleScreen_bottom.nbfp", "/bg/Jackhammer/titleScreen_bottom_02.nbfc", "/bg/Jackhammer/titleScreen_bottom_02.nbfs", "/bg/Jackhammer/titleScreen_bottom_02.nbfp", "/bg/Jackhammer/titleScreen_top.nbfc", "/bg/Jackhammer/titleScreen_top.nbfs", "/bg/Jackhammer/titleScreen_top.nbfp", "/bg/Jackhammer/titleScreen_top_02.nbfc", "/bg/Jackhammer/titleScreen_top_02.nbfs", "/bg/Jackhammer/titleScreen_top_02.nbfp", "/fonts/Burbank.fnt", "/fonts/Comicrazy.fnt", "/fonts/ComicrazyHUD.fnt", "/fonts/ComicrazyTitle.fnt", "/fonts/bigUI.fnt", "/fonts/faceFront.fnt", "/fonts/hugeUI.fnt", "/fonts/smallUI.fnt", "/strings/adhocstrings.st2", "/strings/adhocstrings.crc", "/strings/localizationStrings.st2", "/strings/localizationStrings.crc", "/strings/savegamestrings.st2", "/strings/savegamestrings.crc", "/strings/wifistrings.st2", "/strings/wifistrings.crc", "/palettes/jackhammer_aqua.nbfp", "/palettes/jackhammer_black.nbfp", "/palettes/jackhammer_blue.nbfp", "/palettes/jackhammer_brown.nbfp", "/palettes/jackhammer_darkGreen.nbfp", "/palettes/jackhammer_fuschia.nbfp", "/palettes/jackhammer_green.nbfp", "/palettes/jackhammer_hotsauce.nbfp", "/palettes/jackhammer_lime.nbfp", "/palettes/jackhammer_orange.nbfp", "/palettes/jackhammer_peach.nbfp", "/palettes/jackhammer_purple.nbfp", "/palettes/jackhammer_red.nbfp", "/palettes/jackhammer_yellow.nbfp", "/game.rdt", "/archive_list.txt" };

        public void Open()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            if (mode == Mode.Arc && activeArc != null)
            {
                openFileDialog1.InitialDirectory = Path.GetDirectoryName(activeArc.filename);
            }
            else if (mode == Mode.Rdt && activeRdt != null)
            {
                openFileDialog1.InitialDirectory = Path.GetDirectoryName(activeRdt.filename);
            }

            openFileDialog1.Filter = "1PP archives (*.arc,*.rdt,*.bin)|*.arc;*.rdt;*.bin";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                switch (Path.GetExtension(openFileDialog1.FileName))
                {
                    case ".arc":
                        mode = Mode.Arc;

                        randomizeRDTSpritesToolStripMenuItem.Visible = false;
                        massRDTExportToolStripMenuItem.Visible = false;
                        massXMExportToolStripMenuItem.Visible = false;
                        RDTsettingVersionToolStripButton.Visible = false;

                        ParseArc(openFileDialog1.FileName);
                        activeArc.ViewArcInFileTree();
                        break;
                    case ".bin":
                        mode = Mode.Bin;

                        randomizeRDTSpritesToolStripMenuItem.Visible = false;
                        massRDTExportToolStripMenuItem.Visible = false;
                        massXMExportToolStripMenuItem.Visible = true;
                        RDTsettingVersionToolStripButton.Visible = false;

                        ParseBin(openFileDialog1.FileName);
                        MakeFileTree();
                        break;
                    case ".rdt":
                        mode = Mode.Rdt;

                        randomizeRDTSpritesToolStripMenuItem.Visible = true;
                        massRDTExportToolStripMenuItem.Visible = true;
                        massXMExportToolStripMenuItem.Visible = false;
                        RDTsettingVersionToolStripButton.Visible = true;

                        ParseRdt(openFileDialog1.FileName);
                        MakeFileTree();
                        break;
                }
            }
            else
            {
                return;
            }
        }

        public void ParseArc(string filename)
        {
            arcfile newarc = new arcfile();

            activeArc = newarc;
            newarc.arcname = Path.GetFileName(filename);
            newarc.filename = filename;
            newarc.filebytes = File.ReadAllBytes(filename);
            newarc.form1 = this;
            newarc.ReadArc();
        }

        public void ParseBin(string filename)
        {
            binfile newbin = new binfile();

            activeBin = newbin;
            newbin.binname = Path.GetFileName(filename);
            newbin.filename = filename;
            newbin.filebytes = File.ReadAllBytes(filename);
            newbin.form1 = this;

            DialogResult dialogResult = MessageBox.Show("Is this a music archive?", "Music or sfx?", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                activeBin.binMode = binfile.binmode.music;
            }
            else if (dialogResult == DialogResult.No)
            {
                activeBin.binMode = binfile.binmode.sfx;
            }

            if (activeBin.binMode == binfile.binmode.sfx)
            {
                newbin.ReadBin();
            }
            else
            {
                newbin.ReadMusicBin();
            }
        }

        public void ParseRdt(string filename)
        {
            rdtfile newrdt = new rdtfile();

            activeRdt = newrdt;
            newrdt.arcname = Path.GetFileName(filename);
            newrdt.filename = filename;
            newrdt.filebytes = File.ReadAllBytes(filename);
            newrdt.form1 = this;
            newrdt.ReadRdt();
        }

        private void saveFileExtractorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileEditor sfe = new SaveFileEditor();
            sfe.form1 = this;
            sfe.Show();
        }

        public uint CalculateHash(string filename)
        {
            uint output = 0xFFFFFFFF;

            //0x021C4680 in EU EPF is the start of a table that it adds an offset to to get the uint value for the big xor

            for (int i = 0; i < filename.Length; i++)
            {
                output = (output >> 8) ^ table_for_hashing[(((((byte)filename[i]) ^ output) << 0x18) >> 0x16) / 4];
            }

            output = ~output;       //logical NOT, basically inverts the bits

            return output;
        }

        public Color ABGR1555_to_RGBA32(ushort input)
        {

            //stored in file as
            //0110 1110 1101 1111
            //GGGR RRRR ABBB BBGG

            //therefore, when read as a little endian ushort, it should be like this, which is what is read here.
            //1101 1111 0110 1110
            //ABBB BBGG GGGR RRRR

            int b = (input & 0x7C00) >> 7;
            int g = (input & 0x03E0) >> 2;
            int r = (input & 0x001F) << 3;

            int a = 0xFF;

            Color output = Color.FromArgb(a, r, g, b);

            return output;
        }

        public ushort ColorToABGR1555(Color input)
        {
            ushort output = 0;

            output |= (ushort)(input.R >> 3);
            output |= (ushort)((input.G >> 3) << 5);
            output |= (ushort)((input.B >> 3) << 10);

            return output;
        }

        public void WriteU32ToArray(byte[] array, int offset, uint input)
        {
            array[offset] = (byte)(input & 0x000000FF);
            array[offset + 1] = (byte)((input & 0x0000FF00) >> 8);
            array[offset + 2] = (byte)((input & 0x00FF0000) >> 16);
            array[offset + 3] = (byte)((input & 0xFF000000) >> 24);
        }

        public void WriteU16ToArray(byte[] array, int offset, ushort input)
        {
            array[offset] = (byte)(input & 0x00FF);
            array[offset + 1] = (byte)((input & 0xFF00) >> 8);
        }

        public void WriteS16ToArray(byte[] array, int offset, short input)
        {
            array[offset] = (byte)(input & 0x00FF);
            array[offset + 1] = (byte)((input & 0xFF00) >> 8);
        }

        public string GetOrMakeDirectoryForFileName(string path)  //recursively make directories so that a given path exists
        {
            path = path.Replace("/", "\\");

            string[] splitPath = path.Split('\\');

            string workingPath = "";

            for (int i = 0; i < splitPath.Length - 1; i++)   //it's -1 because we don't want to create a directory for the last part of the path, because that's the file.
            {
                workingPath += splitPath[i] + "\\";

                if (!Directory.Exists(workingPath))
                {
                    Directory.CreateDirectory(workingPath);
                }
            }

            return path;
        }

        public int Find_Closest_File_To(uint requested_hash, arcfile arc)   //this is how the game looks up files by hash, and if it can't find the exact one, it uses the closest one instead
        {
            int i;

            int arc_file_count = (int)arc.filecount;

            int startPosition = 0;

            if (0 < arc_file_count)
            {
                do
                {
                    i = arc_file_count / 2; //interval bisection to find the closest file to the requested one

                    if (arc.archivedfiles[startPosition + i].hash < requested_hash)
                    {
                        startPosition = startPosition + i + 1;
                        i = arc_file_count - (i + 1);
                    }
                    arc_file_count = i;
                } while (0 < i);
            }

            if (startPosition >= arc.archivedfiles.Count || startPosition < 0)   //Couldn't find that hash in the arc (we went off the scale looking for it) We won't be able to get a filename for it, but may try some string variants later
            {
                startPosition = 999999999;
            }
            else if (arc.archivedfiles[startPosition].hash != requested_hash)    //Couldn't find that hash in the arc (we got as close as we could, but couldn't find the exact hash)! We won't be able to get a filename for it, but may try some string variants later
            {
                startPosition = 999999999;
            }

            return startPosition;   //returns index of the best match file
        }

        public uint readU32FromArray(Byte[] input, int offset)
        {
            uint output = (uint)input[offset] + ((uint)input[offset + 1] * 0x100) + ((uint)input[offset + 2] * 0x10000) + ((uint)input[offset + 3] * 0x1000000);

            return output;
        }

        public int readIntFromArray(Byte[] input, int offset)
        {
            int output = (int)input[offset] + ((int)input[offset + 1] * 0x100) + ((int)input[offset + 2] * 0x10000) + ((int)input[offset + 3] * 0x1000000);

            return output;
        }

        public void MakeFileTree()
        {

            int unnamed_files_count = 0;
            bool keep_unnamed_files_folder = false;

            FileTree.Nodes.Clear();

            treeNodesAndArchivedFiles = new Dictionary<TreeNode, archivedfile>();
            foldersProcessed = new Dictionary<string, TreeNode>();
            TreeNode NamesNotFoundFolder = null;

            if (mode == Mode.Arc)
            {
                FileTree.Nodes.Add(Path.GetFileName(activeArc.filename).Replace("/", ""));
                NamesNotFoundFolder = FileTree.Nodes[0].Nodes.Add("Names_not_found");
                foldersProcessed.Add(FileTree.Nodes[0].Text, FileTree.Nodes[0]);
                NamesNotFoundFolder.ImageIndex = 0;
                NamesNotFoundFolder.SelectedImageIndex = 0;
            }

            FileTree.BeginUpdate();

            List<archivedfile> archivedfiles = null;

            if (mode == Mode.Arc)
            {
                archivedfiles = activeArc.archivedfiles;
            }
            else if (mode == Mode.Rdt)
            {
                archivedfiles = activeRdt.archivedfiles;
            }
            else if (mode == Mode.Bin)
            {
                archivedfiles = new List<archivedfile>();

                if (activeBin.binMode == binfile.binmode.sfx)
                {
                    foreach (sfxfile sfx in activeBin.sfxfiles)
                    {
                        archivedfile newArchivedFile = new archivedfile();
                        newArchivedFile.filename = Path.GetFileName(activeBin.filename) + sfx.indexInBin;
                        newArchivedFile.linkedSfx = sfx;
                        archivedfiles.Add(newArchivedFile);
                    }
                }
                else
                {
                    foreach (xmfile mus in activeBin.xmfiles)
                    {
                        archivedfile newArchivedFile = new archivedfile();
                        newArchivedFile.filename = mus.name;
                        newArchivedFile.linkedXm = mus;
                        archivedfiles.Add(newArchivedFile);
                    }

                    // archivedfile sampleCollection = new archivedfile();
                    //sampleCollection.filename = "SAMPLES.sampleCollection";
                    //archivedfiles.Add(sampleCollection);
                }
            }

            foreach (archivedfile file in archivedfiles)
            {
                if (file.filename == "FILENAME_NOT_SET")
                {
                    keep_unnamed_files_folder = true;
                    unnamed_files_count++;

                    if (activeArc.knownfilemagic.Keys.Contains(file.filemagic))
                    {
                        file.treeNode = NamesNotFoundFolder.Nodes.Add(file.hash.ToString() + "." + activeArc.knownfilemagic[file.filemagic]);
                    }
                    else
                    {
                        file.treeNode = NamesNotFoundFolder.Nodes.Add(file.hash.ToString() + "." + file.filemagic.ToString());
                    }

                    file.treeNode.ImageIndex = 1;
                    file.treeNode.SelectedImageIndex = 1;
                    treeNodesAndArchivedFiles.Add(file.treeNode, file);

                    continue;
                }

                //now add entries to the file tree

                List<string> dirs = new List<string>();

                string tempfilepath = null;

                if (mode == Mode.Arc)
                {
                    tempfilepath = Path.GetFileName(activeArc.filename).Replace("/", "");
                }
                else if (mode == Mode.Rdt)
                {
                    tempfilepath = Path.GetFileName(activeRdt.filename).Replace("/", "");
                }
                else if (mode == Mode.Bin)
                {
                    tempfilepath = Path.GetFileName(activeBin.filename).Replace("/", "");
                }


                if (file.filename[0] != '/')
                {
                    tempfilepath += "/";
                }
                tempfilepath += file.filename;

                if (tempfilepath[tempfilepath.Length - 1] == '/')
                {
                    tempfilepath.Remove(tempfilepath.Length - 1);
                }

                if (tempfilepath[0] == '/')
                {
                    tempfilepath = tempfilepath.Substring(1, tempfilepath.Length - 1);
                }

                int number_of_dir_levels = tempfilepath.Split('/').Length;

                for (int d = 0; d < number_of_dir_levels - 1; d++)  //store a string for each level of the directory, so that we can check each folder individually (by this I mean checking whether or not it already exists in the tree)
                {
                    dirs.Add(Path.GetDirectoryName(tempfilepath));
                    tempfilepath = Path.GetDirectoryName(tempfilepath);

                    if (tempfilepath[tempfilepath.Length - 1] == '/')
                    {
                        tempfilepath.Remove(tempfilepath.Length - 1);
                    }
                }

                bool isRoot = true;
                TreeNode newestNode = new TreeNode();

                for (int f = dirs.Count - 1; f >= 0; f--)
                {
                    if (!foldersProcessed.Keys.Contains(dirs[f].ToLower()))    //if the folder isn't in the tree yet
                    {
                        if (!isRoot)
                        {   //add to the chain of nodes
                            FileTree.SelectedNode = newestNode;
                            newestNode = new TreeNode(Path.GetFileName(dirs[f]));
                            newestNode.ImageIndex = 0;
                            newestNode.SelectedImageIndex = 0;
                            FileTree.SelectedNode.Nodes.Add(newestNode);
                        }
                        else
                        { //create a root node first
                            newestNode = new TreeNode(Path.GetFileName(dirs[f]));
                            newestNode.ImageIndex = 0;
                            newestNode.SelectedImageIndex = 0;
                            FileTree.Nodes.Add(newestNode);
                            isRoot = false;
                        }

                        foldersProcessed.Add(dirs[f].ToLower(), newestNode);  //add it to the list of folders we've put in the tree   
                    }
                    else
                    {
                        newestNode = foldersProcessed[dirs[f].ToLower()]; //set the parent node of the next folder to the existing node
                        newestNode.ImageIndex = 0;
                        newestNode.SelectedImageIndex = 0;
                        FileTree.SelectedNode = newestNode;
                        isRoot = false;
                    }
                }
                FileTree.SelectedNode = newestNode;
                newestNode = new TreeNode(Path.GetFileName(file.filename));
                newestNode.ImageIndex = 1;
                newestNode.SelectedImageIndex = 1;
                FileTree.SelectedNode.Nodes.Add(newestNode);
                file.treeNode = newestNode;
                treeNodesAndArchivedFiles.Add(newestNode, file);
            }

            if (mode == Mode.Arc && !keep_unnamed_files_folder)
            {
                NamesNotFoundFolder.Remove();
            }

            for (int i = FileTree.Nodes.Count - 1; i >= 0; i--)   //delete any erroneous root nodes that occasionally crop up
            {
                if (FileTree.Nodes[i].Nodes == null || FileTree.Nodes[i].Nodes.Count == 0)
                {
                    FileTree.Nodes[i].Remove();
                }
            }

            FileTree.Sort();
            FileTree.CollapseAll();
            FileTree.EndUpdate();

            Console.Write("number of unnamed files: " + unnamed_files_count);
        }

        private void FileTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (mode == Mode.Arc)
                {
                    if (treeNodesAndArchivedFiles.Keys.Contains(FileTree.SelectedNode))
                    {
                        //file
                        archivedFileContextMenu.Show(Cursor.Position);
                    }
                    else
                    {
                        //folder
                        archivedFolderContextMenu.Show(Cursor.Position);
                    }
                }
                else if (mode == Mode.Rdt)
                {
                    if (treeNodesAndArchivedFiles.Keys.Contains(FileTree.SelectedNode))
                    {
                        //file
                        rdtSubfileContextMenu.Show(Cursor.Position);
                    }
                    else
                    {
                        //folder
                        archivedFolderContextMenu.Show(Cursor.Position);
                    }
                }
                else if (mode == Mode.Bin)
                {
                    if (treeNodesAndArchivedFiles.Keys.Contains(FileTree.SelectedNode))
                    {
                        //file
                        binFileContextMenu.Show(Cursor.Position);
                    }
                    else
                    {
                        //folder
                        binFolderContextMenu.Show(Cursor.Position);
                    }
                }

            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)  //RENAME FILE
        {
            if (FileTree.SelectedNode != null && FileTree.SelectedNode.Parent != null)
            {
                FileTree.LabelEdit = true;

                if (!FileTree.SelectedNode.IsEditing)
                {
                    FileTree.SelectedNode.BeginEdit();
                }
            }
            else
            {
                MessageBox.Show("You cannot rename that file.", "Invalid selection");
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)  //EXPORT FILE
        {
            treeNodesAndArchivedFiles[FileTree.SelectedNode].ExportToFile();
        }

        private void addFileToFolder_Click(object sender, EventArgs e)          //ADD FILE TO FOLDER
        {
            TreeNode parentNode = FileTree.SelectedNode;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "All files (*.*)|*.*";

            if (mode == Mode.Rdt){
                openFileDialog1.Filter = "1PP sprite files (*.sprite*)|*.sprite";
            }

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (mode == Mode.Arc)
                {
                    foreach (string openedFileName in openFileDialog1.FileNames)
                    {
                        bool AbortThisParticularFile = false;

                        foreach (TreeNode child in parentNode.Nodes)    //check to see if this file already exists and ask the user if they want to replace it or not
                        {
                            if (child.Text == Path.GetFileName(openedFileName) && treeNodesAndArchivedFiles.ContainsKey(child))
                            {
                                DialogResult dialogResult = MessageBox.Show("There is already a file with the name \n\"" + child.Text + "\" in that directory.\nWould you like to replace it?", "File already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (dialogResult == DialogResult.Yes)
                                {
                                    activeArc.archivedfiles.Remove(treeNodesAndArchivedFiles[child]);
                                    treeNodesAndArchivedFiles.Remove(child);
                                    parentNode.Nodes.Remove(child);
                                }
                                else
                                {
                                    AbortThisParticularFile = true;
                                }
                                break;
                            }
                        }

                        if (AbortThisParticularFile)
                        {
                            continue;
                        }

                        archivedfile newfile = new archivedfile();

                        newfile.filename = parentNode.FullPath.Replace(FileTree.Nodes[0].Text, "").Replace("\\", "/");

                        if (newfile.filename.Length > 0 && newfile.filename[newfile.filename.Length - 1] != '/')
                        {
                            newfile.filename += "/";
                        }

                        if (newfile.filename.Length == 0 && mode == Mode.Arc)
                            {
                            newfile.filename = "/";
                            }

                        newfile.filename += Path.GetFileName(openedFileName);

                        newfile.filebytes = File.ReadAllBytes(openedFileName);
                        newfile.size = newfile.filebytes.Length;
                        newfile.parentarcfile = activeArc;
                        newfile.ReadFile();

                        TreeNode newNode = parentNode.Nodes.Add(Path.GetFileName(openedFileName));
                        newNode.ImageIndex = 1;
                        newNode.SelectedImageIndex = 1;

                        newfile.treeNode = newNode;

                        treeNodesAndArchivedFiles.Add(newNode, newfile);

                        activeArc.archivedfiles.Add(newfile);
                    }
                }
                else if (mode == Mode.Rdt)
                {
                    foreach (string openedFileName in openFileDialog1.FileNames)
                    {
                        bool AbortThisParticularFile = false;

                        foreach (TreeNode child in parentNode.Nodes)    //check to see if this file already exists and ask the user if they want to replace it or not
                        {
                            if (child.Text == Path.GetFileNameWithoutExtension(openedFileName) && treeNodesAndArchivedFiles.ContainsKey(child))
                            {
                                DialogResult dialogResult = MessageBox.Show("There is already a file with the name \n\"" + child.Text + "\" in that directory.\nWould you like to replace it?", "File already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (dialogResult == DialogResult.Yes)
                                {
                                    activeRdt.archivedfiles.Remove(treeNodesAndArchivedFiles[child]);
                                    treeNodesAndArchivedFiles.Remove(child);
                                    parentNode.Nodes.Remove(child);
                                }
                                else
                                {
                                    AbortThisParticularFile = true;
                                }
                                break;
                            }
                        }

                        if (AbortThisParticularFile)
                        {
                            continue;
                        }

                        rdtfile newSprite = new rdtfile();

                        newSprite.filename = parentNode.FullPath.Replace(FileTree.Nodes[0].Text, "").Replace("\\", "/");  //erase name of root folder (i.e. the rdt name) but keep the rest of the path

                        if (newSprite.filename.Length > 0 && newSprite.filename[newSprite.filename.Length - 1] != '/')
                        {
                            newSprite.filename += "/";        //add a slash at the end if needed
                        }

                        newSprite.filename += Path.GetFileNameWithoutExtension(openedFileName);   //add new filename to end of directory string

                        if (newSprite.filename[0] == '/')
                        {
                            newSprite.filename = newSprite.filename.Substring(1, newSprite.filename.Length - 1);  //if the first character in the filename is '/', remove that character
                        }

                        newSprite.filebytes = File.ReadAllBytes(openedFileName);
                        newSprite.form1 = this;
                        newSprite.ReadRdt();

                        TreeNode newNode = parentNode.Nodes.Add(Path.GetFileNameWithoutExtension(openedFileName));
                        newNode.ImageIndex = 1;
                        newNode.SelectedImageIndex = 1;
                        newSprite.archivedfiles[0].treeNode = newNode;
                        newSprite.archivedfiles[0].filename = newSprite.filename;


                        treeNodesAndArchivedFiles.Add(newNode, newSprite.archivedfiles[0]);

                        activeRdt.archivedfiles.Add(newSprite.archivedfiles[0]);
                    }
                }
            }
        }

        private void addFolderToFolder_Click(object sender, EventArgs e)            //ADD CHILD FOLDER
        {
            AddChildFolder(FileTree.SelectedNode);
        }

        private void addChildFolderRDT_Click(object sender, EventArgs e)
        {
            AddChildFolder(FileTree.SelectedNode);
        }

        public void AddChildFolder(TreeNode targetNode)
        {

            foreach (TreeNode node in targetNode.Nodes)
            {
                if (node.Text == "New Folder")
                {
                    MessageBox.Show("There is already a file/folder with the name \n\"New Folder\" in that directory!", "Folder name conflict", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            targetNode.Nodes.Add("New Folder");
        }

        private void renameFolderToolStripMenuItem_Click(object sender, EventArgs e)    //RENAME FOLDER
        {
            if (FileTree.SelectedNode != null && FileTree.SelectedNode.Parent != null)
            {
                FileTree.LabelEdit = true;

                if (!FileTree.SelectedNode.IsEditing)
                {
                    FileTree.SelectedNode.BeginEdit();
                }
            }
            else
            {
                MessageBox.Show("You cannot rename the root folder.", "Invalid selection");
            }
        }

        private void exportToolStripMenuItem1_Click(object sender, EventArgs e) //EXPORT FOLDER
        {
            if (mode == Mode.Arc)
            {
                activeArc.ExportFolder(FileTree.SelectedNode);
            }
            else if (mode == Mode.Rdt)
            {
                MessageBox.Show("Folder exports are not supported for RDT files.", "Not supported", MessageBoxButtons.OK);
            }
        }



        private void deleteFolder_Click(object sender, EventArgs e) //DELETE FOLDER
        {
            TreeNode node = FileTree.SelectedNode;

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete the folder \"" + node.FullPath + "\" and all its subfolders?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Yes)
            {
                DeleteSubfiles(node);
                node.Remove();
            }
        }

        public void DeleteSubfiles(TreeNode node)
        {
            foreach (TreeNode child in node.Nodes)
            {
                if (child == null)
                {
                    continue;
                }

                if (treeNodesAndArchivedFiles.ContainsKey(child))        //if it's a file
                {
                    if (mode == Mode.Arc)
                    {
                        activeArc.archivedfiles.Remove(treeNodesAndArchivedFiles[child]);
                    }
                    else if (mode == Mode.Rdt)
                    {
                        activeRdt.archivedfiles.Remove(treeNodesAndArchivedFiles[child]);
                    }

                    treeNodesAndArchivedFiles.Remove(child);
                }
                else                                                     //if it's a folder
                {
                    DeleteSubfiles(child);
                    child.Remove();
                }
            }
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e) //REPLACE FILE
        {
            treeNodesAndArchivedFiles[FileTree.SelectedNode].ReplaceFile();
        }

        public void WriteIntToArray(Byte[] array, int offset, int input)
        {
            array[offset] = (byte)input;
            array[offset + 1] = (byte)(input >> 8);
            array[offset + 2] = (byte)(input >> 0x10);
            array[offset + 3] = (byte)(input >> 0x18);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mode == Mode.Arc)
            {
                activeArc.RebuildArc();

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.FileName = Path.GetFileName(activeArc.filename);

                saveFileDialog1.Title = "Save arc file";
                saveFileDialog1.CheckPathExists = true;
                saveFileDialog1.Filter = "1PP archive (*.arc)|*.arc|All files (*.*)|*.*";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    activeArc.filename = saveFileDialog1.FileName;
                    File.WriteAllBytes(saveFileDialog1.FileName, activeArc.filebytes);
                }
            }
            else if (mode == Mode.Rdt)
            {
                activeRdt.RebuildRDT();
            }
            else if (mode == Mode.Bin)
            {
                if (activeBin.binMode == binfile.binmode.music){
                    MessageBox.Show("Note: this is an experimental feature.");
                    }
                activeBin.SaveBin();
            }
        }

        private void openRDTarchivedfile_Click(object sender, EventArgs e)          //OPEN RDT ARCHIVED FILE
        {
            treeNodesAndArchivedFiles[FileTree.SelectedNode].OpenRDTSubfileInEditor(true);
        }

        private void renameRDTarchfile_Click(object sender, EventArgs e)         //RENAME RDT ARCHIVED FILE
        {
            if (FileTree.SelectedNode != null && FileTree.SelectedNode.Parent != null)
            {
                FileTree.LabelEdit = true;

                if (!FileTree.SelectedNode.IsEditing)
                {
                    FileTree.SelectedNode.BeginEdit();
                }
            }
            else
            {
                MessageBox.Show("You cannot rename that file.", "Invalid selection");
            }
        }

        private void rdtSpriteToPNGs_Click(object sender, EventArgs e)          //EXPORT PNGS FROM RDT ARCHIVED FILE
        {
            archivedfile selectedFile = treeNodesAndArchivedFiles[FileTree.SelectedNode];

            ExportRdtSpriteAsPNG(selectedFile, true);
        }

        private void deleteRDTarchivedfile_Click(object sender, EventArgs e)        //DELETE RDT ARCHIVED FILE
        {
            TreeNode node = FileTree.SelectedNode;

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete the file \"" + node.Text + "\"?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Yes)
            {
                activeRdt.archivedfiles.Remove(treeNodesAndArchivedFiles[node]);
                treeNodesAndArchivedFiles.Remove(node);
                node.Remove();
            }
        }

        public void ExportRdtSpriteAsPNG(archivedfile selectedFile, bool askDir)
        {

            bool OpenedSpriteEditorJustForThis = false;

            if (selectedFile.spriteEditor == null)
            {
                selectedFile.OpenRDTSubfileInEditor(false);
                OpenedSpriteEditorJustForThis = true;
            }

            if (selectedFile.spriteEditor == null || selectedFile.rdtSubfileDataList[0].filebytes == null || selectedFile.rdtSubfileDataList[0].filebytes.Length == 0)
            {
                return;
            }


            string filename = Path.GetFileName(selectedFile.filename);

            if (askDir)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.FileName = Path.GetFileName(selectedFile.filename);

                saveFileDialog1.Title = "Save sprite images";
                saveFileDialog1.CheckPathExists = true;
                saveFileDialog1.DefaultExt = ".png";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    filename = saveFileDialog1.FileName;
                }
            }

            //get total dimensions of the whole sprite once movement is taken into account

            int width = 0;
            int height = 0;

            for(int i = 0; i < selectedFile.spriteEditor.images.Count; i++)
            {
                rdtSubfileData image = selectedFile.spriteEditor.images[i];

                if (image.image == null){
                    image.LoadImage(GetPalette(selectedFile.spriteEditor.palettes[i].filebytes, 1, selectedFile.RDTSpriteBPP));
                }

                if (image.offsetX + image.image.Width > width) { width = image.offsetX + image.image.Width; }
                if (image.offsetY + image.image.Height > height) { height = image.offsetY + image.image.Height; }
            }

            //create a blank template background of that size

            Bitmap bg = new Bitmap(width, height);

            for (int y = 0; y < bg.Height; y++)
            {
                for (int x = 0; x < bg.Width; x++)
                {
                    bg.SetPixel(x, y, selectedFile.RDTSpriteAlphaColour);
                }
            }

            //now adjust each image according to its offset and export it

            for (int i = 0; i < selectedFile.spriteEditor.images.Count; i++)
            {
                Bitmap frame = new Bitmap(bg);

                int offsetX = selectedFile.spriteEditor.images[i].offsetX;
                int offsetY = selectedFile.spriteEditor.images[i].offsetY;

                for (int y = 0; y < selectedFile.spriteEditor.images[i].image.Height; y++)
                {
                    for (int x = 0; x < selectedFile.spriteEditor.images[i].image.Width; x++)
                    {
                        frame.SetPixel(x + offsetX, y + offsetY, selectedFile.spriteEditor.images[i].image.GetPixel(x, y));
                    }
                }
                frame.Save(filename.Replace(".png", "") + "_" + (i + 1) + ".png");
            }

            if (OpenedSpriteEditorJustForThis)
            {
                selectedFile.spriteEditor.Close();
            }
        }

        public Color[] GetPalette(Byte[] input, int offset, byte bpp)
        {

            Color[] palette = new Color[0];

            if (bpp == 4)
            {
                palette = new Color[16];

                for (int i = 0; i < 16; i++)
                {
                    palette[i] = ABGR1555_to_RGBA32(BitConverter.ToUInt16(input, offset));
                    offset += 2;
                }
            }
            else if (bpp == 8)
            {
                palette = new Color[256];

                for (int i = 0; i < 256; i++)
                {
                    palette[i] = ABGR1555_to_RGBA32(BitConverter.ToUInt16(input, offset));
                    offset += 2;
                }
            }
            else if (bpp == 3)
            {
                palette = new Color[8];

                Console.WriteLine("3BPP");

                for (int i = 0; i < 8; i++)
                {
                    palette[i] = ABGR1555_to_RGBA32(BitConverter.ToUInt16(input, offset));
                    offset += 2;
                }
            }
            else if (bpp == 5)
            {
                palette = new Color[32];

                Console.WriteLine("5BPP");

                for (int i = 0; i < 32; i++)
                {
                    palette[i] = ABGR1555_to_RGBA32(BitConverter.ToUInt16(input, offset));
                    offset += 2;
                }
            }
            return palette;
        }

        private void FileTree_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                if (e.Label.Length > 0)
                {
                    if (e.Label.IndexOfAny(new char[] { '@', ',', '!' }) == -1)
                    {
                        // Stop editing without canceling the label change.
                        e.Node.EndEdit(false);

                        if (treeNodesAndArchivedFiles.ContainsKey(e.Node))  //if file
                        {
                            string[] nodeNameBits = e.Node.FullPath.Split('\\');

                            string newNodeName = "";

                            nodeNameBits[nodeNameBits.Length - 1] = e.Label;

                            for (int i = 1; i < nodeNameBits.Length; i++)
                            {
                                newNodeName += "/" + nodeNameBits[i];
                            }

                            treeNodesAndArchivedFiles[e.Node].filename = newNodeName;

                        }
                        else    //if folder
                        {
                            e.Node.Text = e.Label;

                            foreach (TreeNode child in e.Node.Nodes)
                            {
                                if (treeNodesAndArchivedFiles.ContainsKey(child))   //if child is a file
                                {
                                    string childfilename = treeNodesAndArchivedFiles[child].filename;
                                    string replacementPathSection = e.Node.FullPath.Replace(FileTree.Nodes[0].Text, "").Replace('\\', '/');
                                    Console.WriteLine(replacementPathSection);
                                    Console.WriteLine(childfilename);
                                    childfilename = childfilename.Replace(Path.GetDirectoryName(childfilename).Replace('\\', '/'), replacementPathSection);
                                    treeNodesAndArchivedFiles[child].filename = childfilename;

                                    Console.WriteLine("new child filename: " + childfilename);
                                }
                            }
                        }
                    }
                    else
                    {
                        /* Cancel the label edit action, inform the user, and
                           place the node in edit mode again. */
                        e.CancelEdit = true;
                        MessageBox.Show("Invalid name.\n" +
                           "The invalid characters are: '@', ',', '!'",
                           "Error");
                        e.Node.BeginEdit();
                    }
                }
                else
                {
                    /* Cancel the label edit action, inform the user, and
                       place the node in edit mode again. */
                    e.CancelEdit = true;
                    MessageBox.Show("Name cannot be blank",
                       "Error");
                    e.Node.BeginEdit();
                }
            }
        }

        public Bitmap NBFCtoImage(Byte[] input, int offset, int width, int height, Color[] palette, byte bpp)  //palettes aren't always the same length, this function is designed for the image in the downloadable newsletter
        {
            Bitmap bm = new Bitmap(width, height);

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
            else if (bpp == 5)
            {
                Console.WriteLine("5BPP image not yet handled");

                bm = new Bitmap(width, height);

                int currentBitInByte = 0;
                Color c;

                int stage = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (x >= width) //check whether or not the line ended midway through the byte
                        {
                            //curOffset++;
                            continue;
                        }

                        int temp = 0;
                        switch (stage) //not the best way of doing it, but the other method wasn't working. (and this also doesn't work)
                        {
                            case 0:
                                //5 bits
                                c = palette[(input[curOffset]) >> 3];
                                bm.SetPixel(x, y, c);
                                break;
                            case 1:
                                //5 bits
                                temp = (input[curOffset]) & 0x07;
                                curOffset++;
                                temp |= (input[curOffset]) >> 3;
                                c = palette[temp];
                                bm.SetPixel(x, y, c);
                                break;
                            case 2:
                                //5 bits
                                temp = (input[curOffset] >> 1) & 0x1F;
                                c = palette[temp];
                                bm.SetPixel(x, y, c);
                                break;
                            case 3:
                                //5 bits
                                temp = (input[curOffset] & 0x01);
                                curOffset++;
                                temp |= (input[curOffset] >> 3);
                                c = palette[temp];
                                bm.SetPixel(x, y, c);
                                break;
                            case 4:
                                //5 bits
                                temp = (input[curOffset] & 0x0F);

                                curOffset++;
                                temp |= (input[curOffset] >> 3) & 0x10;
                                c = palette[temp];
                                bm.SetPixel(x, y, c);
                                break;
                            case 5:
                                //5 bits
                                temp = (input[curOffset] >> 2) & 0x1F;
                                c = palette[temp];
                                bm.SetPixel(x, y, c);
                                break;
                            case 6:
                                //5 bits
                                temp = (input[curOffset] & 0x03);
                                curOffset++;
                                temp |= (input[curOffset] >> 3) & 0x1C;
                                c = palette[temp];
                                bm.SetPixel(x, y, c);
                                break;
                            case 7:
                                //5 bits
                                temp = input[curOffset] & 0x1F;
                                curOffset++;
                                c = palette[temp];
                                bm.SetPixel(x, y, c);

                                if ((curOffset - 8) % 5 != 0)
                                {
                                    Console.WriteLine("out of sync");
                                }
                                break;
                        }

                        stage++;

                        if (stage > 7)
                        {
                            stage = 0;
                        }
                    }
                }
            }

            return bm;
        }




        public Byte[] ImageToNBFC(Bitmap input, byte BPP, Color[] palette) {

            Byte[] output = new byte[0];

            int offset = 0;

            if (BPP == 4)
            {
                output = new byte[(input.Width * input.Height) / 2];

            }
            else if (BPP == 8)
            {
                output = new byte[(input.Width * input.Height)];
            }


            Color newPixel;

            if (BPP == 4)
            {
                for (int y = 0; y < input.Height; y++)
                {
                    for (int x = 0; x < input.Width; x++)
                    {
                        newPixel = input.GetPixel(x, y);
                        output[offset] = (byte)(output[offset] | (byte)FindIndexOfColorInPalette(palette, Color.FromArgb(newPixel.A, newPixel.R & 0xF8, newPixel.G & 0xF8, newPixel.B & 0xF8)));
                        if (x < input.Width - 1)
                        {
                            x++;
                            newPixel = input.GetPixel(x, y);
                            output[offset] = (byte)(output[offset] | (byte)(FindIndexOfColorInPalette(palette, Color.FromArgb(newPixel.A, newPixel.R & 0xF8, newPixel.G & 0xF8, newPixel.B & 0xF8)) << 4));
                        }

                        offset++;
                    }
                }
            }
            else
            {
                for (int y = 0; y < input.Height; y++)
                {
                    for (int x = 0; x < input.Width; x++)
                    {
                        newPixel = input.GetPixel(x, y);
                        output[offset] = (byte)FindIndexOfColorInPalette(palette, Color.FromArgb(newPixel.A, newPixel.R & 0xF8, newPixel.G & 0xF8, newPixel.B & 0xF8));
                        offset++;
                    }
                }
            }

            return output;
        }

        public int FindIndexOfColorInPalette(Color[] p, Color c)
        {
            for (int i = 0; i < p.Length; i++)
            {
                if ((c.R & 0xF8) == (p[i].R & 0xF8) && (c.G & 0xF8) == p[i].G && (c.B & 0xF8) == (p[i].B & 0xF8))
                {
                    return i;
                }
            }
            Console.WriteLine("hmm, not found");
            Console.WriteLine("Image R " + c.R);
            Console.WriteLine("Image G " + c.G);
            Console.WriteLine("Image B " + c.B);

            return 0; //if it wasn't found
        }

        private void massRDTExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mode == Mode.Rdt)
            {
                MessageBox.Show("Files will be exported to the same directory as EPFExplorer's exe.");
                foreach (archivedfile f in activeRdt.archivedfiles)
                {
                    ExportRdtSpriteAsPNG(f, false);
                }
            }
        }

        public class GifFrameExtraInfo{
            public int minX = 99999;
            public int minY = 99999;
            public int maxX = 0;
            public int maxY = 0;
        }

        private void replaceToolStripMenuItem1_Click(object sender, EventArgs e)    //REPLACE RDT SPRITE
        {
            TreeNode node = FileTree.SelectedNode;
            archivedfile fileToBeReplaced = treeNodesAndArchivedFiles[FileTree.SelectedNode];

            string oldName = fileToBeReplaced.filename;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select sprite file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.Filter = "1PP sprite data, GIF image (*.sprite, .gif)|*.sprite;*.gif";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                switch (Path.GetExtension(openFileDialog1.FileName))
                {
                    case ".sprite":

                        if (fileToBeReplaced.spriteEditor != null)
                            {
                            fileToBeReplaced.spriteEditor.Close();
                            }
                        rdtfile newSprite = new rdtfile();

                        newSprite.arcname = Path.GetFileName(openFileDialog1.FileName);
                        newSprite.filename = openFileDialog1.FileName;
                        newSprite.filebytes = File.ReadAllBytes(openFileDialog1.FileName);
                        newSprite.form1 = this;
                        newSprite.ReadRdt();
                        newSprite.archivedfiles[0].filename = oldName;
                        treeNodesAndArchivedFiles.Remove(node);
                        activeRdt.archivedfiles[activeRdt.archivedfiles.IndexOf(fileToBeReplaced)] = newSprite.archivedfiles[0];
                        treeNodesAndArchivedFiles.Add(node, activeRdt.archivedfiles[activeRdt.archivedfiles.IndexOf(newSprite.archivedfiles[0])]);
                        break;
                    case ".gif":

                        if (fileToBeReplaced.spriteEditor != null)
                        {
                            fileToBeReplaced.spriteEditor.Close();
                        }

                        if (fileToBeReplaced.rdtSubfileDataList.Count == 0){
                            fileToBeReplaced.ReadFile();
                            }

                        Image animatedImage = Image.FromFile(openFileDialog1.FileName);

                        List<Bitmap> framesAsBitmaps = new List<Bitmap>();

                        PropertyItem metadata = animatedImage.PropertyItems[0];
                        List<int> frameDurations = new List<int>();

                        for (int i = 0; i < animatedImage.GetFrameCount(FrameDimension.Time); i++)
                        {
                            animatedImage.SelectActiveFrame(FrameDimension.Time, i);
                            
                            MemoryStream strm = new MemoryStream();
                            animatedImage.Save(strm, ImageFormat.Png);
                            framesAsBitmaps.Add((Bitmap)Image.FromStream(strm));
                            frameDurations.Add(BitConverter.ToInt32(metadata.Value, i * 4));
                        }

                        List<Color> colors = new List<Color>();

                        foreach (Bitmap frame in framesAsBitmaps)
                            {
                            for (int y = 0; y < frame.Height; y++)
                                {
                                for (int x = 0; x < frame.Width; x++)
                                    {
                                    Color c = frame.GetPixel(x, y);
                                    c = Color.FromArgb(0xFF, c.R & 0xF8, c.G & 0xF8, c.B & 0xF8);

                                    if (!colors.Contains(c))
                                        {
                                        colors.Add(c);
                                        }
                                    }
                                }
                            }

                        if (fileToBeReplaced.RDTSpriteBPP == 4 && colors.Count > 16 && colors.Count <= 256){
                            MessageBox.Show("This gif has " + colors.Count + " colours, which exceeds the maximum colour count for a 4BPP sprite (16). If you really want to import it, change this sprite's BPP to 8, then try again.");
                            return;
                        }

                        if (colors.Count > 256){
                            MessageBox.Show("Too many colours! You had: " + colors.Count + " colours. The maximum allowed is 256.");
                            return;
                        }

                        //ask the user to choose the alpha colour

                        ChooseAlphaColourWindow chooseAlphaColourWindow = new ChooseAlphaColourWindow();
                        chooseAlphaColourWindow.form1 = this;
                        chooseAlphaColourWindow.colors = colors;
                        chooseAlphaColourWindow.Init();
                        DialogResult result = chooseAlphaColourWindow.ShowDialog();

                        Color AlphaColor = colors[alphaColorIndexForGifImport];

                      
                        //get the overall extreme pixels of all the images that isn't the alpha colour
                        //these are going to be the boundaries from which offsets are measured etc

                        //this is so that we can crop the image and do the thing with the offsets

                        int minX = 99999;
                        int minY = 99999;
                        int maxX = 0;
                        int maxY = 0;

                        List<GifFrameExtraInfo> extraInfo = new List<GifFrameExtraInfo>();

                        for(int i = 0; i < framesAsBitmaps.Count; i++)
                            {
                            extraInfo.Add(new GifFrameExtraInfo());
                            }

                        for(int i = 0; i < framesAsBitmaps.Count; i++)
                            {
                            for (int y = 0; y < framesAsBitmaps[i].Height; y++)
                                {
                                for (int x = 0; x < framesAsBitmaps[i].Width; x++)
                                    {
                                    Color c = framesAsBitmaps[i].GetPixel(x, y);

                                    if (!((c.R & 0xF8) == (AlphaColor.R & 0xF8) && (c.G & 0xF8) == (AlphaColor.G & 0xF8) && (c.B & 0xF8) == (AlphaColor.B & 0xF8)))
                                        {
                                        if (x < minX) { minX = x; }
                                        if (y < minY) { minY = y; }
                                        if (x > maxX) { maxX = x; }
                                        if (y > maxY) { maxY = y; }

                                        if (x < extraInfo[i].minX) { extraInfo[i].minX = x; }
                                        if (y < extraInfo[i].minY) { extraInfo[i].minY = y; }
                                        if (x > extraInfo[i].maxX) { extraInfo[i].maxX = x; }
                                        if (y > extraInfo[i].maxY) { extraInfo[i].maxY = y; }
                                        }
                                    }
                                }
                            }

                        //and now actually cut out and add the images

                        fileToBeReplaced.RDTSpriteAlphaColour = AlphaColor;
                        fileToBeReplaced.RDTSpriteNumFrames = (ushort)framesAsBitmaps.Count;
                        fileToBeReplaced.RDTSpriteWidth = (ushort)(maxX - minX);
                        fileToBeReplaced.RDTSpriteHeight = (ushort)(maxY - minY);
                        fileToBeReplaced.RDTSpriteFrameDurations.Clear();

                        foreach (rdtSubfileData.setting s in fileToBeReplaced.rdtSubfileDataList[1].spriteSettings)
                            {
                            switch (s.name)
                                {
                                case "center":
                                    s.X = framesAsBitmaps[0].Width / 2;
                                    s.Y = framesAsBitmaps[0].Height / 2;
                                    break;
                                case "bounds":
                                    s.X = minX;
                                    s.Y = minY;
                                    s.X2 = minX+(maxX - minX)+1;
                                    s.Y2 = minY+(maxY - minY)+1;
                                    break;
                                }
                            }

                        for (int i = fileToBeReplaced.rdtSubfileDataList.Count - 1; i >= 0; i--)
                            {
                            if (fileToBeReplaced.rdtSubfileDataList[i].graphicsType == "image" || fileToBeReplaced.rdtSubfileDataList[i].graphicsType == "palette")
                                {
                                fileToBeReplaced.rdtSubfileDataList.RemoveAt(i);
                                }
                            }

                        for (int i = 0; i < framesAsBitmaps.Count; i++)
                            {
                            fileToBeReplaced.RDTSpriteFrameDurations.Add((ushort)(frameDurations[i] * 10));
                            GifFrameExtraInfo sizes = extraInfo[i];

                            rdtSubfileData newGfx = new rdtSubfileData();
                            newGfx.graphicsType = "image";
                            newGfx.subfileType = 0x04;
                            newGfx.offsetX = (short)((sizes.minX)+1);
                            newGfx.offsetY = (short)((sizes.minY)+1);

                            newGfx.image = new Bitmap((sizes.maxX - sizes.minX)+1, (sizes.maxY - sizes.minY)+1);
                            
                            for (int y = 0; y < newGfx.image.Height; y++)
                                {
                                for (int x = 0; x < newGfx.image.Width; x++)
                                    {
                                    newGfx.image.SetPixel(x,y,framesAsBitmaps[i].GetPixel(sizes.minX+x,sizes.minY+y));
                                    }
                                }

                            rdtSubfileData newPal = new rdtSubfileData();
                            newPal.graphicsType = "palette";
                            newPal.subfileType = 0x04;
                            newPal.filebytes = new byte[32];    //I think this only needs to be a dummy palette
                            fileToBeReplaced.rdtSubfileDataList.Add(newPal);
                            fileToBeReplaced.rdtSubfileDataList.Add(newGfx);
                            }
                        if (fileToBeReplaced.spriteEditor != null)
                            {
                            fileToBeReplaced.spriteEditor.Close();
                            }
                        fileToBeReplaced.spriteEditor = null;
                        break;
                    }
                }
        }

        private void rawDataToolStripMenuItem_Click(object sender, EventArgs e) //EXPORT RDT SPRITE RAW DATA
        {
            TreeNode selectedNode = FileTree.SelectedNode;
            int indexOfArchivedFileInRdt = activeRdt.archivedfiles.IndexOf(treeNodesAndArchivedFiles[selectedNode]);
            archivedfile targetfile = treeNodesAndArchivedFiles[selectedNode];

            archivedfile BackupArchivedfile = new archivedfile(targetfile);

            rdtfile forExport = new rdtfile();
            forExport.form1 = this;
            forExport.archivedfiles = new List<archivedfile>();
            forExport.archivedfiles.Add(targetfile);
            forExport.is_only_sprite_container = true;
            forExport.RebuildRDT();

            targetfile = BackupArchivedfile;
            
        }

        private void deleteArchivedFile_Click(object sender, EventArgs e)
        {
            TreeNode node = FileTree.SelectedNode;

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete the file \"" + node.Text + "\"?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Yes)
            {
                activeArc.archivedfiles.Remove(treeNodesAndArchivedFiles[node]);
                treeNodesAndArchivedFiles.Remove(node);
                node.Remove();
            }
        }

        private void MPBTSBEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MPB_TSB_EditorForm mpb_tsb_editor = new MPB_TSB_EditorForm();
            mpb_tsb_editor.form1 = this;
            mpb_tsb_editor.Show();
        }

        private void exportToolStripMenuItem3_Click(object sender, EventArgs e) //EXPORT FROM BIN FILE
        {
            if (treeNodesAndArchivedFiles[FileTree.SelectedNode].filename.Contains(".sampleCollection"))
                {
                activeBin.ExportMusicSamples();
                return;
                }

            if (activeBin.binMode == binfile.binmode.sfx)
                {
                treeNodesAndArchivedFiles[FileTree.SelectedNode].linkedSfx.Export();
                }
            else
                {
                treeNodesAndArchivedFiles[FileTree.SelectedNode].linkedXm.Export();
                }
        }

        private void replaceXM_Click(object sender, EventArgs e)
        {
            archivedfile selectedFile = treeNodesAndArchivedFiles[FileTree.SelectedNode];

            if (activeBin.binMode == binfile.binmode.music)
            {
                MessageBox.Show("This feature is experimental and may produce unexpected results in-game.");

                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                openFileDialog1.Title = "Replace "+ Path.GetFileName(selectedFile.filename);
                openFileDialog1.CheckFileExists = true;
                openFileDialog1.CheckPathExists = true;
                openFileDialog1.Filter = "Extended Module (*.xm)|*.xm";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                    selectedFile.linkedXm.Replace_With_New_XM(openFileDialog1.FileName);
                    }
            }
            else
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                openFileDialog1.Title = "Replace " + Path.GetFileName(selectedFile.filename);
                openFileDialog1.CheckFileExists = true;
                openFileDialog1.CheckPathExists = true;
                openFileDialog1.Filter = "ADPCM WAV (*.wav)|*.wav";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    byte[] wavFile = File.ReadAllBytes(openFileDialog1.FileName);

                    //look for fmt chunk
                    int i = 0;
                    while (!(wavFile[i] == (byte)'f' && wavFile[i + 1] == (byte)'m' && wavFile[i + 2] == (byte)'t' && wavFile[i + 3] == 0x20))
                    {
                        i++;
                    }

                    i += 8;

                    if (wavFile[i] != 0x11) //if not ADPCM
                    {
                        MessageBox.Show("Only IMA ADPCM .wav files are allowed!\nYou can export these from Audacity by choosing 'other uncompressed files' in the dropdown menu when saving the WAV file. \n(If it's not there, you may need to install FFMPEG.)");
                        return;
                    }

                    i += 4;

                    selectedFile.linkedSfx.samplerate = BitConverter.ToUInt32(wavFile, i);

                    //look for data chunk
                    while (!(wavFile[i] == (byte)'d' && wavFile[i + 1] == (byte)'a' && wavFile[i + 2] == (byte)'t' && wavFile[i + 3] == (byte)'a'))
                    {
                        i++;
                    }

                    i += 0x08;

                    selectedFile.linkedSfx.filebytes = new byte[(wavFile.Length - i) - 0x6A];

                    int startOfData = i;

                    for (i = startOfData; i < wavFile.Length - 0x6A; i++)
                    {
                        selectedFile.linkedSfx.filebytes[i - startOfData] = wavFile[i];
                    }

                    selectedFile.linkedSfx.isPCM = false;

                    selectedFile.filename = Path.GetFileName(openFileDialog1.FileName) + " (" + Path.GetFileName(activeBin.filename) + selectedFile.linkedSfx.indexInBin + ")";
                
                    //THE GAME ASSUMES A BLOCK ALIGNMENT OF 0xFFFF. If your imported audio file has anything else, it will sound messed up. I don't know how one would go about making WAV files with the correct block aligment. Audacity seems to decide it automatically.
                }
            }
        }

        private void randomizeRDTSpritesToolStripMenuItem_Click(object sender, EventArgs e)
        {
        if (mode == Mode.Rdt)
            {
                DialogResult result = MessageBox.Show("Randomize the RDT sprites on next save?", "Randomize RDT sprites?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                    {
                    activeRdt.randomizeOnNextSave = true;
                    }
                else if (result == DialogResult.No)
                    {
                    activeRdt.randomizeOnNextSave = false;
                }
            }
        else
            {
                MessageBox.Show("That feature is only for RDT files!", "RDT file required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void massXMExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            if (activeBin.binMode == binfile.binmode.music)
                {
                saveFileDialog1.FileName = "Save XM files here";
                }
            else
                {
                saveFileDialog1.FileName = "Save wav files here";
                }
            
            saveFileDialog1.Title = "Choose folder";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.Filter = "Directory |directory";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (activeBin.binMode == binfile.binmode.music)
                    {
                    foreach (TreeNode node in treeNodesAndArchivedFiles.Keys)
                        {
                        if (treeNodesAndArchivedFiles[node].linkedXm == null)
                            {
                            continue;
                            }

                        treeNodesAndArchivedFiles[node].linkedXm.customExportFolder = Path.GetDirectoryName(saveFileDialog1.FileName);
                        treeNodesAndArchivedFiles[node].linkedXm.Export();
                        }
                    }
                else
                    {
                    foreach (TreeNode node in treeNodesAndArchivedFiles.Keys)
                    {
                        if (treeNodesAndArchivedFiles[node].linkedSfx == null)
                        {
                            continue;
                        }

                        treeNodesAndArchivedFiles[node].linkedSfx.customExportFolder = Path.GetDirectoryName(saveFileDialog1.FileName);
                        treeNodesAndArchivedFiles[node].linkedSfx.Export();
                    }
                }
            }
        }

        private void missionEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MissionEditor missionEditor = new MissionEditor();
            missionEditor.form1 = this;
            missionEditor.LoadFormControls();
            missionEditor.Show();
        }

        private void exportAsGIFAnimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            archivedfile selectedFile = treeNodesAndArchivedFiles[FileTree.SelectedNode];
            selectedFile.OpenRDTSubfileInEditor(false);

            if (selectedFile.spriteEditor.images.Count == 0)
                {
                return;
                }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.FileName = Path.GetFileName(selectedFile.filename);

            saveFileDialog1.Title = "Save "+Path.GetFileName(selectedFile.filename)+" as gif file";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.Filter = "GIF animation (*.gif)|*.gif|All files (*.*)|*.*";


            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                NGif.AnimatedGifEncoder enc = new NGif.AnimatedGifEncoder();
                enc.Start(saveFileDialog1.FileName);
                enc.SetRepeat(0);
                enc.SetQuality(1); //best quality
                enc.SetTransparent(selectedFile.RDTSpriteAlphaColour);
                enc.perfectColours = true;

                int width = 0;
                int height = 0;

                foreach (rdtSubfileData image in selectedFile.spriteEditor.images)
                    {
                    if (image.offsetX + image.image.Width > width) { width = image.offsetX + image.image.Width; }
                    if (image.offsetY + image.image.Height > height) { height = image.offsetY + image.image.Height; }
                    }

                //create a blank template background

                Bitmap bg = new Bitmap(width,height);

                for (int y = 0; y < bg.Height; y++)
                {
                    for (int x = 0; x < bg.Width; x++)
                    {
                        bg.SetPixel(x,y,selectedFile.RDTSpriteAlphaColour);
                    }
                }

                //now adjust each image according to the offset and display it

                for (int i = 0; i < selectedFile.spriteEditor.images.Count; i++)
                {
                    Bitmap frame = new Bitmap(bg);

                    int offsetX = selectedFile.spriteEditor.images[i].offsetX;
                    int offsetY = selectedFile.spriteEditor.images[i].offsetY;

                    for (int y = 0; y < selectedFile.spriteEditor.images[i].image.Height; y++)
                        {
                        for (int x = 0; x < selectedFile.spriteEditor.images[i].image.Width; x++)
                            {
                            frame.SetPixel(x + offsetX,y + offsetY,selectedFile.spriteEditor.images[i].image.GetPixel(x,y));
                            }
                        }
                    frame.Save("Test");

                    enc.AddFrame(frame);
                    enc.SetDelay((int)Math.Round((float)(selectedFile.RDTSpriteFrameDurations[i])));
                }

                enc.Finish();
            }

            selectedFile.spriteEditor.Close();
            selectedFile.spriteEditor = null;
        }

        private void RDTsettingVersionCP_Click(object sender, EventArgs e)
        {
            activeRdt.ben10mode = false;
        }

        private void RDTSettingversionBen10_Click(object sender, EventArgs e)
        {
            activeRdt.ben10mode = true;
        }

        public List<Color> GetPaletteForImage(Bitmap image) {

            List<Color> output = new List<Color>();

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color c = image.GetPixel(x, y);
                    c = Color.FromArgb(0xFF, c.R & 0xF8, c.G & 0xF8, c.B & 0xF8);

                    if (!output.Contains(c))
                    {
                        output.Add(c);
                    }
                }
            }

            return output;
        }


        //==============================================================================
        //==============================================================================
        //||                                                                          ||
        //||                         Debug utilities                                  ||
        //||                                                                          ||
        //==============================================================================
        //==============================================================================

        private void button1_Click(object sender, EventArgs e)
        {
            //TestStringsAgainstHashes();

            //StringCSVToOneLine();
        }

        public void TestStringsAgainstHashes()
        {
            string[] strings_to_test = new string[] { };

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select arc file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.Filter = "arc (*.arc)|*.arc";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (BinaryReader reader = new BinaryReader(File.Open(openFileDialog1.FileName, FileMode.Open)))
                {
                    uint count = reader.ReadUInt32();

                    uint[] hashes = new uint[count];

                    for (int i = 0; i < count; i++)
                    {
                        hashes[i] = reader.ReadUInt32();
                        reader.BaseStream.Position += 0x08;
                    }

                    foreach (string s in strings_to_test)
                    {
                        foreach (uint hash in hashes)
                        {
                            if (CalculateHash(s) == hash)
                            {
                                Console.WriteLine("hooray!");
                            }
                        }
                    }
                }
            }
        }

        private void StringCSVToOneLine()   //this is what was used to turn the csv of filename etc strings from ghidra into an array of strings
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.Filter = "csv (*.csv)|*.csv";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] strings = File.ReadAllLines(openFileDialog1.FileName);

                string output = "";

                foreach (string s in strings)
                {
                    if (s.Contains("/") && !s.Contains("?") && s.Length > 2 && !s.Contains(":") && !s.Contains("|") && !s.Contains("[") && !s.Contains("]") && !s.Contains("%") && !s.Contains(":") && !s.Contains("<") && !s.Contains("(") && !s.Contains(";") && s[0] != 'u')
                    {
                        if (s[2] == '/')
                        {
                            output += "\"" + s.Substring(2, s.Length - 3) + ", ";
                        }
                        else
                        {
                            output += "\"/" + s.Substring(2, s.Length - 3) + ", ";
                        }
                    }
                }
                File.WriteAllText(openFileDialog1.FileName + "output", output);
            }
        }

        public List<string> GetFilenamePermutations(string input)   //get a bunch of variations on a filename
        {
            Console.WriteLine("NEVER USE THIS FOR LARGE ARRAYS OF FILENAMES");

            List<string> output = new List<string>();

            bool testedWithScripts = false;
            bool testedWithChunks = false;
            bool testedWithLuc = false;
            bool testedWithLua = false;

            string s = input;

            anotherVariation:

            output.Add(s);          //normal filename with path

            if (s[0] == '/')
            {
                output.Add(s.Substring(1, s.Length - 1));  //normal with path, but without initial slash
            }

            output.Add(Path.GetFileName(s));    //normal with only filename

            output.Add("/" + Path.GetFileName(s));  //normal with only filename, but with initial slash

            output.Add(s.ToLower());          //lower case filename with path

            output.Add("/" + Path.GetFileName(s.ToLower()));  //normal with only filename, but with initial slash

            //now decide if we need to go round again with a new variation

            if (input.Contains("chunks") && !testedWithScripts)
            {
                testedWithChunks = true;
                s = input.Replace("chunks", "scripts");
                s = input.Replace("Chunks", "Scripts");
                goto anotherVariation;
            }

            if (input.Contains("scripts") && !testedWithChunks)
            {
                testedWithChunks = true;
                s = input.Replace("scripts", "chunks");
                s = input.Replace("Scripts", "Chunks");
                goto anotherVariation;
            }

            if (input.Contains(".lua") && !testedWithLuc)
            {
                testedWithLuc = true;
                s = input.Replace(".lua", ".luc");
                goto anotherVariation;
            }

            if (input.Contains(".luc") && !testedWithLua)
            {
                testedWithLua = true;
                s = input.Replace(".luc", ".lua");
                goto anotherVariation;
            }

            // if (!input.Contains(".") && IndexInExtensionsArray < extensions.Count - 1)
            //     {
            //      s = input + extensions[IndexInExtensionsArray];
            //    IndexInExtensionsArray++;
            //    goto anotherVariation;
            //   }

            return output;
        }

        private void addSoundFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode parentNode = FileTree.SelectedNode;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select new audio clip file";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "IMA ADPCM WAV files (*.wav*)|*.wav*|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in openFileDialog1.FileNames)
                {
                    byte[] wavFile = File.ReadAllBytes(filename);

                    //look for fmt chunk
                    int i = 0;
                    while (!(wavFile[i] == (byte)'f' && wavFile[i + 1] == (byte)'m' && wavFile[i + 2] == (byte)'t' && wavFile[i + 3] == 0x20))
                    {
                        i++;
                    }

                    i += 8;

                    if (wavFile[i] != 0x11) //if not ADPCM
                    {
                        MessageBox.Show("Only IMA ADPCM .wav files are allowed!\nYou can export these from Audacity by choosing 'other uncompressed files' in the dropdown menu when saving the WAV file. \n(If it's not there, you may need to install FFMPEG.)");
                        continue;
                    }

                    i += 4;

                    sfxfile newSfxFile = new sfxfile();
                    newSfxFile.samplerate = BitConverter.ToUInt32(wavFile, i);

                    //look for data chunk
                    while (!(wavFile[i] == (byte)'d' && wavFile[i + 1] == (byte)'a' && wavFile[i + 2] == (byte)'t' && wavFile[i + 3] == (byte)'a'))
                    {
                        i++;
                    }

                    i += 0x08;

                    newSfxFile.filebytes = new byte[(wavFile.Length - i) - 0x6A];
                   
                    int startOfData = i;

                    for (i = startOfData; i < wavFile.Length - 0x6A; i++)
                    {
                        newSfxFile.filebytes[i - startOfData] = wavFile[i];
                    }

                    archivedfile newArchivedFile = new archivedfile();

                    newSfxFile.isPCM = false;
                    newSfxFile.indexInBin = activeBin.sfxfiles.Count;
                    newSfxFile.parentbinfile = activeBin;

                    newArchivedFile.linkedSfx = newSfxFile;
                    newArchivedFile.filename = Path.GetFileName(filename) + " (" + Path.GetFileName(activeBin.filename) + newSfxFile.indexInBin + ")";

                    activeBin.sfxfiles.Add(newSfxFile);

                    newArchivedFile.treeNode = parentNode.Nodes.Add(newArchivedFile.filename);
                    newArchivedFile.treeNode.ImageIndex = 1;
                    newArchivedFile.treeNode.SelectedImageIndex = 1;

                    treeNodesAndArchivedFiles.Add(newArchivedFile.treeNode, newArchivedFile);
                }
            }
        }
    }
}