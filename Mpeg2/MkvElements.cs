using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
namespace VideoFiles
{
    public class Element
    {
        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        EbmlElementType elementType;
        public EbmlElementType ElementType
        {
            get { return elementType; }
            set { elementType = value; }
        }
        public Element(string n, int i, string t)
        {
            name = n;
            id = i;
            switch(t)
            {
                case "m":
                    elementType = EbmlElementType.MASTER;
                    break;
                case "u":
                    elementType = EbmlElementType.UNSIGNED;
                    break;
                case "i":
                    elementType = EbmlElementType.SIGNED;
                    break;
                case "s":
                    elementType = EbmlElementType.TEXTA;
                    break;
                case "8":
                    elementType = EbmlElementType.TEXTU;
                    break;
                case "b":
                    elementType = EbmlElementType.BINARY;
                    break;
                case "d":
                    elementType = EbmlElementType.DATE;
                    break;
                case "f":
                    elementType = EbmlElementType.FLOAT;
                    break;
                default:
                    break;
            }
   //         if (name == "Segment")                elementType = EbmlElementType.JUST_GO_ON;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class MkvELements
    {
        List<Element> list = new List<Element>();
        public List<Element> List
        {
            get { return list; }
            set { list = value; }
        }
        public MkvELements()
        {
            list.Add(new Element("EBML", 0x1A45DFA3, "m"));
            list.Add(new Element("EBMLVersion", 0x4286, "u"));
            list.Add(new Element("EBMLReadVersion", 0x42F7, "u"));
            list.Add(new Element("EBMLMaxIDLength", 0x42F2, "u"));
            list.Add(new Element("EBMLMaxSizeLength", 0x42F3, "u"));
            list.Add(new Element("DocType", 0x4282, "s"));
            list.Add(new Element("DocTypeVersion", 0x4287, "u"));
            list.Add(new Element("DocTypeReadVersion", 0x4285, "u"));
            list.Add(new Element("Void", 0xEC, "b"));
            list.Add(new Element("CRC-32", 0xBF, "b"));
            list.Add(new Element("SignatureSlot", 0x1B538667, "m"));
            list.Add(new Element("SignatureAlgo", 0x7E8A, "u"));
            list.Add(new Element("SignatureHash", 0x7E9A, "u"));
            list.Add(new Element("SignaturePublicKey", 0x7EA5, "b"));
            list.Add(new Element("Signature", 0x7EB5, "b"));
            list.Add(new Element("SignatureElements", 0x7E5B, "m"));
            list.Add(new Element("SignatureElementList", 0x7E7B, "m"));
            list.Add(new Element("SignedElement", 0x6532, "b"));
            list.Add(new Element("Segment", 0x18538067, "m"));
            list.Add(new Element("SeekHead", 0x114D9B74, "m"));
            list.Add(new Element("Seek", 0x4DBB, "m"));
            list.Add(new Element("SeekID", 0x53AB, "b"));
            list.Add(new Element("SeekPosition", 0x53AC, "u"));
            list.Add(new Element("Info", 0x1549A966, "m"));
            list.Add(new Element("SegmentUID", 0x73A4, "b"));
            list.Add(new Element("SegmentFilename", 0x7384, "8"));
            list.Add(new Element("PrevUID", 0x3CB923, "b"));
            list.Add(new Element("PrevFilename", 0x3C83AB, "8"));
            list.Add(new Element("NextUID", 0x3EB923, "b"));
            list.Add(new Element("NextFilename", 0x3E83BB, "8"));
            list.Add(new Element("SegmentFamily", 0x4444, "b"));
            list.Add(new Element("ChapterTranslate", 0x6924, "m"));
            list.Add(new Element("ChapterTranslateEditionUID", 0x69FC, "u"));
            list.Add(new Element("ChapterTranslateCodec", 0x69BF, "u"));
            list.Add(new Element("ChapterTranslateID", 0x69A5, "b"));
            list.Add(new Element("TimecodeScale", 0x2AD7B1, "u"));
            list.Add(new Element("Duration", 0x4489, "f"));
            list.Add(new Element("DateUTC", 0x4461, "d"));
            list.Add(new Element("Title", 0x7BA9, "8"));
            list.Add(new Element("MuxingApp", 0x4D80, "8"));
            list.Add(new Element("WritingApp", 0x5741, "8"));
            list.Add(new Element("Cluster", 0x1F43B675, "m"));
            list.Add(new Element("Timecode", 0xE7, "u"));
            list.Add(new Element("SilentTracks", 0x5854, "m"));
            list.Add(new Element("SilentTrackNumber", 0x58D7, "u"));
            list.Add(new Element("Position", 0xA7, "u"));
            list.Add(new Element("PrevSize", 0xAB, "u"));
            list.Add(new Element("SimpleBlock", 0xA3, "b"));
            list.Add(new Element("BlockGroup", 0xA0, "m"));
            list.Add(new Element("Block", 0xA1, "b"));
            list.Add(new Element("BlockVirtual", 0xA2, "b"));
            list.Add(new Element("BlockAdditions", 0x75A1, "m"));
            list.Add(new Element("BlockMore", 0xA6, "m"));
            list.Add(new Element("BlockAddID", 0xEE, "u"));
            list.Add(new Element("BlockAdditional", 0xA5, "b"));
            list.Add(new Element("BlockDuration", 0x9B, "u"));
            list.Add(new Element("ReferencePriority", 0xFA, "u"));
            list.Add(new Element("ReferenceBlock", 0xFB, "i"));
            list.Add(new Element("ReferenceVirtual", 0xFD, "i"));
            list.Add(new Element("CodecState", 0xA4, "b"));
            list.Add(new Element("DiscardPadding", 0x75A2, "i"));
            list.Add(new Element("Slices", 0x8E, "m"));
            list.Add(new Element("TimeSlice", 0xE8, "m"));
            list.Add(new Element("LaceNumber", 0xCC, "u"));
            list.Add(new Element("FrameNumber", 0xCD, "u"));
            list.Add(new Element("BlockAdditionID", 0xCB, "u"));
            list.Add(new Element("Delay", 0xCE, "u"));
            list.Add(new Element("SliceDuration", 0xCF, "u"));
            list.Add(new Element("ReferenceFrame", 0xC8, "m"));
            list.Add(new Element("ReferenceOffset", 0xC9, "u"));
            list.Add(new Element("ReferenceTimeCode", 0xCA, "u"));
            list.Add(new Element("EncryptedBlock", 0xAF, "b"));
            list.Add(new Element("Tracks", 0x1654AE6B, "m"));
            list.Add(new Element("TrackEntry", 0xAE, "m"));
            list.Add(new Element("TrackNumber", 0xD7, "u"));
            list.Add(new Element("TrackUID", 0x73C5, "u"));
            list.Add(new Element("TrackType", 0x83, "u"));
            list.Add(new Element("FlagEnabled", 0xB9, "u"));
            list.Add(new Element("FlagDefault", 0x88, "u"));
            list.Add(new Element("FlagForced", 0x55AA, "u"));
            list.Add(new Element("FlagLacing", 0x9C, "u"));
            list.Add(new Element("MinCache", 0x6DE7, "u"));
            list.Add(new Element("MaxCache", 0x6DF8, "u"));
            list.Add(new Element("DefaultDuration", 0x23E383, "u"));
            list.Add(new Element("DefaultDecodedFieldDuration", 0x234E7A, "u"));
            list.Add(new Element("TrackTimecodeScale", 0x23314F, "f"));
            list.Add(new Element("TrackOffset", 0x537F, "i"));
            list.Add(new Element("MaxBlockAdditionID", 0x55EE, "u"));
            list.Add(new Element("Name", 0x536E, "8"));
            list.Add(new Element("Language", 0x22B59C, "s"));
            list.Add(new Element("CodecID", 0x86, "s"));
            list.Add(new Element("CodecPrivate", 0x63A2, "b"));
            list.Add(new Element("CodecName", 0x258688, "8"));
            list.Add(new Element("AttachmentLink", 0x7446, "u"));
            list.Add(new Element("CodecSettings", 0x3A9697, "8"));
            list.Add(new Element("CodecInfoURL", 0x3B4040, "s"));
            list.Add(new Element("CodecDownloadURL", 0x26B240, "s"));
            list.Add(new Element("CodecDecodeAll", 0xAA, "u"));
            list.Add(new Element("TrackOverlay", 0x6FAB, "u"));
            list.Add(new Element("CodecDelay", 0x56AA, "u"));
            list.Add(new Element("SeekPreRoll", 0x56BB, "u"));
            list.Add(new Element("TrackTranslate", 0x6624, "m"));
            list.Add(new Element("TrackTranslateEditionUID", 0x66FC, "u"));
            list.Add(new Element("TrackTranslateCodec", 0x66BF, "u"));
            list.Add(new Element("TrackTranslateTrackID", 0x66A5, "b"));
            list.Add(new Element("Video", 0xE0, "m"));
            list.Add(new Element("FlagInterlaced", 0x9A, "u"));
            list.Add(new Element("StereoMode", 0x53B8, "u"));
            list.Add(new Element("AlphaMode", 0x53C0, "u"));
            list.Add(new Element("OldStereoMode", 0x53B9, "u"));
            list.Add(new Element("PixelWidth", 0xB0, "u"));
            list.Add(new Element("PixelHeight", 0xBA, "u"));
            list.Add(new Element("PixelCropBottom", 0x54AA, "u"));
            list.Add(new Element("PixelCropTop", 0x54BB, "u"));
            list.Add(new Element("PixelCropLeft", 0x54CC, "u"));
            list.Add(new Element("PixelCropRight", 0x54DD, "u"));
            list.Add(new Element("DisplayWidth", 0x54B0, "u"));
            list.Add(new Element("DisplayHeight", 0x54BA, "u"));
            list.Add(new Element("DisplayUnit", 0x54B2, "u"));
            list.Add(new Element("AspectRatioType", 0x54B3, "u"));
            list.Add(new Element("ColourSpace", 0x2EB524, "b"));
            list.Add(new Element("GammaValue", 0x2FB523, "f"));
            list.Add(new Element("FrameRate", 0x2383E3, "f"));
            list.Add(new Element("Audio", 0xE1, "m"));
            list.Add(new Element("SamplingFrequency", 0xB5, "f"));
            list.Add(new Element("OutputSamplingFrequency", 0x78B5, "f"));
            list.Add(new Element("Channels", 0x9F, "u"));
            list.Add(new Element("ChannelPositions", 0x7D7B, "b"));
            list.Add(new Element("BitDepth", 0x6264, "u"));
            list.Add(new Element("TrackOperation", 0xE2, "m"));
            list.Add(new Element("TrackCombinePlanes", 0xE3, "m"));
            list.Add(new Element("TrackPlane", 0xE4, "m"));
            list.Add(new Element("TrackPlaneUID", 0xE5, "u"));
            list.Add(new Element("TrackPlaneType", 0xE6, "u"));
            list.Add(new Element("TrackJoinBlocks", 0xE9, "m"));
            list.Add(new Element("TrackJoinUID", 0xED, "u"));
            list.Add(new Element("TrickTrackUID", 0xC0, "u"));
            list.Add(new Element("TrickTrackSegmentUID", 0xC1, "b"));
            list.Add(new Element("TrickTrackFlag", 0xC6, "u"));
            list.Add(new Element("TrickMasterTrackUID", 0xC7, "u"));
            list.Add(new Element("TrickMasterTrackSegmentUID", 0xC4, "b"));
            list.Add(new Element("ContentEncodings", 0x6D80, "m"));
            list.Add(new Element("ContentEncoding", 0x6240, "m"));
            list.Add(new Element("ContentEncodingOrder", 0x5031, "u"));
            list.Add(new Element("ContentEncodingScope", 0x5032, "u"));
            list.Add(new Element("ContentEncodingType", 0x5033, "u"));
            list.Add(new Element("ContentCompression", 0x5034, "m"));
            list.Add(new Element("ContentCompAlgo", 0x4254, "u"));
            list.Add(new Element("ContentCompSettings", 0x4255, "b"));
            list.Add(new Element("ContentEncryption", 0x5035, "m"));
            list.Add(new Element("ContentEncAlgo", 0x47E1, "u"));
            list.Add(new Element("ContentEncKeyID", 0x47E2, "b"));
            list.Add(new Element("ContentSignature", 0x47E3, "b"));
            list.Add(new Element("ContentSigKeyID", 0x47E4, "b"));
            list.Add(new Element("ContentSigAlgo", 0x47E5, "u"));
            list.Add(new Element("ContentSigHashAlgo", 0x47E6, "u"));
            list.Add(new Element("Cues", 0x1C53BB6B, "m"));
            list.Add(new Element("CuePoint", 0xBB, "m"));
            list.Add(new Element("CueTime", 0xB3, "u"));
            list.Add(new Element("CueTrackPositions", 0xB7, "m"));
            list.Add(new Element("CueTrack", 0xF7, "u"));
            list.Add(new Element("CueClusterPosition", 0xF1, "u"));
            list.Add(new Element("CueRelativePosition", 0xF0, "u"));
            list.Add(new Element("CueDuration", 0xB2, "u"));
            list.Add(new Element("CueBlockNumber", 0x5378, "u"));
            list.Add(new Element("CueCodecState", 0xEA, "u"));
            list.Add(new Element("CueReference", 0xDB, "m"));
            list.Add(new Element("CueRefTime", 0x96, "u"));
            list.Add(new Element("CueRefCluster", 0x97, "u"));
            list.Add(new Element("CueRefNumber", 0x535F, "u"));
            list.Add(new Element("CueRefCodecState", 0xEB, "u"));
            list.Add(new Element("Attachments", 0x1941A469, "m"));
            list.Add(new Element("AttachedFile", 0x61A7, "m"));
            list.Add(new Element("FileDescription", 0x467E, "8"));
            list.Add(new Element("FileName", 0x466E, "8"));
            list.Add(new Element("FileMimeType", 0x4660, "s"));
            list.Add(new Element("FileData", 0x465C, "b"));
            list.Add(new Element("FileUID", 0x46AE, "u"));
            list.Add(new Element("FileReferral", 0x4675, "b"));
            list.Add(new Element("FileUsedStartTime", 0x4661, "u"));
            list.Add(new Element("FileUsedEndTime", 0x4662, "u"));
            list.Add(new Element("Chapters", 0x1043A770, "m"));
            list.Add(new Element("EditionEntry", 0x45B9, "m"));
            list.Add(new Element("EditionUID", 0x45BC, "u"));
            list.Add(new Element("EditionFlagHidden", 0x45BD, "u"));
            list.Add(new Element("EditionFlagDefault", 0x45DB, "u"));
            list.Add(new Element("EditionFlagOrdered", 0x45DD, "u"));
            list.Add(new Element("ChapterAtom", 0xB6, "m"));
            list.Add(new Element("ChapterUID", 0x73C4, "u"));
            list.Add(new Element("ChapterStringUID", 0x5654, "8"));
            list.Add(new Element("ChapterTimeStart", 0x91, "u"));
            list.Add(new Element("ChapterTimeEnd", 0x92, "u"));
            list.Add(new Element("ChapterFlagHidden", 0x98, "u"));
            list.Add(new Element("ChapterFlagEnabled", 0x4598, "u"));
            list.Add(new Element("ChapterSegmentUID", 0x6E67, "b"));
            list.Add(new Element("ChapterSegmentEditionUID", 0x6EBC, "u"));
            list.Add(new Element("ChapterPhysicalEquiv", 0x63C3, "u"));
            list.Add(new Element("ChapterTrack", 0x8F, "m"));
            list.Add(new Element("ChapterTrackNumber", 0x89, "u"));
            list.Add(new Element("ChapterDisplay", 0x80, "m"));
            list.Add(new Element("ChapString", 0x85, "8"));
            list.Add(new Element("ChapLanguage", 0x437C, "s"));
            list.Add(new Element("ChapCountry", 0x437E, "s"));
            list.Add(new Element("ChapProcess", 0x6944, "m"));
            list.Add(new Element("ChapProcessCodecID", 0x6955, "u"));
            list.Add(new Element("ChapProcessPrivate", 0x450D, "b"));
            list.Add(new Element("ChapProcessCommand", 0x6911, "m"));
            list.Add(new Element("ChapProcessTime", 0x6922, "u"));
            list.Add(new Element("ChapProcessData", 0x6933, "b"));
            list.Add(new Element("Tags", 0x1254C367, "m"));
            list.Add(new Element("Tag", 0x7373, "m"));
            list.Add(new Element("Targets", 0x63C0, "m"));
            list.Add(new Element("TargetTypeValue", 0x68CA, "u"));
            list.Add(new Element("TargetType", 0x63CA, "s"));
            list.Add(new Element("TagTrackUID", 0x63C5, "u"));
            list.Add(new Element("TagEditionUID", 0x63C9, "u"));
            list.Add(new Element("TagChapterUID", 0x63C4, "u"));
            list.Add(new Element("TagAttachmentUID", 0x63C6, "u"));
        }
    }
    public class Vint
    {
        /// <summary>
        /// Reads an EBML coded integer from a stream
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="actualValue"></param>
        /// <returns></returns>
        public static ELEMENTARY_TYPE ReadEBML(BitStreamReader sw, out long actualValue)
        {
            ELEMENTARY_TYPE vint = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            actualValue = (byte)vint.Value;
            if ((actualValue & 0x80) == 0x80)
                actualValue = actualValue & 0x7F;
            else if ((actualValue & 0x40) == 0x40)
            {
                sw.Position--;
                vint = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                actualValue = (short)vint.Value;
                actualValue = actualValue & 0x3FFF;
                return vint;
            }
            else
            {
                sw.Position--;
                if ((actualValue & 0x20) == 0x20)
                {
                    vint = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 3);
                    byte[] l = (byte[])vint.Value;
                    actualValue = l[0];
                    for (int i = 1; i < l.Length; i++)
                        actualValue = (actualValue * 256) + l[i];
                    actualValue = actualValue & 0x0FFFFF;
                    return vint;
                }
                if ((actualValue & 0x10) == 0x10)
                {
                    vint = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 4);
                    byte[] l = (byte[])vint.Value;
                    actualValue = l[0];
                    for (int i = 1; i < l.Length; i++)
                        actualValue = (actualValue * 256) + l[i];
                    actualValue = actualValue & 0x0FFFFFFF;
                    return vint;
                }
                if ((actualValue & 0x08) == 0x08)
                {
                    vint = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 5);
                    byte[] l = (byte[])vint.Value;
                    actualValue = l[0];
                    for (int i = 1; i < l.Length; i++)
                        actualValue = (actualValue * 256) + l[i];
                    actualValue = actualValue & 0x0FFFFFFF;
                    return vint;
                }
                if ((actualValue & 0x04) == 0x04)
                {
                    vint = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 6);
                    byte[] l = (byte[])vint.Value;
                    actualValue = l[0];
                    for (int i = 1; i < l.Length; i++)
                        actualValue = (actualValue * 256) + l[i];
                    actualValue = actualValue & 0x0FFFFFFF;
                    return vint;
                }
                if ((actualValue & 0x02) == 0x02)
                {
                    vint = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 7);
                    byte[] l = (byte[])vint.Value;
                    actualValue = l[0];
                    for (int i = 1; i < l.Length; i++)
                        actualValue = (actualValue * 256) + l[i];
                    actualValue = actualValue & 0x0FFFFFFF;
                    return vint;
                }
                if ((actualValue & 0x01) == 0x01)
                {
                    vint = new ELEMENTARY_TYPE(sw, 0, typeof(long));
                    actualValue = (long)vint.Value;
                    actualValue = actualValue & 0xFFFFFFFFF;
                    return vint;
                }
            }
            return vint;
        }
        public long ToLong(ELEMENTARY_TYPE vint)
        {
            byte[] l = (byte[])vint.Value;
            long value = l[0];
            for (int i = 1; i < l.Length; i++)
                value = (value * 256) + l[i];
            return value;
        }
        public static float nanosecondsToSeconds(long nano)
        {
            return ((float)nano / 1000000000);
        }
        public static long BytesToInt(byte[] dn)
        {
            int val = 0;
            for (int x = 0; x < dn.Length; x++)
                val = (val * 256 + dn[x]);
            return val;
        }

    }
    public enum EbmlElementType
    {
        VOID = 0,
        MASTER = 1,// read all subelements and return tree. Don't use this too large things like Segment
        UNSIGNED = 2,
        SIGNED = 3,
        TEXTA = 4,
        TEXTU = 5,
        BINARY = 6,
        FLOAT = 7,
        DATE = 8,
        JUST_GO_ON = 10
    }

}
