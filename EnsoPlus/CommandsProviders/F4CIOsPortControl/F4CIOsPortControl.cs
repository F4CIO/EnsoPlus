using System;
using System.Collections.Generic;
using System.Text;

using Extension;
using EnsoPlus;
using EnsoPlus.Entities;
using Common;

namespace EnsoPlus.CommandsProviders.F4CIOsPortControl
{
    class F4CIOsPortControl : ICommandsProvider
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

            commands.Add(new Command("activate house alarm", " ", "Activates House Alarm Application", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/true, /*Can use file selection as parameter:*/true,
                new ParameterInputArguments()
                ));

            commands.Add(new Command("house alarm restart", " ", "Sets House Alarm Application to Waiting To Exit state", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/true, /*Can use file selection as parameter:*/true,
                new ParameterInputArguments()
                ));

            commands.Add(new Command("deactivate house alarm", " ", "This is description", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/true, /*Can use file selection as parameter:*/true,
                new ParameterInputArguments()
                ));

            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("ProviderSample: Executing command '{0}' ...", command.Name));
            if (command.Name == "activate house alarm")
            {
               // MessagesHandler.Display( string.Format("Executing {0} ...", command.Name));
				CraftSynth.BuildingBlocks.WindowsNT.WindowsMessage.Send("F4CIOsPortControl", 273, 704, 0);
            }
            else
                if (command.Name == "deactivate house alarm")
                {
                    // MessagesHandler.Display( string.Format("Executing {0} ...", command.Name));
					CraftSynth.BuildingBlocks.WindowsNT.WindowsMessage.Send("F4CIOsPortControl", 273, 705, 0);
                }
                else
                    if (command.Name == "house alarm restart")
                    {
                        // MessagesHandler.Display( string.Format("Executing {0} ...", command.Name));
						CraftSynth.BuildingBlocks.WindowsNT.WindowsMessage.Send("F4CIOsPortControl", 273, 703, 0);
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
                        throw new ApplicationException(string.Format("ProviderSample: Command not found. Command: {0} {1}", command.Name, command.Postfix));
                    }
        }

        public void ProcessingBeforeParameterInput(Command command, ref bool cancel)
        {
        }

        #endregion
    }
}
