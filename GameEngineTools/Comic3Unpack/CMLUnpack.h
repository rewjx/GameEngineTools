#pragma once

#ifndef CMLUNPACK_H
#define CMLUNPACK_H

#include<iostream>
#include<vector>
#include<zlib.h>
#include<stdio.h>
#include"Struct.h"
#include"Helper.h"

void ExtractCMLMain(char* filepath, char* savepath, int nameCP);

#endif // !CMLUNPACK_H
