using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.Logging;

namespace Common
{
	public class HandlerForPreviousVersions
	{
		public static Dictionary<string, string> GetOtherEnsoPlusVersionsPresent()
		{
			Dictionary<string, string> r = new Dictionary<string, string>();
			string currentEnsoPlusWorkingFolder = Helper.GetEnsoPlusWorkingFolder();
			var siblingsFolderPaths = CraftSynth.BuildingBlocks.IO.FileSystem.GetFolderPaths(CraftSynth.BuildingBlocks.IO.FileSystem.GetParentFolderPath(currentEnsoPlusWorkingFolder));
			foreach (string siblingFolderPath in siblingsFolderPaths)
			{
				if (string.Compare(siblingFolderPath.Trim('\\'), currentEnsoPlusWorkingFolder.Trim('\\'), StringComparison.OrdinalIgnoreCase) != 0)
				{
					try
					{
						string ensoPlusExePath = Path.Combine(siblingFolderPath, "EnsoPlus.exe");
						string ensoPlusVersionPath = Path.Combine(siblingFolderPath, "Version.txt");
						string candidateVersion = File.ReadAllText(ensoPlusVersionPath);
						if (File.Exists(ensoPlusExePath) && File.Exists(ensoPlusVersionPath) &&
						    !string.IsNullOrEmpty(candidateVersion) && candidateVersion != "Debug" &&
						    !r.ContainsKey(candidateVersion))
						{
							r.Add(candidateVersion, siblingFolderPath);
						}
					}
					catch (Exception) { }
				}
			}
			if (r.Count > 0)
			{
				var l = r.OrderBy(key => key.Key);
				r = l.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
			}
			return r;
		}

		/// <summary>
		/// Gets the latest other version that is less than running one. If not such is found returns null.
		/// </summary>
		/// <returns></returns>
		public static KeyValuePair<string, string>? GetPreviousEnsoPlusVersionAndFolderIfPresent()
		{
			KeyValuePair<string, string>? r = null;
			var otherVersions = GetOtherEnsoPlusVersionsPresent();
			
			if (otherVersions.Count == 0)
			{
				r = null;
			}
			else
			{
				string currentVersion = Helper.GetEnsoPlusVersion();
				
				KeyValuePair<string, string>? lastLower = null;
				foreach (KeyValuePair<string, string> otherVersion in otherVersions)
				{
					if (CompareVersions(otherVersion.Key, currentVersion) < 0)
					{
						lastLower = otherVersion;
					}
				}

				if (lastLower == null)
				{
					r = null;
				}
				else
				{
					r = lastLower;
				}
			}
			return r;
		}

		public static bool ImportSettingsFromFolder(string folderPath)
		{
			bool importedAll = true;

			
			Logging.AddActionLog("Importing settings from:"+folderPath);
			List<string> oldVersionFilesToCopy = new List<string>();
			oldVersionFilesToCopy.Add(Path.Combine(folderPath, "EnsoPlusSettings.xml"));	
			oldVersionFilesToCopy.Add(Path.Combine(folderPath, "OSD.Bookmarks.ini"));
			oldVersionFilesToCopy.Add(Path.Combine(folderPath, "SelectionListener.ini"));

			string now = CraftSynth.BuildingBlocks.Common.DateAndTime.GetCurrentDateAndTimeInSortableFormatForFileSystem();

			foreach (string oldVersionFilePath in oldVersionFilesToCopy)
			{
				try
				{
					string currentVersionFilePath = oldVersionFilePath.Replace(folderPath, Helper.GetEnsoPlusWorkingFolder());
					if (File.Exists(currentVersionFilePath))
					{
						File.Move(currentVersionFilePath, currentVersionFilePath + "." + now + ".backup");
					}
					File.Copy(oldVersionFilePath, currentVersionFilePath, true);
				}
				catch (Exception e)
				{
					importedAll = false;
					Logging.AddErrorLog("Importing of settings from folder failed. Folder:" + folderPath);
					Logging.AddExceptionLog(e);
				}
			}

			Settings.Initialize();
			Settings.Current.Save();

			return importedAll;
		}

		public static void OfferImportingOfSettingsFromPreviousVersion()
		{
			var pv = GetPreviousEnsoPlusVersionAndFolderIfPresent();
			if (pv != null)
			{
				if (MessageBox.Show(string.Format("Previous version '{0}' detected. Do you want to import settings?", pv.Value.Key), "Enso+", MessageBoxButtons.YesNo)== DialogResult.Yes)
				{
					if (!ImportSettingsFromFolder(pv.Value.Value))
					{
						MessageBox.Show("Not all settings could be imported.", "Enso+", MessageBoxButtons.OK);
					}
				} 
			}
		}

		private static int CompareVersions(string a, string b)
		{
			
				try
				{
					List<string> aList = a.Split('.').ToList();
					for (int i = aList.Count - 1; i >= 0; i--)
					{
						int aInt = int.Parse(aList[i]);
						aList[i] = aInt.ToString().PadRight(3);
					}
					a = string.Empty;
					foreach (string aItem in aList)
					{
						a = a + aItem + ".";
					}
					a = a.TrimEnd('.');
				}
				catch (Exception e)
				{
					Common.Logging.AddErrorLog("CompareVersions a:"+a);
					Common.Logging.AddExceptionLog(e);
					a = "0";
				}

			
				try
				{
					List<string> bList = b.Split('.').ToList();
					for (int i = bList.Count - 1; i >= 0; i--)
					{
						int bInt = int.Parse(bList[i]);
						bList[i] = bInt.ToString().PadRight(3);
					}
					b = string.Empty;
					foreach (string bItem in bList)
					{
						b = b + bItem + ".";
					}
					b = b.TrimEnd('.');
				}
				catch (Exception e)
				{
					Common.Logging.AddErrorLog("CompareVersions b:"+b);
					Common.Logging.AddExceptionLog(e);
					b = "0";
				}

			int r = string.Compare(a, b);
			return r;
		}
	}
}
