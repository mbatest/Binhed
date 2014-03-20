using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Utils;

namespace BinHed
{
    public class SequenceEventArgs : EventArgs
    {
        public DateTime FrameDateTime;
        public DateTime FrameTimeCode;
        public int FrameNumber;
        //        public Bitmap FrameBitmap;
        public int SequenceNumber;
        public SequenceEventArgs(DateTime fdt, DateTime ftc, int frn, int sn)
        {
            FrameDateTime = fdt;
            FrameTimeCode = ftc;
            FrameNumber = frn;
            SequenceNumber = sn;
        }
    }
    public delegate void SequenceEventHandler(object sender, SequenceEventArgs e);
    public class EndFileEventArgs : EventArgs
    {
        public EndFileEventArgs(DateTime fdt, DateTime ftc, int frn, int sq)
        {
        }
    }
    public delegate void EndFileEventHandler(object sender, SequenceEventArgs e);
    /// <summary>
    /// This classes provides the analysis of computerSystem standard DV 
    /// segment file, returning all header information as well
    /// as computerSystem decomposition of the file in shots according to 
    /// timestamp information.
    ///<frame>
    /// Frame length 144000 for PAL + 4 byte tagCode 00dc
    /// + 4 byte tagCode for lengthString 0x23280
    /// One audio bloc every 25 images of length 192000 or 128000
    /// PositionOfStructureInFile with 01wb
    /// Some empty frames sometimes
    /// </frame> 
    ///<timecode>
    /// time tagCode is in the  block PositionOfStructureInFile with 0x3F 0x07 0x00 (normally type_Code Real_Address 0x54)
    /// in computerSystem five bytes field PositionOfStructureInFile with 0x13 type_Code Real_Address 114 (0x72).
    /// </timecode>
    /// <datetime>
    /// date and time are in the  block PositionOfStructureInFile with 0x5F 0x07 0x02 (normally type_Code Real_Address 0x194)
    /// in  five bytes fields PositionOfStructureInFile with 0x62 and 0x63 type_Code Real_Address 462 and 467 (01cE and 0x1D3).
    /// </datetime>
    ///<splitting>
    ///SEQUENCE is split according to the date/time and frame number.
    ///If date/time differs by more than one second sequence is split (works 
    ///for original DV Frames)
    ///if date/time n/computerSystem then frame number is used (works for generated frames)
    /// </summary>
    public class AviAnalyze: LOCALIZED_DATA
    {
        #region Inner variables
        private ArrayList sequences;
        private AVISTREAMINFO[] fileStruc;
        private AVIHEADER fileHdr;
        public bool hasIndex = false;
        string fileName;
        private FileStream aviFile;
        private int buf = 144004;
        byte[] vauxdata;
        private string vidstr = "00dc";//"00db"
        private int len = 4;
        private byte[] ckid = new byte[4];
        private string ch = "";
        private long entriesInUse;
        ArrayList indexFrame = new ArrayList();
        ArrayList indexFrameStart = new ArrayList();
        ArrayList indexAudio = new ArrayList();
        private DateTime currentFrameDate;
        private DateTime currentTimeCode;
        private int currentFrameNumber = 0;
        private int currentSequenceNumber = 0;
        private bool stop = false;
        #endregion
        #region Public variables
        public bool StopAnalyse
        {
            set
            {
                stop = value;
            }
        }
        /// <summary>
        /// date Time for the current segment frame
        /// </summary>
        public DateTime CurrentFrameDate
        {
            get
            {
                return currentFrameDate;
            }
        }
        /// <summary>
        /// Time Code for the current segment frame
        /// </summary>
        public DateTime CurrentTimeCode
        {
            get
            {
                return currentTimeCode;
            }
        }
        /// <summary>
        /// 0 based number of the current frame
        /// </summary>
        public int CurrentFrameNumber
        {
            get
            {
                return currentFrameNumber - 1;
            }
        }

        public int SequenceNumber
        {
            get
            {
                return currentSequenceNumber;
            }
        }

        /// <summary>
        /// Analyse listLrfObjects contains descripive codeBuffer about the 
        /// segment file
        /// </summary>
        private ArrayList analyseList;
        [CategoryAttribute("Fichier"), DescriptionAttribute("Taille")]
        public ArrayList AnalyseList
        {
            get { return analyseList; }
            set { analyseList = value; }
        }
        public SortedList frameList = new SortedList();
        [CategoryAttribute("Fichier"), DescriptionAttribute("AVI Header")]
        public AVIHEADER FileHdr
        {
            get
            {
                return fileHdr;
            }
        }
        [CategoryAttribute("Fichier"), DescriptionAttribute("AVI StreamInfo")]
        public AVISTREAMINFO[] FileStruc
        {
            get
            {
                return fileStruc;
            }
        }
        public ArrayList Sequence
        {
            get
            {
                return sequences;
            }
        }
        public event SequenceEventHandler NewSequence;
        public event EndFileEventHandler EndOfFile;
        public event SequenceEventHandler NewFrameRead;
        #endregion
        #region Open and close
        public bool OpenAviFile(string fName)
        {
            try
            {
                fileName = fName;
                aviFile = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public AviAnalyze(string fileName)
        {
            aviFile = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            AviHeaders();
            aviFile.Close();
        }
        public void CloseAviFile()
        {
            if (aviFile != null) aviFile.Close();
        }
        #endregion
        #region Find headers
        public void AviHeaders()
        {
            analyseList = new ArrayList();
            ///Reading file header
            ///RIFF (length) AVI 
            ///LIST (length) hdrl
            ///avih (length)
            fileHdr.Riff = AddString("Header");
            fileHdr.FileSize = AddLong("Size");
            fileHdr.FType = AddString("Type");
            fileHdr.List = AddString("Structure");
            fileHdr.LstSize = AddLong("Size");
            fileHdr.HeaderL = AddString("Header");
            fileHdr.Av = AddString("MainAviHeader");
            //
            fileHdr.SSize = AddLong("Size");
            fileHdr.MicroSecPerFrame = AddLong("MicroSecPerFrame");
            fileHdr.MaxBytesperSec = AddLong("MaxBytesperSec");
            fileHdr.Granularity = AddLong("Granularity");
            fileHdr.Flags = AddLong("Flags");
            fileHdr.NbTotalFrames = AddLong("TotalFrames");
            fileHdr.InitialFrame = AddLong("InitialFrame");
            fileHdr.NbStreams = AddLong("Streams");
            fileHdr.SuggestedBufferSize = AddLong("Suggested Buffer Size");
            fileHdr.Width = AddLong("Width");
            fileHdr.Height = AddLong("Height");
            fileHdr.scale = AddLong("R");
            fileHdr.rate = AddLong("R");
            fileHdr.start = AddLong("R");
            fileHdr.length = AddLong("R");
            ///<summary>
            ///Stream listLrfObjects
            ///<summary>
            string dm = AddString("List start");
            long nbTot = AddLong("Size");
            fileStruc = new AVISTREAMINFO[fileHdr.NbStreams];
            int streamNumber = 0;
            while (streamNumber < fileHdr.NbStreams)
            {
                while (ch != "movi")
                {
                    aviFile.Read(ckid, 0, len);
                    ch = ByteTos(ckid);
                    streamNumber = Interpret(streamNumber);
                    if (streamNumber == fileHdr.NbStreams) break;
                }
            }
            while (ch != "movi")
            {
                aviFile.Read(ckid, 0, len);
                ch = ByteTos(ckid);
                Interpret(0);
            }
            analyseList.Add(aviFile.Position.ToString("x8") + " List " + ch);
            //Dans le cas directionBit'un fichier 'iavs' il y computerSystem directionBit'autres informations
            //avant un second 'movi'
            if (fileStruc[0].fccType == "iavs")
            {
                while (ch != vidstr)
                {
                    aviFile.Read(ckid, 0, len);
                    ch = ByteTos(ckid);
                    Interpret(0);
                }
            }
            else
            {
                //On recherche le premier frame qui commence
                //par object_length'indicateur vidstr
                do
                {
                    aviFile.Read(ckid, 0, len);
                    ch = ByteTos(ckid);
                    Interpret(0);
                } while ((ch != vidstr)&&(aviFile.Position<aviFile.Length/10));

            }
        }
        private int Interpret(int streamNumber)
        {
            long stSize = 0;
            switch (ch)
            {
                case "ix00":
                    entriesInUse = UpdateIndex();
                    break;
                case "ix01":
                    UpdateIndexAud();
                    break;
                case "01wb":
                    aviFile.Read(ckid, 0, len);
                    long u = ByteToi(ckid);
                    aviFile.Seek(u, SeekOrigin.Current);
                    break;
                case "JUNK":
                    aviFile.Read(ckid, 0, len);
                    long lh = ByteToi(ckid);
                    analyseList.Add(aviFile.Position.ToString("x8") + ch + " " + lh.ToString());
                    aviFile.Seek((int)lh, SeekOrigin.Current);
                    break;
                case "LIST":
                    analyseList.Add(aviFile.Position.ToString("x8") + " List : " + ch);
                    AddLong("Size");
                    break;
                case "vprp":
                    break;
                case "dmlh":
                    analyseList.Add(aviFile.Position.ToString("x8") + ch);
                    break;
                case "strl":
                    analyseList.Add(aviFile.Position.ToString("x8") + " Flux n° " + streamNumber.ToString() + " " + ch);
                    break;
                case "strh":
                    analyseList.Add(aviFile.Position.ToString("x8") + " Header : " + ch);
                    stSize = AddLong("Size");
                    fileStruc[streamNumber].fccType = AddString("Type de flux"); stSize -= 4;
                    fileStruc[streamNumber].fccHandler = AddString("Gestionnaire"); stSize -= 4;
                    if (fileStruc[streamNumber].fccHandler == "RLE ") vidstr = "00db";
                    fileStruc[streamNumber].dwFlags = AddLong("Flags"); stSize -= 4;
                    fileStruc[streamNumber].wPriority = AddShort("Priority"); stSize -= 2;
                    fileStruc[streamNumber].wLanguage = AddShort("Language"); stSize -= 2;
                    fileStruc[streamNumber].dwInitialFrames = AddLong("Initial Frame"); stSize -= 4;
                    fileStruc[streamNumber].dwScale = AddLong("Scale"); stSize -= 4;
                    fileStruc[streamNumber].dwRate = AddLong("Rate"); stSize -= 4;
                    fileStruc[streamNumber].dwStart = AddLong("Start"); stSize -= 4;
                    fileStruc[streamNumber].dwLength = AddLong("Length"); stSize -= 4;
                    fileStruc[streamNumber].dwSuggestedBufferSize = AddLong("Suggested Buffer Size"); stSize -= 4;
                    if (fileStruc[streamNumber].fccType == "vids") buf = (int)fileStruc[streamNumber].dwSuggestedBufferSize;
                    fileStruc[streamNumber].dwQuality = AddLong("Quality"); stSize -= 4;
                    fileStruc[streamNumber].dwSampleSize = AddLong("Sample Size"); stSize -= 4;
                    AddShort("Left"); stSize -= 2;
                    AddShort("Top"); stSize -= 2;
                    AddShort("Right"); stSize -= 2;
                    AddShort("Bottom"); stSize -= 2;
                    break;
                case "strf":
                    analyseList.Add(aviFile.Position.ToString("x8") + "Format " + ch);
                    AddLong("Taille");
                    switch (fileStruc[streamNumber].fccType)
                    {
                        case "iavs":
                            AddLong("DVAAuxSrc");
                            AddLong("DVAAuxCtl");
                            AddLong("DVAAuxSrc1");
                            AddLong("DVAAuxCtl1");
                            AddLong("DVVAuxSrc");
                            AddLong("DVVAuxCtl");
                            AddLong("Reserved");
                            AddLong("Reserved");
                            break;
                        case "vids":
                            stSize = AddLong("Size");
                            AddLong("Width"); stSize -= 4;
                            AddLong("Height"); stSize -= 4;
                            AddShort("Planes"); stSize -= 2;
                            AddShort("BitCount"); stSize -= 2;
                            AddString("Compression"); stSize -= 4;
                            int buf2 = (int)AddLong("ImageSize"); stSize -= 4;
                            if ((buf2 < buf) && (fileStruc[streamNumber].fccType == "vids")) buf = buf2;
                            AddLong("xPelsPerMeter"); stSize -= 4;
                            AddLong("yPelsPerMeter"); stSize -= 4;
                            AddLong("Color_Table"); stSize -= 4;
                            AddLong("Important Color_Table"); stSize -= 4;
                            streamNumber++;
                            break;
                        case "auds":
                            AddShort("Format"); stSize -= 2;
                            AddShort("nChannels"); stSize -= 2;
                            AddLong("SamplesPerRec"); stSize -= 4;
                            AddLong("AvgBytesPerRec"); stSize -= 4;
                            AddShort("BlockAlign"); stSize -= 2;
                            AddShort("BitsPerSample"); stSize -= 2;
                            streamNumber++;
                            break;
                    }
                    break;
                case "strd":
                    long a = AddLong("");
                    for (int uu = 0; uu < (a / 4); uu++) AddLong("");
                    break;
                case "odml":
                    analyseList.Add(aviFile.Position.ToString("x8") + " Open DML List " + ch);
                    AddString("");
                    long length = AddLong("Length");
                    long dw = AddLong("Real Frame Number"); length -= 4;
                    aviFile.Seek(length, SeekOrigin.Current);
                    break;
                case "indx":
                    //Type 1 DV dv file
                    analyseList.Add(aviFile.Position.ToString("x8") + " " + ch);
                    long lhn = AddLong("Size");
                    aviFile.Seek(lhn, SeekOrigin.Current);
                    /*                   AddShort("LongPerEntry");
                                       AddByte("SubType");
                                       AddByte("Type");
                                       int entryInUse = (int)AddLong("Index in use");
                                       string ss = AddString("Frame marker");
                   //                    if (streamNumber == 0) vidstr = ss;
                                       int[] ent = new int[entryInUse];
                                       for (int ux = 0; ux < entryInUse; ux++)
                                       {
                                           aviFile.Read(ckid, 0, strLen);
                                           ent[ux] = (int)ByteToi(ckid);
                                           debListeMFiles += strLen;

                                       }*/
                    break;
                default:
                    break;

            }
            return streamNumber;
        }
        #endregion
        #region Search for sequences
        public void FrameAnalysis()
        {
            FrameAnalysis(0, fileHdr.NbTotalFrames);
            EndOfFile(this, new SequenceEventArgs(CurrentFrameDate, CurrentTimeCode, (int)fileHdr.NbTotalFrames, SequenceNumber));
        }
        public void FrameAnalysis(long startFrame, long endFrame)
        {
            sequences = new ArrayList();
            int frameSequenceLength = 0;
            int sequenceNumber = 1;
            DateTime startComp = DateTime.Now;
            if (indexFrame.Count > 0)
            {
                DateTime oldFrame = DateTime.MinValue;
                DateTime newFrame = DateTime.MinValue;
                int prevFrame = 0;
                IndexData inx = (IndexData)indexFrameStart[0];
                aviFile.Seek((long)inx.startOffset, SeekOrigin.Begin);
                int debut = 0;
                UInt64 movieOffset = inx.startOffset;
                buf = (int)fileStruc[0].dwSuggestedBufferSize;
                vauxdata = new byte[buf];
                currentFrameNumber = 0;
                // First frame handling
                FrameData fr = (FrameData)indexFrame[0];
                long offSet = (long)movieOffset + fr.offset;
                aviFile.Seek(offSet - 8, SeekOrigin.Begin);
                if (fr.size <= buf)
                {
                    aviFile.Read(vauxdata, 0, 1000);
                    int start = 0x1D2;
                    if (vauxdata[start] == 0x62)
                    {
                        DateTime[] timeData = ComputeCodeAndStampFrame(vauxdata, start);
                        currentFrameDate = timeData[0];
                        currentTimeCode = timeData[1];
                        currentSequenceNumber = sequenceNumber;
                        NewSequence(this, new SequenceEventArgs(currentFrameDate, currentTimeCode, currentFrameNumber, sequenceNumber));
                        sequenceNumber += 1;
                    }
                }
                oldFrame = fr.timeStamp;
                currentFrameNumber += 1;
                int numIndex = 0;
                TimeSpan tsLength = new TimeSpan(0, 0, 1);
                while (currentFrameNumber < fileHdr.NbTotalFrames)
                {
                    int fin = (int)Math.Min(debut + entriesInUse, fileHdr.NbTotalFrames) - 1;
                    while (currentFrameNumber <= fin)
                    {
                        if (stop)
                            return;
                        fr = (FrameData)indexFrame[currentFrameNumber];
                        offSet = (long)movieOffset + fr.offset;
                        aviFile.Seek(offSet - 8, SeekOrigin.Begin);
                        if (fr.size <= buf)
                        {
                            aviFile.Read(vauxdata, 0, 1000);
                            int start = 0x1D2;
                            if (vauxdata[start] == 0x62)
                            {
                                DateTime[] timeData = ComputeCodeAndStampFrame(vauxdata, start);
                                currentFrameDate = timeData[0];
                                currentTimeCode = timeData[1];
                                //Test : la première vérification évite des choses étranges
                                // et repose sur object_length'hypothèse raisonnable de séquences directionBit'au moins 5 frames, ie 1/5 de secondes
                                if ((frameSequenceLength > 5) && (currentFrameDate != oldFrame) && ((currentFrameDate > oldFrame.Add(tsLength)) || currentFrameDate < oldFrame))
                                {
                                    //new sequence

                                    NewSequence(this, new SequenceEventArgs(currentFrameDate, currentTimeCode, currentFrameNumber, sequenceNumber));
                                    currentSequenceNumber = sequenceNumber;
                                    sequenceNumber += 1;
                                    prevFrame = currentFrameNumber;
                                    frameSequenceLength = 0;
                                }
                                oldFrame = currentFrameDate;
                            }
                        }
                        frameList.Add(currentFrameNumber, new FrameData(currentFrameNumber, currentFrameDate, currentTimeCode));
                        if (currentFrameNumber == fileHdr.NbTotalFrames - 1)
                        {
                            DateTime endComp = DateTime.Now;
                            TimeSpan ts = endComp - startComp;
                            int sec = ts.Minutes * 60 + ts.Seconds;
                            int speed = (int)fileHdr.NbTotalFrames / sec;
                            return;
                        }
                        currentFrameNumber += 1;
                        NewFrameRead(this, new SequenceEventArgs(currentFrameDate, currentTimeCode, currentFrameNumber, currentSequenceNumber));
                        frameSequenceLength += 1;
                    }
                    while (ch != "ix00")
                    {
                        aviFile.Read(ckid, 0, len);
                        ch = ByteTos(ckid);
                    }
                    entriesInUse = UpdateIndex();
                    ch = "";
                    debut = debut + inx.startFrame;
                    numIndex += 1;
                    inx = (IndexData)indexFrameStart[numIndex];
                    movieOffset = inx.startOffset;
                }
            }
            else
            {
                FrameAnalysisAlter(startFrame, endFrame);
            }
        }
        private long UpdateIndex()
        {
            analyseList.Add(ch);
            AddLong("Index Size");
            AddShort("LongPerEntry");
            AddByte("SubType");
            AddByte("Type");
            long entryInUse = AddLong("Entries in use");
            vidstr = AddString("dwChunkid");
            UInt32 q1 = (UInt32)AddLong("quad1");
            UInt32 q2 = (UInt32)AddLong("quad2");
            AddLong("dwReserved");
            for (int u = 0; u < entryInUse; u++)
            {
                aviFile.Read(ckid, 0, len);
                long off = (int)ByteToi(ckid);
                aviFile.Read(ckid, 0, len);
                long dwSize = ByteToi(ckid);
                FrameData fr = new FrameData(off, dwSize);
                if (ch == "ix00")
                {
                    indexFrame.Add(fr);
                }
            }
            IndexData ix = new IndexData((int)entryInUse, q1, q2);
            indexFrameStart.Add(ix);
            return entryInUse;
        }
        private void UpdateIndexAud()
        {
            analyseList.Add(ch);
            long lg = AddLong("Index Size");
            long deb = aviFile.Position;
            AddShort("LongPerEntry");
            AddByte("SubType");
            AddByte("Type");
            long entryInUse = AddLong("Entries in use");
            AddString("dwChunkid");
            UInt32 q1 = (UInt32)AddLong("quad1");
            UInt32 q2 = (UInt32)AddLong("quad2");
            AddLong("dwReserved");
            for (int u = 0; u < entryInUse; u++)
            {
                aviFile.Read(ckid, 0, len);
                long off = (int)ByteToi(ckid);
                aviFile.Read(ckid, 0, len);
                long dwSize = ByteToi(ckid);
            }
            lg = (lg + deb) - aviFile.Position;
            aviFile.Seek(lg, SeekOrigin.Current);
        }
        /// <summary>
        /// Alternate scanning method when index are not available
        /// This method scans the segment file, lists all frames and 
        /// compute all sequence starts.
        /// Corrections are brought when timecode is incorrect
        /// Dans certains cas :
        /// Le film est organisé en blocs (de deux gigas ?), précédé par la séquence RIFF AVIX
        /// et clos par un index. Cas de Shanghai Février.avi Dans ce cas le nombre de frames
        /// indiquées est normalement celui du premier RIFF. Pas le cas pour ce film
        /// <validity>
        /// This method is used for movies without indexes ix00
        /// </validity>
        /// </summary>
        public void FrameAnalysisAlter(long startFrame, long endFrame)
        {
            // Retour début mises à jour 
            //This function reads sequentially all the segment frames in computerSystem codeBuffer
            buf = 144000;
            byte[] vauxdata = new byte[buf];
            int frameSequence = 0;
            int numSequence = 1;
            //These variable are used for sequence splitting
            DateTime frameLoc = DateTime.MinValue;
            DateTime oldFrameLoc = DateTime.MinValue;
            DateTime beforeFrame = DateTime.MinValue;
            DateTime oldFrame = DateTime.MinValue;
            DateTime newFrame = DateTime.MinValue;
            long startOldFrame = 0;
            long startNewFrame = 0;
            long sizeFrame = 0;
            currentFrameNumber = 0;
            int frameStart = currentFrameNumber;
            int prevFrame = 0;
            SEQUENCE s = new SEQUENCE();
            s.seqNumber = 1;
            s.startFrame = 0;
            bool correct = true;
            long numtr = 0;
            int vidLen = 0;
            long audLen = 0;
            int framCount = 0;
            TimeSpan delay = new TimeSpan(0, 0, 0);
            do
            {
                while (ch != vidstr)
                {
                    if (numtr >= endFrame) break;
                    if (ch == "01wb")
                    {
                        aviFile.Read(ckid, 0, len);
                        audLen = ByteToi(ckid);
                        aviFile.Seek(audLen, SeekOrigin.Current); startNewFrame = aviFile.Position;
                        if ((framCount < 24) && (numtr > 24))
                        {
                            // utile dans un cas : yunnan2.avi, problème frame 15067 au lieu de 15064
                            // manque trois frames entre deux audios au frame 15046
                            numtr += (25 - framCount);
                        }
                        framCount = 0;
                    }
                    if (ch == "RIFF")
                    {
                    }
                    if ((ch == "ix00") || (ch == "ix01") || (ch == "00ix") || (ch == "01ix") || (ch == "idx1") || (ch == "idx0"))
                    {
                        //skipping index codeBuffer
                        aviFile.Read(ckid, 0, len);
                        long indLen = ByteToi(ckid);
                        aviFile.Seek(indLen, SeekOrigin.Current);
                        startNewFrame = aviFile.Position;
                    }
                    if (aviFile.Read(ckid, 0, len) < 0) return;
                    ch = ByteTos(ckid);
                    if (aviFile.Position >= aviFile.Length) break;
                }
                startOldFrame = startNewFrame;
                startNewFrame = aviFile.Position;
                ch = "";
                //One more frame has been found and can be analyzed
                sizeFrame = startNewFrame - startOldFrame;
                aviFile.Read(ckid, 0, len);
                vidLen = (int)ByteToi(ckid);
                if (vidLen == 144000)
                {
                    aviFile.Read(vauxdata, 0, 500);
                    if (vauxdata[0x1CA] == 0x62)
                    {
                        DateTime[] timeData = ComputeCodeAndStampFrame(vauxdata, 0x1CA);
                        newFrame = timeData[0];
                        frameLoc = timeData[1];
                        //                        Trace.WriteLine(framCount.ToString() + " " + numtr.ToString() + " " + frameNb.ToString() + " " + frameStart.ToString() + " " + vidLen.ToString() + " " + sizeFrame.ToString() + " " + frameLoc.ToLongTimeString() + " " + newFrame.ToLongTimeString());
                        // Criterion : timestamp has changed, for more than one second - Le + 5 sert pour shanghai 200503 02.avi à vérifier ?
                        if ((currentFrameNumber > 1) && (newFrame != beforeFrame) && (newFrame != oldFrame) && ((newFrame > oldFrame.Add(new TimeSpan(0, 0, 1))) || newFrame < oldFrame) && (currentFrameNumber >= prevFrame + 5))
                        {
                            //                           NewSequence(this,new SequenceEventArgs(fr.timeStamp,fr.timeCode,frNb,lineRead.seqNumber));
                            //                            Trace.WriteLine("Nouvelle SEQUENCE " + numtr.ToString() + " " + frameStart.ToString() + "xxxxxxxxxxxxxxxxx");
                            int secd = frameSequence / 25;
                            int mn = secd / 60;
                            secd = frameSequence / 25 - 60 * mn;
                            int fr = frameSequence - 25 * secd;
                            s.duration = new TimeSpan(0, 0, mn, secd, fr * 10);
                            ///Saving the current sequence
                            s.endFrame = (int)numtr - 1;
                            s.validDate = correct;
                            sequences.Add(s);
                            //new sequence
                            numSequence += 1;
                            s = new SEQUENCE();
                            s.seqNumber = numSequence;
                            s.startFrame = (int)numtr;
                            s.timeStamp = (correct ? newFrame : DateTime.MaxValue);
                            prevFrame = currentFrameNumber;
                            frameSequence = 0;
                        }
                        frameList.Add(currentFrameNumber, new FrameData(currentFrameNumber, newFrame, frameLoc));
                        frameSequence += 1;
                        beforeFrame = oldFrame;
                        oldFrame = newFrame;
                        oldFrameLoc = frameLoc;
                        currentFrameNumber += 1;
                        // Correction
                    }
                    // Jump to Change candidate frame
                    aviFile.Seek(buf - 500, SeekOrigin.Current);
                    frameStart += 1;
                    framCount += 1;
                    numtr++;
                }
                // Prepare Change frame
                if (aviFile.Read(ckid, 0, len) < 0) return;
                ch = ByteTos(ckid);
            }
            while (numtr < endFrame);
            // Handling the last one
            int secda = frameSequence / 25;
            int mna = secda / 60;
            secda = frameSequence / 25 - 60 * mna;
            int fra = frameSequence - 25 * secda;
            s.duration = new TimeSpan(0, 0, mna, secda, fra * 10);
            s.endFrame = currentFrameNumber - 1;
            s.validDate = correct;
            numSequence += 1;
            s.seqNumber = numSequence;
            if (correct)
            {
                s.timeStamp = newFrame;
            }
            else
            {
                s.timeStamp = DateTime.MaxValue;
            }
            sequences.Add(s);
        }
        private DateTime[] ComputeCodeAndStampFrame(byte[] data, int start)
        {
            DateTime[] result = new DateTime[2];
            if ((data[start] == 0x62) && (data[start + 5] == 0x63))
            {
                int a1 = 0;
                int a2 = 0;
                //Jour
                a1 = (int)data[start + 2] & 0x0F;
                a2 = ((int)data[start + 2] & 0x30) / 16;
                int jour = 10 * a2 + a1;
                a1 = (int)data[start + 3] & 0x0F;
                a2 = ((int)data[start + 3] & 0x10) / 16;
                int mois = 10 * a2 + a1;
                a1 = (int)data[start + 4] & 0x0F;
                a2 = ((int)data[start + 4] & 0x30) / 16;
                int an = 10 * a2 + a1;
                if (an < 30) an += 2000;
                //Heure
                a1 = (int)data[start + 9] & 0x0F;
                a2 = ((int)data[start + 9] & 0x30) / 16;
                int hour = 10 * a2 + a1;
                // Minutes
                a1 = (int)data[start + 8] & 0x0F;
                a2 = ((int)data[start + 8] & 0x70) / 16;
                int min = 10 * a2 + a1;
                // second
                a1 = (int)data[start + 7] & 0x0F;
                a2 = ((int)data[start + 7] & 0x70) / 16;
                int sec = 10 * a2 + a1;
                // frames
                a1 = (int)data[start + 6] & 0x0F;
                a2 = ((int)data[start + 6] & 0x30) / 16;
                int millis = (10 * a2 + a1) * 40;
                result[0] = new DateTime(an, mois, jour, hour, min, sec);
            }
            else
            {
                result[0] = DateTime.MinValue;
            }
            int debut = start - 0x15C;
            if (data[debut] == 0x13)
            {
                int[] d = new int[9];
                d[1] = (int)data[debut + 1] & 0x0F;
                d[2] = ((int)data[debut + 1] & 0x30) / 16;
                d[3] = (int)data[debut + 2] & 0x0F;
                d[4] = ((int)data[debut + 2] & 0x70) / 16;
                d[5] = (int)data[debut + 3] & 0x0F;
                d[6] = ((int)data[debut + 3] & 0x70) / 16;
                d[7] = (int)data[debut + 4] & 0x0F;
                d[8] = ((int)data[debut + 4] & 0x30) / 16;
                //               Trace.WriteLine(directionBit[8].ToString() + directionBit[7].ToString() + "h" + directionBit[6].ToString() + directionBit[5].ToString() + ":" + directionBit[4].ToString() + directionBit[3].ToString() + "F" + directionBit[2].ToString() + directionBit[1].ToString());
                int h = 10 * d[8] + d[7];
                int m = 10 * d[6] + d[5];
                int s = 10 * d[4] + d[3];
                int f = 10 * d[2] + d[1];
                result[1] = new DateTime(1, 1, 1, h, m, s, f * 40);
            }
            else
            {
                result[1] = DateTime.MinValue;
            }
            return result;
        }
        #endregion
        #region Utility functions
        private string AddString(string comment)
        {
            aviFile.Read(ckid, 0, 4);
            string tmp = ByteTos(ckid);
            analyseList.Add(comment + " " + tmp);
            return tmp;
        }
        private long AddLong(string comment)
        {
            aviFile.Read(ckid, 0, 4);
            long tmp = ByteToi(ckid);
            analyseList.Add(comment + " " + tmp);
            return tmp;
        }
        private short AddShort(string comment)
        {
            aviFile.Read(ckid, 0, 2);
            short tmp = (short)ByteToi(ckid);
            analyseList.Add(comment + " " + tmp);
            return tmp;
        }
        private int AddByte(string comment)
        {
            aviFile.Read(ckid, 0, 1);
            int tmp = (int)ByteToi(ckid);
            analyseList.Add(comment + " " + tmp);
            return tmp;
        }
        private string ByteTos(byte[] c)
        {
            string ret = "";
            for (int i = 0; i < c.Length; i++)
            {
                ret = ret + (char)c[i];
            }
            return ret;
        }
        private string ByteToh(byte[] c)
        {
            string ret = "";
            for (int i = 0; i < c.Length; i++)
            {
                ret = ret + c[i].ToString("x2");
            }
            return ret;
        }
        private long ByteToi(byte[] c)
        {
            long taille = 0;
            for (int w = 0; w < c.Length; w++)
            {
                taille = 256 * taille + (uint)c[c.Length - 1 - w];
            }
            return taille;
        }
        #endregion
    }
    [Serializable]
    public class FrameData : LOCALIZED_DATA
    {
        public int frameNumber;
        public Int64 offset;
        public long size;
        public DateTime timeStamp;
        public DateTime timeCode;
        public FrameData(long off, long sz)
        {
            offset = off;
            size = sz;
        }
        public FrameData(int n, DateTime tS, DateTime tC)
        {
            frameNumber = n;
            timeStamp = tS;
            timeCode = tC;
        }
    }
    public class IndexData : LOCALIZED_DATA
    {
        public int startFrame;
        public UInt32 q1;
        public UInt32 q2;
        public UInt64 startOffset;
        public IndexData(int st, UInt32 quad1, UInt32 quad2)
        {
            startFrame = st;
            q1 = quad1;
            q2 = quad2;
            UInt64 aa = UInt32.MaxValue;
            aa += 1;
            startOffset = quad2 * aa + quad1;
        }

    }
    [Serializable]
    public struct AVIHEADER 
    {
        private string riff;
        private long fileSize;
        private string av;
        private string list;
        private long lstSize;
        private string headerL;
        private string fType;
        private long sSize;
        private long microSecPerFrame;
        [CategoryAttribute("AVI Header"), DescriptionAttribute("Marqueur")]
        public string Riff
        {
            get { return riff; }
            set { riff = value; }
        }
        [CategoryAttribute("AVI Header"), DescriptionAttribute("FileSize")]
        public long FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }
        [CategoryAttribute("AVI Header"), DescriptionAttribute("MainAviHeader")]
        public string Av
        {
            get { return av; }
            set { av = value; }
        }
        [CategoryAttribute("AVI Header"), DescriptionAttribute("Marqueur")]
        public string List
        {
            get { return list; }
            set { list = value; }
        }
        [CategoryAttribute("AVI Header"), DescriptionAttribute("List Size")]
        public long LstSize
        {
            get { return lstSize; }
            set { lstSize = value; }
        }
        [CategoryAttribute("AVI Header"), DescriptionAttribute("Header")]
        public string HeaderL
        {
            get { return headerL; }
            set { headerL = value; }
        }
        [CategoryAttribute("AVI Header"), DescriptionAttribute("Type")]
        public string FType
        {
            get { return fType; }
            set { fType = value; }
        }
        [CategoryAttribute("AVI Header"), DescriptionAttribute("Size")]
        public long SSize
        {
            get { return sSize; }
            set { sSize = value; }
        }
        [CategoryAttribute("AVI Header"), DescriptionAttribute("MicroSeconds per Frame")]
        public long MicroSecPerFrame
        {
            get { return microSecPerFrame; }
            set { microSecPerFrame = value; }
        }
        [CategoryAttribute("AVI Header"), DescriptionAttribute("Max Bytes per Seconds")]
        public long MaxBytesperSec
        {
            get { return maxBytesperSec; }
            set { maxBytesperSec = value; }
        }
        [CategoryAttribute("AVI Header"), DescriptionAttribute("Granularity")]
        public long Granularity
        {
            get { return granularity; }
            set { granularity = value; }
        }
        [CategoryAttribute("AVI Header"), DescriptionAttribute("Flags")]
        public long Flags
        {
            get { return flags; }
            set { flags = value; }
        }
        [CategoryAttribute("AVI Header"), DescriptionAttribute("Number Of Frames")]
        public long NbTotalFrames
        {
            get { return nbTotalFrames; }
            set { nbTotalFrames = value; }
        }
        [CategoryAttribute("AVI Header"), DescriptionAttribute("Initial Frame")]
        public long InitialFrame
        {
            get { return initialFrame; }
            set { initialFrame = value; }
        }
        [CategoryAttribute("AVI Header"), DescriptionAttribute("Number of Streams")]
        public long NbStreams
        {
            get { return nbStreams; }
            set { nbStreams = value; }
        }
        [CategoryAttribute("AVI Header"), DescriptionAttribute("BufferSize")]
        public long SuggestedBufferSize
        {
            get { return suggestedBufferSize; }
            set { suggestedBufferSize = value; }
        }
        [CategoryAttribute("AVI Header"), DescriptionAttribute("Height")]
        public long Height
        {
            get { return height; }
            set { height = value; }
        }
        [CategoryAttribute("AVI Header"), DescriptionAttribute("Width")]
        public long Width
        {
            get { return width; }
            set { width = value; }
        }
        private long maxBytesperSec;
        private long granularity;
        private long flags;
        private long nbTotalFrames;
        private long initialFrame;
        private long nbStreams;
        private long suggestedBufferSize;
        private long width;
        private long height;
        public long scale;
        public long rate;
        public long start;
        public long length;
    }
    [Serializable]
    public struct AVISTREAMINFO 
    {
        public string fccType;
        public string fccHandler;
        public long dwFlags;
        public long dwCaps;
        public short wPriority;
        public short wLanguage;
        public long dwScale;
        public long dwRate;
        public long dwStart;
        public long dwLength;
        public long dwInitialFrames;
        public long dwSuggestedBufferSize;
        public long dwQuality;
        public long dwSampleSize;
        public RECT rcFrame;
        public UInt32 dwEditCount;
        public UInt32 dwFormatChangeCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public UInt16[] szName;
    }
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RECT
    {
        public UInt32 left;
        public UInt32 top;
        public UInt32 right;
        public UInt32 bottom;
    }
    [Serializable]
    public class SEQUENCE
    {
        public bool isInProject = true;
        public int place = 0;
        public int CodeBD = 0;
        public string Comment = "";
        public string titre = "";
        public string lieu = "";
        public int seqNumber = 0;
        public int startFrame;
        public int endFrame;
        public int frameperSec;
        public TimeSpan duration = new TimeSpan(0, 0, 0);
        public DateTime timeStamp;
        public DateTime timeCode;
        public string originFrame;
        public DateTime frameLocation;
        public bool validDate;
        public int EndFrame
        {
            set
            {
                endFrame = value;
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param nameIndex="st">BlockLength frame</param>
        /// <param nameIndex="ef">End frame</param>
        public static TimeSpan ComputeDuration(SEQUENCE seq)
        {
            int durFrame = seq.endFrame - seq.startFrame;
            int secFrame = durFrame / 25;
            int minFrame = secFrame / 60;
            secFrame = secFrame - minFrame * 60;
            int fr = durFrame - secFrame * 25;
            return new TimeSpan(0, 0, minFrame, secFrame, fr * 10);
        }
        public void SetEndFrame(int ef)
        {
            endFrame = ef;
            duration = SEQUENCE.ComputeDuration(this);
        }
        public SEQUENCE(int st, int ef)
        {
            seqNumber = 0;
            startFrame = st;
            endFrame = ef;
            frameperSec = 0;
            duration = TimeSpan.MinValue;
            timeStamp = DateTime.MinValue;
            timeCode = DateTime.MinValue;
            originFrame = "";
            frameLocation = DateTime.MinValue;
            validDate = false;
        }
        public SEQUENCE(int st, DateTime dateTime, DateTime timeCode, int seq)
        {
            seqNumber = seq;
            startFrame = st;
            endFrame = 0;
            frameperSec = 0;
            duration = TimeSpan.MinValue;
            timeStamp = dateTime;
            this.timeCode = timeCode;
            originFrame = "";
            frameLocation = DateTime.MinValue;
            validDate = false;
        }

        public SEQUENCE()
        {
            seqNumber = 0;
            frameperSec = 0;
            duration = TimeSpan.MinValue;
            timeStamp = DateTime.MinValue;
            timeCode = DateTime.MinValue;
            originFrame = "";
            frameLocation = DateTime.MinValue;
            validDate = false;

        }
    }
}
