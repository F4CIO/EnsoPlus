using System;
using System.Collections.Generic;
using System.Text;
using Common;
using EnsoPlus.Entities;
using Extension;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace EnsoPlus.CommandsProviders.Comparer
{
    public class Comparer : ICommandsProvider
    {
        private object comparerItem1 = null;
        private object comparerItem2 = null;

        #region ICommandsProvider Members

        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            commands.Add(new Command("comparer item 1", "[item 1]", "Memorize selected string/file/folder path or work item to use later by compare command.", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
                new ParameterInputArguments(/*Caption:*/"Compare Item 1", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
                /*Suggestions sources:*/
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Shortcuts.Shortcuts),
                typeof(WorkItemsProviders.Contacts.Contacts),
                typeof(WorkItemsProviders.ShortcutTemplates.ShortcutTemplates),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("compare case sensitive", "[item 2]", "Compare selected string/file/folder path or work item with item marked by 'comparer item 1' command.", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
                new ParameterInputArguments(/*Caption:*/"Compare Item 1", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
                /*Suggestions sources:*/
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Shortcuts.Shortcuts),
                typeof(WorkItemsProviders.Contacts.Contacts),
                typeof(WorkItemsProviders.ShortcutTemplates.ShortcutTemplates),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("compare case insensitive", "[item 2]", "Compare selected string/file/folder path or work item with item marked by 'comparer item 1' command.", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
                new ParameterInputArguments(/*Caption:*/"Compare Item 1", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
                            /*Suggestions sources:*/
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Shortcuts.Shortcuts),
                typeof(WorkItemsProviders.Contacts.Contacts),
                typeof(WorkItemsProviders.ShortcutTemplates.ShortcutTemplates),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("compare in notepad", "[item 2]", "Compare selected string/file/folder path or work item with item marked by 'comparer item 1' command.", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
                new ParameterInputArguments(/*Caption:*/"Compare Item 1", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
                /*Suggestions sources:*/
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Shortcuts.Shortcuts),
                typeof(WorkItemsProviders.Contacts.Contacts),
                typeof(WorkItemsProviders.ShortcutTemplates.ShortcutTemplates),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            //commands.Add(new Command("commandWithNoPostfix", " ", "This is description", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/true, /*Can use file selection as parameter:*/true,
            //    new ParameterInputArguments()
            //    ));

            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("Comparer: Executing command '{0}' ...", command.Name));
            if (command.Name == "comparer item 1" )
            {

                if(command.parametersOnExecute[0] is StringWorkItem)
                {
                    this.comparerItem1 = command.parametersOnExecute[0].GetValueAsText();
                    MessagesHandler.Display( "Item 1 marked.");
                }else
                {
                    MessagesHandler.Display( "Only text is supported currently.");
                }
                

            }
            else
                if (command.Name == "compare case sensitive" || command.Name== "compare case insensitive")
                {
                    if(this.comparerItem1==null)
                    {
                        MessagesHandler.Display( "Use 'comparer item 1' command first.");
                    }else
                    {
                        if(command.parametersOnExecute[0] is StringWorkItem)
                        {
                            this.comparerItem2 = command.parametersOnExecute[0].GetValueAsText();

                            bool areEqual = false;
                            bool compareAsStrings = true;

                            if (command.Name != "compare case insensitive")
                            {
                                if (CraftSynth.BuildingBlocks.IO.FileSystem.IsFilePathValid(this.comparerItem1.ToString()) &&
                                    CraftSynth.BuildingBlocks.IO.FileSystem.IsFilePathValid(this.comparerItem2.ToString()) &&
                                    File.Exists(this.comparerItem1.ToString()) &&
                                    File.Exists(this.comparerItem2.ToString())
                                    )
                                {
                                    Thread bringToFrontAssistant = new Thread(BringToFront);
                                    bringToFrontAssistant.Start("Comparer");
                                    if (MessageBox.Show(string.Format("Marked items:\n\r{0}\n\r{1}\n\r are paths to existing files on disk. Should file content be compared?", this.comparerItem1.ToString(), this.comparerItem2.ToString()), "Comparer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                                    {
                                        compareAsStrings = false;
                                    }
                                }
                            }

                            if (compareAsStrings)
                            {

                                CompareOptions compareOprions = CompareOptions.Ordinal;
                                if (command.Name == "compare case sensitive")
                                {
                                    compareOprions = CompareOptions.Ordinal;
                                }
                                else if (command.Name == "compare case insensitive")
                                {
                                    compareOprions = CompareOptions.OrdinalIgnoreCase;
                                }

                                areEqual = string.Compare(this.comparerItem1.ToString(), this.comparerItem2.ToString(), CultureInfo.InvariantCulture, compareOprions) == 0;
                            }
                            else
                            {
                                areEqual = CraftSynth.BuildingBlocks.IO.FileSystem.CompareFileContent(this.comparerItem1.ToString(), this.comparerItem2.ToString());

                            }

                            if (areEqual)
                            {
                                MessagesHandler.Display( "Equal.");
                            }
                            else
                            {
                                MessagesHandler.Display( "Different.");
                            }

                        }else
                        {
                            MessagesHandler.Display( "Only text is supported currently.");
                        }
                    }

                }
                else
                    if (command.Name == "compare in notepad")
                    {
                        if(this.comparerItem1==null)
                        {
                            MessagesHandler.Display( "Use 'comparer item 1' command first.");
                        }else
                        {
                            if(command.parametersOnExecute[0] is StringWorkItem)
                            {
                                this.comparerItem2 = command.parametersOnExecute[0].GetValueAsText();

                                string notepadContent = string.Format("{0}\r\n{1}",
                                    this.comparerItem1,
                                    this.comparerItem2);

                                CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenTextInNotepad(Common.Helper.GetEnsoTempraryFilePath("txt"), notepadContent);
                            }else
                            {
                                MessagesHandler.Display( "Only text is supported currently.");
                            }
                        }
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

        private void BringToFront(object o)
        {
            string caption = (string)o;
            int i = 5;
            while (i > 0)
            {
                i++;
                Thread.Sleep(1000);
                if (CraftSynth.BuildingBlocks.WindowsNT.Misc.SetForegroundWindow(caption)) i = 0;
            }
        }

        public void ProcessingBeforeParameterInput(Command command, ref bool cancel)
        {
        }

        #endregion
    }
}
