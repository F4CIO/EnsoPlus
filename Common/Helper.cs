using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using CraftSynth.BuildingBlocks.WindowsNT;
using Microsoft.Win32;
using System.Deployment.Application;
using System.Drawing;

namespace Common
{
    public class Helper
    {
        #region Log files operations

        public static void OpenActionsLog()
        {
             Misc.OpenFile(Settings.Current.ActionsLogFilePath);
        }

        public static void ReopenErrorsLog()
        {
            if ( Misc.IsWindowOpen(Path.GetFileName(Settings.Current.ErrorsLogFilePath) + " - Notepad"))
            {
                 Misc.CloseWindow(Path.GetFileName(Settings.Current.ErrorsLogFilePath) + " - Notepad");
            }

             Misc.OpenFile(Settings.Current.ErrorsLogFilePath);
        }

        public static void ReopenActionsLog()
        {
            if ( Misc.IsWindowOpen(Path.GetFileName(Settings.Current.ActionsLogFilePath) + " - Notepad"))
            {
                 Misc.CloseWindow(Path.GetFileName(Settings.Current.ActionsLogFilePath) + " - Notepad");
            }

             Misc.OpenFile(Settings.Current.ActionsLogFilePath);
        }

        public static void ReopenExceptionsLog()
        {
            if ( Misc.IsWindowOpen(Path.GetFileName(Settings.Current.ExceptionsLogFilePath) + " - Notepad"))
            {
                 Misc.CloseWindow(Path.GetFileName(Settings.Current.ExceptionsLogFilePath) + " - Notepad");
            }

             Misc.OpenFile(Settings.Current.ExceptionsLogFilePath);
        }

        public static void OpenExceptionsLog()
        {
             Misc.OpenFile(Settings.Current.ExceptionsLogFilePath);
        }

        public static void PreDeleteLogs()
        {
            try
            {
                if (Settings.Current.DeleteActionsLogOnStart) File.Delete(Settings.Current.ActionsLogFilePath);
            }
            catch (Exception exception)
            { }

            try
            {
                if (Settings.Current.DeleteErrorsLogOnStart) File.Delete(Settings.Current.ErrorsLogFilePath);
            }
            catch (Exception exception)
            { }

            try
            {
                if (Settings.Current.DeleteExceptionsLogOnStart) File.Delete(Settings.Current.ExceptionsLogFilePath);
            }
            catch (Exception exception)
            { }
        }
        #endregion Log file operations

        

	    public static string GetEnsoPlusVersion()
	    {			 
			string vfp = Path.Combine(GetEnsoPlusWorkingFolder(), "Version.txt");
			string r = File.ReadAllText(vfp);
			return r;
	    }

        public static string GetEnsoPlusWorkingFolder()
        {
            return CraftSynth.BuildingBlocks.UI.WindowsForms.Misc.ApplicationPhysicalPath;
        }

		#region Enso+ process handling
        public static Process TryGetEnsoPlusProcess()
        {
            Process ensoPlusProcess = null;

            Process[] ps = Process.GetProcessesByName("EnsoPlus");
            if (ps.Length > 0)
            {
                ensoPlusProcess = ps[0];
            }

            return ensoPlusProcess;
        }

        public static void KillEnsoPlus(Process ensoPlusProcess)
        {
            ensoPlusProcess.Kill();
            Thread.Sleep(3000);
        }    
		
		public static void KillEnsoPlusGuardIfExist()
		{
			var p =TryGetEnsoPlusGuardProcess();
			if (p != null)
			{
				p.Kill();
				Thread.Sleep(3000);
			}
		}

	    public static Process TryGetEnsoPlusGuardProcess()
        {
            Process ensoPlusProcess = null;

            Process[] ps = Process.GetProcessesByName("EnsoPlus.Guard");
            if (ps.Length > 0)
            {
                ensoPlusProcess = ps[0];
            }

            return ensoPlusProcess;
        }

        #endregion Enso+ processes handling

        #region Other
        public static string GetEnsoTempraryFilePath(string extensionWithoutLeadingPoint)
        {
            return Path.Combine(Path.GetTempPath(),"EnsoPlus_"+ Guid.NewGuid().ToString())+"."+extensionWithoutLeadingPoint;
        }

	    public static string GetCurrentWindowsUserName()
	    {
		    string r = Environment.UserName;
		    if (r.Contains('\\'))
		    {
			    r = r.Remove(0, r.IndexOf('\\'));
		    }
		    return r;
	    }

	    #endregion
    }
}
