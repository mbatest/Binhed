using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;

namespace VideoFiles
{
    public class AVIFile : LOCALIZED_DATA
    {
        #region Members
        private ELEMENTARY_TYPE riff;
        private ELEMENTARY_TYPE fileSize;
        private ELEMENTARY_TYPE fType;
        AVIHEADER aviHeader;
        #endregion
        List<AVISTREAMINFO> aviStreamInfo = new List<AVISTREAMINFO>();
        List<Chunk> chunks;
        public ELEMENTARY_TYPE Riff
        {
            get { return riff; }
            set { riff = value; }
        }
        public ELEMENTARY_TYPE FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }
        public ELEMENTARY_TYPE FileType
        {
            get { return fType; }
            set { fType = value; }
        }
        public AVIHEADER AviHeader
        {
            get { return aviHeader; }
            set { aviHeader = value; }
        }
        public List<Chunk> Chunks
        {
            get { return chunks; }
            set { chunks = value; }
        }
        public List<AVISTREAMINFO> AviStreamInfo
        {
            get { return aviStreamInfo; }
            set { aviStreamInfo = value; }
        }
        public AVIFile(string FileName)
        {
            BitStreamReader sw = new BitStreamReader(FileName, false);
            PositionOfStructureInFile = sw.Position;
            Riff = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);//"Header");
            FileSize = new ELEMENTARY_TYPE(sw, 0, typeof(int));//"Size");
            FileType = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);//"Type");
            chunks = new List<Chunk>();
            while (!sw.Eof)
            {
                Chunk ch = new Chunk(sw, 0, null);
                chunks.Add(ch);
            }
        }
    }
    public class AVIHEADER : LOCALIZED_DATA
    {
        private ELEMENTARY_TYPE av;
        private ELEMENTARY_TYPE list;
        private ELEMENTARY_TYPE lstSize;
        private ELEMENTARY_TYPE headerL;
        private ELEMENTARY_TYPE sSize;
        private ELEMENTARY_TYPE microSecPerFrame;
        private ELEMENTARY_TYPE maxBytesperSec;
        private ELEMENTARY_TYPE granularity;
        private ELEMENTARY_TYPE flags;
        private ELEMENTARY_TYPE nbTotalFrames;
        private ELEMENTARY_TYPE initialFrame;
        private ELEMENTARY_TYPE nbStreams;
        private ELEMENTARY_TYPE suggestedBufferSize;
        private ELEMENTARY_TYPE width;
        private ELEMENTARY_TYPE height;
        public ELEMENTARY_TYPE List
        {
            get { return list; }
            set { list = value; }
        }
        public ELEMENTARY_TYPE LstSize
        {
            get { return lstSize; }
            set { lstSize = value; }
        }
        public ELEMENTARY_TYPE HeaderL
        {
            get { return headerL; }
            set { headerL = value; }
        }
        public ELEMENTARY_TYPE MainAviHeader
        {
            get { return av; }
            set { av = value; }
        }
        public ELEMENTARY_TYPE Size
        {
            get { return sSize; }
            set { sSize = value; }
        }
        public ELEMENTARY_TYPE MicroSecPerFrame
        {
            get { return microSecPerFrame; }
            set { microSecPerFrame = value; }
        }
        public ELEMENTARY_TYPE MaxBytesperSec
        {
            get { return maxBytesperSec; }
            set { maxBytesperSec = value; }
        }
        public ELEMENTARY_TYPE Granularity
        {
            get { return granularity; }
            set { granularity = value; }
        }
        public ELEMENTARY_TYPE Flags
        {
            get { return flags; }
            set { flags = value; }
        }
        public ELEMENTARY_TYPE NbTotalFrames
        {
            get { return nbTotalFrames; }
            set { nbTotalFrames = value; }
        }
        public ELEMENTARY_TYPE InitialFrame
        {
            get { return initialFrame; }
            set { initialFrame = value; }
        }
        public ELEMENTARY_TYPE NbStreams
        {
            get { return nbStreams; }
            set { nbStreams = value; }
        }
        public ELEMENTARY_TYPE SuggestedBufferSize
        {
            get { return suggestedBufferSize; }
            set { suggestedBufferSize = value; }
        }
        public ELEMENTARY_TYPE Width
        {
            get { return width; }
            set { width = value; }
        }
        public ELEMENTARY_TYPE Height
        {
            get { return height; }
            set { height = value; }
        }

        private ELEMENTARY_TYPE scale;
        private ELEMENTARY_TYPE rate;
        private ELEMENTARY_TYPE start;
        private ELEMENTARY_TYPE length;

        public ELEMENTARY_TYPE Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        public ELEMENTARY_TYPE Rate
        {
            get { return rate; }
            set { rate = value; }
        }
        public ELEMENTARY_TYPE Start
        {
            get { return start; }
            set { start = value; }
        }
        public ELEMENTARY_TYPE Length
        {
            get { return length; }
            set { length = value; }
        }
        public AVIHEADER(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            MainAviHeader = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);//"MainAviHeader");
            //
            Size = new ELEMENTARY_TYPE(sw, 0, typeof(int));//"Size");
            MicroSecPerFrame = new ELEMENTARY_TYPE(sw, 0, typeof(int));//"MicroSecPerFrame");
            MaxBytesperSec = new ELEMENTARY_TYPE(sw, 0, typeof(int));//"MaxBytesperSec");
            Granularity = new ELEMENTARY_TYPE(sw, 0, typeof(int));//"Granularity");
            Flags = new ELEMENTARY_TYPE(sw, 0, typeof(int));//"Flags");
            NbTotalFrames = new ELEMENTARY_TYPE(sw, 0, typeof(int));//"TotalFrames");
            InitialFrame = new ELEMENTARY_TYPE(sw, 0, typeof(int));//"InitialFrame");
            NbStreams = new ELEMENTARY_TYPE(sw, 0, typeof(int));//"Streams");
            SuggestedBufferSize = new ELEMENTARY_TYPE(sw, 0, typeof(int));//"Suggested Buffer Size");
            Width = new ELEMENTARY_TYPE(sw, 0, typeof(int));//"Width");
            Height = new ELEMENTARY_TYPE(sw, 0, typeof(int));//"Height");
            scale = new ELEMENTARY_TYPE(sw, 0, typeof(int));//"R");
            rate = new ELEMENTARY_TYPE(sw, 0, typeof(int));//"R");
            start = new ELEMENTARY_TYPE(sw, 0, typeof(int));//"R");
            length = new ELEMENTARY_TYPE(sw, 0, typeof(int));//"R");
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class AVISTREAMINFO : LOCALIZED_DATA
    {
        private ELEMENTARY_TYPE fccType;
        private ELEMENTARY_TYPE fccHandler;
        private ELEMENTARY_TYPE dwFlags;
        private ELEMENTARY_TYPE dwCaps;
        private ELEMENTARY_TYPE wPriority;
        private ELEMENTARY_TYPE wLanguage;
        private ELEMENTARY_TYPE dwScale;
        private ELEMENTARY_TYPE dwRate;
        private ELEMENTARY_TYPE dwStart;
        private ELEMENTARY_TYPE dwLength;
        private ELEMENTARY_TYPE dwInitialFrames;
        private ELEMENTARY_TYPE dwSuggestedBufferSize;
        private ELEMENTARY_TYPE dwQuality;
        private ELEMENTARY_TYPE dwSampleSize;
        private RECT rcFrame;
        private ELEMENTARY_TYPE dwEditCount;
        private ELEMENTARY_TYPE dwFormatChangeCount;
        private ELEMENTARY_TYPE szName;
        public ELEMENTARY_TYPE FccType
        {
            get { return fccType; }
            set { fccType = value; }
        }
        public ELEMENTARY_TYPE FccHandler
        {
            get { return fccHandler; }
            set { fccHandler = value; }
        }
        public ELEMENTARY_TYPE Caps
        {
            get { return dwCaps; }
            set { dwCaps = value; }
        }
        public ELEMENTARY_TYPE Flags
        {
            get { return dwFlags; }
            set { dwFlags = value; }
        }
        public ELEMENTARY_TYPE WPriority
        {
            get { return wPriority; }
            set { wPriority = value; }
        }
        public ELEMENTARY_TYPE WLanguage
        {
            get { return wLanguage; }
            set { wLanguage = value; }
        }
        public ELEMENTARY_TYPE Scale
        {
            get { return dwScale; }
            set { dwScale = value; }
        }
        public ELEMENTARY_TYPE Rate
        {
            get { return dwRate; }
            set { dwRate = value; }
        }
        public ELEMENTARY_TYPE Start
        {
            get { return dwStart; }
            set { dwStart = value; }
        }
        public ELEMENTARY_TYPE Length
        {
            get { return dwLength; }
            set { dwLength = value; }
        }
        public ELEMENTARY_TYPE InitialFrames
        {
            get { return dwInitialFrames; }
            set { dwInitialFrames = value; }
        }
        public ELEMENTARY_TYPE SuggestedBufferSize
        {
            get { return dwSuggestedBufferSize; }
            set { dwSuggestedBufferSize = value; }
        }
        public ELEMENTARY_TYPE Quality
        {
            get { return dwQuality; }
            set { dwQuality = value; }
        }
        public ELEMENTARY_TYPE SampleSize
        {
            get { return dwSampleSize; }
            set { dwSampleSize = value; }
        }

        public RECT RcFrame
        {
            get { return rcFrame; }
            set { rcFrame = value; }
        }

        public ELEMENTARY_TYPE EditCount
        {
            get { return dwEditCount; }
            set { dwEditCount = value; }
        }
        public ELEMENTARY_TYPE FormatChangeCount
        {
            get { return dwFormatChangeCount; }
            set { dwFormatChangeCount = value; }
        }

        public ELEMENTARY_TYPE SzName
        {
            get { return szName; }
            set { szName = value; }
        }
        public AVISTREAMINFO(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            fccType = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            fccHandler = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            dwFlags = new ELEMENTARY_TYPE(sw, 0, typeof(int)); ;
            wPriority = new ELEMENTARY_TYPE(sw, 0, typeof(short)); ;
            wLanguage = new ELEMENTARY_TYPE(sw, 0, typeof(short)); ;
            dwInitialFrames = new ELEMENTARY_TYPE(sw, 0, typeof(int)); ;
            dwScale = new ELEMENTARY_TYPE(sw, 0, typeof(int)); ;
            dwRate = new ELEMENTARY_TYPE(sw, 0, typeof(int)); ;
            dwStart = new ELEMENTARY_TYPE(sw, 0, typeof(int)); ;
            dwLength = new ELEMENTARY_TYPE(sw, 0, typeof(int)); ;
            dwSuggestedBufferSize = new ELEMENTARY_TYPE(sw, 0, typeof(int)); ;
            dwQuality = new ELEMENTARY_TYPE(sw, 0, typeof(int)); ;
            dwSampleSize = new ELEMENTARY_TYPE(sw, 0, typeof(int)); ;
            rcFrame = new RECT(sw);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class RECT : LOCALIZED_DATA
    {
        public ELEMENTARY_TYPE left;
        public ELEMENTARY_TYPE top;
        public ELEMENTARY_TYPE right;
        public ELEMENTARY_TYPE bottom;
        public RECT(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            left = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            top = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            right = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            bottom = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class Chunk : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE header;
        ELEMENTARY_TYPE length;
        List<Chunk> content = new List<Chunk>();
        AVISTREAMINFO av;
        private AVIHEADER aviHeader;
        List<AVIINDEXENTRY> aviIndexEntry;
        ELEMENTARY_TYPE strn;
        BITMAPINFO bitmapInfo;
        WAVEFORMATEX waveFomat;
        ELEMENTARY_TYPE total;
        DVINFO dvInfo;
        public ELEMENTARY_TYPE Header
        {
            get { return header; }
            set { header = value; }
        }
        public ELEMENTARY_TYPE Length
        {
            get { return length; }
            set { length = value; }
        }
        public List<Chunk> Content
        {
            get { return content; }
            set { content = value; }
        }
        public AVIHEADER AviHeader
        {
            get { return aviHeader; }
            set { aviHeader = value; }
        }
        public AVISTREAMINFO AviStreamInfo
        {
            get { return av; }
            set { av = value; }
        }
        public List<AVIINDEXENTRY> AviIndexEntry
        {
            get { return aviIndexEntry; }
            set { aviIndexEntry = value; }
        }
        private List<INFODATA> info;
        public List<INFODATA> Info
        {
            get { return info; }
            set { info = value; }
        }
        public ELEMENTARY_TYPE StreamName
        {
            get { return strn; }
            set { strn = value; }
        }
        public BITMAPINFO BitmapInfo
        {
            get { return bitmapInfo; }
            set { bitmapInfo = value; }
        }
        public WAVEFORMATEX WaveFomat
        {
            get { return waveFomat; }
            set { waveFomat = value; }
        }
        public ELEMENTARY_TYPE Total_Frames
        {
            get { return total; }
            set { total = value; }
        }
        public DVINFO DvInfo
        {
            get { return dvInfo; }
            set { dvInfo = value; }
        }
        public Chunk(BitStreamReader sw, int l, Chunk parent)
        {
            PositionOfStructureInFile = sw.Position;
            header = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            switch ((string)header.Value)
            {
                case "idx1":
                    length = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    aviIndexEntry = new List<AVIINDEXENTRY>();
                    while (sw.Position < PositionOfStructureInFile + (int)length.Value)
                    {
                        AVIINDEXENTRY ev = new AVIINDEXENTRY(sw);
                        aviIndexEntry.Add(ev);
                    }
                    break;
                case "dmlh":
                    length = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    total = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    break;
                case "indx":
                case "00db":
                case "00dc":
                case "ix00":
                    length = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    break;
                case "ix01":
                    length = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    break;
                case "00wb":
                case "01wb":
                    length = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    break;
                case "hdrl":
                    aviHeader = new AVIHEADER(sw);
                    break;
                case "INFO":
                    info = new List<INFODATA>();
                    long start = sw.Position;
                    while (sw.Position < start + l)
                    {
                        INFODATA inf = new INFODATA(sw);
                        if (sw.Position > start + l)
                        {
                            sw.Position = start + l;
                            break;
                        }
                        info.Add(inf);
                    }
                    break;
                case "JUNK":
                    length = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    break;
                case "LIST":
                    length = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    while (sw.Position < PositionOfStructureInFile + (int)length.Value + 8)
                    {
                        Chunk ch = new Chunk(sw, (int)length.Value - 4, this);
                        content.Add(ch);
                    }
                    break;
                case "vprp":
                    break;
                case "rec ":
                    break;
                case "strl":
                    Chunk chu = new Chunk(sw, 0, parent);
                    content.Add(chu);
                    break;
                case "strh"://(<Stream header>)
                    length = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    av = new AVISTREAMINFO(sw);
                    break;
                case "strf"://<Stream format> depends on the stream type
                    length = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    switch ((string)parent.Content[0].Content[0].AviStreamInfo.FccType.Value)
                    {
                        case "iavs":
                            dvInfo = new DVINFO(sw);
                            break;
                        case "vids":
                            bitmapInfo = new BITMAPINFO(sw);
                            break;
                        case "auds":
                            waveFomat = new WAVEFORMATEX(sw);
                            break;
                    }
                    break;
                case "strd"://<Additional header data>
                    break;
                case "strn"://<Stream name>
                    length = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                    if ((int)length.Value > 0)
                    {
                        strn = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (int)length.Value);
                    }
                    break;
                case "odml":
                    break;
                case "movi":
                    break;
                default:
                    break;

            }
            if (length != null)
                sw.Position = PositionOfStructureInFile + (int)length.Value +8;
            if (sw.Position % 2 == 1)
                sw.Position++;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }

    }
    public class DVINFO
    {
        ELEMENTARY_TYPE dwDVAAuxSrc;
        ELEMENTARY_TYPE dwDVAAuxCtl;
        ELEMENTARY_TYPE dwDVAAuxSrc1;
        ELEMENTARY_TYPE dwDVAAuxCtl1;
        ELEMENTARY_TYPE dwDVVAuxSrc;
        ELEMENTARY_TYPE dwDVVAuxCtl;
        ELEMENTARY_TYPE dwDVReserved;
        public DVINFO(BitStreamReader sw)
        {
            dwDVAAuxSrc = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            dwDVAAuxCtl = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            dwDVAAuxSrc1 = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            dwDVAAuxCtl = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            dwDVVAuxSrc = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            dwDVVAuxCtl = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            dwDVReserved = new ELEMENTARY_TYPE(sw, 0, typeof(int[]), 2);
        }
    }
    public class BITMAPINFO : LOCALIZED_DATA
    {
        BITMAPINFOHEADER bmiHeader;

        public BITMAPINFOHEADER BmiHeader
        {
            get { return bmiHeader; }
            set { bmiHeader = value; }
        }
        RGBQUAD bmiColors;

        public RGBQUAD BmiColors
        {
            get { return bmiColors; }
            set { bmiColors = value; }
        }
        public BITMAPINFO(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            bmiHeader = new BITMAPINFOHEADER(sw);
  //          bmiColors = new RGBQUAD(sw);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class BITMAPINFOHEADER : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE biSize;

        public ELEMENTARY_TYPE BiSize
        {
            get { return biSize; }
            set { biSize = value; }
        }
        ELEMENTARY_TYPE biWidth;

        public ELEMENTARY_TYPE BiWidth
        {
            get { return biWidth; }
            set { biWidth = value; }
        }
        ELEMENTARY_TYPE biHeight;

        public ELEMENTARY_TYPE BiHeight
        {
            get { return biHeight; }
            set { biHeight = value; }
        }
        ELEMENTARY_TYPE biPlanes;

        public ELEMENTARY_TYPE BiPlanes
        {
            get { return biPlanes; }
            set { biPlanes = value; }
        }
        ELEMENTARY_TYPE biBitCount;

        public ELEMENTARY_TYPE BiBitCount
        {
            get { return biBitCount; }
            set { biBitCount = value; }
        }
        ELEMENTARY_TYPE biCompression;

        public ELEMENTARY_TYPE BiCompression
        {
            get { return biCompression; }
            set { biCompression = value; }
        }
        ELEMENTARY_TYPE biSizeImage;

        public ELEMENTARY_TYPE BiSizeImage
        {
            get { return biSizeImage; }
            set { biSizeImage = value; }
        }
        ELEMENTARY_TYPE biXPelsPerMeter;

        public ELEMENTARY_TYPE BiXPelsPerMeter
        {
            get { return biXPelsPerMeter; }
            set { biXPelsPerMeter = value; }
        }
        ELEMENTARY_TYPE biYPelsPerMeter;

        public ELEMENTARY_TYPE BiYPelsPerMeter
        {
            get { return biYPelsPerMeter; }
            set { biYPelsPerMeter = value; }
        }
        ELEMENTARY_TYPE biClrUsed;

        public ELEMENTARY_TYPE BiClrUsed
        {
            get { return biClrUsed; }
            set { biClrUsed = value; }
        }
        ELEMENTARY_TYPE biClrImportant;

        public ELEMENTARY_TYPE BiClrImportant
        {
            get { return biClrImportant; }
            set { biClrImportant = value; }
        }
        public BITMAPINFOHEADER(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            biSize = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            biWidth = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            biHeight = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            biPlanes=  new ELEMENTARY_TYPE(sw, 0, typeof(short));
            biBitCount = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            biCompression = new ELEMENTARY_TYPE(sw, 0, Encoding.Default,4);
            biSizeImage = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            biXPelsPerMeter = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            biYPelsPerMeter = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            biClrUsed = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            biClrImportant =new ELEMENTARY_TYPE(sw, 0, typeof(int));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class RGBQUAD : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE rgbBlue;

        public ELEMENTARY_TYPE RgbBlue
        {
            get { return rgbBlue; }
            set { rgbBlue = value; }
        }
        ELEMENTARY_TYPE rgbGreen;

        public ELEMENTARY_TYPE RgbGreen
        {
            get { return rgbGreen; }
            set { rgbGreen = value; }
        }
        ELEMENTARY_TYPE rgbRed;

        public ELEMENTARY_TYPE RgbRed
        {
            get { return rgbRed; }
            set { rgbRed = value; }
        }
        ELEMENTARY_TYPE rgbReserved;

        public ELEMENTARY_TYPE RgbReserved
        {
            get { return rgbReserved; }
            set { rgbReserved = value; }
        }
        public RGBQUAD(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            rgbBlue = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            rgbGreen = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            rgbRed = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            rgbReserved = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class WAVEFORMATEX : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE wFormatTag;

        public ELEMENTARY_TYPE WFormatTag
        {
            get { return wFormatTag; }
            set { wFormatTag = value; }
        }
        ELEMENTARY_TYPE nChannels;

        public ELEMENTARY_TYPE NChannels
        {
            get { return nChannels; }
            set { nChannels = value; }
        }
        ELEMENTARY_TYPE nSamplesPerSec;

        public ELEMENTARY_TYPE NSamplesPerSec
        {
            get { return nSamplesPerSec; }
            set { nSamplesPerSec = value; }
        }
        ELEMENTARY_TYPE nAvgBytesPerSec;

        public ELEMENTARY_TYPE NAvgBytesPerSec
        {
            get { return nAvgBytesPerSec; }
            set { nAvgBytesPerSec = value; }
        }
        ELEMENTARY_TYPE nBlockAlign;

        public ELEMENTARY_TYPE NBlockAlign
        {
            get { return nBlockAlign; }
            set { nBlockAlign = value; }
        }
        ELEMENTARY_TYPE wBitsPerSample;

        public ELEMENTARY_TYPE WBitsPerSample
        {
            get { return wBitsPerSample; }
            set { wBitsPerSample = value; }
        }
        ELEMENTARY_TYPE cbSize;

        public ELEMENTARY_TYPE CbSize
        {
            get { return cbSize; }
            set { cbSize = value; }
        }
        public WAVEFORMATEX(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            wFormatTag = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            nChannels = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            nSamplesPerSec = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            nAvgBytesPerSec = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            nBlockAlign = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            wBitsPerSample = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            cbSize = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }

    public class INFODATA : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE ckid;
        ELEMENTARY_TYPE length;
        ELEMENTARY_TYPE data;
        public ELEMENTARY_TYPE Ckid
        {
            get { return ckid; }
            set { ckid = value; }
        }
        public ELEMENTARY_TYPE Length
        {
            get { return length; }
            set { length = value; }
        }
        public ELEMENTARY_TYPE Data
        {
            get { return data; }
            set { data = value; }
        }
        public INFODATA(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            ckid = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            length = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            data = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (int)length.Value);
            if ((int)length.Value % 2 == 1)
                sw.Position++;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class AVIINDEXENTRY : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE ckid;
        ELEMENTARY_TYPE dwFlags;
        ELEMENTARY_TYPE dwChunkOffset;
        ELEMENTARY_TYPE dwChunkLength;
        public ELEMENTARY_TYPE Ckid
        {
            get { return ckid; }
            set { ckid = value; }
        }
        public ELEMENTARY_TYPE Flags
        {
            get { return dwFlags; }
            set { dwFlags = value; }
        }

        public ELEMENTARY_TYPE ChunkOffset
        {
            get { return dwChunkOffset; }
            set { dwChunkOffset = value; }
        }
        public ELEMENTARY_TYPE ChunkLength
        {
            get { return dwChunkLength; }
            set { dwChunkLength = value; }
        }
        public AVIINDEXENTRY(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            ckid = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
            dwFlags = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            dwChunkOffset = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            dwChunkLength = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
}