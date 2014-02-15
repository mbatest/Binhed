using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BookReader;

namespace BinHed
{
    public partial class LRFView : Form
    {
        public LRFFileReader rf;

        public LRFView(string FileName)
        {
            InitializeComponent();
            lrfViewer1.Set(FileName);
            rf = lrfViewer1.rf;
        }
    }
}
