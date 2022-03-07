using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ZstdNet;


namespace AquaArc
{
    class AquaArc
    {

        public static uint AquaSignature = 0x41555141;

        private string arcName;

        private string savePath;

        private Encoding enc;

        public AquaArc(string arcname)
        {
            this.arcName = arcname;
            enc = Encoding.GetEncoding(932);
        }

        public AquaArcInfoStruct ReadFileInfos()
        {
            AquaArcInfoStruct arcInfo = new AquaArcInfoStruct();
            byte[] header2Bytes = null;
            uint header1Size = 0x18;
            using (FileStream fs = new FileStream(arcName, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    arcInfo.header.ReadAquaHeader(br);
                    if (arcInfo.header.signature != AquaSignature)  //"AQUA"
                    {
                        Console.WriteLine("非法的AQUA文件: " + arcName);
                        return null;
                    }
                    uint leftSize = arcInfo.header.totalInfoSize - header1Size;
                    header2Bytes = new byte[leftSize];
                    fs.Read(header2Bytes, 0, header2Bytes.Length);
                }
            }
            if (header2Bytes == null)
            {
                Console.WriteLine("读取文件头出错: " + arcName);
                return null;
            }
            if ((arcInfo.header.encFlag & 1) != 0)
            {
                Encryption.Decrypt(header2Bytes, 65);
            }
            using (MemoryStream ms = new MemoryStream(header2Bytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    for (int i = 0; i < arcInfo.header.fileCount; i++)
                    {
                        AquaFileInfoItem item = new AquaFileInfoItem();
                        item.ReadAquaFileInfoItem(br);
                        long curOffset = ms.Position;
                        ms.Seek(item.nameOffset - header1Size, SeekOrigin.Begin);
                        string filename = Helper.ReadCString(br, enc);
                        item.fileName = filename;
                        arcInfo.fileInfos.Add(item);
                        ms.Seek(curOffset, SeekOrigin.Begin);
                    }
                }
            }
            return arcInfo;
        }


        public byte[] UnpackOneFile(BinaryReader br, AquaFileInfoItem item)
        {
            br.BaseStream.Seek(item.dataOffset, SeekOrigin.Begin);
            byte[] orgBytes = br.ReadBytes((int)item.packSize);
            if ((item.flag & 2) != 0)
            {
                Encryption.Decrypt(orgBytes, (byte)(item.nameHash & 0xFF));
            }
            if (item.packSize == item.unpackSize)
            {
                return orgBytes;
            }
            byte[] unpack = Helper.zstdUncompress(orgBytes);
            return unpack;
        }

        public void Unpack(string savepath = null)
        {
            this.savePath = savepath;
            if (string.IsNullOrWhiteSpace(savepath))
            {
                string dir = Path.GetDirectoryName(this.arcName);
                string pureName = Path.GetFileNameWithoutExtension(this.arcName);
                this.savePath = Path.Combine(dir, pureName + "_unpack");
            }
            if (!Directory.Exists(this.savePath))
            {
                Directory.CreateDirectory(this.savePath);
            }
            AquaArcInfoStruct infos = ReadFileInfos();
            if (infos == null)
                return;
            using (FileStream fs = new FileStream(arcName, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    for (int i = 0; i < infos.fileInfos.Count; i++)
                    {
                        byte[] data = UnpackOneFile(br, infos.fileInfos[i]);
                        string saveName = Path.Combine(savePath, infos.fileInfos[i].fileName);
                        string dir = Path.GetDirectoryName(saveName);
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        Console.WriteLine(saveName);
                        using (FileStream outfs = new FileStream(saveName, FileMode.Create, FileAccess.Write))
                        {
                            outfs.Write(data, 0, data.Length);
                        }
                        Console.WriteLine("unpack " + infos.fileInfos[i].fileName + " success!");
                    }
                }
            }
        }

        public byte[] PackOneFile(AquaFileInfoItem item, string filepath, out uint newUnpackSize)
        {
            newUnpackSize = 0;
            string filename = Path.Combine(filepath, item.fileName);
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("未找到封包所需文件:" + item.fileName);
            }
            byte[] data = null;
            using(FileStream fs = new FileStream(filename, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
            }
            newUnpackSize = (uint)data.Length;
            byte[] compress_data = null;
            //先进行压缩
            if(item.unpackSize == item.packSize)
            {
                compress_data = data;
            }
            else
            {
                compress_data = Helper.zstdCompress(data);
            }
            //再进行加密
            if((item.flag & 2) != 0)
            {
                Encryption.Encrypt(compress_data, (byte)(item.nameHash & 0xFF));
            }
            return compress_data;
        }

        /// <summary>
        /// 封包AQUA格式,需要原封包文件,只进行文件内容替换
        /// </summary>
        /// <param name="filepath">待封包的文件路径</param>
        /// <param name="savename">新封包保存文件名</param>
        public void Pack(string filepath, string savename=null)
        {
            if(!Directory.Exists(filepath))
            {
                throw new ArgumentException("需要提供待封包文件路径");
            }
            if(string.IsNullOrWhiteSpace(savename))
            {
                string dir = Path.GetDirectoryName(this.arcName);
                string pureName = Path.GetFileNameWithoutExtension(this.arcName);
                string ext = Path.GetExtension(this.arcName);
                this.savePath = Path.Combine(dir, pureName + "_newpack" + ext);
            }
            AquaArcInfoStruct infos = ReadFileInfos();
            if(infos == null)
            {
                throw new Exception("读取原封包失败");
            }
            byte[] orgHeaderBytes = null;
            using(FileStream fs = new FileStream(this.arcName, FileMode.Open))
            {
                orgHeaderBytes = new byte[infos.header.totalInfoSize];
                fs.Read(orgHeaderBytes, 0, orgHeaderBytes.Length);
            }
            using(FileStream fs = new FileStream(this.savePath, FileMode.Create))
            {
                using(BinaryWriter bw = new BinaryWriter(fs))
                {
                    //先写入原封包文件头
                    bw.Write(orgHeaderBytes);
                    for(int i=0; i<infos.fileInfos.Count; i++)
                    {
                        byte[] data = PackOneFile(infos.fileInfos[i], filepath, out uint newUnpackSize);
                        long data_offset = fs.Position;
                        bw.Write(data);
                        //修改属性
                        infos.fileInfos[i].unpackSize = newUnpackSize;
                        infos.fileInfos[i].packSize = (uint)data.Length;
                        infos.fileInfos[i].dataOffset = data_offset;
                        Console.WriteLine("pack: " + infos.fileInfos[i].fileName + " success!");
                    }

                    //写封包头部信息
                    fs.Seek(0, SeekOrigin.Begin);
                    infos.header.WriteAquaHeader(bw);
                    //header2可能加密,先写入另一个内存流
                    byte[] header2Bytes = null;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using(BinaryWriter msbw = new BinaryWriter(ms))
                        {
                            for(int i=0; i<infos.fileInfos.Count; i++)
                            {
                                infos.fileInfos[i].WriteAquaFileInfoItem(msbw);
                            }
                            ms.Seek(0, SeekOrigin.Begin);
                            header2Bytes = ms.ToArray();
                        }
                    }
                    if ((infos.header.encFlag & 1) != 0)
                    {
                        Encryption.Encrypt(header2Bytes, 65);
                    }  
                    //更新文件头
                    bw.Write(header2Bytes);
                    Console.WriteLine("pack headers success!");
                }
            }
        }
        public static bool IsAquaArc(string arc)
        {
            bool rtn = false;
            try
            {
                using (FileStream fs = new FileStream(arc, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        uint sig = br.ReadUInt32();
                        if (sig == AquaSignature)
                        {
                            rtn = true;
                        }
                    }
                }
            }
            catch
            {
                rtn = false;
            }
            return rtn;
        }
    }
}
