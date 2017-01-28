using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Common;
using EnsoPlus.Entities;

namespace EnsoPlus.WorkItemsProviders.Shortcuts
{
    class ShortcutsLists: IWorkItemsProvider
    {
        public static List<Entities.WorkItems.Shortcuts> GetAll()
        {
            List<Entities.WorkItems.Shortcuts> shortcutsLists = new List<Entities.WorkItems.Shortcuts>();
            string shortcutsFolder = Settings.Current.EnsoLearnAsOpenCommandsFolder;
            CraftSynth.BuildingBlocks.IO.FileSystem.CreateFolderIfItDoesNotExist(shortcutsFolder);

            List<string> shortcutsListsFilesPaths = CraftSynth.BuildingBlocks.IO.FileSystem.GetFilePaths(shortcutsFolder, false ,"*." + Entities.WorkItems.Shortcuts.extension);
            foreach (var shortcutListFilePath in shortcutsListsFilesPaths)
            {
                Entities.WorkItems.Shortcuts shortcutsList = new Entities.WorkItems.Shortcuts(shortcutListFilePath);
                shortcutsLists.Add(shortcutsList);
            }
            return shortcutsLists;
        }

        #region IParameterTypeProvider Members

        public bool SuggestionsCachingAllowed()
        {
            return true;
        }

        public Dictionary<string, IWorkItem> GetSuggestions()
        {
            Dictionary<string, IWorkItem> suggestions = new Dictionary<string, IWorkItem>();

            foreach (Entities.WorkItems.Shortcuts item in GetAll())
            {
                item.provider = this;
                suggestions.Add(item.GetCaption(), item);
            }

            return suggestions;
        }

        public IWorkItem GetParameterFromSelectedSuggestion(IWorkItem selectedSuggestion)
        {
            return selectedSuggestion;
        }

        public void Remove(IWorkItem workItemToRemove)
        {
            Entities.WorkItems.Shortcuts shortcutsListToRemove = workItemToRemove as Entities.WorkItems.Shortcuts;
            string retiredFilePath = Path.Combine(Settings.Current.EnsoLearnAsOpenCommandsRetiredFilesFolder, Path.GetFileName(shortcutsListToRemove.shortcutsFilePath));
            System.IO.File.Move(shortcutsListToRemove.shortcutsFilePath, retiredFilePath);
            SuggestionsCache.DropCache(this.GetType());         
        }

        #endregion
    }
    
}
