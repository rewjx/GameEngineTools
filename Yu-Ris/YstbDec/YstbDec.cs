using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace YstbDec
{
    class YstbDec
    {
        private byte[] xorKey = null;

        private string inPath;

        private string outPath;


        public YstbDec(string inpath, string outpath)
        {
            this.inPath = inpath;
            this.outPath = outpath;
        }

        public uint CalcXorKey(string keyStr, Encoding enc)
        {
            byte[] buffer = enc.GetBytes(keyStr);
            uint key = 0xFFFFFFFF;
            for(int i=0; i<buffer.Length; i++)
            {
                key = (key ^ buffer[i]) ^ (key >> 8);
            }
            key = ~key;
            ChangeKey2ByteArr(key);
            Console.WriteLine(Convert.ToString(key, 16));
            return key;
        }

        public void ChangeKey2ByteArr(uint key)
        {
            xorKey = new byte[4];
            xorKey[3] = (byte)(key & 0xFF);
            key >>= 8;
            xorKey[2] = (byte)(key & 0xFF);
            key >>= 8;
            xorKey[1] = (byte)(key & 0xFF);
            key >>= 8;
            xorKey[0] = (byte)(key & 0xFF);
        }

        public void DecryptArr(byte[] buffer)
        {
            int idx = 0;
            for(int i=0; i<buffer.Length; i++)
            {
                buffer[i] = (byte)(buffer[i] ^ xorKey[idx & 3]);
                idx += 1;
            }
        }

        public void ProcessMain()
        {
            //输入是单个文件
            if(File.Exists(inPath))
            {
                if(outPath == null)
                {
                    string pureName = Path.GetFileNameWithoutExtension(inPath);
                    string newPureName = pureName + "_dec";
                    string ext = Path.GetExtension(inPath);
                    outPath = Path.Combine(Path.GetDirectoryName(inPath), newPureName + ext);
                }
                DecryptOneYSTB(inPath, outPath);
            }
            //文件夹
            else
            {
                if(outPath == null)
                {
                    string pdir = Path.GetDirectoryName(inPath);
                    string curName = Path.GetFileName(inPath);
                    outPath = Path.Combine(pdir, curName + "_dec");
                }
                if (!Directory.Exists(outPath))
                {
                    Directory.CreateDirectory(outPath);
                }
                DirectoryInfo dir = new DirectoryInfo(inPath);
                foreach (FileInfo item in dir.GetFiles("*.ybn"))
                {
                    string saveName = Path.Combine(outPath, item.Name);
                    DecryptOneYSTB(item.FullName, saveName);
                }

            }
        }

        public void DecryptOneYSTB(string inFile, string outFile)
        {
            using (FileStream fs = new FileStream(inFile, FileMode.Open))
            {
                using (FileStream ofs = new FileStream(outFile, FileMode.Create))
                {
                    //先读取文件头
                    int headerSize = 0x20;
                    byte[] header = new byte[headerSize];
                    fs.Read(header, 0, header.Length);
                    uint funcSize, paramSize, strSize, unkSize;
                    using (MemoryStream ms = new MemoryStream(header))
                    {
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            uint sig = br.ReadUInt32();
                            if (sig != 0x42545359)  //"YSTB"
                            {
                                Console.WriteLine(inFile +  "  is invalid YSTB file");
                                return;
                            }
                            uint version = br.ReadUInt32();
                            uint funCount = br.ReadUInt32();
                            funcSize = br.ReadUInt32();
                            paramSize = br.ReadUInt32();
                            strSize = br.ReadUInt32();
                            unkSize = br.ReadUInt32();
                        }
                    }
                    //解密函数区
                    fs.Seek(headerSize, SeekOrigin.Begin);
                    byte[] funcRegion = null;
                    if (funcSize > 0)
                    {
                        funcRegion = new byte[funcSize];
                        fs.Read(funcRegion, 0, funcRegion.Length);
                        DecryptArr(funcRegion);
                    }
                    //解密参数区
                    byte[] paramRegion = null;
                    if (paramSize > 0)
                    {
                        paramRegion = new byte[paramSize];
                        fs.Read(paramRegion, 0, paramRegion.Length);
                        DecryptArr(paramRegion);
                    }
                    //解密字符串区
                    byte[] strRegion = null;
                    if (strSize > 0)
                    {
                        strRegion = new byte[strSize];
                        fs.Read(strRegion, 0, strRegion.Length);
                        DecryptArr(strRegion);
                    }
                    //解密未知区
                    byte[] unkRegion = null;
                    if (unkSize > 0)
                    {
                        unkRegion = new byte[unkSize];
                        fs.Read(unkRegion, 0, unkRegion.Length);
                        DecryptArr(unkRegion);
                    }
                    ofs.Write(header, 0, headerSize);
                    if (funcRegion != null)
                    {
                        ofs.Write(funcRegion, 0, funcRegion.Length);
                    }
                    if (paramRegion != null)
                    {
                        ofs.Write(paramRegion, 0, paramRegion.Length);
                    }
                    if (strRegion != null)
                    {
                        ofs.Write(strRegion, 0, strRegion.Length);
                    }
                    if (unkRegion != null)
                    {
                        ofs.Write(unkRegion, 0, unkRegion.Length);
                    }
                    Console.WriteLine("write " + outFile + " finish");

                }
            }
        }

    }
}
