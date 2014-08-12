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
        private Pbap ProfilePbap;
        private Hfp ProfileHfp;

        public Client()
        {
            ProfilePbap = new Pbap();
            ProfileHfp = new Hfp();
        }

        public BluetoothDeviceInfo[] GetPairDevices()
        {
            BluetoothClient client = new BluetoothClient();
            return client.DiscoverDevices(10, false, true, false);
        }

        public bool Connect(BluetoothDeviceInfo device)
        {
            try
            {
                ProfilePbap.Connect(device);
                ProfileHfp.Connect(device);

                Console.WriteLine("OnConnected");
            }
            catch
            {
                Console.WriteLine("Bluetooth Connected Fail!");
                return false;
            }

            return true;
        }

        public void Tel()
        {
            ProfileHfp.Dial();
        }

        public void getBook()
        {
            PhoneBook[] books = ProfilePbap.GetPhoneBooks(PbapFolder.pb);

            foreach (PhoneBook book in books)
            {
                Console.WriteLine("{0}, {1}", book.Name, book.PhoneNumbers[0]);
            }
        }
        
    }
}
