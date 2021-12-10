using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SxArc
{
    /// <summary>
    /// 解密解压后的sx结构部分
    /// </summary>
    class SxStruct
    {
        public byte[] unkHeader1;

        public int nameCount;

        public List<string> nameTables;

        public int entryCount;

        public List<EntryInfo> entrys;

        public ushort unkCount1;

        public int unkSize1 = 40;

        public byte[] unkItems1;

        public ushort unkCount2;

        public int unkSize2 = 24;

        public byte[] unkItems2;

        public FileTree root;

        public void ReadSxStruct(BinaryReader reader, Encoding encoding)
        {
            unkHeader1 = reader.ReadBytes(8);
            nameCount = Helper.IntEndianTran(reader.ReadInt32());
            nameTables = new List<string>();
 
            for(int i=0; i<nameCount; i++)
            {
                byte len = reader.ReadByte();
                if(len == 0)
                {
                    nameTables.Add("");
                    continue;
                }
                byte[] data = reader.ReadBytes(len);
                string str = encoding.GetString(data);
                nameTables.Add(str);
            }
            entryCount = Helper.IntEndianTran(reader.ReadInt32());
            entrys = new List<EntryInfo>();
            for(int i=0; i<entryCount; i++)
            {
                EntryInfo item = new EntryInfo();
                item.ReadEntryInfo(reader);
                entrys.Add(item);
            }
            unkCount1 = (ushort)Helper.ShortEndianTran(reader.ReadInt16());
            unkItems1 = reader.ReadBytes(unkCount1 * unkSize1);
            unkCount2 = (ushort)Helper.ShortEndianTran(reader.ReadInt16());
            if(unkCount2 == 0)
            {
                unkItems2 = null;
            }
            else
            {
                unkItems2 = reader.ReadBytes(unkCount2 * unkSize2);
            }
            root = FileTree.ReadFileTree(reader);

        }

    }

    class EntryInfo
    {
        public short unk;

        public short flag;

        /// <summary>
        /// 存储实际偏移，文件中的偏移为实际偏移右移4位的结果
        /// </summary>
        public long offset;

        public uint fileSize;

        public void ReadEntryInfo(BinaryReader reader)
        {
            short v = reader.ReadInt16();
            unk = Helper.ShortEndianTran(v);
            v = reader.ReadInt16();
            flag = Helper.ShortEndianTran(v);
            int iv = reader.ReadInt32();
            offset = ((long)(uint)Helper.IntEndianTran(iv)) << 4;
            iv = reader.ReadInt32();
            fileSize = (uint)(Helper.IntEndianTran(iv));
        }
    }



    class FileTree
    {
        public List<FileTree> sonNodes;

        public int nameTableIdx;

        public int storageIdx;

        public static FileTree ReadFileTree(BinaryReader reader)
        {
            FileTree root = new FileTree();
            ushort nodesCount = (ushort)Helper.ShortEndianTran(reader.ReadInt16());
            root.nameTableIdx = Helper.IntEndianTran(reader.ReadInt32());
            root.storageIdx = Helper.IntEndianTran(reader.ReadInt32());
            if (nodesCount > 0)
            {
                root.sonNodes = new List<FileTree>(nodesCount);
            }
            else
            {
                root.sonNodes = null;
            }
            for(int i=0; i<nodesCount; i++)
            {
                FileTree node = ReadFileTree(reader);
                root.sonNodes.Add(node);
            }
            return root;

        }
    }
}
