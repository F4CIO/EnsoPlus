using System;
using System.Collections.Generic;
using System.Text;

namespace EnsoPlus.Entities
{
    class KeyValue<KeyType, ValueType> :IWorkItem
    {
        public IWorkItemsProvider provider;
        public KeyType key;
        public ValueType value;

        #region IWorkItem Members

        public IWorkItemsProvider GetProvider()
        {
            return this.provider;
        }

        public string GetCaption()
        {
            return this.key.ToString();
        }

        public string GetValueAsText()
        {
            return this.value.ToString();
        }

        public object GetValue()
        {
            return this.value;
        }

        #endregion
    }
}
