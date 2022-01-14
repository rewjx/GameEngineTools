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
    }
}
