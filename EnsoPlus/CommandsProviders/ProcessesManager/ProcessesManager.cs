using System;
using System.Collections.Generic;
using System.Text;

using Extension;
using EnsoPlus;
using EnsoPlus.Entities;
using Common;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace EnsoPlus.CommandsProviders.ProcessesManager
{
    class ProcessesManager : ICommandsProvider
    {
        #region ICommandsProvider Members

        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            commands.Add(new Command("kill", "[process name]", "Kills process specified by name", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments(/*Caption:*/"process to kill", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
                /*Suggestions sources:*/
                typeof(WorkItemsProviders.Processes.Processes)                
                ));

            commands.Add(new Command("restart process", "[process name]", "Restarts process specified by name", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments(/*Caption:*/"process to kill", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
                /*Suggestions sources:*/
                typeof(WorkItemsProviders.Processes.Processes)
                ));

            //commands.Add(new Command("commandWithNoPostfix", " ", "This is description", null, EnsoPostfixType.None, this, /*Can use selection as parameter:*/false,
            //    new ParameterInputArguments()
            //    ));

            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("ProcessesManager: Executing command '{0}' ...", command.Name));

            if (command.Name == "kill" || command.Name == "restart process")
            {
                if (command.parametersOnExecute[0] is ProcessWorkItem)
                {
                    ProcessWorkItem processWorkItem = command.parametersOnExecute[0] as ProcessWorkItem;
                    FinishCommand(command.Name, processWorkItem.procces, service);
                }
                else if (command.parametersOnExecute[0] is StringWorkItem)
                {
                    Process processById = null;
                    try
                    {
                        int id = Convert.ToInt32(command.parametersOnExecute[0].GetValueAsText());
                        processById = Process.GetProcessById(id);
                    }
                    catch { }
                    if (processById != null)
                    {
                        FinishCommand(command.Name, processById, service);
                    }
                    else
                    {
                        Process[] processesByName = Process.GetProcessesByName(command.parametersOnExecute[0].GetValueAsText());
                        if (processesByName.Length == 1)
                        {
                            FinishCommand(command.Name, processesByName[0], service);
                        }
                        //else if (processesByName.Length > 1)
                        //{
                        //    if (MessageBox.Show(string.Format("{0} proccesses have name '{1}'. Kill'em all?", processesByName.Length, command.parametersOnExecute[0].GetValueAsText()), "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        //    {
                        //        //string message = string.Empty;
                        //        foreach (Process p in processesByName)
                        //        {
                        //            p.Kill();
                        //        }
                        //        MessagesHandler.Display( "Killed");
                        //    }
                        //}
                        else if (processesByName.Length == 0 || processesByName.Length > 1)
                        {
                            //if (MessageBox.Show(string.Format("None is found. Search partialy?"), "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            //{
                            //MessagesHandler.Display( "0 found. Searching partialy...");    

                            Process[] allProcesses = Process.GetProcesses();
                            List<Process> processesThatContainPhraseInName = new List<Process>();
                            foreach (Process processToCheck in allProcesses)
                            {
                                if (-1 != processToCheck.ProcessName.IndexOf(command.parametersOnExecute[0].GetValueAsText(), StringComparison.InvariantCultureIgnoreCase))
                                {
                                    processesThatContainPhraseInName.Add(processToCheck);
                                }
                            }

                            if (processesThatContainPhraseInName.Count == 0)
                            {
                                MessagesHandler.Display( "0 found");
                            }
                            else
                            {
                                string resultNames = string.Empty;
                                foreach (Process p in processesThatContainPhraseInName)
                                {
                                    resultNames += string.Format(",\r\n{0} ({1})", p.ProcessName, p.Id);
                                }
                                if (!string.IsNullOrEmpty(resultNames)) resultNames = resultNames.Remove(0, 3);
                                if (MessageBox.Show(string.Format("{0}\r\n\r\nfound. Affect all?", resultNames), "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                                {
                                    //string message = string.Empty;
                                     FinishCommand(command.Name, processesThatContainPhraseInName, service);
                                }
                            }

                            //}
                        }
                    }

                }
                else
                {
                    var warperException = new ApplicationException(string.Format("Process manager: Unsupported type '{0}'found as parameter.", command.parametersOnExecute[0].GetType().FullName));
					Common.Logging.AddExceptionLog(warperException);
                    MessagesHandler.Display( "Unsupported type found as parameter.");
                }
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
                        throw new ApplicationException(string.Format("ProcessManager: Command not found. Command: {0} {1}", command.Name, command.Postfix));
                    }
        }


        

        #endregion

        public static void FinishCommand(string commandName, Process selectedProcess, IEnsoService service)
        {
            if (commandName == "kill")
            {
                selectedProcess.Kill();
                string message = string.Format("{0} killed", selectedProcess.ProcessName);
                Logging.AddActionLog(string.Format("ProcessesManager: {0}", message));
                MessagesHandler.Display( message);
            }
            else if (commandName == "restart process")
            {
                string processExePath = selectedProcess.MainModule.FileName;
                selectedProcess.Kill();
                ContinueWhenProcessIsNotActive(selectedProcess);
                string message = string.Format("{0} killed", selectedProcess.ProcessName);
                Logging.AddActionLog(string.Format("ProcessesManager: {0}", message));
                MessagesHandler.Display( message);

                CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenFile(processExePath);
                message = string.Format("{0} restarted", selectedProcess.ProcessName);
                Logging.AddActionLog(string.Format("ProcessesManager: {0}", message));
                MessagesHandler.Display( message);
            }
        }

        public static void ContinueWhenProcessIsNotActive(Process terminatingProcess)
        {
            while (IsProcessActive(terminatingProcess))
            {
                Thread.Sleep(2000);
            }
        }        

        public static bool IsProcessActive(Process process)
        {
            bool active = false;
            foreach (Process activeProcessWithSameName in Process.GetProcessesByName(process.ProcessName))
            {
                if (activeProcessWithSameName.Id == process.Id)
                {
                    active = true;
                    break;
                }
            }
            return active;
        }
        
        private void FinishCommand(string commandName, List<Process> selectedProcesses, IEnsoService service)
        {
            string message = string.Empty;

            if (commandName == "kill")
            {
                foreach (Process p in selectedProcesses)
                {
                    p.Kill();
                    message += string.Format(",\r\n{0} ({1})", p.ProcessName, p.Id);
                    MessagesHandler.Display( message+"Killed");
                }
                message = message + " - Killed all";
                Logging.AddActionLog(string.Format("ProcessesManager: {0}", message));
                MessagesHandler.Display( message);
            }
            else if(commandName=="restart process")
            {
                List<string> processesFilePaths = new List<string>();
                //call kill
                foreach (Process p in selectedProcesses)
                {
                    processesFilePaths.Add(p.MainModule.FileName);
                    p.Kill();                   
                }

                //wait
                foreach (Process p in selectedProcesses)
                {
                    ContinueWhenProcessIsNotActive(p);
                    if (message != string.Empty) message += ",";
                    message += string.Format("\r\n{0} ({1})", p.ProcessName, p.Id);
                    MessagesHandler.Display( message + "Killed");
                }
                message = message + " - Killed all";
                Logging.AddActionLog(string.Format("ProcessesManager: {0}", message));
                MessagesHandler.Display( message);

                //start
                message = string.Empty;                
                foreach (string processFilePath in processesFilePaths)
                {
                    CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenFile(processFilePath);
                    if (message != string.Empty) message += ",";
                    message += string.Format("\r\n{0} ", Path.GetFileName(processFilePath));
                    MessagesHandler.Display( message + "Restarted");
                }
                message = message + " - Restarted all";
                Logging.AddActionLog(string.Format("ProcessesManager: {0}", message));
                MessagesHandler.Display( message);
            }
        }

        public void ProcessingBeforeParameterInput(Command command, ref bool cancel)
        {
        }

    }
}
