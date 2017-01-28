using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

using Extension;
using EnsoPlus;
using EnsoPlus.Entities;
using Common;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace EnsoPlus.CommandsProviders.FileManager
{
    class FileManager : ICommandsProvider
    {
        #region ICommandsProvider Members
        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            commands.Add(new Command("copy to", "[destination path]", "Copies selected file/folder (or selected path) to specified destination path (or choosed item)", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments(/*Caption:*/"copy to", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
                            /*Suggestions sources:*/
                typeof(WorkItemsProviders.Shortcuts.Shortcuts),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.ReflectionData.ReflectionData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("move to", "[destination path]", "Copies selected file/folder (or selected path) to specified destination path (or choosed item)", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments(/*Caption:*/"move to", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
                /*Suggestions sources:*/
                typeof(WorkItemsProviders.Shortcuts.Shortcuts),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.ReflectionData.ReflectionData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            //commands.Add(new Command("copy-to", "[destination path] [source path]", "Copies selected file/folder (or selected path) to specified path (or choosed item)", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
            //    new ParameterInputArguments(/*Caption:*/"copy to", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
            //    /*Suggestions sources:*/
            //    typeof(WorkItemsProviders.Shortcuts.Shortcuts),
            //    typeof(WorkItemsProviders.MemorizedData.MemorizedData),                
            //    typeof(WorkItemsProviders.ReflectionData.ReflectionData)
            //    ));

            //commands.Add(new Command("move to", "[destination path] [source path]", "Copies selected file/folder (or selected path) to specified path (or choosed item)", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
            //    new ParameterInputArguments(/*Caption:*/"move to", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
            //                /*Suggestions sources:*/
            //    typeof(WorkItemsProviders.Shortcuts.Shortcuts),
            //    typeof(WorkItemsProviders.MemorizedData.MemorizedData),
            //    typeof(WorkItemsProviders.ReflectionData.ReflectionData)
            //    ));

            commands.Add(new Command("empty", "[path]", "Deletes content of selected folder, path or item", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
                new ParameterInputArguments(/*Caption:*/"empty", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
                            /*Suggestions sources:*/
                typeof(WorkItemsProviders.Shortcuts.Shortcuts),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.ReflectionData.ReflectionData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("save", "[what] to", "Save selected item or work item to file path specified in Save dialog.", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments(/*Caption:*/"what to save", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
                            /*Suggestions sources:*/
                typeof(WorkItemsProviders.Shortcuts.Shortcuts),
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.CallerHistory.CallerHistory),
                typeof(WorkItemsProviders.Contacts.Contacts),
                typeof(WorkItemsProviders.DisplayMessagesHistory.DisplayMessagesHistory),
                typeof(WorkItemsProviders.Clipboard.ClipboardImage),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("save", "[what] and copy its file path", "Save selected item or work item to file path specified in Save dialog and copies that path to clipboard.", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
               new ParameterInputArguments(/*Caption:*/"what to save", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
                /*Suggestions sources:*/
               typeof(WorkItemsProviders.Shortcuts.Shortcuts),
               typeof(WorkItemsProviders.MemorizedData.MemorizedData),
               typeof(WorkItemsProviders.CallerHistory.CallerHistory),
               typeof(WorkItemsProviders.Contacts.Contacts),
               typeof(WorkItemsProviders.DisplayMessagesHistory.DisplayMessagesHistory),
               typeof(WorkItemsProviders.Clipboard.ClipboardImage),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
               ));


            //commands.Add(new Command("commandWithNoPostfix", " ", "This is description", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/true, /*Can use file selection as parameter:*/true,
            //    new ParameterInputArguments()
            //    ));

            commands.Add(new Command("copy file content as text", "[file path]", "Opens selected file (or file on selected path) and copies its content to clipboard", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
                new ParameterInputArguments(/*Caption:*/"File Path", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
                /*Suggestions sources:*/                
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("FileManager: Executing command '{0}' ...", command.Name));

            if (command.Name == "copy to" || command.Name == "move to")
            {                
                string selectedPath = service.GetUnicodeSelection();
                if (string.IsNullOrEmpty(selectedPath))
                {
                    selectedPath = service.GetFileSelection()[0];
                }

                if (string.IsNullOrEmpty(selectedPath))
                {
                    MessagesHandler.Display( "Nothing is selected therefore no destination path.");
                    Logging.AddErrorLog("FileManager: Nothing is selected therefore no destination path.");
                }
                else
                {

                    try
                    {
                        string destinationPath = Path.Combine(command.parametersOnExecute[0].GetValueAsText(), Path.GetFileName(selectedPath));

                        if (command.Name == "copy to")
                        {    
                            Logging.AddActionLog(string.Format("FileManager: Copying '{0}' to {1} ...",selectedPath, command.parametersOnExecute[0].GetValueAsText()));
                            CraftSynth.BuildingBlocks.IO.FileSystem.CopyFileOrFolder(selectedPath, command.parametersOnExecute[0].GetValueAsText());
                            if (CraftSynth.BuildingBlocks.IO.FileSystem.FileOrFolderExist(destinationPath))
                            {
                                Logging.AddActionLog("FileManager: Copied.");
                                MessagesHandler.Display( "Copied.");
                            }
                            else
                            {
                                string errorMessage = string.Format("Destination file/folder '{0}' does not exist after copy operation.", destinationPath);
                                Logging.AddErrorLog(string.Format("FileManager: {0}", errorMessage));
                                MessagesHandler.Display( errorMessage);
                            }
                        }
                        else
                            if (command.Name == "move to")
                            {
                                Logging.AddActionLog(string.Format("FileManager: Moving '{0}' to {1} ...", selectedPath, command.parametersOnExecute[0].GetValueAsText()));
                                CraftSynth.BuildingBlocks.IO.FileSystem.MoveFileOrFolder(selectedPath, command.parametersOnExecute[0].GetValueAsText());
                                if (CraftSynth.BuildingBlocks.IO.FileSystem.FileOrFolderDoesNotExist(destinationPath))
                                {
                                    string errorMessage = string.Format("Destination file/folder '{0}' does not exist after move operation.", destinationPath);
                                    Logging.AddErrorLog(string.Format("FileManager: {0}", errorMessage));
                                    MessagesHandler.Display( errorMessage);
                                }
                                else if (CraftSynth.BuildingBlocks.IO.FileSystem.FileOrFolderExist(selectedPath))
                                {
                                    string errorMessage = string.Format("Destination file/folder '{0}' still exist after move operation.", destinationPath);
                                    Logging.AddErrorLog(string.Format("FileManager: {0}", errorMessage));
                                    MessagesHandler.Display( errorMessage);
                                }
                                else
                                {
                                    Logging.AddActionLog("FileManager: Moved.");
                                    MessagesHandler.Display( "Moved.");
                                }
                            }
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        Logging.AddErrorLog(string.Format("FileManager: File not found: {0}", command.parametersOnExecute[1].GetValueAsText()));
                        MessagesHandler.Display( "File not found.", command.parametersOnExecute[1].GetValueAsText());
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {
                        Logging.AddErrorLog(string.Format("FileManager: Folder not found: {0}", command.parametersOnExecute[1].GetValueAsText()));
                        MessagesHandler.Display( "Folder not found.", command.parametersOnExecute[1].GetValueAsText());
                    }
                    catch (Exception exception)
                    {
                        Logging.AddErrorLog(string.Format("FileManager: {0}", exception.Message));
                        MessagesHandler.Display( exception.Message);
                    }
                }
                

            }
            else
                if (command.Name == "empty")
                {
                    string path = command.parametersOnExecute[0].GetValueAsText();
                    Logging.AddActionLog(string.Format("FileManager: Emptying '{0}' ...", path));

                    bool isFolder = true;
                        if (File.Exists(path) && !Directory.Exists(path))
                        {
                            isFolder = false;
                        }
                        else
                            if (!File.Exists(path) && Directory.Exists(path))
                            {
                                isFolder = true;
                            }

                    try
                    {
                        Logging.AddActionLog(string.Format("FileManager:   Deleting '{0}' ...", path));
                        CraftSynth.BuildingBlocks.IO.FileSystem.DeleteFileOrFolder(path, false);

                        if (!isFolder)
                        {
                            Logging.AddActionLog(string.Format("FileManager:   Creating '{0}' ...", path));
                            File.Create(path);
                        }
                        else
                        {
                            Logging.AddActionLog(string.Format("FileManager:   Creating '{0}' ...", path));
                            Directory.CreateDirectory(path);
                        }

                        Logging.AddActionLog("FileManager: Contents deleted.");
                        MessagesHandler.Display( "Contents deleted.");
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        Logging.AddErrorLog(string.Format("FileManager: File not found: {0}", command.parametersOnExecute[1].GetValueAsText()));
                        MessagesHandler.Display( "File not found.", command.parametersOnExecute[1].GetValueAsText());
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {
                        Logging.AddErrorLog(string.Format("FileManager: Folder not found: {0}", command.parametersOnExecute[1].GetValueAsText()));
                        MessagesHandler.Display( "Folder not found.", command.parametersOnExecute[1].GetValueAsText());
                    }
                    catch (Exception exception)
                    {
                        //if operation failed but item is empty report success
                        if (!isFolder && File.Exists(path))
                        {
                            FileInfo fileInfo = new FileInfo(path);
                            if (fileInfo.Length == 0)
                            {
                                exception = null;
                            }
                        }
                        else if(Directory.Exists(path))
                        {
                            if (CraftSynth.BuildingBlocks.IO.FileSystem.GetFilePaths(path).Count == 0 && CraftSynth.BuildingBlocks.IO.FileSystem.GetFolderPaths(path).Count == 0)
                            {
                                exception = null;
                            }
                        }

                        if (exception == null)
                        {
                            Logging.AddActionLog("FileManager: Contents deleted.");
                            MessagesHandler.Display( "Contents deleted.");
                        }
                        else
                        {
                            Logging.AddErrorLog(string.Format("FileManager: {0}", exception.Message));
                            MessagesHandler.Display( exception.Message);
                        }
                    }
                }
                else
                    if ((command.Name == "save" && command.Postfix == "[what] to")||
                        (command.Name == "save" && command.Postfix == "[what] and copy its file path"))
                    {

                        ParameterizedThreadStart ts = new ParameterizedThreadStart(ProcessCommand);
                        clipboardHost = new Thread(ts);
                        clipboardHost.SetApartmentState(ApartmentState.STA);
                        KeyValuePair<IEnsoService, Command> parameters = new KeyValuePair<IEnsoService, Command>(service, command);
                        clipboardHost.Start(parameters);
                        clipboardHost.Join();
                    }
                    else
                        if (command.Name == "copy file content as text")
                        {
                            string filePath = command.parametersOnExecute[0].GetValueAsText();
                            string content = File.ReadAllText(filePath);
                            if (string.Empty == (content.Trim()))
                            {
                                MessagesHandler.Display( string.Format("File is empty.", filePath));
                            }
                            else
                            {
								CraftSynth.BuildingBlocks.IO.Clipboard.SetTextToClipboard(content);

                                //content = BuildingBlocks.DataAccess.Clipboard.Helper.GetTextFromClipboard();
                                string message = content.Substring(0, (content.Length <= 200) ? content.Length : 200);
                                if (content.Length > 200)
                                {
                                    message = message + "...";
                                }

                                try
                                {
                                    MessagesHandler.Display( message, string.Format("{0} characters on clipboard", content.Length));
                                }
                                catch
                                {
                                    MessagesHandler.Display( string.Format("Copied {0} characters.", content.Length));
                                }
                            }
                        }  
                        
                        //    else
                //        if (command.Name == "command name" && command.Postfix == "postfix [item] [item2]")
                //        {
                //            MessagesHandler.Display( string.Format("Executing {0} ...", command.Name));

            //        }
                    else
                    {
                        throw new ApplicationException(string.Format("FileManager: Command not found. Command: {0} {1}", command.Name, command.Postfix));
                    }
        }

        private static Thread clipboardHost;
        private void ProcessCommand(Object o)
        {
            KeyValuePair<IEnsoService, Command> parameters = (KeyValuePair<IEnsoService, Command>)o;
            IEnsoService service = parameters.Key;
            Command command = parameters.Value;

            if (command.parametersOnExecute[0] is ImageWorkItem)
            {
                string targetFilePath = null;
                try
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.DefaultExt = ".png";
                    saveFileDialog.Filter = ".bmp|.bmp|.emf|.emf|.exif|.exif|.gif|.gif|.ico|.ico|.jpg|.jpg|.png|.png|.tif|.tif|.wmf|.wmf";
                    SetSaveFileDialogCommonOptions(saveFileDialog);

                    Thread bringToFrontAssistant = new Thread(BringToFront);
                    bringToFrontAssistant.Start(saveFileDialog.Title);

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Settings.Current.FileManagerSaveToDefaultFolder = Path.GetDirectoryName(saveFileDialog.FileName);
                        targetFilePath = saveFileDialog.FileName;
                        string targetExtension = Path.GetExtension(targetFilePath);
                        ImageFormat targetImageFormat = ImageFormat.Png;
                        if (string.Compare(targetExtension, ".bmp", true) == 0) targetImageFormat = ImageFormat.Bmp;
                        else
                            if (string.Compare(targetExtension, ".emf", true) == 0) targetImageFormat = ImageFormat.Emf;
                            else
                                if (string.Compare(targetExtension, ".exif", true) == 0) targetImageFormat = ImageFormat.Exif;
                                else
                                    if (string.Compare(targetExtension, ".gif", true) == 0) targetImageFormat = ImageFormat.Gif;
                                    else
                                        if (string.Compare(targetExtension, ".ico", true) == 0) targetImageFormat = ImageFormat.Icon;
                                        else
                                            if (string.Compare(targetExtension, ".jpg", true) == 0) targetImageFormat = ImageFormat.Jpeg;
                                            else
                                                if (string.Compare(targetExtension, ".png", true) == 0) targetImageFormat = ImageFormat.Png;
                                                else
                                                    if (string.Compare(targetExtension, ".tif", true) == 0) targetImageFormat = ImageFormat.Tiff;
                                                    else
                                                        if (string.Compare(targetExtension, ".wmf", true) == 0) targetImageFormat = ImageFormat.Wmf;

                        ImageWorkItem imageWorkItem = (ImageWorkItem)command.parametersOnExecute[0];
                        imageWorkItem.image.Save(targetFilePath, targetImageFormat);

                        Logging.AddActionLog(string.Format("FileManager: Image saved to '{0}'.", targetFilePath));
                        MessagesHandler.Display( string.Format("Saved.", command.Name));

                        if (command.Postfix == "[what] and copy its file path")
                        {
							CraftSynth.BuildingBlocks.IO.Clipboard.SetTextToClipboard(targetFilePath);
                            Logging.AddActionLog(string.Format("ClipboardManager: file path '{0}' copied to clipboard.", targetFilePath));
                            MessagesHandler.Display( "Image saved and its file path copied.");
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new ApplicationException(string.Format("FileManager: Failed to save image to '{0}'.", targetFilePath ?? "not set"), exception);

                }


            }
            else
                if ((command.parametersOnExecute[0] is Entities.StringWorkItem)||
                    (command.parametersOnExecute[0] is Entities.Shortcut)||
                    (command.parametersOnExecute[0] is Entities.Contact)||
                    (command.parametersOnExecute[0] is Entities.CallerHistoryItem)||
                    (command.parametersOnExecute[0] is Entities.MemorizedString))
                {
                    string targetFilePath = null;
                    try
                    {
                        MessagesHandler.Display(command.parametersOnExecute[0].GetValueAsText());

                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.DefaultExt = ".txt";
                        saveFileDialog.Filter = ".txt|.txt";
                        SetSaveFileDialogCommonOptions(saveFileDialog);

                        Thread bringToFrontAssistant = new Thread(BringToFront);
                        bringToFrontAssistant.Start(saveFileDialog.Title);

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            Settings.Current.FileManagerSaveToDefaultFolder = Path.GetDirectoryName(saveFileDialog.FileName);
                            targetFilePath = saveFileDialog.FileName;

                            File.WriteAllText(targetFilePath, command.parametersOnExecute[0].GetValueAsText());

                            Logging.AddActionLog(string.Format("FileManager: Text saved from work item to '{0}'.", targetFilePath));
                            MessagesHandler.Display( string.Format("Saved.", command.Name));

                            if (command.Postfix == "[what] and copy its file path")
                            {
								CraftSynth.BuildingBlocks.IO.Clipboard.SetTextToClipboard(targetFilePath);
                                Logging.AddActionLog(string.Format("ClipboardManager: file path '{0}' copied to clipboard.", targetFilePath));
                                MessagesHandler.Display( "Text saved and its file path copied.");
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        throw new ApplicationException(string.Format("FileManager: Failed to save text from work item to '{0}'.", targetFilePath ?? "not set"), exception);

                    }


                }
                else
                {
                    Logging.AddErrorLog(string.Format("FileManager: Tried to save unsupported work item type."));
                    MessagesHandler.Display( "Can not save work item of this type.");
                }
        }

        private static void SetSaveFileDialogCommonOptions(SaveFileDialog saveFileDialog)
        {
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.AddExtension = true;
            saveFileDialog.CreatePrompt = false;
            saveFileDialog.InitialDirectory = Settings.Current.FileManagerSaveToDefaultFolder;
            saveFileDialog.SupportMultiDottedExtensions = true;
            saveFileDialog.Title = "Save As";
            saveFileDialog.ValidateNames = true;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CheckFileExists = false;
            saveFileDialog.CheckPathExists = true;
        }

        private void BringToFront(object o)
        {            
            string caption = (string)o;
            int i = 5;
            while (i > 0)
            {
                i++;
                Thread.Sleep(1000);
				if (CraftSynth.BuildingBlocks.WindowsNT.Misc.SetForegroundWindow(caption)) i = 0;                
            }
        }

        public void ProcessingBeforeParameterInput(Command command, ref bool cancel)
        {
        }

        #endregion
    }
}
