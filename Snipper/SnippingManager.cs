using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows;
using System.Windows.Media;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Snipper
{
    public class SnippingManager
    {
        private static SnippingManager _Instance = null;
        public static SnippingManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new SnippingManager();
                }
                return _Instance;
            }
        }

        private HotKey _hkeyWindowCap;
        public HotKey hkeyWindowCap
        {
            get
            {
                return _hkeyWindowCap;
            }
            set
            {
                if (_hkeyWindowCap != null)
                {
                    _hkeyWindowCap.Dispose();
                }
                _hkeyWindowCap = value;
            }
        }

        private HotKey _hkeyAreaCap;
        public HotKey hkeyAreaCap
        {
            get
            {
                return _hkeyAreaCap;
            }
            set
            {
                if (_hkeyAreaCap != null)
                {
                    _hkeyAreaCap.Dispose();
                }
                _hkeyAreaCap = value;
            }
        }

        internal enum SaveMode
        {
            ClipboardOnly,
            FileOnly,
            ClipboardAndFile
        }

        private string _saveLocation;
        internal string SaveLocation
        {
            get
            {
                return _saveLocation;
            }
            set
            {
                _saveLocation = value;
            }
        }

        private SaveMode _savingMode;
        internal SaveMode SavingMode
        {
            get
            {
                return _savingMode;
            }
            set
            {
                _savingMode = value;
            }
        }

        private SnippingManager()
        {
            _hkeyWindowCap = null;
            _hkeyAreaCap = null;
            _savingMode = SaveMode.ClipboardOnly;
            _saveLocation = "";
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref ScreenArea lpRect);

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        [StructLayout(LayoutKind.Sequential)]
        private struct ScreenArea
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public void HotKeyHandler(Object sender, HotKeyEventArgs e)
        {
            HotKeyProcesser(e.id);
        }

        private void HotKeyProcesser(int keyID)
        {
            if (keyID == Constants.CAP_WINDOW_HOTKEY)
            {
                ScreenArea lpRect = new ScreenArea();
                IntPtr hWnd = GetActiveWindow();
                GetWindowRect(hWnd, ref lpRect);
                ScreenCapToBitMap(lpRect.Left, lpRect.Top, lpRect.Right, lpRect.Bottom);
            }
            else if (keyID == Constants.CAP_AREA_HOTKEY)
            {
                AreaSelectionCanvas selectArea = new AreaSelectionCanvas();
                ScreenCapToBitMap((int)selectArea.minX, (int)selectArea.minY, (int)selectArea.maxX, (int)selectArea.maxY);
            }
        }

        private void ScreenCapToBitMap(int minX, int minY, int maxX, int maxY)
        {
            Console.WriteLine(minX + " " + minY + " " + maxX + " " + maxY);
        }
    }
}
