using System;
using System.IO;

namespace AtxImage
{

	public class FileMem : File
	{
		public FileMem(string name, byte[] data)
		{
			base.Filename = name;
			this.Data = data;
		}



		public override Stream GetStream()
		{
			if (this.Data == null)
				return null;
			MemoryStream memoryStream = new MemoryStream(this.Data);
			Stream result = memoryStream;
			return result;
		}


		public override byte[] ReadBytes()
		{
			return this.Data;
		}

		public override void Dispose()
		{
		}

		public override int GetSize()
		{
			if (this.Data == null)
				return 0;
			return this.Data.Length;
		}

		private byte[] Data = null;
	}
}
