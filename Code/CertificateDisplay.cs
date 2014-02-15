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
    public partial class CertificateDisplay : UserControl
    {
        ASN1_OBJECT Certificate;
        public CertificateDisplay()
        {
            InitializeComponent();
        }
        public void Init(ASN1_OBJECT certificate)
        {
            Certificate = certificate;
            certifView.Nodes.Clear();
            foreach (ASN1_OBJECT cd in certificate.Nodes)
            {
                if (cd.Length != 0)
                {
                    certifView.Nodes.Add(DisplayNode(cd));
                }
            }
        }
        private TreeNode DisplayNodeOld(ASN1_OBJECT asnObject)
        {
            if (asnObject.AsnTag.IsConstructed)
            {
                // Shortens branches
                while ((asnObject.Nodes!=null)&&(asnObject.Nodes.Count == 1))
                    asnObject = asnObject.Nodes[0];
            }
            if (asnObject.Offset >= 0x6396e)
            {
            }
            TreeNode tn = null;
            switch (asnObject.AsnTag.TagType)
            {
                case ASN_TAG_TYPE.NULL_ID:
                    return null;
                case ASN_TAG_TYPE.BIT_STRING:
                case ASN_TAG_TYPE.BOOLEAN:
                case ASN_TAG_TYPE.INTEGER:
                    if (asnObject.Length == 1)
                        return null;
                    else
                        tn = new TreeNode(asnObject.ToString());
                    break;

                case ASN_TAG_TYPE.UTC_TIME:
                    tn = new TreeNode(asnObject.DataString);
                    break;
                default:
                    tn = new TreeNode(asnObject.ToString());
                    break;
            }
            if (tn == null)
                return tn;
            if (asnObject.Nodes != null)
            {
                if (asnObject.Nodes.Count == 2)
                {
                    if ((asnObject.Nodes[0].AsnTag.TagType == ASN_TAG_TYPE.OBJECT_ID)/* && ((ASN_TAG_TYPE)asnObject.Nodes[1].AsnTag.TagType == ASN_TAG_TYPE.PRINTABLE_STRING)*/)
                    {
                        TreeNode tt = null;
                        switch ((ASN_TAG_TYPE)asnObject.Nodes[1].AsnTag.TagType)
                        {
                            case ASN_TAG_TYPE.OCTET_STRING:
                                if (asnObject.Nodes[1].Nodes.Count == 0)
                                    tt = new TreeNode(asnObject.Nodes[0].DataString + ": " + asnObject.Nodes[1].DataString);
                                else
                                    tt = DisplayNode(asnObject.Nodes[1]);
                                break;
                            case ASN_TAG_TYPE.PRINTABLE_STRING:
                            default:
                                string text = (asnObject.Nodes[0]).DataString;
                                if ((asnObject.Nodes[1]).AsnTag.TagType != ASN_TAG_TYPE.NULL_ID)
                                    text += ": " + (asnObject.Nodes[1]).DataString;
                                tt = new TreeNode(text);
                                break;
                        }
                        return tt;
                    }
                }
                foreach (ASN1_OBJECT cd in asnObject.Nodes)
                {
                    switch (cd.AsnTag.TagType)
                    {
                        case ASN_TAG_TYPE.OBJECT_ID:
                            tn.Nodes.Add(/*callingLine.Object_type + " " + */cd.DataString);
                            break;
                        case ASN_TAG_TYPE.SEQUENCE:
                        case ASN_TAG_TYPE.SET:
                            asnObject = cd;
                            tn.Nodes.Add(DisplayNode(asnObject));
                            break;
                        case ASN_TAG_TYPE.INTEGER:
                            tn.Nodes.Add((cd).DataString);
                            break;
                        default:
                            try
                            {
                                TreeNode tt = DisplayNode(cd);
                                if (tt != null)
                                    tn.Nodes.Add(tt);
                            }
                            catch { }
                            break;
                    }
                }
            }
            return tn;
        }
        private TreeNode DisplayNode(ASN1_OBJECT asnObject)
        {
            if (asnObject.AsnTag.IsConstructed)
            {
                // Shortens branches
                while ((asnObject.Nodes != null) && (asnObject.Nodes.Count == 1))
                    asnObject = asnObject.Nodes[0];
            }
            if ((asnObject.AsnTag.TagType == ASN_TAG_TYPE.BIT_STRING)||(asnObject.AsnTag.TagType == ASN_TAG_TYPE.OCTET_STRING))
            {
                if ((asnObject.Nodes != null) && (asnObject.Nodes.Count == 1))
                    asnObject = asnObject.Nodes[0];
            }
            TreeNode tn = new TreeNode(asnObject.ToString());
            if (asnObject.Offset == 0x639df)
            {
            }
            if (asnObject.HasSons)
            {
                if (asnObject.Nodes.Count == 2)
                {
                    #region Case of two sons
                    if ((asnObject.Nodes[0].AsnTag.TagType == ASN_TAG_TYPE.OBJECT_ID)/* && ((ASN_TAG_TYPE)asnObject.Nodes[1].AsnTag.TagType == ASN_TAG_TYPE.PRINTABLE_STRING)*/)
                    {
                        TreeNode tt = null;
                        ASN1_OBJECT secondNode = asnObject.Nodes[1];
                        switch (secondNode.AsnTag.TagType)
                        {
                            case ASN_TAG_TYPE.OCTET_STRING:
                                if (!asnObject.Nodes[1].HasSons)
                                    tt = new TreeNode(asnObject.Nodes[0].DataString + ": " + secondNode.DataString);
                                else
                                {
                                    tt = new TreeNode(asnObject.Nodes[0].DataString);
                                    tt.Nodes.Add(DisplayNode(secondNode));
                                }
                                break;
                            case ASN_TAG_TYPE.PRINTABLE_STRING:
                                string txt = (asnObject.Nodes[0]).DataString;
                                txt += ": " + (asnObject.Nodes[1]).DataString;
                                tt = new TreeNode(txt);
                                break;
                            default:
                                string text = (asnObject.Nodes[0]).DataString;
                                tt = new TreeNode(text);
                                if (asnObject.Nodes[1].AsnTag.IsConstructed)
                                {
                                    tt.Nodes.Add(DisplayNode(asnObject.Nodes[1]));
                                }
                                else
                                {
                                    if ((asnObject.Nodes[1]).AsnTag.TagType != ASN_TAG_TYPE.NULL_ID)
                                        tt.Text += ": " + (asnObject.Nodes[1]).DataString;
                                }
                                break;
                        }
                        return tt;
                    }
                    #endregion
                }
                #region generic case
                foreach (ASN1_OBJECT cd in asnObject.Nodes)
                {
                    if (cd.AsnTag.IsConstructed)
                    {
                        try
                        {
                            TreeNode t = DisplayNode(cd);
                            if (t != null)
                                tn.Nodes.Add(t);
                        }
                        catch { }
                    }
                    else
                    {
                        switch (cd.AsnTag.TagType)
                        {
                            case ASN_TAG_TYPE.OBJECT_ID:
                                tn.Nodes.Add(cd.DataString);
                                break;
                            case ASN_TAG_TYPE.SEQUENCE:
                            case ASN_TAG_TYPE.SET:
                                try
                                {
                                    TreeNode t = DisplayNode(cd);
                                    if (t != null)
                                        tn.Nodes.Add(t);
                                }
                                catch { }
                                break;
                            case ASN_TAG_TYPE.BOOLEAN:
                            case ASN_TAG_TYPE.INTEGER:
                                tn.Nodes.Add(cd.DataString);
                                break;
                            default:
                                try
                                {
                                    TreeNode tt = DisplayNode(cd);
                                    if (tt != null)
                                        tn.Nodes.Add(tt);
                                }
                                catch { }
                                break;
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region no sons
                switch (asnObject.AsnTag.TagType)
                {
                    case ASN_TAG_TYPE.NULL_ID:
                        return null;
                    case ASN_TAG_TYPE.BIT_STRING:
                    case ASN_TAG_TYPE.BOOLEAN:
                    case ASN_TAG_TYPE.INTEGER:
                        if (asnObject.Length == 1)
                            return null;
                        else
                            tn = new TreeNode(asnObject.ToString());
                        break;
                    case ASN_TAG_TYPE.UTC_TIME:
                        tn = new TreeNode(asnObject.DataString);
                        break;
                    default:
                        tn = new TreeNode(asnObject.ToString());
                        break;
                }
                #endregion
            }
            return tn;
        }
    }
}
