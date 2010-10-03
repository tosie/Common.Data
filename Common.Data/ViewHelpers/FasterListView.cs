using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Common.Data {
    /// <summary>
    /// Is a child class of the original <see cref="System.Windows.Forms.ListView"/>, that improves the drawing speed. See http://www.virtualdub.org/blog/pivot/entry.php?id=273 for details.
    /// </summary>
    public class FasterListView : ListView {
        
        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

        private uint LVM_SETTEXTBKCOLOR = 0x1026;

        [StructLayout(LayoutKind.Sequential)]
        private struct NMHDR {
            public IntPtr hwndFrom;
            public uint idFrom;
            public uint code;
        }

        private const uint NM_CUSTOMDRAW = unchecked((uint)-12);

        /// <summary>
        /// Constructor.
        /// </summary>
        public FasterListView() : base() {
            SendMessage(Handle, LVM_SETTEXTBKCOLOR, IntPtr.Zero, unchecked((IntPtr)(int)0xFFFFFF));
        }

        /// <summary>
        /// Overwritten window handler, that intercepts the NM_CUSTOMDRAW message and ignores it.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m) {
            if (m.Msg == 0x204E) {
                NMHDR hdr = (NMHDR)m.GetLParam(typeof(NMHDR));
                if (hdr.code == NM_CUSTOMDRAW) {
                    m.Result = (IntPtr)0;
                    return;
                }
            }

            base.WndProc(ref m);
        }
    }
}
