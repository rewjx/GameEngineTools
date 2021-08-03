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


		public abstract Stream GetStream();


		public string ReadString()
		{
			byte[] array = this.ReadBytes();
			string result;
			if (array == null)
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

		public abstract void Dispose();


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


		internal string ToStringUtf16BE(byte[] src, int index = 0)
		{
			return Encoding.GetEncoding("unicodeFFFE").GetString(src, index, src.Length - index);
		}


		internal string ToStringUtf16LE(byte[] src, int index = 0)
		{
			return Encoding.GetEncoding("utf-16").GetString(src, index, src.Length - index);
		}


		internal string ToStringUtf8(byte[] src, int index = 0)
		{
			return Encoding.UTF8.GetString(src, index, src.Length - index);
		}


		internal string ToStringShiftJIS(byte[] src)
		{
			return Encoding.GetEncoding("shift_jis").GetString(src);
		}


		internal string ToStringAscii(byte[] src)
		{
			return Encoding.ASCII.GetString(src);
		}


		public abstract int GetSize();


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
