using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Code
{
    public partial class ShowCode : UserControl
    {
        public ShowCode()
        {
            InitializeComponent();
            codeLines.Columns.Add("Offset", 80);
            codeLines.Columns.Add("Binary", 150);
            codeLines.Columns.Add("Code", 300);
        }
        public void Init(Executable exe)
        {
            codeLines.Items.Clear();
            long position = exe.Disassembly.lines.Values[0].PositionOfStructureInFile;
            for (int i = 0; i < exe.Disassembly.lines.Count;i++ )
            {
                CodeLine c = exe.Disassembly.lines.Values[i];
                if (c.PositionOfStructureInFile - position > 2)
                {
                    ListViewItem lv = new ListViewItem("Gap : ");
                    lv.SubItems.Add((exe.NT_Headers.OptionalHeader.BaseOfCode + position).ToString("x8"));
                    lv.SubItems.Add((exe.NT_Headers.OptionalHeader.BaseOfCode + c.PositionOfStructureInFile).ToString("x8"));
                    codeLines.Items.Add(lv);
                    lv.BackColor = Color.Green;
                }
                 try
                {
                    long adress = exe.NT_Headers.OptionalHeader.ImageBase + exe.NT_Headers.OptionalHeader.BaseOfCode + c.PositionOfStructureInFile;
                    ListViewItem lv = new ListViewItem(adress.ToString("x8"));// exe.NT_Headers.OptionalHeader.BaseOfCode.ToString("x8") + ":" + callOrJump.PositionOfStructureInFile.ToString("x8"));
                    lv.SubItems.Add(c.BinaryCode);
                    lv.SubItems.Add(c.ToString());
                    if (c.ToString().Contains("RET"))
                        lv.BackColor = Color.Red;
                    if (c.ToString().Contains("CALL"))
                        lv.BackColor = Color.Blue;
                    if (c.ToString().StartsWith("J"))
                        lv.BackColor = Color.Yellow;
                    if (c.ToString().Contains("Spare"))
                        lv.BackColor = Color.LightGreen;
                    if (c.ToString().Contains("raw data"))
                        lv.BackColor = Color.LightSalmon;
                    if (c.ToString().Contains("label"))
                        lv.BackColor = Color.LightSkyBlue;
                    if (c.ToString().Contains("Unknown"))
                        lv.BackColor = Color.LightSlateGray;
                    codeLines.Items.Add(lv);
                    if (codeLines.Items.Count > 15000) return;
                }
                catch { }
                position = c.PositionOfStructureInFile + c.BinaryCode.Length / 2;
                if (c.BinaryCode.Length % 2 == 1)
                    position++;
            }
            return;
        }
        public void SelectLine(long ln)
        {
            foreach (ListViewItem lv in codeLines.Items)
            {
                lv.Selected = false;
                lv.ForeColor = Color.Black;
            }
            foreach (ListViewItem lv in codeLines.Items)
            {
                if (lv.Text == ln.ToString("x8"))
                {
                    lv.Selected = true;
                    lv.EnsureVisible();
                    lv.ForeColor = Color.Red;
                    break;
                }
            }
        }
    }
}
