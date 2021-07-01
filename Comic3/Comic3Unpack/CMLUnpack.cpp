#define _CRT_SECURE_NO_WARNINGS
#include"CMLUnpack.h"


CMLHeader ReadCMLHeader(FILE* file)
{
	fseek(file, 0, SEEK_END);
	long fileSize = ftell(file);
	if (fileSize < CML_Header_LEN)
	{
		throw "Not a valid CML File";
	}
	CMLHeader header;
	memset(&header, 0, sizeof(CMLHeader));
	fseek(file, 0, SEEK_SET);
	fread(header.magic, 1, 8, file);
	if (strcmp(header.magic, CMLMagic) != 0)
	{
		throw "Not a valid CML File";
	}
	fread(header.unk1, 1, 0x100, file);
	fread(&header.FileNum, 1, 4, file);
	return header;
}

std::vector<CMLLIHeader> ReadFileHeaderFromCML(FILE* file, CMLHeader header)
{
	fseek(file, CML_Header_LEN, SEEK_SET);
	std::vector<CMLLIHeader> files;
	for (int i = 0; i < header.FileNum; i++)
	{
		CMLLIHeader fileHeader;
		memset(&fileHeader, 0, sizeof(CMLLIHeader));
		fread(fileHeader.name_unk, 1, 0x100, file);
		fread(&fileHeader.Flag1, 4, 1, file);
		fread(&fileHeader.Flag2, 4, 1, file);
		fread(&fileHeader.FileStartOffset, 4, 1, file);
		fread(&fileHeader.FileSize, 4, 1, file);
		files.push_back(fileHeader);
	}
	return files;
}






void Decrypt_CMLFile_1(CMLHeader header, CMLLIHeader fileHeader, WCHAR* savePath, FILE* file, int nameCodePage)
{
	WCHAR filePath[512] = { 0 };
	wcscpy(filePath, savePath);
	wcscat(filePath, L"\\");
	char cFileName[256] = { 0 };
	strcpy(cFileName, fileHeader.name_unk);
	WCHAR* filename = Convert2WideChar(cFileName, nameCodePage);
	wcscat(filePath, filename);
	FILE* writeFile = _wfopen(filePath, L"wb");
	long offset = CML_Header_LEN + header.FileNum * CML_META_ELE_LEN + fileHeader.FileStartOffset;
	fseek(file, offset, SEEK_SET);
	unsigned int keyIndex = 0;
	unsigned int readSize = 0;
	char data[0x800];
	while (readSize < fileHeader.FileSize)
	{
		int curBytes = min(0x800, fileHeader.FileSize - readSize);
		int byte = fread(data, 1, curBytes, file);
		if (byte != curBytes)
		{
			throw "Not a valid CML File";
		}
		fwrite(data, 1, curBytes, writeFile);
		readSize += curBytes;
	}
	fclose(writeFile);
	delete[] filename;
}

bool ExtractOneCMLFile(CMLHeader header, CMLLIHeader fileHeader, WCHAR* savepath, FILE* file, int nameCP)
{
	if ((int)fileHeader.Flag1 < 0)
	{
		std::cout << "Not Implemented !" << std::endl;
	}
	else {
		Decrypt_CMLFile_1(header, fileHeader, savepath, file, nameCP);
	}
	return true;
}



void ExtractCMLMain(char* filepath, char* savepath, int nameCP)
{
	WCHAR* wideSavePath = Convert2WideChar(savepath, GetACP());
	FILE* cml = fopen(filepath, "rb");
	CMLHeader cmlheader = ReadCMLHeader(cml);
	std::vector<CMLLIHeader> files = ReadFileHeaderFromCML(cml, cmlheader);
	std::cout << "total file num in cml: " << files.size() << std::endl;
	for (int i = 0; i < files.size(); i++)
	{
		ExtractOneCMLFile(cmlheader, files[i], wideSavePath, cml, nameCP);
	}
	fclose(cml);
	delete[] wideSavePath;
}


