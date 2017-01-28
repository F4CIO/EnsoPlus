using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CraftSynth.BuildingBlocks.Common;
using CraftSynth.BuildingBlocks.Logging;
using CraftSynth.Win32Clipboard;
using Extension;
using EnsoPlus.Entities;
using Common;
using System.IO;

namespace EnsoPlus
{
    public class EnsoPlus
    {
        public static EnsoPlus current;
        public FormMain formMain;
        private IEnsoService _service;
        private IEnsoService service
        {
            get
            {
                return _service;
            }
            set
            {
                if (_service == null)
                {
                    _service = value;
                    InitializeAfterServiceWasSet();
                }
                else
                {
                    _service = value;
                }
            }
        }

        private List<ICommandsProvider> commandsProviders;
        private List<Command> commands;
        private Dictionary<string , MergedCommand> mergedCommands;


        public EnsoPlus()
        {            
            current = this;
            //current.formMain = new FormMain();
            //formMain.Text = "Enso+";
            Initialize();       
        }

		

        private void Initialize()
        {
            OSD.OSD.SettingsFilePath = Path.Combine(CraftSynth.BuildingBlocks.UI.Console.ApplicationPhysicalPath, "OSD.ini");
            PopulateCommandsProviders();
            PopulateCommands();
            MergeCommands();
            SuggestionsCache.DropAllCache();
			SuggestionsCache.BuildAllCache();
        }

        private void InitializeAfterServiceWasSet()
        {
            
            string version = CraftSynth.BuildingBlocks.Common.Misc.version ?? string.Empty;

            bool success = false;
            while (!success)
            {
                try
                {
                    string versionFile = Path.Combine(Common.Helper.GetEnsoPlusWorkingFolder(), "Version.txt");
                    if (version == "1.0.0.0")
                    {
                        if (File.Exists(versionFile))
                        {
                            version = File.ReadAllText(versionFile);
                        }
                        else
                        {
                            version = "-by F4CIO-";
                        }
                    }
                    else
                    {
                        File.WriteAllText(versionFile, version);
                    }
                    MessagesHandler.Display("Welcome to Enso+ !", version);
                    success = true;
                }
                catch (Exception exception)
                {
                    if (exception.InnerException.Message == "Unable to connect to the remote server")
                    {
                        System.Threading.Thread.Sleep(2000);
                    }
                    else
                    {
                        throw exception;
                    }
                }
            }
        }

        private void PopulateCommandsProviders()
        {
            this.commandsProviders = new List<ICommandsProvider>();

            this.commandsProviders.Add(new CommandsProviders.Memorizer.Memorizer() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.Opener.Opener() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.TOpener.TOpener() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.FOpener.FOpener() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.COpener.COpener() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.IOpener.IOpener() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.Caller.Caller() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.WebSearcher.WebSearcher() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.ClipboardManager.ClipboardManager() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.MacroManager.MacroManager() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.Misc.Misc() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.ProcessesManager.ProcessesManager() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.MessagesDisplayer.MessagesDisplayer() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.CommandsManager.CommandsManager() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.FileManager.FileManager() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.BookmarkManager.BookmarkManager() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.Comparer.Comparer() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.SelectionListenerCommandsProvider.SelectionListenerCommandsProvider() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.BackupManager.BackupManager() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.F4CIOsPortControl.F4CIOsPortControl() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.RdcOpener.RdcOpener() as ICommandsProvider);
            this.commandsProviders.Add(new CommandsProviders.SsmsOpener.SsmsOpener() as ICommandsProvider);
        }

        private void PopulateCommands()
        {
            this.commands = new List<Command>();
            foreach (ICommandsProvider commandsProvider in this.commandsProviders)
            {
                commands.AddRange(commandsProvider.GetCommands());
            }
        }

        private void MergeCommands()
        {
            this.mergedCommands = new Dictionary<string, MergedCommand>();

            Dictionary<string, List<Command>> commandsByName = new Dictionary<string, List<Command>>();

            foreach (Command command in this.commands)
            {
                if (!commandsByName.ContainsKey(command.Name))
                {
                    commandsByName[command.Name] = new List<Command>();
                }
                commandsByName[command.Name].Add(command);
            }

            foreach (List<Command> commandsWithSameName in commandsByName.Values)
            {
                string mergedCommandPostfix = string.Empty;
                string mergedCommandDescription = string.Empty;
                List<ICommandsProvider> commandsProviders = new List<ICommandsProvider>();
                List<Command> sourceCommands = new List<Command>();
                foreach (Command commandInGroup in commandsWithSameName)
                {
                    mergedCommandPostfix += string.Format("{0} | ", commandInGroup.Postfix);
                    mergedCommandDescription += string.Format("{0} | ", commandInGroup.Description);
                    if (!commandsProviders.Contains(commandInGroup.provider))
                    {
                        commandsProviders.Add(commandInGroup.provider);
                    }
                    if (!sourceCommands.Contains(commandInGroup))
                    {
                        sourceCommands.Add(commandInGroup);
                    }
                }
                mergedCommandPostfix = mergedCommandPostfix.Remove(mergedCommandPostfix.LastIndexOf(" | "));
                mergedCommandDescription = mergedCommandDescription.Remove(mergedCommandDescription.LastIndexOf(" | "));

                MergedCommand mergedCommand = new MergedCommand();
                mergedCommand.Name = commandsWithSameName[0].Name;
                mergedCommand.Postfix = mergedCommandPostfix;
                mergedCommand.Description = mergedCommandDescription;
                mergedCommand.Help = null;
                mergedCommand.PostfixType = EnsoPostfixType.Arbitrary;
                mergedCommand.provider = commandsProviders[0];
                mergedCommand.commandsProviders = commandsProviders;
                mergedCommand.sourceCommands = sourceCommands;
                this.mergedCommands.Add(commandsWithSameName[0].Name, mergedCommand);
            }

        }

        public void Reinitialize()
        {
            if (this.service != null && this.mergedCommands != null)
            {
                this.Initialize();
            }
        }

	    public Dictionary<string, MergedCommand> GetMergedCommands()
	    {
		    return this.mergedCommands;
	    } 

        #region IEnsoExtension Members

        private static void PrepareHelp(EnsoCommand command)
        {
            if (string.IsNullOrEmpty(command.Help))
            {
                command.Help = command.Description.Replace("&", "_").Replace(@"/", "_").Replace(@"\", "_").Replace(@"#", "_").Replace(@"<", "_").Replace(@">", "_");
            }
            if (string.IsNullOrEmpty(command.Help))
            {
                command.Help = "No help specified.";
            }
        }

        public void OnCommand(EnsoCommand ensoCommand, string postfix, IntPtr foregroundWindowForGrab)
        {
			Logging.AddDebugLog("OnCommand:"+ensoCommand.Name+" postfix:"+postfix+"------------------------------------------------------------------------------------------------");
			try
			{
                //SelectionListener.Listener.Current.IfOpenedPause();

                MergedCommand mergedCommand = this.mergedCommands[ensoCommand.Name];

                //if none of potentially chosen commands use selection skip expensive operations:
                //  this.service.GetFileSelection() and/or this.service.GetUnicodeSelection();
                bool skipGetUnicodeSelection = true;
                bool skipGetFileSelection = true;
                foreach (var sourceCommand in mergedCommand.sourceCommands)
                {
                    if (sourceCommand.canUseTextSelectionForParameter)
                    {
                        skipGetUnicodeSelection = false;                        
                    }

                    if (sourceCommand.canUseFileSelectionForParameter)
                    {
                        skipGetFileSelection = false;
                    }

                    if (!skipGetUnicodeSelection && !skipGetFileSelection)
                    {
                        break;
                    }
                }               
				Logging.AddDebugLog("OnCommand: skipGetUnicodeSelection="+skipGetUnicodeSelection+" skipGetFilesSelection="+skipGetFileSelection);
               
				//do we need file selection?
				if ((ensoCommand.Name == "topen" || ensoCommand.Name == "rtopen" || ensoCommand.Name == "empty") )
                {
	                try
	                {
		                if (CraftSynth.BuildingBlocks.WindowsNT.Misc.GetForegroundWindowCaption().Contains("Total Commander"))
		                {
			                skipGetFileSelection = true;
		                }
	                }
	                catch
	                {
	                }
                }
				
				//read selection
				ClipboardData selectedData = null;
				if (!skipGetUnicodeSelection || !skipGetFileSelection)
				{
					selectedData = HandlerForSelection.Get(foregroundWindowForGrab);
				}
				Logging.AddDebugLog("OnCommand: first 100 chars of HandlerForSelection.Get=" + (selectedData==null?"null":selectedData.AsUnicodeText).FirstXChars(100,"..."));

				//Get text selection if needed   
				string selectedText = null;
                if (!skipGetUnicodeSelection && selectedData!=null)
                {
	                selectedText = selectedData.AsUnicodeText;
                }             
				Logging.AddDebugLog("OnCommand: first 100 chars of selectedText=" + (selectedText??"null").FirstXChars(100,"..."));
			
                //Get file selection if needed
                string[] fileSelectionArray = null;
                if( !skipGetFileSelection &&
                    !skipGetUnicodeSelection && string.IsNullOrEmpty(selectedText) && selectedData!=null)
                {
	                var ffl = selectedData.AsFileFolderList;
	                if (ffl == null)
	                {
		                fileSelectionArray = null;
	                }
					else
	                {
		                fileSelectionArray = ffl.ToArray();
	                }
	                ; //ex: this.service.GetFileSelection();
                }else
                {
                    fileSelectionArray = new string[] { };
                }              
				Logging.AddDebugLog("OnCommand: first 100 chars of fileSelectionArray=" + (fileSelectionArray==null? "null": Syntax.FileSelectionArrayToString(fileSelectionArray)).FirstXChars(100,"..."));

                if (fileSelectionArray!=null && fileSelectionArray.Length > 0 && string.IsNullOrEmpty(selectedText) )
                {
                    selectedText = Syntax.FileSelectionArrayToString(fileSelectionArray);
                }             
                
                Command bestCandidateForUsedCommand = null;                
                bool bestCandidateForUsedCommandRequiresParameterInput = false;                
                foreach (Command sourceCommand in mergedCommand.sourceCommands)
                {
					Logging.AddDebugLog("OnCommand: Syntax.ExtractParameterValues...");
                    int parameterCountInSyntax = 0;
                    List<string> parametersFromInlineCommand = Syntax.ExtractParameterValues(sourceCommand.Postfix, postfix, (GetSelectionForCommand(selectedText, sourceCommand)), mergedCommand.sourceCommands.Count == 1, out parameterCountInSyntax);
					
					Logging.AddDebugLog("OnCommand: parametersFromInlineCommand="+(parametersFromInlineCommand==null?"null":parametersFromInlineCommand.ToCSV()));
                    if (parametersFromInlineCommand == null)
                    {
                        continue;
                    }        
					
                    //replace jockers - should be refactored
	                Logging.AddDebugLog("OnCommand: replace jockers...");
                    int i = parametersFromInlineCommand.Count - 1;
                    while (i >= 0)
                    {
                        if (parametersFromInlineCommand[i] == Syntax.lastMessageInPostfix)
                        {
                            parametersFromInlineCommand[i] = MessagesHandler.GetLastFromHistory().Text;
                        } 
                        else if (parametersFromInlineCommand[i] == Syntax.selectionInPostfix1 || parametersFromInlineCommand[i] == Syntax.selectionInPostfix2)
                        {
                            parametersFromInlineCommand.RemoveAt(i);
                        }
                        i--;
                    }
					Logging.AddDebugLog("OnCommand: Determine best candidate...");

                    if ((GetSelectionForCommand(selectedText, sourceCommand) == string.Empty && parametersFromInlineCommand.Count == parameterCountInSyntax) ||
                        (parameterCountInSyntax==0 &&  sourceCommand.Postfix==" ") ||
                        (GetSelectionForCommand(selectedText, sourceCommand) != string.Empty && parametersFromInlineCommand.Count == parameterCountInSyntax && GetSelectionForCommand(selectedText, sourceCommand).CompareTo(parametersFromInlineCommand[parametersFromInlineCommand.Count - 1]) == 0))
                    {
                        bestCandidateForUsedCommand = sourceCommand.GetClone();
                        bestCandidateForUsedCommand.parametersOnExecute = StringWorkItem.CreateInstances(parametersFromInlineCommand);
                        bestCandidateForUsedCommandRequiresParameterInput = false;
						Logging.AddDebugLog("OnCommand: bc=a");
                        break;
                    }
                    else if (GetSelectionForCommand(selectedText, sourceCommand) != string.Empty && parametersFromInlineCommand.Count == parameterCountInSyntax && GetSelectionForCommand(selectedText, sourceCommand).CompareTo(parametersFromInlineCommand[parametersFromInlineCommand.Count - 1]) != 0)
                    {
                        bestCandidateForUsedCommand = sourceCommand.GetClone();
                        bestCandidateForUsedCommand.parametersOnExecute = StringWorkItem.CreateInstances(parametersFromInlineCommand);
                        bestCandidateForUsedCommandRequiresParameterInput = false;
						Logging.AddDebugLog("OnCommand: bc=b");
                    }
                    else if (parametersFromInlineCommand.Count == (parameterCountInSyntax - 1))
                    {
                        bestCandidateForUsedCommand = sourceCommand.GetClone();
                        bestCandidateForUsedCommand.parametersOnExecute = StringWorkItem.CreateInstances(parametersFromInlineCommand);
                        bestCandidateForUsedCommandRequiresParameterInput = true;
						Logging.AddDebugLog("OnCommand: bc=c");
                    }
                }

               

                if (bestCandidateForUsedCommand == null)
                {
					Logging.AddDebugLog("OnCommand: postfix Invalid!");
                    MessagesHandler.Display( "Postfix invalid!", ensoCommand.Name + " " + ensoCommand.Postfix);
                }
                else
                {
					Logging.AddDebugLog("OnCommand: bestCandidateForUsedCommand=" + bestCandidateForUsedCommand.Name);
					Logging.AddDebugLog("OnCommand: replace 'last' parameter with last used parameter/WorkItem");
					//replace 'last' parameter with last used parameter/WorkItem
					int j=0;
					while(j<bestCandidateForUsedCommand.parametersOnExecute.Count)
					{
						Logging.AddDebugLog("OnCommand: first 100 chars of paramsOnExecute[" +j+"]="+(bestCandidateForUsedCommand.parametersOnExecute[j].GetValueAsText().FirstXChars(100,"...")));
						if (bestCandidateForUsedCommand.parametersOnExecute[j].GetValueAsText() == Syntax.lastParameterInPostfix)
						{
							bestCandidateForUsedCommand.parametersOnExecute[j] = WorkItemsProviders.CommandsHistory.CommandsHistory.GetLastWorkItem();
							Logging.AddDebugLog("OnCommand: first 100 chars of paramsOnExecute[" +j+"]="+bestCandidateForUsedCommand.parametersOnExecute[j].GetValueAsText().FirstXChars(100,"..."));
						}
						
						j++;
					}

                    if (bestCandidateForUsedCommand.parameterInputArguments.acceptOnlySuggested && !bestCandidateForUsedCommandRequiresParameterInput && bestCandidateForUsedCommand.parametersOnExecute.Count > 0)
                    {//user entered all parameters and command uses cloased parameter group
						Logging.AddDebugLog("OnCommand: user entered all parameters and command uses cloased parameter group");
						Dictionary<string,IWorkItem> suggestions = GetAvailableSuggestions(bestCandidateForUsedCommand);
                        IWorkItem selectedSuggestion = null;
                        if (suggestions.TryGetValue(bestCandidateForUsedCommand.parametersOnExecute[bestCandidateForUsedCommand.parametersOnExecute.Count - 1].GetCaption(), out selectedSuggestion))
                        {//user-entered parameter does not exist in group - add it to list
							Logging.AddDebugLog("OnCommand: user-entered parameter does not exist in group - add it to list");
                            IWorkItem postProcessedSuggestion = PostProcessSelectedSuggestion(selectedSuggestion);
                            if (postProcessedSuggestion == null)
                            {//user probably canceled command - abort command
								Logging.AddDebugLog("OnCommand: user probably canceled command - abort command");
                                return;
                            }
                            bestCandidateForUsedCommand.parametersOnExecute[bestCandidateForUsedCommand.parametersOnExecute.Count - 1] = postProcessedSuggestion;
                        }
                        else
                        {//user-entered parameter does not exist in group - plan input parameter box
							Logging.AddDebugLog("OnCommand: user-entered parameter does not exist in group - plan input parameter box");
	                        if (bestCandidateForUsedCommand.parameterInputArguments.acceptOnlySuggested)
	                        {
		                        bestCandidateForUsedCommand.parameterInputArguments.predefinedValue = string.Empty;
	                        }
							else { 
								bestCandidateForUsedCommand.parameterInputArguments.predefinedValue = bestCandidateForUsedCommand.parametersOnExecute[bestCandidateForUsedCommand.parametersOnExecute.Count - 1].GetCaption();
                            }
							bestCandidateForUsedCommand.parametersOnExecute.RemoveAt(bestCandidateForUsedCommand.parametersOnExecute.Count - 1);
                            bestCandidateForUsedCommandRequiresParameterInput = true;
                        }  
                    }

	                if (!bestCandidateForUsedCommandRequiresParameterInput)
	                {Logging.AddDebugLog("OnCommand: bestCandidateForUsedCommandRequiresParameterInput==false");
	                    Logging.AddDebugLog("OnCommand: ExecuteCommandCandidate...");
						ExecuteCommandCandidate(bestCandidateForUsedCommand);
	                }
					else
					{
						Logging.AddDebugLog("OnCommand: ProcessingBeforeParameterInput...");
						bool cancel = false;
                        bestCandidateForUsedCommand.provider.ProcessingBeforeParameterInput(bestCandidateForUsedCommand, ref cancel);
						Logging.AddDebugLog("OnCommand: ProcessingBeforeParameterInput done. cancel="+cancel);
						if (cancel) return;

						Logging.AddDebugLog("OnCommand: GetAvailableSuggestions...");
                        Dictionary<string, IWorkItem> suggestions = GetAvailableSuggestions(bestCandidateForUsedCommand);

                        //prepare parameters to suggestions
                        bestCandidateForUsedCommand.parameterInputArguments.suggestions = new List<string>();
                        foreach (var suggestion in suggestions)
                        {
                            bestCandidateForUsedCommand.parameterInputArguments.suggestions.Add(suggestion.Key);
                        }

                        //execute dropbox
						Logging.AddDebugLog("OnCommand: execute dropbox...");
                        try
                        {
							PostParameterInputArguments contextData  = new PostParameterInputArguments();
	                        contextData.suggestions = suggestions;
	                        contextData.bestCandidateForUsedCommand = bestCandidateForUsedCommand;
	                        contextData.bestCandidateForUsedCommandRequiresParameterInput = bestCandidateForUsedCommandRequiresParameterInput;
							
                            ParameterInput.Display(bestCandidateForUsedCommand.parameterInputArguments, ParameterInput_OnClose, contextData, Screen.FromPoint(Cursor.Position));
                        }
                        catch (Exception exception)
                        {
                            MessagesHandler.Display( "Error", exception.Message);
                            Logging.AddErrorLog("Parameter input failed: " + exception.Message + ((exception.InnerException != null) ? ":" + exception.InnerException.Message : ""));
							Common.Logging.AddExceptionLog(exception);
                        }
                    }

                }

			}
			catch (Exception exception)
			{
				Logging.AddErrorLog("Command execution failed: " + exception.Message + ((exception.InnerException != null) ? ":" + exception.InnerException.Message : ""));
				Logging.AddExceptionLog(exception);
				throw exception;
			}
            //SelectionListener.Listener.Current.IfOpenedContinue();
        }
	  
		private void ParameterInput_OnClose(string result, object contextData)
	    {
		    PostParameterInputArguments cd = contextData as PostParameterInputArguments;
			cd.selectedSuggestionCaption = result;

			//determine selected parameter
			Logging.AddDebugLog("ParameterInput_OnClose: first 100 chars of cd.selectedSuggestionCaption="+(result??"null").FirstXChars(100,"..."));
			bool abort = false;
			if (!string.IsNullOrEmpty(cd.selectedSuggestionCaption))
			{
				IWorkItem selectedSuggestion = null;
				if (cd.suggestions.TryGetValue(cd.selectedSuggestionCaption, out selectedSuggestion))
				{//user has selected suggestion   
					Logging.AddDebugLog("ParameterInput_OnClose: user has selected suggestion");         
					IWorkItem postProcessedSelectedSuggestion = PostProcessSelectedSuggestion(selectedSuggestion);
					if (postProcessedSelectedSuggestion == null)
					{//user probably canceled command - abort command
						Logging.AddDebugLog("ParameterInput_OnClose: user probably canceled command - abort command");
						abort = true;
					}
					cd.bestCandidateForUsedCommand.parametersOnExecute.Add(postProcessedSelectedSuggestion);
				}
				else
				{//user typed free text
					Logging.AddDebugLog("ParameterInput_OnClose: user typed free text");
					cd.bestCandidateForUsedCommand.parametersOnExecute.Add(new StringWorkItem(cd.selectedSuggestionCaption));
				}

				cd.bestCandidateForUsedCommandRequiresParameterInput = false;
			}
			
			if(!abort)
			{
				if (!cd.bestCandidateForUsedCommandRequiresParameterInput)
				{
					Logging.AddDebugLog("ParameterInput_OnClose: ExecuteCommandCandidate...");
					ExecuteCommandCandidate(cd.bestCandidateForUsedCommand);
				}
		    }
	    }  
		
		private void ExecuteCommandCandidate(Command bestCandidateForUsedCommand)
	    {
		    try
		    {
			    if (bestCandidateForUsedCommand.Name != "repeat")
			    {
				    WorkItemsProviders.CommandsHistory.CommandsHistory.Add(bestCandidateForUsedCommand);
			    }
			    bestCandidateForUsedCommand.Execute(service);
		    }
		    catch (Exception exception)
		    {
			    MessagesHandler.Display("Command execution failed", exception.Message);
			    Logging.AddErrorLog("Command execution failed: " + exception.Message +
			                        ((exception.InnerException != null)? ":" + exception.InnerException.Message + ((exception.InnerException.InnerException != null)? ":" + exception.InnerException.InnerException.Message: ""): "")
				    );
			    Common.Logging.AddExceptionLog(exception);
		    }
	    }

	    private static string GetSelectionForCommand(string selectedText, Command sourceCommand)
        {
            return sourceCommand.canUseTextSelectionForParameter || sourceCommand.canUseFileSelectionForParameter ? selectedText??string.Empty : string.Empty;
        }

        private IWorkItem PostProcessSelectedSuggestion(IWorkItem selectedSuggestion)
        {
            IWorkItem postProcessedSelectedSuggestion = null;
            try
            {                
                postProcessedSelectedSuggestion = selectedSuggestion.GetProvider().GetParameterFromSelectedSuggestion(selectedSuggestion);                
            }
            catch (Exception exception)
            {
                MessagesHandler.Display( "Selected suggestion postprocess failed", exception.Message);
                Logging.AddErrorLog("Selected suggestion postprocess failed: " + exception.Message + ((exception.InnerException != null) ? ":" + exception.InnerException.Message : ""));
				Common.Logging.AddExceptionLog(exception);
                postProcessedSelectedSuggestion = null;
            }
            return postProcessedSelectedSuggestion;
        }

        private Dictionary<string,IWorkItem> GetAvailableSuggestions(Command bestCandidateForUsedCommand)
        {
			MessagesHandler.Display("Retrieving suggestions. Please wait...");
            Dictionary<string, IWorkItem> suggestions = new Dictionary<string, IWorkItem>();
            
            foreach (Type supportedParameterTypeProvider in bestCandidateForUsedCommand.supportedParameterTypeProviders)
            {
                try
                {
					Application.DoEvents();
                    IWorkItemsProvider ptp = Assembly.GetExecutingAssembly().CreateInstance(supportedParameterTypeProvider.FullName) as IWorkItemsProvider;                    
                    foreach (KeyValuePair<string, IWorkItem> kv in SuggestionsCache.Get(ptp))
                    {
                        IWorkItem existingWorkItemWithSameName = null;
                        if (suggestions.TryGetValue(kv.Key, out existingWorkItemWithSameName))
                        {
                            string message = string.Format("Can't add item '{0}' from provider '{1}' because item with same name allready loaded from provider {2}.",
                                                kv.Key,
                                                kv.Value.GetProvider().GetType().Name,
                                                existingWorkItemWithSameName.GetProvider().GetType().Name);
                            Logging.AddErrorLog(message);
                            //MessagesHandler.Display(message);
                        }
                        else
                        {
                            suggestions.Add(kv.Key, kv.Value);
                        }
                    }
					
                }
                catch (Exception exception)
                {
                    MessagesHandler.Display( "Error occured while retriving suggestions", exception.Message);
                    Logging.AddErrorLog("Error occured while retriving suggestions: " + exception.Message + ((exception.InnerException != null) ? ":" + exception.InnerException.Message : ""));
					Common.Logging.AddExceptionLog(exception);
                }
            }
			MessagesHandler.HideAllMessages();
            return suggestions;
        }
		#endregion

	    public static void NotifyOtherApplicationsAboutClipboardChange()
	    {
		    if (CraftSynth.BuildingBlocks.Common.Misc.GetProcessesByNamePart("WheelOfHistory").Count() > 0)
		    {
			    EventWaitHandle doneWithInit = new EventWaitHandle(false, EventResetMode.ManualReset, "WOHCopyWaitHandle");
			    doneWithInit.Set();
		    }
	    }
    }

	public class PostParameterInputArguments
	{
		public string selectedSuggestionCaption;
		public Dictionary<string, IWorkItem> suggestions;
		public Command bestCandidateForUsedCommand;
		public bool bestCandidateForUsedCommandRequiresParameterInput;
	}
}
