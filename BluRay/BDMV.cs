using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using Utils;

namespace BluRay
{
    // http://git.videolan.org/?p=libbluray.git;a=tree;f=src/libbluray;hb=HEAD

    [DefaultPropertyAttribute("BDMV")]
    public class Bdmv
    {
        string header;
        [CategoryAttribute("Header"), DescriptionAttribute("Provider")]
        public string Header
        {
            get { return header; }
            set { header = value; }
        }
    }
    [DefaultPropertyAttribute("MovieObject")]
    public class MovieObject : Bdmv
    {
        private List<Command> commands = new List<Command>();
        public List<Command> Commands
        {
            get { return commands; }
            set { commands = value; }
        }
        public MovieObject(string fileName)
        {
            MOBJ_BDMV mo = new MOBJ_BDMV(fileName);
            BinaryFileReader FS = new BinaryFileReader(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, false);
            Header = FS.ReadString(8);
            if (Header.IndexOf("MOBJ") != 0)
                return;
            FS.Position = 0x28;
            int length = FS.ReadInteger();
            FS.Position = 0x30;
            int nbCommande = FS.ReadShort();
            for (int i = 0; i < nbCommande; i++)
            {
                commands.Add(new Command(FS));
            }
        }
    }
    public class Command
    {
        bool resume_intention_flag;
        bool menu_call_mask;
        bool title_search_mask;
        int taille;
        int etat;
        List<Instruction> lb = new List<Instruction>();
        public string Resume_intention
        {
            get { if (!resume_intention_flag) return "resume"; else return "suspend"; }
        }
        public string Menu_call
        {
            get { if (!menu_call_mask) return "enable"; else return "disable"; }
        }
        public string Title_search
        {
            get { if (!title_search_mask) return "enable"; else return "disable"; }
        }
        public List<Instruction> InstructionsList
        {
            get { return lb; }
            set { lb = value; }
        }
        public Command(BinaryFileReader FS)
        {
            int etat = FS.ReadByte();
            FS.ReadByte();
            resume_intention_flag = ((etat & 0x80) == 0x80);
            menu_call_mask = ((etat & 0x40) == 0x40);
            title_search_mask = ((etat & 0x20) == 0x20);
            int nbInstructions = FS.ReadShort();
            List<int> codes = new List<int>();
            for (int u = 0; u < nbInstructions; u++)
            {
                Instruction inst = new Instruction(FS.ReadBytes(12));
                lb.Add(inst);
            }
        }
    }
    public class Instruction
    {
        private byte[] instruction;
        private int firstInt;
        private int thirdInt;
        private int secondInt;
        public string LineCode
        {
            get
            {
                return Decode(instruction);
            }
        }
        public int Source
        {
            get { return secondInt; }
            set { secondInt = value; }
        }
        public int Destination
        {
            get { return thirdInt; }
            set { thirdInt = value; }
        }
        public int Opcode
        {
            get { return firstInt; }
            set { firstInt = value; }
        }
        private string Decode(byte[] inst)
        {
            firstInt = ConvertBuffer.ReadInteger(inst, 0);
            secondInt = ConvertBuffer.ReadInteger(inst, 4);
            thirdInt = ConvertBuffer.ReadInteger(inst, 8);
            string s = "";
            // Premier octet : code principal
            // Second octet : les bits 1 et 2 de inst[1] : opérandes immédiats
            // Derniers octets code secondaire
            // Second mot op1, premier bit à 1 : GPR sinon PSR
            // Troisième mot op2, premier bit à 1 : GPR sinon PSR
            string opcode = "";
            int nbOp = 0;
            bool firstOpIm = false;
            bool secondOpIm = false;
            if ((inst[1] & 0x80) == 0x80)
                firstOpIm = true;
            if ((inst[1] & 0x40) == 0x40)
                secondOpIm = true;
            string operande = "";
            long codeOp = firstInt & 0xFF0FFFFF;
            switch (codeOp)
            {
                #region Operation code
                case 0x00020000:
                    opcode = "Break";
                    break;
                case 0x01040000:
                    opcode = "Resume";
                    break;
                case 0x20010000:
                    opcode = "GoTo";
                    break;
                case 0x21000000:
                    opcode = "Jump Object ";
                    break;
                case 0x21010000:
                    opcode = "Jump Title ";
                    break;
                case 0x21020000:
                    opcode = "Call Object ";
                    break;
                case 0x22000000:
                    opcode = "Play PL ";
                    break;
                case 0x21030000:
                    opcode = "Call Title ";
                    break;
                case 0x3100000A:
                    opcode = "SetOutputMode ";
                    break;
                case 0x42410000:
                    opcode = "Play PL_PI ";
                    break;
                case 0x42020000:
                    opcode = "Play PL_MK ";
                    break;
                case 0x51000001:
                    opcode = "SetStream ()";
                    break;
                case 0x51000002:
                    opcode = "SetNVTimer ";
                    break;
                case 0x51000003:
                    opcode = "SetButtonPage ()";
                    break;
                case 0x51000006:
                    opcode = "SetSecondaryStream (0)";
                    break;
                case 0x5100000B:
                    opcode = "SetStreamSS ()";
                    break;
                #endregion
            }
            switch (inst[0])
            {
                #region Number operands
                case 0x00:
                    opcode = "NOP";
                    break;
                case 0x20:
                    nbOp = 1;
                    break;
                case 0x21:
                    nbOp = 1;
                    break;
                case 0x22:
                    nbOp = 1;
                    break;
                case 0x31://buttons
                    nbOp = 1;
                    break;
                case 0x40:
                    break;
                case 0x42:
                    nbOp = 2;
                    break;
                case 0x48://compare : 
                    opcode = compare[inst[2]];
                    nbOp = 2;
                    break;
                case 0x50:// set
                    opcode = ops[inst[3]];
                    nbOp = 2;
                    break;
                case 0x51:// Set system
                    nbOp = 1;
                    break;
                default:
                    break;
                #endregion
            }
            if (nbOp >= 1)
            {
                int a = ConvertBuffer.ReadShortInteger(inst, 6);
                string op1 = a.ToString();// entier sur 4 octets (ou 3 ?)
                if (firstOpIm)
                    operande += op1;
                else
                {
                    if ((inst[4] & 0x80) == 0x80)
                        operande += " PSR " + op1;
                    else
                        operande += " GPR " + op1;
                }
            }
            if (nbOp >= 2)
            {
                int b = ConvertBuffer.ReadInteger(inst, 0x08);
                string op2 = b.ToString();
                if (secondOpIm)
                    operande += ", " + op2;
                else
                {
                    if ((inst[4] & 0x80) == 0x80)
                        operande += " PSR " + op2;
                    else
                        operande += ", GPR " + op2;
                }
            }
            s = opcode + operande;
            return s;
        }
        public Instruction(byte[] buffer)
        {
            instruction = buffer;
        }
        public override string ToString()
        {
            return LineCode;
        }
        #region Data
        string[] ops = new string[] { "-", "Move", "Swap", "Add", "Sub", "Mul", "Div", "Mod", "Rnd", "And", "Or", "XOr", "Bit Set", "Bit Clear", "Shift Left", "Shift Right" };
        string[] jumps = new string[] { "Jump Object", "Jump Title", "Call Object", "Call Title", "Resume" };
        string[] gotos = new string[] { "NOP", "Goto", "Break" };
        string[] plays = new string[] { "Play PL", "Play PL_PI", "Play PL_MK", "Terminate PL", "Link PI", "Link MK" };
        string[] compare = new string[] { "-", "BC", "EQ", "NE", "GE", "GT", "LE", "LT" };
        string[] setsystem = new string[] { "Set stream", "SetNVTimer", "Set button Page", "Enable button", "Disable button", "Set secondary stream", "Popup menu off ", "Still on", "Still off", "Set output mode", "Set stream SS" };
        #endregion
    }
    [DefaultPropertyAttribute("Index")]
    public class IndexBdmv : Bdmv
    {
        string contentProvider;
        int num;
        int menuNum;
        int mode2dInd;
        int stereoInd;
        int firstPlaybackInd;
        int topMenuInd;
        int movieInd;
        AppInfo appInfo;
        PlayListEntry firstPlayback;
        PlayListEntry topMenu;

        public PlayListEntry TopMenu
        {
            get { return topMenu; }
            set { topMenu = value; }
        }
        public PlayListEntry FirstPlayback
        {
            get { return firstPlayback; }
            set { firstPlayback = value; }
        }
        List<PlayListEntry> playList = new List<PlayListEntry>();
        #region Properties
        [CategoryAttribute("Video"), DescriptionAttribute("AppInfo")]
        public AppInfo AppInfo
        {
            get { return appInfo; }
            set { appInfo = value; }
        }
        [CategoryAttribute("Video"), DescriptionAttribute("Play Lists")]
        public List<PlayListEntry> PlayList
        {
            get { return playList; }
            set { playList = value; }
        }
        [CategoryAttribute("Provider"), DescriptionAttribute("Provider")]
        public string ContentProvider
        {
            get { return contentProvider; }
            set { contentProvider = value; }
        }
        [CategoryAttribute("Top Menu"), DescriptionAttribute("Type")]
        public string Hdmv
        {
            // 0x52 :  BD-J (bit 1) HDMV (bit2 ) Prohibited1 (bit 3) Prob 2 (bit4) 
            get
            {
                int h = firstPlaybackInd & 0xF0;
                switch (h)
                {
                    case 0x40:
                        return "HDMV";
                    case 0x80:
                        return "BD-J";
                    default:
                        return "";
                }
            }
        }
        [CategoryAttribute("Top Menu"), DescriptionAttribute("Menu Number Movie object")]
        public int MenuNumber
        {
            get { return menuNum; }
            set { menuNum = value; }
        }
        [CategoryAttribute("First Playback"), DescriptionAttribute("First Playback in Movie object")]
        public int Number
        {
            get { return num; }
            set { num = value; }
        }
        [CategoryAttribute("First Playback"), DescriptionAttribute("Type")]
        public string Permitted
        {
            // 0x52 :  BD-J (bit 1) HDMV (bit2 ) Prohibited1 (bit 3) Prob 2 (bit4) 
            get
            {
                int h = firstPlaybackInd & 0x0F;
                switch (h)
                {
                    case 0x00:
                        return "Permitted";
                    case 0x20:
                        return "Prohibited 1";
                    case 0x01:
                        return "Prohibited 2";
                    default:
                        return "";
                }
            }
        }
        [CategoryAttribute("First Playback"), DescriptionAttribute("Segments")]
        public string Movie
        {
            // 0x56 : bit 2 (Object_Type /interactive)4
            get
            {
                {
                    int h = firstPlaybackInd & 0xFF;
                    switch (h)
                    {
                        case 0x40:
                            return "Interactive";
                        case 0x80:
                            return "Movie";
                        default:
                            return "";
                    }
                }
            }
        }
        List<string> mpls = new List<string>();
        List<MPLS> list = new List<MPLS>();
        [CategoryAttribute("PlayList"), DescriptionAttribute("MPLS files")]
        public List<MPLS> MplsList
        {
            get { return list; }
            set { list = value; }
        }
        MovieObject mv;
        public MovieObject MovieObject
        {
            get { return mv; }
            set { mv = value; }
        }
        public List<string> PlayLists
        {
            get { return mpls; }
            set { mpls = value; }
        }
        #endregion
        // http://www.faqs.org/patents/app/20090317067
        public IndexBdmv(string fileName)
        {
            INDEX_BDMV ind = new INDEX_BDMV(fileName);
            ConvertBuffer.littleEndian = false;
            BinaryFileReader FS = new BinaryFileReader(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, false);
            byte[] buffer = new byte[16];
            buffer = new byte[FS.Length];
            FS.Read(buffer, 0, (int)FS.Length);
            FS.Position = 0;
            Header = FS.ReadString(8); 
            if (Header.IndexOf("INDX") != 0)
                return;
            int index_start = FS.ReadInteger();// ConvertBuffer.ReadInteger(buffer, 0x08);  // 0x08 fin de la chaine index_start
            int extensionData = FS.ReadInteger();//ConvertBuffer.ReadInteger(buffer, 0x0c);  //start extension buffer
            FS.Position = 0x28;
            int length = FS.ReadInteger();
            if (length != 0x22)//expected 0x22
            {
            }
            appInfo = new BluRay.AppInfo(FS.ReadBytes(2));///buffer[0x2c];// 0x2C : 20 : stereoscopic ; buffer[0x2d];// 0x2D première partie (4 bits) résolution, seconde frame rate
            contentProvider = FS.ReadString(index_start - (int)FS.Position);// ConvertBuffer.ReadString(buffer, 0x2e, index_start - 0x2e);// 0x2e Content index_start sur 32 octets
            byte[] userdata = FS.ReadBytes(0x4E - (int)FS.Position);
            //        bs.Position = 0x4E;
            int blocLength = FS.ReadInteger();// bs.Postion + blocLength = extensionData 
            # region First playback : 12 bytes
            FirstPlayback = new PlayListEntry(FS.ReadBytes(12));
            #endregion
            #region Top Menu 12 bytes
            // 0x5e : Top menu 0, non 40
            topMenu = new PlayListEntry(FS.ReadBytes(12));
            #endregion
            int playlistNumber = FS.ReadShort();//Short ? ConvertBuffer.ReadInteger(buffer, 0x68); // 0x68 ; nombre d'entrées (playlist)
            for (int u = 0; u < playlistNumber; u++)
            {
                #region Playlists
                byte objType = FS.ReadByte();// byte 1
                FS.ReadBytes(3);
                PlayListEntry pl = new PlayListEntry();
                playList.Add(pl);
                //              Buffer.BlockCopy(buffer, start, loc, 0, 12);
                // chaque entrée 40 00 00 00 (HDMV)/ 00 (40 pour interactif) 00 (00 numClips) /00 00 00 00 
                // suivi de 6 mots
                switch (objType & 0xC0)// bits 1 et 2
                {
                    case 0x40:
                        pl.ObjectType = "HDMV";
                        byte[] pbType = FS.ReadBytes(2);
                        switch (pbType[0])
                        {
                            case 0x40:
                                pl.Object_Type = "Interactive";
                                break;
                            case 0x0:
                                pl.Object_Type = "Movie";
                                break;
                            default:
                                pl.Object_Type = "-";
                                break;
                        }
                        pl.ListNumber = FS.ReadShort();
                        byte[] reserved = FS.ReadBytes(4);
                        break;
                    case 0x80:
                        pl.ObjectType = "BD-J";
                        FS.ReadBytes(8);
                        break;
                    default:
                        pl.ObjectType = "-";
                        break;
                }
                switch (objType & 0x30)// bits 3 et 4
                {
                    case 0x00:
                        pl.Permitted = "Permitted";
                        break;
                    case 0x20:
                        pl.Permitted = "Prohibited 1";
                        break;
                    case 0x10:
                        pl.Permitted = "Prohibited 2";
                        break;
                }
                //        pl.ListNumber = oid;// ConvertBuffer.ReadInteger(loc, 0x04);
                //               start += 12;
                #endregion
            }
            // 0x68 + nbOfClips * 12 : 
            // fin : numéro de fichiers et ordre
            int lengthExtensionData = FS.ReadInteger();//ConvertBuffer.ReadInteger(buffer, extensionData);
            int lengthFirstblock = FS.ReadInteger();
            FS.ReadBytes(4);
            int ID1 = FS.ReadShort();
            int ID2 = FS.ReadShort();
            int st = FS.ReadInteger();
            int lg = FS.ReadInteger();
            //           bs.Position = extensionData + lengthFirstblock;
            String idex = FS.ReadString(4);
            FS.ReadBytes(4);//0181]An area "reserved" having a data length of 32 bits is arranged subsequent to the field TypeIndicator
            int TableOfPlayListStartAddress = FS.ReadInteger();
            FS.ReadInteger();
            FS.Position += TableOfPlayListStartAddress;
            int nb = FS.ReadShort();
            for (int u = 0; u < nb; u++)
            {
                string n = FS.ReadString(5);
                mpls.Add(n + ".mpls");
                FS.ReadByte();
                int rank = FS.ReadInteger();
                playList[rank].Name = n;
            }
            FS.Close();
            mv = new MovieObject(Path.GetDirectoryName(FS.Name) + "\\MovieObject.bdmv");
            foreach (string s in mpls)
            {

                string fn = Path.GetDirectoryName(FS.Name) + "\\PLAYLIST\\" + s;
                MPLS mp = new MPLS(fn);
                list.Add(mp);
            }
        }
        void _parse_hdmv_obj(byte[] b) /*BITSTREAM* bs, INDX_HDMV_OBJ* hdmv*/
        {
            /*       // 8 octets : 
                      hdmv->playback_type = bs_read(bs, 2);
                      bs_skip(bs, 14); //2 octets
                      hdmv->id_ref = bs_read(bs, 16); // 2 octets
                      bs_skip(bs, 32); // 4 octets
             */
            ;
        }
        void _parse_bdj_obj(byte[] b) /*BITSTREAM* bs, INDX_BDJ_OBJ* bdj*/
        {
            /*            bdj->playback_type = bs_read(bs, 2);
                        bs_skip(bs, 14); // 2 octets
                        bs_read_bytes(bs, (uint8_t*)bdj->name, 5); // 5 octets ?
                        bdj->name[5] = 0;
                        bs_skip(bs, 8); // 1 octet
             */
        }
        void _parse_playback_obj(byte[] b) /*BITSTREAM* bs, INDX_PLAY_ITEM* obj*/
        {
            /*          obj->object_type = bs_read(bs, 2);
                      bs_skip(bs, 30);

                      if (obj->object_type == 1)
                      {
                          return _parse_hdmv_obj(bs, &obj->hdmv);
                      }
                      else
                      {
                          return _parse_bdj_obj(bs, &obj->bdj);
                      }*/
        }

    }
    public class AppInfo
    {
        bool initial_output_mode_preference;
        bool content_exist_flag;
        byte videoInd;
        [CategoryAttribute("Video"), DescriptionAttribute("Resolution")]
        public string Video
        {//0x2D première partie résolution, seconde frame rate
            get
            {
                int high = (videoInd & 0xF0) >> 4;
                return Segment.resolution[high];
            }
        }
        [CategoryAttribute("Video"), DescriptionAttribute("Frame rate")]
        public string Framerate
        {
            // 0x2D première partie résolution, seconde frame rate
            get
            {
                int low = videoInd & 0x0F;
                return Segment.frameRate[low];
            }
        }
        [CategoryAttribute("Unknown"), DescriptionAttribute("Segments")]
        public string Mode2d
        {
            get
            {
                if (initial_output_mode_preference)
                    return "SS";
                else
                    return "2D";
            }
        }
        [CategoryAttribute("Video"), DescriptionAttribute("Segments")]
        public bool Stereo
        {
            get
            {
                return (content_exist_flag);
            }
        }
        public AppInfo(byte[] b)
        {
            initial_output_mode_preference = (b[0] & 0x40) == 0x40;
            content_exist_flag = (b[0] & 0x20) == 0x20;
            videoInd = b[1];
        }
    }
    [DefaultPropertyAttribute("Index")]
    public class PlayListEntry
    {
        private string hdmv;
        private string movie;
        private string permitted;
        private string name;
        [CategoryAttribute("Video"), DescriptionAttribute("Name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        [CategoryAttribute("Video"), DescriptionAttribute("Type")]
        public string Permitted
        {
            get { return permitted; }
            set { permitted = value; }
        }
        [CategoryAttribute("Video"), DescriptionAttribute("Type")]
        public string ObjectType
        {
            get { return hdmv; }
            set { hdmv = value; }
        }
        [CategoryAttribute("Video"), DescriptionAttribute("Segments")]
        public string Object_Type
        {
            get { return movie; }
            set { movie = value; }
        }
        public int nb;
        [CategoryAttribute("Video"), DescriptionAttribute("Numero")]
        public int ListNumber
        {
            get { return nb; }
            set { nb = value; }
        }
        public PlayListEntry(byte[] b)
        {
            int objType = b[0];
            int movieInd = b[4];// buffer[0x56];// 0x56 : bit 2 (Object_Type /interactive)4
            switch (objType & 0xC0)// bits 1 et 2
            {
                case 0x40:
                    ObjectType = "HDMV";

                    switch (b[4])
                    {
                        case 0x40:
                            Object_Type = "Interactive";
                            break;
                        case 0x0:
                            Object_Type = "Movie";
                            break;
                    }
                    ListNumber = b[6] * 256 + b[7];
                    break;
                case 0x80:
                    ObjectType = "BD-J";
                    break;
            }
            switch (objType & 0x30)// bits 3 et 4
            {
                case 0x00:
                    Permitted = "Permitted";
                    break;
                case 0x20:
                    Permitted = "Prohibited 1";
                    break;
                case 0x10:
                    Permitted = "Prohibited 2";
                    break;
            }

        }
        public PlayListEntry()
        {

        }
    }
}
