using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows;
using System.Windows.Media;
using System.IO;

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

        private HotKey _hkeyWindowCap = null;
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

        private HotKey _hkeyAreaCap = null;
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

        private SnippingManager()
        {
            
        }
       
        public void HotKeyHandler(Object sender, HotKeyEventArgs e)
        {
            HotKeyProcesser(e.id);
        }

        public void HotKeyProcesser(int keyID)
        {
            if (keyID == Constants.CAP_WINDOW_HOTKEY)
            {
                //get window
            }
            else if (keyID == Constants.CAP_AREA_HOTKEY)
            {
                new AreaSelectionCanvas();
            }
        }
    }
}
