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
    public partial class ClrViewer : UserControl
    {
        IMAGE_COR20_HEADER clr;
        public ClrViewer()
        {
            InitializeComponent();
        }
        public void Init(Executable exe)
        {
            clr = exe.DataDirs[14].clrHeader;
            TreeNode tn = new TreeNode("Entry Point " + clr.EntryPointToken.ToString("x8"));
            Clr.Nodes.Add(tn);
            tn = new TreeNode("Flags " + clr.FlagData);
            Clr.Nodes.Add(tn);
            tn = new TreeNode("Run time version : " + clr.MajorRuntimeVersion.ToString() + "." + clr.MinorRuntimeVersion.ToString());
            Clr.Nodes.Add(tn);
            tn = new TreeNode("Metadata");
            Clr.Nodes.Add(tn);
            tn.Nodes.Add("CLI Version : " + clr.Metadata.VersionString);
            TreeNode ttn = new TreeNode("Guids");
            foreach (Guid g in clr.Metadata.Guids)
                ttn.Nodes.Add(g.ToString());
            tn.Nodes.Add(ttn);
            ttn = new TreeNode("Types");
            tn.Nodes.Add(ttn);
            for (int i = 1; i < clr.Metadata.TypeDefStruct.Length; i++)
            {
                TreeNode tt = new TreeNode(clr.Metadata.TypeDefStruct[i].ToString());
                tt.Tag = clr.Metadata.TypeDefStruct[i];
                ttn.Nodes.Add(tt);
                tt.Nodes.Add(clr.Metadata.TypeDefStruct[i].NameSpace);
                tt.Nodes.Add(clr.Metadata.TypeDefStruct[i].parentClass);
            }
            ttn = new TreeNode("User strings");
            tn.Nodes.Add(ttn);
            ttn = new TreeNode("Constants");
            tn.Nodes.Add(ttn);
        }
        private void Clr_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode t = Clr.SelectedNode;
            if (t.Tag != null) 
                if (t.Tag.GetType() == typeof(TypeDefTableRow))
                {
                    #region Classes
                    TypeDefTableRow trow = (TypeDefTableRow)t.Tag;
                    t.Nodes.Clear();
                    t.Nodes.Add(trow.NameSpace);
                    t.Nodes.Add(trow.parentClass);
                    if (trow.methods != null)
                    {
                        TreeNode tMeth = new TreeNode("Methods");
                        t.Nodes.Add(tMeth);
                        for (int i = 0; i < trow.methods.Count; i++)
                        {
                            MethodTableRow mt = trow.methods[i];
                            TreeNode tt = new TreeNode(mt.Name);
                            tMeth.Nodes.Add(tt);
                            if (mt.meths != null)
                                tt.Nodes.Add(mt.meths);
                        }
                    }
                    if (trow.fields != null)
                    {
                        TreeNode tFields = new TreeNode("Fields");
                        t.Nodes.Add(tFields);
                        for (int i = 0; i < trow.fields.Count; i++)
                        {
                            FieldTableRow mt = trow.fields[i];
                            TreeNode tt = new TreeNode(mt.Name);
                            tFields.Nodes.Add(tt);
                            tt.Nodes.Add(mt.FieldSignature);
                        }
                    }
                    if (trow.properties != null)
                    {
                        TreeNode tProperties = new TreeNode("Properties");
                        t.Nodes.Add(tProperties);
                        for (int i = 0; i < trow.properties.Count; i++)
                        {
                            PropertyTableRow mt = trow.properties[i];
                            TreeNode tt = new TreeNode(mt.Name);
                            tProperties.Nodes.Add(tt);
                        }
                    }
                    if (trow.events != null)
                    {
                        TreeNode tEvents = new TreeNode("Events");
                        t.Nodes.Add(tEvents);
                        for (int i = 0; i < trow.events.Count; i++)
                        {
                            EventTableRow mt = trow.events[i];
                            TreeNode tt = new TreeNode(mt.Name);
                            tEvents.Nodes.Add(tt);
                            tt.Nodes.Add(mt.tablename);
                            tt.Nodes.Add(mt.c);

                        }
                    }
                    #endregion
                }
            if (t.Text == "User strings")
            {
                if (t.Nodes.Count == 0)
                    foreach (string s in clr.Metadata.UserStrings)
                    {
                        t.Nodes.Add(s);
                    }
            }
            if (t.Text == "Constants")
            {
                if (t.Nodes.Count == 0)
                    for(int i = 1; i<clr.Metadata.ConstantsStruct.Length;i++)
                    {
                        TreeNode tn = new TreeNode(clr.Metadata.ConstantsStruct[i].Name);
                        t.Nodes.Add(tn);
                        tn.Nodes.Add(clr.Metadata.ConstantsStruct[i].type);
                    }
            }
            t.Expand();
        }
    }
}
