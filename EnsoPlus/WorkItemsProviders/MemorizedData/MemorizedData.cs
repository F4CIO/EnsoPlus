using System;
using System.Collections.Generic;
using System.Text;

using EnsoPlus.Entities;
using Common;
using System.IO;

namespace EnsoPlus.WorkItemsProviders.MemorizedData
{
    class MemorizedData:IWorkItemsProvider
    {  
        #region IParameterTypeProvider Members

        public bool SuggestionsCachingAllowed()
        {
            return true;
        }

        Dictionary<string, IWorkItem> IWorkItemsProvider.GetSuggestions()
        {
            Dictionary<string, IWorkItem> items = new Dictionary<string, IWorkItem>();

            CraftSynth.BuildingBlocks.IO.FileSystem.CreateFolderIfItDoesNotExist(Settings.Current.MemorizerDataFolder);
            List<String> filePaths = CraftSynth.BuildingBlocks.IO.FileSystem.GetFilePaths(Settings.Current.MemorizerDataFolder);
            Dictionary<string, string> fileNames = new Dictionary<string, string>();
            foreach (var filePath in filePaths)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    int i = 0;
                    while (fileNames.ContainsKey(fileName)) { fileName += string.Format("({0})", i); i++; };

                    MemorizedString memorizedString = new MemorizedString(fileName, filePath, null);
                    memorizedString.provider = this;
                    items.Add(fileName, memorizedString);
                }
                catch (Exception exception)
                {
					Common.Logging.AddExceptionLog(exception);
                }
            }

            return items;
        }

        public IWorkItem GetParameterFromSelectedSuggestion(IWorkItem selectedSuggestion)
        {
            MemorizedString memorizedString = selectedSuggestion as MemorizedString;

            if (memorizedString.data == null)
            {
                try
                {
                    memorizedString.data = File.ReadAllText(memorizedString.filePath);                    
                }
                catch (Exception exception)
                {
                    memorizedString = null;
	                throw exception;
                }
            }

            return memorizedString;
        }

        public void Remove(IWorkItem workItemToRemove)
        {
            MemorizedString memorizedString = workItemToRemove as MemorizedString;
            System.IO.File.Delete(memorizedString.filePath);
            SuggestionsCache.DropCache(this.GetType());           
        }

        #endregion
    }
}
