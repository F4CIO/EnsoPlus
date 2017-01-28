using System;
using System.Collections.Generic;
using System.Text;

namespace EnsoPlus.Entities
{
    public interface IWorkItem
    {
        IWorkItemsProvider GetProvider();
        string GetCaption();
        string GetValueAsText();
        object GetValue();       
    }
}
