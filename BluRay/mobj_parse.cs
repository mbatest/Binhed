using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;

namespace BluRay
{
    public class MOBJ_BDMV : LOCALIZED_DATA
    {
        private string header;
        int extension_data_start;
        int data_len;
        public short num_objects;
        private List<MOBJ_OBJECT> objects;
        public string fileName;
        #region Properties
        public string Header
        {
            get { return header; }
            set { header = value; }
        }
        public List<MOBJ_OBJECT> Commands
        {
            get { return objects; }
            set { objects = value; }
        }
        #endregion
        public MOBJ_BDMV(string fileName)
        {
            this.fileName = fileName;
            BitStreamReader bs = new BitStreamReader(fileName, true);
            bs.BitPosition = 0;
            header = bs.ReadString(8);
            if (!header.Contains("MOBJ"))
            {
                //BD_DEBUG(DBG_NAV, "MovieObject.bdmv failed signature match: expected MOBJ0100 got %8.8s\n", bs.buf);
                return;
            }
            extension_data_start = bs.ReadInteger();
            bs.BitPosition = 8 * 0x28;// 8 bytes reserved ???
            data_len = bs.ReadInteger();
            bs.SkipBit(32); /* reserved */
            num_objects = bs.ReadShort();
            objects = new List<MOBJ_OBJECT>();
            for (int i = 0; i < num_objects; i++)
            {
                MOBJ_OBJECT mobj = new MOBJ_OBJECT(bs);
                objects.Add(mobj);
            }
            bs.Close();
        }
    }
    #region Movie objects
    public struct HDMV_INSN
    {
        #region Raw data
        public int Rawdata;
        public byte op_cnt;//     : 3;  /* operand count */
        public byte grp;//  : 2;  /* command group */
        public byte sub_grp;// : 3;  /* command sub-group */
        public bool imm_op1;//  : 1;  /* I-flag for operand 1 */
        public bool imm_op2;// : 1;  /* I-flag for operand 2 */
        public byte reserved1;// : 2;
        public byte branch_opt;// : 4;  /* branch option */
        public byte reserved2;//  : 4;
        public byte cmp_opt;// : 4;  /* compare option */
        public byte reserved3;// : 3;
        public byte set_opt;// : 5;  /* set option */
        #endregion
    }
    public class MOBJ_CMD : LOCALIZED_DATA
    {
        private int firstInt;
        private int thirdInt;
        private int secondInt;
        public HDMV_INSN insn;
        private int dst;
        private int src;
        private string lineCode;
        private byte[] instruction;
        public long position;
        public long length;
        #region Properties
        public string LineCode
        {
            get { return lineCode; }
            set { lineCode = value; }
        }
        public int Source
        {
            get { return src; }
            set { src = value; }
        }
        public int Destination
        {
            get { return dst; }
            set { dst = value; }
        }
        public int Opcode
        {
            get { return firstInt; }
            set { firstInt = value; }
        }
        #endregion
        public override string ToString()
        {
            return lineCode;
        }
        public MOBJ_CMD(BitStreamReader bs)
        {
            position = bs.Position;
            length = 12;
            instruction = bs.ReadBytes(12);
            BitStreamReader bb = new BitStreamReader(instruction, true);
            firstInt = bb.ReadInteger();
            secondInt = bb.ReadInteger();
            thirdInt = bb.ReadInteger();
            bb.BitPosition = 0;
            insn.Rawdata = firstInt;
            insn.op_cnt = (byte)bb.ReadIntFromBits(3);
            insn.grp = (byte)bb.ReadIntFromBits(2);
            insn.sub_grp = (byte)bb.ReadIntFromBits(3);
            insn.imm_op1 = bb.ReadBool();
            insn.imm_op2 = bb.ReadBool();
            bb.SkipBit(2);    /* reserved */
            insn.branch_opt = (byte)bb.ReadIntFromBits(4);
            bb.SkipBit(4);    /* reserved */
            insn.cmp_opt = (byte)bb.ReadIntFromBits(4);
            bb.SkipBit(3);    /* reserved */
            insn.set_opt = (byte)bb.ReadIntFromBits(5);
            dst = bb.ReadIntFromBits(32);
            src = bb.ReadIntFromBits(32);
            lineCode = Decode(instruction);
        }
        private string Decode(byte[] inst)
        {
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
                string op1 = dst.ToString();// entier sur 4 octets (ou 3 ?)
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
                string op2 = src.ToString();
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
        #region Data
        string[] ops = new string[] { "-", "Move", "Swap", "Add", "Sub", "Mul", "Div", "Mod", "Rnd", "And", "Or", "XOr", "Bit Set", "Bit Clear", "Shift Left", "Shift Right" };
        string[] jumps = new string[] { "Jump Object", "Jump Title", "Call Object", "Call Title", "Resume" };
        string[] gotos = new string[] { "NOP", "Goto", "Break" };
        string[] plays = new string[] { "Play PL", "Play PL_PI", "Play PL_MK", "Terminate PL", "Link PI", "Link MK" };
        string[] compare = new string[] { "-", "BC", "EQ", "NE", "GE", "GT", "LE", "LT" };
        string[] setsystem = new string[] { "Set stream", "SetNVTimer", "Set button Page", "Enable button", "Disable button", "Set secondary stream", "Popup menu off ", "Still on", "Still off", "Set output mode", "Set stream SS" };
        #endregion
    }
    public class MOBJ_OBJECT : LOCALIZED_DATA
    {
        private bool resume_intention_flag;// : 1;
        private bool menu_call_mask;//  : 1;
        private bool title_search_mask;//  : 1;
        private short num_cmds;
        private List<MOBJ_CMD> cmds;
        public long position;
        public long length;
        #region Properties
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
        public List<MOBJ_CMD> Commands
        {
            get { return cmds; }
            set { cmds = value; }
        }
        #endregion
        public MOBJ_OBJECT(BitStreamReader bs)
        {
            position = bs.Position;
            resume_intention_flag = bs.ReadBool();
            menu_call_mask = bs.ReadBool();
            title_search_mask = bs.ReadBool();
            bs.SkipBit(13); /* padding */
            num_cmds = bs.ReadShort();
            cmds = new List<MOBJ_CMD>();
            for (int i = 0; i < num_cmds; i++)
            {
                cmds.Add(new MOBJ_CMD(bs));
            }
            length = bs.Position - position;
        }
    }
    #endregion
}
