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
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MLifter.Components
{
    public class ScrollListView : ListView
    {
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        private const int SB_HORIZONT = 0x0;
        private const int SB_VERTICAL = 0x1;
        int wParam;

        public event ScrollEventHandler HScrollChanged;
        public event ScrollEventHandler VScrollChanged;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case WM_HSCROLL: wParam = m.WParam.ToInt32();
                    if (HScrollChanged == null) return;
                    HScrollChanged(this, new ScrollEventArgs(EventType(LOWORD(wParam)),
                      GetScrollPos(this.Handle, SB_HORIZONT))); break;
                case WM_VSCROLL: wParam = m.WParam.ToInt32();
                    if (VScrollChanged == null) return;
                    VScrollChanged(this, new ScrollEventArgs(EventType(LOWORD(wParam)),
                      GetScrollPos(this.Handle, SB_VERTICAL))); break;
            }
        }

        public static int HIWORD(int n) { return ((n >> 0x10) & 0xffff); }
        public static int LOWORD(int n) { return (n & 0xffff); }

        [DllImport("user32.dll")]
        static public extern int GetScrollPos(System.IntPtr hWnd, int nBar);

        private ScrollEventType EventType(int wParam)
        {
            if (wParam < Enum.GetValues(typeof(ScrollEventType)).Length)
                return (ScrollEventType)wParam;
            else return ScrollEventType.EndScroll;
        }
    }
}
