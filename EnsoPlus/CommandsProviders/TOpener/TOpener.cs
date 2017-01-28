using System;
using System.Collections.Generic;
using System.Text;

using EnsoPlus;
using EnsoPlus.Entities;
using Extension;
using System.IO;
using Common;

namespace EnsoPlus.CommandsProviders.TOpener
{
    class TOpener:ICommandsProvider
    {
        private enum TotalCommanderPane
        {
            Left,
            Right
        }

        #region ICommandsProvider Members

        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            //commands.Add( new Command("open in Total Commander", "[learned item]", "Opens previousely learned item in Total Commander", null, EnsoPostfixType.Arbitrary);

            commands.Add( new Command("topen", "[item]", "Opens selected folder, file or path string in left pane of Total Commander", null, EnsoPostfixType.Arbitrary, this, true, true,
                new ParameterInputArguments("topen", null, false, false, "", false, false),
                typeof( WorkItemsProviders.MemorizedData.MemorizedData),
                typeof( WorkItemsProviders.Shortcuts.Shortcuts),
                typeof(WorkItemsProviders.ReflectionData.ReflectionData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)));

            commands.Add(new Command("rtopen", "[item]", "Opens selected folder, file or path string in right pane of Total Commander", null, EnsoPostfixType.Arbitrary, this, true, true,
              new ParameterInputArguments("topen", null, false, false, "", false, false),
              typeof(WorkItemsProviders.MemorizedData.MemorizedData),
              typeof(WorkItemsProviders.Shortcuts.Shortcuts),
              typeof(WorkItemsProviders.ReflectionData.ReflectionData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)));

            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("TOpener: Executing command '{0}' ...", command.Name));

            if (command.Name == "topen"|| command.Name=="rtopen")
            {
                Logging.AddActionLog(string.Format("TOpener: Opening '{0}' in Total Commander ...", command.parametersOnExecute[0].GetValueAsText()));
                MessagesHandler.Display( string.Format("Opening {0} ...", command.parametersOnExecute[0].GetValueAsText()));

                if (IsPathSuitable(command.parametersOnExecute[0].GetValueAsText()))
                {
                    TotalCommanderPane pane = TotalCommanderPane.Left;
                    if (command.Name == "topen")
                    {
                        pane = TotalCommanderPane.Left;
                    }
                    else if (command.Name == "rtopen")
                    {
                        pane = TotalCommanderPane.Right;
                    }
                    OpenInTotalComander(command.parametersOnExecute[0].GetValueAsText(), pane);
                }
                else
                {//extract filepaths and try to open their parent folder
                    List<string> fileSelectionList = Syntax.FileSelectionFromString(command.parametersOnExecute[0].GetValueAsText());

                    if (fileSelectionList.Count == 0)
                    {
                        Logging.AddErrorLog(string.Format("TOpener: File/Folder '{0}' not found.", command.parametersOnExecute[0].GetValueAsText()));
                        MessagesHandler.Display( "File/Folder not found.", command.parametersOnExecute[0].GetValueAsText());
                    }
                    else
                    {
                        string parentFolder = Directory.GetParent(fileSelectionList[0]).FullName;
                        if (IsPathSuitable(parentFolder))
                        {                            
                            TotalCommanderPane pane = TotalCommanderPane.Left;
                            if (command.Name == "topen")
                            {
                                pane = TotalCommanderPane.Left;
                            }
                            else if(command.Name=="rtopen")
                            {
                                pane = TotalCommanderPane.Right;
                            }
                            OpenInTotalComander(parentFolder, pane);
                        }
                        else
                        {
                            Logging.AddErrorLog(string.Format("TOpener: File/Folder '{0}' not found.",parentFolder));
                            MessagesHandler.Display( "File/Folder not found.", parentFolder);
                        }

                        //foreach (string filePath in fileSelectionList)
                        //{
                        //    if (File.Exists(filePath) || Directory.Exists(filePath))
                        //    {
                        //        OpenInTotalComander(filePath);
                        //    }
                        //    else
                        //    {
                        //        MessagesHandler.Display( "File/Folder not found.", filePath);
                        //    }
                        //}
                    }
                    
                }
            }
        }

        private static bool IsPathSuitable(string path)
        {
            return !path.Contains("\r\n") && 
                (File.Exists(path) || Directory.Exists(path) || path.StartsWith(@"\\"));
        }

        private static void OpenInTotalComander(string filePath, TotalCommanderPane pane)
        {
            string exePath = Settings.Current.TOpenerExePath;
            string parametersFormat = null;
            if (pane == TotalCommanderPane.Left)
            {
                parametersFormat = Settings.Current.TOpenerParametersFormatForOpenInLeftPane;
            }
            else if(pane==TotalCommanderPane.Right)
            {
                parametersFormat = Settings.Current.TOpenerParametersFormatForOpenInRightPane;
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
