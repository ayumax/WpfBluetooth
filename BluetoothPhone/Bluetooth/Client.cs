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

using BluetoothPhone.Bluetooth.Profile;
using BluetoothPhone.Bluetooth.Profile.Pbap;
using BluetoothPhone.Bluetooth.Profile.Hfp;

namespace BluetoothPhone.Bluetooth
{
    public class Client
    {
        public BluetoothClient LocalClient { get; private set; }

        private Pbap ProfilePbap;
        private Hfp ProfileHfp;

        public Client()
        {
            LocalClient = new BluetoothClient();
            LocalClient.Encrypt = true;

            ProfilePbap = new Pbap(this);
            ProfileHfp = new Hfp(this);
        }

        public BluetoothDeviceInfo[] GetPairDevices()
        {
            return LocalClient.DiscoverDevices(10, false, true, false);
        }

        public bool Connect(BluetoothDeviceInfo device)
        {
            try
            {
                ProfilePbap.Connect(device);
                ProfileHfp.Connect(device);
            }
            catch
            {
                return false;
            }

            return true;
        }

    }
}
