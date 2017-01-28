using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Extension;
using EnsoPlus.MessageHandler;

namespace Extension
{
    public class MessagesHandler
    {
        private static List<EnsoMessage> _history;
        private static List<EnsoMessage> history
        {
            get
            {
                if (_history == null)
                {
                    _history = new List<EnsoMessage>();
                }
                return _history;
            }
            set
            {
                _history = value;
            }
        }

        private static void AddToHistory(EnsoMessage message)
        {
            history.Add(message);
        }

        public static List<EnsoMessage> GetAllFromHistory()
        {
            List<EnsoMessage> items = new List<EnsoMessage>();

            foreach (EnsoMessage message in history)
            {
                items.Add(message);
            }

            return items;
        }

        public static EnsoMessage GetLastFromHistory()
        {
            if (history.Count == 0)
            {
                return new EnsoMessage("There were no messages.");
            }
            else
            {
                return history[history.Count - 1];
            }
        }

        public static void Display(EnsoMessage message, bool addToHistory = true)
        {
            if (addToHistory)
            {
                AddToHistory(message);
            }
	        MessageHandler.ShowMessage(message.Text, message.Subtext);
        }

        public static void Display(string messageText, string messageSubtext = null)
        {
            EnsoMessage message = new EnsoMessage(messageText, messageSubtext);
            Display(message, true);
        }

	    public static void HideAllMessages()
	    {
		    MessageHandler.HideAllMessages();
	    }
    }
}
