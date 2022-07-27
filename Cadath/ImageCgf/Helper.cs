using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO;

namespace ImageCgf
{
    static class Helper
    {
        public static byte[] ZlibCompress(byte[] data)
        {
            var ms = new MemoryStream();
            var ds = new DeflaterOutputStream(ms);
            ds.Write(data, 0, data.Length);
            ds.Close();
            return ms.ToArray();
        }

        public static uint RotR(uint v, int count)
        {
            count &= 0x1F;
            return v >> count | v << (32 - count);
        }
        public static uint RotL(uint v, int count)
        {
            count &= 0x1F;
            return v << count | v >> (32 - count);
        }
    }
}
