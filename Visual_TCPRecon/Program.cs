using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace Visual_TCPRecon
{
    static class Program
    {

        static void RegisterOcx(string ocxPath)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = "regsvr32.exe";
            p.StartInfo.Arguments = ocxPath;
            p.Start();
            p.WaitForExit();
        }

        public static string FindExternal(string baseName)
        {
            string f = Application.StartupPath;

            for (int i = 0; i < 6; i++)
            {
                if (!File.Exists(f + "\\" + baseName)) f = Path.GetDirectoryName(f);
            }

            if (!File.Exists(f + "\\" + baseName))
            {
                MessageBox.Show("Could not locate " + baseName + " in: " + f);
                return "";
            }

            f += "\\" + baseName;
            return f;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var clsIdKey = Registry.ClassesRoot.OpenSubKey("rhexed.HexEd");
            if (clsIdKey == null) 
            {
                string ocx = FindExternal("hexed.ocx");
                if (ocx.Length != 0) RegisterOcx(ocx);
            }
             


            Application.Run(new Form1());
        }
    }
}
