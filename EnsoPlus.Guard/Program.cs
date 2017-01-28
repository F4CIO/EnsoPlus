using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace EnsoPlus.Guard
{
	class Program
	{
		#region Private Members

		private const string _processName = "EnsoPlus";
		private const string _fileName = "EnsoPlus.exe";
		private const int _intervalInSeconds = 5;
		private static Timer _timer;
		#endregion

		#region Properties
		#endregion

		#region Public Methods
		#endregion

		#region Constructors And Initialization
		static void Main(string[] args)
		{
			if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Count() >= 2)
			{
				// MessageBox.Show("Enso+ is already running.");
			}
			else
			{
				_timer = new Timer(Callback, null, new TimeSpan(0,0,_intervalInSeconds),new TimeSpan(0,0,_intervalInSeconds));
				Application.Run();
			}
			
		}

		private static void Callback(object state)
		{
			if (!Process.GetProcessesByName(_processName).Any())
			{
				string ensoPlusExeFilePath = Path.Combine(CraftSynth.BuildingBlocks.Common.Misc.ApplicationRootFolderPath, _fileName);
				CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenFile(ensoPlusExeFilePath);
			}
		}

		#endregion

		#region Deinitialization And Destructors
		#endregion

		#region Event Handlers
		#endregion

		#region Private Methods
		#endregion

		#region Helpers
		#endregion
	}
}
