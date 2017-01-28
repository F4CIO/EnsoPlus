using System;
using System.Collections.Generic;
using System.Text;

using EnsoPlus;
using EnsoPlus.Entities;
using Extension;
using System.IO;
using Common;

namespace EnsoPlus.CommandsProviders.FOpener
{
    class FOpener : ICommandsProvider
    {
        private const string saveAllTabsUserManual = @"Install firefox addon 'Copy All Urls'. Every time before using this command press CTRL+ALT+C."; //TODO: Add text: For url markup set '<a href="$url">$title</a><br/>'. 

        #region ICommandsProvider Members

        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            commands.Add(new Command("fopen in new window", "[item]", "Opens item or selected string in new window of Firefox", null, EnsoPostfixType.Arbitrary, this, true, true,
                new ParameterInputArguments("fopen in new window", null, false, false, "", false, false),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Shortcuts.Shortcuts),
                typeof(WorkItemsProviders.Shortcuts.ShortcutsLists),
                typeof(WorkItemsProviders.BookmarkBrowser.BookmarkBrowser),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("fopen in new tab", "[item]", "Opens item or selected string in new tab of Firefox", null, EnsoPostfixType.Arbitrary, this, true, true,
              new ParameterInputArguments("fopen in new tab", null, false, false, "", false, false),
              typeof(WorkItemsProviders.MemorizedData.MemorizedData),
              typeof(WorkItemsProviders.Shortcuts.Shortcuts),
              typeof(WorkItemsProviders.Shortcuts.ShortcutsLists),
               typeof(WorkItemsProviders.BookmarkBrowser.BookmarkBrowser),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
              ));

            commands.Add(new Command("save-all-tabs-as", "[shortcut name]", "Saves urls from tabs as shortcut. Use fopen command to open them later.", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
            new ParameterInputArguments(/*Caption:*/"save all tabs as", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
                        /*Suggestions sources:*/
            typeof(WorkItemsProviders.MemorizedData.MemorizedData)
            ));

            commands.Add(new Command("preview in firefox", "[html]", "Opens item or selected string as html in new window of Firefox", null, EnsoPostfixType.Arbitrary, this, true, false,
                new ParameterInputArguments("html", null, false, false, "", false, false),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("preview in new tab of firefox", "[html]", "Opens item or selected string as html in new window of Firefox", null, EnsoPostfixType.Arbitrary, this, true, false,
                new ParameterInputArguments("html", null, false, false, "", false, false),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("test js in firefox in new window", "[javascript]", "Opens item or selected string as js in new window of Firefox", null, EnsoPostfixType.Arbitrary, this, true, false,
               new ParameterInputArguments("javascript", null, false, false, "", false, false),
               typeof(WorkItemsProviders.MemorizedData.MemorizedData),
               typeof(WorkItemsProviders.Clipboard.ClipboardText)
               ));

            commands.Add(new Command("test js in firefox in new tab", "[javascript]", "Opens item or selected string as js in new window of Firefox", null, EnsoPostfixType.Arbitrary, this, true, false,
                new ParameterInputArguments("javascript", null, false, false, "", false, false),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("test js function in firefox in new window", "[function name] [javascript]", "Opens item or selected string as js in new window of Firefox", null, EnsoPostfixType.Arbitrary, this, true, false,
              new ParameterInputArguments("javascript", null, false, false, "", false, false),
              typeof(WorkItemsProviders.MemorizedData.MemorizedData),
              typeof(WorkItemsProviders.Clipboard.ClipboardText)
              ));

            commands.Add(new Command("test js function in firefox in new tab", "[function name] [javascript]", "Opens item or selected string as js in new window of Firefox", null, EnsoPostfixType.Arbitrary, this, true, false,
                new ParameterInputArguments("javascript", null, false, false, "", false, false),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("FOpener: Executing command '{0}' ...", command.Name));

            if (command.Name == "fopen in new window" || command.Name == "fopen in new tab")
            {
                if (command.parametersOnExecute[0] is Entities.WorkItems.Shortcuts)
                {
                    Entities.WorkItems.Shortcuts shortcuts = command.parametersOnExecute[0] as Entities.WorkItems.Shortcuts;

                    Logging.AddActionLog(string.Format("FOpener: Opening '{0}' in Firefox ...", shortcuts.shortcutsFilePath));
                    MessagesHandler.Display( string.Format("Opening {0} ...", shortcuts.caption));

                    foreach (Shortcut shortcut in shortcuts.shortcuts)
                    {
                        Logging.AddActionLog(string.Format("FOpener: Opening '{0}' in Firefox ...", shortcut.targetPath));

                        OpenInFirefox(shortcut.targetPath, command.Name.Contains("tab"));
                    }
                }
                else
                {
                    Logging.AddActionLog(string.Format("FOpener: Opening '{0}' in Firefox ...", command.parametersOnExecute[0].GetValueAsText()));
                    MessagesHandler.Display( string.Format("Opening {0} ...", command.parametersOnExecute[0].GetValueAsText()));

                    OpenInFirefox(command.parametersOnExecute[0].GetValueAsText(), command.Name == "fopen in new tab");
                }
            }else
                if (command.Name == "save-all-tabs-as")
                {
                    SaveAllTabs(false, service, command.parametersOnExecute[0].GetValueAsText(), null,"FOpener");
                }
                else
                    if (command.Name == "preview in firefox" || command.Name == "preview in new tab of firefox")                    
                    {
                        string filePath = Common.Helper.GetEnsoTempraryFilePath("htm");
                        File.WriteAllText(filePath, command.parametersOnExecute[0].GetValueAsText());
                        OpenInFirefox(filePath, command.Name.Contains("tab"));
                    }
                    else
                        if (command.Name == "test js in firefox in new window" || command.Name == "test js in firefox in new tab")                       
                    {
                        string filePath = Common.Helper.GetEnsoTempraryFilePath("htm");
                        string js = command.parametersOnExecute[0].GetValueAsText();
                        string templatePath = Path.Combine(Common.Helper.GetEnsoPlusWorkingFolder(), @"Resources\TemplateJavascriptTest.htm");
                        string template = File.ReadAllText(templatePath);
                        string html = template.Replace("{0}", js);
                        File.WriteAllText(filePath, html);
                        OpenInFirefox(filePath, command.Name.Contains("tab"));
                    }
            if (command.Name == "test js function in firefox in new window" || command.Name == "test js function in firefox in new tab")
                    {
                        string filePath = Common.Helper.GetEnsoTempraryFilePath("htm");                        
                        string functionName = command.parametersOnExecute[0].GetValueAsText();
                        string js = command.parametersOnExecute[1].GetValueAsText();
                        string templatePath = Path.Combine(Common.Helper.GetEnsoPlusWorkingFolder(), @"Resources\TemplateJavascriptFunctionTest.htm");
                        string template = File.ReadAllText(templatePath);                        
                        if (js.IndexOf(functionName, StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            functionName = js.Substring(js.IndexOf(functionName, StringComparison.OrdinalIgnoreCase), functionName.Length);
                        }
                        string html = template.Replace("{0}", functionName);
                        html = html.Replace("{1}", js);
                        File.WriteAllText(filePath, html);
                        OpenInFirefox(filePath, command.Name.Contains("tab"));
                    }


        }

        public static void SaveAllTabs(bool saveInOneFile, Extension.IEnsoService service, string caption, string shortcutsFolder, string commandProviderNameForLog)
        {
			string clipboardText = CraftSynth.BuildingBlocks.IO.Clipboard.GetTextFromClipboard();
            clipboardText = clipboardText.Trim();
            if (string.IsNullOrEmpty(clipboardText))
            {
                MessagesHandler.Display( "Error: Clipboard empty.", saveAllTabsUserManual);
            }
            else
            {
                Entities.WorkItems.Shortcuts shortcuts = null;
                try
                {
                    shortcuts = new Entities.WorkItems.Shortcuts(caption, clipboardText);
                    if (shortcuts.shortcuts.Count == 0)
                    {
                        throw new ApplicationException("No urls in clipboard.");
                    }
                }
                catch (Exception)
                {
                    MessagesHandler.Display( "Error: Invalid text format in clipboard.", saveAllTabsUserManual);
                    Logging.AddErrorLog(commandProviderNameForLog+": Error: Invalid text format in clipboard");
                }

                if (shortcuts != null && shortcuts.shortcuts.Count>0)
                {
                    if (string.IsNullOrEmpty(shortcutsFolder))
                    {
                        shortcutsFolder = Settings.Current.EnsoLearnAsOpenCommandsFolder;
                    }
                    CraftSynth.BuildingBlocks.IO.FileSystem.CreateFolderIfItDoesNotExist(shortcutsFolder);

                    if (saveInOneFile)
                    {
                        shortcuts.Save(shortcutsFolder);
                        SuggestionsCache.DropCache(typeof(WorkItemsProviders.Shortcuts.ShortcutsLists));
                        Logging.AddActionLog(string.Format(commandProviderNameForLog + ": {0} tabs saved in '{1}'.", shortcuts.shortcuts.Count,Path.Combine(shortcutsFolder, caption)));
                        MessagesHandler.Display( string.Format("{0} tabs saved in one shortcut {1}.", shortcuts.shortcuts.Count,caption));
                    }
                    else
                    {
                        shortcuts.SaveAsSeparateShortcuts(shortcutsFolder);
                        Logging.AddActionLog(string.Format(commandProviderNameForLog + ": {0} tabs saved as separate files in '{1}'.", shortcuts.shortcuts.Count,shortcutsFolder));
                        MessagesHandler.Display( string.Format("{0} shortcuts saved in category {1}.", shortcuts.shortcuts.Count,caption));
                    }
                }
            }
        }

        public static void OpenInFirefox(string filePath, bool inNewTab)
        {
            string exePath = Settings.Current.FOpenerExePath;
            string parametersFormat = null;
            if (!inNewTab)
            {
                parametersFormat = Settings.Current.FOpenerParametersFormatForOpenInNewWindow;
            }
            else
            {
                parametersFormat = Settings.Current.FOpenerParametersFormatForOpenInNewTab;
            }
            string parameters = string.Format(parametersFormat, filePath);

            CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenFile(exePath, parameters);
        }        
        
        public void ProcessingBeforeParameterInput(Command command, ref bool cancel)
        {
        }

        #endregion

    }
}
