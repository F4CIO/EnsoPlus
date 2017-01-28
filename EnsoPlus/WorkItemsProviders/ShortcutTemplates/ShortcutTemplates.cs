using System;
using System.Collections.Generic;
using System.Text;

using EnsoPlus.Entities;
using Common;
using System.IO;
using Extension;
using System.Xml.Serialization;

namespace EnsoPlus.WorkItemsProviders.ShortcutTemplates
{
    public class ShortcutTemplates : IWorkItemsProvider
    {
        public static void CreateShortcutTemplate(string name, string queryStringTemplate)
        {
            string filePath = BuildFilePath(name);
            if (File.Exists(filePath))
            {
                throw new ApplicationException("Shortcut template with same name allready exists");
            }
            else
            {
                string[] contents = new string[] { "[InternetShortcut]", "URL="+queryStringTemplate };
                File.WriteAllLines(filePath, contents);
            }

        }

        public static Dictionary<string, ShortcutTemplate> GetNamesWithFilePaths()
        {
            Dictionary<string, ShortcutTemplate> parameters = new Dictionary<string, ShortcutTemplate>();

            string path = Settings.Current.WebSearchExtensionFolder;
            if (path == null)
            {
                return parameters;
            }

            CraftSynth.BuildingBlocks.IO.FileSystem.CreateFolderIfItDoesNotExist(path);
            string[] filePaths = Directory.GetFiles(path, "*.template.url");
            foreach (string filePath in filePaths)
            {
                try
                {
                    ShortcutTemplate shortcutTemplate = new ShortcutTemplate();
                    shortcutTemplate.caption = Path.GetFileNameWithoutExtension(filePath).Replace(".template", string.Empty);
                    shortcutTemplate.filePath = filePath;
                    shortcutTemplate.body = null;
                    parameters.Add(shortcutTemplate.caption, shortcutTemplate);
                }
                catch (Exception exception)
                {
                    string message = "ShortcutTemplates: Error occurred while reading file: " + filePath;
                    throw new Exception(message, exception);
                }
            }

            return parameters;
        }

        public static string BuildFilePath(string name)
        {            
            return Path.Combine(Settings.Current.WebSearchExtensionFolder, name + ".template.url");
        }

        public static string GetTemplate(string filePath)
        {
            string template = null;
            try
            {
                string[] content = File.ReadAllLines(filePath);
                string templateLine = content[1].Trim();
                template = templateLine.Substring(templateLine.IndexOf('=') + 1, templateLine.Length - templateLine.IndexOf('=') - 1).Trim();
            }
            catch (Exception exception)
            {
                string message = "ShortcutTemplates: Error occurred while reading file: " + filePath;
                throw new Exception(message, exception);
            }
            return template;
        }

        #region IParameterTypeProvider Members

        public bool SuggestionsCachingAllowed()
        {
            return true;
        }

        public Dictionary<string, IWorkItem> GetSuggestions()
        {
            Dictionary<string, ShortcutTemplate> shortcutTemplatesByName = GetNamesWithFilePaths();
            Dictionary<string, IWorkItem> workItemsByName = new Dictionary<string, IWorkItem>();
            foreach (string shortcutTemplateName in shortcutTemplatesByName.Keys)
            {
                shortcutTemplatesByName[shortcutTemplateName].provider = this;                
                workItemsByName[shortcutTemplateName] = shortcutTemplatesByName[shortcutTemplateName] as IWorkItem;
            }
            return workItemsByName;
        }

        public IWorkItem GetParameterFromSelectedSuggestion(IWorkItem selectedSuggestion)
        {
            //ShortcutTemplate selectedShortcutTemplate = selectedSuggestion as ShortcutTemplate;
            //if (selectedShortcutTemplate.body == null)
            //{
            //    selectedShortcutTemplate.body = GetTemplate(selectedShortcutTemplate.filePath);
            //}
            //return selectedShortcutTemplate;
            return selectedSuggestion;
        }

        public void Remove(IWorkItem workItemToRemove)
        {
            ShortcutTemplate shortcutTemplate = workItemToRemove as ShortcutTemplate;
            File.Delete(shortcutTemplate.filePath);
            SuggestionsCache.DropCache(this.GetType());
            EnsoPlus.current.Reinitialize();
        }

        #endregion
                
    }
}
