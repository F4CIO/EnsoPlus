using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IWshRuntimeLibrary;

namespace LnkFileReader
{
    public class Helper
    {
        /// <summary>
        /// Reads .lnk file and returns target property.
        /// </summary>
        /// <param name="lnkPath"></param>
        /// <returns></returns>
        public static string GetLnkTarget(string lnkPath)
        {           

            string result = null;

            //read as local shortcut
            WshShell shell = new WshShell();
            IWshShortcut wshShortcut = (IWshShortcut)shell.CreateShortcut(lnkPath);
            result = wshShortcut.TargetPath;

            if (string.IsNullOrEmpty(result) && System.IO.File.Exists(lnkPath))
            {//read as internet shortcut                
                string fileContents = System.IO.File.ReadAllText(lnkPath, Encoding.Unicode);
                int targetStartIndex = fileContents.IndexOf("http://", 0, StringComparison.InvariantCultureIgnoreCase);
                if (targetStartIndex == -1)
                {
                    targetStartIndex = fileContents.IndexOf("https://", 0, StringComparison.InvariantCultureIgnoreCase);
                }
                if (targetStartIndex == -1)
                {
                    targetStartIndex = fileContents.IndexOf("ftp://", 0, StringComparison.InvariantCultureIgnoreCase);
                }
                if (targetStartIndex == -1)
                {
                    targetStartIndex = fileContents.IndexOf("ftps://", 0, StringComparison.InvariantCultureIgnoreCase);
                }
                if (targetStartIndex != -1)
                {
                    result = fileContents.Substring(targetStartIndex).Trim();
                    result = result.Replace("\0", string.Empty);
                }
            }
            return result;
        }
    }
}
