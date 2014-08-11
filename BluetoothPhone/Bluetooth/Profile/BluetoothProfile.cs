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
        protected BluetoothClient localClient { get { return client.LocalClient; } }

        private Client client;
        private Guid ProfileID;

        public BluetoothProfile(Client client, Guid ProfileID)
        {
            this.client = client;
            this.ProfileID = ProfileID;
        }

        public void Connect(BluetoothDeviceInfo device)
        {
            localClient.Connect(device.DeviceAddress, ProfileID);
        }

        protected virtual void OnConnected()
        {
        }
    }
}
