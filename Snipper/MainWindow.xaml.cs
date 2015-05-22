using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.IO;

namespace Snipper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow _Instance = null;
        public static MainWindow Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new MainWindow();
                }
                return _Instance;
            }
        }

        public event EventHandler ShowEvent;
        public event EventHandler CloseEvent;
        private string SaveDirectory;
        private bool settingsDirty;

        private MainWindow() : base()
        {
            InitializeComponent();
            ShowEvent += ShowEventHandler;
            CloseEvent += CloseEventHandler;
            LoadSettings();
            settingsDirty = false;
            //SnippingManager.Instance.hkeyWindowCap = new HotKey(Constants.CAP_WINDOW_HOTKEY, (uint)(ModifierKeys.Control | ModifierKeys.Shift), (uint)VirtualKey.N9, SnippingManager.Instance.HotKeyHandler);
            SnippingManager.Instance.hkeyWindowCap = new HotKey(Constants.CAP_WINDOW_HOTKEY, (uint)(ModifierKeys.Control), (uint)VirtualKey.N, SnippingManager.Instance.HotKeyHandler);
            //SnippingManager.Instance.hkeyAreaCap = new HotKey(Constants.CAP_AREA_HOTKEY, (uint)(ModifierKeys.Control | ModifierKeys.Shift), (uint)VirtualKey.N8, SnippingManager.Instance.HotKeyHandler);
            SnippingManager.Instance.hkeyAreaCap = new HotKey(Constants.CAP_AREA_HOTKEY, (uint)(ModifierKeys.Control), (uint)VirtualKey.B, SnippingManager.Instance.HotKeyHandler);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        public void ExecuteShowEvent()
        {
            EventHandler handler = ShowEvent;
            if (handler != null)
            {
                handler(null, new EventArgs());
            }
        }

        public void ExecuteCloseEvent()
        {
            EventHandler handler = CloseEvent;
            if (handler != null)
            {
                handler(null, new EventArgs());
            }
        }

        private void ShowEventHandler(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
            this.Focus();  
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void CloseEventHandler(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (HotKeyWindow.RegisteredKeys != null)
            {
                foreach (HotKey hkey in HotKeyWindow.RegisteredKeys)
                {
                    hkey.Dispose();
                }
            }
            base.OnClosed(e);
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void SaveDirectoryTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox source = (TextBox)sender;
            SaveDirectory = source.Text;
            settingsDirty = true;
        }

        private void WindowCapTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            settingsDirty = true;
        }

        private void SelectionCapTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            settingsDirty = true;
        }

        public void SaveSettings()
        {
            settingsDirty = false;
        }

        public void LoadSettings()
        {
            settingsDirty = false;
        }

        public void PromptSave()
        {

        }
    }
}
