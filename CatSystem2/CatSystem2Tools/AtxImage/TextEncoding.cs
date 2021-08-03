using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AtxImage
{

	/// <summary>
	/// 编码检测
	/// </summary>
	public class TextEncoding
	{
		public static TextEncoding.EncodingType GetEncoding(byte[] src)
		{
			bool flag = TextEncoding.CheckAscii(src);
			TextEncoding.EncodingType result;
			if (flag)
			{
				result = TextEncoding.EncodingType.ASCII;
			}
			else
			{
				bool flag2 = TextEncoding.CheckUtf8(src);
				if (flag2)
				{
					result = TextEncoding.EncodingType.UTF8;
				}
				else
				{
					bool flag3 = TextEncoding.CheckShiftJIS(src);
					if (flag3)
					{
						result = TextEncoding.EncodingType.ShiftJIS;
					}
					else
					{
						result = TextEncoding.EncodingType.unknown;
					}
				}
			}
			return result;
		}

		public static bool CheckAscii(byte[] src)
		{
			using (MemoryStream memoryStream = new MemoryStream(src))
			{
				int i = 0;
				int num = src.Length;
				while (i < num)
				{
					byte b = (byte)memoryStream.ReadByte();
					i++;
					bool flag = b > 127;
					if (flag)
					{
						return false;
					}
				}
			}
			return true;
		}

		public static bool CheckShiftJIS(byte[] src)
		{
			using (MemoryStream memoryStream = new MemoryStream(src))
			{
				int i = 0;
				int num = src.Length;
				while (i < num)
				{
					byte b = (byte)memoryStream.ReadByte();
					i++;
					bool flag = b <= 127 || (161 <= b && b <= 223);
					if (!flag)
					{
						bool flag2 = b > 239;
						if (flag2)
						{
							return false;
						}
						bool flag3 = i >= num;
						if (flag3)
						{
							return false;
						}
						byte b2 = (byte)memoryStream.ReadByte();
						i++;
						bool flag4 = (64 <= b2 && b2 <= 126) || (128 <= b2 && b2 <= 252);
						if (!flag4)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public static bool CheckUtf8(byte[] src)
		{
			using (MemoryStream memoryStream = new MemoryStream(src))
			{
				int i = 0;
				int num = src.Length;
				while (i < num)
				{
					byte b = (byte)memoryStream.ReadByte();
					i++;
					bool flag = b <= 127;
					if (!flag)
					{
						bool flag2 = b < 194 || 244 < b;
						if (flag2)
						{
							return false;
						}
						bool flag3 = b <= 223;
						if (flag3)
						{
							bool flag4 = i + 1 > num;
							if (flag4)
							{
								return false;
							}
							byte b2 = (byte)memoryStream.ReadByte();
							i++;
							bool flag5 = b2 < 128 || 191 < b2;
							if (flag5)
							{
								return false;
							}
						}
						else
						{
							bool flag6 = b <= 239;
							if (flag6)
							{
								bool flag7 = i + 2 > num;
								if (flag7)
								{
									return false;
								}
								byte b3 = (byte)memoryStream.ReadByte();
								byte b4 = (byte)memoryStream.ReadByte();
								i += 2;
								bool flag8 = b3 < 128 || 191 < b3;
								if (flag8)
								{
									return false;
								}
								bool flag9 = b4 < 128 || 191 < b4;
								if (flag9)
								{
									return false;
								}
								bool flag10 = b == 224;
								if (flag10)
								{
									bool flag11 = b3 < 160;
									if (flag11)
									{
										return false;
									}
								}
								bool flag12 = b == 237;
								if (flag12)
								{
									bool flag13 = b3 > 159;
									if (flag13)
									{
										return false;
									}
								}
							}
							else
							{
								bool flag14 = b <= 244;
								if (flag14)
								{
									bool flag15 = i + 3 > num;
									if (flag15)
									{
										return false;
									}
									byte b5 = (byte)memoryStream.ReadByte();
									byte b6 = (byte)memoryStream.ReadByte();
									byte b7 = (byte)memoryStream.ReadByte();
									i += 3;
									bool flag16 = b5 < 128 || 191 < b5;
									if (flag16)
									{
										return false;
									}
									bool flag17 = b6 < 128 || 191 < b6;
									if (flag17)
									{
										return false;
									}
									bool flag18 = b7 < 128 || 191 < b7;
									if (flag18)
									{
										return false;
									}
									bool flag19 = b == 240;
									if (flag19)
									{
										bool flag20 = b5 < 144;
										if (flag20)
										{
											return false;
										}
									}
									bool flag21 = b == 244;
									if (flag21)
									{
										bool flag22 = b5 > 143;
										if (flag22)
										{
											return false;
										}
									}
								}
							}
						}
					}
				}
			}
			return true;
		}


	public enum EncodingType
	{

		unknown,

		ShiftJIS,

		UTF8,

		ASCII
	}
}

}
