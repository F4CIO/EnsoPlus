using System;
using System.Collections.Generic;
using System.Text;

namespace EnsoPlus.Entities
{
    class CallerHistoryItem:Contact,IWorkItem
    {
        public DateTime dateAndTime;

        public CallerHistoryItem(Contact contact)
        {
            this.name = contact.name;
            this.number = contact.number;
            this.provider = contact.provider;
            this.dateAndTime = DateTime.Now;
        }

        #region IWorkItem Members
       
        public override string GetCaption()
        {
            return string.Format("Caller history: {0} {1} - {2}", this.dateAndTime.ToShortDateString(), this.dateAndTime.ToShortTimeString(), base.GetCaption());
        }
       
        #endregion
    }
}
