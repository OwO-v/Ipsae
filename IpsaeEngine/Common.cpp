#include "pch.h"
#include "Common.h"
#include <spdlog/sinks/stdout_color_sinks.h>
#include <spdlog/sinks/rotating_file_sink.h>


// =============================================================================================
// Common functions
// =============================================================================================

/// <summary>
/// 로거를 초기화하고 콘솔 및 파일 출력을 설정합니다.
/// </summary>
void InitializeLogger()
{
	CreateDirectoryA("C:\\Ipsae\\logs", NULL);
	std::string logPath = "C:\\Ipsae\\logs\\ipsae.log";

	auto console_sink = std::make_shared<spdlog::sinks::stdout_color_sink_mt>();
	auto file_sink = std::make_shared<spdlog::sinks::rotating_file_sink_mt>(logPath, 1024 * 1024 * 5, 3);

	auto logger = std::make_shared<spdlog::logger>("ipsae", spdlog::sinks_init_list{ console_sink, file_sink });
	logger->set_pattern("[%Y-%m-%d %H:%M:%S.%e] [%^%l%$] %v");
	logger->set_level(spdlog::level::trace);

	spdlog::set_default_logger(logger);
	spdlog::flush_every(std::chrono::seconds(3));
}

/// <summary>
/// 엔진이 WAITING 상태일 때 타임아웃까지 대기합니다.
/// </summary>
/// <param name="state">엔진 상태 포인터</param>
/// <param name="caller">호출자 모듈명 (로그용)</param>
/// <returns>대기 해제 시 true, 타임아웃 시 false (ENGINE_ERROR로 전환됨)</returns>
bool WaitForEngineWaiting(ENGINE_STATE* state, const char* caller)
{
	if (state->status != ENGINE_WAITING)
		return true;

	DWORD64 waitStart = GetTickCount64();

	while (state->status == ENGINE_WAITING)
	{
		if (GetTickCount64() - waitStart > TIMEOUT_WAITING)
		{
			spdlog::warn("[{}] Waiting 상태 지연으로 작업을 중단합니다.", caller);
			state->status = ENGINE_ERROR;
			return false;
		}
		Sleep(100);
	}
	return true;
}

/// <summary>
/// IP 주소를 문자열로 변환합니다.
/// </summary>
/// <param name="ip">변환할 IP 주소 (32비트 정수).</param>
/// <param name="buf">결과 문자열을 저장할 버퍼.</param>
/// <param name="bufLen">버퍼의 크기 (바이트 단위).</param>
void IpToStr(UINT32 ip, char* buf, size_t bufLen)
{
	UINT8* b = (UINT8*)&ip;
	sprintf_s(buf, bufLen, "%u.%u.%u.%u", b[0], b[1], b[2], b[3]);
}

/// <summary>
/// 문자열을 IP 주소로 변환합니다.
/// </summary>
/// <param name="str">변환할 문자열 (예: "192.168.0.1").</param>
/// <returns>변환된 IP 주소 (32비트 정수).</returns>
UINT32 StrToIp(const char* str)
{
	UINT8 b[4];
	sscanf_s(str, "%hhu.%hhu.%hhu.%hhu", &b[0], &b[1], &b[2], &b[3]);
	return *(UINT32*)b;
}