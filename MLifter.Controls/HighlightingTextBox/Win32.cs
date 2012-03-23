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
using System;
using System.Runtime;
using System.Runtime.InteropServices;

using HWND = System.IntPtr;

namespace MLifter.Controls.SyntaxHighlightingTextBox
{
	/// <summary>
	/// Summary Description for Win32.
	/// </summary>
	public class Win32
	{
		private Win32()
		{
		}

		public const int WM_USER = 0x400;
		public const int WM_PAINT = 0xF;
		public const int WM_KEYDOWN = 0x100;
		public const int WM_KEYUP = 0x101;
		public const int WM_CHAR = 0x102;

		public const int EM_GETSCROLLPOS  =       (WM_USER + 221);
		public const int EM_SETSCROLLPOS  =       (WM_USER + 222);

		public const int VK_CONTROL = 0x11;
		public const int VK_UP = 0x26;
		public const int VK_DOWN = 0x28;
		public const int VK_NUMLOCK = 0x90;

		public const short KS_ON = 0x01;
		public const short KS_KEYDOWN = 0x80;

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT 
		{
			public int x;
			public int y;
		}

		[DllImport("user32")] public static extern int SendMessage(HWND hwnd, int wMsg, int wParam, IntPtr lParam);
		[DllImport("user32")] public static extern int PostMessage(HWND hwnd, int wMsg, int wParam, int lParam);
		[DllImport("user32")] public static extern short GetKeyState(int nVirtKey);
		[DllImport("user32")] public static extern int LockWindowUpdate(HWND hwnd);

	}
}
