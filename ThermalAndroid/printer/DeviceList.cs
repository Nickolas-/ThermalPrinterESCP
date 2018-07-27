using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
//using java.io.IOException;
//using java.util.Set;
//using java.util.UUID;
//using android.app.ListActivity;
//using android.bluetooth.BluetoothAdapter;
//using android.bluetooth.BluetoothDevice;
//using android.bluetooth.BluetoothSocket;
//using android.content.BroadcastReceiver;
//using android.content.Context;
//using android.content.Intent;
//using android.content.IntentFilter;
//using android.os.Bundle;
//using android.view.Menu;
//using android.view.MenuItem;
//using android.view.View;
//using android.widget.ArrayAdapter;
//using android.widget.ListView;
//using android.widget.Toast;

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
    public class DeviceList : ListActivity
    {
        public const int REQUEST_CONNECT_BT = 0; //0 * 2300;
        private const int REQUEST_ENABLE_BT = 0; //0 * 1000;
        private BluetoothAdapter mBluetoothAdapter = null;
        private ArrayAdapter mArrayAdapter = null;

        static private ArrayAdapter<BluetoothDevice> btDevices = null;

        private UUID SPP_UUID = UUID.FromString("8ce255c0-200a-11e0-ac64-0800200c9a66");
        // UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

        static private BluetoothSocket mbtSocket = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Title = "Bluetooth Devices";
            
            try
            {
                if (InitDevicesList() == 0)
                {
                    this.Finish();
                    return;
                }

            }
            catch (Exception ex)
            {
                this.Finish();
                return;
            }

            mBTReceiver = new MyBroadcastReceiver(mArrayAdapter);

            IntentFilter btIntentFilter = new IntentFilter(
                    BluetoothDevice.ActionFound);
            RegisterReceiver(mBTReceiver, btIntentFilter);
        }

        public static BluetoothSocket getSocket()
        {
            return mbtSocket;
        }

        private void FlushData()
        {
            try
            {
                if (mbtSocket != null)
                {
                    mbtSocket.Close();
                    mbtSocket = null;
                }

                if (mBluetoothAdapter != null)
                {
                    mBluetoothAdapter.CancelDiscovery();
                }

                if (btDevices != null)
                {
                    btDevices.Clear();
                    btDevices = null;
                }

                if (mArrayAdapter != null)
                {
                    mArrayAdapter.Clear();
                    mArrayAdapter.NotifyDataSetChanged();
                    mArrayAdapter.NotifyDataSetInvalidated();
                    mArrayAdapter = null;
                }

                Finish();
            }
            catch (Exception ex)
            {
            }
            

        }
        private int InitDevicesList()
        {
            FlushData();

            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (mBluetoothAdapter == null)
            {
                Toast.MakeText(this,
                        "Bluetooth not supported!!", ToastLength.Long).Show();
                return -1;
            }

            if (mBluetoothAdapter.IsDiscovering)
            {
                mBluetoothAdapter.CancelDiscovery();
            }

            mArrayAdapter = new ArrayAdapter (this, Resource.Layout.layout_list);

            ListAdapter = mArrayAdapter;

            Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
            try
            {
                StartActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
            }
            catch (Exception ex)
            {
                return -2;
            }

            Toast.MakeText(Android.App.Application.Context,
                    "Getting all available Bluetooth Devices", ToastLength.Short)
                    .Show();

            return 0;

        }

        protected override void OnActivityResult(int reqCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(reqCode, resultCode, intent);

            switch (reqCode)
            {
                case REQUEST_ENABLE_BT:

                    if (resultCode == Result.Ok)
                    {
                        ICollection<BluetoothDevice> btDeviceList = mBluetoothAdapter.BondedDevices;
                        try
                        {
                            if (btDeviceList.Count > 0)
                            {

                               foreach(var device in btDeviceList) // for (BluetoothDevice device : btDeviceList)
                                {
                                    if (btDeviceList.Contains(device) == false)
                                    {

                                        btDevices.Add(device);

                                        mArrayAdapter.Add(device.Name + "\n"
                                                + device.Address);
                                        mArrayAdapter.NotifyDataSetInvalidated();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    break;
            }
            mBluetoothAdapter.StartDiscovery();

        }

        private MyBroadcastReceiver mBTReceiver;

        private class MyBroadcastReceiver : BroadcastReceiver
        {
            private ArrayAdapter mArrayAdapter;
            public MyBroadcastReceiver(ArrayAdapter mArrayAdapter)
            {
                this.mArrayAdapter = mArrayAdapter;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                String action = intent.Action;
                if (BluetoothDevice.ActionFound.Equals(action))
                {
                    BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);

                    try
                    {
                        if (btDevices == null)
                        {
                            btDevices = new ArrayAdapter<BluetoothDevice>(Application.Context, Resource.Layout.layout_list);
                        }

                        if (btDevices.GetPosition(device) < 0)
                        {
                            btDevices.Add(device);
                            mArrayAdapter.Add(device.Name + "\n" + device.Address + "\n");
                            mArrayAdapter.NotifyDataSetInvalidated();
                        }
                    }
                    catch (Exception ex)
                    {
                        //ex.fillInStackTrace();
                    }
                }
            }
        }

        protected override void OnListItemClick(ListView l, View v, int position,
                                   long id)
        {
            base.OnListItemClick(l, v, position, id);

            if (mBluetoothAdapter == null)
            {
                return;
            }

            if (mBluetoothAdapter.IsDiscovering)
            {
                mBluetoothAdapter.CancelDiscovery();
            }

            Toast.MakeText(this,
                    "Connecting to " + btDevices.GetItem(position).Name + ","
            + btDevices.GetItem(position).Address, ToastLength.Short).Show();

            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    bool gotuuid = btDevices.GetItem(position).FetchUuidsWithSdp();
                    UUID uuid = btDevices.GetItem(position).GetUuids()[0].Uuid;
                    mbtSocket = btDevices.GetItem(position)
                            .CreateRfcommSocketToServiceRecord(uuid);

                    mbtSocket.Connect();
                }
                catch (Exception ex)
                {
                    mbtSocket = null;
                    return;
                }
            });
        }
    }
}