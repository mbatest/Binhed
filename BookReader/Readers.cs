using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using Utils;

namespace BookReader
{
    public class Reader : LOCALIZED_DATA
    {
        private Header hdr;

        public Header Header
        {
            get { return hdr; }
            set { hdr = value; }
        }

        public string text;
        public Reader()
        {
        }
     }
    public class MobiFileReader : Reader
    {
        public MobiFileReader(string FileName)
        {
            BitStreamReader sw = new BitStreamReader(FileName, true);
            Header = new Header(sw);
            sw.Close();
            ReadPage(FileName, 1);
            for (int i = 0; i < 10/*Header.PdHr.Index.Count*/; i++)
            {
                Header.PdHr.Index[i].Page= Read(FileName, i);
             }
        }
        public void ReadPage(string FileName, int pageNumber)
        {
            BitStreamReader sw = new BitStreamReader(FileName, false);
            sw.Position = (int)Header.PdHr.Index[pageNumber].Offset.Value;
            int length = (int)Header.PdHr.Index[pageNumber + 1].Offset.Value - (int)Header.PdHr.Index[pageNumber].Offset.Value;
            text = Uncompress(sw.ReadBytes(length));
        }
        public PrcPage Read(string FileName, int pageNumber)
        {
            BitStreamReader sw = new BitStreamReader(FileName, false);
            sw.Position = (int)Header.PdHr.Index[pageNumber].Offset.Value;
            PrcPage p = new PrcPage(sw, PageLength(pageNumber));
            return p;
        }
        public int PageLength(int pageNumber)
        {
            return (int)Header.PdHr.Index[pageNumber + 1].Offset.Value - (int)Header.PdHr.Index[pageNumber].Offset.Value;       
        }
        public static string Uncompress(byte[] buffer)
        {
            #region Uncompress
            //http://wiki.mobileread.com/wiki/PalmDOC 
            Encoding enc = Encoding.Default;
            string t = "";
            int i = 0;
            while (i < buffer.Length)
            {
                try
                {
                    if (buffer[i] == 0x00)
                    {
                        t += enc.GetString(buffer, i, 1);
                        i++;
                    }
                    else if ((buffer[i] >= 0x01) && (buffer[i] <= 0x08))
                    {
                        int start = i + 1;
                        int end = start + buffer[i];
                        i++;
                        for (int j = start; j < end; j++)
                        {
                            string a = enc.GetString(buffer, j, 1);
                            t += a;
                            i++;
                        }
                    }
                    else if ((buffer[i] >= 0x09) && (buffer[i] <= 0x7F))
                    {
                        t += enc.GetString(buffer, i, 1);
                        i++;
                    }
                    else if ((buffer[i] >= 0x80) && (buffer[i] <= 0xbF))
                    {
                        int dist1 = buffer[i] & 0x3F;
                        if (buffer[i] == 0x80)
                        { }
                        int dist2 = buffer[i + 1] & 0xF8;
                        int dist = (256 * dist1 + dist2) >> 3;
                        int lengthData = buffer[i + 1] & 0x07;
                        try
                        {
                            t += t.Substring(t.Length - dist, lengthData + 3);
                        }
                        catch (Exception e)
                        {
                            for (int u = 0; u < lengthData + 3; u++)
                                t += " ";
                        }
                        i += 2;
                    }
                    else if ((buffer[i] >= 0xc0) && (buffer[i] <= 0xFF))
                    {
                        byte[] bb = new byte[1];
                        bb[0] = Convert.ToByte(buffer[i] ^ 0x80);
                        t += " " + enc.GetString(bb, 0, 1);
                        i++;
                    }
                }
                catch (Exception ex) { }
            }
            #endregion
            return t;
        }
    }
    #region Utility classes for Mobi
    public class indexEntry : LOCALIZED_DATA
    {
        private ELEMENTARY_TYPE number;
        private ELEMENTARY_TYPE offset;
        private ELEMENTARY_TYPE type;
        private PrcPage page;
        public ELEMENTARY_TYPE Number
        {
            get { return number; }
            set { number = value; }
        }
        public ELEMENTARY_TYPE Offset
        {
            get { return offset; }
            set { offset = value; }
        }
        public ELEMENTARY_TYPE Type
        {
            get { return type; }
            set { type = value; }
        }
        public PrcPage Page
        {
            get { return page; }
            set { page = value; }
        }
        public indexEntry(BitStreamReader sw, bool sh)
        {
            PositionOfStructureInFile = sw.Position;
            if (sh)
            {
                offset = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(int)sw.ReadInteger();
                number = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(int)sw.ReadInteger();
            }
            else
            {
                offset = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(int)sw.ReadInteger();
                type = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 3);//sw.ReadString(3);
                number = new ELEMENTARY_TYPE(sw, 0, typeof(byte));//int)sw.ReadByte();
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        private void ReadPage(string FileName)
        {
            BitStreamReader sw = new BitStreamReader(FileName, false);
            sw.Position = (int)Offset.Value;
     /*       int length = (int)Header.PdHr.Index[numberOfPages + 1].Offset.Value - (int)Header.PdHr.Index[numberOfPages].Offset.Value;
            text = MobiFileReader.ReadUncompressedText(sw,length);*/
        }
        public override string ToString()
        {
            return page.Text.Substring(0, 50);
        }
    }
    public class exthEntry : LOCALIZED_DATA
    {
        private ELEMENTARY_TYPE typeext;
        private ELEMENTARY_TYPE length;
        private ELEMENTARY_TYPE data;

        public ELEMENTARY_TYPE Typeext
        {
            get { return typeext; }
            set { typeext = value; }
        }
        public ELEMENTARY_TYPE Length
        {
            get { return length; }
            set { length = value; }
        }
        public ELEMENTARY_TYPE Data
        {
            get { return data; }
            set { data = value; }
        }
        public string TypeString
        {
            get
            {
                switch ((int)typeext.Value)
                {
                    case 1: return "drm_server_id";
                    case 2: return "drm_commerce_id";
                    case 3: return "drm_ebookbase_book_id";
                    case 100: return "author";
                    case 101: return "publisher";
                    case 102: return "imprint";
                    case 103: return "description";
                    case 104: return "isbn";
                    case 105: return "subject";
                    case 106: return "publishingdate";
                    case 107: return "review";
                    case 108: return "contributor";
                    case 109: return "rights";
                    case 110: return "subjectcode";
                    case 111: return "type";
                    case 112: return "source";
                    case 113: return "asin";
                    case 114: return "versionnumber";
                    case 115: return "sample";
                    case 116: return "startreading";
                    case 118: return "retail price (as text)";
                    case 119: return "retail price currency (as text)";
                    case 201: return "coveroffset";
                    case 202: return "thumboffset";
                    case 203: return "hasfakecover";
                    case 204: return "204 Unknown";
                    case 205: return "205 Unknown";
                    case 206: return "206 Unknown";
                    case 207: return "207 Unknown";
                    case 208: return "208 Unknown";
                    case 300: return "300 Unknown";
                    case 401: return "clippinglimit";
                    case 402: return "publisherlimit";
                    case 403: return "403 Unknown";
                    case 404: return "404 ttsflag";
                    case 501: return "cdetype";
                    case 502: return "lastupdatetime";
                    case 503: return "updatedtitle";
                    default: return "";
                }
            }
        }
        public exthEntry(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            typeext = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(int)sw.ReadInteger();
            length = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(int)sw.ReadInteger();
            data = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (int)length.Value - 8);// sw.ReadString((int)length.Value - 8);
            LengthInFile = sw.Position - sw.Position;
        }
    }
    public class palmDocHdr : LOCALIZED_DATA
    {
        private ELEMENTARY_TYPE title;
        private ELEMENTARY_TYPE attribs;
        private ELEMENTARY_TYPE version;
        private ELEMENTARY_TYPE create;
        private ELEMENTARY_TYPE modif;
        private ELEMENTARY_TYPE backup;
        private ELEMENTARY_TYPE modificationNumber;
        private ELEMENTARY_TYPE appInfo;
        private ELEMENTARY_TYPE sortInfo;
        private ELEMENTARY_TYPE type;
        private ELEMENTARY_TYPE creator;
        private ELEMENTARY_TYPE uniqueId;
        private ELEMENTARY_TYPE nextRecId;
        private ELEMENTARY_TYPE recordsNumber;
        private List<indexEntry> index = new List<indexEntry>();
        private DateTime createDate;
        private DateTime modifDate;
        private DateTime backupDate;

        public ELEMENTARY_TYPE Title
        {
            get { return title; }
            set { title = value; }
        }
        public ELEMENTARY_TYPE Attribs
        {
            get { return attribs; }
            set { attribs = value; }
        }
        public ELEMENTARY_TYPE Version
        {
            get { return version; }
            set { version = value; }
        }
        public ELEMENTARY_TYPE Create
        {
            get { return create; }
            set { create = value; }
        }
        public ELEMENTARY_TYPE Modif
        {
            get { return modif; }
            set { modif = value; }
        }
        public ELEMENTARY_TYPE Backup
        {
            get { return backup; }
            set { backup = value; }
        }
        public ELEMENTARY_TYPE ModificationNumber
        {
            get { return modificationNumber; }
            set { modificationNumber = value; }
        }
        public ELEMENTARY_TYPE AppInfo
        {
            get { return appInfo; }
            set { appInfo = value; }
        }
        public ELEMENTARY_TYPE SortInfo
        {
            get { return sortInfo; }
            set { sortInfo = value; }
        }
        public ELEMENTARY_TYPE Type
        {
            get { return type; }
            set { type = value; }
        }
        public ELEMENTARY_TYPE Creator
        {
            get { return creator; }
            set { creator = value; }
        }
        public ELEMENTARY_TYPE UniqueId
        {
            get { return uniqueId; }
            set { uniqueId = value; }
        }
        public ELEMENTARY_TYPE NextRecId
        {
            get { return nextRecId; }
            set { nextRecId = value; }
        }
        public ELEMENTARY_TYPE RecordsNumber
        {
            get { return recordsNumber; }
            set { recordsNumber = value; }
        }
        public DateTime CreateDate
        {
            get { return createDate; }
            set { createDate = value; }
        }
        public DateTime ModifDate
        {
            get { return modifDate; }
            set { modifDate = value; }
        }
        public DateTime BackupDate
        {
            get { return backupDate; }
            set { backupDate = value; }
        }

        public List<indexEntry> Index
        {
            get { return index; }
            set { index = value; }
        }
        public palmDocHdr(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            title = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 32);// sw.ReadString(32);
            attribs = new ELEMENTARY_TYPE(sw, 0, typeof(ushort));// sw.ReadShort();
            version = new ELEMENTARY_TYPE(sw, 0, typeof(ushort));//(uint)sw.ReadShort();//
            create = new ELEMENTARY_TYPE(sw, 0, typeof(int));//
            modif = new ELEMENTARY_TYPE(sw, 0, typeof(int));//
            backup = new ELEMENTARY_TYPE(sw, 0, typeof(int));//
            createDate = new DateTime(1970, 1, 1) + new TimeSpan(0, 0, (int)create.Value);
            modifDate = new DateTime(1970, 1, 1) + new TimeSpan(0, 0, (int)modif.Value);
            backupDate = new DateTime(1970, 1, 1) + new TimeSpan(0, 0, (int)backup.Value);
            modificationNumber = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(int)sw.ReadInteger();
            appInfo = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(int)sw.ReadInteger();
            sortInfo = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();
            type = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);// sw.ReadString(4);
            creator = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);// sw.ReadString(4);
            uniqueId = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(int)sw.ReadInteger();
            nextRecId = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(int)sw.ReadInteger();
            #region Read PalmDoc Header
            recordsNumber = new ELEMENTARY_TYPE(sw, 0, typeof(short));//(int)sw.ReadShort();
            #endregion
            if ((string)creator.Value == "MOBI")
            {
                for (int u = 0; u < (short)recordsNumber.Value; u++)
                {
                    index.Add(new indexEntry(sw, true));
                }
            }
            else
            {
                for (int u = 0; u < (short)recordsNumber.Value; u++)
                {
                    index.Add(new indexEntry(sw, false));
                }
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "";
        }
    }
    public class mobiHdr : LOCALIZED_DATA
    {
        private ELEMENTARY_TYPE identifier;
        private ELEMENTARY_TYPE hdrLength;
        private ELEMENTARY_TYPE mbType;
        private ELEMENTARY_TYPE texEncoding;
        private ELEMENTARY_TYPE uniqueId;
        private ELEMENTARY_TYPE generatorVersion;
        private ELEMENTARY_TYPE firstNonBook;
        private ELEMENTARY_TYPE fullNameOffset;
        private ELEMENTARY_TYPE fullNamelength;
        private ELEMENTARY_TYPE locale;
        private ELEMENTARY_TYPE inpLang;
        private ELEMENTARY_TYPE outLang;
        private ELEMENTARY_TYPE mobiVersion;
        private ELEMENTARY_TYPE formatVersion;
        private ELEMENTARY_TYPE firstImIndex;
        private ELEMENTARY_TYPE exthflags;
        private ELEMENTARY_TYPE drmOffset;
        private ELEMENTARY_TYPE drmCOunt;
        private ELEMENTARY_TYPE drmSize;
        private ELEMENTARY_TYPE drmFlags;
        public ELEMENTARY_TYPE Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        }
        public ELEMENTARY_TYPE HdrLength
        {
            get { return hdrLength; }
            set { hdrLength = value; }
        }

        public ELEMENTARY_TYPE MbType
        {
            get { return mbType; }
            set { mbType = value; }
        }
        public ELEMENTARY_TYPE TexEncoding
        {
            get { return texEncoding; }
            set { texEncoding = value; }
        }
        public ELEMENTARY_TYPE UniqueId
        {
            get { return uniqueId; }
            set { uniqueId = value; }
        }
        public ELEMENTARY_TYPE GeneratorVersion
        {
            get { return generatorVersion; }
            set { generatorVersion = value; }
        }
        public ELEMENTARY_TYPE FirstNonBook
        {
            get { return firstNonBook; }
            set { firstNonBook = value; }
        }
        public ELEMENTARY_TYPE FullNameOffset
        {
            get { return fullNameOffset; }
            set { fullNameOffset = value; }
        }
        public ELEMENTARY_TYPE FullNamelength
        {
            get { return fullNamelength; }
            set { fullNamelength = value; }
        }
        public ELEMENTARY_TYPE Locale
        {
            get { return locale; }
            set { locale = value; }
        }
        public ELEMENTARY_TYPE InpLang
        {
            get { return inpLang; }
            set { inpLang = value; }
        }
        public ELEMENTARY_TYPE OutLang
        {
            get { return outLang; }
            set { outLang = value; }
        }
        public ELEMENTARY_TYPE MobiVersion
        {
            get { return mobiVersion; }
            set { mobiVersion = value; }
        }
        public ELEMENTARY_TYPE FormatVersion
        {
            get { return formatVersion; }
            set { formatVersion = value; }
        }

        public ELEMENTARY_TYPE FirstImIndex
        {
            get { return firstImIndex; }
            set { firstImIndex = value; }
        }
        public ELEMENTARY_TYPE Exthflags
        {
            get { return exthflags; }
            set { exthflags = value; }
        }
        public ELEMENTARY_TYPE DrmOffset
        {
            get { return drmOffset; }
            set { drmOffset = value; }
        }
        public ELEMENTARY_TYPE DrmCOunt
        {
            get { return drmCOunt; }
            set { drmCOunt = value; }
        }
        public ELEMENTARY_TYPE DrmSize
        {
            get { return drmSize; }
            set { drmSize = value; }
        }
        public ELEMENTARY_TYPE DrmFlags
        {
            get { return drmFlags; }
            set { drmFlags = value; }
        }
        public string MobiType
        {
            get
            {
                switch ((int)mbType.Value)
                {
                    case 2:
                        return "Mobipocket Book";

                    case 3:
                        return " PalmDoc Book";

                    case 4:
                        return " Audio";

                    case 257:
                        return " News";

                    case 258:
                        return " News_Feed";

                    case 259:
                        return "News_Magazine";

                    case 513:
                        return " PICS";

                    case 514:
                        return " WORD";

                    case 515:
                        return " XLS";

                    case 516:
                        return " PPT";

                    case 517:
                        return " TEXT";

                    case 518:
                        return " HTML";
                    default:
                        return "";
                }
            }
        }
        public mobiHdr(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            int startHeader = (int)sw.Position;
            identifier = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);// sw.ReadString(4);
            hdrLength = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();//the lengthString of the MOBI FileHeader, including the previous 4 bytes 
            int endHeadr = (int)sw.Position - 8 + (int)hdrLength.Value;
            mbType = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();
            texEncoding = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();
            uniqueId = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();
            generatorVersion = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();
            sw.ReadBytes(40);
            firstNonBook = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();
            fullNameOffset = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();
            fullNamelength = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();
            int fullNameOfswet = startHeader + (int)fullNameOffset.Value;
            locale = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();
            inpLang = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();
            outLang = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();
            mobiVersion = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();
            firstImIndex = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();
            sw.ReadBytes(16);// 0
            exthflags = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();//ExtFlag if sw.ReadBit 6 there is an EXTH
            sw.ReadBytes(32);
            drmOffset = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger(); //DRM
            drmCOunt = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();//DRM
            drmSize = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();//DRM
            drmFlags = new ELEMENTARY_TYPE(sw, 0, typeof(int));// (int)sw.ReadInteger();//DRM
            int bytesTotheEnd = (int)sw.ReadInteger();
            sw.ReadBytes(2);
            LengthInFile = sw.Position - PositionOfStructureInFile;
            sw.Position = endHeadr;          
        }

    }
    public class ExtHdr : LOCALIZED_DATA
    {
        private ELEMENTARY_TYPE identifier;
        private ELEMENTARY_TYPE hdrLength;
        private ELEMENTARY_TYPE recCount;
        public ELEMENTARY_TYPE Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        }
        public ELEMENTARY_TYPE HdrLength
        {
            get { return hdrLength; }
            set { hdrLength = value; }
        }
        public ELEMENTARY_TYPE RecCount
        {
            get { return recCount; }
            set { recCount = value; }
        }
        public List<exthEntry> index = new List<exthEntry>();
        public ExtHdr(BitStreamReader sw)
        {
            identifier = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);//sw.ReadString(4);
            hdrLength = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(int)sw.ReadInteger();
            recCount = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(int)sw.ReadInteger();
        }
    }
    public class Header : LOCALIZED_DATA
    {
        private string author;
        private string title;
        public string creator;
        public string language;
        public string publisher;
        public string date;
        public string bookId;

        private palmDocHdr pdHr;
        private ELEMENTARY_TYPE compression;

        public string Author
        {
            get { return author; }
            set { author = value; }
        }
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public palmDocHdr PdHr
        {
            get { return pdHr; }
            set { pdHr = value; }
        }
        public ELEMENTARY_TYPE Compression
        {
            get { return compression; }
            set { compression = value; }
        }
        public string CompressionName
        {
            get
            {
                if (compression != null)
                    switch ((short)compression.Value)
                    {
                        case 0:
                            return "No compression";
                        case 1:
                            return "Old Mobipocket Encryption";
                        case 2:
                            return "Mobipocket Encryption ";
                        default: return "";
                    }
                else return "";
            }
        }
        #region Only Mobi
        private ELEMENTARY_TYPE textLength;
        private ELEMENTARY_TYPE recordCount;
        private ELEMENTARY_TYPE recordSize;
        private ELEMENTARY_TYPE currentPosition;
        private mobiHdr mHdr;
        private ExtHdr extHdr;
        public ELEMENTARY_TYPE TextLength
        {
            get { return textLength; }
            set { textLength = value; }
        }
        public ELEMENTARY_TYPE RecordCount
        {
            get { return recordCount; }
            set { recordCount = value; }
        }
        public ELEMENTARY_TYPE RecordSize
        {
            get { return recordSize; }
            set { recordSize = value; }
        }
        public ELEMENTARY_TYPE CurrentPosition
        {
            get { return currentPosition; }
            set { currentPosition = value; }
        }
        public mobiHdr MHdr
        {
            get { return mHdr; }
            set { mHdr = value; }
        }
        public ExtHdr ExtHdr
        {
            get { return extHdr; }
            set { extHdr = value; }
        }
        #endregion
        public Header(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            pdHr = new palmDocHdr(sw);
            #region Read PalmDoc Header
 //           int recordsNumber = (int)sw.ReadShort();
            #endregion
            if ((string) pdHr.Creator.Value == "MOBI")
            {
                #region MOBI FileHeader
                sw.ReadBytes(2);//Gap to data
                compression = new ELEMENTARY_TYPE(sw, 0, typeof(short));//(uint)sw.ReadShort();//Compression
                sw.ReadBytes(2);//unused
                textLength =new ELEMENTARY_TYPE(sw,0,typeof(int));//(int)sw.ReadInteger();
                recordCount = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(uint)sw.ReadShort();//Number of PDB records used for the text of the book.
                recordSize = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(uint)sw.ReadShort();//Number of PDB records used for the text of the book.
                currentPosition = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(int)sw.ReadInteger();//current position in the text
                mHdr = new mobiHdr(sw);
                if ((int)mHdr.Exthflags.Value != 0)
                {
                    extHdr = new ExtHdr(sw);
                    for (int u = 0; u < (int)extHdr.RecCount.Value; u++)
                    {
                        exthEntry entry = new exthEntry(sw);
                        extHdr.index.Add(entry);
                        switch ((int)entry.Typeext.Value)
                        {
                            case 100:
                                author = (string)entry.Data.Value;
                                break;
                            case 101:
                                publisher = (string)entry.Data.Value;
                                break;
                        }
                    }

                }
                int start = (int)pdHr.Index[0].Offset.Value + (int)mHdr.FullNameOffset.Value;
                sw.Position = start;
                title = sw.ReadString((int)mHdr.FullNamelength.Value);
                #endregion
            }
            else
            {
                #region TEXt FileHeader
                sw.Position = (int)pdHr.Index[0].Offset.Value;
                sw.Position = (int)pdHr.Index[1].Offset.Value;
                string text = MobiFileReader.Uncompress(sw.ReadBlock(800)); 
                int i = text.IndexOf("Title>") + "Title>".Length;
                int j = text.IndexOf('<', i);
                title = text.Substring(i, j - i);
                i = text.IndexOf("Creator>", j) + "Creator>".Length;
                j = text.IndexOf('<', i);
                author = text.Substring(i, j - i);
                i = text.IndexOf("Date>", j) + "Date>".Length;
                j = text.IndexOf('<', i);
                date = text.Substring(i, j - i);
                sw.Position = (int)pdHr.Index[2].Offset.Value;
                #endregion
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class PrcPage : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE data;
        string text;

        public ELEMENTARY_TYPE Data
        {
            get { return data; }
            set { data = value; }
        }
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        public PrcPage(BitStreamReader sw, int length)
        {
            PositionOfStructureInFile = sw.Position;
            data = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), length);
            LengthInFile = length;
             text = MobiFileReader.Uncompress((byte[])data.Value);
       }
        public override string ToString()
        {
            return text.Substring(0, 40);
        }
    }
    #endregion
}
