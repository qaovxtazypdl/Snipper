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
using System.ComponentModel;

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

        //variables to hold old values
        private string c_SaveDirectory;
        private uint c_WindowCapModifiers, c_WindowCapKey, c_SelectionCapModifiers, c_SelectionCapKey;
        private bool c_SaveToFolderChecked, c_CopyToClipboardChecked;

        //variables to hold new uncommitted values.
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

            SettingsDirty = false;
            SaveDirectory = "?";
            WindowCapModifiers = WindowCapKey = SelectionCapModifiers = SelectionCapKey = 0;
            SaveToFolderChecked = CopyToClipboardChecked = false;
            BackupCurrentSettings();
            LoadSettings();
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

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!PromptSaveIfDirty())
            {
                e.Cancel = true;
                return;
            }

            if (HotKeyWindow.RegisteredKeys != null)
            {
                foreach (HotKey hkey in HotKeyWindow.RegisteredKeys)
                {
                    hkey.Dispose();
                }
            }
            base.OnClosing(e);
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
            e.Handled = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelSave();
            e.Handled = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            e.Handled = true;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!PromptSaveIfDirty())
            {
                return;
            }
            this.WindowState = WindowState.Minimized;
            this.Hide();
            e.Handled = true;
        }

        private void SaveDirectoryTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            TextBox source = (TextBox)sender;
            SaveDirectory = source.Text;
            SettingsDirty = true;
            e.Handled = true;
        }

        private void SaveDirButton_Checked(object sender, RoutedEventArgs e)
        {
            SaveToFolderChecked = true;
            SettingsDirty = true;
            e.Handled = true;
        }

        void SaveDirButton_Unchecked(object sender, RoutedEventArgs e)
        {
            SaveToFolderChecked = false;
            SettingsDirty = true;
            e.Handled = true;
        }

        private void CopyClipboardButton_Checked(object sender, RoutedEventArgs e)
        {
            CopyToClipboardChecked = true;
            SettingsDirty = true;
            e.Handled = true;
        }

        void CopyClipboardButton_Unchecked(object sender, RoutedEventArgs e)
        {
            CopyToClipboardChecked = false;
            SettingsDirty = true;
            e.Handled = true;
        }

        private bool SaveSettings()
        {
            if (!CheckDirExistence(SaveDirectory))
            {
                return false;
            }

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
            return true;
        }

        private void ApplySettings()
        {
            BackupCurrentSettings();
            SnippingManager.Instance.ClearHotkeys();
            SnippingManager.Instance.hkeyWindowCap = new HotKey(Constants.CAP_WINDOW_HOTKEY, WindowCapModifiers, WindowCapKey, SnippingManager.Instance.HotKeyHandler);
            SnippingManager.Instance.hkeyAreaCap = new HotKey(Constants.CAP_AREA_HOTKEY, SelectionCapModifiers, SelectionCapKey, SnippingManager.Instance.HotKeyHandler);
            SnippingManager.Instance.SaveLocation = SaveDirectory;
            uint saveModeMask = (uint)(CopyToClipboardChecked? 0x1 : 0x0) | (uint)(SaveToFolderChecked? 0x2 : 0x0);
            SnippingManager.Instance.SavingMode = saveModeMask;
            ReloadUISettings();
            SettingsDirty = false;
        }

        private void BackupCurrentSettings() {
            c_SaveDirectory = SaveDirectory;
            c_WindowCapModifiers = WindowCapModifiers;
            c_WindowCapKey = WindowCapKey;
            c_SelectionCapModifiers = SelectionCapModifiers;
            c_SelectionCapKey = SelectionCapKey;
            c_SaveToFolderChecked = SaveToFolderChecked;
            c_CopyToClipboardChecked = CopyToClipboardChecked;
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
                //open console
                this.Show();
                this.Activate();
                this.Focus();  
            }
            ApplySettings();
            if (!CheckDirExistence(SaveDirectory)) return;
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

        //returns whether save failed
        private bool PromptSaveIfDirty()
        {
            if (SettingsDirty)
            {
                MessageBoxResult saveWarningResult = MessageBox.Show("You have unsaved changes. Save?", "Snipper - Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (saveWarningResult == MessageBoxResult.Yes)
                {
                    return SaveSettings();
                }
                else
                {
                    CancelSave();
                }
            }
            return true;
        }

        private bool CheckDirExistence(string dir)
        {
            if (!Directory.Exists(dir))
            {
                MessageBox.Show("The directory supplied does not exist. Please create the directory or enter a valid one.", "Snipper - Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void CancelSave()
        {
            SaveDirectory = c_SaveDirectory;
            WindowCapModifiers = c_WindowCapModifiers;
            WindowCapKey = c_WindowCapKey;
            SelectionCapModifiers = c_SelectionCapModifiers;
            SelectionCapKey = c_SelectionCapKey;
            SaveToFolderChecked = c_SaveToFolderChecked;
            CopyToClipboardChecked = c_CopyToClipboardChecked;
            ReloadUISettings();
        }

        private void ReloadUISettings()
        {
            SaveFolderCheckBox.IsChecked = SaveToFolderChecked;
            CopyClipCheckBox.IsChecked = CopyToClipboardChecked;
            SaveDirTextBox.Text = SaveDirectory;
            WinSelTextBox.Text = GetKeyComboString(WindowCapModifiers, WindowCapKey);
            AreaSelTextBox.Text = GetKeyComboString(SelectionCapModifiers, SelectionCapKey);
        }

        private string GetKeyComboString(uint modifiers, uint key) 
        {
            string combo = "";
            if ((modifiers & (uint)ModifierKeys.Control) != 0)
            {
                combo += "CTRL+";
            }
            if ((modifiers & (uint)ModifierKeys.Alt) != 0)
            {
                combo += "ALT+";
            }
            if ((modifiers & (uint)ModifierKeys.Shift) != 0)
            {
                combo += "SHIFT+";
            }
            if (key == 0)
            {
                combo += "...";
            }
            else
            {
                combo += Constants.vkeyMap[key];
            }
            return combo;
        }

        private void WindowCapTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            uint vkey = (uint)KeyInterop.VirtualKeyFromKey(e.Key);
            uint modifiers = (uint)e.KeyboardDevice.Modifiers;
            if (Constants.vkeyMap.ContainsKey(vkey) && modifiers != 0 && HotKey.isHotKeyAvilable(modifiers, vkey) && (modifiers != SelectionCapModifiers || vkey != SelectionCapKey))
            {
                WinSelTextBox.Text = GetKeyComboString(modifiers, (uint)KeyInterop.VirtualKeyFromKey(e.Key));
                WindowCapKey = vkey;
                WindowCapModifiers = modifiers;
                SettingsDirty = true;
            }
        }

        private void SelectionCapTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            uint vkey = (uint)KeyInterop.VirtualKeyFromKey(e.Key);
            uint modifiers = (uint)e.KeyboardDevice.Modifiers;
            if (Constants.vkeyMap.ContainsKey(vkey) && modifiers != 0 && HotKey.isHotKeyAvilable(modifiers, vkey) && (modifiers != WindowCapModifiers || vkey != WindowCapKey))
            {
                AreaSelTextBox.Text = GetKeyComboString(modifiers, (uint)KeyInterop.VirtualKeyFromKey(e.Key));
                SelectionCapKey = vkey;
                SelectionCapModifiers = modifiers;
                SettingsDirty = true;
            }
        }
    }
}