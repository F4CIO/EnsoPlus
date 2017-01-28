using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using CraftSynth.BuildingBlocks.Common;

namespace EnsoPlus.WheelOfHistoryCompatibility
{
	/// <summary>
	/// Usage:
	///  
	/// using(WheelOfHistoryController.DisableClipsGrabbingInThisBlock()) 
	/// {
	///     //do something...no clip collection in WheelOfHistory will occur
	/// }                                                
	/// </summary>
	public class WheelOfHistoryController
	{
		private static readonly object _lock = new object();

		public static WheelOfHistoryPausedBlock DisableClipsGrabbingInThisBlock()
		{
			return new WheelOfHistoryPausedBlock();
		}

		public static void DisableClipsGrabbing()
		{
			lock (_lock)
			{
				if (Process.GetProcessesByName("WheelOfHistory").Any())
				{
					EventWaitHandle e = new EventWaitHandle(false, EventResetMode.ManualReset, "CraftSynth.WheelOfHistory.HowManyProcessesBlockClipboardObserver.Signal_Increase");
					e.Set();
					System.Windows.Forms.Application.DoEvents();
					//CraftSynth.BuildingBlocks.Common.Misc.BeepInNewThread();
				}
			}
		}

		public static void EnableClipsGrabbingAfter2Seconds()
		{
			Thread t = new Thread(delegate(object o)
					{
						Thread.Sleep(2000);
						EnableClipsGrabbing();
					}
				);
			t.Start();
		}

		public static void EnableClipsGrabbing()
		{
			lock (_lock)
			{
				if (Process.GetProcessesByName("WheelOfHistory").Any())
				{
					EventWaitHandle e = new EventWaitHandle(false, EventResetMode.ManualReset, "CraftSynth.WheelOfHistory.HowManyProcessesBlockClipboardObserver.Signal_Decrease");
					e.Set();
					System.Windows.Forms.Application.DoEvents();
					//CraftSynth.BuildingBlocks.Common.Misc.BeepInNewThread();
				}
			}
		}
	}

	public class WheelOfHistoryPausedBlock : IDisposable
	{
		public WheelOfHistoryPausedBlock()
		{
			WheelOfHistoryController.DisableClipsGrabbing();                                      //DISABLE
		}

		// Public implementation of Dispose pattern callable by consumers. 
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Flag: Has Dispose already been called? 
		bool _disposed = false;

		// Protected implementation of Dispose pattern. 
		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				WheelOfHistoryController.EnableClipsGrabbingAfter2Seconds();                      //ENABLE

				if (disposing)
				{
					// Free any managed objects here. 
					//
				}

				// Free any unmanaged objects here. 
				//


				this._disposed = true;
			}
		}

		~WheelOfHistoryPausedBlock()
		{
			this.Dispose(false);
		}
	}
}
