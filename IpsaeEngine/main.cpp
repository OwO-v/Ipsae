#include <windows.h>
#include <fwpmu.h>
#include <stdio.h>
#include <io.h>
#include <fcntl.h>

#pragma comment(lib, "fwpuclnt.lib")

static const GUID IPSAE_SUBLAYER_KEY =
{ 0x40e26dc7, 0x2881, 0x42f6, { 0x90, 0x9c, 0xae, 0x30, 0xf9, 0x14, 0x7b, 0x8a } };


static const UINT16 BLOCK_PORT = 2843;

int wmain()
{
    _setmode(_fileno(stdout), _O_U16TEXT);

    HANDLE engineHandle = NULL;
    UINT64 filterId = 0;
    DWORD result = 0;

    FWPM_SESSION0 session = {};
    session.displayData.name = const_cast<wchar_t*>(L"IpsaeEngine Session");
    session.displayData.description = const_cast<wchar_t*>(L"Ipsae WFP Demo Session");
    session.flags = FWPM_SESSION_FLAG_DYNAMIC;

    result = FwpmEngineOpen0(NULL, RPC_C_AUTHN_WINNT, NULL, &session, &engineHandle);
    if (result != ERROR_SUCCESS)
    {
        wprintf(L"[FAIL] FwpmEngineOpen0: 0x%08X\n", result);
        if (result == ERROR_ACCESS_DENIED)
            wprintf(L"       -> 관리자 권한으로 실행하세요.\n");
        return 1;
    }
    wprintf(L"[OK] WFP 엔진 세션 열림 (handle: 0x%p)\n", engineHandle);

    FWPM_SUBLAYER0 subLayer = {};
    subLayer.subLayerKey = IPSAE_SUBLAYER_KEY;
    subLayer.displayData.name = const_cast<wchar_t*>(L"Ipsae SubLayer");
    subLayer.displayData.description = const_cast<wchar_t*>(L"Ipsae Demo SubLayer");
    subLayer.weight = 0x8000;

    result = FwpmSubLayerAdd0(engineHandle, &subLayer, NULL);
    if (result != ERROR_SUCCESS)
    {
        wprintf(L"[FAIL] FwpmSubLayerAdd0: 0x%08X\n", result);
        FwpmEngineClose0(engineHandle);
        return 1;
    }
    wprintf(L"[OK] Sublayer 등록 완료\n");


    FWPM_FILTER_CONDITION0 condition = {};
    condition.fieldKey = FWPM_CONDITION_IP_REMOTE_PORT;
    condition.matchType = FWP_MATCH_EQUAL;
    condition.conditionValue.type = FWP_UINT16;
    condition.conditionValue.uint16 = BLOCK_PORT;

    FWPM_FILTER0 filter = {};
    filter.displayData.name = const_cast<wchar_t*>(L"Ipsae Block Port 12345");
    filter.displayData.description = const_cast<wchar_t*>(L"Block outbound traffic on port 12345");
    filter.layerKey = FWPM_LAYER_OUTBOUND_TRANSPORT_V4;
    filter.subLayerKey = IPSAE_SUBLAYER_KEY;
    filter.action.type = FWP_ACTION_BLOCK;
    filter.weight.type = FWP_EMPTY;
    filter.numFilterConditions = 1;
    filter.filterCondition = &condition;

    result = FwpmFilterAdd0(engineHandle, &filter, NULL, &filterId);
    if (result != ERROR_SUCCESS)
    {
        wprintf(L"[FAIL] FwpmFilterAdd0: 0x%08X\n", result);
        FwpmSubLayerDeleteByKey0(engineHandle, &const_cast<GUID&>(IPSAE_SUBLAYER_KEY));
        FwpmEngineClose0(engineHandle);
        return 1;
    }
    wprintf(L"[OK] 필터 등록 완료 (filterId: %llu, port: %u)\n", filterId, BLOCK_PORT);

    wprintf(L"\n포트 %u 아웃바운드 트래픽이 차단 중입니다.\n", BLOCK_PORT);
    wprintf(L"Enter 키를 누르면 필터를 제거하고 종료합니다...\n");
    getwchar();

    result = FwpmFilterDeleteById0(engineHandle, filterId);
    if (result == ERROR_SUCCESS)
        wprintf(L"[OK] 필터 제거 완료 (filterId: %llu)\n", filterId);
    else
        wprintf(L"[WARN] FwpmFilterDeleteById0: 0x%08X\n", result);

    GUID sublayerKey = IPSAE_SUBLAYER_KEY;
    result = FwpmSubLayerDeleteByKey0(engineHandle, &sublayerKey);
    if (result == ERROR_SUCCESS)
        wprintf(L"[OK] Sublayer 제거 완료\n");
    else
        wprintf(L"[WARN] FwpmSubLayerDeleteByKey0: 0x%08X\n", result);

    result = FwpmEngineClose0(engineHandle);
    if (result == ERROR_SUCCESS)
        wprintf(L"[OK] WFP 엔진 세션 종료\n");
    else
        wprintf(L"[WARN] FwpmEngineClose0: 0x%08X\n", result);

    return 0;
}
