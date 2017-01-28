using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CraftSynth.BuildingBlocks.Logging;
using CraftSynth.Win32Clipboard;
using EnsoPlus;
using EnsoPlus.WheelOfHistoryCompatibility;

namespace Common
{
	public class HandlerForSelection
	{
		public static ClipboardData Get(IntPtr foregroundWindow)
		{
			ClipboardData dataFromScreen;

			using (WheelOfHistoryController.DisableClipsGrabbingInThisBlock())
			{ 
				CustomTraceLog log = new CustomTraceLog("Grabb...", false, false, CustomTraceLogAddLinePostProcessingEvent);

				var originalClipboardData = CraftSynth.Win32Clipboard.ClipboardProxy.GrabClipboardDataStripped(false);

				Application.DoEvents();
				CraftSynth.Win32Clipboard.ClipboardProxy.ClearClipboard(log);
				Application.DoEvents();

				if (IntPtr.Zero != foregroundWindow)
				{
					CraftSynth.BuildingBlocks.WindowsNT.Misc.SetForegroundWindow(foregroundWindow);
					log.AddLine("foregr ok" + foregroundWindow.ToString());
				}
				else
				{
					log.AddLine("forgr empty");
				}

				try
				{
					SendKeys.SendWait("^c");
				}
				catch (Exception ex)
				{
					Logging.AddExceptionLog(ex);
					SendKeys.SendWait("^c");
				}
				Application.DoEvents();

				dataFromScreen = CraftSynth.Win32Clipboard.ClipboardProxy.GrabClipboardData(false, log);
				Application.DoEvents();
				Logging.AddActionLog("grab:" + dataFromScreen == null ? "null" : dataFromScreen.Formats.Count.ToString());
				//if (dataFromScreen != null && dataFromScreen.Formats.Count == 0)
				//{
				//	Thread.Sleep(500);
				//	 dataFromScreen = CraftSynth.Win32Clipboard.ClipboardProxy.GrabClipboardData(false, log);
				//	Logging.AddActionLog("grab2:"+dataFromScreen==null?"null":dataFromScreen.Formats.Count.ToString());
				//}
				CraftSynth.Win32Clipboard.ClipboardProxy.RestoreClipboardData(originalClipboardData, false);

				Application.DoEvents();
			}

			return dataFromScreen;
		}

		private static void CustomTraceLogAddLinePostProcessingEvent(CustomTraceLog sender, string line, bool inNewLine, int level, string lineVersionSuitableForLineEnding, string lineVersionSuitableForNewLine)
		{
			Logging.AddActionLog(line);
		}

		public static void Put(string text)
		{
			using (WheelOfHistoryController.DisableClipsGrabbingInThisBlock())
			{ 
				var clipboardData = CraftSynth.Win32Clipboard.ClipboardProxy.GrabClipboardDataStripped(false);
				Application.DoEvents();

				Clipboard.SetText(text);
				Application.DoEvents();

			
				//SendKeys.SendWait("^v");
				//Application.DoEvents();
				try
				{
					KeyboardSimulator.SimulateCtrlV();
					Thread.Sleep(100);
				}
				catch (Exception e)
				{
					try
					{
						CraftSynth.BuildingBlocks.WindowsNT.Misc.ThrowApplicationExceptionIfErrorOccurred();
					}
					catch (Exception innerE)
					{
						e = new Exception(e.Message, innerE);
					}
					Logging.AddExceptionLog(e);
					if (Debugger.IsAttached)
					{
						CraftSynth.BuildingBlocks.Common.Misc.BeepInNewThread();
					}
				}


				CraftSynth.Win32Clipboard.ClipboardProxy.RestoreClipboardData(clipboardData, false);
				Application.DoEvents();
			}
		}

		//public static void Paste(ClipboardHistoryItem clipboardHistoryItem)
		//{
		//	DisplayHistoryDelayer.DisplayHistoryDelayer_Callback -= DisplayHistory;
		//	DisplayHistoryDelayer.Abort();
		//	if (!clipboardHistoryItem.Content.IsEmpty && !clipboardHistoryItem.Content.IsJustTextOrUnicodeThatIsEmptyOrNull)
		//	{
		//		ClipboardObserver.Pause();

		//		contentNotToCollect = clipboardHistoryItem.Content;
		//		HandlerForClipboard.Set(clipboardHistoryItem.Content);
		//		if (Program.lastForegroundWindow != null)
		//		{
		//			BuildingBlocks.WindowsNT.Misc.SetForegroundWindowForced(Program.lastForegroundWindow);
		//			Application.DoEvents();
		//		}

		//		//Program.ClipboardObserver = null;
		//		//GC.Collect();

		//		//WindowsInput.InputSimulator inputSimulator = new InputSimulator();
		//		//inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);

		//		SendKeys.SendWait("^v");

		//		//Thread.Sleep(5000);
		//		//Program.ClipboardObserver = new ClipboardObserver();
		//		//Program.ClipboardObserver.ClipboardTextChanged += ClipboardObserver_ClipboardTextChanged;

		//		//int key = int.Parse(KeyText.Text);
		//		//const int KEYEVENTF_KEYUP = 0x0002;
		//		//keybd_event((byte)key, 0, 0, IntPtr.Zero);
		//		//keybd_event((byte)key, 0, KEYEVENTF_KEYUP, IntPtr.Zero);

		//		Application.DoEvents();
		//		if (Program.lastForegroundWindow != null)
		//		{
		//			BuildingBlocks.WindowsNT.Misc.SetForegroundWindowForced(Program.lastForegroundWindow);
		//			Application.DoEvents();
		//		}

		//		ClipboardObserver.Continue();
		//	}
		//}

	}
}
