using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Navigateur;
using LowLevel;
using Utils;
using Microsoft.Win32;
using System.Diagnostics;

namespace BinHed
{
    public partial class Editor : Form
    {
        List<string> FileList = new List<string>();
        List<string> extensions = new List<string>();
        public Editor()
        {
            InitializeComponent();
            #region Extensions informations
            RegistryKey rkRoot = Registry.ClassesRoot;
            string[] keyNames = rkRoot.GetSubKeyNames();
            foreach (string s in keyNames)
            {
                if ((s.StartsWith(".")) && (s.Length < 7))
                {
                    RegistryKey rkFileType = rkRoot.OpenSubKey(s);
                    object defaultValue = rkFileType.GetValue("");
                    extensions.Add(s + ":" + (string)defaultValue);
                }
            }
            #endregion
            navigator.Init();
            ShowExplorer();
            tabs.TabPages.Clear();
        }
        private void refresh_Click(object sender, EventArgs e)
        {
            navigator.Init();
        }

        private void navigator_NavIndexChanged(Navigue.SelectedIndexEventArg ev)
        {
            splitContainer4.Panel1Collapsed = true;
            if (Array.IndexOf(FileList.ToArray(), ev.FileName) < 0)
            {
                string f = ev.FileName;
                FileList.Add(f);
                OpenFile(f);
            }
            else
            {
                tabs.SelectedTab = null;
                foreach (TabPage tp in tabs.TabPages)
                {
                    if (tp.Text == Path.GetFileName(ev.FileName))
                    {
                        tabs.SelectedTab = tp;
                        break;
                    }
                }
                if (tabs.SelectedTab == null)
                {
                    FileList.Add(ev.FileName);
                    TabPage tp = new TabPage();
                    FileEdit f = new FileEdit();
                    f.DataEvent += new DataEventHandler(fileEdit_DataEvent);
                    tp.Controls.Add(f);
                    f.Dock = DockStyle.Fill;
                    f.OpenFile(ev.FileName);
                    tp.Text = Path.GetFileName(ev.FileName);
                    tabs.TabPages.Add(tp);
                    tabs.SelectedTab = tp;
                }
            }
        }

        private void OpenFile(string f)
        {
            TabPage tp = new TabPage();
            FileEdit fileEdit = new FileEdit();
            fileEdit.DataEvent += new DataEventHandler(fileEdit_DataEvent);
            tp.Controls.Add(fileEdit);
            fileEdit.Dock = DockStyle.Fill;
            fileEdit.OpenFile(f);
            tp.Text = Path.GetFileName(f);
            tabs.TabPages.Add(tp);
            tabs.SelectedTab = tp;
        }
        private void ouvrirToolStripButton_Click(object sender, EventArgs e)
        {
            ShowExplorer();
        }
        private void ShowHardware()
        {
            TabPage tp = new TabPage();
            tp.Text = "Computer data";
            HardwareViewer hd = new HardwareViewer();
            tp.Controls.Add(hd);
            hd.Dock = DockStyle.Fill;
            tabs.TabPages.Add(tp);
            tabs.SelectedTab = tp;

        }
        private void fileEdit_DataEvent(object sender, DataEventArgs e)
        {
            dataInspector1.ShowData(e.Data);
        }
        private void explorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowExplorer();
        }
        private void ShowExplorer()
        {
            splitContainer4.Panel1Collapsed = false;
            splitContainer1.Panel2Collapsed = true;
        }
        private void dataInspectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer4.Panel1Collapsed = false;
            splitContainer1.Panel1Collapsed = true;
            splitContainer1.Panel2Collapsed = false;
        }
        private void hardwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHardware();
        }
        private void rawDiskAccessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region elevation
            // Elevate the process if it is not run as administrator. 
            if (!Helpers.IsRunAsAdmin())
            {
                // Launch itself as administrator 
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = Application.ExecutablePath;
                proc.Verb = "runas";


                try
                {
                    Process.Start(proc);
                }
                catch
                {
                    // The user refused the elevation. 
                    // Do nothing and return directly ... 
                    return;
                }
                Application.Exit();  // Quit itself 
            }
            else
            {
                MessageBox.Show("The process is running as administrator", "UAC");
                splitContainer4.Panel1Collapsed = true;
                TabPage tp = new TabPage();
                FileEdit f = new FileEdit();
                f.DataEvent += new DataEventHandler(fileEdit_DataEvent);
                tp.Controls.Add(f);
                f.Dock = DockStyle.Fill;
                tp.Text = "Raw disk access";
                tabs.TabPages.Add(tp);
                tabs.SelectedTab = tp;
                f.RawDiskAccess();
            }

            #endregion

        }

        private void nouveauToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog opfd = new OpenFileDialog();
            if(opfd.ShowDialog()== System.Windows.Forms.DialogResult.OK)
            {
                OpenFile(opfd.FileName);
            }

        }

 
    }
}