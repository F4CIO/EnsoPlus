using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Drawing;
using System.ComponentModel;

namespace Common
{
    public class SerializibleFont:IXmlSerializable
    {
        public string fontAsString;

        public SerializibleFont()
        {
        }

        public SerializibleFont(Font font)
        {
            this.font = font;
        }

        public Font font
        {
            get
            {                
                return (Font)TypeDescriptor.GetConverter(typeof(Font)).ConvertFromString(this.fontAsString);
            }
            set
            {
                this.fontAsString = TypeDescriptor.GetConverter(typeof(Font)).ConvertToString(value);
            }
        }
       
        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.Read();
            this.fontAsString = reader.ReadString();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteString(this.fontAsString);
        }

        #endregion
    }
}
