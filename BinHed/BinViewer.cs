using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace BinHed
{
    public partial class BinViewer : UserControl
    {
        private string fileName;
        private string mapName;
        long currentPosition;
        int lengthBlock;
        List<byte[]> lineBuffers;
        public BinViewer()
        {
            InitializeComponent();
          }
        public void Init(string fileName)
        {
            this.fileName = fileName;
            mapName = "file";
            lineBuffers = new List<byte[]>();
            MemoryMappedFile mf = null;
            try
            {
                mf = MemoryMappedFile.CreateFromFile(fileName, FileMode.Open, mapName);
                currentPosition = 0;
                lengthBlock = 16;
                int maxLine = 50;
                int line = 0;
                if (mf != null)
                {
                    using (MemoryMappedViewStream FS = mf.CreateViewStream())
                    {
                        FS.Seek(currentPosition, SeekOrigin.Begin);
                        byte[] buffer = new byte[16];
                        FS.Read(buffer, 0, 16);
                        lineBuffers.Add(buffer);
                        while ((FS.Position < FS.Length) && (line < maxLine))
                        {
                            buffer = new byte[16];
                            FS.Read(buffer, 0, 16);
                            lineBuffers.Add(buffer);
                            line++;
                        }
                        currentPosition = FS.Position;
                        FS.Close();
                        FillListBoxes();
                    }
                }
            }
            catch { }
            finally 
            {
                if (mf != null)
                    mf.Dispose();
            }

        }
        public void SetHex()
        {
            grid1.SetHex();
            grid2.SetHex();
            FillListBoxes();
        }
    }
}
