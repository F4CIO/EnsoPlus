using System;
using System.Collections.Generic;
using System.Text;

using EnsoPlus;
using EnsoPlus.Entities;
using Extension;
using System.IO;
using Common;

namespace EnsoPlus.CommandsProviders.IOpener
{
    class IOpener : ICommandsProvider
    {
        #region ICommandsProvider Members

        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            commands.Add(new Command("iopen in new window", "[item]", "Opens item or selected string in new window of Internet Explorer", null, EnsoPostfixType.Arbitrary, this, true, true,
                new ParameterInputArguments("iopen in new window", null, false, false, "", false, false),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Shortcuts.Shortcuts),
                typeof(WorkItemsProviders.Shortcuts.ShortcutsLists),
                typeof(WorkItemsProviders.BookmarkBrowser.BookmarkBrowser),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("iopen in new tab", "[item]", "Opens item or selected string in new tab of Internet Explorer", null, EnsoPostfixType.Arbitrary, this, true, true,
              new ParameterInputArguments("iopen in new tab", null, false, false, "", false, false),
              typeof(WorkItemsProviders.MemorizedData.MemorizedData),
              typeof(WorkItemsProviders.Shortcuts.Shortcuts),
              typeof(WorkItemsProviders.Shortcuts.ShortcutsLists),
               typeof(WorkItemsProviders.BookmarkBrowser.BookmarkBrowser),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
              ));

            commands.Add(new Command("preview in IE", "[html]", "Opens item or selected string as html in new window of Internet Explorer", null, EnsoPostfixType.Arbitrary, this, true, false,
                new ParameterInputArguments("html", null, false, false, "", false, false),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("preview in new tab of IE", "[html]", "Opens item or selected string as html in new window of Internet Explorer", null, EnsoPostfixType.Arbitrary, this, true, false,
                new ParameterInputArguments("html", null, false, false, "", false, false),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("test js in IE in new window", "[javascript]", "Opens item or selected string as js in new window of Internet Explorer", null, EnsoPostfixType.Arbitrary, this, true, false,
               new ParameterInputArguments("javascript", null, false, false, "", false, false),
               typeof(WorkItemsProviders.MemorizedData.MemorizedData),
               typeof(WorkItemsProviders.Clipboard.ClipboardText)
               ));

            commands.Add(new Command("test js in IE in new tab", "[javascript]", "Opens item or selected string as js in new window of Internet Explorer", null, EnsoPostfixType.Arbitrary, this, true, false,
                new ParameterInputArguments("javascript", null, false, false, "", false, false),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("test js function in IE in new window", "[function name] [javascript]", "Opens item or selected string as js in new window of Internet Explorer", null, EnsoPostfixType.Arbitrary, this, true, false,
              new ParameterInputArguments("javascript", null, false, false, "", false, false),
              typeof(WorkItemsProviders.MemorizedData.MemorizedData),
              typeof(WorkItemsProviders.Clipboard.ClipboardText)
              ));

            commands.Add(new Command("test js function in IE in new tab", "[function name] [javascript]", "Opens item or selected string as js in new window of Internet Explorer", null, EnsoPostfixType.Arbitrary, this, true, false,
                new ParameterInputArguments("javascript", null, false, false, "", false, false),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("IOpener: Executing command '{0}' ...", command.Name));

            if (command.Name == "iopen in new window" || command.Name == "iopen in new tab")
            {
                if (command.parametersOnExecute[0] is Entities.WorkItems.Shortcuts)
                {
                    Entities.WorkItems.Shortcuts shortcuts = command.parametersOnExecute[0] as Entities.WorkItems.Shortcuts;

                    Logging.AddActionLog(string.Format("IOpener: Opening '{0}' in Internet Explorer ...", shortcuts.shortcutsFilePath));
                    MessagesHandler.Display( string.Format("Opening {0} ...", shortcuts.caption));

                    foreach (Shortcut shortcut in shortcuts.shortcuts)
                    {
                        Logging.AddActionLog(string.Format("IOpener: Opening '{0}' in Internet Explorer ...", shortcut.targetPath));

                        OpenInInternetExplorer(shortcut.targetPath, command.Name.Contains("tab"));
                    }
                }
                else
                {
                    Logging.AddActionLog(string.Format("IOpener: Opening '{0}' in Internet Explorer ...", command.parametersOnExecute[0].GetValueAsText()));
                    MessagesHandler.Display( string.Format("Opening {0} ...", command.parametersOnExecute[0].GetValueAsText()));

                    OpenInInternetExplorer(command.parametersOnExecute[0].GetValueAsText(), command.Name == "iopen in new tab");
                }
            }
            else
                if (command.Name == "preview in IE" || command.Name == "preview in new tab of IE")
                {
                    string filePath = Common.Helper.GetEnsoTempraryFilePath("htm");
                    File.WriteAllText(filePath, command.parametersOnExecute[0].GetValueAsText());
                    OpenInInternetExplorer(filePath, command.Name.Contains("tab"));
                }
                else
                    if (command.Name == "test js in IE in new window" || command.Name == "test js in IE in new tab")
                    {
                        string filePath = Common.Helper.GetEnsoTempraryFilePath("htm");
                        string js = command.parametersOnExecute[0].GetValueAsText();
                        string templatePath = Path.Combine(Common.Helper.GetEnsoPlusWorkingFolder(), @"Resources\TemplateJavascriptTest.htm");
                        string template = File.ReadAllText(templatePath);
                        string html = template.Replace("{0}", js);
                        File.WriteAllText(filePath, html);
                        OpenInInternetExplorer(filePath, command.Name.Contains("tab"));
                    }
            if (command.Name == "test js function in IE in new window" || command.Name == "test js function in IE in new tab")
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
                OpenInInternetExplorer(filePath, command.Name.Contains("tab"));
            }


        }



        public static void OpenInInternetExplorer(string filePath, bool inNewTab)
        {
            string exePath = Settings.Current.IOpenerExePath;
            string parametersFormat = null;
            if (!inNewTab)
            {
                parametersFormat = Settings.Current.IOpenerParametersFormatForOpenInNewWindow;
            }
            else
            {
                parametersFormat = Settings.Current.IOpenerParametersFormatForOpenInNewTab;
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
