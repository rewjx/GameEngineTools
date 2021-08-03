using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ProtoBuf;
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

	    ~AtxImageBase()
        {
			if(textures != null)
            {
				for(int i=0; i<textures.Count; i++)
                {
					if(textures[i] != null)
                    {
						textures[i].Dispose();
                    }
                }
            }
        }
		public bool Load(string filepath, string baseName= null)
        {
			FileStream fs = new FileStream(filepath, FileMode.Open);
			if(baseName == null)
            {
				baseName = Path.GetFileNameWithoutExtension(filepath);
            }
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
				if (array == null || !this.AddImage(text, array))
				{
					break;
				}
				this.TextureCount += 1;
				num++;
			}
			array = this.ReadBytes(zf, ATLASPBNAME);
			if (array != null)
			{
				using (MemoryStream memoryStream = new MemoryStream(array))
				{
					this.layoutInfo = Serializer.Deserialize<LayoutInfo>(memoryStream);
					this.layoutInfo.OnAfterDeserialize();
				}
			}
			else
			{
				array = this.ReadBytes(zf, ATLASNAME);
				using (FileMem fileMem = new FileMem(ATLASNAME, array))
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
			if (entry == null)
				return null;
			byte[] result = null;
			int num = (int)entry.Size;
			using (Stream inputStream = zf.GetInputStream(entry))
			{
				result = new byte[num];
				inputStream.Read(result, 0, num);
			}
			return result;
		}

		public LayoutInfo.CanvasInfo GetCanvasInfo()
		{
			if (this.layoutInfo == null)
				return null;

			return this.layoutInfo.Canvas;
		}

		public LayoutInfo.BlockInfo GetBlockInfo(string name)
		{
			if (this.layoutInfo == null)
				return null;
			string key = name.ToLower();
			if(this.layoutInfo.blockInfoMap.ContainsKey(key))
            {
				return this.layoutInfo.blockInfoMap[key];
            }
			return null;
		}

		public LayoutInfo.BlockInfo GetBlockInfo(string name, int id)
		{
			if (this.layoutInfo == null)
				return null;
			LayoutInfo.BlockInfo result = this.layoutInfo.SearchBlock(name, id);		
			return result;
		}

		public LayoutInfo.AttributeInfo GetAttributeInfo(int id)
		{
			foreach (LayoutInfo.BlockInfo blockInfo in this.layoutInfo.Block)
			{
				foreach (LayoutInfo.AttributeInfo attributeInfo in blockInfo.Attribute)
				{
					if (attributeInfo.id == id)
					{
						return attributeInfo;
					}
				}
			}
			return null;
		}

		//public List<LayoutInfo.AttributeInfo> GetAttributeInfoList()
		//{
		//	foreach (LayoutInfo.BlockInfo blockInfo in this.layoutInfo.Block)
		//	{
		//		bool flag = blockInfo.Attribute != null;
		//		if (flag)
		//		{
		//			return blockInfo.Attribute;
		//		}
		//	}
		//	return null;
		//}

		protected const string ATLASNAME = "atlas.json";

		protected const string ATLASPBNAME = "atlas.pb";

		public bool IsTextureReadable = false;
	
	}
}
