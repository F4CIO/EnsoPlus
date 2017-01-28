using System;
using System.Collections.Generic;
using System.Text;

using EnsoPlus;
using EnsoPlus.Entities;
using Extension;
using System.IO;
using Common;

namespace EnsoPlus.CommandsProviders.COpener
{
    class COpener : ICommandsProvider
    {
        #region ICommandsProvider Members

        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            commands.Add(new Command("copen in new window", "[item]", "Opens item or selected string in new window of Chrome", null, EnsoPostfixType.Arbitrary, this, true, true,
                new ParameterInputArguments("copen in new window", null, false, false, "", false, false),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Shortcuts.Shortcuts),
                typeof(WorkItemsProviders.Shortcuts.ShortcutsLists),
                typeof(WorkItemsProviders.BookmarkBrowser.BookmarkBrowser),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("copen in new tab", "[item]", "Opens item or selected string in new tab of Chrome", null, EnsoPostfixType.Arbitrary, this, true, true,
              new ParameterInputArguments("copen in new tab", null, false, false, "", false, false),
              typeof(WorkItemsProviders.MemorizedData.MemorizedData),
              typeof(WorkItemsProviders.Shortcuts.Shortcuts),
              typeof(WorkItemsProviders.Shortcuts.ShortcutsLists),
               typeof(WorkItemsProviders.BookmarkBrowser.BookmarkBrowser),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
              ));

            commands.Add(new Command("copen in new incognito window", "[item]", "Opens item or selected string in new incognito window of Chrome", null, EnsoPostfixType.Arbitrary, this, true, true,
                new ParameterInputArguments("copen in new incognito window", null, false, false, "", false, false),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Shortcuts.Shortcuts),
                typeof(WorkItemsProviders.Shortcuts.ShortcutsLists),
                typeof(WorkItemsProviders.BookmarkBrowser.BookmarkBrowser),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("copen in new incognito tab", "[item]", "Opens item or selected string in new incognito tab of Chrome", null, EnsoPostfixType.Arbitrary, this, true, true,
              new ParameterInputArguments("copen in new incognito tab", null, false, false, "", false, false),
              typeof(WorkItemsProviders.MemorizedData.MemorizedData),
              typeof(WorkItemsProviders.Shortcuts.Shortcuts),
              typeof(WorkItemsProviders.Shortcuts.ShortcutsLists),
               typeof(WorkItemsProviders.BookmarkBrowser.BookmarkBrowser),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
              ));

            commands.Add(new Command("preview in chrome", "[html]", "Opens item or selected string as html in new window of Chrome", null, EnsoPostfixType.Arbitrary, this, true, false,
                new ParameterInputArguments("html", null, false, false, "", false, false),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("preview in new tab of chrome", "[html]", "Opens item or selected string as html in new window of Chrome", null, EnsoPostfixType.Arbitrary, this, true, false,
                new ParameterInputArguments("html", null, false, false, "", false, false),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("test js in chrome in new window", "[javascript]", "Opens item or selected string as js in new window of Chrome", null, EnsoPostfixType.Arbitrary, this, true, false,
               new ParameterInputArguments("javascript", null, false, false, "", false, false),
               typeof(WorkItemsProviders.MemorizedData.MemorizedData),
               typeof(WorkItemsProviders.Clipboard.ClipboardText)
               ));

            commands.Add(new Command("test js in chrome in new tab", "[javascript]", "Opens item or selected string as js in new window of Chrome", null, EnsoPostfixType.Arbitrary, this, true, false,
                new ParameterInputArguments("javascript", null, false, false, "", false, false),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("test js function in chrome in new window", "[function name] [javascript]", "Opens item or selected string as js in new window of Chrome", null, EnsoPostfixType.Arbitrary, this, true, false,
              new ParameterInputArguments("javascript", null, false, false, "", false, false),
              typeof(WorkItemsProviders.MemorizedData.MemorizedData),
              typeof(WorkItemsProviders.Clipboard.ClipboardText)
              ));

            commands.Add(new Command("test js function in chrome in new tab", "[function name] [javascript]", "Opens item or selected string as js in new window of Chrome", null, EnsoPostfixType.Arbitrary, this, true, false,
                new ParameterInputArguments("javascript", null, false, false, "", false, false),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("COpener: Executing command '{0}' ...", command.Name));

            if (command.Name == "copen in new window" || command.Name == "copen in new tab" ||
                command.Name == "copen in new incognito window" || command.Name == "copen in new incognito tab")
            {
                if (command.parametersOnExecute[0] is Entities.WorkItems.Shortcuts)
                {
                    Entities.WorkItems.Shortcuts shortcuts = command.parametersOnExecute[0] as Entities.WorkItems.Shortcuts;

                    Logging.AddActionLog(string.Format("COpener: Opening '{0}' in Chrome ...", shortcuts.shortcutsFilePath));
                    MessagesHandler.Display( string.Format("Opening {0} ...", shortcuts.caption));

                    foreach (Shortcut shortcut in shortcuts.shortcuts)
                    {
                        Logging.AddActionLog(string.Format("COpener: Opening '{0}' in Chrome ...", shortcut.targetPath));

                        OpenInChrome(shortcut.targetPath, command.Name.Contains("tab"), command.Name.Contains("incognito"));
                    }
                }
                else
                {
                    Logging.AddActionLog(string.Format("COpener: Opening '{0}' in Chrome ...", command.parametersOnExecute[0].GetValueAsText()));
                    MessagesHandler.Display( string.Format("Opening {0} ...", command.parametersOnExecute[0].GetValueAsText()));

                    OpenInChrome(command.parametersOnExecute[0].GetValueAsText(), command.Name.Contains("tab"), command.Name.Contains("incognito"));
                }
            }
            else                
                    if (command.Name == "preview in chrome" || command.Name == "preview in new tab of chrome")
                    {
                        string filePath = Common.Helper.GetEnsoTempraryFilePath("htm");
                        File.WriteAllText(filePath, command.parametersOnExecute[0].GetValueAsText());
                        OpenInChrome(filePath, command.Name.Contains("tab"), command.Name.Contains("incognito"));
                    }
                    else
                        if (command.Name == "test js in chrome in new window" || command.Name == "test js in chrome in new tab")
                        {
                            string filePath = Common.Helper.GetEnsoTempraryFilePath("htm");
                            string js = command.parametersOnExecute[0].GetValueAsText();
                            string templatePath = Path.Combine(Common.Helper.GetEnsoPlusWorkingFolder(), @"Resources\TemplateJavascriptTest.htm");
                            string template = File.ReadAllText(templatePath);
                            string html = template.Replace("{0}", js);
                            File.WriteAllText(filePath, html);
                            OpenInChrome(filePath, command.Name.Contains("tab"), command.Name.Contains("incognito"));
                        }
            if (command.Name == "test js function in chrome in new window" || command.Name == "test js function in chrome in new tab")
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
                OpenInChrome(filePath, command.Name.Contains("tab"), command.Name.Contains("incognito"));
            }


        }

       

        public static void OpenInChrome(string filePath, bool inNewTab, bool inIncognitoMode)
        {
            string exePath = Settings.Current.COpenerExePath;
            string parametersFormat = null;
            if (!inNewTab)
            {
                if (inIncognitoMode)
                {
                    parametersFormat = Settings.Current.COpenerParametersFormatForOpenInNewIncognitoWindow;
                }
                else
                {
                    parametersFormat = Settings.Current.COpenerParametersFormatForOpenInNewWindow;
                }
            }
            else
            {
                if (inIncognitoMode)
                {
                    parametersFormat = Settings.Current.COpenerParametersFormatForOpenInNewIncognitoTab;
                }
                else
                {
                    parametersFormat = Settings.Current.COpenerParametersFormatForOpenInNewTab;
                }
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
