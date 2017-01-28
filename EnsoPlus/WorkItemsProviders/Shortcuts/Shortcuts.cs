using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EnsoPlus.Entities;
using Common;
using System.IO;
using Extension;
using System.Diagnostics;


namespace EnsoPlus.WorkItemsProviders.Shortcuts
{
    class Shortcuts : IWorkItemsProvider
    {
        private List<Shortcut> GetShortcuts()
        {
            List<Shortcut> shortcuts = new List<Shortcut>();
            string shortcutsFolder = Settings.Current.EnsoLearnAsOpenCommandsFolder;
            CraftSynth.BuildingBlocks.IO.FileSystem.CreateFolderIfItDoesNotExist(shortcutsFolder);
            
            List<string> shortcutsFilesPaths = CraftSynth.BuildingBlocks.IO.FileSystem.GetFilePaths(shortcutsFolder);
            foreach (var shortcutFilePath in shortcutsFilesPaths)
            {
                Shortcut shortcut = Shortcuts.ReadShortcutFile(shortcutFilePath);
                if (shortcut != null) shortcuts.Add(shortcut);
            }
            return shortcuts;
        }

        public static Shortcut ReadShortcutFile(string shortcutFilePath)
        {
			
			//File.AppendAllText("d:\\sss.txt", shortcutFilePath+"\r\n");
            Shortcut result = null;
			try
			{
				//if(
				//	Path.GetInvalidPathChars().Any(c=>Path.GetDirectoryName(shortcutFilePath).Contains(c)) ||
				//	Path.GetInvalidFileNameChars().Any(c=>Path.GetFileName(shortcutFilePath).Contains(c))
				//	)
				//{
				//	result = null;
				//}
				//else { 
					if (Path.GetExtension(shortcutFilePath).EndsWith("lnk", StringComparison.InvariantCultureIgnoreCase))
					{
						try
						{
							string target = null;

							if (!CraftSynth.BuildingBlocks.WindowsNT.Misc.Is64BitOperatingSystem)
							{//requires Interop.IWshRuntimeLibrary which can not run as x64!!
                        
								IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
								IWshRuntimeLibrary.IWshShortcut wshShortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutFilePath);
								target = wshShortcut.TargetPath;

								if (
									//!shortcut.TargetPath.StartsWith("http", StringComparison.InvariantCultureIgnoreCase) &&
									//!shortcut.TargetPath.StartsWith("ftp", StringComparison.InvariantCultureIgnoreCase)
									!string.IsNullOrEmpty(target) //make sure that target is on file system
									)
								{
									Shortcut shortcut = new Shortcut();
									shortcut.caption = Path.GetFileNameWithoutExtension(shortcutFilePath);
									shortcut.targetPath = target;
									shortcut.shortcutFilePath = shortcutFilePath;
									result = shortcut;
								}
								else
								{
									Shortcut shortcut = new Shortcut();
									shortcut.caption = Path.GetFileNameWithoutExtension(shortcutFilePath);
									shortcut.targetPath = CraftSynth.BuildingBlocks.IO.FileSystem.GetTargetPathFromLnkManualy(shortcutFilePath);
									shortcut.shortcutFilePath = shortcutFilePath;
									result = shortcut;
								}
							}
							else
							{//run separate process as x86 and send it lnk filepath.
								FormLnkFileReaderProxy.Initialize();
								target = FormLnkFileReaderProxy.WaitForLnkTarget(shortcutFilePath);

								if (!string.IsNullOrEmpty(target))
								{
									Shortcut shortcut = new Shortcut();
									shortcut.caption = Path.GetFileNameWithoutExtension(shortcutFilePath);
									shortcut.targetPath = target;
									shortcut.shortcutFilePath = shortcutFilePath;
									result = shortcut;
								}
							}
						}
						catch (Exception exception)
						{
							result = null;
							Logging.AddErrorLog(string.Format("Error occured while reading shortcut '{0}': {1}", shortcutFilePath, exception.Message));
							Common.Logging.AddExceptionLog(exception);
						}
					}
					else if (Path.GetExtension(shortcutFilePath).EndsWith("url", StringComparison.InvariantCultureIgnoreCase))
					{
						string targetPath = null;
						try
						{
							string[] content = System.IO.File.ReadAllLines(shortcutFilePath);
							targetPath = content.Single(l => l.ToUpper().StartsWith("URL=")).Split('=')[1].Trim();

							Shortcut shortcut = new Shortcut();
							shortcut.caption = Path.GetFileNameWithoutExtension(shortcutFilePath);
							shortcut.targetPath = targetPath;
							shortcut.shortcutFilePath = shortcutFilePath;
							result = shortcut;
						}
						catch (Exception exception)
						{
							result = null;
							Logging.AddErrorLog(string.Format("Error occured while reading shortcut '{0}': {1}", shortcutFilePath, exception.Message));
							Common.Logging.AddExceptionLog(exception);
						}
					}
				//}
			}
			catch (Exception exception) {
				result = null;
				Logging.AddErrorLog(string.Format("Error occured while reading shortcut '{0}': {1}", shortcutFilePath, exception.Message));
				Common.Logging.AddExceptionLog(exception);
			}
            return result;
        }


        #region IParameterTypeProvider Members

        public bool SuggestionsCachingAllowed()
        {
            return true;
        }

        public Dictionary<string, IWorkItem> GetSuggestions()
        {
            Dictionary<string, IWorkItem> suggestions = new Dictionary<string, IWorkItem>();

            foreach (Shortcut shortcut in GetShortcuts())
            {
                shortcut.provider = this;
                suggestions[shortcut.caption] = shortcut;
            }

            return suggestions;
        }

        public IWorkItem GetParameterFromSelectedSuggestion(IWorkItem selectedSuggestion)
        {
            return selectedSuggestion;
        }

        public void Remove(IWorkItem workItemToRemove)
        {
            Shortcut shortcutToRemove = workItemToRemove as Shortcut;
            string retiredFilePath = Path.Combine(Settings.Current.EnsoLearnAsOpenCommandsRetiredFilesFolder, Path.GetFileName(shortcutToRemove.shortcutFilePath));
            System.IO.File.Move( shortcutToRemove.shortcutFilePath, retiredFilePath);
            SuggestionsCache.DropCache(this.GetType());           
        }

        #endregion
    }
}
