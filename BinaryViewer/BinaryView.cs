using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using Utils;

namespace BinaryViewer
{
    public partial class BinaryView : UserControl
    {
        public event DataEventHandler dataEvent;
        public event ScrollHandler scrollHandler;
        public int PanelWidth
        {
            get { return panelWidth; }
            set { panelWidth = value; }
        }
        #region Private members
        byte[] buffer;
        private int panelWidth;
        long startAddress = 0;
        long endAddress;
        private string fileName;
        private string mapName;
        long currentPosition;
        int numberOfColumns = 16;
        int startLine;
        int maxLine = 50;
        long searchStart = 0;
        int lineHeight;
        int firstColumnWidth;
        int normalColumnWidth;
        int top;
        int selectedRow;
        bool showText = true;
        bool showHex = true;
        Point selectedSubItem;
        List<Point> selectedSubItems;
        List<Row> rows = new List<Row>();
        ViewMode mode = ViewMode.Hex;
        int viewingMode = 0x10;
        int startColumn = 0;
        Font headerFont;
        Font lineFont;
        Brush headerBrush;
        private ScrollMode scrollMode;
        #endregion
        public BinaryView()
        {
            InitializeComponent();
            headerFont = new System.Drawing.Font("Courrier New", 6.75F);
            lineFont = new System.Drawing.Font("Courrier New", 6.75F);
            headerBrush = Brushes.Black;
            rows = new List<Row>();
            maxLine = 0x200;
        }
        public void Init(string fileName)
        {
            this.fileName = fileName;
            mapName = "file";
            FillRows(0, 0x200);
            selectedRow = -1;
            selectedSubItem = Point.Empty;
            this.Refresh();
            panelWidth = 664;
        }
        public void Init(byte[] buffer, long offset)
        {
            rows.Clear();
            this.buffer = buffer;
            int i = 0;
            while (i < buffer.Length)
            {
                byte[] locbuff = new byte[0x10];
                Buffer.BlockCopy(buffer, i, locbuff, 0, 0x10);
                rows.Add(new Row((offset + i) / 0x10, locbuff));
                i += 0x10;
            }
            selectedRow = -1;
            selectedSubItem = Point.Empty;
            panelWidth = 664;
            switch (scrollMode)
            {
                case ScrollMode.Sector:
                     vScrollBar1.Value =(int) (offset/0x200*reduc);
                    break;
                case ScrollMode.Linear:
                    break;
            }
            this.Refresh();
        }
        public void SetMode(ViewMode mode)
        {
            this.mode = mode;
            switch (mode)
            {
                case ViewMode.Hex:
                    viewingMode = 0x10;
                    break;
                case ViewMode.Bit:
                    viewingMode = 2;
                    break;
                case ViewMode.Dec:
                    viewingMode = 0xa;
                    break;
            }
            Refresh();
        }
        public void ShowText()
        {
            showText = !showText;
            Refresh();
        }
        public void SetScrollMode(ScrollMode scr)
        {
            scrollMode = scr;
        }
        int reduc = 1;
        public void SetScrollSize(ulong s)
        {
            if(s>0xffffffff)
            {
                reduc = (int)((s&0xffffffff00000000)>>32);
                vScrollBar1.SmallChange = 1000;
            }

            vScrollBar1.Maximum = (int)((long)s / reduc);
        }
        public void ShowHex()
        {
            showHex = !showHex;
            Refresh();
        }
        public void ZoomIn()
        {
            headerFont = new System.Drawing.Font("Courrier New", headerFont.Size + 0.5F);
            lineFont = new System.Drawing.Font("Courrier New", headerFont.Size + 0.5F);
            Refresh();
        }
        public void ZoomOut()
        {
            float fontSize = headerFont.Size - 0.5F;
            if (fontSize < 0.5f)
                fontSize = headerFont.Size;
            headerFont = new System.Drawing.Font("Courrier New", fontSize);
            lineFont = new System.Drawing.Font("Courrier New", fontSize);
            Refresh();
        }
        public void Highlight(long position, long length, object o)
        {
            UnSelect();
            if (o == null)
            {
                switch (scrollMode)
                {
                    case ScrollMode.Sector:
                        break;
                    case ScrollMode.Linear:
                        GotoAddress(Math.Max(position - 5 * 0x10, 0));
                       break;
                }
                int row = (int)position / 0x10 - startLine;
                int col = (int)position % 0x10;
                selectedSubItem = new Point(row, col);
                List<Point> sb = new List<Point>();
                for (int i = 0; i < length; i++)
                {
                    rows[row].SubItems[col].Selected = true;
                    sb.Add(new Point(row, col));
                    col += 1;
                    if (col == 0x10)
                    {
                        row++;
                        col = 0;
                    }
                    if (row >= rows.Count)
                        break;
                }
            }
            else
            {
                BinPosition bin = (BinPosition)o;
                UnSelect();
                long p = bin.bitPosition / 0x8;
                long start = bin.bitPosition % 0x8;
                GotoAddress(p);
                int row = (int)p / 0x10 - startLine;
                int col = (int)p % 0x10;
                byte[] b = new byte[1];
                string bitPattern = "1";
                selectedSubItem = new Point(row, col);
                for (int i = 0; i < b.Length; i++)
                {
                    rows[row].SubItems[(col + i) % 0x10].Selected = true;
                    rows[row].SubItems[(col + i) % 0x10].BinaryPattern = bitPattern;
                    rows[row].SubItems[(col + i) % 0x10].PatternStart = (int)start;
                    if (col + i == 0x0f)
                        row++;
                }
            }
            Refresh();
        }
        public long Search(string s, SearchType searchType, long searchStart)
        {
            long p = -1;
            List<string> bitPatterns = new List<string>();
            List<int> startPatterns = new List<int>();
            this.searchStart = searchStart;
            byte[] b = null;
            switch (searchType)
            {
                #region
                case SearchType.Hex:
                    b = TextToByte(s);
                    break;
                case SearchType.Ansi:
                    b = Encoding.Default.GetBytes(s);
                    break;
                case SearchType.Unicode:
                    b = Encoding.Unicode.GetBytes(s);
                    break;
                case SearchType.Address:
                    if (s.Length < 8)
                    {
                        int add = 8 - s.Length;
                        for (int i = 0; i < add; i++)
                            s = "0" + s;
                    }
                    b = TextToByte(s);
                    break;
                #endregion
            }
            switch (searchType)
            {
                case SearchType.Address:
                    byte[] c = new byte[4];
                    for (int i = 0; i < 4; i++)
                        c[i] = b[3 - i];
                    int ad = BitConverter.ToInt32(c, 0);
                    GotoAddress(ad);
                    Refresh();
                    break;
                default:
                    #region Search intData
                    if (searchType == SearchType.Binary)
                    {
                        BitStreamReader bs = new BitStreamReader(fileName, true);
                        long bitposition = bs.FindBinaryPattern(s, searchStart);
                        if (bitposition == -1)         
                            return bitposition;
                        p = bitposition / 0x08;
                        bs.Close();
                        int sh = (int)(bitposition % 0x08);
                        bitPatterns.Add(s.Substring(0, Math.Min(s.Length, 9 - sh)));
                        startPatterns.Add(sh - 1);
                        s = s.Substring(Math.Min(s.Length, 9 - sh));
                        while (s.Length > 0)
                        {
                            string u = "";
                            if (s.Length > 8)
                                u = s.Substring(0, 8);
                            else
                                u = s;
                            bitPatterns.Add(u);
                            startPatterns.Add(0);
                            s = s.Substring(u.Length);
                        }
                        int bLength = 1;
                        long l = sh + s.Length;
                        if (l > 0x8)
                        {
                            bLength += 1;
                        }
                        b = new byte[bitPatterns.Count];
                        searchStart = p + bLength;
                    }
                    else
                        p = Search(b, searchStart);
                    if (p > -1)
                    {
                        UnSelect();
                        GotoAddress(p);
                        int row = (int)p / 0x10 - startLine;
                        int col = (int)p % 0x10;
                        selectedSubItem = new Point(row, col);
                        for (int i = 0; i < b.Length; i++)
                        {
                            rows[row].SubItems[(col + i) % 0x10].Selected = true;
                            if (searchType == SearchType.Binary)
                            {
                                rows[row].SubItems[(col + i) % 0x10].BinaryPattern = bitPatterns[i];
                                rows[row].SubItems[(col + i) % 0x10].PatternStart = startPatterns[i];
                            }
                            if (col + i == 0x0f)
                                row++;
                        }
                        searchStart = p;
                        Refresh();
                    }
                    #endregion
                    break;
            }
            return searchStart;
        }
        #region Mouse events
        void BinView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Point p = e.Location;
            if (p.X < firstColumnWidth)
            {
                selectedRow = SelectRow(p);
            }
            else
            {
                UnSelect();
                Point sel = FindSelectedItem(p);
                if (sel.X != -1)
                {
                    rows[sel.X].SubItems[sel.Y].Selected = true;
                    selectedSubItem = sel;
                    selectedSubItems = new List<Point>();
                    selectedSubItems.Add(selectedSubItem);
                }
            }
            Refresh();
        }
        void BinaryView_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (selectedSubItems != null)
            {
                List<SubItem> selection = new List<SubItem>();
                string text = "";
                foreach (Point p in selectedSubItems)
                {
                    selection.Add(rows[p.X].SubItems[p.Y]);
                    text += rows[p.X].SubItems[p.Y].ToString();
                }
            }
        }
        void BinaryView_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Point sel = FindSelectedItem(e.Location);
            if ((sel.X != -1) & (sel.Y != -1))
            {
                List<byte> b = new List<byte>();
                int x = sel.X;
                if (x >= rows.Count)
                    return;
                int y = sel.Y;
                for (int i = 0; i < 20; i++)
                {
                    b.Add(rows[x].SubItems[y].Value);
                    y++;
                    if (y > 0xf)
                    {
                        y = 0;
                        x++;
                        if (x >= rows.Count)
                            break;
                    }

                }
                if (dataEvent != null)
                    dataEvent(this.dataEvent, new DataEventArgs(b.ToArray()));
                Refresh();
                if ((e.Button == System.Windows.Forms.MouseButtons.Left) && (selectedSubItem != Point.Empty))
                {
                    rows[sel.X].SubItems[sel.Y].Selected = true;
                    selectedSubItems.Add(sel);
                    Refresh();
                }
            }
        }
        void BinView_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            return;
            startLine -= e.Delta / 120;
            if (startLine >= 0)
            {
                int line = startLine;
                FillRows(line, maxLine);
                Refresh();
            }
            else
                startLine = 0;

        }
        void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (scrollMode == ScrollMode.Sector)
            {
                if (scrollHandler != null)
                {
                    scrollHandler(this,new ScrollArgs(e.NewValue));
                }
            }
            else
            {
                if (startLine >= 0)
                {
                    startLine = e.NewValue;
                    if (selectedRow > -1)
                        selectedRow -= (e.NewValue - e.OldValue);
                    if (selectedSubItem != Point.Empty)
                        selectedSubItem.X -= (e.NewValue - e.OldValue);
                    FillRows(startLine, maxLine);
                    Refresh();
                }
                else
                    startLine = 0;
            }
        }
        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            startColumn = e.NewValue;
            Refresh();
        }
        #endregion
        public string s = "";
        public string sbin = "";
        #region Private methods
        private void BinView_Paint(object sender, PaintEventArgs e)
        {
            if (!(showHex || showText))
            {
                panelWidth = 50;
                return;
            }
            if (rows.Count == 0)
                return;
            #region Display headers
            e.Graphics.DrawString("Start address", headerFont, headerBrush, new PointF(3, 0));
            SizeF size = e.Graphics.MeasureString("Start address", headerFont);
            int left = (int)size.Width + 5;
            lineHeight = (int)size.Height + 3;
            firstColumnWidth = left;
            string iToS = "";
            switch (mode)
            {
                case ViewMode.Hex:
                    iToS = (1).ToString("x2");
                    break;
                case ViewMode.Bit:
                    iToS = Convert.ToString(255, 2);
                    break;
                case ViewMode.Octal:
                    iToS = Convert.ToString(16, 8);
                    break;
                case ViewMode.Dec:
                    iToS = (100).ToString();
                    break;
            }
            normalColumnWidth = (int)e.Graphics.MeasureString(iToS, headerFont).Width + 5;
            if (showHex)
            {
                #region Show hex
                for (int i = startColumn; i < numberOfColumns; i++)
                {
                    #region choose intData
                    iToS = "";
                    switch (mode)
                    {
                        case ViewMode.Hex:
                            iToS = i.ToString("x2");
                            break;
                        case ViewMode.Bit:
                            iToS = Convert.ToString(i, 2);
                            break;
                        case ViewMode.Octal:
                            iToS = Convert.ToString(i, 8);
                            break;
                        case ViewMode.Dec:
                            iToS = i.ToString();
                            break;
                    }
                    #endregion
                    e.Graphics.DrawString(iToS, headerFont, headerBrush, new PointF(left + 3, 0));
                    left += normalColumnWidth;
                }
                left += 25;
                #endregion
            }
            if (showText)
            {
                #region show text
                for (int i = 0; i < numberOfColumns; i++)
                {
                    e.Graphics.DrawString(i.ToString("x2"), headerFont, headerBrush, new PointF(left, 0));
                    left += normalColumnWidth;
                }
                #endregion
            }
            #endregion
            panelWidth = left;
     //       Trace.WriteLine(panelWidth);
            top = lineHeight;
            s = "";
            sbin = "";
            #region display lines
            for (int i = 0; i < rows.Count; i++)
            {
                rows[i].DisplayRow(e.Graphics, top, 0, startColumn, firstColumnWidth, normalColumnWidth, lineHeight, lineFont, showHex, showText, mode);
                top += lineHeight;
                s += rows[i].s + Environment.NewLine;
                sbin += rows[i].sbin + Environment.NewLine;
                if (top > this.Bottom + 50)
                    break;
            }
            #endregion
        }
        private int SelectRow(Point p)
        {
            int select = -1;
            selectedSubItem = Point.Empty;
            for (int i = 0; i < rows.Count; i++)
            {
                rows[i].Selected = false;
                if (rows[i].Contains(p.Y))
                {
                    rows[i].Selected = true;
                    select = i;
                }
                foreach (SubItem s in rows[i].SubItems)
                    s.Selected = false;
            }
            return select;
        }
        private Point FindSelectedItem(Point p)
        {
            Point select = new Point(-1, -1);
            for (int i = 0; i < rows.Count; i++)
            {
                for (int j = 0; j < rows[i].SubItems.Count; j++)
                {
                    if (rows[i].SubItems[j].Contains(p))
                    {
                        if (rows[i].SubItems[j].Selected == false)
                        {
                             select = new Point(i, j);
                        }
                    }
                }
            }
            return select;
        }
        private void UnSelect()
        {
            for (int i = 0; i < rows.Count; i++)
            {
                rows[i].Selected = false;
                selectedRow = -1;
                for (int j = 0; j < rows[i].SubItems.Count; j++)
                {
                    rows[i].SubItems[j].Selected = false;
                    rows[i].SubItems[j].PatternStart = 0;
                    rows[i].SubItems[j].BinaryPattern = "";
                    selectedSubItem = Point.Empty;
                }
            }
        }
        private void FillRows(int line, int maxLine)
        {
            rows = new List<Row>();
            if (buffer != null)
            {
                BitStreamReader ms = new BitStreamReader(buffer, false);
                int start = line * 16;
                ms.Position = start;
                while ((ms.Position<ms.Length)&(line<maxLine+startLine))
                {
                    byte[] b = ms.ReadBytes(16);
                    rows.Add(new Row(line, b));
                    line++;
                }
                ms.Close();
            }
            else
            {
                FileStream FS = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                vScrollBar1.Maximum = (int)FS.Length / 16;
                FS.Seek(startLine * 16, SeekOrigin.Begin);
                startAddress = FS.Position;
                byte[] localbuffer = new byte[16];
                FS.Read(localbuffer, 0, 16);
                rows.Add(new Row(line, localbuffer));
                line++;
                while ((FS.Position < FS.Length) && (line < maxLine + startLine))
                {
                    localbuffer = new byte[16];
                    FS.Read(localbuffer, 0, 16);
                    rows.Add(new Row(line, localbuffer));
                    line++;
                }
                currentPosition = FS.Position;
                endAddress = FS.Position;
                FS.Close();
            }
            if (selectedRow > 0)
                rows[selectedRow].Selected = true;
            if ((selectedSubItem != Point.Empty) & (selectedSubItem.X > 0) & (selectedSubItem.X < rows.Count))
                rows[selectedSubItem.X].SubItems[selectedSubItem.Y].Selected = true;
        }
        private void GotoAddress(long address)
        {
            int lineNumber = (int)address / 0x10;
            startLine = lineNumber;
         //   if ((address < startAddress) || (address > endAddress))
                FillRows(lineNumber, maxLine);
            vScrollBar1.Value = startLine;
        }
        private byte[] TextToByte(string number)
        {
            if ((number.Length % 2) != 0)
                return null;
            byte[] ou = new byte[number.Length / 2];
            NumberStyles styles = NumberStyles.HexNumber;
            int i = 0;
            while (i < number.Length)
            {
                byte o;
                string numb = number.Substring(i, 2);
                if (!byte.TryParse(numb, styles, null as IFormatProvider, out o))
                    return null;
                else
                    ou[i / 2] = o;
                i += 2;
            }
            return ou;
        }
        private byte[] TextToBinary(string number)
        {
            if ((number.Length % 8) != 0)
                return null;
            byte[] ou = new byte[number.Length / 8 + 1];
            int i = 0;
            while (i < number.Length)
            {
                int o = 0;
                for (int j = 0; j < 8; j++)
                {
                    if ((number[i + j] != '0') & (number[i + j] != '1'))
                        return null;
                    o = o * 2 + int.Parse(number[i + j].ToString());

                }
                ou[i / 8] = (byte)o;
                i += 8;
            }
            return ou;
        }
        private long Search(byte[] search, long searchStart)
        {
            if (fileName == null)
                return -1;
            FileStream FS = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            if (search == null)
                return -1;
            if (search.Length == 0)
                return -1;
            long position = -1;
            FS.Position = searchStart;
            while (FS.Position < FS.Length)
            {
                if ((int)FS.ReadByte() == search[0])
                {
                    FS.Position--;
                    long start = FS.Position;
                    byte[] buf = new byte[search.Length];
                    bool found = true;
                    FS.Read(buf, 0, search.Length);
                    for (int i = 0; i < search.Length; i++)
                    {
                        if (search[i] != buf[i])
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found)
                        position = start;
                    if (found)
                        break;
                }
            }
            FS.Close();
            return position;
        }
        #endregion
    }
    public class Row
    {
        public string s = "";
        public string sbin = "";

        #region Private members
        int width;
        int top;
        int lineHeight;
        int left;
        int dataLeft;
        int dataWidth;
        long lineNumber;
        byte[] lineData;
        private bool selected;
        private List<SubItem> subItems;
        #endregion
        #region Properties
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }
        public List<SubItem> SubItems
        {
            get { return subItems; }
            set { subItems = value; }
        }
        #endregion
        public Row(long lineNumber, byte[] lineData)
        {
            this.lineNumber = lineNumber;
            this.lineData = lineData;
            subItems = new List<SubItem>();
            for (int i = 0; i < lineData.Length; i++)
                subItems.Add(new SubItem(lineData[i]));
        }
        public bool Contains(int i)
        {
            return (top < i) & (i < top + lineHeight);
        }
        public void DisplayRow(Graphics g, int top, int left,int startColumn, int firstColumnWidth, int normalColumnWidth, int lineHeight, Font lineFont, bool showHex, bool showText, ViewMode mode)
        {
            this.top = top;
            this.lineHeight = lineHeight;
            Pen p = new Pen(Color.LightPink);
            p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            if (selected)
                g.FillRectangle(Brushes.Red, new Rectangle(0, top, firstColumnWidth, lineHeight));
            g.DrawRectangle(p, new Rectangle(0, top, firstColumnWidth, lineHeight));
            g.DrawString((lineNumber * 0x10).ToString("x8"), lineFont, Brushes.Black, new PointF(3, top));
            dataLeft = firstColumnWidth;
            dataWidth = normalColumnWidth;
            if (showHex)
            {
                for (int i = startColumn; i < lineData.Length; i++)
                {
                    subItems[i].Draw(g, new Rectangle(dataLeft - 2, top, normalColumnWidth, lineHeight), i, lineFont, true, mode);
                    dataLeft += dataWidth;
                }
                dataLeft += 25;
            }
            if (showText)
            {
                for (int i = 0; i < lineData.Length; i++)
                {
                    subItems[i].Draw(g, new Rectangle(dataLeft - 2, top, normalColumnWidth, lineHeight), i, lineFont, false, mode);
                    dataLeft += dataWidth;
                    s += subItems[i].Value.ToString("x2")+" ";
                    sbin += subItems[i].ToString();
                }
            }
        }
    }
    public class SubItem
    {
        #region Private members
        private Rectangle rectBin;
        private Rectangle rectStr;
        private bool selected;
        private int patternStart;
        private Color backColor;
        private string binaryPattern = "";
        private byte data;
        private Color selectedColor = Color.Red;
        #endregion
        #region Properties
        public byte Value
        {
            get { return data; }
            set { data = value; }
        }
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }
        public string BinaryPattern
        {
            get { return binaryPattern; }
            set { binaryPattern = value; }
        }
        public int PatternStart
        {
            get { return patternStart; }
            set { patternStart = value; }
        }
        #endregion
        public override string ToString()
        {
            return TextFromByte(data);
        }
        public SubItem(byte data)
        {
            this.data = data;
        }
        public SubItem(byte data, Color selectedColor)
        {
            this.data = data;
        }
        public bool Contains(Point p)
        {
            return rectBin.Contains(p) || rectStr.Contains(p);
        }
        public void Draw(Graphics g, Rectangle rect, int i, Font lineFont, bool hex, ViewMode mode)
        {
            int bound = 2;
            if (hex)
                this.rectBin = rect;
            else
                rectStr = rect;
            SolidBrush brush = new SolidBrush(Color.Beige);
            if (i % 2 == 0)
                g.FillRectangle(brush, rect);
            if (selected)
            {
                brush = new SolidBrush(selectedColor);
                g.FillRectangle(brush, rect);
            }
            Pen p = new Pen(Color.LightPink);
            p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            g.DrawRectangle(p, rect);
            #region String to display
            string a = TextFromByte(data);
            if (hex)
                switch (mode)
                {
                    case ViewMode.Hex:
                        a = data.ToString("x2");
                        break;
                    case ViewMode.Bit:
                        a = Convert.ToString(data, 2);
                        if (a.Length < 8)
                        {
                            int k = 8 - a.Length;
                            for (int u = 0; u <= k; u++)
                                a = "0" + a;
                        }
                        break;
                    case ViewMode.Octal:
                        a = Convert.ToString(data, 8);
                        if (a.Length < 2)
                            for (int u = 0; u <= 2 - a.Length; u++)
                                a = "0" + a;
                        break;
                    case ViewMode.Dec:
                        a = data.ToString();
                        if (a.Length < 3)
                            for (int u = 0; u <= 3 - a.Length; u++)
                                a = " " + a;
                        break;
                }
            #endregion
            if (hex & (binaryPattern != "" & mode == ViewMode.Bit))
            {
                g.FillRectangle(Brushes.LightBlue, rect);
                string s = a.Substring(0, patternStart);
                float x = rect.Left + bound;
                g.DrawString(s, lineFont, Brushes.Black, new PointF(x, rect.Top));
                SizeF size = g.MeasureString(s, lineFont, new PointF(x, rect.Top), StringFormat.GenericTypographic);
                x += size.Width;
                s = a.Substring( patternStart, binaryPattern.Length);
                g.DrawString(s, lineFont, Brushes.Red, new PointF(x, rect.Top));
                size = g.MeasureString(s, lineFont, new PointF(x, rect.Top), StringFormat.GenericTypographic);
                x += size.Width;
                s = a.Substring(patternStart + binaryPattern.Length, a.Length - (patternStart + binaryPattern.Length));
                g.DrawString(s, lineFont, Brushes.Black, new PointF(x, rect.Top));
            }
            else
                g.DrawString(a, lineFont, Brushes.Black, new Point(rect.Left + bound, rect.Top));
        }
        private static string TextFromByte(byte b)
        {
            string text = "";
            Encoding enc = Encoding.Default;
            if (b < 0x20)
            {
                text += ".";
            }
            else
            {
                if ((b == 0x00))
                    text += " ";
                else
                {
                    byte[] c = new byte[1];
                    c[0] = b;
                    text += enc.GetString(c, 0, c.Length);
                }
            }
            return text;
        }
    }
    public enum SearchType { Hex, Ansi, Unicode, Address, Binary }
    public enum ViewMode { Bit, Octal, Hex, Dec }
    public enum ScrollMode { Linear, Sector }
}
