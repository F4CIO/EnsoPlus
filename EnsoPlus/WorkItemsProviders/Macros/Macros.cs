using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Common;
using EnsoPlus.Entities;
using Extension;

namespace EnsoPlus.WorkItemsProviders.Macros
{
    class Macros : IWorkItemsProvider
    {
        public static List<Macro> GetNamesAndFilePaths()
        {
            List<Macro> items = new List<Macro>();

            string macrosFolder = Settings.Current.MacrosDataFolder;
            CraftSynth.BuildingBlocks.IO.FileSystem.CreateFolderIfItDoesNotExist(macrosFolder);

            string[] fileNames = Directory.GetFiles(macrosFolder, "*.mcr");
            foreach (string fileName in fileNames)
            {
                try
                {
                    string filePath = Path.Combine(macrosFolder, fileName);                    
                    string name = Path.GetFileNameWithoutExtension(fileName).ToLower();
                    Macro macro = new Macro();
                    macro.name = name;
                    macro.filePath = filePath;
                    items.Add(macro);
                }
                catch (Exception exception)
                {
                    Logging.AddErrorLog("ExtensionMacro: Adding of macro '" + fileName + "' failed : " + exception.Message);
					Common.Logging.AddExceptionLog(exception);
                }
            
            }
            return items;
        }

        private static string ExtractValue(string syntax)
        {
            if (syntax.IndexOf(":") == -1)
            {
            }
            else
            {
                syntax = syntax.Substring(syntax.IndexOf(':') + 1, syntax.Length - (syntax.IndexOf(':') + 1)).Trim();
            }
            return syntax;
        }

        public static Macro Get(string filePath, bool readBodyAlso)
        {
            Macro macro = null;
            try
            {
                StreamReader streamReader = File.OpenText(filePath);
                macro = new Macro();
                macro.syntax = ExtractValue(streamReader.ReadLine());
                macro.name = Path.GetFileNameWithoutExtension(filePath).ToLower();
                macro.postfix = Syntax.ExtractPostfix(macro.syntax);
                macro.description = ExtractValue(streamReader.ReadLine());
                macro.preload = bool.Parse(ExtractValue(streamReader.ReadLine()));
                macro.filePath = filePath;
                streamReader.Close();
                
                if (readBodyAlso)
                {
                    macro.body = File.ReadAllLines(filePath);
                }                
            }
            catch (Exception exception)
            {
				Common.Logging.AddExceptionLog(exception);
            }
            return macro;
        }      

        #region IParameterTypeProvider Members

        public bool SuggestionsCachingAllowed()
        {
            return true;
        }

        public Dictionary<string, IWorkItem> GetSuggestions()
        {
            Dictionary<string, IWorkItem> suggestions = new Dictionary<string, IWorkItem>();
            foreach (Macro macro in GetNamesAndFilePaths())
            {
                macro.provider = this;
                suggestions.Add(macro.name, macro);
            }
            return suggestions;
        }

        public IWorkItem GetParameterFromSelectedSuggestion(IWorkItem selectedSuggestion)
        {
            Macro macro = Get((selectedSuggestion as Macro).filePath, true);
            macro.provider = this;
            return macro;
        }

        public void Remove(IWorkItem workItemToRemove)
        {
            Macro macro = workItemToRemove as Macro;
            File.Delete(macro.filePath);
            SuggestionsCache.DropCache(this.GetType());
            EnsoPlus.current.Reinitialize();
        }

        #endregion
    }
}
