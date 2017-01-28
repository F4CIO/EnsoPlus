using System;
using System.Collections.Generic;
using System.Text;

using Extension;
using EnsoPlus;
using EnsoPlus.Entities;
using Common;

namespace EnsoPlus.CommandsProviders.MessagesDisplayer
{
    class MessagesDisplayer : ICommandsProvider
    {      
        #region ICommandsProvider Members

        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            //commands.Add(new Command("command name", "postfix [item] [item2]", "This is description", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
            //    new ParameterInputArguments(/*Caption:*/"ProviderSample", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
            //    /*Suggestions sources:*/
            //    typeof(ParameterTypeProviders.MemorizedData.MemorizedData),
            //    typeof(ParameterTypeProviders.Shortcuts.Shortcuts),
            //    typeof(ParameterTypeProviders.Contacts.Contacts),
            //    typeof(ParameterTypeProviders.ShortcutTemplates.ShortcutTemplates)
            //    ));

            commands.Add(new Command("display-last-message", " ", "Shows last displayed message", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false, /*Can use file selection as parameter:*/false,
                new ParameterInputArguments()
                ));

            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("MessagesDisplayer: Executing command '{0}' ...", command.Name));

            if (command.Name == "display-last-message")
            {
                MessagesHandler.Display(MessagesHandler.GetLastFromHistory(),false);
                Logging.AddActionLog(string.Format("MessagesDisplayer: Last message '{0}' displayed.", MessagesHandler.GetLastFromHistory()));
            }
            else
                //if (command.Name == "command name" && command.Postfix == "postfix [item] [item2]")
                //{
                //    MessagesHandler.Display( string.Format("Executing {0} ...", command.Name));

                //}
                //else
                //    if (command.Name == "command name" && command.Postfix == "postfix [item] [item2]")
                //    {
                //        MessagesHandler.Display( string.Format("Executing {0} ...", command.Name));

                //    }
                //    else
                    {
                        throw new ApplicationException(string.Format("MessagesDisplayer: Command not found. Command: {0} {1}", command.Name, command.Postfix));
                    }
        }        
        
        public void ProcessingBeforeParameterInput(Command command, ref bool cancel)
        {
        }


        #endregion
    }
}
