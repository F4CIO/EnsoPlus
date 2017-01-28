using System;
using System.Collections.Generic;
using System.Text;

namespace EnsoPlus.Entities
{
    public class StringWorkItem:IWorkItem
    {
        private string _value;

        public StringWorkItem(string value)
        {
            this._value = value;
        }

        public static List<IWorkItem> CreateInstances(List<string> values)
        {
            List<IWorkItem> instances = new List<IWorkItem>();
            foreach (string value in values)
            {
                instances.Add(new StringWorkItem(value));
            }
            return instances;
        }

        #region IWorkItem Members

        public IWorkItemsProvider provider;
        public IWorkItemsProvider GetProvider()
        {
            return this.provider;
        }

        public string GetCaption()
        {
            return this._value;
        }

        public string GetValueAsText()
        {
            return this._value;
        }

        public object GetValue()
        {
            return this._value;
        }

        #endregion
    }
}
