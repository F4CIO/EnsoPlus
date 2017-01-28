using System;
using System.Collections.Generic;
using System.Text;

namespace EnsoPlus.Entities
{
    class MemorizedString:IWorkItem
    {
        #region IWorkItem Members

        public IWorkItemsProvider provider;
        public IWorkItemsProvider GetProvider()
        {
            return this.provider;
        }

        public string caption;
        public string filePath;
        public string data;

        public MemorizedString(string caption, string filePath, string data)
        {
            this.caption = caption;
            this.filePath = filePath;
            this.data = data;
        }

        public string GetCaption()
        {
            return this.caption;
        }

        public string GetValueAsText()
        {
            return this.data;
        }

        public object GetValue()
        {
            return this.data;
        }

        #endregion
    }
}
