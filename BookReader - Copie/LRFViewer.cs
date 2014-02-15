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
            rf.Parse();
            rf.Structure();
            if (this.Parent != null)
                this.Parent.Text = "LRFViewer : " + rf.Author + " - " + rf.Title;
            root = new TreeNode(rf.root.ToString());
            root.Tag = rf.root;
            treeView1.Nodes.Add(root);
            foreach (Page p in rf.pages)
            {
                TreeNode tnp = new TreeNode(p.ToString());
                tnp.Tag = p;
                root.Nodes.Add(tnp);
                foreach(Block b in p.blocks)
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
                        tatt.Nodes.Add(new TreeNode(tb.text.lrfObjects[1].data.ToString()));
                    }

                }
            }
  //          node.ExpandAll();
            currentPageNumber = 0;
            currentBlock = 0;
            if (rf.textblocks.Count > 0)
                currentText = rf.textblocks[currentBlock];
            currentPage = rf.pages[currentPageNumber];
            webBrowser1.DocumentText = currentPage.pageText;
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
            if (currentPageNumber < rf.pages.Count - 1)
            {
                try
                {
                    currentPageNumber++;
                    currentPage = rf.pages[currentPageNumber]; ;
                    webBrowser1.DocumentText = currentPage.pageText;
                }
                catch (Exception ex) { }
            }
            
        }
        private void back_Click(object sender, EventArgs e)
        {
            if (currentPageNumber >= 1)
            {
                currentPageNumber--;
                currentPage = rf.pages[currentPageNumber]; ;
                webBrowser1.DocumentText = currentPage.pageText;
            }

        }

        private void showStructure_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = !splitContainer1.Panel1Collapsed;
        }
    }
}
