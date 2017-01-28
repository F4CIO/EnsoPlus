using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CraftSynth.BuildingBlocks.WindowsNT
{
	public enum WindowState
	{
		NormalWidthAndHeight,
		Minimized,
		Maximized,
		NonMinimized
	}

	public class WindowsManager
	{
		#region Native methods
		private struct WINDOWPLACEMENT
		{
			public int length;
			public int flags;
			public int showCmd;
			public System.Drawing.Point ptMinPosition;
			public System.Drawing.Point ptMaxPosition;
			public System.Drawing.Rectangle rcNormalPosition;
		}

		[DllImport("user32.dll")]
    	[return: MarshalAs(UnmanagedType.Bool)]
    	private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
		
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWindowVisible(IntPtr hWnd);

		/// <summary>
		/// https://msdn.microsoft.com/en-us/library/windows/desktop/ms633548%28v=vs.85%29.aspx
		/// </summary>
		private enum ShowWindowEnum
		{
			SW_HIDE = 0,
			SW_SHOWNORMAL = 1,
			SW_SHOWMINIMIZED = 2,
			SW_SHOWMAXIMIZED = 3,
			SW_MAXIMIZE = 3,
			SW_SHOWNOACTIVATE = 4,
			SW_SHOW = 5,
			SW_MINIMIZE = 6,
			SW_SHOWMINNOACTIVE = 7,
			SW_SHOWNA = 8,
			SW_RESTORE = 9,
			SW_SHOWDEFAULT = 10,
			SW_FORCEMINIMIZE = 11
		};

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
		private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);
		#endregion

		public static WindowState? GetWindowState(IntPtr handle)
		{
			WindowState? r = null;

			if (handle != IntPtr.Zero)
			{
				WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
				GetWindowPlacement(handle, ref placement);
				switch (placement.showCmd)
				{
					case 1:
						r = WindowState.NormalWidthAndHeight;
						break;
					case 2:
						r = WindowState.Minimized;
						break;
					case 3:
						r = WindowState.Maximized;
						break;
				}
			}

			return r;
		}

		public static void SetWindowState(IntPtr handle, WindowState newWindowState, bool performShow, bool activate)
		{
			//translates this to common sense: https://msdn.microsoft.com/en-us/library/windows/desktop/ms633518%28v=vs.85%29.aspx
			switch (newWindowState)
			{
				case WindowState.NormalWidthAndHeight:
					if (performShow)
					{
						if (activate)
						{
							var ws = GetWindowState(handle);
							if (ws.HasValue && ws.Value == WindowState.Minimized)
							{
								ShowWindow(handle, ShowWindowEnum.SW_RESTORE);
							}
							else
							{
								ShowWindow(handle, ShowWindowEnum.SW_SHOWNORMAL);
							}
						}
						else
						{
							ShowWindow(handle, ShowWindowEnum.SW_SHOWNOACTIVATE);
						}
					}
					else
					{
						if (activate)
						{
							//no soultion ATM
						}
						else
						{
							//no solution ATM
						}
					}
					break;
				case WindowState.Minimized:
					if (performShow)
					{
						ShowWindow(handle, activate ? ShowWindowEnum.SW_SHOWMINIMIZED : ShowWindowEnum.SW_SHOWMINNOACTIVE);
					}
					else
					{
						ShowWindow(handle, activate ? ShowWindowEnum.SW_MINIMIZE: ShowWindowEnum.SW_MINIMIZE);
					}
					break;
				case WindowState.Maximized:
					if (performShow)
					{
						if (activate)
						{
							ShowWindow(handle, ShowWindowEnum.SW_SHOWMAXIMIZED);
						}
						else
						{
							ShowWindow(handle, ShowWindowEnum.SW_SHOWNA);
							ShowWindow(handle, ShowWindowEnum.SW_MAXIMIZE);
						}
					}
					else
					{
						if (activate)
						{
							//no solution ATM
						}
						else
						{
							ShowWindow(handle, ShowWindowEnum.SW_MAXIMIZE);
						}
					}
					break;
				case WindowState.NonMinimized:
					var state = GetWindowState(handle);
					bool isVisible = IsWindowVisible(handle);
					//if (state == null)
					//{
					//}
					//else
					//{
						if (state.Value == WindowState.Maximized && performShow && isVisible == false)
						{//already nonMinimized but need to perform show
							if (activate)
							{
								ShowWindow(handle, ShowWindowEnum.SW_SHOWMAXIMIZED);
							}
							else
							{
								ShowWindow(handle, ShowWindowEnum.SW_SHOWNA);
							}
						}
						else if (state.Value == WindowState.NormalWidthAndHeight && performShow && isVisible == false)
						{//already nonMinimized but need to perform show
							if (activate)
							{
								ShowWindow(handle, ShowWindowEnum.SW_SHOW);
							}
							else
							{
								ShowWindow(handle, ShowWindowEnum.SW_SHOWNA);
							}
						}
						else if (state.Value == WindowState.Minimized)
						{
							if (performShow)
							{
								if (activate)
								{
									ShowWindow(handle, ShowWindowEnum.SW_RESTORE);
								}
								else
								{
									ShowWindow(handle, ShowWindowEnum.SW_SHOWNOACTIVATE);
								}
							}
							else
							{
								//no solution ATM
							}
						}
					//}
					break;
				default:
					throw new ArgumentOutOfRangeException("SetWindowState: not recognized argument="+newWindowState);
			}
		}
	}
}
