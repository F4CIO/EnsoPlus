using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace CraftSynth.BuildingBlocks.IO
{
	public enum RegistryRetryScenario
	{
		TryDefaultOnly,
		Try32BitOnly,
		Try64BitOnly,
		TryDefaultAndThen32AndThen64,
		TryDefauldAndThen64AndThen32
	}
	public class Registry
	{
		/// <summary>
		/// Gets value as string from windows registry item.
		/// </summary>
		/// <param name="keyPath">Full path to registry item. Example: HKEY_CURRENT_USER\Software\Humanized\Enso\Location</param>
		/// <param name="retryScenario">On 32-bit OS there is only single node. On 64-bit OS a 32-bit application will be looking at the HKLM\Software\Wow6432Node node by default and 64-bit application will use HKLM\Software. Using this parameter you can override this behaviour.</param>
		/// <returns>Value as string or null if keyPath not found or error occured.</returns>
		public static string GetRegistryStringValue(string keyPath, RegistryRetryScenario retryScenario)
		{
			string r;
			switch (retryScenario)
			{
				case RegistryRetryScenario.TryDefaultOnly:
					r = GetRegistryStringValue(keyPath);
					break;
				case RegistryRetryScenario.Try32BitOnly:
					r = GetRegistryStringValue(keyPath, 32);
					break;
				case RegistryRetryScenario.Try64BitOnly:
					r = GetRegistryStringValue(keyPath, 64);
					break;
				case RegistryRetryScenario.TryDefaultAndThen32AndThen64:
					r = GetRegistryStringValue(keyPath);
					if (r == null)
					{
						r = GetRegistryStringValue(keyPath, 32);
					}
					if (r == null)
					{
						r = GetRegistryStringValue(keyPath, 64);
					}
					break;
				case RegistryRetryScenario.TryDefauldAndThen64AndThen32:
					r = GetRegistryStringValue(keyPath);
					if (r == null)
					{
						r = GetRegistryStringValue(keyPath, 64);
					}
					if (r == null)
					{
						r = GetRegistryStringValue(keyPath, 32);
					}
					break;
				default:
					throw new Exception("retryScenario '"+retryScenario+"' not implemented.");
			}
			return r;
		}
		/// <summary>
		/// Gets value as string from windows registry item.
		/// </summary>
		/// <param name="keyPath">Full path to registry item. Example: HKEY_CURRENT_USER\Software\Humanized\Enso\Location</param>
		/// <param name="forceUsageOf32Or64Subkey">On 32-bit OS there is only single node. On 64-bit OS a 32-bit application will be looking at the HKLM\Software\Wow6432Node node by default and 64-bit application will use HKLM\Software. Using this parameter you can override this behaviour.</param>
		/// <returns>Value as string or null if keyPath not found or error occured.</returns>
		public static string GetRegistryStringValue(string keyPath, int? forceUsageOf32Or64Subkey = null)
		{
			string stringValue = null;
			try
			{
				string[] keyPathSteps = keyPath.Trim().Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
				RegistryKey hive = null;
				if (keyPathSteps[0] == "HKEY_CURRENT_USER")
				{
					hive = Microsoft.Win32.Registry.CurrentUser;
				}
				else if (keyPathSteps[0] == "HKEY_LOCAL_MACHINE")
				{
					hive = Microsoft.Win32.Registry.LocalMachine;
				}
				else if (keyPathSteps[0] == "HKEY_CLASSES_ROOT")
				{
					hive = Microsoft.Win32.Registry.ClassesRoot;
				}
				else if (keyPathSteps[0] == "HKEY_CURRENT_CONFIG")
				{
					hive = Microsoft.Win32.Registry.CurrentConfig;
				}
				else if (keyPathSteps[0] == "HKEY_DYN_DATA")
				{
					hive = Microsoft.Win32.Registry.DynData;
				}
				else if (keyPathSteps[0] == "HKEY_PERFORMANCE_DATA")
				{
					hive = Microsoft.Win32.Registry.PerformanceData;
				}
				else
				{
					throw new Exception("Hive name not recognized.");
				}

				string hiveName = keyPathSteps[0];
				string subKey = null;
				if (keyPathSteps.Length > 2)
				{
					int i = 1;
					subKey = string.Empty;
					while (i < keyPathSteps.Length - 1)
					{
						subKey += keyPathSteps[i] + '\\';
						i++;
					}
					subKey.Remove(subKey.Length - 2);
					hive = hive.OpenSubKey(subKey);
				}
				string propertyName = keyPathSteps[keyPathSteps.Length - 1];

				if (forceUsageOf32Or64Subkey == null)
				{
					object value = hive.GetValue(propertyName);
					stringValue = value.ToString();
				}
				else if (forceUsageOf32Or64Subkey == 32)
				{
					stringValue = RegistryWOW6432.GetRegKey32(hiveName, subKey, propertyName);
				}
				else if (forceUsageOf32Or64Subkey == 64)
				{
					stringValue = RegistryWOW6432.GetRegKey64(hiveName, subKey, propertyName);
				}
				else
				{
					throw new Exception("forceUsageOf32Or64Subkey must be 32 or 64.");
				}

			}
			catch (Exception) { stringValue = null; }

			return stringValue;
		}

		/// <summary>
		/// Creates or modifies value in windows registry.
		/// </summary>
		/// <param name="keyPath">Full path to registry item. Example: HKEY_CURRENT_USER\Software\Humanized\Enso\Location</param>
		/// <param name="retryScenario">On 32-bit OS there is only single node. On 64-bit OS a 32-bit application will be looking at the HKLM\Software\Wow6432Node node by default and 64-bit application will use HKLM\Software. Using this parameter you can override this behaviour.</param>
		/// <returns>True for success.</returns>
		public static bool SetRegistryValue(string keyPath, RegistryRetryScenario retryScenario, RegistryValueKind valueType, object value)
		{
			bool r;
			switch (retryScenario)
			{
				case RegistryRetryScenario.TryDefaultOnly:
					r = SetRegistryValue(keyPath, valueType, value);
					break;
				case RegistryRetryScenario.Try32BitOnly:
					r = SetRegistryValue(keyPath, valueType, value, 32);
					break;
				case RegistryRetryScenario.Try64BitOnly:
					r = SetRegistryValue(keyPath, valueType, value, 64);
					break;
				case RegistryRetryScenario.TryDefaultAndThen32AndThen64:
					r = SetRegistryValue(keyPath, valueType, value);
					if (r == false)
					{
						r = SetRegistryValue(keyPath, valueType, value, 32);
					}
					if (r == false)
					{
						r = SetRegistryValue(keyPath, valueType, value, 64);
					}
					break;
				case RegistryRetryScenario.TryDefauldAndThen64AndThen32:
					r = SetRegistryValue(keyPath, valueType, value);
					if (r == false)
					{
						r = SetRegistryValue(keyPath, valueType, value, 64);
					}
					if (r == false)
					{
						r = SetRegistryValue(keyPath, valueType, value, 32);
					}
					break;
				default:
					throw new Exception("retryScenario '" + retryScenario + "' not implemented.");
			}
			return r;
		}
		/// <summary>
		/// Creates or modifies value in windows registry.
		/// </summary>
		/// <param name="keyPath">Full path to registry item. Example: HKEY_CURRENT_USER\Software\Humanized\Enso\Location</param>
		/// <param name="forceUsageOf32Or64Subkey">On 32-bit OS there is only single node. On 64-bit OS a 32-bit application will be looking at the HKLM\Software\Wow6432Node node by default and 64-bit application will use HKLM\Software. Using this parameter you can override this behaviour.</param>
		/// <returns>True for success.</returns>
		public static bool SetRegistryValue(string keyPath, RegistryValueKind valueType, object value, int? forceUsageOf32Or64Subkey = null)
		{
			bool success = false;
			try
			{
				string[] keyPathSteps = keyPath.Trim().Split(new char[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);
				RegistryKey hive = null;
				if (keyPathSteps[0] == "HKEY_CURRENT_USER")
				{
					hive = Microsoft.Win32.Registry.CurrentUser;
				}
				else if (keyPathSteps[0] == "HKEY_LOCAL_MACHINE")
				{
					hive = Microsoft.Win32.Registry.LocalMachine;
				}
				else if (keyPathSteps[0] == "HKEY_CLASSES_ROOT")
				{
					hive = Microsoft.Win32.Registry.ClassesRoot;
				}
				else if (keyPathSteps[0] == "HKEY_CURRENT_CONFIG")
				{
					hive = Microsoft.Win32.Registry.CurrentConfig;
				}
				else if (keyPathSteps[0] == "HKEY_DYN_DATA")
				{
					hive = Microsoft.Win32.Registry.DynData;
				}
				else if (keyPathSteps[0] == "HKEY_PERFORMANCE_DATA")
				{
					hive = Microsoft.Win32.Registry.PerformanceData;
				}
				else
				{
					throw new Exception("Hive name not recognized.");
				}

				string hiveName = keyPathSteps[0];
				string subKey = null;
				if (keyPathSteps.Length > 2)
				{
					int i = 1;
					subKey = string.Empty;
					while (i < keyPathSteps.Length - 1)
					{
						subKey += keyPathSteps[i] + '\\';
						i++;
					}
					subKey.Remove(subKey.Length - 2);
					hive = hive.OpenSubKey(subKey);
				}
				string propertyName = keyPathSteps[keyPathSteps.Length - 1];

				if (forceUsageOf32Or64Subkey == null)
				{
					hive.SetValue(propertyName, value, valueType);
					success = true;
				}
				else if (forceUsageOf32Or64Subkey == 32)
				{
					success = RegistryWOW6432.SetRegKey32(hiveName, subKey, propertyName, valueType, value);
				}
				else if (forceUsageOf32Or64Subkey == 64)
				{
					success = RegistryWOW6432.SetRegKey64(hiveName, subKey, propertyName, valueType, value);
				}
				else
				{
					throw new Exception("forceUsageOf32Or64Subkey must be 32 or 64.");
				}

			}
			catch (Exception)
			{
				success = false;
			}

			return success;
		}

	}


	/// <summary>
	/// An extension class to allow a registry key to allow it to get the
	/// registry in the 32 bit (Wow6432Node) or 64 bit regular registry key
	/// 
	/// Source: http://www.rhyous.com/2011/01/24/how-read-the-64-bit-registry-from-a-32-bit-application-or-vice-versa/
	/// 
	/// Usage: 
	///  <example>
	///  class Program
	///  {
	///    static void Main(string[] args)
	///    {
	///        string value64 = string.Empty;
	///        string value32 = string.Empty;
	///
	///        byte[] byteValue64 = new byte[1024];
	///        byte[] byteValue32 = new byte[1024];
	///        RegistryKey localKey = Registry.LocalMachine;
	///         
	///        localKey = localKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
	///        if (localKey != null)
	///        {
	///            value32 = localKey.GetRegKey32("RegisteredOrganization");
	///            value64 = localKey.GetRegKey64("RegisteredOrganization");
	///
	///            // byteValue32 = localKey.GetRegKey32AsByteArray("DigitalProductId"); // Key doesn't exist by default in 32 bit
	///            byteValue64 = localKey.GetRegKey64AsByteArray("DigitalProductId");
	///        }
	///    }
	///  }
	///  </example>
	/// </summary>
	public static class RegistryWOW6432
	{
		#region Read value
		#region Member Variables
		#region Read 64bit Reg from 32bit app
		public static UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(0x80000002u);
		public static UIntPtr HKEY_CURRENT_USER = new UIntPtr(0x80000001u);

		[DllImport("Advapi32.dll")]
		static extern uint RegOpenKeyEx(
			UIntPtr hKey,
			string lpSubKey,
			uint ulOptions,
			int samDesired,
			out int phkResult);

		[DllImport("Advapi32.dll")]
		static extern uint RegCloseKey(int hKey);

		[DllImport("advapi32.dll", EntryPoint = "RegQueryValueEx")]
		public static extern int RegQueryValueEx(
			int hKey,
			string lpValueName,
			int lpReserved,
			ref RegistryValueKind lpType,
			StringBuilder lpData,
			ref uint lpcbData);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueEx")]
		private static extern int RegQueryValueEx(
			int hKey,
			string lpValueName,
			int lpReserved,
			ref RegistryValueKind lpType,
			[Out] byte[] lpData,
			ref uint lpcbData);
		#endregion
		#endregion

		#region Functions
		public static string GetRegKey64(string hiveName, string subKeyPath, string propertyName)
		{
			return GetRegKey64(GetRegHiveFromString(hiveName), subKeyPath, RegSAM.WOW64_64Key, propertyName);
		}
		public static string GetRegKey64(this RegistryKey inKey, String inPropertyName)
		{
			string strKey = inKey.ToString();
			string regHive = strKey.Split('\\')[0];
			string regPath = strKey.Substring(strKey.IndexOf('\\') + 1);
			return GetRegKey64(GetRegHiveFromString(regHive), regPath, RegSAM.WOW64_64Key, inPropertyName);
		}

		public static string GetRegKey32(string hiveName, string subKeyPath, string propertyName)
		{
			return GetRegKey64(GetRegHiveFromString(hiveName), subKeyPath, RegSAM.WOW64_32Key, propertyName);
		}

		public static string GetRegKey32(this RegistryKey inKey, String inPropertyName)
		{
			string strKey = inKey.ToString();
			string regHive = strKey.Split('\\')[0];
			string regPath = strKey.Substring(strKey.IndexOf('\\') + 1);
			return GetRegKey64(GetRegHiveFromString(regHive), regPath, RegSAM.WOW64_32Key, inPropertyName);
		}

		public static byte[] GetRegKey64AsByteArray(this RegistryKey inKey, String inPropertyName)
		{
			string strKey = inKey.ToString();
			string regHive = strKey.Split('\\')[0];
			string regPath = strKey.Substring(strKey.IndexOf('\\') + 1);
			return GetRegKey64AsByteArray(GetRegHiveFromString(regHive), regPath, RegSAM.WOW64_64Key, inPropertyName);
		}

		public static byte[] GetRegKey32AsByteArray(this RegistryKey inKey, String inPropertyName)
		{
			string strKey = inKey.ToString();
			string regHive = strKey.Split('\\')[0];
			string regPath = strKey.Substring(strKey.IndexOf('\\') + 1);
			return GetRegKey64AsByteArray(GetRegHiveFromString(regHive), regPath, RegSAM.WOW64_32Key, inPropertyName);
		}

		private static UIntPtr GetRegHiveFromString(string inString)
		{
			if (inString == "HKEY_LOCAL_MACHINE")
				return HKEY_LOCAL_MACHINE;
			if (inString == "HKEY_CURRENT_USER")
				return HKEY_CURRENT_USER;
			return UIntPtr.Zero;
		}

		static public string GetRegKey64(UIntPtr inHive, String inKeyName, RegSAM in32or64key, String inPropertyName)
		{
			//UIntPtr HKEY_LOCAL_MACHINE = (UIntPtr)0x80000002;
			int hkey = 0;

			try
			{
				uint lResult = RegOpenKeyEx(inHive, inKeyName, 0, (int)RegSAM.QueryValue | (int)in32or64key, out hkey);
				if (0 != lResult) return null;
				RegistryValueKind lpType = 0;
				uint lpcbData = 1024;
				StringBuilder strBuffer = new StringBuilder(1024);
				RegQueryValueEx(hkey, inPropertyName, 0, ref lpType, strBuffer, ref lpcbData);
				string value = strBuffer.ToString();
				return value;
			}
			finally
			{
				if (0 != hkey) RegCloseKey(hkey);
			}
		}

		static public byte[] GetRegKey64AsByteArray(UIntPtr inHive, String inKeyName, RegSAM in32or64key, String inPropertyName)
		{
			int hkey = 0;

			try
			{
				uint lResult = RegOpenKeyEx(inHive, inKeyName, 0, (int)RegSAM.QueryValue | (int)in32or64key, out hkey);
				if (0 != lResult) return null;
				RegistryValueKind lpType = 0;
				uint lpcbData = 2048;

				// Just make a big buffer the first time
				byte[] byteBuffer = new byte[1000];
				// The first time, get the real size
				RegQueryValueEx(hkey, inPropertyName, 0, ref lpType, byteBuffer, ref lpcbData);
				// Now create a correctly sized buffer
				byteBuffer = new byte[lpcbData];
				// now get the real value
				RegQueryValueEx(hkey, inPropertyName, 0, ref lpType, byteBuffer, ref lpcbData);

				return byteBuffer;
			}
			finally
			{
				if (0 != hkey) RegCloseKey(hkey);
			}
		}
		#endregion

		#region Enums
		public enum RegSAM
		{
			QueryValue = 0x0001,
			SetValue = 0x0002,
			CreateSubKey = 0x0004,
			EnumerateSubKeys = 0x0008,
			Notify = 0x0010,
			CreateLink = 0x0020,
			WOW64_32Key = 0x0200,
			WOW64_64Key = 0x0100,
			WOW64_Res = 0x0300,
			Read = 0x00020019,
			Write = 0x00020006,
			Execute = 0x00020019,
			AllAccess = 0x000f003f
		}
		#endregion
		#endregion Read value

		#region Modify value
		//[DllImport("advapi32.dll", SetLastError = true)]
		//static extern int RegSetValueEx(
		//	IntPtr hKey,
		//	[MarshalAs(UnmanagedType.LPStr)] string lpValueName,
		//	int Reserved,
		//	Microsoft.Win32.RegistryValueKind dwType,
		//	[MarshalAs(UnmanagedType.LPStr)] string lpData,
		//	int cbData);

		// Alternate definition - more correct
		[DllImport("advapi32.dll", SetLastError = true)]
		static extern uint RegSetValueEx(
			 UIntPtr hKey,
			 [MarshalAs(UnmanagedType.LPStr)]
			 string lpValueName,
			 int Reserved,
			 RegistryValueKind dwType,
			 IntPtr lpData,
			 int cbData);

		public static bool SetRegKey64(string hiveName, string subKeyPath, string propertyName, RegistryValueKind valueType, object value)
		{
			return SetRegKey64(GetRegHiveFromString(hiveName), subKeyPath, RegSAM.WOW64_64Key, propertyName, valueType, value);
		}
		public static bool SetRegKey64(this RegistryKey inKey, String inPropertyName, RegistryValueKind valueType, object value)
		{
			string strKey = inKey.ToString();
			string regHive = strKey.Split('\\')[0];
			string regPath = strKey.Substring(strKey.IndexOf('\\') + 1);
			return SetRegKey64(GetRegHiveFromString(regHive), regPath, RegSAM.WOW64_64Key, inPropertyName, valueType, value);
		}

		public static bool SetRegKey32(string hiveName, string subKeyPath, string propertyName, RegistryValueKind valueType, object value)
		{
			return SetRegKey64(GetRegHiveFromString(hiveName), subKeyPath, RegSAM.WOW64_32Key, propertyName, valueType, value);
		}

		public static bool SetRegKey32(this RegistryKey inKey, String inPropertyName, RegistryValueKind valueType, object value)
		{
			string strKey = inKey.ToString();
			string regHive = strKey.Split('\\')[0];
			string regPath = strKey.Substring(strKey.IndexOf('\\') + 1);
			return SetRegKey64(GetRegHiveFromString(regHive), regPath, RegSAM.WOW64_32Key, inPropertyName, valueType, value);
		}

		static public bool SetRegKey64(UIntPtr inHive, String inKeyName, RegSAM in32or64key, String inPropertyName, RegistryValueKind valueType, object value)
		{
			bool success = false;

			int hkey = 0;
			try
			{
				uint lResult = RegOpenKeyEx(inHive, inKeyName, 0, (int) RegSAM.SetValue | (int) in32or64key, out hkey);
				if (0 != lResult)
				{
					success = false;
				}
				else
				{
					int size = 0;
					IntPtr pData = IntPtr.Zero;
					switch (valueType)
					{
						case RegistryValueKind.String:
							size = ((string) value).Length + 1;
							pData = Marshal.StringToHGlobalAnsi((string) value);
							break;
						case RegistryValueKind.DWord:
							size = Marshal.SizeOf(typeof (Int32));
							pData = Marshal.AllocHGlobal(size);
							Marshal.WriteInt32(pData, (int) value);
							break;
						case RegistryValueKind.QWord:
							size = Marshal.SizeOf(typeof (Int64));
							pData = Marshal.AllocHGlobal(size);
							Marshal.WriteInt64(pData, (long) value);
							break;
					}

					// Set the value
					UIntPtr hkeyAsUintPtr = new UIntPtr((uint) hkey);
					uint retVal = RegSetValueEx(hkeyAsUintPtr, inPropertyName, 0, valueType, pData, size);
					success = retVal == 0;
				}
			}
			catch (Exception)
			{
				success = false;
			}
			finally
			{
				if (0 != hkey) RegCloseKey(hkey);
			}
			return success;
		}
		#endregion
	}
}
