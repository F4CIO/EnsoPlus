using System;
using System.Collections.Generic;
using System.Text;

using EnsoPlus.Entities;
using Extension;
using EnsoPlus.WorkItemsProviders.ShortcutTemplates;
using System.Diagnostics;
using Common;

namespace EnsoPlus.CommandsProviders.WebSearcher
{
    class WebSearcher : ICommandsProvider
    {
        #region ICommandsProvider Members

        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            commands.Add(new Command("learn as web search command with name", "[command name] [uri template]", "Creates command from selected or specified uri template. Template example: http://www.google.com/search?q={0}", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/false,
                    new ParameterInputArguments("uri template", null, false, false, "", false, false)
                    ));    
           

            Dictionary<string,ShortcutTemplate> shortcutTemplates = ShortcutTemplates.GetNamesWithFilePaths();
            foreach (ShortcutTemplate shortcutTemplate in shortcutTemplates.Values)
            {
                commands.Add(new Command(shortcutTemplate.caption, "[phrase]", "Searches web for specified or selected phrase", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/false,
                    new ParameterInputArguments(shortcutTemplate.caption, null, false, false, "", false, false)
                    ));               
            }

            return commands;
        }

        public void ExecuteCommand(IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("WebSearcher: Executing command '{0}' ...", command.Name));

            if (command.Name == "learn as web search command with name")
            {
                ShortcutTemplates.CreateShortcutTemplate(command.parametersOnExecute[0].GetValueAsText(), command.parametersOnExecute[1].GetValueAsText());
                SuggestionsCache.DropCache(typeof(WorkItemsProviders.ShortcutTemplates.ShortcutTemplates));
                EnsoPlus.current.Reinitialize();
                string message = string.Format("{0} is now a command.", command.parametersOnExecute[0].GetValueAsText());
                Logging.AddActionLog(string.Format("WebSearcher: {0}", message));
                MessagesHandler.Display(  message);
            }
            else
            {
                string shortcutFilePath = ShortcutTemplates.BuildFilePath(command.Name);
                string template = ShortcutTemplates.GetTemplate(shortcutFilePath);
                string queryString = string.Format(template, command.parametersOnExecute[0].GetValueAsText());
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = queryString;
                Logging.AddActionLog(string.Format("WebSearcher: Starting '{0}' ...", process.StartInfo.FileName));
                process.Start();
            }
        }
        public void ProcessingBeforeParameterInput(Command command, ref bool cancel)
        {
        }

        #endregion

    }
}
