using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utils;

namespace VideoFiles
{
    public class MkvFile : LOCALIZED_DATA
    {
        List<MkvTag> ebml;
        List<MkvTag> tags = new List<MkvTag>();
        ELEMENTARY_TYPE segment;

        public List<MkvTag> Ebml
        {
            get { return ebml; }
            set { ebml = value; }
        }
        public List<MkvTag> Tags
        {
            get { return tags; }
            set { tags = value; }
        }
        public ELEMENTARY_TYPE Segment
        {
            get { return segment; }
            set { segment = value; }
        }
        public static SortedList<int, Element> tagIds = new SortedList<int, Element>();
        public MkvFile(string FileName)
        {
            MkvELements mk = new MkvELements();
            foreach (Element e in mk.List)
                tagIds.Add(e.Id, e);
  //          FileInfo f = new FileInfo(FileName);        
            BitStreamReader sw = new BitStreamReader(FileName, true);
            #region Header
            while (sw.Position < 0x70)
            {
                MkvTag ebml = new MkvTag(sw, 0);
                tags.Add(ebml);
            }
            #endregion
            LengthInFile = sw.Position;
            sw.Close();
            return;
         }
        private static SortedList<int, TaggedElement> alltags = new SortedList<int, TaggedElement>();

        public static SortedList<int, TaggedElement> AllTagsPresents
        {
            get { return MkvFile.alltags; }
            set { MkvFile.alltags = value; }
        }
        public List<TaggedElement> TagsPresent
        {
            get { return MkvFile.alltags.Values.ToList(); }

        }
    }
    public class MkvTag : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE tag;
        DataBlock data;
        List<MkvTag> subtags;
        ELEMENTARY_TYPE dataLength;
        Element tagValue = null;
        public string TagValue
        {
            get { return tagValue.Name; }
        }
        public ELEMENTARY_TYPE Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        public ELEMENTARY_TYPE DataLength
        {
            get { return dataLength; }
            set { dataLength = value; }
        }
        long longueur;
        public List<MkvTag> Subtags
        {
            get { return subtags; }
            set { subtags = value; }
        }
        public DataBlock Data
        {
            get { return data; }
            set { data = value; }
        }
        public String DataValue
        {
            get
            {
                byte[] dn = (byte[])data.Value.Value;
                switch (tagValue.ElementType)
                {
                    case EbmlElementType.DATE:
                        long dat = BitConverter.ToInt64(dn, 0);
                        DateTime dt = new DateTime(2001, 1, 1);
                        TimeSpan ts = new TimeSpan((long)Vint.nanosecondsToSeconds(dat) / 100);
         //               long w = 0x5B04DCAEA477800;
         //              ts = new TimeSpan(w/100);
                        dt += ts;
                        return dt.ToLongDateString()+ " "+dt.ToLongTimeString();
                    case EbmlElementType.FLOAT:
                        double d = 0f;
                        if (dn.Length==4)
                          d = BitConverter.ToSingle(dn, 0);
                        if (dn.Length == 8)
                            d = BitConverter.ToDouble(dn, 0);
                        return d.ToString();
                    case EbmlElementType.SIGNED:
                    case EbmlElementType.UNSIGNED:
                        return Vint.BytesToInt(dn).ToString("x2");
                    case EbmlElementType.TEXTA:
                        return Encoding.Default.GetString(dn);
                    case EbmlElementType.TEXTU:
                        return Encoding.UTF8.GetString(dn);
                    default:
                        return "";
                }
            }
        }

        public String FormattedValue
        {
            get
            {
                switch (tagValue.Name)
                {
                    case "TrackType":
                        switch (DataValue)
                        {
                            case "01": return "video";
                            case "02": return "audio";
                            case "03": return "complex";
                            case "10": return "logo";
                            case "11": return "subtitle";
                            case "12": return "buttons";
                            case "20": return "control";
                            default: return null;
                        }
                    case "FlagLacing":
                        switch (DataValue)
                        {
                            case "00": return "No Lacing";
                            default: return null;
                        }
                    case "DefaultDuration":
                        long l = long.Parse(DataValue, System.Globalization.NumberStyles.HexNumber);
                        return Vint.nanosecondsToSeconds(l).ToString() + " secondes par frame";
                    case "Duration":
                        long ll = 0;
                        byte[] dn = (byte[])data.Value.Value;
                        if (dn.Length == 4) ll= BitConverter.ToInt32((byte[])data.Value.Value, 0);
                        else ll = BitConverter.ToInt64(dn, 0);
                        return (ll).ToString() ;
                    case "TimecodeScale":
                        string l1 = Vint.BytesToInt((byte[])data.Value.Value).ToString();;
                        if (l1 == "1000000")
                            l1 += " (Timestamps in seconds)";
                        return l1;

                }
                return null;

            }
        }
        SimpleBlock bk;
        public SimpleBlock SimpleBlock
        {
            get { return bk; }
            set { bk = value; }
        }
        public MkvTag(BitStreamReader sw, long startAdress)
        {
            PositionOfStructureInFile = sw.Position;
            IdentifyTag(sw);
            if (sw.Position >= sw.Length)
                return;
            if (tagValue != null)
            {
                TaggedElement el;
                if (!MkvFile.AllTagsPresents.TryGetValue(tagValue.Id, out el))
                {
                    TaggedElement tg = new TaggedElement(tagValue);
                    tg.Tags.Add(this);
                    MkvFile.AllTagsPresents.Add(tagValue.Id, tg);
                }
                else
                    el.Tags.Add(this);
                switch (tagValue.ElementType)
                {
                    case EbmlElementType.MASTER:
                        DataLength = Vint.ReadEBML(sw, out longueur);
                        subtags = new List<MkvTag>();
                        int startA = (int)sw.Position;
                        while (sw.Position < startA + longueur)
                        {
                            MkvTag t = new MkvTag(sw, PositionOfStructureInFile);
                            if (t.tagValue == null)
                                return;
                            subtags.Add(t);
                        }
                        LengthInFile = longueur;
                //        if (sw.Position >= sw.Length)                            return;
                        break;
                    default:
                        switch (tagValue.Name)
                        {
                            case "SeekPosition":
                                data = new DataBlock(sw, tagValue.ElementType);
                                subtags = new List<MkvTag>();
                                long start = sw.Position;
                                long jump = startAdress + int.Parse(DataValue, System.Globalization.NumberStyles.HexNumber) - 5;// ???
                                sw.Position = jump;
                         //       while (sw.Position<jump +0x1000)
                                {
                                    MkvTag t = new MkvTag(sw, jump);
                                    subtags.Add(t);
                                }
                                sw.Position = start;
                                break;
                            case "SimpleBlock":
                                bk = new SimpleBlock(sw, 0);//GetLength(sw));
                                break;
                            default:
                                if (tagValue.Name == "DateUTC")
                                {

                                }
                                data = new DataBlock(sw, tagValue.ElementType);
                                break;
                        }
                        if (data != null) LengthInFile = data.Longueur;
                        //   else 
                        LengthInFile = sw.Position - PositionOfStructureInFile;
                        break;
                }
            }
            else
                return;
            if (LengthInFile == 0) LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        private void IdentifyTag(BitStreamReader sw)
        {
            long length;
            tag = Vint.ReadEBML(sw, out length);
            if (tag == null)
                return;
            switch (tag.Value.GetType().Name)
            {
                case "Int32":
                    MkvFile.tagIds.TryGetValue((int)tag.Value, out tagValue);
                    break;
                case "Int16":
                    MkvFile.tagIds.TryGetValue((short)tag.Value, out tagValue);
                    break;
                case "Byte[]":
                    byte[] b = (byte[])tag.Value;
                    int val = b[0];
                    for (int x = 1; x < b.Length; x++)
                        val = (val * 256) + b[x];
                    MkvFile.tagIds.TryGetValue(val, out tagValue);
                    break;
                case "Byte":
                    MkvFile.tagIds.TryGetValue((byte)tag.Value, out tagValue);
                    break;
                default:
                    break;
            }
        }
        public override string ToString()
        {
            return TagValue;
        }
    }
    public class DataBlock:LOCALIZED_DATA
    {
        long longueur;
        ELEMENTARY_TYPE dataLength;
        private string formattedValue;
        public ELEMENTARY_TYPE DataLength
        {
            get { return dataLength; }
            set { dataLength = value; }
        }
        public long Longueur
        {
            get { return longueur; }
            set { longueur = value; }
        }
        ELEMENTARY_TYPE value;
        public ELEMENTARY_TYPE Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public string FormattedValue
        {
            get { return formattedValue; }
            set { formattedValue = value; }
        }

        public DataBlock(BitStreamReader sw, EbmlElementType el_type)
        {
            PositionOfStructureInFile = sw.Position;
            dataLength = Vint.ReadEBML(sw, out longueur);
            value = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), (int)longueur);
            LengthInFile = sw.Position - PositionOfStructureInFile;
            switch(el_type)
            {
                case EbmlElementType.TEXTA:
                    formattedValue = Encoding.Default.GetString((byte[])value.Value);
                    break;
                case EbmlElementType.TEXTU:
                    formattedValue = Encoding.UTF8.GetString((byte[])value.Value);
                   break;
                case EbmlElementType.DATE:
                    break;
                case EbmlElementType.SIGNED:
                case EbmlElementType.UNSIGNED:
                    break;
            }
        }
     }
    public class SimpleBlock:LOCALIZED_DATA
    {
        ELEMENTARY_TYPE blockLength;
        ELEMENTARY_TYPE track_Number;
        ELEMENTARY_TYPE time_Code;
        ELEMENTARY_TYPE flags;
        ELEMENTARY_TYPE data;
        bool? isKeyFrame;
        ELEMENTARY_TYPE numberOfFrames;

         long track_number_Value;
        public long Track_number_Value
        {
            get { return track_number_Value; }
            set { track_number_Value = value; }
        }
        public ELEMENTARY_TYPE Track_Number
        {
            get { return track_Number; }
            set { track_Number = value; }
        }
        public ELEMENTARY_TYPE Time_Code
        {
            get { return time_Code; }
            set { time_Code = value; }
        }
        public ELEMENTARY_TYPE Flags
        {
            get { return flags; }
            set { flags = value; }
        }
        public bool? IsKeyFrame
        {
            get { return isKeyFrame; }
            set { isKeyFrame = value; }
        }
        public ELEMENTARY_TYPE NumberOfFrames
        {
            get { return numberOfFrames; }
            set { numberOfFrames = value; }
        }
        public ELEMENTARY_TYPE Data
        {
            get { return data; }
            set { data = value; }
        }        
        public SimpleBlock(BitStreamReader sw, long length)
        {
            PositionOfStructureInFile = sw.Position;
            long longueur;
            blockLength = Vint.ReadEBML(sw,out longueur);
            track_Number = Vint.ReadEBML(sw, out track_number_Value);
            time_Code = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            flags = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            byte fl = (byte)flags.Value;
            if(((int)fl&0x80)==0x80)
            {
                isKeyFrame = true;
            }
            long nb;
            if (((int)fl & 0x06)!=0)
            {
                numberOfFrames = new ELEMENTARY_TYPE(sw, 0, typeof(byte)); 
                longueur--;
            }
            data = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), (int)longueur - 4);
            long start = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
   //         sw.Position = start;
            data = null;
        }
    }
    public class TaggedElement:LOCALIZED_DATA
    {
        private Element element;
        public Element Element
        {
            get { return element; }
            set { element = value; }
        }
        private List<MkvTag> tags;
        public List<MkvTag> Tags
        {
            get { return tags; }
            set { tags = value; }
        }
        public TaggedElement(Element el)
        {
            element = el;
            tags = new List<MkvTag>();
        }
        public override string ToString()
        {
            string occ = " occurrence";
            if (tags.Count > 1)
                occ += "s";
            return  element.Name + " (" +element.Id.ToString("x2") + ", " + Element.ElementType.ToString()+") : "+ tags.Count.ToString()+ occ;
        }
    }
 }
