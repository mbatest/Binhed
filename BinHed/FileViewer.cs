using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utils;

namespace BinHed
{
    public partial class FileViewer : UserControl
    {
        public event DataSelectedEventHandler dataSelected;
        public event DataRequestEventHandler dataRequested;
        ImageList iml;
        ILOCALIZED_DATA im;
        public FileViewer()
        {
            InitializeComponent();
        }
        public void Init(ILOCALIZED_DATA im)
        {
            Propriétés.Nodes.Clear();
            this.im = im;
            TreeNode root = new TreeNode("File data", 0, 1);
            Bitmap b = new Bitmap(20, 20);
            iml = new ImageList();
            iml.Images.Add(b);
            iml.ImageSize = new System.Drawing.Size(20, 20);
            FillTreeNode(root, im);
            root.Tag = im;
            Propriétés.Nodes.Add(root);
        }
        private bool FillTreeNode(TreeNode root, ILOCALIZED_DATA currentNode)
        {
            bool filled = true;
            Type t = currentNode.GetType();
            root.Nodes.Clear();
            System.Reflection.PropertyInfo[] p = t.GetProperties();
            foreach (System.Reflection.PropertyInfo v in p)
            {
                try
                {
                    object[] att = v.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    string s = v.Name.Replace("_", " ") + " ";
                    if (att.Length > 0)
                    {
                        s = ((DescriptionAttribute)att[0]).Description + " ";
                    }
                    PropertyDescriptorCollection col = TypeDescriptor.GetProperties(t);
                    if (currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null) != null)
                    {
                        if (v.PropertyType.IsArray)
                        {
                            #region Arrays
                            Array a = (Array)currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null);
                            if (a.Length > 0)
                            {
                                TreeNode ts = new TreeNode(s, 0, 1);
                                ts.Tag = currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null);
                                root.Nodes.Add(ts);

                                foreach (var element in a)
                                {
                                    if (element != null)
                                    {
                                        TreeNode ta = new TreeNode(element.ToString(), 0, 1);
                                        ta.Tag = element;
                                        ts.Nodes.Add(ta);
                                    }
                                }
                            }
                            #endregion
                        }
                        else
                            if (v.PropertyType.IsGenericType)
                            {
                                #region Lists
                                if (v.PropertyType.Name.Contains("List"))
                                {
                                    Type[] tp = v.PropertyType.GetGenericArguments();
                                    IList a = (IList)currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null);
                                    if (a.Count > 0)
                                    {
                                        TreeNode ts = new TreeNode("List "+ s, 0, 1);
                                        ts.Tag = currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null);
                                        root.Nodes.Add(ts);

                                        foreach (var x in a)
                                        {
                                            if (x.GetType().IsGenericType)
                                            {
                                                TreeNode ta = new TreeNode("List", 0, 1);
                                                ta.Tag = x;
                                                ts.Nodes.Add(ta);
                                                if (x.GetType().Name.Contains("List"))
                                                {
                                                    IList an = (IList)x;
                                                    if (an.Count > 0)
                                                    {
                                                        foreach (var xk in an)
                                                        {
                                                            TreeNode tan = new TreeNode(xk.ToString(), 0, 1);
                                                            tan.Tag = xk;
                                                            ta.Nodes.Add(tan);

                                                        }
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                string details = "";
                                                if ((x.GetType().Name == "String")||(x.GetType().Name == "CodeLine"))

                                                    details = x.ToString();
                                                else
                                                    details = x.GetType().Name;

                                                TreeNode ta = new TreeNode(details, 0, 1);
                                                ta.Tag = x;
                                                ts.Nodes.Add(ta);
                                            }

                                        }
                                    }
                                }
                                else if (v.PropertyType.Name.Contains("Nullable"))
                                {
                                    Type[] tp = v.PropertyType.GetGenericArguments();
                                    var a = currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null);
                                    switch (tp[0].Name)
                                    {
                                        case "DateTime":
                                            TreeNode ts = new TreeNode(v.Name + " "+ a.ToString(), 0, 1);
                                            ts.Tag = v;
                                            root.Nodes.Add(ts);
                                            break;
                                    }
                                }

                                #endregion
                            }
                            else
                            {
                                #region Display according to type
                                switch (currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null).GetType().Name)
                                {
                                    #region Primitive types
                                    case "Byte":
                                        byte ub = (byte)currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null);
                                        s += "0x" + ub.ToString("x2");
                                        break;
                                    case "Int16":
                                        short us = (short)currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null);
                                        s += "0x" + us.ToString("x4");
                                        break;
                                    case "UInt16":
                                        ushort u = (ushort)currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null);
                                        s += "0x" + u.ToString("x4");
                                        break;
                                    case "Int32":
                                        int uis = (int)currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null);
                                        s += "0x" + uis.ToString("x8");
                                        break;
                                    case "UInt32":
                                        uint ui = (uint)currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null);
                                        s += "0x" + ui.ToString("x8");
                                        break;
                                    case "UInt64":
                                        ulong uul = (ulong)currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null);
                                        s += "0x" + uul.ToString("x16");
                                        break;
                                    case "Int64":
                                        long ul = (long)currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null);
                                        s += "0x" + ul.ToString("x16");
                                        break;
                                    case "String":
                                        if (currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null).ToString() == "")
                                            s = "";
                                        else
                                            s += currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null).ToString();
                                        break;
                                    case "Bitmap":
                                        s = "";
                                        iml.Images.Add((Bitmap)currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null));
                                        TreeNode tal = new TreeNode("Album Art", iml.Images.Count - 1, iml.Images.Count - 1);
                                        Propriétés.ImageList = iml;
                                        root.Nodes.Add(tal);
                                        break;
                                    case "Bool":
                                        break;
                                    case "Byte[]":
                                        byte[] ubs = (byte[])currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null);
                                        s += "0x";
                                        foreach (byte uby in ubs)
                                            s += uby.ToString("x2");

                                        break;
                                    #endregion
                                    default:
                                        #region class type
                                        switch (v.PropertyType.Name)
                                        {
                                            case "Boolean":
                                            case "ELEMENTARY_TYPE":
                                                s += currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null).ToString();
                                                break;
                                            case "DateTime":
                                                s += currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null).ToString();
                                                break;
                                            case "Instruction":
                                                 s += currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null).ToString();
                                               break;
                                            case "CodeLigne":
                                               s += currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null).ToString();
                                               break;
                                        }
                                        //    var x=  currentNode.GetType().GetProperty(v.Name);
                                        //    TreeNode tn = new TreeNode(x.Name, 0, 1);
                                        //    tn.Tag = x;
                                        //    root.Nodes.Add(tn);
                                        #endregion
                                        break;
                                }
                                if (s != "")
                                {
                                    TreeNode ts = new TreeNode(s, 0, 1);
                                    ts.Tag = currentNode.GetType().GetProperty(v.Name).GetValue(currentNode, null);
                                    root.Nodes.Add(ts);
                                }
                                else
                                {
                                }
                                #endregion
                            }
                    }
                }
                catch { }
            }
            return filled;
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode tn = Propriétés.SelectedNode;
            Type t = tn.Tag.GetType();
            if (tn.Tag != null)
            {
                try
                {
                    ILOCALIZED_DATA im = (ILOCALIZED_DATA)tn.Tag;
                    FillTreeNode(tn, im);
                    if (dataSelected != null)
                        dataSelected(this, new DataSelectedEventArgs(im.PositionOfStructureInFile, im.LengthInFile));
                    if (dataRequested != null)
                        dataRequested(this, new DataRequestArgs(tn, tn.Tag));
                }
                catch
                {
                    if ((tn.Text.Contains("Data Start Sector"))||(tn.Text.Contains("DataStartSector")))
                        if (dataSelected != null)
                            dataSelected(this, new DataSelectedEventArgs((long)tn.Tag * 0x200, 0x200, null));
                    if(tn.Text.Contains("StartCluster"))
                       if (dataSelected != null)
                           dataSelected(this, new DataSelectedEventArgs((long)tn.Tag * 0x200*8, 0x200, null));
                }
           //     tn.Expand();
            }

        }
    }
    public class DataRequestArgs
    {
        TreeNode sourceNode;
        object sourceObject;


        public TreeNode SourceNode
        {
            get { return sourceNode; }
            set { sourceNode = value; }
        }
        public object SourceObject
        {
            get { return sourceObject; }
            set { sourceObject = value; }
        }
        public DataRequestArgs(TreeNode n, object o)
        {
            sourceNode = n;
            sourceObject = o;
        }
    }
    public delegate void DataRequestEventHandler(object sender, DataRequestArgs e);

}
