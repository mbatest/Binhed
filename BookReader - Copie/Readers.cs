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
        private string author;
        private string title;
        private string creator;
        private string language;
        private string publisher;
        private string date;
        private string bookId;
        [CategoryAttribute("Livre"), DescriptionAttribute("Auteur")]
        public string Author
        {
            get { return author; }
            set { author = value; }
        }
        [CategoryAttribute("Livre"), DescriptionAttribute("Titre")]
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        [CategoryAttribute("Livre"), DescriptionAttribute("Creator")]
        public string Creator
        {
            get { return creator; }
            set { creator = value; }
        }
        [CategoryAttribute("Livre"), DescriptionAttribute("Language")]
        public string Language
        {
            get { return language; }
            set { language = value; }
        }
        [CategoryAttribute("Livre"), DescriptionAttribute("Publisher")]
        public string Publisher
        {
            get { return publisher; }
            set { publisher = value; }
        }
        [CategoryAttribute("Livre"), DescriptionAttribute("Date")]
        public string Date
        {
            get { return date; }
            set { date = value; }
        }
        [CategoryAttribute("Livre"), DescriptionAttribute("BookId")]
        public string BookId
        {
            get { return bookId; }
            set { bookId = value; }
        }
        public string text;
        Header hdr = new Header();
        public FileStream FS;
        public Reader()
        {
        }
        public int ReadByte()
        {
            byte[] buffer = new byte[1];
            FS.Read(buffer, 0, 1);
            return (int)buffer[0];
        }
        public int ReadShortInteger()
        {
            uint data = 0;
            byte[] buffer = new byte[2];
            FS.Read(buffer, 0, 2);
            data = Convert.ToUInt16(((uint)buffer[0] << 8) + ((uint)buffer[1]));
            return (int)data;
        }
        public string ReadString(int length)
        {
            byte[] buffer = new byte[length];
            FS.Read(buffer, 0, length);
            int strLength = 0;
            while ((strLength < length) && (buffer[strLength] != 0x00))
                strLength++;
            return Encoding.Default.GetString(buffer, 0, strLength);
        }
        public uint ReadInteger()
        {
            uint data = 0;
            byte[] buffer = new byte[4];
            FS.Read(buffer, 0, 4);
            data = Convert.ToUInt32(((uint)buffer[0] << 24) + ((uint)buffer[1] << 16) + ((uint)buffer[2] << 8) + ((uint)buffer[3]));
            return data;
        }
    }
    public class MobiFileReader : Reader
    {
        Header hdr = new Header();
        public MobiFileReader(string FileName)
        {
            FS = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            #region Read PalmDoc Header
            hdr.pdHr.title = ReadString(32);
            byte[] buffer = new byte[256];
            hdr.pdHr.attribs = (uint)ReadShortInteger();
            hdr.pdHr.version = (uint)ReadShortInteger();//
            hdr.pdHr.create = new DateTime(1970, 1, 1) + new TimeSpan(0, 0, (int)ReadInteger());
            hdr.pdHr.modif = new DateTime(1970, 1, 1) + new TimeSpan(0, 0, (int)ReadInteger());
            hdr.pdHr.backup = new DateTime(1970, 1, 1) + new TimeSpan(0, 0, (int)ReadInteger());
            hdr.pdHr.modificationNumber = (int)ReadInteger();
            hdr.pdHr.appInfo = (int)ReadInteger();
            hdr.pdHr.sortInfo = (int)ReadInteger();
            hdr.pdHr.type = ReadString(4);
            hdr.pdHr.creator = ReadString(4);
            hdr.pdHr.uniqueId = (int)ReadInteger();
            hdr.pdHr.nextRecId = (int)ReadInteger();
            int recordsNumber = (int)ReadShortInteger();
            #endregion
            if (hdr.pdHr.creator == "MOBI")
            {
                #region MOBI FileHeader
                for (int u = 0; u < recordsNumber; u++)
                {
                    hdr.pdHr.index.Add(ReadIndexEntry(true));
                }
                FS.Read(buffer, 0, 2);//Gap to data
                hdr.compression = (uint)ReadShortInteger();//Compression
                FS.Read(buffer, 0, 2);//unused
                hdr.textLength = (int)ReadInteger();
                hdr.recordCount = (uint)ReadShortInteger();//Number of PDB records used for the text of the book.
                hdr.recordSize = (uint)ReadShortInteger();//Number of PDB records used for the text of the book.
                hdr.currentPosition = (int)ReadInteger();//current position in the text
                int startHeader = (int)FS.Position;
                hdr.mHdr.identifier = ReadString(4);
                hdr.mHdr.hdrLength = (int)ReadInteger();//the lengthString of the MOBI FileHeader, including the previous 4 bytes 
                int endHeadr = (int)FS.Position - 8 + hdr.mHdr.hdrLength;
                hdr.mHdr.mbType = (int)ReadInteger();
                hdr.mHdr.texEncoding = (int)ReadInteger();
                hdr.mHdr.uniqueId = (int)ReadInteger();
                hdr.mHdr.generatorVersion = (int)ReadInteger();
                FS.Read(buffer, 0, 40);
                hdr.mHdr.firstNonBook = (int)ReadInteger();
                hdr.mHdr.fullNameOffset = (int)ReadInteger();
                hdr.mHdr.fullNamelength = (int)ReadInteger();
                int fullNameOffset = startHeader + hdr.mHdr.fullNameOffset;
                hdr.mHdr.locale = (int)ReadInteger();
                hdr.mHdr.inpLang = (int)ReadInteger();
                hdr.mHdr.outLang = (int)ReadInteger();
                hdr.mHdr.mobiVersion = (int)ReadInteger();
                hdr.mHdr.firstImIndex = (int)ReadInteger();
                FS.Read(buffer, 0, 16);// 0
                hdr.mHdr.exthflags = (int)ReadInteger();//ExtFlag if ReadBit 6 there is an EXTH
                FS.Read(buffer, 0, 32);
                hdr.mHdr.drmOffset = (int)ReadInteger(); //DRM
                hdr.mHdr.drmCOunt = (int)ReadInteger();//DRM
                hdr.mHdr.drmSize = (int)ReadInteger();//DRM
                hdr.mHdr.drmFlags = (int)ReadInteger();//DRM
                int bytesTotheEnd = (int)ReadInteger();
                FS.Read(buffer, 0, 2);
                FS.Seek(endHeadr, SeekOrigin.Begin);
                if (hdr.mHdr.exthflags != 0)
                {
                    hdr.extHdr.identifier = ReadString(4);
                    hdr.extHdr.hdrLength = (int)ReadInteger();
                    hdr.extHdr.recCount = (int)ReadInteger();
                    for (int u = 0; u < hdr.extHdr.recCount; u++)
                    {
                        exthEntry entry = new exthEntry();
                        entry.typeext = (int)ReadInteger();
                        entry.length = (int)ReadInteger();
                        entry.data = ReadString(entry.length - 8);
                        hdr.extHdr.index.Add(entry);
                        switch (entry.typeext)
                        {
                            case 100:
                                Author = entry.data;
                                break;
                            case 101:
                                Publisher = entry.data;
                                break;
                        }
                    }

                }
                int start = hdr.pdHr.index[0].offset + hdr.mHdr.fullNameOffset;
                FS.Seek(start, SeekOrigin.Begin);
                Title = ReadString(hdr.mHdr.fullNamelength);
                #endregion
            }
            else
            {
                #region TEXt FileHeader
                for (int u = 0; u < recordsNumber; u++)
                {
                    hdr.pdHr.index.Add(ReadIndexEntry(false));
                }
                FS.Seek(hdr.pdHr.index[0].offset, SeekOrigin.Begin);
                FS.Seek(hdr.pdHr.index[1].offset, SeekOrigin.Begin);
                text = ReadUncompressedText(1000);
                int i = text.IndexOf("Title>") + "Title>".Length;
                int j = text.IndexOf('<', i);
                Title = text.Substring(i, j - i);
                i = text.IndexOf("Creator>", j) + "Creator>".Length;
                j = text.IndexOf('<', i);
                Author = text.Substring(i, j - i);
                i = text.IndexOf("Date>", j) + "Date>".Length;
                j = text.IndexOf('<', i);
                Date = text.Substring(i, j - i);
                FS.Seek(hdr.pdHr.index[2].offset, SeekOrigin.Begin);
                #endregion
            }
            FS.Close();
            ReadPage(FileName, 1);
        }
        public void ReadPage(string FileName, int pageNumber)
        {
            FS = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            FS.Seek(hdr.pdHr.index[pageNumber].offset, SeekOrigin.Begin);
            text = ReadUncompressedText(hdr.pdHr.index[pageNumber + 1].offset - hdr.pdHr.index[pageNumber].offset);
        }
        private indexEntry ReadIndexEntry(bool sh)
        {
            if (sh)
            {
                indexEntry ind = new indexEntry();
                ind.offset = (int)ReadInteger();
                ind.number = (int)ReadInteger();
                return ind;
            }
            else
            {
                indexEntry ind = new indexEntry();
                ind.offset = (int)ReadInteger();
                ind.type = ReadString(3);
                ind.number = (int)ReadByte();
                return ind;
            }
        }
        private string ReadUncompressedText(int length)
        {
            byte[] buffer = new byte[length];
            FS.Read(buffer, 0, length);
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
                catch (Exception ex) { return t; }
            }
            return t;
        }
    }
    #region Utility classes for Mobi
    public class indexEntry
    {
        public int number;
        public int offset;
        public string type;
    }
    public class exthEntry
    {
        public int typeext;
        public int length;
        public string data;
        public string TypeString
        {
            get
            {
                switch (typeext)
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
    }
    public class palmDocHdr
    {
        public string title;
        public uint attribs;
        public uint version;
        public DateTime create;
        public DateTime modif;
        public DateTime backup;
        public int modificationNumber;
        public int appInfo;
        public int sortInfo;
        public string type;
        public string creator;
        public int uniqueId;
        public int nextRecId;
        public List<indexEntry> index = new List<indexEntry>();
    }
    public class mobiHdr
    {
        public string identifier;
        public int hdrLength;
        public int mbType;
        public int texEncoding;
        public int uniqueId;
        public int generatorVersion;
        public int firstNonBook;
        public int fullNameOffset;
        public int fullNamelength;
        public int locale;
        public int inpLang;
        public int outLang;
        public int mobiVersion;
        public int formatVersion;
        public int firstImIndex;
        public int exthflags;
        public int drmOffset;
        public int drmCOunt;
        public int drmSize;
        public int drmFlags;
        public string MobiType
        {
            get
            {
                switch (mbType)
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
    }
    public class ExtHdr
    {
        public string identifier;
        public int hdrLength;
        public int recCount;
        public List<exthEntry> index = new List<exthEntry>();
    }
    public class Header
    {
        public palmDocHdr pdHr = new palmDocHdr();
        public uint compression;
        public string CompressionName
        {
            get
            {
                switch (compression)
                {
                    case 0:
                        return "No compression";
                    case 1:
                        return "Old Mobipocket Encryption";
                    case 2:
                        return "Mobipocket Encryption ";
                    default: return "";
                }
            }
        }
        public int textLength;
        public uint recordCount;
        public uint recordSize;
        public int currentPosition;
        public mobiHdr mHdr = new mobiHdr();
        public ExtHdr extHdr = new ExtHdr();
    }
    #endregion
}
