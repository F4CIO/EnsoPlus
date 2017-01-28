using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Common;
using EnsoPlus.Entities;

namespace EnsoPlus.WorkItemsProviders.ProviderSample
{
    class ProviderSample : IWorkItemsProvider
    {
        public static List<string> GetAll()
        {
            List<string> items = new List<string>();





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

            foreach (string item in GetAll())
            {
                //.provider = this;
                suggestions.Add(item, new StringWorkItem(item));
            }

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
