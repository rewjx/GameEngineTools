#define _CRT_SECURE_NO_WARNINGS
#include"CP3Unpack.h"

CP3Header ReadCP3Header(FILE* file)
{
	fseek(file, 0, SEEK_END);
	long size = ftell(file);
	if (size < CP3_Header_LEN)
	{
		throw "Not a valid CP3 File";
	}
	CP3Header header;
	memset(&header, 0, sizeof(CP3Header));
	int val;
	fseek(file, 0, SEEK_SET);
	fread(header.magic, 1, 8, file);
	if (strcmp(header.magic, CP3Magic) != 0)
	{
		throw "Not a valid CP3 File";
	}
	fread(header.unk1, 1, 0x100, file);
	fread(&val, 4, 1, file);
	header.unk2 = BigEndian2Little(val);
	fread(&val, 4, 1, file);
	header.unk3 = BigEndian2Little(val);
	fread(&val, 4, 1, file);
	header.unk4 = BigEndian2Little(val);
	fread(&val, 4, 1, file);
	header.FileHash = BigEndian2Little(val);
	fread(header.key, 1, 0x100, file);
	return header;
}


std::vector<CP3LIUnpackData> ReadLIHeadersFromCP3(FILE* file)
{
	fseek(file, 0, SEEK_END);
	long cp3Size = ftell(file);
	long curOff = CP3_Header_LEN;
	fseek(file, curOff, SEEK_SET);
	std::vector<CP3LIUnpackData> files;
	unsigned int val;
	while (curOff < cp3Size)
	{
		CP3LIUnpackData info;
		memset(&info, 0, sizeof(CP3LIUnpackData));
		info.LIheaderOffset = ftell(file);
		fread(info.header.magic, 1, 2, file);
		if (info.header.magic[0] != 'L' || info.header.magic[1] != 'I')
		{
			break;
		}
		fread(&val, 4, 1, file);
		info.header.FileHash = BigEndian2Little(val);
		fread(&info.header.FileNameLen, 1, 1, file);
		fread(info.header.FileName, 1, info.header.FileNameLen, file);
		fread(&val, 4, 1, file);
		info.header.Flag = BigEndian2Little(val);
		fread(&val, 4, 1, file);
		info.header.unk1 = BigEndian2Little(val);
		fread(&val, 4, 1, file);
		info.header.FileSize = BigEndian2Little(val);
		fread(&val, 4, 1, file);
		info.header.unk2 = BigEndian2Little(val);
		fread(&val, 4, 1, file);
		info.header.unk3 = BigEndian2Little(val);
		info.FileContentOffset = ftell(file);
		files.push_back(info);
		// seek to next file
		fseek(file, info.header.FileSize, SEEK_CUR);
		curOff = ftell(file);
	}
	return files;
}



void Decrypt_CP3File_1(CP3Header cp3Header, CP3LIUnpackData liData, WCHAR* savePath, FILE* file, int nameCodePage)
{
	WCHAR filePath[512] = { 0 };
	wcscpy(filePath, savePath);
	wcscat(filePath, L"\\");
	WCHAR* filename = Convert2WideChar(liData.header.FileName, nameCodePage);
	wcscat(filePath, filename);
	FILE* writeFile = _wfopen(filePath, L"wb");
	fseek(file, liData.FileContentOffset, SEEK_SET);
	unsigned int keyIndex = 0;
	unsigned int readSize = 0;
	char data[0x800];
	while (readSize < liData.header.FileSize)
	{
		int curBytes = min(0x800, liData.header.FileSize - readSize);
		int byte = fread(data, 1, curBytes, file);
		if (byte != curBytes)
		{
			throw "Not a valid CP3 File";
		}
		if ((liData.header.Flag & 0x80000) != 0)
		{
			xor_decrypt_data(data, curBytes, cp3Header.key, &keyIndex);
		}
		fwrite(data, 1, curBytes, writeFile);
		readSize += curBytes;
	}
	fclose(writeFile);
	delete[] filename;
}

bool ExtractOneCP3File(CP3Header header, CP3LIUnpackData data, WCHAR* savePath, FILE* file, int nameCP)
{
	if (data.header.Flag < 0)
	{
		std::cout << "Not Implemented !" << std::endl;
	}
	else {

		Decrypt_CP3File_1(header, data, savePath, file, nameCP);
	}
	return true;
}



void ExtractCP3Main(char* filepath, char* savepath, int nameCP)
{
	WCHAR* wideSavePath = Convert2WideChar(savepath, GetACP());
	FILE* cp3 = fopen(filepath, "rb");
	CP3Header cp3Header = ReadCP3Header(cp3);
	std::vector<CP3LIUnpackData> files = ReadLIHeadersFromCP3(cp3);
	std::cout << "total file num in cp3: " << files.size() << std::endl;
	SetConsoleOutputCP(932);
	for (int i = 0; i < files.size(); i++)
	{
		ExtractOneCP3File(cp3Header, files[i], wideSavePath, cp3, nameCP);
	}
	fclose(cp3);
	delete[] wideSavePath;
}
