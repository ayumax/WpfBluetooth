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
        private Client blueTooth;

        public PhoneManager()
        {
        }

        public void InitPhone()
        {
            blueTooth = new Client();
            BluetoothDeviceInfo[] devices = blueTooth.GetPairDevices();

            foreach(BluetoothDeviceInfo device in devices)
            {
                if (device.DeviceName == "SC-02E")
                {
                    blueTooth.Connect(device);
                }
            }
        }

        public void Tel()
        {
            blueTooth.Tel();
        }

        public void getBook()
        {
            blueTooth.getBook();
        }

        
    }
}
