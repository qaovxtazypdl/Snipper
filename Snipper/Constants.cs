﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snipper
{
    internal static class Constants
    {
        public const int CAP_WINDOW_HOTKEY = 1;
        public const int CAP_AREA_HOTKEY = 2;

        public const string APP_GUID = "f2d44a6c-b312-430d-b18c-914e8cbe11b2";
        public const string SETTINGS_FILE_NAME = "settings.xml";

        public const string SETTINGS_TAG = "settings";
        public const string SAVE_DIR_TAG = "savedir";
        public const string SEL_CAP_HKEY_TAG = "selcaphkey";
        public const string WIN_CAP_HKEY_TAG = "wincaphkey";
        public const string COPY_CLIP_TAG = "docopyclip";
        public const string SAVE_IMAGE_TAG = "dosaveimage";
        public const string SUPRESS_TAG = "dosupress";
        public const string MODIFIERS_TAG = "mod";
        public const string KEY_TAG = "key";



        public static Dictionary<uint, string> vkeyMap;
        static Constants()
        {
            vkeyMap = new Dictionary<uint, string>();
            vkeyMap[0x08] = "BKSPC";
            vkeyMap[0x13] = "PAUSE";
            vkeyMap[0x20] = "SPACE";
            vkeyMap[0x21] = "PGUP";
            vkeyMap[0x22] = "PGDN";
            vkeyMap[0x23] = "END";
            vkeyMap[0x24] = "HOME";
            vkeyMap[0x25] = "LEFT";
            vkeyMap[0x26] = "UP";
            vkeyMap[0x27] = "RIGHT";
            vkeyMap[0x28] = "DOWN";
            vkeyMap[0x29] = "SELECT";
            vkeyMap[0x2D] = "INS";
            vkeyMap[0x2E] = "DEL";
            vkeyMap[0x30] = "0";
            vkeyMap[0x31] = "1";
            vkeyMap[0x32] = "2";
            vkeyMap[0x33] = "3";
            vkeyMap[0x34] = "4";
            vkeyMap[0x35] = "5";
            vkeyMap[0x36] = "6";
            vkeyMap[0x37] = "7";
            vkeyMap[0x38] = "8";
            vkeyMap[0x39] = "9";
            vkeyMap[0x41] = "A";
            vkeyMap[0x42] = "B";
            vkeyMap[0x43] = "C";
            vkeyMap[0x44] = "D";
            vkeyMap[0x45] = "E";
            vkeyMap[0x46] = "F";
            vkeyMap[0x47] = "G";
            vkeyMap[0x48] = "H";
            vkeyMap[0x49] = "I";
            vkeyMap[0x4A] = "J";
            vkeyMap[0x4B] = "K";
            vkeyMap[0x4C] = "L";
            vkeyMap[0x4D] = "M";
            vkeyMap[0x4E] = "N";
            vkeyMap[0x4F] = "O";
            vkeyMap[0x50] = "P";
            vkeyMap[0x51] = "Q";
            vkeyMap[0x52] = "R";
            vkeyMap[0x53] = "S";
            vkeyMap[0x54] = "T";
            vkeyMap[0x55] = "U";
            vkeyMap[0x56] = "V";
            vkeyMap[0x57] = "W";
            vkeyMap[0x58] = "X";
            vkeyMap[0x59] = "Y";
            vkeyMap[0x5A] = "Z";
            vkeyMap[0x60] = "N0";
            vkeyMap[0x61] = "N1";
            vkeyMap[0x62] = "N2";
            vkeyMap[0x63] = "N3";
            vkeyMap[0x64] = "N4";
            vkeyMap[0x65] = "N5";
            vkeyMap[0x66] = "N6";
            vkeyMap[0x67] = "N7";
            vkeyMap[0x68] = "N8";
            vkeyMap[0x69] = "N9";
            vkeyMap[0x6A] = "N*";
            vkeyMap[0x6B] = "N+";
            vkeyMap[0x6C] = "N\\";
            vkeyMap[0x6D] = "N-";
            vkeyMap[0x6E] = "N.";
            vkeyMap[0x6F] = "N/";
            vkeyMap[0x70] = "F1";
            vkeyMap[0x71] = "F2";
            vkeyMap[0x72] = "F3";
            vkeyMap[0x73] = "F4";
            vkeyMap[0x74] = "F5";
            vkeyMap[0x75] = "F6";
            vkeyMap[0x76] = "F7";
            vkeyMap[0x77] = "F8";
            vkeyMap[0x78] = "F9";
            vkeyMap[0x79] = "F10";
            vkeyMap[0x7A] = "F11";
            vkeyMap[0x7B] = "F12";
            vkeyMap[0x7C] = "F13";
            vkeyMap[0x7D] = "F14";
            vkeyMap[0x7E] = "F15";
            vkeyMap[0x7F] = "F16";
            vkeyMap[0x80] = "F17";
            vkeyMap[0x81] = "F18";
            vkeyMap[0x82] = "F19";
            vkeyMap[0x83] = "F20";
            vkeyMap[0x84] = "F21";
            vkeyMap[0x85] = "F22";
            vkeyMap[0x86] = "F23";
            vkeyMap[0x87] = "F24";
            vkeyMap[0xAD] = "VolumeMute";
            vkeyMap[0xAE] = "VolumeDown";
            vkeyMap[0xAF] = "VolumeUp";
            vkeyMap[0xB0] = "MediaNextTrack";
            vkeyMap[0xB1] = "MediaPrevTrack";
            vkeyMap[0xB2] = "MediaStop";
            vkeyMap[0xB3] = "MediaPlayPause";
            vkeyMap[0xBA] = ";";
            vkeyMap[0xBB] = "=";
            vkeyMap[0xBC] = ",";
            vkeyMap[0xBD] = "-";
            vkeyMap[0xBE] = ".";
            vkeyMap[0xBF] = "/";
        }
    }
}
