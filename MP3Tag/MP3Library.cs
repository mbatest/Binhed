using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.ComponentModel;
using Utils;

namespace MP3Library
{
 /*   public class TagDataReader
    {
        public event TagsEventHandler FileTreated;
        public event TagsDirectoryHandler DirectoryTreated;
        public TagDataReader(string path)
        {
            this.path = path;
        }
        public void ReadDirectoryAsync()
        {
            Thread t = new Thread(new ThreadStart(ReadDirectory));
            t.IsBackground = true;
            t.Start();
        }
        public void ReadDirectory()
        {
            TreeNode rootNode = ReadDirectory(path);
            if (DirectoryTreated != null)
                DirectoryTreated(new TagsDirectoryArgs(rootNode, musicFileNames));
        }
        public MusicFileClass ReadFile(string fileName)
        {
            bool isHidden = ((File.GetAttributes(fileName) & FileAttributes.Hidden) == FileAttributes.Hidden);
            if (isHidden)
                return null;
            string extension = Path.GetExtension(fileName.ToLower());
            TagsEventArgs tagName = null;
            switch (extension)
            {
                case ".mp3":
                case ".wma":
                    #region Music
                    MusicFileClass md = new MusicFileClass(fileName);
                    if (md != null)
                        musicFileNames.Add(md);
                    tagName = new TagsEventArgs(md);
                    break;
                    #endregion
                default:
                    tagName = new TagsEventArgs(null);
                    break;
            }
            if (FileTreated != null)
                FileTreated(tagName);
            return tagName.musicData;
        }
        #region Private members
        string path;
        List<MusicFileClass> musicFileNames = new List<MusicFileClass>();
        private TreeNode ReadDirectory(string path)
        {
            TreeNode tR = new TreeNode(path);
            string[] direct = Directory.GetDirectories(path);
            for (int i = 0; i < direct.Length; i++)
            {
                tR.Nodes.Add(ReadDirectory(direct[i]));
            }
            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = files[i];
                try
                {
                    if (FileTreated != null)
                    {
                    }
                }
                catch (Exception exception) { }

            }
            return tR;
        }
        #endregion
    }*/
    #region Music
    public class MusicFileClass : LOCALIZED_DATA
    {
        #region Private members
        #region Tags fileName
        //http://www.id3.org/id3v2.3.0
        private string mp3_AENC = "";//[#sec4.20 Audio encryption] 
        private string mp3_APIC = "";//[#sec4.15 Attached picture]
        private string mp3_COMM = "";//[#sec4.11 Comments]
        private string mp3_COMR = "";//[#sec4.25 Commercial frame]
        private string mp3_ENCR = "";//[#sec4.26 Encryption method registration]
        private string mp3_EQUA = "";//[#sec4.13 Equalization]
        private string mp3_ETCO = "";//[#sec4.6 Event timing codes]
        private string mp3_GEOB = "";//[#sec4.16 General encapsulated object]
        private string mp3_GRID = "";//[#sec4.27 Group identification registration]
        private string mp3_IPLS = "";//[#sec4.4 Involved people list]
        private string mp3_LINK = "";//[#sec4.21 Linked information]
        private string mp3_MCDI = "";//[#sec4.5 Music CD identifier]
        private string mp3_MLLT = "";//[#sec4.7 MPEG location lookup table]
        private string mp3_OWNE = "";//[#sec4.24 Ownership frame]
        private string mp3_PRIV = "";//[#sec4.28 Private frame]
        private string mp3_PCNT = "";//[#sec4.17 Play counter]
        private string mp3_POPM = "";//[#sec4.18 Popularimeter]
        private string mp3_POSS = "";//[#sec4.22 Position synchronisation frame]
        private string mp3_RBUF = "";//[#sec4.19 Recommended buffer size]
        private string mp3_RVAD = "";//[#sec4.12 Relative volume adjustment]
        private string mp3_RVRB = "";//[#sec4.14 Reverb]
        private string mp3_SYLT = "";//[#sec4.10 Synchronized lyric/text]
        private string mp3_SYTC = "";//[#sec4.8 Synchronized tempo codes]
        private string mp3_TALB = "";//[#TALB Album/Movie/Show title]
        private string mp3_TBPM = "";//[#TBPM BPM (beats per minute)]
        private string mp3_TCOM = "";//[#TCOM Composer]
        private string mp3_TCON = "";//[#TCON Content type]
        private string mp3_TCOP = "";//[#TCOP Copyright message]
        private string mp3_TDAT = "";//[#TDAT Date]
        private string mp3_TDLY = "";//[#TDLY Playlist delay]
        private string mp3_TENC = "";//[#TENC Encoded by]
        private string mp3_TEXT = "";//[#TEXT Lyricist/Text writer]
        private string mp3_TFLT = "";//[#TFLT File type]
        private string mp3_TIME = "";//[#TIME Time]
        private string mp3_TIT1 = "";//[#TIT1 Content group description]
        private string mp3_TIT2 = "";//[#TIT2 Title/songname/content description]
        private string mp3_TIT3 = "";//[#TIT3 Subtitle/Description refinement]
        private string mp3_TKEY = "";//[#TKEY Initial key]
        private string mp3_TLAN = "";//[#TLAN Language(s)]
        private string mp3_TLEN = "";//[#TLEN Length]
        private string mp3_TMED = "";//[#TMED Media type]
        private string mp3_TOAL = "";//[#TOAL Original album/movie/show title]
        private string mp3_TOFN = "";//[#TOFN Original filename]
        private string mp3_TOLY = "";//[#TOLY Original lyricist(s)/text writer(s)]
        private string mp3_TOPE = "";//[#TOPE Original artist(s)/performer(s)]
        private string mp3_TORY = "";//[#TORY Original release year]
        private string mp3_TOWN = "";//[#TOWN File owner/licensee]
        private string mp3_TPE1 = "";//[#TPE1 Lead performer(s)/Soloist(s)]
        private string mp3_TPE2 = "";//[#TPE2 Band/orchestra/accompaniment]
        private string mp3_TPE3 = "";//[#TPE3 Conductor/performer refinement]
        private string mp3_TPE4 = "";//[#TPE4 Interpreted, remixed, or otherwise modified by]
        private string mp3_TPOS = "";//[#TPOS Part of a set]
        private string mp3_TPUB = "";//[#TPUB Publisher]
        private string mp3_TRCK = "";//[#TRCK Track number/Position in set]
        private string mp3_TRDA = "";//[#TRDA Recording dates]
        private string mp3_TRSN = "";//[#TRSN Internet radio station name]
        private string mp3_TRSO = "";//[#TRSO Internet radio station owner]
        private string mp3_TSIZ = "";//[#TSIZ Size]
        private string mp3_TSRC = "";//[#TSRC ISRC (international standard recording code)]
        private string mp3_TSSE = "";//[#TSEE Software/Hardware and settings used for encoding]
        private string mp3_TYER = "";//[#TYER Year]
        private string mp3_TXXX = "";//[#TXXX User defined text information frame]
        private string mp3_UFID = "";//[#sec4.1 Unique file identifier]
        private string mp3_USER = "";//[#sec4.23 Terms of use]
        private string mp3_USLT = "";//[#sec4.9 Unsychronized lyric/text transcription]
        private string mp3_WCOM = "";//[#WCOM Commercial information]
        private string mp3_WCOP = "";//[#WCOP Copyright/Legal information]
        private string mp3_WOAF = "";//[#WOAF Official audio file webpage]
        private string mp3_WOAR = "";//[#WOAR Official artist/performer webpage]
        private string mp3_WOAS = "";//[#WOAS Official audio source webpage]
        private string mp3_WORS = "";//[#WORS Official internet radio station homepage]
        private string mp3_WPAY = "";//[#WPAY Payment]
        private string mp3_WPUB = "";//[#WPUB Publishers official webpage]
        private string mp3_WXXX = "";//[#WXXX User defined URL link frame]
        private Image albumArt;
        private int imageIndex;
        private int imageSize;
        private string artType;
        byte pictureType;
        string mimeType;
        #endregion
        private bool treated = false;
        private List<Composer> composers = new List<Composer>();
        private List<Author> authors = new List<Author>();
        private string fileName;
        int lengthInMs;

        public int LengthInMs
        {
            get { return lengthInMs; }
            set { lengthInMs = value; }
        }
        #endregion
        public bool Treated
        {
            get { return treated; }
            set { treated = value; }
        }
        #region Properties
        [CategoryAttribute("Music File"), DescriptionAttribute("Composers List")]
        public List<Composer> ComposersList
        {
            get { return composers; }
            set { composers = value; }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Authors List")]
        public List<Author> AuthorsList
        {
            get { return authors; }
            //          set { authors = value; }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Album Art")]
        public Image AlbumArt
        {
            get
            {
                try
                {
                    if (albumArt != null)
                        return albumArt;
                    //           FindAlbumArt(fileName);
                    else
                    {
                        Image im = Image.FromFile(LargeImagePath);
                        return im;
                    }
                }
                catch (Exception EX) { return null; }
            }
        }
        public String LargeImagePath
        {
            get
            {
                return Path.GetDirectoryName(FileName) + @"\AlbumArt_{" + WMCollectionId.ToString() + "}_Large.jpg";
            }
        }
        public String SmallImagePath
        {
            get
            {
                return Path.GetDirectoryName(FileName) + @"\AlbumArt_{" + WMCollectionId.ToString() + "}_Small.jpg";
            }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("FileName")]
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Album")]
        public string Album
        {
            get
            {
                return mp3_TALB;
            }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Music type")]
        public string MusicType
        {
            get
            {
                return mp3_TIT1;
            }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Title")]
        public string Title
        {
            get
            {
                return mp3_TIT2;
            }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Title detail")]
        public string TitleDetail
        {
            get
            {
                return mp3_TIT3;
            }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Key")]
        public string Key
        {
            get
            {
                return mp3_TKEY;
            }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Author name")]
        public string AuthorName
        {
            get
            {
                return mp3_TPE1;
            }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Composer")]
        public string Composer
        {
            get
            {
                return mp3_TCOM;
            }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Publisher")]
        public string Publisher
        {
            get
            {
                return mp3_TPUB;
            }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Genre")]
        public string Genre
        {
            get
            {
                return mp3_TCON;
            }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("MediaType")]
        public string MediaType
        {
            get
            {
                return mp3_TMED;
            }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Lead Soloist")]
        public string LeadSoloist
        {
            get
            {
                return mp3_TPE1;
            }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Band")]
        public string Band
        {
            get
            {
                return mp3_TPE2;
            }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Conductor")]
        public string Conductor
        {
            get
            {
                return mp3_TPE3;
            }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("FileType")]
        public string FileType
        {
            get
            {
                return mp3_TFLT;
            }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Track Number")]
        public string TrackNumber
        {
            get { return mp3_TRCK; }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Year")]
        public string Year
        {
            get { return mp3_TYER; }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Track length")]
        public string TrackLength
        {
            get
            {
                if (mp3_TLEN != "")
                {
                     return FormatLength(lengthInMs);
                }
                else { return getFormattedLength(intLength); ; }
            }
        }
        public string DiskKey
        {
            get
            {
                return mp3_MCDI;
            }
        }
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }
        #endregion
        public ID3Header ID3Header
        {
            get { return mp3Header; }
            set { mp3Header = value; }
        }
        [CategoryAttribute("Music File"), DescriptionAttribute("Mp3 tags")]
        public List<ID3Tag> ID3Tags
        {
            get { return attribs; }
        }
        public List<MP3Frame> Mp3Frames
        {
            get { return mp3Frames; }
            set { mp3Frames = value; }
        }
        public int Number_Of_Frames
        {
            get
            {
                if (Mp3Frames != null)
                    return Mp3Frames.Count;
                return 0;
            }
        }
        public ID1Tag Id1Tag
        {
            get { return id1Tag; }
            set { id1Tag = value; }
        }
        public Guid WMCollectionId;
        public Guid WMMediaClassPrimaryID;
        public Guid WMContentID3;
        public Guid WMCollectionGroupID;
        public string WMUniqueFileIdentifier;
        public string WMProvider;
        private bool isSelected = false;
        private List<ID3Tag> attribs = new List<ID3Tag>();
        public override string ToString()
        {
            return FileName;
        }
        private List<MP3Frame> mp3Frames;

        public MusicFileClass(string fileName)
        {
            this.fileName = fileName;
            //         getTagsOld(fileName);
            if (fileName.ToLower().EndsWith("mp3"))
            {
                getTags(fileName);

            }
            if (fileName.ToLower().EndsWith("wma"))
                getWMA(fileName);
        }
        #region Lecture WMA
        WMA_Content_Block wma_Block;
        public WMA_Content_Block Wma_Block
        {
            get { return wma_Block; }
            set { wma_Block = value; }
        }
        List<ELEMENTARY_TYPE> guids;

        public List<ELEMENTARY_TYPE> Guids
        {
            get { return guids; }
            set { guids = value; }
        }
        WMA_Header wma_header;
        ASF_Data_Object asf_data_object;

        public WMA_Header Wma_header
        {
            get { return wma_header; }
            set { wma_header = value; }
        }
        public ASF_Data_Object Asf_data_object
        {
            get { return asf_data_object; }
            set { asf_data_object = value; }
        }
        public void getWMA(string fileName)
        {
            BitStreamReader sw = new BitStreamReader(fileName, false);
            guids = new List<ELEMENTARY_TYPE>();
            wma_header= new WMA_Header(sw);
            asf_data_object = new ASF_Data_Object(sw);
            sw.Close();
        }
        private void processContentBlockWMA(BitStreamReader sw)
        {
            short lTitle, lAuthor, lCopyright, lDescription, lRating;
            lTitle = sw.ReadShort();
            lAuthor = sw.ReadShort();
            lCopyright = sw.ReadShort();
            lDescription = sw.ReadShort();
            lRating = sw.ReadShort();
            if (lTitle > 0)
            {
                 mp3_TIT2 = sw.ReadString(lTitle/2, Encoding.Unicode);// readUnicodeString(i, sw);
            }
            if (lAuthor > 0)
                mp3_TPE1 = sw.ReadString(lAuthor/2, Encoding.Unicode);//readUnicodeString(Convert.ToInt16(lAuthor), sw);
            if (lDescription > 0)
                mp3_TPE1 = sw.ReadString(lDescription/2, Encoding.Unicode);// readUnicodeString(Convert.ToInt16(lDescription), sw);
            if (lRating > 0)
                mp3_TPE1 = sw.ReadString(lRating/2, Encoding.Unicode);// readUnicodeString(Convert.ToInt16(lRating), sw);
        }
        private void processExtendedContentBlockWMA(BitStreamReader br)
        {
            Int16 numAttrs, dataType, dataLen, sValue;
            string attrName, strValue;
            byte[] bValue;
            int i, iValue, index;
            long lValue;
            value valueObject;
            numAttrs = br.ReadShort();
            index = 0;
            for (i = 0; i < numAttrs; i++)
            {
                try
                {
                    int str = br.ReadShort();
                    attrName = br.ReadString(str, Encoding.Unicode); 
                    dataType = br.ReadShort();

                    switch (dataType)
                    {
                        #region Read data
                        case 0:
                            str = br.ReadShort();
                            strValue = br.ReadString(str, Encoding.Unicode);
                            valueObject.dataType = 0;
                            valueObject.index = index;
                            //attrs.Add(attrName, valueObject);
                            //attrValues.Add(value);
                            switch (attrName)
                            {
                                case "WM/Lyrics":
                                    break;
                                case "WM/AlbumTitle":
                                    mp3_TALB = strValue;
                                    break;
                                case "WM/AlbumArtist":
                                    mp3_TPE1 = strValue;
                                    break;
                                case "WM/Genre":
                                    mp3_TCON = strValue;
                                    break;
                                case "WM/TrackNumber":
                                    mp3_TRCK = strValue;
                                    break;
                                case "WM/Year":
                                    break;
                                case "WM/Composer":
                                    mp3_TCOM = strValue;
                                    break;
                                case "WM/Publisher":
                                    break;
                            }
                            attrName += ";" + strValue;
                            index += 1;
                            break;
                        case 1:
                            dataLen = br.ReadShort();
                            bValue = new byte[dataLen - 1];
                            bValue = br.ReadBytes(dataLen);
                            valueObject.dataType = 1;
                            valueObject.index = index;
                            //attrs.Add(attrName, valueObject);
                            //attrValues.Add(bValue);
                            index += 1;
                            break;
                        case 2:
                            dataLen = br.ReadShort();
                            iValue = br.ReadInteger();
                            valueObject.dataType = 2;
                            valueObject.index = index;
                            //attrs.Add(attrName, valueObject);
                            //attrValues.Add(boolValue);
                            index += 1;
                            break;
                        case 3:
                            dataLen = br.ReadShort();
                            iValue = br.ReadInteger();
                            valueObject.dataType = 3;
                            valueObject.index = index;
                            //attrs.Add(attrName, valueObject);
                            //attrValues.Add(iValue);
                            index += 1;
                            break;
                        case 4:
                            dataLen = br.ReadShort();
                            lValue = br.ReadLong();
                            valueObject.dataType = 4;
                            valueObject.index = index;
                            //attrs.Add(attrName, valueObject);
                            //attrValues.Add(lValue);
                            index += 1;
                            break;
                        case 5:
                            dataLen = br.ReadShort();
                            sValue = br.ReadShort();
                            valueObject.dataType = 5;
                            valueObject.index = index;
                            //attrs.Add(attrName, valueObject);
                            //attrValues.Add(valueLength);
                            index += 1;
                            break;
                        #endregion
                    }
                    attribs.Add(new ID3Tag(attrName, ""));
                }
                catch (Exception ex) { }
            }
        }
        public void getWMAOld(string fileName)
        {
            bool CBDone = false;
            bool ECBDone = false;
            Guid hdrGUID = new Guid("75B22630-668E-11CF-A6D9-00AA0062CE6C");
            Guid contentGUID = new Guid("75B22633-668E-11CF-A6D9-00AA0062CE6C");
            Guid extendedContentGUID = new Guid("D2D0A440-E307-11D2-97F0-00A0C95EA850");
            FileStream FS = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(FS);
            byte[] Buffer = new byte[100];
            int[] nb = new int[4];
            Guid g = Guid.Empty;
            ReadGuid(ref g, br);
            Int64 i = br.ReadInt64();// the size of the entire block
            Int32 i32 = br.ReadInt32();//the number of entries
            byte[] bt = br.ReadBytes(2);// two reserved bytes
            while (ReadGuid(ref g, br))
            {
                long sizeBlock = br.ReadInt64();
                if (br.BaseStream.Position + sizeBlock > FS.Length)
                {
                    break;
                }

                if (Guid.Equals(g, contentGUID))
                {
                    processContentBlockWMAOld(br);
                    CBDone = true;
                    if (ECBDone)
                        break;
                }
                else if (Guid.Equals(g, extendedContentGUID))
                {
                    processExtendedContentBlockWMAOld(br);
                    ECBDone = true;
                    if (CBDone)
                        break;
                }
                else
                {
                    sizeBlock -= 24;// already read the guid header info
                    br.BaseStream.Position += sizeBlock;
                }
            }
            FS.Close();
        }
        private void processContentBlockWMAOld(BinaryReader br)
        {
            short lTitle, lAuthor, lCopyright, lDescription, lRating, i;
            lTitle = br.ReadInt16();
            lAuthor = br.ReadInt16();
            lCopyright = br.ReadInt16();
            lDescription = br.ReadInt16();
            lRating = br.ReadInt16();
            if (lTitle > 0)
            {
                i = Convert.ToInt16(lTitle);
                mp3_TIT2 = readUnicodeString(i, br);
            }
            if (lAuthor > 0)
                mp3_TPE1 = readUnicodeString(Convert.ToInt16(lAuthor), br);
            if (lDescription > 0)
                mp3_TPE1 = readUnicodeString(Convert.ToInt16(lDescription), br);
            if (lRating > 0)
                mp3_TPE1 = readUnicodeString(Convert.ToInt16(lRating), br);
        }
        private void processExtendedContentBlockWMAOld(BinaryReader br)
        {
            Int16 numAttrs, dataType, dataLen, sValue;
            string attrName, strValue;
            byte[] bValue;
            int i, iValue, index;
            long lValue;
            value valueObject;
            numAttrs = br.ReadInt16();
            index = 0;
            for (i = 0; i < numAttrs; i++)
            {
                try
                {
                    attrName = readUnicodeString(br);
                    dataType = br.ReadInt16();

                    switch (dataType)
                    {
                        #region Read data
                        case 0:
                            strValue = readUnicodeString(br);
                            valueObject.dataType = 0;
                            valueObject.index = index;
                            //attrs.Add(attrName, valueObject);
                            //attrValues.Add(value);
                            switch (attrName)
                            {
                                case "WM/Lyrics":
                                    break;
                                case "WM/AlbumTitle":
                                    mp3_TALB = strValue;
                                    break;
                                case "WM/AlbumArtist":
                                    mp3_TPE1 = strValue;
                                    break;
                                case "WM/Genre":
                                    mp3_TCON = strValue;
                                    break;
                                case "WM/TrackNumber":
                                    mp3_TRCK = strValue;
                                    break;
                                case "WM/Year":
                                    break;
                                case "WM/Composer":
                                    mp3_TCOM = strValue;
                                    break;
                                case "WM/Publisher":
                                    break;
                            }
                            attrName += ";" + strValue;
                            index += 1;
                            break;
                        case 1:
                            dataLen = br.ReadInt16();
                            bValue = new byte[dataLen - 1];
                            bValue = br.ReadBytes(dataLen);
                            valueObject.dataType = 1;
                            valueObject.index = index;
                            //attrs.Add(attrName, valueObject);
                            //attrValues.Add(bValue);
                            index += 1;
                            break;
                        case 2:
                            dataLen = br.ReadInt16();
                            iValue = br.ReadInt32();
                            valueObject.dataType = 2;
                            valueObject.index = index;
                            //attrs.Add(attrName, valueObject);
                            //attrValues.Add(boolValue);
                            index += 1;
                            break;
                        case 3:
                            dataLen = br.ReadInt16();
                            iValue = br.ReadInt32();
                            valueObject.dataType = 3;
                            valueObject.index = index;
                            //attrs.Add(attrName, valueObject);
                            //attrValues.Add(iValue);
                            index += 1;
                            break;
                        case 4:
                            dataLen = br.ReadInt16();
                            lValue = br.ReadInt64();
                            valueObject.dataType = 4;
                            valueObject.index = index;
                            //attrs.Add(attrName, valueObject);
                            //attrValues.Add(lValue);
                            index += 1;
                            break;
                        case 5:
                            dataLen = br.ReadInt16();
                            sValue = br.ReadInt16();
                            valueObject.dataType = 5;
                            valueObject.index = index;
                            //attrs.Add(attrName, valueObject);
                            //attrValues.Add(valueLength);
                            index += 1;
                            break;
                        #endregion
                    }
                    attribs.Add(new ID3Tag(attrName, ""));
                }
                catch (Exception ex) { }
            }
        }
        private bool ReadGuid(ref Guid g, BinaryReader br)
        {
            int int1 = br.ReadInt32();
            if (int1 == -1)
            {
                return false;
            }
            short shrt1 = br.ReadInt16();
            short shrt2 = br.ReadInt16();
            byte[] b = br.ReadBytes(8);
            g = new Guid(int1, shrt1, shrt2, b);
            return true;

        }
        private string readUnicodeString(BinaryReader br)
        {
            Int16 datalen;
            datalen = br.ReadInt16();
            return readUnicodeString(datalen, br);
        }
        private string readUnicodeString(Int16 len, BinaryReader br)
        {
            //Can't use .NET functions, since they expect the length field to be a single byte for strings < 256 chars
            byte[] bt = new byte[len];
            br.BaseStream.Read(bt, 0, len);
            Decoder d = Encoding.GetEncoding("Unicode").GetDecoder();
            char[] chars = new Char[d.GetCharCount(bt, 0, len)];
            int charLen = d.GetChars(bt, 0, bt.Length, chars, 0);
            string valTag = "";
            for (int j = 0; j < chars.Length; j++)
            {
                if (chars[j].ToString() != "\0")
                {
                    valTag += chars[j].ToString();
                }
            }
            return valTag;
        }
        #endregion
        private static string FormatLength(int ms)
        {
            int sec = ms / 1000;
            int hour = sec / 3600;
            sec = sec % 3600;
            int min = sec / 60;
            sec = sec % 60;
            return hour.ToString("d2") + ":" + min.ToString("d2") + ":" + sec.ToString("d2");
        }
        #region Lecture fileName
        ID3Header mp3Header;
        ID1Tag id1Tag;

        private string getTags(string MP3)
        {
            BitStreamReader sw = new BitStreamReader(MP3, true);
            mp3Header = new ID3Header(sw);
            int[] nb = new int[4];
            if (((string)mp3Header.ID3v2_file_identifier.Value).StartsWith("ID3"))
            {
                while (sw.Position < mp3Header.Size)
                {
                    ID3Tag mp = new ID3Tag(sw);
                    if (!((string)mp.TagName.Value).StartsWith("\0"))
                        attribs.Add(mp);
                    if (mp.TagName.ToString() == "TLEN")
                    {
                        mp3_TLEN = mp.TagValue.Value.ToString();
                        lengthInMs = Convert.ToInt32(mp3_TLEN);
                    }
                }
            }
            else
            { }
            mp3Frames = new List<MP3Frame>();
            sw.Position = mp3Header.Size;
            while (sw.Position < sw.Length - 129)
            {
                try
                {
                    MP3Frame mp = new MP3Frame(sw);
                    mp3Frames.Add(mp);
                }
                catch { }
            }
            sw.Position = (int)sw.Length - 128;
            id1Tag = new ID1Tag(sw);
            sw.Close();
            ReadMP3Information(MP3);
            return "";
        }

        private void CutMP3(string MP3)
        {
            FileStream sr = new FileStream(MP3, FileMode.Open, FileAccess.Read);
            FileStream mpwrite = new FileStream(@"E:\test2noimage.mp3", FileMode.Create, FileAccess.Write);
            #region Write tags
            long pos = mp3Header.Size;
            long apic = pos;
            long endApic = 0;
            foreach (ID3Tag id in ID3Tags)
            {
                if (id.TagName.Value.ToString() == "APIC")
                {
                    apic = id.PositionOfStructureInFile;
                    endApic = apic + id.LengthInFile;
                    break;
                }
            }
            #region Skip thumbnail
                    while (sr.Position < apic)
                     {
                         int a  =sr.ReadByte();
                         mpwrite.WriteByte((byte)a);
                     }
                     sr.Position = endApic;
            #endregion
            while (sr.Position < pos)
            {
                int a = sr.ReadByte();
                mpwrite.WriteByte((byte)a);
            }

            #endregion
            #region Write selected frames
            int startFrames = 700;
            int numberOfFrames = 500;
            int durée = 15;
            if (lengthInMs != 0)
                numberOfFrames = (Number_Of_Frames * 1000 * durée) / lengthInMs + 1;
            sr.Position = mp3Frames[startFrames].PositionOfStructureInFile;
            pos = mp3Frames[startFrames + numberOfFrames].PositionOfStructureInFile;

            while (sr.Position < pos)
            {
                int a = sr.ReadByte();
                mpwrite.WriteByte((byte)a);
            }
            #endregion
            #region write ID1Tag
            sr.Position = id1Tag.PositionOfStructureInFile;
            while (sr.Position < sr.Length)
            {
                int a = sr.ReadByte();
                mpwrite.WriteByte((byte)a);
            }
            #endregion
            mpwrite.Close();
        }
        private string GetStyle(string code)
        {
            string tagValue = "";
            switch (code)
            {
                case "(0)":
                    tagValue = "Blues ";
                    break;
                case "(1)":
                    tagValue = "Classic Rock";
                    break;
                case "(2)":
                    tagValue = "Country";
                    break;
                case "(3)":
                    tagValue = "Dance";
                    break;
                case "(4)":
                    tagValue = "Disco";
                    break;
                case "(5)":
                    tagValue = "Funk";
                    break;
                case "(6)":
                    tagValue = "Grunge";
                    break;
                case "(7)":
                    tagValue = "Hip-Hop";
                    break;
                case "(8)":
                    tagValue = "Jazz";
                    break;
                case "(9)":
                    tagValue = "Metal";
                    break;
                case "(10)":
                    tagValue = "New Age";
                    break;
                case "(11)":
                    tagValue = "Oldies";
                    break;
                case "(12)":
                    tagValue = "Other";
                    break;
                case "(13)":
                    tagValue = "Pop";
                    break;
                case "(14)":
                    tagValue = "R&B";
                    break;
                case "(15)":
                    tagValue = "Rap";
                    break;
                case "(16)":
                    tagValue = "Reggae";
                    break;
                case "(17)":
                    tagValue = "Rock";
                    break;
                case "(18)":
                    tagValue = "Techno";
                    break;
                case "(19)":
                    tagValue = "Industrial";
                    break;
                case "(20)":
                    tagValue = "Alternative";
                    break;
                case "(21)":
                    tagValue = "Ska";
                    break;
                case "(22)":
                    tagValue = "Death Metal";
                    break;
                case "(23)":
                    tagValue = "Pranks";
                    break;
                case "(24)":
                    tagValue = "Soundtrack";
                    break;
                case "(25)":
                    tagValue = "Euro-Techno";
                    break;
                case "(26)":
                    tagValue = "Ambient";
                    break;
                case "(27)":
                    tagValue = "Trip-Hop";
                    break;
                case "(28)":
                    tagValue = "Vocal";
                    break;
                case "(29)":
                    tagValue = "Jazz+Funk";
                    break;
                case "(30)":
                    tagValue = "Fusion";
                    break;
                case "(31)":
                    tagValue = "Trance";
                    break;
                case "(32)":
                    tagValue = "Classical";
                    break;
                case "(33)":
                    tagValue = "Instrumental";
                    break;
                case "(34)":
                    tagValue = "Acid";
                    break;
                case "(35)":
                    tagValue = "House";
                    break;
                case "(36)":
                    tagValue = "Game";
                    break;
                case "(37)":
                    tagValue = "Sound Clip";
                    break;
                case "(38)":
                    tagValue = "Gospel";
                    break;
                case "(39)":
                    tagValue = "Noise";
                    break;
                case "(40)":
                    tagValue = "AlternRock";
                    break;
                case "(41)":
                    tagValue = "Bass";
                    break;
                case "(42)":
                    tagValue = "Soul";
                    break;
                case "(43)":
                    tagValue = "Punk";
                    break;
                case "(44)":
                    tagValue = "Space";
                    break;
                case "(45)":
                    tagValue = "Meditative";
                    break;
                case "(46)":
                    tagValue = "Instrumental Pop";
                    break;
                case "(47)":
                    tagValue = "Instrumental Rock";
                    break;
                case "(48)":
                    tagValue = "Ethnic";
                    break;
                case "(49)":
                    tagValue = "Gothic";
                    break;
                case "(50)":
                    tagValue = "Darkwave";
                    break;
                case "(51)":
                    tagValue = "Techno-Industrial";
                    break;
                case "(52)":
                    tagValue = "Electronic";
                    break;
                case "(53)":
                    tagValue = "Pop-Folk";
                    break;
                case "(54)":
                    tagValue = "Eurodance";
                    break;
                case "(55)":
                    tagValue = "Dream";
                    break;
                case "(56)":
                    tagValue = "Southern Rock";
                    break;
                case "(57)":
                    tagValue = "Comedy";
                    break;
                case "(58)":
                    tagValue = "Cult";
                    break;
                case "(59)":
                    tagValue = "Gangsta";
                    break;
                case "(60)":
                    tagValue = "Top 40";
                    break;
                case "(61)":
                    tagValue = "Christian Rap";
                    break;
                case "(62)":
                    tagValue = "Pop/Funk";
                    break;
                case "(63)":
                    tagValue = "Jungle";
                    break;
                case "(64)":
                    tagValue = "Native American";
                    break;
                case "(65)":
                    tagValue = "Cabaret";
                    break;
                case "(66)":
                    tagValue = "New Wave";
                    break;
                case "(67)":
                    tagValue = "Psychadelic";
                    break;
                case "(68)":
                    tagValue = "Rave";
                    break;
                case "(69)":
                    tagValue = "Showtunes";
                    break;
                case "(70)":
                    tagValue = "Trailer";
                    break;
                case "(71)":
                    tagValue = "Lo-Fi";
                    break;
                case "(72)":
                    tagValue = "Tribal";
                    break;
                case "(73)":
                    tagValue = "Acid Punk";
                    break;
                case "(74)":
                    tagValue = "Acid Jazz";
                    break;
                case "(75)":
                    tagValue = "Polka";
                    break;
                case "(76)":
                    tagValue = "Retro";
                    break;
                case "(77)":
                    tagValue = "Musical";
                    break;
                case "(78)":
                    tagValue = "Rock & Roll";
                    break;
                case "(79)":
                    tagValue = "Hard Rock";
                    break;
                case "(80)":
                    tagValue = "Folk";
                    break;
                case "(81)":
                    tagValue = "Folk-Rock";
                    break;
                case "(82)":
                    tagValue = "National Folk";
                    break;
                case "(83)":
                    tagValue = "Swing";
                    break;
                case "(84)":
                    tagValue = "Fast Fusion";
                    break;
                case "(85)":
                    tagValue = "Bebob";
                    break;
                case "(86)":
                    tagValue = "Latin";
                    break;
                case "(87)":
                    tagValue = "Revival";
                    break;
                case "(88)":
                    tagValue = "Celtic";
                    break;
                case "(89)":
                    tagValue = "Bluegrass";
                    break;
                case "(90)":
                    tagValue = "Avantgarde";
                    break;
                case "(91)":
                    tagValue = "Gothic Rock";
                    break;
                case "(92)":
                    tagValue = "Progressive Rock";
                    break;
                case "(93)":
                    tagValue = "Psychedelic Rock";
                    break;
                case "(94)":
                    tagValue = "Symphonic Rock";
                    break;
                case "(95)":
                    tagValue = "Slow Rock";
                    break;
                case "(96)":
                    tagValue = "Big Band";
                    break;
                case "(97)":
                    tagValue = "Chorus";
                    break;
                case "(98)":
                    tagValue = "Easy Listening";
                    break;
                case "(99)":
                    tagValue = "Acoustic";
                    break;
                case "(100)":
                    tagValue = "Humour";
                    break;
                case "(101)":
                    tagValue = "Speech";
                    break;
                case "(102)":
                    tagValue = "Chanson";
                    break;
                case "(103)":
                    tagValue = "Opera";
                    break;
                case "(104)":
                    tagValue = "Chamber Music";
                    break;
                case "(105)":
                    tagValue = "Sonata";
                    break;
                case "(106)":
                    tagValue = "Symphony";
                    break;
                case "(107)":
                    tagValue = "Booty Bass";
                    break;
                case "(108)":
                    tagValue = "Primus";
                    break;
                case "(109)":
                    tagValue = "Porn Groove";
                    break;
                case "(110)":
                    tagValue = "Satire";
                    break;
                case "(111)":
                    tagValue = "Slow Jam";
                    break;
                case "(112)":
                    tagValue = "Club";
                    break;
                case "(113)":
                    tagValue = "Tango";
                    break;
                case "(114)":
                    tagValue = "Samba";
                    break;
                case "(115)":
                    tagValue = "Folklore";
                    break;
                case "(116)":
                    tagValue = "Ballad";
                    break;
                case "(117)":
                    tagValue = "Power Ballad";
                    break;
                case "(118)":
                    tagValue = "Rhythmic Soul";
                    break;
                case "(119)":
                    tagValue = "Freestyle";
                    break;
                case "(120)":
                    tagValue = "Duet";
                    break;
                case "(121)":
                    tagValue = "Punk Rock";
                    break;
                case "(122)":
                    tagValue = "Drum Solo";
                    break;
                case "(123)":
                    tagValue = "A capella";
                    break;
                case "(124)":
                    tagValue = "Euro-House";
                    break;
                case "(125)":
                    tagValue = "Dance Hall";
                    break;
            }
            return tagValue;
        }
        private string GetArtType(byte at)
        {
            string artType = "";
            switch (pictureType)
            {
                case 0x00:
                    artType = "Other";
                    break;
                case 0x01:
                    artType = "32x32 pixels 'file icon' (PNG only)";
                    break;
                case 0x02:
                    artType = "Other file icon";
                    break;
                case 0x03:
                    artType = "Cover (front)";
                    break;
                case 0x04:
                    artType = "Cover (back)";
                    break;
                case 0x05:
                    artType = "Leaflet page";
                    break;
                case 0x06:
                    artType = "Media (e.g. lable side of CD)";
                    break;
                case 0x07:
                    artType = "Lead artist/lead performer/soloist";
                    break;
                case 0x08:
                    artType = "Artist/performer";
                    break;
                case 0x09:
                    artType = "Conductor";
                    break;
                case 0x0A:
                    artType = "Band/Orchestra";
                    break;
                case 0x0B:
                    artType = "Composer";
                    break;
                case 0x0C:
                    artType = "Lyricist/text writer";
                    break;
                case 0x0D:
                    artType = "Recording Location";
                    break;
                case 0x0E:
                    artType = "During recording";
                    break;
                case 0x0F:
                    artType = "During performance";
                    break;
                case 0x10:
                    artType = "Movie/video screen capture";
                    break;
                case 0x11:
                    artType = "A bright coloured fish";
                    break;
                case 0x12:
                    artType = "Illustration";
                    break;
                case 0x13:
                    artType = "Band/artist logotype";
                    break;
                case 0x14:
                    artType = "Publisher/Studio logotype";
                    break;
            }
            return artType;
        }
        #endregion
        #region Public variables for storing the information about the fileName
        public int intBitRate;
        public string strFileName;
        public long lngFileSize;
        public int intFrequency;
        public string strMode;
        public int intLength;
        public string strLengthFormatted;
        #endregion
        public bool ReadMP3Information(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            // Set the filename not including the path information
            strFileName = @fs.Name;
            char[] chrSeparators = new char[] { '\\', '/' };
            string[] strSeparator = strFileName.Split(chrSeparators);
            int intUpper = strSeparator.GetUpperBound(0);
            strFileName = strSeparator[intUpper];
            // Replace ' with '' for the SQL INSERT statement
            strFileName = strFileName.Replace("'", "''");
            // Set the file size
            lngFileSize = fs.Length;

            byte[] bytHeader = new byte[4];
            byte[] bytVBitRate = new byte[12];
            int intPos = 0;
            // Keep reading 4 bytes from the header until we know for sure that in 
            // fact it's an fileName
            do
            {
                fs.Position = intPos;
                fs.Read(bytHeader, 0, 4);
                intPos++;
                LoadMP3Header(bytHeader);
            }
            while (!IsValidHeader() && (fs.Position != fs.Length));
            // If the current file stream position is equal to the length, 
            // that means that we've read the entire file and it's not a valid fileName file
            if (fs.Position != fs.Length)
            {
                intPos += 3;
                #region Decalage selon version
                if (getVersionIndex() == 3)    // MPEG Version 1
                {
                    if (getModeIndex() == 3)    // Single Channel
                    {
                        intPos += 17;
                    }
                    else
                    {
                        intPos += 32;
                    }
                }
                else                        // MPEG Version 2.0 or 2.5
                {
                    if (getModeIndex() == 3)    // Single Channel
                    {
                        intPos += 9;
                    }
                    else
                    {
                        intPos += 17;
                    }
                }
                #endregion
                // Check to see if the fileName has a variable bitrate
                fs.Position = intPos;
                fs.Read(bytVBitRate, 0, 12);
                boolVBitRate = LoadVBRHeader(bytVBitRate);
                // Once the file's read in, then assign the properties of the file to the public variables
                intBitRate = getBitrate();
                intFrequency = getFrequency();
                strMode = getMode();
                intLength = getLengthInSeconds();
                strLengthFormatted = getFormattedLength(intLength);
                fs.Close();
                return true;
            }
            return false;
        }
        #region Private variables used in the process of reading in the fileName files
        private ulong bithdr;
        private bool boolVBitRate;
        private int intVFrames;
        private void LoadMP3Header(byte[] c)
        {
            // this thing is quite interesting, it works like the following
            // c[0] = 00000011
            // c[1] = 00001100
            // c[2] = 00110000
            // c[3] = 11000000
            // the operator << means that we'll move the bits in that direction
            // 00000011 << 24 = 00000011000000000000000000000000
            // 00001100 << 16 =         000011000000000000000000
            // 00110000 << 24 =                 0011000000000000
            // 11000000       =                         11000000
            //                +_________________________________
            //                  00000011000011000011000011000000
            bithdr = (ulong)(((c[0] & 255) << 24) | ((c[1] & 255) << 16) | ((c[2] & 255) << 8) | ((c[3] & 255)));
        }
        private bool LoadVBRHeader(byte[] inputheader)
        {
            // If it's a variable bitrate fileName, the first 4 bytes will read 'Xing'
            // since they're the ones who added variable bitrate-edness to MP3s
            if (inputheader[0] == 88 && inputheader[1] == 105 &&
                inputheader[2] == 110 && inputheader[3] == 103)
            {
                int flags = (int)(((inputheader[4] & 255) << 24) | ((inputheader[5] & 255) << 16) | ((inputheader[6] & 255) << 8) | ((inputheader[7] & 255)));
                if ((flags & 0x0001) == 1)
                {
                    intVFrames = (int)(((inputheader[8] & 255) << 24) | ((inputheader[9] & 255) << 16) | ((inputheader[10] & 255) << 8) | ((inputheader[11] & 255)));
                    return true;
                }
                else
                {
                    intVFrames = -1;
                    return true;
                }
            }
            return false;
        }
        private bool IsValidHeader()
        {
            return (((getFrameSync() & 2047) == 2047) &&
                    ((getVersionIndex() & 3) != 1) &&
                    ((getLayerIndex() & 3) != 0) &&
                    ((getBitrateIndex() & 15) != 0) &&
                    ((getBitrateIndex() & 15) != 15) &&
                    ((getFrequencyIndex() & 3) != 3) &&
                    ((getEmphasisIndex() & 3) != 2));
        }
        private int getFrameSync()
        {
            return (int)((bithdr >> 21) & 2047);
        }
        private int getVersionIndex()
        {
            return (int)((bithdr >> 19) & 3);
        }
        private int getLayerIndex()
        {
            return (int)((bithdr >> 17) & 3);
        }
        private int getProtectionBit()
        {
            return (int)((bithdr >> 16) & 1);
        }
        private int getBitrateIndex()
        {
            return (int)((bithdr >> 12) & 15);
        }
        private int getFrequencyIndex()
        {
            return (int)((bithdr >> 10) & 3);
        }
        private int getPaddingBit()
        {
            return (int)((bithdr >> 9) & 1);
        }
        private int getPrivateBit()
        {
            return (int)((bithdr >> 8) & 1);
        }
        private int getModeIndex()
        {
            return (int)((bithdr >> 6) & 3);
        }
        private int getModeExtIndex()
        {
            return (int)((bithdr >> 4) & 3);
        }
        private int getCoprightBit()
        {
            return (int)((bithdr >> 3) & 1);
        }
        private int getOrginalBit()
        {
            return (int)((bithdr >> 2) & 1);
        }
        private int getEmphasisIndex()
        {
            return (int)(bithdr & 3);
        }
        private double getVersion()
        {
            double[] table = { 2.5, 0.0, 2.0, 1.0 };
            return table[getVersionIndex()];
        }
        private int getLayer()
        {
            return (int)(4 - getLayerIndex());
        }
        private int getBitrate()
        {
            // If the file has a variable bitrate, then we return an integer average bitrate,
            // otherwise, we use a lookup table to return the bitrate
            if (boolVBitRate)
            {
                double medFrameSize = (double)lngFileSize / (double)getNumberOfFrames();
                return (int)((medFrameSize * (double)getFrequency()) / (1000.0 * ((getLayerIndex() == 3) ? 12.0 : 144.0)));
            }
            else
            {
                int[, ,] table =        {
                                { // MPEG 2 & 2.5
                                    {0,  8, 16, 24, 32, 40, 48, 56, 64, 80, 96,112,128,144,160,0}, // Layer III
                                    {0,  8, 16, 24, 32, 40, 48, 56, 64, 80, 96,112,128,144,160,0}, // Layer II
                                    {0, 32, 48, 56, 64, 80, 96,112,128,144,160,176,192,224,256,0}  // Layer I
                                },
                                { // MPEG 1
                                    {0, 32, 40, 48, 56, 64, 80, 96,112,128,160,192,224,256,320,0}, // Layer III
                                    {0, 32, 48, 56, 64, 80, 96,112,128,160,192,224,256,320,384,0}, // Layer II
                                    {0, 32, 64, 96,128,160,192,224,256,288,320,352,384,416,448,0}  // Layer I
                                }
                                };

                return table[getVersionIndex() & 1, getLayerIndex() - 1, getBitrateIndex()];
            }
        }
        private int getFrequency()
        {
            int[,] table =    {    
                            {32000, 16000,  8000}, // MPEG 2.5
                            {    0,     0,     0}, // reserved
                            {22050, 24000, 16000}, // MPEG 2
                            {44100, 48000, 32000}  // MPEG 1
                        };

            return table[getVersionIndex(), getFrequencyIndex()];
        }
        private string getMode()
        {
            switch (getModeIndex())
            {
                default:
                    return "Stereo";
                case 1:
                    return "Joint Stereo";
                case 2:
                    return "Dual Channel";
                case 3:
                    return "Single Channel";
            }
        }
        private int getLengthInSeconds()
        {
            // "intKilBitFileSize" made by dividing by 1000 in order to match the "Kilobits/second"
            int intKiloBitFileSize = (int)((8 * lngFileSize) / 1000);
            return (int)(intKiloBitFileSize / getBitrate());
        }
        private string getFormattedLength(int s)
        {
            // Seconds to display
            int ss = s % 60;
            // Complete number of minutes
            int m = (s - ss) / 60;
            // Minutes to display
            int mm = m % 60;
            // Complete number of hours
            int h = (m - mm) / 60;
            // Make "hh:mm:ss"
            return h.ToString("D2") + ":" + mm.ToString("D2") + ":" + ss.ToString("D2");
        }
        private int getNumberOfFrames()
        {
            // Again, the number of MPEG frames is dependant on whether it's a variable bitrate fileName or not
            if (!boolVBitRate)
            {
                double medFrameSize = (double)(((getLayerIndex() == 3) ? 12 : 144) * ((1000.0 * (float)getBitrate()) / (float)getFrequency()));
                return (int)(lngFileSize / medFrameSize);
            }
            else
                return intVFrames;
        }
        #endregion
        struct value
        {
            public Int16 dataType;
            public int index;
        }
    }
    public class WMA_Header : LOCALIZED_DATA
    {
        public static SortedList<Guid, string> guids;
        ELEMENTARY_TYPE guid, size, number_of_Header_Objects, reserved;//0102
        List<WMA_Content_Block> wma_Block;
        public ELEMENTARY_TYPE Block_Guid
        {
            get { return guid; }
            set { guid = value; }
        }
        public ELEMENTARY_TYPE Size
        {
            get { return size; }
            set { size = value; }
        }

        public ELEMENTARY_TYPE Number_of_Header_Objects
        {
            get { return number_of_Header_Objects; }
            set { number_of_Header_Objects = value; }
        }

        public ELEMENTARY_TYPE Reserved
        {
            get { return reserved; }
            set { reserved = value; }
        }
        public List<WMA_Content_Block> Wma_Blocks
        {
            get { return wma_Block; }
            set { wma_Block = value; }
        }
        public void FillGuids()
        {
            guids = new SortedList<Guid, string>();
            guids.Add(new Guid("75B22630-668E-11CF-A6D9-00AA0062CE6C"), "ASF_Header_Object");
            guids.Add(new Guid("75B22636-668E-11CF-A6D9-00AA0062CE6C"), "ASF_Data_Object");
            guids.Add(new Guid("33000890-E5B1-11CF-89F4-00A0C90349CB"), "ASF_Simple_Index_Object");
            guids.Add(new Guid("D6E229D3-35DA-11D1-9034-00A0C90349BE"), "ASF_Index_Object");
            guids.Add(new Guid("FEB103F8-12AD-4C64-840F-2A1D2F7AD48C"), "ASF_Media_Object_Index_Object");
            guids.Add(new Guid("3CB73FD0-0C4A-4803-953D-EDF7B6228F0C"), "ASF_Timecode_Index_Object");
            guids.Add(new Guid("8CABDCA1-A947-11CF-8EE4-00C00C205365"), "ASF_File_Properties_Object");
            guids.Add(new Guid("B7DC0791-A9B7-11CF-8EE6-00C00C205365"), "ASF_Stream_Properties_Object");
            guids.Add(new Guid("5FBF03B5-A92E-11CF-8EE3-00C00C205365"), "ASF_Header_Extension_Object");
            guids.Add(new Guid("86D15240-311D-11D0-A3A4-00A0C90348F6"), "ASF_Codec_List_Object");
            guids.Add(new Guid("1EFB1A30-0B62-11D0-A39B-00A0C90348F6"), "ASF_Script_Command_Object");
            guids.Add(new Guid("F487CD01-A951-11CF-8EE6-00C00C205365"), "ASF_Marker_Object");
            guids.Add(new Guid("D6E229DC-35DA-11D1-9034-00A0C90349BE"), "ASF_Bitrate_Mutual_Exclusion_Object");
            guids.Add(new Guid("75B22635-668E-11CF-A6D9-00AA0062CE6C"), "ASF_Error_Correction_Object");
            guids.Add(new Guid("75B22633-668E-11CF-A6D9-00AA0062CE6C"), "ASF_Content_Description_Object");
            guids.Add(new Guid("D2D0A440-E307-11D2-97F0-00A0C95EA850"), "ASF_Extended_Content_Description_Object");
            guids.Add(new Guid("2211B3FA-BD23-11D2-B4B7-00A0C955FC6E"), "ASF_Content_Branding_Object");
            guids.Add(new Guid("7BF875CE-468D-11D1-8D82-006097C9A2B2"), "ASF_Stream_Bitrate_Properties_Object");
            guids.Add(new Guid("2211B3FB-BD23-11D2-B4B7-00A0C955FC6E"), "ASF_Content_Encryption_Object");
            guids.Add(new Guid("298AE614-2622-4C17-B935-DAE07EE9289C"), "ASF_Extended_Content_Encryption_Object");
            guids.Add(new Guid("2211B3FC-BD23-11D2-B4B7-00A0C955FC6E"), "ASF_Digital_Signature_Object");
            guids.Add(new Guid("1806D474-CADF-4509-A4BA-9AABCB96AAE8"), "ASF_Padding_Object");
            guids.Add(new Guid("14E6A5CB-C672-4332-8399-A96952065B5A"), "ASF_Extended_Stream_Properties_Object");
            guids.Add(new Guid("A08649CF-4775-4670-8A16-6E35357566CD"), "ASF_Advanced_Mutual_Exclusion_Object");
            guids.Add(new Guid("D1465A40-5A79-4338-B71B-E36B8FD6C249"), "ASF_Group_Mutual_Exclusion_Object");
            guids.Add(new Guid("D4FED15B-88D3-454F-81F0-ED5C45999E24"), "ASF_Stream_Prioritization_Object");
            guids.Add(new Guid("A69609E6-517B-11D2-B6AF-00C04FD908E9"), "ASF_Bandwidth_Sharing_Object");
            guids.Add(new Guid("7C4346A9-EFE0-4BFC-B229-393EDE415C85"), "ASF_Language_List_Object");
            guids.Add(new Guid("C5F8CBEA-5BAF-4877-8467-AA8C44FA4CCA"), "ASF_Metadata_Object");
            guids.Add(new Guid("44231C94-9498-49D1-A141-1D134E457054"), "ASF_Metadata_Library_Object");
            guids.Add(new Guid("D6E229DF-35DA-11D1-9034-00A0C90349BE"), "ASF_Index_Parameters_Object");
            guids.Add(new Guid("6B203BAD-3F11-48E4-ACA8-D7613DE2CFA7"), "ASF_Media_Object_Index_Parameters_Object");
            guids.Add(new Guid("F55E496D-9797-4B5D-8C8B-604DFE9BFB24"), "ASF_Timecode_Index_Parameters_Object");
   //         guids.Add(new Guid("75B22630-668E-11CF-A6D9-00AA0062CE6C"), "ASF_Compatibility_Object");
            guids.Add(new Guid("43058533-6981-49E6-9B74-AD12CB86D58C"), "ASF_Advanced_Content_Encryption_Object");
            guids.Add(new Guid("F8699E40-5B4D-11CF-A8FD-00805F5C442B"), "ASF_Audio_Media");
            guids.Add(new Guid("BC19EFC0-5B4D-11CF-A8FD-00805F5C442B"), "ASF_Video_Media");
            guids.Add(new Guid("59DACFC0-59E6-11D0-A3AC-00A0C90348F6"), "ASF_Command_Media");
            guids.Add(new Guid("B61BE100-5B4E-11CF-A8FD-00805F5C442B"), "ASF_JFIF_Media");
            guids.Add(new Guid("35907DE0-E415-11CF-A917-00805F5C442B"), "ASF_Degradable_JPEG_Media");
            guids.Add(new Guid("91BD222C-F21C-497A-8B6D-5AA86BFC0185"), "ASF_File_Transfer_Media");
            guids.Add(new Guid("3AFB65E2-47EF-40F2-AC2C-70A90D71D343"), "ASF_Binary_Media");
            guids.Add(new Guid("776257D4-C627-41CB-8F81-7AC7FF1C40CC"), "ASF_Web_Stream_Media_Subtype");
            guids.Add(new Guid("DA1E6B13-8359-4050-B398-388E965BF00C"), "ASF_Web_Stream_Format");
            guids.Add(new Guid("20FB5700-5B55-11CF-A8FD-00805F5C442B"), "ASF_No_Error_Correction");
            guids.Add(new Guid("BFC3CD50-618F-11CF-8BB2-00AA00B4E220"), "ASF_Audio_Spread");
            guids.Add(new Guid("ABD3D211-A9BA-11cf-8EE6-00C00C205365"), "ASF_Reserved_1");
            guids.Add(new Guid("7A079BB6-DAA4-4e12-A5CA-91D38DC11A8D"), "ASF_Content_Encryption_System_Windows_Media_DRM_Network_Devices");
            guids.Add(new Guid("86D15241-311D-11D0-A3A4-00A0C90348F6"), "ASF_Reserved_2");
        }
        private List<string> blocks = new List<string>();

        public List<string> Blocks
        {
            get { return blocks; }
            set { blocks = value; }
        }
        public WMA_Header(BitStreamReader sw)
        {
            Guid hdrGUID = new Guid("75B22630-668E-11CF-A6D9-00AA0062CE6C");
            Guid contentGUID = new Guid("75B22633-668E-11CF-A6D9-00AA0062CE6C");
            Guid extendedContentGUID = new Guid("D2D0A440-E307-11D2-97F0-00A0C95EA850");
            FillGuids();
            guid = new ELEMENTARY_TYPE(sw, 0, typeof(Guid));
            size = new ELEMENTARY_TYPE(sw, 0, typeof(long));
            number_of_Header_Objects = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            reserved = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 2);
            Wma_Blocks = new List<WMA_Content_Block>();
            for (int u = 0; u < (int)number_of_Header_Objects.Value; u++)
            {
                WMA_Content_Block c_block = new WMA_Content_Block(sw);
                Wma_Blocks.Add(c_block);
            }

        }
    }
    public class ASF_Data_Object: LOCALIZED_DATA
    {
        ELEMENTARY_TYPE object_id, size, file_id, total_packets, reserved;

        public ELEMENTARY_TYPE Object_id
        {
            get { return object_id; }
            set { object_id = value; }
        }

        public ELEMENTARY_TYPE Size
        {
            get { return size; }
            set { size = value; }
        }

        public ELEMENTARY_TYPE File_id
        {
            get { return file_id; }
            set { file_id = value; }
        }

        public ELEMENTARY_TYPE Total_packets
        {
            get { return total_packets; }
            set { total_packets = value; }
        }

        public ELEMENTARY_TYPE Reserved
        {
            get { return reserved; }
            set { reserved = value; }
        }
        List<WMA_Packets> wma_packets;

        public List<WMA_Packets> Wma_packets
        {
            get { return wma_packets; }
            set { wma_packets = value; }
        }
        string bloc_name;

        public string Bloc_name
        {
            get { return bloc_name; }
            set { bloc_name = value; }
        }
        public ASF_Data_Object(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            object_id = new ELEMENTARY_TYPE(sw, 0, typeof(Guid));
            size = new ELEMENTARY_TYPE(sw, 0, typeof(long));
            file_id = new ELEMENTARY_TYPE(sw, 0, typeof(Guid));
            total_packets = new ELEMENTARY_TYPE(sw, 0, typeof(long));
            reserved = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            wma_packets = new List<WMA_Packets>();
            bloc_name = WMA_Header.guids.Values[WMA_Header.guids.IndexOfKey((Guid)object_id.Value)];
            for (int i = 0; i < (long)total_packets.Value;i++ )
            {
                wma_packets.Add(new WMA_Packets(sw));
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return bloc_name;
        }
    }
    public class WMA_Packets : LOCALIZED_DATA
    {
        public WMA_Packets(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            byte b = sw.ReadByte();
            if ((b & 0x80) == 0x80)//Error correction
            {
                int ec_Length = b & 0x0f;
                sw.ReadBytes(2);
            }

            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class WMA_Content_Block : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE guid, sizeofBlock;

        ELEMENTARY_TYPE lTitle, lAuthor, lCopyright, lDescription, lRating;
        ELEMENTARY_TYPE title, author, copyright, description, rating;
        ELEMENTARY_TYPE numAttrs;
        List<WMA_extended_attribute> wma_extended_attributes;
        ELEMENTARY_TYPE bitrate_Records_Count;
        List<WMA_Bitrate_Record> bitrate_records;
        ELEMENTARY_TYPE file_id, file_size, creation_date, data_packets_count, play_duration, send_duration,
             preroll, flags, minimum_data_packet_size, maximum_data_packet_size, maximum_bitrate;
        ELEMENTARY_TYPE Reserved, codec_Entries_Count;
        List<WMA_Codec_Entry> codec_entries;
        ELEMENTARY_TYPE stream_type, error_correction_type, time_offset, type_specific_data_length, 
            error_correction_data_length, flags_stream, reserved_stream, type_specific_data, error_correction_data;


        #region Properties
        public ELEMENTARY_TYPE Guid
        {
            get { return guid; }
            set { guid = value; }
        }
        public ELEMENTARY_TYPE SizeofBlock
        {
            get { return sizeofBlock; }
            set { sizeofBlock = value; }
        }
        #endregion
        #region Content_block
        public ELEMENTARY_TYPE LTitle
        {
            get { return lTitle; }
            set { lTitle = value; }
        }
        public ELEMENTARY_TYPE LAuthor
        {
            get { return lAuthor; }
            set { lAuthor = value; }
        }
        public ELEMENTARY_TYPE LCopyright
        {
            get { return lCopyright; }
            set { lCopyright = value; }
        }
        public ELEMENTARY_TYPE LDescription
        {
            get { return lDescription; }
            set { lDescription = value; }
        }
        public ELEMENTARY_TYPE LRating
        {
            get { return lRating; }
            set { lRating = value; }
        }
        public ELEMENTARY_TYPE Title
        {
            get { return title; }
            set { title = value; }
        }
        public ELEMENTARY_TYPE Author
        {
            get { return author; }
            set { author = value; }
        }
        public ELEMENTARY_TYPE Copyright
        {
            get { return copyright; }
            set { copyright = value; }
        }
        public ELEMENTARY_TYPE Description
        {
            get { return description; }
            set { description = value; }
        }
        public ELEMENTARY_TYPE Rating
        {
            get { return rating; }
            set { rating = value; }
        }
        #endregion
        #region Extended block
        public ELEMENTARY_TYPE NumAttrs
        {
            get { return numAttrs; }
            set { numAttrs = value; }
        }
        public List<WMA_extended_attribute> Wma_extended_attributes
        {
            get { return wma_extended_attributes; }
            set { wma_extended_attributes = value; }
        }
        #endregion
        #region Bitrate
        public ELEMENTARY_TYPE Bitrate_Records_Count
        {
            get { return bitrate_Records_Count; }
            set { bitrate_Records_Count = value; }
        }
        public List<WMA_Bitrate_Record> Bitrate_records
        {
            get { return bitrate_records; }
            set { bitrate_records = value; }
        }
        #endregion
        #region File properties
        public ELEMENTARY_TYPE File_id
        {
            get { return file_id; }
            set { file_id = value; }
        }

        public ELEMENTARY_TYPE File_size
        {
            get { return file_size; }
            set { file_size = value; }
        }

        public ELEMENTARY_TYPE Creation_date
        {
            get { return creation_date; }
            set { creation_date = value; }
        }

        public ELEMENTARY_TYPE Data_packets_count
        {
            get { return data_packets_count; }
            set { data_packets_count = value; }
        }

        public ELEMENTARY_TYPE Play_duration
        {
            get { return play_duration; }
            set { play_duration = value; }
        }

        public ELEMENTARY_TYPE Send_duration
        {
            get { return send_duration; }
            set { send_duration = value; }
        }

        public ELEMENTARY_TYPE Preroll
        {
            get { return preroll; }
            set { preroll = value; }
        }

        public ELEMENTARY_TYPE Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        public ELEMENTARY_TYPE Minimum_data_packet_size
        {
            get { return minimum_data_packet_size; }
            set { minimum_data_packet_size = value; }
        }

        public ELEMENTARY_TYPE Maximum_data_packet_size
        {
            get { return maximum_data_packet_size; }
            set { maximum_data_packet_size = value; }
        }

        public ELEMENTARY_TYPE Maximum_bitrate
        {
            get { return maximum_bitrate; }
            set { maximum_bitrate = value; }
        }
        #endregion
        #region Codec
        public ELEMENTARY_TYPE Codec_Entries_Count
        {
            get { return codec_Entries_Count; }
            set { codec_Entries_Count = value; }
        }
        public List<WMA_Codec_Entry> Codec_entries
        {
            get { return codec_entries; }
            set { codec_entries = value; }
        }
        #endregion
        #region Stream properties
        public ELEMENTARY_TYPE Stream_type
        {
            get { return stream_type; }
            set { stream_type = value; }
        }
        public ELEMENTARY_TYPE Error_correction_type
        {
            get { return error_correction_type; }
            set { error_correction_type = value; }
        }
        public ELEMENTARY_TYPE Time_offset
        {
            get { return time_offset; }
            set { time_offset = value; }
        }
        public ELEMENTARY_TYPE Type_specific_data_length
        {
            get { return type_specific_data_length; }
            set { type_specific_data_length = value; }
        }
        public ELEMENTARY_TYPE Error_correction_data_length
        {
            get { return error_correction_data_length; }
            set { error_correction_data_length = value; }
        }
        public ELEMENTARY_TYPE Flags1
        {
            get { return flags_stream; }
            set { flags_stream = value; }
        }
        public ELEMENTARY_TYPE Reservedflags_stream
        {
            get { return reserved_stream; }
            set { reserved_stream = value; }
        }
        public ELEMENTARY_TYPE Type_specific_data
        {
            get { return type_specific_data; }
            set { type_specific_data = value; }
        }
        public ELEMENTARY_TYPE Error_correction_data
        {
            get { return error_correction_data; }
            set { error_correction_data = value; }
        }
        #endregion

        string guid_name;
        public WMA_Content_Block(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            guid = new ELEMENTARY_TYPE(sw, 0, typeof(Guid));
            sizeofBlock = new ELEMENTARY_TYPE(sw, 0, typeof(long));
            guid_name = WMA_Header.guids.Values[WMA_Header.guids.IndexOfKey((Guid)guid.Value)];
            switch (guid_name)
            {
                case "ASF_Content_Description_Object":
                    lTitle = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                    lAuthor = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                    lCopyright = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                    lDescription = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                    lRating = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                    if ((short)lTitle.Value > 0)
                        title = new ELEMENTARY_TYPE(sw, 0, Encoding.Unicode, (short)lTitle.Value / 2);
                    if ((short)lAuthor.Value > 0)
                        author = new ELEMENTARY_TYPE(sw, 0, Encoding.Unicode, (short)lAuthor.Value / 2);
                    if ((short)lCopyright.Value > 0)
                        copyright = new ELEMENTARY_TYPE(sw, 0, Encoding.Unicode, (short)lCopyright.Value / 2);
                    if ((short)lDescription.Value > 0)
                        description = new ELEMENTARY_TYPE(sw, 0, Encoding.Unicode, (short)lDescription.Value / 2);
                    if ((short)lRating.Value > 0)
                        rating = new ELEMENTARY_TYPE(sw, 0, Encoding.Unicode, (short)lRating.Value / 2);
                    LengthInFile = sw.Position - PositionOfStructureInFile;
                    break;
                case "ASF_Extended_Content_Description_Object":
                    numAttrs = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                    wma_extended_attributes = new List<WMA_extended_attribute>();
                    for (int u = 0; u < (short)numAttrs.Value; u++)
                    {
                        wma_extended_attributes.Add(new WMA_extended_attribute(sw));
                    }
                    break;
                case "ASF_File_Properties_Object":
                    file_id = new ELEMENTARY_TYPE(sw, 0, typeof(Guid));
                    file_size = new ELEMENTARY_TYPE(sw, 0, typeof(long));
                    creation_date = new ELEMENTARY_TYPE(sw, 0, typeof(long));
                    data_packets_count = new ELEMENTARY_TYPE(sw, 0, typeof(long));
                    play_duration = new ELEMENTARY_TYPE(sw, 0, typeof(long));
                    send_duration = new ELEMENTARY_TYPE(sw, 0, typeof(long));
                    preroll = new ELEMENTARY_TYPE(sw, 0, typeof(long));
                    flags = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    minimum_data_packet_size = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    maximum_data_packet_size = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    maximum_bitrate = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    break;
                case "ASF_Header_Extension_Object":
                    sw.ReadBlock((int)(long)sizeofBlock.Value - 24);
                    break;
                case "ASF_Codec_List_Object":
                    Reserved = new ELEMENTARY_TYPE(sw, 0, typeof(Guid));
                    codec_Entries_Count = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    codec_entries = new List<WMA_Codec_Entry>();
                    for (int uc = 0; uc < (int)codec_Entries_Count.Value; uc++)
                    {
                        codec_entries.Add(new WMA_Codec_Entry(sw));
                    }
                    break;
                case "ASF_Stream_Properties_Object":
                    int start = (int) sw.Position;
                    stream_type = new ELEMENTARY_TYPE(sw, 0, typeof(Guid));
                    error_correction_type = new ELEMENTARY_TYPE(sw, 0, typeof(Guid));
                    time_offset = new ELEMENTARY_TYPE(sw, 0, typeof(long));
                    type_specific_data_length = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    error_correction_data_length = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    flags_stream = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                    reserved_stream = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    type_specific_data = new ELEMENTARY_TYPE(sw, 0, typeof(long));
                    error_correction_data = new ELEMENTARY_TYPE(sw, 0, typeof(long));
                    int le = (int)sw.Position - start;
                    sw.ReadBlock((int)(long)sizeofBlock.Value - 24 - le);
                    break;
                case "ASF_Stream_Bitrate_Properties_Object":
                    bitrate_Records_Count = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                    bitrate_records = new List<WMA_Bitrate_Record>();
                    for (int u = 0; u < (short)bitrate_Records_Count.Value; u++)
                    {
                        bitrate_records.Add(new WMA_Bitrate_Record(sw));
                    }
                    break;
                default:
                    sw.ReadBlock((int)(long)sizeofBlock.Value - 24);
                    break;
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return guid_name;
        }
    }
    public class WMA_Codec_Entry:LOCALIZED_DATA
    {
        ELEMENTARY_TYPE codec_Type, name_length, name, description_length, description, information_length, information;

        public ELEMENTARY_TYPE Codec_Type
        {
            get { return codec_Type; }
            set { codec_Type = value; }
        }
        public ELEMENTARY_TYPE Name_length
        {
            get { return name_length; }
            set { name_length = value; }
        }
        public ELEMENTARY_TYPE Name
        {
            get { return name; }
            set { name = value; }
        }
        public ELEMENTARY_TYPE Description_length
        {
            get { return description_length; }
            set { description_length = value; }
        }
        public ELEMENTARY_TYPE Description
        {
            get { return description; }
            set { description = value; }
        }
        public ELEMENTARY_TYPE Information_length
        {
            get { return information_length; }
            set { information_length = value; }
        }
        public ELEMENTARY_TYPE Information
        {
            get { return information; }
            set { information = value; }
        }
        public string Codec
        {
            get
            {
                switch ((short)codec_Type.Value)
                {
                    case 1: return "Video codec";
                    case 2: return "Audio codec";
                    default: return "unknown";
                }
            }
        }
        public WMA_Codec_Entry(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            codec_Type = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            name_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            if ((short)name_length.Value / 2 > 0)
                name = new ELEMENTARY_TYPE(sw, 0, Encoding.Unicode, (short)name_length.Value / 2 );
            description_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            if ((short)description_length.Value / 2 > 0)
                description = new ELEMENTARY_TYPE(sw, 0, Encoding.Unicode, (short)description_length.Value / 2);
            information_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            if ((short)information_length.Value / 2 > 0)
                information = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), (short)information_length.Value);

            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return (string)name.Value;
        }
    }
    public class WMA_Bitrate_Record : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE flags, average_bitrate;
        public ELEMENTARY_TYPE Flags
        {
            get { return flags; }
            set { flags = value; }
        }
        public ELEMENTARY_TYPE Average_bitrate
        {
            get { return average_bitrate; }
            set { average_bitrate = value; }
        }
        public WMA_Bitrate_Record(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            flags = new ELEMENTARY_TYPE(sw, 0, typeof(short)); ;
            average_bitrate = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class WMA_extended_attribute : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE nameLength, attrName, dataType, valueLength, value;
        Image image;

        public ELEMENTARY_TYPE NameLength
        {
            get { return nameLength; }
            set { nameLength = value; }
        }
        public ELEMENTARY_TYPE AttrName
        {
            get { return attrName; }
            set { attrName = value; }
        }
        public ELEMENTARY_TYPE DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }
        public ELEMENTARY_TYPE ValueLength
        {
            get { return valueLength; }
            set { valueLength = value; }
        }
        public ELEMENTARY_TYPE Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public Image Image
        {
            get { return image; }
            set { image = value; }
        }
        public WMA_extended_attribute(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            try
            {
                nameLength = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                attrName = new ELEMENTARY_TYPE(sw, 0, Encoding.Unicode, (short)nameLength.Value / 2);
                dataType = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                switch ((short)dataType.Value)
                {
                    #region Read data
                    case 0:
                        valueLength = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                        value = new ELEMENTARY_TYPE(sw, 0, Encoding.Unicode, (short)valueLength.Value / 2);
                        /*                  switch ((string)attrName.Value)
                                          {
                                             case "WM/Lyrics":
                                                  break;
                                              case "WM/AlbumTitle":
                                                  mp3_TALB = value;
                                                  break;
                                              case "WM/AlbumArtist":
                                                  mp3_TPE1 = value;
                                                  break;
                                              case "WM/Genre":
                                                  mp3_TCON = value;
                                                  break;
                                              case "WM/TrackNumber":
                                                  mp3_TRCK = value;
                                                  break;
                                              case "WM/Year":
                                                  break;
                                              case "WM/Composer":
                                                  mp3_TCOM = value;
                                                  break;
                                              case "WM/Publisher":
                                                  break;
                                          }*/
                        break;
                    case 1:
                        valueLength = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                        if ((string)attrName.Value == "WM/Picture")
                        {
                            value = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), (short)valueLength.Value);

                        }
                        else
                            value = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), (short)valueLength.Value);
                         break;
                    case 2:
                        valueLength = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                        value = new ELEMENTARY_TYPE(sw, 0, typeof(int));                  
                        break;
                    case 3:
                        valueLength = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                        value = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                        break;
                    case 4:
                        valueLength = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                        value = new ELEMENTARY_TYPE(sw, 0, typeof(long));
                        break;
                    case 5:
                        valueLength = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                        value = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                        break;
                    #endregion
                }
            }
            catch (Exception ex) { }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return attrName + " : " + value.ToString(); ;
        }
    }
    public class ID1Tag : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE oldTag;
        ELEMENTARY_TYPE song_Name;
        ELEMENTARY_TYPE artist;
        ELEMENTARY_TYPE album;
        ELEMENTARY_TYPE year;
        ELEMENTARY_TYPE comment;
        ELEMENTARY_TYPE genre;
        #region Properties
        public ELEMENTARY_TYPE OldTag
        {
            get { return oldTag; }
            set { oldTag = value; }
        }
        public ELEMENTARY_TYPE Song_Name
        {
            get { return song_Name; }
            set { song_Name = value; }
        }
        public ELEMENTARY_TYPE Artist
        {
            get { return artist; }
            set { artist = value; }
        }
        public ELEMENTARY_TYPE Album
        {
            get { return album; }
            set { album = value; }
        }
        public ELEMENTARY_TYPE Year
        {
            get { return year; }
            set { year = value; }
        }
        public ELEMENTARY_TYPE Comment
        {
            get { return comment; }
            set { comment = value; }
        }
        public ELEMENTARY_TYPE Genre
        {
            get { return genre; }
            set { genre = value; }
        }
        #endregion
        public ID1Tag(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            oldTag = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 3);
            song_Name = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 30);
            artist =  new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 30);
            album = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 30);
            year = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 30);
            comment = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            genre = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 1);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class ID3Header : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE iD3v2_file_identifier;//   "ID3" 
        ELEMENTARY_TYPE iD3v2_version;//          $03 00
        ELEMENTARY_TYPE iD3v2_flags;     //        %abc00000
        ELEMENTARY_TYPE iD3v2_size;
        bool unsynchronisation;//Bit 7 in the 'ID3v2 flags' indicates whether or not unsynchronisation is used (see section 5 for details); a set bit indicates usage. 
        bool extended_header;//The second bit (bit 6) indicates whether or not the header is followed by an extended header. The extended header is described in section 3.2. 
        bool experimental_indicator;//The third bit (bit 5) should be used as an 'experimental indicator'. This flag should always be set when the tag is in an experimental stage. 
        private int size;

        public bool Unsynchronisation
        {
            get { return unsynchronisation; }
            set { unsynchronisation = value; }
        }
        public bool Extended_header
        {
            get { return extended_header; }
            set { extended_header = value; }
        }
        public bool Experimental_indicator
        {
            get { return experimental_indicator; }
            set { experimental_indicator = value; }
        }
        public int Size
        {
            get { return size; }
            set { size = value; }
        }
        public ELEMENTARY_TYPE ID3v2_file_identifier
        {
            get { return iD3v2_file_identifier; }
            set { iD3v2_file_identifier = value; }
        }
        public ELEMENTARY_TYPE ID3v2_version
        {
            get { return iD3v2_version; }
            set { iD3v2_version = value; }
        }
        public ELEMENTARY_TYPE ID3v2_flags
        {
            get { return iD3v2_flags; }
            set { iD3v2_flags = value; }
        }
        public ELEMENTARY_TYPE ID3v2_size
        {
            get { return iD3v2_size; }
            set { iD3v2_size = value; }
        }
        public ID3Header(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            iD3v2_file_identifier = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 3);
            iD3v2_version = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 2);
            iD3v2_flags = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            iD3v2_size = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 4);
            byte[] localBuffer = (byte[])iD3v2_size.Value;
            //The ID3v2 tag size is encoded with four bytes where the most significant bit (bit 7) is set to zero in every byte, making a total of 28 bits. The zeroed bits are ignored, so a 257 bytes long tag is represented as $00 00 02 01. 
            int[] nb = new int[4];
            nb[3] = localBuffer[3] | ((localBuffer[2] & 1) << 7);
            nb[2] = ((localBuffer[2] >> 1) & 63) | ((localBuffer[1] & 3) << 6);
            nb[1] = ((localBuffer[1] >> 2) & 31) | ((localBuffer[0] & 7) << 5);
            nb[0] = ((localBuffer[0] >> 3) & 15);
            size = (int)(10 + (ulong)nb[3] | ((ulong)nb[2] << 8) | ((ulong)nb[1] << 16) | ((ulong)nb[0] << 24));
            unsynchronisation=(((byte)iD3v2_flags.Value) ==0x80);
            extended_header = (((byte)iD3v2_flags.Value) == 0x40);
            experimental_indicator = (((byte)iD3v2_flags.Value) == 0x20);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class MusicMatchTag : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE start;
        ELEMENTARY_TYPE padding;
        ELEMENTARY_TYPE magic_1;

        ELEMENTARY_TYPE xing_1;
        ELEMENTARY_TYPE musicMatch_1;
        ELEMENTARY_TYPE picture_extension;
        ELEMENTARY_TYPE image_size;
        ELEMENTARY_TYPE image_data;
        ELEMENTARY_TYPE magic;
        ELEMENTARY_TYPE xing;
        ELEMENTARY_TYPE musicMatch;
        ELEMENTARY_TYPE paddingSpace;

        List<ELEMENTARY_TYPE> extra;
        ELEMENTARY_TYPE song_length;
        ELEMENTARY_TYPE song_Title;
        ELEMENTARY_TYPE album_Length;
        ELEMENTARY_TYPE album_Title;
        ELEMENTARY_TYPE artist_Length;
        ELEMENTARY_TYPE artist_Name;
        ELEMENTARY_TYPE genre_length;//            $xx xx
        ELEMENTARY_TYPE genre;//                 <ASCII string>
        ELEMENTARY_TYPE tempo_length;//               $xx xx
        ELEMENTARY_TYPE tempo;//               <ASCII string>
        ELEMENTARY_TYPE mood_length;//              $xx xx
        ELEMENTARY_TYPE mood;//               <ASCII string>
        ELEMENTARY_TYPE situation_length;//          $xx xx
        ELEMENTARY_TYPE situation;//              <ASCII string>
        ELEMENTARY_TYPE preference_length;//          $xx xx
        ELEMENTARY_TYPE preference;//              <ASCII string>
        ELEMENTARY_TYPE song_duration_length;//     $xx xx
        ELEMENTARY_TYPE song_duration;//   <ASCII string>
        ELEMENTARY_TYPE creation_date;//         <8-byte IEEE-64 float>
        ELEMENTARY_TYPE play_counter;//         $xx xx xx xx
        ELEMENTARY_TYPE original_filename_length;//   $xx xx
        ELEMENTARY_TYPE original_filename;//      <ASCII string>
        ELEMENTARY_TYPE serial_number_length;//      $xx xx
        ELEMENTARY_TYPE serial_number;//         <ASCII string>
        ELEMENTARY_TYPE track_number;//           $xx xx
        ELEMENTARY_TYPE notes_length;
        ELEMENTARY_TYPE notes;
        ELEMENTARY_TYPE artist_bio_length;
        ELEMENTARY_TYPE artist_bio;
        ELEMENTARY_TYPE lyrics_length;
        ELEMENTARY_TYPE lyrics;
        ELEMENTARY_TYPE artist_url_length;
        ELEMENTARY_TYPE artist_url;
        ELEMENTARY_TYPE buy_cd_url_length;
        ELEMENTARY_TYPE buy_cd_url;
        ELEMENTARY_TYPE artist_email_length;
        ELEMENTARY_TYPE artist_email;
        ELEMENTARY_TYPE pd0;
        ELEMENTARY_TYPE padding2;
        ELEMENTARY_TYPE image_extension_offset;
        ELEMENTARY_TYPE image_binary_offset;
        ELEMENTARY_TYPE unused_offset;
        ELEMENTARY_TYPE version_info_offset;
        ELEMENTARY_TYPE audio_metadata_offset;
        ELEMENTARY_TYPE footer_string;

        #region Properties
        public ELEMENTARY_TYPE Magic_1
        {
            get { return magic_1; }
            set { magic_1 = value; }
        }
        public ELEMENTARY_TYPE Xing_1
        {
            get { return xing_1; }
            set { xing_1 = value; }
        }
        public ELEMENTARY_TYPE MusicMatch_1
        {
            get { return musicMatch_1; }
            set { musicMatch_1 = value; }
        }
        public ELEMENTARY_TYPE Picture_extension
        {
            get { return picture_extension; }
            set { picture_extension = value; }
        }
        public ELEMENTARY_TYPE Image_size
        {
            get { return image_size; }
            set { image_size = value; }
        }
        public ELEMENTARY_TYPE Image_data
        {
            get { return image_data; }
            set { image_data = value; }
        }
        public ELEMENTARY_TYPE Start
        {
            get { return start; }
            set { start = value; }
        }
        public ELEMENTARY_TYPE Padding
        {
            get { return padding; }
            set { padding = value; }
        }
        public ELEMENTARY_TYPE Magic
        {
            get { return magic; }
            set { magic = value; }
        }
        public ELEMENTARY_TYPE Xing
        {
            get { return xing; }
            set { xing = value; }
        }
        public ELEMENTARY_TYPE MusicMatch
        {
            get { return musicMatch; }
            set { musicMatch = value; }
        }
        public List<ELEMENTARY_TYPE> Extra
        {
            get { return extra; }
            set { extra = value; }
        }
        public ELEMENTARY_TYPE PaddingSpace
        {
            get { return paddingSpace; }
            set { paddingSpace = value; }
        }
        public ELEMENTARY_TYPE Song_length
        {
            get { return song_length; }
            set { song_length = value; }
        }
        public ELEMENTARY_TYPE Song_Title
        {
            get { return song_Title; }
            set { song_Title = value; }
        }
        public ELEMENTARY_TYPE Album_Length
        {
            get { return album_Length; }
            set { album_Length = value; }
        }
        public ELEMENTARY_TYPE Album_Title
        {
            get { return album_Title; }
            set { album_Title = value; }
        }
        public ELEMENTARY_TYPE Artist_Length
        {
            get { return artist_Length; }
            set { artist_Length = value; }
        }
        public ELEMENTARY_TYPE Artist_Name
        {
            get { return artist_Name; }
            set { artist_Name = value; }
        }
        public ELEMENTARY_TYPE Genre_length
        {
            get { return genre_length; }
            set { genre_length = value; }
        }
        public ELEMENTARY_TYPE Genre
        {
            get { return genre; }
            set { genre = value; }
        }
        public ELEMENTARY_TYPE Tempo_length
        {
            get { return tempo_length; }
            set { tempo_length = value; }
        }
        public ELEMENTARY_TYPE Tempo
        {
            get { return tempo; }
            set { tempo = value; }
        }
        public ELEMENTARY_TYPE Mood_length
        {
            get { return mood_length; }
            set { mood_length = value; }
        }
        public ELEMENTARY_TYPE Mood
        {
            get { return mood; }
            set { mood = value; }
        }
        public ELEMENTARY_TYPE Situation_length
        {
            get { return situation_length; }
            set { situation_length = value; }
        }
        public ELEMENTARY_TYPE Situation
        {
            get { return situation; }
            set { situation = value; }
        }
        public ELEMENTARY_TYPE Preference_length
        {
            get { return preference_length; }
            set { preference_length = value; }
        }
        public ELEMENTARY_TYPE Preference
        {
            get { return preference; }
            set { preference = value; }
        }
        public ELEMENTARY_TYPE Song_duration_length
        {
            get { return song_duration_length; }
            set { song_duration_length = value; }
        }
        public ELEMENTARY_TYPE Song_duration
        {
            get { return song_duration; }
            set { song_duration = value; }
        }
        public ELEMENTARY_TYPE Creation_date
        {
            get { return creation_date; }
            set { creation_date = value; }
        }
        public ELEMENTARY_TYPE Play_counter
        {
            get { return play_counter; }
            set { play_counter = value; }
        }
        public ELEMENTARY_TYPE Original_filename_length
        {
            get { return original_filename_length; }
            set { original_filename_length = value; }
        }
        public ELEMENTARY_TYPE Original_filename
        {
            get { return original_filename; }
            set { original_filename = value; }
        }
        public ELEMENTARY_TYPE Serial_number_length
        {
            get { return serial_number_length; }
            set { serial_number_length = value; }
        }
        public ELEMENTARY_TYPE Serial_number
        {
            get { return serial_number; }
            set { serial_number = value; }
        }
        public ELEMENTARY_TYPE Track_number
        {
            get { return track_number; }
            set { track_number = value; }
        }
        public ELEMENTARY_TYPE Notes_length
        {
            get { return notes_length; }
            set { notes_length = value; }
        }
        public ELEMENTARY_TYPE Notes
        {
            get { return notes; }
            set { notes = value; }
        }
        public ELEMENTARY_TYPE Artist_bio_length
        {
            get { return artist_bio_length; }
            set { artist_bio_length = value; }
        }

        public ELEMENTARY_TYPE Artist_bio
        {
            get { return artist_bio; }
            set { artist_bio = value; }
        }
        public ELEMENTARY_TYPE Lyrics_length
        {
            get { return lyrics_length; }
            set { lyrics_length = value; }
        }

        public ELEMENTARY_TYPE Lyrics
        {
            get { return lyrics; }
            set { lyrics = value; }
        }
        public ELEMENTARY_TYPE Artist_url_length
        {
            get { return artist_url_length; }
            set { artist_url_length = value; }
        }
        public ELEMENTARY_TYPE Artist_url
        {
            get { return artist_url; }
            set { artist_url = value; }
        }

        public ELEMENTARY_TYPE Buy_cd_url_length
        {
            get { return buy_cd_url_length; }
            set { buy_cd_url_length = value; }
        }
        public ELEMENTARY_TYPE Buy_cd_url
        {
            get { return buy_cd_url; }
            set { buy_cd_url = value; }
        }
        public ELEMENTARY_TYPE Artist_email_length
        {
            get { return artist_email_length; }
            set { artist_email_length = value; }
        }
        public ELEMENTARY_TYPE Artist_email
        {
            get { return artist_email; }
            set { artist_email = value; }
        }
        public ELEMENTARY_TYPE Pd0
        {
            get { return pd0; }
            set { pd0 = value; }
        }
        public ELEMENTARY_TYPE Padding2
        {
            get { return padding2; }
            set { padding2 = value; }
        }
        public ELEMENTARY_TYPE Image_extension_offset
        {
            get { return image_extension_offset; }
            set { image_extension_offset = value; }
        }
        public ELEMENTARY_TYPE Image_binary_offset
        {
            get { return image_binary_offset; }
            set { image_binary_offset = value; }
        }
        public ELEMENTARY_TYPE Unused_offset
        {
            get { return unused_offset; }
            set { unused_offset = value; }
        }
        public ELEMENTARY_TYPE Version_info_offset
        {
            get { return version_info_offset; }
            set { version_info_offset = value; }
        }
        public ELEMENTARY_TYPE Audio_metadata_offset
        {
            get { return audio_metadata_offset; }
            set { audio_metadata_offset = value; }
        }
        public ELEMENTARY_TYPE Footer_string
        {
            get { return footer_string; }
            set { footer_string = value; }
        }
        #endregion
        public MusicMatchTag(BitStreamReader sw, int frame_sync)
        {
            // 18273645 ??
            //"Brava Software Inc.             "
            PositionOfStructureInFile = sw.Position;
            return;
            if (frame_sync == 0x202)
            {
                start = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
                padding = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 8);
            }
            else
            {
                magic_1 = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 10);
                xing_1 = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 10);
                musicMatch_1 = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 10);
                paddingSpace = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 226);
                picture_extension = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                image_size = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                image_data = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            }
            magic = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 10);
            xing = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 10);
            musicMatch = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 10);
            paddingSpace = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 226);
            bool ltl = sw.LittleEndian;
            sw.LittleEndian = false;
            song_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            if ((short)song_length.Value > 0)
                song_Title = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)song_length.Value);
            album_Length = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            if ((short)album_Length.Value > 0)
                album_Title = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)album_Length.Value);
            artist_Length = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            if ((short)artist_Length.Value > 0)
                artist_Name = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)artist_Length.Value);
            genre_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));//            $xx xx
            if ((short)genre_length.Value > 0)
                genre = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)genre_length.Value);//                 <ASCII string>
            tempo_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));//               $xx xx
            if ((short)tempo_length.Value > 0)
                tempo = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)tempo_length.Value);//               <ASCII string>
            mood_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));//              $xx xx
            if ((short)mood_length.Value > 0)
                mood = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)mood_length.Value);//               <ASCII string>
            situation_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));//          $xx xx
            if ((short)situation_length.Value > 0)
                situation = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)situation_length.Value);//              <ASCII string>
            preference_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));//          $xx xx
            if ((short)preference_length.Value > 0)
                preference = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)preference_length.Value);//              <ASCII string>
            song_duration_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));//     $xx xx
            if ((short)song_duration_length.Value > 0)
                song_duration = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)song_duration_length.Value);//   <ASCII string>
            creation_date = new ELEMENTARY_TYPE(sw, 0, typeof(long));//         <8-byte IEEE-64 float>
            play_counter = new ELEMENTARY_TYPE(sw, 0, typeof(int)); ;//         $xx xx xx xx
            original_filename_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));//   $xx xx
            if ((short)original_filename_length.Value > 0)
                original_filename = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)original_filename_length.Value);//      <ASCII string>
            serial_number_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));//      $xx xx
            if ((short)serial_number_length.Value > 0)
                serial_number = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)serial_number_length.Value);//         <ASCII string>
            track_number = new ELEMENTARY_TYPE(sw, 0, typeof(short));//           $xx xx
            notes_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            if ((short)notes_length.Value > 0)
                notes = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)notes_length.Value);
            artist_bio_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            if ((short)artist_bio_length.Value > 0)
                artist_bio = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)artist_bio_length.Value);
            lyrics_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            if ((short)lyrics_length.Value > 0)
                lyrics = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)lyrics_length.Value);
            artist_url_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            if ((short)artist_url_length.Value > 0)
                artist_url = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)artist_url_length.Value);
            buy_cd_url_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            if ((short)buy_cd_url_length.Value > 0)
                buy_cd_url = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)buy_cd_url_length.Value);
            artist_email_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            if ((short)artist_email_length.Value > 0)
                artist_email = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)artist_email_length.Value);
            pd0 = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 16);
            padding2 = new ELEMENTARY_TYPE(sw, 0, Encoding.Default);
            if (((string)musicMatch.Value).Contains("2."))
            {
                sw.Position -= 4;
                image_extension_offset = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                image_binary_offset = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                unused_offset = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                version_info_offset = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                audio_metadata_offset = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                footer_string = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 48);
            }
            sw.LittleEndian = ltl;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class MP3Header : LOCALIZED_DATA
    {
        /*        ELEMENTARY_TYPE frame_synchronizer;
                ELEMENTARY_TYPE version_ID;
                ELEMENTARY_TYPE layer;
                ELEMENTARY_TYPE cRC_Protection;
                ELEMENTARY_TYPE bitrate_index;
                ELEMENTARY_TYPE sampling_rate_frequency_index;
                ELEMENTARY_TYPE padding;
                ELEMENTARY_TYPE private_bit;
                ELEMENTARY_TYPE channel;
                ELEMENTARY_TYPE mode_extension;
                ELEMENTARY_TYPE copyright;
                ELEMENTARY_TYPE original;
                ELEMENTARY_TYPE emphasis;
        */
        public int Frame_synchronizer
        {
            get { return frame_synchronizer; }
            set { frame_synchronizer = value; }
        }
        public int Version_ID
        {
            get { return version_ID; }
            set { version_ID = value; }
        }
        public int Layer
        {
            get { return layer; }
            set { layer = value; }
        }
        public int CRC_Protection
        {
            get { return cRC_Protection; }
            set { cRC_Protection = value; }
        }
        public int Bitrate_index
        {
            get { return bitrate; }
            set { bitrate = value; }
        }
        public int Sampling_rate_frequency_index
        {
            get { return frequency; }
            set { frequency= value; }
        }
        public int Padding
        {
            get { return padding; }
            set { padding = value; }
        }
        public int Private_bit
        {
            get { return private_bit; }
            set { private_bit = value; }
        }
        public int Channel
        {
            get { return channel; }
            set { channel = value; }
        }
        public int Mode_extension
        {
            get { return mode_extension; }
            set { mode_extension = value; }
        }
        public int Copyright
        {
            get { return copyright; }
            set { copyright = value; }
        }
        public int Original
        {
            get { return original; }
            set { original = value; }
        }
        public int Emphasis
        {
            get { return emphasis; }
            set { emphasis = value; }
        }
        int frame_synchronizer;
        int version_ID;
        int layer;
        int cRC_Protection;
        int bitrate;
        int frequency;
        int padding;
        int private_bit;
        int channel;
        int mode_extension;
        int copyright;
        int original;
        int emphasis;
        int bitrate_value;

        public int Bitrate_value
        {
            get { return bitrate_value; }
            set { bitrate_value = value; }
        }
        int frequency_value;

        public int Frequency_value
        {
            get { return frequency_value; }
            set { frequency_value = value; }
        }
        public string Mpeg
        {
            get
            {
                string[] mpeg_version = new[] { "MPEG_Version_2", "MPEG Version 1", "Unknown", "" };
                string[] mpeg_layer = new[] { "reserved", "Layer III", "Layer II", "Layer I" };
                return mpeg_version[version_ID] + " " + mpeg_layer[layer];
            }
        }
        public string Channels
        {
            get
            {
                string[] channels = new string[] { "Stereo", "Joint Stereo", "Dual", "Mono (single channel)" };
                return channels[channel];
            }
        }
        MusicMatchTag musicMatch;

        public MusicMatchTag MusicMatch
        {
            get { return musicMatch; }
            set { musicMatch = value; }
        }
        public MP3Header(BitStreamReader sw, bool aa)
        {
            PositionOfStructureInFile = sw.Position;
            int[] bitrates = new int[] { -1, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, -1 };
            int[] frequencies = new int[] { 44100, 48000, 32000, -1 };
            string[] emphas = new string[] { "None", "50/15", "reserved", "CCIT J.17" };
            // 18273645 ??
            //"Brava Software Inc.             "
            frame_synchronizer = sw.ReadIntFromBits(12);
            while ((frame_synchronizer != 0xfff)&(sw.Position<sw.Length))
            {
                if (frame_synchronizer == 0x202)
                {
     /*               extra = new List<ELEMENTARY_TYPE>();
                    sw.Position += 3;
                    byte a = sw.ReadByte();
                    while (a == 0x0)
                    {
                        a = sw.ReadByte();
                    }
                    sw.Position -= 1;
               //     long l = sw.Length - 128 - sw.Position;
                    while (sw.Position < sw.Length - 128)
                    {
                //        ELEMENTARY_TYPE start = 
                        extra.Add(new ELEMENTARY_TYPE(sw, 0, Encoding.Default));

                        a = sw.ReadByte();
                        while (a == 0x0)
                        {
                            a = sw.ReadByte();
                        }
                        sw.Position -= 1;
                    }
                    return;*/
                }
                sw.BitPosition = sw.Position * 8;
                PositionOfStructureInFile = sw.Position;
                frame_synchronizer = sw.ReadIntFromBits(12);
             }
            version_ID = sw.ReadIntFromBits(1);
            layer = sw.ReadIntFromBits(2);
            cRC_Protection = sw.ReadIntFromBits(1);
            bitrate = sw.ReadIntFromBits(4);
            bitrate_value = bitrates[bitrate];
            frequency = sw.ReadIntFromBits(2);
            frequency_value = frequencies[frequency];
            padding = sw.ReadIntFromBits(1);
            private_bit = sw.ReadIntFromBits(1);
            channel = sw.ReadIntFromBits(2);
            mode_extension = sw.ReadIntFromBits(2);
            copyright = sw.ReadIntFromBits(1);
            original = sw.ReadIntFromBits(1);
            emphasis = sw.ReadIntFromBits(1);
            LengthInFile = 4;
            sw.Position = (int)PositionOfStructureInFile + 4;
        }
        public MP3Header(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            int[] bitrates = new int[] { -1, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, -1 };
            int[] frequencies = new int[] { 44100, 48000, 32000, -1 };
            string[] emphas = new string[] { "None", "50/15", "reserved", "CCIT J.17" };
            frame_synchronizer = sw.ReadIntFromBits(11);
            while ((frame_synchronizer != 0x7ff) & (sw.Position < sw.Length))
            {
                if ((frame_synchronizer == 0x202)||(frame_synchronizer == 0x313))//MusicMatch ?
                {
                    sw.Position -= 1;
                    musicMatch = new MusicMatchTag(sw, frame_synchronizer);
                    return;
                }
                sw.BitPosition = sw.Position * 8;
                PositionOfStructureInFile = sw.Position;
                frame_synchronizer = sw.ReadIntFromBits(11);
            }
            version_ID = sw.ReadIntFromBits(2);
            layer = sw.ReadIntFromBits(2);
            cRC_Protection = sw.ReadIntFromBits(1);
            bitrate = sw.ReadIntFromBits(4);
            bitrate_value = bitrates[bitrate];
            frequency = sw.ReadIntFromBits(2);
            frequency_value = frequencies[frequency];
            padding = sw.ReadIntFromBits(1);
            private_bit = sw.ReadIntFromBits(1);
            channel = sw.ReadIntFromBits(2);
            mode_extension = sw.ReadIntFromBits(2);
            copyright = sw.ReadIntFromBits(1);
            original = sw.ReadIntFromBits(1);
            emphasis = sw.ReadIntFromBits(1);
            LengthInFile = 4;
            sw.Position = (int)PositionOfStructureInFile + 4;
        }
       
        public override string ToString()
        {
            return bitrate_value.ToString() + " " + frequency_value.ToString();
        }
        public int FrameSize()
        {
            int Frame_Size = 0;
            switch (layer)
            {
                case 1://Layer II & III 
                case 2:
                    Frame_Size = (int)((144 * bitrate_value * 1000 / frequency_value) + padding);
                    break;
                case 3://Layer I
                    Frame_Size = (12 * bitrate_value / frequency_value + Padding) * 4;
                    break;
                default:
                    return 5;
                    break;
            }
            return Frame_Size;
        }
    }
    public class MP3Frame : LOCALIZED_DATA
    {
        MP3Header mp3header;
        ELEMENTARY_TYPE data;

        public MP3Header Mp3header
        {
            get { return mp3header; }
            set { mp3header = value; }
        }
        public ELEMENTARY_TYPE Data
        {
            get { return data; }
            set { data = value; }
        }
        private static readonly int[,] samplesPerFrame = new int[,] { 
        {   // MPEG Version 1 
            384,    // Layer1 
            1152,   // Layer2 
            1152    // Layer3 
        }, 
        {   // MPEG Version 2 & 2.5 
            384,    // Layer1 
            1152,   // Layer2 
            576     // Layer3 
        } 
        };

        public MP3Frame(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            mp3header = new MP3Header(sw);
            PositionOfStructureInFile = mp3header.PositionOfStructureInFile;
            try
            {
                data = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), mp3header.FrameSize() - 4);
            }
            catch { }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "MP3_Frame";
        }
    }
    public class ID3Tag : LOCALIZED_DATA
    {
        public string tag;
        public string value;
        private ELEMENTARY_TYPE tagName;
        private ELEMENTARY_TYPE tagSizeE;
        // %abc00000 %ijk00000 
        /*a - Tag alter preservation
        b - File alter preservation
        c - Read only
        i - Compression
        j – Encryption
        k - Grouping identity*/
        private ELEMENTARY_TYPE encoder_type;

        private ELEMENTARY_TYPE tagFlags;

        private ELEMENTARY_TYPE tagValue;
        private ATTACHED_PICTURE attached_Picture;
        private ELEMENTARY_TYPE guid;
        private ELEMENTARY_TYPE private_value;
        private ELEMENTARY_TYPE private_Name;


        public ELEMENTARY_TYPE TagName
        {
            get { return tagName; }
            set { tagName = value; }
        }
        public ELEMENTARY_TYPE TagSize
        {
            get { return tagSizeE; }
            set { tagSizeE = value; }
        }
        public ELEMENTARY_TYPE TagFlags
        {
            get { return tagFlags; }
            set { tagFlags = value; }
        }
        public ELEMENTARY_TYPE Encoder_type
        {
            get { return encoder_type; }
            set { encoder_type = value; }
        }
        public ELEMENTARY_TYPE TagValue
        {
            get { return tagValue; }
            set { tagValue = value; }
        }
        public ATTACHED_PICTURE Attached_Picture
        {
            get { return attached_Picture; }
            set { attached_Picture = value; }
        }
        public ELEMENTARY_TYPE Private_Name
        {
            get { return private_Name; }
            set { private_Name = value; }
        }
        public ELEMENTARY_TYPE Private_value
        {
            get { return private_value; }
            set { private_value = value; }
        }
        public ELEMENTARY_TYPE Guid
        {
            get { return guid; }
            set { guid = value; }
        }
        public ID3Tag(string tag, string value)
        {
            this.tag = tag;
            this.value = value;
        }
        public ID3Tag(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            tagName = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            tagSizeE = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            int tagSize = (int)tagSizeE.Value - 1;
            tagFlags = new ELEMENTARY_TYPE(sw, 0, typeof(ushort));
            //chose encoder
            Encoding d = Encoding.GetEncoding("iso-8859-1");
            if ((string)tagName.Value != "PRIV")
            {
                encoder_type = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
                if (((byte)encoder_type.Value) == 1)
                {
                    d = Encoding.Unicode;
                    tagSize /= 2;
                }
            }
            value = "";
            #region Selection
            switch ((string)tagName.Value)
            {
                case "AENC"://[#sec4.20 Audio encryption] 
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "APIC"://[#sec4.15 Attached picture]
                    attached_Picture = new ATTACHED_PICTURE(sw, tagSize);
                    break;
                case "COMM"://[#sec4.11 Comments]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "COMR"://[#sec4.25 Commercial frame]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "ENCR"://[#sec4.26 Encryption method registration]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "EQUA"://[#sec4.13 Equalization]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "ETCO"://[#sec4.6 Event timing codes]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "GEOB"://[#sec4.16 General encapsulated object]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                     break;
                case "GRID"://[#sec4.27 Group identification registration]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                     break;
                case "IPLS"://[#sec4.4 Involved people list]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "LINK"://[#sec4.21 Linked information]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "MCDI"://[#sec4.5 Music CD identifier]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                     break;
                case "MLLT"://[#sec4.7 MPEG location lookup table]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "OWNE"://[#sec4.24 Ownership frame]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                   break;
                case "PRIV"://[#sec4.28 Private frame]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, Encoding.Default);
                    switch ((string)tagValue.Value)
                    {
                        case "WM/MediaClassPrimaryID":
                        case "WM/MediaClassSecondaryID":
                        case "WM/WMContentID":
                        case "WM/WMCollectionID":
                        case "WM/WMCollectionGroupID":
                        case "WM/WMContentID3":
                            guid = new ELEMENTARY_TYPE(sw, 0, typeof(Guid));
                            break;
                        default:
                            private_value = new ELEMENTARY_TYPE(sw, 0, Encoding.Unicode);
                            break;
                    }
                    break;
                case "PCNT"://[#sec4.17 Play counter]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "POPM"://[#sec4.18 Popularimeter]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "POSS"://[#sec4.22 Position synchronisation frame]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "RBUF"://[#sec4.19 Recommended buffer size]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                     break;
                case "RVAD"://[#sec4.12 Relative volume adjustment]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "RVRB"://[#sec4.14 Reverb]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "SYLT"://[#sec4.10 Synchronized lyric/text]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                     break;
                case "SYTC"://[#sec4.8 Synchronized tempo codes]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                     break;
                case "TALB"://[#TALB Album/Movie/Show title]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TBPM"://[#TBPM BPM (beats per minute)]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TCOM"://[#TCOM Composer]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    if ((string)tagValue.Value != "")
                    {
                        string[] names = ((string)tagValue.Value).Replace("/", ";").Split(';');
                        foreach (string n in names)
                        {
                            Composer c = new Composer();
                            c.ComposerName = n.Trim();
                            //          composers.Add(c);
                            //          c.tracks.Add(this);
                        }
                    }
                    break;
                case "TCON"://[#TCON Content type]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TCOP"://[#TCOP Copyright message]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                     break;
                case "TDAT"://[#TDAT Date]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                     break;
                case "TDLY"://[#TDLY Playlist delay]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TENC"://[#TENC Encoded by]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TEXT"://[#TEXT Lyricist/Text writer]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TFLT"://[#TFLT File type]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                     break;
                case "TIME"://[#TIME Time]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TIT1"://[#TIT1 Content group description]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TIT2"://[#TIT2 Title/songname/content description]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TIT3"://[#TIT3 Subtitle/Description refinement]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TKEY"://[#TKEY Initial key]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                   break;
                case "TLAN"://[#TLAN Language(s)]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TLEN"://[#TLEN Length]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    
                    break;
                case "TMED"://[#TMED Media type]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);                 
                    break;
                case "TOAL"://[#TOAL Original album/movie/show title]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                     break;
                case "TOFN"://[#TOFN Original filename]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TOLY"://[#TOLY Original lyricist(s)/text writer(s)]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TOPE"://[#TOPE Original artist(s)/performer(s)]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TORY"://[#TORY Original release year]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                     break;
                case "TOWN"://[#TOWN File owner/licensee]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TPE1"://[#TPE1 Lead performer(s)/Soloist(s)]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TPE2"://[#TPE2 Band/orchestra/accompaniment]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    string[] artistsNames = ((string)tagValue.Value).Split(';');
                    foreach (string sName in artistsNames)
                    {
                        Author c = new Author();
                        c.AuthorName = sName.Trim();
                        //                authors.Add(c);
                    }
                    break;
                case "TPE3"://[#TPE3 Conductor/performer refinement]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TPE4"://[#TPE4 Interpreted, remixed, or otherwise modified by]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                     break;
                case "TPOS"://[#TPOS Part of a set]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TPUB"://[#TPUB Publisher]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TRCK"://[#TRCK Track number/Position in set]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TRDA"://[#TRDA Recording dates]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                     break;
                case "TRSN"://[#TRSN Internet radio station name]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TRSO"://[#TRSO Internet radio station owner]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TSIZ"://[#TSIZ Size]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                     break;
                case "TSRC"://[#TSRC ISRC (international standard recording code)]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TSSE"://[#TSEE Software/Hardware and settings used for encoding]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TYER"://[#TYER Year]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "TXXX"://[#TXXX User defined text information frame]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "UFID"://[#sec4.1 Unique file identifier]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "USER"://[#sec4.23 Terms of use]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "USLT"://[#sec4.9 Unsychronized lyric/text transcription]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "WCOM"://[#WCOM Commercial information]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "WCOP"://[#WCOP Copyright/Legal information]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "WOAF"://[#WOAF Official audio file webpage]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "WOAR"://[#WOAR Official artist/performer webpage]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "WOAS"://[#WOAS Official audio source webpage]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "WORS"://[#WORS Official internet radio station homepage]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "WPAY"://[#WPAY Payment]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "WPUB"://[#WPUB Publishers official webpage]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                    break;
                case "WXXX"://[#WXXX User defined URL link frame]
                    tagValue = new ELEMENTARY_TYPE(sw, 0, d, tagSize);
                     break;
            }
            #endregion
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        private string ReadString(BitStreamReader sw, int fs)
        {
            string valTag = "";
            try
            {
                byte[] s = sw.ReadBytes(1);
                Encoding d = Encoding.GetEncoding("iso-8859-1");
                if (s[0] == 1)
                {
                    d = Encoding.Unicode; fs /= 2;
                }
                fs -= 1;
                valTag = sw.ReadString(fs, d);
                return valTag;
            }
            catch (Exception ex) { }
            return valTag;
        }
        private string ReadBaseString(BitStreamReader sw, int fs)
        {
            string valTag = "";
            try
            {
                byte[] s = new byte[1];
                //                sw.Seek(i, SeekOrigin.Begin);
                Decoder d = Encoding.GetEncoding("iso-8859-1").GetDecoder();
                s = sw.ReadBlock(fs);
                char[] chars = new Char[d.GetCharCount(s, 0, fs)];
                int charLen = d.GetChars(s, 0, s.Length, chars, 0);
                valTag = "";
                for (int j = 0; j < chars.Length; j++)
                {
                    //        if (chars[j].ToString() != "\0")
                    {
                        valTag += chars[j].ToString();
                    }
                }
            }
            catch (Exception ex) { }
            return valTag;
        }
        #region Tags fileName
        //http://www.id3.org/id3v2.3.0
        private Image albumArt;
        private int imageIndex;
        private int imageSize;
        private string artType;
        byte pictureType;
        string mimeType;
        #endregion
        public Guid WMCollectionId;
        public Guid WMMediaClassPrimaryID;
        public Guid WMContentID3;
        public Guid WMCollectionGroupID;
        public string WMUniqueFileIdentifier;
        public string WMProvider;
        private static Guid GetGuid(string s, string sw)
        {
            string st = s.Substring(sw.Length + 1);
            byte[] c = new byte[16];
            for (int k = 0; k < st.Length; k++)
                c[k] = (byte)st[k];
            Guid x = new Guid(c);
            return x;
        }

        public override string ToString()
        {
            return tagName + " " + value;
        }
        public enum Picture_Type
        {
            Other = 0x00, PNG_file_icon_ = 0x01,
            Other_file_icon = 0x02,
            Cover_front = 0x03,
            Cover_back = 0x04,
            Leaflet_page = 0x05,
            Media_side_of_CD = 0x06,
            Lead_artist_lead_performer_soloist = 0x07,
            Artist_performer = 0x08,
            Conductor = 0x09,
            Band_Orchestra = 0x0A,
            composer = 0x0B,
            Lyricist_text_writer = 0x0C,
            Recording_Location = 0x0D,
            During_recording = 0x0E,
            During_performance = 0x0F,
            Movie_video_screen_capture = 0x10,
            A_bright_coloured_fish = 0x11,
            Illustration = 0x12,
            Band_artist_logotype = 0x13,
            Publisher_Studio_logotype = 0x14
        }
    }
    public class ATTACHED_PICTURE : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE text_encoding;
        ELEMENTARY_TYPE mIME_type;
        ELEMENTARY_TYPE picture_type;
        ELEMENTARY_TYPE description;
        ELEMENTARY_TYPE picture_data;
        private Image image;






        public ELEMENTARY_TYPE Text_encoding
        {
            get { return text_encoding; }
            set { text_encoding = value; }
        }
        public ELEMENTARY_TYPE MIME_type
        {
            get { return mIME_type; }
            set { mIME_type = value; }
        }
        public ELEMENTARY_TYPE Picture_type
        {
            get { return picture_type; }
            set { picture_type = value; }
        }
        public ELEMENTARY_TYPE Description
        {
            get { return description; }
            set { description = value; }
        }
        public ELEMENTARY_TYPE Picture_data
        {
            get { return picture_data; }
            set { picture_data = value; }
        }
        public Image Image
        {
            get { return image; }
            set { image = value; }
        }
        public ATTACHED_PICTURE(BitStreamReader sw, int tagsize)
        {
            PositionOfStructureInFile = sw.Position;
      //      text_encoding = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            mIME_type = new ELEMENTARY_TYPE(sw, 0, Encoding.Default);
            picture_type = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            description = new ELEMENTARY_TYPE(sw, 0, Encoding.Default);
            long len = sw.Position - PositionOfStructureInFile;
            picture_data = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), tagsize - (int)len);
            MemoryStream ms = new MemoryStream((byte[])picture_data.Value);
            image = Image.FromStream(ms);
            LengthInFile = sw.Position - PositionOfStructureInFile;

        }
    }

    #endregion
    #region Event Handlers
    public class TagsEventArgs
    {
        public MusicFileClass musicData;
        public TagsEventArgs(MusicFileClass md)
        {
            musicData = md;
        }
    }
    public delegate void TagsEventHandler(TagsEventArgs e);
    public class TagsDirectoryArgs
    {
        public List<MusicFileClass> musicFiles;
        public TreeNode tN;
        public TagsDirectoryArgs(TreeNode tn, List<MusicFileClass> musicFiles)
        {
            tN = tn;
            this.musicFiles = musicFiles;
        }
    }
    public delegate void TagsDirectoryHandler(TagsDirectoryArgs e);
    #endregion
}