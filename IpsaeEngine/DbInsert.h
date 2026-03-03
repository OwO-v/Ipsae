#pragma once
#include "Common.h"

unsigned int __stdcall StartDbInsertThread(void* param);

void EnqueueDbInsert(const std::string& data);