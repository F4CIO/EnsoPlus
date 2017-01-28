using System;
using System.Collections.Generic;
using System.Text;

using Extension;
using EnsoPlus;
using EnsoPlus.Entities;
using Common;
using EnsoPlus.WorkItemsProviders.CommandsHistory;

namespace EnsoPlus.CommandsProviders.CommandsManager
{
    class CommandsManager : ICommandsProvider
    {
        #region ICommandsProvider Members

        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            commands.Add(new Command("remove", "[item]", "Removes work item", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments(/*Caption:*/"remove", null, /*Offer all suggestions:*/true, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/true,/*Case sensitive:*/false),
                /*Suggestions sources:*/
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Shortcuts.Shortcuts),
                typeof(WorkItemsProviders.Shortcuts.ShortcutsLists),
                typeof(WorkItemsProviders.Macros.Macros),
                typeof(WorkItemsProviders.ShortcutTemplates.ShortcutTemplates),
                typeof(WorkItemsProviders.BookmarkBrowser.BookmarkBrowser)
                ));

            commands.Add(new Command("forget", "[item]", "Removes work item", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments(/*Caption:*/"forgetf", null, /*Offer all suggestions:*/true, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/true,/*Case sensitive:*/false),
                            /*Suggestions sources:*/
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Shortcuts.Shortcuts),
                typeof(WorkItemsProviders.Macros.Macros),
                typeof(WorkItemsProviders.ShortcutTemplates.ShortcutTemplates),
                typeof(WorkItemsProviders.BookmarkBrowser.BookmarkBrowser)
                ));

            commands.Add(new Command("repeat", " ", "Executes last command", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false, /*Can use file selection as parameter:*/false,
                new ParameterInputArguments()
                ));

            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("CommandsManager: Executing command '{0}' ...", command.Name));

            if (command.Name == "repeat")
            {
                Logging.AddActionLog(string.Format("CommandsManager: Repeating last command: {0}", CommandsHistory.GetLast().caption));
                CommandsHistory.GetLast().Execute(service);
            }
            else
                if (command.Name == "remove" || command.Name=="forget")
                {                                        
                    command.parametersOnExecute[0].GetProvider().Remove(command.parametersOnExecute[0]);  
                    Logging.AddActionLog(string.Format("CommandsManager: Command '{0}' removed.", command.parametersOnExecute[0].GetCaption()));
                    MessagesHandler.Display( string.Format("{0} removed.", command.parametersOnExecute[0].GetCaption()));
                }
                else
                    if (command.Name == "command name" && command.Postfix == "postfix [item] [item2]")
                    {
                        MessagesHandler.Display( string.Format("Executing {0} ...", command.Name));

                    }
                    else
                    {
                        throw new ApplicationException(string.Format("HistoryManager: Command not found. Command: {0} {1}", command.Name, command.Postfix));
                    }
        }        
        
        public void ProcessingBeforeParameterInput(Command command, ref bool cancel)
        {
        }

        #endregion
    }
}
