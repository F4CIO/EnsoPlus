using System;
using System.Collections.Generic;
using System.Text;
using DotRas;
using EnsoPlus;
using EnsoPlus.Entities;
using Common;

namespace EnsoPlus.CommandsProviders.Caller
{
    public class TargetDotRas
    {
        private static string networkConnectionName
        {
            get
            {
                return Settings.Current.CallerNetworkConnectionName;
            }
        }
        private static string networkCredentialUsername
        {
            get
            {
                return Settings.Current.NetworkCredentialUsername;
            }
        }
        private static string networkCredentialPassword
        {
            get
            {
                return Settings.Current.NetworkCredentialPassword;
            }
        }
        private static RasDialer dialer;


        public static void Dial(Contact contact)
        {
            RasPhoneBook phoneBook = new RasPhoneBook();
            phoneBook.Open();

            if (!phoneBook.Entries.Contains(networkConnectionName))
            {
                DotRas.RasEntry entry = RasEntry.CreateDialUpEntry(networkConnectionName, contact.number, RasDevice.GetDeviceByName("Modem", RasDeviceType.Modem, false));                
                phoneBook.Entries.Add(entry);
            }

            RasDialer dialer = new RasDialer();
            phoneBook.Entries[networkConnectionName].PhoneNumber = contact.number;            
            phoneBook.Entries[networkConnectionName].Update();
            dialer.EntryName = networkConnectionName;            
            dialer.DialCompleted += new EventHandler<DialCompletedEventArgs>(Dialer.dialer_DialCompleted);
            dialer.StateChanged += new EventHandler<StateChangedEventArgs>(Dialer.dialer_StateChanged);
            TargetDotRas.dialer = dialer;

            dialer.DialAsync(new System.Net.NetworkCredential(networkCredentialUsername, networkCredentialPassword));            
        }

        public static bool Cancel()
        {
            bool cancelled = false;

            if (TargetDotRas.dialer != null)
            {
                TargetDotRas.dialer.DialAsyncCancel();
                cancelled = true;
            }

            return cancelled;
        }

        public static void Dispose()
        {
            dialer.Dispose();
        }

    
    }
}
