using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
//using android.graphics.Bitmap;
//using android.util.Log;

//using java.util.ArrayList;
//using java.util.List;
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
    public class Utils
    {

        // UNICODE 0x23 = #
        public static byte[] UNICODE_TEXT = new byte[] {0x23, 0x23, 0x23,
            0x23, 0x23, 0x23,0x23, 0x23, 0x23,0x23, 0x23, 0x23,0x23, 0x23, 0x23,
            0x23, 0x23, 0x23,0x23, 0x23, 0x23,0x23, 0x23, 0x23,0x23, 0x23, 0x23,
            0x23, 0x23, 0x23};

        private static String hexStr = "0123456789ABCDEF";
        private static String[] binaryArray = { "0000", "0001", "0010", "0011",
            "0100", "0101", "0110", "0111", "1000", "1001", "1010", "1011",
            "1100", "1101", "1110", "1111" };

        public static byte[] DecodeBitmap(Bitmap bmp)
        {
            int bmpWidth = bmp.Width;
            int bmpHeight = bmp.Height;

            List<String> list = new List<String>(); //binaryString list
            StringBuilder sb;


            int bitLen = bmpWidth / 8;
            int zeroCount = bmpWidth % 8;

            String zeroStr = "";
            if (zeroCount > 0)
            {
                bitLen = bmpWidth / 8 + 1;
                for (int i = 0; i < (8 - zeroCount); i++)
                {
                    zeroStr = zeroStr + "0";
                }
            }

            for (int i = 0; i < bmpHeight; i++)
            {
                sb = new StringBuilder();
                for (int j = 0; j < bmpWidth; j++)
                {
                    int color = bmp.GetPixel(j, i);

                    int r = (color >> 16) & 0xff;
                    int g = (color >> 8) & 0xff;
                    int b = color & 0xff;

                    // if color close to white，bit='0', else bit='1'
                    if (r > 160 && g > 160 && b > 160)
                        sb.Append("0");
                    else
                        sb.Append("1");
                }
                if (zeroCount > 0)
                {
                    sb.Append(zeroStr);
                }
                list.Add(sb.ToString());
            }

            List<String> bmpHexList = BinaryListToHexStringList(list);
            String commandHexString = "1D763000";
            String widthHexString = (bmpWidth % 8 == 0 ? bmpWidth / 8 : (bmpWidth / 8 + 1)).ToString("X"); //Int32.toHexString(bmpWidth % 8 == 0 ? bmpWidth / 8 : (bmpWidth / 8 + 1));
            if (widthHexString.Length > 2)
            {
                //Log.E("decodeBitmap error", " width is too large");
                return null;
            }
            else if (widthHexString.Length == 1)
            {
                widthHexString = "0" + widthHexString;
            }
            widthHexString = widthHexString + "00";

            String heightHexString = bmpHeight.ToString("X");
            if (heightHexString.Length > 2)
            {
                // Log.e("decodeBitmap error", " height is too large");
                return null;
            }
            else if (heightHexString.Length == 1)
            {
                heightHexString = "0" + heightHexString;
            }
            heightHexString = heightHexString + "00";

            List<String> commandList = new List<String>();
            commandList.Add(commandHexString + widthHexString + heightHexString);
            commandList.AddRange(bmpHexList);

            return hexList2Byte(commandList);
        }

        public static List<String> BinaryListToHexStringList(List<String> list)
        {
            List<String> hexList = new List<String>();
            foreach (var binaryStr in list) //for (String binaryStr : list)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < binaryStr.Length; i += 8)
                {
                    String str = binaryStr.Substring(i, i + 8);

                    String hexString = myBinaryStrToHexString(str);
                    sb.Append(hexString);
                }
                hexList.Add(sb.ToString());
            }
            return hexList;

        }

        public static String myBinaryStrToHexString(String binaryStr)
        {
            String hex = "";
            String f4 = binaryStr.Substring(0, 4);
            String b4 = binaryStr.Substring(4, 8);
            for (int i = 0; i < binaryArray.Length; i++)
            {
                if (f4.Equals(binaryArray[i]))
                    hex += hexStr.Substring(i, i + 1);
            }
            for (int i = 0; i < binaryArray.Length; i++)
            {
                if (b4.Equals(binaryArray[i]))
                    hex += hexStr.Substring(i, i + 1);
            }

            return hex;
        }

        public static byte[] hexList2Byte(List<String> list)
        {
            List<byte[]> commandList = new List<byte[]>();

            foreach (var hexStr in list) //for (String hexStr : list)
            {
                commandList.Add(hexStringToBytes(hexStr));
            }
            byte[] bytes = sysCopy(commandList);
            return bytes;
        }

        public static byte[] hexStringToBytes(String hexString)
        {
            if (hexString == null || hexString.Equals(""))
            {
                return null;
            }
            hexString = hexString.ToUpper();
            int length = hexString.Length / 2;
            char[] hexChars = hexString.ToCharArray();
            byte[] d = new byte[length];
            for (int i = 0; i < length; i++)
            {
                int pos = i * 2;
                d[i] = (byte)(charToByte(hexChars[pos]) << 4 | charToByte(hexChars[pos + 1]));
            }
            return d;
        }

        public static byte[] sysCopy(List<byte[]> srcArrays)
        {
            int len = 0;
            foreach (byte[] srcArray in srcArrays) //for (byte[] srcArray : srcArrays)
            {
                len += srcArray.Length;
            }
            byte[] destArray = new byte[len];
            int destLen = 0;
            foreach (byte[] srcArray in srcArrays) //for (byte[] srcArray : srcArrays)
            {
                Array.Copy(srcArray, 0, destArray, destLen, srcArray.Length);
                destLen += srcArray.Length;
            }
            return destArray;
        }

        private static byte charToByte(char c)
        {
            return (byte)"0123456789ABCDEF".IndexOf(c);
        }
    }

}


