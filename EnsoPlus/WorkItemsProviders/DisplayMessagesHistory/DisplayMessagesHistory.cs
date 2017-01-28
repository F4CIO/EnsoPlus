using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Common;
using EnsoPlus.Entities;
using Extension;

namespace EnsoPlus.WorkItemsProviders.DisplayMessagesHistory
{
    class DisplayMessagesHistory : IWorkItemsProvider
    {      
        #region IParameterTypeProvider Members

        public bool SuggestionsCachingAllowed()
        {
            return false;
        }

        public Dictionary<string, IWorkItem> GetSuggestions()
        {
            Dictionary<string, IWorkItem> suggestions = new Dictionary<string, IWorkItem>();

            foreach (EnsoMessage ensoMessage in MessagesHandler.GetAllFromHistory())
            {
                DisplayMessage displayMessage = new DisplayMessage(ensoMessage.Text, ensoMessage.Subtext);
                displayMessage.provider = this;
                suggestions[displayMessage.GetCaption()] = displayMessage;
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
