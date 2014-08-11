using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Bluetooth.AttributeIds;
using InTheHand.Net.Ports;
using InTheHand.Net.Sockets;

namespace BluetoothPhone.Bluetooth
{
    class Pairing
    {
        private BluetoothClient localClient;


        private BluetoothWin32Authentication _win32Auth;

        private const string DEVICE_PIN = "0000";


        public event Action<BluetoothDeviceInfo> OnSearchDevice;
        public event Action<BluetoothDeviceInfo> OnConnected;


        public Pairing(BluetoothClient client)
        {
            this.localClient = client;
        }

        public void Search()
        {
            // component is used to manage device discovery
            BluetoothComponent localComponent = new BluetoothComponent(localClient);

            // async methods, can be done synchronously too

            localComponent.DiscoverDevicesProgress += new EventHandler<DiscoverDevicesEventArgs>(component_DiscoverDevicesProgress);
            localComponent.DiscoverDevicesComplete += new EventHandler<DiscoverDevicesEventArgs>(component_DiscoverDevicesComplete);

            localComponent.DiscoverDevicesAsync(255, true, true, true, true, null);
        }



        private void component_DiscoverDevicesProgress(object sender, DiscoverDevicesEventArgs e)
        {
            // log and save all found devices
            for (int i = 0; i < e.Devices.Length; i++)
            {
                if (e.Devices[i].Remembered)
                {
                    Console.WriteLine(e.Devices[i].DeviceName + " (" + e.Devices[i].DeviceAddress + "): Device is known");
                }
                else
                {
                    Console.WriteLine(e.Devices[i].DeviceName + " (" + e.Devices[i].DeviceAddress + "): Device is unknown");
                }

                if (OnSearchDevice != null)
                {
                    OnSearchDevice(e.Devices[i]);
                }

            }
        }

        private void component_DiscoverDevicesComplete(object sender, DiscoverDevicesEventArgs e)
        {
            // log some stuff
        }

        public bool PairingTo(BluetoothDeviceInfo device)
        {

            if (!device.Authenticated)
            {
                device.SetServiceState(BluetoothService.PhonebookAccessPse, true);

                _win32Auth = new BluetoothWin32Authentication(HandleWin32Auth);


                // replace DEVICE_PIN here, synchronous method, but fast
                return BluetoothSecurity.PairRequest(device.DeviceAddress, DEVICE_PIN);
            }

            return true;
        }

        public void RemovePair(BluetoothDeviceInfo device)
        {
            if (device == null) return;

            BluetoothSecurity.RemoveDevice(device.DeviceAddress);
        }


        void HandleWin32Auth(object sender, BluetoothWin32AuthenticationEventArgs e)
        {
            Console.WriteLine("AuthMeth: " + e.AuthenticationMethod);
            e.Confirm = true;
        }

        public void Connecting(BluetoothDeviceInfo device, Guid service)
        {
            // check if device is paired
            //if (device.Authenticated)
            //{
            //  set pin of device to connect with
            localClient.SetPin(DEVICE_PIN);

            // async connection method
            localClient.BeginConnect(device.DeviceAddress, service, new AsyncCallback(Connect), device);
            //}

        }

        // callback
        private void Connect(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                // client is connected now :)
                BluetoothDeviceInfo device = result.AsyncState as BluetoothDeviceInfo;


                if (OnConnected != null)
                {
                    OnConnected(device);
                }
            }
        }
    }
}
