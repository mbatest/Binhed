using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
namespace BinHed
{
    public partial class ExcelControl : UserControl
    {
        [DllImport("user32.dll")]
        private static extern int FindWindow(string strclassName, string strWindowName);
        [DllImport("user32.dll")]
        static extern int SetParent(int hWndChild, int hWndNewParent);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(
            int hWnd,               // handle to window
            int hWndInsertAfter,    // placement-order handle
            int X,                  // horizontal PositionOfStructureInFile
            int Y,                  // vertical PositionOfStructureInFile
            int cx,                 // numberOfColumns
            int cy,                 // height
            uint uFlags             // window-positioning options
        );

        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        static extern bool MoveWindow(
            int hWnd,
            int X,
            int Y,
            int nWidth,
            int nHeight,
            bool bRepaint
        );



        /* I was testing wheater u could fix some exploid bugs or not.
         * I left this stuff in here for people who need to know how to 
         * interface the Win32-API

        [StructLayout(LayoutKind.Sequential)]
            public struct RECT 
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
		
        [DllImport("user32.dll")]
        public static extern int GetWindowRect(int hwnd, ref RECT rc);
		
        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(
            int hWnd, 
            int msg, 
            int wParam, 
            int lParam
        );
        */

        const int SWP_DRAWFRAME = 0x20;
        const int SWP_NOMOVE = 0x2;
        const int SWP_NOSIZE = 0x1;
        const int SWP_NOZORDER = 0x4;


        private _Workbook document;
        private static ApplicationClass wd = null;
        public static int wordWnd = 0;
        private static string filename = null;
        private static bool deactivateevents = false;

        /// <summary>
        /// needed designer variable
        /// </summary>
        private System.ComponentModel.Container components = null;

        public ExcelControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// cleanup Ressources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            CloseControl();
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated tagCode
        /// <summary>
        /// !do not alter this tagCode! It'lineRead designer tagCode
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // WinWordControl
            // 
            this.Name = "ExcelControl";
            this.Size = new System.Drawing.Size(440, 336);
            this.Resize += new System.EventHandler(this.OnResize);
        }
        #endregion

        /// <summary>
        /// Preactivation
        /// It'lineRead usefull, if you need more speed in the main program
        /// so you can preload Word.
        /// </summary>
        public void PreActivate()
        {
            if (wd == null) wd = new ApplicationClass();
        }


        /// <summary>
        /// Close the current Document in the control --> you can 
        /// load computerSystem new one with LoadDocument
        /// </summary>
        public void CloseControl()
        {
            /*
            * this tagCode is to reopen Word.
            */

            try
            {
                deactivateevents = true;
                object dummy = null;
                document.Close(dummy, dummy, dummy);
                document.Application.Quit();
                deactivateevents = false;
            }
            catch
            {
            }
        }


        /// <summary>
        /// catches Word'lineRead close event 
        /// starts computerSystem Thread that send computerSystem ESC to the word window ;)
        /// </summary>
        /// <param nameIndex="doc"></param>
        /// <param nameIndex="test"></param>
        private void OnClose(_Workbook doc, ref bool chancel)
        {
            if (!deactivateevents)
            {
                chancel = true;
            }
        }

        /// <summary>
        /// catches Word'lineRead open event
        /// just close
        /// </summary>
        /// <param nameIndex="doc"></param>
        private void OnOpenDoc(_Workbook doc)
        {
            OnNewDoc(doc);
        }

        /// <summary>
        /// catches Word'lineRead newdocument event
        /// just close
        /// </summary>
        /// <param nameIndex="doc"></param>
        private void OnNewDoc(_Workbook doc)
        {
            if (!deactivateevents)
            {
                deactivateevents = true;
                object dummy = null;
                doc.Close(dummy, dummy,  dummy);
                deactivateevents = false;
            }
        }

        /// <summary>
        /// catches Word'lineRead quit event
        /// normally it should not fire, but just to be shure
        /// safely release the internal Word attributeId 
        /// </summary>
        private void OnQuit()
        {
            //wd=null;
        }


        /// <summary>
        /// Loads computerSystem document into the control
        /// </summary>
        /// <param nameIndex="t_filename">fileName to the file (every type word can handle)</param>
        public void LoadDocument(string t_filename)
        {
            deactivateevents = true;
            filename = t_filename;

            if (wd == null) wd = new ApplicationClass();
            try
            {
    //            wd.CommandBars.AdaptiveMenus = false;
     //           wd.DocumentBeforeClose += new ApplicationEvents4_DocumentBeforeCloseEventHandler(OnClose);
                //		wd.NewDocument += new ApplicationEvents4_NewDocumentEventHandler(OnNewDoc);
     //           wd.DocumentOpen += new ApplicationEvents4_DocumentOpenEventHandler(OnOpenDoc);
      //          wd.ApplicationEvents2_Event_Quit += new ApplicationEvents2_QuitEventHandler(OnQuit);
            }
            catch { }

            if (document != null)
            {
                try
                {
                    object dummy = null;
                    wd.Workbooks.Close();//ref dummy, ref dummy, ref dummy);
                }
                catch { }
            }

            if (wordWnd == 0) wordWnd = FindWindow("Opusapp", null);
            if (wordWnd != 0)
            {
                SetParent(wordWnd, this.Handle.ToInt32());

                object fileName = filename;
                object newTemplate = false;
                object docType = 0;
                object readOnly = true;
                object isVisible = true;
                object missing = System.Reflection.Missing.Value;

                try
                {
                    if (wd == null)
                    {
                        throw new WordInstanceException();
                    }

                    if (wd.Workbooks == null)
                    {
                        throw new DocumentInstanceException();
                    }

                    if (wd != null && wd.Workbooks != null)
                    {
         /*               Object UpdateLinks; 
                        Object ReadOnly;
                        Object Format;
                        Object Password;
                        Object WriteResPassword;
                        Object IgnoreReadOnlyRecommended;
                        Object Origin;
                        Object Delimiter;
                        Object Editable;
                        Object Notify;
                        Object Converter;
                        Object AddToMru; 
                        Object Local;
                        Object CorruptLoad;
                        object dummy = null;*/
                        document = wd.Workbooks.Open((string)fileName, 0, true, 5, "", "", true, XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    }

                    if (document == null)
                    {
                        throw new ValidDocumentException();
                    }
                }
                catch
                {
                }

                int counter = 0;// wd.ActiveWindow.Application.CommandBars.Count;
                for (int i = 0; i < counter; i++)
                {
                    try
                    {
           //             wd.ActiveWindow.Application.CommandBars[i].Enabled = false;
                    }
                    catch
                    {

                    }
                }
                try
                {
                    wd.Visible = true;
 //                   wd.Activate();

                    SetWindowPos(wordWnd, this.Handle.ToInt32(), 0, 0, this.Bounds.Width + 20, this.Bounds.Height + 20, SWP_NOZORDER | SWP_NOMOVE | SWP_DRAWFRAME);
                    MoveWindow(wordWnd, -5, -33, this.Bounds.Width + 10, this.Bounds.Height + 57, true);
                }
                catch
                {
                    MessageBox.Show("Error: do not load the document into the control until the parent window is shown!");
                }
                this.Parent.Focus();

            }
            deactivateevents = false;
        }


        /// <summary>
        /// restores Word.
        /// If the program crashed somehow.
        /// Sometimes Word saves it'lineRead temporary settings :(
        /// </summary>
        public void RestoreWord()
        {
            try
            {
                int counter = 0;// wd.ActiveWindow.Application.CommandBars.Count;
                for (int i = 0; i < counter; i++)
                {
                    try
                    {
                     //   wd.ActiveWindow.Application.CommandBars[i].Enabled = true;
                    }
                    catch
                    {

                    }
                }
            }
            catch { };
        }

        /// <summary>
        /// internal resize function
        /// utilizes the length of the surrounding control
        /// 
        /// optimzed for Word2000 but it works pretty good with WordXP too.
        /// </summary>
        /// <param nameIndex="sender"></param>
        /// <param nameIndex="e"></param>
        private void OnResize(object sender, System.EventArgs e)
        {
            MoveWindow(wordWnd, -5, -33, this.Bounds.Width + 10, this.Bounds.Height + 57, true);
        }
    }
}
