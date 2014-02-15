using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BookReader;

namespace BookReader
{
    public partial class LRFViewer : UserControl
    {
        TreeNode root;
        public LRFFileReader rf;
        int currentBlock;
        int currentPageNumber;
        TextBlock currentText;
        Page currentPage;
        public LRFViewer()
        {
            InitializeComponent();

        }
        public void Set(string FileName)
        {
            rf = new LRFFileReader(FileName);
         //   rf.ParseXml();
            rf.Structure();
            root = new TreeNode(rf.Book_Structure.ToString());
            root.Tag = rf.Book_Structure;
            treeView1.Nodes.Add(root);
            foreach (Page p in rf.Pages)
            {
                TreeNode tnp = new TreeNode(p.ToString());
                tnp.Tag = p;
                root.Nodes.Add(tnp);
                foreach(Block b in p.Blocks)
                {
                    TreeNode tnb = new TreeNode(b.ToString());
                    tnb.Tag = b;
                    tnp.Nodes.Add(tnb);
                    foreach (TextBlock tb in b.text)
                    {
                        TreeNode tnt = new TreeNode(tb.ToString());
                        tnt.Tag = tb;
                        tnb.Nodes.Add(tnt);
                        TreeNode tatt= new TreeNode(tb.attribs.ToString());
                        tnt.Nodes.Add(tatt);
                        tatt.Nodes.Add(new TreeNode(tb.text.LrfTags[1].Data.ToString()));
                    }

                }
            }
  //          node.ExpandAll();
            currentPageNumber = 0;
            currentBlock = 0;
            if (rf.Textblocks.Count > 0)
                currentText = rf.Textblocks[currentBlock];
            currentPage = rf.Pages[currentPageNumber];
            webBrowser1.DocumentText = currentPage.PageText;
            if (rf.CoverImage != null)
                pictureBox1.Image = rf.CoverImage;
            
            textBox1.Text = rf.Comment;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode cur = treeView1.SelectedNode;
            if (cur == null)
                return;
            if (cur.ToString().IndexOf("Text") >= 0)
            {
                TextBlock t = (TextBlock)cur.Tag;
               webBrowser1.DocumentText = t.t;
            }
        }
        private void forward_Click(object sender, EventArgs e)
        {
            if (currentPageNumber < rf.Pages.Count - 1)
            {
                try
                {
                    currentPageNumber++;
                    currentPage = rf.Pages[currentPageNumber]; ;
                    webBrowser1.DocumentText = currentPage.PageText;
                }
                catch (Exception ex) { }
            }
            
        }
        private void back_Click(object sender, EventArgs e)
        {
            if (currentPageNumber >= 1)
            {
                currentPageNumber--;
                currentPage = rf.Pages[currentPageNumber]; ;
                webBrowser1.DocumentText = currentPage.PageText;
            }

        }

        private void showStructure_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = !splitContainer1.Panel1Collapsed;
        }
    }
}
