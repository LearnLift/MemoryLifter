/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/

// Autorun.cpp : Defines the entry point for the application.
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
