using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Xml;
using System.ComponentModel;
using System.Text;
using Utils;

namespace ExifLibrary
{
    //http://owl.phy.queensu.ca/~phil/exiftool/TagNames/JPEG.html
    #region JPEG Exif Data
    public class JPEGSegment : LOCALIZED_DATA
    {
        #region Private members
        private EXIFType ExifType;
        private List<EXIFTag> jFIFTags;
        List<EXIFTag> ifdTags;
        List<EXIFTag> exifTags;
        ELEMENTARY_TYPE dateAcquired;

        List<ELEMENTARY_TYPE> keywords;
        List<XMPPerson> personList;
        List<EXIFTag> gpsTags;
        List<EXIFTag> IFD1;

        private int startTiffHeader;
        private ELEMENTARY_TYPE thumbnail;
        private string manufacturer = "";
        private ELEMENTARY_TYPE marker;
        private ELEMENTARY_TYPE length;
        private byte[] locBuffer;
        private int startIndex;
        ELEMENTARY_TYPE rating;
        ELEMENTARY_TYPE description;
        ELEMENTARY_TYPE exifIdentifierCode;
        ELEMENTARY_TYPE byteOrder;
        ELEMENTARY_TYPE q42;
        ELEMENTARY_TYPE iFD0Offset;
        ELEMENTARY_TYPE nIFDPointer;
        ELEMENTARY_TYPE xmlData;
        QUANTIZATION_TABLE qtData;
        HUFFMANN_TABLE htData;
        private SOFMarker sofMarker;
        private SOSMarkerSegment sosMarker;
        private DRIMarkerSegment driMarker;
        private JFIFMarkerSegment jfifMarker;
        private ICCMarkerSegment icc_Marker;
        private ELEMENTARY_TYPE comment;
        ICC_PROFILE icc_profile;
        private FPXR_Segment fPXRSegment;



        #endregion
        #region Properties
        public ELEMENTARY_TYPE Marker
        {
            get { return marker; }
            set { marker = value; }
        }
        public JPEGMarker JPEGMarker
        {
            get
            {
                return (JPEGMarker)(ushort)marker.Value;
            }
        }
        public ICCMarkerSegment Icc_Marker
        {
            get { return icc_Marker; }
            set { icc_Marker = value; }
        }
        public ELEMENTARY_TYPE Length
        {
            get { return length; }
        }
        public ELEMENTARY_TYPE ExifIdentifierCode
        {
            get { return exifIdentifierCode; }
            set { exifIdentifierCode = value; }
        }
        public ELEMENTARY_TYPE ByteOrder
        {
            get { return byteOrder; }
            set { byteOrder = value; }
        }
        public ELEMENTARY_TYPE Q42
        {
            get { return q42; }
            set { q42 = value; }
        }
        public ELEMENTARY_TYPE IFD0Offset
        {
            get { return iFD0Offset; }
            set { iFD0Offset = value; }
        }
        public ELEMENTARY_TYPE NextIFDPointer
        {
            get { return nIFDPointer; }
            set { nIFDPointer = value; }
        }
        public JFIFMarkerSegment JfifMarker
        {
            get { return jfifMarker; }
            set { jfifMarker = value; }
        }
        public List<EXIFTag> IfdTags
        {
            get { return ifdTags; }
            set { ifdTags = value; }
        }
        public List<EXIFTag> ExifTags
        {
            get { return exifTags; }
            set { exifTags = value; }
        }
        public List<EXIFTag> IFD1_Tags
        {
            get { return IFD1; }
            set { IFD1 = value; }
        }

        public ELEMENTARY_TYPE Thumbnail
        {
            get { return thumbnail; }
            set { thumbnail = value; }
        }
        public List<ELEMENTARY_TYPE> Keywords
        {
            get { return keywords; }
            set { keywords = value; }
        }
        public List<XMPPerson> Person_List
        {
            get { return personList; }
            set { personList = value; }
        }
        public ELEMENTARY_TYPE Rating
        {
            get { return rating; }
            set { rating = value; }
        }
        public ELEMENTARY_TYPE Description
        {
            get { return description; }
            set { description = value; }
        }
        public ELEMENTARY_TYPE Date_Acquired
        {
            get { return dateAcquired; }
            set { dateAcquired = value; }
        }
        public List<EXIFTag> GpsTags
        {
            get { return gpsTags; }
            set { gpsTags = value; }
        }
        public SOSMarkerSegment SosMarker
        {
            get { return sosMarker; }
            set { sosMarker = value; }
        }
        public SOFMarker SofMarker
        {
            get { return sofMarker; }
            set { sofMarker = value; }
        }
        public ELEMENTARY_TYPE XmlData
        {
            get { return xmlData; }
            set { xmlData = value; }
        }
        public ELEMENTARY_TYPE Comment
        {
            get { return comment; }
            set { comment = value; }
        }
        public QUANTIZATION_TABLE Quantization
        {
            get { return qtData; }
            set { qtData = value; }
        }
        public HUFFMANN_TABLE Huffmann
        {
            get { return htData; }
            set { htData = value; }
        }
        public ICC_PROFILE Icc_profile
        {
            get { return icc_profile; }
            set { icc_profile = value; }
        }
        public DRIMarkerSegment DriMarker
        {
            get { return driMarker; }
            set { driMarker = value; }
        }
        public FPXR_Segment FPXRSegment
        {
            get { return fPXRSegment; }
            set { fPXRSegment = value; }
        }
        #endregion
        public JPEGSegment(BitStreamReader sw, int offset)
        {
            PositionOfStructureInFile = sw.Position;
            marker = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));
            if ((ushort)marker.Value == 00) // empty block between two images
                return;
            if ((ushort)marker.Value == 0xffd8)// New image. May be more than 1 in MPF
                return;
            if (((ushort)marker.Value & 0xff00) != 0xff00)                 return;
            startIndex = (int)sw.Position;
            length = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));// sw.ReadShort();
            locBuffer = sw.ReadBytes((ushort)length.Value - 2);//+ 6);

            if ((ushort)marker.Value == 0xffda)
            {
                while (sw.Position < sw.Length)
                {
                    int bs = sw.ReadByte();
                    {
                        if (bs == 0xff)
                        {
                            bs = sw.ReadByte();
                            if (bs == 0xd9)
                            {
                                break;
                            }
                            else
                                bs = sw.ReadByte(); ;
                        }
                    }

                }
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return ((JPEGMarker)(ushort)marker.Value).ToString();
        }
        public void ParseSegment(BitStreamReader sw)
        {
            if (length == null)
                return;
            sw.Position = (int)PositionOfStructureInFile + 2;
            switch (JPEGMarker)
            {
                case JPEGMarker.APP0:
                    #region Rare
                    sw.ReadShort();
                    exifIdentifierCode = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 5);// ReadString(startIndex, 5);
                    switch (((string)exifIdentifierCode.Value).Substring(0, 4))
                    {
                        case "JFIF":
                          #region Cas de fichiers JFIF : peu d'information
    /*                         jFIFTags = new List<EXIFTag>();
                            byte[] high = sw.ReadBytes(2);//ReadByte(startIndex, 2);
                            jFIFTags.Add(new EXIFTag("JFIF Version", high[0].ToString() + "." + high[1].ToString()));
                            BufferConvert.littleEndian = false;
                            byte[] unit = sw.ReadBytes(1);//ReadByte(startIndex, 1);//01 dots per inches
                            jFIFTags.Add(new EXIFTag("Resolution unit", "dots per inches"));
                            int xD = sw.ReadShort() & 0x0000ffff; // Horizontal pixel density
                            jFIFTags.Add(new EXIFTag("Horizontal pixel density", xD.ToString()));
                            int yD = sw.ReadShort() & 0x0000ffff; // Vertical pixel density
                            jFIFTags.Add(new EXIFTag("Vertical pixel density", yD.ToString()));
                            byte[] th = sw.ReadBytes(1); // Thumbnail horizontal pixel intOffset
                            byte[] tv = sw.ReadBytes(1); // Thumbnail vertical pixel intOffset
    */
                            jfifMarker = new JFIFMarkerSegment(sw);
                            break;
                            #endregion
                        case "JFXX":
                            break;
                    }
                    break;
                    #endregion
                case JPEGMarker.APP1:
                    sw.ReadShort();
                    exifIdentifierCode = new ELEMENTARY_TYPE(sw, 0, Encoding.Default);// ReadString(startIndex, 5);
                    switch (((string)exifIdentifierCode.Value))
                    {
                        case "Exif":
                            #region Exif
                            startTiffHeader = sw.Position;
                            int Pad = sw.ReadByte();
                            #region TIFF Marker
                            byteOrder = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 2);// sw.ReadString(2);
                            if ((string)byteOrder.Value == "MM")
                                sw.LittleEndian = true;
                            else
                                sw.LittleEndian = false;
                            q42 = new ELEMENTARY_TYPE(sw, 0, typeof(short));//fixed value 002A
                            iFD0Offset = new ELEMENTARY_TYPE(sw, 0, typeof(int));//normalement 0x8
                            #endregion
                            try
                            {
                                #region 0th IFD
                                ifdTags = GetTags(sw);
                                #endregion
                                nIFDPointer = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                                int nextIFDPointer = (int)nIFDPointer.Value;
                                nextIFDPointer += 4 + (int)iFD0Offset.Value;
                                if (ifdTags.Count > 0)
                                {
                                    #region Exif IFD - ne fonctionne pas pour les fichiers JFIF
                                    int exifIndex = 0;
                                    int gpsIndex = 0;
                                    foreach (EXIFTag ext in ifdTags)
                                    {

                                        switch (ext.tagCode)
                                        {
                                            case EXIFCode.Exif_IFD_Pointer:
                                                exifIndex = (int)ext.TagData.Value;
                                                exifIndex += 2 + (int)iFD0Offset.Value;
                                                break;
                                            case EXIFCode.Info_GPS:
                                                gpsIndex = (int)ext.TagData.Value + 2 + (int)iFD0Offset.Value;
                                                break;
                                        }
                                    }
                                    if (exifIndex != 0)
                                    {
                                        sw.Position = exifIndex + 2;// Why +2
                                        exifTags = GetTags(sw);
                                        if (exifTags.Count > 0)
                                        {
                                            int index = 0;
                                            foreach (EXIFTag ext in exifTags)
                                            {
                                                if (ext.TagName == EXIFCode.ExifInteroperabilityOffset.ToString())
                                                {
                                                    if (int.TryParse(ext.TagValue, out index))
                                                        index += 2 + (int)iFD0Offset.Value;
                                                    break;
                                                }
                                            }
                                            sw.Position = index + 2;
                                            List<EXIFTag> moreTags = GetTags(sw);
                                            foreach (EXIFTag ex in moreTags)
                                                exifTags.Add(ex);
                                        }
                                    }
                                    if (gpsIndex != 0)
                                    {
                                        sw.Position = gpsIndex +2 ;
                                        ExifType = EXIFType.GPS;
                                        gpsTags = GetTags(sw);
                                        ExifType = EXIFType.Exif;
                                    }

                                    #endregion
                                }
                                #region 1st IFD
                                sw.Position = nextIFDPointer;// -2;
                                try
                                {
                                    IFD1 = GetTags(sw);
                                    int ind = 0;
                                    int byteCount = 0;
                                    foreach (EXIFTag ext in IFD1)
                                    {
                                        switch (ext.tagCode)
                                        {
                                            case EXIFCode.Jpeg_IF_Offset:
                                                try
                                                {
                                                    ind = int.Parse(ext.TagValue, System.Globalization.NumberStyles.HexNumber);
                                                }
                                                catch { }
                                                ind += 2 + (int)iFD0Offset.Value;
                                                break;
                                            case EXIFCode.Jpeg_IF_Byte_Count:
                                                try
                                                {
                                                    byteCount = int.Parse(ext.TagValue, System.Globalization.NumberStyles.HexNumber);
                                                }
                                                catch { }
                                                break;
                                        }
                                    }
                                    if (ind != 0)
                                        sw.Position = ind + 2;
                                    thumbnail = new ELEMENTARY_TYPE(sw, 0, typeof(Bitmap), byteCount);
                                }
                                catch { }
                                #endregion
                            }
                            catch (Exception ex) { }
                            break;
                            #endregion
                        case "http://ns.adobe.com/xap/1.0/":
                            #region XMP
                            try
                            {
                                int stIndex = sw.Position;
                                xmlData = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (ushort)length.Value);
                                ParseXMP((string)xmlData.Value, stIndex);
                            }
                            catch (Exception exc) { }
                            break;
                            #endregion
                    }
                    break;
                case JPEGMarker.APP2://FlashPix
                    int blockLength = sw.ReadShort();
                    exifIdentifierCode = new ELEMENTARY_TYPE(sw,0,Encoding.Default);// new ELEMENTARY_TYPE(sw, 0, 4, Encoding.Default);// ReadString(startIndex, 5);
                    switch (((string)exifIdentifierCode.Value))
                    {
                        case "MPF":
                            startTiffHeader = sw.Position;
                            byteOrder = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 2);// sw.ReadString(2);
                            if ((string)byteOrder.Value == "MM")
                                sw.LittleEndian = true;
                            else
                                sw.LittleEndian = false;
                            q42 = new ELEMENTARY_TYPE(sw, 0, typeof(short));//fixed value 002A
                            iFD0Offset = new ELEMENTARY_TYPE(sw, 0, typeof(int));//normalement 0x8
                            ExifType = EXIFType.MPF;
                            ifdTags = GetTags(sw);
                            ExifType = EXIFType.Exif;
                            break;
                        case "ICC_PROFILE":
                            icc_profile = new ICC_PROFILE(sw);
                            break;
                        case "FPXR":
                            fPXRSegment= new FPXR_Segment(sw,(ushort)length.Value);
                            break;
                        default:
                            break;
                    }
                    break;
                case JPEGMarker.APP4://Scalado
                    sw.ReadShort();
                    exifIdentifierCode = new ELEMENTARY_TYPE(sw,0,Encoding.Default);// new ELEMENTARY_TYPE(sw, 0, 4, Encoding.Default);// ReadString(startIndex, 5);
                    switch (((string)exifIdentifierCode.Value))
                    {
                        case "SCALADO":
                            break;
                    }
                    break;
                case JPEGMarker.APP10://Nikon
                    sw.ReadShort();
                    exifIdentifierCode = new ELEMENTARY_TYPE(sw, 0, Encoding.Default);// new ELEMENTARY_TYPE(sw, 0, 4, Encoding.Default);// ReadString(startIndex, 5);
                    switch (((string)exifIdentifierCode.Value))
                    {
                        case "SCALADO":
                            break;
                    }
                    break;
                case ExifLibrary.JPEGMarker.COM:
                    sw.ReadShort();
                    comment = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (ushort)length.Value - 2);
                    break;
                case JPEGMarker.DHT://Define Huffman table
                    htData = new HUFFMANN_TABLE(sw, (ushort)length.Value);
                    break;
                case JPEGMarker.DQT://Define quantization table
                    qtData = new QUANTIZATION_TABLE(sw, (ushort)length.Value);
                    break;
                case JPEGMarker.SOF0://Start of Frame
                    sw.ReadShort();//length;
                    sofMarker = new SOFMarker(sw);
                    break;
                case JPEGMarker.SOS:
                    sw.ReadShort();//length;
                    sosMarker = new SOSMarkerSegment(sw);
                    break;
                case ExifLibrary.JPEGMarker.DRI:
                    sw.ReadShort();//length;
                    driMarker = new DRIMarkerSegment(sw);
                    break;
            }
        }
        private List<EXIFTag> GetTags(BitStreamReader sw)
        {
            List<EXIFTag> tags = new List<EXIFTag>();
            short entries = sw.ReadShort();
            for (int i = 0; i < entries; i++)
            {
                EXIFTag exTag = new EXIFTag(sw, startTiffHeader, manufacturer, ExifType);
                if (exTag.tagCode == EXIFCode.Manufacturer)
                    manufacturer = exTag.TagValue;
                tags.Add(exTag);
            }
            return tags;
        }
        private void ParseXMP(string XMP, int position)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                string beginCapture = "<rdf:RDF";//"<xmp";
                string endCapture = "</rdf:RDF>"; //"</xmp:xmpmeta>"
                int beginPos;
                int endPos;
                beginPos = XMP.IndexOf(beginCapture);
                endPos = XMP.IndexOf(endCapture, 0);
                position += beginPos ; // ????
                XMP = XMP.Substring(beginPos, (endPos - beginPos) + endCapture.Length);

                doc.LoadXml(XMP);

                XmlNamespaceManager NamespaceManager = new XmlNamespaceManager(doc.NameTable);
                NamespaceManager.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
                NamespaceManager.AddNamespace("exif", "http://ns.adobe.com/exif/1.0/");
                NamespaceManager.AddNamespace("x", "adobe:ns:meta/");
                NamespaceManager.AddNamespace("xap", "http://ns.adobe.com/xap/1.0/");
                NamespaceManager.AddNamespace("tiff", "http://ns.adobe.com/tiff/1.0/");
                NamespaceManager.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
                NamespaceManager.AddNamespace("MicrosoftPhoto", "http://ns.microsoft.com/photo/1.0");
                NamespaceManager.AddNamespace("MP", "http://ns.microsoft.com/photo/1.2/");
                NamespaceManager.AddNamespace("MPReg", "http://ns.microsoft.com/photo/1.2/t/Region#");
                NamespaceManager.AddNamespace("MPRI", "http://ns.microsoft.com/photo/1.2/t/RegionInfo#");
                NamespaceManager.AddNamespace("Iptc4xmpExt", "http://iptc.org/std/Iptc4xmpExt/2008-02-29/#");
                // get ratings
                XmlNode xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/xap:Rating", NamespaceManager);
                // Alternatively, there is a common form of RDF shorthand that writes simple properties as
                // attributes of the rdf:description element.
                if (xmlNode == null)
                {
                    xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description", NamespaceManager);
                    xmlNode = xmlNode.Attributes["xap:Rating"];
                }

                if (xmlNode != null)
                {
                    int index = XMP.IndexOf(xmlNode.InnerText);
                    rating = new ELEMENTARY_TYPE(xmlNode.InnerText, position + index);
                }
                xmlNode = doc.SelectSingleNode("/Iptc4xmpExt:CountryName", NamespaceManager);
                // get keywords
                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/dc:subject/rdf:Bag", NamespaceManager);
                if (xmlNode != null)
                {
                    keywords = new List<ELEMENTARY_TYPE>();
                    foreach (XmlNode li in xmlNode)
                    {
                        int index = XMP.IndexOf(li.InnerText);
                        keywords.Add(new ELEMENTARY_TYPE(li.InnerText, position + index));
                    }
                }
                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/Iptc4xmpExt:LocationCreated/Iptc4xmpExt:CountryName", NamespaceManager);
 
                // get description
                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/dc:description/rdf:Alt", NamespaceManager);

                if (xmlNode != null)
                {
                    int index = XMP.IndexOf(xmlNode.ChildNodes[0].InnerText);
                    description = new ELEMENTARY_TYPE(xmlNode.ChildNodes[0].InnerText, position + index);
                }
                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/MicrosoftPhoto:DateAcquired", NamespaceManager);
                if (xmlNode != null)
                {
                    int index = XMP.IndexOf(xmlNode.ChildNodes[0].InnerText);
                    dateAcquired = new ELEMENTARY_TYPE(xmlNode.ChildNodes[0].InnerText, position + index);
                }
                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/MicrosoftPhoto:LastKeywordXMP/rdf:Bag", NamespaceManager);
                if (xmlNode != null)
                {
                    int index = XMP.IndexOf(xmlNode.ChildNodes[0].InnerText);
                    string a = xmlNode.ChildNodes[0].InnerText;
                }
                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/MP:RegionInfo/rdf:Description/MPRI:Regions/rdf:Bag", NamespaceManager);
                if (xmlNode != null)
                {
                    foreach (XmlNode li in xmlNode.ChildNodes)
                    {
                        XmlNode xmlName = doc.SelectSingleNode("/rdf:RDF/rdf:Description/MP:RegionInfo/rdf:Description/MPRI:Regions/rdf:Bag/rdf:li/rdf:Description/MPReg:PersonDisplayName", NamespaceManager);
                        XmlNode xmlRegion = doc.SelectSingleNode("/rdf:RDF/rdf:Description/MP:RegionInfo/rdf:Description/MPRI:Regions/rdf:Bag/rdf:li/rdf:Description/MPReg:Rectangle", NamespaceManager);
                    }
                }
                XmlNodeList persons = doc.SelectNodes("/rdf:RDF/rdf:Description/MP:RegionInfo/rdf:Description/MPRI:Regions/rdf:Bag/rdf:li/rdf:Description", NamespaceManager);
                if (persons.Count > 0)
                {

                    personList = new List<XMPPerson>();
                    foreach (XmlNode person in persons)
                    {
                        XMPPerson p = new XMPPerson();
                        foreach (XmlNode nd in person.ChildNodes)
                        {
                            switch (nd.Name)
                            {
                                case "MPReg:PersonDisplayName":
                                    p.Name = nd.InnerText;
                                    break;
                                case "MPReg:Rectangle":
                                    try
                                    {
                                        string[] recte = nd.InnerText.Split(',');
                                        float x = Convert.ToSingle(recte[0].Replace(".", ","));
                                        float y = Convert.ToSingle(recte[1].Replace(".", ","));
                                        float w = Convert.ToSingle(recte[2].Replace(".", ","));
                                        float h = Convert.ToSingle(recte[3].Replace(".", ","));
                                        p.Rectangle = new RectangleF(x, y, w, h);
                                    }
                                    catch (Exception Exc) { }
                                    break;
                            }
                        }
                        personList.Add(p);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occured while reading meta-data from image. The error was: " + ex.Message);
            }
            finally
            {
                doc = null;
            }
        }
    }
    [Serializable]
    [DefaultPropertyAttribute("Name")]
    public class JPGFileData : LOCALIZED_DATA
    {
        #region Private members
        List<JPEGSegment> segs = new List<JPEGSegment>();
        private string path;
        private long fileSize;
        FileAttributes fileAttributes;
        private DateTime fileDate;
        #endregion
        #region Properties
        public List<JPEGSegment> JpegSegments
        {
            get { return segs; }
            set { segs = value; }
        }
        [CategoryAttribute("Fichier"), DescriptionAttribute("Taille")]
        public long FileSize
        {
            get { return fileSize; }
        }
        [CategoryAttribute("Fichier"), DescriptionAttribute("Date de création")]
        public DateTime FileDate
        {
            get { return fileDate; }
        }
        [CategoryAttribute("Fichier"), DescriptionAttribute("Chemin complet")]
        public string FullPath
        {
            get { return path; }
        }
        [CategoryAttribute("Fichier"), DescriptionAttribute("Nom")]
        public string FileName
        {
            get { return Path.GetFileName(path); }
        }
        [CategoryAttribute("Fichier"), DescriptionAttribute("Chemin")]
        public string FilePath
        {
            get { return Path.GetDirectoryName(path); }
        }
        [CategoryAttribute("Fichier"), DescriptionAttribute("Attributs")]
        public FileAttributes FileAttributes
        {
            get { return fileAttributes; }
        }
        #endregion
        public JPGFileData(string fileName)
        {
            this.path = fileName;
            FileInfo f = new FileInfo(fileName);
            fileDate = f.CreationTime;
            fileSize = f.Length;
            fileAttributes = f.Attributes;
            string ext = Path.GetExtension(fileName).ToLower();
            GetData();
         //   Synthèse();
        }
        #region Parse file
        private void GetData()
        {
    //        BufferConvert.littleEndian = false;
            BitStreamReader sw = new BitStreamReader(path, true);
            byte[] buf = sw.ReadBytes(2000);
            sw.Position = 0;
            #region File Header
            while (sw.Position < sw.Length)
            {
                #region Read JPEG markers // 4 bytes
                JPEGSegment jp = new JPEGSegment(sw, 0);
                if ((int)jp.JPEGMarker != 0x00)
                    segs.Add(jp);
                #endregion
            }
            #endregion
            for (int i = 0; i < segs.Count; i++)
            {
                sw.Position = (int)segs[i].PositionOfStructureInFile + 2;
                segs[i].ParseSegment(sw);
            }
            sw.Close();
        }
        #endregion
/*        private void Synthèse()
        {
            foreach (EXIFTag exf in ifdTags)
            {
                switch (exf.tagCode)
                {
                    case EXIFCode.DateTimeOriginal:
                        dateTaken = Convert.ToDateTime(exf.TagValue);
                        break;
                    case EXIFCode.Manufacturer:
                        make = exf.TagValue;
                        break;
                    case EXIFCode.Model:
                        model = exf.TagValue;
                        break;
                    case EXIFCode.Orientation:
                        orientation = exf.TagValue;
                        break;
                }
            }
            foreach (EXIFTag exf in exifTags)
            {
                switch (exf.tagCode)
                {
                    case EXIFCode.DateTimeOriginal:
                        dateTaken = DateTime.ParseExact(exf.TagValue, "yyyy:MM:dd HH:mm:ss\0", null);
                        break;
                    case EXIFCode.ExifVersion:
                        exifVersion = exf.TagValue;
                        break;
                    case EXIFCode.ExifImageWidth:
                        int.TryParse(exf.TagValue, out imageWidth);
                        break;
                    case EXIFCode.ExifImageHeight:
                        int.TryParse(exf.TagValue, out imageHeight);
                        break;
                    case EXIFCode.Manufacturer:
                        make = exf.TagValue;
                        break;
                    case EXIFCode.Model:
                        model = exf.TagValue;
                        break;
                    case EXIFCode.ExposureTime:
                        exposition = exf.TagValue;
                        break;
                    case EXIFCode.FNumber:
                        focale = exf.TagValue;
                        break;
                    case EXIFCode.Flash:
                        flash = exf.TagValue;
                        break;

                }
            }
        }*/
    }
    #endregion
    public class XMPPerson : LOCALIZED_DATA
    {
        private string name;
        private RectangleF rectangle;
        public RectangleF Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }

}
