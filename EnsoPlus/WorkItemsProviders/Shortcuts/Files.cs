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
    class FilesWorkItems : IWorkItemsProvider
    {
        private List<FileWorkItem> GetFiles()
        {
			List<FileWorkItem> items = new List<FileWorkItem>();
			string shortcutsFolder = Settings.Current.EnsoLearnAsOpenCommandsFolder;
			CraftSynth.BuildingBlocks.IO.FileSystem.CreateFolderIfItDoesNotExist(shortcutsFolder);

			List<string> filePaths = CraftSynth.BuildingBlocks.IO.FileSystem.GetFilePaths(shortcutsFolder);
			foreach (var fp in filePaths)
			{
				items.Add(new FileWorkItem(fp));
			}
			return items;
        }

        #region IParameterTypeProvider Members

        public bool SuggestionsCachingAllowed()
        {
            return true;
        }

        public Dictionary<string, IWorkItem> GetSuggestions()
        {
            Dictionary<string, IWorkItem> suggestions = new Dictionary<string, IWorkItem>();
			foreach (FileWorkItem f in GetFiles())
            {
                f.provider = this;
                suggestions[f.caption] = f;
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
