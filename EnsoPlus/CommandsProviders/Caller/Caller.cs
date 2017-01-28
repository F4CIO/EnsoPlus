using System;
using System.Collections.Generic;
using System.Text;

using EnsoPlus.Entities;
using Extension;
using Common;

namespace EnsoPlus.CommandsProviders.Caller
{
    class Caller: ICommandsProvider
    {
        #region ICommandsProvider Members

        public List<Command> GetCommands()        
        {
            List<Command> commands = new List<Command>();

            commands.Add(new Command("call", "[wich number]", "Calls selected or specified number using dial-up modem", null, EnsoPostfixType.Arbitrary, this,/*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments("call", null, false, false, "", false, false),
                typeof( WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Contacts.Contacts),
                typeof(WorkItemsProviders.CallerHistory.CallerHistory),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)));

            commands.Add(new Command("recall", " ", "Repeat last call", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
            new ParameterInputArguments()
            ));

            commands.Add(new Command("dial", "[wich number]", "Calls selected or specified number using dial-up modem", null, EnsoPostfixType.Arbitrary, this,/*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments("call", null, false, false, "", false, false),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText),
                typeof(WorkItemsProviders.Contacts.Contacts),
                typeof(WorkItemsProviders.CallerHistory.CallerHistory)));

            commands.Add(new Command("redial", " ", "Repeat last call", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments()
                ));

            commands.Add(new Command("later", " ", "Drops current call", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments()));
            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
			//Exception ee = new Exception("inner one");
			//Exception ee2 = new Exception("inner two",ee);
			//Exception ee3 = new Exception("test ex",ee2);
			//throw ee3;

            Logging.AddActionLog(string.Format("Caller: Executing command '{0}' ...", command.Name));

            if (command.Name == "call" || command.Name == "dial")
            {
                Dialer.service = service;
                if (command.parametersOnExecute[0] is StringWorkItem)
                {
                    Dialer.contactToDial = new ContactUnknown(command.parametersOnExecute[0].GetValueAsText());
                }
                else if (command.parametersOnExecute[0] is Contact)
                {
                    Dialer.contactToDial = (command.parametersOnExecute[0] as Contact);
                }

                WorkItemsProviders.CallerHistory.CallerHistory.Add(Dialer.contactToDial);
                Dialer.Dial();
            }
            else
                if (command.Name == "recall" || command.Name == "redial")
                {
                    Dialer.service = service;
                    Dialer.Redial();
                }
                else
                    if (command.Name == "later")
                    {
                        Dialer.service = service;
                        Dialer.Cancel();
                    }
                    else
                    {
                        throw new ApplicationException(string.Format("Caller: Command not found. Command: {0} {1}", command.Name, command.Postfix));
                    }
            
        }        
        
        public void ProcessingBeforeParameterInput(Command command, ref bool cancel)
        {           
        }

        #endregion


    }
}
