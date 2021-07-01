//using System;
//using System.Collections.Generic;

//namespace AtxImage
//{
//	// Token: 0x02000007 RID: 7
//	public class AtxMesh
//	{
//		// Token: 0x17000010 RID: 16
//		// (get) Token: 0x06000042 RID: 66 RVA: 0x00002F71 File Offset: 0x00001171
//		// (set) Token: 0x06000043 RID: 67 RVA: 0x00002F79 File Offset: 0x00001179
//		public float anchorX { get; private set; }

//		// Token: 0x17000011 RID: 17
//		// (get) Token: 0x06000044 RID: 68 RVA: 0x00002F82 File Offset: 0x00001182
//		// (set) Token: 0x06000045 RID: 69 RVA: 0x00002F8A File Offset: 0x0000118A
//		public float anchorY { get; private set; }

//		// Token: 0x17000012 RID: 18
//		// (get) Token: 0x06000046 RID: 70 RVA: 0x00002F93 File Offset: 0x00001193
//		// (set) Token: 0x06000047 RID: 71 RVA: 0x00002F9B File Offset: 0x0000119B
//		public float width { get; private set; }

//		// Token: 0x17000013 RID: 19
//		// (get) Token: 0x06000048 RID: 72 RVA: 0x00002FA4 File Offset: 0x000011A4
//		// (set) Token: 0x06000049 RID: 73 RVA: 0x00002FAC File Offset: 0x000011AC
//		public float height { get; private set; }

//		// Token: 0x17000014 RID: 20
//		// (get) Token: 0x0600004A RID: 74 RVA: 0x00002FB5 File Offset: 0x000011B5
//		// (set) Token: 0x0600004B RID: 75 RVA: 0x00002FBD File Offset: 0x000011BD
//		public float offsetX { get; private set; }

//		// Token: 0x17000015 RID: 21
//		// (get) Token: 0x0600004C RID: 76 RVA: 0x00002FC6 File Offset: 0x000011C6
//		// (set) Token: 0x0600004D RID: 77 RVA: 0x00002FCE File Offset: 0x000011CE
//		public float offsetY { get; private set; }

//		// Token: 0x17000016 RID: 22
//		// (get) Token: 0x0600004E RID: 78 RVA: 0x00002FD7 File Offset: 0x000011D7
//		// (set) Token: 0x0600004F RID: 79 RVA: 0x00002FDF File Offset: 0x000011DF
//		public int priority { get; private set; }

//		// Token: 0x17000017 RID: 23
//		// (get) Token: 0x06000050 RID: 80 RVA: 0x00002FE8 File Offset: 0x000011E8
//		// (set) Token: 0x06000051 RID: 81 RVA: 0x00002FF0 File Offset: 0x000011F0
//		public Rect boundingBox { get; private set; } = default(Rect);

//		// Token: 0x17000018 RID: 24
//		// (get) Token: 0x06000052 RID: 82 RVA: 0x00002FF9 File Offset: 0x000011F9
//		// (set) Token: 0x06000053 RID: 83 RVA: 0x00003001 File Offset: 0x00001201
//		public Quaternion rotation { get; private set; } = default(Quaternion);

//		// Token: 0x06000054 RID: 84 RVA: 0x0000300C File Offset: 0x0000120C
//		public AtxMesh(AtxImageBase atx, string filename, int id, ImageFlipAttribute flip, bool posMode = false)
//		{
//			this.posMode = false;
//			bool flag = !(this.modeFes = (id >= 0));
//			if (flag)
//			{
//				id = 0;
//				this.posMode = posMode;
//			}
//			this.canvasInfo = atx.GetCanvasInfo();
//			LayoutInfo.BlockInfo blockInfo = atx.GetBlockInfo(filename, id);
//			bool flag2 = blockInfo != null;
//			if (flag2)
//			{
//				this.SetStatusFromInfo(blockInfo);
//				this.AddMeshListFromBlockInfo(atx, blockInfo);
//				this.blockAnchor = this.GetAnchorFromAttribute(blockInfo, flip.Attribute);
//				this.posInfo = this.AdjustPosition(atx.GetCanvasInfo(), blockInfo, flip.Attribute, 0f);
//				this.boundingBox = new Rect(blockInfo.offsetX, blockInfo.offsetY, blockInfo.width, blockInfo.height);
//				this.Rectangle.x = (int)(this.posInfo.offset.x - this.posInfo.anchor.x);
//				this.Rectangle.y = (int)(this.posInfo.offset.y - this.posInfo.anchor.y);
//				this.Rectangle.width = (int)blockInfo.width;
//				this.Rectangle.height = (int)blockInfo.height;
//				foreach (KeyValuePair<int, AtxMesh.MeshListWithRange> keyValuePair in this.indexedMeshList)
//				{
//					AtxMesh.MeshListWithRange value = keyValuePair.Value;
//					value.start = this.vertIndex.Count / 6;
//					this.MakeVertex(blockInfo, value.meshList, flip);
//					value.size = this.vertIndex.Count / 6 - value.start;
//				}
//			}
//		}

//		// Token: 0x06000055 RID: 85 RVA: 0x00003248 File Offset: 0x00001448
//		public void Dispose()
//		{
//			this.indexedMeshList.Clear();
//			this.vertPos.Clear();
//			this.vertUv.Clear();
//			this.vertIndex.Clear();
//		}

//		// Token: 0x06000056 RID: 86 RVA: 0x0000327C File Offset: 0x0000147C
//		public Rect GetBoundingBox()
//		{
//			return this.boundingBox;
//		}

//		// Token: 0x06000057 RID: 87 RVA: 0x00003294 File Offset: 0x00001494
//		public void GetRectangleList(List<RectInt> list)
//		{
//			list.Add(this.Rectangle);
//		}

//		// Token: 0x06000058 RID: 88 RVA: 0x000032A4 File Offset: 0x000014A4
//		public void GetTextures(List<int> textureIndexList)
//		{
//			foreach (KeyValuePair<int, AtxMesh.MeshListWithRange> keyValuePair in this.indexedMeshList)
//			{
//				bool flag = !textureIndexList.Contains(keyValuePair.Key);
//				if (flag)
//				{
//					textureIndexList.Add(keyValuePair.Key);
//				}
//			}
//		}

//		// Token: 0x06000059 RID: 89 RVA: 0x0000331C File Offset: 0x0000151C
//		public void GetVertex(int tex, List<Vector3> pos, List<Vector2> uv, List<int> index)
//		{
//			int count = index.Count;
//			int num = index.Count / 6 * 4;
//			AtxMesh.MeshListWithRange meshListWithRange;
//			bool flag = this.indexedMeshList.TryGetValue(tex, out meshListWithRange);
//			if (flag)
//			{
//				int start = meshListWithRange.start;
//				int size = meshListWithRange.size;
//				pos.AddRange(this.vertPos.GetRange(start * 4, size * 4));
//				uv.AddRange(this.vertUv.GetRange(start * 4, size * 4));
//				index.AddRange(this.vertIndex.GetRange(start * 6, size * 6));
//				for (int i = count; i < index.Count; i++)
//				{
//					int index2 = i;
//					index[index2] += num;
//				}
//			}
//		}

//		// Token: 0x0600005A RID: 90 RVA: 0x000033F0 File Offset: 0x000015F0
//		private void SetStatusFromInfo(LayoutInfo.BlockInfo blockInfo)
//		{
//			this.anchorX = blockInfo.anchorX;
//			this.anchorY = blockInfo.anchorY;
//			this.width = blockInfo.width;
//			this.height = blockInfo.height;
//			this.offsetX = blockInfo.offsetX;
//			this.offsetY = blockInfo.offsetY;
//			this.priority = blockInfo.priority;
//		}

//		// Token: 0x0600005B RID: 91 RVA: 0x0000345C File Offset: 0x0000165C
//		private void AddMeshListFromBlockInfo(AtxImageBase atx, LayoutInfo.BlockInfo blockInfo)
//		{
//			foreach (LayoutInfo.MeshInfo meshInfo in blockInfo.Mesh)
//			{
//				int texNo = meshInfo.texNo;
//				AtxMesh.MeshListWithRange meshListWithRange;
//				bool flag = !this.indexedMeshList.TryGetValue(texNo, out meshListWithRange);
//				if (flag)
//				{
//					meshListWithRange = new AtxMesh.MeshListWithRange();
//					this.indexedMeshList[texNo] = meshListWithRange;
//				}
//				meshListWithRange.Add(meshInfo);
//			}
//		}

//		// Token: 0x0600005C RID: 92 RVA: 0x000034EC File Offset: 0x000016EC
//		private Vector2 GetAnchorFromAttribute(LayoutInfo.BlockInfo blockInfo, ImageFlipAttribute.AttributeType attribute)
//		{
//			Vector2 vector = new Vector2(0f, 0f);
//			bool flag = attribute.HasFlag(ImageFlipAttribute.AttributeType.FlipUD);
//			if (flag)
//			{
//				vector.y = 1f - vector.y;
//			}
//			bool flag2 = attribute.HasFlag(ImageFlipAttribute.AttributeType.Rotate90);
//			if (flag2)
//			{
//				float x = vector.x;
//				float y = vector.y;
//				vector.x = 1f - y;
//				vector.y = x;
//			}
//			bool flag3 = attribute.HasFlag(ImageFlipAttribute.AttributeType.Rotate180);
//			if (flag3)
//			{
//				vector.x = 1f - vector.x;
//				vector.y = 1f - vector.y;
//			}
//			bool flag4 = attribute.HasFlag(ImageFlipAttribute.AttributeType.Rotate270);
//			if (flag4)
//			{
//				float x2 = vector.x;
//				float y2 = vector.y;
//				vector.x = y2;
//				vector.y = 1f - x2;
//			}
//			return vector;
//		}

//		// Token: 0x0600005D RID: 93 RVA: 0x000035FC File Offset: 0x000017FC
//		private void MakeVertex(LayoutInfo.BlockInfo blockInfo, List<LayoutInfo.MeshInfo> meshList, ImageFlipAttribute flip)
//		{
//			int num = 0;
//			foreach (LayoutInfo.MeshInfo meshInfo in meshList)
//			{
//				this.AddSquareMesh(blockInfo, meshInfo, num);
//				num += 4;
//			}
//		}

//		// Token: 0x0600005E RID: 94 RVA: 0x00003658 File Offset: 0x00001858
//		private void AddSquareMesh(LayoutInfo.BlockInfo blockInfo, LayoutInfo.MeshInfo meshInfo, int index)
//		{
//			float num = this.posInfo.position.x + meshInfo.srcOffsetX;
//			float num2 = this.posInfo.position.y - meshInfo.srcOffsetY - meshInfo.height;
//			num -= this.blockAnchor.x * blockInfo.width;
//			num2 -= this.blockAnchor.y * blockInfo.height;
//			bool flag = this.modeFes;
//			if (flag)
//			{
//				num += blockInfo.offsetX;
//				num2 -= blockInfo.offsetY;
//			}
//			Rect rect = new Rect(0f, 0f, meshInfo.width, meshInfo.height);
//			rect.position = new Vector2(num, num2);
//			float z = 0f;
//			this.vertPos.Add(new Vector3(rect.xMin, rect.yMin, z));
//			this.vertPos.Add(new Vector3(rect.xMax, rect.yMax, z));
//			this.vertPos.Add(new Vector3(rect.xMax, rect.yMin, z));
//			this.vertPos.Add(new Vector3(rect.xMin, rect.yMax, z));
//			this.vertUv.Add(new Vector2(meshInfo.texU1, 1f - meshInfo.texV2));
//			this.vertUv.Add(new Vector2(meshInfo.texU2, 1f - meshInfo.texV1));
//			this.vertUv.Add(new Vector2(meshInfo.texU2, 1f - meshInfo.texV2));
//			this.vertUv.Add(new Vector2(meshInfo.texU1, 1f - meshInfo.texV1));
//			this.vertIndex.Add(index);
//			this.vertIndex.Add(3 + index);
//			this.vertIndex.Add(1 + index);
//			this.vertIndex.Add(index);
//			this.vertIndex.Add(1 + index);
//			this.vertIndex.Add(2 + index);
//		}

//		// Token: 0x0600005F RID: 95 RVA: 0x0000387C File Offset: 0x00001A7C
//		private AtxMesh.AdjustPositionInfo AdjustPosition(LayoutInfo.CanvasInfo canvasInfo, LayoutInfo.BlockInfo blockInfo, ImageFlipAttribute.AttributeType attribute, float dimention)
//		{
//			AtxMesh.AdjustPositionInfo adjustPositionInfo = new AtxMesh.AdjustPositionInfo();
//			bool flag = attribute.HasFlag(ImageFlipAttribute.AttributeType.FlipLR);
//			if (flag)
//			{
//			}
//			bool flag2 = attribute.HasFlag(ImageFlipAttribute.AttributeType.FlipUD);
//			if (flag2)
//			{
//			}
//			bool flag3 = attribute.HasFlag(ImageFlipAttribute.AttributeType.Rotate90);
//			if (flag3)
//			{
//			}
//			bool flag4 = attribute.HasFlag(ImageFlipAttribute.AttributeType.Rotate180);
//			if (flag4)
//			{
//			}
//			bool flag5 = attribute.HasFlag(ImageFlipAttribute.AttributeType.Rotate270);
//			if (flag5)
//			{
//			}
//			float z = dimention * 0.01f;
//			adjustPositionInfo.offset = new Vector2(blockInfo.offsetX, blockInfo.offsetY);
//			adjustPositionInfo.anchor = new Vector2(blockInfo.anchorX, blockInfo.anchorY);
//			bool flag6 = this.posMode;
//			if (flag6)
//			{
//				adjustPositionInfo.anchor += adjustPositionInfo.offset;
//			}
//			bool flag7 = attribute.HasFlag(ImageFlipAttribute.AttributeType.FlipLR);
//			if (flag7)
//			{
//				adjustPositionInfo.offset.x = (float)canvasInfo.Width - blockInfo.offsetX - blockInfo.width;
//			}
//			bool flag8 = attribute.HasFlag(ImageFlipAttribute.AttributeType.FlipUD);
//			if (flag8)
//			{
//				adjustPositionInfo.offset.y = (float)canvasInfo.Height - blockInfo.offsetY - blockInfo.height;
//				adjustPositionInfo.anchor.y = (float)canvasInfo.Height - blockInfo.anchorY;
//			}
//			bool flag9 = attribute.HasFlag(ImageFlipAttribute.AttributeType.Rotate90);
//			if (flag9)
//			{
//				Vector2 vector = adjustPositionInfo.anchor;
//				adjustPositionInfo.anchor.x = (float)canvasInfo.Height - vector.y;
//				adjustPositionInfo.anchor.y = vector.x;
//				vector = adjustPositionInfo.offset;
//				adjustPositionInfo.offset.x = (float)canvasInfo.Height - vector.y - blockInfo.height;
//				adjustPositionInfo.offset.y = vector.x;
//			}
//			bool flag10 = attribute.HasFlag(ImageFlipAttribute.AttributeType.Rotate180);
//			if (flag10)
//			{
//				adjustPositionInfo.anchor.x = (float)canvasInfo.Width - adjustPositionInfo.anchor.x;
//				adjustPositionInfo.anchor.y = (float)canvasInfo.Height - adjustPositionInfo.anchor.y;
//				adjustPositionInfo.offset.x = (float)canvasInfo.Width - adjustPositionInfo.offset.x - blockInfo.width;
//				adjustPositionInfo.offset.y = (float)canvasInfo.Height - adjustPositionInfo.offset.y - blockInfo.height;
//			}
//			bool flag11 = attribute.HasFlag(ImageFlipAttribute.AttributeType.Rotate270);
//			if (flag11)
//			{
//				Vector2 vector2 = adjustPositionInfo.anchor;
//				adjustPositionInfo.anchor.x = vector2.y;
//				adjustPositionInfo.anchor.y = (float)canvasInfo.Width - adjustPositionInfo.offset.x - blockInfo.width;
//				vector2 = adjustPositionInfo.offset;
//				adjustPositionInfo.offset.x = vector2.y;
//				adjustPositionInfo.offset.y = (float)canvasInfo.Width - vector2.x - blockInfo.width;
//			}
//			adjustPositionInfo.position = new Vector3(adjustPositionInfo.offset.x, -adjustPositionInfo.offset.y, z);
//			AtxMesh.AdjustPositionInfo adjustPositionInfo2 = adjustPositionInfo;
//			adjustPositionInfo2.position.x = adjustPositionInfo2.position.x - adjustPositionInfo.anchor.x;
//			AtxMesh.AdjustPositionInfo adjustPositionInfo3 = adjustPositionInfo;
//			adjustPositionInfo3.position.y = adjustPositionInfo3.position.y + adjustPositionInfo.anchor.y;
//			return adjustPositionInfo;
//		}

//		// Token: 0x04000021 RID: 33
//		private Dictionary<int, AtxMesh.MeshListWithRange> indexedMeshList = new Dictionary<int, AtxMesh.MeshListWithRange>();

//		// Token: 0x04000022 RID: 34
//		private List<Vector3> vertPos = new List<Vector3>();

//		// Token: 0x04000023 RID: 35
//		private List<Vector2> vertUv = new List<Vector2>();

//		// Token: 0x04000024 RID: 36
//		private List<int> vertIndex = new List<int>();

//		// Token: 0x0400002E RID: 46
//		private Vector2 blockAnchor;

//		// Token: 0x0400002F RID: 47
//		private AtxMesh.AdjustPositionInfo posInfo;

//		// Token: 0x04000030 RID: 48
//		private LayoutInfo.CanvasInfo canvasInfo;

//		// Token: 0x04000031 RID: 49
//		private RectInt Rectangle = default(RectInt);

//		// Token: 0x04000032 RID: 50
//		private bool modeFes = false;

//		// Token: 0x04000033 RID: 51
//		private bool posMode = false;

//		// Token: 0x02000026 RID: 38
//		private class MeshListWithRange
//		{
//			// Token: 0x060001B4 RID: 436 RVA: 0x00008E78 File Offset: 0x00007078
//			public void Add(LayoutInfo.MeshInfo mesh)
//			{
//				this.meshList.Add(mesh);
//			}

//			// Token: 0x040000C3 RID: 195
//			public List<LayoutInfo.MeshInfo> meshList = new List<LayoutInfo.MeshInfo>();

//			// Token: 0x040000C4 RID: 196
//			public int start;

//			// Token: 0x040000C5 RID: 197
//			public int size;
//		}

//		// Token: 0x02000027 RID: 39
//		private class AdjustPositionInfo
//		{
//			// Token: 0x040000C6 RID: 198
//			public Vector3 position;

//			// Token: 0x040000C7 RID: 199
//			public Vector2 offset;

//			// Token: 0x040000C8 RID: 200
//			public Vector2 anchor;
//		}
//	}
//}
