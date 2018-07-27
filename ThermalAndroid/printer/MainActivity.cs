using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
//using android.app.Activity;
//using android.graphics.Bitmap;
//using android.graphics.BitmapFactory;
//using android.os.Bundle;

//using java.io.IOException;
//using java.io.OutputStream;
//using java.io.UnsupportedEncodingException;
//using java.util.Calendar;

//using android.bluetooth.BluetoothSocket;
//using android.content.Intent;
//using android.util.Log;
//using android.view.View;
//using android.view.View.OnClickListener;
//using android.widget.Button;
//using android.widget.EditText;
using Android.Bluetooth;
using Java.IO;
using static Android.Views.View;
using Android.Content;
using System.Threading;
using Android.Graphics;
using Android.Util;
using System.Text;
using Java.Util;
using System.IO;
//using Java.Lang;

namespace ThermalAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private String TAG = "Main Activity";
        EditText message;
        Button btnPrint, btnBill;

        byte FONT_TYPE;
        private static BluetoothSocket btsocket;
        private static Stream outputStream;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            //base.OnCreate(savedInstanceState);

            //SetContentView(Resource.Layout.activity_main);

            //Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //         SetSupportActionBar(toolbar);

            //FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            //         fab.Click += FabOnClick;
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            btnPrint = (Button)FindViewById(Resource.Id.btnPrint);

            btnPrint.Click += ((sender, e) => PrintPhoto()); 
        }

        private void PrintCustom(string msg, int size, int align)
        {
            //Print config "mode"
            byte[] cc = new byte[] { 0x1B, 0x21, 0x03 };  // 0- normal size text
                                                          //byte[] cc1 = new byte[]{0x1B,0x21,0x00};  // 0- normal size text
            byte[] bb = new byte[] { 0x1B, 0x21, 0x08 };  // 1- only bold text
            byte[] bb2 = new byte[] { 0x1B, 0x21, 0x20 }; // 2- bold with medium text
            byte[] bb3 = new byte[] { 0x1B, 0x21, 0x10 }; // 3- bold with large text
            try
            {
                switch (size)
                {
                    case 0:
                        outputStream.Write(cc);
                        break;
                    case 1:
                        outputStream.Write(bb);
                        break;
                    case 2:
                        outputStream.Write(bb2);
                        break;
                    case 3:
                        outputStream.Write(bb3);
                        break;
                }

                switch (align)
                {
                    case 0:
                        //left align
                        outputStream.Write(PrinterCommands.ESC_ALIGN_LEFT);
                        break;
                    case 1:
                        //center align
                        outputStream.Write(PrinterCommands.ESC_ALIGN_CENTER);
                        break;
                    case 2:
                        //right align
                        outputStream.Write(PrinterCommands.ESC_ALIGN_RIGHT);
                        break;
                }
                outputStream.Write(Encoding.ASCII.GetBytes(msg));
                outputStream.Write(new [] { PrinterCommands.LF });
                //outputStream.write(cc);
                //printNewLine();
            }
            catch (Exception e)
            {
               // e.printStackTrace();
            }

        }

        //print photo
        public void PrintPhoto()
        {
            if (btsocket == null)
            {
                Intent BTIntent = new Intent(this, typeof(DeviceList));
                StartActivityForResult(BTIntent, DeviceList.REQUEST_CONNECT_BT);
            }
            else
            {
                Stream opstream = null;
                try
                {
                    opstream = btsocket.OutputStream;
                }
                catch (Exception e)
                {
                   // e.printStackTrace();
                }
                outputStream = opstream;

                try
                {
                    Bitmap bmp = BitmapFactory.DecodeResource(Resources, Resource.Drawable.img);
                    if (bmp != null)
                    {
                        byte[] command = Utils.DecodeBitmap(bmp);
                        outputStream.Write(PrinterCommands.ESC_ALIGN_CENTER);
                        PrintText(command);
                    }
                    else
                    {
                        //Log.e("Print Photo error", "the file doesn't exist");
                    }
                }
                catch (Exception e)
                {
                    //e.PrintStackTrace();
                    //Log.e("PrintTools", "the file doesn't exist");
                }
            }
    
        }

        //print new line
        private void PrintNewLine()
        {
            try
            {
                outputStream.Write(PrinterCommands.FEED_LINE);
            }
            catch (Exception e)
            {
                // e.printStackTrace();
            }

        }

        public static void ResetPrint()
        {
            try
            {
                outputStream.Write(PrinterCommands.ESC_FONT_COLOR_DEFAULT);
                outputStream.Write(PrinterCommands.FS_FONT_ALIGN);
                outputStream.Write(PrinterCommands.ESC_ALIGN_LEFT);
                outputStream.Write(PrinterCommands.ESC_CANCEL_BOLD);
                outputStream.Write(new[] { PrinterCommands.LF });
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }
        }

        private void PrintText(string msg)
        {
            try
            {
                // Print normal text
                outputStream.Write(Encoding.ASCII.GetBytes(msg));
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }

        }

        private void PrintText(byte[] msg)
        {
            try
            {
                // Print normal text
                outputStream.Write(msg);
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                btsocket = DeviceList.getSocket();
                if (btsocket != null)
                {
                    PrintText(message.Text.ToString());
                }

            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }
        }
    }
}




















































        //DEFAULT
    //public override bool OnCreateOptionsMenu(IMenu menu)
    //    {
    //        //MenuInflateResource.Inflate(Resource.Menu.menu_main, menu);
    //        //return true;
    //    }

    //    public override bool OnOptionsItemSelected(IMenuItem item)
    //    //{
    //    //    int id = item.ItemId;
    //    //    if (id == Resource.Id.action_settings)
    //    //    {
    //    //        return true;
    //    //    }

    //    //    return base.OnOptionsItemSelected(item);
    //    }

    //    private void FabOnClick(object sender, EventArgs eventArgs)
    //    {
    //        //View view = (View) sender;
    //        //SnackbaResource.Make(view, "Replace with your own action", SnackbaResource.LengthLong)
    //        //    .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
    //    }
