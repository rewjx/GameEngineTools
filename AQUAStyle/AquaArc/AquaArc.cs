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

        public AquaArc(string arcname, string savepath = null)
        {
            this.arcName = arcname;
            this.savePath = savepath;
            if (this.savePath == null)
            {
                string dir = Path.GetDirectoryName(arcname);
                string pureName = Path.GetFileNameWithoutExtension(arcname);
                this.savePath = Path.Combine(dir, pureName + "_unpack");
            }
            enc = Encoding.GetEncoding(932);
            if (!Directory.Exists(this.savePath))
            {
                Directory.CreateDirectory(this.savePath);
            }
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

        public void Unpack()
        {
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
                        using (FileStream outfs = new FileStream(saveName, FileMode.Create))
                        {
                            outfs.Write(data, 0, data.Length);
                        }
                        Console.WriteLine("unpack " + infos.fileInfos[i].fileName + " success!");
                    }
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
