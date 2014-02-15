using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BookReader
{
    public partial class PdfViewer : UserControl
    {
        PdfReader reader;
        public PdfViewer()
        {
            InitializeComponent();
        }
        public void Init(string FileName)
        {
            reader = new PdfReader(FileName);
            treeView1.Nodes.Add(AddNode(reader.Root));
        }
        private TreeNode AddNode(PdfObject obj)
        {
                TreeNode t = new TreeNode();
                t.Text = obj.ToString();
                t.Tag = obj;
                AddKeys(t,obj.Dictionnary );
                foreach (PdfObject p in obj.Attributes)
                {
                    if (p != null)
                    {
                        TreeNode tp = new TreeNode();
                        tp.Text = p.ToString();
                        tp.Tag = p;
                        t.Nodes.Add(tp);
                    }
              }
                return t;
        }
        private void AddKeys(TreeNode root, Dictionary<string, string> dic)
        {
            foreach (KeyValuePair<string, string> kvp in dic)
            {
                TreeNode tk = new TreeNode(kvp.Key + " : " + kvp.Value);
                string t = kvp.Value;
                switch (t[0])
                {
                    case '<':
                        t = kvp.Value.Replace("<<", "").Replace(">>", "");
                        break;
                    case '[':
                        t = kvp.Value.Replace("[", "").Replace("]", "");
                        break;
                    default:
                        break;
                }
                t = t.Trim();
                string[] s = t.Split(' ');
                if (t.Contains("R"))
                {
                    string text = "";
                    int lev = Array.IndexOf(s, "R", 0);
                    while ((lev >= 0) && (lev < s.Length))
                    {
                        text = s[lev - 2] + " " + s[lev - 1] + " " + s[lev];
                        TreeNode tn = new TreeNode(text);
                        tk.Nodes.Add(tn);
                        lev = Array.IndexOf(s, "R", lev + 1);
                    }
                }
                else
                {
                    if (s.Length > 1)
                        foreach (string ss in s)
                        {
                            if (ss.Trim() != "")
                            {
                                TreeNode tn = new TreeNode(ss);
                                tk.Nodes.Add(tn);
                            }
                        }
                    root.Nodes.Add(tk);
                }
            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
            PdfObject pdf = (PdfObject)treeView1.SelectedNode.Tag;
            if (pdf == null)
                return;
            textBox1.DocumentText = "";
            richTextBox1.Text = "";
            if (treeView1.SelectedNode.Nodes.Count == 0)
                AddKeys(treeView1.SelectedNode, pdf.Dictionnary);
            if (pdf.SubType == "Image")
            {
            }
            if (pdf.Attributes.Count == 0)
            {
            //    if (pdf.Dictionnary.Count == 0)
                    reader.ParseDic(pdf);
               foreach (PdfObject p in pdf.Attributes)
                {
                    treeView1.SelectedNode.Nodes.Add(AddNode(p));
                }
            }
            treeView1.SelectedNode.Expand();
            if (pdf == null)
                return;
            if (pdf.DecodedStream != null)
            {
                textBox1.DocumentText = pdf.DecodedStream;
                string t = pdf.DecodedStream;
                richTextBox1.Text = InnerText(t);
            }
            else if (pdf.RawStreamData != null)
            {
                textBox1.DocumentText = encoder.GetString( pdf.RawStreamData);
                richTextBox1.Text= textBox1.DocumentText;
            }
            else if (pdf.StreamData != null)
                textBox1.DocumentText = encoder.GetString(pdf.StreamData);
            else if (pdf.RawData != null)
                textBox1.DocumentText = pdf.RawData;
        }

        private string InnerText(string t)
        {
            string toShow = "";
            int start = 0;
            while (start < t.Length)
            {
                if (toShow.Contains("suf"))
                {
                }
                start = t.IndexOf('(', start) + 1;
                if (start > 0)
                {
                    int f = t.IndexOf(')', start);
                    if (f > -1)
                        toShow += t.Substring(start, f - start);
                    else
                        break;
                    start++;
                }
                else
                    break;
            }
            return toShow;
        }
    }
}
