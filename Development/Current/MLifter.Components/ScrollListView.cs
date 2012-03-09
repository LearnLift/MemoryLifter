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
