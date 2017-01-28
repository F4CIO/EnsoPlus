using System;
using System.Collections.Generic;
using System.Text;

using Extension;
using EnsoPlus;
using EnsoPlus.Entities;
using Common;
using System.IO;
using System.Threading;

namespace EnsoPlus.CommandsProviders.BookmarkManager
{
    class BookmarkManager : ICommandsProvider
    {
        private IEnsoService service;

        public static void SaveLinkFile(string filePath, string targetUrl)
        {
            string[] contents = new string[] { "[InternetShortcut]", "URL=" + targetUrl };
            File.WriteAllLines(filePath, contents);
        }

        #region ICommandsProvider Members

        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            commands.Add(new Command("bookmark as", "[name] [selected url]", "Saves selected url under desired name and in desired category.", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/true,/*Can use file selection as parameter:*/true,
                new ParameterInputArguments(/*Caption:*/"Bookmark Name", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
                /*Suggestions sources:*/
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("bookmark all tabs as", "[name]", "Saves urls of all tabs under desired name and in desired category.", null, EnsoPostfixType.Arbitrary, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments(/*Caption:*/"Bookmark Name", null, /*Offer all suggestions:*/false, /*Read only:*/false, /*predefined value:*/"", /*Accept only suggested:*/false,/*Case sensitive:*/false),
                /*Suggestions sources:*/
                typeof(WorkItemsProviders.MemorizedData.MemorizedData),
                typeof(WorkItemsProviders.Clipboard.ClipboardText)
                ));

            commands.Add(new Command("bookmark all tabs in one category", " ", "Saves urls of all tabs as separate bookmarks in same category.", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false,/*Can use file selection as parameter:*/false,
                new ParameterInputArguments()
                ));

            commands.Add(new Command("open bookmark", " ", "Shows bookmark browser and opens selected item.", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/false, /*Can use file selection as parameter:*/false,
                new ParameterInputArguments()
                ));

            //commands.Add(new Command("commandWithNoPostfix", " ", "This is description", null, EnsoPostfixType.None, this, /*Can use text selection as parameter:*/true, /*Can use file selection as parameter:*/true,
            //    new ParameterInputArguments()
            //    ));

            return commands;
        }

        public void ExecuteCommand(Extension.IEnsoService service, Command command)
        {
            Logging.AddActionLog(string.Format("BookmarkManager: Executing command '{0}' ...", command.Name));
            if (command.Name == "bookmark as")
            {
                //MessagesHandler.Display( "Loading bookmark folders ...");

                this.service = service;
                OSD.OSD.SettingsFilePath = "OSD.Bookmarks.ini";
                OSD.Menu.Menu bookmarkMenu = new OSD.Menu.Menu(null, "Bookmarks", Settings.Current.EnsoLearnAsOpenCommandsFolder, false, false, false, false, "[New Category]", false, new OSD.Menu.MenuItemChosenDelegate(MenuItemChoosen));

                Thread bringToFrontAssistant = new Thread(BringToFront);
                bringToFrontAssistant.Start("OSD");

                OSD.Menu.MenuItem selectedMenuItem = OSD.Menu.Menu.ShowMenu(bookmarkMenu);
                if (selectedMenuItem != null)
                {
                    string bookmarkFilePath = Path.Combine((string)selectedMenuItem.tag, command.parametersOnExecute[0].GetValueAsText() + ".url");
                    BookmarkManager.SaveLinkFile(bookmarkFilePath, command.parametersOnExecute[1].GetValueAsText());
                    string message = command.parametersOnExecute[1].GetValueAsText();                    
                    MessagesHandler.Display( string.Format("{0} saved as bookmark.", message));
                }

            }else
            if (command.Name == "bookmark all tabs as")
            {
                this.service = service;
                OSD.OSD.SettingsFilePath = "OSD.Bookmarks.ini";
                OSD.Menu.Menu bookmarkMenu = new OSD.Menu.Menu(null, "Bookmarks", Settings.Current.EnsoLearnAsOpenCommandsFolder, false, false, false, false, "[New Category]", false, new OSD.Menu.MenuItemChosenDelegate(MenuItemChoosen));
                
                Thread bringToFrontAssistant = new Thread(BringToFront);
                bringToFrontAssistant.Start("OSD");

                OSD.Menu.MenuItem selectedMenuItem = OSD.Menu.Menu.ShowMenu(bookmarkMenu);

                if (selectedMenuItem != null)
                {
                    //string bookmarkFilePath = Path.Combine((string)selectedMenuItem.tag, command.parametersOnExecute[0].GetCaption() + "."+Entities.WorkItems.Shortcuts.extension);
                    CommandsProviders.FOpener.FOpener.SaveAllTabs(true, service, command.parametersOnExecute[0].GetCaption(), (string)selectedMenuItem.tag, "BookmarkManager");
                }
            }else
            if (command.Name == "bookmark all tabs in one category")
            {
                this.service = service;
                OSD.OSD.SettingsFilePath = "OSD.Bookmarks.ini";
                OSD.Menu.Menu bookmarkMenu = new OSD.Menu.Menu(null, "Bookmarks", Settings.Current.EnsoLearnAsOpenCommandsFolder, false, false, false, false, "[New Category]", false, new OSD.Menu.MenuItemChosenDelegate(MenuItemChoosen));

                Thread bringToFrontAssistant = new Thread(BringToFront);
                bringToFrontAssistant.Start("OSD");

                OSD.Menu.MenuItem selectedMenuItem = OSD.Menu.Menu.ShowMenu(bookmarkMenu);

                if (selectedMenuItem != null)
                {
                    //string bookmarkFilePath = Path.Combine((string)selectedMenuItem.tag, command.parametersOnExecute[0].GetCaption() + "."+Entities.WorkItems.Shortcuts.extension);
                    CommandsProviders.FOpener.FOpener.SaveAllTabs(false, service, Path.GetFileNameWithoutExtension(selectedMenuItem.tag.ToString()), (string)selectedMenuItem.tag, "BookmarkManager");
                    
                }
            }
            else if (command.Name == "open bookmark")
            {
                
                //MessagesHandler.Display( "Loading bookmark folders ...");
                OSD.OSD.SettingsFilePath = "OSD.Bookmarks.ini";
                OSD.Menu.Menu bookmarkMenu = new OSD.Menu.Menu(null, "Bookmarks", Settings.Current.EnsoLearnAsOpenCommandsFolder, true, false, true, false, "[All From This Category]", false, null);

                Thread bringToFrontAssistant = new Thread(BringToFront);
                bringToFrontAssistant.Start("OSD");

                OSD.Menu.MenuItem selectedMenuItem = OSD.Menu.Menu.ShowMenu(bookmarkMenu);
                if (selectedMenuItem != null)
                {
                    List<string> filePathsToOpen = new List<string>();
                    if (selectedMenuItem.text == "[All From This Category]")
                    {
                        List<string> filePathsInFolder = CraftSynth.BuildingBlocks.IO.FileSystem.GetFilePaths(selectedMenuItem.tag.ToString());
                        foreach (string filePathInFolder in filePathsInFolder)
                        {
                            if(filePathInFolder.EndsWith(Entities.WorkItems.Shortcuts.extension, StringComparison.InvariantCultureIgnoreCase)||
                                filePathInFolder.EndsWith("lnk", StringComparison.InvariantCultureIgnoreCase)||
                                filePathInFolder.EndsWith("url", StringComparison.InvariantCultureIgnoreCase)
                                )
                            {
                                filePathsToOpen.Add(filePathInFolder);
                            }
                        }
                    }
                    else
                    {
                        filePathsToOpen.Add(selectedMenuItem.tag.ToString());
                    }

                    foreach (string filePathToOpen in filePathsToOpen)
                    {
                        if (filePathToOpen.EndsWith(Entities.WorkItems.Shortcuts.extension, StringComparison.OrdinalIgnoreCase))
                        {
                            Entities.WorkItems.Shortcuts shortcuts = new Entities.WorkItems.Shortcuts(filePathToOpen);

                            Logging.AddActionLog(string.Format("BookmarkManager: Opening '{0}' in Firefox ...", shortcuts.shortcutsFilePath));
                            MessagesHandler.Display( string.Format("Opening {0} ...", shortcuts.caption));

                            foreach (Shortcut shortcut in shortcuts.shortcuts)
                            {
                                Logging.AddActionLog(string.Format("BookmarkManager: Opening '{0}' in Firefox ...", shortcut.targetPath));

                                CommandsProviders.FOpener.FOpener.OpenInFirefox(shortcut.targetPath, true);
                            }
                        }
                        else
                        {
                            MessagesHandler.Display( string.Format("Opening {0} ...", filePathToOpen));
                            CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenFile(filePathToOpen);
                        }
                    }
                }
            }
            else

            // if (command.Name == "command name" && command.Postfix == "postfix [item] [item2]")
            //{
            //    MessagesHandler.Display( string.Format("Executing {0} ...", command.Name));

            //} else
            {
                throw new ApplicationException(string.Format("BookmarkManager: Command not found. Command: {0} {1}", command.Name, command.Postfix));
            }
        }

        public void ProcessingBeforeParameterInput(Command command, ref bool cancel)
        {
        }

        #endregion

        private void MenuItemChoosen(object sender, OSD.Menu.MenuItemChosenEventArgs e)
        {
            //if (e.selectedMenuItem.value is OSD.Menu.Menu)
            //{
            //    OSD.Menu.Menu subMenu = e.selectedMenuItem.value as OSD.Menu.Menu;
            //    OSD.Menu.Menu.ChangePage(subMenu);
            //}
            //else
            //{
            //    OSD.Menu.Menu.Close();
            //}
            if (e.selectedMenuItem.text == "[New Category]")
            {
                OSD.OSD.SettingsFilePath = "OSD.Bookmarks.ini";
                OSD.Menu.Menu.Close();
                do
                {
					throw new NotImplementedException(); //TODO: finish display async
					//string newCategoryName = ParameterInput.Display("Category", new List<string>(), false, false, "", false, false);
					//if (newCategoryName == null)
					//{
					//	break;
					//}
					//string newFolderPath = Path.Combine(e.selectedMenuItem.tag.ToString(), newCategoryName);
					//if (Directory.Exists(newFolderPath))
					//{
					//	MessagesHandler.Display("Category allready exist. Specify different name.");
					//}
					//else
					//{
					//	Directory.CreateDirectory(newFolderPath);
					//	OSD.Menu.Menu newMenu = new OSD.Menu.Menu(e.currentMenu, "Bookmarks", newFolderPath, false, false, false, false, "[New Category]", false, new OSD.Menu.MenuItemChosenDelegate(MenuItemChoosen));
					//	OSD.Menu.MenuItem newMenuItem = new OSD.Menu.MenuItem(newCategoryName, true, false, newMenu);
					//	newMenuItem.tag = newFolderPath;
					//	e.currentMenu.items.Insert(1, newMenuItem);

					//	Thread bringToFrontAssistant = new Thread(BringToFront);
					//	bringToFrontAssistant.Start("OSD");

					//	OSD.Menu.Menu.ShowMenu(e.currentMenu);

					//	break;
					//}
                } while (true);
            }
            else
            {
                OSD.Menu.Menu.Close();
            }

        }

        private void BringToFront(object o)
        {
            string caption = (string)o;
            int i = 5;
            while (i > 0)
            {
                i--;
                Thread.Sleep(1000);
                if (CraftSynth.BuildingBlocks.WindowsNT.Misc.SetForegroundWindow(caption)) i = 0;
            }
        }



    }
}
