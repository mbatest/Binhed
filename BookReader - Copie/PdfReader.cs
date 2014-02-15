using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Drawing;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Utils;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace BookReader
{
    public class PdfReader
    {
        #region Private data
        FileStream FS;
        System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
        public string text;
        private Dictionary<int, PdfObject> objects = new Dictionary<int, PdfObject>();
        private Dictionary<int, int> objRef = new Dictionary<int, int>();
        private List<string> entries = new List<string>();
        private string header;
        List<Trailer> trailer = new List<Trailer>();
        List<PdfObject> pages = new List<PdfObject>();
        List<PdfObject> pdfObjects = new List<PdfObject>();
        PdfObject root;
        string FileName;
        #endregion
        #region Private methods
        private int FindXrefAddress(long end)
        {
            int offsetToEnd = 50;
            FS.Seek(end - offsetToEnd, SeekOrigin.Begin);
            Byte[] buff = new byte[offsetToEnd];
            FS.Read(buff, 0, offsetToEnd);
            //Recherche entrée xref : son adresse sous forme de chaine avant le marqueur de fin
            int n = offsetToEnd - 2;
            int lf = 0x0a;
            int cr = 0x0d;
            List<byte> t = new List<byte>();
            while ((buff[n] == lf) || (buff[n] == cr))
                n--;
            n -= 5;
            while ((buff[n] == lf) || (buff[n] == cr))
                n--;
            int fin = n;
            while ((buff[n] != lf) && (buff[n] != cr))
                n--;
            for (int u = n; u <= fin; u++)
                t.Add(buff[u]);
            int adxref = int.Parse(Encoding.Default.GetString(t.ToArray()));
            return adxref;
        }
        private void FindDocumentStructure()
        {
            root = ReadObjDescription(TrailerList[0].Root);
            PdfObject Info = ReadObjDescription(TrailerList[0].Info);
            root.AddAttribute(Info);
            string pagesN;
            root.Dictionnary.TryGetValue("Pages", out pagesN);
            int pagesNumber = int.Parse(((pagesN.Split(' '))[0]));
            #region Looking for pages
            ParseDic(root);
            #endregion
        }
        private void FindObjectRef(int u)
        {
            string[] objOffset = entries[u].Split(' ');
            int offset = int.Parse(objOffset[0]);
            if (objOffset[2].IndexOf("n") >= 0)
            {
                FS.Seek(offset, SeekOrigin.Begin);
                string number = ReadString();
                objRef.Add(int.Parse(number), offset);
            }
        }
        private void FindRawContent(PdfObject pf)
        {
            int length = 0;
            string strlength;
            if (pf.Dictionnary.TryGetValue("Length", out strlength))
            {
                if (strlength.Contains("R"))
                {
                    string data = strlength.Split(' ')[0];
                    PdfObject p;
                    if (!objects.TryGetValue(int.Parse(data), out p))
                        p = ReadObjDescription(int.Parse(data));
                    length = int.Parse(p.RawData);
                }
                else
                    length = int.Parse(strlength);
            }
            if (length == 0)
                return;
            int offset;
            objRef.TryGetValue(pf.Number, out offset);
            FS.Seek(offset, SeekOrigin.Begin);
            string line = ReadLine();
            while (!line.Contains("stream"))
            {
                line += ReadLine();
            }
            long start = FS.Position;
            int i = FS.ReadByte();
            while ((i == 0x0a) || (i == 0x0d))
            {
                start++;
                i = FS.ReadByte();
            }
            FS.Seek(start, SeekOrigin.Begin);
            byte[] buf = new byte[length];
            FS.Read(buf, 0, length);
            pf.RawStreamData = buf;
            if (pf.RawStreamData != null)
                pf.StreamData = Decompress(pf);
        }
        private PdfObject ReadObjDescription(int u)
        {
            if (u == 4)
            { }
            FS = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            int offset;
            if (!objRef.TryGetValue(u, out offset))
                return null;
            FS.Seek(offset, SeekOrigin.Begin);
            #region Object header
            List<byte> lbyte = new List<byte>();
            string number = ReadString();
            string gen = ReadString();
            string type = ReadString(3);
            #endregion
            PdfObject pdfObj = new PdfObject(number, gen, type);
            objects.Add(u, pdfObj);
            int dt = FS.ReadByte();
            while ((dt == 0x20) || (dt == 0x0a) || (dt == 0x0d))
                dt = FS.ReadByte();
            string l = "";
            int depth = 0;
            bool par = false;
            if (dt == 0x3c)
            {
                #region Lecture du dictionnaire
                depth++;
                List<byte> dictionary = new List<byte>();
                dictionary.Add((byte)dt);
                while (depth > 0)
                {
                    dt = FS.ReadByte();
                    if (dt == 0x28)
                        par = true;
                    if (dt == 0x29)
                        par = false;
                    if (!par)
                    {
                        if (dt == 0x3c) depth++;
                        if (dt == 0x3e)
                            depth--;
                    }
                    dictionary.Add((byte)dt);
                }
                //             Encoding encoder = Encoding.Default;
                l = encoder.GetString(dictionary.ToArray());
                if (l.Contains("endobj"))
                    l = l.Substring(0, l.IndexOf("endobj") + "endobj".Length);
                #endregion

            }
            else
            {
                #region Directement les données
                l = ReadLine();
                byte[] db = new byte[1];
                db[0] = (byte)dt;
                l = Encoding.Default.GetString(db) + l;
                if (l.Contains("endobj"))
                    l = l.Substring(0, l.IndexOf("endobj") + "endobj".Length);
                pdfObj.RawData = l;
                #endregion;
            }
            pdfObj.data += l;
            if (l != "")
                try
                {
                    pdfObj.AddToDictionnary(l);
                }
                catch (Exception e) { Trace.WriteLine(e.Message); }
            try
            {
                //                FindRawContent(pdfObj);
                int length = 0;
                string strlength;
                if (pdfObj.Dictionnary.TryGetValue("Length", out strlength))
                {
                    if (strlength.Contains("R"))
                    {
                        string data = strlength.Split(' ')[0];
                        PdfObject p;
                        if (!objects.TryGetValue(int.Parse(data), out p))
                            p = ReadObjDescription(int.Parse(data));
                        length = int.Parse(p.RawData);
                    }
                    else
                        length = int.Parse(strlength);
                }
                if (length != 0)
                {
                    objRef.TryGetValue(pdfObj.Number, out offset);
                    FS.Seek(offset, SeekOrigin.Begin);
                    string line = ReadLine();
                    while (!line.Contains("stream"))
                    {
                        line += ReadLine();
                    }
                    long start = FS.Position;
                    int i = FS.ReadByte();
                    while ((i == 0x0a) || (i == 0x0d))
                    {
                        start++;
                        i = FS.ReadByte();
                    }
                    FS.Seek(start, SeekOrigin.Begin);
                    byte[] buf = new byte[length];
                    FS.Read(buf, 0, length);
                    pdfObj.RawStreamData = buf;
       //             if (pdfObj.RawStreamData != null)                         pdfObj.StreamData = Decompress(pdfObj);
                }
                if (l.Contains("XML"))
                    pdfObj.DecodedStream = encoder.GetString(pdfObj.RawStreamData);
            }
            catch (Exception e) { Trace.WriteLine(e.Message); }
            //            FS.Close();
            return pdfObj;
        }
        private string ReadString(int length)
        {
            List<byte> bt = new List<byte>();
            while (bt.Count < length)
            {
                bt.Add((byte)FS.ReadByte());
            }
            return encoder.GetString(bt.ToArray());
        }
        private string ReadString()
        {
            //         Encoding encoder = Encoding.Default;
            List<byte> bt = new List<byte>();
            byte data = (byte)FS.ReadByte();
            while (data != 0x20)
            {
                bt.Add(data);
                data = (byte)FS.ReadByte();
            }
            return encoder.GetString(bt.ToArray());
        }
        private byte[] Decompress(PdfObject pdf)
        {
            byte[] buffer = pdf.RawStreamData;
            string filter;
            if (pdf.Dictionnary.TryGetValue("Filter", out filter))
            {
                if (filter.Contains("["))
                {
                    filter.Replace("[", "").Replace("]", "").Replace("/", "");
                    string[] fil = filter.Split(' ');
                    foreach (string s in fil)
                    {
                    }
                }
                switch (filter)
                {
                    case "ASCIIHexDecode":
                    case "ASCII85Decode":
                    case "LZWDecode":
                    case "RunLengthDecode":
                        break;
                    case "DCTDecode":
                        break;
                    case "CCITTFaxDecode":
                        TIFFFaxDecoder Tf = new TIFFFaxDecoder(1, 0, 0);
                        break;
                    case "FlateDecode":
                        try
                        {
                            byte[] b = new byte[buffer.Length * 3];
                            Inflater i = new Inflater();
                            i.SetInput(buffer);
                            i.Inflate(b);
                            int x = i.TotalOut;
                            byte[] bOut = new byte[i.TotalOut];
                            Buffer.BlockCopy(b, 0, bOut, 0, i.TotalOut);
                            UnicodeEncoding encoderU = new UnicodeEncoding();
                            pdf.RawData = encoderU.GetString(pdf.RawStreamData);
                            return bOut;
                        }
                        catch { return null; }
                    default:
                        pdf.RawData = encoder.GetString(pdf.RawStreamData);
                        return buffer;
                }
            }
            return null;
        }
        private byte[] ReadStream(int length)
        {
            List<byte> b = new List<byte>();
            if (length != 0)
            {
                int st = 0;
                while (st < length)
                {
                    b.Add((byte)FS.ReadByte());
                    st++;
                }
            }
            else
            {

                byte t = (byte)FS.ReadByte();
                while (((t != 0x0d) && (t != 0x0a)))
                {
                    b.Add(t);
                    t = (byte)FS.ReadByte();
                }
            }
            return b.ToArray();
        }
        private string ReadLine()
        {
            int lf = 0x0a;
            int cr = 0x0d;
            List<byte> bytesLine = new List<byte>();
            int t = FS.ReadByte();
            while ((t == cr) || (t == lf))
                t = FS.ReadByte();
            while ((FS.Position < FS.Length) && (t != cr) && (t != lf))
            {
                bytesLine.Add((byte)t);
                t = FS.ReadByte();
            }
            return Encoding.Default.GetString(bytesLine.ToArray());
        }
        #endregion
        public PdfObject Root
        {
            get { return root; }
            set { root = value; }
        }
        [CategoryAttribute("Pdf"), DescriptionAttribute("Pages")]
        public List<PdfObject> Pages
        {
            get { return pages; }
            set { pages = value; }
        }
        [CategoryAttribute("Pdf"), DescriptionAttribute("Trailer")]
        public List<Trailer> TrailerList
        {
            get { return trailer; }
            set { trailer = value; }
        }
        [CategoryAttribute("Pdf"), DescriptionAttribute("Objects")]
        public List<PdfObject> PdfObjects
        {
            get { return pdfObjects; }
            set { pdfObjects = value; }
        }
        [CategoryAttribute("Pdf"), DescriptionAttribute("Entries")]
        public List<string> Entries
        {
            get { return entries; }
            set { entries = value; }
        }
        [CategoryAttribute("Pdf"), DescriptionAttribute("Header")]
        public string Header
        {
            get { return header; }
            set { header = value; }
        }
        public PdfReader(string FileName)
        {
            this.FileName = FileName;
            BinaryReader bf = new BinaryReader(
            FS = new FileStream(FileName, FileMode.Open, FileAccess.Read));
            #region Read Header
            header = ReadLine();
            #endregion
            #region First crossref
            int adxref = FindXrefAddress(FS.Length);
            FS.Seek(adxref, SeekOrigin.Begin);
            string l = ReadLine();//Xref line
            string entriesTxt = ReadLine();
            string[] ent = entriesTxt.Split(' ');
            int nbEntries = int.Parse(ent[1]);
            for (int u = 0; u < nbEntries; u++)
            {
                string entry = ReadLine();
                entries.Add(entry);
            }
            #endregion
            #region First trailer
            Trailer currentTrailer = null;
            string s = ReadLine();
            while (!s.Contains("trailer"))
                s = ReadLine();
            if (s.IndexOf("trailer") >= 0)
            {
                s = ReadLine();
                while (!s.Contains(">>"))
                {
                    s += ReadLine();
                }
                currentTrailer = new Trailer(s);
                trailer.Add(currentTrailer);
            }
            #endregion
            #region read all trailers
            while (currentTrailer.Prev != 0)
            {
                adxref = currentTrailer.Prev;// FindXrefAddress(FS.Position - 11);
                FS.Seek(adxref, SeekOrigin.Begin);
                entriesTxt = ReadLine();
                entriesTxt = ReadLine();
                ent = entriesTxt.Split(' ');
                nbEntries = int.Parse(ent[1]);
                for (int u = 0; u < nbEntries; u++)
                {
                    string entry = ReadLine();
                    entries.Add(entry);
                }
                #region read new trailer
                string k = ReadLine();
                while (!k.Contains("trailer"))
                    k = ReadLine();
                currentTrailer = new Trailer(ReadLine());
                trailer.Add(currentTrailer);
                #endregion
            }
            #endregion
            #region Find objects offsets
            for (int u = 0; u < entries.Count; u++)
            {
                try
                {
                    FindObjectRef(u);
                    Trace.WriteLine(u.ToString());
                }
                catch (Exception e) { }
            }
            #endregion
            FS.Close();
            #region Computes Structure
            try
            {
                FindDocumentStructure();
            }
            catch { }
            #endregion
        }
        public void ParseDic(PdfObject pf)
        {
            //   if ((pf.Dictionnary == null) || (pf.Dictionnary.Count == 0))
            //       return;
            if (pf.Number == 1)
            {
            }
            foreach (KeyValuePair<string, string> kvp in pf.Dictionnary)
            {
                #region Dictionnary
                if (!kvp.Key.Contains("Parent") && !kvp.Key.Contains("Prev") && kvp.Value.Contains(" R"))
                {
                    try
                    {
                        string[] n = kvp.Value.Replace("[", "").Replace("]", "").Trim().Split(' ');
                        int lev = Array.IndexOf(n, "R", 0);
                        while ((lev >= 0) && (lev < n.Length))
                        {
                            int num = -1;
                            if (lev >= 2)
                                num = int.Parse(n[lev - 2]);
                            if (num >= 0)
                            {
                                PdfObject pdf;
                                if (!objects.TryGetValue(num, out pdf))
                                    pdf = ReadObjDescription(num);
                                if (pdf != null)
                                {
                                    if (pdf.ObjectType == null)
                                        pdf.ObjectType += kvp.Key;
                                    pf.AddAttribute(pdf);
                                    switch (kvp.Key)
                                    {
                                        case "D":
                                            break;
                                        case "A":
                                            break;
                                        case "Limits":
                                        case "Outlines":
                                        case "Next":
                                        case "First":
                                            //      ParseDicRec(pdf);
                                            break;
                                        case "Last":
                                            break;
                                        case "Annots":
                                            string[] pages = pdf.RawData.Replace("[", "").Replace("]", "").Split(' ');
                                            int i = 0;
                                            while (i < pages.Length)
                                            {
                                                PdfObject ann;
                                                if (!objects.TryGetValue(int.Parse(pages[i]), out ann))
                                                    ann = ReadObjDescription(int.Parse(pages[i]));
                                                i += 3;
                                                if (ann != null)
                                                    pdf.AddAttribute(ann);
                                            }
                                            break;
                                        case "Contents":
                                            switch (pf.ObjectType)
                                            {
                                                case "Page":
                                                    if (pf.RawStreamData == null)
                                                        pf.RawStreamData = pdf.RawStreamData;
                                                    else
                                                    {
                                                        byte[] buf = new byte[pdf.RawStreamData.Length + pf.RawStreamData.Length];
                                                        Buffer.BlockCopy(pf.RawStreamData, 0, buf, 0, pf.RawStreamData.Length);
                                                        Buffer.BlockCopy(pdf.RawStreamData, 0, buf, pf.RawStreamData.Length, pdf.RawStreamData.Length);
                                                        pdf.StreamData = buf;
                                                    }
                                                    Encoding  enc = Encoding.Default;
                                                    pdf.DecodedStream = enc.GetString(pdf.RawStreamData);
                                                    break;
                                            }
                                            break;
                                        default:
                                            if (pdf != null)
                                            {
                                                switch (pf.ObjectType)
                                                {
                                                    case "Page":
                                                        Encoding enc = Encoding.Default;
                                                        if (pdf.StreamData != null)
                                                            pdf.DecodedStream = enc.GetString(pdf.StreamData);
                                                        break;
                                                    case "Metadata":
                                                        pdf.DecodedStream = encoder.GetString(pdf.RawStreamData);
                                                        break;
                                                    case "FontDescriptor":
                                                        break;
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            lev = Array.IndexOf(n, "R", lev + 1);
                        }
                    }
                    catch (Exception e) { Trace.WriteLine(e.Message); }
                }
                #endregion
            }
            if (pf.RawStreamData != null)
            {
                byte[] b = Decompress(pf);
                if (b != null)
                    pf.DecodedStream = encoder.GetString(b);
            }
            if (pf.RawData != null)
                if (pf.RawData.Contains("R"))
                {
                    string[] n = pf.RawData.Replace("[", "").Replace("]", "").Trim().Split(' ');
                    int lev = Array.IndexOf(n, "R", 0);
                    while ((lev >= 0) && (lev < n.Length))
                    {
                        int num = -1;
                        if (lev >= 2)
                            num = int.Parse(n[lev - 2]);
                        if (num >= 0)
                        {
                            PdfObject pdf;
                            if (!objects.TryGetValue(num, out pdf))
                                pdf = ReadObjDescription(num);
                            if (pdf != null)
                            {
                                //                          if (pdf.ObjectType == null)                                 pdf.ObjectType += kvp.Key;
                                pf.AddAttribute(pdf);
                            }
                            lev = Array.IndexOf(n, "R", lev + 1);
                        }
                    }
                }
        }
        public void Close()
        {
            if (FS != null)
                FS.Close();
        }
    }
    public class Trailer
    {
        private int docSize;
        private int root;
        private int info;
        private int prev;
        private int xRefStm;
        public int XRefStm
        {
            get { return xRefStm; }
            set { xRefStm = value; }
        }
        public int Prev
        {
            get { return prev; }
            set { prev = value; }
        }
        public int DocSize
        {
            get { return docSize; }
            set { docSize = value; }
        }
        public int Root
        {
            get { return root; }
            set { root = value; }
        }
        public int Info
        {
            get { return info; }
            set { info = value; }
        }
        public Trailer(string t)
        {
            string[] tdat = t.Replace(">>", "").Split('/');
            foreach (string ts in tdat)
            {
                string[] tt = ts.Split(' ');
                if (tt.Length > 1)
                {
                    try
                    {
                        string dd = tt[0];
                        int d = int.Parse(tt[1]);
                        switch (tt[0])
                        {
                            case "Size":
                                docSize = d;
                                break;
                            case "Root":
                                root = d;
                                break;
                            case "Info":
                                info = d;
                                break;
                            case "Prev":
                                prev = d;
                                break;
                            case "XRefStm":
                                XRefStm = d;
                                break;
                        }
                    }
                    catch { }
                }
            }
        }
    }
    public class PdfObject
    {
        public string data;
        #region Private data
        private List<string> dictionnary = new List<string>();
        private byte[] streamData;
        private string decodedStream;
        private string rawData;
        private byte[] rawStreamData;
        private string subType;
        private List<PdfObject> attributes = new List<PdfObject>();
        private string objectType;
        private int number;
        private string gen;
        private string t;
        #endregion
        #region Public data
        public string SubType
        {
            get { return subType; }
            set { subType = value; }
        }
        public byte[] RawStreamData
        {
            get { return rawStreamData; }
            set { rawStreamData = value; }
        }
        public string ObjectType
        {
            get { return objectType; }
            set { objectType = value; }
        }
        public List<PdfObject> Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }
        public string RawData
        {
            get { return rawData; }
            set { rawData = value; }
        }
        public string DecodedStream
        {
            get { return decodedStream; }
            set { decodedStream = value; }
        }
        public byte[] StreamData
        {
            get { return streamData; }
            set { streamData = value; }
        }
        public Dictionary<string, string> Dictionnary
        {
            get { return dict; }
        }
        public Dictionary<string, string> dict = new Dictionary<string, string>();
        #endregion
        public int Number
        {
            get { return number; }
            set { number = value; }
        }
        public PdfObject(string num, string g, string type)
        {

            number = int.Parse(num);
            gen = g;
            t = type;
        }
        public void AddAttribute(PdfObject pdf)
        {
            bool found = false;
            foreach (PdfObject att in Attributes)
            {
                if (att.Number == pdf.Number)
                {
                    found = true;
                }
            }
            if (!found)
                attributes.Add(pdf);
        }
        public void AddToDictionnary(string l)
        {
            string pattern = @"/|<<|>>|[\[\(\)\]\.-]|[\w*+*-*:*]*";
            MatchCollection mc = Regex.Matches(l, pattern);
            List<string> patt = new List<string>();
            foreach (Match m in mc)
                if ((m.Value != " ") && (m.Value != ""))
                    patt.Add(m.Value);
            dictionnary.Add(l);
            string key = "";
            string value = "";
            int u = 0;
            if (patt[0] == "<<")
            {
                u++;
            }
            else
                return;
            while (u < patt.Count)
            {
                if (patt[u] == "/")
                    u++;
                key = patt[u];
                if (key == "BaseFont")
                {
                }
                while ((patt[u] == " ") || (patt[u] == ((char)0x0d).ToString()) || (patt[u] == ((char)0x0a).ToString()))
                    u++;
                value = GetData(patt, ref u);
                if (key == "Type")
                {
                    objectType = value;
                }
                if (key == "Subtype")
                {
                    subType = value;
                }

                if (key != "/")
                    try
                    {
                        dict.Add(key, value);
                    }
                    catch { }
                u++;
            }
        }
        public override string ToString()
        {
            string t = number.ToString() + " obj : " + objectType;
            return t;
        }
        private string GetData(List<string> patt, ref int u)
        {
            string value = "";
            u++;
            switch (patt[u])
            {
                case "/":
                    u++;
                    while ((patt[u] != "/") && (patt[u] != ">>"))
                    {
                        value += patt[u];
                        u++;
                    }
                    break;
                case "(":
                    do
                    {
                        value += patt[u] + " ";
                        u++;
                    }
                    while ((patt[u] != ")") && (patt[u] != ">>"));
                    value += patt[u];
                    u++;
                    break;
                case "[":
                    int inbra = 0;
                    inbra++;
                    value += patt[u];
                    u++;
                    while (inbra > 0)
                    {
                        if (patt[u] == "]") inbra--;
                        if (patt[u] == "[") inbra++;
                        value += patt[u] + " ";
                        u++;
                    }
                    break;
                case "<<":
                    while (patt[u] != ">>")
                    {
                        value += patt[u] + " ";
                        u++;
                    }
                    value += patt[u];
                    u++;

                    break;
                default:
                    while ((u < patt.Count) && (patt[u] != "/") && (patt[u] != ">>"))
                    {
                        value += patt[u] + " ";
                        u++;
                    }
                    break;
            }
            return value;
        }
    }
}
