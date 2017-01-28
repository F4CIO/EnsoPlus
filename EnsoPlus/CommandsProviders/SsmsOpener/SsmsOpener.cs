using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using CraftSynth.BuildingBlocks.Common;
using EnsoPlus;
using EnsoPlus.Entities;
using Extension;
using System.IO;
using Common;

namespace EnsoPlus.CommandsProviders.SsmsOpener
{
    class SsmsOpener:ICommandsProvider
    {
      
        #region ICommandsProvider Members

        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            //commands.Add( new Command("open in Total Commander", "[learned item]", "Opens previousely learned item in Total Commander", null, EnsoPostfixType.Arbitrary);

            commands.Add( new Command("ssms", "[connection info]", "Opens SQL Server Management Studio and uses selected text to connect to server and database", null, EnsoPostfixType.Arbitrary, this, true, false,
                new ParameterInputArguments("connection info", null, false, false, "", false, false),
                typeof( WorkItemsProviders.MemorizedData.MemorizedData),
                typeof( WorkItemsProviders.CommandsHistory.CommandsHistory),
                //typeof( WorkItemsProviders.Shortcuts.Shortcuts),
                //typeof(WorkItemsProviders.ReflectionData.ReflectionData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)));

			commands.Add(new Command("ssms as default user", "[connection info]", "Opens SQL Server Management Studio and uses selected text to connect to server and database", null, EnsoPostfixType.Arbitrary, this, true, false,
				new ParameterInputArguments("connection info", null, false, false, "", false, false),
				typeof(WorkItemsProviders.MemorizedData.MemorizedData),
			    typeof(WorkItemsProviders.CommandsHistory.CommandsHistory),
				//typeof( WorkItemsProviders.Shortcuts.Shortcuts),
				//typeof(WorkItemsProviders.ReflectionData.ReflectionData),
				typeof(WorkItemsProviders.Clipboard.ClipboardText)));

            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("SsmsOpener: Executing command '{0}' ...", command.Name));

			if (command.Name == "ssms" || command.Name == "ssms as default user")
            {
				//if (command.Name=="rdc" && IsPathSuitable(command.parametersOnExecute[0].GetValueAsText()))
				//{//open .rdc file
				//	Logging.AddActionLog(string.Format("RdcOpener: Opening file '{0}' in RDC ...", command.parametersOnExecute[0].GetValueAsText()));
				//	MessagesHandler.Display( string.Format("Opening file {0} ...", command.parametersOnExecute[0].GetValueAsText()));
					
				//	CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenFile(command.parametersOnExecute[0].GetValueAsText());
				//}
				//else
				//{
					Thread thread = new Thread(() => SsmsInNewThread(command));
					thread.Start();
                //}
			}
        }

	    private static void SsmsInNewThread(Command command)
	    {

		   	Logging.AddActionLog(string.Format("SsmsOpener: Parsing input string..."));

			//Data Source=64.90.169.231;user id=ica_intranet; pwd=xxx;database=ICA_INTRANET;Timeout=100000
		    DbConnectionInfo dbi = new DbConnectionInfo(command.parametersOnExecute[0].GetValueAsText().Trim());

		    if (command.Name == "ssms as default user")
		    {
			    if (dbi.username == null)
			    {
				    dbi.username = Settings.Current.RdcDefaultUsername;
			    }
				
				if (dbi.password == null)
			    {
				    dbi.password = Settings.Current.RdcDefaultPassword;
			    }
		    }

		    if (!dbi.isValid)
		    {
			    Logging.AddActionLog(string.Format("SsmsOpener: Invalid connection info. Parsed: '{0}'", dbi.ToString("xxxx")));
				MessagesHandler.Display(string.Format(@"Invalid connection info. Please use .net connection string syntax or: server[[:port]\instance] database [userdomain\]username password"));
			}
			else
		    {   
				Logging.AddActionLog(string.Format("SsmsOpener: Opening '{0}' in SSMS ...",dbi.ToString("xxxx")));
				MessagesHandler.Display(string.Format("Connecting to {0} ...",dbi.serverName));

			    string p = string.Empty;
			    if (dbi.serverName != null)
			    {
				    p = p + "-s " + dbi.serverName;
				    if (dbi.serverPort != null)
				    {
					    p = p + ":" + dbi.serverPort;
				    }
				    if (dbi.instanceName != null)
				    {
					    p = p + "\\" + dbi.instanceName;
				    }
				    p = p + " ";
			    }
			    if (dbi.databaseName != null)
			    {
				    p = p + "-d " + dbi.databaseName + " ";
			    }
			    if (dbi.useIntegratedSecurity)
			    {
				    p = p + "-E ";
			    }
			    else
			    {
				    if (dbi.username != null)
				    {
					    p = p + "-u " + dbi.username + " ";
				    }
				    if (dbi.password != null)
				    {
					    p = p + "-p " + dbi.password + " ";
				    }
			    }
			    p = p + "-nosplash";	
				
				Process ssmsProcess = new Process();
				ssmsProcess.StartInfo.FileName = "ssms";
				ssmsProcess.StartInfo.Arguments = p;
				ssmsProcess.Start();
		    }
	    }

        public void ProcessingBeforeParameterInput(Command command, ref bool cancel)
        {
        }

        #endregion
    }
}
