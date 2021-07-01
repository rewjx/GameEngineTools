using System;
using System.IO;

namespace AtxImage
{
	// Token: 0x02000013 RID: 19
	public class FileMem : File
	{
		// Token: 0x06000106 RID: 262 RVA: 0x00005F4A File Offset: 0x0000414A
		public FileMem(string name, byte[] data)
		{
			base.Filename = name;
			this.Data = data;
		}



		// Token: 0x0600010A RID: 266 RVA: 0x0000600C File Offset: 0x0000420C
		public override Stream GetStream()
		{
			bool flag = this.Data == null;
			Stream result;
			if (flag)
			{
				result = null;
			}
			else
			{
				MemoryStream memoryStream = new MemoryStream(this.Data);
				result = memoryStream;
			}
			return result;
		}

		// Token: 0x0600010B RID: 267 RVA: 0x0000603C File Offset: 0x0000423C
		public override byte[] ReadBytes()
		{
			return this.Data;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00006054 File Offset: 0x00004254
		public override void Dispose()
		{
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00006058 File Offset: 0x00004258
		public override int GetSize()
		{
			bool flag = this.Data == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				result = this.Data.Length;
			}
			return result;
		}

		// Token: 0x04000062 RID: 98
		private byte[] Data = null;
	}
}
