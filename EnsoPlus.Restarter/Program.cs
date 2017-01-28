using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Common;
using System.Threading;

namespace EnsoRestarter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            RestartEnsoPlus();
            //Application.Run(new Form1());
        }

        private static void RestartEnsoPlus()
        {
            try
            {
			    Process ensoPlusProcess = Helper.TryGetEnsoPlusProcess();
                if (ensoPlusProcess != null)
                {                   
                    Helper.KillEnsoPlus(ensoPlusProcess);	
					string ensoPlusGuardExeFilePath = Path.Combine(CraftSynth.BuildingBlocks.Common.Misc.ApplicationRootFolderPath, "EnsoPlus.Guard.exe");
					CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenFile(ensoPlusGuardExeFilePath);
                }  
            }
            catch { }            
        }
    }
}
