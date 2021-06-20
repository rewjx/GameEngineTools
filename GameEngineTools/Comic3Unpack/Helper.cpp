#include"Helper.h"

unsigned int BigEndian2Little(unsigned int v)
{
	unsigned char b1 = v & 0xFF;
	unsigned char b2 = (v >> 8) & 0xFF;
	unsigned char b3 = (v >> 16) & 0xFF;
	unsigned char b4 = (v >> 24) & 0xFF;
	unsigned int rtn = (b1 << 8) + b2;
	rtn = (rtn << 8) + b3;
	rtn = (rtn << 8) + b4;
	return rtn;
}


void xor_decrypt_data(char* buf, unsigned int len, char* key, unsigned int* keyIndex)
{
	for (int i = 0; i < len; i++)
	{
		char kv = *(key + (*keyIndex % 0x100));
		*(buf + i) = *(buf + i) ^ kv;
		*keyIndex += 1;
	}
}


char* ConvertStrEncoding(char* str, int orgCodePage, int newCodePage)
{
	int wsize = MultiByteToWideChar(orgCodePage, 0, str, -1, NULL, 0);
	WCHAR* wstr = new WCHAR[wsize + 1];
	memset(wstr, 0, sizeof(WCHAR) * (wsize + 1));
	int realSize = MultiByteToWideChar(orgCodePage, 0, str, -1, wstr, wsize);
	int newsize = WideCharToMultiByte(newCodePage, 0, wstr, -1, NULL, 0, NULL, NULL);
	char* nstr = new char[newsize + 1];
	memset(nstr, 0, sizeof(char) * (newsize + 1));
	int ret = WideCharToMultiByte(newCodePage, 0, wstr, -1, nstr, newsize, NULL, NULL);
	delete[] wstr;
	return nstr;
}

WCHAR* Convert2WideChar(char* str, int codepage)
{
	int wsize = MultiByteToWideChar(codepage, 0, str, -1, NULL, 0);
	WCHAR* wstr = new WCHAR[wsize + 1];
	memset(wstr, 0, sizeof(WCHAR) * (wsize + 1));
	int realSize = MultiByteToWideChar(codepage, 0, str, -1, wstr, wsize);
	return wstr;
}
