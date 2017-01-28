using System;
using System.Collections.Generic;
using System.Text;

using Extension;
using EnsoPlus;
using EnsoPlus.Entities;
using Common;
using EnsoPlus.WorkItemsProviders.Macros;
using System.IO;
using System.Diagnostics;

namespace EnsoPlus.CommandsProviders.MacroManager
{
    class MacroManager : ICommandsProvider
    {
        #region ICommandsProvider Members

        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            List<Macro> macrosNamesAndFilePaths = Macros.GetNamesAndFilePaths();
            foreach (Macro m in macrosNamesAndFilePaths)
            {                
                Macro macro = Macros.Get(m.filePath, false);
                commands.Add(new Command(macro.name, macro.postfix, macro.description, null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
                new ParameterInputArguments(/*Caption:*/"macro", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false)
                /*Suggestions sources:*/                
                )); 
            }

            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("MacroManager: Executing command '{0}' ...", command.Name));

            string errorMessage = null;

            Macro macro = null;
            if (errorMessage == null)
            {
                try
                {
                    string filePath = Path.Combine(Settings.Current.MacrosDataFolder, command.Name + ".mcr");
                    macro = Macros.Get(filePath, true);
                }
                catch (Exception exception)
                {
                    errorMessage = "Can not load macro. " + exception.Message;
                }                
            }

            if (errorMessage == null)
            {
                string macroString = string.Empty;
                foreach (string line in macro.body)
                {
                    macroString += line + "\r";
                }
                for (int i = 0; i < command.parametersOnExecute.Count; i++)
                {
                    macroString = macroString.Replace(Settings.Current.startOfMacroParameter + i + Settings.Current.endOfMacroParameter, command.parametersOnExecute[i].GetValueAsText());
                }
                try
                {
                    File.WriteAllText(Path.Combine(Settings.Current.MacrosDataFolder, "Macro.tmp.mcr"), macroString);
                }
                catch (Exception exception)
                {
                    errorMessage = "Error occured while writeing temp file." + exception.Message;
                }
            }

            if (errorMessage == null)
            {
                try
                {
                    Process process = new Process();
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.FileName = Path.Combine(Settings.Current.MacrosDataFolder, "Macro.tmp.mcr");
                    Logging.AddActionLog(string.Format("MacroManager: Starting macro '{0}' ({1}) ...", macro.filePath, macro.GetCaption()));
                    process.Start();
                }
                catch (Exception exception)
                {
                    errorMessage = "Error occured while starting temp file. " + exception.Message;
                }
            }

            if (errorMessage != null)
            {
                throw new ApplicationException("ExtensionMacro: OnCommand failed :" + errorMessage);
            }
        }        
        
        public void ProcessingBeforeParameterInput(Command command, ref bool cancel)
        {
        }

        #endregion
    }
}
