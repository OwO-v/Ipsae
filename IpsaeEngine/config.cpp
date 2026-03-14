#include <windows.h>
#include <string>
#include <cstdlib>

using namespace std;

string iniInterfaceParser()
{
	char interfaceOutput[256];
	string userName = getenv("USERPROFILE");

	if (userName.empty())
	{
		MessageBoxA(NULL, "USERPROFILE 환경 변수를 가져오는 데 실패했습니다.", "오류", MB_OK | MB_ICONERROR);
		return;
	}

	userName += "\\Documents\\Codes\\Ipsae\\IpsaeEngine\\config.ini";

	LPCTSTR fullPath = userName.c_str();

	GetPrivateProfileStringA("Engine", "Interface", "", interfaceOutput, sizeof(interfaceOutput), fullPath);

	return std::string(interfaceOutput);
}