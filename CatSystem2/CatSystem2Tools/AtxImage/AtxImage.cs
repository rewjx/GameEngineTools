//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AtxImage
//{
//	public class AtxImage : AtxImageBase
//	{
//		// Token: 0x17000008 RID: 8
//		// (get) Token: 0x0600001A RID: 26 RVA: 0x000025FD File Offset: 0x000007FD
//		// (set) Token: 0x0600001B RID: 27 RVA: 0x00002605 File Offset: 0x00000805
//		public List<Texture2D> texture2D { get; private set; } = new List<Texture2D>();

//		// Token: 0x0600001C RID: 28 RVA: 0x00002610 File Offset: 0x00000810
//		protected override bool AddImage(string textureName, byte[] data)
//		{
//			base.DataSize = 0;
//			using (FileMem fileMem = new FileMem(textureName, data))
//			{
//				Texture2D texture2D = fileMem.ReadTexture2D(!this.IsTextureReadable);
//				this.texture2D.Add(texture2D);
//				base.DataSize += texture2D.width * texture2D.height * 4;
//			}
//			return true;
//		}
//	}
//}
