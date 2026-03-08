#pragma once
#include "Common.h"


void StopPacketCapture();
unsigned int StartPacketCapture(HANDLE hReadyEvent);

unsigned int __stdcall StartPacketCaptureThread(void* param);