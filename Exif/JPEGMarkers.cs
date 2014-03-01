using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;

namespace ExifLibrary
{
    public class JFIFMarkerSegment : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE majorVersion;
        ELEMENTARY_TYPE minorVersion;
        ELEMENTARY_TYPE resUnits;
        ELEMENTARY_TYPE xdensity;
        ELEMENTARY_TYPE ydensity;
        ELEMENTARY_TYPE thumbWidth;
        ELEMENTARY_TYPE thumbHeight;

        public ELEMENTARY_TYPE MajorVersion
        {
            get { return majorVersion; }
            set { majorVersion = value; }
        }
        public ELEMENTARY_TYPE MinorVersion
        {
            get { return minorVersion; }
            set { minorVersion = value; }
        }

        public ELEMENTARY_TYPE ResUnits
        {
            get { return resUnits; }
            set { resUnits = value; }
        }
        public ELEMENTARY_TYPE Xdensity
        {
            get { return xdensity; }
            set { xdensity = value; }
        }
        public ELEMENTARY_TYPE Ydensity
        {
            get { return ydensity; }
            set { ydensity = value; }
        }
        public ELEMENTARY_TYPE ThumbWidth
        {
            get { return thumbWidth; }
            set { thumbWidth = value; }
        }
        public ELEMENTARY_TYPE ThumbHeight
        {
            get { return thumbHeight; }
            set { thumbHeight = value; }
        }
        //  JFIFThumbRGB thumb = null;  // If present
        //  ArrayList extSegments = new ArrayList();
        public JFIFMarkerSegment(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            majorVersion = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            minorVersion = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            resUnits = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            xdensity = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            ydensity = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            thumbWidth = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            thumbHeight = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            /*   if (thumbWidth > 0) {
            thumb = new JFIFThumbRGB(buffer, thumbWidth, thumbHeight);
        }*/
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    #region ICC Marker
    public class ICCMarkerSegment : LOCALIZED_DATA
    {
        //      ArrayList chunks = null;
        byte[] profile = null; // The complete profile when it's fully read
        // May remain null when writing
        private static int ID_SIZE = 12;
        ELEMENTARY_TYPE chunkNum;
        ELEMENTARY_TYPE numChunks;
        List<string> data;


        public ELEMENTARY_TYPE ChunkNum
        {
            get { return chunkNum; }
            set { chunkNum = value; }
        }

        public ELEMENTARY_TYPE NumChunks
        {
            get { return numChunks; }
            set { numChunks = value; }
        }
        public List<string> Data
        {
            get { return data; }
            set { data = value; }
        }
        public ICCMarkerSegment(BitStreamReader sw, int length)
        {
            PositionOfStructureInFile = sw.Position;
            chunkNum = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            // get the total number of chunks
            numChunks = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            data = new List<string>();
            while (sw.Position < PositionOfStructureInFile + length)
            {
                string s = sw.ReadStringZ(Encoding.Default);
                if (s.Length > 1)
                    data.Add(s);
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }

    }
    public class ICC_PROFILE : LOCALIZED_DATA
    {
        ICC_PROFILE_HEADER header;
        ELEMENTARY_TYPE tagCount;
        ELEMENTARY_TYPE tag_Signature;

        ELEMENTARY_TYPE offset_to_beginning;
        ELEMENTARY_TYPE size_of_tag_data_element;
        private List<ICC_TAG> iccTags;

        public ICC_PROFILE_HEADER Header
        {
            get { return header; }
            set { header = value; }
        }
        public ELEMENTARY_TYPE TagCount
        {
            get { return tagCount; }
            set { tagCount = value; }
        }
        public ELEMENTARY_TYPE Tag_Signature
        {
            get { return tag_Signature; }
            set { tag_Signature = value; }
        }
        public ELEMENTARY_TYPE Offset_to_beginning
        {
            get { return offset_to_beginning; }
            set { offset_to_beginning = value; }
        }
        public ELEMENTARY_TYPE Size_of_tag_data_element
        {
            get { return size_of_tag_data_element; }
            set { size_of_tag_data_element = value; }
        }
        public List<ICC_TAG> Icc_Tags
        {
            get { return iccTags; }
            set { iccTags = value; }
        }
        public ICC_PROFILE(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            header = new ICC_PROFILE_HEADER(sw);
            tagCount = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            tag_Signature = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            offset_to_beginning = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            size_of_tag_data_element = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            iccTags = new List<ICC_TAG>();
            for (int i = 0; i < (int)tagCount.Value; i++)
            {
                iccTags.Add(new ICC_TAG(sw, (int)PositionOfStructureInFile + 2));
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class ICC_PROFILE_HEADER : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE profile_size;
        ELEMENTARY_TYPE preferred_cmm_type;
        ELEMENTARY_TYPE profile_version_number;
        ELEMENTARY_TYPE profile_device_class;
        ELEMENTARY_TYPE colour_space_of_data;
        ELEMENTARY_TYPE profile_connection_space;
        ELEMENTARY_TYPE date_and_time;
        ELEMENTARY_TYPE acsp;
        ELEMENTARY_TYPE primary_platform_signature;
        ELEMENTARY_TYPE profile_flags;
        ELEMENTARY_TYPE device_manufacturer;
        ELEMENTARY_TYPE device_model;
        ELEMENTARY_TYPE device_attributes;
        ELEMENTARY_TYPE rendering_intent;
        XYZNumber xyznumber;
        ELEMENTARY_TYPE profile_creator_signature;
        ELEMENTARY_TYPE profile_id;
        ELEMENTARY_TYPE year;
        ELEMENTARY_TYPE month;
        ELEMENTARY_TYPE day;
        ELEMENTARY_TYPE hour;
        ELEMENTARY_TYPE min;
        ELEMENTARY_TYPE second;


        public ELEMENTARY_TYPE Profile_size
        {
            get { return profile_size; }
            set { profile_size = value; }
        }
        public ELEMENTARY_TYPE Preferred_cmm_type
        {
            get { return preferred_cmm_type; }
            set { preferred_cmm_type = value; }
        }
        public ELEMENTARY_TYPE Profile_version_number
        {
            get { return profile_version_number; }
            set { profile_version_number = value; }
        }
        public ELEMENTARY_TYPE Profile_device_class
        {
            get { return profile_device_class; }
            set { profile_device_class = value; }
        }
        public ELEMENTARY_TYPE Colour_space_of_data
        {
            get { return colour_space_of_data; }
            set { colour_space_of_data = value; }
        }
        public ELEMENTARY_TYPE Profile_connection_space
        {
            get { return profile_connection_space; }
            set { profile_connection_space = value; }
        }
        public ELEMENTARY_TYPE Year
        {
            get { return year; }
            set { year = value; }
        }
        public ELEMENTARY_TYPE Month
        {
            get { return month; }
            set { month = value; }
        }
        public ELEMENTARY_TYPE Day
        {
            get { return day; }
            set { day = value; }
        }
        public ELEMENTARY_TYPE Hour
        {
            get { return hour; }
            set { hour = value; }
        }
        public ELEMENTARY_TYPE Min
        {
            get { return min; }
            set { min = value; }
        }
        public ELEMENTARY_TYPE Second
        {
            get { return second; }
            set { second = value; }
        }
        public ELEMENTARY_TYPE Acsp
        {
            get { return acsp; }
            set { acsp = value; }
        }
        public ELEMENTARY_TYPE Primary_platform_signature
        {
            get { return primary_platform_signature; }
            set { primary_platform_signature = value; }
        }
        public ELEMENTARY_TYPE Profile_flags
        {
            get { return profile_flags; }
            set { profile_flags = value; }
        }
        public ELEMENTARY_TYPE Device_manufacturer
        {
            get { return device_manufacturer; }
            set { device_manufacturer = value; }
        }
        public ELEMENTARY_TYPE Device_model
        {
            get { return device_model; }
            set { device_model = value; }
        }
        public ELEMENTARY_TYPE Device_attributes
        {
            get { return device_attributes; }
            set { device_attributes = value; }
        }
        public ELEMENTARY_TYPE Rendering_intent
        {
            get { return rendering_intent; }
            set { rendering_intent = value; }
        }
        public XYZNumber Xyznumber
        {
            get { return xyznumber; }
            set { xyznumber = value; }
        }
        public ELEMENTARY_TYPE Profile_creator_signature
        {
            get { return profile_creator_signature; }
            set { profile_creator_signature = value; }
        }
        public ELEMENTARY_TYPE Profile_id
        {
            get { return profile_id; }
            set { profile_id = value; }
        }
        ELEMENTARY_TYPE reserved;
        public ICC_PROFILE_HEADER(BitStreamReader sw)
        {
            sw.ReadShort();
            PositionOfStructureInFile = sw.Position;
            profile_size = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            preferred_cmm_type = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            profile_version_number = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            profile_device_class = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            colour_space_of_data = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            profile_connection_space = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            year = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            month = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            day = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            hour = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            min = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            second = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            acsp = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            primary_platform_signature = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            profile_flags = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            device_manufacturer = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            device_model = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            device_attributes = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 8);
            rendering_intent = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            xyznumber = new XYZNumber(sw);
            profile_creator_signature = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            profile_id = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 16);
            reserved = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 28);//must be zero
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }

    }
    public class ICC_TAG : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE tag_Signature;
        ELEMENTARY_TYPE tag_Offset;
        ELEMENTARY_TYPE tag_Size;
        ELEMENTARY_TYPE tag_Data;
        XYZType xyz;
        curveType curve;
        s15Fixed16ArrayType numbers;

        public ELEMENTARY_TYPE Tag_Signature
        {
            get { return tag_Signature; }
            set { tag_Signature = value; }
        }
        public ELEMENTARY_TYPE Tag_Offset
        {
            get { return tag_Offset; }
            set { tag_Offset = value; }
        }
        public ELEMENTARY_TYPE Tag_Size
        {
            get { return tag_Size; }
            set { tag_Size = value; }
        }
        public ELEMENTARY_TYPE Tag_Data
        {
            get { return tag_Data; }
            set { tag_Data = value; }
        }
        public s15Fixed16ArrayType Numbers
        {
            get { return numbers; }
            set { numbers = value; }
        }
        public XYZType XYZ
        {
            get { return xyz; }
            set { xyz = value; }
        }
        public curveType Curve
        {
            get { return curve; }
            set { curve = value; }
        }
        public ICC_TAG(BitStreamReader sw, int start)
        {
            PositionOfStructureInFile = sw.Position;
            tag_Signature = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            tag_Offset = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            tag_Size = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            LengthInFile = sw.Position - PositionOfStructureInFile;
            long pos = sw.Position;
            sw.Position = start + (int)Tag_Offset.Value;
            switch ((string)tag_Signature.Value)
            {
                case "chad":
                    numbers = new s15Fixed16ArrayType(sw, (int)tag_Size.Value);
                    break;
                case "rTRC":// curve
                case "bTRC":
                case "kTRC":
                case "gTRC":
                    curve = new curveType(sw, (int)tag_Size.Value);
                    break;
                case "cprt":
                case "desc":
                    string s = sw.ReadStringZ(Encoding.Default);
                    byte a = sw.ReadByte();
                    int l = s.Length;
                    while (a == 0x0)
                    {
                        a = sw.ReadByte();
                        l++;
                    }
                    sw.Position -= 1;
                    tag_Data = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (int)tag_Size.Value - l);
                    break;
                case "wtpt":
                case "bXYZ":
                case "gXYZ":
                    xyz = new XYZType(sw, (int)tag_Size.Value);
                    break;
                case "dscm":
                case "XYZ":
                default:
                    tag_Data = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), (int)tag_Size.Value);
                    break;
            }
            sw.Position = pos;
        }
        public override string ToString()
        {
            string s = (string)tag_Signature.Value;
            if (tag_Data != null)
                s += " : " + tag_Data.ToString();
            return s;
        }
    }
    public class curveType : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE count_value;
        ELEMENTARY_TYPE signature;
        ELEMENTARY_TYPE reserved;
        List<ELEMENTARY_TYPE> numbers;


        public ELEMENTARY_TYPE Signature
        {
            get { return signature; }
            set { signature = value; }
        }
        public ELEMENTARY_TYPE Reserved
        {
            get { return reserved; }
            set { reserved = value; }
        }
        public ELEMENTARY_TYPE Count_value
        {
            get { return count_value; }
            set { count_value = value; }
        }
        public List<ELEMENTARY_TYPE> Numbers
        {
            get { return numbers; }
            set { numbers = value; }
        }
        public curveType(BitStreamReader sw, int length)
        {
            PositionOfStructureInFile = sw.Position;
            signature = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            reserved = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            count_value = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            numbers = new List<ELEMENTARY_TYPE>();
            while (sw.Position < PositionOfStructureInFile + length)
                numbers.Add(new ELEMENTARY_TYPE(sw, 0, typeof(short)));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class XYZType : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE signature;
        ELEMENTARY_TYPE reserved;
        List<XYZNumber> numbers;

        public ELEMENTARY_TYPE Signature
        {
            get { return signature; }
            set { signature = value; }
        }
        public ELEMENTARY_TYPE Reserved
        {
            get { return reserved; }
            set { reserved = value; }
        }
        public List<XYZNumber> Numbers
        {
            get { return numbers; }
            set { numbers = value; }
        }
        public XYZType(BitStreamReader sw, int length)
        {
            PositionOfStructureInFile = sw.Position;
            signature = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            reserved = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            numbers = new List<XYZNumber>();
            while (sw.Position < PositionOfStructureInFile + length)
                numbers.Add(new XYZNumber(sw));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }

    }
    public class XYZNumber : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE x;
        ELEMENTARY_TYPE y;
        ELEMENTARY_TYPE z;

        public ELEMENTARY_TYPE X
        {
            get { return x; }
            set { x = value; }
        }
        public ELEMENTARY_TYPE Y
        {
            get { return y; }
            set { y = value; }
        }
        public ELEMENTARY_TYPE Z
        {
            get { return z; }
            set { z = value; }
        }
        public XYZNumber(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            x = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            y = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            z = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "XYZ Number";
        }
    }
    public class s15Fixed16ArrayType : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE signature;
        ELEMENTARY_TYPE reserved;
        List<ELEMENTARY_TYPE> numbers;

        public List<ELEMENTARY_TYPE> Numbers
        {
            get { return numbers; }
            set { numbers = value; }
        }

        public ELEMENTARY_TYPE Signature
        {
            get { return signature; }
            set { signature = value; }
        }
        public ELEMENTARY_TYPE Reserved
        {
            get { return reserved; }
            set { reserved = value; }
        }
        public s15Fixed16ArrayType(BitStreamReader sw, int length)
        {
            PositionOfStructureInFile = sw.Position;
            signature = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            reserved = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            numbers = new List<ELEMENTARY_TYPE>();
            while (sw.Position < PositionOfStructureInFile + length)
                numbers.Add(new ELEMENTARY_TYPE(sw, 0, typeof(int)));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    #endregion
    [Serializable]
    public class MakerNote : LOCALIZED_DATA
    {
        List<EXIFTag> tags;
        ELEMENTARY_TYPE names;
        ELEMENTARY_TYPE nbTags;
        List<ELEMENTARY_TYPE> otherTags;

        public ELEMENTARY_TYPE Name
        {
            get { return names; }
            set { names = value; }
        }
        public ELEMENTARY_TYPE NbTags
        {
            get { return nbTags; }
            set { nbTags = value; }
        }
        public List<EXIFTag> Tags
        {
            get { return tags; }
            set { tags = value; }
        }
        public List<ELEMENTARY_TYPE> OtherTags
        {
            get { return otherTags; }
            set { otherTags = value; }
        }
        public MakerNote(BitStreamReader sw, string manufacturer, int startTiffHeader)
        {
            PositionOfStructureInFile = sw.Position;
            bool ltl = sw.LittleEndian;
            tags = new List<EXIFTag>();
            int makerNoteStart = (int)sw.Position;
            EXIFType exif = EXIFType.Exif;
            switch (manufacturer)
            {
                case "NIKON CORPORATION\0":
                    exif = EXIFType.Nikon;
                    names = new ELEMENTARY_TYPE(sw, 0, Encoding.Default);
                    sw.ReadBytes(4);
                    string byteOrder = sw.ReadString(2);
                    sw.ReadShort();
                    sw.ReadShort();
                    sw.ReadShort();
                    sw.LittleEndian = false;
                    makerNoteStart += 9;
                    nbTags = new ELEMENTARY_TYPE(sw, 0, typeof(short));// sw.ReadShort();
 
                     break;
                case "SONY\0":
                    exif = EXIFType.Sony;
                    names = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 12);
                    sw.LittleEndian = false;
                    nbTags = new ELEMENTARY_TYPE(sw, 0, typeof(short));// sw.ReadShort();
                    break;
                case "MINOLTA CO.,LTD\0":
                    exif = EXIFType.MinoltaDimage;
                    sw.LittleEndian = false;
                    nbTags = new ELEMENTARY_TYPE(sw, 0, typeof(short));// sw.ReadShort();
                    //    sw.Position += 8;
                    //                 names = new ELEMENTARY_TYPE(sw, 0, 4, Encoding.Default);
                    break;
                case "FUJIFILM\0":
                    exif = EXIFType.FujiFilm;
                    names = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 12);
                    sw.LittleEndian = false;
                    nbTags = new ELEMENTARY_TYPE(sw, 0, typeof(short));// sw.ReadShort();
                    break;
                case "CASIO COMPUTER CO.,LTD.\0":
                    exif = EXIFType.Casio_Type2;
                    names = new ELEMENTARY_TYPE(sw, 0, Encoding.Default);
                    sw.Position += 2;
                    sw.LittleEndian = true;
                    nbTags = new ELEMENTARY_TYPE(sw, 0, typeof(short));// sw.ReadShort();
                    //    sw.Position += (short)nbTags.Value * 0x0C + 0x12;
                    break;
                case "OLYMPUS OPTICAL CO.,LTD\0":
                    exif = EXIFType.Olympus;
                    names = new ELEMENTARY_TYPE(sw, 0, Encoding.Default);
                    sw.Position += 2;
                    sw.LittleEndian = false;
                    nbTags = new ELEMENTARY_TYPE(sw, 0, typeof(short));// sw.ReadShort();
                    //    sw.Position += (short)nbTags.Value * 0x0C + 0x12;
                    break;
                case "Eastman Kodak Company\0":
                    exif = EXIFType.Kodak;
                    sw.Position += 8;
                    otherTags = new List<ELEMENTARY_TYPE>();
                    names = new ELEMENTARY_TYPE(sw, 0, Encoding.Default);
                    sw.Position = makerNoteStart + 40;
                    ELEMENTARY_TYPE camera = new ELEMENTARY_TYPE(sw, 0, Encoding.Default);// sw.ReadShort();
                    otherTags.Add(camera);
                    sw.Position = makerNoteStart + 108;
                    ELEMENTARY_TYPE imageWidth = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    otherTags.Add(imageWidth);
                    ELEMENTARY_TYPE imageHeight = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    otherTags.Add(imageHeight);
                    break;
            }
            if (exif != EXIFType.Kodak)
            {
                for (int i = 0; i < (short)nbTags.Value; i++)
                {
                    EXIFTag exTag = new EXIFTag(sw, makerNoteStart, manufacturer, exif);
                    tags.Add(exTag);
                }
            }
            sw.LittleEndian = ltl;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "";
        }
    }
    public class MPFIMAGE : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE flags;
        ELEMENTARY_TYPE mPImageStart;
        ELEMENTARY_TYPE mPImageLength;
        ELEMENTARY_TYPE dependentImage2EntryNumber;
        ELEMENTARY_TYPE dependentImage1EntryNumber;
        int absoluteOffset;

        public ELEMENTARY_TYPE Flags
        {
            get { return flags; }
            set { flags = value; }
        }
        public ELEMENTARY_TYPE MPImageLength
        {
            get { return mPImageLength; }
            set { mPImageLength = value; }
        }
        public ELEMENTARY_TYPE MPImageStart
        {
            get { return mPImageStart; }
            set { mPImageStart = value; }
        }
        public ELEMENTARY_TYPE DependentImage1EntryNumber
        {
            get { return dependentImage1EntryNumber; }
            set { dependentImage1EntryNumber = value; }
        }
        public ELEMENTARY_TYPE DependentImage2EntryNumber
        {
            get { return dependentImage2EntryNumber; }
            set { dependentImage2EntryNumber = value; }
        }
        public MPImageType MP_Image_Type { get { return (MPImageType)((uint)flags.Value & 0x00ffffff); } }
        public int AbsoluteOffset
        {
            get { return absoluteOffset; }
            set { absoluteOffset = value; }
        }
        public bool Is_Dependent_Parent_Image
        { get { return ((uint)flags.Value & 0x80000000) == 0x80000000; } }
        public bool Is_Dependent_child_image
        { get { return ((uint)flags.Value & 0x40000000) == 0x40000000; } }
        public bool Is_Representative_image
        { get { return ((uint)flags.Value & 0x20000000) == 0x20000000; } }
        public MPFIMAGE(BitStreamReader sw, int startIndex)
        {
            PositionOfStructureInFile = sw.Position;
            flags = new ELEMENTARY_TYPE(sw, 0, typeof(uint));
            //         if (((uint)flags.Value & 0x070000000) == 00)

            mPImageLength = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            mPImageStart = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            absoluteOffset = (int)mPImageStart.Value + startIndex;
            dependentImage1EntryNumber = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            dependentImage2EntryNumber = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "MP Image Data";
        }
    }
    #region Quantization
    public class QUANTIZATION_TABLE : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE qtlength;
        ELEMENTARY_TYPE qtInfo;
        List<Qtable> qTables;
        public ELEMENTARY_TYPE Qtlength
        {
            get { return qtlength; }
            set { qtlength = value; }
        }
        public ELEMENTARY_TYPE QtInfo
        {
            get { return qtInfo; }
            set { qtInfo = value; }
        }
        public List<Qtable> QTables
        {
            get { return qTables; }
            set { qTables = value; }
        }
        public QUANTIZATION_TABLE(BitStreamReader sw, int length)
        {
            PositionOfStructureInFile = sw.Position;
            qtlength = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            //        qtInfo = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            //            qtTable = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), length - 3);
            qTables = new List<Qtable>();
            while (sw.Position < PositionOfStructureInFile + length)
                //            for (int i = 0; i < (byte)qtInfo.Value; i++)
                qTables.Add(new Qtable(sw));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class Qtable : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE element;
        int QTABLE_SIZE = 64;
        ELEMENTARY_TYPE data; // 64 elements, in natural order

        public int ElementPrecision
        { get { return (byte)(((byte)element.Value & 0xf0) >> 4); } }
        public int TableId
        { get { return (byte)(((byte)element.Value & 0x0f)); } }
        public ELEMENTARY_TYPE Data
        {
            get { return data; }
            set { data = value; }
        }
        public Qtable(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            element = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            data = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 0x40);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "Quantization table";
        }
    }
    #endregion
    public class HUFFMANN_TABLE : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE qtlength;
        ELEMENTARY_TYPE qtInfo;
        List<ELEMENTARY_TYPE> codeNb;
        List<List<ELEMENTARY_TYPE>> codes;

        public ELEMENTARY_TYPE Qtlength
        {
            get { return qtlength; }
            set { qtlength = value; }
        }
        public ELEMENTARY_TYPE QtID
        {
            get { return qtInfo; }
            set { qtInfo = value; }
        }
        public List<List<ELEMENTARY_TYPE>> Codes
        {
            get { return codes; }
            set { codes = value; }
        }
        public HUFFMANN_TABLE(BitStreamReader sw, int length)
        {
            PositionOfStructureInFile = sw.Position;
            sw.ReadShort();//length
            //           qtlength = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            qtInfo = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            codeNb = new List<ELEMENTARY_TYPE>();
            int nb = 0;
            for (int i = 0; i < 0x10; i++)
            {
                ELEMENTARY_TYPE s = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
                codeNb.Add(s);
                nb += (byte)s.Value;
            }
            codes = new List<List<ELEMENTARY_TYPE>>();
            foreach (ELEMENTARY_TYPE el in codeNb)
            {
                List<ELEMENTARY_TYPE> dt = new List<ELEMENTARY_TYPE>();
                codes.Add(dt);
                for (int i = 0; i < (byte)el.Value; i++)
                {
                    dt.Add(new ELEMENTARY_TYPE(sw, 0, typeof(byte)));
                }
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    #region Start of scan
    public class SOSMarkerSegment : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE numComponents;
        ELEMENTARY_TYPE startSpectralSelection;
        ELEMENTARY_TYPE endSpectralSelection;
        ELEMENTARY_TYPE approx;
        ScanComponentSpec[] componentSpecs;

        public ELEMENTARY_TYPE NumComponents
        {
            get { return numComponents; }
            set { numComponents = value; }
        }
        public ScanComponentSpec[] ComponentSpecs
        {
            get { return componentSpecs; }
            set { componentSpecs = value; }
        }
        public ELEMENTARY_TYPE StartSpectralSelection
        {
            get { return startSpectralSelection; }
            set { startSpectralSelection = value; }
        }
        public ELEMENTARY_TYPE EndSpectralSelection
        {
            get { return endSpectralSelection; }
            set { endSpectralSelection = value; }
        }
        public int approxHigh
        { get { return (byte)(((byte)approx.Value & 0xf0) >> 4); } }
        public int approxLow
        { get { return (byte)(((byte)approx.Value & 0x0f)); } }
        public SOSMarkerSegment(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            numComponents = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            componentSpecs = new ScanComponentSpec[(byte)numComponents.Value];
            for (int i = 0; i < (byte)numComponents.Value; i++)
            {
                componentSpecs[i] = new ScanComponentSpec(sw);
            }
            startSpectralSelection = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            endSpectralSelection = new ELEMENTARY_TYPE(sw, 0, typeof(byte)); ;
            approx = new ELEMENTARY_TYPE(sw, 0, typeof(byte)); ;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class ScanComponentSpec : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE componentSelector;
        ELEMENTARY_TYPE huffman;

        public ELEMENTARY_TYPE ComponentSelector
        {
            get { return componentSelector; }
            set { componentSelector = value; }
        }
        public ELEMENTARY_TYPE Huffman
        {
            get { return huffman; }
            set { huffman = value; }
        }
        public int dcHuffTable
        { get { return (byte)(((byte)huffman.Value & 0xF0) >> 4); } }
        public int acHuffTable
        { get { return (byte)(((byte)huffman.Value & 0x0f)); } }

        public ScanComponentSpec(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            componentSelector = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            huffman = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "Scan component";
        }
    }
    #endregion
    #region Sart of file
    public class SOFMarker : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE samplePrecision;
        ELEMENTARY_TYPE numLines;
        ELEMENTARY_TYPE samplesPerLine;
        ELEMENTARY_TYPE numComponents;
        List<ComponentSpec> components;
        public ELEMENTARY_TYPE SamplePrecision
        {
            get { return samplePrecision; }
            set { samplePrecision = value; }
        }
        public ELEMENTARY_TYPE NumLines
        {
            get { return numLines; }
            set { numLines = value; }
        }
        public ELEMENTARY_TYPE SamplesPerLine
        {
            get { return samplesPerLine; }
            set { samplesPerLine = value; }
        }
        public ELEMENTARY_TYPE NumComponents
        {
            get { return numComponents; }
            set { numComponents = value; }
        }
        public List<ComponentSpec> Components
        {
            get { return components; }
            set { components = value; }
        }
        public SOFMarker(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            samplePrecision = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            numLines = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            samplesPerLine = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            numComponents = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            components = new List<ComponentSpec>();
            for (int i = 0; i < (byte)numComponents.Value; i++)
            {
                components.Add(new ComponentSpec(sw));
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class ComponentSpec : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE componentId;
        ELEMENTARY_TYPE samplingFactor;
        ELEMENTARY_TYPE qtableSelector;
        public ELEMENTARY_TYPE ComponentId
        {
            get { return componentId; }
            set { componentId = value; }
        }
        public ELEMENTARY_TYPE SamplingFactor
        {
            get { return samplingFactor; }
            set { samplingFactor = value; }
        }
        public ELEMENTARY_TYPE QtableSelector
        {
            get { return qtableSelector; }
            set { qtableSelector = value; }
        }
        public byte HsamplingFactor
        {
            get { return (byte)(((byte)samplingFactor.Value & 0xF0) >> 4); }
        }
        public byte VsamplingFactor
        {
            get { return (byte)((byte)samplingFactor.Value & 0xF); }
        }
        public ComponentSpec(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            componentId = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            samplingFactor = new ELEMENTARY_TYPE(sw, 0, typeof(byte));//>>4
            qtableSelector = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "Component specification";
        }
    }
    #endregion
    public class DRIMarkerSegment : LOCALIZED_DATA
    {
        /**
         * Restart interval, or 0 if none is specified.
         */
        ELEMENTARY_TYPE restartInterval;

        public ELEMENTARY_TYPE RestartInterval
        {
            get { return restartInterval; }
            set { restartInterval = value; }
        }
        public DRIMarkerSegment(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            restartInterval = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    #region FPX
    public class FPXR_Stream : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE index_to_content_list;
        ELEMENTARY_TYPE offset_to_stream;
        ELEMENTARY_TYPE sizeOfSection;
        ELEMENTARY_TYPE numberOfItems;
        List<FPXR_Property> properties;
        FPXR_PROPERTYSET_HEADER propertyset_header;
        ELEMENTARY_TYPE fmtId;
        ELEMENTARY_TYPE offset_to_properties;


        public ELEMENTARY_TYPE Index_to_content_list
        {
            get { return index_to_content_list; }
            set { index_to_content_list = value; }
        }
        public ELEMENTARY_TYPE Offset_to_stream
        {
            get { return offset_to_stream; }
            set { offset_to_stream = value; }
        }
        public FPXR_PROPERTYSET_HEADER Propertyset_header
        {
            get { return propertyset_header; }
            set { propertyset_header = value; }
        }
        public ELEMENTARY_TYPE FmtId
        {
            get { return fmtId; }
            set { fmtId = value; }
        }
        public ELEMENTARY_TYPE Offset_to_properties
        {
            get { return offset_to_properties; }
            set { offset_to_properties = value; }
        }

        public ELEMENTARY_TYPE SizeOfSection
        {
            get { return sizeOfSection; }
            set { sizeOfSection = value; }
        }
        public ELEMENTARY_TYPE NumberOfItems
        {
            get { return numberOfItems; }
            set { numberOfItems = value; }
        }
        public List<FPXR_Property> Properties
        {
            get { return properties; }
            set { properties = value; }
        }
        public FPXR_Stream(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            index_to_content_list = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            offset_to_stream = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            propertyset_header = new FPXR_PROPERTYSET_HEADER(sw);
            fmtId = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 16);
            offset_to_properties = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            int startOfSection = (int)sw.Position;
            sizeOfSection = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            numberOfItems = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            properties = new List<FPXR_Property>();
            for (int i = 0; i < (int)numberOfItems.Value; i++)
            {
                properties.Add(new FPXR_Property(sw, startOfSection));
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class FPXR_Property : LOCALIZED_DATA
    {

        ELEMENTARY_TYPE propertyId;
        ELEMENTARY_TYPE offset;
        ELEMENTARY_TYPE propertyType;
        ELEMENTARY_TYPE value;
        List<string> strings;


        public ELEMENTARY_TYPE PropertyId
        {
            get { return propertyId; }
            set { propertyId = value; }
        }
        public ELEMENTARY_TYPE Offset
        {
            get { return offset; }
            set { offset = value; }
        }
        public ELEMENTARY_TYPE PropertyType
        {
            get { return propertyType; }
            set { propertyType = value; }
        }
        public ELEMENTARY_TYPE Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public List<string> Strings
        {
            get { return strings; }
            set { strings = value; }
        }
        public FlashPixObjects Property_Id
        { get { return (FlashPixObjects)(int)propertyId.Value; } }
        public string Property_Datatype
        { get { return ((OLE_Types)(int)propertyType.Value).ToString(); } }
        public FPXR_Property(BitStreamReader sw, int position)
        {
            PositionOfStructureInFile = sw.Position;
            propertyId = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            offset = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            LengthInFile = sw.Position - PositionOfStructureInFile;
            int pos = (int)sw.Position;
            sw.Position = position + (int)offset.Value;
            propertyType = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            bool isArray = false;
            if (((int)propertyType.Value & 0x1000) == 0x1000)
            {
                isArray = true;
                propertyType.Value = (int)propertyType.Value & 0x0fff;
            }
            switch ((OLE_Types)(int)propertyType.Value)
            {
                case OLE_Types.VT_UI2:
                    value = new ELEMENTARY_TYPE(sw, 0, typeof(ushort));
                    break;
                case OLE_Types.VT_I2:
                    value = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                    break;
                case OLE_Types.VT_UI4:
                case OLE_Types.VT_R4:
                case OLE_Types.VT_I4:
                    value = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    break;
                case OLE_Types.VT_LPWSTR:
                    if (isArray)
                    {
                        int nb = sw.ReadInteger();
                        strings = new List<string>();
                        for (int i = 0; i < nb; i++)
                        {
                            int size = sw.ReadInteger();
                            strings.Add(sw.ReadString(size, Encoding.Unicode));
                        }
                    }
                    else
                    {
                        int size = sw.ReadInteger();
                        value = new ELEMENTARY_TYPE(sw, 0, Encoding.Unicode, size);
                    }
                    break;
                case OLE_Types.VT_CLSID:
                    value = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 16);
                    break;
                case OLE_Types.VT_BLOB:
                    int sizeB = sw.ReadInteger();
                    value = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), sizeB);
                    break;
            }
            sw.Position = pos;
        }
        public override string ToString()
        {
            string s = ((FlashPixObjects)(int)propertyId.Value).ToString();
            switch ((FlashPixObjects)(int)propertyId.Value)
            {
                case FlashPixObjects.Code_Page:
                    s += (CodePage)(short)value.Value;
                    break;
            }
            return s;
        }
    }
    public class FPXR_PROPERTYSET_HEADER : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE wByteOrder;// Always 0xFFFE
        ELEMENTARY_TYPE wFormat;// Should be 0
        ELEMENTARY_TYPE dwOSVer;// System version
        ELEMENTARY_TYPE clsid; // Application CLSID
        ELEMENTARY_TYPE reserved;// Should be 1

        public ELEMENTARY_TYPE ByteOrder
        {
            get { return wByteOrder; }
            set { wByteOrder = value; }
        }
        public ELEMENTARY_TYPE Format
        {
            get { return wFormat; }
            set { wFormat = value; }
        }
        public ELEMENTARY_TYPE OSVer
        {
            get { return dwOSVer; }
            set { dwOSVer = value; }
        }
        public ELEMENTARY_TYPE Clsid
        {
            get { return clsid; }
            set { clsid = value; }
        }
        public ELEMENTARY_TYPE Reserved
        {
            get { return reserved; }
            set { reserved = value; }
        }
        public FPXR_PROPERTYSET_HEADER(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            wByteOrder = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            wFormat = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            dwOSVer = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            clsid = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 16);
            reserved = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            LengthInFile = sw.Position - PositionOfStructureInFile;

        }
    }
    public class FPXR_Content : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE entitySize;
        ELEMENTARY_TYPE defaultValue;
        ELEMENTARY_TYPE entityName;
        ELEMENTARY_TYPE entityClassId;
        public ELEMENTARY_TYPE EntitySize
        {
            get { return entitySize; }
            set { entitySize = value; }
        }
        public ELEMENTARY_TYPE DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }
        public ELEMENTARY_TYPE EntityName
        {
            get { return entityName; }
            set { entityName = value; }
        }
        public ELEMENTARY_TYPE EntityClassId
        {
            get { return entityClassId; }
            set { entityClassId = value; }
        }
        public FPXR_Content(BitStreamReader sw)
        {
            //http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/FlashPix.html
            PositionOfStructureInFile = sw.Position;
            entitySize = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            byte[] b = BitConverter.GetBytes((int)entitySize.Value);
            int count = ((b[0] * 256 + b[1]) * 256) + b[2] * 256 + b[3];
            entitySize.Value = count;
            defaultValue = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            entityName = new ELEMENTARY_TYPE(sw, 0, Encoding.Unicode);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return (string)entityName.Value;
        }

    }
    public class FPXR_Segment : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE version;
        ELEMENTARY_TYPE type;
        List<FPXR_Content> contents;
        ELEMENTARY_TYPE interoperability;
        private FPXR_Stream fPXR_Stream;


        public ELEMENTARY_TYPE Version
        {
            get { return version; }
            set { version = value; }
        }
        public ELEMENTARY_TYPE Type
        {
            get { return type; }
            set { type = value; }
        }
        public List<FPXR_Content> Contents
        {
            get { return contents; }
            set { contents = value; }
        }
        public ELEMENTARY_TYPE Interoperability
        {
            get { return interoperability; }
            set { interoperability = value; }
        }
        public FPXR_Stream FPXR_Stream
        {
            get { return fPXR_Stream; }
            set { fPXR_Stream = value; }
        }
        public FPXR_Segment(BitStreamReader sw, int length)
        {
            //http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/FlashPix.html
            PositionOfStructureInFile = sw.Position;
            version = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            type = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            switch ((byte)type.Value)
            {
                case 0: // standard image stringData
                    break;
                case 1://Content list
                    interoperability = new ELEMENTARY_TYPE(sw, 0, typeof(short));//number of entries ?
                    byte[] b = BitConverter.GetBytes((short)interoperability.Value);
                    int count = b[0] * 256 + b[1];
                    contents = new List<FPXR_Content>();
                    for (int i = 0; i < count; i++)
                    {
                        contents.Add(new FPXR_Content(sw));
                    }
                    break;
                case 2://Stream stringData
                    FPXR_Stream = new FPXR_Stream(sw);
                    break;
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;

        }
    }
    #endregion
}
