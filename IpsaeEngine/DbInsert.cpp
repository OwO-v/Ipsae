#include "pch.h"
#include "DbInsert.h"
#include "sqlite3.h"

#pragma comment(lib, "sqlite3.lib")

#pragma region values

static ThreadSafeQueue<std::string> s_dbInsertQueue;

static const char* DB_LIST[] = {
	"tb_threat_host",
	"tb_access_host",
	"tb_detect_proc",
	"tb_config_host",
};

#pragma endregion

#pragma region Forward Declaration

static int CheckDbTableList(sqlite3* db);
static int GetThreatHostList(sqlite3* db, std::unordered_set<std::string>& hosts);
static unsigned int StartDbInsert(HANDLE hReadyEvent);

#pragma endregion

#pragma region functions

void EnqueueDbInsert(const std::string& data)
{
	s_dbInsertQueue.Push(data);
}

unsigned int __stdcall StartDbInsertThread(void* param)
{
	THREAD_CONTEXT* context = (THREAD_CONTEXT*)param;

	return StartDbInsert(context->hReadyEvent);
}

static int CheckDbTableList(sqlite3* db)
{
	// 초기화
	sqlite3_stmt* stmt = NULL;

	// DB 유효성 검사
	if (db == NULL)
	{
		wprintf(L"[FAIL][DbInsert] Can't check DB table list: DB is NULL\n");
		return 1;
	}

	// SQL 준비
	const char* selectSql = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=?;";
	int rc = sqlite3_prepare_v2(db, selectSql, -1, &stmt, NULL);
	if (rc != SQLITE_OK)
	{
		wprintf(L"[FAIL][DbInsert] SELECT: %hs\n", sqlite3_errmsg(db));
		sqlite3_finalize(stmt);
		return 1;
	}

	// SQL 실행 결과 및 결과 처리
	for (auto tableName : DB_LIST)
	{		
		sqlite3_bind_text(stmt, 1, tableName, -1, SQLITE_STATIC);
		sqlite3_step(stmt);

		int exists = sqlite3_column_int(stmt, 0);
		if (exists == 0)
		{
			wprintf(L"[FAIL][DbInsert] 테이블 확인 실패: %hs\n", tableName);
			sqlite3_finalize(stmt);

			return 1;
		}

		// 다음 작업 준비
		sqlite3_reset(stmt);
	}
	
	// SQL 문 종료 및 결과 반환
	sqlite3_finalize(stmt);
	return 0;
}

static int GetThreatHostList(sqlite3* db, std::unordered_set<std::string> &hosts)
{
	// 초기화
	sqlite3_stmt* stmt = NULL;
	std::unordered_set<std::string> threat_hosts;

	// DB 유효성 검사
	if (db == NULL)
	{
		wprintf(L"[FAIL][DbInsert] Can't check DB table list: DB is NULL\n");
		return 1;
	}

	// SQL 준비
	const char* selectSql = "SELECT host FROM tb_threat_host;";
	int rc = sqlite3_prepare_v2(db, selectSql, -1, &stmt, NULL);
	if (rc != SQLITE_OK)
	{
		wprintf(L"[FAIL][DbInsert] SELECT: %hs\n", sqlite3_errmsg(db));
		sqlite3_finalize(stmt);
		return 1;
	}

	// SQL 실행 및 결과 처리
	while (sqlite3_step(stmt) == SQLITE_ROW)
	{
		const char* host = (const char*)sqlite3_column_text(stmt, 0);
		if (host != NULL)
			threat_hosts.insert(host);
	}

	// SQL 문 종료
	sqlite3_finalize(stmt);

	// 결과 반환 - 완료 시 조회된 호스트 목록을 참조 매개변수에 저장
	hosts = std::move(threat_hosts);
	return 0;
}

static unsigned int StartDbInsert(HANDLE hReadyEvent)
{
	sqlite3* db = NULL;
	sqlite3_stmt* stmt = NULL;
	std::unordered_set<std::string> threat_hosts;
	std::string data;

	// DB Open
	int rc = sqlite3_open("ipsaedb.db", &db);
	if (rc != SQLITE_OK)
	{
		wprintf(L"[FAIL][DbInsert] sqlite3_open: %hs\n", sqlite3_errmsg(db));
		sqlite3_close(db);
		return 1;
	}

	// Table 점검
	if (CheckDbTableList(db) > 0)
	{
		wprintf(L"[FAIL][DbInsert] 테이블 확인 실패\n");
		sqlite3_close(db);
		return 1;
	}

	// 유해 IP 목록 수집
	if (GetThreatHostList(db, threat_hosts) > 0)
	{
		wprintf(L"[FAIL][DbInsert] 유해 호스트 목록 조회 실패\n");
		sqlite3_close(db);
		return 1;
	}

	// Main 에게 Thread 가 준비되었음을 알림
	SetEvent(hReadyEvent);

	//DB Insert Queue에서 Pop 하여 DB에 저장
	while (s_dbInsertQueue.WaitAndPop(data))
	{
		//TODO: DB에 데이터 삽입 로직 추가
	}

	sqlite3_close(db);
	return 0;
}

#pragma endregion