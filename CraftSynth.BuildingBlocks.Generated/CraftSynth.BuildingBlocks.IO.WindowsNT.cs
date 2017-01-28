using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CraftSynth.BuildingBlocks.IO
{
    public class WindowsNT
    {
		/// <summary>
		/// Saves url as windows internet shortcut to specified file path. 
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="url"></param>
		public static void SaveAsInternetShortcut(string filePath, string url)
		{
			string[] contents = new string[] { "[InternetShortcut]", "URL=" + url };
			File.WriteAllLines(filePath, contents);
		}

		/// <summary>
		/// Reads .lnk file and returns target property.
		/// </summary>
		/// <param name="lnkPath"></param>
		/// <returns></returns>
		public static string GetLnkTarget(string lnkPath)
		{
			var shl = new Shell32.Shell();         // Move this to class scope
			lnkPath = System.IO.Path.GetFullPath(lnkPath);
			var dir = shl.NameSpace(System.IO.Path.GetDirectoryName(lnkPath));
			var itm = dir.Items().Item(System.IO.Path.GetFileName(lnkPath));
			var lnk = (Shell32.ShellLinkObject)itm.GetLink;
			return lnk.Path;
		}
    }
}
