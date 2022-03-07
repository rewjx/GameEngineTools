using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ZstdNet;

namespace AquaArc
{
    class Helper
    {
        public static string ReadCString(BinaryReader br, Encoding enc)
        {
            List<byte> strBytes = new List<byte>();
            byte b = br.ReadByte();
            while(b != 0)
            {
                strBytes.Add(b);
                b = br.ReadByte();
            }
            string str = enc.GetString(strBytes.ToArray());
            return str;
        }

        public static byte[] zstdUncompress(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                using (ZstdNet.DecompressionStream ds = new DecompressionStream(ms))
                {
                    using (MemoryStream outms = new MemoryStream())
                    {
                        ds.CopyTo(outms);
                        outms.Seek(0, SeekOrigin.Begin);
                        return outms.ToArray();
                    }
                }
            }
        }

        public static byte[] zstdCompress(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                using (MemoryStream outms = new MemoryStream())
                {
                    using (ZstdNet.CompressionStream cs = new CompressionStream(outms))
                    {
                        cs.Write(buffer, 0, buffer.Length);
                    }
                    outms.Seek(0, SeekOrigin.Begin);
                    return outms.ToArray();
                }
            }
        }
    }
}
