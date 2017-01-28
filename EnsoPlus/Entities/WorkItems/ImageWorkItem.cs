using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace EnsoPlus.Entities
{
    class ImageWorkItem : IWorkItem
    {
        public string filePath;
        public Image image;

        #region IWorkItem Members

        public IWorkItemsProvider provider;
        public IWorkItemsProvider GetProvider()
        {
            return this.provider;
        }

        public string GetCaption()
        {
            string name = null;
            try
            {
                name = Path.GetFileName(this.filePath);
            }
            catch(Exception)
            {                
            }
            if(name == null) name = this.filePath;
            return name;
        }

        public string GetValueAsText()
        {
            return Path.GetFileName(this.filePath);
        }

        public object GetValue()
        {
            return this.image;
        }

        #endregion
    }
}

