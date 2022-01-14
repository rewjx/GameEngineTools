using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AquaArc
{
    class ASFAArc
    {
        public static uint AsfaSignature = 0x41465341;

        private string arcName;

        private string savePath;

        private Encoding enc;

        public ASFAArc(string arcname, string savepath = null)
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

        public AsfaArcInfoStruct ReadFileInfos()
        {
            AsfaArcInfoStruct arcInfo = new AsfaArcInfoStruct();
            int header1Size = 0x28;
            int fileInfoSize = 0;
            int fileItemSize = 32;
            int nameTableSize = 0;
            byte[] fileInfoBytes = null;
            byte[] nameTableBytes = null;
            using (FileStream fs = new FileStream(arcName, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    arcInfo.header.ReadAsfaHeader(br);
                    if (arcInfo.header.signature != AsfaSignature)  //"ASFA"
                    {
                        Console.WriteLine("非法的ASFA文件: " + arcName);
                        return null;
                    }
                    fileInfoSize = (int)arcInfo.header.fileCount * fileItemSize;
                    fileInfoBytes = br.ReadBytes(fileInfoSize);
                    nameTableSize = (int)arcInfo.header.totalInfoSize - header1Size - fileInfoSize;
                    nameTableBytes = br.ReadBytes(nameTableSize);
                }
            }
            if (fileInfoBytes == null || nameTableBytes == null)
            {
                Console.WriteLine("读取文件头出错: " + arcName);
                return null;
            }

            using (MemoryStream ms = new MemoryStream(fileInfoBytes))
            {
                using (MemoryStream nms = new MemoryStream(nameTableBytes))
                {
                    using (BinaryReader fbr = new BinaryReader(ms))
                    {
                        using (BinaryReader nbr = new BinaryReader(nms))
                        {
                            for (int i = 0; i < arcInfo.header.fileCount; i++)
                            {
                                ASFAFileInfoItem item = new ASFAFileInfoItem();
                                item.ReadAsfaFileInfoItem(fbr);
                                nms.Seek(item.nameOffset, SeekOrigin.Begin);
                                string filename = Helper.ReadCString(nbr, enc);
                                item.fileName = filename;
                                arcInfo.fileInfos.Add(item);
                            }
                        }
                    }
                }
            }
            return arcInfo;
        }


        public byte[] UnpackOneFile(BinaryReader br, ASFAFileInfoItem item, uint totalHeaderSize)
        {
            br.BaseStream.Seek(item.dataOffset + totalHeaderSize, SeekOrigin.Begin);
            byte[] orgBytes = br.ReadBytes((int)item.packSize);
            if (item.packSize == item.unpackSize)
            {
                return orgBytes;
            }
            byte[] unpack = Helper.zstdUncompress(orgBytes);
            return unpack;
        }

        public void Unpack()
        {
            AsfaArcInfoStruct infos = ReadFileInfos();
            if (infos == null)
                return;
            using (FileStream fs = new FileStream(arcName, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    for (int i = 0; i < infos.fileInfos.Count; i++)
                    {
                        byte[] data = UnpackOneFile(br, infos.fileInfos[i], infos.header.totalInfoSize);
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

        public static bool IsAsfaArc(string arc)
        {
            bool rtn = false;
            try
            {
                using (FileStream fs = new FileStream(arc, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        uint sig = br.ReadUInt32();
                        if (sig == AsfaSignature)
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
