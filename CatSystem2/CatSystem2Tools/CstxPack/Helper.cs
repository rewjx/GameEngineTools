using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace CstxPack
{
    public class Helper
    {

        public static byte[] Uncompress(byte[] data, int pos, int count)
        {
            using (MemoryStream ms = new MemoryStream(data, pos, count))
            {
                using (MemoryStream outms = new MemoryStream())
                {
                    using (DeflateStream df = new DeflateStream(ms, CompressionMode.Decompress))
                    {
                        df.CopyTo(outms);
                    }
                    return outms.ToArray();
                }
            }
        }

        public static byte[] Compress(byte[] data, int pos, int count)
        {
            using (MemoryStream ms = new MemoryStream(data, pos, count))
            {
                using (MemoryStream outms = new MemoryStream())
                {
                    using (DeflateStream df = new DeflateStream(outms, CompressionMode.Compress))
                    {
                        ms.CopyTo(df);
                    }
                    return outms.ToArray();
                }
            }
        }

        public static byte[] BinaryPack(byte[] buf, string fileSig, bool isCompress=true)
        {
            byte[] sigByte = Encoding.ASCII.GetBytes(fileSig);
            byte[] compressByte = null;
            if (isCompress)
            {
                compressByte = Compress(buf, 0, buf.Length);
            }
            else
            {
                compressByte = buf;
            }
            using(MemoryStream ms = new MemoryStream())
            {
                using(BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(sigByte);
                    bw.Write(buf.Length);
                    bw.Write(compressByte.Length);
                    int mode = isCompress ? 1 : 0;
                    bw.Write(mode);
                    bw.Write(compressByte);
                    return ms.ToArray();
                }
            }
        }


        public static byte[] BinaryUnpack(byte[] buf)
        {
            byte[] data = null;
            using (MemoryStream ms = new MemoryStream(buf))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    uint sig = br.ReadUInt32();
                    int origLen = br.ReadInt32();
                    int compressLen = br.ReadInt32();
                    int mode = br.ReadInt32();
                    long pos = ms.Position;
                    long leftLen = ms.Length - pos - compressLen;
                    data = new byte[origLen + leftLen];
                    if (mode == 1)
                    {
                        byte[] unzip = Uncompress(buf, (int)pos, compressLen);
                        unzip.CopyTo(data, 0);
                        if (leftLen > 0)
                        {
                            Array.Copy(buf, pos + compressLen, data, origLen, leftLen);
                        }
                    }
                    else
                    {
                        Array.Copy(buf, pos, data, 0, data.Length);
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// 检查文件头标记是否是给定的标记
        /// </summary>
        /// <param name="data"></param>
        /// <param name="targetSig"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public static bool CheckFileSignature(byte[] data, string targetSig, int startPos=0)
        {
            int len = targetSig.Length;
            if (data == null || data.Length < len)
                return false;
            for(int i=0; i<len; i++)
            {
                if (data[startPos + i] != targetSig[i])
                    return false;
            }
            return true;
        }

        public static void WriteBinFile(byte[] data, string savePath)
        {
            using (FileStream fs = new FileStream(savePath, FileMode.Create))
            {
                fs.Write(data, 0, data.Length);
            }
        }

        public static byte[] ReadBinFile(string path)
        {
            byte[] data = null;
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, (int)fs.Length);
            }
            return data;
        }
        public static readonly char[] spaces = { ' ', '\t' };

        public static readonly char labelHeader = '#';

        public static readonly char labelSplitter = '.';

        public static readonly string SelectTag = "fselect";



    }
}
