using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnsoPlus.Entities
{
    public class BackupProfile:IWorkItem
    {
        public IWorkItemsProvider Provider { get; set; }
        public string Name { get; set; }
        public string SourceFolder { get; set; }
        public string DestinationFolder { get; set; }

        #region IWorkItem Members

        public IWorkItemsProvider GetProvider()
        {
            return this.Provider;
        }

        public string GetCaption()
        {
            return this.Name;
        }

        public string GetValueAsText()
        {
            return this.Name;
        }

        public object GetValue()
        {
            return this;
        }

        #endregion
    }
}
