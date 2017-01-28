using System;
using System.Collections.Generic;
using System.Text;

//Source: http://www.codeproject.com/Articles/30270/XML-Serialization-of-Complex-NET-Objects
namespace CraftSynth.BuildingBlocks.IO.Xml.CustomXmlSerializer
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class XmlIgnoreBaseTypeAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class CustomXmlSerializationOptionsAttribute : Attribute
    {
        public CustomXmlSerializer.SerializationOptions SerializationOptions = new CustomXmlSerializer.SerializationOptions();        

        public CustomXmlSerializationOptionsAttribute(bool useTypeCache, bool useGraphSerialization)
        {
            SerializationOptions.UseTypeCache = useTypeCache;
            SerializationOptions.UseGraphSerialization = useGraphSerialization;            
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class XmlSerializeAsCustomTypeAttribute : Attribute
    {
    }
}
