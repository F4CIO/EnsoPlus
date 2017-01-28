using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CraftSynth.BuildingBlocks.WindowsNT
{
    public class NativeMethods
    {
		#region Constants
		public const int WM_COPYDATA = 0x4A;
		public const int WM_CLOSE = 0x0010;
		#endregion

		#region Structs
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;

			public POINT(int x, int y)
			{
				this.X = x;
				this.Y = y;
			}

			//public static implicit operator System.Drawing.Point(POINT p)
			//{
			//    return new System.Drawing.Point(p.X, p.Y);
			//}

			//public static implicit operator POINT(System.Drawing.Point p)
			//{
			//    return new POINT(p.X, p.Y);
			//}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		public struct COPYDATASTRUCT
		{
			public IntPtr dwData;
			public int cbData;
			public IntPtr lpData;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct GUITHREADINFO
		{
			public uint cbSize;
			public uint flags;
			public IntPtr hwndActive;
			public IntPtr hwndFocus;
			public IntPtr hwndCapture;
			public IntPtr hwndMenuOwner;
			public IntPtr hwndMoveSize;
			public IntPtr hwndCaret;
			public RECT rcCaret;
		};

		GUITHREADINFO guiInfo;

		[StructLayoutAttribute(LayoutKind.Sequential)]
		public struct DROPFILES
		{
			public Int32 pFiles;// offset of file list
			public POINT pt;	// drop point (client coords)
			public bool fNC;	// is it on NonClient area and pt is in screen coords
			public bool fWide;	// wide character flag (pFiles is ANSI or UNICODE)
		}
		#endregion

		#region Methods
		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(POINT Point);

		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(out NativeMethods.POINT lpPoint);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern uint GetWindowModuleFileName(IntPtr hwnd, StringBuilder lpszFileName, uint cchFileNameMax);

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowThreadProcessId(int hWnd, out int ProcessId);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(System.IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

		[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWow64Process(
			[In] IntPtr hProcess,
			[Out] out bool wow64Process
		);



		/*- Retrieves Title Information of the specified window -*/
		[DllImport("user32.dll")]
		public static extern int GetWindowText(int hWnd, StringBuilder text, int count);

		/*- Retrieves Id of the thread that created the specified window -*/
		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint GetWindowThreadProcessId(int hWnd, out uint lpdwProcessId);

		/*- Retrieves information about active window or any specific GUI thread -*/
		[DllImport("user32.dll", EntryPoint = "GetGUIThreadInfo")]
		public static extern bool GetGUIThreadInfo(uint tId, out GUITHREADINFO threadInfo);

		/*- Retrieves Handle to the ForeGroundWindow -*/
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		/*- Converts window specific point to screen specific -*/
		[DllImport("user32.dll")]
		public static extern bool ClientToScreen(IntPtr hWnd, out Point position);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll")]
		public static extern IntPtr GetClipboardOwner();

		[DllImport("shell32.dll")]
		public static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);
		public const int CSIDL_COMMON_STARTMENU = 0x16;  // common start menu: \Windows\Start Menu\Programs
		public const int CSIDL_STARTMENU = 0x0B;  // current user's start menu
		#endregion
    }
}
