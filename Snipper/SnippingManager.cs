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
using Hardcodet.Wpf.TaskbarNotification;
using GlobalHotKeys;

namespace Snipper
{
    public class SnippingManager : IDisposable
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

        private bool _SupressNotifications;
        public bool SupressBalloonNotifications
        {
            get
            {
                return _SupressNotifications;
            }
            set
            {
                _SupressNotifications = value;
            }
        }

        private SnippingManager()
        {
            _disposed = false;
            _SupressNotifications = false;
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
            MainWindow.Instance.TrayIcon.HideBalloonTip();
            string result = HotKeyProcesser(e.id);
            if (!SupressBalloonNotifications)
            {
                if (result == "")
                {
                    MainWindow.Instance.TrayIcon.ShowBalloonTip("Snipper", "Snip completed successfully.", BalloonIcon.Info);
                }
                else
                {
                    MainWindow.Instance.TrayIcon.ShowBalloonTip("Snipper", result, BalloonIcon.Warning);
                }
            }
        }

        private string HotKeyProcesser(int keyID)
        {
            string result = "";
            if (keyID == Constants.CAP_WINDOW_HOTKEY)
            {
                REKT lpRect = new REKT();
                IntPtr hWnd = GetForegroundWindow();

                if (GetWindowRect(hWnd, ref lpRect))
                {
                    result += ProcessScreenshot(lpRect.Left, lpRect.Top, lpRect.Right, lpRect.Bottom);
                }
            }
            else if (keyID == Constants.CAP_AREA_HOTKEY)
            {
                AreaSelectionCanvas selectArea = new AreaSelectionCanvas();
                if (selectArea.DialogResult == true)
                {
                    result += ProcessScreenshot((int)selectArea.minX, (int)selectArea.minY, (int)selectArea.maxX, (int)selectArea.maxY);
                }
            }
            return result;
        }

        private string ProcessScreenshot(int minX, int minY, int maxX, int maxY)
        {
            int width = maxX - minX;
            int height = maxY - minY;
            string result = "";

            using (Bitmap screenBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen(minX, minY, 0, 0, new System.Drawing.Size(width, height));
                }

                if (screenBmp != null)
                {
                    if ((SavingMode & (uint)SaveMode.ToClipboard) != 0)
                    {
                        result += CopyBitmapToClipboard(screenBmp);
                    }
                    if ((SavingMode & (uint)SaveMode.ToFile) != 0)
                    {
                        result += SaveBitmapToFile(screenBmp);
                    }
                }
                else
                {
                    result += "No image was captured. (null bitmap) ";
                }
            }
            return result;
        }

        private string SaveBitmapToFile(Bitmap screencap)
        {
            DateTime currentTime = DateTime.Now;
            string filename = String.Format("{0}-{1}-{2}_{3}_{4}_{5}_{6}", currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second, currentTime.Millisecond);
            try
            {
                screencap.Save(Path.Combine(SaveLocation, filename + ".png"), ImageFormat.Png);
            }
            catch (UnauthorizedAccessException)
            {
                return "Could not save to directory - no write access. ";
            }
            return "";
        }

        private string CopyBitmapToClipboard(Bitmap screencap)
        {
            System.Windows.Forms.Clipboard.SetImage(screencap);
            return "";
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

        private bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (hkeyAreaCap != null)
            {
                hkeyAreaCap.Dispose();
            }
            if (hkeyWindowCap != null)
            {
                hkeyWindowCap.Dispose();
            }
            _disposed = true;
        }
    }
}
