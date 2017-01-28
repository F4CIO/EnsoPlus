using System;
using System.Collections.Generic;
using System.Text;

namespace EnsoPlus.Entities
{
    public class ShortcutTemplate:IWorkItem
    {       
        public string caption;
        public string filePath;
        public string body;

        #region IWorkItem Members

        public IWorkItemsProvider provider;
        public IWorkItemsProvider GetProvider()
        {
            return this.provider;
        }

        public string GetCaption()
        {
            return this.caption;
        }

        public string GetValueAsText()
        {
            return this.filePath;
        }

        public object GetValue()
        {
            return this.body;
        }

        #endregion
    }
}
