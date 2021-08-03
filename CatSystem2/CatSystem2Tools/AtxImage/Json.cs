using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Xml;


namespace AtxImage
{
	public class Json
	{

		public static string Serialize<T>(T obj)
		{
			
			string result = null;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (StreamReader streamReader = new StreamReader(memoryStream))
					{
						DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T));
						dataContractJsonSerializer.WriteObject(memoryStream, obj);
						memoryStream.Position = 0L;
						result = streamReader.ReadToEnd();
					}
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		public static string SerializeWithIndent<T>(T obj)
		{
			string result = null;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (StreamReader streamReader = new StreamReader(memoryStream))
					{
						using (XmlDictionaryWriter xmlDictionaryWriter = JsonReaderWriterFactory.CreateJsonWriter(memoryStream, Encoding.UTF8, true, true))
						{
							DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T));
							dataContractJsonSerializer.WriteObject(xmlDictionaryWriter, obj);
							memoryStream.Position = 0L;
							result = streamReader.ReadToEnd();
						}
					}
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		public static T Deserialize<T>(string jsonString) where T : class
		{
			T t = default(T);
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
				{
					DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T));
					t = (T)((object)dataContractJsonSerializer.ReadObject(memoryStream));
				}
			}
			catch
			{
				t = default(T);
			}
			return t;
		}

	}
}
