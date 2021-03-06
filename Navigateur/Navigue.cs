﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Security.AccessControl;


namespace Navigateur
{
    public partial class Navigue : UserControl
    {
        public class SelectedIndexEventArg : EventArgs
        {
            private string fileName;
            public string FileName
            {
                get { return fileName; }
                set { fileName = value; }
            }
            public SelectedIndexEventArg(string file)
            {
                fileName = file;
            }
        }
        public delegate void SelectedIndexEvent(SelectedIndexEventArg ev);
        public event SelectedIndexEvent NavIndexChanged;
        public Navigue()
        {
            InitializeComponent();
        }
        public bool Init()
        {
            repertoire.Nodes.Clear();
            string[] drives = Environment.GetLogicalDrives();
            foreach (string strDrive in drives)
            {
                DriveInfo drv = new DriveInfo(strDrive);
                string driveName = strDrive;
                if (drv.DriveType == DriveType.Fixed)
                {
                    driveName += " " + drv.VolumeLabel;
                    TreeNode tn = new TreeNode(driveName, 0, 1);
                    tn.Tag = drv;
                    repertoire.Nodes.Add(tn);
                }
                if ((drv.DriveType == DriveType.CDRom) && drv.IsReady)
                {
                    driveName += " " + drv.VolumeLabel;
                    TreeNode tn = new TreeNode(driveName, 0, 1);
                    tn.Tag = drv;
                    repertoire.Nodes.Add(tn);
                }
            }
            
            return true;
        }
        private void repertoire_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode tn = repertoire.SelectedNode;
            if (tn.Nodes.Count == 0)
            {
                Type tp = tn.Tag.GetType();
                string path = "";
                switch (tp.Name)
                {
                    case "DriveInfo":
                        DriveInfo dr = (DriveInfo)tn.Tag;
                        path = dr.Name;
                        break;
                    case "String":
                        path = (string)tn.Tag;
                        break;
                }
                if (Directory.Exists(path))
                {
                    try
                    {
                        DirectoryInfo di = new DirectoryInfo(path);
                        FileInfo fi = new FileInfo(path);
                        string[] files = Directory.GetFiles(path);
                        foreach (string s in files)
                        {
                            string a = Path.GetFileName(s);
                            TreeNode ttn = new TreeNode(a, 2, 2);
                            ttn.Tag = s;
                            if (tn.Checked)
                                ttn.Checked = true;
                            tn.Nodes.Add(ttn);
                        }
                        string[] nodes = Directory.GetDirectories(path);
                        foreach (string s in nodes)
                        {
                            string a = Path.GetFileName(s);
                            TreeNode ttn = new TreeNode(a, 0, 1);
                            ttn.Tag = s;
                            if (tn.Checked)
                                ttn.Checked = true;
                            tn.Nodes.Add(ttn);
                        }
                        tn.Expand();
                    }
                    catch { }
                }
                #region un fichier est sélectionné
                if (tn.Nodes.Count == 0)
                {
                    if (File.Exists(path))
                        if (NavIndexChanged != null)
                        {
                            NavIndexChanged(new SelectedIndexEventArg(path));
                        }
                }
                #endregion
            }
        }
    }
}
