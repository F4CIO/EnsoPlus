using System;
using System.Collections.Generic;
using System.Text;

using Extension;
using EnsoPlus;
using EnsoPlus.Entities;
using Common;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace EnsoPlus.CommandsProviders.Misc
{
    class Misc : ICommandsProvider
    {
        #region ICommandsProvider Members

        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            //commands.Add(new Command("edit settings", " ", "Opens Enso+ settings file in notepad", null, EnsoPostfixType.Arbitrary, this, /*Can use selection as parameter:*/false,
            //    new ParameterInputArguments(/*Caption:*/"ProviderSample", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
            //    /*Suggestions sources:*/
            //    typeof(ParameterTypeProviders.MemorizedData.MemorizedData),
            //    typeof(ParameterTypeProviders.Shortcuts.Shortcuts),
            //    typeof(ParameterTypeProviders.Contacts.Contacts),
            //    typeof(ParameterTypeProviders.ShortcutTemplates.ShortcutTemplates)
            //    ));

            //commands.Add(new Command("command name", "postfix [item] [item2]", "This is description", null, EnsoPostfixType.Arbitrary, this, /*Can use selection as parameter:*/true,
            //    new ParameterInputArguments(/*Caption:*/"ProviderSample", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
            //    /*Suggestions sources:*/
            //    typeof(ParameterTypeProviders.MemorizedData.MemorizedData),
            //    typeof(ParameterTypeProviders.Shortcuts.Shortcuts),
            //    typeof(ParameterTypeProviders.Contacts.Contacts),
            //    typeof(ParameterTypeProviders.ShortcutTemplates.ShortcutTemplates)
            //    ));

            //commands.Add(new Command("command name", "postfix [item] [item2]", "This is description", null, EnsoPostfixType.Arbitrary, this, /*Can use selection as parameter:*/true,
            //    new ParameterInputArguments(/*Caption:*/"ProviderSample", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
            //    /*Suggestions sources:*/
            //    typeof(ParameterTypeProviders.MemorizedData.MemorizedData),
            //    typeof(ParameterTypeProviders.Shortcuts.Shortcuts),
            //    typeof(ParameterTypeProviders.Contacts.Contacts),
            //    typeof(ParameterTypeProviders.ShortcutTemplates.ShortcutTemplates)
            //    ));



            commands.Add(new Command("edit general settings", " ", "Opens Enso+ settings file in notepad", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments()
                ));

            commands.Add(new Command("edit selection listener settings", " ", "Opens Enso+ Selection Listener settings file in notepad", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments()
                ));

            commands.Add(new Command("exit Enso+", " ", "Closes Enso+", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments()
                ));

            commands.Add(new Command("close Enso+", " ", "Closes Enso+", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments()
                ));    
			
			commands.Add(new Command("caps lock on", " ", "Turns on caps lock key", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments()
                ));
			commands.Add(new Command("caps lock off", " ", "Turns off caps lock key", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments()
                ));

            commands.Add(new Command("clear cached suggestions", " ", "Clears used suggestion lists from memory", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments()
                ));

            commands.Add(new Command("restart Enso+", " ", "Restarts Enso+ application", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments()
                ));
            //commands.Add(new Command("commandWithNoPostfix", " ", "This is description", null, EnsoPostfixType.None, this, /*Can use selection as parameter:*/false,
            //     new ParameterInputArguments()
            //     ));

            commands.Add(new Command("open in notepad", "[item]", "Shows item in notepad", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
               new ParameterInputArguments("item", null, false, false, string.Empty, false, false),
               typeof(WorkItemsProviders.MemorizedData.MemorizedData),
               typeof(WorkItemsProviders.Contacts.Contacts),
               typeof(WorkItemsProviders.Shortcuts.Shortcuts),
               typeof(WorkItemsProviders.CallerHistory.CallerHistory),
               typeof(WorkItemsProviders.ReflectionData.ReflectionData),
               typeof(WorkItemsProviders.BookmarkBrowser.BookmarkBrowser),
               typeof(WorkItemsProviders.Clipboard.ClipboardText),
               typeof(WorkItemsProviders.Misc.AutoGeneratedText)));
           
            commands.Add(new Command("preview in notepad", "[item]", "Shows item in notepad", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
               new ParameterInputArguments("item", null, false, false, string.Empty, false, false),
               typeof(WorkItemsProviders.MemorizedData.MemorizedData),
               typeof(WorkItemsProviders.Contacts.Contacts),
               typeof(WorkItemsProviders.Shortcuts.Shortcuts),
               typeof(WorkItemsProviders.CallerHistory.CallerHistory),
               typeof(WorkItemsProviders.ReflectionData.ReflectionData),
               typeof(WorkItemsProviders.BookmarkBrowser.BookmarkBrowser),
               typeof(WorkItemsProviders.Clipboard.ClipboardText),
               typeof(WorkItemsProviders.Misc.AutoGeneratedText)));
            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("Misc: Executing command '{0}' ...", command.Name));

            if (command.Name == "edit general settings")
            {
                Logging.AddActionLog(string.Format("Misc: Opening settings file '{0}' ...",Settings.FilePath));
                MessagesHandler.Display( string.Format("Opening {0} ...", Settings.FilePath));

                Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Arguments = string.Format("\"{0}\"", Settings.FilePath);
                process.StartInfo.FileName = "Notepad";
                process.Start();
                CraftSynth.BuildingBlocks.WindowsNT.Misc.SetForegroundWindow("EnsoPlusSettings.xml - Notepad");
                Logging.AddActionLog("Misc: Settings file opened.");
            }
            else
                if (command.Name == "edit selection listener settings")
                {
                    Logging.AddActionLog(string.Format("Misc: Opening settings file '{0}' ...", SelectionListener.Settings.FilePath));
                    MessagesHandler.Display( string.Format("Opening {0} ...", SelectionListener.Settings.FilePath));

                    Process process = new Process();
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.Arguments = string.Format("\"{0}\"", SelectionListener.Settings.FilePath);
                    process.StartInfo.FileName = "Notepad";
                    process.Start();
                    CraftSynth.BuildingBlocks.WindowsNT.Misc.SetForegroundWindow("SelectionListener.ini - Notepad");
                    Logging.AddActionLog("Misc: Settings file opened.");
                }
            else
                if (command.Name == "exit Enso+" || command.Name =="close Enso+")
                {
                    Exit();
                }
                else 
					if (command.Name == "caps lock on")
					{
						Logging.AddActionLog("Misc: Turning caps lock on ...");
						//MessagesHandler.Display( string.Format("Exiting Enso+ ..."));
						Program.cornerLauncher.TurnOnCapsLock();
					}
					else
					if (command.Name == "caps lock off")
					{
						Logging.AddActionLog("Misc: Turning caps lock off ...");
						//MessagesHandler.Display( string.Format("Exiting Enso+ ..."));
						Program.cornerLauncher.TurnOffCapsLock();
					}
					else
                        if (command.Name == "clear cached suggestions")
                        {
                            SuggestionsCache.DropAllCache();
                            Logging.AddActionLog("Misc: Cache cleared.");
                            MessagesHandler.Display( "Cache cleared.");
                        }else
                                if (command.Name == "restart Enso+")
                                {
                                    Restart();
                                }
                                else
                                    if (command.Name == "open in notepad" || command.Name == "preview in notepad")
                                    {
                                        //MessagesHandler.Display( string.Format("Executing {0} ...", command.Name));
                                        CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenTextInNotepad(Common.Helper.GetEnsoTempraryFilePath("txt"), command.parametersOnExecute[0].GetValueAsText());
                                    }
                                    else
                                    //if (command.Name == "command name" && command.Postfix == "postfix [item] [item2]")
                                    //{
                                    //    MessagesHandler.Display( string.Format("Executing {0} ...", command.Name));

                                    //}
                                    //else
                                    {
                                        throw new ApplicationException(string.Format("Misc: Command not found. Command: {0} {1}", command.Name, command.Postfix));
                                    }
        }

	    public static void Exit()
	    {
		    Logging.AddActionLog("Misc: Exiting Enso+ ...");
		    MessagesHandler.Display(string.Format("Exiting Enso+ ..."));

		    Process ensoPlusGuardProcess = Helper.TryGetEnsoPlusGuardProcess();
		    if (ensoPlusGuardProcess != null)
		    {
			    Helper.KillEnsoPlusGuardIfExist();
		    }
		    Application.Exit();

		    MessagesHandler.Display(string.Format("Exited. Thanks for using."));
	    }

	    public static void Restart()
	    {
		    Logging.AddActionLog("Misc: Restarting Enso+ ....");
		    MessagesHandler.Display("Restarting ...");
		    CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenFile(Path.Combine(CraftSynth.BuildingBlocks.UI.Console.ApplicationPhysicalPath, "EnsoPlus.Restarter.exe"));
	    }

	    public void ProcessingBeforeParameterInput(Command command, ref bool cancel)
        {
           
        }

        #endregion
    }
}
