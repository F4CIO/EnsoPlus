using System;
using System.Collections.Generic;
using System.Text;

namespace EnsoPlus.Entities
{
    public interface IWorkItemsProvider
    {
        bool SuggestionsCachingAllowed();
        Dictionary<string, IWorkItem> GetSuggestions();
        IWorkItem GetParameterFromSelectedSuggestion(IWorkItem selectedSuggestion);
        void Remove(IWorkItem workItemToRemove);
    }
}
