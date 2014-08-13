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

            // 見つからなかったのでサーチ
            //Pairing();
        }

        public BluetoothDeviceInfo[] GetPairDevices()
        {
            BluetoothClient client = new BluetoothClient();
            return client.DiscoverDevices(10, false, true, false);
        }

        public void Pairing()
        {
            bluetoothPair.OnSearchDevice += bluetoothPair_OnSearchDevice;
            bluetoothPair.Search();
        }

        void bluetoothPair_OnSearchDevice(BluetoothDeviceInfo obj)
        {
            if (obj.DeviceName == ConnectPhoneName)
            {
                Connect(obj);
            }
        }

        public bool Connect(BluetoothDeviceInfo device)
        {
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

        public void Tel()
        {
            ProfileHfp.Dial();
        }

        public void getBook()
        {
            //PhoneBook[] books = ProfilePbap.GetPhoneBooks(PbapFolder.pb);

            //foreach (PhoneBook book in books)
            //{
            //    Console.WriteLine("{0}, {1}", book.Name, book.PhoneNumbers[0]);
            //}

            PhoneBook book = ProfilePbap.GetPhoneBookFromPhoneNumber(PbapFolder.pb, "08036865985");
            if (book != null)
            {
                Console.WriteLine("{0}, {1}", book.Name, book.PhoneNumbers[0]);
            }
        }
        
    }
}
