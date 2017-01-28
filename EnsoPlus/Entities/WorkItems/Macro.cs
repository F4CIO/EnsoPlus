using System;
using System.Collections.Generic;
using System.Text;

namespace EnsoPlus.Entities
{
    public class Macro:IWorkItem
    {
         public string syntax;
         public string name;
         public string postfix;
         public string description;
         public bool preload;
         public string[] body;
         public string filePath;

         #region IWorkItem Members

         public IWorkItemsProvider provider;
         public IWorkItemsProvider GetProvider()
         {
             return this.provider;
         }

         public string GetCaption()
         {
             return name;
         }

         public string GetValueAsText()
         {
             return body.ToString();
         }

         public object GetValue()
         {
             return body;
         }

         #endregion
    }
}
