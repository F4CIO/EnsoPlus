using System;
using System.Collections.Generic;
using System.Text;
using Common;
using System.IO;
using EnsoPlus.Entities;

namespace EnsoPlus.WorkItemsProviders.Contacts
{
    class SourceNokiaE61
    {
        private class Entry
        {
            public string caption = string.Empty;
            public Dictionary<string, string> details = new Dictionary<string, string>();
        }

        public static List<Contact> GetContacts()
        {
            List<Contact> contacts = new List<Contact>();

            try
            {
                string contactsFilePath = Settings.Current.CallerE61ContactsFilePath;
                CraftSynth.BuildingBlocks.IO.FileSystem.CreateFileIfItDoesNotExist(contactsFilePath);
                if (!File.Exists(contactsFilePath)) throw new ApplicationException("File not found:" + contactsFilePath);                

                
                string[] lines = File.ReadAllLines(contactsFilePath);

                Entry entry = new Entry();
                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        if (entry != null)
                        {
                            foreach (var detailKey in entry.details.Keys)
                            {
                                if(entry.caption.EndsWith(", "))entry.caption = entry.caption.Remove(entry.caption.LastIndexOf(", "));
                                string caption = string.Format("{0} ({1}:{2})",entry.caption, detailKey, entry.details[detailKey]);
                                contacts.Add(new Contact(caption, entry.details[detailKey]));
                            }
                        }
                        entry = new Entry();
                    }
                    else
                    {
                                                
                        string[] lineSides = line.Split(':');
                        lineSides[0] =  lineSides[0].Trim();
                        lineSides[1] =  lineSides[1].Trim();
                        if(!string.IsNullOrEmpty(lineSides[0]) && !string.IsNullOrEmpty(lineSides[1]))
                        {
                            if (lineSides[0].Contains("phone") || 
                                lineSides[0].Contains("mobile") || 
                                lineSides[0].Contains("email")
                               )
                            {
                                entry.details.Add(lineSides[0], lineSides[1]);
                            }
                            else                        
                            {
                                entry.caption += string.Format("{0}, ", lineSides[1]);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {                
                Logging.AddErrorLog("ExtensionCaller: SourceNokiaE61.GetContacts failed : " + exception.Message + ((exception.InnerException != null) ? ":" + exception.InnerException.Message : ""));
				Common.Logging.AddExceptionLog(exception);
            }

            return contacts;
        }
    }
}
