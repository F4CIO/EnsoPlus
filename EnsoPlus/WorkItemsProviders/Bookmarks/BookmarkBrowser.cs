using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Common;
using EnsoPlus.Entities;
using Extension;

namespace EnsoPlus.WorkItemsProviders.BookmarkBrowser
{
    class BookmarkBrowser : IWorkItemsProvider
    {
        //public static List<string> GetAll()
        //{
        //    List<string> items = new List<string>();

        //    items.Add("[Browse Bookmarks]");

        //    return items;
        //}

        #region IParameterTypeProvider Members

        public bool SuggestionsCachingAllowed()
        {
            return false;
        }

        public Dictionary<string, IWorkItem> GetSuggestions()
        {
            Dictionary<string, IWorkItem> suggestions = new Dictionary<string, IWorkItem>();

            StringWorkItem bookmarkBrowser = new StringWorkItem("[Browse Bookmarks]");
            bookmarkBrowser.provider = this;
            suggestions.Add("[Browse Bookmarks]", bookmarkBrowser);

            return suggestions;
        }

        public IWorkItem GetParameterFromSelectedSuggestion(IWorkItem selectedSuggestion)
        {
            OSD.Menu.Menu bookmarkMenu = new OSD.Menu.Menu(null, "Bookmarks", Settings.Current.EnsoLearnAsOpenCommandsFolder, true, false, true, false, null, true, new OSD.Menu.MenuItemChosenDelegate(MenuItemChoosen));
            OSD.Menu.MenuItem selectedMenuItem = OSD.Menu.Menu.ShowMenu(bookmarkMenu);
            if (selectedMenuItem != null)
            {
                string shortcutFilePath = (string)selectedMenuItem.tag;
                Shortcut bookmarkShortcut = WorkItemsProviders.Shortcuts.Shortcuts.ReadShortcutFile(shortcutFilePath);
                bookmarkShortcut.provider = this;
                selectedSuggestion = (IWorkItem)bookmarkShortcut;
            }
            else
            {
                selectedSuggestion = null;
            }

            return selectedSuggestion;
        }

        private void MenuItemChoosen(object sender, OSD.Menu.MenuItemChosenEventArgs e)
        {
            if (e.selectedMenuItem.value is OSD.Menu.Menu)
            {
                OSD.Menu.Menu subMenu = e.selectedMenuItem.value as OSD.Menu.Menu;
                OSD.Menu.Menu.ChangePage(subMenu);
            }
            else
            {
                OSD.Menu.Menu.Close();
            }
        }

        public void Remove(IWorkItem workItemToRemove)
        {
            Shortcut bookmark = (Shortcut)workItemToRemove;
            string destinationFilePath = Path.Combine(Settings.Current.EnsoLearnAsOpenCommandsFolder, Path.GetFileName(bookmark.shortcutFilePath));
            File.Move(bookmark.shortcutFilePath, destinationFilePath);            
            SuggestionsCache.DropCache(this.GetType());   
        }

        #endregion
    }
}
