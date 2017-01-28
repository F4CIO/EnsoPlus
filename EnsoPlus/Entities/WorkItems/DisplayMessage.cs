using System;
using System.Collections.Generic;
using System.Text;

using Extension;

namespace EnsoPlus.Entities
{
    public class DisplayMessage:EnsoMessage, IWorkItem
    {
        #region IWorkItem Members

        public IWorkItemsProvider provider;
        public IWorkItemsProvider GetProvider()
        {
            return this.provider;
        }

        public DisplayMessage(string text):base(text)
        {
        }

        public DisplayMessage(string text, string subbtext)
            : base(text, subbtext)
        {
        }

        public string GetCaption()
        {
            string caption = "Message: ";
            if(!string.IsNullOrEmpty(this.Text))
            {
                caption += this.Text;
            }
            if (!string.IsNullOrEmpty(this.Subtext))
            {
                if (!string.IsNullOrEmpty(this.Text))
                {
                    caption += " | ";
                }
                caption += this.Subtext;
            }
            return caption;
        }

        public object GetValue()
        {
            return this.Text;
        }

        public string GetValueAsText()
        {
            string valueAsText = string.Empty;
            if (!string.IsNullOrEmpty(this.Text))
            {
                valueAsText += this.Text;
            }
            if (!string.IsNullOrEmpty(this.Subtext))
            {
                if (!string.IsNullOrEmpty(this.Text))
                {
                    valueAsText += " | ";
                }
                valueAsText += this.Subtext;
            }
            return valueAsText;
        }

        #endregion
    }
}
