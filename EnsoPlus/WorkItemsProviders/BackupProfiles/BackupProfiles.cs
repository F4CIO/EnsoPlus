using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Common;
using EnsoPlus.Entities;

namespace EnsoPlus.WorkItemsProviders.BackupProfiles
{
    public class BackupProfiles : IWorkItemsProvider
    {
        public static List<BackupProfile> GetAll()
        {
            List<BackupProfile> items = new List<BackupProfile>();

            string backupProfilesFolder = Settings.Current.BackupProfilesFolder;
            CraftSynth.BuildingBlocks.IO.FileSystem.CreateFolderIfItDoesNotExist(backupProfilesFolder);

            string[] fileNames = Directory.GetFiles(backupProfilesFolder, "*.bp");
            foreach (string fileName in fileNames)
            {
                try
                {
                    string filePath = Path.Combine(backupProfilesFolder, fileName);
                    string name = Path.GetFileNameWithoutExtension(fileName).ToLower();
                    BackupProfile backupProfile = new BackupProfile();
                    backupProfile.Name = name;
                    items.Add(backupProfile);
                }
                catch (Exception exception)
                {
                    Logging.AddErrorLog("ExtensionMacro: Adding of backup profile '" + fileName + "' failed : " + exception.Message);
					Common.Logging.AddExceptionLog(exception);
                }

            }



            return items;
        }

        public static List<string> GetAllNames()
        {
            var result = new List<string>();
            var profiles = GetAll();
            foreach (var profile in profiles)
            {
                result.Add(profile.Name);
            }
            return result;
        }

        public static string GetFilePath(string backupProfileName)
        {
            CraftSynth.BuildingBlocks.IO.FileSystem.CreateFolderIfItDoesNotExist(Settings.Current.BackupProfilesFolder);
            return string.Format(@"{0}\{1}.bp", Settings.Current.BackupProfilesFolder, backupProfileName);
        }

        public static BackupProfile Load(string backupProfileName)
        {
            BackupProfile backupProfile = null;
            try
            {
                string filePath = GetFilePath(backupProfileName);
                string[] lines = File.ReadAllLines(filePath);
                backupProfile = new BackupProfile();
                backupProfile.Name = backupProfileName;
                backupProfile.SourceFolder = lines[0];
                backupProfile.DestinationFolder = lines[1];
            }
            catch (Exception exception)
            {
				Common.Logging.AddExceptionLog(exception);
            }
            return backupProfile;
        }

        public static void Save(BackupProfile backupProfile)
        {
            File.WriteAllLines(GetFilePath(backupProfile.Name), new string[] { backupProfile.SourceFolder, backupProfile.DestinationFolder });
        }

        #region IParameterTypeProvider Members

        public bool SuggestionsCachingAllowed()
        {
            return true;
        }

        public Dictionary<string, IWorkItem> GetSuggestions()
        {
            Dictionary<string, IWorkItem> suggestions = new Dictionary<string, IWorkItem>();

            foreach (BackupProfile item in GetAll())
            {
                item.Provider = this;
                suggestions.Add(item.Name, item);
            }

            return suggestions;
        }

        public IWorkItem GetParameterFromSelectedSuggestion(IWorkItem selectedSuggestion)
        {
            BackupProfile backupProfile = Load(selectedSuggestion.GetCaption());
            backupProfile.Provider = this;
            return backupProfile;
        }

        public void Remove(IWorkItem workItemToRemove)
        {
            string filePath = GetFilePath(workItemToRemove.GetCaption());
            File.Delete(filePath);
            SuggestionsCache.DropCache(this.GetType());
            EnsoPlus.current.Reinitialize();
        }

        #endregion
    }
}
