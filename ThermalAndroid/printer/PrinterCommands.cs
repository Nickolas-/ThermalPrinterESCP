using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;

using Android.Bluetooth;
using Java.IO;
using static Android.Views.View;
using Android.Content;
using System.Threading;
using Android.Graphics;
using Android.Util;
using System.Text;
using Java.Util;
using System.Collections.Generic;

namespace ThermalAndroid
{
    public class PrinterCommands
    {
        public static byte HT = 0x9;
        public static byte LF = 0x0A;
        public static byte CR = 0x0D;
        public static byte ESC = 0x1B;
        public static byte DLE = 0x10;
        public static byte GS = 0x1D;
        public static byte FS = 0x1C;
        public static byte STX = 0x02;
        public static byte US = 0x1F;
        public static byte CAN = 0x18;
        public static byte CLR = 0x0C;
        public static byte EOT = 0x04;

        public static byte[] INIT = { 27, 64 };
        public static byte[] FEED_LINE = { 10 };

        public static byte[] SELECT_FONT_A = { 20, 33, 0 };

        public static byte[] SET_BAR_CODE_HEIGHT = { 29, 104, 100 };
        public static byte[] PRINT_BAR_CODE_1 = { 29, 107, 2 };
        public static byte[] SEND_NULL_BYTE = { 0x00 };

        public static byte[] SELECT_PRINT_SHEET = { 0x1B, 0x63, 0x30, 0x02 };
        public static byte[] FEED_PAPER_AND_CUT = { 0x1D, 0x56, 66, 0x00 };

        public static byte[] SELECT_CYRILLIC_CHARACTER_CODE_TABLE = { 0x1B, 0x74, 0x11 };

        public static byte[] SELECT_BIT_IMAGE_MODE = { 0x1B, 0x2A, 33, 255, 0 };//-128, 0 };
        public static byte[] SET_LINE_SPACING_24 = { 0x1B, 0x33, 24 };
        public static byte[] SET_LINE_SPACING_30 = { 0x1B, 0x33, 30 };

        public static byte[] TRANSMIT_DLE_PRINTER_STATUS = { 0x10, 0x04, 0x01 };
        public static byte[] TRANSMIT_DLE_OFFLINE_PRINTER_STATUS = { 0x10, 0x04, 0x02 };
        public static byte[] TRANSMIT_DLE_ERROR_STATUS = { 0x10, 0x04, 0x03 };
        public static byte[] TRANSMIT_DLE_ROLL_PAPER_SENSOR_STATUS = { 0x10, 0x04, 0x04 };

        public static  byte[] ESC_FONT_COLOR_DEFAULT = new byte[] { 0x1B, (byte)'r', 0x00 };
        public static  byte[] FS_FONT_ALIGN = new byte[] { 0x1C, 0x21, 1, 0x1B,
            0x21, 1 };
        public static  byte[] ESC_ALIGN_LEFT = new byte[] { 0x1b, (byte)'a', 0x00 };
        public static  byte[] ESC_ALIGN_RIGHT = new byte[] { 0x1b, (byte)'a', 0x02 };
        public static  byte[] ESC_ALIGN_CENTER = new byte[] { 0x1b, (byte)'a', 0x01 };
        public static  byte[] ESC_CANCEL_BOLD = new byte[] { 0x1B, 0x45, 0 };


        /****************************************/
        public static  byte[] ESC_HORIZONTAL_CENTERS = new byte[] { 0x1B, 0x44, 20, 28, 00 };
        public static  byte[] ESC_CANCLE_HORIZONTAL_CENTERS = new byte[] { 0x1B, 0x44, 00 };
        /*********************************************/

        public static byte[] ESC_ENTER = new byte[] { 0x1B, 0x4A, 0x40 };
        public static byte[] PRINTE_TEST = new byte[] { 0x1D, 0x28, 0x41 };

    }
}