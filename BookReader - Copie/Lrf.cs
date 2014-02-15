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

//using System.IO.Compression;

namespace BookReader
{
    public class LRFFileReader : Reader
    {
        #region Constants
        /// <summary>
        /// Size of a WORD
        /// </summary>
        public const int WORD_SIZE = 2;
        /// <summary>
        /// Size of a DWORD
        /// </summary>
        public const int DWORD_SIZE = 4;
        /// <summary>
        /// Size of a QWORD
        /// </summary>
        public const int QWORD_SIZE = 8;
        /// <summary>
        /// Offset of lengthString of uncompressed info block in LRF file
        /// </summary>
        public const int LRF_OFFSET_UNCOMXML_SIZE = 0x54;
        /// <summary>
        /// Offset of compressed info block data in LRF file
        /// </summary>
        public const int LRF_OFFSET_COMXML_DATA = LRF_OFFSET_UNCOMXML_SIZE + DWORD_SIZE;
        /// <summary>
        /// Offset of Header Signature
        /// </summary>
        public const int LRF_OFFSET_HEADER_SIGNATURE = 0x00;
        /// <summary>
        /// Offset of Header Version
        /// </summary>
        public const int LRF_OFFSET_HEADER_VERSION = 0x08;
        /// <summary>
        /// Offset of Header PseudoEncryption
        /// </summary>
        public const int LRF_OFFSET_HEADER_PSEUDOENCRYPTION = 0x0A;
        /// <summary>
        /// Offset of Header RootObjectID
        /// </summary>
        public const int LRF_OFFSET_HEADER_ROOTOBJECTID = 0x0C;
        /// <summary>
        /// Offset of Header NumObjects
        /// </summary>
        public const int LRF_OFFSET_HEADER_NUMOBJECTS = 0x10;
        /// <summary>
        /// Offset of Header ObjectIndexOffset
        /// </summary>
        public const int LRF_OFFSET_HEADER_OBJECTINDEXOFFSET = 0x18;
        /// <summary>
        /// Offset of Header Unknown1
        /// </summary>
        public const int LRF_OFFSET_HEADER_UNKNOWN1 = 0x20;
        /// <summary>
        /// Offset of Header Flags
        /// </summary>
        public const int LRF_OFFSET_HEADER_FLAGS = 0x24;
        /// <summary>
        /// Offset of Header Unknown2
        /// </summary>
        public const int LRF_OFFSET_HEADER_UNKNOWN2 = 0x25;
        /// <summary>
        /// Offset of Header Unknown3
        /// </summary>
        public const int LRF_OFFSET_HEADER_UNKNOWN3 = 0x26;
        /// <summary>
        /// Offset of Header Unknown4
        /// </summary>
        public const int LRF_OFFSET_HEADER_UNKNOWN4 = 0x28;
        /// <summary>
        /// Offset of Header Height
        /// </summary>
        public const int LRF_OFFSET_HEADER_HEIGHT = 0x2A;
        /// <summary>
        /// Offset of Header Width
        /// </summary>
        public const int LRF_OFFSET_HEADER_WIDTH = 0x2C;
        /// <summary>
        /// Offset of Header Unknown5
        /// </summary>
        public const int LRF_OFFSET_HEADER_UNKNOWN5 = 0x2E;
        /// <summary>
        /// Offset of Header Unknown6
        /// </summary>
        public const int LRF_OFFSET_HEADER_UNKNOWN6 = 0x2F;
        /// <summary>
        /// Offset of Header Unknown7
        /// </summary>
        public const int LRF_OFFSET_HEADER_UNKNOWN7 = 0x30;
        /// <summary>
        /// Offset of Header TOCObjectID
        /// </summary>
        public const int LRF_OFFSET_HEADER_TOCOBJECTID = 0x44;
        /// <summary>
        /// Offset of Header TOCObjectOffset
        /// </summary>
        public const int LRF_OFFSET_HEADER_TOCOBJECTOFFSET = 0x48;
        /// <summary>
        /// Offset of Header XMLCompSize
        /// </summary>
        public const int LRF_OFFSET_HEADER_COMXMLSIZE = 0x4C;
        /// <summary>
        /// Offset of Hearder Thumbnail Image Type
        /// </summary>
        public const int LRF_OFFSET_HEADER_THUMBIMAGETYPE = 0x4E;
        /// <summary>
        /// Offset of Header GIFSize
        /// </summary>
        public const int LRF_OFFSET_HEADER_GIFSIZE = 0x50;
        #endregion
        #region Private methods
        /// <summary>
        /// Updates the compressed XMLInfo Block
        /// </summary>
        private void updateCompXMLInfoBlock()
        {
            /* Allocate the required temporary memory for the Compressed Info Block */
            /* Yes, I know that this doesn'editBox need to be 2, but I can'editBox find the exact
               size right. */
            byte[] tempBytesCompXMLInfoBlock = new byte[hdr.bytesXMLInfoBlock.Length * 2];
            /* Compress the info block into the temporary array */
            Deflater d = new Deflater();
            d.SetInput(hdr.bytesXMLInfoBlock);
            d.Finish();
            /* Copy the size of the uncompressed block and the temporary array into the */
            /* permanent byte array */
            int length = d.Deflate(tempBytesCompXMLInfoBlock);
            this.hdr.bytesCompXMLInfoBlock = new byte[length + DWORD_SIZE];
            byte[] bytesUncompressedXMLSize = BitConverter.GetBytes(hdr.bytesXMLInfoBlock.Length);
            Buffer.BlockCopy(bytesUncompressedXMLSize, 0, hdr.bytesCompXMLInfoBlock, 0, DWORD_SIZE);
            Buffer.BlockCopy(tempBytesCompXMLInfoBlock, 0, hdr.bytesCompXMLInfoBlock, DWORD_SIZE, length);
            this.hdr.xmlCompSize = hdr.bytesCompXMLInfoBlock.Length;
        }
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
        private void relocateOffsets(byte[] lrfData, Int64 numObjects, Int64 objectIndexOffset, int diff)
        {
            for (int obNum = 0; obNum < numObjects; obNum++)
            {
                Int32 oldOffset = BitConverter.ToInt32(lrfData, (int)(objectIndexOffset + (16 * obNum) + 4));
                byte[] bytesNewOffset = BitConverter.GetBytes(oldOffset + diff);
                Buffer.BlockCopy(bytesNewOffset, 0, lrfData, (int)(objectIndexOffset + (16 * obNum) + 4), DWORD_SIZE);
            }
        }
        /// <summary>
        /// Get the entire LRF file as an array of bytes.
        /// </summary>
        /// <returns>
        /// LRF file as array of bytes.
        /// </returns>
        public byte[] getAsBytes()
        {
            byte[] bytesHeader = generateHeader();
            byte[] lrfData = new byte[bytesHeader.Length + hdr.bytesCompXMLInfoBlock.Length + bytesGIFData.Length + hdr.bytesLRFObjectData.Length];

            /* Copy in the sections one after the other*/
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

            /* Generate the signature */
            Buffer.BlockCopy(this.hdr.lrfSignature, 0, header, LRF_OFFSET_HEADER_SIGNATURE, 8);

            /* Generate the version */
            Byte[] bytesVersion = BitConverter.GetBytes(this.hdr.version);
            Buffer.BlockCopy(bytesVersion, 0, header, LRF_OFFSET_HEADER_VERSION, WORD_SIZE);

            /* Generate the PsuedoEncryption key */
            Byte[] bytesPseudoEncryption = BitConverter.GetBytes(this.hdr.pseudoEncryption);
            Buffer.BlockCopy(bytesPseudoEncryption, 0, header, LRF_OFFSET_HEADER_PSEUDOENCRYPTION, WORD_SIZE);

            /* Generate the RootObjectID */
            Byte[] bytesRootObjectID = BitConverter.GetBytes(this.hdr.rootObjectID);
            Buffer.BlockCopy(bytesRootObjectID, 0, header, LRF_OFFSET_HEADER_ROOTOBJECTID, DWORD_SIZE);

            /* Generate the NumberOfObjects */
            Byte[] bytesNumberOfObjects = BitConverter.GetBytes(this.hdr.numObjects);
            Buffer.BlockCopy(bytesNumberOfObjects, 0, header, LRF_OFFSET_HEADER_NUMOBJECTS, 8);

            /* Generate Unknown1 */
            Byte[] bytesUnknown1 = BitConverter.GetBytes(this.hdr.unknown1);
            Buffer.BlockCopy(bytesUnknown1, 0, header, LRF_OFFSET_HEADER_UNKNOWN1, DWORD_SIZE);

            /* Generate flags */
            header[LRF_OFFSET_HEADER_FLAGS] = this.hdr.flags;

            /* Generate Unknown2 */
            header[LRF_OFFSET_HEADER_UNKNOWN2] = this.hdr.unknown2;

            /* Generate Unknown3 */
            Byte[] bytesUnknown3 = BitConverter.GetBytes(this.hdr.unknown3);
            Buffer.BlockCopy(bytesUnknown3, 0, header, LRF_OFFSET_HEADER_UNKNOWN3, WORD_SIZE);

            /* Generate Unknown4 */
            Byte[] bytesUnknown4 = BitConverter.GetBytes(this.hdr.unknown4);
            Buffer.BlockCopy(bytesUnknown4, 0, header, LRF_OFFSET_HEADER_UNKNOWN4, WORD_SIZE);

            /* Generate Height */
            Byte[] bytesHeight = BitConverter.GetBytes(this.hdr.height);
            Buffer.BlockCopy(bytesHeight, 0, header, LRF_OFFSET_HEADER_HEIGHT, WORD_SIZE);

            /* Generate Width */
            Byte[] bytesWidth = BitConverter.GetBytes(this.hdr.width);
            Buffer.BlockCopy(bytesWidth, 0, header, LRF_OFFSET_HEADER_WIDTH, WORD_SIZE);

            /* Generate Unknown5 */
            header[LRF_OFFSET_HEADER_UNKNOWN5] = this.hdr.unknown5;

            /* Generate Unknown6 */
            header[LRF_OFFSET_HEADER_UNKNOWN6] = this.hdr.unknown6;

            /* Generate Unknown7 */
            Buffer.BlockCopy(this.hdr.unknown7, 0, header, LRF_OFFSET_HEADER_UNKNOWN7, 20);

            /* Generate the TOCObjectID */
            Byte[] bytesTocObjectID = BitConverter.GetBytes(this.hdr.tocObjectID);
            Buffer.BlockCopy(bytesTocObjectID, 0, header, LRF_OFFSET_HEADER_TOCOBJECTID, DWORD_SIZE);

            /* Generate the xmlCompSize */
            Byte[] bytesXMLCompSize = BitConverter.GetBytes(this.hdr.xmlCompSize);
            Buffer.BlockCopy(bytesXMLCompSize, 0, header, LRF_OFFSET_HEADER_COMXMLSIZE, WORD_SIZE);

            /* Generate Thumbnail Image Type */
            Byte[] bytesThumbImageType = BitConverter.GetBytes(this.hdr.thumbImageType);
            Buffer.BlockCopy(bytesThumbImageType, 0, header, LRF_OFFSET_HEADER_THUMBIMAGETYPE, WORD_SIZE);

            /* Generate the gifSize */
            Byte[] bytesGIFSize = BitConverter.GetBytes(this.hdr.gifSize);
            Buffer.BlockCopy(bytesGIFSize, 0, header, LRF_OFFSET_HEADER_GIFSIZE, DWORD_SIZE);

            int currentLRFObjectOffset = this.hdr.xmlCompSize + this.hdr.gifSize + header.Length;
            int diff = currentLRFObjectOffset - hdr.originalLRFObjectOffset;

            /* Generate the ObjectIndexOffset */
            this.hdr.objectIndexOffset = this.hdr.objectIndexOffset + diff;
            Byte[] bytesObjectIndexOffset = BitConverter.GetBytes(this.hdr.objectIndexOffset);
            Buffer.BlockCopy(bytesObjectIndexOffset, 0, header, LRF_OFFSET_HEADER_OBJECTINDEXOFFSET, 8);

            /* Generate the tocObjectOffset */
            this.hdr.tocObjectOffset = this.hdr.tocObjectOffset + diff;
            Byte[] bytesTocObjectOffset = BitConverter.GetBytes(this.hdr.tocObjectOffset);
            Buffer.BlockCopy(bytesTocObjectOffset, 0, header, LRF_OFFSET_HEADER_TOCOBJECTOFFSET, DWORD_SIZE);

            return header;
        }
        /// <summary>
        /// Parse the FileHeader from an LRF byte array
        /// </summary>
        private LRFHeader parseHeader(byte[] lrfBytes)
        {
            LRFHeader hdr = new LRFHeader();
            /* Extract the signature */
            hdr.lrfSignature = new Byte[8];
            Buffer.BlockCopy(lrfBytes, LRF_OFFSET_HEADER_SIGNATURE, hdr.lrfSignature, 0, 8);
            /* Extract the version */
            hdr.version = BitConverter.ToInt16(lrfBytes, LRF_OFFSET_HEADER_VERSION);
            /* Extract the PsuedoEncryption key */
            hdr.pseudoEncryption = BitConverter.ToInt16(lrfBytes, LRF_OFFSET_HEADER_PSEUDOENCRYPTION);
            /* Extract the RootObjectID */
            hdr.rootObjectID = BitConverter.ToInt32(lrfBytes, LRF_OFFSET_HEADER_ROOTOBJECTID);
            /* Extract the NumberOfObjects */
            hdr.numObjects = BitConverter.ToInt64(lrfBytes, LRF_OFFSET_HEADER_NUMOBJECTS);
            /* Extract the ObjectIndexOffset */
            hdr.objectIndexOffset = BitConverter.ToInt64(lrfBytes, LRF_OFFSET_HEADER_OBJECTINDEXOFFSET);
            /* Extract Unknown1 */
            hdr.unknown1 = BitConverter.ToInt32(lrfBytes, LRF_OFFSET_HEADER_UNKNOWN1);
            /* Extract flags */
            hdr.flags = lrfBytes[LRF_OFFSET_HEADER_FLAGS];
            /* Extract Unknown2 */
            hdr.unknown2 = lrfBytes[LRF_OFFSET_HEADER_UNKNOWN2];
            /* Extract Unknown3 */
            hdr.unknown3 = BitConverter.ToInt16(lrfBytes, LRF_OFFSET_HEADER_UNKNOWN3);
            /* Extract Unknown4 */
            hdr.unknown4 = BitConverter.ToInt16(lrfBytes, LRF_OFFSET_HEADER_UNKNOWN4);
            /* Extract Height */
            hdr.height = BitConverter.ToInt16(lrfBytes, LRF_OFFSET_HEADER_HEIGHT);
            /* Extract Width */
            hdr.width = BitConverter.ToInt16(lrfBytes, LRF_OFFSET_HEADER_WIDTH);
            /* Extract Unknown5 */
            hdr.unknown5 = lrfBytes[LRF_OFFSET_HEADER_UNKNOWN5];
            /* Extract Unknown6 */
            hdr.unknown6 = lrfBytes[LRF_OFFSET_HEADER_UNKNOWN6];
            /* Extract Unknown7 */
            hdr.unknown7 = new Byte[20];
            Buffer.BlockCopy(lrfBytes, LRF_OFFSET_HEADER_UNKNOWN7, hdr.unknown7, 0, 20);
            /* Extract the TOCObjectID */
            hdr.tocObjectID = BitConverter.ToInt32(lrfBytes, LRF_OFFSET_HEADER_TOCOBJECTID);
            /* Extract the tocObjectOffset */
            hdr.tocObjectOffset = BitConverter.ToInt32(lrfBytes, LRF_OFFSET_HEADER_TOCOBJECTOFFSET);
            /* Extract the xmlCompSize */
            hdr.xmlCompSize = BitConverter.ToInt16(lrfBytes, LRF_OFFSET_HEADER_COMXMLSIZE);
            /* Extract Thumbnail Image Type */
            hdr.thumbImageType = BitConverter.ToInt16(lrfBytes, LRF_OFFSET_HEADER_THUMBIMAGETYPE);
            /* Extract the gifSize */
            hdr.gifSize = BitConverter.ToInt32(lrfBytes, LRF_OFFSET_HEADER_GIFSIZE);
            return hdr;
        }
        #endregion
        private byte[] bytesGIFData;
        /// <summary>
        /// The file size of the LRF file
        /// </summary>
        private long fileSize;
        private string location;
        private string freeText = "";
        private LRFHeader hdr;
        private List<ObjectEntry> entries = new List<ObjectEntry>();
        private Image coverImage;
        /// <summary>
        /// The GIF data for the LRF thumbnail image
        /// </summary>
        public byte[] GIFData
        {
            get { return bytesGIFData; }
            set
            {
                bytesGIFData = value;
                hdr.gifSize = bytesGIFData.Length;

                /* Only do this if we actually have a thumbnail image */
                if (hdr.gifSize > 0)
                {
                    /* Set the Image Type: 0x12 for PNG, 0x14 for GIF, 0x13 for BM and 0x10 for JPEG */
                    if ((bytesGIFData[1] == 'P') && (bytesGIFData[2] == 'N') && (bytesGIFData[3] == 'G'))
                    {
                        /* It'lineRead a PNG */
                        hdr.thumbImageType = 0x12;
                    }
                    else if ((bytesGIFData[0] == 'B') && (bytesGIFData[1] == 'M'))
                    {
                        /* It'lineRead a BM */
                        hdr.thumbImageType = 0x13;
                    }
                    else if ((bytesGIFData[6] == 'J') && (bytesGIFData[7] == 'F') &&
                         (bytesGIFData[8] == 'I') && (bytesGIFData[9] == 'F'))
                    {
                        /* It'lineRead a JFIF */
                        hdr.thumbImageType = 0x11;
                    }
                    else if ((bytesGIFData[0] == 'G') && (bytesGIFData[1] == 'I') &&
                         (bytesGIFData[2] == 'F') && (bytesGIFData[3] == '8'))
                    {
                        /* It'lineRead a GIF */
                        hdr.thumbImageType = 0x14;
                    }
                    else
                    {
                        /* I give up, lets just call it a GIF and hope nobody notices */
                        hdr.thumbImageType = 0x14;
                    }
                }
            }
        }
        public long FileSize
        {
            get { return fileSize; }
        }
        /// <summary>
        /// The uncompressed UTF-16 info block
        /// </summary>
        public byte[] XMLInfoBlock
        {
            get { return hdr.bytesXMLInfoBlock; }
            set { hdr.bytesXMLInfoBlock = value; updateCompXMLInfoBlock(); }
        }
        List<Tag> l = new List<Tag>();
        SortedList<uint, ObjectEntry> list = new SortedList<uint, ObjectEntry>();
        /// <summary>
        /// Initializes a new instance of the LRFFileReader class, using data from the
        /// specified LRF file.
        /// </summary>
        /// <param name="filename">
        /// The name of the LRF file to read from.
        /// </param>
        #region Utility
        private void FillTagList()
        {
            #region create
            // - 1 = variable number of parameters
            // - 2 = invalid tag
            l.Add(new Tag(0X00, 6, "0bject Start "));
            l.Add(new Tag(0X01, 0, "0bject End "));
            l.Add(new Tag(0X02, 4, "0bject Info Link "));
            l.Add(new Tag(0X03, 4, "Link "));
            l.Add(new Tag(0X04, 4, "Stream Size "));
            l.Add(new Tag(0X05, -1, "Stream Start "));//Mod from 0
            l.Add(new Tag(0X06, 0, "Stream End "));
            l.Add(new Tag(0X07, 4, "Contained Objects List "));
            l.Add(new Tag(0X08, 4, ""));
            l.Add(new Tag(0X09, 4, ""));
            l.Add(new Tag(0X0A, 4, ""));
            l.Add(new Tag(0X0B, -1, ""));
            l.Add(new Tag(0X0C, -2, ""));
            l.Add(new Tag(0X0D, -1, ""));
            l.Add(new Tag(0X0E, 2, ""));
            l.Add(new Tag(0X0F, -2, ""));
            l.Add(new Tag(0X10, -2, ""));
            l.Add(new Tag(0X11, 2, "Font Size "));
            l.Add(new Tag(0X12, 2, "Font Width "));
            l.Add(new Tag(0X13, 2, "Font Escapement "));
            l.Add(new Tag(0X14, 2, "Font Orientation "));
            l.Add(new Tag(0X15, 2, "Font Weight "));
            l.Add(new Tag(0X16, -1, "Font Facename "));
            l.Add(new Tag(0X17, 4, "Text Color "));
            l.Add(new Tag(0X18, 4, "Text Bg Color "));
            l.Add(new Tag(0X19, 2, "Word Space "));
            l.Add(new Tag(0X1A, 2, "Letter Space "));
            l.Add(new Tag(0X1B, 2, "Base Line Skip "));
            l.Add(new Tag(0X1C, 2, "Line Space "));
            l.Add(new Tag(0X1D, 2, "Par Indent "));
            l.Add(new Tag(0X1E, 2, "Par Skip "));
            l.Add(new Tag(0X1F, -2, ""));
            l.Add(new Tag(0X20, -2, ""));
            l.Add(new Tag(0X21, 2, ""));
            l.Add(new Tag(0X22, 2, ""));
            l.Add(new Tag(0X23, 2, ""));
            l.Add(new Tag(0X24, 2, ""));
            l.Add(new Tag(0X25, 2, "Page Height "));
            l.Add(new Tag(0X26, 2, "Page Width "));
            l.Add(new Tag(0X27, 2, ""));
            l.Add(new Tag(0X28, 2, ""));
            l.Add(new Tag(0X29, 6, ""));
            l.Add(new Tag(0X2A, 2, ""));
            l.Add(new Tag(0X2B, 2, ""));
            l.Add(new Tag(0X2C, 2, ""));
            l.Add(new Tag(0X2D, 4, ""));
            l.Add(new Tag(0X2E, 2, ""));
            l.Add(new Tag(0X2F, -2, ""));
            l.Add(new Tag(0X30, -2, ""));
            l.Add(new Tag(0X31, 2, "Block Width "));
            l.Add(new Tag(0X32, 2, "Block Height "));
            l.Add(new Tag(0X33, 2, "Block Rule "));
            l.Add(new Tag(0X34, 4, ""));
            l.Add(new Tag(0X35, 2, ""));
            l.Add(new Tag(0X36, 2, ""));
            l.Add(new Tag(0X37, 4, ""));
            l.Add(new Tag(0X38, 2, ""));
            l.Add(new Tag(0X39, 2, ""));
            l.Add(new Tag(0X3A, 2, ""));
            l.Add(new Tag(0X3B, -2, ""));
            l.Add(new Tag(0X3C, 2, ""));
            l.Add(new Tag(0X3D, 2, ""));
            l.Add(new Tag(0X3E, 2, ""));
            l.Add(new Tag(0X3F, -2, ""));
            l.Add(new Tag(0X40, -2, ""));
            l.Add(new Tag(0X41, 2, "Mini Page Height "));
            l.Add(new Tag(0X42, 2, "Mini Page Width "));
            l.Add(new Tag(0X43, -2, ""));
            l.Add(new Tag(0X44, 4, ""));
            l.Add(new Tag(0X45, 4, ""));
            l.Add(new Tag(0X46, 2, "Location Y "));
            l.Add(new Tag(0X47, 2, "Location X "));
            l.Add(new Tag(0X48, 2, ""));
            l.Add(new Tag(0X49, 8, "Put Sound "));
            l.Add(new Tag(0X4A, 8, "Image Rect "));
            l.Add(new Tag(0X4B, 4, "Image Size "));
            l.Add(new Tag(0X4C, 4, "Image Stream "));
            l.Add(new Tag(0X4D, 0, ""));
            l.Add(new Tag(0X4E, 12, ""));
            l.Add(new Tag(0X4F, -2, ""));
            l.Add(new Tag(0X50, -2, ""));
            l.Add(new Tag(0X51, 2, "Canvas Width "));
            l.Add(new Tag(0X52, 2, "Canvas Height "));
            l.Add(new Tag(0X53, 4, ""));
            l.Add(new Tag(0X54, 2, "Stream Flags "));
            l.Add(new Tag(0X55, -1, ""));
            l.Add(new Tag(0X56, -1, ""));
            l.Add(new Tag(0X57, 2, ""));
            l.Add(new Tag(0X58, 2, ""));
            l.Add(new Tag(0X59, -1, "Font File Name "));
            l.Add(new Tag(0X5A, -1, ""));
            l.Add(new Tag(0X5B, 4, "View Point "));
            l.Add(new Tag(0X5C, -1, "Page List "));
            l.Add(new Tag(0X5D, -1, "Font Face Name "));
            l.Add(new Tag(0X5E, 2, ""));
            l.Add(new Tag(0X5F, -2, ""));
            l.Add(new Tag(0X60, -2, ""));
            l.Add(new Tag(0X61, 2, ""));
            l.Add(new Tag(0X62, 0, ""));
            l.Add(new Tag(0X63, 0, ""));
            l.Add(new Tag(0X64, 0, ""));
            l.Add(new Tag(0X65, 0, ""));
            l.Add(new Tag(0X66, 0, ""));
            l.Add(new Tag(0X67, 0, ""));
            l.Add(new Tag(0X68, 0, ""));
            l.Add(new Tag(0X69, 0, ""));
            l.Add(new Tag(0X6A, 0, ""));
            l.Add(new Tag(0X6B, 0, ""));
            l.Add(new Tag(0X6C, 8, "Jump To "));
            l.Add(new Tag(0X6D, -1, ""));
            l.Add(new Tag(0X6E, 0, ""));
            l.Add(new Tag(0X6F, -2, ""));
            l.Add(new Tag(0X70, -2, ""));
            l.Add(new Tag(0X71, 0, ""));
            l.Add(new Tag(0X72, 0, ""));
            l.Add(new Tag(0X73, 10, "Ruled Line "));
            l.Add(new Tag(0X74, -2, ""));
            l.Add(new Tag(0X75, 2, "Ruby Align "));
            l.Add(new Tag(0X76, 2, "Ruby Overhang "));
            l.Add(new Tag(0X77, 2, "Emp Dots Position "));
            l.Add(new Tag(0X78, 4, "Emp Dots Code "));// mod from -1
            l.Add(new Tag(0X79, 2, "Emp Line Position "));
            l.Add(new Tag(0X7A, 2, "Emp Line Mode "));
            l.Add(new Tag(0X7B, 4, "Child Page Tree "));
            l.Add(new Tag(0X7C, 4, "Parent Page Tree "));
            l.Add(new Tag(0X7D, -2, ""));
            l.Add(new Tag(0X7E, -2, ""));
            l.Add(new Tag(0X7F, -2, ""));
            l.Add(new Tag(0X80, -2, ""));
            l.Add(new Tag(0X81, 0, "Italic ","<i>"));
            l.Add(new Tag(0X82, 0, "Italic ","</i>"));
            l.Add(new Tag(0X83, -2, ""));
            l.Add(new Tag(0X84, -2, ""));
            l.Add(new Tag(0X85, -2, ""));
            l.Add(new Tag(0X86, -2, ""));
            l.Add(new Tag(0X87, -2, ""));
            l.Add(new Tag(0X88, -2, ""));
            l.Add(new Tag(0X89, -2, ""));
            l.Add(new Tag(0X8A, -2, ""));
            l.Add(new Tag(0X8B, -2, ""));
            l.Add(new Tag(0X8C, -2, ""));
            l.Add(new Tag(0X8D, -2, ""));
            l.Add(new Tag(0X8E, -2, ""));
            l.Add(new Tag(0X8F, -2, ""));
            l.Add(new Tag(0X90, -2, ""));
            l.Add(new Tag(0X91, -2, ""));
            l.Add(new Tag(0X92, -2, ""));
            l.Add(new Tag(0X93, -2, ""));
            l.Add(new Tag(0X94, -2, ""));
            l.Add(new Tag(0X95, -2, ""));
            l.Add(new Tag(0X96, -2, ""));
            l.Add(new Tag(0X97, -2, ""));
            l.Add(new Tag(0X98, -2, ""));
            l.Add(new Tag(0X99, -2, ""));
            l.Add(new Tag(0X9A, -2, ""));
            l.Add(new Tag(0X9B, -2, ""));
            l.Add(new Tag(0X9C, -2, ""));
            l.Add(new Tag(0X9D, -2, ""));
            l.Add(new Tag(0X9E, -2, ""));
            l.Add(new Tag(0X9F, -2, ""));
            l.Add(new Tag(0XA0, -2, ""));
            l.Add(new Tag(0XA1, 4, "Begin P ", "<P>"));
            l.Add(new Tag(0XA2, 0, "End P ", "</P>"));
            l.Add(new Tag(0XA3, -2, ""));
            l.Add(new Tag(0XA4, -2, ""));
            l.Add(new Tag(0XA5, -1, "Koma Gaiji "));
            l.Add(new Tag(0XA6, 0, "Koma Emp Dot Char "));
            l.Add(new Tag(0XA7, 4, "Begin Button ",""));
            l.Add(new Tag(0XA8, 0, "End Button ",""));
            l.Add(new Tag(0XA9, 0, "Begin Ruby "));
            l.Add(new Tag(0XAA, 0, "End Ruby "));
            l.Add(new Tag(0XAB, 0, "Begin Ruby Base "));
            l.Add(new Tag(0XAC, 0, "End Ruby Base "));
            l.Add(new Tag(0XAD, 0, "Begin Ruby Text "));
            l.Add(new Tag(0XAE, 0, "End Ruby Text "));
            l.Add(new Tag(0XAF, -2, ""));
            l.Add(new Tag(0XB0, -2, ""));
            l.Add(new Tag(0XB1, 0, "Koma Yokomoji "));
            l.Add(new Tag(0XB2, 0, ""));
            l.Add(new Tag(0XB3, 0, "Tate "));
            l.Add(new Tag(0XB4, 0, "Tate "));
            l.Add(new Tag(0XB5, 0, "Nekase "));
            l.Add(new Tag(0XB6, 0, "Nekase "));
            l.Add(new Tag(0XB7, 0, "Begin Sup "));
            l.Add(new Tag(0XB8, 0, "End Sup "));
            l.Add(new Tag(0XB9, 0, "Begin Sub "));
            l.Add(new Tag(0XBA, 0, "End Sub "));
            l.Add(new Tag(0XBB, 0, ""));
            l.Add(new Tag(0XBC, 0, ""));
            l.Add(new Tag(0XBD, 0, ""));
            l.Add(new Tag(0XBE, 0, ""));
            l.Add(new Tag(0XBF, -2, ""));
            l.Add(new Tag(0XC0, -2, ""));
            l.Add(new Tag(0XC1, 0, "Begin Emp Line "));
            l.Add(new Tag(0XC2, 0, ""));
            l.Add(new Tag(0XC3, 2, "Begin Draw Char "));
            l.Add(new Tag(0XC4, 0, "End Draw Char "));
            l.Add(new Tag(0XC5, 2, ""));
            l.Add(new Tag(0XC6, 2, ""));
            l.Add(new Tag(0XC7, 0, ""));
            l.Add(new Tag(0XC8, 2, "Koma Auto Spacing "));
            l.Add(new Tag(0XC9, 0, ""));
            l.Add(new Tag(0XCA, 2, "Space ", "&nbsp;"));
            l.Add(new Tag(0XCB, -1, ""));
            l.Add(new Tag(0XCC, 2, ""));
            l.Add(new Tag(0XCD, -2, ""));
            l.Add(new Tag(0XCE, -2, ""));
            l.Add(new Tag(0XCF, -2, ""));
            l.Add(new Tag(0XD0, -2, ""));
            l.Add(new Tag(0XD1, -1, "Koma Plot "));
            l.Add(new Tag(0XD2, 0, "EOL ",""));
            l.Add(new Tag(0XD3, -2, ""));
            l.Add(new Tag(0XD4, 2, "Wait "));
            l.Add(new Tag(0XD5, -2, ""));
            l.Add(new Tag(0XD6, 0, "Sound Stop "));
            l.Add(new Tag(0XD7, 14, "Move Obj "));
            l.Add(new Tag(0XD8, 4, "Book Font "));
            l.Add(new Tag(0XD9, 8, "Koma Plot Text "));
            l.Add(new Tag(0XDA, 2, ""));
            l.Add(new Tag(0XDB, 2, ""));
            l.Add(new Tag(0XDC, 2, ""));
            l.Add(new Tag(0XDD, 2, "Char Space "));
            l.Add(new Tag(0XDE, -2, ""));
            l.Add(new Tag(0XDF, -2, ""));
            l.Add(new Tag(0XE0, -2, ""));
            l.Add(new Tag(0XE1, -2, ""));
            l.Add(new Tag(0XE2, -2, ""));
            l.Add(new Tag(0XE3, -2, ""));
            l.Add(new Tag(0XE4, -2, ""));
            l.Add(new Tag(0XE5, -2, ""));
            l.Add(new Tag(0XE6, -2, ""));
            l.Add(new Tag(0XE7, -2, ""));
            l.Add(new Tag(0XE8, -2, ""));
            l.Add(new Tag(0XE9, -2, ""));
            l.Add(new Tag(0XEA, -2, ""));
            l.Add(new Tag(0XEB, -2, ""));
            l.Add(new Tag(0XEC, -2, ""));
            l.Add(new Tag(0XED, -2, ""));
            l.Add(new Tag(0XEE, -2, ""));
            l.Add(new Tag(0XEF, -2, ""));
            l.Add(new Tag(0XF0, -2, ""));
            l.Add(new Tag(0XF1, 2, "Line Width "));
            l.Add(new Tag(0XF2, 4, "Line Color "));
            l.Add(new Tag(0XF3, 4, "Fill Color "));
            l.Add(new Tag(0XF4, 2, "Line Mode "));
            l.Add(new Tag(0XF5, 4, "Move To "));
            l.Add(new Tag(0XF6, 4, "Line To "));
            l.Add(new Tag(0XF7, 4, "Draw Box "));
            l.Add(new Tag(0XF8, 4, "Draw Ellipse "));
            l.Add(new Tag(0XF9, 6, ""));
            l.Add(new Tag(0XFA, -2, ""));
            l.Add(new Tag(0XFB, -2, ""));
            l.Add(new Tag(0XFC, -2, ""));
            l.Add(new Tag(0XFD, -2, ""));
            l.Add(new Tag(0XFE, -2, ""));
            l.Add(new Tag(0XFF, -2, ""));

            #endregion
        }
        private uint ReadUint(byte[] buffer)
        {
            uint i = Convert.ToUInt32(((uint)buffer[3] << 24) + ((uint)buffer[2] << 16) + ((uint)buffer[1] << 8) + ((uint)buffer[0]));
            return i;
        }
        private uint ReadUint(byte[] b, ref int start)
        {
            byte[] buffer = new byte[4];
            Buffer.BlockCopy(b, start, buffer, 0, 4);
            uint i = Convert.ToUInt32(((uint)buffer[3] << 24) + ((uint)buffer[2] << 16) + ((uint)buffer[1] << 8) + ((uint)buffer[0]));
            start += 4;
            return i;
        }
        private uint ReadShortUint(byte[] buffer)
        {
            uint i = ((uint)buffer[1] << 8) + ((uint)buffer[0]);
            return i;
        }
        private uint ReadShortUint(byte[] b, ref int start)
        {
            byte[] buffer = new byte[2];
            Buffer.BlockCopy(b, start, buffer, 0, 2);
            uint i = ((uint)buffer[1] << 8) + ((uint)buffer[0]);
            start += 2;
            return i;
        }
        private void ParseEntry(ObjectEntry entry)
        {
            try
            {
                List<LRFObject> lrfObjects = new List<LRFObject>();
                int startEntry = 0;
                entry.obType = entry.buffer[6];
                LRFObject onj = new LRFObject();
                entry.lrfObjects.Add(onj);
                onj.F500[0] = entry.buffer[startEntry];
                onj.F500[1] = entry.buffer[startEntry + 1];
                startEntry += 2;
                if (onj.F500[0] == 0x00)
                {
                    onj.data = new byte[6];
                    Buffer.BlockCopy(entry.buffer, 2, onj.data, 0, 6);
                    startEntry += 6;
                    byte[] localBuffer = new byte[4];
                    onj.t = l[onj.F500[0]];
                    onj.obType = onj.F500[0];
                }
                while (startEntry + 1 < entry.size)
                {
                    onj = new LRFObject();
                    entry.lrfObjects.Add(onj);
                    onj.start = startEntry;
                    onj.F500[0] = entry.buffer[startEntry];
                    onj.F500[1] = entry.buffer[startEntry + 1];
                    startEntry += 2;
                    onj.t = l[onj.F500[0]];
                    if (onj.F500[1] == 0xF5)
                        onj.obType = onj.F500[0];
                    else
                    {
                    }
                    int datasize = onj.t.length;
                    if (datasize >= 0)
                        try
                        {
                            onj.data = new byte[datasize];
                            for (int u = 0; u < datasize; u++)
                            {
                                onj.data[u] = entry.buffer[startEntry];
                                startEntry++;
                            }
                            if (onj.obType == 0x04)
                                entry.streamSize = ReadUint(onj.data);
                        }
                        catch (Exception ex) { }
                    else if (onj.obType == 0x05)
                    {
                        onj.data = new byte[entry.streamSize];
                        for (int u = 0; u < entry.streamSize; u++)
                        {
                            onj.data[u] = entry.buffer[startEntry];
                            startEntry++;
                        }
                        byte[] b = new byte[4];
                        Buffer.BlockCopy(onj.data, 0, b, 0, 4);
                        uint data = ReadUint(b);
                    }
                    else
                    {
                        try
                        {
 /*                           if (onj.obType == 0x0b)
                            {
                                int size = entry.buffer[startEntry];
                                onj.data = new byte[size*4];
                                startEntry+=2;
                                for (int inTime = 0; inTime < onj.data.Length; inTime++)
                                {
                                    onj.data[inTime] = entry.buffer[startEntry];
                                    startEntry++;
                                }
                            }
                            else*/
                            {
                                List<byte> ints = new List<byte>();
                                while ((startEntry + 2 < entry.size))
                                {
                                    ints.Add(entry.buffer[startEntry]);
                                    startEntry++;
                                    if ((entry.buffer[startEntry + 1] == 0xF5) && (entry.buffer[startEntry] != 0x00) && (l[entry.buffer[startEntry]].length >= 0))
                                    {
                                        break;
                                    }
                                }
                                onj.data = new byte[ints.Count];
                                for (int k = 0; k < ints.Count; k++)
                                    onj.data[k] = ints[k];
                            }
                        }
                        catch { }
                    }
                    onj.length = onj.data.Length;
                }
            }
            catch (Exception ex) { }
        }
        #endregion
        public ObjectEntry root;
        public List<Page> pages;
        public List<TextBlock> textblocks;
        public int textPagesNumber = 0;
        public int totalTextSize = 0;
        public LRFFileReader(string filename)
        {
            // Structure : Header (54 Bytes) + XMLcompressed data + Data
            this.location = filename;
            this.fileSize = new FileInfo(filename).Length;
            #region ReadEntireLRFFile
            /* Used to store the whole LRF file as bytes */
            byte[] lrfBytes = new Byte[this.fileSize];
            /* Read the entire LRF file into lrfBytes */
            byte[] tempBuffer = new Byte[4096];
            FileStream fs = new FileStream(filename, FileMode.Open);
            int numBytesRead = 0;
            int curPosition = 0;
            while (0 != (numBytesRead = fs.Read(tempBuffer, 0, 4096)))
            {
                Buffer.BlockCopy(tempBuffer, 0, lrfBytes, curPosition, numBytesRead);
                curPosition += numBytesRead;
            }
            fs.Close();
            tempBuffer = null;
            #endregion
            hdr = parseHeader(lrfBytes);
            #region ReadCompressedInfoBlock
            /* Allocate the temporary memory required for the Compressed Info Block */
            int infoLength = BitConverter.ToInt16(lrfBytes, LRF_OFFSET_HEADER_COMXMLSIZE);
            byte[] bytesInfoData = new Byte[infoLength - DWORD_SIZE];
            /* Copy the Info block into the temporary buffer */
            Buffer.BlockCopy(lrfBytes, LRF_OFFSET_COMXML_DATA, bytesInfoData, 0, (infoLength - DWORD_SIZE));
            #endregion
            #region ReadGIFData
            /* Allocate the required memory for the GIF */
            int gifLength = BitConverter.ToInt32(lrfBytes, LRF_OFFSET_HEADER_GIFSIZE);
            if (gifLength > 0)
            {
                bytesGIFData = new Byte[gifLength];
                /* Copy the GIF data into bytesGIFData */
                Buffer.BlockCopy(lrfBytes, (LRF_OFFSET_UNCOMXML_SIZE + infoLength), bytesGIFData, 0, gifLength);
            }
            #endregion
            #region ReadLRFObjectPayload
            /* Remember where we originally started */
            hdr.originalLRFObjectOffset = (LRF_OFFSET_UNCOMXML_SIZE + infoLength) + gifLength;
            /* Allocate the required memory for the data */
            int lrfDataOffset = (LRF_OFFSET_UNCOMXML_SIZE + infoLength) + gifLength;
            int lrfDataLength = (int)this.fileSize - lrfDataOffset;
            hdr.bytesLRFObjectData = new Byte[lrfDataLength];
            /* Copy the LRF data into bytesLRFObjectData */
            Buffer.BlockCopy(lrfBytes, lrfDataOffset, hdr.bytesLRFObjectData, 0, lrfDataLength);
            #endregion
            #region UncompressInfoBlock
            /* Allocate the required memory for the Uncompressed Info Block */
            int infoUncompressedLength = BitConverter.ToInt32(lrfBytes, LRF_OFFSET_UNCOMXML_SIZE);
            hdr.bytesXMLInfoBlock = new byte[infoUncompressedLength];
            /* Uncompress the info block */
            Inflater i = new Inflater();
            i.SetInput(bytesInfoData);
            i.Inflate(hdr.bytesXMLInfoBlock);
            #endregion
            FillTagList();
            #region Read index
            int start = (int)hdr.objectIndexOffset;
            for (int u = 0; u < hdr.numObjects; u++)
            {
                ObjectEntry entry = new ObjectEntry();
                entry.id = ReadUint(lrfBytes,ref start);
                if (entry.id == 0x6ec)
                {
                }

                entry.offset = ReadUint(lrfBytes,ref start);
                entry.size = ReadUint(lrfBytes,ref start);
                entry.reserved = ReadUint(lrfBytes, ref start);
                entry.buffer = new byte[entry.size];
                Buffer.BlockCopy(lrfBytes, (int)entry.offset, entry.buffer, 0, (int)entry.size);
                ParseEntry(entry);
                entries.Add(entry);
                try
                {
                    list.Add(entry.id,entry);
                }
                catch { }
            }
            #endregion
        }
        public string Comment
        {
            get { return freeText; }
            set { freeText = value; }
        }
        public string FileName
        {
            get { return location; }
            set { location = value; }
        }
        public Image CoverImage
        {
            get { return coverImage; }
            set { coverImage = value; }
        }
        public void Parse()
        {
            XmlTextReader textReader = new XmlTextReader(new MemoryStream(XMLInfoBlock));
            textReader.MoveToContent();
            while (textReader.Read())
            {
                /* Only bother with this node if it'lineRead an element containing data */
                if ((textReader.NodeType == XmlNodeType.Element) && (!textReader.IsEmptyElement))
                {
                    switch (textReader.Name)
                    {
                        case "Page":
                            string page = textReader.ReadElementString();
                            break;
                        case "Creator":
                            Creator = textReader.ReadElementString();
                            break;
                        case "Language":
                            Language = textReader.ReadElementString();
                            break;
                        case "BookID":
                            BookId = textReader.ReadElementString();
                            break;
                        case "Author":
                            if (textReader.GetAttribute("reading") != null)
                            {
                                Author = textReader.GetAttribute("reading");
                            }
                            Author = textReader.ReadElementString();
                            break;
                        case "Title":
                            if (textReader.GetAttribute("reading") != null)
                            {
                                Title = textReader.GetAttribute("reading");
                            }
                            Title = textReader.ReadElementString();
                            break;
                        case "CreationDate":
                            Date = textReader.ReadElementString().Trim();
                            break;
                        case "Publisher":
                            Publisher = textReader.ReadElementString().Trim();
                            break;
                        case "FreeText":
                            freeText = textReader.ReadElementString().Trim();
                            break;
                    }
                }
            }
        }
        public void Structure()
        {
            uint tocind = (uint)hdr.tocObjectID;
            uint bookIndex = (uint)hdr.rootObjectID;
            root = list[bookIndex];
            LRFObject lro = root.lrfObjects[1];
            uint pageTreeIndex = ReadShortUint(lro.data);
            ObjectEntry pageTreeObject = list[pageTreeIndex];
            int st = 0;
            uint pageNumber = ReadShortUint(pageTreeObject.lrfObjects[1].data, ref st);
            byte[] buffer = new byte[4];
            pages = new List<Page>();
            textblocks = new List<TextBlock>();
            ObjectEntry imageBlock = null;
            List<ObjectEntry> textPages = new List<ObjectEntry>();
            for(int u = 0; u <pageNumber;u++)
            {
                #region read one page
                uint uu = ReadUint(pageTreeObject.lrfObjects[1].data, ref st);
                ObjectEntry oo = list[uu];
                Page p = new Page(oo);
                p.pageText = "";
                pages.Add(p);
                //lrfObjects[1] : list of related entries, block, text, attributes ...
                #region get all objects
                int start = 0;
                uint obNb = ReadShortUint(oo.lrfObjects[1].data, ref start);
                if (u == 0xf)
                { }
                while (start+4 < oo.lrfObjects[1].data.Length)
                {
                    try
                    {
                        ObjectEntry obj = list[ReadUint(oo.lrfObjects[1].data, ref start)];
                        p.loclist.Add(obj);
                        if (obj.obType == 0x0C)
                        {
                            if (imageBlock == null)
                                imageBlock = obj;
                        }
                    }
                    catch { }
                }
                #endregion
                //lrfObjects[2] : link contains Bloc attribute
                try
                {
                    p.attribs = list[ReadUint(oo.lrfObjects[2].data)];
                }
                catch { }
                //lrfObjects[3] : pointeur sur le père
                //lrfObjects[6] : stream start contains data (list of blocks or canvases)

                byte[] locBuf = oo.lrfObjects[6].data; // Stream object
                int locSt = 0;
                while (locSt < locBuf.Length)
                {
                    #region
                    if ((locBuf[locSt] == 0x03) && (locBuf[locSt + 1] == 0xF5))
                    {
                        locSt += 2;// SkipBit 03F5
                        // Block ou Canvas
                        ObjectEntry ob = list[ReadUint(locBuf, ref locSt)];
                        byte[] textBuf = ob.lrfObjects[4].data;
                        switch (ob.obType)
                        {
                            case 0x0D:
                                #region Add canvas
                                Canvas c = new Canvas(ob);
                                p.canvases.Add(c);
                                Buffer.BlockCopy(ob.lrfObjects[2].data, 0, buffer, 0, 2);
                                c.Height = (int)ReadShortUint(buffer);
                                Buffer.BlockCopy(ob.lrfObjects[6].data, 0, buffer, 0, 2);
                                c.Height = (int)ReadShortUint(buffer);
                                break;
                                #endregion
                            case 0x06:
                                #region Add block
                                Block b = new Block(ob);
                                p.blocks.Add(b);
                                //ob.lrfObjects[1] contains Bloc attribute
                                b.attribs = list[ReadUint(ob.lrfObjects[1].data)];
                                //ob.lrfObjects[4] contains child address
                                int txtSt = 0;
                                while (txtSt < textBuf.Length)
                                {
                                    #region Add text
                                    txtSt += 2;// SkipBit 03F5
                                    ObjectEntry ot = list[ReadUint(textBuf, ref txtSt)];
                                    TextBlock tex = new TextBlock(ot);
                                    b.text.Add(tex);
                                    textblocks.Add(tex);
                                    tex.attribs = list[ReadUint(ot.lrfObjects[1].data)];
                                                              try
                                                              {
                                                                string t = tex.Uncompress();
                                                                p.pageText += t;
                                                              }
                                                              catch(Exception ex) { }
                                    #endregion
                                    textPages.Add(ot);
                                    textPagesNumber++;
                                    totalTextSize += ot.lrfObjects[4].data.Length;
                                }
                                break;
                                #endregion
                        }
                    }
                    else
                    {
                        Tag t = l[locBuf[locSt]];
                        locSt += 2;
                        locSt += t.length;
                        p.others.Add(t);
                    }
                    #endregion
                }
                #endregion
            }
            if (imageBlock != null)
            {
                ObjectEntry image = list[ReadUint(imageBlock.lrfObjects[3].data)];
                uint size = ReadUint(imageBlock.lrfObjects[2].data);
                uint code = ReadUint(image.lrfObjects[3].data);
                if (code == 0xE0FFD8FF)
                {
                    MemoryStream mm = new MemoryStream(image.lrfObjects[3].data);
                    //JPEG image
                    coverImage = Image.FromStream(mm);
                }
            }
        }
    }
    public class Page
    {
        ObjectEntry page;
        public string pageText;
        public List<ObjectEntry> loclist = new List<ObjectEntry>();
        public List<Block> blocks = new List<Block>();
        public List<Canvas> canvases = new List<Canvas>();
        public List<Tag> others = new List<Tag>();
        public ObjectEntry attribs ;
        public Page(ObjectEntry o)
        {
            page = o;
        }
        public override string ToString()
        {
            return page.ToString();
        }
    }
    public class Block
    {
        ObjectEntry block;
        public ObjectEntry attribs;
        public List<TextBlock> text = new List<TextBlock>();
        public Block(ObjectEntry o)
        {
            block = o;
        }
        public override string ToString()
        {
            return block.ToString();
        }
    }
    public class Canvas
    {
        ObjectEntry canvas;
        public List<Block> blocs = new List<Block>();
        public int Height;
        public int Width;
        public Canvas(ObjectEntry c)
        {
            canvas = c;
        }
        public override string ToString()
        {
            return canvas.ToString();
        }
    }
    public class TextBlock
    {
        public ObjectEntry text;
        public string t;
        
        public ObjectEntry attribs;
        public TextBlock(ObjectEntry t)
        {
            text = t;
        }
        public string Uncompress()
        {
            LRFObject lro = text.lrfObjects[4];
            int len = text.lrfObjects[4].length;
            byte[] data = new byte[len-4];
            Buffer.BlockCopy(text.lrfObjects[4].data, 4, data, 0, len-4 );
            byte[] buffer = Decompress(data);
            List<string> tags = new List<string>();
            int u = 0;
            string truc = Encoding.Unicode.GetString(buffer);
            string s = "";
            List<byte> txt = new List<byte>();
            List<List<byte>> txts = new List<List<byte>>();
            while (u < buffer.Length - 1)
            {
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
                        txt.Add(buffer[u+1]);
                    }
                         u+=2;
                }
            }
            t = "";
            if (s != "")
                tags.Add(s);
            foreach(string st in tags)
                t += st; ;
            return t;
        }

        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream gzip = new DeflaterOutputStream(output);
            gzip.Write(data, 0, data.Length);
            gzip.Close();
            return output.ToArray();
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
      /*
        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            DeflateStream gzip = new DeflateStream(output,
                              CompressionMode.Compress);
            gzip.Write(data, 0, data.Length);
            gzip.Close();
            return output.ToArray();
        }
        public static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream();
            input.Write(data, 0, data.Length);
            input.Position = 0;
            DeflateStream gzip = new DeflateStream(input,
                              CompressionMode.Decompress);
            MemoryStream output = new MemoryStream();
            byte[] buff = new byte[64];
            int read = -1;
            read = gzip.Read(buff, 0, Math.Min(buff.Length,data.Length));
            while (read > 0)
            {
                output.Write(buff, 0, read);
                read = gzip.Read(buff, 0, buff.Length);
            }
            gzip.Close();
            return output.ToArray();
        }
        */
        public override string ToString()
        {
            return text.ToString();
        }

    }
    public class Tag
    {
        public int id;
        public int length;
        public string name;
        public string value = "";
        public Tag(int id, int length, string name)
        {
            this.id = id;
            this.length = length;
            this.name = name;
        }
        public Tag(int id, int length, string name, string value)
        {
            this.id = id;
            this.length = length;
            this.name = name;
            this.value = value;
        }
    }
    public class ObjectEntry
    {
        public uint obType;
        public uint id;
        public uint offset;
        public uint size;
        public uint reserved;
        public uint streamSize;
        public byte[] buffer;
        public List<LRFObject> lrfObjects = new List<LRFObject>();
        public override string ToString()
        {
            string text = id.ToString("x2")+" "+ size.ToString()+ " ";
            switch (obType)
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
    public class LRFObject
    {
        public byte[] F500 = new byte[2];
        public uint obType;
        public byte[] data;
        public Tag t;
        public int start;
        public int length;
        public override string ToString()
        {
            return t.name + " " + data.Length.ToString(); ;
        }
    }
    public class LRFHeader
    {
        #region Private data
        /// <summary>
        /// LRF Signature
        /// </summary>
        public byte[] lrfSignature;
        /// <summary>
        /// Keeps track of how much the FileHeader has grown/shrunk.
        /// Without this the offsets would get messed up.
        /// </summary>
        public int originalLRFObjectOffset;
        /// <summary>
        /// LRF Version
        /// </summary>
        public int version;
        /// <summary>
        /// LRF PsuedoEncryption
        /// </summary>
        public int pseudoEncryption;
        /// <summary>
        /// LRF RootObjectID
        /// </summary>
        public int rootObjectID;
        /// <summary>
        /// LRF NumObjects
        /// </summary>
        public Int64 numObjects;
        /// <summary>
        /// LRF ObjectIndexOffset
        /// </summary>
        public Int64 objectIndexOffset;
        /// <summary>
        /// LRF Unknown1
        /// </summary>
        public int unknown1;
        /// <summary>
        /// LRF Flags
        /// </summary>
        public byte flags;
        /// <summary>
        /// LRF Unknown2
        /// </summary>
        public byte unknown2;
        /// <summary>
        /// LRF Unknown3
        /// </summary>
        public int unknown3;
        /// <summary>
        /// LRF Unknown4
        /// </summary>
        public int unknown4;
        /// <summary>
        /// LRF Height
        /// </summary>
        public int height;
        /// <summary>
        /// LRF Width
        /// </summary>
        public int width;
        /// <summary>
        /// LRF Unknown5
        /// </summary>
        public byte unknown5;
        /// <summary>
        /// LRF Unknown6
        /// </summary>
        public byte unknown6;
        /// <summary>
        /// LRF Unknown7
        /// </summary>
        public byte[] unknown7;
        /// <summary>
        /// LRF TOCObjectID
        /// </summary>
        public int tocObjectID;
        /// <summary>
        /// LRF TOCObjectOffset
        /// </summary>
        public int tocObjectOffset;
        /// <summary>
        /// LRF XMLCompSize
        /// </summary>
        public int xmlCompSize;
        /// <summary>
        /// LRF Thumbnail Image Type
        /// </summary>
        public int thumbImageType;
        /// <summary>
        /// LRF GIFSize
        /// </summary>
        public int gifSize;
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
        #endregion
    }
}

