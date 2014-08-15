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
            //blueTooth.InitPhone("あゆまのiPhone5c");
            blueTooth.InitPhone("SC-02E");
        }


        public void Tel()
        {
            blueTooth.Tel("05058833540");
        }

        public void Hook()
        {
            blueTooth.Hook();
        }

        public void Terminate()
        {
            blueTooth.Terminate();
        }

        public void getBook()
        {
            blueTooth.getPhoneBooks(Bluetooth.Profile.Pbap.PbapFolder.pb);
        }

        
    }
}
