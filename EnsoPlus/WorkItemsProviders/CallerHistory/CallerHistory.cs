using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Common;
using EnsoPlus.Entities;
using Extension;

namespace EnsoPlus.WorkItemsProviders.CallerHistory
{
    class CallerHistory : IWorkItemsProvider
    {
        private static List<CallerHistoryItem> _history;
        private static List<CallerHistoryItem> history
        {
            get
            {
                if (_history == null)
                {
                    _history = new List<CallerHistoryItem>();
                }
                return _history;
            }
            set
            {
                _history = value;
            }
        }       

        public static List<CallerHistoryItem> GetAll()
        {
            //CallerHistoryItem[] result = new CallerHistoryItem[] { };
            //history.CopyTo(result);

            return history;
        }

        public static CallerHistoryItem GetLast()
        {
            if (history.Count == 0)
            {
                return null;
            }
            else
            {
                return history[history.Count - 1];
            }
        }

        public static void Add(CallerHistoryItem callerHistoryItem)
        {
            history.Add(callerHistoryItem);         
        }

        public static void Add(Contact contact)
        {
            CallerHistoryItem callerHistoryItem = new CallerHistoryItem(contact);
            Add(callerHistoryItem);
        }

        #region IParameterTypeProvider Members

        public bool SuggestionsCachingAllowed()
        {
            return false;
        }

        public Dictionary<string, IWorkItem> GetSuggestions()
        {
            Dictionary<string, IWorkItem> suggestions = new Dictionary<string, IWorkItem>();

            foreach (CallerHistoryItem callerHistoryItem in GetAll())
            {
                callerHistoryItem.provider = this;
                suggestions[callerHistoryItem.GetCaption()] = callerHistoryItem as IWorkItem;
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
