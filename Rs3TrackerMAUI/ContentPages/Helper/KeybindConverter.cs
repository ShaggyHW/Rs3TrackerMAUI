﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rs3TrackerMAUI.ContentPages.Helper {
    public class KeybindConverter {
        public static string KeyConversion(int keyCode) {
            string keyName = "";
            switch (keyCode) {
                case 1: keyName = "Escape"; break;
                case 2: keyName = "1"; break;
                case 3: keyName = "2"; break;
                case 4: keyName = "3"; break;
                case 5: keyName = "4"; break;
                case 6: keyName = "5"; break;
                case 7: keyName = "6"; break;
                case 8: keyName = "7"; break;
                case 9: keyName = "8"; break;
                case 10: keyName = "9"; break;
                case 11: keyName = "0"; break;
                case 12: keyName = "-"; break;
                case 13: keyName = "="; break;
                case 14: keyName = "Backspace"; break;
                case 15: keyName = "Tab"; break;
                case 16: keyName = "Q"; break;
                case 17: keyName = "W"; break;
                case 18: keyName = "E"; break;
                case 19: keyName = "R"; break;
                case 20: keyName = "T"; break;
                case 21: keyName = "Y"; break;
                case 22: keyName = "U"; break;
                case 23: keyName = "I"; break;
                case 24: keyName = "O"; break;
                case 25: keyName = "P"; break;
                case 26: keyName = "["; break;
                case 27: keyName = "]"; break;
                case 28: keyName = "Enter"; break;
                case 29: keyName = "Ctrl"; break;
                case 30: keyName = "A"; break;
                case 31: keyName = "S"; break;
                case 32: keyName = "D"; break;
                case 33: keyName = "F"; break;
                case 34: keyName = "G"; break;
                case 35: keyName = "H"; break;
                case 36: keyName = "J"; break;
                case 37: keyName = "K"; break;
                case 38: keyName = "L"; break;
                case 39: keyName = ";"; break;
                case 40: keyName = "'"; break;
                case 41: keyName = "`"; break;
                case 42: keyName = "Shift"; break;
                case 43: keyName = "\\"; break;
                case 44: keyName = "Z"; break;
                case 45: keyName = "X"; break;
                case 46: keyName = "C"; break;
                case 47: keyName = "V"; break;
                case 48: keyName = "B"; break;
                case 49: keyName = "N"; break;
                case 50: keyName = "M"; break;
                case 51: keyName = ","; break;
                case 52: keyName = "."; break;
                case 53: keyName = "/"; break;
                case 54: keyName = "ShiftRight"; break;
                case 55: keyName = "NumpadMultiply"; break;
                case 56: keyName = "Alt"; break;
                case 57: keyName = "Space"; break;
                case 58: keyName = "CapsLock"; break;
                case 59: keyName = "F1"; break;
                case 60: keyName = "F2"; break;
                case 61: keyName = "F3"; break;
                case 62: keyName = "F4"; break;
                case 63: keyName = "F5"; break;
                case 64: keyName = "F6"; break;
                case 65: keyName = "F7"; break;
                case 66: keyName = "F8"; break;
                case 67: keyName = "F9"; break;
                case 68: keyName = "F10"; break;
                case 71: keyName = "Numpad7"; break;
                case 72: keyName = "Numpad8"; break;
                case 73: keyName = "Numpad9"; break;
                case 74: keyName = "NumpadSubtract"; break;
                case 75: keyName = "Numpad4"; break;
                case 76: keyName = "Numpad5"; break;
                case 77: keyName = "Numpad6"; break;
                case 78: keyName = "NumpadAdd"; break;
                case 79: keyName = "Numpad1"; break;
                case 80: keyName = "Numpad2"; break;
                case 81: keyName = "Numpad3"; break;
                case 82: keyName = "Numpad0"; break;
                case 83: keyName = "NumpadDecimal"; break;
                case 87: keyName = "F11"; break;
                case 88: keyName = "F12"; break;
                case 91: keyName = "F13"; break;
                case 92: keyName = "F14"; break;
                case 93: keyName = "F15"; break;
                case 99: keyName = "F16"; break;
                case 100: keyName = "F17"; break;
                case 101: keyName = "F18"; break;
                case 102: keyName = "F19"; break;
                case 103: keyName = "F20"; break;
                case 104: keyName = "F21"; break;
                case 105: keyName = "F22"; break;
                case 106: keyName = "F23"; break;
                case 107: keyName = "F24"; break;
                case 3613: keyName = "CtrlRight"; break;
                case 3637: keyName = "NumpadDivide"; break;
                case 3640: keyName = "AltRight"; break;
                case 3655: keyName = "Home"; break;
                case 3657: keyName = "PageUp"; break;
                case 3663: keyName = "End"; break;
                case 3665: keyName = "PageDown"; break;
                case 3666: keyName = "Insert"; break;
                case 3667: keyName = "Delete"; break;
                case 3675: keyName = "Meta"; break;
                case 3676: keyName = "MetaRight"; break;
                case 57416: keyName = "ArrowUp"; break;
                case 57419: keyName = "ArrowLeft"; break;
                case 57421: keyName = "ArrowRight"; break;
                case 57424: keyName = "ArrowDown"; break;
            }
            return keyName;
        }
    }
}
