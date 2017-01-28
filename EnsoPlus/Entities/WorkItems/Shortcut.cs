using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EnsoPlus.Entities
{
    class FileWorkItem: IWorkItem
    {
        public string filePath;
        public string caption;

	    public FileWorkItem(string filePath)
	    {
		    this.filePath = filePath;
		    this.caption = Path.GetFileNameWithoutExtension(filePath);
	    }

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
            return this.filePath;
        }

        #endregion
    }
}
