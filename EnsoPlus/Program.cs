using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Common;
using ControlCornerLauncher;
using Extension;

namespace EnsoPlus
{
	public class Program
	{
		public static bool IsFirstRun;
		public static CornerLauncher cornerLauncher;
		public static EnsoPlus extensionPlus;
		/// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        //[STAThreadAttribute]
		public static void Main()
        {
            Logging.AddActionLog("Starting Enso+ ...");           

            //IsFirstRun = !File.Exists(Settings.FilePath);

            //Not working
            //Application.ThreadException += new ThreadExceptionEventHandler(BuildingBlocks.Exceptions.EnterpriseLibrary.ExceptionHandler.Application_ThreadException);
			Application.ThreadException += Application_ThreadException;

		  try
		  {
			  if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Count() >= 2)
			  {
				  MessageBox.Show("Enso+ is already running.");
			  }
			  else
			  {
				  //Logging.AddActionLog("Starting EnsoPlus.Guard in not running already ...");
				  string ensoPlusGuardExeFilePath = Path.Combine(CraftSynth.BuildingBlocks.Common.Misc.ApplicationRootFolderPath, "EnsoPlus.Guard.exe");
				  CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenFile(ensoPlusGuardExeFilePath);

				  Helper.PreDeleteLogs();

				  if (Settings.Current.firstRun ||
				      !File.Exists(Path.Combine(Common.Helper.GetEnsoPlusWorkingFolder(), "Version.txt")))
				  {
					  Common.HandlerForPreviousVersions.OfferImportingOfSettingsFromPreviousVersion();
				  }

				  if (Settings.Current.firstRun)
				  {
					  Settings.Current.firstRun = false;
					  Settings.Current.Save();
				  }

				  extensionPlus = new EnsoPlus();
				  var mergedCommands = extensionPlus.GetMergedCommands();
				  cornerLauncher = new CornerLauncher(mergedCommands, OnClose);
				  // extensionPlus.OnCommand( mergedCommands[mergedCommands.Keys.Single(k=>k=="open")].sourceCommands[0], string.Empty);
				  ParameterInput.Init();

				  Logging.AddActionLog(string.Format("Started Enso+ {0}",CraftSynth.BuildingBlocks.Common.Misc.version ?? string.Empty));
				  MessagesHandler.Display("Welcome to Enso+");

				  var f = new FormMain();
				  f.Visible = false;
				  f.Width = 0;
				  f.Height = 0;
				  Application.Run(f);
				  
			  }
		  }
		  catch (Exception exception)
		  {
			  Logging.AddErrorLog("Server: Starting failed: " + exception.Message);
			  Common.Logging.AddExceptionLog(exception);
		  }

        }
		
		private static void OnClose(EnsoCommand c, string postfix, IntPtr fw)
		{
			Program.extensionPlus.OnCommand(c, postfix, fw);
		}

		static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			Common.Logging.AddExceptionLog(e.Exception);
		}


		//private void InitializeComponent()
		//{
		//	ToolStripMenuItem restartToolStripMenuItem = new ToolStripMenuItem();
		//	restartToolStripMenuItem.Text = Resource1.ResourceManager.GetString("restartToolStripMenuItem_Text");
		//	restartToolStripMenuItem.Click += new EventHandler(restartToolStripMenuItem_Click);

		//	ToolStripMenuItem closeToolStripMenuItem = new ToolStripMenuItem();
		//	closeToolStripMenuItem.Text = Resource1.ResourceManager.GetString("closeToolStripMenuItem_Text");
		//	closeToolStripMenuItem.Click += new EventHandler(closeToolStripMenuItem_Click);

		//	ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
		//	contextMenuStrip.Items.AddRange(new ToolStripItem[] { restartToolStripMenuItem, closeToolStripMenuItem });

		//	notifyIcon = new NotifyIcon();
		//	notifyIcon.ContextMenuStrip = contextMenuStrip;
		//	notifyIcon.Icon = (Icon)Resource1.ResourceManager.GetObject("notifyIcon_Icon");
		//	notifyIcon.Text = Resource1.ResourceManager.GetString("notifyIcon_Text");
		//	notifyIcon.Visible = true;
		//}

		//private void restartToolStripMenuItem_Click(object sender, EventArgs e)
		//{
		//	try
		//	{
		//		CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenFile(Path.Combine(CraftSynth.BuildingBlocks.UI.Console.ApplicationPhysicalPath, "EnsoRestarter.exe"));
		//	}
		//	catch (Exception exception)
		//	{
		//		Logging.AddErrorLog("Server: Can't restart: " + exception.Message);
		//		Common.Logging.AddExceptionLog(exception);
		//	}
		//}

		//private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		//{
		//	notifyIcon.Visible = false;
		//	Application.Exit();
		//}
	}
}
