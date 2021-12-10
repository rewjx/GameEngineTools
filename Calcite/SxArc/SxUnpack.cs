using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ZstdNet;

namespace SxArc
{
    class SxUnpack
    {
        private string indexFile;

        private string storageFile;

        private string savePath;

        private IndexEncryptKey enc;

        private FileEncryptKey fileEnc;

        /// <summary>
        /// 记录文件树中包含的文件(不确定是否有文件不被文件树包含)
        /// </summary>
        private HashSet<int> fileTreeIds;

        /// <summary>
        /// 记录各文件名对应的flag
        /// </summary>
        private Dictionary<string, uint> fileFlags;


        public SxUnpack(string indexFile, string storageFile, string savepath)
        {
            this.indexFile = indexFile;
            this.storageFile = storageFile;
            this.savePath = savepath;
        }

        public void SetEncKeys(IndexEncryptKey k, FileEncryptKey fk)
        {
            this.enc = k;
            this.fileEnc = fk;
        }

        public byte[] UnpackSXIndexFile()
        {
            if (!File.Exists(indexFile))
                return null;
            MemoryStream decStream = null;
            using (FileStream fs = new FileStream(indexFile, FileMode.Open))
            {
                using(BinaryReader br = new BinaryReader(fs))
                {
                    uint sig = br.ReadUInt32();
                    if(sig != 0x58585353)
                    {
                        throw new InvalidDataException("无效的sx索引文件");
                    }
                    uint cyper = br.ReadUInt32();
                    if(cyper != 0x4C464544)
                    {
                        throw new InvalidDataException("无效的sx索引文件");
                    }
                    int rk = br.ReadInt32();
                    int k = Helper.IntEndianTran(rk);
                    int highK = k < 0 ? -1 : 0;
                    int leftLen = (int)fs.Length - 16;
                    long addv = k + (long)leftLen;
                    int highv = (int)(addv >> 32);
                    int lowv = (int)(addv & 0xFFFFFFFF);
                    int key1 = k ^ (enc.mulKey * lowv - enc.minusKey) ^ enc.xorKey1;
                    ulong mulv1 = (uint)enc.mulKey * (ulong)(uint)highv;
                    int ckhigh = (int)(mulv1 & 0xFFFFFFFF);
                    ulong mulv2 = (uint)enc.mulKey * (ulong)(uint)lowv;
                    ckhigh += (int)(mulv2 >> 32);
                    int cklow = (int)(mulv2 & 0xFFFFFFFF);
                    long ck = (uint)ckhigh;
                    ck <<= 32;
                    ck = ck | (uint)cklow;
                    ck -= enc.minusKey;
                    int key2 = highK ^ (int)(ck >> 32) ^ enc.xorKey2;
                    fs.Seek(16, SeekOrigin.Begin);
                    byte[] encdata = br.ReadBytes(leftLen);
                    MemoryStream orgs = new MemoryStream(encdata);
                    Encryption crypt = new Encryption();
                    decStream = crypt.DecryptData(orgs, (uint)key1, (uint)key2);
                    orgs.Close();
                }
            }
            if (decStream == null)
                return null;
            using(BinaryReader br = new BinaryReader(decStream))
            {
                decStream.Seek(0, SeekOrigin.Begin);
                uint unpackSize = (uint)Helper.IntEndianTran((int)br.ReadUInt32());
                byte[] compressData = br.ReadBytes((int)(decStream.Length - decStream.Position));
                using (MemoryStream cs = new MemoryStream(compressData))
                {
                    using (ZstdNet.DecompressionStream ds = new DecompressionStream(cs))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ds.CopyTo(ms);
                            ms.Seek(0, SeekOrigin.Begin);
                            byte[] unpackData = new byte[ms.Length];
                            ms.Read(unpackData, 0, (int)ms.Length);
                            return unpackData;
                        }
                    }
                }
            }
        }

        public void WriteLogFiles(SxStruct sx, string path)
        {
            string headpath = Path.Combine(path, "header.bin");
            if (!Helper.WriteFile(sx.unkHeader1, headpath))
            {
                Console.WriteLine("write file failed: " + headpath);
            }
            string unk1path = Path.Combine(path, "unk1.bin");
            if (!Helper.WriteFile(sx.unkItems1, unk1path))
            {
                Console.WriteLine("write file failed: " + unk1path);
            }
            string unk2path = Path.Combine(path, "unk2.bin");
            if (!Helper.WriteFile(sx.unkItems2, unk2path))
            {
                Console.WriteLine("write file failed: " + unk2path);
            }
        }

        public void UnpackAndSaveFile(byte[] data, string path, EntryInfo info)
        {
            //需要解密
            if ((info.flag & 0x10) == 0)
            {
                uint modOffset = (uint)(info.offset >> 4);
                uint key1 = modOffset ^ (info.fileSize << fileEnc.shiftNum1) ^ fileEnc.xorKey1;
                uint key2 = (info.fileSize >> fileEnc.shiftNum2) ^ fileEnc.xorKey2;
                MemoryStream ms = new MemoryStream(data);
                Encryption crypt = new Encryption();
                MemoryStream decs = crypt.DecryptData(ms, key1, key2);
                ms.Close();
                //解密前后数据长度不会改变，直接使用原buffer
                decs.Seek(0, SeekOrigin.Begin);
                decs.Read(data, 0, (int)decs.Length);
                decs.Close();
            }
            //需要解压
            if((info.flag & 3) != 0)
            {
                int unpackLen = 0;
                byte[] unpackData = null;
                using(MemoryStream ms = new MemoryStream(data))
                {
                    using(BinaryReader br = new BinaryReader(ms))
                    {
                        unpackLen = Helper.IntEndianTran(br.ReadInt32());
                    }
                }
                using(MemoryStream ms = new MemoryStream(data, 4, data.Length-4))
                {
                    using(ZstdNet.DecompressionStream ds = new DecompressionStream(ms))
                    {
                        using(MemoryStream os = new MemoryStream())
                        {
                            ds.CopyTo(os);
                            os.Seek(0, SeekOrigin.Begin);
                            unpackData = new byte[(int)os.Length];
                            os.Read(unpackData, 0, unpackData.Length);
                        }
                    }
                }
                if (unpackData == null || unpackLen != unpackData.Length)
                {
                    Console.WriteLine("Erros may have occurred while decompressing the data");
                }
                data = unpackData;
            }
            Helper.WriteFile(data, path);

        }

        public void ExportFileTree(SxStruct sx, FileTree node, BinaryReader storageReader, string curPath)
        {
            string newPath = curPath;
            string name = sx.nameTables[node.nameTableIdx];
            //是有效的目录
            if (!string.IsNullOrEmpty(name) && node.storageIdx < 0)
            {
                newPath = Path.Combine(curPath, name);
                if(!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
            }
            //保存文件
            if(node.storageIdx >= 0)
            {
                EntryInfo info = sx.entrys[node.storageIdx];
                storageReader.BaseStream.Seek(info.offset, SeekOrigin.Begin);
                byte[] data = storageReader.ReadBytes((int)info.fileSize);
                string saveName = Path.Combine(curPath, name);
                fileTreeIds.Add(node.storageIdx);
                uint un_flag = Helper.Combine2Short((ushort)info.unk, (ushort)info.flag);
                string difPath = saveName.Replace(savePath, "");
                fileFlags.Add(difPath, un_flag);
                UnpackAndSaveFile(data, saveName, info);
            }
            if(node.sonNodes != null)
            {
                for(int i=0; i<node.sonNodes.Count; i++)
                {
                    ExportFileTree(sx, node.sonNodes[i], storageReader, newPath);
                }
            }
        }

        public void ExportFiles(SxStruct sx)
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            string arcPath = Path.Combine(savePath, "arcFiles");
            if(!Directory.Exists(arcPath))
            {
                Directory.CreateDirectory(arcPath);
            }
            fileTreeIds = new HashSet<int>();
            fileFlags = new Dictionary<string, uint>();
            using(FileStream fs = new FileStream(storageFile, FileMode.Open))
            {
                using(BinaryReader br = new BinaryReader(fs))
                {
                    ExportFileTree(sx, sx.root, br, arcPath);

                    string unUsePath = Path.Combine(savePath, "unUseFiles");
                    for (int i = 0; i < sx.entrys.Count; i++)
                    {
                        //出现在封包内但是没被文件树索引(有这样的文件吗？不确定)
                        if (!fileTreeIds.Contains(i))
                        {
                            if (!Directory.Exists(unUsePath))
                            {
                                Directory.CreateDirectory(unUsePath);
                            }
                            string pureName = "unuse_" + string.Format("{0:D5}", i);
                            string saveName = Path.Combine(unUsePath, pureName);
                            EntryInfo info = sx.entrys[i];
                            fs.Seek(info.offset, SeekOrigin.Begin);
                            byte[] data = br.ReadBytes((int)info.fileSize);
                            UnpackAndSaveFile(data, saveName, info);
                            fileFlags.Add(pureName, Helper.Combine2Short((ushort)info.unk, (ushort)info.flag));
                        }
                    }
                }
            }
        }



        public void Unpack(int codepage=932)
        {
            if(this.enc == null)
            {
                enc = new IndexEncryptKey();
            }
            if(this.fileEnc == null)
            {
                this.fileEnc = new FileEncryptKey();
            }
            byte[] data = UnpackSXIndexFile();
            if(data == null)
            {
                Console.WriteLine("Faild to decrypt and unpack index file");
                return;
            }
            SxStruct s = new SxStruct();
            Encoding encoding = Encoding.GetEncoding(codepage);
            using(MemoryStream ms = new MemoryStream(data))
            {
                using(BinaryReader br = new BinaryReader(ms))
                {
                    s.ReadSxStruct(br, encoding);
                }
            }
            ExportFiles(s);
            //然后保存未能解析的文件
            string logPath = Path.Combine(savePath, "logFiles");
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            WriteLogFiles(s, logPath);
            //保存flag文件
            string flagName = Path.Combine(logPath, "FileFlag.data");
            Helper.SerializeAndSave(fileFlags, flagName);
        }
    }
}
