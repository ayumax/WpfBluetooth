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
using Brecham.Obex;
using Brecham.Obex.Objects;

namespace BluetoothPhone.Bluetooth.Profile
{
    abstract class BluetoothProfile
    {
        public BluetoothClient LocalClient { get; private set; }

        private Guid ProfileID;
        protected BluetoothDeviceInfo ConnectedDevice;

        public BluetoothProfile(Guid ProfileID)
        {
            LocalClient = new BluetoothClient();

            this.ProfileID = ProfileID;
        }

        public void Connect(BluetoothDeviceInfo device, PairingSupport pairSupport)
        {
            //pairSupport.OnConnected += pairSupport_OnConnected;
            //pairSupport.Connecting(LocalClient, device, ProfileID);

            LocalClient.Connect(device.DeviceAddress, ProfileID);

            ConnectedDevice = device;


            OnConnected();
        }

        void pairSupport_OnConnected(BluetoothDeviceInfo obj)
        {
            Console.WriteLine("OnConnected:{0}", this.GetType());
            OnConnected();
        }

        protected virtual void OnConnected()
        {
           
        }
    }
}
