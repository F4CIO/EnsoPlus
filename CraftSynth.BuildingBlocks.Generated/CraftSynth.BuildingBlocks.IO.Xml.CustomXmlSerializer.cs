using System;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;
using System.Xml.Serialization;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Text;
using System.IO;

//Source: http://www.codeproject.com/Articles/30270/XML-Serialization-of-Complex-NET-Objects

//-----------------------Example class
//namespace Example
//{
//	class Program
//	{
//		static void Main(string[] args)
//		{
//			Test1 t = Test1.Get();//new Test1();
//			t.A = 1;
//			t.B = 2;
//			t.Str = "this is a string";
//			t.Dt1 = DateTime.Now;
//			t.Dt2 = DateTime.Today.AddDays(10);
//			t.Arr = new int[] { 1, 2, 3 };
//			t.baseInt = 33;
//			t.baseStr = "this is a basestring";
//			t.SetValues(15, 3.4);
//			t.privDbl = 5.6;
//			t.base1 = new Base1();
//			t.base1.baseInt = 99;
//			t.base1.baseStr = "base1's basestring";
//			t.base2 = t.base1;
//			t.tEnum = TestEnum.enum2;

//			XmlDocument doc = CustomXmlSerializer.Serialize(t, 1, "Test1");
//			doc.Save(@"d:\out.xml");

//			Test1 t2 = (Test1)CustomXmlDeserializer.Deserialize(doc.OuterXml, 1, new TestMeTypeConverter());

//		}
//	}

//	//[XmlIgnoreBaseType]
//	class Base1
//	{
//		public int baseInt;
//		public string baseStr;
//		protected int protInt = 1;
//		private double privDbl = 2.3;

//		public void SetValues(int prot, double priv)
//		{
//			protInt = prot;
//			privDbl = priv;
//		}
//	}

//	//[CustomXmlSerializationOptions(true, false)]
//	class Test1 : Base1
//	{
//		private Test1()
//		{ }

//		public static Test1 Get()
//		{
//			return new Test1();
//		}

//		public TestEnum tEnum;
//		public int A;
//		public int B;
//		public string Str;
//		public DateTime Dt1;
//		public DateTime Dt2;
//		public int[] Arr;
//		public double privDbl;
//		public Base1 base1;
//		public Base1 base2;
//	}

//	enum TestEnum
//	{
//		enum1 = 1,
//		enum2 = 2
//	}
//}
//----------------------------------------------


namespace CraftSynth.BuildingBlocks.IO.Xml.CustomXmlSerializer
{
    public class CustomXmlSerializer : CustomXmlSerializerBase
    {   
        Dictionary<Type, TypeInfo> typeCache = new Dictionary<Type, TypeInfo>();        
        Dictionary<Type, IDictionary<ObjKeyForCache, ObjInfo>> objCache = new Dictionary<Type, IDictionary<ObjKeyForCache, ObjInfo>>();
        int objCacheNextId = 0;
        SerializationOptions options;

        protected CustomXmlSerializer(SerializationOptions opt)
        {
            options = opt;
        }        

        void SetTypeInfo(Type objType, XmlElement element)
        {
            if (!options.UseTypeCache)
            {
                // add detailed type information
                WriteTypeToNode(element, objType);
                return;
            }
            TypeInfo typeInfo;
            if (typeCache.TryGetValue(objType, out typeInfo))
            {
                XmlElement onlyElement = typeInfo.OnlyElement;
                if (onlyElement != null)
                {
                    // set the type of the element to be a reference to the type ID
                    // since the element is no longer the only one of this type                    
                    typeInfo.WriteTypeId(onlyElement);                    
                    onlyElement.RemoveAttribute("type");
                    onlyElement.RemoveAttribute("assembly");
                    typeInfo.OnlyElement = null;
                }
                typeInfo.WriteTypeId(element);                
            }
            else
            {
                // add type to cache
                typeInfo = new TypeInfo();
                typeInfo.TypeId = typeCache.Count;
                typeInfo.OnlyElement = element;
                typeCache.Add(objType, typeInfo);
                // add detailed type information
                WriteTypeToNode(element, objType);
            }            
        }

        static void WriteTypeToNode(XmlElement element, Type objType)
        {
            element.SetAttribute("type", objType.FullName);
            element.SetAttribute("assembly", objType.Assembly.FullName);
        }

        XmlElement GetTypeInfoNode()
        {
            XmlElement element = doc.CreateElement("TypeCache");
            foreach (KeyValuePair<Type, TypeInfo> kv in typeCache)
            {
                if (kv.Value.OnlyElement == null)
                {
                    // there is more than one element having this type
                    XmlElement e = doc.CreateElement("TypeInfo");
                    kv.Value.WriteTypeId(e);                    
                    WriteTypeToNode(e, kv.Key);
                    element.AppendChild(e);
                }
            }
            return element.HasChildNodes ? element : null;
        }

        public static XmlDocument Serialize(object obj, int ver, string rootName)
        {
            // determine serialization options
            SerializationOptions serOptions = new SerializationOptions();
            if (obj != null)
            {
                Type objType = obj.GetType();
                object[] attribs = objType.GetCustomAttributes(typeof(CustomXmlSerializationOptionsAttribute), false);
                if (attribs.Length > 0)
                {
                    serOptions = ((CustomXmlSerializationOptionsAttribute)attribs[0]).SerializationOptions;
                }
            }
            // create serializer
            CustomXmlSerializer serializer = new CustomXmlSerializer(serOptions);
            XmlElement element = serializer.SerializeCore(rootName, obj);
            element.SetAttribute("version", ver.ToString());
            element.SetAttribute("culture", Thread.CurrentThread.CurrentCulture.ToString());            
            // add typeinfo
            XmlElement typeInfo = serializer.GetTypeInfoNode();
            if (typeInfo != null)
            {
                element.PrependChild(typeInfo);
                element.SetAttribute("hasTypeCache", "true");
            }
            // add serialized data
            serializer.doc.AppendChild(element);
            return serializer.doc;
        }        

        bool AddObjToCache(Type objType, object obj, XmlElement element)
        {
            ObjKeyForCache kfc = new ObjKeyForCache(obj);
            IDictionary<ObjKeyForCache, ObjInfo> entry;            
            if (objCache.TryGetValue(objType, out entry))
            {                
                // look for this particular object                
                ObjInfo objInfoFound;
                if (entry.TryGetValue(kfc, out objInfoFound))
                {
                    // the object has already been added
                    if (objInfoFound.OnlyElement != null)
                    {
                        objInfoFound.WriteObjId(objInfoFound.OnlyElement);
                        objInfoFound.OnlyElement = null;
                    }
                    // write id to element
                    objInfoFound.WriteObjId(element);
                    return false;
                }                
            }
            else
            {
                // brand new type in the cache
                entry = new Dictionary<ObjKeyForCache, ObjInfo>(1);
                objCache.Add(objType, entry);
            }
            // object not found, add it
            ObjInfo objInfo = new ObjInfo();
            objInfo.Id = objCacheNextId;
            objInfo.OnlyElement = element;
            entry.Add(kfc, objInfo);
            objCacheNextId++;
            return true;
        }

        static bool CheckForcedSerialization(Type objType)
        {
            object[] attribs = objType.GetCustomAttributes(typeof(XmlSerializeAsCustomTypeAttribute), false);
            return attribs.Length > 0;
        }

        XmlElement SerializeCore(string name, object obj)
        {
	        name = name.Replace("<", "_oI_");
	        name = name.Replace(">", "_Io_");
            XmlElement element = doc.CreateElement(name);
            if (obj == null)
            {
                element.SetAttribute("value", "null");
                return element;
            }

            Type objType = obj.GetType();

            if (objType.IsClass && objType != typeof(string))
            {
                // check if we have already serialized this object
                if (options.UseGraphSerialization && !AddObjToCache(objType, obj, element))
                {
                    return element;
                }                
                // the object has just been added                
                SetTypeInfo(objType, element);

                if (CheckForcedSerialization(objType))
                {
                    // serialize as complex type
                    SerializeComplexType(obj, element);
                    return element;
                }

                IXmlSerializable xmlSer = obj as IXmlSerializable;
                if (xmlSer == null)
                {
                    // does not know about automatic serialization
                    IEnumerable arr = obj as IEnumerable;
                    if (arr == null)
                    {
                        SerializeComplexType(obj, element);
                    }
                    else
                    {
                        foreach (object arrObj in arr)
                        {
                            XmlElement e = SerializeCore(name, arrObj);
                            element.AppendChild(e);
                        }
                    }
                }
                else
                {
                    // can perform the serialization itself
                    StringBuilder sb = new StringBuilder();
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.ConformanceLevel = ConformanceLevel.Fragment;
                    settings.Encoding = Encoding.UTF8;
                    settings.OmitXmlDeclaration = true;
                    XmlWriter wr = XmlWriter.Create(sb, settings);
                    wr.WriteStartElement("value");
                    xmlSer.WriteXml(wr);
                    wr.WriteEndElement();                    
                    wr.Close();

                    element.InnerXml = sb.ToString();
                }
            }
            else
            {
                // the object has just been added                
                SetTypeInfo(objType, element);

                if (CheckForcedSerialization(objType))
                {
                    // serialize as complex type
                    SerializeComplexType(obj, element);
                    return element;
                }
                
                if (objType.IsEnum)
                {
                    object val = Enum.Format(objType, obj, "d");
                    element.SetAttribute("value", val.ToString());
                }
                else
                {
                    if (objType.IsPrimitive || objType == typeof(string) || 
                        objType == typeof(DateTime) || objType == typeof(decimal))
                    {
                        element.SetAttribute("value", obj.ToString());
                    }
                    else
                    {
                        // this is most probably a struct
                        SerializeComplexType(obj, element);
                    }
                }                    
            }

            return element;
        }

        void SerializeComplexType(object obj, XmlElement element)
        {
            Type objType = obj.GetType();
            // get all instance fields
            IDictionary<string, FieldInfo> fields = GetTypeFieldInfo(objType);
            foreach (KeyValuePair<string, FieldInfo> kv in fields)                
            {                    
                // serialize field
                XmlElement e = SerializeCore(kv.Key, kv.Value.GetValue(obj));
                element.AppendChild(e);
            }
        }        

        class ObjInfo
        {
            internal int Id;
            internal XmlElement OnlyElement;

            internal void WriteObjId(XmlElement element)
            {
                element.SetAttribute("id", Id.ToString());
            }
        }

        struct ObjKeyForCache : IEquatable<ObjKeyForCache>
        {
            object m_obj;

            public ObjKeyForCache(object obj)
            {
                m_obj = obj;
            }

            public bool Equals(ObjKeyForCache other)
            {
                return object.ReferenceEquals(m_obj, other.m_obj);
            }
        }

        public class SerializationOptions
        {
            public bool UseTypeCache = true;
            public bool UseGraphSerialization = true;
        }
    }
}
