using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Common;
using EnsoPlus.Entities;
using System.Diagnostics;

namespace EnsoPlus.WorkItemsProviders.Processes
{
    class Processes : IWorkItemsProvider
    {
        public List<ProcessWorkItem> GetAll()
        {
            List<ProcessWorkItem> items = new List<ProcessWorkItem>();

            Process[] allProcesses = Process.GetProcesses();
            foreach(Process p in allProcesses)
            {              
                ProcessWorkItem processWorkItem = new ProcessWorkItem();
                processWorkItem.procces = p;
                long reminder = 0;
                processWorkItem.caption = string.Format("{0} ({1}, {2}kb+{3}kb)", p.ProcessName, p.Id, Math.DivRem(p.WorkingSet64, 1024,out reminder), Math.DivRem(p.VirtualMemorySize64,1024,out reminder));
                
                items.Add(processWorkItem);
            }

            return items;
        }

        #region IParameterTypeProvider Members

        public bool SuggestionsCachingAllowed()
        {
            return false;
        }

        public Dictionary<string, IWorkItem> GetSuggestions()
        {
            Dictionary<string, IWorkItem> suggestions = new Dictionary<string, IWorkItem>();

            foreach (ProcessWorkItem proocessWorkItem in GetAll())
            {
                proocessWorkItem.provider = this;
                suggestions.Add(proocessWorkItem.caption, proocessWorkItem);
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
