using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AquaArc
{
    class Encryption
    {
        public static byte ROR(byte data, byte bits)
        {
            byte v1 = (byte)(data >> bits);
            byte v2 = (byte)(data << (8 - bits));
            return (byte)(v1 | v2);
        }

        public static void Decrypt(byte[] buffer, byte ror)
        {
            ror = (byte)(ror & 7);
            for (int i = 0; i < buffer.Length; i++)
            {
                byte data = buffer[i];
                byte v1 = (byte)(data >> 4);
                byte v2 = (byte)(v1 ^ data);
                byte decdata = ROR((byte)(v2 ^ (byte)(16 * v2)), ror);
                buffer[i] = decdata;
            }
        }

        /// <summary>
        /// Decrypt的逆运算，用于封包
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="ror"></param>
        public static void Encrypt(byte[] buffer, byte ror)
        {
            ror = (byte)(ror & 7);
            //以byte位单位的循环右移x位的逆运算位循环右移8-x位
            ror = (byte)(8 - ror);
            for(int i=0; i<buffer.Length; i++)
            {
                byte v3 = ROR(buffer[i], ror);
                byte v2 = (byte)(v3 ^ (byte)(v3 << 4));
                byte v1 = (byte)(v2 >> 4);
                byte encdata = (byte)(v2 ^ v1);
                buffer[i] = encdata;
            }
        }
    }
}
