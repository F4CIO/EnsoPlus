using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Common;
using EnsoPlus.Entities;

namespace EnsoPlus.WorkItemsProviders.Contacts
{
    class Contacts : IWorkItemsProvider
    {
        public static List<Contact> GetContactsFromAllSources()
        {
            //Dictionary<string, string> contacts = new Dictionary<string, string>();

            //Dictionary<string, string> contactsFromE61 = SourceNokiaE61.GetContacts();
            //foreach (var contact in contactsFromE61)
            //{
            //    contacts[contact.Key] = contact.Value;
            //}

            //return contacts;

            return SourceNokiaE61.GetContacts();
        }

        #region IParameterTypeProvider Members

        public bool SuggestionsCachingAllowed()
        {
            return true;
        }

        public Dictionary<string, IWorkItem> GetSuggestions()
        {
            Dictionary<string, IWorkItem> suggestions = new Dictionary<string, IWorkItem>();

            List<Contact> contacts = GetContactsFromAllSources();
            foreach (Contact contact in contacts)
            {
                contact.provider = this;
                suggestions.Add(contact.GetCaption(), contact);
            }

            return suggestions;
        }

        public IWorkItem GetParameterFromSelectedSuggestion(IWorkItem selectedSuggestion)
        {
            return selectedSuggestion;
        }

        public void Remove(IWorkItem workItemToRemove)
        {
            throw new ApplicationException("Operation not supported.");
        }

        #endregion
    }
}
