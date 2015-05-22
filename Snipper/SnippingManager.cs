using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;

using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

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
                _hkeyAreaCap = value;
            }
        }

        public enum SaveMode : uint
        {
            None = 0x0,
            ToClipboard = 0x1,
            ToFile = 0x2
        }

        private string _saveLocation;
        public string SaveLocation
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

        private uint _savingMode;
        public uint SavingMode
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
            _savingMode = (uint)SaveMode.ToClipboard | (uint)SaveMode.ToFile;
            _saveLocation = "";
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref REKT lpRect);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [StructLayout(LayoutKind.Sequential)]
        private struct REKT
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
                REKT lpRect = new REKT();
                IntPtr hWnd = GetForegroundWindow();

                if (GetWindowRect(hWnd, ref lpRect))
                {
                    ProcessScreenshot(lpRect.Left, lpRect.Top, lpRect.Right, lpRect.Bottom);
                }
            }
            else if (keyID == Constants.CAP_AREA_HOTKEY)
            {
                AreaSelectionCanvas selectArea = new AreaSelectionCanvas();
                if (selectArea.DialogResult == true)
                {
                    ProcessScreenshot((int)selectArea.minX, (int)selectArea.minY, (int)selectArea.maxX, (int)selectArea.maxY);
                }
            }
        }

        private void ProcessScreenshot(int minX, int minY, int maxX, int maxY)
        {
            BitmapSource screencap = null;

            int width = maxX - minX;
            int height = maxY - minY;

            using (Bitmap screenBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen(minX, minY, 0, 0, new System.Drawing.Size(width, height));
                    screencap = Imaging.CreateBitmapSourceFromHBitmap(
                        screenBmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }

            if (screencap != null)
            {
                if ((SavingMode & (uint)SaveMode.ToClipboard) != 0)
                {
                    CopyBitmapToClipboard(screencap);
                }
                if ((SavingMode & (uint)SaveMode.ToFile) != 0)
                {
                    SaveBitmapToFile(screencap);
                }
            }
        }

        private void SaveBitmapToFile(BitmapSource screencap)
        {
            DateTime currentTime = DateTime.Now;
            string filename = String.Format("{0}-{1}-{2}_{3}_{4}_{5}_{6}", currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second, currentTime.Millisecond);
            using (FileStream fileStream = new FileStream(Path.Combine(SaveLocation, filename + ".png"), FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(screencap));
                encoder.Save(fileStream);
            }
        }

        private void CopyBitmapToClipboard(BitmapSource screencap)
        {
            System.Windows.Clipboard.SetImage(screencap);
        }

        public void ClearHotkeys()
        {
            if (hkeyWindowCap != null)
            {
                hkeyWindowCap.Dispose();
            }
            if (hkeyAreaCap != null)
            {
                hkeyAreaCap.Dispose();
            }
        }
    }
}
