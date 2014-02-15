using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BinHed
{
    public partial class Grid : ListView
    {
        #region Private members
        TextBox editBox = new TextBox();
        ListViewItem.ListViewSubItem sub;
        List<byte[]> lineBuffers;
        bool lineNumberHex;// How to display the Line numbers
        bool Hex = false;// Hexadecimal mode
        int selectedRow;
        int selectedColumn;
        int selectedLine;
        #endregion
        #region Public methods
        public event EventHandler SubItemSelected;
        public event EventHandler LineSelected;
        public Grid()
        {
            InitializeComponent();
            listView1.Controls.Add(editBox);
            editBox.Hide();
        }
        public int SelectedColumn
        {
            get { return selectedColumn; }
            set { selectedColumn = value; }
        }
        public int SelectedRow
        {
            get { return selectedRow; }
            set { selectedRow = value; }
        }
        public int SelectedLine
        {
            get { return selectedLine; }
            set { selectedLine = value; }
        }
        public int TopItem
        {
            get { return listView1.TopItem.Index; }
        }
        public bool LineNumberHex
        {
            get { return lineNumberHex; }
            set { lineNumberHex = value; }
        }
        public void SetHex()
        {
            lineNumberHex = !lineNumberHex;
            listView1.Items.Clear();
            for (int line = 0; line < lineBuffers.Count; line++)
            {
                FillLine(line, lineBuffers[line], Hex);
            }
        }
        public void Clear()
        {
            listView1.Items.Clear();
        }
        public void Init(List<string> columnHeaders)
        {
            foreach (string s in columnHeaders)
            {
                listView1.Columns.Add(s, 30);
            }
        }
        public void InitHex(bool hex)
        {
            listView1.Columns.Add("Line number", 80);
            int sz = 22;
            if (hex)
                sz = 25;
            for (int i = 0; i < 16; i++)
            {
                listView1.Columns.Add(i.ToString("x2"), sz);
            }
            Hex = hex;
        }
        public void FillHex(List<byte[]> lineBuffers, bool hex)
        {
            this.lineBuffers = lineBuffers;
            for (int line = 0; line < lineBuffers.Count; line++)
            {
                FillLine(line, lineBuffers[line], hex);
            }
        }
        public void FillLine(int lineNumber, byte[] buffer, bool hex)
        {
            int start = lineNumber * 16;
            int end = start + 16;
            string lineText = lineNumber.ToString("x4") + ":" + start.ToString("x6");
            ListViewItem lv = new ListViewItem(lineText);
            lv.UseItemStyleForSubItems = false;
            lv.Font = new Font("Courrier New", 6.75F);
            if (hex)
            {
                for (int i = 0; i < 16; i++)
                {
                    Color back = Color.White;
                    if ((i % 2) == 1)
                    { back = Color.Beige; }
                    ListViewItem.ListViewSubItem lvs = new ListViewItem.ListViewSubItem(lv, buffer[i].ToString("x2"), Color.Black, back, new Font("Arial", 8));
                    lv.SubItems.Add(lvs);
                    lvs.Tag = "Data";
                }
                listView1.Items.Add(lv);
            }
            else
            {
                for (int i = 0; i < 16; i++)
                {
                    Color back = Color.White;
                    if ((i % 2) == 1)
                    { back = Color.Beige; }
                    byte[] b = new byte[1];
                    b[0] = buffer[i];
                    string text = TextFromByte(b);
                    ListViewItem.ListViewSubItem lvs = new ListViewItem.ListViewSubItem(lv, text, Color.Black, back, new Font("Arial", 8));
                    lv.SubItems.Add(lvs);
                    lvs.Tag = "Data";
                }
                listView1.Items.Add(lv);
            }
        }
        /*       public void SelectSubItem(int row, int column)
               {
                   sub = listView1.Items[row].SubItems[column];
                   SelectSubItem();
               }*/
        public void UpdateSubItem(int row, int column)
        {
            sub = listView1.Items[row].SubItems[column];
            byte[] b = new byte[1];
            b[0] = lineBuffers[row][column];
            if (!Hex)
                sub.Text = TextFromByte(b);
            else
                sub.Text = b[0].ToString("x2");

        }
        public void SelectLine(int lineNumber, int TopItem)
        {
            listView1.SelectedIndices.Clear();
            //           listView1.SelectedIndices.Add(lineNumber);
            listView1.Focus();
            listView1.Items[lineNumber].Selected = true;
            listView1.EnsureVisible(lineNumber);
            listView1.TopItem = listView1.Items[TopItem];
            //  listView1.Select();
            //   listView1.Focus();
        }
        #endregion
        #region Private methods
        private void FillLine(object[] objets)
        {
            ListViewItem lv = new ListViewItem(objets[0].ToString());
            lv.UseItemStyleForSubItems = false;
            for (int i = 1; i < objets.Length; i++)
            {
                Color back = Color.White;
                if ((i % 2) == 1)
                { back = Color.Beige; }
                ListViewItem.ListViewSubItem lvs = new ListViewItem.ListViewSubItem(lv, objets[i].ToString(), Color.Black, back, new Font("Arial", 8));
                lv.SubItems.Add(lvs);
            }
            listView1.Items.Add(lv);
        }
        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo lv = listView1.HitTest((e.Location));
            sub = lv.SubItem;
            if (sub == null)
                return;
            if ((string)sub.Tag != "Data")
            {
                if (listView1.SelectedIndices.Count > 0)
                {
                    selectedLine = listView1.SelectedIndices[0];
                    editBox.Hide();
                    if (LineSelected != null)
                        LineSelected(this, new EventArgs());
                }
            }
            else
            {
                if (lv.Item.SubItems.IndexOf(sub) > 0)
                {
                    selectedRow = lv.Item.Index;
                    selectedColumn = lv.Item.SubItems.IndexOf(sub);
                    int start = lv.Item.Index * 16 + selectedColumn - 1;
                    lv.Item.Text = lv.Item.Index.ToString("x4") + ":" + start.ToString("x6");
                    SelectSubItem();
                }
            }
        }
        private void SelectSubItem()
        {
            editBox.Text = sub.Text;
            editBox.Focus();
            editBox.SelectAll();
            editBox.Bounds = sub.Bounds;
            editBox.LostFocus += new EventHandler(t_LostFocus);
            editBox.Show();
        }
        private void t_LostFocus(object sender, EventArgs e)
        {
            if (ValidateText(editBox.Text))
            {
                if (Hex)
                {
                    sub.Text = editBox.Text.ToLower();
                    lineBuffers[selectedRow][selectedColumn] = TextToByte(sub.Text);
                }
                else
                {
                    sub.Text = editBox.Text;
                    lineBuffers[selectedRow][selectedColumn] = CharToByte(sub.Text[0]);
                }
                if (SubItemSelected != null)
                    SubItemSelected(this, new EventArgs());
            }
            int start = selectedRow * 16;
            listView1.Items[selectedRow].Text = selectedRow.ToString("x4") + ":" + start.ToString("x6");

            editBox.Hide();
        }
        private bool ValidateText(string t)
        {
            bool ok = true;
            Regex r = new Regex(@"^[A-Fa-f0-9]*$");
            if (Hex)
            {
                if (t.Length != 2)
                {
                    ok = false;
                }
                else if (!r.IsMatch(t))
                    ok = false;
            }
            else
            {
                if (t.Length != 1)
                {
                    ok = false;
                }
            }
            return ok;
        }
        private static string TextFromByte(byte[] b)
        {
            string text = "";
            Encoding enc = Encoding.Default;
            if (b[0] < 0x20)
            {
                text += ".";
            }
            else
            {
                if ((b[0] == 0x00))
                    text += " ";
                else
                {
                    text += enc.GetString(b, 0, b.Length);
                }
            }
            return text;
        }
        private byte TextToByte(string numb)
        {
            byte o;
            NumberStyles styles = NumberStyles.HexNumber;
            if (byte.TryParse(numb, styles, null as IFormatProvider, out o))
                return o;
            else return 0;
        }
        private byte CharToByte(char c)
        {
            byte o = (byte)c;
            return o;
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        #endregion
    }
}


