using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace BinHed
{
    static class Program
    {
        /// <summary>
        /// Point directionBit'entrée principal de object_length'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Editor());
        }
    }
}