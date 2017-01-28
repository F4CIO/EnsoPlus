using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using EnsoPlus;
using EnsoPlus.Entities;
using Extension;
using System.IO;
using Common;

namespace EnsoPlus.CommandsProviders.RdcOpener
{
    class RdcOpener:ICommandsProvider
    {
      
        #region ICommandsProvider Members

        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            //commands.Add( new Command("open in Total Commander", "[learned item]", "Opens previousely learned item in Total Commander", null, EnsoPostfixType.Arbitrary);

            commands.Add( new Command("rdc", "[item]", "Opens Remote Desktop Connection for selected IP with or without port number", null, EnsoPostfixType.Arbitrary, this, true, true,
                new ParameterInputArguments("rdc", null, false, false, "", false, false),
                typeof( WorkItemsProviders.MemorizedData.MemorizedData),
               typeof( WorkItemsProviders.CommandsHistory.CommandsHistory),
                //typeof( WorkItemsProviders.Shortcuts.Shortcuts),
                //typeof(WorkItemsProviders.ReflectionData.ReflectionData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)));

			commands.Add(new Command("rdc-as-default-user", "[item]", "Opens Remote Desktop Connection for selected IP with or without port number using RdcDefaultUsername and RdcDefaultPassword from settings file.", null, EnsoPostfixType.Arbitrary, this, true, true,
			  new ParameterInputArguments("rdc-as-default-user", null, false, false, "", false, false),
			  typeof(WorkItemsProviders.MemorizedData.MemorizedData),
			  typeof(WorkItemsProviders.CommandsHistory.CommandsHistory),
			  //typeof(WorkItemsProviders.Shortcuts.Shortcuts),
				//typeof(WorkItemsProviders.ReflectionData.ReflectionData),
			  typeof(WorkItemsProviders.Clipboard.ClipboardText)));

            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("RDCOpener: Executing command '{0}' ...", command.Name));

			if (command.Name == "rdc" || command.Name == "rdc-as-default-user")
            {
                if (command.Name=="rdc" && IsPathSuitable(command.parametersOnExecute[0].GetValueAsText()))
                {//open .rdc file
					Logging.AddActionLog(string.Format("RdcOpener: Opening file '{0}' in RDC ...", command.parametersOnExecute[0].GetValueAsText()));
					MessagesHandler.Display( string.Format("Opening file {0} ...", command.parametersOnExecute[0].GetValueAsText()));
					
					CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenFile(command.parametersOnExecute[0].GetValueAsText());
                }
                else
                {
					Thread thread = new Thread(() => RdcOpenInNewThread(command));
					thread.Start();
                }
			}
        }

	    private static void RdcOpenInNewThread(Command command)
	    {
		    Logging.AddActionLog(string.Format("RdcOpener: Opening '{0}' in RDC ...",command.parametersOnExecute[0].GetValueAsText().Trim().Split(' ')[0]));
		    MessagesHandler.Display(string.Format("Connecting to {0} ...",command.parametersOnExecute[0].GetValueAsText().Trim().Split(' ')[0]));

		    List<string> parts = command.parametersOnExecute[0].GetValueAsText().
			    Trim().Replace("\\r", " ").Replace("\\n", " ").
			    Replace(" /user:", " ").Replace(" /User:", " ").Replace(" /USER:", " ").
			    Replace(" /pass:", " ").Replace(" /Pass:", " ").Replace(" /PASS:", " ").
			    Split(' ').ToList();

		    if (command.Name == "rdc-as-default-user" && parts.Count == 1)
		    {
			    //parts.RemoveAt(0);//as
			    //parts.RemoveAt(0);//default
			    //parts.RemoveAt(0);//user

			    parts.Add(Settings.Current.RdcDefaultUsername);
			    parts.Add(Settings.Current.RdcDefaultPassword);

			    //final list should be ip,user1,pass1
		    }

		    if (parts.Count == 1)
		    {//example: 192.168.0.1:12
			    Process rdcProcess = new Process();
			    rdcProcess.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\mstsc.exe");
			    rdcProcess.StartInfo.Arguments = string.Format("/v {0}", parts[0]);
			    rdcProcess.Start();
		    }
		    else if (parts.Count == 2)
		    {//example: 192.168.0.1:12 /user:mynet\Administrator
			    Process rdcProcess = new Process();
			    rdcProcess.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe");
			    rdcProcess.StartInfo.Arguments = string.Format("/generic:TERMSRV/{0} /user:{1}", parts[0], parts[1]);
			    rdcProcess.Start();

			    rdcProcess.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\mstsc.exe");
			    rdcProcess.StartInfo.Arguments = string.Format("/v {0}", parts[0]);
			    rdcProcess.Start();

			    Thread.Sleep(10000);

			    rdcProcess.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe");
			    rdcProcess.StartInfo.Arguments = string.Format("/delete:TERMSRV/{0}", parts[0]);
			    rdcProcess.Start();
		    }
		    else if (parts.Count == 3)
		    {//example: 192.168.0.1:12 /user:mynet\Administrator /pass:somepass1
			    Process rdcProcess = new Process();
			    rdcProcess.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe");
			    rdcProcess.StartInfo.Arguments = string.Format("/generic:TERMSRV/{0} /user:{1} /pass:{2}", parts[0], parts[1],parts[2]);
			    rdcProcess.Start();

			    rdcProcess.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\mstsc.exe");
			    rdcProcess.StartInfo.Arguments = string.Format("/v {0}", parts[0]);
			    rdcProcess.Start();

			    Thread.Sleep(10000);

			    rdcProcess.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe");
			    rdcProcess.StartInfo.Arguments = string.Format("/delete:TERMSRV/{0}", parts[0]);
			    rdcProcess.Start();
		    }
		    else
		    {
			    Logging.AddErrorLog(string.Format("RdcOpener: Invalid 'IP username password': '{0}'.",command.parametersOnExecute[0].GetValueAsText()));
			    MessagesHandler.Display("Syntax 'IP:port username password' not recognized. Port, username and password are optional.",command.parametersOnExecute[0].GetValueAsText());
		    }
	    }

	    private static bool IsPathSuitable(string path)
        {
            return !path.Contains("\r\n") && 
                (File.Exists(path) || path.StartsWith(@"\\"));
        }

	    private static void RDC(string ipWithOrWithoutPortWithOrWithoutCredentials)
	    {
		    
	    } 

        public void ProcessingBeforeParameterInput(Command command, ref bool cancel)
        {
        }

        #endregion
    }
}
