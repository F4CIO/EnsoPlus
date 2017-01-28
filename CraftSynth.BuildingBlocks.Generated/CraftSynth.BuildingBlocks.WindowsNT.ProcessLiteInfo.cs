using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;

namespace CraftSynth.BuildingBlocks.WindowsNT
{
	[Serializable]
	public class ProcessLiteInfo
	{
		public int Id;
		public string Name;
		public string FilePath;
		public string MainWindowTitle;
		public byte[] IconBytes;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="process"></param>
		/// <param name="loadFilePathAlso">
		/// Dont use it if not neccessary. See MS _bug : 
		/// http://stackoverflow.com/questions/10986486/c-sharp-only-part-of-a-readprocessmemory-or-writeprocessmemory-request-was-compl
		/// </param>
		public ProcessLiteInfo(Process process, bool tryLoadingFilePathAlso, bool tryToPopulateIconBytesAlso)
		{
			this.Id = process.Id;
			this.Name = process.ProcessName;

			if (tryLoadingFilePathAlso)
			{
				TryToPopulateFilePathIfIsNullOrEmpty(process);
			}

			if (tryToPopulateIconBytesAlso)
			{
				this.TryToPopulateIconBytesIfIsNull();
			}


			this.MainWindowTitle = process.MainWindowTitle;
		}

		public void TryToPopulateFilePathIfIsNullOrEmpty(Process process)
		{
			if (process != null && string.IsNullOrEmpty(this.FilePath))
			{
				try
				{
					this.FilePath = process.MainModule.FileName;
				}
				catch (Exception e)
				{
					// http://stackoverflow.com/questions/10986486/c-sharp-only-part-of-a-readprocessmemory-or-writeprocessmemory-request-was-compl
				}
			}

			if (string.IsNullOrEmpty(this.FilePath))
			{
				try
				{
					this.FilePath = GetMainModuleFilepath(this.Id);
				}
				catch (Exception ee)
				{
				}
			}
		}

		public void TryToPopulateIconBytesIfIsNull()
		{
			if (!string.IsNullOrEmpty(this.FilePath) && this.IconBytes == null)
			{
				try
				{
					using (Icon icon = Icon.ExtractAssociatedIcon(this.FilePath))
					{
						if (icon != null)
						{
							using (MemoryStream ms = new MemoryStream())
							{
								icon.Save(ms);
								this.IconBytes = ms.ToArray();
							}
						}
					}
				}
				catch (Exception e)
				{
					Debug.Fail("Can't extract icon from '" + this.FilePath + "'. " + e.Message);
				}
			}
		}

		public ProcessLiteInfo(
			int Id,
			string Name,
			string FilePath,
			string MainWindowTitle, 
			byte[] IconBytes)
		{
			this.Id = Id;
			this.Name = Name;
			this.FilePath = FilePath;
			this.MainWindowTitle = MainWindowTitle;
			this.IconBytes = IconBytes;
		}

		public ProcessLiteInfo()
		{
			//used for deserialization
		}

		public byte[] Serialize()
		{
			List<Object> list = new List<object>();
			list.Add(2);//version
			list.Add(this.Id);
			list.Add(this.Name);
			list.Add(this.FilePath);
			list.Add(this.MainWindowTitle);
			list.Add(this.IconBytes);

			byte[] r = CraftSynth.BuildingBlocks.IO.Xml.Misc.Serialize(list, false);
			return r;
		}

		public static ProcessLiteInfo Deserialize(byte[] bytes)
		{
			var r = new ProcessLiteInfo();

			List<object> list = (List<object>)CraftSynth.BuildingBlocks.IO.Xml.Misc.Deserialize(bytes, false);
			int version = (int)list[0];
			if (version == 1)
			{
				r.Id = (int)list[1];
				r.Name = (string)list[2];
				r.FilePath = (string)list[3];
				r.MainWindowTitle = (string)list[4];
			}
			else if (version == 2)
			{
				r.Id = (int)list[1];
				r.Name = (string)list[2];
				r.FilePath = (string)list[3];
				r.MainWindowTitle = (string)list[4];
				r.IconBytes = (byte[]) list[5];
			}
			else
			{
				throw new Exception("ProcessLiteInfo.Deserialize: version not recognized. Version=" + version ?? "null");
			}

			return r;
		}

		public decimal SizeInBytesOfManagedpart()
		{
			return sizeof (int)
			       + (this.Name == null ? 0 : sizeof (char)*this.Name.Length)
				   + (this.MainWindowTitle == null ? 0 : sizeof(char) * this.MainWindowTitle.Length)
				   + (this.FilePath == null ? 0 : sizeof(char) * this.FilePath.Length)
				   + (this.IconBytes == null ? 0 : sizeof(byte) * this.IconBytes.Count());
		}

		/// <summary>
		/// Source: http://stackoverflow.com/questions/9501771/how-to-avoid-a-win32-exception-when-accessing-process-mainmodule-filename-in-c
		/// </summary>
		/// <param name="processId"></param>
		/// <returns></returns>
		private static string GetMainModuleFilepath(int processId)
		{
			string wmiQueryString = "SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE ProcessId = " + processId;
			using (var searcher = new ManagementObjectSearcher(wmiQueryString))
			{
				using (var results = searcher.Get())
				{
					ManagementObject mo = results.Cast<ManagementObject>().FirstOrDefault();
					if (mo != null)
					{
						return (string) mo["ExecutablePath"];
					}
				}
			}
			return null;
		}
	}

	public static class ProcessLiteInfoExtender
	{
		public static byte[] SerializeOrIfNullReturnNull(this ProcessLiteInfo pli)
		{
			if (pli == null)
			{
				return null;
			}
			else
			{
				return pli.Serialize();
			}
		}

		public static ProcessLiteInfo DeserializeOrIfNullReturnNull(this byte[] bytes)
		{
			if (bytes == null)
			{
				return null;
			}
			else
			{
				return ProcessLiteInfo.Deserialize(bytes);
			}
		}

		public static decimal SizeInBytesOfManagedPartOrIfNullZero(this ProcessLiteInfo pli)
		{
			if (pli == null)
			{
				return 0;
			}
			else
			{
				return pli.SizeInBytesOfManagedpart();

			}
		}

	}
}
