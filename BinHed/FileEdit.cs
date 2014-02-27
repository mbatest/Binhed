using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using BookReader;
using ExifLibrary;
using MP3Library;
using ZipFiles;
using Mpeg2Files;
using BluRay;
using Code;
using LowLevel;
using Utils;
using BinaryViewer;
using System.Diagnostics;

namespace BinHed
{
    public partial class FileEdit : UserControl
    {
        public event DataEventHandler DataEvent;
        #region Private members
        WebBrowser webBrowser1 = new WebBrowser();
        string FileName;
        int currentPage = 0;
        long searchStart;
        SearchType searchT;
        FileHeader hdr;
        MobiFileReader mf;
        #endregion
        public FileEdit()
        {
            InitializeComponent();
        }
        public void OpenFile(string FileName)
        {
            prcButton.Visible = false;
            nextPage.Visible = false;
            this.textBoxPath.Visible = false;
            this.toolStripLabel4.Visible = false;
            this.secteur.Visible = false;
            this.Dump.Visible = false;
            this.back.Visible = false;
            this.next.Visible = false;
            this.toolStripLabel2.Visible = false;
            this.textBoxOffset.Visible = false;
            this.toolStripLabel3.Visible = false;
            this.textBoxSize.Visible = false;
            this.toolStripLabel5.Visible = false;
            this.Cylinder.Visible = false;
            this.toolStripLabel6.Visible = false;
            this.TrackN.Visible = false;
            this.toolStripLabel7.Visible = false;
            this.Sector.Visible = false;
            this.parseBlock.Visible = false;
            this.FileName = FileName;
            FileInfo fi = new FileInfo(FileName);
            
            fileNameLabel.Text = FileName + " : Length = " + fi.Length.ToString();
            binaryView.Init(FileName);
            Analyse(FileName);
            splitContainer1.SplitterDistance = binaryView.PanelWidth;
            tabControl1.TabPages[0].Text = fi.Name;
        }
        /// <summary>
        /// Makes specific analysis of known type files
        /// </summary>
        /// <param nameIndex="FileName">nameIndex of file to test</param>
        private void Analyse(string FileName)
        {
            #region Tests file.extension
            splitContainer3.Panel2Collapsed = true;
            BinaryFileReader fs = new BinaryFileReader(FileName, false);
            hdr = new FileHeader(fs.ReadBytes(20), Path.GetExtension(FileName));
            exv.Visible = true;
            switch (Path.GetExtension(FileName).ToLower())
            {
                #region Web
                case ".htm":
                case ".html":
                case ".xml":
                    webBrowser1.Url = new Uri(FileName);
                    splitContainer3.Panel2.Controls.Clear();
                    splitContainer3.Panel2.Controls.Add(webBrowser1);
                    splitContainer3.Panel2Collapsed = false;
                    webBrowser1.Dock = DockStyle.Fill;
                    break;
                #endregion
                #region Microsoft Office
                case ".xls":
                case ".xslx":
                    #region Excel
                    if (Path.GetExtension(FileName).ToLower() == ".xslx")
                    {
                        Zip zx = new Zip(FileName);
                        exv.Init(zx);
                    }
                    /*                  ExcelControl xls = new ExcelControl(); 
                                        xls.LoadDocument(FileName);
                                        xls.Dock = DockStyle.Fill;
                                        tabControl1.TabPages[2].Controls.Add(xls);
                                        xls.Visible = true;*/
                    break;
                    #endregion
                case ".doc":
                case ".docx":
                    #region Word
                    if (Path.GetExtension(FileName).ToLower() == ".docx")
                    {
                        Zip zx = new Zip(FileName);
                        exv.Init(zx);
                    }
  /*                  WinWordControl winWordControl1 = new WinWordControl();
                    splitContainer3.Panel2.Controls.Clear();
                    splitContainer3.Panel2.Controls.Add(winWordControl1);
                    splitContainer3.Panel2Collapsed = false;
                    winWordControl1.Dock = DockStyle.Fill;
                    winWordControl1.Visible = true;
                    winWordControl1.LoadDocument(FileName);*/
                    break;
                    #endregion
                #endregion
                #region ebooks
                case ".prc":
                    MobiFileReader mob = new MobiFileReader(FileName);
                    exv.Init(mob);
                    webBrowser1.Visible = true;
                    webBrowser1.DocumentText = mob.text;
                    splitContainer3.Panel2.Controls.Clear();
                    splitContainer3.Panel2.Controls.Add(webBrowser1);
                    splitContainer3.Panel2Collapsed = false;
                    webBrowser1.Dock = DockStyle.Fill;
                    prcButton.Visible = true;
                    nextPage.Visible = true;
                    break;
                case ".lrf":
                    LRFFileReader vr = new LRFFileReader(FileName);
                    exv.Init(vr);
                    break;
                #endregion
                #region Images
                case ".jpg":
                    JPGFileData jpg = new JPGFileData(FileName);
                    exv.Init(jpg);
                    PictureBox p = new PictureBox();
                    p.SizeMode = PictureBoxSizeMode.StretchImage;
                    p.Width = 400;// splitContainer1.Panel2.Width;
                    p.Image = Image.FromFile(FileName);
                    p.Height = p.Width * p.Image.Height / p.Image.Width;
                    splitContainer3.Panel2.Controls.Clear();
                    splitContainer3.Panel2.Controls.Add(p);
                    splitContainer3.Panel2Collapsed = false;
                    break;
                case ".bmp":
                    BinDecoder bind = new BinDecoder(FileName);
                    exv.Init(bind);
                    p = new PictureBox();
                    p.SizeMode = PictureBoxSizeMode.AutoSize;
                    p.Image = Image.FromFile(FileName);
                    splitContainer3.Panel2.Controls.Add(p);
                    splitContainer3.Panel2Collapsed = false;
                    break;
                #endregion
                #region Audio
                case ".mp3":
                case ".wma":
                    MusicFileClass mf = new MusicFileClass(FileName);
                    exv.Init(mf);
                    break;
                #endregion
                case ".zip":
                    Zip z = new Zip(FileName);
                    exv.Init(z);
                    break;
                case ".ttf":
                    // Font file
                    break;
                case ".avi":
                    break;
                #region Mpeg2 endCode video
                case ".mpg":
                case ".mpeg":
                case ".vob":
                    Mpeg2 mpg = new Mpeg2(FileName);
                    exv.Init(mpg);
                    break;
                case ".ifo":
                case ".bup":
                    IFO ifo = new IFO(FileName);
                    exv.Init(ifo);
                    break;
                #endregion
                #region BD video
                case ".m2ts":
                    M2TS mts = new M2TS(FileName);
                    exv.Init((ILOCALIZED_DATA)mts);
                    break;
                case ".mpls":
                    MPLS mp = new MPLS(FileName);
                    exv.Init((ILOCALIZED_DATA)mp);
                    break;
                case ".clpi":
                    CLPI cp = new CLPI(FileName);
                    exv.Init((ILOCALIZED_DATA)cp);
                    break;
                case ".bdmv":
                    if (FileName.IndexOf("index") >= 0)
                    {
                        exv.Visible = false;
                        INDEX_BDMV bd = new INDEX_BDMV(FileName);
                        BDViewer bdv = new BDViewer();
                        bdv.tabSelected += new TabSelectedEventHandler(bdv_tabSelected);
                        bdv.Init(bd);
                        splitContainer3.Panel1.Controls.Add(bdv);
                        bdv.Dock = DockStyle.Fill;
                        bdv.dataSelected += new DataSelectedEventHandler(exv_dataSelected);
                        TabPage tp = new TabPage(Path.GetFileName(bd.MovieObject.fileName));
                        tabControl1.TabPages.Add(tp);
                        BinaryView bn = new BinaryView();
                        bn.Init(bd.MovieObject.fileName);
                        tp.Controls.Add(bn);
                        bn.Dock = DockStyle.Fill;
                        foreach (MPLS mpl in bd.MplsList)
                        {
                            tp = new TabPage(Path.GetFileName(mpl.LongName));
                            tabControl1.TabPages.Add(tp);
                            bn = new BinaryView();
                            bn.Init(mpl.LongName);
                            tp.Controls.Add(bn);
                            bn.Dock = DockStyle.Fill;
                        }
                    }
                    else if (FileName.IndexOf("Movie") >= 0)
                    {
                        MOBJ_BDMV mo = new MOBJ_BDMV(FileName);
                        exv.Init((ILOCALIZED_DATA)mo);
                    }
                    break;
                #endregion
                #region Text like files
                case ".ps":
                    PostScriptReader ps = new PostScriptReader(FileName);
                    exv.Init(ps);
                    break;
                case ".pdf":
                    PdfViewer pdf = new PdfViewer();
                    pdf.Init(FileName);
                    exv.Visible = false;
                    splitContainer3.Panel1.Controls.Add(pdf);
                    pdf.Dock = DockStyle.Fill;
                    break;
                case ".log":
                case ".ini":
                case ".cs":
                case ".txt":
                case ".bat":
                    exv.Visible = false;
                    TextBox t = new TextBox();
                    t.Multiline = true;
                    t.ScrollBars = ScrollBars.Both;
                    StreamReader sr = new StreamReader(FileName);
                    t.Text = sr.ReadToEnd();
                    sr.Close();
                    splitContainer3.Panel1.Controls.Add(t);
                    t.Dock = DockStyle.Fill;
                    break;
                #endregion
                #region PE files
                case ".exe":
                case ".dll":
                    exv.Visible = false;
                    Executable ex = new Executable(FileName);
                    ExeViewer exeViewer = new ExeViewer();
                    exeViewer.Init(ex);
                    exeViewer.dataSelected += exv_dataSelected;
                    splitContainer3.Panel1.Controls.Add(exeViewer);
                    exeViewer.Dock = DockStyle.Fill;
                    break;
                #endregion
                case "ITSF"://chm : http://www.russotto.net/chm/chmformat.html

                default:
                    #region Analyse header and try to identify file type
                    switch (hdr.HeaderBinarySt)
                    {
                        case "5346504B"://SFKP Sound Forge (Peak Data File) 
                            break;
                        case "44564456":
                            //DVD IFO (or BUP)
                            ifo = new IFO(FileName);
                            break;
                        case "47494638"://Gif
                            break;
                        case "89504e47":
                            //PNG
                            break;
                        case "4d534346":
                            //CAB
                            break;
                        case "49545346":
                        case "31be0000"://wri ?
                            break;
                        case "00010000":
                            //ttf, mdb
                            break;
                        case "00000100":
                            // ico
                            break;
                        case "504b0304":
                            Zip zi = new Zip(FileName);
                            exv.Init(zi);
                            break;
                        case "52617221":
                            break;
                        case "ffd8ffe0":
                            //Jpg
                            break;
                        case "d0cf11e0":
                            //office
                            break;
                        case "3f5f0300":
                            //help
                            break;
                        case "000001ba":
                            mpg = new Mpeg2(FileName);
                            exv.Init(mpg);
                            break;
                        default:
                            if (hdr.HeaderBinarySt.StartsWith("1f8b"))
                            {
                                //gz file
                                MessageBox.Show("gz file, to do");
                                byte compressionMethods = hdr.header[2];
                                if (compressionMethods == 0x08)
                                {
                                    //deflate
                                }
                                byte flags = hdr.header[3];
                                if (flags != 0x00)
                                {
                                }
                                Gzip gz = new Gzip(FileName);
                                exv.Init(gz);
                            }
                            #region binary header
                            switch (hdr.ShortHeaderString)
                            {
                                case "BM":// BMP
                                    BinDecoder bi = new BinDecoder(FileName);
                                    exv.Init(bi);
                                    break;
                                case "MK":
                                    break;
                                case "PK":
                                    //Zip, jar
                                    break;
                                case "MZ":
                                    //exe, dll
                                    break;
                                default:
                                    #region text header
                                    switch (hdr.HeaderString)
                                    {
                                        case "DVDV":
                                            //DVD IFO (or BUP)
                                            ifo = new IFO(FileName);
                                            break;
                                        case "ITSF":
                                            BinDecoder bid = new BinDecoder(FileName);
                                            //Microsoft Compiled HTML Help File
                                            break;
                                        case "GIF8":
                                            break;
                                        case "MSCF"://Microsoft cabinet file
                                            break;
                                        case "Rar!":
                                            break;
                                        case "RIFF":
                                            //avi
                                            AviAnalyze av = new AviAnalyze(FileName);
                                            exv.Init(av);
                                            break;
                                        case "%PDF":
                                            break;
                                        case "‰PNG":
                                            break;
                                        case "<?xm"://xml
                                            break;
                                        case "ID3":
                                            //mp3
                                            break;
                                        case "L\0R\0":
                                            //lrf
                                            break;
                                        case "ÐÏà":
                                            //office
                                            break;
                                        case "BZh9":
                                            //tar.gz
                                            break;
                                        case "\0 \0\0":
                                            break;
                                        default:
                                            break;
                                    }
                                    #endregion
                                    break;
                            }
                            #endregion
                            break;
                    }
                    #endregion
                    break;
            }
            #endregion
        }
        #region Event handlers
        private void bdv_tabSelected(object sender, TabSelectedArgs e)
        {
            foreach (TabPage tp in tabControl1.TabPages)
            {
                if (tp.Text.ToLower() == e.Name.ToLower())
                {
                    tabControl1.SelectedTab = tp;
                    break;
                }
            }
        }
        private void exv_dataSelected(object sender, DataSelectedEventArgs e)
        {
            BinaryView view1 = (BinaryView)tabControl1.SelectedTab.Controls[0];
            long position = e.position;
            if (rawDiskAccess)
            {
                if (position > 0)
                {
                    long secteur = position / da.SizeBuffer;
                    position = position % da.SizeBuffer;
                    ChangeSector(secteur);
                }
            }
            view1.Highlight(position, e.length, e.o);
        }
        private void reAnalyse_Click(object sender, EventArgs e)
        {
            OpenFile(FileName);
        }
        private void searchText_TextChanged(object sender, EventArgs e)
        {
            BinaryView view1 = (BinaryView)tabControl1.SelectedTab.Controls[0];
            searchStart = view1.Search(searchText.Text, searchT, 0);
        }
        private void hexa_Click(object sender, EventArgs e)
        {
            BinaryView view1 = (BinaryView)tabControl1.SelectedTab.Controls[0];
            switch (hexa.Text)
            {
                case "0x":
                    hexa.Text = "01";
                    view1.SetMode(ViewMode.Bit);
                    break;
                case "01":
                    hexa.Text = "0D";
                    view1.SetMode(ViewMode.Dec);
                    break;
                case "0D":
                    hexa.Text = "0x";
                    view1.SetMode(ViewMode.Hex);
                    break;
            }
        }
        private void nextPage_Click(object sender, EventArgs e)
        {
            if (mf != null)
            {
                currentPage++;
                mf.ReadPage(FileName, currentPage);
                webBrowser1.Visible = true;
                webBrowser1.DocumentText = mf.text;
            }
        }
        private void suiteFichier_Click(object sender, EventArgs e)
        {
            searchStart = binaryView.Search(searchText.Text, searchT, searchStart + 1);
        }
        private void prcButton_Click(object sender, EventArgs e)
        {
            if (Path.GetExtension(FileName) == ".prc")
            {
                mf = new MobiFileReader(FileName);
                webBrowser1.Visible = true;
                webBrowser1.DocumentText = mf.text;
            }
            if (Path.GetExtension(FileName) == ".lrf")
            {
                LRFView view = new LRFView(FileName);
                tabControl1.TabPages[2].Controls.Add(view);
                view.Dock = DockStyle.Fill;
            }
        }
        private void hexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            searchType.Text = t.Text;
            hexToolStripMenuItem.Checked = false;
            ansiToolStripMenuItem.Checked = false;
            unicodeToolStripMenuItem.Checked = false;
            addressToolStripMenuItem.Checked = false;
            binaryToolStripMenuItem.Checked = false;
            switch (t.Text)
            {
                case "Hex":
                    searchT = SearchType.Hex;
                    hexToolStripMenuItem.Checked = true;
                    break;
                case "Ansi":
                    searchT = SearchType.Ansi;
                    ansiToolStripMenuItem.Checked = true;
                    break;
                case "Unicode":
                    searchT = SearchType.Unicode;
                    unicodeToolStripMenuItem.Checked = true;
                    break;
                case "Address":
                    searchT = SearchType.Address;
                    addressToolStripMenuItem.Checked = true;
                    break;
                case "Binary":
                    searchT = SearchType.Binary;
                    binaryToolStripMenuItem.Checked = false;
                    break;
            }
        }
        private void view1_dataEvent(object sender, DataEventArgs e)
        {
            if (DataEvent != null)
                DataEvent(sender, e);
        }
        private void showText_Click(object sender, EventArgs e)
        {
            BinaryView view1 = (BinaryView)tabControl1.SelectedTab.Controls[0];
            view1.ShowText();
            splitContainer1.SplitterDistance = view1.PanelWidth;
        }
        private void showHex_Click(object sender, EventArgs e)
        {
            BinaryView view1 = (BinaryView)tabControl1.SelectedTab.Controls[0];
            view1.ShowHex();
            splitContainer1.SplitterDistance = view1.PanelWidth;
        }
        private void zoomOut_Click(object sender, EventArgs e)
        {
            BinaryView view1 = (BinaryView)tabControl1.SelectedTab.Controls[0];
            view1.ZoomOut();
        }
        private void zoomIn_Click(object sender, EventArgs e)
        {
            BinaryView view1 = (BinaryView)tabControl1.SelectedTab.Controls[0];
            view1.ZoomIn();
        }
        #endregion
        #region Raw Access event handlers
        RawDiskAccess da;
        private bool rawDiskAccess = false;
        public void RawDiskAccess()
        {
             rawDiskAccess = true;
            da = new RawDiskAccess();
            binaryView.SetScrollMode(ScrollMode.Sector);
            reAnalyse.Visible = false;
            nouveauToolStripButton.Visible = false;
            ouvrirToolStripButton.Visible = false;
            toolStripSeparator.Visible = false;
            couperToolStripButton.Visible = false;
            copierToolStripButton.Visible = false;
            collerToolStripButton.Visible = false;
            parseBlock.Visible = true;
            binaryView.scrollHandler += new ScrollHandler(view1_scrollHandler);
            fileNameLabel.Visible = false;
            foreach (Win32_Drive p in da.Drives)
            {
                if (p.MediaLoaded)
                {
                    ToolStripMenuItem m = new ToolStripMenuItem(p.DeviceID);
                    m.Tag = p;
                    m.Click += new EventHandler(m_Click);
                    textBoxPath.DropDownItems.Add(m);
                }
            }
            textBoxPath.Text = @"\\.\PhysicalDrive0";
        }
        private void UpdateTextBoxes()
        {
            secteur.Text = da.SecteurNumber.ToString("x2");
            Cylinder.Text = da.Cylinder.ToString("x2");
            Sector.Text = da.Sector.ToString("x2");
            TrackN.Text = da.Track.ToString("x2");
            textBoxOffset.Text = da.Offset.ToString("x2");
            textBoxSize.Text = da.SizeBuffer.ToString("x2");
        }
        private void m_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem m = (ToolStripMenuItem)sender;
            textBoxPath.Text = m.Text;
        }
        /// <summary>
        /// Fonction accessible en mode Raw de lecture du disque
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void parseBlock_Click(object sender, EventArgs e)
        {
            // Analyze un secteur
            long sectorNumber = long.Parse(secteur.Text, System.Globalization.NumberStyles.HexNumber);
            int size = int.Parse(textBoxSize.Text, System.Globalization.NumberStyles.HexNumber);
            ILOCALIZED_DATA o = (ILOCALIZED_DATA)da.ParseBlock(sectorNumber, size * 2);
            if (o != null)
            {
                FileViewer exv = new FileViewer();
                exv.Init(o);
                splitContainer3.Panel2Collapsed = false;
                TabPage tp = new TabPage("File");

                mftEntries.TabPages.Add(tp); ;
                tp.Controls.Add(exv);
                mftEntries.SelectedTab = tp;
                exv.dataSelected += new DataSelectedEventHandler(exv_dataSelected);
                exv.dataRequested+= new DataRequestEventHandler(exv_dataRequested);
                exv.Dock = DockStyle.Fill;
            }
        }
        private void textBoxPath_TextChanged(object sender, EventArgs e)
        {
            da.Current_Drive = null;
            foreach (Win32_Drive pa in da.Drives)
            {
                if (pa.DeviceID.ToUpper() == textBoxPath.Text.ToUpper())
                {
                    da.Current_Drive = pa;
                    switch (pa.GetType().Name)
                    {
                        case "Win32_DiskDrive":

                            Win32_DiskDrive p = (Win32_DiskDrive)pa;
                            da.ReadsFirstSector(p);
                            binaryView.SetScrollSize(p.TotalSectors);
                            break;
                        case "Win32_CDROMDrive":
                            Win32_CDROMDrive c = (Win32_CDROMDrive)pa;
                            da.ReadsFirstSector(c);
                            break;
                    }
                }
            }
            binaryView.Init(da.SectorBuffer, 0);
            UpdateTextBoxes();
            exv.Init(da);
            splitContainer3.Panel1.Controls.Clear();
            splitContainer3.Panel1.Controls.Add(exv);
            splitContainer3.Panel2Collapsed = true;
            exv.dataSelected += new DataSelectedEventHandler(exv_dataSelected);
            exv.dataRequested += new DataRequestEventHandler(exv_dataRequested);
            exv.Dock = DockStyle.Fill;
        }
        private void exv_dataRequested(object sender, DataRequestArgs e)
        {
            if (da != null)
            {
                object o = da.RequestData(e.SourceObject);
                if (o != null)
                {
                    TreeNode tn = new TreeNode(o.ToString());
                    tn.Tag = o;
                    e.SourceNode.Nodes.Add(tn);
                }
            }
        }
        private void view1_scrollHandler(object sender, ScrollArgs e)
        {
            ChangeSector(e.Secteur);
        }
        private void next_Click(object sender, EventArgs e)
        {
            if (da.SecteurNumber < (long)((Win32_DiskDrive)da.Current_Drive).TotalSectors - 1)
                ChangeSector(da.SecteurNumber + 1);
        }
        private void back_Click(object sender, EventArgs e)
        {
            if (da.SecteurNumber > 0)
                ChangeSector(da.SecteurNumber - 1);
        }
        private void Dump_Click(object sender, EventArgs e)
        {
            try
            {
                long secteurNumber = int.Parse(secteur.Text, System.Globalization.NumberStyles.HexNumber);
                ChangeSector(secteurNumber);
            }
            catch { }
        }
        private void ChangeSector(long e)
        {
            int sizeBuf = int.Parse(textBoxSize.Text, System.Globalization.NumberStyles.HexNumber);
            da.Change(e, sizeBuf);
            if (da.SectorBuffer != null)
            {
                binaryView.Init(da.SectorBuffer, e * da.SizeBuffer);
                UpdateTextBoxes();
            }
        }
        #endregion
    }
    public class FileHeader
    {
        public string Extension;
        public string HeaderBinarySt;
        public string HeaderString;
        public string ShortHeaderString;
        public byte[] header;
        public FileHeader(byte[] header, string ext)
        {
            this.header = header;
            Extension = ext;
            HeaderBinarySt = "";
            for (int u = 0; u < 4; u++)
                HeaderBinarySt += header[u].ToString("x2");
            HeaderString = Encoding.Default.GetString(header);
            byte[] c = new byte[2];
            Buffer.BlockCopy(header, 0, c, 0, 2);
            ShortHeaderString = Encoding.Default.GetString(c);
        }
        public override string ToString()
        {
            return Extension + " " + HeaderString + " " + HeaderBinarySt;
        }
    }
}
