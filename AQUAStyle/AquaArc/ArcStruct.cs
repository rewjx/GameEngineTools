using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AquaArc
{
    //0x18字节
    public class AquaHeader
    {
        public uint signature;

        public UInt16 unk1;

        public UInt16 encFlag;

        public ushort fileCount;

        public ushort unk2;

        public uint unk3;

        public uint totalInfoSize;

        public uint unk4;

        public void ReadAquaHeader(BinaryReader br)
        {
            signature = br.ReadUInt32();
            unk1 = br.ReadUInt16();
            encFlag = br.ReadUInt16();
            fileCount = br.ReadUInt16();
            unk2 = br.ReadUInt16();
            unk3 = br.ReadUInt32();
            totalInfoSize = br.ReadUInt32();
            unk4 = br.ReadUInt32();
        }

        public void WriteAquaHeader(BinaryWriter bw)
        {
            bw.Write(signature);
            bw.Write(unk1);
            bw.Write(encFlag);
            bw.Write(fileCount);
            bw.Write(unk2);
            bw.Write(unk3);
            bw.Write(totalInfoSize);
            bw.Write(unk4);
        }


    }


    //一个32字节

    public class AquaFileInfoItem
    {
        public uint packSize;

        public uint unpackSize;

        public uint nameHash;

        public uint flag;

        public uint unk1; //namelength?

        public uint nameOffset;

        public long dataOffset;

        //添加额外项
        public string fileName = null;

        public void ReadAquaFileInfoItem(BinaryReader br)
        {
            packSize = br.ReadUInt32();
            unpackSize = br.ReadUInt32();
            nameHash = br.ReadUInt32();
            flag = br.ReadUInt32();
            unk1 = br.ReadUInt32();
            nameOffset = br.ReadUInt32();
            dataOffset = br.ReadInt64();
        }

        public void WriteAquaFileInfoItem(BinaryWriter bw)
        {
            bw.Write(packSize);
            bw.Write(unpackSize);
            bw.Write(nameHash);
            bw.Write(flag);
            bw.Write(unk1);
            bw.Write(nameOffset);
            bw.Write(dataOffset);
        }
    }

    public class AquaArcInfoStruct
    {
        public AquaHeader header;

        public List<AquaFileInfoItem> fileInfos;

        public AquaArcInfoStruct()
        {
            header = new AquaHeader();
            fileInfos = new List<AquaFileInfoItem>();
        }

    }

    //0x28字节
    public class ASFAHeader
    {
        public uint signature;

        public uint unk1;

        public ushort fileCount;

        public ushort unk2;

        public uint unk3;

        public uint totalInfoSize;

        public byte[] otherBytes;

        public void ReadAsfaHeader(BinaryReader br)
        {
            signature = br.ReadUInt32();
            unk1 = br.ReadUInt32();
            fileCount = br.ReadUInt16();
            unk2 = br.ReadUInt16();
            unk3 = br.ReadUInt32();
            totalInfoSize = br.ReadUInt32();
            otherBytes = br.ReadBytes(20);
        }
    }

    public class ASFAFileInfoItem
    {
        public uint unk1;

        public uint nameOffset;

        public long dataOffset; //相对数据区的偏移，而不是整个文件

        public uint packSize;

        public uint unk3;

        public uint unpackSize;

        public uint unk4;

        //额外增加项，文件中不存在
        public string fileName = null;

        public void ReadAsfaFileInfoItem(BinaryReader br)
        {
            unk1 = br.ReadUInt32();
            nameOffset = br.ReadUInt32();
            dataOffset = br.ReadInt64();
            packSize = br.ReadUInt32();
            unk3 = br.ReadUInt32();
            unpackSize = br.ReadUInt32();
            unk4 = br.ReadUInt32();
        }
    }


    public class AsfaArcInfoStruct
    {
        public ASFAHeader header;

        public List<ASFAFileInfoItem> fileInfos;

        public AsfaArcInfoStruct()
        {
            header = new ASFAHeader();
            fileInfos = new List<ASFAFileInfoItem>();
        }

    }
}
