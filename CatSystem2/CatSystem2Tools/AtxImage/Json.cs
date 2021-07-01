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
		// Token: 0x0600008D RID: 141 RVA: 0x0000488C File Offset: 0x00002A8C
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

		// Token: 0x0600008E RID: 142 RVA: 0x00004938 File Offset: 0x00002B38
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

		// Token: 0x0600008F RID: 143 RVA: 0x00004A0C File Offset: 0x00002C0C
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
