﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Messaging;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GlobalHotKeys
{ 
    public partial class HotKeyWindow : Window, IDisposable
    {
        private static HotKeyWindow _Instance = null;
        private static List<HotKey> RegisteredKeys = new List<HotKey>();

        public static HotKeyWindow Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new HotKeyWindow();
                }
                return _Instance;
            }
        }

        private bool disposed = false;
        internal IntPtr hWnd;
        private HotKeyWindow() : base()
        {
            //make this window invisible.
            InitializeComponent();
            hWnd = (new WindowInteropHelper(this)).Handle;
            Show();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if(msg == HotKey.WM_HOTKEY)
            {
                //check hotkey register ID
                int id = wParam.ToInt32();

                //raise the event with the hotkey ID
                foreach (HotKey hkey in RegisteredKeys)
                {
                    if (hkey.id == id)
                    {
                        hkey.RaiseHotKeyEvent();
                    }
                }

                handled = true;
            }
            return IntPtr.Zero;
        }

        internal static void RegisterHotKey(HotKey hkey)
        {
            RegisteredKeys.Add(hkey);
        }

        public void Dispose()
        {
            Dispose(true);
            disposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (RegisteredKeys != null)
            {
                foreach (HotKey hkey in HotKeyWindow.RegisteredKeys)
                {
                    hkey.Dispose();
                }
            }
        }
    }
}
