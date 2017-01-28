using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Common;
using EnsoPlus.Entities;

namespace EnsoPlus.WorkItemsProviders.ReflectionData
{
    class ReflectionData : IWorkItemsProvider
    {     

        #region IParameterTypeProvider Members

        public bool SuggestionsCachingAllowed()
        {
            return false;
        }

        public Dictionary<string, IWorkItem> GetSuggestions()
        {
            Dictionary<string, IWorkItem> suggestions = new Dictionary<string, IWorkItem>();

            KeyValue<string, string> settingsFile = new KeyValue<string, string>();
            settingsFile.provider = this;
            settingsFile.key = "Settings file";
            settingsFile.value = Path.GetFullPath( Settings.FilePath);
            suggestions.Add(settingsFile.key, settingsFile);

            KeyValue<string, string> exeFile = new KeyValue<string, string>();
            exeFile.provider = this;
            exeFile.key = "Application exe file";
            exeFile.value = CraftSynth.BuildingBlocks.UI.Console.ApplicationPhysicalExeFilePath;
            suggestions.Add(exeFile.key, exeFile);

            KeyValue<string, string> exceptionsFile = new KeyValue<string, string>();
            exceptionsFile.provider = this;
            exceptionsFile.key = "Exceptions log file";
            exceptionsFile.value = Path.GetFullPath(Settings.Current.ExceptionsLogFilePath);
            suggestions.Add(exceptionsFile.key, exceptionsFile);

            KeyValue<string, string> actionsFile = new KeyValue<string, string>();
            actionsFile.provider = this;
            actionsFile.key = "Actions log file";
            actionsFile.value = Path.GetFullPath(Settings.Current.ActionsLogFilePath);
            suggestions.Add(actionsFile.key, actionsFile);

            KeyValue<string, string> errorsFile = new KeyValue<string, string>();
            errorsFile.provider = this;
            errorsFile.key = "Errors log file";
            errorsFile.value = Path.GetFullPath(Settings.Current.ErrorsLogFilePath);
            suggestions.Add(errorsFile.key, errorsFile);
           
            return suggestions;
        }

        public IWorkItem GetParameterFromSelectedSuggestion(IWorkItem selectedSuggestion)
        {
            return selectedSuggestion;
        }

        public void Remove(IWorkItem workItemToRemove)
        {
            throw new ApplicationException("Operation not supported.");
        }

        #endregion
    }
}
