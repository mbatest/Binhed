using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;

namespace Code
{
    #region Security
    #region enumerations
    enum WIN_CERTIFICATE_REVISION
    {
        WIN_CERT_REVISION_1_0 = 0x0100,
        WIN_CERT_REVISION_2_0 = 0x0200
    }
    enum WIN_CERTIFICATE_TYPE
    {
        WIN_CERT_TYPE_X509 = 1,
        WIN_CERT_TYPE_PKCS_SIGNED_DATA = 2,
        WIN_CERT_TYPE_RESERVED_1 = 3,
        WIN_CERT_TYPE_TS_STACK_SIGNED = 4,
        WIN_CERT_TYPE_PKCS1_SIGN = 0x0009
    }
    public enum CRT_RDN
    {
         CERT_RDN_ANY_TYPE =0x00,
        CERT_RDN_ENCODED_BLOB =0x01,
        CERT_RDN_OCTET_STRING =0x02,
        CERT_RDN_NUMERIC_STRING =0x03,
        CERT_RDN_PRINTABLE_STRING =0x04,
        CERT_RDN_TELETEX_STRING =0x05,
        CERT_RDN_T61_STRING =0x05,
        CERT_RDN_VIDEOTEX_STRING =0x06,
        CERT_RDN_IA5_STRING =0x07,
        CERT_RDN_GRAPHIC_STRING =0x08,
        CERT_RDN_VISIBLE_STRING =0x09,
        CERT_RDN_ISO646_STRING =0x09,
        CERT_RDN_GENERAL_STRING =0x0a,
        CERT_RDN_UNIVERSAL_STRING =0x0b,
        CERT_RDN_INT4_STRING =0x0b,
        CERT_RDN_BMP_STRING =0x0c,
        CERT_RDN_UNICODE_STRING =0x0c, 
        CERT_RDN_UTF8_STRING =0x0d,
        CERT_RDN_TYPE_MASK = 0x000000ff
   }
    public enum ASN_TAG_TYPE
    {
        UNIVERSAL = 0x00,
        BOOLEAN = 0x01,
        INTEGER = 0x02,
        BIT_STRING = 0x03,// Array of bits
        OCTET_STRING = 0x04,
        NULL_ID = 0x05,
        OBJECT_ID = 0x06,
        OBJECT_DESCRIPTOR = 0x07,
        EXTERNAL = 0x08,
        REAL = 0x09,
        ENUMERATED = 0x0a,
        EMBEDDED_PDV = 0x0b,
        UTF8_STR = 0x0c,
        RELATIVE_OID = 0x0d,
        RES1 = 0x0e,
        RES2 = 0x0f,
        SEQUENCE = 0x10,
        SET = 0x11,
        NUMERIC_STR = 0x12,
        PRINTABLE_STRING = 0x13,
        TeletexString = 0x14,
        VideotexString = 0x15,
        IA5_STR = 0x16,
        UTC_TIME = 0x17,
        GENERALIZED_TIME = 0x18,
        GRAPHIC_STRING = 0x19,
        VISIBLE_STR = 0x1a,
        GENERAL_STR = 0x1b,
        UNIVERSAL_STR = 0x1c,
        CHARACTER_STR = 0x1d,
        BMP_STR = 0x1e,
        TAG_MASK = 0x1F,//A tag number field of 1F indicates that the tag number is stored in subsequent bytes in base-128 in big-endian order where the 8th bit is 1 if more bytes follow and 0 for the last byte of the tag number.
        MICROSOFT_SPECIFIC = 0x86
    }
    public enum CERTIFICATE_TYPE
    {
        WIN_CERT_TYPE_X509 = 0x0001,// X.509 certificate.
        WIN_CERT_TYPE_PKCS_SIGNED_DATA = 0x0002,//PKCS SignedData structure.
        WIN_CERT_TYPE_RESERVED_1 = 0x0003,// Reserved.
        WIN_CERT_TYPE_PKCS1_SIGN = 0x0009
    }
    public class Key
    {
      public   string Soid;
      public string Name;
      public Key(string Soid, string Name)
      {
          this.Soid = Soid;
          this.Name = Name;
      }
    }
    #endregion
    public class ASN1Tag
    {
        private int tag;
        private byte data;
        private int Class;
        private ASN_TAG_TYPE tagType;
        private bool isContextSpecific;
        private bool isApplication;
        private bool isUniversal;
        private bool isPrivate;
        private bool isConstructed;
        #region Properties
        public bool IsContextSpecific
        {
            get { return isContextSpecific; }
            set { isContextSpecific = value; }
        }
        public bool IsApplication
        {
            get { return isApplication; }
            set { isApplication = value; }
        }
        public bool IsUniversal
        {
            get { return isUniversal; }
            set { isUniversal = value; }
        }
        public bool IsPrivate
        {
            get { return isPrivate; }
            set { isPrivate = value; }
        }
        public bool IsConstructed
        {
            get { return isConstructed; }
            set { isConstructed = value; }
        }
        public ASN_TAG_TYPE TagType
        {
            get { return tagType; }
        }
        #endregion
        public ASN1Tag(byte tg)
        {
            data = tg;
            Class = (tg & 0xC0) >> 6;
            /* Universal 00
               Application 01
               Context-specific 10
               Private 11*/
            isUniversal = Class == 0x00;
            isApplication = Class == 0x01;
            isContextSpecific = Class == 0x2;
            isPrivate = Class == 0x3;
            isConstructed = (tg & 0x20) == 0x20;
            tag = tg & 0x1F;
            tagType = (ASN_TAG_TYPE)tag;
        }
        public override string ToString()
        {
            if (isContextSpecific & IsConstructed)
            {
                return "OPTIONAL[" + (tag & 0x0F).ToString() + "]";
            }
            string dat = tagType.ToString();
            if (IsConstructed)
                dat = "Constructed " + dat;
            if (isPrivate)
                dat = "Private " + dat;
            if (isContextSpecific)
                dat = "Context Specific " + dat;
            return dat;
        }
    }
    public class ASN1_OBJECT : IMAGE_BASE_DATA
    {
        #region Private members
        private long offset;
        private int length;
        private ASN1Tag asnTag;
        private byte tag;
        private List<ASN1_OBJECT> nodes ;
        private byte[] data;
        private ASN1_OBJECT parent;
        #endregion
        #region Private methods
        public ASN1Tag AsnTag
        {
            get { return asnTag; }
            set { asnTag = value; }
        }
        private int FindLength(MemoryStream sw)
        {
            int bb = sw.ReadByte();
            int dataLength = 0;
            if ((bb & 0x80) == 0x80)
            {
                dataLength = bb & 0x7F;
                byte[] dt = new byte[dataLength];
                sw.Read(dt, 0, dataLength);
                dataLength = 0;
                for (int i = 0; i < dt.Length; i++)
                {
                    dataLength = dataLength * 256 + dt[i];
                }
            }
            else
            {
                dataLength = bb;
            }
            return dataLength;
        }
        private int FindLength(BinaryFileReader sw)
        {
            int bb = sw.ReadByte();
            int dataLength = 0;
            if ((bb & 0x80) == 0x80)
            {
                dataLength = bb & 0x7F;
                byte[] dt = new byte[dataLength];
                sw.Read(dt, 0, dataLength);
                dataLength = 0;
                for (int i = 0; i < dt.Length; i++)
                {
                    dataLength = dataLength * 256 + dt[i];
                }
            }
            else
            {
                dataLength = bb;
            }
            return dataLength;
        }
        private string DecodeOID(byte[] pData)
        {

            int dw;
            List<int> decoded = new List<int>();
            decoded.Add(pData[0] / 40);
            decoded.Add(pData[0] % 40);
            for (int deb = 1; deb < pData.Length; deb++)
            {
                if ((pData[deb] & 0x80) == 0x80)
                {
                    dw = 0;
                    while ((pData[deb] & 0x80) == 0x80)
                    {

                        dw <<= 7;  // *128
                        dw += (pData[deb] & 0x7F);
                        deb++;
                    }
                    dw <<= 7;
                    dw += (pData[deb] & 0x7F);
                    decoded.Add(dw);
                }
                else // last byte
                {
                    decoded.Add(pData[deb]);
                }
            }
            string dec = "";
            for (int u = 0; u < decoded.Count; u++)
            {
                dec += decoded[u].ToString();
                if (u < decoded.Count - 1)
                    dec += ".";
            }
            return dec;
        }
        private string DecodeTime(byte[] pData)
        {
            int tm_year = 2000;
            if ((data[0]-0x30) >= 5)
                tm_year = 1900;
            tm_year += (pData[0] -0x30) * 10 + pData[1] -0x30;
            int tm_month = (pData[2] - 0x30) * 10 + pData[3] - 0x30;
            int tm_day = (pData[4] - 0x30) * 10 + pData[5] - 0x30;
            int tm_hour = (pData[6] - 0x30) * 10 + pData[7] - 0x30;
            int tm_min = (pData[8] - 0x30) * 10 + pData[9] - 0x30;
            int tm_sec = (pData[10] - 0x30) * 10 + pData[11] - 0x30;
            DateTime dt = new DateTime(tm_year, tm_month, tm_day, tm_hour, tm_min, tm_sec);
            return dt.ToShortDateString() + " "+dt.ToLongTimeString();
        }
        private string oid;
        #endregion
        #region  Properties
        public bool HasSons
        { get { return nodes != null; } }
        public string Object_type
        {
            get
            {
                return asnTag.ToString();
/*              if ((tag & 0xa0) == 0xa0)
                {
                    return "OPTIONAL[" + (tag & 0x0F).ToString() + "]";
                }
                else
                    return ((ASN_TAG_TYPE)tag).ToString();*/
            }
        }
        public long Offset
        {
            get { return offset; }
            set { offset = value; }
        }
        public int Length
        {
            get { return length; }
            set { length = value; }
        }
        public byte Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        public List<ASN1_OBJECT> Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }
        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }
        public string DataString
        {
            get
            {
                switch (AsnTag.TagType)
                {
                    case ASN_TAG_TYPE.UTC_TIME:
                        string time =DecodeTime(data);
                        return time;
                    case ASN_TAG_TYPE.UTF8_STR:
                        return Encoding.Default.GetString(data);
                    case ASN_TAG_TYPE.MICROSOFT_SPECIFIC:
                    case ASN_TAG_TYPE.PRINTABLE_STRING:
                        return Encoding.Default.GetString(data);
                    case ASN_TAG_TYPE.OBJECT_ID:
                        #region OID userStrings
                        string[] soid_codes = new string[] { "1.2.840.113549", "1.2.840.113549.1", 
            "1.2.840.113549.2", "1.2.840.113549.3", "1.2.840.113549.1.1", "1.2.840.113549.1.2", 
            "1.2.840.113549.1.3", "1.2.840.113549.1.4", "1.2.840.113549.1.5", "1.2.840.113549.1.6", 
            "1.2.840.113549.1.7", "1.2.840.113549.1.8", "1.2.840.113549.1.9", "1.2.840.113549.1.10", 
            "1.2.840.113549.1.12", "1.2.840.113549.1.1.1", "1.2.840.113549.1.1.2", "1.2.840.113549.1.1.3", 
            "1.2.840.113549.1.1.4", "1.2.840.113549.1.1.5", "1.2.840.113549.1.1.6", "1.2.840.113549.1.3.1", 
            "1.2.840.113549.1.7.1", "1.2.840.113549.1.7.2", "1.2.840.113549.1.7.3", "1.2.840.113549.1.7.4",
            "1.2.840.113549.1.7.5", "1.2.840.113549.1.7.5", "1.2.840.113549.1.7.6", "1.2.840.113549.1.9.1", 
            "1.2.840.113549.1.9.2", "1.2.840.113549.1.9.3", "1.2.840.113549.1.9.4", "1.2.840.113549.1.9.5", "1.2.840.113549.1.9.6", 
            "1.2.840.113549.1.9.7", "1.2.840.113549.1.9.9", "1.2.840.113549.1.9.9", "1.2.840.113549.1.9.14", "1.2.840.113549.1.9.15", 
            "1.2.840.113549.1.9.15.1", "1.2.840.113549.1.9.16.3", "1.2.840.113549.1.9.16.3.5", "1.2.840.113549.1.9.16.3.6", 
            "1.2.840.113549.1.9.16.3.7", "1.2.840.113549.2.2", "1.2.840.113549.2.4", "1.2.840.113549.2.5", "1.2.840.113549.3.2", 
            "1.2.840.113549.3.4", "1.2.840.113549.3.7", "1.2.840.113549.3.9", "1.2.840.10046", "1.2.840.10046.2.1", "1.2.840.10040",
            "1.2.840.10040.4.1", "1.2.840.10040.4.3", "2.5", "2.5.8", "2.5.8.1", "2.5.8.2", "2.5.8.3", "2.5.8.1.1", "1.3.14", "1.3.14.3.2", 
            "1.3.14.3.2.2", "1.3.14.3.2.3", "1.3.14.3.2.4", "1.3.14.3.2.6", "1.3.14.3.2.7", "1.3.14.3.2.8", "1.3.14.3.2.9", "1.3.14.3.2.10", "1.3.14.3.2.11", "1.3.14.3.2.12", "1.3.14.3.2.13", "1.3.14.3.2.14", "1.3.14.3.2.15", "1.3.14.3.2.16", "1.3.14.3.2.17", "1.3.14.3.2.18", "1.3.14.3.2.19", "1.3.14.3.2.20", "1.3.14.3.2.21", "1.3.14.3.2.22", "1.3.14.3.2.23", "1.3.14.3.2.24", "1.3.14.3.2.25", "1.3.14.3.2.26", "1.3.14.3.2.27", "1.3.14.3.2.28", "1.3.14.3.2.29", "1.3.14.7.2", "1.3.14.7.2.1", "1.3.14.7.2.2", "1.3.14.7.2.3", "1.3.14.7.2.2.1", "1.3.14.7.2.3.1", "2.16.840.1.101.2.1", "2.16.840.1.101.2.1.1.1", "2.16.840.1.101.2.1.1.2", "2.16.840.1.101.2.1.1.3", "2.16.840.1.101.2.1.1.4", "2.16.840.1.101.2.1.1.5", "2.16.840.1.101.2.1.1.6", "2.16.840.1.101.2.1.1.7", "2.16.840.1.101.2.1.1.8", "2.16.840.1.101.2.1.1.9", "2.16.840.1.101.2.1.1.10", "2.16.840.1.101.2.1.1.11", "2.16.840.1.101.2.1.1.12", "2.16.840.1.101.2.1.1.13", "2.16.840.1.101.2.1.1.14", "2.16.840.1.101.2.1.1.15", "2.16.840.1.101.2.1.1.16", "2.16.840.1.101.2.1.1.17", "2.16.840.1.101.2.1.1.18", "2.16.840.1.101.2.1.1.19", "2.16.840.1.101.2.1.1.20", "2.16.840.1.101.2.1.1.21", "2.5.4.3", "2.5.4.4", "2.5.4.5", "2.5.4.6", "2.5.4.7", "2.5.4.8", "2.5.4.9", "2.5.4.10", "2.5.4.11", "2.5.4.12", "2.5.4.13", "2.5.4.14", "2.5.4.15", "2.5.4.16", "2.5.4.17", "2.5.4.18", "2.5.4.19", "2.5.4.20", "2.5.4.21", "2.5.4.22", "2.5.4.23", "2.5.4.24", "2.5.4.25", "2.5.4.26", "2.5.4.27", "2.5.4.28", "2.5.4.29", "2.5.4.30", "2.5.4.31", "2.5.4.32", "2.5.4.33", "2.5.4.34", "2.5.4.35", "2.5.4.36", "2.5.4.37", "2.5.4.38", "2.5.4.39", "2.5.4.40", "2.5.4.42", "2.5.4.43", "2.5.4.46", "2.5.29.1", "2.5.29.2", "2.5.29.3", "2.5.29.4", "2.5.29.5", "2.5.29.7", "2.5.29.8", "2.5.29.9", "2.5.29.10", "2.5.29.14", "2.5.29.15", "2.5.29.16", "2.5.29.17", "2.5.29.18", "2.5.29.19", "2.5.29.20", "2.5.29.21", "2.5.29.23", "2.5.29.27", "2.5.29.28", "2.5.29.30", "2.5.29.31", "2.5.29.32", "2.5.29.32.0", "2.5.29.33", "2.5.29.35", "2.5.29.36", "2.5.29.37", "2.5.29.46", "0.9.2342.19200300.100.1.25", "1.2.840.113549.1.9.20", "1.2.840.113549.1.9.21", "1.3.6.1.4.1.311.2.1.14", "1.3.6.1.4.1.311.10.2", "1.3.6.1.4.1.311.10.3.1", "1.3.6.1.4.1.311.10.3.2", "1.3.6.1.4.1.311.10.3.4", "1.3.6.1.4.1.311.10.3.4.1", "1.3.6.1.4.1.311.10.3.5", "1.3.6.1.4.1.311.10.3.6", "1.3.6.1.4.1.311.10.3.7", "1.3.6.1.4.1.311.10.3.8", "1.3.6.1.4.1.311.10.3.9", "1.3.6.1.4.1.311.10.3.10", "1.3.6.1.4.1.311.10.3.11", "1.3.6.1.4.1.311.10.3.12", "1.3.6.1.4.1.311.10.3.13", "1.3.6.1.4.1.311.10.3.14", "1.3.6.1.4.1.311.10.4.1", "1.3.6.1.4.1.311.10.8.1", "1.3.6.1.4.1.311.10.9.1", "1.3.6.1.4.1.311.10.10.1", "1.3.6.1.4.1.311.10.10.1.1", "1.3.6.1.4.1.311.10.12.1", "1.3.6.1.4.1.311.13.1", "1.3.6.1.4.1.311.13.2.1", "1.3.6.1.4.1.311.13.2.2", "1.3.6.1.4.1.311.13.2.3", "1.3.6.1.4.1.311.17.1", "1.3.6.1.4.1.311.17.2", "1.3.6.1.4.1.311.20.1", "1.3.6.1.4.1.311.20.2", "1.3.6.1.4.1.311.20.2.1", "1.3.6.1.4.1.311.20.2.2", "1.3.6.1.4.1.311.20.3", "1.3.6.1.4.1.311.21.2", "1.3.6.1.4.1.311.21.3", "1.3.6.1.4.1.311.21.4", "1.3.6.1.4.1.311.21.5", "1.3.6.1.4.1.311.21.6", "1.3.6.1.4.1.311.21.7", "1.3.6.1.4.1.311.21.8", "1.3.6.1.4.1.311.21.9", "1.3.6.1.4.1.311.21.10", "1.3.6.1.4.1.311.21.11", "1.3.6.1.4.1.311.21.12", "1.3.6.1.4.1.311.21.13", "1.3.6.1.4.1.311.21.14",
            "1.3.6.1.4.1.311.21.15", "1.3.6.1.4.1.311.21.16", "1.3.6.1.4.1.311.21.17", "1.3.6.1.4.1.311.21.19", "1.3.6.1.4.1.311.21.20", "1.3.6.1.4.1.311.21.21", "1.3.6.1.4.1.311.21.22", "1.3.6.1.4.1.311.10.7.1", "1.3.6.1.5.5.7", "1.3.6.1.5.5.7.1", "1.3.6.1.5.5.7.1.1", "1.3.6.1.5.5.7.2.1", "1.3.6.1.5.5.7.2.2", "1.3.6.1.5.5.7.3", "1.3.6.1.5.5.7.3.1", "1.3.6.1.5.5.7.3.2", "1.3.6.1.5.5.7.3.3", "1.3.6.1.5.5.7.3.4", "1.3.6.1.5.5.7.3.5", "1.3.6.1.5.5.7.3.6", "1.3.6.1.5.5.7.3.7", "1.3.6.1.5.5.7.3.8", "1.3.6.1.5.5.7.6.2", "1.3.6.1.5.5.7.7", "1.3.6.1.5.5.7.7.1", "1.3.6.1.5.5.7.7.2", "1.3.6.1.5.5.7.7.3", "1.3.6.1.5.5.7.7.4", "1.3.6.1.5.5.7.7.5", "1.3.6.1.5.5.7.7.6", "1.3.6.1.5.5.7.7.7", "1.3.6.1.5.5.7.7.8", "1.3.6.1.5.5.7.7.9", "1.3.6.1.5.5.7.7.10", "1.3.6.1.5.5.7.7.11", "1.3.6.1.5.5.7.7.15", "1.3.6.1.5.5.7.7.16", "1.3.6.1.5.5.7.7.17", "1.3.6.1.5.5.7.7.18", "1.3.6.1.5.5.7.7.19", "1.3.6.1.5.5.7.7.21", "1.3.6.1.5.5.7.7.22", "1.3.6.1.5.5.7.7.23", "1.3.6.1.5.5.7.12.2", "1.3.6.1.5.5.7.12.3", "1.3.6.1.5.5.7.48", "1.3.6.1.5.5.7.48.1", "1.3.6.1.5.5.7.48.2", "1.3.6.1.5.5.8.2.2", "2.16.840.1.113730", "2.16.840.1.113730.1", "2.16.840.1.113730.1.1", "2.16.840.1.113730.1.2", "2.16.840.1.113730.1.3", "2.16.840.1.113730.1.4", "2.16.840.1.113730.1.7", "2.16.840.1.113730.1.8", "2.16.840.1.113730.1.12", "2.16.840.1.113730.1.13","1.3.6.1.4.1.311","1.3.6.1.4.1.311.2","1.3.6.1.4.1.311.2.1.4","1.3.6.1.4.1.311.2.1.11","1.3.6.1.4.1.311.2.1.12","1.3.6.1.4.1.311.2.1.15","1.3.6.1.4.1.311.2.1.10","1.3.6.1.4.1.311.2.1.26","1.3.6.1.4.1.311.2.1.27","1.3.6.1.4.1.311.2.1.28","1.3.6.1.4.1.311.2.1.29","1.3.6.1.4.1.311.2.1.30","1.3.6.1.4.1.311.2.1.14","1.3.6.1.4.1.311.2.1.18","1.3.6.1.4.1.311.2.1.19","1.3.6.1.4.1.311.2.1.20","1.3.6.1.4.1.311.2.1.21","1.3.6.1.4.1.311.2.1.22","1.3.6.1.4.1.311.2.1.25","1.3.6.1.4.1.311.2.1.25","1.3.6.1.4.1.311.2.2","1.3.6.1.4.1.311.2.2.1","1.3.6.1.4.1.311.2.2.2","1.3.6.1.4.1.311.2.2.3","1.3.6.1.4.1.311.3","1.3.6.1.4.1.311.3.2.1","1.3.6.1.4.1.311.4","1.3.6.1.4.1.311.10","1.3.6.1.4.1.311.10.1","1.3.6.1.4.1.311.10.1.1","1.3.6.1.4.1.311.10.2","1.3.6.1.4.1.311.10.3.1","1.3.6.1.4.1.311.10.3.2","1.3.6.1.4.1.311.10.3.3","1.3.6.1.4.1.311.10.3.3.1","1.3.6.1.4.1.311.10.3.4","1.3.6.1.4.1.311.10.3.4.1","1.3.6.1.4.1.311.10.3.5","1.3.6.1.4.1.311.10.3.6","1.3.6.1.4.1.311.10.3.7","1.3.6.1.4.1.311.10.3.8","1.3.6.1.4.1.311.10.3.9","1.3.6.1.4.1.311.10.3.10","1.3.6.1.4.1.311.10.3.11","1.3.6.1.4.1.311.10.3.12","1.3.6.1.4.1.311.10.4.1","1.3.6.1.4.1.311.10.5.1","1.3.6.1.4.1.311.10.5.2","1.3.6.1.4.1.311.10.6.1","1.3.6.1.4.1.311.10.6.2","1.3.6.1.4.1.311.10.7","1.3.6.1.4.1.311.10.7.1","1.3.6.1.4.1.311.10.8.1","1.3.6.1.4.1.311.10.9.1","1.3.6.1.4.1.311.10.10","1.3.6.1.4.1.311.10.10.1","1.3.6.1.4.1.311.10.11","1.3.6.1.4.1.311.10.11.","1.3.6.1.4.1.311.10.12","1.3.6.1.4.1.311.10.12.1","1.3.6.1.4.1.311.12","1.3.6.1.4.1.311.12.1.1","1.3.6.1.4.1.311.12.1.2","1.3.6.1.4.1.311.12.2.1","1.3.6.1.4.1.311.12.2.2","1.3.6.1.4.1.311.13","1.3.6.1.4.1.311.13.1","1.3.6.1.4.1.311.13.2.1","1.3.6.1.4.1.311.13.2.2","1.3.6.1.4.1.311.15","1.3.6.1.4.1.311.16","1.3.6.1.4.1.311.16.4","1.3.6.1.4.1.311.17","1.3.6.1.4.1.311.17.1","1.3.6.1.4.1.311.18","1.3.6.1.4.1.311.19","1.3.6.1.4.1.311.20","1.3.6.1.4.1.311.20.1","1.3.6.1.4.1.311.20.2","1.3.6.1.4.1.311.20.2.1","1.3.6.1.4.1.311.20.2.2","1.3.6.1.4.1.311.20.2.3","1.3.6.1.4.1.311.20.3","1.3.6.1.4.1.311.21","1.3.6.1.4.1.311.21.1","1.3.6.1.4.1.311.25","1.3.6.1.4.1.311.25.1","1.3.6.1.4.1.311.30","1.3.6.1.4.1.311.31",
        "1.3.6.1.4.1.311.31.1","1.3.6.1.4.1.311.40","1.3.6.1.4.1.311.41","1.3.6.1.4.1.311.42","1.3.6.1.4.1.311.88"};
                        string[] soi = new string[]{"RSA","PKCS","RSA HASH","RSA ENCRYPT","PKCS 1","PKCS 2","PKCS 3","PKCS 4","PKCS 5","PKCS 6","PKCS 7","PKCS 8","PKCS 9","PKCS 10","PKCS 11","RSA RSA","RSA MD2RSA","RSA MD4RSA","RSA MD5RSA","RSA SHA1RSA","RSA SET0AEP RSA","RSA DH","RSA data","RSA signedData","RSA envelopedData","RSA signEnvData","RSA digestedData","RSA hashedData","RSA encryptedData","RSA emailAddr","RSA unstructName","RSA contentType","RSA messageDigest","RSA signingTime","RSA counterSign","RSA challengePwd","RSA unstructAddr","RSA extCertAttrs","RSA certExtensions","RSA SMIMECapabilities","RSA preferSignedData","RSA SMIMEalg","RSA SMIMEalgESDH","RSA SMIMEalgCMS3DESwrap","RSA SMIMEalgCMSRC2wrap","RSA MD2","RSA MD4","RSA MD5","RSA RC2CBC","RSA RC4","RSA DES EDE3 CBC","RSA RC5 CBCPad","ANSI X942","ANSI X942 DH","X957","X957 DSA","X957 SHA1DSA","DS","DSALG","DSALG CRPT","DSALG HASH","DSALG SIGN","DSALG RSA","OIW","OIWSEC","OIWSEC md4RSA","OIWSEC md5RSA","OIWSEC md4RSA2","OIWSEC desECB","OIWSEC desCBC","OIWSEC desOFB","OIWSEC desCFB","OIWSEC desMAC","OIWSEC rsaSign","OIWSEC dsa","OIWSEC shaDSA","OIWSEC mdc2RSA","OIWSEC shaRSA","OIWSEC dhCommMod","OIWSEC desEDE","OIWSEC sha","OIWSEC mdc2","OIWSEC dsaComm","OIWSEC dsaCommSHA","OIWSEC rsaXchg","OIWSEC keyHashSeal","OIWSEC md2RSASign","OIWSEC md5RSASign","OIWSEC sha1","OIWSEC dsaSHA1","OIWSEC dsaCommSHA1","OIWSEC sha1RSASign","OIWDIR","OIWDIR CRPT","OIWDIR HASH","OIWDIR SIGN","OIWDIR md2","OIWDIR md2RSA","INFOSEC","INFOSEC sdnsSignature","INFOSEC mosaicSignature","INFOSEC sdnsConfidentiality","INFOSEC mosaicConfidentiality","INFOSEC sdnsIntegrity","INFOSEC mosaicIntegrity","INFOSEC sdnsTokenProtection","INFOSEC mosaicTokenProtection","INFOSEC sdnsKeyManagement","INFOSEC mosaicKeyManagement","INFOSEC sdnsKMandSig","INFOSEC mosaicKMandSig","INFOSEC SuiteASignature","INFOSEC SuiteAConfidentiality","INFOSEC SuiteAIntegrity","INFOSEC SuiteATokenProtection","INFOSEC SuiteAKeyManagement","INFOSEC SuiteAKMandSig","INFOSEC mosaicUpdatedSig","INFOSEC mosaicKMandUpdSig","INFOSEC mosaicUpdateInteg","COMMON NAME","SUR NAME","DEVICE SERIAL NUMBER","COUNTRY NAME","LOCALITY NAME","STATE OR PROVINCE NAME","STREET ADDRESS","ORGANIZATION NAME","ORGANIZATIONAL UNIT NAME","TITLE","DESCRIPTION","SEARCH GUIDE","BUSINESS CATEGORY","POSTAL ADDRESS","POSTAL CODE","POST OFFICE BOX","PHYSICAL DELIVERY OFFICE NAME","TELEPHONE NUMBER","TELEX NUMBER","TELETEXT TERMINAL IDENTIFIER","FACSIMILE TELEPHONE NUMBER","X21 ADDRESS","INTERNATIONAL ISDN NUMBER","REGISTERED ADDRESS","DESTINATION INDICATOR","PREFERRED DELIVERY METHOD","PRESENTATION ADDRESS","SUPPORTED APPLICATION CONTEXT","MEMBER","OWNER","ROLE OCCUPANT","SEE ALSO","USER PASSWORD","USER CERTIFICATE","CA CERTIFICATE","AUTHORITY REVOCATION LIST","CERTIFICATE REVOCATION LIST","CROSS CERTIFICATE PAIR","GIVEN NAME","INITIALS","DN QUALIFIER","AUTHORITY KEY IDENTIFIER","KEY ATTRIBUTES","CERT POLICIES 95","KEY USAGE RESTRICTION","LEGACY POLICY MAPPINGS","SUBJECT ALT NAME","ISSUER ALT NAME","SUBJECT DIR ATTRS","BASIC CONSTRAINTS","SUBJECT KEY IDENTIFIER","KEY USAGE","PRIVATEKEY USAGE PERIOD","SUBJECT ALT NAME2","ISSUER ALT NAME2","BASIC CONSTRAINTS2","CRL NUMBER","CRL REASON CODE","REASON CODE HOLD","DELTA CRL INDICATOR","ISSUING DIST POINT","NAME CONSTRAINTS","CRL DIST POINTS","CERT POLICIES","ANY CERT POLICY","POLICY MAPPINGS","AUTHORITY KEY IDENTIFIER2","POLICY CONSTRAINTS","ENHANCED KEY USAGE","FRESHEST CRL","DOMAIN COMPONENT","PKCS 12 FRIENDLY NAME ATTR","PKCS 12 LOCAL KEY ID","CERT EXTENSIONS","NEXT UPDATE LOCATION","KP CTL USAGE SIGNING","KP TIME STAMP SIGNING","KP EFS","EFS RECOVERY","WHQL CRYPTO","NT5 CRYPTO","OEM WHQL CRYPTO","EMBEDDED NT CRYPTO","ROOT LIST SIGNER","KP QUALIFIED SUBORDINATION","KP KEY RECOVERY","KP DOCUMENT SIGNING","KP LIFETIME SIGNING","KP MOBILE DEVICE SOFTWARE","YESNO TRUST ATTR","REMOVE CERTIFICATE","CROSS CERT DIST POINTS","CTL","SORTED CTL","ANY APPLICATION POLICY","RENEWAL CERTIFICATE","ENROLLMENT NAME VALUE PAIR","ENROLLMENT CSP PROVIDER","OS VERSION","PKCS 12 KEY PROVIDER NAME ATTR","LOCAL MACHINE KEYSET","AUTO ENROLL CTL USAGE","ENROLL CERTTYPE EXTENSION","ENROLLMENT AGENT","KP SMARTCARD LOGON","CERT MANIFOLD","CERTSRV PREVIOUS CERT HASH","CRL VIRTUAL BASE","CRL NEXT PUBLISH","KP CA EXCHANGE","KP KEY RECOVERY AGENT","CERTIFICATE TEMPLATE","ENTERPRISE OID ROOT","RDN DUMMY SIGNER","APPLICATION CERT POLICIES","APPLICATION POLICY MAPPINGS","APPLICATION POLICY CONSTRAINTS","ARCHIVED KEY ATTR","CRL SELF CDP","REQUIRE CERT CHAIN POLICY","ARCHIVED KEY CERT HASH","ISSUED CERT HASH","DS EMAIL REPLICATION","REQUEST CLIENT INFO","ENCRYPTED KEY HASH","CERTSRV CROSSCA VERSION","KEYID RDN","PKIX","PKIX PE","AUTHORITY INFO ACCESS","PKIX POLICY QUALIFIER CPS","PKIX POLICY QUALIFIER USERNOTICE","PKIX KP","PKIX KP SERVER AUTH","PKIX KP CLIENT AUTH","PKIX KP CODE SIGNING","PKIX KP EMAIL PROTECTION","PKIX KP IPSEC END SYSTEM","PKIX KP IPSEC TUNNEL","PKIX KP IPSEC USER","PKIX KP TIMESTAMP SIGNING","PKIX NO SIGNATURE","CMC","CMC STATUS INFO","CMC IDENTIFICATION","CMC IDENTITY PROOF","CMC DATA RETURN","CMC TRANSACTION ID","CMC SENDER NONCE","CMC RECIPIENT NONCE","CMC ADD EXTENSIONS","CMC ENCRYPTED POP","CMC DECRYPTED POP","CMC LRA POP WITNESS","CMC GET CERT","CMC GET CRL","CMC REVOKE REQUEST","CMC REG INFO","CMC RESPONSE INFO","CMC QUERY PENDING","CMC ID POP LINK RANDOM","CMC ID POP LINK WITNESS","CT PKI DATA","CT PKI RESPONSE","PKIX ACC DESCR","PKIX OCSP","PKIX CA ISSUERS","IPSEC KP IKE INTERMEDIATE","NETSCAPE","NETSCAPE CERT EXTENSION","NETSCAPE CERT TYPE","NETSCAPE BASE URL","NETSCAPE REVOCATION URL","NETSCAPE CA REVOCATION URL","NETSCAPE CERT RENEWAL URL","NETSCAPE CA POLICY URL","NETSCAPE SSL SERVER NAME","NETSCAPE COMMENT","Microsoft OID. ","Authenticode ",
                            "SPC INDIRECT DATA OBJID","SPC STATEMENT TYPE OBJID ","SPC SP OPUS INFO OBJID ","SPC PE IMAGE DATA OBJID",
                            "SPC SP AGENCY INFO OBJID ","SPC MINIMAL CRITERIA OBJID ","SPC FINANCIAL CRITERIA OBJID ","SPC LINK OBJID ","SPC HASH INFO OBJID","SPC SIPINFO OBJID","SPC CERT EXTENSIONS OBJID","SPC RAW FILE DATA OBJID","SPC STRUCTURED STORAGE DATA OBJID","SPC JAVA CLASS DATA OBJID","SPC INDIVIDUAL SP KEY PURPOSE OBJID",
    "SPC COMMERCIAL SP KEY PURPOSE OBJID","SPC CAB DATA OBJID ","SPC GLUE RDN OBJID ",
    "CTL for Software Publishers Trusted CAs","TRUSTED CODESIGNING CA LIST","TRUSTED CLIENT AUTH CA LIST","TRUSTED SERVER AUTH CA LIST","Time Stamping. ","SPC TIME STAMP REQUEST OBJID ","Permissions ","Crypto 2.0 ","CTL","SORTED CTL ","NEXT UPDATE LOCATION ","KP CTL USAGE SIGNING ","KP TIME STAMP SIGNING","SERVER GATED CRYPTO","SERIALIZED ","EFS CRYPTO ","EFS RECOVERY ","WHQL CRYPTO","NT5 CRYPTO ","OEM WHQL CRYPTO","EMBEDDED NT CRYPTO ","ROOT LIST SIGNER ","KP QUALIFIED SUBORDINATION ","KP KEY RECOVERY","KP DOCUMENT SIGNING","YESNO TRUST ATTR ","DRM","DRM INDIVIDUALIZATION","LICENSES ","LICENSE SERVER ","MICROSOFT RDN PREFIX ","KEYID RDN","REMOVE CERTIFICATE ","CROSS CERT DIST POINTS "," Microsoft CMC OIDs","CMC ADD ATTRIBUTES "," Microsoft certificate property OIDs ","CERT PROP ID PREFIX"," CryptUI ","ANY APPLICATION POLICY ","Catalog. ","CATALOG LIST ","CATALOG LIST MEMBER","CAT NAMEVALUE OBJID","CAT MEMBERINFO OBJID ","Microsoft PKCS10 OIDs. ",
    "RENEWAL CERTIFICATE","ENROLLMENT NAME VALUE PAIR ","ENROLLMENT CSP PROVIDER","Microsoft Java ","Microsoft Outlook/Exchange ","Outlook Express","Microsoft PKCS12 attributes. ","LOCAL MACHINE KEYSET ",
    "Microsoft Hydra. ","Microsoft ISPU Test ","Microsoft Enrollment Infrastructure","AUTO ENROLL CTL USAGE","ENROLL CERTTYPE EXTENSION","ENROLLMENT AGENT ","KP SMARTCARD LOGON ","NT PRINCIPAL NAME","CERT MANIFOLD","Microsoft CertSrv Infrastructure ","CERTSRV CA VERSION ","Microsoft Directory Service",
    "NTDS REPLICATION ","IIS ","Windows updates and service packs ","PRODUCT UPDATE ","Fonts. ",
    "Microsoft Licensing and Registration ","Microsoft Corporate PKI (ITG) ","CAPICOM. "};
                        #endregion
                        if (asnTag.IsContextSpecific)
                        {
                            return Encoding.Default.GetString(data);
                        }
                        else
                        {
                             int index = Array.IndexOf(soid_codes, oid);
                             string s = soi[index];
                        //    string s2 = SOID_KEYS.FindKey(oid);
                            return s; /*oid + " " +  soi[index]*/;
                        }
                    case ASN_TAG_TYPE.BOOLEAN:
                    case ASN_TAG_TYPE.INTEGER:
                        string txt = "";
                        if (data != null)
                            for (int i = 0; i < data.Length; i++)
                                txt += data[i].ToString("x2") + " ";
                        return txt;
                     default:
                        string text = "";
                        if (data != null)
                            for (int i = 0; i < data.Length; i++)
                                text += data[i].ToString("x2") + " ";
                        return text;
                }
            }
        }
        #endregion
        public static List<string> oids = new List<string>();
        public ASN1_OBJECT(BinaryFileReader sw, long offset)
        {
            SOID_KEYS.SetKeys();
            PositionOfStructureInFile = sw.Position;
            this.offset = sw.Position + offset;
            tag = (byte)sw.ReadByte();
            asnTag = new ASN1Tag(tag);
            length = 1;
            if ((ASN_TAG_TYPE)tag != ASN_TAG_TYPE.NULL_ID)
            {
                length = FindLength(sw);
            }
            if (length == 0)
                length = 1;
            #region Parse userStrings
            if (asnTag.IsConstructed)
            {
                #region The node has children // bit 6 of tag is 1
                nodes = new List<ASN1_OBJECT>();
                long end = sw.Position + length;
                while (sw.Position < end)
                {
                    nodes.Add(new ASN1_OBJECT(sw, offset));
                }
                #endregion
            }
            else
            {
                #region Pure userStrings node //bit 6 is tag 0
                int start = 0;
                data = new byte[length];
                if (length <= sw.Length - sw.Position)
                    sw.Read(data, 0, data.Length);
                switch (asnTag.TagType)
                {
                    case ASN_TAG_TYPE.BIT_STRING:
                    case ASN_TAG_TYPE.OCTET_STRING:
                        #region Parse bit or octet string
                        if (length > 1)
                        {

                            if ((ASN_TAG_TYPE)tag == ASN_TAG_TYPE.BIT_STRING)
                            {
                                start = 1;// Byte 1 unused in BIT_STRING
                            }
                            Buffer.BlockCopy(data, start, data, 0, data.Length - start);
                            if ((data[0] == 0x30) || (data[0] == 0x03))
                            {
                                try
                                {
                                    ASN1_OBJECT node = new ASN1_OBJECT(data, this.offset, data.Length - start);
                                    if (nodes == null)
                                        nodes = new List<ASN1_OBJECT>();
                                    nodes.Add(node);
                                }
                                catch { }
                            }
                        }
                        #endregion
                        break;
                    case ASN_TAG_TYPE.OBJECT_ID:
                        #region Parse object
                        if (!AsnTag.IsContextSpecific)
                        {
                            try
                            {
                                oid = DecodeOID(data);
                                ASN1_OBJECT.oids.Add(oid);
                                string OID = SOID_KEYS.FindKey(oid);
                                if (oid.Contains("2.24.116.116.112.58.47.47.99.114.108.46"))
                                {
                                }
                                switch (OID)
                                {
                                    case "AUTHORITY_INFO_ACCESS":
                                        break;
                                    case "Authority Key Identifier"://2.5.29.35 
                                        break;
                                }
                            }
                            catch { }
                        }
                        break;
                        #endregion
                }
                #endregion
            }
            #endregion
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public ASN1_OBJECT(byte[] buffer, long offset, int length)
        {
            SOID_KEYS.SetKeys();
            this.offset = offset;
            this.length = length;
            // Reads root node
            MemoryStream ms = new MemoryStream(buffer);
            tag = (byte) ms.ReadByte();
            asnTag = new ASN1Tag(tag);
            if (tag == 0)
                return;
            int nodeLength = FindLength(ms);
            nodes = new List<ASN1_OBJECT>();
            while (ms.Position < buffer.Length)
            {
                nodes.Add(new ASN1_OBJECT(ms, this.offset));
            }
            ms.Close();
        }
        public ASN1_OBJECT(MemoryStream sw, long offset)
        {
            this.offset = sw.Position + offset;
            tag = (byte)sw.ReadByte();
            asnTag = new ASN1Tag(tag);
            length = 1;
            if ((ASN_TAG_TYPE)tag != ASN_TAG_TYPE.NULL_ID)
            {
                length = FindLength(sw);
            }
            if (length == 0)
                length = 1;
            #region Parse userStrings
            if (asnTag.IsConstructed)
            {
                #region The node has children // bit 6 of tag is 1
                nodes = new List<ASN1_OBJECT>();
                long end = sw.Position + length;
                while (sw.Position < end)
                {
                    nodes.Add(new ASN1_OBJECT(sw, offset));
                }
                #endregion
            }
            else
            {
                #region Pure userStrings node //bit 6 is tag 0
                int start = 0;
                data = new byte[length];
                if (length <= sw.Length - sw.Position)
                    sw.Read(data, 0, data.Length);
                switch (asnTag.TagType)
                {
                    case ASN_TAG_TYPE.BIT_STRING:
                    case ASN_TAG_TYPE.OCTET_STRING:
                        #region Parse bit or octet string
                        if (length > 1)
                        {

                            if ((ASN_TAG_TYPE)tag == ASN_TAG_TYPE.BIT_STRING)
                            {
                                start = 1;// Byte 1 unused in BIT_STRING
                            }
                            Buffer.BlockCopy(data, start, data, 0, data.Length - start);
                            if ((data[0] == 0x30) || (data[0] == 0x03))
                            {
                                try
                                {
                                    ASN1_OBJECT node = new ASN1_OBJECT(data, this.offset, data.Length - start);
                                    if (nodes == null)
                                        nodes = new List<ASN1_OBJECT>();
                                    nodes.Add(node);
                                }
                                catch { }
                            }
                        }
                        #endregion
                        break;
                    case ASN_TAG_TYPE.OBJECT_ID:
                        if (!AsnTag.IsContextSpecific)
                        {
                            try
                            {
                                oid = DecodeOID(data);
                                ASN1_OBJECT.oids.Add(oid);
                                string OID = SOID_KEYS.FindKey(oid);
                                if (oid.Contains("2.24.116.116.112.58.47.47.99.114.108.46"))
                                {
                                }
                                switch (OID)
                                {
                                    case "AUTHORITY_INFO_ACCESS":
                                        break;
                                    case "Authority Key Identifier"://2.5.29.35 
                                        break;
                                }
                            }
                            catch { }
                        }
                        break;
                }
                #endregion
            }
            #endregion
        }
        public override string ToString()
        {
            return Object_type.ToString() + " Offset " + offset.ToString("x4") + " " + DataString;
        }
    }
    public class CERTIFICATE_TABLE: IMAGE_BASE_DATA
    {
        private int dwLength;
        private short wCertificateType;
        private short wRevision;//The only defined certificate revision is WIN_CERT_REVISION_1_0 (0x0100)
        private CERTIFICATE_TYPE certificate_type;
        private ASN1_OBJECT certificate;
        #region Properties
        public string Certificate_Revision
        {
            get { return ((WIN_CERTIFICATE_REVISION)wRevision).ToString(); }
        }
        public int Length
        {
            get { return dwLength; }
            set { dwLength = value; }
        }
        public CERTIFICATE_TYPE Certificate_type
        {
            get { return certificate_type; }
            set { certificate_type = value; }
        }
        public ASN1_OBJECT Certificate
        {
            get { return certificate; }
            set { certificate = value; }
        }
        #endregion
        public CERTIFICATE_TABLE(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            dwLength = sw.ReadInteger();
            wRevision = sw.ReadShort();
            wCertificateType = sw.ReadShort();
            certificate_type = (CERTIFICATE_TYPE)wCertificateType;
            LengthInFile = sw.Position - PositionOfStructureInFile;
            switch (certificate_type)
            {
                case CERTIFICATE_TYPE.WIN_CERT_TYPE_X509:
                    break;
                case CERTIFICATE_TYPE.WIN_CERT_TYPE_PKCS_SIGNED_DATA:
                    byte[] buffer = sw.ReadBytes(dwLength);
                    sw.Position = PositionOfStructureInFile + 8;
            //        certificate = new ASN1_OBJECT(buffer, sw.Position, dwLength);
                    certificate = new ASN1_OBJECT(sw, dwLength);
                    break;
                case CERTIFICATE_TYPE.WIN_CERT_TYPE_PKCS1_SIGN:
                    break;
            }
        }
    }
    #endregion
}