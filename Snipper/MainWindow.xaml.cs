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
using System.Xml;

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
        private uint WindowCapModifiers, WindowCapKey, SelectionCapModifiers, SelectionCapKey;
        private bool SaveToFolderChecked, CopyToClipboardChecked;
        private bool _SettingsDirty;
        private bool SettingsDirty
        {
            get
            {
                return _SettingsDirty;
            }
            set
            {
                SaveButton.IsEnabled = value;
                _SettingsDirty = value;
            }
        }

        private MainWindow() : base()
        {
            InitializeComponent();
            ShowEvent += ShowEventHandler;
            CloseEvent += CloseEventHandler;
            LoadSettings();
            SettingsDirty = false;
            SaveDirectory = "?";
            WindowCapModifiers = WindowCapKey = SelectionCapModifiers = SelectionCapKey = 0;
            SaveToFolderChecked = CopyToClipboardChecked = false;

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

        private void SaveDirectoryTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            TextBox source = (TextBox)sender;
            SaveDirectory = source.Text;
            SettingsDirty = true;
        }

        private void SaveDirButton_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox source = (CheckBox)sender;
            SaveToFolderChecked = source.IsChecked == true;
            SettingsDirty = true;
        }

        private void CopyClipboardButton_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox source = (CheckBox)sender;
            CopyToClipboardChecked = source.IsChecked == true;
            SettingsDirty = true;
        }

        private void SaveSettings()
        {
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;

            XmlWriter xmlWriter = XmlWriter.Create(Constants.SETTINGS_FILE_NAME, xmlSettings);
            xmlWriter.WriteStartDocument();

            xmlWriter.WriteStartElement(Constants.SETTINGS_TAG);
            {
                xmlWriter.WriteStartElement(Constants.SAVE_DIR_TAG);
                xmlWriter.WriteString(SaveDirectory);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement(Constants.SEL_CAP_HKEY_TAG);
                xmlWriter.WriteStartElement(Constants.MODIFIERS_TAG);
                {
                    xmlWriter.WriteString("" + SelectionCapModifiers);
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement(Constants.KEY_TAG);
                {
                    xmlWriter.WriteString("" + SelectionCapKey);
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement(Constants.WIN_CAP_HKEY_TAG);
                xmlWriter.WriteStartElement(Constants.MODIFIERS_TAG);
                {
                    xmlWriter.WriteString("" + WindowCapModifiers);
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement(Constants.KEY_TAG);
                {
                    xmlWriter.WriteString("" + WindowCapKey);
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement(Constants.COPY_CLIP_TAG);
                xmlWriter.WriteString("" + CopyToClipboardChecked);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement(Constants.SAVE_IMAGE_TAG);
                xmlWriter.WriteString("" + SaveToFolderChecked);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            ApplySettings();
        }

        private void ApplySettings()
        {
            SnippingManager.Instance.hkeyWindowCap = new HotKey(Constants.CAP_WINDOW_HOTKEY, WindowCapModifiers, WindowCapKey, SnippingManager.Instance.HotKeyHandler);
            SnippingManager.Instance.hkeyAreaCap = new HotKey(Constants.CAP_AREA_HOTKEY, SelectionCapModifiers, SelectionCapKey, SnippingManager.Instance.HotKeyHandler);
            SnippingManager.Instance.SaveLocation = SaveDirectory;
            uint saveModeMask = (uint)(CopyToClipboardChecked? 0x1 : 0x0) & (uint)(SaveToFolderChecked? 0x1 : 0x0);
            SnippingManager.Instance.SavingMode = saveModeMask;
            SettingsDirty = false;

            SaveFolderCheckBox.IsChecked = SaveToFolderChecked;
            CopyClipCheckBox.IsChecked = CopyToClipboardChecked;
            SaveDirTextBox.Text = SaveDirectory;
            //
        }

        private void LoadSettings()
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(Constants.SETTINGS_FILE_NAME))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == Constants.SETTINGS_TAG)
                            {
                                LoadSettings(reader);
                            }
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            break;
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                //fail silently
            }
            ApplySettings();
        }

        private void LoadSettings(XmlReader reader) {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == Constants.SAVE_DIR_TAG)
                    {
                        reader.Read(); //read value node
                        SaveDirectory = reader.Value;
                        reader.Read(); //read end tag
                    }
                    else if (reader.Name == Constants.SEL_CAP_HKEY_TAG)
                    {
                        while (reader.Read())
                        {
                            if (reader.Name == Constants.MODIFIERS_TAG)
                            {
                                reader.Read();
                                SelectionCapModifiers = (uint)Int32.Parse(reader.Value);
                                reader.Read();
                            }
                            else if (reader.Name == Constants.KEY_TAG)
                            {
                                reader.Read();
                                SelectionCapKey = (uint)Int32.Parse(reader.Value);
                                reader.Read();
                            }
                            else if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                break;
                            }
                        }
                    }
                    else if (reader.Name == Constants.WIN_CAP_HKEY_TAG)
                    {
                        while (reader.Read())
                        {
                            if (reader.Name == Constants.MODIFIERS_TAG)
                            {
                                reader.Read();
                                WindowCapModifiers = (uint)Int32.Parse(reader.Value);
                                reader.Read();
                            }
                            else if (reader.Name == Constants.KEY_TAG)
                            {
                                reader.Read();
                                WindowCapKey = (uint)Int32.Parse(reader.Value);
                                reader.Read();
                            }
                            else if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                break;
                            }
                        }
                    }
                    else if (reader.Name == Constants.COPY_CLIP_TAG)
                    {
                        reader.Read();
                        CopyToClipboardChecked = reader.Value == ("" + true);
                        reader.Read();
                    }
                    else if (reader.Name == Constants.SAVE_IMAGE_TAG)
                    {
                        reader.Read();
                        SaveToFolderChecked = reader.Value == ("" + true);
                        reader.Read();
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
            }
        }

        private void PromptSaveIfDirty()
        {
            MessageBoxResult saveWarningResult = MessageBox.Show("You have unsaved changes. Save?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (saveWarningResult == MessageBoxResult.Yes)
            {
                SaveSettings();
            }
            else
            {
                CancelSave();
            }
        }

        private void CancelSave();

        private void WindowCapTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            SettingsDirty = true;
        }

        private void SelectionCapTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            SettingsDirty = true;
        }

        private void WindowCapTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            SettingsDirty = true;
        }

        private void SelectionCapTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            SettingsDirty = true;
        }
    }
}

//TODO: minimize button remove focus
//implement the 9 functions above
//nice bg image/ transparency for the main window.
//error checking on dir existnecen