#define _CRT_SECURE_NO_WARNINGS
#include"CP3Unpack.h"
#include"CMLUnpack.h"


bool isEqualMagic(char* filename, char* magic)
{
	FILE* file = fopen(filename, "rb");
	char fileMagic[8] = { 0 };
	bool rtn = false;
	if (file != NULL)
	{
		fread(fileMagic, 1, 8, file);
		if (strcmp(fileMagic, magic) == 0)
			rtn = true;
		fclose(file);
	}
	return rtn;
}


int main(int argc, char* argv[])
{
	if (argc < 3)
	{
		std::cout << "usage:  *.exe unpackFile savePath [optional: FileNameCodePage]" << std::endl;
		return 0;
	}
	char* unpackFile= argv[1];
	char* savePath = argv[2];
	int codePage = 932;
	if (argc > 3)
	{
		codePage = atoi(argv[3]);
		if (codePage == 0)
		{
			std::cout << "usage:  *.exe unpackFile savePath [optional: FileNameCodePage]" << std::endl;
			return 0;
		}
	}
	if (isEqualMagic(unpackFile, (char*)CP3Magic))
	{
		ExtractCP3Main(unpackFile, savePath, codePage);
		return 0;
	}
	if (isEqualMagic(unpackFile, (char*)CMLMagic))
	{
		ExtractCMLMain(unpackFile, savePath, codePage);
		return 0;
	}
	std::cout << "not supported file type" << std::endl;
}

