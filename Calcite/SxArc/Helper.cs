using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace SxArc
{
    class Helper
    {
        public static int IntEndianTran(int v)
        {
            byte v1 = (byte)(v & 0xFF);
            v >>= 8;
            byte v2 = (byte)(v & 0xFF);
            v >>= 8;
            byte v3 = (byte)(v & 0xFF);
            v >>= 8;
            byte v4 = (byte)(v & 0xFF);
            int rtn = (int)((v1 << 24) | (v2 << 16) | (v3 << 8) | v4);
            return rtn;
        }

        public static short ShortEndianTran(short v)
        {
            byte v1 = (byte)(v & 0x00FF);
            v >>= 8;
            byte v2 = (byte)(v & 0x00FF);
            short rtn = (short)(v1 << 8 | v2);
            return rtn;
        }

        public static uint Combine2Short(ushort high, ushort low)
        {
            uint rtn = 0;
            rtn = high;
            rtn <<= 16;
            rtn = rtn | low;
            return rtn;
        }

        public static Tuple<ushort, ushort> Extract2Short(uint v)
        {
            ushort low = (ushort)(v & 0x0000FFFF);
            v = v >> 16;
            ushort high = (ushort)(v & 0x0000FFFF);
            return new Tuple<ushort, ushort>(high, low);
        }

        public static bool WriteFile(byte[] data, string name)
        {
            if (data == null)
                return true;
            try
            {
                using (FileStream fs = new FileStream(name, FileMode.Create))
                {
                    fs.Write(data, 0, data.Length);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }

        }

        public static void SerializeAndSave(Object o, string path)
        {
            using(FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter binary = new BinaryFormatter();
                binary.Serialize(fs, o);
            }
        }
    }
}
