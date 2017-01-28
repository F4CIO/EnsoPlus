using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;
using System.IO;

using Common;
using EnsoPlus.Entities;
using Extension;

namespace EnsoPlus.WorkItemsProviders.CommandsHistory
{
    class CommandsHistory : IWorkItemsProvider
    {
        private static List<Command> _history;
        private static List<Command> history
        {
            get
            {
                if (_history == null)
                {
                    _history = new List<Command>();
                }
                return _history;
            }
            set
            {
                _history = value;
            }
        }

        private static List<IWorkItem> workItemHistory = new List<IWorkItem>();

        public static List<Command> GetAll()
        {
            List<Command> result = new List<Command>();
			if (history != null)
			{
				foreach (Command command in history)
				{
					result.Add(command.GetClone());
				}
			}
	        return result;
        }

        public static Command GetLast()
        {
            if (history.Count == 0)
            {
                Command dummyCommand = new Command("display", "[item name]", "Shows item on screen", null, EnsoPostfixType.Arbitrary, new CommandsProviders.Memorizer.Memorizer(), /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments("display", null, false, false, string.Empty, true, false),
                typeof( WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Contacts.Contacts),
                typeof(WorkItemsProviders.Shortcuts.Shortcuts));

                dummyCommand.parametersOnExecute.Add(new StringWorkItem("No commands in history."));

                return dummyCommand;
            }
            else
            {
                return history[history.Count - 1];
            }
        }

        public static IWorkItem GetLastWorkItem()
        {
            IWorkItem lastWorkItem = new StringWorkItem("No work items in history");

            int i=history.Count-1;
            while(i>=0)
            {
                if (history[i].parametersOnExecute.Count > 0)
                {
                    string lastParameterOnExecuteValueAsText = history[i].parametersOnExecute[history[i].parametersOnExecute.Count - 1].GetValueAsText();
                    if ( lastParameterOnExecuteValueAsText != "last")
                    {
                        lastWorkItem = history[i].parametersOnExecute[history[i].parametersOnExecute.Count - 1];
                        break;
                    }                    
                }
                i--;                
            }

            return lastWorkItem;
        }

        public static void Add(Command command)
        {
            history.Add(command);
            foreach(IWorkItem workItem in command.parametersOnExecute)
            {
                
                workItemHistory.Add(workItem);
            }
        }

        #region IParameterTypeProvider Members

        public bool SuggestionsCachingAllowed()
        {
            return false;
        }

        public Dictionary<string, IWorkItem> GetSuggestions()
        {
            Dictionary<string, IWorkItem> suggestions = new Dictionary<string, IWorkItem>();

	        string c = null;
            foreach (Command command in GetAll())
            {
	            c = command.caption;
	            int i = 0;
	            while (suggestions.ContainsKey(c))
	            {
		            i++;
		            c = string.Format("{0} ({1})", command.caption, i);

		            if (i > 1000)
		            {
			            throw new Exception("To many same-name suggestions.");
		            }
	            }
                suggestions.Add(c, command as IWorkItem);
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
