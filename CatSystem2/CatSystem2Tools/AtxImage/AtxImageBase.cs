using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ProtoBuf;
using Newtonsoft.Json;
using ICSharpCode.SharpZipLib.Zip;
using System.Drawing;

namespace AtxImage
{
	public class AtxImageBase
	{
		public LayoutInfo layoutInfo { get; protected set; }

		public int TextureCount { get; protected set; }

		public string FileName { get; private set; }

		public List<Bitmap> textures = new List<Bitmap>();


		public bool Load(string filepath, string baseName= null)
        {
			FileStream fs = new FileStream(filepath, FileMode.Open);
			byte[] data = null;
			using(BinaryReader br = new BinaryReader(fs))
            {
				data = br.ReadBytes((int)fs.Length);
            }
			if(data != null)
            {
				return this.Load(data, baseName);
            }
			return false;
        }

		public bool Load(byte[] atx, string baseName = null)
		{
			this.FileName = baseName;
			this.layoutInfo = null;
			bool result;
			using (MemoryStream memoryStream = new MemoryStream(atx))
			{
				result = this.Load(memoryStream, baseName);
			}
			return result;
		}

		public bool Load(Stream stream, string baseName = null)
		{
			this.FileName = baseName;
			this.layoutInfo = null;
			this.textures = new List<Bitmap>();
			stream.Seek(0L, SeekOrigin.Begin);
			ZipFile zf = new ZipFile(stream);
			this.TextureCount = 0;
			int num = 0;
			byte[] array;
			for (; ; )
			{
				string text = string.Format("tex{0}.png", num);
				array = this.ReadBytes(zf, text);
				bool flag = array == null || !this.AddImage(text, array);
				if (flag)
				{
					break;
				}
				int textureCount = this.TextureCount;
				this.TextureCount = textureCount + 1;
				num++;
			}
			array = this.ReadBytes(zf, "atlas.pb");
			bool flag2 = array != null;
			if (flag2)
			{
				using (MemoryStream memoryStream = new MemoryStream(array))
				{
					this.layoutInfo = Serializer.Deserialize<LayoutInfo>(memoryStream);
					this.layoutInfo.OnAfterDeserialize();
				}
			}
			else
			{
				array = this.ReadBytes(zf, "atlas.json");
				using (FileMem fileMem = new FileMem("atlas.json", array))
				{
					string jsonString = fileMem.ReadString();
					this.layoutInfo = Json.Deserialize<LayoutInfo>(jsonString);
				
				}
			}
			return this.layoutInfo != null;
		}

        protected bool AddImage(string textureName, byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
				Bitmap img = new Bitmap(ms);
				if(img != null)
                {
					textures.Add(img);
				}
            }
			return true;
		}

		protected byte[] ReadBytes(ZipFile zf, string name)
		{
			ZipEntry entry = zf.GetEntry(name);
			bool flag = entry == null;
			byte[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				int num = (int)entry.Size;
				using (Stream inputStream = zf.GetInputStream(entry))
				{
					byte[] array = new byte[num];
					inputStream.Read(array, 0, num);
					result = array;
				}
			}
			return result;
		}

		public LayoutInfo.CanvasInfo GetCanvasInfo()
		{
			bool flag = this.layoutInfo == null;
			LayoutInfo.CanvasInfo result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = this.layoutInfo.Canvas;
			}
			return result;
		}

		public LayoutInfo.BlockInfo GetBlockInfo(string name)
		{
			bool flag = this.layoutInfo == null;
			LayoutInfo.BlockInfo result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string key = name.ToLower();
				bool flag2 = !this.layoutInfo.blockInfoMap.ContainsKey(key);
				if (flag2)
				{
					result = null;
				}
				else
				{
					result = this.layoutInfo.blockInfoMap[key];
				}
			}
			return result;
		}

		public LayoutInfo.BlockInfo GetBlockInfo(string name, int id)
		{
			bool flag = this.layoutInfo == null;
			LayoutInfo.BlockInfo result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = this.layoutInfo.SearchBlock(name, id);
			}
			return result;
		}

		public LayoutInfo.AttributeInfo GetAttributeInfo(int id)
		{
			foreach (LayoutInfo.BlockInfo blockInfo in this.layoutInfo.Block)
			{
				foreach (LayoutInfo.AttributeInfo attributeInfo in blockInfo.Attribute)
				{
					bool flag = attributeInfo.id == id;
					if (flag)
					{
						return attributeInfo;
					}
				}
			}
			return null;
		}

		public List<LayoutInfo.AttributeInfo> GetAttributeInfoList()
		{
			foreach (LayoutInfo.BlockInfo blockInfo in this.layoutInfo.Block)
			{
				bool flag = blockInfo.Attribute != null;
				if (flag)
				{
					return blockInfo.Attribute;
				}
			}
			return null;
		}

		protected const string ATLASNAME = "atlas.json";

		protected const string ATLASPBNAME = "atlas.pb";

		public bool IsTextureReadable = false;
	
	}
}
