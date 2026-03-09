#pragma once
#include "Common.h"


void StopPacketCapture(const char* caller);

unsigned int __stdcall StartPacketCaptureThread(void* param);

// 메모
// Inspector에서 위협 DB를 조회할 때 DbInsertThread에 요청?
// 아니면 그냥 DBInsertThread에서 조회?