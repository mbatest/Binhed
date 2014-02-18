using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BinHed
{
    public partial class DataInspector : UserControl
    {
        public DataInspector()
        {
            InitializeComponent();
        }
        public void ShowData(byte[] b)
        {
            listView1.Items.Clear();
            ListViewItem lv = new ListViewItem("Byte array");
            string bs = "";
            for (int u = 0; u < b.Length; u++)
                bs += b[u].ToString("x2") + " ";
            lv.SubItems.Add(bs);
            listView1.Items.Add(lv); 
            lv = new ListViewItem("Byte");
            lv.SubItems.Add(b[0].ToString("x2"));
            listView1.Items.Add(lv);
            if (b.Length < 2) return;
             short s = BitConverter.ToInt16(b, 0);
            lv = new ListViewItem("Short");
            lv.SubItems.Add(s.ToString());
            ushort us = BitConverter.ToUInt16(b, 0);
            lv = new ListViewItem("Unsigned Short");
            lv.SubItems.Add(us.ToString());
            listView1.Items.Add(lv);
            if (b.Length < 4) return;
            int i = BitConverter.ToInt32(b, 0);
            lv = new ListViewItem("Integer");
            lv.SubItems.Add(i.ToString());
            listView1.Items.Add(lv);
            uint ui = BitConverter.ToUInt32(b, 0);
            lv = new ListViewItem("Unsigned Integer");
            lv.SubItems.Add(ui.ToString());
            listView1.Items.Add(lv);
            if (b.Length < 8) return;
            long l = BitConverter.ToInt64(b, 0);
            lv = new ListViewItem("Long Integer");
            lv.SubItems.Add(l.ToString());
            listView1.Items.Add(lv);
            ulong ul = BitConverter.ToUInt64(b, 0);
            lv = new ListViewItem("Unsigned Long Integer");
            lv.SubItems.Add(ul.ToString());
            listView1.Items.Add(lv);
            double r = BitConverter.ToDouble(b, 0);
            lv = new ListViewItem("Float");
            lv.SubItems.Add(r.ToString());
            listView1.Items.Add(lv);
            lv = new ListViewItem("String");
            lv.SubItems.Add(Encoding.Default.GetString(b));
            listView1.Items.Add(lv);
            lv = new ListViewItem("Unicode");
            lv.SubItems.Add(Encoding.Unicode.GetString(b));
            listView1.Items.Add(lv);

        }
    }
}
