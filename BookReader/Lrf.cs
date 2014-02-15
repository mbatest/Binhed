// LRFFileReader.cs
//
// Copyright (C) 2004-2005  Peter Knowles
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330,
// Boston, MA 02111-1307, USA.

using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Drawing;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Utils;

//using System.IO.Compression;

namespace BookReader
{
    public class LRFHeader : LOCALIZED_DATA
    {
        #region Private members
        private ELEMENTARY_TYPE signature;// = 0x00;
        private ELEMENTARY_TYPE version;// = 0x08;
        private ELEMENTARY_TYPE pseudoEncryption;//= 0x0A;
        private ELEMENTARY_TYPE rootObjectID;// = 0x0C;
        private ELEMENTARY_TYPE numObjects;// = 0x10;
        private ELEMENTARY_TYPE objectIndexOffset;// = 0x18;
        private ELEMENTARY_TYPE unknown1;// = 0x20;
        private ELEMENTARY_TYPE flags;// = 0x24;
        private ELEMENTARY_TYPE unknown2;// = 0x25;
        private ELEMENTARY_TYPE unknown3;// = 0x26;
        private ELEMENTARY_TYPE unknown4;// = 0x28;
        private ELEMENTARY_TYPE height;// = 0x2A;
        private ELEMENTARY_TYPE width;// = 0x2C;
        private ELEMENTARY_TYPE unknown5;// = 0x2E;
        private ELEMENTARY_TYPE unknown6;// = 0x2F;
        private ELEMENTARY_TYPE unknown7;// = 0x30;
        private ELEMENTARY_TYPE tocObjectID;// = 0x44;
        private ELEMENTARY_TYPE tocObjectOffset;// = 0x48;
        private ELEMENTARY_TYPE xmlCompSize;// = 0x4C;
        private ELEMENTARY_TYPE thumbImageType;// = 0x4E;
        private ELEMENTARY_TYPE gifSize;// = 0x50;
        private ELEMENTARY_TYPE unComXmlSize;// = 0x54;
        #endregion
        #region Properties
        public ELEMENTARY_TYPE Signature
        {
            get { return signature; }
            set { signature = value; }
        }
        /// <summary>
        /// Offset of Header Version
        /// </summary>
        public ELEMENTARY_TYPE Version
        {
            get { return version; }
            set { version = value; }
        }
        /// <summary>
        /// Offset of Header PseudoEncryption
        /// </summary>
        public ELEMENTARY_TYPE PseudoEncryption
        {
            get { return pseudoEncryption; }
            set { pseudoEncryption = value; }
        }
        /// <summary>
        /// Offset of Header RootObjectID
        /// </summary>
        public ELEMENTARY_TYPE RootObjectID
        {
            get { return rootObjectID; }
            set { rootObjectID = value; }
        }
        /// <summary>
        /// Offset of Header NumObjects
        /// </summary>
        public ELEMENTARY_TYPE NumObjects
        {
            get { return numObjects; }
            set { numObjects = value; }
        }
        /// <summary>
        /// Offset of Header ObjectIndexOffset
        /// </summary>
        public ELEMENTARY_TYPE ObjectIndexOffset
        {
            get { return objectIndexOffset; }
            set { objectIndexOffset = value; }
        }
        /// <summary>
        /// Offset of Header Unknown1
        /// </summary>
        public ELEMENTARY_TYPE Unknown1
        {
            get { return unknown1; }
            set { unknown1 = value; }
        }
        /// <summary>
        /// Offset of Header Flags
        /// </summary>
        public ELEMENTARY_TYPE Flags
        {
            get { return flags; }
            set { flags = value; }
        }
        /// <summary>
        /// Offset of Header Unknown2
        /// </summary>
        public ELEMENTARY_TYPE Unknown2
        {
            get { return unknown2; }
            set { unknown2 = value; }
        }
        /// <summary>
        /// Offset of Header Unknown3
        /// </summary>
        public ELEMENTARY_TYPE Unknown3
        {
            get { return unknown3; }
            set { unknown3 = value; }
        }
        /// <summary>
        /// Offset of Header Unknown4
        /// </summary>
        public ELEMENTARY_TYPE Unknown4
        {
            get { return unknown4; }
            set { unknown4 = value; }
        }
        /// <summary>
        /// Offset of Header Height
        /// </summary>
        public ELEMENTARY_TYPE Height
        {
            get { return height; }
            set { height = value; }
        }
        /// <summary>
        /// Offset of Header Width
        /// </summary>
        public ELEMENTARY_TYPE Width
        {
            get { return width; }
            set { width = value; }
        }
        /// <summary>
        /// Offset of Header Unknown5
        /// </summary>
        public ELEMENTARY_TYPE Unknown5
        {
            get { return unknown5; }
            set { unknown5 = value; }
        }
        /// <summary>
        /// Offset of Header Unknown6
        /// </summary>
        public ELEMENTARY_TYPE Unknown6
        {
            get { return unknown6; }
            set { unknown6 = value; }
        }
        /// <summary>
        /// Offset of Header Unknown7
        /// </summary>
        public ELEMENTARY_TYPE Unknown7
        {
            get { return unknown7; }
            set { unknown7 = value; }
        }
        /// <summary>
        /// Offset of Header TOCObjectID
        /// </summary>
        public ELEMENTARY_TYPE TocObjectID
        {
            get { return tocObjectID; }
            set { tocObjectID = value; }
        }
        /// <summary>
        /// Offset of Header TOCObjectOffset
        /// </summary>
        public ELEMENTARY_TYPE TocObjectOffset
        {
            get { return tocObjectOffset; }
            set { tocObjectOffset = value; }
        }
        /// <summary>
        /// Offset of Header XMLCompSize
        /// </summary>
        public ELEMENTARY_TYPE XmlCompSize
        {
            get { return xmlCompSize; }
            set { xmlCompSize = value; }
        }
        /// <summary>
        /// Offset of Hearder Thumbnail Image Type
        /// </summary>
        public ELEMENTARY_TYPE ThumbImageType
        {
            get { return thumbImageType; }
            set { thumbImageType = value; }
        }
        /// <summary>
        /// Offset of Header GIFSize
        /// </summary>
        public ELEMENTARY_TYPE GifSize
        {
            get { return gifSize; }
            set { gifSize = value; }
        }
        /// <summary>
        /// Offset of length String of uncompressed info block in LRF file
        /// </summary>
        public ELEMENTARY_TYPE UnComXmlSize
        {
            get { return unComXmlSize; }
            set { unComXmlSize = value; }
        }
        /// <summary>
        /// Path of LRF file
        /// </summary>
        public byte[] bytesLRFObjectData;
        /// <summary>
        /// The uncompressed UTF-16 info block
        /// </summary>
        public byte[] bytesXMLInfoBlock;
        /// <summary>
        /// The compressed UTF-16 info block
        /// </summary>
        public byte[] bytesCompXMLInfoBlock;
        /// <summary>
        /// The GIF file data for the thumnail image
        /// </summary>
        public int originalLRFObjectOffset;
        #endregion
        public LRFHeader(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            signature = new ELEMENTARY_TYPE(sw, 0, Encoding.Unicode);
            // Extract the version 
            version = new ELEMENTARY_TYPE(sw, 0, typeof(short));// sw.ReadShort(); 
            // Extract the PsuedoEncryption key 
            pseudoEncryption = new ELEMENTARY_TYPE(sw, 0, typeof(short));//sw.ReadShort(); //BitConverter.ToInt16(lrfBytes, LRF_OFFSET_HEADER_PSEUDOENCRYPTION);
            // Extract the RootObjectID 
            rootObjectID = new ELEMENTARY_TYPE(sw, 0, typeof(int));//sw.ReadInteger(); //BitConverter.ToInt32(lrfBytes, LRF_OFFSET_HEADER_ROOTOBJECTID);
            // Extract the NumberOfObjects 
            numObjects = new ELEMENTARY_TYPE(sw, 0, typeof(long));//sw.ReadLong(); //BitConverter.ToInt64(lrfBytes, LRF_OFFSET_HEADER_NUMOBJECTS);
            // Extract the ObjectIndexOffset 
            objectIndexOffset = new ELEMENTARY_TYPE(sw, 0, typeof(long));//sw.ReadLong(); //BitConverter.ToInt64(lrfBytes, LRF_OFFSET_HEADER_OBJECTINDEXOFFSET);
            // Extract Unknown1 
            unknown1 = new ELEMENTARY_TYPE(sw, 0, typeof(int));//sw.ReadInteger(); //BitConverter.ToInt32(lrfBytes, LRF_OFFSET_HEADER_UNKNOWN1);
            // Extract flags 
            flags = new ELEMENTARY_TYPE(sw, 0, typeof(byte));//sw.ReadByte(); //lrfBytes[LRF_OFFSET_HEADER_FLAGS];
            // Extract Unknown2 
            unknown2 = new ELEMENTARY_TYPE(sw, 0, typeof(byte));//sw.ReadByte(); //lrfBytes[LRF_OFFSET_HEADER_UNKNOWN2];
            // Extract Unknown3 
            unknown3 = new ELEMENTARY_TYPE(sw, 0, typeof(short));//sw.ReadShort();//BitConverter.ToInt16(lrfBytes, LRF_OFFSET_HEADER_UNKNOWN3);
            // Extract Unknown4 
            unknown4 = new ELEMENTARY_TYPE(sw, 0, typeof(short));//sw.ReadShort();//BitConverter.ToInt16(lrfBytes, LRF_OFFSET_HEADER_UNKNOWN4);
            // Extract Height 
            height = new ELEMENTARY_TYPE(sw, 0, typeof(short));//sw.ReadShort();//BitConverter.ToInt16(lrfBytes, LRF_OFFSET_HEADER_HEIGHT);
            // Extract Width 
            width = new ELEMENTARY_TYPE(sw, 0, typeof(short));//sw.ReadShort();//BitConverter.ToInt16(lrfBytes, LRF_OFFSET_HEADER_WIDTH);
            // Extract Unknown5 
            unknown5 = new ELEMENTARY_TYPE(sw, 0, typeof(byte));//sw.ReadByte(); //lrfBytes[LRF_OFFSET_HEADER_UNKNOWN5];
            // Extract Unknown6 
            unknown6 = new ELEMENTARY_TYPE(sw, 0, typeof(byte));//sw.ReadByte(); //lrfBytes[LRF_OFFSET_HEADER_UNKNOWN6];
            // Extract Unknown7 
            unknown7 = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 20);//sw.ReadBytes(20); //new Byte[20];
            // Buffer.BlockCopy(lrfBytes, LRF_OFFSET_HEADER_UNKNOWN7, unknown7, 0, 20);
            // Extract the TOCObjectID 
            tocObjectID = new ELEMENTARY_TYPE(sw, 0, typeof(int)); //sw.ReadInteger(); //BitConverter.ToInt32(lrfBytes, LRF_OFFSET_HEADER_TOCOBJECTID);
            // Extract the tocObjectOffset 
            tocObjectOffset = new ELEMENTARY_TYPE(sw, 0, typeof(int));// sw.ReadInteger(); //BitConverter.ToInt32(lrfBytes, LRF_OFFSET_HEADER_TOCOBJECTOFFSET);
            // Extract the xmlCompSize 
            xmlCompSize = new ELEMENTARY_TYPE(sw, 0, typeof(short));//sw.ReadShort();//BitConverter.ToInt16(lrfBytes, LRF_OFFSET_HEADER_COMXMLSIZE);
            // Extract Thumbnail Image Type 
            thumbImageType = new ELEMENTARY_TYPE(sw, 0, typeof(short));//sw.ReadShort();//BitConverter.ToInt16(lrfBytes, LRF_OFFSET_HEADER_THUMBIMAGETYPE);
            // Extract the gifSize 
            gifSize = new ELEMENTARY_TYPE(sw, 0, typeof(int));//sw.ReadInteger(); //BitConverter.ToInt32(lrfBytes, LRF_OFFSET_HEADER_GIFSIZE);
            unComXmlSize = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class LrfObject : LOCALIZED_DATA
    {
        #region Private members
        private ELEMENTARY_TYPE id;
        private ELEMENTARY_TYPE offset;
        private ELEMENTARY_TYPE size;
        private ELEMENTARY_TYPE reserved;
        private List<LRFTag> lrfTags;
        private ELEMENTARY_TYPE bufferE;
        private LRFTag baseTag;
        private LrfObjectType objType;
        private ELEMENTARY_TYPE streamSize;
        private List<LrfObject> sons;
        private ELEMENTARY_TYPE text;
        private ELEMENTARY_TYPE numberOfPages;
        private List<ELEMENTARY_TYPE> pageNumbers;
        private List<ELEMENTARY_TYPE> tagAddres;
        private string pageText;
        private Image image;
        private List<LRFTag> others = new List<LRFTag>();
        private LrfObject attribute;
        TOC tc;
        List<string> tags;
        List<Text_Tag> text_Tags;
        byte[] buffer;
        #endregion
        #region Basic data
        public ELEMENTARY_TYPE Id
        {
            get { return id; }
            set { id = value; }
        }
        public ELEMENTARY_TYPE Offset
        {
            get { return offset; }
            set { offset = value; }
        }
        public ELEMENTARY_TYPE Size
        {
            get { return size; }
            set { size = value; }
        }
        public ELEMENTARY_TYPE Reserved
        {
            get { return reserved; }
            set { reserved = value; }
        }
        public LrfObjectType ObjType
        {
            get { return objType; }
            set { objType = value; }
        }
        #endregion
        #region Parsed data
        public LrfObject Attribute
        {
            get { return attribute; }
            set { attribute = value; }
        }
        public ELEMENTARY_TYPE StreamSize
        {
            get { return streamSize; }
            set { streamSize = value; }
        }
        public ELEMENTARY_TYPE Text
        {
            get { return text; }
            set { text = value; }
        }
        public List<LrfObject> Sons
        {
            get { return sons; }
            set { sons = value; }
        }
        public List<LRFTag> Others
        {
            get { return others; }
            set { others = value; }
        }
        public TOC Table_Of_Contents
        {
            get { return tc; }
            set { tc = value; }
        }
        public List<LRFTag> LrfTags
        {
            get { return lrfTags; }
            set { lrfTags = value; }
        }
        public List<Text_Tag> Text_Tags
        {
            get { return text_Tags; }
            set { text_Tags = value; }
        }
        #endregion
        #region Page tree
        public ELEMENTARY_TYPE NumberOfPages
        {
            get { return numberOfPages; }
            set { numberOfPages = value; }
        }
        public List<ELEMENTARY_TYPE> PageNumbers
        {
            get { return pageNumbers; }
            set { pageNumbers = value; }
        }
        public List<ELEMENTARY_TYPE> TagAddres
        {
            get { return tagAddres; }
            set { tagAddres = value; }
        }
        public string PageText
        {
            get { return pageText; }
            set { pageText = value; }
        }
        public Image Image
        {
            get { return image; }
            set { image = value; }
        }
        #endregion
        public LrfObject()
        {
        }
        public LrfObject(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            id = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(uint)sw.ReadInteger();
            offset = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(uint)sw.ReadInteger();
            size = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(uint)sw.ReadInteger();
            reserved = new ELEMENTARY_TYPE(sw, 0, typeof(int));//(uint)sw.ReadInteger();
            int pos = sw.Position;
            sw.Position = (int)offset.Value;
            bufferE = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), (int)size.Value);//new byte[size];
            lrfTags = new List<LRFTag>();
            sw.Position = (int)offset.Value;
            baseTag = new LRFTag(sw, 0, 0, 0);
            lrfTags.Add(baseTag);
            objType = (LrfObjectType)(short)baseTag.ObType.Value;
            while (sw.Position < (int)offset.Value + (int)size.Value)
            {
                int siz = 0;
                if (StreamSize != null)
                    siz = (int)StreamSize.Value;
                LRFTag onj = new LRFTag(sw, 0, (short)baseTag.ObType.Value, siz);
                lrfTags.Add(onj);
                if (onj.StreamSize != null)
                    StreamSize = onj.StreamSize;
            }
            switch (objType)
            {
                case LrfObjectType.Toc:
                    //LrfTags[3].Data.Value
                    string s = Encoding.Unicode.GetString((byte[])LrfTags[3].Data.Value);
                    BitStreamReader ms = new BitStreamReader((byte[])LrfTags[3].Data.Value, false);
                    tc = new TOC(ms, LrfTags[3].PositionOfStructureInFile + 2);
                    break;
                case LrfObjectType.Text:
                    #region Decompress and parse text
                    Text = new ELEMENTARY_TYPE(Uncompress(), LrfTags[4].PositionOfStructureInFile, LrfTags[4].LengthInFile);
                    #endregion
                    break;
                case LrfObjectType.Block:
                    ParseLinks(4);
                    break;
                case LrfObjectType.Page:
                    ParseLinks(6);
                    break;
                case LrfObjectType.Canvas:
                    /*
                     *                                     Buffer.BlockCopy((byte[])ob.LrfTags[2].Data.Value, 0, buffer, 0, 2);
                                    c.Height = (int)ReadShortUint(buffer);
                                    Buffer.BlockCopy((byte[])ob.LrfTags[6].Data.Value, 0, buffer, 0, 2);
                                    c.Height = (int)ReadShortUint(buffer);
*/
                    break;
            }
            sw.Position = pos;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        private void ParseLinks(int streamIndex)
        {
            BitStreamReader ms = new BitStreamReader((byte[])LrfTags[streamIndex].Data.Value, false);
            while (ms.Position < ms.Length)
            {
                others.Add(new LRFTag(ms, LrfTags[6].Data.PositionOfStructureInFile, 0, 0));
            }
        }
        public string Uncompress()
        {
            LRFTag lro = LrfTags[4];
            int len = ((byte[])LrfTags[4].Data.Value).Length;
            byte[] data = new byte[len - 4];
            Buffer.BlockCopy((byte[])LrfTags[4].Data.Value, 4, data, 0, len - 4);
            buffer = Decompress(data);
            int u = 0;
            string truc = Encoding.Unicode.GetString(buffer);
            string s = "";
            List<byte> txt = new List<byte>();
            tags = new List<string>();
            List<List<byte>> txts = new List<List<byte>>();
            BitStreamReader ms = new BitStreamReader(buffer, false);
            text_Tags = new List<Text_Tag>();
            while (ms.Position < ms.Length)
            {
                Text_Tag tt = new Text_Tag(ms, LrfTags[4].Data.PositionOfStructureInFile);
                text_Tags.Add(tt);
            }
            while (u < buffer.Length - 1)
            {
                #region Parse
                if (buffer[u + 1] == 0xf5)
                {
                    if (txt.Count > 0)
                    {
                        s = Encoding.Unicode.GetString(txt.ToArray());
                        tags.Add(s);
                        txts.Add(txt);
                        txt.Clear();
                    }

                    switch (buffer[u])
                    {
                        case 0x00:
                            s = "<P>";
                            break;
                        case 0x11:
                            s = "";//Font Size
                            u += 2;// Size ?
                            break;
                        case 0x12:
                        case 0x13:
                        case 0x14:
                        case 0x15://Font weight
                            s = "";
                            u += 2;// Weight ?
                            break;
                        case 0x17:
                        case 0x18:
                            s = "";//
                            u += 4;// Weight ?
                            break;
                        case 0x19://Word space
                        case 0x1A:
                        case 0x1B://Base Line SkipBit
                        case 0x1C:
                        case 0x1D:
                        case 0x1E:
                        case 0x25:
                        case 0x26:
                        case 0x31:
                        case 0x32:
                        case 0x33:
                            s = "";
                            u += 2;
                            break;
                        case 0x81:
                            s = "<I>";
                            break;
                        case 0x82:
                            s = "</I>";
                            break;
                        case 0xa1:
                        case 0xa2:
                            s = "<P>";
                            u += 4;
                            break;
                        case 0xa7:
                            s = "<a href=\"\">";//Begin button ?
                            //link reference
                            u += 4;
                            break;
                        case 0xa8:
                            s = "</a>";//end button
                            break;
                        case 0xb7:
                            s = "<sup>";//Begin sup
                            break;
                        case 0xb8:
                            s = "</sup>";//End sup
                            break;
                        case 0x16://Font Face name
                        case 0xd1:
                            s = "";
                            while (buffer[u + 3] != 0xF5)
                            {
                                u++;
                                s += "_";
                            }
                            break;
                        case 0xd2:
                            s = "</P>";
                            break;
                        case 0xcc:
                            s = "";//<a href
                            u += 2;
                            break;
                        default:
                            s = "";
                            break;
                    }
                    if (s != "")
                        tags.Add(s);
                    s = "";
                    u += 2;
                }
                else
                {
                    //             if ((buffer[inTime] != 0x00) || (buffer[inTime + 1] != 0x00))
                    {
                        txt.Add(buffer[u]);
                        txt.Add(buffer[u + 1]);
                    }
                    u += 2;
                }
                #endregion
            }
            string t = "";
            if (s != "")
                tags.Add(s);
            foreach (Text_Tag tag in text_Tags)
                t += tag.Html + " ";
            return t;
        }
        public byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream();
            input.Write(data, 0, data.Length);
            input.Position = 0;
            InflaterInputStream gzip = new InflaterInputStream(input);
            MemoryStream output = new MemoryStream();
            byte[] buff = new byte[64];
            int read = -1;
            read = gzip.Read(buff, 0, Math.Min(buff.Length, data.Length));
            while (read > 0)
            {
                output.Write(buff, 0, read);
                read = gzip.Read(buff, 0, buff.Length);
            }
            gzip.Close();
            return output.ToArray();
        }
        public void AddSon(LrfObject obj)
        {
            if (sons == null)
                sons = new List<LrfObject>();
            sons.Add(obj);
        }
        public override string ToString()
        {
            string text = ((int)id.Value).ToString("x2") /*+ " " + ((int)size.Value).ToString() */+ " : ";
            switch ((short)baseTag.ObType.Value)
            {
                case 0x01: text = text + "Page Tree ";
                    break;
                case 0x02: text = text + "Page ";
                    break;
                case 0x03: text = text + "Header ";
                    break;
                case 0x04: text = text + "Footer ";
                    break;
                case 0x05: text = text + "Page Atr ";
                    break;
                case 0x06: text = text + "Block ";
                    break;
                case 0x07: text = text + "Block Atr ";
                    break;
                case 0x08: text = text + "Mini Page ";
                    break;
                case 0x09: text = text + "Block List ";
                    break;
                case 0x0A: text = text + "Text ";
                    if (Text != null)
                        if (((string)Text.Value).Length < 50)
                            text += (string)Text.Value;
                        else
                            text += ((string)Text.Value).Substring(0, 50);
                    break;
                case 0x0B: text = text + "Text Atr ";
                    break;
                case 0x0C: text = text + "Image ";
                    break;
                case 0x0D: text = text + "Canvas ";
                    break;
                case 0x0E: text = text + "Paragraph Atr ";
                    break;
                case 0x11: text = text + "Image Stream ";
                    break;
                case 0x12: text = text + "Import ";
                    break;
                case 0x13: text = text + "Button ";
                    break;
                case 0x14: text = text + "Window ";
                    break;
                case 0x15: text = text + "Pop Up Win ";
                    break;
                case 0x16: text = text + "Sound ";
                    break;
                case 0x17: text = text + "Plane Stream ";
                    break;
                case 0x19: text = text + "Font ";
                    break;
                case 0x1A: text = text + "Object Info ";
                    break;
                case 0x1C: text = text + "Book Atr ";
                    break;
                case 0x1D: text = text + "Simple Text ";
                    break;
                case 0x1E: text = text + "Toc ";
                    break;
                default: text = text + "";
                    break;
            }
            return text;
        }
    }
    public class Text_Tag : LOCALIZED_DATA
    {
        private ELEMENTARY_TYPE tag;
        private ELEMENTARY_TYPE data;
        private string html;
        private Image_Data image;

        public ELEMENTARY_TYPE Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        public ELEMENTARY_TYPE Data
        {
            get { return data; }
            set { data = value; }
        }
        public string Html
        {
            get { return html; }
            set { html = value; }
        }
        public Image_Data Image
        {
            get { return image; }
            set { image = value; }
        }
        public Text_Tag(BitStreamReader ms, long offset)
        {
            string txt = "";
            tag = new ELEMENTARY_TYPE(ms, offset, typeof(ushort));
            if (((ushort)tag.Value & 0xff00) == 0xf500)
            {
                #region Parse text tag
                switch ((ushort)tag.Value & 0x00ff)
                {
                    case 0x00:
                        break;
                    case 0x11://Font size
                        data = new ELEMENTARY_TYPE(ms, offset, typeof(short));// u += 2;// Size ?
                        html = "font-size";
                        html += ":" + ((short)data.Value).ToString();
                        break;
                    case 0x12:
                    case 0x13:
                    case 0x14:
                    case 0x15://Font weight
                        //       s = "";
                        data = new ELEMENTARY_TYPE(ms, offset, typeof(short));//u += 2;// Weight ?
                        html = "font-weight";
                        switch ((short)data.Value)
                        {
                            case 0x2bc:
                                html += ":bold";
                                break;
                            case 0x190:
                                html += ":normal";
                                break;
                            default:
                                break;
                        }
                        break;
                    case 0x16://Font Face name
                        /*           while (buffer[u + 3] != 0xF5)
                                   {
                                       u++;
                                       s += "_";
                                   }*/
                        break;
                    case 0x17:
                    case 0x18:
                        //     s = "";//
                        data = new ELEMENTARY_TYPE(ms, offset, typeof(int));//u += 4;// Weight ?
                        break;
                    case 0x19://Word space
                    case 0x1A:
                    case 0x1B://Base Line SkipBit
                    case 0x1C:
                    case 0x1D:
                    case 0x1E:
                    case 0x25:
                    case 0x26:
                    case 0x31:
                    case 0x32:
                    case 0x33:
                        data = new ELEMENTARY_TYPE(ms, offset, typeof(short));//u += 2;
                        break;
                    case 0x81:
                        html = "<I>";
                        break;
                    case 0x82:
                        html = "</I>";
                        break;
                    case 0xa1:
                        data = new ELEMENTARY_TYPE(ms, offset, typeof(int));
                        html = "<P>";
                        break;
                    case 0xa2:
                        html = "</P>";
                        break;
                    case 0xa7:
                        data = new ELEMENTARY_TYPE(ms, offset, typeof(int));// u += 4;
                        html = "<a href=\"" + ((int)data.Value).ToString("x8") + "\">";
                        break;
                    case 0xa8:
                        html = "</a>";//end button
                        break;
                    case 0xb7:
                        html = "<sup>";//Begin sup
                        break;
                    case 0xb8:
                        html = "</sup>";//End sup
                        break;
                    case 0xcc:
                        int length = ms.ReadShort();
                        data = new ELEMENTARY_TYPE(ms, offset, Encoding.Unicode, length / 2);
                        html = (string)data.Value;
                        break;
                    case 0xd1:
                        //         s = "";
                        image = new Image_Data(ms, offset);
                        html = "<img src= \"" + ((int)image.Image_reference.Value).ToString("x8") + "\"/>";
                        break;
                    case 0xd2:
                        //         s = "</P>";
                        break;
                    default:
                        //     s = "";
                        break;
                }
                #endregion
            }
            else
            {
                txt += Encoding.Unicode.GetString(BitConverter.GetBytes((ushort)tag.Value));
            }
        }
        public override string ToString()
        {
            string t = ((ushort)tag.Value).ToString("x2");
            switch ((ushort)tag.Value & 0x00ff)
            {
                case 0xcc:
                    t += " : " + (string)data.Value;
                    break;
                default:
                    t += " : " + LRFFileReader.TagTypes[(ushort)tag.Value & 0x00ff].name;
                    break;
            }
            return t;
        }
    }
    public class Image_Data : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE unk1;
        ELEMENTARY_TYPE image_reference;
        ELEMENTARY_TYPE unk2;

        public ELEMENTARY_TYPE Unk1
        {
            get { return unk1; }
            set { unk1 = value; }
        }
        public ELEMENTARY_TYPE Image_reference
        {
            get { return image_reference; }
            set { image_reference = value; }
        }
        public ELEMENTARY_TYPE Unk2
        {
            get { return unk2; }
            set { unk2 = value; }
        }
        public Image_Data(BitStreamReader ms, long offset)
        {
            unk1 = new ELEMENTARY_TYPE(ms, offset, typeof(int));
            image_reference = new ELEMENTARY_TYPE(ms, offset, typeof(int));
            unk2 = new ELEMENTARY_TYPE(ms, offset, typeof(int));
        }
    }
    #region Table of contents
    public class TOC : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE nbEntries;
        List<ELEMENTARY_TYPE> entries_offset;
        List<TOC_Entry> entries;
        ELEMENTARY_TYPE page_destination;
        ELEMENTARY_TYPE bloc_destination;

        public ELEMENTARY_TYPE NbEntries
        {
            get { return nbEntries; }
            set { nbEntries = value; }
        }
        public List<ELEMENTARY_TYPE> Entry_Offsets
        {
            get { return entries_offset; }
            set { entries_offset = value; }
        }
        public ELEMENTARY_TYPE Page_destination
        {
            get { return page_destination; }
            set { page_destination = value; }
        }
        public ELEMENTARY_TYPE Bloc_destination
        {
            get { return bloc_destination; }
            set { bloc_destination = value; }
        }
        public List<TOC_Entry> Entries
        {
            get { return entries; }
            set { entries = value; }
        }
        public TOC(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            entries_offset = new List<ELEMENTARY_TYPE>();
            entries = new List<TOC_Entry>();
            while (sw.Position < sw.Length)
            {
                nbEntries = new ELEMENTARY_TYPE(sw, offset, typeof(int));
                for (int w = 0; w < (int)nbEntries.Value; w++)
                {
                    entries_offset.Add(new ELEMENTARY_TYPE(sw, offset, typeof(uint)));
                }
                page_destination = new ELEMENTARY_TYPE(sw, offset, typeof(uint));
                bloc_destination = new ELEMENTARY_TYPE(sw, offset, typeof(uint));
                for (int w = 0; w < (int)nbEntries.Value; w++)
                {
                    TOC_Entry entry = new TOC_Entry(sw, offset + 2);
                    entries.Add(entry);
                }
            }
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }
    public class TOC_Entry : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE strLength;

        public ELEMENTARY_TYPE StrLength
        {
            get { return strLength; }
            set { strLength = value; }
        }
        ELEMENTARY_TYPE title;

        public ELEMENTARY_TYPE Title
        {
            get { return title; }
            set { title = value; }
        }
        ELEMENTARY_TYPE page_destination;

        public ELEMENTARY_TYPE Page_destination
        {
            get { return page_destination; }
            set { page_destination = value; }
        }
        ELEMENTARY_TYPE bloc_destination;

        public ELEMENTARY_TYPE Bloc_destination
        {
            get { return bloc_destination; }
            set { bloc_destination = value; }
        }
        public TOC_Entry(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            strLength = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));
            title = new ELEMENTARY_TYPE(sw, offset, Encoding.Unicode, (ushort)strLength.Value / 2);
            if (sw.Position >= sw.Length)
                return;
            page_destination = new ELEMENTARY_TYPE(sw, offset, typeof(uint));
            bloc_destination = new ELEMENTARY_TYPE(sw, offset, typeof(uint));
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return (string)title.Value;
        }
    }
    #endregion
    public class LRFTag : LOCALIZED_DATA
    {
        #region Private members
        private ELEMENTARY_TYPE f500;
        private ELEMENTARY_TYPE data;
        private TagType type;
        private ELEMENTARY_TYPE obType;
        private ELEMENTARY_TYPE streamSize;
        private List<ELEMENTARY_TYPE> content_Ids;
        private ELEMENTARY_TYPE ids_Number;
        private ELEMENTARY_TYPE image_width;
        private ELEMENTARY_TYPE image_Height;
        private ELEMENTARY_TYPE image_Size;
        private ELEMENTARY_TYPE imageStream_Id;

        private ELEMENTARY_TYPE font;
        private ELEMENTARY_TYPE fontLength;
        private ELEMENTARY_TYPE page_Destination;
        private ELEMENTARY_TYPE bloc_Destination;
        #endregion
        #region Basic properties
        public ELEMENTARY_TYPE F500
        {
            get { return f500; }
            set { f500 = value; }
        }
        public ELEMENTARY_TYPE ObType
        {
            get { return obType; }
            set { obType = value; }
        }
        public ELEMENTARY_TYPE StreamSize
        {
            get { return streamSize; }
            set { streamSize = value; }
        }
        public ELEMENTARY_TYPE Data
        {
            get { return data; }
            set { data = value; }
        }
        public TagType TagType
        {
            get { return type; }
            set { type = value; }
        }
        #endregion
        #region List
        public ELEMENTARY_TYPE Ids_Number
        {
            get { return ids_Number; }
            set { ids_Number = value; }
        }
        public List<ELEMENTARY_TYPE> Content_Ids
        {
            get { return content_Ids; }
            set { content_Ids = value; }
        }
        #endregion
        #region Link
        public ELEMENTARY_TYPE Page_Destination
        {
            get { return page_Destination; }
            set { page_Destination = value; }
        }
        public ELEMENTARY_TYPE Bloc_Destination
        {
            get { return bloc_Destination; }
            set { bloc_Destination = value; }
        }
        #endregion
        #region Image data
        public ELEMENTARY_TYPE Image_width
        {
            get { return image_width; }
            set { image_width = value; }
        }
        public ELEMENTARY_TYPE Image_Height
        {
            get { return image_Height; }
            set { image_Height = value; }
        }
        public ELEMENTARY_TYPE Image_Size
        {
            get { return image_Size; }
            set { image_Size = value; }
        }
        public ELEMENTARY_TYPE Image_Id
        {
            get { return imageStream_Id; }
            set { imageStream_Id = value; }
        }
        #endregion
        #region Font
        public ELEMENTARY_TYPE FontLength
        {
            get { return fontLength; }
            set { fontLength = value; }
        }
        public ELEMENTARY_TYPE Font
        {
            get { return font; }
            set { font = value; }
        }
        #endregion
        public LRFTag(BitStreamReader sw, long offset, int objType, int streamsize)
        {
            PositionOfStructureInFile = sw.Position + offset;
            f500 = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 2); ;
            if (((byte[])f500.Value)[1] == 0xF5)
                type = LRFFileReader.TagTypes[((byte[])f500.Value)[0]];
            else
            {
                return;
            }
            if (((byte[])f500.Value)[0] == 0x00)//start tag
            {
                int objId = sw.ReadInteger();
                obType = new ELEMENTARY_TYPE(sw, offset, typeof(short));
                LengthInFile = sw.Position + offset - PositionOfStructureInFile;
                return;
            }
            int datasize = type.length;
            if (((byte[])f500.Value)[0] == 0x6A)
            {
            }
            if (datasize >= 0)// Fixed size
            {
                try
                {
                    switch (((byte[])f500.Value)[0])
                    {
                        case 0x03://link;
                            data = new ELEMENTARY_TYPE(sw, offset, typeof(uint));
                            break;
                        case 0x04://streamSize
                            streamSize = new ELEMENTARY_TYPE(sw, offset, typeof(int));
                            break;
                        case 0x4a://Image rect
                            data = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 4);
                            image_Height = new ELEMENTARY_TYPE(sw, offset, typeof(short));
                            image_width = new ELEMENTARY_TYPE(sw, offset, typeof(short));
                            break;
                        case 0x4b:
                            image_Size = new ELEMENTARY_TYPE(sw, offset, typeof(uint));
                            break;
                        case 0x4c:
                            imageStream_Id = new ELEMENTARY_TYPE(sw, offset, typeof(uint));
                            break;
                        case 0x6c:
                            page_Destination = new ELEMENTARY_TYPE(sw, offset, typeof(uint));
                            bloc_Destination = new ELEMENTARY_TYPE(sw, offset, typeof(uint));
                            break;
                        case 0x75:
                        case 0x76:
                        case 0x77:
                        case 0x79:
                        case 0x7A:
                            data = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));
                            break;
                        case 0x7B://child page tree
                            data = new ELEMENTARY_TYPE(sw, offset, typeof(uint));
                            break;
                        default:
                            data = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), datasize); //sw.ReadBytes(datasize);
                            break;
                    }
                }
                catch (Exception ex) { }
            }
            else //Variable length
            {
                try
                {
                    switch (((byte[])f500.Value)[0])
                    {
                        case 0x05:
                            data = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), streamsize); //Reads data stream;
                            break;
                        case 0x0b://Page
                            ids_Number = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));
                            content_Ids = new List<ELEMENTARY_TYPE>();
                            for (int i = 0; i < (ushort)ids_Number.Value; i++)
                            {
                                content_Ids.Add(new ELEMENTARY_TYPE(sw, offset, typeof(uint)));
                            }
                            break;
                        case 0x5c://pagelist
                            ids_Number = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));
                            content_Ids = new List<ELEMENTARY_TYPE>();
                            for (int i = 0; i < (ushort)ids_Number.Value; i++)
                            {
                                content_Ids.Add(new ELEMENTARY_TYPE(sw, offset, typeof(uint)));
                            }
                            break;
                        default:
                            List<byte> ints = new List<byte>();
                            int start = sw.Position;
                            byte x = sw.ReadByte();
                            while (x != 0xF5)
                            {
                                ints.Add(x);
                                x = sw.ReadByte();
                            }
                            sw.Position = start;
                            if (ints.Count > 1)
                                switch (((byte[])f500.Value)[0])
                                {
                                    case 0x16:
                                        fontLength = new ELEMENTARY_TYPE(sw, offset, typeof(short));
                                        font = new ELEMENTARY_TYPE(sw, offset, Encoding.Unicode, (ints.Count - 3) / 2);// (short)fontLength.Value / 2 +1);
                                        break;
                                    default:
                                        data = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), ints.Count - 1);
                                        break;
                                }
                            break;
                    }
                }
                catch { }
            }
            // length = data.Length;
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return type.name + " ";// +data.Length.ToString(); ;
        }
    }
    public class TagType
    {
        public int id;
        public int length;
        public string name;
        public string value = "";
        public TagType(int id, int length, string name)
        {
            this.id = id;
            this.length = length;
            this.name = name;
        }
        public TagType(int id, int length, string name, string value)
        {
            this.id = id;
            this.length = length;
            this.name = name;
            this.value = value;
        }
    }
    #region Enumerations
    public enum LrfObjectType
    {
        PageTree = 0x01,
        Page = 0x02,
        Header = 0x03,
        Footer = 0x04,
        PageAtr = 0x05,
        Block = 0x06,
        BlockAtr = 0x07,
        MiniPage = 0x08,
        BlockList = 0x09,
        Text = 0x0A,
        TextAtr = 0x0B,
        Image = 0x0C,
        Canvas = 0x0D,
        ParagraphAtr = 0x0E,
        ImageStream = 0x11,
        Import = 0x12,
        Button = 0x13,
        Window = 0x14,
        PopUpWin = 0x15,
        Sound = 0x16,
        PlaneStream = 0x17,
        Font = 0x19,
        ObjectInfo = 0x1A,
        BookAtr = 0x1C,
        SimpleText = 0x1D,
        Toc = 0x1E
    }
    public enum Ruby_Overhang { auto, start, end, none }
    public enum Ruby_Align { auto, start, left, center, end, right, distribute_letter, distribute_space, line_edge }
    public enum Ruby_Position { before, after, inter_character, inline }
    #endregion
    #region Useless
    public class Page : LrfObject
    {
        LrfObject page;
        private string pageText;
        private List<LrfObject> loclist = new List<LrfObject>();
        private List<Block> blocks = new List<Block>();
        private List<Canvas> canvases = new List<Canvas>();

        public string PageText
        {
            get { return pageText; }
            set { pageText = value; }
        }
        public List<LrfObject> Loclist
        {
            get { return loclist; }
            set { loclist = value; }
        }
        public List<Block> Blocks
        {
            get { return blocks; }
            set { blocks = value; }
        }
        public List<Canvas> Canvases
        {
            get { return canvases; }
            set { canvases = value; }
        }
        public List<TagType> others = new List<TagType>();
        public LrfObject attribs;
        public Page(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public Page(LrfObject oo)
        {
            page = oo;
            pageText = "";
            PositionOfStructureInFile = oo.PositionOfStructureInFile;
            LengthInFile = oo.LengthInFile;
        }
        public override string ToString()
        {
            return page.ToString();
        }
    }
    public class Block : LrfObject
    {
        LrfObject block;
        public LrfObject attribs;
        public List<TextBlock> text = new List<TextBlock>();
        public Block(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public Block(LrfObject o)
        {
            block = o;
        }
        public override string ToString()
        {
            return block.ToString();
        }
    }
    public class Canvas : LrfObject
    {
        LrfObject canvas;
        public List<Block> blocs = new List<Block>();
        public int Height;
        public int Width;
        public Canvas(LrfObject c)
        {
            canvas = c;
        }
        public Canvas(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return canvas.ToString();
        }
    }
    public class TextBlock : LrfObject
    {
        public LrfObject text;
        public string t;

        public LrfObject attribs;
        public TextBlock(LrfObject t)
        {
            text = t;
        }
        public string Uncompress()
        {
            LRFTag lro = text.LrfTags[4];
            int len = ((byte[])text.LrfTags[4].Data.Value).Length;
            byte[] data = new byte[len - 4];
            Buffer.BlockCopy((byte[])text.LrfTags[4].Data.Value, 4, data, 0, len - 4);
            byte[] buffer = Decompress(data);
            List<string> tags = new List<string>();
            int u = 0;
            string truc = Encoding.Unicode.GetString(buffer);
            string s = "";
            List<byte> txt = new List<byte>();
            List<List<byte>> txts = new List<List<byte>>();
            while (u < buffer.Length - 1)
            {
                #region Parse
                if (buffer[u + 1] == 0xf5)
                {
                    if (txt.Count > 0)
                    {
                        s = Encoding.Unicode.GetString(txt.ToArray());
                        tags.Add(s);
                        txts.Add(txt);
                        txt.Clear();
                    }

                    switch (buffer[u])
                    {
                        case 0x00:
                            s = "<P>";
                            break;
                        case 0x11:
                            s = "";//Font Size
                            u += 2;// Size ?
                            break;
                        case 0x12:
                        case 0x13:
                        case 0x14:
                        case 0x15://Font weight
                            s = "";
                            u += 2;// Weight ?
                            break;
                        case 0x17:
                        case 0x18:
                            s = "";//
                            u += 4;// Weight ?
                            break;
                        case 0x19://Word space
                        case 0x1A:
                        case 0x1B://Base Line SkipBit
                        case 0x1C:
                        case 0x1D:
                        case 0x1E:
                        case 0x25:
                        case 0x26:
                        case 0x31:
                        case 0x32:
                        case 0x33:
                            s = "";
                            u += 2;
                            break;
                        case 0x81:
                            s = "<I>";
                            break;
                        case 0x82:
                            s = "</I>";
                            break;
                        case 0xa1:
                        case 0xa2:
                            s = "<P>";
                            u += 4;
                            break;
                        case 0xa7:
                            s = "<h1>";//Begin button ?
                            u += 4;
                            break;
                        case 0xa8:
                            s = "</h1>";//end button
                            break;
                        case 0xb7:
                            s = "<sup>";//Begin sup
                            break;
                        case 0xb8:
                            s = "</sup>";//End sup
                            break;
                        case 0x16://Font Face name
                        case 0xd1:
                            s = "";
                            while (buffer[u + 3] != 0xF5)
                            {
                                u++;
                                s += "_";
                            }
                            break;
                        case 0xd2:
                            s = "</P>";
                            break;
                        case 0xcc:
                            s = "";//<a href
                            u += 2;
                            break;
                        default:
                            s = "";
                            break;
                    }
                    if (s != "")
                        tags.Add(s);
                    s = "";
                    u += 2;
                }
                else
                {
                    //             if ((buffer[inTime] != 0x00) || (buffer[inTime + 1] != 0x00))
                    {
                        txt.Add(buffer[u]);
                        txt.Add(buffer[u + 1]);
                    }
                    u += 2;
                }
                #endregion
            }
            t = "";
            if (s != "")
                tags.Add(s);
            foreach (string st in tags)
                t += st;
            return t;
        }
        public static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream();
            input.Write(data, 0, data.Length);
            input.Position = 0;
            InflaterInputStream gzip = new InflaterInputStream(input);
            MemoryStream output = new MemoryStream();
            byte[] buff = new byte[64];
            int read = -1;
            read = gzip.Read(buff, 0, Math.Min(buff.Length, data.Length));
            while (read > 0)
            {
                output.Write(buff, 0, read);
                read = gzip.Read(buff, 0, buff.Length);
            }
            gzip.Close();
            return output.ToArray();
        }

        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream gzip = new DeflaterOutputStream(output);
            gzip.Write(data, 0, data.Length);
            gzip.Close();
            return output.ToArray();
        }
        public override string ToString()
        {
            return text.ToString();
        }
    }
    #endregion

}

