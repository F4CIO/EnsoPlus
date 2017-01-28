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
    class StartMenuShortcuts : IWorkItemsProvider
    {
        private List<Shortcut> GetShortcuts()
        {
            List<Shortcut> shortcuts = new List<Shortcut>();
	       
			string shortcutsFolder = CraftSynth.BuildingBlocks.WindowsNT.Misc.GetStartMenuFolderPathForAllUsers();
            List<string> shortcutsFilesPaths = CraftSynth.BuildingBlocks.IO.FileSystem.GetFilePaths(shortcutsFolder, true);
            foreach (var shortcutFilePath in shortcutsFilesPaths)
            {
                Shortcut shortcut = Shortcuts.ReadShortcutFile(shortcutFilePath);
                if (shortcut != null) shortcuts.Add(shortcut);
            }
			
			shortcutsFolder = CraftSynth.BuildingBlocks.WindowsNT.Misc.GetStartMenuFolderPathForCurrentUser();
            shortcutsFilesPaths = CraftSynth.BuildingBlocks.IO.FileSystem.GetFilePaths(shortcutsFolder, true);
            foreach (var shortcutFilePath in shortcutsFilesPaths)
            {
	            if (!shortcuts.Exists(s => string.Compare(s.shortcutFilePath, shortcutFilePath, StringComparison.OrdinalIgnoreCase) == 0))
	            {
		            Shortcut shortcut = Shortcuts.ReadShortcutFile(shortcutFilePath);
		            if (shortcut != null) shortcuts.Add(shortcut);
	            }
            }

            return shortcuts;
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
        }

        #endregion
    }
}
