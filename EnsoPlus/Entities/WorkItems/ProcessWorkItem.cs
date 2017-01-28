using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace EnsoPlus.Entities
{
    class ProcessWorkItem : IWorkItem
    {
        public Process procces;

        //Can not bild caption on the fly becouse it would be different every time becouse memory info.
        public string caption;

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
            return this.caption;
        }

        public object GetValue()
        {
            return this;
        }

        #endregion
    }
}
