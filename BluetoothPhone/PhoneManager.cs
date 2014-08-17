using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InTheHand.Net.Sockets;

using BluetoothPhone.Bluetooth;

namespace BluetoothPhone
{
    public class PhoneManager
    {
        public delegate void OnRingHandler(string phoneNumber, PhoneBook book);
        public event OnRingHandler OnRing;

        public delegate void OnUpdatePhoneStatusHandler(PhoneStatus.PhoneStatusKind Status);
        public event OnUpdatePhoneStatusHandler OnUpdatePhoneStatus;

        private Client blueTooth;

        public PhoneManager()
        {
            blueTooth = new Client();
            blueTooth.OnRing += blueTooth_OnRing;
            blueTooth.OnUpdatePhoneStatus += blueTooth_OnUpdatePhoneStatus;
        }


        public void InitAtBluetoothDialog()
        {
            blueTooth.InitAtBluetoothDialog();
        }

        public string[] GetPairingDevices()
        {
            List<string> deviceNameList = new List<string>();

            BluetoothDeviceInfo[] devices = blueTooth.GetPairDevices();
            foreach (BluetoothDeviceInfo device in devices)
            {
                deviceNameList.Add(device.DeviceName);
            }

            return deviceNameList.ToArray();
        }

        public bool ConnectDivece(string deviceName)
        {
            return blueTooth.InitPhone(deviceName);
        }


       


        public void Tel(string phoneNumber)
        {
            blueTooth.Tel(phoneNumber);
        }

        public void Hook()
        {
            blueTooth.Hook();
        }

        public void Terminate()
        {
            blueTooth.Terminate();
        }

        public PhoneBook[] GetBook()
        {
            return blueTooth.getPhoneBooks(Bluetooth.Profile.Pbap.PbapFolder.pb);
        }


        /**
         * Event Handler
         * */
        private void blueTooth_OnRing(string phoneNumber, PhoneBook book)
        {
            if (OnRing != null) OnRing(phoneNumber, book);
        }

        private void blueTooth_OnUpdatePhoneStatus(PhoneStatus.PhoneStatusKind Status)
        {
            if (OnUpdatePhoneStatus != null) OnUpdatePhoneStatus(Status);
        }
    }
}
