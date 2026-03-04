#pragma once
#include "Models.h"
#include "Common.h"

unsigned int __stdcall StartDbInsertThread(void* param);

void EnqueueDbInsert(const DB_INSERT_BATCH& data);