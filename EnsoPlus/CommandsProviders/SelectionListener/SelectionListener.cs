using System;
using System.Collections.Generic;
using System.Text;

using Extension;
using EnsoPlus.Entities;
using Common;
using System.Windows.Forms;

namespace EnsoPlus.CommandsProviders.SelectionListenerCommandsProvider
{
    class SelectionListenerCommandsProvider : ICommandsProvider
    {
        #region ICommandsProvider Members

        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            //commands.Add(new Command("command name", "postfix [item] [item2]", "This is description", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
            //    new ParameterInputArguments(/*Caption:*/"ProviderSample", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
            //    /*Suggestions sources:*/
            //    typeof(WorkItemsProviders.MemorizedData.MemorizedData),
            //    typeof(WorkItemsProviders.Shortcuts.Shortcuts),
            //    typeof(WorkItemsProviders.Contacts.Contacts),
            //    typeof(WorkItemsProviders.ShortcutTemplates.ShortcutTemplates)
            //    ));

            commands.Add(new Command("watch selections", " ", "Track and memorises and shows all text you select or copy so you can use it later.", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false, /*Can use file selection as parameter:*/false,
                new ParameterInputArguments()
                ));

            commands.Add(new Command("stop watching of selections", " ", "Closes selection listener.", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false, /*Can use file selection as parameter:*/false,
                new ParameterInputArguments()
            ));

            commands.Add(new Command("assign key combination for paste operation", " ", "Specify key combination with wich you will paste highlighted item from selection listener.", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false, /*Can use file selection as parameter:*/false,
                new ParameterInputArguments()
                ));

            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("ProviderSample: Executing command '{0}' ...", command.Name));
            if (command.Name == "watch selections")
            {                   
                SelectionListener.SelectionListener.Current.Start(service, command);                
            }
            else
                if (command.Name == "stop watching of selections")
                {
                    //MessagesHandler.Display( string.Format("Closing selection listener ...", command.Name));
                    SelectionListener.SelectionListener.Current.Stop();
                } else
                    if (command.Name == "assign key combination for paste operation")
                {
                    //MessagesHandler.Display( string.Format("Closing selection listener ...", command.Name));
                    SelectionListener.SelectionListener.Current.AssignPasteKeyCombination(service);
                }
                //else
                //    if (command.Name == "command name" && command.Postfix == "postfix [item] [item2]")
                //    {
                //        MessagesHandler.Display( string.Format("Executing {0} ...", command.Name));

                //    }
                    else
                    {
                        throw new ApplicationException(string.Format("ProviderSample: Command not found. Command: {0} {1}", command.Name, command.Postfix));
                    }
        }

        public void ProcessingBeforeParameterInput(Command command, ref bool cancel)
        {
        }

        #endregion
    }
}
