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
        private string ConnectPhoneName;

        private PairingSupport bluetoothPair;
        private Pbap ProfilePbap;
        private Hfp ProfileHfp;

        public Client()
        {
            bluetoothPair = new PairingSupport();

            ProfilePbap = new Pbap();
            ProfileHfp = new Hfp();

            ProfileHfp.OnRing += ProfileHfp_OnRing;
        }

 
        public void InitPhone(string phoneName)
        {
            ConnectPhoneName = phoneName;

            BluetoothDeviceInfo[] devices = GetPairDevices();

            foreach (BluetoothDeviceInfo device in devices)
            {
                if (device.DeviceName == ConnectPhoneName)
                {
                    Connect(device);

                    return;
                }
            }

        }

        public BluetoothDeviceInfo[] GetPairDevices()
        {
            BluetoothClient client = new BluetoothClient();
            return client.DiscoverDevices(10, false, true, false);
        }


        public bool Connect(BluetoothDeviceInfo device)
        {
            if (device.Connected)
            {
                // すでに接続済み
                return true;
            }

            try
            {
                ProfilePbap.Connect(device, bluetoothPair);
                ProfileHfp.Connect(device, bluetoothPair);

                Console.WriteLine("OnConnected");
            }
            catch
            {
                Console.WriteLine("Bluetooth Connected Fail!");
                return false;
            }

            return true;
        }

        public void Tel(string PhoneNumber)
        {
            ProfileHfp.Dial(PhoneNumber);
        }

        public PhoneBook[] getPhoneBooks(PbapFolder Folder, int MaxNum = -1)
        {
            return ProfilePbap.GetPhoneBooks(Folder, MaxNum);
        }
        

         


        void ProfileHfp_OnRing(string phoneNumber)
        {
            PhoneBook book = ProfilePbap.GetPhoneBookFromPhoneNumber(PbapFolder.pb, phoneNumber);
            if (book != null)
            {
                Console.WriteLine("{0}, {1}", book.Name, book.PhoneNumbers[0]);
            }
        }

    }
}
