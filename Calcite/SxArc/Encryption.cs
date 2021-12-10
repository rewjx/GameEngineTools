using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SxArc
{
    class IndexEncryptKey
    {
        public int mulKey;

        public int minusKey;

        public int xorKey1;

        public int xorKey2;

        public IndexEncryptKey()
        {
            SetDefaultKey();
        }

        public void SetDefaultKey()
        {
            mulKey = 961;
            minusKey = 124789;
            xorKey1 = 0x2E76034B;
            xorKey2 = 0x2E6;
        }
    }

    class FileEncryptKey
    {
        public uint xorKey1;

        public uint xorKey2;

        public int shiftNum1;

        public int shiftNum2;

        public FileEncryptKey()
        {
            SetDefaultKey();
        }

        public void SetDefaultKey()
        {
            xorKey1 = 0x2E76034B;
            xorKey2 = 0x2E6;
            shiftNum1 = 16;
            shiftNum2 = 16;
        }
    }


    class Encryption
    {
        public uint xorKey1;

        public uint xorKey2;

        public uint xorKey3;

        public uint xorKey4;

        public uint xorKey5;

        public int shiftNumber1;

        public int shiftNumber2;

        public int shiftNumber3;

        public int shiftNumber4;

        public int shiftNumber5;

        public Encryption()
        {
            SetDefaultKey();
        }

        public void SetDefaultKey()
        {
            xorKey1 = 0x75BCD15;
            xorKey2 = 0x549139A;
            xorKey3 = 0x159A55E5;
            xorKey4 = 0x8E415C26;
            xorKey5 = 0x4D9D5BB8;
            shiftNumber1 = 11;
            shiftNumber2 = 8;
            shiftNumber3 = 19;
            shiftNumber4 = 4;
            shiftNumber5 = 12;
        }

        public MemoryStream  DecryptData(MemoryStream rs, uint key1, uint key2)
        {
            if (rs.Length <= 0)
                return null;
            uint tk1 = key2 ^ xorKey1;
            uint tk2 = tk1 << shiftNumber1;
            uint tk3 = tk1 ^ tk2;
            uint uk1 = tk3 ^ (tk3 >> shiftNumber2) ^ xorKey2;
            uint tk4 = key1 ^ xorKey3;
            uint tk5 = tk4 << shiftNumber1;
            uint tk6 = tk4 ^ tk5;
            uint uk2 = uk1 ^ tk6 ^ ((tk6 ^ (uk1 >> shiftNumber1)) >> shiftNumber2);
            uint uk3 = uk2 ^ (uk2 >> shiftNumber3) ^ xorKey4;
            uint uk4 = uk3 ^ (uk3 >> shiftNumber3) ^ xorKey5;
            int loopCount = (int)rs.Length >> 2;
            MemoryStream rtn = new MemoryStream();
            using (BinaryReader br = new BinaryReader(rs, Encoding.ASCII, true))
            {
                using (BinaryWriter bw = new BinaryWriter(rtn, Encoding.ASCII, true))
                {
                    for (int i = 0; i < loopCount; i++)
                    {
                        uint lk1 = uk1 ^ (uk1 << shiftNumber1);
                        uint lk2 = uk4 ^ lk1 ^ ((lk1 ^ (uk4 >> shiftNumber1)) >> shiftNumber2);
                        uint lk3 = uk2 ^ (uk2 << shiftNumber1);
                        uk2 = uk4;
                        uk4 = lk2 ^ lk3 ^ ((lk3 ^ (lk2 >> shiftNumber1)) >> shiftNumber2);
                        uint v = br.ReadUInt32();
                        uint decv = v ^ (lk2 >> shiftNumber4) ^ (uk4 << shiftNumber5);
                        bw.Write(decv);
                        uk1 = uk3;
                        uk3 = lk2;
                    }
                    byte[] leftdata = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
                    bw.Write(leftdata);
                }
            }
            return rtn;
        }

    }
}
