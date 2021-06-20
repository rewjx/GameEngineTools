#pragma once

#ifndef  HELPER_H
#define HELPER_H
#include<Windows.h>

unsigned int BigEndian2Little(unsigned int v);

void xor_decrypt_data(char* buf, unsigned int len, char* key, unsigned int* keyIndex);

char* ConvertStrEncoding(char* str, int orgCodePage, int newCodePage);

WCHAR* Convert2WideChar(char* str, int codepage);

#endif // ! HELPER_H
