using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextBin
{
    class Helper
    {
        public static int ToInt32BigEndian(byte[] data, int startpos)
        {
            return (data[startpos] << 24) | (data[startpos + 1] << 16) | 
                (data[startpos + 2] << 8) | (data[startpos + 3]);
        }

        public static short ToInt16BigEndian(byte[] data, int startpos)
        {
            return (short)((data[startpos] << 8) | data[startpos + 1]);
        }

        public static void WriteShortBigEndian(short value, ref byte[] data, int startpos)
        {
            //扩容数组
            if(startpos + 2 > data.Length)
            {
                byte[] newdata = new byte[startpos + 2];
                Array.Copy(data, newdata, data.Length);
                data = newdata;
            }
            data[startpos + 1] = (byte)(value & 0xFF);
            value = (byte)(value >> 8);
            data[startpos] = (byte)(value & 0xFF);
        }

        public static void WriteIntBigEndian(int value, ref byte[] data, int startpos)
        {
            //扩容数组
            if (startpos + 4 > data.Length)
            {
                byte[] newdata = new byte[startpos + 4];
                Array.Copy(data, newdata, data.Length);
                data = newdata;
            }
            data[startpos + 3] = (byte)(value & 0xFF);
            value = value >> 8;
            data[startpos + 2] = (byte)(value & 0xFF);
            value = value >> 8;
            data[startpos + 1] = (byte)(value & 0xFF);
            value = value >> 8;
            data[startpos]     = (byte)(value & 0xFF);
        }
    }
}
