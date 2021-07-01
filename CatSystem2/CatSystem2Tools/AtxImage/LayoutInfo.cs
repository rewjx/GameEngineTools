using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
namespace AtxImage
{

	[ProtoContract]
	[Serializable]
	public class LayoutInfo
	{
		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			this.blockInfoMap = new Dictionary<string, LayoutInfo.BlockInfo>();
			foreach (LayoutInfo.BlockInfo blockInfo in this.Block)
			{
				blockInfo.filename = blockInfo.filename.ToLower();
				bool flag = !this.blockInfoMap.ContainsKey(blockInfo.filename);
				if (flag)
				{
					this.blockInfoMap.Add(blockInfo.filename, blockInfo);
				}
			}
		}

		public LayoutInfo.BlockInfo SearchBlock(string filename, int id)
		{
			bool flag = !this.blockInfoMap.ContainsKey(filename);
			LayoutInfo.BlockInfo result;
			if (flag)
			{
				result = null;
			}
			else
			{
				filename = filename.ToLower();
				foreach (LayoutInfo.BlockInfo blockInfo in this.Block)
				{
					bool flag2 = blockInfo.filename == filename;
					if (flag2)
					{
						bool flag3 = blockInfo.id == id;
						if (flag3)
						{
							return blockInfo;
						}
					}
				}
				result = null;
			}
			return result;
		}

		[ProtoMember(1)]
		public LayoutInfo.CanvasInfo Canvas;

		[ProtoMember(2)]
		public List<LayoutInfo.BlockInfo> Block;

		[NonSerialized]
		public Dictionary<string, LayoutInfo.BlockInfo> blockInfoMap;

		[ProtoContract]
		[Serializable]
		public class CanvasInfo
		{
			[ProtoMember(1)]
			public int Width;

			[ProtoMember(2)]
			public int Height;
		}

		[ProtoContract]
		[Serializable]
		public class MeshInfo
		{
			[ProtoMember(1)]
			public int texNo;

			[ProtoMember(2)]
			public float offsetX;

			[ProtoMember(3)]
			public float offsetY;

			[ProtoMember(4)]
			public float srcOffsetX;

			[ProtoMember(5)]
			public float srcOffsetY;

			[ProtoMember(6)]
			public float texU1;

			[ProtoMember(7)]
			public float texV1;

			[ProtoMember(8)]
			public float texU2;

			[ProtoMember(9)]
			public float texV2;

			[ProtoMember(10)]
			public float viewX;

			[ProtoMember(11)]
			public float viewY;

			[ProtoMember(12)]
			public float width;

			[ProtoMember(13)]
			public float height;

            public override string ToString()
            {
				return string.Format("offsetX:{0}\t offsetY:{1}\t srcOffsetX:{2}\t srcOffsetY{3}\t\n" +
					"viewX:{4}\t viewY:{5}\t width:{6}\t height:{7}\n texu1:{8}\t " +
					"texu2:{9}\t texv1：{10}\t texv2：{11}\t texNo:{12}\n", offsetX, offsetY,
					srcOffsetX, srcOffsetY, viewX.ToString(), viewY.ToString(), width.ToString(),
					height.ToString(), texU1.ToString(), texU2.ToString(),
					texV1.ToString(), texV2.ToString(), texNo.ToString());
            }
        }

		[ProtoContract]
		[Serializable]
		public class AttributeInfo
		{
			[ProtoMember(1)]
			public int id;

			[ProtoMember(2)]
			public int x;

			[ProtoMember(3)]
			public int y;

			[ProtoMember(4)]
			public int width;

			[ProtoMember(5)]
			public int height;

			[ProtoMember(6)]
			public int color;

            public override string ToString()
            {
				return string.Format("id: {0}\t\n x: {1}\t y:{2}\t " +
					"width:{3}\t height:{4}\t", id, x, y, width, height);
            }
        }

		[ProtoContract]
		[Serializable]
		public class BlockInfo
		{
			[ProtoMember(1)]
			public string filename;

			[ProtoMember(2)]
			public string filenameOld;

			[ProtoMember(3)]
			public int id;

			[ProtoMember(4)]
			public float anchorX;

			[ProtoMember(5)]
			public float anchorY;

			[ProtoMember(6)]
			public float width;

			[ProtoMember(7)]
			public float height;

			[ProtoMember(8)]
			public float offsetX;

			[ProtoMember(9)]
			public float offsetY;

			[ProtoMember(10)]
			public int priority;

			[ProtoMember(11)]
			public List<LayoutInfo.MeshInfo> Mesh;

			[ProtoMember(12)]
			public List<LayoutInfo.AttributeInfo> Attribute;

            public override string ToString()
            {
                string s1 =  string.Format("filename:{0}\t\n" +
                    "anchorX:{1}\t anchorY:{2}\t" +
                    "offsetX:{3}\t offsetY:{4}\t " +
                    "width:{5}\t height:{6}\t" +
                    "priority:{7}\t id:{8}", filename, anchorX.ToString(), anchorY.ToString(),
                    offsetX.ToString(), offsetY.ToString(), width.ToString(), height.ToString(),
                    priority.ToString(), id.ToString() +"\n------------------------------\n");
                string s = "";
				foreach(MeshInfo info in Mesh)
                {
					s += info.ToString() + "\n";
                }
				return s1+s;
			}
        }
	}


}
