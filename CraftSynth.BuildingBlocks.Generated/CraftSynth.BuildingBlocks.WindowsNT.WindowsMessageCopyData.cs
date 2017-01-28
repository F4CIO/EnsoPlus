using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CraftSynth.BuildingBlocks.WindowsNT
{
	public class WindowsMessageCopyData
	{
		public static void SendMessageGeneric(IntPtr handle, int msg, IntPtr wparam, IntPtr lparam)
		{
			NativeMethods.SendMessage(handle, msg, wparam, lparam);
		}


		public static void SendMessageWithData(IntPtr destHandle, string str, IntPtr srcHandle)
		{
			NativeMethods.COPYDATASTRUCT cds;

			cds.dwData = srcHandle;
			str = str + '\0';

			cds.cbData = str.Length + 1;
			cds.lpData = Marshal.AllocCoTaskMem(str.Length);
			cds.lpData = Marshal.StringToCoTaskMemAnsi(str);
			IntPtr iPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(cds));
			Marshal.StructureToPtr(cds, iPtr, true);

			// send to the MFC app
			NativeMethods.SendMessage(destHandle, NativeMethods.WM_COPYDATA, IntPtr.Zero, iPtr);

			// Don't forget to free the allocatted memory 
			Marshal.FreeCoTaskMem(cds.lpData);
			Marshal.FreeCoTaskMem(iPtr);
		}

		public static void SendMessageWithDataUsingHGlobal(IntPtr destHandle, string str, IntPtr srcHandle)
		{
			NativeMethods.COPYDATASTRUCT cds;

			cds.dwData = srcHandle;
			str = str + '\0';

			cds.cbData = str.Length + 1;
			cds.lpData = Marshal.AllocHGlobal(str.Length);
			cds.lpData = Marshal.StringToHGlobalAnsi(str);
			IntPtr iPtr = Marshal.AllocHGlobal(Marshal.SizeOf(cds));
			Marshal.StructureToPtr(cds, iPtr, true);

			// send to the MFC app
			NativeMethods.SendMessage(destHandle, NativeMethods.WM_COPYDATA, srcHandle, iPtr);

			// Don't forget to free the allocatted memory 
			Marshal.FreeCoTaskMem(cds.lpData);
			Marshal.FreeCoTaskMem(iPtr);
		}
	}
}
