using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Drawing;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Utils;

namespace BookReader
{
    public class LRFFileReader : Reader
    {
        #region Private methods
        /// <summary>
        /// Updates the compressed XMLInfo Block
        /// </summary>
        /*        private void updateCompXMLInfoBlock()
                {
                    /* Allocate the required temporary memory for the Compressed Info Block */
        /* Yes, I know that this doesn'type need to be 2, but I can'editBox find the exact                size right. 
        byte[] tempBytesCompXMLInfoBlock = new byte[(int)hdr.xmlInfoBlock.Length * 2];
        /* Compress the info block into the temporary array 
        Deflater d = new Deflater();
        d.SetInput(hdr.bytesXMLInfoBlock);
        d.Finish();
        /* Copy the size of the uncompressed block and the temporary array into the 
        /* permanent byte array 
        int length = d.Deflate(tempBytesCompXMLInfoBlock);
        this.hdr.bytesCompXMLInfoBlock = new byte[length + DWORD_SIZE];
        byte[] bytesUncompressedXMLSize = BitConverter.GetBytes(hdr.bytesXMLInfoBlock.Length);
        Buffer.BlockCopy(bytesUncompressedXMLSize, 0, hdr.bytesCompXMLInfoBlock, 0, DWORD_SIZE);
        Buffer.BlockCopy(tempBytesCompXMLInfoBlock, 0, hdr.bytesCompXMLInfoBlock, DWORD_SIZE, length);
        this.hdr.xmlCompSize = hdr.bytesCompXMLInfoBlock.Length;
    }*/
        /// <summary>
        /// Walks the object tree, and moves all offsets by a certain value
        /// </summary>
        /// <parameter name="lrfData">
        /// The object data
        /// </parameter>
        /// <parameter name="numObjects">
        /// The number of objects
        /// </parameter>
        /// <parameter name="objectIndexOffset">
        /// The offset to the object index.
        /// </parameter>
        /// <parameter name="diff">
        /// The adjustment value for each offset
        /// </parameter>
        /*      private void relocateOffsets(byte[] lrfData, Int64 numObjects, Int64 objectIndexOffset, int diff)
              {
                  for (int obNum = 0; obNum < numObjects; obNum++)
                  {
                      Int32 oldOffset = BitConverter.ToInt32(lrfData, (int)(objectIndexOffset + (16 * obNum) + 4));
                      byte[] bytesNewOffset = BitConverter.GetBytes(oldOffset + diff);
                      Buffer.BlockCopy(bytesNewOffset, 0, lrfData, (int)(objectIndexOffset + (16 * obNum) + 4), DWORD_SIZE);
                  }
              }*/
        /*        /// <summary>
                /// Get the entire LRF file as an array of bytes.
                /// </summary>
                /// <returns>
                /// LRF file as array of bytes.
                /// </returns>
                public byte[] getAsBytes()
                {
                    byte[] bytesHeader = generateHeader();
                    byte[] lrfData = new byte[bytesHeader.Length + hdr.bytesCompXMLInfoBlock.Length + bytesGIFData.Length + hdr.bytesLRFObjectData.Length];

                    /* Copy in the sections one after the other
                    Buffer.BlockCopy(bytesHeader, 0, lrfData, 0, bytesHeader.Length);
                    Buffer.BlockCopy(hdr.bytesCompXMLInfoBlock, 0, lrfData, bytesHeader.Length, hdr.bytesCompXMLInfoBlock.Length);
                    Buffer.BlockCopy(bytesGIFData, 0, lrfData, bytesHeader.Length + hdr.bytesCompXMLInfoBlock.Length, bytesGIFData.Length);
                    Buffer.BlockCopy(hdr.bytesLRFObjectData, 0, lrfData,
                                 bytesHeader.Length + hdr.bytesCompXMLInfoBlock.Length + bytesGIFData.Length, hdr.bytesLRFObjectData.Length);

                    int currentLRFObjectOffset = this.hdr.xmlCompSize + this.hdr.gifSize + bytesHeader.Length;
                    int diff = currentLRFObjectOffset - hdr.originalLRFObjectOffset;
                    relocateOffsets(lrfData, this.hdr.numObjects, BitConverter.ToInt64(bytesHeader, LRF_OFFSET_HEADER_OBJECTINDEXOFFSET), diff);

                    return lrfData;
                }
                /// <summary>
                /// Generate the FileHeader
                /// </summary>
                /// <returns>
                /// bytes array representing the LRF FileHeader.
                /// </returns>
                private byte[] generateHeader()
                {
                    byte[] header = new byte[84];

                    /* Generate the signature 
                    Buffer.BlockCopy(this.hdr.signature, 0, header, LRF_OFFSET_HEADER_SIGNATURE, 8);

                    /* Generate the version 
                    Byte[] bytesVersion = BitConverter.GetBytes(this.hdr.version);
                    Buffer.BlockCopy(bytesVersion, 0, header, LRF_OFFSET_HEADER_VERSION, WORD_SIZE);

                    /* Generate the PsuedoEncryption key 
                    Byte[] bytesPseudoEncryption = BitConverter.GetBytes(this.hdr.pseudoEncryption);
                    Buffer.BlockCopy(bytesPseudoEncryption, 0, header, LRF_OFFSET_HEADER_PSEUDOENCRYPTION, WORD_SIZE);

                    /* Generate the RootObjectID 
                    Byte[] bytesRootObjectID = BitConverter.GetBytes(this.hdr.rootObjectID);
                    Buffer.BlockCopy(bytesRootObjectID, 0, header, LRF_OFFSET_HEADER_ROOTOBJECTID, DWORD_SIZE);

                    /* Generate the NumberOfObjects 
                    Byte[] bytesNumberOfObjects = BitConverter.GetBytes(this.hdr.numObjects);
                    Buffer.BlockCopy(bytesNumberOfObjects, 0, header, LRF_OFFSET_HEADER_NUMOBJECTS, 8);

                    /* Generate Unknown1 
                    Byte[] bytesUnknown1 = BitConverter.GetBytes(this.hdr.unknown1);
                    Buffer.BlockCopy(bytesUnknown1, 0, header, LRF_OFFSET_HEADER_UNKNOWN1, DWORD_SIZE);

                    /* Generate flags 
                    header[LRF_OFFSET_HEADER_FLAGS] = this.hdr.flags;

                    /* Generate Unknown2 
                    header[LRF_OFFSET_HEADER_UNKNOWN2] = this.hdr.unknown2;

                    /* Generate Unknown3 
                    Byte[] bytesUnknown3 = BitConverter.GetBytes(this.hdr.unknown3);
                    Buffer.BlockCopy(bytesUnknown3, 0, header, LRF_OFFSET_HEADER_UNKNOWN3, WORD_SIZE);

                    /* Generate Unknown4 
                    Byte[] bytesUnknown4 = BitConverter.GetBytes(this.hdr.unknown4);
                    Buffer.BlockCopy(bytesUnknown4, 0, header, LRF_OFFSET_HEADER_UNKNOWN4, WORD_SIZE);

                    /* Generate Height 
                    Byte[] bytesHeight = BitConverter.GetBytes(this.hdr.height);
                    Buffer.BlockCopy(bytesHeight, 0, header, LRF_OFFSET_HEADER_HEIGHT, WORD_SIZE);

                    /* Generate Width 
                    Byte[] bytesWidth = BitConverter.GetBytes(this.hdr.width);
                    Buffer.BlockCopy(bytesWidth, 0, header, LRF_OFFSET_HEADER_WIDTH, WORD_SIZE);

                    /* Generate Unknown5 
                    header[LRF_OFFSET_HEADER_UNKNOWN5] = this.hdr.unknown5;

                    /* Generate Unknown6 
                    header[LRF_OFFSET_HEADER_UNKNOWN6] = this.hdr.unknown6;

                    /* Generate Unknown7 
                    Buffer.BlockCopy(this.hdr.unknown7, 0, header, LRF_OFFSET_HEADER_UNKNOWN7, 20);

                    /* Generate the TOCObjectID 
                    Byte[] bytesTocObjectID = BitConverter.GetBytes(this.hdr.tocObjectID);
                    Buffer.BlockCopy(bytesTocObjectID, 0, header, LRF_OFFSET_HEADER_TOCOBJECTID, DWORD_SIZE);

                    /* Generate the xmlCompSize 
                    Byte[] bytesXMLCompSize = BitConverter.GetBytes(this.hdr.xmlCompSize);
                    Buffer.BlockCopy(bytesXMLCompSize, 0, header, LRF_OFFSET_HEADER_COMXMLSIZE, WORD_SIZE);

                    /* Generate Thumbnail Image Type 
                    Byte[] bytesThumbImageType = BitConverter.GetBytes(this.hdr.thumbImageType);
                    Buffer.BlockCopy(bytesThumbImageType, 0, header, LRF_OFFSET_HEADER_THUMBIMAGETYPE, WORD_SIZE);

                    /* Generate the gifSize 
                    Byte[] bytesGIFSize = BitConverter.GetBytes(this.hdr.gifSize);
                    Buffer.BlockCopy(bytesGIFSize, 0, header, LRF_OFFSET_HEADER_GIFSIZE, DWORD_SIZE);

                    int currentLRFObjectOffset = this.hdr.xmlCompSize + this.hdr.gifSize + header.Length;
                    int diff = currentLRFObjectOffset - hdr.originalLRFObjectOffset;

                    /* Generate the ObjectIndexOffset 
                    this.hdr.objectIndexOffset = this.hdr.objectIndexOffset + diff;
                    Byte[] bytesObjectIndexOffset = BitConverter.GetBytes(this.hdr.objectIndexOffset);
                    Buffer.BlockCopy(bytesObjectIndexOffset, 0, header, LRF_OFFSET_HEADER_OBJECTINDEXOFFSET, 8);

                    /* Generate the tocObjectOffset 
                    this.hdr.tocObjectOffset = this.hdr.tocObjectOffset + diff;
                    Byte[] bytesTocObjectOffset = BitConverter.GetBytes(this.hdr.tocObjectOffset);
                    Buffer.BlockCopy(bytesTocObjectOffset, 0, header, LRF_OFFSET_HEADER_TOCOBJECTOFFSET, DWORD_SIZE);

                    return header;
                }*/
        /// <summary>
        /// ParseXml the FileHeader from an LRF byte array
        /// </summary>

        #endregion
        #region Private members
        private byte[] bytesGIFData;
        private long fileSize;
        private string location;
        private string freeText = "";
        private LRFHeader hdr;
        private List<LrfObject> index;
        string publisher;
        string date;
        string title;
        string author;
        string bookId;
        string creator;
        string language;
        private List<LrfObject> images;
        public List<Page> Pages;
        private LrfObject root;
        public List<TextBlock> Textblocks;

        private string xml;
        #endregion
        #region Properties
        public LRFHeader LrfHeader
        {
            get { return hdr; }
            set { hdr = value; }
        }
        public List<LrfObject> Index
        {
            get { return index; }
            set { index = value; }
        }
        public LrfObject Book_Structure
        {
            get { return root; }
            set { root = value; }
        }
        public List<LrfObject> Images
        {
            get { return images; }
            set { images = value; }
        }
        public string FileName
        {
            get { return location; }
            set { location = value; }
        }
        public long FileSize
        {
            get { return fileSize; }
        }
        public string Comment
        {
            get { return freeText; }
            set { freeText = value; }
        }
        public Image CoverImage
        {
            get
            {
                if (Images != null)
                    return Images[0].Image;
                else return null;
            }
        }
        public string Xml
        {
            get { return xml; }
            set { xml = value; }
        }
        public string Publisher
        {
            get { return publisher; }
            set { publisher = value; }
        }
        public string Date
        {
            get { return date; }
            set { date = value; }
        }
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public string Author
        {
            get { return author; }
            set { author = value; }
        }
        public string BookId
        {
            get { return bookId; }
            set { bookId = value; }
        }
        public string Creator
        {
            get { return creator; }
            set { creator = value; }
        }
        public string Language
        {
            get { return language; }
            set { language = value; }
        }

        public byte[] GIFData
        {
            get { return bytesGIFData; }
            /*        set
                    {
                        bytesGIFData = value;
                        hdr.gifSize = bytesGIFData.Length;

                        /* Only do this if we actually have a thumbnail image 
                        if (hdr.gifSize > 0)
                        {
                            /* Set the Image Type: 0x12 for PNG, 0x14 for GIF, 0x13 for BM and 0x10 for JPEG 
                            if ((bytesGIFData[1] == 'P') && (bytesGIFData[2] == 'N') && (bytesGIFData[3] == 'G'))
                            {
                                /* It'lineRead a PNG 
                                hdr.thumbImageType = 0x12;
                            }
                            else if ((bytesGIFData[0] == 'B') && (bytesGIFData[1] == 'M'))
                            {
                                /* It'lineRead a BM 
                                hdr.thumbImageType = 0x13;
                            }
                            else if ((bytesGIFData[6] == 'J') && (bytesGIFData[7] == 'F') &&
                                 (bytesGIFData[8] == 'I') && (bytesGIFData[9] == 'F'))
                            {
                                /* It'lineRead a JFIF 
                                hdr.thumbImageType = 0x11;
                            }
                            else if ((bytesGIFData[0] == 'G') && (bytesGIFData[1] == 'I') &&
                                 (bytesGIFData[2] == 'F') && (bytesGIFData[3] == '8'))
                            {
                                /* It'lineRead a GIF 
                                hdr.thumbImageType = 0x14;
                            }
                            else
                            {
                                /* I give up, lets just call it a GIF and hope nobody notices 
                                hdr.thumbImageType = 0x14;
                            }
                        }
                    }*/
        }
        private byte[] XMLInfoBlock
        {
            get { return hdr.bytesXMLInfoBlock; }
        }
        #endregion
        public static List<TagType> TagTypes = new List<TagType>();
        SortedList<uint, LrfObject> listLrfObjects = new SortedList<uint, LrfObject>();
        #region Utility
        private void FillTagList()
        {
            #region create tags
            // - 1 = variable number of parameters
            // - 2 = invalid tag
            TagTypes.Add(new TagType(0X00, 6, "0bject Start "));
            TagTypes.Add(new TagType(0X01, 0, "0bject End "));
            TagTypes.Add(new TagType(0X02, 4, "0bject Info Link "));
            TagTypes.Add(new TagType(0X03, 4, "Link "));
            TagTypes.Add(new TagType(0X04, 4, "Stream Size "));
            TagTypes.Add(new TagType(0X05, -1, "Stream Start "));//Mod from 0
            TagTypes.Add(new TagType(0X06, 0, "Stream End "));
            TagTypes.Add(new TagType(0X07, 4, "odd-side header ID"));
            TagTypes.Add(new TagType(0X08, 4, "even-side header ID"));
            TagTypes.Add(new TagType(0X09, 4, "odd-side footer ID"));
            TagTypes.Add(new TagType(0X0A, 4, "even-side footer ID"));
            TagTypes.Add(new TagType(0X0B, -1, "content IDs"));
            TagTypes.Add(new TagType(0X0C, -2, "Unknown"));
            TagTypes.Add(new TagType(0X0D, -1, "Unknown"));
            TagTypes.Add(new TagType(0X0E, 2, "Unknown"));
            TagTypes.Add(new TagType(0X0F, -2, "Unknown"));
            TagTypes.Add(new TagType(0X10, -2, "Unknown"));
            TagTypes.Add(new TagType(0X11, 2, "Font Size "));
            TagTypes.Add(new TagType(0X12, 2, "Font Width "));
            TagTypes.Add(new TagType(0X13, 2, "Font Escapement "));
            TagTypes.Add(new TagType(0X14, 2, "Font Orientation "));
            TagTypes.Add(new TagType(0X15, 2, "Font Weight "));
            TagTypes.Add(new TagType(0X16, -1, "Font Facename "));
            TagTypes.Add(new TagType(0X17, 4, "Text Color "));
            TagTypes.Add(new TagType(0X18, 4, "Text Bg Color "));
            TagTypes.Add(new TagType(0X19, 2, "Word Space "));
            TagTypes.Add(new TagType(0X1A, 2, "Letter Space "));
            TagTypes.Add(new TagType(0X1B, 2, "Base Line Skip "));
            TagTypes.Add(new TagType(0X1C, 2, "Line Space "));
            TagTypes.Add(new TagType(0X1D, 2, "Par Indent "));
            TagTypes.Add(new TagType(0X1E, 2, "Par Skip "));
            TagTypes.Add(new TagType(0X1F, -2, "Unknown"));
            TagTypes.Add(new TagType(0X20, -2, "Unknown"));
            TagTypes.Add(new TagType(0X21, 2, "top margin in dots"));
            TagTypes.Add(new TagType(0X22, 2, "header height in dots"));
            TagTypes.Add(new TagType(0X23, 2, "header separation in dots"));
            TagTypes.Add(new TagType(0X24, 2, "left margin in dots"));
            TagTypes.Add(new TagType(0X25, 2, "Page Height "));
            TagTypes.Add(new TagType(0X26, 2, "Page Width "));
            TagTypes.Add(new TagType(0X27, 2, "footer space in dots"));
            TagTypes.Add(new TagType(0X28, 2, "footer height in dots"));
            TagTypes.Add(new TagType(0X29, 6, "background image reference"));
            TagTypes.Add(new TagType(0X2A, 2, "show or hide"));
            TagTypes.Add(new TagType(0X2B, 2, "high or low"));
            TagTypes.Add(new TagType(0X2C, 2, "even-side margin"));
            TagTypes.Add(new TagType(0X2D, 4, "Unknown"));
            TagTypes.Add(new TagType(0X2E, 2, "block attributes"));
            TagTypes.Add(new TagType(0X2F, -2, "Unknown"));
            TagTypes.Add(new TagType(0X30, -2, "Unknown"));
            TagTypes.Add(new TagType(0X31, 2, "Block Width "));
            TagTypes.Add(new TagType(0X32, 2, "Block Height "));
            TagTypes.Add(new TagType(0X33, 2, "Block Rule "));
            TagTypes.Add(new TagType(0X34, 4, "block background color in RGBA"));
            TagTypes.Add(new TagType(0X35, 2, "block layout mode"));
            TagTypes.Add(new TagType(0X36, 2, "frame width in dots"));
            TagTypes.Add(new TagType(0X37, 4, "frame color in RGBA"));
            TagTypes.Add(new TagType(0X38, 2, "frame mode"));
            TagTypes.Add(new TagType(0X39, 2, "top skip in dots"));
            TagTypes.Add(new TagType(0X3A, 2, "side margin in dots"));
            TagTypes.Add(new TagType(0X3B, -2, "Unknown"));
            TagTypes.Add(new TagType(0X3C, 2, "alignment"));
            TagTypes.Add(new TagType(0X3D, 2, "foot skip in dots"));
            TagTypes.Add(new TagType(0X3E, 2, "Unknown"));
            TagTypes.Add(new TagType(0X3F, -2, "Unknown"));
            TagTypes.Add(new TagType(0X40, -2, "Unknown"));
            TagTypes.Add(new TagType(0X41, 2, "Mini Page Height "));
            TagTypes.Add(new TagType(0X42, 2, "Mini Page Width "));
            TagTypes.Add(new TagType(0X43, -2, "Unknown"));
            TagTypes.Add(new TagType(0X44, 4, "Unknown"));
            TagTypes.Add(new TagType(0X45, 4, "Unknown"));
            TagTypes.Add(new TagType(0X46, 2, "Location Y "));
            TagTypes.Add(new TagType(0X47, 2, "Location X "));
            TagTypes.Add(new TagType(0X48, 2, "Unknown"));
            TagTypes.Add(new TagType(0X49, 8, "Put Sound "));
            TagTypes.Add(new TagType(0X4A, 8, "Image Rect "));
            TagTypes.Add(new TagType(0X4B, 4, "Image Size "));
            TagTypes.Add(new TagType(0X4C, 4, "Image Stream "));
            TagTypes.Add(new TagType(0X4D, 0, "Unknown"));
            TagTypes.Add(new TagType(0X4E, 12, "Unknown"));
            TagTypes.Add(new TagType(0X4F, -2, "Unknown"));
            TagTypes.Add(new TagType(0X50, -2, "Unknown"));
            TagTypes.Add(new TagType(0X51, 2, "Canvas Width "));
            TagTypes.Add(new TagType(0X52, 2, "Canvas Height "));
            TagTypes.Add(new TagType(0X53, 4, "Unknown"));
            TagTypes.Add(new TagType(0X54, 2, "Stream Flags "));
            TagTypes.Add(new TagType(0X55, -1, "Unknown"));
            TagTypes.Add(new TagType(0X56, -1, "Unknown"));
            TagTypes.Add(new TagType(0X57, 2, "Unknown"));
            TagTypes.Add(new TagType(0X58, 2, "Unknown"));
            TagTypes.Add(new TagType(0X59, -1, "Font File Name "));
            TagTypes.Add(new TagType(0X5A, -1, "Unknown"));
            TagTypes.Add(new TagType(0X5B, 2, "View Point "));
            TagTypes.Add(new TagType(0X5C, -1, "Page List "));
            TagTypes.Add(new TagType(0X5D, -1, "Font Face Name "));
            TagTypes.Add(new TagType(0X5E, 2, "Unknown"));
            TagTypes.Add(new TagType(0X5F, -2, "Unknown"));
            TagTypes.Add(new TagType(0X60, -2, "Unknown"));
            TagTypes.Add(new TagType(0X61, 2, "button flags"));
            TagTypes.Add(new TagType(0X62, 0, "start base button"));
            TagTypes.Add(new TagType(0X63, 0, "end base button"));
            TagTypes.Add(new TagType(0X64, 0, "start focus-in-button"));
            TagTypes.Add(new TagType(0X65, 0, "end focus-in-button"));
            TagTypes.Add(new TagType(0X66, 0, "start link"));
            TagTypes.Add(new TagType(0X67, 0, "end link"));
            TagTypes.Add(new TagType(0X68, 0, "start up-button"));
            TagTypes.Add(new TagType(0X69, 0, "end up-button"));
            TagTypes.Add(new TagType(0X6A, 0, "start action"));
            TagTypes.Add(new TagType(0X6B, 0, "end action"));
            TagTypes.Add(new TagType(0X6C, 8, "Jump To "));
            TagTypes.Add(new TagType(0X6D, -1, "http link"));
            TagTypes.Add(new TagType(0X6E, 0, "close window"));
            TagTypes.Add(new TagType(0X6F, -2, "Unknown"));
            TagTypes.Add(new TagType(0X70, -2, "Unknown"));
            TagTypes.Add(new TagType(0X71, 0, "Unknown"));
            TagTypes.Add(new TagType(0X72, 0, "Unknown"));
            TagTypes.Add(new TagType(0X73, 10, "Ruled Line "));
            TagTypes.Add(new TagType(0X74, -2, "Unknown"));
            TagTypes.Add(new TagType(0X75, 2, "Ruby Align "));
            TagTypes.Add(new TagType(0X76, 2, "Ruby Overhang "));
            TagTypes.Add(new TagType(0X77, 2, "Emp Dots Position "));
            TagTypes.Add(new TagType(0X78, 4, "Emp Dots Code "));// mod from -1
            TagTypes.Add(new TagType(0X79, 2, "Emp Line Position "));
            TagTypes.Add(new TagType(0X7A, 2, "Emp Line Mode "));
            TagTypes.Add(new TagType(0X7B, 4, "Child Page Tree "));
            TagTypes.Add(new TagType(0X7C, 4, "Parent Page Tree "));
            TagTypes.Add(new TagType(0X7D, -2, "Unknown"));
            TagTypes.Add(new TagType(0X7E, -2, "Unknown"));
            TagTypes.Add(new TagType(0X7F, -2, "Unknown"));
            TagTypes.Add(new TagType(0X80, -2, "Unknown"));
            TagTypes.Add(new TagType(0X81, 0, "Italic ", "<i>"));
            TagTypes.Add(new TagType(0X82, 0, "Italic ", "</i>"));
            TagTypes.Add(new TagType(0X83, -2, "Unknown"));
            TagTypes.Add(new TagType(0X84, -2, "Unknown"));
            TagTypes.Add(new TagType(0X85, -2, "Unknown"));
            TagTypes.Add(new TagType(0X86, -2, "Unknown"));
            TagTypes.Add(new TagType(0X87, -2, "Unknown"));
            TagTypes.Add(new TagType(0X88, -2, "Unknown"));
            TagTypes.Add(new TagType(0X89, -2, "Unknown"));
            TagTypes.Add(new TagType(0X8A, -2, "Unknown"));
            TagTypes.Add(new TagType(0X8B, -2, "Unknown"));
            TagTypes.Add(new TagType(0X8C, -2, "Unknown"));
            TagTypes.Add(new TagType(0X8D, -2, "Unknown"));
            TagTypes.Add(new TagType(0X8E, -2, "Unknown"));
            TagTypes.Add(new TagType(0X8F, -2, "Unknown"));
            TagTypes.Add(new TagType(0X90, -2, "Unknown"));
            TagTypes.Add(new TagType(0X91, -2, "Unknown"));
            TagTypes.Add(new TagType(0X92, -2, "Unknown"));
            TagTypes.Add(new TagType(0X93, -2, "Unknown"));
            TagTypes.Add(new TagType(0X94, -2, "Unknown"));
            TagTypes.Add(new TagType(0X95, -2, "Unknown"));
            TagTypes.Add(new TagType(0X96, -2, "Unknown"));
            TagTypes.Add(new TagType(0X97, -2, "Unknown"));
            TagTypes.Add(new TagType(0X98, -2, "Unknown"));
            TagTypes.Add(new TagType(0X99, -2, "Unknown"));
            TagTypes.Add(new TagType(0X9A, -2, "Unknown"));
            TagTypes.Add(new TagType(0X9B, -2, "Unknown"));
            TagTypes.Add(new TagType(0X9C, -2, "Unknown"));
            TagTypes.Add(new TagType(0X9D, -2, "Unknown"));
            TagTypes.Add(new TagType(0X9E, -2, "Unknown"));
            TagTypes.Add(new TagType(0X9F, -2, "Unknown"));
            TagTypes.Add(new TagType(0XA0, -2, "Unknown"));
            TagTypes.Add(new TagType(0XA1, 4, "Begin P ", "<P>"));
            TagTypes.Add(new TagType(0XA2, 0, "End P ", "</P>"));
            TagTypes.Add(new TagType(0XA3, -2, "Unknown"));
            TagTypes.Add(new TagType(0XA4, -2, "Unknown"));
            TagTypes.Add(new TagType(0XA5, -1, "Koma Gaiji "));
            TagTypes.Add(new TagType(0XA6, 0, "Koma Emp Dot Char "));
            TagTypes.Add(new TagType(0XA7, 4, "Begin Button ", "Unknown"));
            TagTypes.Add(new TagType(0XA8, 2, "End Button ", "Unknown"));
            TagTypes.Add(new TagType(0XA9, 2, "Begin Ruby "));
            TagTypes.Add(new TagType(0XAA, 2, "End Ruby "));
            TagTypes.Add(new TagType(0XAB, 2, "Begin Ruby Base "));
            TagTypes.Add(new TagType(0XAC, 2, "End Ruby Base "));
            TagTypes.Add(new TagType(0XAD, 2, "Begin Ruby Text "));
            TagTypes.Add(new TagType(0XAE, 2, "End Ruby Text "));
            TagTypes.Add(new TagType(0XAF, -2, "Unknown"));
            TagTypes.Add(new TagType(0XB0, -2, "Unknown"));
            TagTypes.Add(new TagType(0XB1, 2, "Koma Yokomoji "));
            TagTypes.Add(new TagType(0XB2, 2, "end yokomoji"));
            TagTypes.Add(new TagType(0XB3, 2, "Tate "));
            TagTypes.Add(new TagType(0XB4, 2, "Tate "));
            TagTypes.Add(new TagType(0XB5, 2, "Nekase "));
            TagTypes.Add(new TagType(0XB6, 2, "Nekase "));
            TagTypes.Add(new TagType(0XB7, 2, "Begin Sup "));
            TagTypes.Add(new TagType(0XB8, 2, "End Sup "));
            TagTypes.Add(new TagType(0XB9, 2, "Begin Sub "));
            TagTypes.Add(new TagType(0XBA, 2, "End Sub "));
            TagTypes.Add(new TagType(0XBB, 2, "start bold"));
            TagTypes.Add(new TagType(0XBC, 2, "end bold"));
            TagTypes.Add(new TagType(0XBD, 2, "start no-break"));
            TagTypes.Add(new TagType(0XBE, 2, "end no-break"));
            TagTypes.Add(new TagType(0XBF, -2, "start emphasis box"));
            TagTypes.Add(new TagType(0XC0, -2, "Unknown"));
            TagTypes.Add(new TagType(0XC1, 2, "Begin emphasis Line "));
            TagTypes.Add(new TagType(0XC2, 0, "end emphasis line"));
            TagTypes.Add(new TagType(0XC3, 2, "Begin Draw Char "));
            TagTypes.Add(new TagType(0XC4, 0, "End Draw Char "));
            TagTypes.Add(new TagType(0XC5, 2, "Unknown"));
            TagTypes.Add(new TagType(0XC6, 2, "Unknown"));
            TagTypes.Add(new TagType(0XC7, 0, "Unknown"));
            TagTypes.Add(new TagType(0XC8, 2, "Koma Auto Spacing "));
            TagTypes.Add(new TagType(0XC9, 0, "Unknown"));
            TagTypes.Add(new TagType(0XCA, 2, "Space ", "&nbsp;"));
            TagTypes.Add(new TagType(0XCB, -1, "Unknown"));
            TagTypes.Add(new TagType(0XCC, 2, "Unicode string"));
            TagTypes.Add(new TagType(0XCD, -2, "Unknown"));
            TagTypes.Add(new TagType(0XCE, -2, "Unknown"));
            TagTypes.Add(new TagType(0XCF, -2, "Unknown"));
            TagTypes.Add(new TagType(0XD0, -2, "Unknown"));
            TagTypes.Add(new TagType(0XD1, -1, "Koma Plot "));
            TagTypes.Add(new TagType(0XD2, 0, "EOL ", "Unknown"));
            TagTypes.Add(new TagType(0XD3, -2, "Unknown"));
            TagTypes.Add(new TagType(0XD4, 2, "Wait "));
            TagTypes.Add(new TagType(0XD5, -2, "Unknown"));
            TagTypes.Add(new TagType(0XD6, 0, "Sound Stop "));
            TagTypes.Add(new TagType(0XD7, 14, "Move Obj "));
            TagTypes.Add(new TagType(0XD8, 4, "Book Font "));
            TagTypes.Add(new TagType(0XD9, 8, "Koma Plot Text "));
            TagTypes.Add(new TagType(0XDA, 2, "Wait sound"));
            TagTypes.Add(new TagType(0XDB, 2, "Unknown"));
            TagTypes.Add(new TagType(0XDC, 2, "Unknown"));
            TagTypes.Add(new TagType(0XDD, 2, "Char Space "));
            TagTypes.Add(new TagType(0XDE, -2, "Unknown"));
            TagTypes.Add(new TagType(0XDF, -2, "Unknown"));
            TagTypes.Add(new TagType(0XE0, -2, "Unknown"));
            TagTypes.Add(new TagType(0XE1, -2, "Unknown"));
            TagTypes.Add(new TagType(0XE2, -2, "Unknown"));
            TagTypes.Add(new TagType(0XE3, -2, "Unknown"));
            TagTypes.Add(new TagType(0XE4, -2, "Unknown"));
            TagTypes.Add(new TagType(0XE5, -2, "Unknown"));
            TagTypes.Add(new TagType(0XE6, -2, "Unknown"));
            TagTypes.Add(new TagType(0XE7, -2, "Unknown"));
            TagTypes.Add(new TagType(0XE8, -2, "Unknown"));
            TagTypes.Add(new TagType(0XE9, -2, "Unknown"));
            TagTypes.Add(new TagType(0XEA, -2, "Unknown"));
            TagTypes.Add(new TagType(0XEB, -2, "Unknown"));
            TagTypes.Add(new TagType(0XEC, -2, "Unknown"));
            TagTypes.Add(new TagType(0XED, -2, "Unknown"));
            TagTypes.Add(new TagType(0XEE, -2, "Unknown"));
            TagTypes.Add(new TagType(0XEF, -2, "Unknown"));
            TagTypes.Add(new TagType(0XF0, -2, "Unknown"));
            TagTypes.Add(new TagType(0XF1, 2, "Line Width "));
            TagTypes.Add(new TagType(0XF2, 4, "Line Color "));
            TagTypes.Add(new TagType(0XF3, 4, "Fill Color "));
            TagTypes.Add(new TagType(0XF4, 2, "Line Mode "));
            TagTypes.Add(new TagType(0XF5, 4, "Move To "));
            TagTypes.Add(new TagType(0XF6, 4, "Line To "));
            TagTypes.Add(new TagType(0XF7, 4, "Draw Box "));
            TagTypes.Add(new TagType(0XF8, 4, "Draw Ellipse "));
            TagTypes.Add(new TagType(0XF9, 6, "Unknown"));
            TagTypes.Add(new TagType(0XFA, -2, "Unknown"));
            TagTypes.Add(new TagType(0XFB, -2, "Unknown"));
            TagTypes.Add(new TagType(0XFC, -2, "Unknown"));
            TagTypes.Add(new TagType(0XFD, -2, "Unknown"));
            TagTypes.Add(new TagType(0XFE, -2, "Unknown"));
            TagTypes.Add(new TagType(0XFF, -2, "Unknown"));

            #endregion
        }
        #endregion
        LrfObject toc;

        public LrfObject Toc
        {
            get { return toc; }
            set { toc = value; }
        }
        public LRFFileReader(string filename)
        {
            FillTagList();
            // Structure : Header (54 Bytes) + XMLcompressed data + Data
            this.location = filename;
            this.fileSize = new FileInfo(filename).Length;
            BitStreamReader sw = new BitStreamReader(filename, false);
            hdr = new LRFHeader(sw);
            #region UncompressInfoBlock
            byte[] infoBlock = sw.ReadBytes((short)hdr.XmlCompSize.Value - 4);
            hdr.bytesXMLInfoBlock = new byte[(int)hdr.UnComXmlSize.Value];
            Inflater i = new Inflater();
            i.SetInput(infoBlock);
            i.Inflate(hdr.bytesXMLInfoBlock);
            xml = Encoding.Default.GetString(hdr.bytesXMLInfoBlock);
            #endregion
            #region ReadGIFData
            /* Allocate the required memory for the GIF */
            int gifLength = (int)hdr.GifSize.Value;
            if (gifLength > 0)
            {
                bytesGIFData = sw.ReadBytes(gifLength);
            }
            #endregion
            #region Read index
            sw.Position = (int)(long)hdr.ObjectIndexOffset.Value;
            index = new List<LrfObject>();
            for (int u = 0; u < (long)hdr.NumObjects.Value; u++)
            {
                LrfObject entry = new LrfObject(sw);
                index.Add(entry);
                try
                {

                    listLrfObjects.Add((uint)(int)entry.Id.Value, entry);
                }
                catch { }
            }
            sw.Close();
            ParseXml();
            Structure();
            #endregion
        }
        public void ParseXml()
        {
            XmlTextReader textReader = new XmlTextReader(new MemoryStream(XMLInfoBlock));
            textReader.MoveToContent();
            while (textReader.Read())
            {
                /* Only bother with this node if it's an element containing data */
                if ((textReader.NodeType == XmlNodeType.Element) && (!textReader.IsEmptyElement))
                {
                    try
                    {
                        switch (textReader.Name)
                        {
                            case "Page":
                                string page = textReader.ReadElementString();
                                break;
                            case "Creator":
                                creator = textReader.ReadElementString();
                                break;
                            case "Language":
                                language = textReader.ReadElementString();
                                break;
                            case "BookID":
                                bookId = textReader.ReadElementString();
                                break;
                            case "Author":
                                if (textReader.GetAttribute("reading") != null)
                                {
                                    author = textReader.GetAttribute("reading");
                                }
                                author = textReader.ReadElementString();
                                break;
                            case "Title":
                                if (textReader.GetAttribute("reading") != null)
                                {
                                    title = textReader.GetAttribute("reading");
                                }
                                title = textReader.ReadElementString();
                                break;
                            case "CreationDate":
                                date = textReader.ReadElementString().Trim();
                                break;
                            case "Publisher":
                                publisher = textReader.ReadElementString().Trim();
                                break;
                            case "FreeText":
                                freeText = textReader.ReadElementString().Trim();
                                break;
                        }
                    }
                    catch { }
                }
            }
        }
        public void Structure()
        {
            int tocind = (int)hdr.TocObjectID.Value;
            try
            {
                toc = listLrfObjects[(uint)tocind];//table of content
            }
            catch { }
            root = listLrfObjects[(uint)(int)hdr.RootObjectID.Value];
            LrfObject pageTreeObject = listLrfObjects[(uint)root.LrfTags[1].Data.Value];
            root.AddSon(pageTreeObject);
            //           BitStreamReader mms = new BitStreamReader((byte[])pageTreeObject.LrfTags[1].Data.Value, false);
            byte[] buffer = new byte[4];
            #region Useless
             Textblocks = new List<TextBlock>();
            List<LrfObject> textPages = new List<LrfObject>();
            #endregion
            for (int u = 0; u < (ushort)pageTreeObject.LrfTags[1].Ids_Number.Value; u++)
            {
                #region read one page
                LrfObject page = listLrfObjects[(uint)pageTreeObject.LrfTags[1].Content_Ids[u].Value];
                pageTreeObject.AddSon(page);
                //lrfTags[1] : list of related index, block, text, attributes ...
                #region get all objects
                for (int i = 0; i < (ushort)page.LrfTags[1].Ids_Number.Value; i++)
                {
                    try
                    {
                        ELEMENTARY_TYPE pageref = page.LrfTags[1].Content_Ids[i];
                        LrfObject obj = listLrfObjects[(uint)pageref.Value];
         //               page.AddSon(obj);
                        if ((int)obj.ObjType == 0x0C)
                        {
                            #region find Images
                            if (images == null)
                                images = new List<LrfObject>();
                            uint imageSize = (uint)obj.LrfTags[2].Image_Size.Value;// two short : height, width
                            LrfObject image = listLrfObjects[(uint)obj.LrfTags[3].Image_Id.Value];
                            uint code = BitConverter.ToUInt32((byte[])image.LrfTags[3].Data.Value, 0);// ReadUint((byte[])image.LrfTags[3].Data.Value);
                            if (code == 0xE0FFD8FF)
                            {
                                try
                                {
                                    MemoryStream imageStream = new MemoryStream((byte[])image.LrfTags[3].Data.Value);
                                    //JPEG image
                                    obj.Image = Image.FromStream(imageStream);
                                    images.Add(obj);
                                    imageStream.Close();
                                }
                                catch { }
                            }
                            #endregion
                        }
                    }
                    catch { }
                }
                #endregion
                //lrfTags[2] : link contains Bloc attribute
                try
                {
                    page.Attribute = listLrfObjects[(uint)page.LrfTags[2].Data.Value];
                }
                catch { }
                foreach (ELEMENTARY_TYPE uref in page.LrfTags[1].Content_Ids)
                {
             //       page.AddSon(listLrfObjects[(uint)uref.Value]);
                }
                foreach (LRFTag tag in page.Others)
                {
                    switch (((byte[])tag.F500.Value)[0])
                    {
                        case 0x03://Link
                            LrfObject ob = listLrfObjects[(uint)tag.Data.Value];
                            page.AddSon(ob);
                            switch (ob.ObjType)
                            {
                                case LrfObjectType.Canvas:
                                    #region Add canvas
                            //        Canvas c = new Canvas(ob);
                                //        page.AddSon(c);
                                    break;
                                    #endregion
                                case LrfObjectType.Block:
                                    #region Add block
                                    foreach (LRFTag tago in ob.Others)
                                    {
                                        LrfObject tx = listLrfObjects[(uint)tago.Data.Value];
                                        ob.AddSon(tx);
                                        tx.Attribute = listLrfObjects[(uint)tx.LrfTags[1].Data.Value];
                                    }
                                    ob.Attribute = listLrfObjects[(uint)ob.LrfTags[1].Data.Value];
                                    break;
                                    #endregion
                                default:
                                    break;
                            }
                            break;
                        case 0x73://Ruled line
                            break;
                        default:
                            try
                            {
                                ob = listLrfObjects[(uint)tag.Data.Value];
                                page.AddSon(ob);
                            }
                            catch { };
                          break;
                    }
                }
                 #endregion
            }
        }
    }
}
