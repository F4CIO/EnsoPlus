using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CraftSynth.BuildingBlocks.IO.Xml
{
	public partial class Misc
	{
		#region Get and Deserialize
		/// <summary>
		/// Creates XmlDocument and if file exist fills it with data. 
		/// In case on an error returns null.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static XmlDocument LoadXMLDocumentFromFile(string filePath)
		{
			XmlDocument doc = new XmlDocument();

			if (File.Exists(filePath))
			{
				doc.Load(filePath);
			}

			return doc;
		}

		/// <summary>
		/// Gets element inner text.
		/// </summary>
		/// <param name="path">List of element names (including target element) that build path to target element.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">Thrown when parameter <paramref>filePath</paramref> is null or empty.</exception>
		//public static string GetXMLElementValue(Stream stream,List<string,int> pathSteps)
		//{
		//    if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("Value must be non-empty string.", "filePath");

		//    XmlTextReader xmlTextReader = new XmlTextReader(stream);

		//    int i=0;
		//    while(i<pathSteps.Count && pathSteps[i]!=xmlTextReader.LocalName)
		//    {

		//        i++;
		//    }

		//    foreach(

		//    return null;
		//}

		/// <summary>
		/// Deserializes specified node.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="node"></param>
		/// <returns></returns>
		public static T Deserialize<T>(XmlNode node)
		{
			T retVal = default(T);

			StringReader reader = new StringReader(node.OuterXml);
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			retVal = (T)serializer.Deserialize(reader);
			//TODO: add code to process inner nodes... this.DeserializeGenerics(node, retVal);

			return retVal;
		}

		/// <summary>
		/// Gets object from specified XML file that matches specified ID on specified xPath.
		/// Returns default value if not found.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ID"></param>
		/// <param name="xPathToID">Example: /Users/User/ID</param>
		/// <returns></returns>
		public static T GetByID<T>(string filePath, string xPathToID, Guid ID)
		{
			T retVal = default(T);

			XmlDocument userDB = LoadXMLDocumentFromFile(filePath);
			XmlNodeList list = userDB.SelectNodes(xPathToID);
			foreach (XmlNode node in list)
			{
				if (node.InnerText != null && new Guid(node.InnerText.Trim()).CompareTo(ID) == 0)
				{
					XmlNode userNode = node.ParentNode;
					retVal = Deserialize<T>(userNode);
					break;
				}
			}

			return retVal;
		}


		/// <summary>
		/// Gets a collection of objects from specified XML file that matches specified ID on specified xPath.
		/// Returns empty list if none is found.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filePath"></param>
		/// <param name="xPathToProperty">Example: /Users/User/ID</param>
		/// <param name="property">Property by which nodes should be filtered</param>
		/// <param name="stringComparasion">Comparasion used when matching</param>
		/// <returns></returns>
		public static List<T> GetByProperty<T>(string filePath, string xPathToProperty, string property, StringComparison stringComparasion)
		{
			List<T> retVal = new List<T>();

			XmlDocument userDB = LoadXMLDocumentFromFile(filePath);
			XmlNodeList list = userDB.SelectNodes(xPathToProperty);
			foreach (XmlNode node in list)
			{
				if (node.InnerText != null && string.Compare(node.InnerText.Trim(), property, stringComparasion) == 0)
				{
					XmlNode userNode = node.ParentNode;
					retVal.Add(Deserialize<T>(userNode));
				}
			}

			return retVal;
		}

		/// <summary>
		/// Gets all objects from specified XML file on specified xPath.
		/// Returns empty list if none is found.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filePath"></param>
		/// <param name="xPathToEntity">Example: /Users/User</param>
		/// <param name="property">Property by which nodes should be filtered</param>
		/// <param name="stringComparasion">Comparasion used when matching</param>
		/// <returns></returns>
		public static List<T> GetAll<T>(string filePath, string xPathToEntity)
		{
			List<T> retVal = new List<T>();

			XmlDocument userDB = LoadXMLDocumentFromFile(filePath);
			XmlNodeList list = userDB.SelectNodes(xPathToEntity);
			foreach (XmlNode node in list)
			{
				if (node.InnerText != null)
				{
					XmlNode userNode = node.ParentNode;
					retVal.Add(Deserialize<T>(userNode));
				}
			}

			return retVal;
		}

		/// <summary>
		/// Deserializes object from file using XmlSerizlizer class.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static object Deserialize(Type type, string filePath)
		{
			object returnValue = null;
			FileStream fileStream = null;
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(type);
				fileStream = new FileStream(filePath, FileMode.Open);
				returnValue = xmlSerializer.Deserialize(fileStream);
			}
			//catch (Exception exception)
			//{
			//    ExceptionHandler.Handle(exception, ExceptionHandlingPolicies.DAL_Wrap_Policy);
			//}
			finally
			{
				try
				{
					fileStream.Close();
				}
				catch { }
			}

			return returnValue;
		}

		/// <summary>
		/// Deserializes from file object that is in XML/Soap format using SoapFormatter or BinaryFormatter.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="useSoapFormatter"></param>
		/// <returns></returns>
		public static object Deserialize(string filePath, bool useSoapFormatter)
		{
			object returnValue = null;
			FileStream fileStream = null;
			try
			{
				fileStream = new FileStream(filePath, FileMode.Open);
				if (useSoapFormatter)
				{
					SoapFormatter soapFormatter = new SoapFormatter();
					returnValue = soapFormatter.Deserialize(fileStream);
				}
				else
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					returnValue = binaryFormatter.Deserialize(fileStream);
				}
			}
			//catch (Exception exception)
			//{
			//    ExceptionHandler.Handle(exception, ExceptionHandlingPolicies.DAL_Wrap_Policy);
			//}
			finally
			{
				try
				{
					fileStream.Close();
				}
				catch { }
			}

			return returnValue;
		}

		/// <summary>
		/// Deserializes from file object that is in XML/Soap format using SoapFormatter or BinaryFormatter.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="useSoapFormatter"></param>
		/// <returns></returns>
		public static object Deserialize(byte[] bytes, bool useSoapFormatter)
		{
			object r = null;

			using(MemoryStream memStream = new MemoryStream(bytes))
			{
				if (useSoapFormatter)
				{
					SoapFormatter soapFormatter = new SoapFormatter();
					r = soapFormatter.Deserialize(memStream);
				}
				else
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					r = binaryFormatter.Deserialize(memStream);
				}
			}

			return r;
		}
		#endregion

		#region Serialize and Save operations
		public static XmlNode CreateNodeFromObject<T>(T entity)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			StringWriter sw = new StringWriter();
			serializer.Serialize(sw, entity);
			XmlDocument serializedEntity = new XmlDocument();
			serializedEntity.LoadXml(sw.ToString());
			//TODO: serialize inner members... this.SerializeGenerics(serializedUser.ChildNodes[1], user);
			return serializedEntity.ChildNodes[1];
		}

		/// <summary>
		/// Saves the entity inside file.
		/// If file does not exist, new file will be created.
		/// If entity allready exists throws "Allready exists" Exception.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <param name="ID"></param>
		/// <param name="filePath"></param>
		/// <param name="xPathToIDProperty">Example: /Users/User/ID</param>
		/// <param name="xPathToParentCollection">Example: /Users)</param>
		/// <param name="stringComparasion"></param>
		public static void SaveEntity<T>(T entity, string ID, string filePath, string xPathToIDProperty, string xPathToParentCollection, StringComparison stringComparasion)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}

			XmlNode serializedEntity = CreateNodeFromObject<T>(entity);

			XmlDocument xmlDoc = LoadXMLDocumentFromFile(filePath);

			List<T> existingEntity = GetByProperty<T>(filePath, xPathToIDProperty, ID, stringComparasion);
			if (existingEntity != null && existingEntity.Count > 0)
			{
				throw new Exception("Allready exists");
			}

			XmlNode entityNode = xmlDoc.SelectSingleNode(xPathToParentCollection);
			XmlElement entityElement = null;
			if (entityNode == null)
			{
				string parentCollectionNodeName = xPathToParentCollection.Substring(xPathToParentCollection.LastIndexOf('/'), xPathToParentCollection.Length - xPathToParentCollection.LastIndexOf('/'));
				entityElement = xmlDoc.CreateElement(parentCollectionNodeName);
				xmlDoc.AppendChild(entityElement);
			}
			else
			{
				entityElement = entityNode as XmlElement;
			}

			XmlNode newNode = xmlDoc.ImportNode(serializedEntity, true);
			entityElement.AppendChild(newNode);

			xmlDoc.Save(filePath);
		}



		/// <summary>
		/// Updates the user in DB or invokes Save operation if user doesn't exist
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filePath"></param>
		/// <param name="entity"></param>
		/// <param name="ID">Example: entity.ToString()</param>
		/// <param name="xPathToIdProperty">/Users/User/ID</param>
		/// <param name="xPathSelectorFormatToIDProperty">Example: /Users/User[ID/text()='{0}']</param>
		/// <param name="xPathToParentCollection">/Users</param>
		/// <param name="stringComparer"></param>
		/// <returns></returns>
		public static bool UpdateEntity<T>(string filePath, T entity, string ID, string xPathToIdProperty, string xPathSelectorFormatToIDProperty, string xPathToParentCollection, StringComparison stringComparasion)
		{
			bool retVal = false;

			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}

			if (File.Exists(filePath))    //db exists
			{
				XmlDocument userDB = LoadXMLDocumentFromFile(filePath);

				string xpath = string.Format(xPathSelectorFormatToIDProperty, ID);

				XmlNode existingUser = userDB.SelectSingleNode(xpath);

				XmlNode newUser = CreateNodeFromObject<T>(entity);

				XmlNode importedNode = userDB.ImportNode(newUser, true);

				if (existingUser != null)
				{
					existingUser.ParentNode.ReplaceChild(importedNode, existingUser);
				}
				else
				{
					XmlNode users = userDB.SelectSingleNode(xPathToParentCollection);
					users.AppendChild(importedNode);
				}

				userDB.Save(filePath);

				retVal = true;
			}
			else
			{
				//Save user
				SaveEntity<T>(entity, ID, filePath, xPathToIdProperty, xPathToParentCollection, stringComparasion);
				retVal = true;
			}


			return retVal;
		}


		/// <summary>
		/// Serializes provided object to XML format to specified file.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="myObject"></param>
		/// <param name="fileName"></param>
		public static void Serialize(object myObject, string filePath)
		{
			StreamWriter streamWritter = null;
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(myObject.GetType());
				streamWritter = new StreamWriter(filePath);
				xmlSerializer.Serialize(streamWritter, myObject);
			}
			//catch (Exception exception)
			//{
			//    ExceptionHandler.Handle(exception, ExceptionHandlingPolicies.DAL_Wrap_Policy);
			//}
			finally
			{
				try
				{
					streamWritter.Close();
				}
				catch { }
			}
		}

		/// <summary>
		/// Serializes provided object to specified file. 
		/// Object serialized to binary format can be deserialized by only .NET application.
		/// Soap format consume more disk space and time comparing to binary format.
		/// </summary>
		/// <param name="myObject"></param>
		/// <param name="fileName"></param>
		/// <param name="useSoapFormatter"></param>
		public static void Serialize(object myObject, string fileName, bool useSoapFormatter)
		{
			FileStream fileStream = null;
			try
			{
				fileStream = new FileStream(fileName, FileMode.Create);
				if (useSoapFormatter)
				{
					SoapFormatter soapFormatter = new SoapFormatter();
					soapFormatter.Serialize(fileStream, myObject);
				}
				else
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(fileStream, myObject);
				}
			}
			//catch (Exception exception)
			//{
			//    ExceptionHandler.Handle(exception, ExceptionHandlingPolicies.DAL_Wrap_Policy);
			//}
			finally
			{
				try
				{
					fileStream.Close();
				}
				catch { }
			}
		}

		/// <summary>
		/// Serializes provided object to byte array. 
		/// Object serialized to binary format can be deserialized by only .NET application.
		/// Soap format consume more disk space and time comparing to binary format.
		/// </summary>
		/// <param name="myObject"></param>
		/// <param name="useSoapFormatter"></param>
		public static byte[] Serialize(object myObject, bool useSoapFormatter)
		{
			byte[] r = null;
			MemoryStream memStream = null;
			try
			{
				memStream = new MemoryStream();
				if (useSoapFormatter)
				{
					SoapFormatter soapFormatter = new SoapFormatter();
					soapFormatter.Serialize(memStream, myObject);
				}
				else
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(memStream, myObject);
				}
				r = memStream.ToArray();
			}
			//catch (Exception exception)
			//{
			//    ExceptionHandler.Handle(exception, ExceptionHandlingPolicies.DAL_Wrap_Policy);
			//}
			finally
			{
				try
				{
					memStream.Close();
				}
				catch { }
			}
			return r;
		}
		#endregion

		/// <summary>
		/// Deletes entity from data store.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="xPathSelectorFormatToIDProperty">Example: /Users/User[ID/text()='{0}']</param>
		/// <param name="ID"></param>
		public static void Delete(string filePath, string xPathSelectorFormatToIDProperty, string ID)
		{
			XmlDocument userDB = LoadXMLDocumentFromFile(filePath);
			string xpath = string.Format(xPathSelectorFormatToIDProperty, ID);
			XmlNode userNode = userDB.SelectSingleNode(xpath);
			if (userNode != null)
			{
				userNode.ParentNode.RemoveChild(userNode);
			}
			userDB.Save(filePath);
		}
	}
}
