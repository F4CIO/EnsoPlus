using System;
using System.Collections.Generic;
using System.Text;

using Extension;
using EnsoPlus;
using EnsoPlus.Entities;
using Common;
using System.IO;

namespace EnsoPlus.CommandsProviders.BackupManager
{
    class BackupManager : ICommandsProvider
    {
        #region ICommandsProvider Members

        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            

            commands.Add(new Command("set as backup source folder", "[folder]", "Defines which folder to compress with 7zip. See command: backup", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
                new ParameterInputArguments(/*Caption:*/"Profile", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false)
                /*Suggestions sources:*/
                
                ));

            commands.Add(new Command("set as backup destination folder", "[folder]", "Defines where to put 7zip archive. See command: backup", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
                new ParameterInputArguments(/*Caption:*/"Profile", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false)
                /*Suggestions sources:*/
                
                ));
            
            commands.Add(new Command("set as backup source folder", "for profile [profile name] [folder]", "Defines which folder to compress with 7zip. See command: backup", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
                new ParameterInputArguments(/*Caption:*/"Profile", null, /*Offer all suggestions:*/true, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
                /*Suggestions sources:*/
                typeof(WorkItemsProviders.BackupProfiles.BackupProfiles)
                ));

            commands.Add(new Command("set as backup destination folder", "for profile [profile name] [folder]", "Defines where to put 7zip archive. See command: backup", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
                new ParameterInputArguments(/*Caption:*/"Profile", null, /*Offer all suggestions:*/true, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
                /*Suggestions sources:*/
                typeof(WorkItemsProviders.BackupProfiles.BackupProfiles)
                ));

            commands.Add(new Command("backup", "[profile name]", "Creates 7zip and puts it in folder defined by backup profile.", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments(/*Caption:*/"Profile", null, /*Offer all suggestions:*/true, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/true,/*Case sensitive:*/false),
                            /*Suggestions sources:*/
                typeof(WorkItemsProviders.BackupProfiles.BackupProfiles)
                ));

            //commands.Add(new Command("commandWithNoPostfix", " ", "This is description", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/true, /*Can use file selection as parameter:*/true,
            //    new ParameterInputArguments()
            //    ));

            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("BackupManager: Executing command '{0}' ...", command.Name));
            if (command.Name == "set as backup source folder" && command.Postfix == "for profile [profile name] [folder]")
            {
                string name = command.parametersOnExecute[0].GetValueAsText();                
                string sourceFolder = command.parametersOnExecute[1].GetValueAsText();

                this.SetSourceFolderOnBackupProfile(service, command, name, sourceFolder);
            }
            else
                if (command.Name == "set as backup destination folder" && command.Postfix == "for profile [profile name] [folder]")
                {
                    string name = command.parametersOnExecute[0].GetValueAsText();                    
                    string destinationFolder = command.parametersOnExecute[1].GetValueAsText();

                    this.SetDestinationFolderOnBackupProfile(service, command, name, destinationFolder);
                }
                else
                    if (command.Name == "set as backup source folder" && command.Postfix == "[folder]")
                    {
                       
                        string sourceFolder = command.parametersOnExecute[0].GetValueAsText();
                        string name = PromptProfileSelectionDialog();
                        if (!string.IsNullOrEmpty(name))
                        {
                            this.SetSourceFolderOnBackupProfile(service, command, name, sourceFolder);
                        }
                    }
                    else
                        if (command.Name == "set as backup destination folder" && command.Postfix == "[folder]")
                        {                            
                            string destinationFolder = command.parametersOnExecute[0].GetValueAsText();
                            string name = PromptProfileSelectionDialog();
                            if (!string.IsNullOrEmpty(name))
                            {
                                this.SetDestinationFolderOnBackupProfile(service, command, name, destinationFolder);
                            }
                        }
                        else
                    if (command.Name == "backup")
                    {                        
                        BackupProfile backupProfile = command.parametersOnExecute[0].GetValue() as BackupProfile;                        
						throw new NotImplementedException(); //TODO: finish display async
						//string label = ParameterInput.Display("Label", new List<string>(), false, false, backupProfile.Name, false, false, null);
						//if (!string.IsNullOrEmpty(label))
						//{
						//	if(!Directory.Exists(backupProfile.SourceFolder)) 
						//	{
						//		MessagesHandler.Display( "Source folder does not exist.", backupProfile.SourceFolder);
						//	}else
						//	if(!Directory.Exists(backupProfile.DestinationFolder))
						//	{
						//		MessagesHandler.Display( "Destination folder does not exist.", backupProfile.DestinationFolder);
						//	}else
						//	{
						//		MessagesHandler.Display( string.Format("Backing up '{0}' to '{1}' ...", backupProfile.SourceFolder, backupProfile.DestinationFolder));
						//		//{EnsoPlusDataFolder}\7z.exe
						//		//a -r "{DestinationFolderPath}\{CurrentDateTime} {Label}\{SourceFolderName}" "{SourceFolderPath}";
						//		string parameters = Settings.Current.BackupManagerParameters;
						//		parameters = parameters.Replace("{DestinationFolderPath}", backupProfile.DestinationFolder);
						//		parameters = parameters.Replace("{CurrentDateTime}", CraftSynth.BuildingBlocks.Common.DateAndTime.GetCurrentDateAndTimeInSortableFormatForFileSystem());
						//		parameters = parameters.Replace("{Label}", label);
						//		parameters = parameters.Replace("{SourceFolderName}", Path.GetFileName(backupProfile.SourceFolder));
						//		parameters = parameters.Replace("{SourceFolderPath}", backupProfile.SourceFolder);
                                                        
						//		CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenFile(Settings.Current.BackupManagerExePath, parameters);
						//	}
						//}
                    }
                    else
                    {
                        throw new ApplicationException(string.Format("BackupManager: Command not found. Command: {0} {1}", command.Name, command.Postfix));
                    }
        }

        private string PromptProfileSelectionDialog()
        {
	        throw new NotImplementedException();
            var profilesNames = WorkItemsProviders.BackupProfiles.BackupProfiles.GetAllNames();
           // string name = ParameterInput.Display("Profile", profilesNames,true, false, "", false, false);
           // return name;
        }

        private void SetDestinationFolderOnBackupProfile(Extension.IEnsoService service, Command command, string name, string destinationFolder)
        {
            string filePath = WorkItemsProviders.BackupProfiles.BackupProfiles.GetFilePath(name);
            BackupProfile backupProfile = null;
            if (File.Exists(filePath))
            {
                backupProfile = WorkItemsProviders.BackupProfiles.BackupProfiles.Load(name);
            }
            else
            {
                backupProfile = new BackupProfile();
                backupProfile.Name = name;
            }
            backupProfile.DestinationFolder = destinationFolder;
            WorkItemsProviders.BackupProfiles.BackupProfiles.Save(backupProfile);

            if (string.IsNullOrEmpty(backupProfile.SourceFolder))
            {
                SuggestionsCache.DropCache(this.GetType());  
                MessagesHandler.Display( string.Format("Profile created. Use 'set as backup source...' command now.", command.Name));
            }
            else
            {
                MessagesHandler.Display( string.Format(string.Format("Profile updated. You can now use 'backup {0}' command to create backups quickly.", name)));
            }
        }

        private void SetSourceFolderOnBackupProfile(Extension.IEnsoService service, Command command, string name, string sourceFolder)
        {
            string filePath = WorkItemsProviders.BackupProfiles.BackupProfiles.GetFilePath(name);
            BackupProfile backupProfile = null;
            if (File.Exists(filePath))
            {
                backupProfile = WorkItemsProviders.BackupProfiles.BackupProfiles.Load(name);
            }
            else
            {
                backupProfile = new BackupProfile();
                backupProfile.Name = name;
            }
            backupProfile.SourceFolder = sourceFolder;
            WorkItemsProviders.BackupProfiles.BackupProfiles.Save(backupProfile);

            if (string.IsNullOrEmpty(backupProfile.DestinationFolder))
            {
                SuggestionsCache.DropCache(this.GetType());  
                MessagesHandler.Display( string.Format("Profile created. Use 'set as backup destination...' command now.", command.Name));
            }
            else
            {
                MessagesHandler.Display( string.Format(string.Format("Profile updated. You can now use 'backup {0}' command to create backups quickly.", name)));
            }
        }

        public void ProcessingBeforeParameterInput(Command command, ref bool cancel)
        {
        }

        #endregion
    }
}
