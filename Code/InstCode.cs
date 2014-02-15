using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace Code
{
    [Serializable]
    public class Attribute
    {
        string Name;
        string Value;
        public Attribute(string n, string v)
        {
            Name = n;
            Value = v;
        }
    }
    [Serializable]
    public class destint
    {
        public string Value;
        #region Nodes
        public string a;
        public string t;
        #endregion
        #region attributes
        /*<!-- @nr: in case of direct register immediateOperand, its hardware number, if any -->
 <!-- @group: in case of direct register immediateOperand, its group -->
 <!-- @type: in case of direct register immediateOperand, its type code, if explicitly needed -->
 <!-- @startAddress: code for addressing method -->
 <!-- @depend: does the resulting immediateOperand value depend on its previous value? -->
 <!-- @displayed: is the implicit immediateOperand displayed within the syntax? -->
*/
        public string nr;
        public string group;
        public string type;
        public string address;
        public string depend;
        public string displayed;
        #endregion
        public override string ToString()
        {
            return a + " " + t;
        }
        public destint(XmlNode xn)
        {
            //        if (xn.InnerText != "") fixedFileInfo = xn.InnerText;
            foreach (XmlAttribute attr in xn.Attributes)
            {
                switch (attr.Name)
                {
                    case "nr":
                        nr = attr.Value;
                        break;
                    case "group":
                        group = attr.Value;
                        break;
                    case "type":
                        type = attr.Value;
                        break;
                    case "address":
                        address = attr.Value;
                        break;
                    case "depend":
                    case "displayed":
                        break;
                }
            }
            foreach (XmlNode attr in xn.ChildNodes)
            {
                switch (attr.Name)
                {
                    case "a":
                        a = attr.InnerText;
                        break;
                    case "t":
                        t = attr.InnerText;
                        break;
                    case "group":
                        group = attr.InnerText;
                        break;
                }
            }
        }
    }
    [Serializable]
    public class syntax_
    {
        public string mnemo;
        public List<destint> dest = new List<destint>();
        public List<destint> src = new List<destint>();
        public List<destint> operandes = new List<destint>();
        public override string ToString()
        {
            return mnemo;
        }
        public syntax_(XmlNode xn)
        {
            foreach (XmlNode xnn in xn.ChildNodes)
            {
                switch (xnn.Name)
                {
                    case "mnem":
                        mnemo = xnn.InnerText;
                        break;
                    case "dst":
                        destint d = new destint(xnn);
                        dest.Add(d);
                        operandes.Add(d);
                        break;
                    case "src":
                        d = new destint(xnn);
                        src.Add(d);
                        operandes.Add(d);
                        break;
                }
            }

        }
    }
    [Serializable]
    public class entry
    {
        #region Attributes
        public int direction;
        public int sign_ext;
        public int op_size;
        public int tttn;
        public int mem_format;
        public bool r;
        public string mod;
        public string ref_;
        public string alias;
        public string part_alias;
        public string doc_part_alias_ref;
        public bool lock_;
        public string attr;
        public string mode = "r";
        public string fpop;
        public bool fpush;
        public bool particular;
        #endregion
        #region Nodes
        public int opcd_ext;
        public string prf;
        public int sec_opcd = -1;
        public string proc_start, proc_end;
        public List<syntax_> syntax;
        public string instr_ext;
        public string grp1;
        public List<string> grp2 = new List<string>();
        public List<string> grp3 = new List<string>();
        public string test_f, modif_f, def_f, undef_f, f_vals,
          test_f_fpu, modif_f_fpu, def_f_fpu, undef_f_fpu, f_vals_fpu;
        public string note;
        #endregion
        public override string ToString()
        {
            return syntax[0].mnemo.ToString();
        }
        public entry(XmlNode son)
        {
            #region Read attributes
            foreach (XmlAttribute attr in son.Attributes)
            {
                try
                {
                    switch (attr.Name)
                    {
                        case "tttn":
                            tttn = int.Parse(attr.Value);
                            break;
                        case "mem_format":
                            mem_format = int.Parse(attr.Value);
                            break;
                        case "mod":
                            mod = attr.Value;
                            break;
                        case "fpush":
                            break;
                        case "op_size":
                            op_size = int.Parse(attr.Value);
                            break;
                        case "direction":
                            direction = int.Parse(attr.Value);
                            break;
                        case "r":
                            r = (attr.Value == "yes");
                            break;
                        case "lock":
                            lock_ = (attr.Value == "yes");
                            break;
                    }
                }
                catch { }
            }
            #endregion
            opcd_ext = -1;
            #region Read nodes
            syntax = new List<syntax_>();
            foreach (XmlNode xn in son.ChildNodes)
            {
                try
                {
                    switch (xn.Name)
                    {
                        case "sec_opcd":
                            sec_opcd = Convert.ToInt32(xn.InnerText, 16);
                            break;
                        case "pref":
                            prf = xn.InnerText;
                            break;
                        case "proc_start":
                            proc_start = xn.InnerText;
                            break;
                        case "proc_end":
                            proc_end = xn.InnerText;
                            break;
                        case "instr_ext":
                            instr_ext = xn.InnerText;
                            break;
                        case "opcd_ext":
                            opcd_ext = Convert.ToInt32(xn.InnerText, 16);
                            break;
                        case "syntax":
                            syntax.Add(new syntax_(xn));
                            break;
                        case "grp1":
                            grp1 = xn.InnerText;
                            break;
                        case "grp2":
                            grp2.Add(xn.InnerText);
                            break;
                        case "grp3":
                            grp3.Add(xn.InnerText);
                            break;
                        case "modif_f":
                            modif_f = xn.InnerText;
                            break;
                        case "def_f":
                            def_f = xn.InnerText;
                            break;
                        case "undef_f_fpu":
                            break;
                        case "note":
                            note = xn.ChildNodes[0].InnerText;
                            break;
                    }
                }
                catch { }
            }
            #endregion
        }
    }
    [Serializable]
    public class Instruction
    {
        public int type;
        public string proc_start, proc_end;
        public byte shortOpCode;
        public bool ModRMHasRegister
        {
            get { return entries[0].r; }
        }
        public bool HasSubInstructions
        {
            get { return subInstructions.Count > 0; }
        }
        public bool HasExtensions
        {
            get { return instByextension.Count > 0; }
        }
        public string opCode;
        public List<entry> entries;
        public string Code
        {
            get
            {
                string send = opCode;
                if (type == 2)
                {
                    send = "0F" + send;
                }
                return send;
            }
        }
        public short long0pCode
        {
            get
            {
                short send = shortOpCode;
                if (type == 2)
                {
                    send = (short)(0x0F * 256 + send);
                }
                return send;
            }
        }
        public SortedList<int, int> subInstructions = new SortedList<int, int>();
        public SortedList<int, int> instByextension = new SortedList<int, int>();
        public Instruction(XmlNode son, int t)
        {
            type = t;
            shortOpCode = (byte)Convert.ToInt32(son.Attributes[0].Value, 16);
            if (shortOpCode == 0x95)
            {
            }
            opCode = shortOpCode.ToString("x2");
            if (shortOpCode == 0xdb)
            {
            }
            entries = new List<entry>();
            int entryIndex = 0;
            foreach (XmlNode entr in son.ChildNodes)
            {
                switch (entr.Name)
                {
                    case "entry":
                        entry en = new entry(entr);
                        entries.Add(en);
                        if (en.sec_opcd != -1)
                        {
                            try
                            {
                                subInstructions.Add(en.sec_opcd, entryIndex);
                            }
                            catch { }
                        }
                        if (en.opcd_ext != -1)
                            try
                            {
                                instByextension.Add(en.opcd_ext, entryIndex);
                            }
                            catch { }
                        entryIndex++;
                        break;
                    case "proc_start": proc_start = entr.InnerText;
                        break;
                    case "proc_end": proc_end = entr.InnerText;
                        break;
                }
            }
        }
        public override string ToString()
        {
            return opCode + " " + entries[0].syntax[0].mnemo;
        }
    }
}
