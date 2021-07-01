#pragma once

#ifndef  CP3UNPACK_H
#define CP3UNPACK_H

#include<iostream>
#include<vector>
#include<zlib.h>
#include<stdio.h>
#include"Struct.h"
#include"Helper.h"

void ExtractCP3Main(char* filepath, char* savepath, int nameCP);

#endif // ! CP3UNPACK_H
