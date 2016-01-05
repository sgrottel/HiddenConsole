using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SG.Utilities.Forms {

    /// <summary>
    /// Utility class to find the tray area
    /// </summary>
    internal static class TrayAreaUtility {

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int Left, Top, Right, Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        private static IntPtr GetSystemTrayHandle() {
            IntPtr hWndTray = FindWindow("Shell_TrayWnd", null);
            if (hWndTray != IntPtr.Zero) {
                hWndTray = FindWindowEx(hWndTray, IntPtr.Zero, "TrayNotifyWnd", null);
                if (hWndTray != IntPtr.Zero) {
                    hWndTray = FindWindowEx(hWndTray, IntPtr.Zero, "SysPager", null);
                    if (hWndTray != IntPtr.Zero) {
                        hWndTray = FindWindowEx(hWndTray, IntPtr.Zero, "ToolbarWindow32", null);
                        return hWndTray;
                    }
                }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Finds the tray icon area
        /// </summary>
        /// <param name="screen">The screen the tray icon area is on</param>
        /// <param name="location">The corner of the screen the tray icon area is at</param>
        internal static void FindTrayAreaLocation(out Screen screen, out Point location) {
            try {
                IntPtr th = GetSystemTrayHandle();
                if (th == IntPtr.Zero) throw new Exception();
                RECT r;
                GetWindowRect(th, out r);
                Point p = new Point((r.Left + r.Right) / 2, (r.Top + r.Bottom) / 2);

                Screen s = Screen.FromPoint(p);
                if (s == null) throw new Exception();

                screen = s;
                location = new Point(
                    (Math.Abs(s.WorkingArea.Left - p.X) < Math.Abs(s.WorkingArea.Right - p.X)) ? s.WorkingArea.Left : s.WorkingArea.Right,
                    (Math.Abs(s.WorkingArea.Top - p.Y) < Math.Abs(s.WorkingArea.Bottom - p.Y)) ? s.WorkingArea.Top : s.WorkingArea.Bottom);

            } catch {
                screen = Screen.PrimaryScreen;
                location = new Point(
                    (screen.Bounds.Left + screen.Bounds.Right) / 2,
                    (screen.Bounds.Top + screen.Bounds.Bottom) / 2);
            }
        }

    }

}
