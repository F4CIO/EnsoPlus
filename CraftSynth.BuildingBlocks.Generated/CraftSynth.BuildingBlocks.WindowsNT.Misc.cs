using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CraftSynth.BuildingBlocks.WindowsNT
{
	public class Misc
	{
		// For Windows Mobile, replace user32.dll with coredll.dll
		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		// Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.
		[DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
		private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

		[DllImport("user32.dll")]
		public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

		public const int WM_SYSCOMMAND = 0x0112;
		public const int SC_CLOSE = 0xF060;

		public static string SystemTemporaryFolderPath
		{
			get
			{
				return System.IO.Path.GetTempPath();
			}
		}

		/// <summary>
		/// Opens file with associated external application.
		/// </summary>
		/// <param name="filePath">Full path with filename of file to open.</param>
		/// <param name="filePath">Parameters to add. Pass null for no parameters.</param>
		/// <exception cref="ArgumentException">Thrown when parameter is null or empty.</exception>
		public static void OpenFile(string filePath, string parameters)
		{
			if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("Value must be non-empty string.", "filePath");

			Process process = new Process();
			process.StartInfo.UseShellExecute = true;
			process.StartInfo.FileName = filePath;
			if (!string.IsNullOrEmpty(parameters))
			{
				process.StartInfo.Arguments = parameters;
			}
			process.Start();
		}

		/// <summary>
		/// Opens file with associated external application.
		/// </summary>
		/// <param name="filePath">Full path with filename of file to open.</param>
		/// <exception cref="ArgumentException">Thrown when parameter is null or empty.</exception>
		public static void OpenFile(string filePath)
		{
			OpenFile(filePath, null);
		}

		public static void OpenTextInNotepad(string text)
		{
			string filePath = Guid.NewGuid().ToString() + ".txt";
			OpenTextInNotepad(filePath, text);
		}

		public static void OpenTextInNotepad(string temporaryFilePath, string text)
		{
			System.IO.File.WriteAllText(temporaryFilePath, text);
			Process process = new Process();
			process.StartInfo.UseShellExecute = true;
			process.StartInfo.FileName = "Notepad";
			process.StartInfo.Arguments = string.Format("\"{0}\"", temporaryFilePath);
			process.Start();
		}

		public static void OpenLikeTextFile(string text)
		{
			string filePath = Guid.NewGuid().ToString() + ".txt";
			OpenLikeTextFile(filePath, text);
		}

		public static void OpenLikeTextFile(string temporaryFilePath, string text)
		{
			System.IO.File.WriteAllText(temporaryFilePath, text);
			OpenFile(temporaryFilePath);
		}

		public static bool IsWindowOpen(string windowTitle)
		{
			if (string.IsNullOrEmpty(windowTitle)) throw new ArgumentException("Value must be non-empty string.", "windowTitle");

			IntPtr handle = FindWindowByCaption(IntPtr.Zero, windowTitle);
			return !handle.Equals(IntPtr.Zero);
		}

		public static Process GetProcessUnderCursor()
		{
			IntPtr ptr = IntPtr.Zero;

			//get window under cursor
			NativeMethods.POINT np;
			if (NativeMethods.GetCursorPos(out np))
			{
				ptr = NativeMethods.WindowFromPoint(np);
			}
			
			//get process id of window
			int processId = 0;
			NativeMethods.GetWindowThreadProcessId((int)ptr, out processId);

			//find process
			Process process = Process.GetProcessById(processId);
			
			return process;
		}

		public static IntPtr GetWindowUnderMouseCursor()
		{
			IntPtr result = IntPtr.Zero;

			NativeMethods.POINT np;
			if (NativeMethods.GetCursorPos(out np))
			{
				result = NativeMethods.WindowFromPoint(np);
			}

			return result;
		}

		public static IntPtr GetWindowUnderPoint(int x, int y)
		{
			IntPtr result = IntPtr.Zero;

			NativeMethods.POINT np;
			np.X = x;
			np.Y = y;
			result = NativeMethods.WindowFromPoint(np);
			
			return result;
		}

		public static IntPtr GetWindowByCaption(string caption)
		{
			return FindWindowByCaption(IntPtr.Zero, caption);
		}
		
		public static Process GetForegroundProcess()
		{
			IntPtr ptr = IntPtr.Zero;

			//get window under cursor
			ptr = GetForegroundWindow();
			
			//get process id of window
			int processId = 0;
			NativeMethods.GetWindowThreadProcessId((int)ptr, out processId);

			//find process
			Process process = Process.GetProcessById(processId);
			
			return process;
		}

		//cant find window
		public static IntPtr GetWindowByClassNameAndCaption(string className, string windowName)
		{
			return FindWindow(className, windowName);
		}

		public static bool CloseWindow(string windowTitle)
		{
			if (string.IsNullOrEmpty(windowTitle)) throw new ArgumentException("Value must be non-empty string.", "windowTitle");


			IntPtr handle = FindWindowByCaption(IntPtr.Zero, windowTitle);
			if (!handle.Equals(IntPtr.Zero))
			{
				SendMessage(handle.ToInt32(), WM_SYSCOMMAND, SC_CLOSE, 0);
				return true;
			}
			return false;
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", SetLastError = true)]
		private static extern Int32 GetWindowText(IntPtr hWnd, StringBuilder windowText, Int32 counter);

		public static string GetForegroundWindowCaption()
		{
			string windowTextString = null;

			StringBuilder windowText = new StringBuilder(256);

			IntPtr windowHandle = GetForegroundWindow();
			ThrowApplicationExceptionIfErrorOccurred(); //!!!

			int returnValue = GetWindowText(windowHandle, windowText, 256);
			ThrowApplicationExceptionIfErrorOccurred(); //!!!

			if (returnValue > 0)
			{
				windowTextString = windowText.ToString();
			}

			return windowTextString;
		}

		public static string GetWindowCaption(IntPtr windowHandle)
		{
			string windowTextString = null;

			StringBuilder windowText = new StringBuilder(256);

			int returnValue = GetWindowText(windowHandle, windowText, 256);
			ThrowApplicationExceptionIfErrorOccurred(); //!!!

			if (returnValue > 0)
			{
				windowTextString = windowText.ToString();
			}

			return windowTextString;
		}

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		public static string GetWindowClassName(IntPtr hWnd)
		{
			int nRet;
			StringBuilder className = new StringBuilder(100);
			//Get the window class name
			nRet = GetClassName(hWnd, className, className.Capacity);
			if (nRet != 0)
			{
				return className.ToString();
			}
			else
			{
				return null;
			}
		}

		public static string GetWindowModuleFileName(IntPtr hWnd)
		{
			StringBuilder fileName = new StringBuilder(2000);
			NativeMethods.GetWindowModuleFileName(hWnd, fileName, 2000);
			return fileName.ToString();
		}

		public static int GetWindowProcessId(IntPtr hWnd)
		{
			int processId = 0;
			NativeMethods.GetWindowThreadProcessId((int)hWnd, out processId);
			return processId;
		}

		public static Process GetWindowProcess(IntPtr hWnd)
		{
			int processId = GetWindowProcessId(hWnd);
			Process process = Process.GetProcessById(processId);
			return process;
		}

		// For Windows Mobile, replace user32.dll with coredll.dll
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		public static bool SetForegroundWindow(string caption)
		{
			IntPtr result = FindWindowByCaption(IntPtr.Zero, caption);
			if (result != IntPtr.Zero)
			{
				SetForegroundWindow(result);
			}
			return result != IntPtr.Zero;
		}

		[DllImport("Kernel32.dll")]
		private static extern Int32 FormatMessage(Int32 dwFlags, Int32 lpSource, Int32 intdwMessageId, Int32 dwLanguageId,
			ref String lpBuffer, Int32 nSize, Int32 Arguments);

		/// <summary>
		/// Extracts and returns error message based on specified error code. If error code is 0 returns 'Operation completed successfully'.
		/// </summary>
		/// <param name="errorCode"></param>
		/// <returns></returns>
		public static string GetLastErrorMessage(Int32 errorCode)
		{
			string lastErrorMessage = null;

			Int32 FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
			Int32 FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
			Int32 FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

			string lpMsgBuf = string.Empty;

			Int32 dwFlags = FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS;


			Int32 returnValue = FormatMessage(dwFlags, 0, errorCode, 0, ref lpMsgBuf, 255, 0);

			if (returnValue == 0)
			{
				lastErrorMessage = null;
			}
			else
			{
				lastErrorMessage = lpMsgBuf;
			}

			return lastErrorMessage;
		}

		/// <summary>
		/// Call this after every call of [DllImport("unmanagedLibrary.dll", SetLastError=true)] method to sumulate exception throwing concept.
		/// </summary>
		public static void ThrowApplicationExceptionIfErrorOccurred()
		{
			Int32 errorCode = Marshal.GetLastWin32Error();
			if (errorCode != 0)
			{
				string errorMessage = GetLastErrorMessage(errorCode);
				if (string.IsNullOrEmpty(errorMessage))
				{
					throw new ApplicationException("Unrecognized error code '" + errorCode + "' returned from unmanaged method call.");
				}
				else
				{
					throw new ApplicationException(errorMessage);
				}
			}
		}

		/// <summary>
		/// Closes main window of specified process.
		/// Returns number of matching processes found.
		/// </summary>
		/// <param name="process"></param>
		/// <param name="force"></param>
		/// <returns></returns>
		public static bool ProcessExists(string process)
		{
			Process[] processes = Process.GetProcessesByName(process);
			return processes.Length > 0;
		}


		/// <summary>
		/// Kills all procceses with specified name.
		/// </summary>
		/// <param name="name"></param>
		internal static void KillProcess(string name)
		{
			Process[] processesToKill = Process.GetProcessesByName(name);
			foreach (var processToKill in processesToKill)
			{
				processToKill.Kill();
			}
		}

		/// <summary>
		/// Closes main window of specified process.
		/// Returns number of matching processes found.
		/// </summary>
		/// <param name="process"></param>
		/// <param name="force"></param>
		/// <returns></returns>
		public static int CloseProcesses(string process, bool force)
		{
			Process[] processes = Process.GetProcessesByName(process);

			for (int i = 0; i < processes.Length; i++)
			{
				processes[i].CloseMainWindow();

				if (force)
					processes[i].Kill();
				else
					processes[i].WaitForExit();
			}

			return processes.Length;
		}

		/// <summary>
		/// Tells if system is 64bit.
		/// 
		/// As Microsoft's Raymond Chen describes, you have to first check if running in a 64-bit process 
		/// (I think in .NET you can do so by checking IntPtr.Size), and if you are running in a 32-bit process, 
		/// you still have to call the Win API function IsWow64Process. If this returns true, 
		/// you are running in a 32-bit process on 64-bit Windows.       
		/// http://blogs.msdn.com/oldnewthing/archive/2005/02/01/364563.aspx
		/// </summary>
		public static bool Is64BitOperatingSystem = (IntPtr.Size == 8) || InternalCheckIsWow64();

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool InternalCheckIsWow64()
		{
			if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
				Environment.OSVersion.Version.Major >= 6)
			{
				using (Process p = Process.GetCurrentProcess())
				{
					bool retVal;
					if (!NativeMethods.IsWow64Process(p.Handle, out retVal))
					{
						return false;
					}
					return retVal;
				}
			}
			else
			{
				return false;
			}
		}

		public static bool IsWindows10()
		{
			var reg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
			string productName = (string)reg.GetValue("ProductName");
			return productName.StartsWith("Windows 10");
		}

		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

		[DllImport("user32.dll")]
		private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

		[DllImport("kernel32.dll")]
		private static extern uint GetCurrentThreadId();

		//Source: http://stackoverflow.com/questions/2671669/is-there-a-reliable-way-to-activate-set-focus-to-a-window-using-c
		//Source 2: http://stackoverflow.com/questions/3787057/need-to-activate-a-window
		//problem to test: http://stackoverflow.com/questions/8075568/visual-studio-2010-hangs-when-i-debug-method-attachthreadinput/8081858#8081858
		public static void SetForegroundWindowForced(IntPtr hWnd, bool ignoreAllErrors = false)
		{
			IntPtr currentThreadId = (IntPtr)GetCurrentThreadId();
			IntPtr foregroundThreadID = (IntPtr)GetWindowThreadProcessId(Misc.GetForegroundWindow(), IntPtr.Zero);
			if (foregroundThreadID != currentThreadId)
			{
				try
				{
					AttachThreadInput((uint) currentThreadId, (uint) foregroundThreadID, true);
					if (!ignoreAllErrors)
					{
						ThrowApplicationExceptionIfErrorOccurred();
					}

					SetForegroundWindow(hWnd);
					if (!ignoreAllErrors)
					{
						ThrowApplicationExceptionIfErrorOccurred();
					}

					AttachThreadInput((uint) currentThreadId, (uint) foregroundThreadID, false);
					if (!ignoreAllErrors)
					{
						ThrowApplicationExceptionIfErrorOccurred();
					}
				}
				catch (Exception e)
				{
					if (ignoreAllErrors)//e.Message.Contains("The segment is already unlocked") || e.Message.Contains("Invalid window handle") || e.Message.Contains("The handle is invalid") ||e.Message.Contains("Attempt to access"))
					{
						//
					}
					else
					{
						throw;
					}
				}
			}
			else
			{
				SetForegroundWindow(hWnd);
			}

		}


		/// <summary>
		/// Evaluates Cursor Position with respect to client screen.
		/// Source: http://www.codeproject.com/Articles/34520/Getting-Caret-Position-Inside-Any-Application
		/// </summary>
		public static Point? GetCaretPosition()
		{
			Point? r;
			Point caretPosition = new Point();
			if (GetActiveProcess() == null)
			{
				r = null;
			}
			else
			{


				// Fetch GUITHREADINFO
				var guiInfo = new NativeMethods.GUITHREADINFO();
				guiInfo.cbSize = (uint)Marshal.SizeOf(guiInfo);

				// Get GuiThreadInfo into guiInfo
				NativeMethods.GetGUIThreadInfo(0, out guiInfo);

				if (guiInfo.rcCaret.Left == 0 && guiInfo.rcCaret.Bottom == 0)
				{
					r = null;
				}
				else
				{
					caretPosition.X = (int)guiInfo.rcCaret.Left + 25;
					caretPosition.Y = (int)guiInfo.rcCaret.Bottom + 25;

					NativeMethods.ClientToScreen(guiInfo.hwndCaret, out caretPosition);

					r = caretPosition;
				}
			}
			return r;
		}

		/// <summary>
		/// Retrieves name of active Process.
		/// </summary>
		/// <returns>Active Process Name</returns>
		private static string GetActiveProcess()
		{
			const int nChars = 256;
			int handle = 0;
			StringBuilder Buff = new StringBuilder(nChars);
			handle = (int)NativeMethods.GetForegroundWindow();

			// If Active window has some title info
			if (NativeMethods.GetWindowText(handle, Buff, nChars) > 0)
			{
				uint lpdwProcessId;
				uint dwCaretID = NativeMethods.GetWindowThreadProcessId(handle, out lpdwProcessId);
				uint dwCurrentID = (uint)Thread.CurrentThread.ManagedThreadId;
				return Process.GetProcessById((int)lpdwProcessId).ProcessName;
			}
			// Otherwise either error or non client region
			return null;
		}

		public static Rectangle? GetWindowPositionAndSize(IntPtr h)
		{
			Rectangle? r = null;
			NativeMethods.RECT rect;
			if (NativeMethods.GetWindowRect(h, out rect) == false)
			{
				r = null;
			}
			else
			{
				r = new Rectangle(new Point(rect.Left, rect.Top), new Size(rect.Right - rect.Left, rect.Bottom - rect.Top));
			};
			return r;
		}

		public static string GetStartMenuFolderPathForAllUsers()
		{
			StringBuilder path = new StringBuilder(260);
			NativeMethods.SHGetSpecialFolderPath(IntPtr.Zero, path, NativeMethods.CSIDL_COMMON_STARTMENU, false);
			string r = path.ToString();
			return r;
		}	
		public static string GetStartMenuFolderPathForCurrentUser()
		{
			StringBuilder path = new StringBuilder(260);
			NativeMethods.SHGetSpecialFolderPath(IntPtr.Zero, path, NativeMethods.CSIDL_STARTMENU, false);
			string r = path.ToString();
			return r;
		}

		public static void DropAllMemoryToSystemsPagefile(bool inNewThread, bool waitToFinish)
		{
			if (!inNewThread)
			{
				DropAllMemoryToSystemsPagefile_InNewThread();
			}
			else
			{
				var t = new Thread(DropAllMemoryToSystemsPagefile_InNewThread);
				t.Start();
				if (waitToFinish)
				{
					t.Join();
				}
			}
		}

		[DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
		private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);

		private static void DropAllMemoryToSystemsPagefile_InNewThread()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
		}



		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

		private const int WM_SETREDRAW = 11;

		public static void SuspendDrawing(Control parent)
		{
			SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
		}

		public static void ResumeDrawing(Control parent)
		{
			SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
			parent.Refresh();
		}

	}
}
