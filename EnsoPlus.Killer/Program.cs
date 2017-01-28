using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Common;
using System.Threading;

namespace EnsoKiller
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            KillEnsoPlus();
        }

        private static void KillEnsoPlus()
        {
            try
            { 
				Process ensoPlusGuardProcess = Helper.TryGetEnsoPlusGuardProcess();
                if (ensoPlusGuardProcess != null)
                {
                    Helper.KillEnsoPlusGuardIfExist();
                }

                Process ensoPlusProcess = Helper.TryGetEnsoPlusProcess();
                if (ensoPlusProcess != null)
                {
                    Helper.KillEnsoPlus(ensoPlusProcess);
                }
            }
            catch { }
        }
    }
}
