using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EnsoPlus.Entities
{
    public class Contact:IWorkItem
    {
        public string name;

        private string _number;
        public string number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = ExtractNumber(value);
            }

        }      

        public Contact()
        {
            name = null;
            number = null;
        }

        public Contact(string nameA, string numberA)
        {
            name = nameA;
            number = numberA;
        }


        private static string ExtractNumber(string numberString)
        {
            string number = string.Empty;

            if (numberString != null)
            {
                numberString = numberString.Trim();
                numberString = numberString.Replace("+381", "0");
                StringBuilder sb = new StringBuilder();
                int i = 0;
                while (i < numberString.Length)
                {
                    if (char.IsDigit(numberString[i]))
                    {
                        sb.Append(numberString[i]);
                    }
                    //else if (sb.Length > 0)
                    //{
                    //    break;
                    //}
                    i++;
                }
                number = sb.ToString();
            }

            return number;
        }

        #region IWorkItem Members

        public IWorkItemsProvider provider;
        public IWorkItemsProvider GetProvider()
        {
            return this.provider;
        }

        public virtual string GetCaption()
        {
            if (string.IsNullOrEmpty(this.name))
            {
                return this.number;
            }
            else
            {
                return name;
                //return string.Format("{0} ({1})", this.name, this.number);
            }            
        }

        public string GetValueAsText()
        {
            return this.number;
        }

        public object GetValue()
        {
            return this.number;
        }

        #endregion
    }

    public class ContactUnknown : Contact
    {
        public ContactUnknown(string numberA)
        {           
            base.number = numberA;
        }
    }
}
