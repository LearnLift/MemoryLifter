// Autorun.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "shellapi.h"
#include "Autorun.h"

int APIENTRY _tWinMain(HINSTANCE hInstance,
					   HINSTANCE hPrevInstance,
					   LPTSTR    lpCmdLine,
					   int       nCmdShow)
{
	HINSTANCE result;
	LPCTSTR lpDefCmd = L"start.hta";
	//try to get the startup path and set it as the working directory
	int nArgs;
	LPWSTR *lpFullCmdLineArray = CommandLineToArgvW(GetCommandLine(), &nArgs);
	wchar_t lpStartupPath[_MAX_PATH];
	wchar_t drive[_MAX_DRIVE], dir[_MAX_DIR], fname[_MAX_FNAME], ext[_MAX_EXT];
	if (nArgs > 0)
	{
		_wsplitpath(lpFullCmdLineArray[0], drive, dir, fname, ext);
		_wmakepath(lpStartupPath, drive, dir, NULL, NULL);
		_wchdir(lpStartupPath);
	}

	//use the default cmd line
	if (wcslen(lpCmdLine) == 0)
	{
		wcscpy(lpCmdLine, lpDefCmd);
	}

	//  Launch the file specified on the command-line 
	result = ShellExecute(NULL, L"open", lpCmdLine, NULL, NULL, SW_SHOWNORMAL);

	//  Check the result
	if ((int)result <= 32)
	{
		//  An error was encountered launching 
		//  the .HTA, probably because the computer 
		//  doesn't have IE5 or greater

		//  Open windows explorer, showing the CD contents
		ShellExecute(NULL, L"explore", lpStartupPath, NULL, NULL, SW_SHOWNORMAL);
		return 1;
	}
	else
	{
		//  Launched OK
		return 0;
	}
}
