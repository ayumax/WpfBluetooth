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

namespace BluetoothPhone.Bluetooth.Profile.Hfp
{
    class Hfp : BluetoothProfile
    {
        public Hfp(Client client)
            : base(client, BluetoothService.Handsfree)
        {
        }

        public void Connect()
        {
        }

        public void Dial(BluetoothDeviceInfo device)
        {
            System.IO.Stream peerStream = localClient.GetStream();

            //String dialCmd1 = "AT+CMER\r";
            //String dialCmd2 = "AT+CIND=?\r";
            //String dialCmd3 = "AT+BRSF=\r";
            String dialCmd4 = "ATD09056123133;\r";

            //Byte[] dcB = System.Text.Encoding.ASCII.GetBytes(dialCmd1);
            //peerStream.Write(dcB, 0, dcB.Length);

            //Byte[] sRes = new Byte[200];
            //peerStream.Read(sRes, 0, 199);
            //Console.WriteLine("\n\r----------\n\r" + System.Text.Encoding.ASCII.GetString(sRes));

            //dcB = System.Text.Encoding.ASCII.GetBytes(dialCmd2);
            //peerStream.Write(dcB, 0, dcB.Length);

            //peerStream.Read(sRes, 0, 199);
            //Console.WriteLine("\n\r----------\n\r" + System.Text.Encoding.ASCII.GetString(sRes));

            //dcB = System.Text.Encoding.ASCII.GetBytes(dialCmd3);
            //peerStream.Write(dcB, 0, dcB.Length);

            //peerStream.Read(sRes, 0, 199);
            //Console.WriteLine("\n\r----------\n\r" + System.Text.Encoding.ASCII.GetString(sRes));

            byte[] dcB = System.Text.Encoding.ASCII.GetBytes(dialCmd4);
            peerStream.Write(dcB, 0, dcB.Length);

        }
    }
}
