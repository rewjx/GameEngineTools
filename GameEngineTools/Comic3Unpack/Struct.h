#pragma once

#ifndef STRUCT_H
#define STRUCT_H

#define CP3_Header_LEN 0x218
#define CML_Header_LEN 0x10C
#define CML_META_ELE_LEN 0x110


#define CMLMagic "CM3LIB"
#define CP3Magic "CM3PKG"

struct CP3Header
{
	char magic[8];
	char unk1[0x100];
	unsigned int unk2;
	unsigned int unk3;
	unsigned int unk4;
	unsigned int  FileHash;
	char key[0x100];
};


struct CP3LIHeader
{
	char magic[2];					//"LI"
	unsigned int FileHash;
	unsigned char FileNameLen;
	char FileName[0x100];				//实际在文件中只有FileNameLen长
	int Flag;
	unsigned int unk1;
	unsigned int FileSize;
	unsigned int unk2;
	unsigned int unk3;
};


/// <summary>
/// For unpack file
/// </summary>
struct CP3LIUnpackData
{
	/// <summary>
	/// the offset of "LI" tag
	/// </summary>
	long LIheaderOffset;

	long FileContentOffset;

	CP3LIHeader header;

};


struct CMLHeader
{
	char magic[8];
	char unk1[0x100];
	unsigned int FileNum;
	char key[0x100];  //实际文件中无该数组，但是为了算法的兼容，此处加上该成员。
};

struct CMLLIHeader {
	char name_unk[0x100];
	unsigned int Flag1;
	unsigned int Flag2;
	unsigned int FileStartOffset;  //相对整个文件meta大小的偏移
	unsigned int FileSize;
};



#endif // !
