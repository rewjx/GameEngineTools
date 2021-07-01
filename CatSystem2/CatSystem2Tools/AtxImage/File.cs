using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace AtxImage
{
	public abstract class File : IDisposable
	{
		public File.ENCODE Encode { get; protected set; } = File.ENCODE.Unknown;


		public string Filename { get; protected set; } = "";


		// Token: 0x060000D1 RID: 209
		public abstract Stream GetStream();

		// Token: 0x060000D2 RID: 210 RVA: 0x00005078 File Offset: 0x00003278
		public string ReadString()
		{
			byte[] array = this.ReadBytes();
			bool flag = array == null;
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				result = this.ToString(array);
			}
			return result;
		}

		public abstract byte[] ReadBytes();

		// Token: 0x060000D4 RID: 212 RVA: 0x000050A8 File Offset: 0x000032A8
		//public AtxImage ReadAtxImage()
		//{
		//	byte[] atx = this.ReadBytes();
		//	AtxImage atxImage = new AtxImage();
		//	atxImage.Load(atx, null);
		//	return atxImage;
		//}

		// Token: 0x060000D5 RID: 213
		public abstract void Dispose();

		// Token: 0x060000D6 RID: 214 RVA: 0x000050D4 File Offset: 0x000032D4
		internal File.ENCODE CheckBom(byte[] str)
		{
			File.ENCODE result = File.ENCODE.Unknown;
			bool flag = str.Length >= 2;
			if (flag)
			{
				bool flag2 = str[0] == 254 && str[1] == byte.MaxValue;
				if (flag2)
				{
					result = File.ENCODE.UTF16BE;
				}
				else
				{
					bool flag3 = str[0] == byte.MaxValue && str[1] == 254;
					if (flag3)
					{
						result = File.ENCODE.UTF16LE;
					}
					else
					{
						bool flag4 = str.Length >= 3;
						if (flag4)
						{
							bool flag5 = str[0] == 239 && str[1] == 187 && str[2] == 191;
							if (flag5)
							{
								result = File.ENCODE.UTF8;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000517C File Offset: 0x0000337C
		internal string GetEncodingName(File.ENCODE enc)
		{
			string result;
			switch (enc)
			{
				case File.ENCODE.ShiftJis:
					result = "shift_jis";
					break;
				case File.ENCODE.UTF8:
					result = "utf-8";
					break;
				case File.ENCODE.UTF16BE:
					result = "unicodeFFFE";
					break;
				case File.ENCODE.UTF16LE:
					result = "utf-16";
					break;
				default:
					result = "utf-8";
					break;
			}
			return result;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x000051D8 File Offset: 0x000033D8
		internal string ToString(byte[] src)
		{
			this.Encode = this.CheckBom(src);
			string result;
			switch (this.Encode)
			{
				case File.ENCODE.UTF8:
					result = this.ToStringUtf8(src, 3);
					break;
				case File.ENCODE.UTF16BE:
					result = this.ToStringUtf16BE(src, 3);
					break;
				case File.ENCODE.UTF16LE:
					result = this.ToStringUtf16LE(src, 3);
					break;
				default:
					switch (TextEncoding.GetEncoding(src))
					{
						case TextEncoding.EncodingType.ShiftJIS:
							this.Encode = File.ENCODE.ShiftJis;
							result = this.ToStringShiftJIS(src);
							break;
						case TextEncoding.EncodingType.UTF8:
							this.Encode = File.ENCODE.UTF8;
							result = this.ToStringUtf8(src, 0);
							break;
						case TextEncoding.EncodingType.ASCII:
							this.Encode = File.ENCODE.Ascii;
							result = this.ToStringAscii(src);
							break;
						default:
							this.Encode = File.ENCODE.Unknown;
							result = "";
							break;
					}
					break;
			}
			return result;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x000052A4 File Offset: 0x000034A4
		internal string ToStringUtf16BE(byte[] src, int index = 0)
		{
			return Encoding.GetEncoding("unicodeFFFE").GetString(src, index, src.Length - index);
		}

		// Token: 0x060000DA RID: 218 RVA: 0x000052CC File Offset: 0x000034CC
		internal string ToStringUtf16LE(byte[] src, int index = 0)
		{
			return Encoding.GetEncoding("utf-16").GetString(src, index, src.Length - index);
		}

		// Token: 0x060000DB RID: 219 RVA: 0x000052F4 File Offset: 0x000034F4
		internal string ToStringUtf8(byte[] src, int index = 0)
		{
			return Encoding.UTF8.GetString(src, index, src.Length - index);
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00005318 File Offset: 0x00003518
		internal string ToStringShiftJIS(byte[] src)
		{
			return Encoding.GetEncoding("shift_jis").GetString(src);
		}

		// Token: 0x060000DD RID: 221 RVA: 0x0000533C File Offset: 0x0000353C
		internal string ToStringAscii(byte[] src)
		{
			return Encoding.ASCII.GetString(src);
		}

		// Token: 0x060000DE RID: 222
		public abstract int GetSize();

		// Token: 0x060000DF RID: 223 RVA: 0x0000535C File Offset: 0x0000355C
		protected Vector2Int GetPngCanvasSize(byte[] data)
		{
			bool flag = data.Length < 24;
			Vector2Int result;
			if (flag)
			{
				result = Vector2Int.zero;
			}
			else
			{
				bool flag2 = data[0] != 137 || data[1] != 80 || data[2] != 78 || data[3] != 71;
				if (flag2)
				{
					result = Vector2Int.zero;
				}
				else
				{
					int num = 16;
					int num2 = 0;
					for (int i = 0; i < 4; i++)
					{
						num2 = num2 * 256 + (int)data[num++];
					}
					int num3 = 0;
					for (int j = 0; j < 4; j++)
					{
						num3 = num3 * 256 + (int)data[num++];
					}
					result = new Vector2Int(num2, num3);
				}
			}
			return result;
		}

		// Token: 0x0200002B RID: 43
		public enum ENCODE
		{

			Unknown,

			ShiftJis,

			UTF8,

			UTF16BE,

			UTF16LE,

			Ascii
		}
	}
}
